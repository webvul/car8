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
        /// 生成 join  Table 子句
        /// </summary>
        /// <remarks>
        /// 如果连接表有 AsTable，而条件或外带查询列属于该表，且没有FromTable，则自动添加 FromTable。
        /// 要避免列自动添加FromTable，可以手动添加FromTable
        /// </remarks>
        /// <typeparam name="T2"></typeparam>
        /// <param name="JoinType"></param>
        /// <param name="JoinTable"></param>
        /// <param name="FuncOnWhere"></param>
        /// <param name="FuncSelect"></param>
        /// <returns></returns>
        public static T Join<T, T2>(this T thi, SqlKeyword JoinType, T2 JoinTable, WhereClip where, IEnumerable<ColumnClip> SelectColumns)
            where T : SelectClip
            where T2 : RuleBase
        {
            if (JoinTable == null) return thi;
            if (JoinType == 0) return thi;

            where.SetKey(SqlKeyword.OnWhere);

            if (SelectColumns != null)
            {
                var listCols = SelectColumns.Where(o => o.EqualsNull() == false).ToList();
                thi.Dna.AddRange(listCols.ToArray());
            }


            thi.Dna.Add(new JoinTableClip(JoinType) { Table = JoinTable, OnWhere = where });
            thi.Dna.Add(where);

            return thi;
        }

        /// <summary>
        /// 生成 Join (select  * from tab ) as a  子查询
        /// </summary>
        /// <param name="JoinType"></param>
        /// <param name="JoinSelect"></param>
        /// <param name="where"></param>
        /// <param name="SelectColumns"></param>
        /// <returns></returns>
        public static T Join<T>(this T select, SqlKeyword JoinType, SelectClip JoinSelect, WhereClip where, IEnumerable<ColumnClip> SelectColumns)
            where T : SelectClip
        {
            where.SetKey(SqlKeyword.OnWhere);

            if (SelectColumns != null)
            {
                select.Dna.AddRange(SelectColumns.Where(o => o.EqualsNull() == false).ToArray());
            }

            select.Dna.Add(new JoinTableClip(JoinType) { SubSelect = JoinSelect, OnWhere = where });
            select.Dna.Add(where);

            return select;
        }

        /// <summary>
        /// 生成 join 子句.
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="JoinTable">连接的表，在两个表只有一个外键关系时，自动生成 On 语句。否则报错</param>
        /// <returns></returns>
        public static T Join<T>(this T select, RuleBase JoinTable)
            where T : SelectClip
        {
            select.Join(SqlKeyword.Join, JoinTable, dbo.GetOnWhere(select.CurrentRule, JoinTable), new Columns());
            return select;
        }



        /// <summary>
        /// 生成 join 子句.
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="JoinTable">连接的表，在两个表只有一个外键关系时，自动生成 On 语句。否则报错</param>
        /// <param name="OneColumn"></param>
        /// <returns></returns>
        public static T Join<T, T2>(this T select, T2 JoinTable, Func<T2, ColumnClip> OneColumn)
            where T : SelectClip
            where T2 : RuleBase
        {
            select.Join(SqlKeyword.Join, JoinTable, dbo.GetOnWhere(select.CurrentRule, JoinTable), new Columns(OneColumn(JoinTable)));
            return select;
        }




        /// <summary>
        /// 生成 join 子句.
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="JoinTable">连接的表，在两个表只有一个外键关系时，自动生成 On 语句。否则报错</param>
        /// <param name="FuncSelect"></param>
        /// <returns></returns>
        public static T Join<T, T2>(this T select, T2 JoinTable, Func<T2, IEnumerable<ColumnClip>> FuncSelect)
            where T : SelectClip
            where T2 : RuleBase
        {
            select.Join(SqlKeyword.Join, JoinTable, dbo.GetOnWhere(select.CurrentRule, JoinTable), FuncSelect == null ? null : FuncSelect(JoinTable));
            return select;
        }
    }
}
