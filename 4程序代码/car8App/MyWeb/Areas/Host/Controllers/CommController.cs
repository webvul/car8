using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using MyOql;
using MyCon;
using MyBiz;
using System.Threading;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using DbEnt.Model;
using System.Linq.Expressions;
using System.Transactions;
using DbEnt;

namespace MyWeb.Areas.Host.Controllers
{
    [NoPowerFilter]
    public class CommController : MyMvcController
    {
        public ActionResult Index(int CommId)
        {
            var shops = dbr.Dept.Select()
                 .Join(dbr.DeptCommunity, (a, b) => a.Id == b.DeptID & b.CommID == CommId, o => (ColumnClip)null)
                 .Join(dbr.Annex, (a, b) => a.TitleExtend == b.Id, o => o.FullName)
                 .OrderBy(o => o.AddTime.Desc)
                 .ToEntityList(o => o._);

            return View(shops);
        }
    }
}
