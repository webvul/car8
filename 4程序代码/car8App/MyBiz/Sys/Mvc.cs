using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Threading;
using MyCmn;
using MyOql;

namespace System
{
    public static class Mvc
    {
        static Mvc()
        {
            Model = new ModelContext();
        }

        public static ModelContext Model { get; private set; }



        public class ModelContext
        {
            /// <summary>
            /// 循环使用时请缓存,从 HttpContext.Current.Items, Mvc 的 RouteData,Request.Params , Header 里得到值。
            /// </summary>
            /// <param name="Key"></param>
            /// <returns></returns>
            public string this[string Key]
            {
                get
                {
                    if (HttpContext.Current == null) return string.Empty;

                    if (HttpContext.Current.Items.Contains(Key))
                    {
                        return HttpContext.Current.Items[Key].AsString();
                    }

                    {
                        var mvc = HttpContext.Current.Handler as System.Web.Mvc.MvcHandler;
                        if (mvc != null)
                        {
                            if (mvc.RequestContext.RouteData.Values.ContainsKey(Key))
                            {
                                return mvc.RequestContext.RouteData.Values[Key].AsString();
                            }
                        }
                    }


                    if (HttpContext.Current.Request.Params.AllKeys.Contains(Key))
                    {
                        return HttpContext.Current.Request.Params[Key];
                    }

                    if (HttpContext.Current.Request.Headers.AllKeys.Contains(Key))
                    {
                        return HttpContext.Current.Request.Headers[Key];
                    }

                    //return HttpContext.Current.Session[Key].AsString();

                    return string.Empty;
                }
                set
                {
                    if (HttpContext.Current == null) return;

                    HttpContext.Current.Items[Key] = value;
                }
            }
        }

        public class MyOqlSetJsonResult : ActionResult
        {
            private MyOqlSet set { get; set; }
            public MyOqlSetJsonResult(MyOqlSet Set) { set = Set; }

            public override void ExecuteResult(ControllerContext context)
            {
                HttpResponseBase response = context.HttpContext.Response;
                response.ContentType = "application/json";

                response.Write(this.set.ToJson());
            }
        }

        public static MyOqlSetJsonResult ToJsonResult(this MyOqlSet set)
        {
            return new MyOqlSetJsonResult(set);
        }
    }
}