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
        /// 生成 join 子句.
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="JoinTable"></param>
        /// <param name="FuncOnWhere"></param>
        /// <param name="OneColumn"></param>
        /// <returns></returns>
        public SelectClip<T> Join<T2>(T2 JoinTable, Func<T, T2, WhereClip> FuncOnWhere, Func<T2, ColumnClip> OneColumn)
            where T2 : RuleBase
        {
            return Join<T2>(JoinTable, FuncOnWhere, o => { return new ColumnClip[] { OneColumn(JoinTable) }; });
        }

        /// <summary>
        /// 生成 join 子句.
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="JoinTable"></param>
        /// <param name="FuncOnWhere"></param>
        /// <returns></returns>
        public SelectClip<T> Join<T2>(T2 JoinTable, Func<T, T2, WhereClip> FuncOnWhere)
            where T2 : RuleBase
        {
            Func<T2, IEnumerable<ColumnClip>> sel = null;
            return Join<T2>(JoinTable, FuncOnWhere, sel);
        }

        /// <summary>
        /// 生成 join 子句.
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="JoinTable"></param>
        /// <param name="FuncOnWhere"></param>
        /// <param name="FuncSelect"></param>
        /// <returns></returns>
        public SelectClip<T> Join<T2>(T2 JoinTable, Func<T, T2, WhereClip> FuncOnWhere, Func<T2, IEnumerable<ColumnClip>> FuncSelect)
            where T2 : RuleBase
        {
            return this.Join(SqlKeyword.Join, JoinTable, FuncOnWhere((T)(object)this.CurrentRule, JoinTable), FuncSelect == null ? (IEnumerable<ColumnClip>)null : FuncSelect(JoinTable));
        }
    }
}

