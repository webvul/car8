using DbEnt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using MyCmn;
using MyOql;
using MyBiz.Admin;
using MyBiz;

namespace MyWeb.Areas.Api
{
    public class ProductType : ApiController
    {
        public ActionResult Query(string uid, ProductionBiz.ProductTypeQueryModel Query)
        {

            WhereClip where = dbr.ProductType.DeptID == MySession.Get(MySessionKey.DeptID);
            where &= dbr.ProductType.UserID == MySession.Get(MySessionKey.UserID);

            Query.Name.HasValue(o => where &= dbr.ProductType.Name.Like(string.Format("%{0}%", o)));


            return dbr.ProductType.Select(o => new ColumnClip[] { o.Id, o.Pid, o.Name, o.Descr, o.SortID, o.AddTime })
                   .Skip(Query.skip)
                   .Take(Query.take)
                   .Where(where)
                   .OrderBy(Query.sort)
                   .ToMyOqlSet()
                   .LoadFlexiGrid();
            //.LoadFlexiTreeGrid(dbr.NoticeType.Id.Name, dbr.NoticeType.Pid.Name);
        }

    }
}