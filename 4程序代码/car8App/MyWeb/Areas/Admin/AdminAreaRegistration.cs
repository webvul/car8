using System.Web.Mvc;
using MyCon;

namespace MyWeb.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistrationOrder
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override int Order
        {
            get { return 5; }
        }

        public override void RegisterAreaOrder(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_NoAction",
                "Admin/{controller}.aspx",
                new { area = AreaName, action = "Index", uid = "" }
            );

            context.MapRoute(
                "Admin_NoId",
                "Admin/{controller}/{action}.aspx",
                new { area = AreaName, action = "Index", uid = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{uid}.aspx",
                new { area = AreaName, action = "Index", uid = UrlParameter.Optional }
            );
        }
    }
}
