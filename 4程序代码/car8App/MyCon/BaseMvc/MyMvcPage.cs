using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Mvc.Ajax;
using System.Web.UI;
using System.Text;
using MyCmn;
using System.Text.RegularExpressions;
using MyBiz;
using MyOql;
using DbEnt;


namespace MyCon
{
    public partial class MyMvcPage : ViewPage
    {
        /// <summary>
        /// 以 add 开头或结尾，就认为是　添加页面。
        /// </summary>
        public bool ActionIsAdd
        {
            get
            {
                return this.Action.StartsWith("add", StringComparison.CurrentCultureIgnoreCase) || this.Action.EndsWith("add", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        /// <summary>
        /// 以 update 开头或结尾　，就认为是　更新页面
        /// </summary>
        public bool ActionIsUpdate
        {
            get
            {
                return this.Action.StartsWith("update", StringComparison.CurrentCultureIgnoreCase) || this.Action.EndsWith("update", StringComparison.CurrentCultureIgnoreCase);
            }
        }


        /// <summary>
        /// Html 文档中 meta 元素，应该保持在最前面
        /// </summary>
        protected List<HtmlNode> MetaNodes { get; set; }

        /// <summary>
        /// 定义的引用资源文件。
        /// </summary>
        public StringDict ResFiles { get; set; }


        public MyMvcPage()
        {
            MetaNodes = new List<HtmlNode>();
            ResFiles = new StringDict();
        }


        /// <summary>
        /// MVC 先执行Controller 后执行Action，最后执行View ，把要注册的资源先放到 Controller 类中。
        /// </summary>
        /// <remarks> 在执行 Controller 的 Action 时 </remarks>
        /// <param name="Key"></param>
        /// <param name="scriptOrCssFile"></param>
        public void RegisterUsingFile(string Key, string scriptOrCssFile)
        {
            if (Key.HasValue() && scriptOrCssFile.HasValue())
            {
                string extFile = scriptOrCssFile.Substring(scriptOrCssFile.LastIndexOf("."));
                ResFiles[Key + extFile] = scriptOrCssFile;
            }
        }

        public override void RenderView(ViewContext viewContext)
        {
            base.RenderView(viewContext);

            MyReRenderView(viewContext, MetaNodes);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }


        /// <summary>
        /// 自定义Html转换器
        /// </summary>
        public static Func<string, MyUrl> MyHtmlTranslater
        {
            get
            {
                return aspxUrl =>
                {
                    var url = new MyUrl(aspxUrl);
                    if (url.Prefix.StartsWith("javascript:", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return url;
                    }

                    if (url.Extension.HasValue() == false ||
                        url.Extension == ".aspx")
                    {
                        url.QuerySegment.All(o =>
                        {
                            if (o.Key.Equals("_", StringComparison.CurrentCultureIgnoreCase)) return true;
                            if (o.Key.Equals("Html", StringComparison.CurrentCultureIgnoreCase)) return true;
                            if (o.Key.Equals("History", StringComparison.CurrentCultureIgnoreCase)) return true;
                            url.PathSegment[url.PathSegment.Length - 1] += "." + o.Key + (o.Value.HasValue() ? "." + o.Value : "");
                            return true;
                        });


                        url.Extension = ".html";

                        url.QuerySegment.Clear();
                        return url;
                    }
                    else return url;
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 保证页面引用的JS 优先级高。 顺序:  
        ///  1.先引用 CSS 等非JS资源。
        ///  2. 引用 必要的JS 库， Jquery 等。
        ///  3. 引用 分隔JS ， 分隔Js 以后， 就没有顺序之分了。
        ///  3.1 JS之后的第一个是 主题文件JS 。
        ///  4. 其它动态注册的 JS 。
        ///  5. 最后是 页面注册的JS 。
        /// </remarks>
        /// <param name="viewContext"></param>
        /// <param name="RenderNodes"></param>
        public void MyReRenderView(ViewContext viewContext, List<HtmlNode> RenderNodes)
        {
            //信息带入
            var con = viewContext.Controller as MyMvcController;

            viewContext.HttpContext.Response.Filter = new ResponseFilter(viewContext.HttpContext.Response.Filter,
                    OriHtml =>
                    {

                        var AddNodes = new List<HtmlNode>();
                        AddNodes.AddRange(RenderNodes);
                        AddNodes.AddRange(GetInitJs());

                        AddNodes.AddRange(CreateJsNode(con.RenderJss.Where(o => o.Key == "_").Select(o => o.Value).FirstOrDefault()));

                        var scriptNodes = CreateJsNode(string.Join("", con.RenderJss.Where(o => o.Key != "_").Select(o => o.Value).ToArray()));
                        if (scriptNodes.Length > 1)
                        {
                            (scriptNodes[1] as HtmlTextNode).Text = string.Join("", con.FunctionJss.Select(o => o.Value).ToArray()) + Environment.NewLine +
                                (scriptNodes[1] as HtmlTextNode).Text;
                        }
                        AddNodes.AddRange(scriptNodes);

                        //添加引用。  
                        foreach (string key in this.ResFiles.Keys)
                        {
                            AddNodes.AddRange(GetUsingFile(this.ResFiles[key]));
                        }

                        AddNodes.AddRange(TakeThemesAndRemoveIt(OriHtml));

                        int InsertIndex = GetBeginNodeIndex(OriHtml);
                        OriHtml.InsertRange(InsertIndex, AddNodes);

                        TakeResToLast(OriHtml);
                    },
                    OriHtml =>
                    {
                        Htmler(OriHtml);
                    });
        }

        /// <summary>
        /// 静态化
        /// </summary>
        /// <param name="OriHtml"></param>
        private static void Htmler(List<HtmlNode> OriHtml)
        {
            //静态化
            if (HttpContext.Current.Request.QueryString["Html"] != "True") return;

            for (int i = 0; i < OriHtml.Count; i++)
            {
                if (OriHtml[i].Type == HtmlNode.NodeType.Tag)
                {
                    HtmlTagNode tn = OriHtml[i] as HtmlTagNode;

                    for (int j = 0; j < tn.Attrs.Count; j++)
                    {
                        HtmlAttrNode an = tn.Attrs[j];
                        if (an.IsSole == true) continue;
                        if (an.Name == "value" &&
                            tn.TagName.IsIn(StringComparer.CurrentCultureIgnoreCase, "input"))
                        {
                            continue;
                        }

                        an.Value = MyHelper.GetHtmlUrl(an.Value, MyHtmlTranslater);
                    }
                }
            }

            var htmlContent = string.Join("", OriHtml.Select(o => o.ToString()).ToArray());
            var fileName = MyHelper.GetHtmlUrl(HttpContext.Current.Request.Url.PathAndQuery, MyHtmlTranslater);
            fileName = HttpContext.Current.Server.MapPath(fileName);
            new FileInfo(fileName).Touch();
            File.WriteAllText(fileName, htmlContent, Encoding.UTF8);
        }

        /// <summary>
        /// 资源后置，把引用的资源放到 html body 的最后部分。
        /// </summary>
        /// <param name="OriHtml"></param>
        private static void TakeResToLast(List<HtmlNode> OriHtml)
        {
            //把所有的Js放到</html>前面。

            var endNodeIndex = 0;
            for (var i = OriHtml.Count - 1; i >= 0; i--)
            {
                var item = OriHtml[i];
                if (item.Type == HtmlNode.NodeType.CloseTag)
                {
                    var node = item as HtmlCloseTagNode;
                    if (string.Equals(node.TagName, "body", StringComparison.CurrentCultureIgnoreCase))
                    {
                        endNodeIndex = i;
                        break;
                    }
                    if (string.Equals(node.TagName, "html", StringComparison.CurrentCultureIgnoreCase))
                    {
                        endNodeIndex = i;
                        continue;
                    }
                }
            }


            if (endNodeIndex >= 0)
            {
                var pos = 0;
                for (var i = 0; i < endNodeIndex - pos; i++)
                {
                    var item = OriHtml[i];
                    if (item.Type == HtmlNode.NodeType.Tag)
                    {
                        var node = item as HtmlTagNode;
                        if (string.Equals("script", node.TagName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (node.IsSole)
                            {
                                OriHtml.Insert(endNodeIndex, OriHtml[i]);
                                OriHtml.RemoveAt(i);

                                i--;
                                pos++;
                                continue;
                            }
                            else if (OriHtml[i + 1].Type == HtmlNode.NodeType.CloseTag)
                            {
                                OriHtml.Insert(endNodeIndex, OriHtml[i]);
                                OriHtml.RemoveAt(i);

                                OriHtml.Insert(endNodeIndex, OriHtml[i]);
                                OriHtml.RemoveAt(i);

                                i--;
                                pos += 2;
                                continue;
                            }
                            else
                            {
                                OriHtml.Insert(endNodeIndex, OriHtml[i]);
                                OriHtml.RemoveAt(i);

                                OriHtml.Insert(endNodeIndex, OriHtml[i]);
                                OriHtml.RemoveAt(i);

                                OriHtml.Insert(endNodeIndex, OriHtml[i]);
                                OriHtml.RemoveAt(i);


                                i--;
                                pos += 3;
                                continue;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 把 主题 资源提取出来，并把它从 OriHtml 中删除
        /// </summary>
        /// <param name="OriHtml"></param>
        /// <returns></returns>
        public static List<HtmlNode> TakeThemesAndRemoveIt(List<HtmlNode> OriHtml)
        {
            //Take the Theme File To the First
            List<HtmlNode> listToFirst = new List<HtmlNode>();
            for (int i = 0; i < OriHtml.Count; i++)
            {
                if (OriHtml[i] is HtmlTagNode)
                {
                    HtmlTagNode tn = OriHtml[i] as HtmlTagNode;

                    if (tn.TagName.IsIn(StringComparer.CurrentCultureIgnoreCase, "link"))
                    {
                        var attrValue = tn.Attrs.Where(o => o.Name.ToLower() == "href").FirstOrDefault();
                        if (attrValue.Value.Contains("App_Themes/"))
                        {
                            listToFirst.Add(tn);
                            OriHtml.RemoveAt(i);
                            if (tn.IsSole == false)
                            {
                                listToFirst.Add(OriHtml[i]);
                                OriHtml.RemoveAt(i);
                                i--;
                            }
                            i--;
                            continue;
                        }
                    }
                }
                else if (OriHtml[i] is HtmlCloseTagNode)
                {
                    HtmlCloseTagNode ctn = OriHtml[i] as HtmlCloseTagNode;

                    if (ctn.TagName.Equals("head", StringComparison.CurrentCultureIgnoreCase))
                    {
                        break;
                    }
                }
            }

            return listToFirst;
        }

        public static int GetBeginNodeIndex(List<HtmlNode> OriHtml)
        {
            int InsertIndex = 0;
            for (int i = 0; i < OriHtml.Count; i++)
            {
                if (OriHtml[i] is HtmlTagNode)
                {
                    HtmlTagNode tn = OriHtml[i] as HtmlTagNode;

                    if (tn.TagName.IsIn(StringComparer.CurrentCultureIgnoreCase,
                         "script"))
                    {
                        InsertIndex = i;
                        break;
                    }
                }
                else if (OriHtml[i] is HtmlCloseTagNode)
                {
                    HtmlCloseTagNode ctn = OriHtml[i] as HtmlCloseTagNode;

                    if (ctn.TagName.Equals("head", StringComparison.CurrentCultureIgnoreCase))
                    {
                        InsertIndex = i;
                        break;
                    }
                }
            }
            return InsertIndex;
        }


        /// <summary>
        /// 生成 jv 变量
        /// </summary>
        /// <returns></returns>
        public static string GetRequestJs()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            HttpContext.Current.Request.QueryString.AllKeys.All(o =>
            {
                if (o == null) return true;
                dict[o] = HttpContext.Current.Request.QueryString[o]; return true;
            });

            if (MyHelper.RequestContext != null)
            {
                MyHelper.RequestContext.RouteData.Values.Keys.All(o =>
                {
                    if (o == null) return true;
                    dict[o] = MyHelper.RequestContext.RouteData.Values[o].AsString(); return true;
                });
            }

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
            retVal.AddRange(GetUsingFile("~/Areas/" + MyHelper.Area + "/Res/MyCss.css"));
            retVal.AddRange(GetUsingFile("~/Res/MyJs.js"));

            return retVal;
        }


        public static HtmlNode[] GetUsingFile(string JsOrCssFile)
        {
            if (JsOrCssFile.EndsWith(".js", StringComparison.CurrentCultureIgnoreCase))
            {
                List<HtmlNode> retVal = new List<HtmlNode>();
                {
                    HtmlTagNode htn = new HtmlTagNode();
                    htn.TagName = "script";
                    htn.Attrs.Add(new HtmlAttrNode() { Name = "type", Value = "text/javascript" });
                    htn.Attrs.Add(new HtmlAttrNode() { Name = "src", Value = (JsOrCssFile).ResolveUrl() });
                    retVal.Add(htn);
                }

                {
                    HtmlCloseTagNode htn = new HtmlCloseTagNode();
                    htn.TagName = "script";
                    retVal.Add(htn);
                }
                return retVal.ToArray();
            }
            else
            {
                List<HtmlNode> retVal = new List<HtmlNode>();
                {
                    HtmlTagNode htn = new HtmlTagNode();
                    htn.TagName = "link";
                    htn.IsSole = true;
                    htn.Attrs.Add(new HtmlAttrNode() { Name = "rel", Value = "stylesheet" });
                    htn.Attrs.Add(new HtmlAttrNode() { Name = "type", Value = "text/css" });
                    htn.Attrs.Add(new HtmlAttrNode() { Name = "href", Value = (JsOrCssFile).ResolveUrl() });
                    retVal.Add(htn);
                }
                return retVal.ToArray();
            }
        }

        public static HtmlNode[] CreateJsNode(string JsText)
        {
            if (JsText.HasValue() == false) return new HtmlNode[0];

            List<HtmlNode> retVal = new List<HtmlNode>();
            {
                HtmlTagNode htn = new HtmlTagNode();
                htn.TagName = "script";
                htn.Attrs.Add(new HtmlAttrNode() { Name = "type", Value = "text/javascript" });
                retVal.Add(htn);
            }
            {
                HtmlTextNode htn = new HtmlTextNode();
                htn.Text = string.Format(@"$(function(){{{0}}});", JsText);
                retVal.Add(htn);
            }
            {
                HtmlCloseTagNode htn = new HtmlCloseTagNode();
                htn.TagName = "script";
                retVal.Add(htn);
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// 对 Header 进行修补.添加必要信息.
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static HtmlTagNode[] PatchHeader(DeptRule.Entity mod)
        {
            if (mod != null)
            {
                var meta1 = new HtmlTagNode();
                meta1.TagName = "meta";
                meta1.Attrs = new List<HtmlAttrNode>();
                meta1.Attrs.Add(new HtmlAttrNode() { Name = "name", Value = "keywords" });
                meta1.Attrs.Add(new HtmlAttrNode() { Name = "content", Value = mod.KeyWords.GetSafeValue() });
                meta1.IsSole = true;

                var meta2 = new HtmlTagNode();
                meta2.TagName = "meta";
                meta2.Attrs = new List<HtmlAttrNode>();
                meta2.Attrs.Add(new HtmlAttrNode() { Name = "name", Value = "description" });
                meta2.Attrs.Add(new HtmlAttrNode() { Name = "content", Value = mod.Name.GetSafeValue() });
                meta2.IsSole = true;

                var meta3 = new HtmlTagNode();
                meta3.TagName = "meta";
                meta3.Attrs = new List<HtmlAttrNode>();
                meta3.Attrs.Add(new HtmlAttrNode() { Name = "name", Value = "author" });
                meta3.Attrs.Add(new HtmlAttrNode() { Name = "content", Value = mod.Name.GetSafeValue() });
                meta3.IsSole = true;


                return new HtmlTagNode[] { meta1, meta2, meta3 };
            }
            return new HtmlTagNode[0] { };
        }



        private RenderResponse _ronse = null;
        public RenderResponse Ronse
        {
            get
            {
                if (_ronse == null) _ronse = new RenderResponse();
                return _ronse;
            }
            set { _ronse = value; }
        }

        public string Controller { get { return MyHelper.Controller; } }
        public string Action { get { return MyHelper.Action; } }

        public string GetRes(params string[] res)
        {
            return res[0];
        }
    }
}
