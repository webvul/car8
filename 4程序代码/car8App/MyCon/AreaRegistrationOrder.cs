using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MyCon
{
    /// <summary>
    /// 根据顺序注册Areas
    /// </summary>
    /// <remarks>
    /// 参见:http://www.cnblogs.com/dozer/archive/2010/04/14/change-order-of-MVC-Areas.html
    /// <code>
    ///     //AreaRegistration.RegisterAllAreas();  这是原来的
    ///     AreaRegistrationOrder.RegisterAllAreasOrder();  //换成这个
    ///     RegisterRoutes(RouteTable.Routes);
    ///     
    ///   public class AdminAreaRegistration : MvcApplication2.AreaRegistrationOrder
    ///   {
    ///       public override string AreaName
    ///       {
    ///           get
    ///           {
    ///               return "Admin";
    ///           }
    ///       }
    ///   
    ///       public override int Order
    ///       {
    ///           get
    ///           {
    ///               return 1;
    ///           }
    ///       }
    ///   
    ///       public override void RegisterAreaOrder(AreaRegistrationContext context)
    ///       {
    ///           context.MapRoute(
    ///               "Admin_default",
    ///               "Admin/{controller}/{action}/{id}",
    ///               new { controller = "Home", action = "Index", id = UrlParameter.Optional }
    ///           );
    ///       }
    ///   }
    /// </code>
    /// </remarks>
    abstract public class AreaRegistrationOrder : AreaRegistration
    {
        /// <summary>
        /// 存放AreaContent
        /// </summary>
        protected static List<AreaRegistrationContext> areaContent = new List<AreaRegistrationContext>();

        /// <summary>
        /// 存放AreaRegistration
        /// </summary>
        protected static List<AreaRegistrationOrder> areaRegistration = new List<AreaRegistrationOrder>();

        /// <summary>
        /// 劫持
        /// </summary>
        /// <param name="context"></param>
        public override void RegisterArea(AreaRegistrationContext context)
        {
            areaContent.Add(context);
            areaRegistration.Add(this);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="context"></param>
        public abstract void RegisterAreaOrder(AreaRegistrationContext context);

        /// <summary>
        /// 顺序
        /// </summary>
        public abstract int Order { get; }

        /// <summary>
        /// 按照顺序注册Areas , 调用该方法,而不要调用 RegisterAllAreas!!
        /// </summary>
        public static void RegisterAllAreasOrder()
        {
            RegisterAllAreas();
            Register();
        }

        /// <summary>
        /// 注册
        /// </summary>
        private static void Register()
        {
            List<int[]> order = new List<int[]>();
            for (int k = 0; k < areaRegistration.Count; k++)
            {
                order.Add(new int[] { areaRegistration[k].Order, k });
            }
            order = order.OrderBy(o => o[0]).ToList();
            foreach (var o in order)
            {
                areaRegistration[o[1]].RegisterAreaOrder(areaContent[o[1]]);
            }
        }
    }
}
