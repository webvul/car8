using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;

namespace MyOql
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class dboSelect
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="cols">
        /// 在多表关联查询中,如果选择了关联表,且 cols 为空, 则不选择主表列.
        /// 如果所有的查询列为空, 则查询列为 *
        /// </param>
        /// <returns></returns>
        public static SelectClip<T> Select<T>(this T obj, params ColumnClip[] cols) where T : RuleBase
        {
            SelectClip<T> sel = new SelectClip<T>(obj);
            if (cols != null)
            {
                sel.Dna.AddRange(cols.Where(o => o.HasData()));
            }
            return sel;
        }



        public static SelectClip<T> SelectWhere<T>(this T CurrentRule, Func<T, WhereClip> func) 
            where T : RuleBase 
        {
            WhereClip where = null;
            if (func != null) where = func(CurrentRule);

            return Select<T>(CurrentRule, CurrentRule.GetColumns()).Where(where);
        }


        /// <summary>
        /// 生成 Select 子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static SelectClip<T> Select<T>(this T obj, Func<T, ColumnClip> func) where T : RuleBase
        {
            ColumnClip col = null;
            if (func != null) col = func(obj);

            return Select<T>(obj, col);
        }




        /// <summary>
        /// 生成 Select 子句。
        /// </summary>
        /// <example>
        ///  var users = dbr.User
        ///                 .Select(o=&gt; new ColumnClip[]{ o.Id, o.Name, (o.Id + "." + o.Name).As("FullName") } )
        ///                 .Where(o=&gt; o.Age.Between( 20, 30 ) )
        ///                 .Take(15)
        ///                 .Skip(30)
        ///                 .OrderBy(o&gt; o.Id.Asc )
        ///                 .ToDictList();
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static SelectClip<T> Select<T>(this T obj, Func<T, IEnumerable<ColumnClip>> func) where T : RuleBase
        {
            return Select<T>(obj, (func.Invoke(obj) ?? new ColumnClip[] { }).ToArray());
        }



        /// <summary>
        /// 选择该实体的全部列。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CurrentRule"></param>
        /// <returns></returns>
        public static SelectClip<T> Select<T>(this T CurrentRule) where T : RuleBase
        {
            return Select<T>(CurrentRule, CurrentRule.GetColumns());
        }


        public static T SmartPager<T>(this T select, long pagerCount)
            where T : SelectClip
        {
            select.SmartRowCount = pagerCount;
            return select; 
        }
    }
}
