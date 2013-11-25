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
        public SelectClip<T> Join<T2>(SqlKeyword JoinType, SelectClip<T2> JoinSelect, Func<T, T2, WhereClip> FuncOnWhere, Func<T2, IEnumerable<ColumnClip>> FuncSelect)
         where T2 : RuleBase, new()
        {
            WhereClip where = FuncOnWhere((T)(this.CurrentRule), (T2)(JoinSelect.CurrentRule));
            IEnumerable<ColumnClip> cols = FuncSelect != null ? FuncSelect((T2)(JoinSelect.CurrentRule)) : null;
            this.Join(JoinType, (SelectClip)JoinSelect, where, cols);
            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// <code>
        ///  var currPage = collection["page"].GetInt();
        ///  var eachCount = collection["rp"].GetInt();
        /// 
        ///  var subQuery = dbr.SpecialItem
        ///       .Select(o => o.ID)
        ///       .Where(o => o.PID == 0)
        ///       .Skip((currPage - 1) * eachCount)
        ///       .Take(eachCount).AsTable("b");
        /// 
        ///  var oqlSet = dbr.SpecialItem
        ///      .Select()
        ///      .Join(SqlKeyword.Join,
        ///          subQuery,
        ///          (a, b) => a.ID == b.ID.FromTable("b") | (a.RootPath + ",").Contains("," + b.ID.FromTable("b") + ","), o=>o.ID.FromTable("b")
        ///          )
        ///      .ToMyOqlSet();
        /// </code>
        /// </example>
        /// <typeparam name="T2"></typeparam>
        /// <param name="JoinType"></param>
        /// <param name="JoinTable"></param>
        /// <param name="FuncOnWhere"></param>
        /// <param name="FuncSelect"></param>
        /// <returns></returns>
        public SelectClip<T> Join<T2>(SqlKeyword JoinType, SelectClip<T2> JoinTable, Func<T, T2, WhereClip> FuncOnWhere, Func<T2, ColumnClip> FuncSelect)
            where T2 : RuleBase, new()
        {
            return Join<T2>(JoinType, JoinTable, FuncOnWhere, o =>
            {
                if (FuncSelect != null)
                {
                    return new ColumnClip[] { FuncSelect(new T2()) };
                }
                else
                {
                    return (ColumnClip[])null;
                }
            });
        }



        public SelectClip<T> Join<T2>(SqlKeyword JoinType, SelectClip<T2> JoinTable, Func<T, T2, WhereClip> FuncOnWhere)
            where T2 : RuleBase, new()
        {
            return Join<T2>(JoinType, JoinTable, FuncOnWhere, o => (ColumnClip[])null);
        }


    }
}

