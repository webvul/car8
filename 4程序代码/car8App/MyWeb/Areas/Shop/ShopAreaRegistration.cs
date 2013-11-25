using System.Web.Mvc;
using MyCon;

namespace MyWeb.Areas.Shop
{
    public class ShopAreaRegistration : AreaRegistrationOrder
    {
        public override string AreaName
        {
            get
            {
                return "Shop";
            }
        }


        public override int Order
        {
            get
            {
                return 5;
            }
        }

        public override void RegisterAreaOrder(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Shop_products",
                "Shop/Products/{WebName}.aspx",
                new { area = AreaName, controller = "Home", action = "Products", WebName = "", Lang = "Zh", page = 0 }
            );

            context.MapRoute(
                "Shop_zhpage",
                "Shop/Products/{WebName}/{page}.aspx",
                new { area = AreaName, controller = "Home", action = "Products", WebName = "", Lang = "Zh", page = 1 },
                new { page = @"\d+" }
            );

            context.MapRoute(
                "Shop_ProductInfo",
                "Shop/ProductInfo/{WebName}/{id}.aspx",
                new { area = AreaName, controller = "Home", action = "ProductInfo", WebName = "", Lang = "Zh", id = 0 },
                new { id = @"\d+" }
            );

            context.MapRoute(
                "Shop_Notices",
                "Shop/Notice/{WebName}.aspx",
                new { area = AreaName, controller = "Home", action = "Notice", WebName = "", Lang = "Zh", page = 0 }
            );

            context.MapRoute(
                "Shop_NoticePage",
                "Shop/Notice/{WebName}/{page}.aspx",
                new { area = AreaName, controller = "Home", action = "Notice", WebName = "", Lang = "Zh", page = 1 },
                new { page = @"\d+" }
            );

            context.MapRoute(
                "Shop_NoticeInfo",
                "Shop/NoticeInfo/{WebName}/{id}.aspx",
                new { area = AreaName, controller = "Home", action = "NoticeInfo", WebName = "", Lang = "Zh", id = 0 },
                new { id = @"\d+" }
            );

            context.MapRoute(
                "Shop_Zh",
                "Shop/{action}/{WebName}.aspx",
                new { area = AreaName, controller = "Home", action = "Index", WebName = "", Lang = "Zh", page = 1 }
            );
        }
    }
}
