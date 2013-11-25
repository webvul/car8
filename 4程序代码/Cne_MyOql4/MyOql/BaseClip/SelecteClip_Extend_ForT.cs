using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using System.Data;
using System.Xml.Serialization;
using System.Xml;

namespace MyOql
{
    public partial class SelectClip
    {
        public MyOqlSet ToMyOqlSet()
        {
            return ToMyOqlSet(SelectOption.WithCount);
        }

        public MyOqlSet ToMyOqlSet(SelectOption Option)
        {
            var retVal = new MyOqlSet(this.CurrentRule);
            var order = this.Dna.FirstOrDefault(o => o.Key == SqlKeyword.OrderBy) as OrderByClip;
            if (order != null)
            {
                retVal.OrderBy = order;
            }

            if (dbo.Event.OnReading(this) == false) return retVal;


            //if (this.CurrentRule.GetBoxyCache() > 0)
            //{
            //    var cacheData = dbo.OnCacheFindSome(this);
            //    if (cacheData != null)
            //    {
            //        return cacheData;
            //    }
            //}


            retVal.Columns =
                       Enumerable.Select(
                           this.Dna.Where(o => o.IsColumn()),
                            o => o.Clone() as ColumnClip
                            ).ToArray();

            CachedMyOqlSet cachedSet = null;


            if (Option == SelectOption.WithCount)
            {
                this.Linker = SqlOperator.Enter;
                this.Next = GetQueryResultCount();
            }

            var myCmd = this.ToCommand();
            string cacheSqlKey = null;
            if (this.CurrentRule.GetCacheSqlTime() > 0 && !this.ContainsFunctionRule())
            {
                cacheSqlKey = this.GetCacheSql(myCmd.FullSql);
                cachedSet = dbo.Event.OnCacheFindBySql(this, cacheSqlKey);
                if (cachedSet.HasValue)
                {
                    if (cachedSet.Set != null)
                        retVal = cachedSet.Set.Clone() as MyOqlSet;
                }
            }

            if (cachedSet == null || (cachedSet.HasValue == false))
            {
                this.ExecuteReader(myCmd, o =>
                {
                    retVal.Rows.Add(o.ToDictionary());
                    return true;
                }, o =>
                {
                    if (Option == SelectOption.WithCount)
                    {
                        retVal.Count = o.GetValue(0).AsLong();
                        this.SmartRowCount = retVal.Count;
                    }
                    return false;
                });
            }

            if (this.CurrentRule.GetCacheSqlTime() > 0 && cacheSqlKey.HasValue() && !this.ContainsFunctionRule())
            {
                dbo.Event.OnCacheAddBySql(this, cacheSqlKey, () => retVal);
            }


            dbo.Event.OnReaded(this);
            return retVal;
        }

        /// <summary>
        /// 通过构造Dna，生成查询条数的语句。
        /// </summary>
        /// <returns></returns>
        public SelectClip GetQueryResultCount()
        {
            var sel = this.Clone() as SelectClip;
            bool hasDistinct = sel.Dna.Any(o => o.Key == SqlKeyword.Distinct) || sel.TableHasPolymer();

            sel.Dna.RemoveAll(o => o.Key == SqlKeyword.Skip);
            sel.Dna.RemoveAll(o => o.Key == SqlKeyword.Take);
            sel.Dna.RemoveAll(o => o.Key == SqlKeyword.OrderBy);

            if (hasDistinct)
            {
                return sel.SelectWrap("__SubQuery__", sel.CurrentRule.Count().As("Cou"));
            }
            else
            {
                sel.Dna.RemoveAll(o => o.IsColumn());
                sel.Dna.Add(sel.CurrentRule.Count().As("Cou"));

                sel.ParameterColumn.Clear();
                sel.RunCommand = null;
                return sel;
            }
        }


        /// <summary>
        /// 采用 NotIn方式进行分页
        /// </summary>
        /// <param name="Option"></param>
        /// <param name="retVal"></param>
        //[Obsolete("采用Sqlserver2000 及以下版本使用 Not In 分页， 尽量不要使用。")]
        //protected void ToMyOqlSetUseOldPageMethod(SelectOption Option, MyOqlSet retVal)
        //{
        //    var Skip_Clip = this.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Skip) as SkipClip;
        //    int Skip = 0;
        //    if (Skip_Clip != null) Skip = Skip_Clip.SkipNumber;
        //    bool hasTop = this.Dna.Exists(o => o.Key == SqlKeyword.Take);

        //    int rc = 0;
        //    bool IsUsePagerCommand = ExecuteReader((reader, usePager, position, index) =>
        //    {
        //        rc = index + 1;
        //        switch (position)
        //        {
        //            case ReaderPositionEnum.BeforeSkip:
        //                break;
        //            case ReaderPositionEnum.InTake:
        //                retVal.Rows.Add(reader.ToDictionary());
        //                break;
        //            case ReaderPositionEnum.AfterTake:
        //                return !usePager;
        //            default:
        //                break;
        //        }

        //        return true;
        //    });


        //    retVal.Count = rc;

        //    if (IsUsePagerCommand == false)
        //    {
        //    }
        //    else
        //    {
        //        retVal.Count = retVal.Rows.Count;

        //        if (Option == SelectOption.WithCount)
        //        {
        //            retVal.Count = GetQueryResultCount();
        //        }
        //    }
        //}



        /// <summary>
        /// 处理从客户Post过来的情况。
        /// </summary>
        /// <param name="pc"></param>
        public void ProcPostOrder(OrderByClip pc)
        {
            if (pc.HasData() == false) return;

            if (pc.PostOrderName.HasValue() == false)
            {
                ProcPostOrder(pc.Next);
                return;
            }

            var postOrder = pc.PostOrderName.Slice(1);
            //如果当前选择列中存在, 则从当前选择列中查找. 
            var findInDna = Enumerable.Select(
                this.Dna
                    .Where(o => o.IsColumn())
                    ,
                    o => o as ColumnClip)
                .FirstOrDefault(o => o.NameEquals(postOrder));

            if (findInDna.EqualsNull() == false)
            {
                pc.Order = findInDna.Clone() as ColumnClip;
                pc.PostOrderName = null;
                return;
            }

            //改变DbName 。

            var colInTab = this.CurrentRule.GetColumns().FirstOrDefault(o => o.NameEquals(postOrder));
            if (colInTab.EqualsNull() == false)
            {
                pc.Order = colInTab.Clone() as ColumnClip;
                pc.PostOrderName = null;
                return;
            }
            else
            {
                //遍历各个 JoinTable.
                foreach (var item in this.Dna.Where(o => o.Key.IsIn(SqlKeyword.Join, SqlKeyword.LeftJoin, SqlKeyword.RightJoin)).ToArray())
                {
                    JoinTableClip joinTable = item as JoinTableClip;
                    if (joinTable.Table != null)
                    {
                        var colInJoin = joinTable.Table.GetColumn(postOrder);
                        if (colInJoin.EqualsNull() == false)
                        {
                            pc.Order = colInJoin.Clone() as ColumnClip;
                            pc.PostOrderName = null;
                            return;
                        }
                    }
                    else
                    {
                        //subselect 没有处理。
                    }
                }
            }

            //如果找不到
            pc.PostOrderName = null;
            pc.Order = new ConstColumn(string.Empty);
        }


    }
}

