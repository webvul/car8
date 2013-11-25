using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MyCmn;
using System.Web;
using MyBiz;
using System.Linq;

namespace MyCon
{
    public class MyHostActionInvoker : ControllerActionInvoker
    {
        public override bool InvokeAction(ControllerContext controllerContext, string actionName)
        {
            return base.InvokeAction(controllerContext, actionName);
        }

    }
    [MyError]
    public class MyHostController : MyMvcController
    {
        protected override IActionInvoker CreateActionInvoker()
        {
            return new MyHostActionInvoker();
        }
        protected override void HandleUnknownAction(string actionName)
        {
            try
            {
                base.HandleUnknownAction(actionName);
            }
            catch (Exception e)
            {
                this.ControllerContext.HttpContext.Response.StatusCode = 404;
                MyError.ProcException(this.ControllerContext, e);
            }
        }
    }
}
