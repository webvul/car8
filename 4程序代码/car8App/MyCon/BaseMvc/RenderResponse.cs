
using System;
using System.Web;
using System.Web.Mvc;

namespace MyCon
{
    public class RenderResponse
    {
        public RenderResponse()
        {
        }

        /// <summary>
        /// 输出文本
        /// </summary>
        /// <param name="StringOne"></param>
        /// <param name="AddString"></param>
        /// <returns></returns>
        public static RenderResponse operator +(RenderResponse StringOne, string AddString)
        {
            HttpContext.Current.Response.Write(AddString);
            return StringOne;
        }

        /// <summary>
        /// 输出Html
        /// </summary>
        /// <param name="StringOne"></param>
        /// <param name="AddString"></param>
        /// <returns></returns>
        public static RenderResponse operator +(RenderResponse StringOne, MvcHtmlString AddString)
        {
            HttpContext.Current.Response.Write(AddString.ToHtmlString());
            return StringOne;
        }

        ///// <summary>
        ///// 输出脚本. 
        ///// </summary>
        ///// <param name="StringOne"></param>
        ///// <param name="JsString"></param>
        ///// <returns></returns>
        //public static RenderResponse operator %(RenderResponse StringOne, string JsString)
        //{
        //    StringOne.RegisterStartupScript(Guid.NewGuid().ToString(), JsString);
        //    return StringOne;
        //}
    }
}
