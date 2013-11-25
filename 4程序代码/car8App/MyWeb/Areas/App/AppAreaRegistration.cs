using System.Web.Mvc;
using MyCon;

namespace MyWeb.Areas.App
{
    public class AppAreaRegistration : AreaRegistrationOrder
    {
        public override string AreaName
        {
            get
            {
                return "App";
            }
        }

        public override int Order
        {
            get { return 15; }
        }

        public override void RegisterAreaOrder(AreaRegistrationContext context)
        {
            context.MapRoute(
                "App_NoAction",
                "App/{controller}.aspx",
                new { area = AreaName, action = "Index", uid = "" }
            );

            context.MapRoute(
                "App_NoId",
                "App/{controller}/{action}.aspx",
                new { area = AreaName, action = "Index", uid = UrlParameter.Optional }
            );

            context.MapRoute(
                "App_default",
                "App/{controller}/{action}/{uid}.aspx",
                new { area = AreaName, action = "Index", uid = UrlParameter.Optional }
            );
        }
    }
}
