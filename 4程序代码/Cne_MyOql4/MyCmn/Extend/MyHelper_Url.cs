using System.Linq;
using MyCmn;
using System.IO;
using System.Reflection;


namespace System.Web.Mvc
{
    public static partial class MyHelper
    {


        /// <summary>
        /// 能够解析 ~/,../,./ 三类开头的URL ， 解析为程序使用的URL ，不带Http 头。
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static string ResolveUrl(this string Url)
        {
            if (Url.HasValue() == false) Url = string.Empty;

            //if (strRoot.HasValue() == false) strRoot = @"/"; 
            Url = Url.Trim();

            if (HttpContext.Current == null)
            {
                return GetPath(Url);
            }

            if (Url.StartsWith("~/"))
            {
                string strRoot = HttpContext.Current.Request.ApplicationPath.TrimEnd("/");
                return strRoot + "/" + Url.TrimStart("~/");
            }
            else if (Url.StartsWith("/") ||
                Url.StartsWith("#") ||
                Url.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) ||
                Url.StartsWith("htts://", StringComparison.CurrentCultureIgnoreCase) ||
                Url.StartsWith("javascript:", StringComparison.CurrentCultureIgnoreCase))
            {
                return Url;
            }
            else if (Url.StartsWith("../"))
            {
                var urlArr = Url.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int upLevel = urlArr.Count(o => o == "..");

                var theUrl = HttpContext.Current.Request.Url.LocalPath.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);



                return (HttpContext.Current.Request.Url.LocalPath.StartsWith("/") ? "/" : "") +
                    string.Join("/", theUrl.Take(theUrl.Length - upLevel - 1).ToArray()) + "/" +
                    string.Join("/", urlArr.Slice(upLevel).ToArray());
            }

            //默认  ./ 开头， 或 a/b/c 形式， 返回 当前URL相对路径下的绝对URL
            var lp = HttpContext.Current.Request.Url.LocalPath;
            lp = lp.Substring(0, lp.LastIndexOf('/'));
            return lp + "/" + Url.TrimStart("./");
        }

        private static string GetPath(string Url)
        {
            var path = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName + Path.DirectorySeparatorChar.ToString();
            if (Url.StartsWith("~/"))
            {
                return new FileInfo(path + Url.Substring(2)).FullName;
            }
            return new FileInfo(path + Url).FullName;
        }

        /// <summary>
        /// 得到程序可用的从根开始的路径。如果想要得到带有 Http 头的URL，请使用： MyUrl.GetUrlPrefix() + VirtualPath.GetUrlFull() 
        /// 如:DocInfo/List.aspx?id=3 ， /DocInfo/List.aspx?id=3   
        /// </summary>
        /// <param name="VirtualPath">从根目录开始的路径。</param>
        /// <returns></returns>
        public static string GetUrlFull(this string VirtualPath)
        {
            //if (HttpContext.Current.CurrentHandler == null)
            //    return null;

            if (VirtualPath.HasValue() == false)
            {
                return MyUrl.GetUrlHeader();
            }
            if (VirtualPath.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) || VirtualPath.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
                return VirtualPath;

            return VirtualPath.ResolveUrl();
        }

        ///// <summary>
        ///// HTML化
        ///// </summary>
        ///// <param name="aspxUrl"></param>
        ///// <returns></returns>
        //public static MyUrl UrlHtmler(string aspxUrl)
        //{
        //    var url = new MyUrl(aspxUrl);

        //    if (url.Prefix.StartsWith("#") ||
        //         url.Prefix.StartsWith("javascript:", StringComparison.CurrentCultureIgnoreCase) ||
        //         !url.Extension.Equals(".aspx", StringComparison.CurrentCultureIgnoreCase) ||
        //        (url.Prefix.HasValue() &&
        //            !url.Prefix.StartsWith("http://localhost", StringComparison.CurrentCultureIgnoreCase) &&
        //            !url.Prefix.StartsWith("https://localhost", StringComparison.CurrentCultureIgnoreCase) &&
        //            !url.Prefix.StartsWith("http://127.0.0.1", StringComparison.CurrentCultureIgnoreCase) &&
        //            !url.Prefix.StartsWith("https://127.0.0.1", StringComparison.CurrentCultureIgnoreCase))
        //        )
        //    {
        //        return new MyUrl(aspxUrl);
        //    }

        //    if (HttpContext.Current.Request.ApplicationPath == "/")
        //    {
        //        var ps = url.PathSegment.ToList();
        //        ps.Insert(0, "Html");
        //        url.PathSegment = ps.ToArray();
        //    }
        //    else
        //    {
        //        var ps = url.PathSegment.ToList();
        //        ps.Insert(1, "Html");
        //        url.PathSegment = ps.ToArray();
        //    }

        //    url.QuerySegment.All(o =>
        //    {
        //        if (o.Key.Equals("Html", StringComparison.CurrentCultureIgnoreCase)) return true;
        //        url.PathSegment[url.PathSegment.Length - 1] += "." + o.Key + "." + o.Value;
        //        return true;
        //    });


        //    url.Extension = ".html";

        //    url.QuerySegment.Clear();

        //    return url;
        //}
        public static string GetHtmlUrl(string aspxUrl, Func<string, MyUrl> Translate)
        {
            var qute = string.Empty;
            if (aspxUrl.StartsWith(@"""") && aspxUrl.EndsWith(@""""))
            {
                aspxUrl = aspxUrl.Substring(1, aspxUrl.Length - 2);
                qute = @"""";
            }
            else if (aspxUrl.StartsWith("'") && aspxUrl.EndsWith("'"))
            {
                aspxUrl = aspxUrl.Substring(1, aspxUrl.Length - 2);
                qute = "'";
            }

            if (Translate != null) return qute + Translate(aspxUrl) + qute;

            return qute + aspxUrl.ResolveUrl() + qute;
        }
    }
}
