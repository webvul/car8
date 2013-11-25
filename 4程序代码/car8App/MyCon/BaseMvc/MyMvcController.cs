using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MyBiz;
using MyCmn;
using MyOql;
using System.Web;
using DbEnt;
using System.Web.UI;



namespace MyCon
{
    public class MyActionInvoker : ControllerActionInvoker
    {

        /// <summary>
        /// 默认情况下启用权限过滤. 在Post(Ajax,表单提交) 或 Controller 继承了 IIgnorePower, 将忽略权限过滤. 
        /// 另外, 如果 Action 上启用了 IgnorePower 的 过滤器,也将忽略权限过滤.
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public override bool InvokeAction(ControllerContext controllerContext, string actionName)
        {
            string msg = "";
            NoPowerFilter noPower = null;
            var hasPower = SetPowerAndLog(controllerContext, actionName, ref msg, ref noPower);


            if (hasPower == false && msg.HasValue())
            {
                if (MyHelper.RequestIsAjax)
                {
                    JsonMsg jm = new JsonMsg();
                    jm.msg = msg;
                    jm.data = "power";
                    jm.ExecuteResult(controllerContext);
                }
                else
                {
                    ContentResult cr = new ContentResult();
                    cr.Content = msg;

                    cr.ExecuteResult(controllerContext);
                }
                return true;
            }

            SetHistory();

            try
            {
                if (noPower != null)
                {
                    using (var sope = new MyOqlConfigScope(ReConfigEnum.SkipPower))
                    {
                        return base.InvokeAction(controllerContext, actionName);
                    }
                }
                else
                {
                    return base.InvokeAction(controllerContext, actionName);
                }
            }
            catch (Exception e)
            {
                if (HttpContext.Current.Request.HttpMethod.Equals("POST", StringComparison.CurrentCultureIgnoreCase))
                {      
                    e.ToLog();

                    new JsonMsg() { msg = e.Message.GetSafeValue() }.ExecuteResult(controllerContext);
                    //HttpContext.Current.Response.StatusCode = 500;
                    return true;
                }
                else
                {
                    throw;
                }
            }
        }

        private bool SetPowerAndLog(ControllerContext controllerContext, string actionName, ref string msg, ref NoPowerFilter noPower)
        {
            //如果是服务，则跳过。
            if (HttpContext.Current == null) return true;

            //如果是 Ajax Post ，则不判断。
            if (HttpContext.Current.Request.HttpMethod.Equals("POST", StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }

            bool controllerHasNoPowerAttribute = false;
            bool actionHasNoPowerAttribute = false;


            ControllerDescriptor controllerDescriptor = GetControllerDescriptor(controllerContext);
            {
                var atrs = controllerDescriptor.GetCustomAttributes(false);
                if (atrs != null && atrs.Length > 0)
                {
                    noPower = atrs.FirstOrDefault(o => (o as NoPowerFilter) != null) as NoPowerFilter;
                    controllerHasNoPowerAttribute = noPower != null;
                }
            }

            if (controllerHasNoPowerAttribute == false)
            {
                ActionDescriptor actionDescriptor = FindAction(controllerContext, controllerDescriptor, actionName);

                if (actionDescriptor != null)
                {
                    var atrs = actionDescriptor.GetCustomAttributes(false);
                    if (atrs != null && atrs.Length > 0)
                    {
                        noPower = atrs.FirstOrDefault(o => (o as NoPowerFilter) != null) as NoPowerFilter;
                        actionHasNoPowerAttribute = noPower != null;
                    }
                }
            }

            if (controllerHasNoPowerAttribute || actionHasNoPowerAttribute)
            {
                return true;
                //LogInfo();
            }
            return PowerTest(controllerContext, actionName, ref msg);
        }

        private static void SetHistory()
        {
            //暂时关闭。udi 2012年11月10日
            return;

            if (MyHelper.RequestIsAjax == false && HttpContext.Current.Request.QueryString["History"].AsBool())
            {
                FixQueue<string> history = new FixQueue<string>(24);
                if (HttpContext.Current.Request.Cookies["History"] != null)
                {
                    try
                    {
                        HttpUtility.UrlDecode(
                            HttpContext.Current.Request.Cookies["History"].Value
                            )
                         .FromJson<List<string>>().All(o =>
                         {
                             history.Enqueue(o);
                             return true;
                         });
                    }
                    catch
                    {
                    }
                }


                if (history.Count == 0 || history.Last() != HttpContext.Current.Request.Url.ToString())
                {
                    history.Enqueue(HttpContext.Current.Request.Url.ToString());

                    var cookie = new HttpCookie("History", history.ToJson());
                    cookie.Expires = DateTime.Now.AddDays(1);
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }
        }


        private bool PowerTest(ControllerContext controllerContext, string actionName, ref string msg)
        {
            var hasPower = true;

            string area = controllerContext.RouteData.DataTokens.GetOrDefault(o => o.ToLower() == "area").AsString();
            string controller = controllerContext.RouteData.Values.GetOrDefault(o => o.ToLower() == "controller").AsString();

            uint act = dbr.PowerAction
                .Select(o => o.Id)
                .Join(dbr.PowerController, (a, b) => a.ControllerID == b.Id)
                .Where(o => o.Action == actionName & dbr.PowerController.Controller == controller & dbr.PowerController.Area == area)
                .SkipPower()
                .SkipLog()
                .ToScalar()
                .AsUInt();

            if (act > 0)
            {
                if (MySession.GetMyPower() == null)
                {
                    hasPower = false;
                    msg = @"<a href='#' onclick=""top.location='" + "~/Login.aspx".GetUrlFull() + @"'"" >请重新登录！</a>";
                }
                else
                {
                    if ((MySession.GetMyPower().Action & MyBigInt.CreateByBitPositon(act)) == MyBigInt.Zero)
                    {
                        hasPower = false;
                        msg = "你无权查看该页面!";
                    }
                }
            }

            return hasPower;
        }
    }
    public class MyMvcController : Controller
    {
        public class MyJsonResult : ActionResult
        {
            public object data { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                HttpResponseBase response = context.HttpContext.Response;
                response.ContentType = "application/json";
                if (this.data.IsDBNull() == false && this.data.GetType().IsNumberType())
                {
                    //修复Js精度。
                    if (this.data.AsDecimal() > int.MaxValue)
                    {
                        this.data = this.data.AsString();
                    }
                }

                response.Write(this.data.ToJson());
            }
        }

        public new MyJsonResult Json(object data)
        {
            return new MyJsonResult { data = data };
        }

        /// <summary>
        /// 取客户端  Post 过来的  Array 数据。
        /// </summary>
        /// <param name="RequestKey"></param>
        /// <returns></returns>
        [Obsolete("已修改jQuery序列化方法，以适应Mvc接收 Array 参数")]
        public string[] GetRequestValue(string RequestKey)
        {
            List<string> list = new List<string>();

            if (HttpContext.Request.Form.AllKeys.Count(o => o == RequestKey) > 0)
            {
                list.Add(HttpContext.Request.Form[RequestKey]);
            }

            foreach (var item in HttpContext.Request.Form.AllKeys.Where(o => o.StartsWith(RequestKey + "[") && o.EndsWith("]")))
            {
                list.Add(HttpContext.Request.Form[item]);
            }
            return list.ToArray();
        }



        //internal Dictionary<string, VarJsResult> Vars { get; set; }
        public void RenderJsVar(string VarName, string Value)
        {
            GodError.Check(HttpContext.Request.QueryString.AllKeys.Contains(VarName), "Url 中已存在该项 : " + VarName);
            GodError.Check(MyHelper.RequestContext.RouteData.DataTokens.ContainsKey(VarName), "路由中已存在该项 : " + VarName);

            this.RenderJss[VarName] = "jv.page()." + VarName + @"=" + new VarJsResult(Value).ToString() + @";";
        }

        /// <summary>
        /// 注入JS执行脚本。
        /// </summary>
        /// <param name="VarName"></param>
        /// <param name="OriJs"></param>
        public void RenderOriJs(string VarName, string OriJs)
        {
            GodError.Check(HttpContext.Request.QueryString.AllKeys.Contains(VarName), "Url 中已存在该项 : " + VarName);
            GodError.Check(MyHelper.RequestContext.RouteData.DataTokens.ContainsKey(VarName), "路由中已存在该项 : " + VarName);

            this.RenderJss[VarName] = OriJs;
        }

        public void RenderJsVar(string VarName, int Value)
        {
            GodError.Check(HttpContext.Request.QueryString.AllKeys.Contains(VarName), "Url 中已存在该项 : " + VarName);
            GodError.Check(MyHelper.RequestContext.RouteData.DataTokens.ContainsKey(VarName), "路由中已存在该项 : " + VarName);

            this.RenderJss[VarName] = "jv.page()." + VarName + @"=" + new VarJsResult(Value).ToString() + @";";
        }
        public void RenderJsVar(string VarName, bool Value)
        {
            GodError.Check(HttpContext.Request.QueryString.AllKeys.Contains(VarName), "Url 中已存在该项 : " + VarName);
            GodError.Check(MyHelper.RequestContext.RouteData.DataTokens.ContainsKey(VarName), "路由中已存在该项 : " + VarName);

            this.RenderJss[VarName] = "jv.page()." + VarName + @"=" + new VarJsResult(Value).ToString() + @";";
        }

        public void RenderJsVar(string VarName, VarJsResult Value)
        {
            GodError.Check(HttpContext.Request.QueryString.AllKeys.Contains(VarName), "Url 中已存在该项 : " + VarName);
            GodError.Check(MyHelper.RequestContext.RouteData.DataTokens.ContainsKey(VarName), "路由中已存在该项 : " + VarName);

            this.RenderJss[VarName] = "jv.page()." + VarName + @"=" + Value.ToString() + @";";
        }

        public void RenderJsVar(string VarName, ActionResult Value)
        {
            GodError.Check(HttpContext.Request.QueryString.AllKeys.Contains(VarName), "Url 中已存在该项 : " + VarName);
            GodError.Check(MyHelper.RequestContext.RouteData.DataTokens.ContainsKey(VarName), "路由中已存在该项 : " + VarName);

            this.RenderJss[VarName] = "jv.page()." + VarName + @"=" + new VarJsResult(Value).ToString() + @";";
        }


        protected override IActionInvoker CreateActionInvoker()
        {
            return new MyActionInvoker();
        }
        protected override void HandleUnknownAction(string actionName)
        {
            this.View(actionName).ExecuteResult(this.ControllerContext);
            //base.HandleUnknownAction(actionName);
        }


        /// <summary>
        /// 设置页面禁止缓存.
        /// </summary>
        public void SetPageNoCache()
        {
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            this.Response.Cache.SetExpires(DateTime.Today.AddDays(-1));
            this.Response.Expires = 0;
            this.Response.Headers["pragma"] = "no-cache";
            this.Response.Headers["Cache-Control"] = "no-store, must-revalidate";
        }

        public MyMvcController()
            : base()
        {
            RenderJss = new StringDict();
            FunctionJss = new StringDict();

            RegisterStartupScript("_", MyMvcPage.GetRequestJs());

        }

        /// <summary>
        /// 输出到客户端的Js代码，不带标签。
        /// </summary>
        public StringDict RenderJss { get; set; }

        /// <summary>
        /// 定义的客户端的 Js 函数。
        /// </summary>
        public StringDict FunctionJss { get; set; }



        public void RegisterStartupScript(string key, string script)
        {
            if (key.HasValue() && script.HasValue())
            {
                RenderJss[key] = script;
            }
        }
        public void RegisterJsFunction(string key, string script)
        {
            if (key.HasValue() && script.HasValue())
            {
                FunctionJss[key] = script;
            }
        }


    }
}
