using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace MyOql
{
    public partial class SelectClip
    {
        public TModel ToEntity<TModel>() where TModel : new()
        {
            return ToEntity<TModel>(() => new TModel());
        }

        public ColumnClip[] GetColumns()
        {
            return Enumerable.Select(this.Dna.Where(o => o.IsColumn()), o => o as ColumnClip).ToArray();
        }

        public TModel ToEntity<TModel>(TModel Model) where TModel : new()
        {
            return ToEntity<TModel>(() => new TModel());
        }

        public string ToEntity(string DefaultValue)
        {
            return ToEntity<string>(() => DefaultValue);
        }

        public List<string> ToEntityList(string DefaultValue)
        {
            return ToEntityList(() => string.Empty);
        }

        public List<int> ToEntityList(int DefaultValue)
        {
            return ToEntityList(() => DefaultValue);
        }

        public List<TModel> ToEntityList<TModel>() where TModel : new()
        {
            return ToEntityList<TModel>(() => new TModel());
        }

        public List<TModel> ToEntityList<TModel>(TModel Model) where TModel : new()
        {
            return ToEntityList<TModel>(() => new TModel());
        }


        public List<TModel> ToEntityList<TModel>(Func<TModel> NewModelFunc)
        {
            return ToDictList().Select(o => dbo.DictionaryToFuncModel(o, NewModelFunc)).ToList();
        }


        /// <summary>
        /// 推荐使用 ToDictList 方法。
        /// </summary>
        /// <returns></returns>
        public virtual DataTable ToDataTable()
        {
            DataSet ds = ToDataSet();
            if (ds == null || ds.Tables.Count == 0) return null;
            return ds.Tables[0];
        }

        //public SelecteClip LeftJoin<T2>(SelecteClip  JoinSelect, Func<T2, WhereClip> FuncOnWhere, Func<T2, IEnumerable<ColumnClip>> FuncSelect)
        //    where T2 : RuleBase, new()
        //{
        //    return Join<T2>(SqlKeyword.LeftJoin, JoinSelect, FuncOnWhere, FuncSelect);
        //}

        //public SelecteClip Join<T2>(SqlKeyword JoinType, SelecteClip<T2> JoinTable, Func<T2, WhereClip> FuncOnWhere)
        //    where T2 : RuleBase, new()
        //{
        //    return Join<T2>(JoinType, JoinTable, FuncOnWhere, o => (ColumnClip[])null);
        //}




        ///// <summary>
        ///// 生成 left join 子句.
        ///// </summary>
        ///// <example>
        /////     WhereClip where = new WhereClip();
        /////
        /////     where &amp;= dbr.App.TProductInfo.ProductCode == uid;
        /////
        /////	    OrderByClip order = new OrderByClip(sortName);
        /////	    order.IsAsc = IsAsc ;
        /////
        /////	    var set = dbr.App.TProductInfo.Select(o=&gt;o.Id)
        /////             .LeftJoin(dbr.App.TRegion, (a, b) =&gt; a.RegionID == b.Id, o=&gt; o.Name.As("RegionName"))
        /////             .Skip( Skip )
        /////             .Take(Take)
        /////             .Where(where)
        /////             .OrderBy(order)
        /////             .ToMyOqlSet(Option) ;
        ///// </example>
        ///// <typeparam name="T2"></typeparam>
        ///// <param name="JoinTable"></param>
        ///// <param name="FuncOnWhere"></param>
        ///// <param name="OneColumn"></param>
        ///// <returns></returns>
        //public SelecteClip LeftJoin<T2>(T2 JoinTable, Func<T2, WhereClip> FuncOnWhere, Func<T2, ColumnClip> OneColumn)
        //    where T2 : RuleBase
        //{
        //    return LeftJoin<T2>(JoinTable, FuncOnWhere, o => { return new ColumnClip[] { OneColumn(JoinTable) }; });
        //}

        ///// <summary>
        ///// 生成 left join 子句.
        ///// </summary>
        ///// <typeparam name="T2"></typeparam>
        ///// <param name="JoinTable"></param>
        ///// <param name="FuncOnWhere"></param>
        ///// <returns></returns>
        //public SelecteClip LeftJoin<T2>(T2 JoinTable, Func<T2, WhereClip> FuncOnWhere)
        //    where T2 : RuleBase
        //{
        //    Func<T2, IEnumerable<ColumnClip>> sel = null;
        //    return LeftJoin<T2>(JoinTable, FuncOnWhere, sel);
        //}

        ///// <summary>
        ///// 生成 left join 子句
        ///// </summary>
        ///// <typeparam name="T2"></typeparam>
        ///// <param name="JoinTable"></param>
        ///// <param name="FuncOnWhere"></param>
        ///// <param name="FuncSelect"></param>
        ///// <returns></returns>
        //public SelecteClip LeftJoin<T2>(T2 JoinTable, Func<T2, WhereClip> FuncOnWhere, Func<T2, IEnumerable<ColumnClip>> FuncSelect)
        //    where T2 : RuleBase
        //{
        //    return Join(SqlKeyword.LeftJoin, JoinTable, FuncOnWhere(JoinTable), FuncSelect);
        //}

        ///// <summary>
        ///// 生成 join 子句.
        ///// </summary>
        ///// <typeparam name="T2"></typeparam>
        ///// <param name="JoinTable"></param>
        ///// <param name="FuncOnWhere"></param>
        ///// <param name="OneColumn"></param>
        ///// <returns></returns>
        //public SelecteClip Join<T2>(T2 JoinTable, Func<T2, WhereClip> FuncOnWhere, Func<T2, ColumnClip> OneColumn)
        //    where T2 : RuleBase
        //{
        //    return Join<T2>(JoinTable, FuncOnWhere, o => { return new ColumnClip[] { OneColumn(JoinTable) }; });
        //}

        ///// <summary>
        ///// 生成 join 子句.
        ///// </summary>
        ///// <typeparam name="T2"></typeparam>
        ///// <param name="JoinTable"></param>
        ///// <param name="FuncOnWhere"></param>
        ///// <returns></returns>
        //public SelecteClip Join<T2>(T2 JoinTable, Func<T2, WhereClip> FuncOnWhere)
        //    where T2 : RuleBase
        //{
        //    Func<T2, IEnumerable<ColumnClip>> sel = null;
        //    return Join<T2>(JoinTable, FuncOnWhere, sel);
        //}

        ///// <summary>
        ///// 生成 join 子句.
        ///// </summary>
        ///// <typeparam name="T2"></typeparam>
        ///// <param name="JoinTable"></param>
        ///// <param name="FuncOnWhere"></param>
        ///// <param name="FuncSelect"></param>
        ///// <returns></returns>
        //public SelecteClip Join<T2>(T2 JoinTable, Func<T2, WhereClip> FuncOnWhere, Func<T2, IEnumerable<ColumnClip>> FuncSelect)
        //    where T2 : RuleBase
        //{
        //    return Join(SqlKeyword.Join, JoinTable, FuncOnWhere, FuncSelect);
        //}

        ///// <summary>
        ///// 生成 right join 子句.
        ///// </summary>
        ///// <typeparam name="T2"></typeparam>
        ///// <param name="JoinTable"></param>
        ///// <param name="FuncOnWhere"></param>
        ///// <param name="OneColumn"></param>
        ///// <returns></returns>
        //public SelecteClip RightJoin<T2>(T2 JoinTable, Func<T2, WhereClip> FuncOnWhere, Func<T2, ColumnClip> OneColumn)
        //    where T2 : RuleBase
        //{
        //    return RightJoin<T2>(JoinTable, FuncOnWhere, o => { return new ColumnClip[] { OneColumn(JoinTable) }; });
        //}

        ///// <summary>
        ///// 生成 right join 子句.
        ///// </summary>
        ///// <typeparam name="T2"></typeparam>
        ///// <param name="JoinTable"></param>
        ///// <param name="FuncOnWhere"></param>
        ///// <returns></returns>
        //public SelecteClip RightJoin<T2>(T2 JoinTable, Func<T2, WhereClip> FuncOnWhere)
        //    where T2 : RuleBase
        //{
        //    Func<T2, IEnumerable<ColumnClip>> sel = null;
        //    return RightJoin<T2>(JoinTable, FuncOnWhere, sel);
        //}


        ///// <summary>
        ///// 生成 right join 子句.
        ///// </summary>
        ///// <typeparam name="T2"></typeparam>
        ///// <param name="JoinTable"></param>
        ///// <param name="FuncOnWhere"></param>
        ///// <param name="FuncSelect"></param>
        ///// <returns></returns>
        //public SelecteClip RightJoin<T2>(T2 JoinTable, Func<T2, WhereClip> FuncOnWhere, Func<T2, IEnumerable<ColumnClip>> FuncSelect)
        //    where T2 : RuleBase
        //{
        //    return Join(SqlKeyword.RightJoin, JoinTable, FuncOnWhere, FuncSelect);
        //}


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <example>
        ///// <code>
        /////  var currPage = collection["page"].GetInt();
        /////  var eachCount = collection["rp"].GetInt();
        ///// 
        /////  var subQuery = dbr.SpecialItem
        /////       .Select(o => o.ID)
        /////       .Where(o => o.PID == 0)
        /////       .Skip((currPage - 1) * eachCount)
        /////       .Take(eachCount).AsTable("b");
        ///// 
        /////  var oqlSet = dbr.SpecialItem
        /////      .Select()
        /////      .Join(SqlKeyword.Join,
        /////          subQuery,
        /////          (a, b) => a.ID == b.ID.FromTable("b") | (a.RootPath + ",").Contains("," + b.ID.FromTable("b") + ","), o=>o.ID.FromTable("b")
        /////          )
        /////      .ToMyOqlSet();
        ///// </code>
        ///// </example>
        ///// <typeparam name="T2"></typeparam>
        ///// <param name="JoinType"></param>
        ///// <param name="JoinTable"></param>
        ///// <param name="FuncOnWhere"></param>
        ///// <param name="FuncSelect"></param>
        ///// <returns></returns>
        //public SelecteClip Join<T2>(SqlKeyword JoinType, SelecteClip<T2> JoinTable, Func<T2, WhereClip> FuncOnWhere, Func<T2, ColumnClip> FuncSelect)
        //    where T2 : RuleBase, new()
        //{
        //    return Join<T2>(JoinType, JoinTable, FuncOnWhere, o =>
        //    {
        //        if (FuncSelect != null)
        //        {
        //            return new ColumnClip[] { FuncSelect(new T2()) };
        //        }
        //        else
        //        {
        //            return (ColumnClip[])null;
        //        }
        //    });
        //}

    }
}

