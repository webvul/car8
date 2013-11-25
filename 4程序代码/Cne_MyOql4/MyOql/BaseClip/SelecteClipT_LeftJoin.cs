using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using System.Data;
using System.Xml.Serialization;
using System.Xml;

namespace MyOql
{
    public partial class SelectClip<T> : SelectClip
        where T : RuleBase
    {

        /// <summary>
        /// 生成 left join 子句.
        /// </summary>
        /// <example>
        ///     WhereClip where = new WhereClip();
        ///
        ///     where &amp;= dbr.App.TProductInfo.ProductCode == uid;
        ///
        ///	    OrderByClip order = new OrderByClip(sortName);
        ///	    order.IsAsc = IsAsc ;
        ///
        ///	    var set = dbr.App.TProductInfo.Select(o=&gt;o.Id)
        ///             .LeftJoin(dbr.App.TRegion, (a, b) =&gt; a.RegionID == b.Id, o=&gt; o.Name.As("RegionName"))
        ///             .Skip( Skip )
        ///             .Take(Take)
        ///             .Where(where)
        ///             .OrderBy(order)
        ///             .ToMyOqlSet(Option) ;
        /// </example>
        /// <typeparam name="T2"></typeparam>
        /// <param name="JoinTable"></param>
        /// <param name="FuncOnWhere"></param>
        /// <param name="OneColumn"></param>
        /// <returns></returns>
        public SelectClip<T> LeftJoin<T2>(T2 JoinTable, Func<T, T2, WhereClip> FuncOnWhere, Func<T2, ColumnClip> OneColumn)
            where T2 : RuleBase
        {
            return LeftJoin<T2>(JoinTable, FuncOnWhere, o => { return new ColumnClip[] { OneColumn(JoinTable) }; });
        }


        /// <summary>
        /// 生成 left join 子句.
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="JoinTable"></param>
        /// <param name="FuncOnWhere"></param>
        /// <returns></returns>
        public SelectClip<T> LeftJoin<T2>(T2 JoinTable, Func<T, T2, WhereClip> FuncOnWhere)
            where T2 : RuleBase
        {
            Func<T2, IEnumerable<ColumnClip>> sel = null;
            return LeftJoin<T2>(JoinTable, FuncOnWhere, sel);
        }


        /// <summary>
        /// 生成 left join 子句
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="JoinTable"></param>
        /// <param name="FuncOnWhere"></param>
        /// <param name="FuncSelect"></param>
        /// <returns></returns>
        public SelectClip<T> LeftJoin<T2>(T2 JoinTable, Func<T, T2, WhereClip> FuncOnWhere, Func<T2, IEnumerable<ColumnClip>> FuncSelect)
            where T2 : RuleBase
        {
            return this.Join(SqlKeyword.LeftJoin, JoinTable, FuncOnWhere((T)(object)this.CurrentRule, JoinTable), FuncSelect == null ? (IEnumerable<ColumnClip>)null : FuncSelect(JoinTable));
        }

        public SelectClip<T> LeftJoin<T2>(SelectClip<T2> JoinSelect, Func<T, T2, WhereClip> FuncOnWhere, Func<T2, IEnumerable<ColumnClip>> FuncSelect)
            where T2 : RuleBase, new()
        {
            return this.Join(SqlKeyword.LeftJoin, JoinSelect, FuncOnWhere, FuncSelect);
        }
    }
}

