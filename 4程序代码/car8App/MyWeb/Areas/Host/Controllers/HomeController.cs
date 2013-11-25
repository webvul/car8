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
    public class HomeController : MyMvcController
    {
        public ActionResult Test()
        {
            var s = new AutoResetEvent(false);
            try
            {
                using (var scope = new TransactionScope())
                {
                    dbr.PowerController.Update(o => o.Area == "udi" & o.Controller == "test").Where(o=>o.Id == 20).Execute();

                    Thread.Sleep(1000);


                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return Content("OK");
        }

        public ActionResult UserValidate()
        {
            return Content(Request.QueryString["key"]);
        }

        public ActionResult UserLogin()
        {
            return View("Login");
        }

        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Logout()
        {
            MySession.LogOut();
            return new JsonMsg();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Prove(FormCollection collectoin)
        {
            if (MySession.Login(collectoin["UserName"], collectoin["Password"]))
            {
                return new JsonMsg();
            }
            else
            {
                MySession.LogOut();
                return new JsonMsg() { msg = "登录失败,请重新登录." };
            }
        }
    }
}
