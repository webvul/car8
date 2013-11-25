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

namespace MyWeb.Areas.Host.Controllers
{
    public class SSOController : Controller
    {
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
    }
}
