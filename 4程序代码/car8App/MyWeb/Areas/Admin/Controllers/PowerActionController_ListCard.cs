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
	public partial class PowerActionController : MyMvcController
	{
        
		public ActionResult List()
		{
			return View();
		}
        
        [HttpPost]
		public ActionResult Query(string uid,PowerActionBiz.QueryModel Query)
		{
            var set =  PowerActionBiz.Query(Query);
            
            //把 set 内容把包成树。 用 WBS




            return set .LoadFlexiGrid();
        }
	 
        [HttpPost]
		public ActionResult Delete(string uid,FormCollection collection)
		{
			string[] delIds = collection["query"].Split(',');
			return PowerActionBiz.Delete( delIds ) ;
		}

		public ActionResult Detail(Int32 uid)
		{
			return View("Card", dbr.PowerAction.FindById(uid));
		}
		public ActionResult Add(string uid)
		{
			return View("Card", new PowerActionRule.Entity());
		}

		[HttpPost]
		public ActionResult Add(string uid,PowerActionRule.Entity Entity)
		{
            return PowerActionBiz.Add(Entity) ;
		}

		public ActionResult Update(Int32 uid)
		{
			return View("Card",dbr.PowerAction.FindById(uid));
		}

		[HttpPost]
		public ActionResult Update(Int32 uid, PowerActionRule.Entity Entity)
		{
            return PowerActionBiz.Update(Entity) ;
		}

    }
}
