using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCon;

namespace MyWeb.Areas.Admin.Controllers
{
    public class UserController : MyMvcController
    {
        //
        // GET: /Admin/User/

        public ActionResult Login()
        {
            return View();
        }

    }
}
