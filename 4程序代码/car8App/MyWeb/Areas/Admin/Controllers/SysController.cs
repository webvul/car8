using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DbEnt;
using MyOql;
using MyCon;

namespace MyWeb.Areas.Admin.Controllers
{
    public class SysController : MyMvcController
    {
        //
        // GET: /Admin/Sys/

        public ActionResult CodeGen()
        {
            List<string> list = dbo.ToDataTable("", @"select name from sys.tables").ToEntityList(() => "");
            return View(list.ToArray());
        }

        //public ActionResult Ent(string uid)
        //{
        //    string ent = MyOql.Helper.MyOqlCodeGen.Do();
        //    ent = HttpUtility.HtmlEncode(ent);
        //    return View((object)ent);
        //}

    }
}
