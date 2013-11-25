using System.Collections.Generic;
using System.Linq;
using MyCmn;
using MyCon;
using MyBiz;
using DbEnt;
using MyOql;

namespace System.Web.Mvc
{
    public static partial class MyHtmlHelper
    {
        public static EnumUIData RegisteEnum<T>(this HtmlHelper html, string HtmlId, T Selected) where T : IComparable, IFormattable, IConvertible
        {
            var con = html.ViewContext.Controller as MyMvcController;
            EnumUIData ed = new EnumUIData(con, HtmlId);
            con.RenderJsVar(HtmlId, new EnumResult<T>(Selected));

            return ed;
        }

        public static EnumUIData RegisteIEnm<T>(this HtmlHelper html, string HtmlId, T Selected)
            where T : IEnm
        {
            var con = html.ViewContext.Controller as MyMvcController;
            EnumUIData ed = new EnumUIData(con, HtmlId);
            con.RenderJsVar(HtmlId, new IEnmResult<T>(Selected));

            return ed;
        }

        public static EnumUIData RegisteArray(this HtmlHelper html, string HtmlId, params string[] ArrayString)
        {
            var con = html.ViewContext.Controller as MyMvcController;
            EnumUIData ed = new EnumUIData(con, HtmlId);
            con.RenderJsVar(HtmlId, new VarJsResult(
                string.Format(@"{{data:""{0}""}}", string.Join(",", ArrayString))) { Type = JsType.Value }
                );
            return ed;
        }

        public static EnumUIData Registe(this HtmlHelper html, string HtmlId, IDictionary<string, string> dataSource)
        {
            var con = html.ViewContext.Controller as MyMvcController;
            EnumUIData ed = new EnumUIData(con, HtmlId);

            var list = dataSource.Select(o => o.Key + ":" + o.Value);

            con.RenderJsVar(HtmlId, new VarJsResult(
                string.Format(@"{{data:""{0}""}}", string.Join(",", list.ToArray()))) { Type = JsType.Value }
                );
            return ed;
        }

        public static EnumUIData Registe(this HtmlHelper html, string HtmlId)
        {
            var con = html.ViewContext.Controller as MyMvcController;
            EnumUIData ed = new EnumUIData(con, HtmlId);

            //con.RenderJsVar(HtmlId, new VarJsResult(
            //    string.Format(@"{{post:""click"",url:""{0}""}}", Url)) { Type = JsType.Value }
            //    );
            return ed;
        }

        public static EnumUIData Registe<T>(this HtmlHelper html, string HtmlId, IEnumerable<T> dataSource)
            where T : IModel
        {
            return Registe<T>(html, HtmlId, dataSource, null, null);
        }


        public static EnumUIData Registe<T>(this HtmlHelper html, string HtmlId, IEnumerable<T> dataSource, string Key, string Value)
            where T : IModel
        {
            var con = html.ViewContext.Controller as MyMvcController;
            EnumUIData ed = new EnumUIData(con, HtmlId);

            var list = new List<string>();
            //:,
            dataSource.All(o =>
                {
                    var dict = dbo.ModelToDictionary(null, o).ToXmlDictionary(k => k.Key.GetAlias(), v => v.Value.AsString());

                    if (Key.HasValue() == false)
                    {
                        Key = dict.ElementAt(0).Key;
                    }
                    if (Value.HasValue() == false)
                    {
                        Value = dict.ElementAt(1).Key;
                    }

                    list.Add(dict[Key] + ":" + dict[Value]);
                    return true;
                });

            con.RenderJsVar(HtmlId, new VarJsResult(
                string.Format(@"{{data:""{0}""}}", string.Join(",", list.ToArray()))) { Type = JsType.Value }
                );
            return ed;
        }
    }
}
