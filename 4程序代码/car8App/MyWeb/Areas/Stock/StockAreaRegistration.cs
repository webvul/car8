using System.Web.Mvc;
using MyCon;

namespace MyWeb.Areas.Stock
{
    public class StockAreaRegistration : AreaRegistrationOrder
    {
        public override string AreaName
        {
            get
            {
                return "Stock";
            }
        }

        public override int Order
        {
            get { return 8; }
        }

        public override void RegisterAreaOrder(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Stock_default",
                "Stock/{controller}/{action}/{uid}",
                new { area = AreaName, action = "Index", uid = "" }
            );
        }
    }
}
