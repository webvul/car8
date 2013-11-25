using System;
using System.Web.Mvc;
using MyCmn;
using DbEnt;
using MyOql;
using MyCon;
using MyBiz.Admin;
using DbEnt;


namespace MyWeb.Areas.Admin.Controllers
{
    public partial class PowerButtonController : MyMvcController
    {

        public ActionResult List()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Query(string uid, PowerButtonBiz.QueryModel Query)
        {
            var set = PowerButtonBiz.Query(Query);

            return set.LoadFlexiGrid();
        }

        [HttpPost]
        public ActionResult Delete(string uid, FormCollection collection)
        {
            string[] delIds = collection["query"].Split(',');
            return PowerButtonBiz.Delete(delIds);
        }

        public ActionResult Detail(Int32 uid)
        {
            return View("Card", dbr.PowerButton.FindById(uid));
        }
        public ActionResult Add(string uid)
        {
            return View("Card", new PowerButtonRule.Entity());
        }

        [HttpPost]
        public ActionResult Add(string uid, PowerButtonRule.Entity Entity)
        {
            return PowerButtonBiz.Add(Entity);
        }

        public ActionResult Update(Int32 uid)
        {
            return View("Card", dbr.PowerButton.FindById(uid));
        }

        [HttpPost]
        public ActionResult Update(Int32 uid, PowerButtonRule.Entity Entity)
        {
            return PowerButtonBiz.Update(Entity);
        }

    }
}
