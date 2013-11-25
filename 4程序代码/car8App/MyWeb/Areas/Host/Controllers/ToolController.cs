using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using MyCon;
using MyOql;
using MyWeb.Areas.Host.Models;
using DbEnt;

namespace MyWeb.Areas.Host.Controllers
{
    public partial class ToolController : MyMvcController
    {
        public ActionResult ViewCache(string obj)
        {
            var em = CacheHelper.GetKeys();
            var dict = new List<MyOqlCacheModel>();

            foreach (var k in em)
            {
                var m = new MyOqlCacheModel();
                m.Key = k;

                if (k.StartsWith("MyOql|Sql|"))
                {
                    m.Type = "sql";
                    m.Object = k.Slice(10, k.IndexOf('|', 11)).AsString();
                }
                else if (k.StartsWith("MyOql|"))
                {
                    m.Type = "oqlid";
                    m.Object = k.Slice(6, k.IndexOf('|', 6)).AsString();
                }
                else
                {
                    m.Type = "app";
                    m.Object = "";
                }

                if (m.Object.Contains(obj.AsString().Trim()))
                {
                    m.Value = Dump(CacheHelper.Get(k, TimeSpan.MinValue, () => (object)null));
                    dict.Add(m);
                }
            }
            return View(dict.Take(100));
        }

        [HttpPost]
        public ActionResult DeleteCache()
        {
            var key = Request.Form["key"];
            if (key.HasValue())
            {
                CacheHelper.Remove(key);
            }
            return new JsonMsg();
        }

        [HttpPost]
        public ActionResult DeleteCacheObj(string obj)
        {
            var model = (ViewCache(obj) as ViewResult).ViewData.Model as IEnumerable<MyOqlCacheModel>;

            if (model != null) model.All(o =>
              {
                  CacheHelper.Remove(o.Key);
                  return true;
              });

            return new JsonMsg();
        }


        /// <summary>
        /// 移除缓存。
        /// </summary>
        /// <param name="uid">表示 Table 或 View 的数据库名称</param>
        /// <param name="Data">表示批量清除缓存,表示 StringDict 的 Json</param>
        /// <param name="Table">表示 Table 或 View 的数据库名称</param>
        /// <param name="Id">表示 Table 或 View 的数据Id , 为空则删除全部数据</param>
        /// <returns></returns>
        public ActionResult RemoveCache(string uid, string Data, string Table, string Id)
        {
            if (Request.QueryString["Data"] != null)
            {
                var data = Request.QueryString["Data"].FromJson<StringDict>();
                if (data != null && data.Count > 0)
                {
                    var loseTable = new List<string>();
                    foreach (var table in data.Keys)
                    {
                        var rule = dbo.Event.Idbr.GetMyOqlEntity(table);
                        if (rule == null)
                        {
                            loseTable.Add(table);
                            continue;
                        }

                        dbo.Event.OnCacheRemoveById(MyContextClip.CreateBySelect(rule), data[table]);
                    };

                    if (loseTable.Count > 0)
                    {
                        return new JsonMsg() { msg = "找不到表或视图 : " + string.Join(",", loseTable.ToArray()) };
                    }
                    else return new JsonMsg();
                }
            }

            //新起一个作用域.
            {
                var table = uid.AsString(Request.QueryString["Table"]);
                var id = Request.QueryString["Id"];

                if (table.HasValue())
                {
                    var rule = dbo.Event.Idbr.GetMyOqlEntity(table);
                    if (rule == null) { return new JsonMsg() { msg = "找不到表或视图 : " + table }; }

                    dbo.Event.OnCacheRemoveById(MyContextClip.CreateBySelect(rule), id);
                }
            }

            return new JsonMsg();
        }

        private string Dump(object obj)
        {
            try
            {
                return obj.ToJson().Slice(0, 400).AsString();
            }
            catch
            {
                List<string> sl = new List<string>();

                Type type = obj.GetType();
                foreach (PropertyInfo item in type.GetProperties())
                {
                    sl.Add(item.Name + ":" + item.GetValue(obj, null).AsString());
                }

                return "{" + string.Join(",", sl.ToArray()) + "}";
            }

        }

        public ActionResult ViewAppConfig()
        {
            return View();
        }

        public MyOqlCodeGenSect GetSect()
        {
            var d = Server.MapPath(@"~/");
            var p = new DirectoryInfo(d).Parent.FullName + @"\MyTool\bin\Debug\MyTool.exe";

            var set = (MyOqlCodeGenSect)ConfigurationManager.OpenExeConfiguration(p).GetSection("MyOqlCodeGen");

            return set;
        }

        [HttpPost]
        public ActionResult AppConfigQuery(QueryModelBase query)
        {
            var sect = GetSect();

            List<StringDict> dict = new List<StringDict>();

            dict.Add(new StringDict("Id:RootTable,Pid:,Name:Table,Type:Root"));
            dict.Add(new StringDict("Id:RootView,Pid:,Name:View,Type:Root"));
            dict.Add(new StringDict("Id:RootFunction,Pid:,Name:Function,Type:Root"));
            dict.Add(new StringDict("Id:RootProc,Pid:,Name:Proc,Type:Root"));

            sect.Table.ToMyList(o => o as MyOqlCodeGenSect.TableCollection.TableGroupCollection).All(tabGoup =>
                {
                    var groupName = tabGoup.Name.HasValue(n => "Group" + n, n => "GroupTable");

                    dict.Add(new StringDict("Id:" + groupName + ",Pid:RootTable,Type:Group,Name:" + tabGoup.Name.HasValue(n => n, n => "(根组)")));


                    tabGoup.ToMyList(t => t as MyOqlCodeGenSect.TableCollection.TableGroupCollection.TableElement).All(tab =>
                        {
                            dict.Add(new StringDict("Id:" + tab.Name + ",Pid:" + groupName + ",Name:" + tab.Name));
                            return true;
                        });
                    return true;
                });
            sect.View.ToMyList(o => o as MyOqlCodeGenSect.ViewCollection.ViewGroupCollection).All(tabGoup =>
            {
                var groupName = tabGoup.Name.HasValue(n => "Group" + n, n => "GroupView");

                dict.Add(new StringDict("Id:" + groupName + ",Pid:RootView,Type:Group,Name:" + tabGoup.Name.HasValue(n => n, n => "(根组)")));


                tabGoup.ToMyList(t => t as MyOqlCodeGenSect.ViewCollection.ViewGroupCollection.ViewElement).All(tab =>
                {
                    dict.Add(new StringDict("Id:" + tab.Name + ",Pid:" + groupName + ",Name:" + tab.Name));
                    return true;
                });
                return true;
            });
            sect.Proc.ToMyList(o => o as MyOqlCodeGenSect.ProcCollection.ProcGroupCollection).All(tabGoup =>
            {
                var groupName = tabGoup.Name.HasValue(n => "Group" + n, n => "GroupProc");

                dict.Add(new StringDict("Id:" + groupName + ",Pid:RootProc,Type:Group,Name:" + tabGoup.Name.HasValue(n => n, n => "(根组)")));


                tabGoup.ToMyList(t => t as MyOqlCodeGenSect.ProcCollection.ProcGroupCollection.ProcElement).All(tab =>
                {
                    dict.Add(new StringDict("Id:" + tab.Name + ",Pid:" + groupName + ",Name:" + tab.Name));
                    return true;
                });
                return true;
            });
            sect.Function.ToMyList(o => o as MyOqlCodeGenSect.FunctionCollection.TableGroupCollection).All(tabGoup =>
            {
                var groupName = tabGoup.Name.HasValue(n => "Group" + n, n => "GroupFunction");

                dict.Add(new StringDict("Id:" + groupName + ",Pid:RootFunction,Type:Group,Name:" + tabGoup.Name.HasValue(n => n, n => "(根组)")));


                tabGoup.ToMyList(t => t as MyOqlCodeGenSect.FunctionCollection.TableGroupCollection.TableElement).All(tab =>
                {
                    dict.Add(new StringDict("Id:" + tab.Name + ",Pid:" + groupName + ",Name:" + tab.Name));
                    return true;
                });
                return true;
            });


            var retVal = new ConstTable().LoadFlexiTreeGrid(dict, 4, "Id", "Pid");

            return retVal;
        }

        public ActionResult AppConfigDetail(string Name)
        {
            var tab = GetSect().Table.GetConfig(Name);
            if (tab.HasValue()) return View("AppConfigCard", tab);

            return View("AppConfigCard");
        }
        public ActionResult AppConfigRootDetail(string uid)
        {
            return View("AppConfigRootCard");
        }

        public ActionResult AppConfigGroupDetail(string uid)
        {
            return View("AppConfigGroupCard");
        }

        [HttpPost]
        public ActionResult AppConfigColumnQuery(string Name)
        {
            //根据实体名称，找出其中的列信息。
            var dict = new List<StringDict>();

            var tab = GetSect().Table.GetConfig(Name);


            Action<string, string, string> addInfo = (name, key, val) =>
            {
                if (dict.Contains(d => d["Name"] == name))
                {
                    dict.First(d => d["Name"] == name)[key] = val;
                }
                else
                {
                    dict.Add(new StringDict("Name:" + name + "," + key + ":" + val));
                }
            };


            tab.PKs.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).All(o =>
                {
                    addInfo(o, "Pk", "true");
                    return true;
                });

            tab.AutoIncreKey.HasValue(o =>
                {
                    addInfo(o, "AutoIncre", "true");
                    return true;
                });
            tab.UniqueKey.HasValue(o =>
                {
                    addInfo(o, "Unique", "true");
                    return true;
                });

            tab.FKs.All(o =>
                {
                    addInfo(o.Column, "Fk", o.ToString());
                    return true;
                });

            tab.Enums.All(o =>
                {
                    addInfo(o.Name, "Enum", o.EnumType);
                    o.TranslateName.HasValue(n => { addInfo(o.Name, "MapName", n); return true; });
                    return true;
                });

            return new ConstTable().LoadFlexiGrid(dict, dict.Count);
        }


         
    }
}
