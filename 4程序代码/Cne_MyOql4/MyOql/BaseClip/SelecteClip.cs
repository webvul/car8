using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using System.Data;
using System.Xml.Serialization;
using System.Xml;
using System.Data.Common;

namespace MyOql
{

    /// <summary>
    /// 查询子句.
    /// </summary>
    [Serializable]
    public partial class SelectClip : ContextClipBase, IXmlSerializable, ICloneable, IJoinable, IAsable
    {
        public SelectClip()
        {
            this.Key = SqlKeyword.Select;
        }

        public SelectClip(RuleBase Entity)
        {
            this.Key = SqlKeyword.Select;
            this.CurrentRule = Entity;
        }

        /// <summary>
        /// 返回CacheSql的缓存项.
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public string GetCacheSql(string strSql)
        {
            var rules = this.Rules;
            if (rules.Length == 0) return string.Empty;

            var ents = string.Join(",", rules.Select(o => o.GetDbName()).ToArray());

            return "MyOql|Sql|" + ents + "|" + strSql.ToUpperInvariant();
        }

        /// <summary>
        /// 智能分页标记的总条数。
        /// </summary>
        /// <remarks>
        /// 当页数位于总页数的后半部分时，如最后一页， 把排序 再倒排，取第一页，来提高查询速度。 
        /// </remarks>
        public long SmartRowCount { get; set; }

        /// <summary>
        /// 判断是否具备缓存实体的条件 . 根据 IdKey 判断.
        /// </summary>
        /// <returns></returns>
        protected bool CheckForEntityCache()
        {
            if (this.Next != null) return false;

            var id = this.CurrentRule.GetIdKey();
            if (dbo.EqualsNull(id)) return false;
            var wheres = this.Dna.Where(o => o.Key == SqlKeyword.Where).ToArray();
            if (wheres.Length != 1) return false;

            var where = wheres.FirstOrDefault() as WhereClip;
            if (where == null) return false;

            if (where.Child != null) return false;
            if (where.Next != null) return false;



            if (where.QueryColumnValue(id.Name).HasValue() == false)
            {
                return false;
            }

            var cols = Enumerable.Select(
                this.Dna.Where(o => o.IsColumn()), o => o as ColumnClip).ToArray();

            if (cols.Length == 0) return true;
            if (cols.Length != this.CurrentRule.GetColumns().Length) return false;

            if (cols.Any(o => o.IsConst() || (o.IsSimple() == false))) return false;

            for (int i = 0; i < cols.Length; i++)
            {
                if (this.CurrentRule.GetColumn(cols[i].GetFullDbName()).EqualsNull()) return false;
            }
            return true;
        }


        /// <summary>
        /// 所有的返回单实体，都调用这个方法。
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="NewModelFunc"></param>
        /// <returns></returns>
        public TModel ToEntity<TModel>(Func<TModel> NewModelFunc)
        {
            //默认返回 null
            TModel ret = default(TModel);
            if (this.CurrentRule.GetCacheTime() > 0 || this.GetUseLog() == 0 || this.GetUsePower() == 0)
            {
                if (dbo.Event.OnReading(this) == false) return ret;
            }
            string IdValue = "";
            bool IsSingleEntityCache = this.CurrentRule.GetCacheTime() > 0 && CheckForEntityCache();
            if (IsSingleEntityCache)
            {
                var cacheData = dbo.Event.OnCacheFindById<TModel>(this, () =>
                {
                    var where = this.Dna.FirstOrDefault(p => p.Key == SqlKeyword.Where) as WhereClip;
                    if (where.IsNull()) return "";
                    IdValue = where.GetIdValue(this.CurrentRule);
                    return IdValue;
                }, NewModelFunc);

                if (cacheData.HasValue)
                {
                    //这里应该返回 cacheData.Model.Clone 才对.
                    if (cacheData.Model != null) return cacheData.Model;

                    return ret;
                }
            }

            //还有一种复杂查询, 需要从CacheSql中取.
            var myCmd = this.ToCommand();
            XmlDictionary<string, object> dict = null;
            string cacheSqlKey = null;
            if (this.CurrentRule.GetCacheSqlTime() > 0 && !this.ContainsFunctionRule())
            {
                cacheSqlKey = this.GetCacheSql(myCmd.FullSql);
                var cachedSet = dbo.Event.OnCacheFindBySql(this, cacheSqlKey);
                if (cachedSet.HasValue)
                {
                    if (cachedSet.Set != null && cachedSet.Set.Rows.Count > 0)
                    {
                        return dbo.DictionaryToFuncModel<string, object, TModel>(cachedSet.Set.Rows.ElementAt(0), NewModelFunc);
                    }

                    return ret;
                }
            }



            ExecuteReader(myCmd, o => { dict = dbo.ToDictionary(o); return false; });



            if (IsSingleEntityCache)
            {
                dbo.Event.OnCacheAddById(this, IdValue, dict);
            }
            else if (this.CurrentRule.GetCacheSqlTime() > 0 && !this.ContainsFunctionRule() && cacheSqlKey.HasValue())
            {
                dbo.Event.OnCacheAddBySql(this, cacheSqlKey, () =>
                    new MyOqlSet(this.CurrentRule).Load(dict == null ? null : new List<XmlDictionary<string, object>> { dict })
                    );
            }

            if (dict != null)
            {
                ret = dbo.DictionaryToFuncModel<string, object, TModel>(dict, NewModelFunc);
            }


            dbo.Event.OnReaded(this);
            return ret;
        }



        public XmlDictionary<string, object> ToDictionary()
        {
            return ToEntity(() => new XmlDictionary<string, object>());
        }


        /// <summary>
        /// 所有的查询列表，都调用这个方法。
        /// </summary>
        /// <returns></returns>
        public List<RowData> ToDictList()
        {
            List<RowData> list = new List<RowData>();
            if (dbo.Event.OnReading(this) == false) return list;

            //if (this.CurrentRule.GetBoxyCache() > 0)
            //{
            //    var cacheData = dbo.OnCacheFindSome(this);
            //    if (cacheData != null)
            //    {
            //        return cacheData
            //            .ToDictList()
            //            .Select(o => o.ToXmlDictionary(k => k.Key.Name, v => v.Value))
            //            .ToList();
            //    }
            //}

            if (this.CurrentRule.GetDataBase().IsIn(DatabaseType.SqlServer2000, DatabaseType.Excel, DatabaseType.MsAccess, DatabaseType.Other))
            {
                //Not In 分页方式。
                ExecuteReader((reader, particalResult, postion, index) =>
                  {
                      switch (postion)
                      {
                          case ReaderPositionEnum.BeforeSkip:
                              break;
                          case ReaderPositionEnum.InTake:
                              list.Add(dbo.ToDictionary(reader));
                              break;
                          case ReaderPositionEnum.AfterTake:
                              break;
                          default:
                              break;
                      }

                      return true;
                  });
            }
            else
            {
                //高级数据库的分页方式
                var myCmd = this.ToCommand();

                string cacheSqlKey = null;
                if (myCmd.CurrentAction.CurrentRule.GetCacheSqlTime() > 0 && !this.ContainsFunctionRule())
                {
                    cacheSqlKey = this.GetCacheSql(myCmd.FullSql);
                    var cacheObj = dbo.Event.OnCacheFindBySql(this, cacheSqlKey);

                    if (cacheObj.HasValue)
                    {
                        if (cacheObj.Set != null) return cacheObj.Set.Rows;

                        return list;
                    }
                }

                ExecuteReader(myCmd, o => { list.Add(o.ToDictionary()); return true; });


                //没有结果也会被缓存 .udi 2012年10月17日 
                if (!this.ContainsFunctionRule() && cacheSqlKey.HasValue())
                {
                    dbo.Event.OnCacheAddBySql(myCmd.CurrentAction, cacheSqlKey, () => new MyOqlSet(this.CurrentRule).Load(list));
                }
            }

            //MyCommand myCmd = this.ToCommand();
            //ExecuteReader(myCmd, o => { list.Add(dbo.ToDictionary(o)); return true; });

            dbo.Event.OnReaded(this);

            return list;
        }

        public string Alias { get; set; }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。
        /// </summary>
        /// <returns>结果集中第一行的第一列。</returns>
        public virtual object ToScalar()
        {
            if (dbo.Event.OnReading(this) == false) return null;

            //var cacheData = dbo.OnCacheFindSome(this);
            //if (cacheData.HasValue()) return cacheData.Rows[0][0];

            var myCmd = this.ToCommand();
            //MyOqlSet Model = null;
            string cacheSqlKey = null;
            if (this.CurrentRule.GetCacheSqlTime() > 0 && !this.ContainsFunctionRule())
            {
                cacheSqlKey = this.GetCacheSql(myCmd.FullSql);
                var cachedModel = dbo.Event.OnCacheFindBySql(this, cacheSqlKey);
                if (cachedModel.HasValue)
                {
                    if (cachedModel.Set.HasData())
                    {
                        return cachedModel.Set.Rows.ElementAt(0).ElementAt(0);
                    }

                    return null;
                }
            }

            //if (Model.HasData())
            //{
            //    return Model.ToDictArray().First().ElementAt(0).Value;
            //}

            if (dbo.Event.OnExecute(this) == false) return null;

            var retVal = dbo.Open<object>(myCmd, this, () =>
            {
                try
                {
                    return myCmd.Command.ExecuteScalar();
                }
                catch (Exception ee)
                {
                    ee.ThrowError(myCmd);
                    throw;
                }
            });

            if (this.CurrentRule.GetCacheSqlTime() > 0 && !this.ContainsFunctionRule())
            {
                dbo.Event.OnCacheAddBySql(this, cacheSqlKey,
                    () => new MyOqlSet(this.CurrentRule)
                    {
                        Columns = new ConstColumn[] { new ConstColumn("Scalar") },
                        Rows = retVal == null ? new List<RowData>() : new List<RowData>() { new RowData() { { "Scalar", retVal } } }
                    });

            }

            dbo.Event.OnReaded(this);
            return retVal;
        }


        public virtual DataSet ToDataSet()
        {
            DataSet ds = new DataSet();
            if (dbo.Event.OnReading(this) == false) return null;

            var myCmd = this.ToCommand();

            var cmd = myCmd.Command;

            if (dbo.Event.OnExecute(this) == false) return null;

            return dbo.Open(myCmd, this, () =>
            {
                try
                {
                    var da = GetDataAdapter();
                    da.SelectCommand = cmd;
                    da.Fill(ds);

                    dbo.Event.OnReaded(this);
                    //cmd.Parameters.Clear();
                    return ds;
                }
                catch (Exception ee)
                {
                    ee.ThrowError(myCmd);
                    throw;
                }
            });
        }


        /// <summary>
        /// 有三个值， Union,UnionAll,Enter
        /// </summary>
        public SqlOperator Linker { get; set; }
        public SelectClip Next { get; set; }

        public SelectClip Union(SelectClip OtherSelect)
        {
            this.Linker = SqlOperator.Union;
            this.Next = OtherSelect;
            return this;
        }

        public SelectClip UnionAll(SelectClip OtherSelect)
        {
            this.Linker = SqlOperator.UnionAll;
            this.Next = OtherSelect;
            return this;
        }

        public SelectClip And(SelectClip OtherSelect)
        {
            this.Linker = SqlOperator.Enter;
            this.Next = OtherSelect;
            return this;
        }


        /// <summary>
        /// 遍历 叶子型（不包含其它子查询） 子查询，Join子查询，In 子查询，Union子查询, 当 当前查询没有子查询时，递归包括自己。
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        public bool Recusion(Func<SelectClip, bool> Func)
        {
            var retVal = true;
            if (this.HasSubQuery() == false)
            {
                retVal &= Func(this);
            }
            if (retVal == false) return false;

            if (this.Dna.Any(o => o.Key == SqlKeyword.Select))
            {
                //嵌套子查询，只有一个。
                var subSelect = this.Dna.First(o => o.Key == SqlKeyword.Select) as SelectClip;

                retVal &= subSelect.Recusion(Func);

                if (retVal == false) return false;
            }

            var joins = this.Dna.Where(o => o.Key.IsIn(SqlKeyword.Join, SqlKeyword.LeftJoin, SqlKeyword.RightJoin))
                .Select(o => o as JoinTableClip)
                .ToList();

            //Join 里的 OnWhere In子查询。
            joins.Where(o =>
                {
                    retVal &= o.OnWhere.Recusion(w =>
                    {
                        if (w.Operator.IsIn(SqlOperator.In, SqlOperator.NotIn))
                        {
                            var inSelct = w.Value as SelectClip;
                            if (inSelct != null)
                            {
                                retVal &= inSelct.Recusion(Func);

                                if (retVal == false) return false;
                            }
                        }
                        return true;
                    });
                    if (retVal == false) return false;


                    //Join 的 OnWhere 子句也有子查询
                    retVal &= o.OnWhere.RecusionQuery(Func);
                    if (retVal == false) return false;
                    return true;
                });

            if (retVal == false) return false;

            //Join子查询。
            joins.Where(o => o.SubSelect != null)
                .All(o =>
                {
                    var join = o.SubSelect as SelectClip;
                    retVal &= join.Recusion(Func);

                    if (retVal == false) return false;
                    return true;
                });

            if (retVal == false) return false;

            this.Dna.Where(o => o.Key == SqlKeyword.Where).All(o =>
                {
                    retVal &= (o as WhereClip).Recusion(w =>
                    {
                        if (w.Operator.IsIn(SqlOperator.In, SqlOperator.NotIn))
                        {
                            var inSelct = w.Value as SelectClip;
                            if (inSelct != null)
                            {
                                retVal &= inSelct.Recusion(Func);

                                if (retVal == false) return false;
                            }
                        }
                        return true;
                    });
                    if (retVal == false) return false;

                    return true;
                });

            if (retVal == false) return false;
            //Union 部分。
            if (this.Next != null) this.Next.Recusion(Func);

            return true;
        }


        /// <summary>
        /// 外带数据，ExectedCommand ，ParameterColumn ,Rules 不克隆。
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            SelectClip select = new SelectClip(this.CurrentRule != null ? (this.CurrentRule.Clone() as RuleBase) : null);
            select.AffectRow = this.AffectRow;
            select.Alias = this.Alias;
            select.Connection = this.Connection;
            select.Linker = this.Linker;

            if (this.Next != null)
            {
                select.Next = this.Next.Clone() as SelectClip;
            }
            select.ReConfig = this.ReConfig;
            select.Transaction = this.Transaction;

            if (this.Dna != null && this.Dna.Count > 0)
            {
                select.Dna.AddRange(this.Dna.Select(o => o.Clone() as SqlClipBase));
            }

            return select;
        }


        /// <summary>
        /// 该方法不会触发 添加缓存，不会执行 OnReaded
        /// </summary>
        /// <param name="myCmd"></param>
        /// <returns></returns>
        public override int Execute(MyCommand myCmd)
        {
            var retVal = ExecuteBase(myCmd);

            return retVal;
        }

        /// <summary>
        /// 该方法不会触发 添加缓存，不会执行 OnReaded
        /// </summary>
        /// <param name="myCmd"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public override bool ExecuteReader(MyCommand myCmd, params Func<DbDataReader, bool>[] func)
        {
            var retVal = ExecuteReaderBase(myCmd, func);

            return retVal;
        }


        public void SetAlias(string Alias)
        {
            this.Alias = Alias;
        }

        public string GetAlias()
        {
            return this.Alias;
        }

        /// <summary>
        /// 生成 skip 子句. 结果集会过滤指定的行数
        /// </summary>
        /// <param name="Num">表示过滤指定的行数</param>
        /// <returns></returns>
        public SelectClip Skip(int Num)
        {
            if (Num <= 0) return this;
            var skip = this.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Skip) as SkipClip;
            if (skip == null)
            {
                this.Dna.Add(new SkipClip() { SkipNumber = Num });
            }
            else
            {
                skip.SkipNumber = Num;
            }
            return this;
        }

        /// <summary>
        /// 生成 take 子句.结果集会只取指定数量的记录
        /// 仅在 &gt;= 0 时有意义。
        /// </summary>
        /// <param name="Num">表示要取的指定的记录数</param>
        /// <returns></returns>
        public SelectClip Take(int Num)
        {
            if (Num <= 0 || Num == int.MaxValue) return this;
            var take = this.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Take) as TakeClip;
            if (take == null)
            {
                this.Dna.Add(new TakeClip() { TakeNumber = Num });
            }
            else
            {
                take.TakeNumber = Num;
            }
            return this;
        }
    }
}

