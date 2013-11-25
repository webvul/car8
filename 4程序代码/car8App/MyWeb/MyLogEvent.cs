using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyCmn;
using DbEnt;
using MyOql;
using MyBiz.Admin;
using System.Web.Mvc;
using MyBiz;
using System.IO;

namespace MyWeb
{
    //public delegate bool MyLogToDelegate(T LogType, string Msg, string User, DateTime AddTime)
    //where T : IComparable, IFormattable, IConvertible;
    [Obsolete("请调用 Log 类.")]
    public class MyLogEvent : ILog
    {
        public MyLogEvent()
        {
        }

        public bool To(string LogType, string Msg, string Detail, string Exception, decimal Value)
        {
            new Func<HttpContext, string, string, string, string, decimal, bool>(MyLogTo).BeginInvoke(HttpContext.Current, LogType, Msg, Detail, Exception, Value, null, null);
            return true;
        }

        private object Sync = new object();

        private bool MyLogTo(HttpContext Context, string LogType, string Msg, string Detail, string Exception, decimal Value)
        {
            if (HttpContext.Current == null) HttpContext.Current = Context;
            try
            {
                var requestData = string.Empty;

                if (HttpContext.Current.Request.HttpMethod == "POST")
                {
                    var d = HttpContext.Current.Request.Form.ToStringDict()
                        .Concat(HttpContext.Current.Request.Headers.ToStringDict())
                        .ToDictionary(o => o.Key, o => o.Value);
                    requestData = d.ToJson();
                }

                if (dbr.Log
                    .Insert(o =>
                        o.Name == "Shop" &
                        o.Type == LogType.AsString() &
                        o.Url == HttpContext.Current.Request.RawUrl &
                        o.Title == HttpContext.Current.Request.Headers["User_Target_Title"] &
                        o.Doer == HttpContext.Current.Request.Headers["User_Target_Element"] &
                        o.Request == requestData &
                        o.Msg == Msg &
                        o.Detail == Detail &
                        o.Exception == Exception &
                        o.Value == Value &
                        o.AddTime == DateTime.Now &
                        o.Ip == Mvc.Model["ClientIp"] &
                        o.Client == HttpContext.Current.Request.UserAgent &
                        o.UserName == MySessionKey.UserName.OnlyGet()
                        )
                    .SkipLog()
                    .SkipPower()
                    .Execute() == 1)
                {
                    return true;
                }
                else return false;
            }
            catch
            {
                try
                {
                    var requestData = string.Empty;

                    if (HttpContext.Current.Request.HttpMethod == "POST")
                    {
                        requestData = HttpContext.Current.Request.Params.ToStringDict().ToJson();
                    }

                    lock (Sync)
                    {
                        File.AppendAllText(HttpContext.Current.Server.MapPath("~/mylog_Err.txt"),
                            "『" + DateTime.Now
                            + @"\t" + "Shop"
                            + @"\t" + LogType.AsString()
                            + @"\t" + HttpContext.Current.Request.RawUrl
                            + @"\t" + HttpContext.Current.Request.Headers["User_Target_Title"]
                            + @"\t" + HttpContext.Current.Request.Headers["User_Target_Element"]
                            + @"\t" + requestData
                            + @"\t" + Msg
                            + @"\t" + Detail
                            + @"\t" + Exception
                            + @"\t" + Value
                            + @"\t" + Mvc.Model["ClientIp"]
                            + @"\t" + HttpContext.Current.Request.UserAgent
                            + @"\t" + MySessionKey.UserName.OnlyGet() +
                            "』");
                    }
                    return true;
                }
                catch { return false; }
            }
        }


    }


    public class MyEnumEvent : IEnumEvent
    {
        /// <summary>
        /// 解析,数据库中枚举项是中文的情况.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public object ToEnum(Type EnumType, string obj, object DefaultValue)
        {
            var type = EnumType.FullName;
            var langRes = TxtResBiz
                .LoadRes(LangEnum.Zh)
                .FirstOrDefault(o => o.Key.StartsWith(type) && o.Value == obj);

            if (langRes != null) return Enum.Parse(EnumType, langRes.Key.Split('.').Last());

            return DefaultValue;
        }
    }


    public class MyResEvent : IRes
    {
        public StringLinker GetRes(string Key)
        {
            return TxtResBiz.GetRes(MyHelper.Lang, Key);
        }
    }

    //[Obsolete("请调用 CacheHelper 类")]
    //public class MyRemoteCacheEvent : ICache
    //{
    //    private CouchbaseClient client { get; set; }
    //    public MyRemoteCacheEvent()
    //    {
    //        this.client = new
    //    }

    //    public void Add(string key, object Data, TimeSpan Time)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public object Get(string key)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IEnumerable<string> GetKeys()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool IsExists(string key)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Remove(string Key)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}