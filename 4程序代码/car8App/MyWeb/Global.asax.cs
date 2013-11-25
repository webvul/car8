using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Configuration;
using MyOql;
using MyCmn;
using MyCon;
using MyBiz;
using System.Data.Common;
using MyBiz.Sys;

namespace MyWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //var rout = routes.MapRoute("Root", "Index.aspx",
            //    new { area = "Host", controller = "Home", action = "Index", uid = "" }
            //    );//根目录匹配

            //rout.DataTokens["area"] = "Host";
        }

        protected void Application_Start()
        {
            AreaRegistrationOrder.RegisterAllAreasOrder();
            RegisterRoutes(RouteTable.Routes);

            RouteTable.Routes.RouteExistingFiles = true;

            //1025 表示 JoinStr。
            dbo.Polymer.Add((SqlOperator)1025);
            dbo.MyFunction[(SqlOperator)1025] = o =>
            {
                if (o == DatabaseType.SqlServer)
                {
                    return "dbo.JoinStr({0})";
                }
                return string.Empty;
            };

            dbo.MyFunction[(SqlOperator)1026] = o =>
            {
                if (o == DatabaseType.SqlServer)
                {
                    return "dbo.FN_GetDateString({0})";
                }
                return string.Empty;
            };

            dbo.DefaultMyOqlConfig =
                //ReConfigEnum.NoLock | 
                ReConfigEnum.RowNumber;


            dbo.Event = MyOqlEvent.GetInstance();

            EnumHelper.EnumEvent = new MyEnumEvent();

            ModelBinders.Binders.DefaultBinder = new MyModelBinder();


            JsonHelper.AddConverter(new MyOql.MyOqlSetJsonNetConverter());

            MyDate.RenderFormat = date =>
            {
                if (date.HasValue() == false) return string.Empty;
                var sl = "yyyy-MM-dd";

                if (date.Hour != 0 || date.Minute != 0 || date.Second != 0)
                {
                    sl += " HH:mm";

                    if (date.Second != 0)
                    {
                        sl += ":ss";
                    }
                }


                return date.ToString(sl.ToString());
            };
            //测试 URL
            //RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);
        }


        protected void Application_Error(object sender, EventArgs e)
        {
            var error = Server.GetLastError();
            if (error == null) return;

            if (HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session["Error"] = error;
            }

            Log.To("SystemError", error.Message, error.Source, error.StackTrace, 0M);
            //Response.Redirect("~/Error.aspx".GetUrlFull());
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session == null) return;

            MyBizHelper.ResetWebConnection();

            HttpContext.Current.Items["ClientIp"] = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].AsString(
                            HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]).AsString(
                            HttpContext.Current.Request.UserHostAddress);

            //if (MyHelper.RequestIsAjax)
            //{
            //    Log.To("PostInfo", string.Empty, string.Empty, 0M);
            //}
        }
    }
}
