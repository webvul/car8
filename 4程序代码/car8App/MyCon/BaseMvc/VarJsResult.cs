using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MyCmn;
using System.Web;
using System.Linq;
using MyBiz;

namespace MyCon
{
    public enum JsType
    {
        /// <summary>
        /// 输出前后带有多引号的文本。
        /// </summary>
        String,

        /// <summary>
        /// 直接输出文本内容。
        /// </summary>
        Value,

        /// <summary>
        /// 直接输出文本内容。 同 Value 
        /// </summary>
        Script,
    }

    public class MyFakeController : ControllerContext
    {
        public class MyController : ControllerBase
        {
            protected override void ExecuteCore()
            {
                throw new NotImplementedException();
            }
        }
        public class MyReponse : HttpResponseBase
        {
            private string contextType = string.Empty;
            public override string ContentType
            {
                get
                {
                    return contextType;
                }
                set
                {
                    contextType = value;
                }
            }
            public StringLinker Content = new StringLinker();
            public override void Write(char ch)
            {
                Content += ch;
            }

            public override void Write(string s)
            {
                Content += s;
            }

            public override void Write(char[] buffer, int index, int count)
            {
                Content += buffer.Slice(index, index + count);
            }
        }
        public class MyContext : HttpContextBase
        {
            public override HttpResponseBase Response
            {
                get
                {
                    return response;
                }
            }

            private MyReponse response = new MyReponse();
        }

        public override bool IsChildAction
        {
            get
            {
                return true;
            }
        }

        private MyController controller = new MyController();
        private System.Web.Routing.RouteData route = new System.Web.Routing.RouteData();
        private MyContext context = new MyContext();
        public MyFakeController()
        {
        }
        public override ControllerBase Controller
        {
            get
            {
                return controller;
            }
            set
            {
                controller = (MyController)value;
            }
        }

        public override HttpContextBase HttpContext
        {
            get
            {
                return context;
            }
            set
            {
                context = (MyContext)value;
            }
        }

        public override System.Web.Routing.RouteData RouteData
        {
            get
            {
                return route;
            }
            set
            {
                route = value;
            }
        }
    }

    public class VarJsResult
    {
        public JsType Type { get; set; }
        public string Content { get; set; }
        public ActionResult Action { get; set; }

        public override string ToString()
        {
            if (Type == JsType.String)
            {
                return string.Format(@"""{0}""", Content);
            }
            return Content;
        }

        public VarJsResult(int value)
        {
            this.Type = JsType.Value;
            Content = value.AsString();
        }

        public VarJsResult(string value)
        {
            this.Type = JsType.String;
            Content = value;
        }

        public VarJsResult(bool value)
        {
            this.Type = JsType.Value;
            Content = value.AsString().ToLower();
        }

        public VarJsResult(ActionResult action)
        {
            this.Type = JsType.Value;
            var con = new MyFakeController();
            action.ExecuteResult(con);
            this.Content = (con.HttpContext.Response as MyCon.MyFakeController.MyReponse).Content;
        }
    }
}
