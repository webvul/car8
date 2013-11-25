using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Text;
using MyCmn;
using System.Web.Routing;
using System.ComponentModel;
using System.Threading;


namespace System.Web.Mvc
{
    public static partial class MyHelper
    {
        public static LangEnum Lang
        {
            get
            {
                string lang = string.Empty;

                if (HttpContext.Current != null && HttpContext.Current.Request.QueryString.AllKeys.Contains("Lang"))
                {
                    lang = HttpContext.Current.Request.QueryString["Lang"];
                    return lang.ToEnum(LangEnum.Zh);
                }

                if (RequestContext != null)
                {
                    lang = RequestContext.RouteData.Values["Lang"].AsString();
                    return lang.ToEnum(LangEnum.Zh);
                }

                if (HttpContext.Current != null)
                {
                    lang = HttpContext.Current.Request.Headers["Accept-Language"];
                    if (lang.HasValue())
                    {
                        //zh-CN,zh;q=0.8
                        var sect = lang.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        var whiloeWord = sect.FirstOrDefault(o => o.All(c => char.IsLetter(c)));
                        if (whiloeWord != null) return whiloeWord.ToEnum(LangEnum.Zh);
                    }
                }

                return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(
                    lang.AsString(Thread.CurrentThread.CurrentCulture.TextInfo.CultureName.Split('-')[0])
                    ).ToEnum<LangEnum>(LangEnum.Zh);
            }
        }

        /// <summary>
        /// 判断请求是否是 Ajax 请求。 只针对 Jquery.
        /// </summary>
        public static bool RequestIsAjax
        {
            get
            {
                if (HttpContext.Current == null) return false;

                return HttpContext.Current.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            }
        }

        public static RequestContext RequestContext
        {
            get
            {
                if (HttpContext.Current == null) return null;
                var handler = HttpContext.Current.Handler as MvcHandler;
                if (handler == null) return null;
                return handler.RequestContext;
            }
        }

        /// <summary>
        /// 取MVC中的 Action
        /// </summary>
        public static string Action
        {
            get
            {
                if (RequestContext == null) return null;
                if (RequestContext.RouteData.Values.Keys.Contains("action"))
                    return RequestContext.RouteData.Values["action"].AsString();
                else return string.Empty;
            }
        }

        /// <summary>
        /// Mvc 中的Controller。
        /// </summary>
        public static string Controller
        {
            get
            {
                if (RequestContext == null) return null;
                if (RequestContext.RouteData.Values.Keys.Contains("controller"))
                    return RequestContext.RouteData.Values["controller"].AsString();
                else return string.Empty;
            }
        }

        public static string Area
        {
            get
            {
                if (RequestContext == null) return null;
                if (RequestContext.RouteData.Values.Keys.Contains("area"))
                    return RequestContext.RouteData.Values["area"].AsString();
                else return string.Empty;
            }
        }

        public static string BeginTag(this HtmlHelper html, HtmlTextWriterTag tag, object Attributes)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(Attributes))
            {
                object val = item.GetValue(Attributes);
                dict[item.Name] = val == null ? null : val.AsString();
            }
            return BeginTag(html, tag, dict);
        }
        public static string BeginTag(this HtmlHelper html, HtmlTextWriterTag tag, Dictionary<string, string> Attributes)
        {
            HtmlTagNode nd = new HtmlTagNode();
            nd.TagName = tag.ToString();
            if (tag.IsIn(HtmlTextWriterTag.Br, HtmlTextWriterTag.Img, HtmlTextWriterTag.Input, HtmlTextWriterTag.Link,
                HtmlTextWriterTag.Nobr, HtmlTextWriterTag.Meta)
                )
            {
                nd.IsSole = true;
            }
            else nd.IsSole = false;

            if (Attributes != null)
            {
                foreach (var item in Attributes)
                {
                    HtmlAttrNode atr = new HtmlAttrNode();
                    atr.Name = item.Key;
                    string val = item.Value.AsString();
                    if (val.HasValue())
                    {
                        atr.IsSole = false;
                        atr.Value = val;
                    }
                    else atr.IsSole = true;

                    nd.Attrs.Add(atr);
                }
            }

            return nd.ToString();
        }


        public static string EndTag(this HtmlHelper html, HtmlTextWriterTag tag)
        {
            if (tag.IsIn(HtmlTextWriterTag.Br, HtmlTextWriterTag.Img, HtmlTextWriterTag.Input, HtmlTextWriterTag.Link,
                HtmlTextWriterTag.Nobr, HtmlTextWriterTag.Meta)
                )
            {
                return "";
            }
            return new HtmlCloseTagNode() { TagName = tag.ToString() }.ToString();
        }

        /// <summary>
        /// 如果当前语言是英文,则显示英文值,否则显示中文值.
        /// </summary>
        /// <remarks>
        /// 只支持中英文.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="Lang"></param>
        /// <param name="ZHRes"></param>
        /// <param name="EnRes"></param>
        /// <returns></returns>
        public static T GetRes<T>(this LangEnum Lang, T ZHRes, T EnRes)
        {
            if (MyHelper.Lang == LangEnum.En) return EnRes;

            return ZHRes;
        }



        private static object _syncObject = new object();
        private static IRes GetResEvent = null;
        private static bool FindResEvent = true;
        public static StringLinker GetRes(this string Key)
        {
            if (GetResEvent == null && !FindResEvent)
            {
                lock (_syncObject)
                {
                    if (GetResEvent == null)
                    {
                        GetResEvent = Activator.CreateInstance(Type.GetType(Configuration.WebConfigurationManager.AppSettings["ResEvent"])) as IRes;
                        FindResEvent = true;
                    }

                }
            }

            if (GetResEvent != null)
            {
                return GetResEvent.GetRes(Key);
            }
            else return Key;
        }

        /// <summary>
        ///  得到 Control 的 HTML 解析部分. [★]
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public static string ToRenderString(this Control con)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter strWrit = new StringWriter(sb);
            HtmlTextWriter htmlWrit = new HtmlTextWriter(strWrit);

            con.RenderControl(htmlWrit);
            return sb.ToString();
        }

    }
}
