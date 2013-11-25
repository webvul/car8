//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using DbEnt;
using MyOql;
//using MyOql.Helper;
//using MyCmn;
//using MyCon;
//using System.Text;
//using System.IO;
//using System.Configuration;
//using System.Collections;

//namespace MyWeb.Areas.Admin.Controllers
//{
//    public partial class AutoGenController : MyMvcController
//    {
//        public ActionResult Entity()
//        {
//            return View();
//        }

//        public ActionResult EntityProcAdd(string uid)
//        {
//            var dict = new Dictionary<string, object>();
//            dict["Name"] = uid;
//            dict["db"] = Request.QueryString["DbConfig"];
//            dict["Owner"] = Request.QueryString["Owner"];

//            return View("Entity_Proc_Card",
//                 new MyOql.MyOqlCodeGenSect.ProcCollection.ProcGroupCollection.ProcElement(dict));
//        }

//        [HttpPost]
//        public ActionResult EntityProcAdd(string uid, MyOql.MyOqlCodeGenSect.ProcCollection.ProcGroupCollection.ProcElement Model)
//        {
//            //Model.Paras = Model.Paras.Replace(" ", "").Replace("&nbsp;", "");


//            //var config = ConfigurationManager.OpenExeConfiguration(Server.MapPath(@"~/MyOql.CodeGen"));
//            //var con = config.Sections["MyOqlCodeGen"] as MyOql.MyOqlCodeGenSect;
//            //con.Procs.BaseAdd(Model);
//            //config.Save();


//            return new JsonMsg();
//        }

//        public ActionResult EntityProcUpdate(string uid)
//        {
//            MyOqlCodeGenSect sect = (MyOqlCodeGenSect)ConfigurationManager.GetSection("MyOqlCodeGen");

//            var ent = sect.Procs.FindProcByName(uid);
//            //var list = GetEntitys<MyOql.MyOqlCodeGenSect.ProcCollection.ProcGroupCollection.ProcElement>(o => o.Procs.GetEnumerator());

//            //var ent = list.First(o => o.Name == uid);

//            //if (ent.db.HasValue() == false) { ent.db = GetCodeGenConfig().Tables.db; }
//            //if (ent.Owner.HasValue() == false) { ent.Owner = GetCodeGenConfig().Tables.Owner; }

//            RenderJsVar("DbConfig", ent.db);
//            RenderJsVar("Owner", ent.Owner);

//            return View("Entity_Proc_Card", ent);
//        }


//        //[HttpPost]
//        //public ActionResult EntityProcUpdate(string uid, MyOql.MyOqlCodeGenSect.MyOqlCodeGenProcsCollection.MyOqlCodeGenProcElement Model)
//        //{
//        //    //UpdateEntity<MyOql.MyOqlCodeGenSect.MyOqlCodeGenProcsCollection.MyOqlCodeGenProcElement>
//        //    //    (o => o.Procs.GetEnumerator(), o =>
//        //    //    {
//        //    //        if (o.Name == Model.Name)
//        //    //        {
//        //    //            o.Descr = Model.Descr;
//        //    //            o.db = Model.db.Replace(" ", "").Replace("&nbsp;", "");
//        //    //            o.Owner = Model.Owner.Replace(" ", "").Replace("&nbsp;", "");
//        //    //            o.Paras = Model.Paras.Replace(" ", "").Replace("&nbsp;", "");
//        //    //            o.Return = Model.Return.Replace(" ", "").Replace("&nbsp;", "");
//        //    //            o.MapName = Model.Return.Replace(" ", "").Replace("&nbsp;", "");
//        //    //            return false;
//        //    //        }
//        //    //        return true;
//        //    //    });


//        //    return new JsonMsg();
//        //}

//        public ActionResult EntityTableQuery(string uid, FormCollection collection)
//        {
//            MyOqlCodeGenSect sect = (MyOqlCodeGenSect)ConfigurationManager.GetSection("MyOqlCodeGen");

//            var list = sect.Tables.GetConfig(o => true); // GetEntitys<MyOql.MyOqlCodeGenSect.TablesCollection.TableGroupCollection.TableElement>(o => o.Tables.GetEnumerator());

//            var query = collection["query"].FromJson<XmlDictionary<string, string>>();

//            var skip = (collection["page"].GetInt() - 1) * collection["rp"].GetInt();
//            var take = collection["rp"].GetInt();

//            if (query.GetOrDefault("Name").HasValue())
//            {
//                list = list.Where(o => o.Name.IndexOf(query.GetOrDefault("Name")) >= 0).ToList();
//            }

//            if (query.GetOrDefault("Descr").HasValue())
//            {
//                list = list.Where(o => o.Descr.IndexOf(query.GetOrDefault("Descr")) >= 0).ToList();
//            }

//            var count = list.Count();
//            list = list.Skip(skip).Take(take).ToList();
//            return dbr.PowerTable.LoadFlexiGrid(list, count);
//        }

//        public ActionResult EntityViewQuery(FormCollection collection)
//        {
//            MyOqlCodeGenSect sect = (MyOqlCodeGenSect)ConfigurationManager.GetSection("MyOqlCodeGen");

//            var list = sect.Tables.GetConfig(O => true);// GetEntitys<MyOql.MyOqlCodeGenSect.MyOqlCodeGenViewsCollection.MyOqlCodeGenViewElement>(o => o.Views.GetEnumerator());

//            var query = collection["query"].FromJson<XmlDictionary<string, string>>();
//            var skip = (collection["page"].GetInt() - 1) * collection["rp"].GetInt();
//            var take = collection["rp"].GetInt();

//            if (query.GetOrDefault("Name").HasValue())
//            {
//                list = list.Where(o => o.Name.IndexOf(query.GetOrDefault("Name")) >= 0).ToList();
//            }

//            if (query.GetOrDefault("Descr").HasValue())
//            {
//                list = list.Where(o => o.Descr.IndexOf(query.GetOrDefault("Descr")) >= 0).ToList();
//            }

//            var count = list.Count();
//            list = list.Skip(skip).Take(take).ToList();

//            return dbr.PowerTable.LoadFlexiGrid(list, count);
//        }

//        public ActionResult EntityProcQuery(FormCollection collection)
//        {
//            MyOqlCodeGenSect sect = (MyOqlCodeGenSect)ConfigurationManager.GetSection("MyOqlCodeGen");

//            var list = sect.Procs.GetConfig(o => true);// GetEntitys<MyOql.MyOqlCodeGenSect.MyOqlCodeGenProcsCollection.MyOqlCodeGenProcElement>(o => o.Procs.GetEnumerator());

//            var query = collection["query"].FromJson<XmlDictionary<string, string>>();

//            var skip = (collection["page"].GetInt() - 1) * collection["rp"].GetInt();
//            var take = collection["rp"].GetInt();


//            if (query.GetOrDefault("Name").HasValue())
//            {
//                list = list.Where(o => o.Name.IndexOf(query.GetOrDefault("Name")) >= 0).ToList();
//            }

//            if (query.GetOrDefault("Descr").HasValue())
//            {
//                list = list.Where(o => o.Descr.IndexOf(query.GetOrDefault("Descr")) >= 0).ToList();
//            }

//            var count = list.Count();
//            list = list.Skip(skip).Take(take).ToList();
//            return dbr.PowerTable.LoadFlexiGrid(list, count);
//        }


//        public ActionResult EntityTableDetail(string uid)
//        {
//            MyOqlCodeGenSect sect = (MyOqlCodeGenSect)ConfigurationManager.GetSection("MyOqlCodeGen");

//            //var list = GetEntitys<MyOql.MyOqlCodeGenSect.MyOqlCodeGenTablesCollection.MyOqlCodeGenTableElement>(o => o.Tables.GetEnumerator());
//            return View("Entity_Table_Card",sect.Tables.GetConfig(uid) ) ;// list.First(o => o.Name == uid));
//        }

//        public ActionResult EntityViewAdd(string uid)
//        {
//            return View("Entity_View_Card",
//                new MyOql.MyOqlCodeGenSect.MyOqlCodeGenViewsCollection.MyOqlCodeGenViewElement()
//                {
//                    Name = uid,
//                    db = Request.QueryString["DbConfig"],
//                    Owner = Request.QueryString["Owner"]
//                });
//        }

//        public ActionResult EntityViewUpdate(string uid)
//        {
//            MyOqlCodeGenSect sect = (MyOqlCodeGenSect)ConfigurationManager.GetSection("MyOqlCodeGen");

//            //var list = GetEntitys<MyOql.MyOqlCodeGenSect.MyOqlCodeGenViewsCollection.MyOqlCodeGenViewElement>(o => o.Views.GetEnumerator());

//            var ent = sect.Tables.GetConfig(uid);// list.First(o => o.Name == uid);

//            if (ent.db.HasValue() == false) { ent.db = GetCodeGenConfig().Tables.db; }
//            if (ent.Owner.HasValue() == false) { ent.Owner = GetCodeGenConfig().Tables.Owner; }

//            RenderJsVar("DbConfig", ent.db);
//            RenderJsVar("Owner", ent.Owner);

//            return View("Entity_View_Card", ent);
//        }

//        public ActionResult EntityViewDetail(string uid)
//        {
//            MyOqlCodeGenSect sect = (MyOqlCodeGenSect)ConfigurationManager.GetSection("MyOqlCodeGen");

//            //var list = GetEntitys<MyOql.MyOqlCodeGenSect.MyOqlCodeGenViewsCollection.MyOqlCodeGenViewElement>(o => o.Views.GetEnumerator());

//            var ent = sect.Tables.GetConfig(uid);// list.First(o => o.Name == uid);

//            RenderJsVar("DbConfig", ent.db);
//            RenderJsVar("Owner", ent.Owner);

//            return View("Entity_View_Card", ent);
//        }

//        [HttpPost]
//        public ActionResult EntityViewUpdate(string uid, MyOql.MyOqlCodeGenSect.TablesCollection.TableGroupCollection.TableElement Model)
//        {
//            //UpdateEntity<MyOql.MyOqlCodeGenSect.MyOqlCodeGenViewsCollection.MyOqlCodeGenViewElement>
//            //    (o => o.Views.GetEnumerator(), o =>
//            //    {
//            //        if (o.Name == Model.Name)
//            //        {
//            //            o.Descr = Model.Descr;
//            //            o.db = Model.db.Replace(" ", "").Replace("&nbsp;", "");
//            //            o.Enums = Model.Enums.Replace(" ", "").Replace("&nbsp;", "");
//            //            o.Owner = Model.Owner.Replace(" ", "").Replace("&nbsp;", "");
//            //            //o.Table = Model.Table.Replace(" ", "").Replace("&nbsp;", "");
//            //            o.MapName = Model.MapName.Replace(" ", "").Replace("&nbsp;", "");
//            //            return false;
//            //        }
//            //        return true;
//            //    });


//            return new JsonMsg();
//        }

//        [HttpPost]
//        public ActionResult EntityViewAdd(string uid, MyOql.MyOqlCodeGenSect.TablesCollection.TableGroupCollection.TableElement Model)
//        {
//            //Model.Enums = Model.Enums.Replace(" ", "").Replace("&nbsp;", "");

//            //var config = ConfigurationManager.OpenExeConfiguration(Server.MapPath(@"~/MyOql.CodeGen"));
//            //var con = config.Sections["MyOqlCodeGen"] as MyOql.MyOqlCodeGenSect;
//            //con.Views.BaseAdd(Model);
//            //config.Save();


//            return new JsonMsg();
//        }



//        public ActionResult EntityTableAdd(string uid)
//        {
//            return View("Entity_Table_Card",
//                new MyOql.MyOqlCodeGenSect.MyOqlCodeGenTablesCollection.MyOqlCodeGenTableElement()
//                {
//                    Name = uid,
//                    db = Request.QueryString["DbConfig"],
//                    Owner = Request.QueryString["Owner"]

//                });
//        }
//        public ActionResult EntityTableUpdate(string uid)
//        {
//            //var table = GetEntity<MyOql.MyOqlCodeGenSect.MyOqlCodeGenTablesCollection.MyOqlCodeGenTableElement>(o=> new o.Tables) ;
//            var ent = GetEntitys<MyOql.MyOqlCodeGenSect.MyOqlCodeGenTablesCollection.MyOqlCodeGenTableElement>(o => o.Tables.GetEnumerator())
//                    .First(o => o.Name == uid);

//            if (ent.db.HasValue() == false) { ent.db = GetCodeGenConfig().Tables.db; }
//            if (ent.Owner.HasValue() == false) { ent.Owner = GetCodeGenConfig().Tables.Owner; }

//            RenderJsVar("DbConfig", ent.db);
//            RenderJsVar("Owner", ent.Owner);

//            return View("Entity_Table_Card", ent);
//        }

//        public ActionResult EntityTableDelete(string uid)
//        {
//            return View("Entity_Table_Card");
//        }


//        private MyOqlCodeGenSect GetCodeGenConfig()
//        {
//            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap()
//            {
//                ExeConfigFilename =
//                    new DirectoryInfo(Server.MapPath("~/")).Parent.FullName + "/AfterBuildEvent/App.config"
//            }, ConfigurationUserLevel.None);//  Server.MapPath(@"~/MyOql.CodeGen"));

//            return config.Sections["MyOqlCodeGen"] as MyOql.MyOqlCodeGenSect;
//        }

//        private List<T> GetEntitys<T>(Func<MyOqlCodeGenSect, IEnumerator> func)
//            where T : class,new()
//        {
//            var con = GetCodeGenConfig();

//            var list = new List<T>();

//            var table = func(con);
//            while (table.MoveNext())
//            {
//                var tab = table.Current as T;
//                list.Add(tab);
//            }

//            return list;
//        }

//        private void UpdateEntity<T>(Func<MyOqlCodeGenSect, IEnumerator> func, Func<T, bool> updateFunc)
//            where T : class,new()
//        {
//            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(Server.MapPath(@"~/MyOql.CodeGen"));

//            var con = config.Sections["MyOqlCodeGen"] as MyOql.MyOqlCodeGenSect;


//            var table = func(con);
//            while (table.MoveNext())
//            {
//                var tab = table.Current as T;

//                if (updateFunc != null && updateFunc(tab) == false)
//                    break;
//            }

//            config.Save();
//        }



//        [HttpPost]
//        public ActionResult GetColumns(string Entity, string DbConfig, string Owner)
//        {
//            var cols = MyOql.Helper.MyOqlCodeGen.GetColumnsFromDb(DatabaseType.SqlServer, Entity, DbConfig, Owner);

//            return Json(cols.Select(o => o.Name));
//        }


//        [HttpPost]
//        public ActionResult EntityTableAdd(string uid, MyOql.MyOqlCodeGenSect.TablesCollection.TableGroupCollection.TableElement Model)
//        {
//            //Model.Enums = Model.Enums.Replace(" ", "").Replace("&nbsp;", "");
//            //Model.FKs = Model.FKs.Replace(" ", "").Replace("&nbsp;", "");

//            //var config = ConfigurationManager.OpenExeConfiguration(Server.MapPath(@"~/MyOql.CodeGen"));
//            //var con = config.Sections["MyOqlCodeGen"] as MyOql.MyOqlCodeGenSect;
//            //con.Tables.BaseAdd(Model);
//            //config.Save();


//            return new JsonMsg();
//        }

//        [HttpPost]
//        public ActionResult EntityTableUpdate(string uid, MyOql.MyOqlCodeGenSect.TablesCollection.TableGroupCollection.TableElement Model)
//        {
//            //UpdateEntity<MyOql.MyOqlCodeGenSect.MyOqlCodeGenTablesCollection.MyOqlCodeGenTableElement>
//            //    (o => o.Tables.GetEnumerator(), o =>
//            //    {
//            //        if (o.Name == Model.Name)
//            //        {
//            //            o.Descr = Model.Descr;
//            //            o.PKs = Model.PKs.Replace(" ", "").Replace("&nbsp;", "");
//            //            o.db = Model.db.Replace(" ", "").Replace("&nbsp;", "");
//            //            o.ComputeKeys = Model.ComputeKeys.Replace(" ", "").Replace("&nbsp;", "");
//            //            o.Enums = Model.Enums.Replace(" ", "").Replace("&nbsp;", "");
//            //            o.FKs = Model.FKs.Replace(" ", "").Replace("&nbsp;", "");
//            //            o.Owner = Model.Owner.Replace(" ", "").Replace("&nbsp;", "");
//            //            o.UniqueKey = Model.UniqueKey.Replace(" ", "").Replace("&nbsp;", "");
//            //            o.MapName = Model.MapName.Replace(" ", "").Replace("&nbsp;", "");
//            //            return false;
//            //        }
//            //        return true;
//            //    });


//            return new JsonMsg();
//        }

//        [HttpPost]
//        public ActionResult EntityTableDelete(FormCollection collection)
//        {
//            var config = ConfigurationManager.OpenExeConfiguration(Server.MapPath(@"~/MyOql.CodeGen"));
//            var con = config.Sections["MyOqlCodeGen"] as MyOql.MyOqlCodeGenSect;

//            collection["query"].Split(',').All(o =>
//                {
//                    con.Tables.BaseRemove(o);
//                    return true;
//                });

//            config.Save();

//            return new JsonMsg();
//        }
//        [HttpPost]
//        public ActionResult EntityProcDelete(FormCollection collection)
//        {
//            var config = ConfigurationManager.OpenExeConfiguration(Server.MapPath(@"~/MyOql.CodeGen"));
//            var con = config.Sections["MyOqlCodeGen"] as MyOql.MyOqlCodeGenSect;

//            collection["query"].Split(',').All(o =>
//                {
//                    con.Procs.BaseRemove(o);
//                    return true;
//                });

//            config.Save();

//            return new JsonMsg();
//        }


//        [HttpPost]
//        public ActionResult EntityViewDelete(FormCollection collection)
//        {
//            var config = ConfigurationManager.OpenExeConfiguration(Server.MapPath(@"~/MyOql.CodeGen"));
//            var con = config.Sections["MyOqlCodeGen"] as MyOql.MyOqlCodeGenSect;

//            collection["query"].Split(',').All(o =>
//            {
//                con.Views.BaseRemove(o);
//                return true;
//            });

//            config.Save();

//            return new JsonMsg();
//        }
//        public ActionResult ForeignKeyQuery(string entity, string db, string owner)
//        {
//            var ent = GetEntitys<MyOql.MyOqlCodeGenSect.MyOqlCodeGenTablesCollection.MyOqlCodeGenTableElement>(o => o.Tables.GetEnumerator())
//                .Where(o => o.Name == entity)
//                .First();

//            var list = new List<XmlDictionary<string, string>>();

//            ent.FKs.AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).All(o =>
//                {
//                    var fks = o.Split("=:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
//                    if (fks.Length < 3) return true;

//                    var dict = new XmlDictionary<string, string>();
//                    dict["Column"] = fks[0];
//                    dict["RefTable"] = fks[1];
//                    dict["RefColumn"] = fks[2];

//                    list.Add(dict);

//                    return true;
//                });

//            var cols = MyOqlCodeGen.GetColumnsFromDb(DatabaseType.SqlServer, entity, db, owner);

//            cols.All(o =>
//            {
//                if (list.Count(e => e["Column"] == o.DbName) == 0)
//                {
//                    var dict = new XmlDictionary<string, string>();
//                    dict["Column"] = o.DbName;
//                    dict["RefTable"] = "";
//                    dict["RefColumn"] = "";

//                    list.Add(dict);
//                }
//                return true;
//            });


//            return dbr.PowerTable.LoadFlexiGrid(list, list.Count);
//        }

//        public ActionResult EnumKeyQuery(string entity, string db, string owner)
//        {
//            var ent = GetEntitys<MyOql.MyOqlCodeGenSect.MyOqlCodeGenTablesCollection.MyOqlCodeGenTableElement>(o => o.Tables.GetEnumerator())
//                    .Where(o => o != null && o.Name == entity)
//                    .FirstOrDefault();

//            string enm = "";

//            if (ent == null)
//            {
//                var viw = GetEntitys<MyOql.MyOqlCodeGenSect.MyOqlCodeGenViewsCollection.MyOqlCodeGenViewElement>(o => o.Tables.GetEnumerator())
//                    .Where(o => o != null && o.Name == entity)
//                    .FirstOrDefault();

//                if (viw != null)
//                {
//                    enm = viw.Enums.ToString();
//                }
//            }
//            else enm = ent.Enums.ToString();



//            var list = new List<XmlDictionary<string, string>>();

//            enm.AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).All(o =>
//            {
//                var fks = o.Split("=:".ToCharArray());
//                if (fks.Length < 2) return true;

//                var dict = new XmlDictionary<string, string>();
//                dict["Column"] = fks[0];
//                dict["Type"] = fks[1];
//                dict["MapName"] = (fks.Length == 3 && fks[2].HasValue()) ? fks[2] : fks[0];

//                list.Add(dict);

//                return true;
//            });


//            var cols = MyOqlCodeGen.GetColumnsFromDb(DatabaseType.SqlServer, entity, db, owner);

//            cols.All(o =>
//                {
//                    if (list.Count(e => e["Column"] == o.DbName) == 0)
//                    {
//                        var dict = new XmlDictionary<string, string>();
//                        dict["Column"] = o.DbName;
//                        dict["Type"] = "";
//                        dict["MapName"] = o.Name;

//                        list.Add(dict);
//                    }
//                    return true;
//                });


//            return dbr.PowerTable.LoadFlexiGrid(list, list.Count);

//        }

//    }
//}
