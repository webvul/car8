using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using MyOql;
using MyCon;
using MyBiz;
using System.Threading;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using DbEnt.Model;
using System.Linq.Expressions;
using DbEnt;

namespace MyWeb.Areas.Host.Controllers
{
    public class TestController : MyMvcController
    {
        public ActionResult Lock()
        {
            Action ac1 = () =>
                {
                    using (var con = dbr.Menu.GetDbConnection())
                    {
                        con.Open();
                        using (var tran = con.BeginTransaction())
                        {
                            var cmd = con.CreateCommand();
                            cmd.Transaction = tran;
                            cmd.CommandText = @"
update PowerController set Controller = 'Reception' where id =1 ; 
waitfor delay '00:00:03';
update PowerAction set Action = 'ReceptionQuery' where id =1 ;
";
                            cmd.ExecuteNonQuery();

                            tran.Commit();
                        }
                    }
                };

            Action ac2 = () =>
            {
                using (var con = dbr.Menu.GetDbConnection())
                {
                    con.Open();
                    using (var tran = con.BeginTransaction())
                    {
                        var cmd = con.CreateCommand();
                        cmd.Transaction = tran;
                        cmd.CommandText = @"
update PowerAction set Action = 'ReceptionQuery' where id =1 ;
waitfor delay '00:00:03';
update PowerController set Controller = 'Reception' where id =1 ; 
";
                        cmd.ExecuteNonQuery();

                        tran.Commit();
                    }
                }
            };


            try
            {
                var h1 = ac1.BeginInvoke(null, null);
                var h2 = ac2.BeginInvoke(null, null);

                h1.AsyncWaitHandle.WaitOne();
                h2.AsyncWaitHandle.WaitOne();
            }
            catch (Exception e)
            {
                throw;
            }
            return null;
        }

        public ActionResult T()
        {
            var res = "";
            var r =
                (
                new Func<bool>(() => { res += "1"; return false; })()
                && new Func<bool>(() => { res += "2"; return false; })()
                || new Func<bool>(() => { res += "3"; return true; })()
                )
                 ;
            return Content(r.ToString() + res);
        }

        public class TxtResOld
        {
            public int Id { get; set; }
            public int Pid { get; set; }
            public string RootPath { get; set; }
            public string Key { get; set; }
            public string En { get; set; }
            public string Zh { get; set; }
        }
        //public ActionResult TestThan()
        //{
        //    var cou = dbr.Person.Select(o => o.Count()).Where(o => o.Name > o.Msn).ToScalar().AsInt();

        //    return Content(cou.ToString());
        //}
        //public static Action<IFoo, Bar> CreateSetPropertyValueAction()
        //{
        //    var property = typeof(IFoo).GetProperty("Bar");
        //    var target = Expression.Parameter(typeof(IFoo));
        //    var propertyValue = Expression.Parameter(typeof(Bar));
        //    var setPropertyValue = Expression.Call(target, property.GetSetMethod(), propertyValue);
        //    return Expression.Lambda<Action<IFoo, Bar>>(setPropertyValue, target, propertyValue).Compile();
        //}

        public JsonMsg dff()
        {
            return new JsonMsg() { msg = "er" };
        }
        public ActionResult TestConst()
        {
            return null;
        }

        public ActionResult TestBetween()
        {
            var d = dbr.Menu.Select(o => o.Name.SubString(1, 1)).Where(o => o.Name == null).ToCommand();

            var res = d.Command.CommandText + Environment.NewLine;

            foreach (IDbDataParameter item in d.Command.Parameters)
            {
                res += item.Value.AsString() + "\t";
            }
            return Content(res);
        }


        //public ActionResult TestMap()
        //{

        //    Func<long> UseDirect = () =>
        //    {
        //        Stopwatch sw = new Stopwatch();
        //        sw.Start();

        //        var d = new TCompanyRule.Entity() { CompanyName = "OODFDF" };

        //        var dict = new XmlDictionary<string, object>();
        //        for (int i = 0; i < 10000; i++)
        //        {
        //            dict["CompanyName"] = "eee" + i.ToString();

        //            var dfff = dbo.DictionaryToModel(dict, () => new TCompanyRule.Entity());
        //        }

        //        return sw.ElapsedMilliseconds;
        //    };




        //    Func<long> UseProperty = () =>
        //    {
        //        Stopwatch sw = new Stopwatch();
        //        sw.Start();


        //        var d = new MyOql.TCompanyRule.Entity() { CompanyName = "OODFDF" };

        //        var dict = new XmlDictionary<string, object>();

        //        var type = d.GetType();

        //        for (int i = 0; i < 10000; i++)
        //        {
        //            dict["CompanyName"] = "eee" + i.ToString();


        //            type.GetProperty("CompanyName").SetValue(d, dict["CompanyName"], null);

        //            new MyOql.TCompanyRule.Entity() { CompanyName = "ERWW" };
        //        }


        //        return sw.ElapsedMilliseconds;
        //    };


        //    return Content(UseDirect().ToString() + "," + UseProperty().ToString());
        //}
        //public ActionResult TestBatch()
        //{
        //    // UpdateToServer , 用时 22180  ms
        //    var set = new MyOqlSet(dbr.Dept);
        //    Func<long> ToSet = () =>
        //    {
        //        Stopwatch sw = new Stopwatch();
        //        sw.Start();
        //        set = dbr.Dept.Select().ToMyOqlSet();

        //        sw.Stop();

        //        return sw.ElapsedMilliseconds;
        //    };


        //    Func<long> UseUpdateToServer = () =>
        //    {
        //        Stopwatch sw = new Stopwatch();
        //        sw.Start();
        //        var sIndex = set.Columns.IndexOf(o => o.NameEquals("SortID"));

        //        for (int i = 0; i < set.Rows.Count; i++)
        //        {
        //            set.Rows[i][sIndex] = 3 * i;
        //        }


        //        set.UpdateToServer(null);

        //        sw.Stop();

        //        return sw.ElapsedMilliseconds;
        //    };


        //    var list = new List<XmlDictionary<string, object>>();
        //    Func<long> ToEntity = () =>
        //    {
        //        Stopwatch sw = new Stopwatch();
        //        sw.Start();

        //        list = dbr.Dept.Select().ToDictList();

        //        sw.Stop();

        //        return sw.ElapsedMilliseconds;
        //    };


        //    //36238
        //    Func<long> UseUpdate = () =>
        //    {
        //        Stopwatch sw = new Stopwatch();
        //        sw.Start();


        //        using (var conn = dbr.Dept.GetDbConnection())
        //        {
        //            conn.Open();
        //            int i = 0;
        //            foreach (var item in list)
        //            {
        //                item[dbr.Dept.SortID.Name] = 4 * i;

        //                dbr.Dept.Update(item).UseConnection(conn).Execute();
        //                i++;
        //            }
        //        }

        //        sw.Stop();

        //        return sw.ElapsedMilliseconds;
        //    };

        //    //125,22343 ; 340,32478 . 111,22181 ; 346,31679  . 105,22238 ; 297,29933
        //    return Content(ToSet().ToString() + "," + UseUpdateToServer().ToString() + " ; " + ToEntity().ToString() + "," + UseUpdate().ToString());
        //}
        ////
        //// GET: /Host/Home/

        //public ActionResult Index()
        //{
        //    var d = TReceptionRule.GetNewId();
        //    var d2 = TReceptionRule.GetNewId();
        //    return View();
        //}

        //}

        public ActionResult Comp()
        {
            var a = DateTime.Now.HasValue();
            var b = DateTime.MinValue.HasValue();
            return Content(b.ToString());
        }
    }
}

