using System.Collections.Generic;
using System.Linq;
using MyCmn;
using MyCon;
using MyBiz;
using DbEnt;
using MyOql;

namespace System.Web.Mvc
{
    public enum TextHelperPostEnum
    {
        None,
        Change,
        Click,
    }

    public enum QuoteEnum
    {
        Radio = 1,
        Check,
    }

    public class EnumResult<T> : ActionResult
        where T : IComparable, IFormattable, IConvertible
    {
        private T value;
        public EnumResult(T SelectValue)
        {
            value = SelectValue;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            string temp = @"{{data:""{0}""{1}}}";
            //if (context.HttpContext.Response.ContentType == "application/json")
            //{
            //    temp = @"{{""data"":""{0}""{1}}}";
            //}
            //else
            //{
            //    temp = @"{{data:""{0}""{1}}}";
            //}

            context.HttpContext.Response.Write(string.Format(temp
                , string.Join(",", EnumHelper.ToEnumList<T>().Select(o => o.ToString() + ":" + (o as Enum).GetRes().ToString()).ToArray())
                , value.HasValue() ? @",selectValue:""" + value.ToString() + @"""" : string.Empty));
        }
    }

    public class IEnmResult<T> : ActionResult
        where T : IEnm
    {
        private IEnm value;
        public IEnmResult(IEnm selectValue)
        {
            value = selectValue;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            Type clasType = typeof(T).BaseType.GetGenericArguments()[0];

            var obj = Activator.CreateInstance(clasType) as IReadEntity;
            if (obj == null) return;

            string temp = @"{{data:""{0}""{1}}}";
            //if (context.HttpContext.Response.ContentType == "application/json")
            //{
            //    temp = @"{{""data"":""{0}""{1}}}";
            //}
            //else
            //{
            //    temp = @"{{data:""{0}""{1}}}";
            //}

            context.HttpContext.Response.Write(string.Format(temp
                , string.Join(",", obj.GetProperties().Select(o =>
                {
                    var ie = obj.GetPropertyValue(o) as IEnm;
                    return ie.Name + ":" + ie.GetRes().ToString();
                }).ToArray())
                    , value == null ? "" : @",selectValue:""" + value.Name + @""""));
        }
    }

    public class UITextHelper
    {
        public bool require { get; set; }
        public string change { get; set; }
        public TextHelperPostEnum post { get; set; }
        public string url { get; set; }
        public string renderCallback { get; set; }
        public string selectValue { get; set; }
        public QuoteEnum quote { get; set; }
        public bool? useSelect { get; set; }
    }


    public class TextHelperJsonResult : ActionResult
    {
        private string str;
        public TextHelperJsonResult(StringDict data)
        {
            this.str = string.Join(",", data.Select(o =>
                {
                    return o.Key + ":" + o.Value;
                }).ToArray());
        }

        public TextHelperJsonResult(params string[] vs)
        {
            this.str = string.Join(",", vs);
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Write(str);
        }
    }


    public class EnumUIData
    {

        string Id { get; set; }
        MyMvcController con { get; set; }
        public EnumUIData(MyMvcController con, string Id) { this.Id = Id; this.con = con; }

        public EnumUIData Input(object InputAttr = null)
        {
            var chkExp = new StringLinker();
            if (InputAttr != null)
            {
                var type = InputAttr.GetType();
                var ps = type.GetProperties();
                ps.All(o =>
                {
                    chkExp += " " + o.Name + @"=""" + FastInvoke.GetPropertyValue(InputAttr, type, type.GetMethod("get_" + o.Name)) + @"""";
                    return true;
                });
            }

            HttpContext.Current.Response.Write(string.Format(@"<input id=""{0}"" name=""{0}"" {1}/>", Id, chkExp.ToString()));
            return this;
        }
        public EnumUIData Radior(UITextHelper TextHelperAttr = null)
        {
            return TextHelper(QuoteEnum.Radio, TextHelperAttr);
        }
        public EnumUIData Checker(UITextHelper TextHelperAttr = null)
        {
            if (TextHelperAttr == null) TextHelperAttr = new UITextHelper();
            TextHelperAttr.useSelect = false;

            return TextHelper(QuoteEnum.Check, TextHelperAttr);
        }


        private EnumUIData TextHelper(QuoteEnum Quote, UITextHelper TextHelperAttr = null)
        {
            var notStrings = new string[] { "change", "click", "callback", "hideCallback", "beforeClickInput", "postCallback", "changeCallback", "clickCallback", "require", "useSelect", "width" };
            StringLinker chkExp = string.Format(@"quote:""{0}""", Quote.ToString().ToLower());


            if (TextHelperAttr != null)
            {
                Func<string, string, string> getAttr = (n, v) =>
                {
                    if (n.IsIn(notStrings)) return "," + n + @":" + v;
                    else return "," + n + @":""" + v + @"""";
                };

                TextHelperAttr.change.HasValue(o => chkExp += getAttr("change", o));
                TextHelperAttr.post.HasValue(o => chkExp += getAttr("post", o.ToString().ToLower()));
                TextHelperAttr.renderCallback.HasValue(o => chkExp += getAttr("renderCallback", o));
                chkExp += getAttr("require", TextHelperAttr.require.AsString().ToLower());
                TextHelperAttr.selectValue.HasValue(o => chkExp += getAttr("selectValue", o));
                TextHelperAttr.url.HasValue(o => chkExp += getAttr("url", o));

                if (TextHelperAttr.useSelect.HasValue)
                {

                    chkExp += ",useSelect:" + (TextHelperAttr.useSelect.Value ? "true" : "false");
                }
            }

            con.RegisterStartupScript("TextHelper_" + Id,
                string.Format(@"$(""#{0}"").TextHelper($.extend({{{1}}},jv.page().{0}));"
                    , Id
                    , chkExp.AsString()
                    ));

            return this;
        }


        public override string ToString()
        {
            return string.Empty;
        }
    }

}
