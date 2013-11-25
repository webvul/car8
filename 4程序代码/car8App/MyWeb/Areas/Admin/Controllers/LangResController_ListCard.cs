using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using DbEnt;
using MyOql;
using MyCon;
using MyBiz;
using MyBiz.Admin;

using DbEnt;

namespace MyWeb.Areas.Admin.Controllers
{
    public partial class LangResController : MyMvcController
    {

        public ActionResult List()
        {
            return View();
        }

        public class QueryModel : QueryModelBase
        {
        }


        [HttpPost]
        public ActionResult Query(string uid, QueryModel Query)
        {
            
            return LangResBiz.Query(
                Query.skip,
                Query.take,
                Query.sort ,
                SelectOption.WithCount,
                null
                )
                .LoadFlexiGrid();
        }

        [HttpPost]
        public ActionResult Delete(string uid, FormCollection collection)
        {
            string[] delIds = collection["query"].Split(',');
            return LangResBiz.Delete(delIds);
        }

        public ActionResult Detail(String uid)
        {
            return View("Card", dbr.ResKey.FindByKey(uid));
        }
        public ActionResult Add(string uid)
        {
            return View("Card", new ResKeyRule.Entity());
        }

        [HttpPost]
        public ActionResult Add(string uid, ResKeyRule.Entity Entity)
        {
            return LangResBiz.Add(Entity);
        }

        public ActionResult Update(String uid)
        {
            return View("Card", dbr.ResKey.FindByKey(uid));
        }

        [HttpPost]
        public ActionResult Update(String uid, ResKeyRule.Entity Entity)
        {
            return LangResBiz.Update(Entity);
        }

    }
}
