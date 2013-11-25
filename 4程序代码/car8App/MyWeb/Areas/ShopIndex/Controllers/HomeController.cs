using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCon;
using MyCmn;

namespace MyWeb.Areas.ShopIndex.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /ShopIndex/Home/

        public ActionResult Index(string uid)
        {
            //return RedirectToRoute(new { area="Shop", controller = "Home", action = "Index", uid = uid, Lang = MyHelper.RequestContext.RouteData.Values["Lang"], page = 1 });

            if (MyHelper.RequestContext.RouteData.Values["Lang"].AsString() == "En")
            {
                Server.Execute(("~/Html/Index/" + uid + "/En.html").GetUrlFull());
            }
            else Server.Execute(("~/Html/Index/" + uid + ".html").GetUrlFull());

            return Content("");
        }

    }
}
