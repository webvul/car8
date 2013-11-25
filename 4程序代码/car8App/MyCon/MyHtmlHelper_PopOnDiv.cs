using System.Collections.Generic;
using System.Linq;
using MyCmn;
using MyOql;

namespace System.Web.Mvc
{
    public class UIPop
    {
        public string area { get; set; }
        public string entity { get; set; }
        public string list { get; set; }
        public string detail { get; set; }
        public string uid { get; set; }
        public bool require { get; set; }
        public string url { get; set; }
        public Dictionary<string, string> query { get; set; }
        public string refClick { get; set; }
        public string callback { get; set; }
        public QuoteEnum mode { get; set; }


        public string Id { get; set; }
        public string KeyTitle { get; set; }
        public string Value { get; set; }
        public string Display { get; set; }
        public string ChkMsg { get; set; }
    }

    public class UIPopData
    {
        UIPop pop;
        public UIPopData(UIPop pop) { this.pop = pop; }

        private string getClickJs()
        {
            var clickJsPara = new List<string>();
            clickJsPara.Add(@"mode:""" + pop.mode.ToString().ToLower() + @"""");

            clickJsPara.Add(@"area:""" + pop.area.AsString(MyHelper.Area) + @"""");
            clickJsPara.Add(@"entity:""" + pop.entity.AsString(MyHelper.Controller) + @"""");
            clickJsPara.Add(@"list:""" + pop.list.AsString("List") + @"""");

            //pop.data.HasValue(o => { clickJsPara.Add(@"data:{key:""" + o.Key + @""",val:""" + o.Value + @"""}"); return true; });
            pop.detail.HasValue(o => { clickJsPara.Add(@"detail:""" + o + @""""); return true; });
            pop.uid.HasValue(o => { clickJsPara.Add(@"uid:""" + o + @""""); return true; });
            pop.url.HasValue(o => { clickJsPara.Add(@"url:""" + o + @""""); return true; });

            pop.callback.HasValue(o => { clickJsPara.Add(@"callback:" + o); return true; });
            pop.refClick.HasValue(o => { clickJsPara.Add(@"refClick:" + o); return true; });

            pop.query.HasValue(o =>
            {
                var qury = new List<string>();
                o.All(d =>
                {
                    qury.Add(d.Key + @":""" + d.Value + @"""");
                    return true;
                });


                clickJsPara.Add(@"query:{" + string.Join(",", qury.ToArray()) + @"}"); return true;
            });
            return string.Format(@"onclick='jv.PopList({{{0}}},event);'", string.Join(",", clickJsPara.ToArray()));
        }


        public string GenDiv(object KvAttr = null)
        {
            StringDict kvDict = null;


            if (KvAttr != null)
            {
                kvDict = dbo.ModelToStringDict(KvAttr);
            }
            else kvDict = new StringDict();


            if (kvDict.ContainsKey("class") == false) kvDict["class"] = "kv";


            string template = @"<div {7}><span class=""key{6}"">{2}<input id=""{0}"" type=""hidden"" value=""{1}"" /></span><span class=""val""><input readonly=""readonly"" class=""ref"" {4} {5} value=""{3}""/></span></div>";

            var chkExp = string.Empty;
            if (pop.require)
            {
                chkExp = string.Format(@"chk=""function(val){{ if ( val == '0' || val.length == 0 ) return '{1}'; }}"""" chkval=""#{0}""", pop.Id, pop.ChkMsg.AsString("必须选择一个值!").GetSafeValue());
            }


            HttpContext.Current.Response.Write(string.Format(template
                , pop.Id
                , pop.Value
                , pop.KeyTitle + "："
                , pop.Display
                , getClickJs()
                , chkExp
                , pop.require ? " must" : ""
                , string.Join(" ", kvDict.Select(o => string.Format(@"{0}=""{1}""", o.Key, o.Value)).ToArray())
              ));

            return string.Empty;
        }

        public string GenTd(object KeyAttr = null, object ValAttr = null)
        {
            StringDict keyDict = null;
            StringDict valDict = null;


            if (KeyAttr != null)
            {
                keyDict = dbo.ModelToStringDict(KeyAttr);
            }
            else keyDict = new StringDict();

            if (ValAttr != null)
            {
                valDict = dbo.ModelToStringDict(ValAttr);
            }
            else valDict = new StringDict();


            if (keyDict.ContainsKey("class") == false) keyDict["class"] = "key";
            if (valDict.ContainsKey("class") == false) valDict["class"] = "val";

            if (pop.require)
            {
                keyDict["class"] = keyDict["class"] + " must";
            }

            string template = @"<td {6}>{2}<input id=""{0}"" type=""hidden"" value=""{1}"" /></td><td {7}><input readonly=""readonly"" class=""ref"" {4} {5} value=""{3}""/></td>";

            var chkExp = string.Empty;
            if (pop.require)
            {
                chkExp = string.Format(@"chk='function(val){{ if ( val == ""0"" || val.length == 0 ) return ""{1}""; }}' chkval=""#{0}""", pop.Id, pop.ChkMsg.AsString("必须选择一个值!").GetSafeValue());
            }


            HttpContext.Current.Response.Write(string.Format(template
                , pop.Id
                , pop.Value
                , pop.KeyTitle + "："
                , pop.Display
                , getClickJs()
                , chkExp
                , string.Join(" ", keyDict.Select(o => string.Format(@"{0}=""{1}""", o.Key, o.Value)).ToArray())
                , string.Join(" ", valDict.Select(o => string.Format(@"{0}=""{1}""", o.Key, o.Value)).ToArray())
              ));

            return string.Empty;
        }
    }
    public static partial class MyHtmlHelper
    {
        public static UIPopData PreparePop(this HtmlHelper html, UIPop pop)
        {
            return new UIPopData(pop);
        }

        public static UIPopData PopRadior<T>(this T Rule, string Id, UIPop pop)
            where T : RuleBase
        {
            if (Id.HasValue()) { pop.Id = Id; }
            pop.mode = QuoteEnum.Radio;
            pop.entity = pop.entity.AsString(Rule.GetName());
            return new UIPopData(pop);
        }

        public static UIPopData PopChecker<T>(this T Rule, string Id, UIPop pop)
            where T : RuleBase
        {
            if (Id.HasValue()) { pop.Id = Id; }
            pop.mode = QuoteEnum.Check;
            pop.entity = pop.entity.AsString(Rule.GetName());
            return new UIPopData(pop);
        }
    }
}
