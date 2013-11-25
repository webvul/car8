using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using MyCmn;
using MyOql.Provider;
using System.Data;

namespace MyOql
{

    //[DataContract]
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 该类不记录CRUD事件. 在子类中记录事件
    /// </remarks>
    [Serializable]
    public abstract partial class ContextClipBase : SqlClipBase
    {
        public ContextClipBase()
        {
            _IdValue = 0;
            Dna = new List<SqlClipBase>();
            ParameterColumn = new List<CommandParameter>();
            ClearedAllCacheTable = new List<string>();
            ClearedSqlCacheTable = new List<string>();
            //Rules = new List<RuleBase>();
        }
        //[DataMember]
        /// <summary>
        /// 当前实体约束
        /// </summary>
        /// <remarks>
        /// 从当前实体约束中 检索 出 实体名,列集合等信息.
        /// </remarks>
        public RuleBase CurrentRule
        {
            get;
            set;
        }
        //[DataMember]
        public List<SqlClipBase> Dna
        {
            get;
            set;
        }

        ///// <summary>
        ///// 获取或设置事务.
        ///// </summary>
        ///// <remarks>
        ///// 当使用事务时(非分布式事务), 要将该事务中同一组操作都使用同一连接.
        ///// </remarks>
        public DbTransaction Transaction { get; set; }

        /// <summary>
        /// 显式使用数据连接.
        /// </summary>
        public DbConnection Connection { get; set; }


        /// <summary>
        /// 追加 基于 And 逻辑的 Where 条件
        /// </summary>
        /// <param name="where"></param>
        public void AppendWhere(WhereClip where)
        {
            var dnaWhere = this.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Where) as WhereClip;
            if (dnaWhere != null)
            {
                dnaWhere &= where;
            }
            else this.Dna.Add(where);
        }


        /// <summary>
        /// API
        /// </summary>
        /// <returns></returns>
        public TranslateSql ToProvider()
        {
            string dbpName = this.CurrentRule.GetConfig().db;
            var dbType = dbo.GetDatabaseType(dbpName);

            return dbo.GetDbProvider(dbType);
        }

        public MyCommand ToCommand() { return ToCommand(null); }
        /// <summary>
        /// 数据库解析命令的核心函数.
        /// </summary>
        /// <returns></returns>
        public MyCommand ToCommand(ContextClipBase MyOqlContext)
        {
            bool IsTop = MyOqlContext == null;
            if (IsTop == false)
            {
                //this.ContainsFunction = MyOqlContext.ContainsFunction;
                this.ParameterColumn = MyOqlContext.ParameterColumn;
                //this.Rules = MyOqlContext.Rules;
            }

            //MyOqlContext.Current = this;
            var dbType = dbo.GetDatabaseType(this.CurrentRule.GetConfig().db);

            var provider = dbo.GetDbProvider(dbType).NewTranslation();

            //断点。
            MyCommand myCmd = provider.ToCommand(this, IsTop);

            if (IsTop == false)
            {
                //MyOqlContext.ContainsFunction = this.ContainsFunction;
                MyOqlContext.ParameterColumn.AddRange(this.ParameterColumn);
                //MyOqlContext.Rules.AddRange(this.Rules);

                MyOqlContext.RunCommand = myCmd;
            }
            else
            {
                this.RunCommand = myCmd;
            }

            dbo.Event.OnGenerateSqled(dbType, myCmd);
            return myCmd;
        }

        /// <summary>
        /// 得到数据库的 Adapter
        /// </summary>
        /// <returns></returns>
        public virtual DbDataAdapter GetDataAdapter()
        {
            string dbpName = this.CurrentRule.GetConfig().db;
            var conf = dbo.GetDatabaseType(dbpName);
            return dbo.GetDbProvider(conf).GetDataAdapter();
        }

        /// <summary>
        /// 执行并返回记录
        /// </summary>
        /// <remarks>
        /// <code>
        ///        int curPos = 0;
        ///         List&lt;XmlDictionary&lt;string, object&gt;&gt; dictList = new List&lt;XmlDictionary&lt;string, object&gt;&gt;();
        ///        
        ///         dbr.Table.Select()
        ///        .ExecuteReader(o =&gt;
        ///        {
        ///            //二次过滤。
        ///            if (o.AsDateTime(o.GetOrdinal("AcceptTime")).AddDays(o.GetValue(o.GetOrdinal("OverdueDaynum")).AsInt()) &lt; DateTime.Today)
        ///            {
        ///                return true;
        ///            }
        ///
        ///
        ///            curPos++;
        ///            if (curPos &lt;= Skip)
        ///            {
        ///                return true;
        ///            }
        ///            if (curPos &gt; Skip + Take)
        ///            {
        ///                return true;
        ///            }
        ///
        ///            dictList.Add(o.ToDictionary());
        ///            return true;
        ///        });
        ///
        ///    return dictList.ToMyOqlSet(dbr.TTask, curPos);
        /// </code>
        /// </remarks>
        /// <param name="func"></param>
        /// <returns>是否成功执行了回调函数。</returns>
        public virtual bool ExecuteReader(Func<DbDataReader, bool> func)
        {
            return ExecuteReader(this.ToCommand(), func);
        }

        /// <summary>
        /// 执行并返回记录
        /// </summary>
        /// <param name="myCmd"></param>
        /// <param name="func"></param>
        /// <returns>是否成功执行了回调函数</returns>
        public abstract bool ExecuteReader(MyCommand myCmd, params Func<DbDataReader, bool>[] func);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myCmd"></param>
        /// <param name="func"></param>
        /// <returns>是否返回了数据</returns>
        protected bool ExecuteReaderBase(MyCommand myCmd, params Func<DbDataReader, bool>[] func)
        {
            var cmd = myCmd.Command;
            if (dbo.Event.OnExecute(this) == false) return false;


            return dbo.Open(myCmd, this, () =>
            {
                try
                {
                    AffectRow = 0;

                    if (myCmd.ExecuteType == ExecuteTypeEnum.SelectWithSkip)
                    {

                        using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            var readerIndex = 0;
                            bool retVal = true;
                            do
                            {
                                if (func.Length <= readerIndex) break;

                                var f = func[readerIndex];
                                retVal = SelectWithSkipExecuteReader(f, reader);
                                if (retVal == false) return retVal;

                                readerIndex++;
                            }
                            while (reader.NextResult());

                            return retVal;
                        }
                    }
                    else
                    {
                        using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            var readerIndex = 0;
                            bool retVal = true;
                            do
                            {
                                while (reader.Read())
                                {
                                    if (func.Length <= readerIndex) break;
                                    var f = func[readerIndex];

                                    AffectRow++;
                                    var ret = f(reader);
                                    if (ret == false)
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                readerIndex++;
                            }
                            while (reader.NextResult());

                            return retVal;
                        }
                    }

                }
                catch (Exception ee)
                {
                    ee.ThrowError(myCmd);
                }

                return AffectRow > 0;
            });
        }

        private bool SelectWithSkipExecuteReader(Func<DbDataReader, bool> func, DbDataReader reader)
        {
            var skipNum = 0;
            var skip = this.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Skip) as SkipClip;
            if (skip != null)
            {
                skipNum = skip.SkipNumber;
            }

            for (int i = 0; i < skipNum; i++)
            {
                if (reader.Read() == false) return false;
            }

            var takeCount = int.MaxValue;
            var take = this.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Take) as TakeClip;
            if (take != null)
            {
                takeCount = take.TakeNumber;
            }

            for (var i = 0; i < takeCount; i++)
            {
                if (reader.Read() == false) return AffectRow > 0;
                AffectRow++;
                var ret = func(reader);
                if (ret == false)
                {
                    return false;
                }
                else
                {
                    continue;
                }
            }

            return true;
        }

        /// <summary>
        /// 记录影响行数
        /// </summary>
        public int AffectRow { get; set; }

        /// <summary>
        /// 返回影响的行数
        /// </summary>
        /// <param name="myCmd"></param>
        /// <returns></returns>
        public abstract int Execute(MyCommand myCmd);

        protected int ExecuteBase(MyCommand myCmd)
        {
            dbo.Check(myCmd == null, "MyCommand 不能为空。", this.CurrentRule);
            dbo.Check(myCmd.Command == null, "DbCommand 不能为空。", this.CurrentRule);

            var cmd = myCmd.Command;

            if (dbo.Event.OnExecute(this) == false) return 0;

            AffectRow = dbo.Open(myCmd, this, () =>
            {
                try
                {
                    int retVal = cmd.ExecuteNonQuery();
                    //cmd.Parameters.Clear();
                    return retVal;
                }
                catch (Exception ee)
                {
                    ee.ThrowError(myCmd);
                    throw;
                }
            });

            return AffectRow;
        }




        /// <summary>
        /// 是否包含聚合函数。
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        public bool TableHasPolymer()
        {
            var group = this.Dna.FirstOrDefault(o => o.Key == SqlKeyword.GroupBy) as GroupByClip;
            if (group != null && group.Groups != null && group.Groups.Count > 0)
            {
                return true;
            }

            return this.Dna.Any(o => o.IsColumn() && (o as ColumnClip).IsPolymer());
        }

        /// <summary>
        /// 上下文Dna中是否包含自增键，唯一键，单一主键或主键。 
        /// </summary>
        /// <returns></returns>
        public bool ContainsIdupKey()
        {
            var names =
                this.Dna
                    .Where(o => o.Key == SqlKeyword.Simple)
                    .Select(o => (o as SimpleColumn).GetFullName().AsString())
                    .ToArray();

            if (this.CurrentRule.GetIdKey().EqualsNull() == false && names.Contains(this.CurrentRule.GetIdKey().GetFullName().AsString()))
            {

                return true;
            }
            else
            {
                return names.Intersect(this.CurrentRule.GetPrimaryKeys().Select(o => o.GetFullName().AsString()))
                   .Count()
               == this.CurrentRule.GetPrimaryKeys().Length;
            }
        }

        /// <summary>
        /// 参数所对应的列. 在 GetParameters 里收集. 在每次 ToCommand 时进行向下传递.
        /// </summary>
        public List<CommandParameter> ParameterColumn { get; set; }

        /// <summary>
        /// 语句包含的 表（已去重返回）
        /// </summary>
        public RuleBase[] Rules
        {
            get
            {
                List<RuleBase> list = new List<RuleBase>();

                list.Add(this.CurrentRule);

                if (this.Dna.Any(o => o.Key == SqlKeyword.Select))
                {
                    //嵌套子查询，只有一个。
                    var subSelect = this.Dna.First(o => o.Key == SqlKeyword.Select) as SelectClip;

                    list.AddRange(subSelect.Rules);
                }

                var joins = this.Dna.Where(o => o.Key.IsIn(SqlKeyword.Join, SqlKeyword.LeftJoin, SqlKeyword.RightJoin))
                    .Select(o => o as JoinTableClip)
                    .ToList();

                //Join 里的 OnWhere In子查询。
                joins.Where(o =>
                {
                    o.OnWhere.Recusion(w =>
                    {
                        if (w.Operator.IsIn(SqlOperator.In, SqlOperator.NotIn))
                        {
                            var inSelct = w.Value as SelectClip;
                            if (inSelct != null)
                            {
                                list.AddRange(inSelct.Rules);
                            }
                        }
                        return true;
                    });



                    //Join 的 OnWhere 子句也有子查询
                    o.OnWhere.RecusionQuery(s => { list.AddRange(s.Rules); return true; });
                    return true;
                });



                //Join子查询。
                joins
                    .All(o =>
                    {
                        if (o.SubSelect != null) { list.AddRange(o.SubSelect.Rules); }
                        else
                        {
                            list.Add(o.Table);
                        }
                        return true;
                    });


                this.Dna.Where(o => o.Key == SqlKeyword.Where).All(o =>
                {
                    (o as WhereClip).Recusion(w =>
                   {
                       if (w.Operator.IsIn(SqlOperator.In, SqlOperator.NotIn))
                       {
                           var inSelct = w.Value as SelectClip;
                           if (inSelct != null)
                           {
                               list.AddRange(inSelct.Rules);
                           }
                       }
                       return true;
                   });

                    return true;
                });

                if (this.Key == SqlKeyword.Select)
                {
                    var sel = this as SelectClip;
                    if (sel != null && sel.Next != null)
                    {
                        list.AddRange(sel.Next.Rules);
                    }
                }

                return list.Distinct((a, b) => a.GetDbName() == b.GetDbName()).ToArray();
            }
        }

        /// <summary>
        /// 清除缓存的
        /// </summary>
        public List<string> ClearedSqlCacheTable { get; set; }

        public List<string> ClearedAllCacheTable { get; set; }

        /// <summary>
        /// 是否存在可变数据实体。
        /// </summary>
        /// <remarks>视图，函数，数据是不稳定的，当其它表数据改变后，也会影响视图，函数的数据，而不能及时清理其缓存。</remarks>
        /// <returns></returns>
        public bool ContainsFunctionRule()
        {
            if ((this.CurrentRule as IFunctionRule) != null) return true;
            else
            {
                return this.Rules.Any(o => (o as IFunctionRule) != null);
            }
        }

        ///// <summary>
        ///// 在 ToCommand 中收集.
        ///// </summary>
        //public bool ContainsFunction { get; set; }

        /// <summary>
        /// 用户额外带的数据。
        /// </summary>
        public object UserData { get; set; }

        public MyCommand RunCommand { get; set; }


        protected ReConfigEnum ReConfig { get; set; }


        /// <summary>
        /// 是否启用权限控制.兼容处理了SkipPower逻辑。 对独立的实体，可以用 new MyContextClip(SqlKeyword.Select){ CurrentRule = Ent }  构造。
        /// </summary>
        /// <returns></returns>
        public MyOqlActionEnum GetUsePower()
        {
            if (this.ContainsConfig(ReConfigEnum.SkipPower)) return (MyOqlActionEnum)0;
            return this.CurrentRule.GetConfig().UsePower.ToEnum<MyOqlActionEnum>();
        }


        /// <summary>
        /// 是否启用日志控制.兼容处理了SkipLog逻辑。对独立的实体，可以用 new MyContextClip(SqlKeyword.Select){ CurrentRule = Ent }  构造。
        /// </summary>
        /// <returns></returns>
        public MyOqlActionEnum GetUseLog()
        {
            if (this.ContainsConfig(ReConfigEnum.SkipLog)) return (MyOqlActionEnum)0;
            return this.CurrentRule.GetConfig().Log.ToEnum<MyOqlActionEnum>();
        }
        /// <summary>
        /// 是否带有  子查询 ，Join 子询。(不包括 Where In子查询) , 不判断 Union 的查询。
        /// </summary>
        /// <returns></returns>
        public bool HasSubQuery()
        {
            if (this.Dna.Any(o => o.Key == SqlKeyword.Select)) return true;
            if (this.Dna.Any(o => o.Key.IsIn(SqlKeyword.Join, SqlKeyword.LeftJoin, SqlKeyword.RightJoin) && (o as JoinTableClip).SubSelect != null)) return true;


            return false;
        }


        public bool ContainsConfig(ReConfigEnum config)
        {
            if (MyOqlConfigScope.Config != null && MyOqlConfigScope.Config.IsValueCreated &&
                MyOqlConfigScope.Config.Value.Contains(config)) return true;

            return ReConfig.Contains(config);
        }

        public void AddConfig(ReConfigEnum config)
        {
            this.ReConfig |= config;
        }



        public override object Clone()
        {
            var delete = MyCmn.ValueProc.NewWithType(this.GetType()) as ContextClipBase;
            delete.AffectRow = this.AffectRow;
            delete.Connection = this.Connection;
            delete.CurrentRule = this.CurrentRule.Clone() as RuleBase;

            if (this.Dna != null && this.Dna.Count > 0)
            {
                delete.Dna.AddRange(this.Dna.Select(o => o.Clone() as SqlClipBase));
            }
            delete.ReConfig = this.ReConfig;
            return delete;
        }


        private int _IdValue;
        public int GetIdValue()
        {
            return ++_IdValue;
        }
    }
}