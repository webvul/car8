using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using MyCmn;

namespace MyCon
{
    /// <summary>
    /// 返回的 结果是 不能执行的操作. 
    /// 如返回 button[] = ["(修改)","#benQuery"] 意味着不能执行该操作.
    /// </summary>
    public class PowerResult : ActionResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.Write(this.ToString());
        }

        public override string ToString()
        {
            return this.ToJson();
        }

        public string msg { get; set; }

        /// <summary>
        /// Grid,Card 用。
        /// </summary>
        public string[] button { get; set; }

        /// <summary>
        /// Card 用。
        /// </summary>
        public string[] edit { get; set; }

        /// <summary>
        /// Grid Card 用。
        /// </summary>
        public string[] view { get; set; }
    }
}
