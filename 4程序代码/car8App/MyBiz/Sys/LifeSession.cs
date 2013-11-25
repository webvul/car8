using System.Xml;
using System;
using MyOql;
using MyCmn;
using MyBiz;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Threading;


namespace MyBiz
{
    public class LifeSession : IDisposable
    {
        //public static Dictionary<string, StringDict> App { get; set; }

        static LifeSession()
        {
            //App = new Dictionary<string, StringDict>();
        }


        List<string> keys = new List<string>();
        public object this[string SessionKey]
        {
            get
            {
                return HttpContext.Current.Session[SessionKey];
            }
            set
            {
                GodError.Check(value == null, () => "数据接收异常");
                keys.Add(SessionKey);
                HttpContext.Current.Session[SessionKey] = value;
            }
        }
        public void Dispose()
        {
            keys.All(o =>
            {
                HttpContext.Current.Session.Remove(o);
                return true;
            });
        }

        public static T OnceGetQuery<T>(string Key, T NewModel)
            where T : class
        {
            if (Key.HasValue() == false)
            {
                throw new GodError("接收Session值的 Key 值不能为空");
            }

            StringDict dict = HttpContext.Current.Session["LifeSession_" + Key] as StringDict;

            if (dict == null) return null;
            var retVal = dict.DictionaryToModel(NewModel);
            GodError.Check(retVal == null, () => "Session项数据 : " + Key + "转换失败, 数据:" + dict.ToJson() + ",转换目标类型:" + typeof(T).FullName);

            HttpContext.Current.Session.Remove("LifeSession_" + Key);
            return retVal;
        }
    }
}