//using System;
//using System.Collections.Generic;
//using System.Linq;
//using MyCmn;

//namespace MyOql
//{
//    public partial class JoinTableData
//    {
//        public string Alias { get; set; }
//        public MyOqlSet Data { get; set; }
//        public WhereClip OnWhere { get; set; }
//        public SqlKeyword JoinType { get; set; }
//        public int Position { get; set; }
//        public bool Hited { get; set; }

//        public object GetValue(string ColumnName)
//        {
//            var colIndex = this.Data.Columns.IndexOf(o => o.Name == ColumnName);
//            return this.Data.Rows[this.Position][colIndex];
//        }

//        public object GetValue(int Index, string ColumnName)
//        {
//            var colIndex = this.Data.Columns.IndexOf(o => o.Name == ColumnName);
//            return this.Data.Rows[Index][colIndex];
//        }

//        public JoinTableData()
//        {
//            this.Position = -1;
//            Hited = false;
//        }
//    }

//    public class BoxyCache
//    {
//        public static MyOqlSet Find_Other(ContextClipBase Context)
//        {
//            //
//            //if (Check(Context) == false) return null;

//            //var container = GetDefine(Context);

//            //MyOqlSet chaSet = GetCacheSet(Context, "a");

//            MyOqlSet set = new MyOqlSet(Context.CurrentRule);
//            return set;
//        }

//        private static MyOqlSet GetCacheSet(ContextClipBase Context, string p)
//        {
//            throw new NotImplementedException();
//        }

//        private static bool Check(ContextClipBase Context)
//        {
//            if (Context.CurrentRule.GetBoxyCache() <= 0) return false;
//            return true;
//        }

//        //-----------------------------------------------------------------------------------

//        public static MyOqlSet Find(ContextClipBase Context)
//        {
//            if (Context.CurrentRule.GetBoxyCache() <= 0) return null;

//            //有任一个Join子句的Table没有启用全量缓存，也不能从缓存中查寻。

//            //先定义出容器（所有关联表的所有列，因为可能Select），
//            //再连接表计算OnWhere，再算聚合，再Where，再分页 ，
//            //最后Select。如果查询中有Sql函数，不满足，则退出。

//            var container = new List<int[]>(); //第一个是主表索引// Dictionary<int,XmlDictionary<string,int>>() ; //分别表示，主数据索引，连接表名字，连接表数据索引。

//            var alias = Context.CurrentRule.GetAlias().AsString(Context.CurrentRule.GetName());
//            var joinData = GetAllCacheData(Context, alias);

//            joinData[alias].Data.Rows.All(d =>
//            {
//                joinData[alias].Position++;

//                joinData.All(joinItem =>
//                {
//                    if (joinItem.Key == alias) return true;

//                    joinItem.Value.Position = -1;
//                    return true;
//                });

//                var extHasData = joinData.All(joinItem =>
//                {
//                    if (joinItem.Key == alias) return true;

//                    FindCacheJoinTable(container, alias, joinData, joinItem);


//                    return true;
//                });

//                return true;
//            });

//            FillRightJoinData(container, alias, joinData);

//            //再用 Where 过滤 container.
//            for (int i = 0; i < container.Count; i++)
//            {
//                var row = container[i];
//                if (ProcWhere(Context, joinData, row) == false)
//                {
//                    container.RemoveAt(i);
//                    i--;
//                }
//            }

//            MyOqlSet set = ToSet(Context, container, joinData);

//            return set;
//        }

//        private static Dictionary<string, JoinTableData> GetAllCacheData(ContextClipBase Context, string alias)
//        {
//            var joinData = new Dictionary<string, JoinTableData>();
//            joinData.Add(alias, new JoinTableData()
//            {
//                Alias = alias,
//                Data = dbo.OnCacheBoxyLoad(Context.CurrentRule).Clone() as MyOqlSet,
//                JoinType = 0
//            });
//            //先Check一下条件。同时，加载出必要的数据。

//            Context.Dna.Where(o => o.Key.IsIn(SqlKeyword.Join, SqlKeyword.LeftJoin, SqlKeyword.RightJoin)).All(o =>
//            {
//                var tab = (o as JoinTableClip);
//                joinData.Add(tab.Table.GetAlias().AsString(tab.Table.GetName()), new JoinTableData()
//                {
//                    Alias = tab.Table.GetAlias().AsString(tab.Table.GetName()),
//                    JoinType = o.Key,
//                    Data = dbo.OnCacheBoxyLoad(tab.Table).Clone() as MyOqlSet,
//                    OnWhere = tab.OnWhere
//                });
//                return true;
//            });
//            return joinData;
//        }

//        private static void FindCacheJoinTable(List<int[]> container, string alias, Dictionary<string, JoinTableData> joinData, KeyValuePair<string, JoinTableData> joinItem)
//        {
//            if (joinItem.Value.JoinType == SqlKeyword.Join || joinItem.Value.JoinType == SqlKeyword.RightJoin)
//            {
//                joinItem.Value.Data.Rows.All(r =>
//                {
//                    joinItem.Value.Position++;

//                    if (ProcJoin(joinData, joinItem.Value) == true)
//                    {
//                        container.Add(joinData.Select(o => o.Value.Position).ToArray());// new CacheDataIndex(joinData[alias].Position, joinData));

//                        //如果有一方是主键，则停止。
//                        if (joinItem.Value.OnWhere.GetIdValue(joinItem.Value.Data.Entity).HasValue() ||
//                            joinItem.Value.OnWhere.GetIdValue(joinData[alias].Data.Entity).HasValue())
//                        {
//                            return false;
//                        }
//                    }

//                    return true;
//                });
//            }
//            else if (joinItem.Value.JoinType == SqlKeyword.LeftJoin)
//            {
//                var hited = false;
//                joinItem.Value.Data.Rows.All(r =>
//                {
//                    joinItem.Value.Position++;

//                    if (ProcJoin(joinData, joinItem.Value) == true)
//                    {
//                        hited |= true;
//                        container.Add(joinData.Select(o => o.Value.Position).ToArray());//new CacheDataIndex(joinData[alias].Position, joinData));

//                        //如果有一方是主键，则停止。
//                        if (joinItem.Value.OnWhere.GetIdValue(joinItem.Value.Data.Entity).HasValue() ||
//                            joinItem.Value.OnWhere.GetIdValue(joinData[alias].Data.Entity).HasValue())
//                        {
//                            return false;
//                        }
//                    }

//                    return true;
//                });

//                //如果没有查出东西。再加一条空记录。
//                if (hited == false)
//                {
//                    container.Add(joinData.Select(o => o.Value.Position).ToArray());//new CacheDataIndex(joinData[alias].Position, joinData));
//                }
//            }
//            return;
//        }

//        private static void FillRightJoinData(List<int[]> container, string alias, Dictionary<string, JoinTableData> joinData)
//        {
//            var curJoinTableIndex = -1;
//            //补全 RightJoin
//            joinData.All(joinItem =>
//            {
//                if (joinItem.Key == alias) return true;

//                joinItem.Value.Position = -1;
//                curJoinTableIndex++;
//                if (joinItem.Value.JoinType == SqlKeyword.RightJoin)
//                {
//                    joinItem.Value.Position = -1;
//                    var IsNotExists = joinItem.Value.Data.Rows.All(o =>
//                    {
//                        joinItem.Value.Position++;
//                        if (container.Exists(c => c[curJoinTableIndex] == joinItem.Value.Position))
//                        {
//                            return false;
//                        }
//                        return true;
//                    });

//                    if (IsNotExists)
//                    {
//                        var emp = new List<int>();
//                        joinData.All(_o =>
//                        {
//                            emp.Add(-1);
//                            return true;
//                        });
//                        emp[curJoinTableIndex] = joinItem.Value.Position;
//                        container.Add(emp.ToArray());
//                    }
//                }
//                return true;
//            });
//        }

//        private static MyOqlSet ToSet(ContextClipBase Context, List<int[]> container, Dictionary<string, JoinTableData> joinData)
//        {
//            MyOqlSet set = new MyOqlSet(Context.CurrentRule);
//            Context.Dna.Where(o => o.Key.IsIn(SqlKeyword.Column)).All(o =>
//            {
//                set.InsertColumn(set.Columns.Count(), o as ColumnClip, null);
//                return true;
//            });

//            Func<Dictionary<string, JoinTableData>, MyOqlSet, int[], object[]> ToData = (tabs, _set, rowIndex) =>
//            {
//                List<object> list = new List<object>();
//                _set.Columns.All(column =>
//                {
//                    //列所有表的索引。
//                    var tabIndex = tabs.IndexOf(o => o.Key == column.Table.GetAlias().AsString(column.Table.GetName()));

//                    list.Add(tabs.ElementAt(tabIndex).Value.GetValue(rowIndex[tabIndex], column.Name));

//                    return true;
//                });

//                return list.ToArray();
//            };


//            //整理成对象。
//            container.All(o =>
//            {
//                set.Rows.Add(ToData(joinData, set, o));
//                return true;
//            });
//            return set;
//        }


//        //LeftJoin 和 RightJoin 总是返回true. 如果 没有命中，则Hited是false。
//        private static bool ProcJoin(Dictionary<string, JoinTableData> _allJoin, JoinTableData _curJoin)
//        {
//            var queryJoin = _allJoin[_curJoin.OnWhere.Query.Table.GetAlias() | new StringLinker(_curJoin.OnWhere.Query.Table.GetName())];
//            var valJoin = _allJoin[(_curJoin.OnWhere.Value as ColumnClip).Table.GetAlias() | new StringLinker((_curJoin.OnWhere.Value as ColumnClip).Table.GetName())];

//            if (queryJoin.GetValue(_curJoin.OnWhere.Query.Name).AsString() ==
//                valJoin.GetValue((_curJoin.OnWhere.Value as ColumnClip).Name).AsString())
//            {
//                _curJoin.Hited = true;
//                return true;
//            }
//            else
//            {
//                _curJoin.Hited = false;
//                return false;
//            }
//            //return GetOnWhereDelegate(_set, _curPos, _curJoin).DynamicInvoke().AsBool();
//        }

//        private static bool ProcWhere(ContextClipBase Context, Dictionary<string, JoinTableData> _allJoin, int[] rowIndex)
//        {
//            var where = Context.Dna.First(o => o.Key == SqlKeyword.Where) as WhereClip;
//            var queryJoin = _allJoin[where.Query.Table.GetAlias() | new StringLinker(where.Query.Table.GetName())];
//            //var valJoin = _allJoin[(where.Value as ColumnClip).Table.GetAlias() | new StringLinker((where.Value as ColumnClip).Table.GetName())];

//            //find query table index
//            var queryTableIndex = _allJoin.IndexOf(o => o.Key == where.Query.Table.GetAlias().AsString(where.Query.Table.GetName()));

//            if (queryJoin.GetValue(rowIndex[queryTableIndex], where.Query.Name).AsString() == where.Value.AsString())
//            //valJoin.GetValue((where.Value as ColumnClip).Name))
//            {
//                return true;
//            }
//            else return false;
//            //return GetWhereDelegate(_set, _index, _curJoin).DynamicInvoke().AsBool();
//        }
//    }
//}
