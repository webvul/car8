using System.Collections.Generic;
using System.Linq;
using MyCmn;
using MyOql;

namespace System.Web.Mvc
{
    public static partial class MyHtmlHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="selected"></param>
        /// <param name="dict">key是名称， value 是url</param>
        /// <returns></returns>
        public static string Navigate(this HtmlHelper html, string selected, Dictionary<string, string> dict)
        {
            if (dict.Count == 0) return string.Empty;
            return @"<div class=""divNavigation"">" +
                string.Join(string.Empty,
                    dict.Select(o => string.Format(@"<span{0}><a href='{1}'>{2}</a></span>"
                        , string.Equals(o.Key, selected, StringComparison.CurrentCultureIgnoreCase) ? @" class=""navselected""" : string.Empty
                        , o.Value, o.Key)).ToArray()
                        )
                        + "</div>";

        }

        public class SubNavigateItemObj
        {
            public string Id { get; set; }
            public string Text { get; set; }
            public string ClickJs { get; set; }
            public string Css { get; set; }
        }

        public static string SubNavigate(this HtmlHelper html, string name, string selectedCode, List<SubNavigateItemObj> data)
        {
            if (data.Count == 0) return string.Empty;
            return @"<table class=""radioNavi""><tr>" +
                string.Join(string.Empty,
                    data.Select(o => string.Format(@"<td{5}><input type=""radio"" id=""{0}"" name=""{1}""{3}{4}/><label for=""{0}"">{2}</label></td>"
                        , o.Id
                        , name
                        , o.Text
                        , string.Equals(o.Id, selectedCode, StringComparison.CurrentCultureIgnoreCase) ? @" checked=""checked""" : string.Empty
                        , o.ClickJs.HasValue() ? @" onclick=""" + o.ClickJs + @"""" : string.Empty
                        , o.Css.HasValue() ? @" class=""" + o.Css + @"""" : string.Empty
                        )
                    ).ToArray()
                 ) + "</tr></table>";

        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="html"></param>
//        /// <param name="name"></param>
//        /// <param name="selectedCode">key是 radio 的 value , value 是 radio 的显示。</param>
//        /// <param name="data"></param>
//        /// <returns></returns>
//        public static string SubNavigate(this HtmlHelper html, string name, string selectedCode, Dictionary<string, string> data)
//        {
//            if (data == null) return string.Empty;
//            return
//                string.Join(string.Empty,
//                    data.Select(o =>
//                    {
//                        var sel = string.Equals(o.Key, selectedCode, StringComparison.CurrentCultureIgnoreCase) ? @" class=""subnavselected""" : string.Empty;

//                        return
//@"<span{Class}><input name=""{Name}"" id=""{Name}_{Value}"" type=""radio"" value=""{Value}""/><label for=""{Name}_{Value}"">{Display}</label></span>"
//                     .Format(new StringDict { { "Class", sel }, { "Name", name }, { "Value", o.Key }, { "Display", o.Value } });
//                    }
//                         ).ToArray()
//                        );

//        }

    }
}
