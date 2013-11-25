using System.Web.Mvc;
using MyCon;

namespace MyWeb.Areas.ShopIndex
{
    public class ShopIndexAreaRegistration : AreaRegistrationOrder
    {
        public override string AreaName
        {
            get
            {
                return "ShopIndex";
            }
        }


        public override int Order
        {
            get
            {
                return 4;
            }
        }

        public override void RegisterAreaOrder(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ShopIndex_Index",
                "{uid}.aspx",
                new { area = AreaName, controller = "Home", action = "Index", uid = "", Lang = "Zh", page = 1 },
                new { uid = "^(?!^Index$).*$" }
            );
            context.MapRoute(
                "ShopIndex_En_Index",
                "{uid}/En.aspx",
                new { area = AreaName, controller = "Home", action = "Index", uid = "", Lang = "En", page = 1 },
                new { uid = "^(?!^Index$).*$" }
            );

        }
    }
}
