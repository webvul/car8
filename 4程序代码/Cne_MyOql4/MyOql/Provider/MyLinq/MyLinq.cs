//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data.SqlClient;
//using System.Data.Common;
//using MyCmn;
//using System.Collections;
//using System.Configuration;
//using System.Reflection;
//using System.Data;

//namespace MyOql.Provider
//{

//    /// <summary>
//    /// AsString 会有问题， 如果数据是 Null,而查询条件是 “”，则会有问题。 待确认。需要把 AsString 变为 真正的 as string。
//    /// </summary>
//    public class MyLinq
//    {
//        /// <summary>
//        /// 返回索引类。
//        /// </summary>
//        internal class IndexSet
//        {
//            public List<string> Table { get; set; }

//            /// <summary>
//            /// 命中的行索引，是一个矩阵， 要保证每行的长度都相同。 
//            /// 第一维表示行，第二维表示列。即： 
//            /// [1,1]
//            /// [2,3]
//            /// [3,5]
//            /// [7,9]
//            /// </summary>
//            public List<int[]> HitedIndex { get; set; }

//            internal int GetValue(int RowIndex, int ColumnIndex)
//            {
//                return HitedIndex[RowIndex][ColumnIndex];
//            }

//            public int[] NewRow()
//            {
//                var row = new int[this.Table.Count];
//                for (int i = 0; i < row.Length; i++)
//                {
//                    row[i] = -1;
//                }
//                return row;
//            }

//            public IndexSet(Dictionary<string, JoinTableData> set)
//            {
//                this.Table = set.Select(o => o.Key).ToList();
//                this.HitedIndex = new List<int[]>();

//                for (int i = 0; i < set.First().Value.Data.Rows.Count; i++)
//                {
//                    var row = NewRow();
//                    row[0] = i;
//                    this.HitedIndex.Add(row);
//                }
//            }

//            /// <summary>
//            /// 找某个表在容器中标记的行索引。
//            /// </summary>
//            /// <param name="TableName">表名。</param>
//            /// <param name="RowIndex">行索引。</param>
//            /// <returns></returns>
//            public int GetTableRowIndex(string TableName, int RowIndex)
//            {
//                int colIndex = this.Table.IndexOf(o => o == TableName);
//                return HitedIndex[RowIndex][colIndex];
//            }

//            internal void RemoveIndex(int hIndex)
//            {
//                HitedIndex.RemoveAt(hIndex);
//            }

//            internal void UpdateData(int hIndex, int curTableIndex, List<int> list)
//            {
//                this.HitedIndex[hIndex][curTableIndex] = list[0];
//                for (int i = 1; i < list.Count - 1; i++)
//                {
//                    this.HitedIndex.Insert(hIndex + 1, this.HitedIndex[hIndex]);
//                }

//            }
//        }

//        /// <summary>
//        /// 是否合法。
//        /// </summary>
//        public bool IsValidate { get; set; }

//        /// <summary>
//        /// 上下文。
//        /// </summary>
//        private ContextClipBase Context { get; set; }

//        /// <summary>
//        /// 数据源。
//        /// </summary>
//        private Dictionary<string, JoinTableData> Source { get; set; }

//        /// <summary>
//        /// 结果容器
//        /// </summary>
//        private IndexSet Container { get; set; }

//        /// <summary>
//        /// 都是And关系，且每一个WhereClip都是标准的 Column Op Value 格式。
//        /// </summary>
//        private WhereClip ValueWhere { get; set; }


//        private WhereClip RelationWhere { get; set; }

//        /// <summary>
//        /// 在查询时，是否重新启用 ValueWhere。
//        /// </summary>
//        private bool ReValueWhere { get; set; }

//        /// <summary>
//        /// 是否从缓存加载成功。
//        /// </summary>
//        private bool IsSucsess { get; set; }


//        public MyLinq(ContextClipBase Context)
//        {
//            this.Context = Context;
//        }

//        public MyOqlSet Find()
//        {
//            IsValidate = Check();
//            if (IsValidate == false) return null;

//            LoadSource();

//            SplitWhereClip();

//            CutUseValueWhere();

//            JoinOnTableData();

//            CutWhere();

//            return ToMyOqlSet();
//        }

//        private MyOqlSet ToMyOqlSet()
//        {
//            MyOqlSet set = new MyOqlSet(this.Context.CurrentRule);
//            set.Columns = Enumerable.Select(
//                this.Context.Dna.Where(o => o.Key.IsIn(SqlKeyword.Column)), o => o as ColumnClip
//                ).ToArray();

//            XmlDictionary<string, int> dict = new XmlDictionary<string, int>();
//            for (int i = 0; i < Container.Table.Count; i++)
//            {
//                dict[Container.Table[i]] = i;
//            }

//            Func<int[], object[]> Translate = indexs =>
//                {
//                    List<object> list = new List<object>();

//                    set.Columns.All(c =>
//                        {
//                            var rowIndex = indexs[dict[c.Table.GetAliasOrName()]];
//                            var curData = Source[c.Table.GetAliasOrName()].Data;
//                            var colIndex = curData.Columns.IndexOf(cc => cc.NameEquals(c.Name));

//                            if (colIndex < 0)
//                            {
//                                //可能有常数列，复合列。
//                            }
//                            else
//                            {
//                                list.Add(curData.Rows[rowIndex][colIndex]);
//                            }
//                            return true;
//                        });

//                    return list.ToArray();
//                };


//            Container.HitedIndex.All(o =>
//                {
//                    set.Rows.Add(Translate(o));
//                    return true;
//                });

//            return set;
//        }

//        private void CutWhere()
//        {

//        }

//        private void JoinOnTableData()
//        {
//            this.Container = new IndexSet(Source);

//            Context.Dna.Where(o => o.Key.IsIn(SqlKeyword.Join, SqlKeyword.LeftJoin, SqlKeyword.RightJoin))
//                .Select(o => o as JoinTableClip).All(joinTable =>
//                    {
//                        ////额外插入的数据，Key是Container 的索行。 Value中另一个行索引。
//                        //Dictionary<int, int> InsertExtendData = new Dictionary<int, int>();

//                        var curTableIndex = this.Container.Table.IndexOf(joinTable.Table.GetAliasOrName());

//                        if (joinTable.Key == SqlKeyword.Join)
//                        {
//                            //hIndex 是当前容器中命中的索引。
//                            for (int hIndex = 0; hIndex < this.Container.HitedIndex.Count; hIndex++)
//                            {
//                                List<int> list = ProcRelation(hIndex, joinTable, o => true);

//                                if (list.Count > 0)
//                                {
//                                    this.Container.UpdateData(hIndex, curTableIndex, list);
//                                }
//                                else
//                                {
//                                    Container.RemoveIndex(hIndex);
//                                    hIndex--;
//                                }
//                            }
//                            //对Join 进行修剪
//                            this.Container.HitedIndex.RemoveAll(o => o[curTableIndex] == -1);
//                        }


//                        return true;
//                    });
//        }


//        /// <summary>
//        /// 处理关系。
//        /// </summary>
//        /// <param name="ContainerIndex">容器中的行索引。</param>
//        /// <param name="joinTable">连接表。</param>
//        /// <param name="Func">如果命中了连接表中的该条数据的回调，返回false停止。</param>
//        /// <returns></returns>
//        private List<int> ProcRelation(int ContainerIndex, JoinTableClip joinTable, Func<int, bool> Func)
//        {
//            List<int> list = new List<int>();
//            var curData = Source[joinTable.Table.GetAliasOrName()].Data;
//            var curTableIndexInContainer = Container.Table.IndexOf(joinTable.Table.GetAliasOrName());
//            var wf = GetWhereFunc(joinTable.OnWhere);
//            int curIndex = -1;

//            #region 遍历每一行。

//            curData.Rows.All(row =>
//                 {
//                     curIndex++;
//                     Container.HitedIndex[ContainerIndex][curTableIndexInContainer] = curIndex;

//                     if (wf(where =>
//                         {
//                             var JoinTable = curData.Entity.GetAliasOrName();

//                             object k = null;
//                             //如果当前查询列是当前关联表的话。
//                             if (where.Query.Table.GetAliasOrName() == JoinTable)
//                             {
//                                 //找当前查询列索引，可缓存，提高性能。
//                                 k = row[curData.Columns.IndexOf(cc => cc.NameEquals(where.Query.Name))];
//                             }
//                             else
//                             {
//                                 //如果不在当前关联表，从Container 中查。 
//                                 var tableRowIndex = Container.GetTableRowIndex(where.Query.Table.GetAliasOrName(), ContainerIndex);

//                                 //查找在容器中的列索引。
//                                 var queryColumnIndex = Source[where.Query.Table.GetAliasOrName()].Data.Columns.IndexOf(c => c.NameEquals(where.Query.Name));
//                                 k = Source[where.Query.Table.GetAliasOrName()].Data.Rows[tableRowIndex][queryColumnIndex];
//                             }


//                             object v = null;
//                             //如果当前查询列是当前关联表的话。
//                             ColumnClip v_column = where.Value as ColumnClip;
//                             if (v_column.EqualsNull() == false)
//                             {
//                                 if (v_column.Table.GetAliasOrName() == JoinTable)
//                                 {
//                                     //找当前查询列索引，可缓存，提高性能。
//                                     v = row[curData.Columns.IndexOf(cc => cc.NameEquals(v_column.Name))];
//                                 }
//                                 else
//                                 {
//                                     //如果不在当前关联表，从Container 中查。 
//                                     var tableRowIndex = Container.GetTableRowIndex(v_column.Table.GetAliasOrName(), ContainerIndex);

//                                     //查找在容器中的列索引。
//                                     var queryColumnIndex = Source[v_column.Table.GetAliasOrName()].Data.Columns.IndexOf(c => c.NameEquals(where.Query.Name));
//                                     v = Source[v_column.Table.GetAliasOrName()].Data.Rows[tableRowIndex][queryColumnIndex];
//                                 }
//                             }
//                             else
//                             {
//                             }

//                             return ProcEachWhere(where, k, v);
//                         }))
//                     {
//                         list.Add(curIndex);
//                     }


//                     return true;
//                 })

//                 ;

//            if (list.Count == 0)
//            {
//                Container.HitedIndex[ContainerIndex][curTableIndexInContainer] = -1;
//            }
//            #endregion

//            return list;
//        }


//        /// <summary>
//        /// 取某个列，在某个容器行的运行时值。如果有复合列，或函数，则计算之。
//        /// </summary>
//        /// <param name="TableAlias">列所属表的别名， 处理自连接问题</param>
//        /// <param name="Column">列。</param>
//        /// <param name="ContainerIndex">容器数据行索引。</param>
//        /// <returns></returns>
//        private object GetQueryValue(string TableAlias, ColumnClip Column, int ContainerIndex)
//        {
//            if (Column.EqualsNull()) return null;

//            //单纯的，查找出该列（肯定是简单列）在容器行的运行时值。
//            Func<string, ColumnClip, object> GetOnlyColumnValue = (tableAlias, col) =>
//                {
//                    return null;
//                };



//            Func<ColumnClip, object, object> GetValue = (col, val) =>
//                {
//                    col.Extend.All(o =>
//                        {
//                            //如果是聚合函数，不处理，


//                            return true;
//                        });
//                    return null;
//                };

//            return null;



//            //Func<object, object> GetFunctionExpression = (strSql) =>
//            //{
//            //    Column.Extend.All(o =>
//            //    {
//            //        if (o.Key.IsIn(SqlOperator.Sum,
//            //                SqlOperator.CountDistinct,
//            //                SqlOperator.Count,
//            //                SqlOperator.Max,
//            //                SqlOperator.Min,
//            //                SqlOperator.Avg,
//            //                SqlOperator.IsNull,
//            //                SqlOperator.Len
//            //            ))
//            //        {
//            //            strSql = string.Format(GetOperator(o.Key), strSql, GetValueExp(o.Value));
//            //            return true;
//            //        }
//            //        else if (o.Key.IsIn(
//            //                SqlOperator.StringIndex,
//            //                SqlOperator.SubString))
//            //        {
//            //            //三参.
//            //            //hasFunction = true;
//            //            var extVal = (KeyValuePair<object, object>)o.Value;

//            //            strSql = string.Format(GetOperator(o.Key), strSql, GetValueExp(extVal.Key), GetValueExp(extVal.Value));
//            //            return true;
//            //        }
//            //        else if (o.Key.IsIn(SqlOperator.Const, SqlOperator.Complex, SqlOperator.FromTable, SqlOperator.Customer))
//            //        {
//            //            return true;
//            //        }

//            //        else if (o.Key == SqlOperator.Cast)
//            //        {
//            //            //两参.
//            //            DbType dbType = (DbType)o.Value;

//            //            strSql = string.Format(GetOperator(o.Key), strSql, GetTypeMap().First(t => t.DbType == dbType).SqlType.ToString() +
//            //                (dbType.DbTypeIsString() ? "(4000)" : ""));
//            //            return true;
//            //        }
//            //        else if (o.Key == SqlOperator.As)
//            //        {
//            //            return true;
//            //        }
//            //        else
//            //        {
//            //            strSql = string.Format(GetOperator(o.Key), ToParaValues(strSql, o.Value.AsString()));
//            //        }

//            //        return true;
//            //    });

//            //    return strSql;
//            //};


//            //Func<object> GetSimpleColumnExpression = () =>
//            //{
//            //    if (Column.Extend.ContainsKey(SqlOperator.Const))
//            //    {
//            //        if (Column.DbType.DbTypeIsNumber()) return Column.DbName.AsDecimal();
//            //        else return   Column.DbName ;
//            //    }
//            //    else if (Column.Extend.ContainsKey(SqlOperator.FromTable))
//            //    {
//            //        return GetMyName(Column.Extend[SqlOperator.FromTable].AsString()) + "." + GetMyName(Column.DbName);
//            //    }

//            //    string retVal = string.Empty;

//            //    if (Column.Table != null)
//            //    {
//            //        var tableAlias = Column.Table.GetAlias();

//            //        if (tableAlias.HasValue()) retVal += GetMyName(tableAlias) + ".";
//            //        else
//            //        {
//            //            retVal += GetTableFullName(Column.Table) + ".";
//            //        }
//            //    }
//            //    return retVal + GetMyName(Column.DbName);

//            //};

//            ////优先考虑复合列.
//            //if (Column.Extend.ContainsKey(SqlOperator.Complex))
//            //{
//            //    Func<ComplexExpresstion, object> _GetComplexExpression = null;
//            //    Func<ComplexExpresstion, object> GetComplexExpression = Ce =>
//            //    {
//            //        if (Ce.Modal.IsDBNull() == false)
//            //        {
//            //            if (Ce.Modal.IsConst())
//            //            {
//            //                if (Ce.Modal.DbType.DbTypeIsNumber())
//            //                {
//            //                    return Ce.Modal.Name.AsDecimal();
//            //                }
//            //                else return Ce.Modal.Name;
//            //            }
//            //            else
//            //                return GetQueryValue("",Ce.Modal, ContainerIndex);

//            //        }
//            //        //还要生成表达式树。
//            //        else return _GetComplexExpression(Ce);// string.Format(GetOperator(Ce.Operator), ToParaValues(_GetComplexExpression(Ce.Query), _GetComplexExpression(Ce.Value)));
//            //    };
//            //    _GetComplexExpression = GetComplexExpression;


//            //    return GetFunctionExpression(GetComplexExpression(Column.Extend[SqlOperator.Complex] as ComplexExpresstion));
//            //}
//            //else return GetFunctionExpression(GetSimpleColumnExpression());
//        }

//        /// <summary>
//        /// 处理OnWhere 或 where条件。
//        /// </summary>
//        /// <remarks>
//        /// </remarks>
//        /// <param name="where"></param>
//        /// <param name="k"></param>
//        /// <param name="v"></param>
//        /// <returns>如果条件匹配，返回true，否则返回false。</returns>
//        private bool ProcEachWhere(WhereClip where, object k, object v)
//        {
//            //判断 kv

//            if (where.Operator == SqlOperator.Equal)
//            {
//                if (k == null || (v == null)) return false;
//                if (where.Query.DbType.DbTypeIsString())
//                {
//                    return k.AsString() == v.AsString();
//                }
//                else if (where.Query.DbType.DbTypeIsNumber())
//                {
//                    return k.AsDecimal() == v.AsDecimal();
//                }
//                else if (where.Query.DbType.DbTypeIsDateTime())
//                {
//                    return k.AsDateTime() == v.AsDateTime();
//                }
//                else
//                    return k == v;
//            }
//            else if (where.Operator == SqlOperator.NotEqual)
//            {
//            }
//            return false;
//        }


//        /// <summary>
//        /// 返回一个 调用委托 。 形如：  bool Func(Func<WhereClip,bool> whereProc)
//        /// 最终形成： (whereProc)=>
//        /// </summary>
//        /// <param name="AllWhere"></param>
//        /// <returns></returns>
//        private Func<Func<WhereClip, bool>, bool> GetWhereFunc(WhereClip AllWhere)
//        {
//            List<WhereClip> list = new List<WhereClip>();
//            var exp = AllWhere.RecusionExpressionFunc(w => list.Add(w)).Compile();

//            Func<Func<WhereClip, bool>, bool> func = eachWhereProc =>
//                {
//                    return exp(eachWhereProc, list.ToArray());
//                };
//            return func;
//        }


//        /// <summary>
//        /// 对容器先进行一些裁剪。
//        /// </summary>
//        private void CutUseValueWhere()
//        {
//            if (ValueWhere == null) return;

//            Action<WhereClip> Cut = where =>
//                {
//                    var set = Source[where.Query.Table.GetAliasOrName()].Data;
//                    var queryColumnIndex = set.Columns.IndexOf(o => o.NameEquals(where.Query));
//                    var queryDbType = set.Columns[queryColumnIndex].DbType;

//                    var val = where.Value;

//                    set.Rows = set.Rows
//                        .Where(o =>
//                        {
//                            var query = o[queryColumnIndex];

//                            return ProcEachWhere(where, query, val);

//                            //switch (where.Operator)
//                            //{
//                            //    case SqlOperator.Between:

//                            //        var kv = val as object[];
//                            //        if (queryDbType.DbTypeIsDateTime())
//                            //        {
//                            //            var q = query.AsDateTime();
//                            //            return (q >= kv[0].AsDateTime()) && (q <= kv[1].AsDateTime());
//                            //        }
//                            //        else if (queryDbType.DbTypeIsNumber())
//                            //        {
//                            //            var q = query.AsDecimal();

//                            //            return (q >= kv[0].AsDecimal()) && (q <= kv[1].AsDecimal());
//                            //        }
//                            //        else
//                            //        {
//                            //            var q = query.AsString();
//                            //            return (q.CompareTo(kv[0].AsString()) >= 0) && (q.CompareTo(kv[1].AsString()) <= 0);
//                            //        }
//                            //    case SqlOperator.Equal:
//                            //        if (queryDbType.DbTypeIsDateTime())
//                            //        {
//                            //            return query.AsDateTime() == val.AsDateTime();
//                            //        }
//                            //        else if (queryDbType.DbTypeIsNumber())
//                            //        {
//                            //            return query.AsDecimal() == val.AsDecimal();
//                            //        }
//                            //        else
//                            //        {
//                            //            return query.AsString() == val.AsString();
//                            //        }
//                            //    case SqlOperator.BigThan:
//                            //        if (queryDbType.DbTypeIsDateTime())
//                            //        {
//                            //            return query.AsDateTime() > val.AsDateTime();
//                            //        }
//                            //        else if (queryDbType.DbTypeIsNumber())
//                            //        {
//                            //            return query.AsDecimal() > val.AsDecimal();
//                            //        }
//                            //        else
//                            //        {
//                            //            return query.AsString().CompareTo(val.AsString()) == 1;
//                            //        }

//                            //    case SqlOperator.LessThan:
//                            //        if (queryDbType.DbTypeIsDateTime())
//                            //        {
//                            //            return query.AsDateTime() < val.AsDateTime();
//                            //        }
//                            //        else if (queryDbType.DbTypeIsNumber())
//                            //        {
//                            //            return query.AsDecimal() < val.AsDecimal();
//                            //        }
//                            //        else
//                            //        {
//                            //            return query.AsString().CompareTo(val.AsString()) == -1;
//                            //        }
//                            //    case SqlOperator.BigEqual:
//                            //        if (queryDbType.DbTypeIsDateTime())
//                            //        {
//                            //            return query.AsDateTime() >= val.AsDateTime();
//                            //        }
//                            //        else if (queryDbType.DbTypeIsNumber())
//                            //        {
//                            //            return query.AsDecimal() >= val.AsDecimal();
//                            //        }
//                            //        else
//                            //        {
//                            //            return query.AsString().CompareTo(val.AsString()) >= 0;
//                            //        }
//                            //    case SqlOperator.LessEqual:
//                            //        if (queryDbType.DbTypeIsDateTime())
//                            //        {
//                            //            return query.AsDateTime() <= val.AsDateTime();
//                            //        }
//                            //        else if (queryDbType.DbTypeIsNumber())
//                            //        {
//                            //            return query.AsDecimal() <= val.AsDecimal();
//                            //        }
//                            //        else
//                            //        {
//                            //            return query.AsString().CompareTo(val.AsString()) <= 0;
//                            //        }
//                            //    case SqlOperator.NotEqual:
//                            //        if (queryDbType.DbTypeIsDateTime())
//                            //        {
//                            //            return query.AsDateTime() != val.AsDateTime();
//                            //        }
//                            //        else if (queryDbType.DbTypeIsNumber())
//                            //        {
//                            //            return query.AsDecimal() != val.AsDecimal();
//                            //        }
//                            //        else
//                            //        {
//                            //            return query.AsString() != val.AsString();
//                            //        }
//                            //    case SqlOperator.In:

//                            //        if (queryDbType.DbTypeIsDateTime())
//                            //        {
//                            //            return (val as object[]).Select(t => t.AsDateTime()).Contains(query.AsDateTime());
//                            //        }
//                            //        else if (queryDbType.DbTypeIsNumber())
//                            //        {
//                            //            return (val as object[]).Select(t => t.AsDecimal()).Contains(query.AsDecimal());
//                            //        }
//                            //        else
//                            //        {
//                            //            return (val as object[]).Select(t => t.AsString()).Contains(query.AsString());
//                            //        }
//                            //    case SqlOperator.NotIn:
//                            //        if (queryDbType.DbTypeIsDateTime())
//                            //        {
//                            //            return (val as object[]).Select(t => t.AsDateTime()).Contains(query.AsDateTime()) == false;
//                            //        }
//                            //        else if (queryDbType.DbTypeIsNumber())
//                            //        {
//                            //            return (val as object[]).Select(t => t.AsDecimal()).Contains(query.AsDecimal());
//                            //        }
//                            //        else
//                            //        {
//                            //            return (val as object[]).Select(t => t.AsString()).Contains(query.AsString());
//                            //        }
//                            //    default:
//                            //        ReValueWhere = true;
//                            //        break;
//                            //}
//                            //return true;
//                        })
//                        .ToList();
//                };

//            ValueWhere.Recusion(o => { Cut(o); return true; });
//        }


//        /// <summary>
//        /// 把 Where条件拆成两部分， 一部分是 值条件，另一部分是 关系条件。
//        /// 当不可分隔时，两部分返回空。
//        /// </summary>
//        private void SplitWhereClip()
//        {
//            //真正的条件是： 对Where条件中全都是 And 操作的Where条件，且非组合查询的 单独Where段进行裁剪。


//            //分离的条件： 所有连接必须都是 And, 查询项必须都是列，没有复合列，没有函数。
//            ValueWhere = new WhereClip();
//            RelationWhere = new WhereClip();

//            Func<WhereClip, bool> WhereIsValue = where =>
//                {
//                    var retVal = where.ValueType.IsIn(WhereValueEnum.Value, WhereValueEnum.ValueArray);

//                    if (where.Child != null)
//                    {
//                        retVal &= where.Child.Recusion(o =>
//                            {
//                                return o.ValueType.IsIn(WhereValueEnum.Value, WhereValueEnum.ValueArray);
//                            });
//                    }

//                    return retVal;
//                };

//            Func<WhereClip, bool> WhereIsMust = where =>
//                {
//                    var retVal = (where.Linker == SqlOperator.And) && (where.Query.Key == SqlKeyword.Column) &&
//                        (where.Query.Extend.Count(o => o.Key.IsIn(SqlOperator.As, SqlOperator.Const) == false) > 0);

//                    if (where.Child != null)
//                    {
//                        retVal &= where.Child.Recusion(o =>
//                            {
//                                return (o.Linker == SqlOperator.And) && (o.Query.Key == SqlKeyword.Column);
//                            });
//                    }

//                    return retVal;
//                };

//            var IsAllValue = true;
//            var IsMust = false;


//            //其实这里只有一个WhereClip
//            Context.Dna.Where(o => o.Key == SqlKeyword.Where).Select(o => o as WhereClip).All(o =>
//                {
//                    var whe = o;
//                    while (whe != null)
//                    {
//                        if (IsMust)
//                        {
//                            IsMust &= WhereIsMust(whe);
//                        }

//                        if (!IsMust) return false;

//                        if (IsAllValue)
//                        {
//                            IsAllValue &= WhereIsValue(whe);
//                        }
//                        whe = whe.Next;
//                    }

//                    return true;
//                });

//            if (!IsMust)
//            {
//                ValueWhere = null;
//                RelationWhere = null;
//            }
//            else if (IsAllValue)
//            {
//                ValueWhere = Context.Dna.First(o => o.Key == SqlKeyword.Where) as WhereClip;
//                return;
//            }
//            else
//            {
//                Context.Dna.Where(o => o.Key == SqlKeyword.Where).Select(o => o as WhereClip).All(o =>
//                    {
//                        var whe = o;
//                        while (whe != null)
//                        {
//                            if (WhereIsValue(whe))
//                            {
//                                ValueWhere &= whe;
//                            }
//                            else
//                            {
//                                RelationWhere &= whe;
//                            }

//                            whe = whe.Next;
//                        }

//                        return true;
//                    });
//            }
//        }

//        private void LoadSource()
//        {
//            var alias = Context.CurrentRule.GetAliasOrName();

//            Source = new Dictionary<string, JoinTableData>();
//            Source.Add(alias, new JoinTableData()
//            {
//                Alias = alias,
//                Data = dbo.OnCacheBoxyLoad(Context.CurrentRule).IfNoValue(null, s => s.Clone() as MyOqlSet),
//                JoinType = 0
//            });


//            //先Check一下条件。同时，加载出必要的数据。

//            Context.Dna.Where(o => o.Key.IsIn(SqlKeyword.Join, SqlKeyword.LeftJoin, SqlKeyword.RightJoin)).All(o =>
//            {
//                var tab = (o as JoinTableClip);
//                Source.Add(tab.Table.GetAliasOrName(), new JoinTableData()
//                {
//                    Alias = tab.Table.GetAliasOrName(),
//                    JoinType = o.Key,
//                    Data = dbo.OnCacheBoxyLoad(tab.Table).Clone() as MyOqlSet,
//                    OnWhere = tab.OnWhere
//                });
//                return true;
//            });
//        }

//        private bool Check()
//        {
//            if (this.Context.CurrentRule.GetBoxyCache() <= 0) return false;
//            return true;
//        }
//    }
//}