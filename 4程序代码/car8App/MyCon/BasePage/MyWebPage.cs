using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using MyCmn;
using MyOql;
using System.Xml;
using MyBiz;
using System.Collections;
using System.Web.Mvc;
using System.Web.UI;
using System.Web;

namespace MyCon
{

    public partial class MyWebPage : Page
    {
        public MyWebPage()
        {
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
        }

        public static string GetRequireJs()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            HttpContext.Current.Request.QueryString.AllKeys.All(o =>
            {
                if (o.HasValue() == false) return true;
                dict[o] = HttpContext.Current.Request.QueryString[o];
                return true;
            });


            return string.Format(@"jv.Root=""~/"";var _ ={{}};_.href=""{2}"";_.Keys=Array();{0}{1}jv.AddRequest(_);",
               string.Join(@"", dict.Keys.Select(o => string.Format(@"_.Keys.push(""{0}"");", o)).ToArray()),
               string.Join(@"", dict.Keys.Select(o => string.Format(@"_[""{0}""]=""{1}"";", o, dict[o])).ToArray()),
               HttpContext.Current.Request.RawUrl
               ) + "jv.MyOnInit();";
        }

        /// <summary>
        /// 添加  Res/MyJs.js 和 Res/MyCss.css
        /// </summary>
        /// <returns></returns>
        public virtual List<HtmlNode> GetInitJs()
        {
            List<HtmlNode> retVal = new List<HtmlNode>();
            retVal.AddRange(MyMvcPage.GetUsingFile("~/Res/MyCss.css"));
            retVal.AddRange(MyMvcPage.GetUsingFile("~/Res/MyJs.js"));

            return retVal;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            Response.Filter = new ResponseFilter(Response.Filter,
                OriHtml =>
                {
                    //保证页面引用的JS 优先级高。
                    //顺序： 1.先引用 CSS 等非JS资源。
                    //       2. 引用 必要的JS 库， Jquery 等。
                    //       3. 引用 分隔JS ， 分隔Js 以后， 就没有顺序之分了。
                    //          3.1JS之后的第一个是 主题文件JS 。
                    //       4. 其它动态注册的 JS 。
                    //       5. 最后是 页面注册的JS 。
                    int InsertIndex = MyMvcPage.GetBeginNodeIndex(OriHtml);

                    var AddNodes = new List<HtmlNode>();

                    AddNodes.AddRange(GetInitJs());
                    AddNodes.AddRange(MyMvcPage.CreateJsNode(GetRequireJs()));
                    AddNodes.AddRange(MyMvcPage.TakeThemesAndRemoveIt(OriHtml));

                    OriHtml.InsertRange(InsertIndex, AddNodes);
                },
            null);
        }
    }
}