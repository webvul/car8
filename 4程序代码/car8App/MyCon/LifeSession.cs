using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using MyCmn;
using System.Threading;
using MyOql;

namespace MyCon
{
    public class LifeSession : IDisposable
    {
        List<string> keys = new List<string>();
        public object this[string SessionKey]
        {
            get
            {
                return HttpContext.Current.Session[SessionKey];
            }
            set
            {
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
            if (Key.HasValue() == false) return null;
            XmlDictionary<string, string> dict = null;

            for (int i = 0; i < 300; i++)
            {
                if (HttpContext.Current.Session[Key] != null)
                {
                    dict = HttpContext.Current.Session[Key] as XmlDictionary<string, string>;
                    HttpContext.Current.Session.Remove(Key);
                    break;
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            if (dict == null) return null;
            return dict.DictionaryToModel(NewModel);
        }
    }
}
