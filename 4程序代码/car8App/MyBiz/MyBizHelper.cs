using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyOql;
using System.Web.Mvc;
using MyCmn;
using System.Data.SqlClient;
using System.Data.Common;
using System.Xml;
using System.Web;
using System.Data;
using System.Threading;
using MyBiz.Admin;
using DbEnt;

namespace MyBiz
{

    public static partial class MyBizHelper
    {
        public static readonly DateTime SQLMinDate = new DateTime(1900, 1, 1);

        public static StringLinker GetRes(this LangEnum Lang, string Key)
        {
            return TxtResBiz.GetRes(Lang, Key);
        }

        //private static ThreadLocal<string> _Ip;
        //public static string GetClientIp()
        //{
        //    if (_Ip == null || !_Ip.IsValueCreated)
        //    {
        //        lock (_Sync_ModuleName)
        //        {
        //            if (_Ip == null || !_Ip.IsValueCreated)
        //            {
        //                _Ip.Value =
        //                    HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].AsString(
        //                    HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]).AsString(
        //                    HttpContext.Current.Request.UserHostAddress);
        //            }
        //        }
        //    }

        //    if (_Ip == null) return string.Empty;
        //    if (_Ip.IsValueCreated == false) return string.Empty;
        //    if (_Ip.Value == null) return string.Empty;

        //    return _Ip.Value;
        //}

        private static ThreadLocal<string[]> _ModuleName;
        private static ThreadLocal<object> _Sync_ModuleName = new ThreadLocal<object>();

        public static string[] GetNavigateText()
        {
            if (_ModuleName == null || !_ModuleName.IsValueCreated)
            {
                lock (_Sync_ModuleName)
                {
                    if (_ModuleName == null || !_ModuleName.IsValueCreated)
                    {
                        if (HttpContext.Current == null) return new string[] { };

                        _ModuleName = new ThreadLocal<string[]>();
                        string url = HttpContext.Current.Request.Url.LocalPath;
                        var set = dbr.Menu
                            .Select(o => new Columns(new ColumnClip[] { o.Id, o.Wbs, o.Text, o.Url, o.Url.StringIndex("?").As("UrlIndex") }))
                            .SelectWrap("m",
                                dbr.Menu.Id.FromTable("m"),
                                dbr.Menu.Wbs.FromTable("m"),
                                dbr.Menu.Text.FromTable("m"),
                                dbr.Menu.Url.FromTable("m").SubString(new ConstColumn(0),
                                    dbo.CaseWhen(new ConstColumn("UrlIndex") { DbType = DbType.Int32 }.FromTable("m") == 0, new ConstColumn(0x1000)).ElseEnd(new ConstColumn("UrlIndex") { DbType = DbType.Int32 }.FromTable("m"))
                                    ).As("Path"),
                                dbr.Menu.Url.FromTable("m").SubString(
                                    dbo.CaseWhen(new ConstColumn("UrlIndex") { DbType = DbType.Int32 }.FromTable("m") == 0, new ConstColumn(0x1000)).ElseEnd(new ConstColumn("UrlIndex") { DbType = DbType.Int32 }.FromTable("m")) + 1,
                                    new ConstColumn(0x1000)).As("Query"),
                                dbr.Menu.Url.FromTable("m").As("Url")
                             )
                            .Where(new ConstColumn("Url").FromTable("m").StringIndex(url) > 0)
                            .SkipPower()
                            .ToMyOqlSet();

                        List<string> list = new List<string>();
                        //int idIndex = set.GetColumnIndex("Id");
                        //int wbsIndex = set.GetColumnIndex("Wbs");
                        //int textIndex = set.GetColumnIndex("Text");
                        //int queryIndex = set.GetColumnIndex("Query");
                        string[] pids = null;
                        int maxSimilarRowIndex = 0;

                        if (set.Rows.Count == 0)
                        {
                            //处理带有 uid 的情况。 数据库里是 .../Admin/Menu/Edit.aspx  但传进来的是： .../Admin/Menu/Edit/1.aspx

                            var lastIndex = url.LastIndexOf('/');
                            url = url.Substring(0, lastIndex) + ".aspx";

                            set = dbr.Menu
                            .Select(o => new Columns(new ColumnClip[] { o.Id, o.Wbs, o.Text, o.Url, o.Url.StringIndex("?").As("UrlIndex") }))
                            .SelectWrap("m",
                                dbr.Menu.Id.FromTable("m"),
                                dbr.Menu.Wbs.FromTable("m"),
                                dbr.Menu.Text.FromTable("m"),
                                dbr.Menu.Url.FromTable("m").SubString(new ConstColumn(0),
                                    dbo.CaseWhen(new ConstColumn("UrlIndex") { DbType = DbType.Int32 }.FromTable("m") == 0, new ConstColumn(0x1000)).ElseEnd(new ConstColumn("UrlIndex") { DbType = DbType.Int32 }.FromTable("m"))
                                    ).As("Path"),
                                dbr.Menu.Url.FromTable("m").SubString(
                                    dbo.CaseWhen(new ConstColumn("UrlIndex") { DbType = DbType.Int32 }.FromTable("m") == 0, new ConstColumn(0x1000)).ElseEnd(new ConstColumn("UrlIndex") { DbType = DbType.Int32 }.FromTable("m")) + 1,
                                    new ConstColumn(0x1000)).As("Query"),
                                dbr.Menu.Url.FromTable("m").As("Url")
                             )
                            .Where(new ConstColumn("Url").FromTable("m").StringIndex(url) > 0)
                            .SkipPower()
                            .ToMyOqlSet();

                            if (set.Rows.Count == 0) return new string[0];
                        }

                        if (set.Rows.Count == 1)
                        {
                            pids = set.Rows[0][dbr.Menu.Wbs.Name].AsString().Split(",").ToArray<string>();
                        }
                        else
                        {
                            Dictionary<int, double> dict = new Dictionary<int, double>();
                            int pos = 0;
                            set.Rows.ForEach(o =>
                            {
                                dict[pos++] = ValueProc.GetSimilar(HttpContext.Current.Request.Url.Query, o["Query"].AsString(), false, ValueProc.SimilarEnum.First);
                            });

                            maxSimilarRowIndex = dict.First(o => (o.Value == dict.Max(d => d.Value))).Key;
                            pids = set.Rows[maxSimilarRowIndex][dbr.Menu.Wbs.Name].AsString().Split(",").ToArray<string>();
                        }

                        list = dbr.Menu
                            .Select(o => o.Text)
                            .Where(o => o.Id.In(pids.Select(d => d.AsInt()).ToArray()))
                            .OrderBy(o => o.Wbs.Asc)
                            .SkipPower()
                            .ToEntityList("");

                        list.Add(set.Rows[maxSimilarRowIndex][dbr.Menu.Text.Name].AsString());
                        _ModuleName.Value = list.ToArray();
                    }
                }
            }

            if (_ModuleName == null) return new string[] { };
            if (_ModuleName.IsValueCreated == false) return new string[] { };
            if (_ModuleName.Value == null) return new string[] { };

            return _ModuleName.Value;
        }

        public static void ResetWebConnection()
        {
            _ModuleName = null;
            //_Ip = null;
        }

        //public static StringLinker GetRes<T>(this T TxtEnum) where T : IComparable, IFormattable, IConvertible
        //{
        //    if (TxtEnum.AsString() == "0")
        //    {
        //        return string.Empty;
        //    }

        //    var type = TxtEnum.GetType().FullName;

        //    return string.Join(",", TxtEnum.ToEnumList()
        //        .Select(o =>
        //        {
        //            var enmValue = o.ToString();
        //            var key = type + "." + enmValue;
        //            var retVal = TxtResBiz.GetRes(MyHelper.Lang, key).ToString();
        //            if (key == retVal) return enmValue;
        //            else return retVal;
        //        }).ToArray());
        //}


        public static StringLinker GetRes(this IEnm TxtEnum)
        {
            return GetRes(TxtEnum, MyHelper.Lang);
        }
        public static StringLinker GetRes(this IEnm TxtEnum, LangEnum Lang)
        {
            if (TxtEnum == null)
            {
                return string.Empty;
            }

            var type = TxtEnum.GetType().FullName;
            var key = type + "." + TxtEnum.Name;
            var retVal = TxtResBiz.GetRes(Lang, key);
            if (key == retVal) return TxtEnum.Name;
            return retVal;
        }

        public static StringLinker GetRes(this Enum TxtEnum)
        {
            return GetRes(TxtEnum, MyHelper.Lang);
        }

        public static StringLinker GetRes(this Enum TxtEnum, LangEnum Lang)
        {
            var type = TxtEnum.GetType();
            var typeName = type.FullName;


            return string.Join(",", TxtEnum.ToEnumList()
                .Select(o =>
                {
                    var enmValue = o.ToString();
                    var key = typeName + "." + enmValue;
                    var retVal = TxtResBiz.GetRes(Lang, key).ToString();

                    if (!retVal.HasValue()) return enmValue;
                    else if (key != retVal) return retVal;
                    else if (TxtEnum.HasValue() == false)
                    {
                        return string.Empty;
                    }
                    else return enmValue;

                }).ToArray());
        }


        public static XmlDictionary<T, string> GetRes<T>(string QueryResValue)
            where T : IComparable, IFormattable, IConvertible
        {
            XmlDictionary<T, string> dict = new XmlDictionary<T, string>();
            GetRes<T>(MyHelper.Lang).Where(o => o.Value.Contains(QueryResValue)).All(o =>
                {
                    dict.Add(o.Key, o.Value);
                    return true;
                });

            return dict;
        }

        public static XmlDictionary<T, string> GetRes<T>(LangEnum Lang)
            where T : IComparable, IFormattable, IConvertible
        {
            var type = typeof(T).GetType().FullName;

            var ds = TxtResBiz.LoadRes(Lang);

            XmlDictionary<T, string> dict = new XmlDictionary<T, string>();
            var res = ds.Where(o => o.Key.Contains(type + ".")).ToArray();
            EnumHelper.ToEnumList<T>().All(o =>
                {
                    dict[o] = res.FirstOrDefault(f => f.Key == type + "." + o.ToString()).AsString(o.ToString());
                    return true;
                });

            return dict;
        }



        /// <summary>
        /// 针对报表分页而做的数据包装. 约定必须要有 页数 属性. SysPageIndex , 且为 Int 型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source">数据源.</param>
        /// <param name="CurrentPageIndex">当前页索引, 从0开始.</param>
        /// <param name="PageCount">总页数</param>
        /// <returns></returns>
        public static IEnumerable<T> ReportWrap<T>(this IEnumerable<T> Source, int CurrentPageIndex, int PageCount)
            where T : IReportModel, new()
        {
            if (PageCount < 2)
            {
                foreach (var item in Source)
                {
                    yield return item;
                }
                yield break;
            }

            Func<int, T> CreateEmptyObj = (CurrentIndex) =>
            {
                T retVal = new T();

                retVal.SysPageIndex = CurrentIndex;

                return retVal;
            };

            for (int i = 0; i < CurrentPageIndex; i++)
            {
                yield return CreateEmptyObj(i + 1);
            }


            foreach (var item in Source)
            {
                item.SysPageIndex = CurrentPageIndex + 1;
                yield return item;
            }

            for (int i = CurrentPageIndex + 1; i < PageCount; i++)
            {
                yield return CreateEmptyObj(i + 1);
            }
        }


        public static XmlNamespaceManager GetNameSpaceManager(this XmlDocument xml, string NameSpace)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xml.NameTable);

            foreach (XmlAttribute item in xml.DocumentElement.Attributes)
            {
                if (item.Prefix.HasValue())
                {
                    nsmgr.AddNamespace(item.LocalName, item.Value);
                }
            }


            if (NameSpace.HasValue())
            {
                nsmgr.AddNamespace(NameSpace, xml.DocumentElement.NamespaceURI);
            }
            return nsmgr;
        }


        public static ColumnClip[] GetColumns<T>(this T Rule, Func<T, ColumnClip[]> selectFunc)
            where T : RuleBase
        {
            if (selectFunc == null) return Rule.GetColumns();
            else return selectFunc(Rule);
        }
    }


}
