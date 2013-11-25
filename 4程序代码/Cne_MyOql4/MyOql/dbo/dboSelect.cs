using System;
using System.Data;
using System.Data.Common;
using MyCmn;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace MyOql
{
    public static partial class dboSelect
    {
        /// <summary>
        /// 连接select 后的选择列。
        /// </summary>
        /// <param name="FuncSelect"></param>
        /// <returns></returns>
        public static T _<T>(this T select, ColumnClip column) where T : SelectClip
        {
            select.Dna.Add(column);
            return select;
        }






        /// <summary>
        /// 生成 order by 子句.
        /// </summary>
        /// <remarks>
        /// 如果排序列的列DbName以 # 开始，则从表中遍历查找该列名的列。
        /// </remarks>
        /// <param name="order"></param>
        /// <returns></returns>
        public static T OrderBy<T>(this T select, OrderByClip order) where T : SelectClip
        {
            if (order.HasData() == false) return select;
            //如果 Group 的列是 ConstColumn ， 则从Dna 中所有表检索列。
            //校验 是否有重复

            //List<string> listName = new List<string>();
            //Action<OrderByClip> CheckRepeat = o =>
            //{
            //    OrderByClip currClip = o;
            //    for (var i = 0; i < 110; i++)
            //    {
            //        if (currClip.HasData() == false) break;
            //        if (i > 100) throw new GodError("OrderBy 子句的排序列超出100");

            //        if (currClip.Order.IsSimple())
            //        {
            //            if (listName.Contains(currClip.Order.Name) == false)
            //            {
            //                listName.Add(currClip.Order.Name);
            //            }
            //            else
            //            {
            //                throw new GodError("OrderBy 子句检测到重复的列引用，重复列：" + currClip.Order.Name);
            //            }
            //        }
            //        currClip = currClip.Next;
            //    }
            //};

            var orderInDna = select.Dna.FirstOrDefault(o => o.Key == SqlKeyword.OrderBy) as OrderByClip;

            select.ProcPostOrder(order);

            if (orderInDna.HasData())
            {
                orderInDna &= order;
            }
            else
            {
                select.Dna.Add(order);
            }

            //CheckRepeat(select.Dna.FirstOrDefault(o => o.Key == SqlKeyword.OrderBy) as OrderByClip);

            return select;
        }


        /// <summary>
        /// 生成 having 子句.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public static T Having<T>(this T select, WhereClip where) where T : SelectClip
        {
            HavingClip having = new HavingClip();
            having.Where = where;
            select.Dna.Add(having);
            return select;
        }



        /// <summary>
        /// 生成 group by 子句
        /// </summary>
        /// <example>
        /// <code>
        ///     var users = dbr.User
        ///         .Select(o=&gt;new ColumnClip[]{ o.DeptID,o.Count().As("cou") } )
        ///         .GroupBy(o=&gt;new ColumnClip[]{ o.DeptID })
        ///         .Having(o=&gt;o.Count() &gt; 3 )
        ///         .ToDictList() ;
        /// </code>        
        /// <code>
        ///     var users = dbr.User     
        ///               .Select(o=&gt;new ColumnClip[]{ o.DeptID.As("User's DeptID"),o.Count().As("cou") } )
        ///               .Join(dbr.Dept,(a,b)=&gt;a.DeptID == b.ID ,o=&gt;o.Name.As("DeptName") )
        ///               .GroupBy(o=&gt;new ColumnClip[]{ o.DeptID , dbr.Dept.Name } )
        ///               .Having(o=&gt;o.Count() &gt; 3 )
        ///               .ToDictList() ;
        /// </code>
        /// </example>
        /// <param name="Group"></param>
        /// <returns></returns>
        public static T GroupBy<T>(this T select, params ColumnClip[] Group)
            where T : SelectClip
        {
            if (Group == null || Group.Length == 0) return select;

            GroupByClip groupInDna = select.Dna.FirstOrDefault(o => o.Key == SqlKeyword.GroupBy) as GroupByClip;

            if (groupInDna == null)
            {
                select.Dna.Add(new GroupByClip());
                groupInDna = select.Dna.FirstOrDefault(o => o.Key == SqlKeyword.GroupBy) as GroupByClip;
            }


            groupInDna.Groups.AddRange(Group);
            return select;
        }


        /// <summary>
        /// 根据聚合函数,自动生成分组.
        /// </summary>
        /// <example>
        /// <code>
        /// var users = dbr.User
        ///             .Select(o=&gt;new ColumnClip[]{ o.DeptID,o.Count().As("cou") } )
        ///             .AutoGroup()
        ///             .Having(o=&gt;o.Count() &gt; 3 )
        ///             .ToDictList() ;
        /// </code>
        /// </example>
        /// <returns></returns>
        public static T AutoGroup<T>(this T select)
            where T : SelectClip
        {
            if (select.Dna.Any(o => o.IsColumn() &&
                (o as ColumnClip).IsPolymer()))
            {
                var cols = select.Dna
                        .Where(o => o.IsColumn() && !(o as ColumnClip).IsPolymer());

                return select.GroupBy(Enumerable.Select(cols, o => o as ColumnClip).ToArray());
            }

            return select;
        }




        /// <summary>
        /// 以子查询为结果集进行再查询.
        /// </summary>
        /// <remarks>
        /// <example>
        /// <code>
        /// select col from ( select col from tab )  as tab 
        /// var d = dbr.ViewProject.Select().SelectWrap("a").ToEntityList(o => dbr.ViewProject._);
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="Alias"> 别名 </param>
        /// <param name="cols"></param>
        /// <returns></returns>

        public static T SelectWrap<T>(this T thi, string Alias, params ColumnClip[] cols)
            where T : SelectClip, new()
        {
            var sel = new T();
            sel.CurrentRule = thi.CurrentRule;
            var alias = Alias.HasValue() ? Alias : thi.CurrentRule.GetName();
            thi.AsTable(alias);
            sel.Dna.Add(thi);
            if (cols != null && cols.Length > 0)
            {
                sel.Dna.AddRange(cols);
            }
            else
            {
                thi.Dna.Where(o => o.IsColumn()).All(o =>
                {
                    var col = o as ColumnClip;
                    var newCol = new SimpleColumn();
                    newCol.Name = col.GetAlias();
                    newCol.DbName = newCol.Name;
                    newCol.DbType = col.DbType;

                    if (col.IsSimple())
                    {
                        var simple = col as SimpleColumn;
                        newCol.TableDbName = simple.TableDbName;
                        newCol.TableName = simple.TableName;
                    }

                    sel.Dna.Add(newCol.FromTable(alias));
                    return true;
                });
            }
            return sel;
        }


 




        /// <summary>
        /// 添加 where 子句.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public static T Where<T>(this T select, WhereClip where)
            where T : SelectClip
        {
            if (where != null)
            {
                select.AppendWhere(where);
            }
            return select;
        }



        /// <summary>
        /// 给表起个别名. <see cref="MyOql.SelectClip.Join"/>
        /// </summary>
        /// <remarks>
        /// 高级用法: 用子查询做联接 .
        /// <code>
        /// var currPage = collection["page"].GetInt();
        /// var eachCount = collection["rp"].GetInt();
        /// 
        /// var subQuery = dbr.SpecialItem
        ///      .Select(o =&gt; o.ID)
        ///      .Where(o =&gt; o.PID == 0)
        ///      .Skip((currPage - 1) * eachCount)
        ///      .Take(eachCount).AsTable("b");
        /// 
        /// var oqlSet = dbr.SpecialItem
        ///     .Select()
        ///     .Join(SqlKeyword.Join,
        ///         subQuery,
        ///         (a, b) =&gt; a.ID == b.ID.FromTable("b") | (a.RootPath + ",").Contains("," + b.ID.FromTable("b") + ","), null
        ///         )
        ///     .ToMyOqlSet();
        /// 
        /// </code>
        /// </remarks>
        /// <param name="Alias"></param>
        /// <returns></returns>
        public static T AsTable<T>(this T select, string Alias) where T : SelectClip
        {
            select.Alias = Alias;
            return select;
        }



        /// <summary>
        /// 生成 group by 子句. 
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T GroupBy<T>(this T select, Func<IEnumerable<ColumnClip>> func)
            where T :SelectClip
        {
            if (func == null) return select;
            select.GroupBy(func.Invoke().ToArray());
            return select;
        }
        /// <summary>
        /// 添加 where 子句.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T Where<T>(this T select, Func<WhereClip> func)
            where T:SelectClip
        {
            if (func != null)
            {
                return select.Where(func());
            }
            else return select;
        }

    }
}
