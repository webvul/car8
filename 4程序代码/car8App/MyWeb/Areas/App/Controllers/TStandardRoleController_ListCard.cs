using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using MyOql;
using MyCon;
using System.Data.OleDb;
using System.Data;
using System.Threading;
using MyBiz.App;
using DbEnt;

namespace MyWeb.Areas.App.Controllers
{
    public partial class TStandardRoleController : MyMvcController
    {

        public ActionResult List()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Query(string uid, FormCollection collection)
        {
            var query = collection["query"].FromJson<XmlDictionary<string, string>>();

            string name = query.GetOrDefault("Name");
            string code = query.GetOrDefault("Code");
            var menu = query.GetOrDefault("Menu").AsInt();

            string sortName = collection["sortname"];
            string sortOrder = collection["sortorder"];

            return TStandardRoleBiz.Query(
                collection["skip"].AsInt(),
                collection["take"].AsInt(),
                sortName,
                sortOrder != "desc",
                SelectOption.WithCount,
                code,
                name,
                menu
                )
                .LoadFlexiGrid();
        }

        [HttpPost]
        public ActionResult Delete(string uid, FormCollection collection)
        {
            string[] delIds = collection["query"].Split(',');
            return TStandardRoleBiz.Delete(delIds);
        }

        public ActionResult Detail(Guid uid)
        {
            return View("Card", dbr.TStandardRole.FindByStandardRoleId(uid));
        }
        public ActionResult Add(string uid)
        {
            return View("Card", new TStandardRoleRule.Entity());
        }

        [HttpPost]
        public ActionResult Add(string uid, TStandardRoleRule.Entity Entity)
        {
            return TStandardRoleBiz.Add(Entity);
        }

        public ActionResult Update(Guid uid)
        {
            return View("Card", dbr.TStandardRole.FindByStandardRoleId(uid));
        }

        [HttpPost]
        public ActionResult Update(String uid, TStandardRoleRule.Entity Entity)
        {
            return TStandardRoleBiz.Update(Entity);
        }

        ///// <summary>
        ///// 查询
        ///// </summary>
        ///// <param name="uid"></param>
        ///// <param name="collection"></param>
        ///// <returns></returns>
        //public ActionResult QueryUserByStanRole(string uid, FormCollection collection)
        //{
        //    var query = collection["query"].FromJson<XmlDictionary<string, string>>();


        //    string sortName = collection["sortname"];
        //    string sortOrder = collection["sortorder"];

        //    return TStandardRoleBiz.QueryUserByStanRole(
        //        collection["skip"].AsInt(),
        //        collection["take"].AsInt(),
        //        sortName,
        //        sortOrder != "desc",
        //        SelectOption.WithCount,
        //        uid
        //        )
        //        .LoadFlexiGrid();
        //}
    }
}
