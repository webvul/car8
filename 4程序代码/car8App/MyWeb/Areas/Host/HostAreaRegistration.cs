using System.Web.Mvc;
using MyCon;

namespace MyWeb.Areas.Host
{
    public class HostAreaRegistration : AreaRegistrationOrder
    {
        public override string AreaName
        {
            get
            {
                return "Host";
            }
        }

        public override int Order
        {
            get { return 1; }
        }

        public override void RegisterAreaOrder(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Host_Login",
                "Login.aspx",
                new { area = AreaName, controller = "Home", action = "Login", uid = UrlParameter.Optional }
            );

            context.MapRoute(
                "Host_Index",
                "Comm/{CommId}/{action}.aspx",
                new { area = AreaName, controller = "Comm", action = "Index" },
                new { CommId = @"[\d]{6}" }
            );

            context.MapRoute(
                "Host_NoAction",
                "Host/{controller}.aspx",
                new { area = AreaName, action = "Index", uid = UrlParameter.Optional }
            );

            context.MapRoute(
                "Host_NoId",
                "Host/{controller}/{action}.aspx",
                new { area = AreaName, action = "Index", uid = UrlParameter.Optional }
            );


            context.MapRoute(
                "Host_default",
                "Host/{controller}/{action}/{uid}.aspx",
                new { area = AreaName, action = "Index", uid = UrlParameter.Optional }
            );
        }
    }
}
