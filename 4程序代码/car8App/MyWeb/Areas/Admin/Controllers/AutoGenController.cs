using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyOql;
using MyCmn;
using MyCon;
using System.IO;
using System.Xml;
using System.Web.Compilation;
using DbEnt;

namespace MyWeb.Areas.Admin.Controllers
{
    [NoPowerFilter]
    [NoLogFilter]
    public partial class AutoGenController : MyMvcController
    {
        public const string EntityNameSpace = "DbEnt";

        private XmlDocument xmlDocForEntity = null;
        public string GetSummary(string MetaMemberType)
        {
            string xpath = string.Format(@"/doc/members/member[@name='{0}']/summary", MetaMemberType);

            if (xmlDocForEntity == null)
            {
                xmlDocForEntity = new XmlDocument();
                xmlDocForEntity.Load(Server.MapPath("~/bin/DbEnt.xml"));
            }

            if (xmlDocForEntity == null) return MetaMemberType.Split('.').Last();

            var node = xmlDocForEntity.SelectSingleNode(xpath);
            if (node == null) return MetaMemberType.Split('.').Last();
            return node.InnerText.AsString(node.Value.AsString(MetaMemberType.Split('.').Last())).Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">RuleType</param>
        /// <returns></returns>
        public string GetSummary(Type type)
        {
            //Type type = BuildManager.GetType(string.Format(@"{2}.{0}{1}Rule", (Group.HasValue() ? Group + "Group." : ""), Entity, EntityNameSpace), false);
            if (type == null) return type.FullName;
            return GetSummary("T:" + type.FullName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">RuleType</param>
        /// <param name="Member"></param>
        /// <returns></returns>
        public string GetSummary(Type type, string Member)
        {
            //Type type = BuildManager.GetType(string.Format(@"{2}.{0}{1}Rule", (Group.HasValue() ? Group + "Group." : ""), Entity, EntityNameSpace), false);
            if (type == null) return Member;
            return GetSummary("P:" + type.FullName + "." + Member);
        }

        public ActionResult Sql(string uid)
        {
            object objSql = null;


            return View("Entity", objSql);
        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult Index()
        {
            //var type = typeof(DeptRule).Assembly.GetTypes();
            //var ents = type.Where(o => o.Namespace == "MyOql" && o.IsSubclassOf(typeof(RuleBase))).Select(o => o.Name.Remove(o.Name.Length - 4)).ToArray();

            return View();
        }
        [HttpPost]
        public ActionResult QueryGroup(FormCollection collection)
        {
            var query = collection["query"];

            var dict = new StringDict();


            var groupType = typeof(dbr).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

            var ary = groupType.Where(o => o.PropertyType.FullName.EndsWith("GroupClass")).Select(o => o.Name).ToArray();

            return new TextHelperJsonResult(ary);
        }


        [HttpPost]
        public ActionResult QueryEnt(FormCollection collection)
        {
            var query = collection["query"];
            var group = collection["Group"];

            var dict = new StringDict();

            if (group.HasValue())
            {
                var groupType = typeof(dbr).GetProperty(group);
                GodError.Check(groupType == null, () => "找不到实体组：" + group);

                var groupEnt = groupType.GetValue(null, null);
                groupType.PropertyType.GetProperties().All(o =>
                    {
                        var ent = o.GetValue(groupEnt, null) as RuleBase;
                        if (ent != null)
                        {
                            dict[ent.GetDbName()] = ent.GetName();
                        }
                        return true;
                    });
            }
            else
            {
                var type = typeof(dbr).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                type.All(o =>
                 {
                     var ent = o.GetValue(null, null) as RuleBase;
                     if (ent != null)
                     {
                         dict[ent.GetDbName()] = ent.GetName();
                     }
                     return true;
                 });
            }

            var ret = new StringDict(dict.Where(o => o.Value.Contains(query)));
            if (ret.Count < 50)
            {
                var left = dict.Minus(ret);

                ret = new StringDict(ret.Concat(left.Take(50 - ret.Count)));
            }

            return new TextHelperJsonResult(ret);
        }


        [HttpPost]
        public ActionResult WriteFile(string uid, FormCollection collection)
        {
            string area = Request.QueryString["Area"];
            string group = Request.QueryString["Group"];
            string entity = Request.QueryString["Entity"];

            JsonMsg jm = new JsonMsg();

            ListCard();
            var mol = ViewData.Model as string[];
            Func<string, string, bool, string> Write = (fileName, Content, Override) =>
                {
                    try
                    {
                        fileName = fileName.ResolveUrl();
                        var fi = new FileInfo(fileName);

                        if (System.IO.Directory.Exists(fi.DirectoryName) == false)
                        {
                            System.IO.Directory.CreateDirectory(fi.DirectoryName);
                        }


                        if (fi.Exists)
                        {
                            if (Override)
                            {
                                System.IO.File.SetAttributes(fileName, System.IO.FileAttributes.Normal);
                                System.IO.File.Delete(fileName);
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }

                        System.IO.File.AppendAllText(fileName, Content);
                    }
                    catch (Exception e)
                    {
                        return e.Message.GetSafeValue();
                    }
                    return string.Empty;
                };

            jm.msg = Write("~/Areas/【if:HasArea】$Area$【else】App【fi】/Views/$Entity$/List.aspx"
                .TmpIf()
                .Var("HasArea", () => area.HasValue())
                .EndIf()

                .Replace("$Area$", area)
                .Replace("$Entity$", entity), mol[0], true);
            if (jm.msg.HasValue())
            {
                return jm;
            }

            jm.msg = Write("~/Areas/【if:HasArea】$Area$【else】App【fi】/Views/$Entity$/Card.aspx"
                .TmpIf()
                .Var("HasArea", () => area.HasValue())
                .EndIf()

                .Replace("$Area$", area)
                .Replace("$Entity$", entity), mol[1], true);
            if (jm.msg.HasValue())
            {
                return jm;
            }

            jm.msg = Write("~/Areas/【if:HasArea】$Area$【else】App【fi】/Controllers/$Entity$Controller.cs"
                .TmpIf()
                .Var("HasArea", () => area.HasValue())
                .EndIf()

                .Replace("$Area$", area)
                .Replace("$Entity$", entity), mol[2], false);
            if (jm.msg.HasValue())
            {
                return jm;
            }

            jm.msg = Write("~/Areas/【if:HasArea】$Area$【else】App【fi】/Controllers/$Entity$Controller_ListCard.cs"
                .TmpIf()
                .Var("HasArea", () => area.HasValue())
                .EndIf()

                .Replace("$Area$", area)
                .Replace("$Entity$", entity), mol[3], true);
            if (jm.msg.HasValue())
            {
                return jm;
            }

            {
                var bizPath = new DirectoryInfo(Server.MapPath("~/")).Parent.FullName + @"\MyBiz\" + area.AsString("App") + @"\" + entity + "Biz.cs";
                jm.msg = Write(bizPath, mol[4], false);
                if (jm.msg.HasValue())
                {
                    return jm;
                }
            }

            {
                var bizPath = new DirectoryInfo(Server.MapPath("~/")).Parent.FullName + @"\MyBiz\" + area.AsString("App") + @"\" + entity + "Biz_Extend.cs";
                jm.msg = Write(bizPath, mol[5], true);
                if (jm.msg.HasValue())
                {
                    return jm;
                }
            }

            return jm;
        }

        public ActionResult ListCard()
        {
            string area = Request.QueryString["Area"];
            string group = Request.QueryString["Group"];
            //数据库的表名称
            string dbEntity = Request.QueryString["Entity"];
            string Ref_Style = Request.QueryString["Ref_Style"];

            var entity = new dbr().GetMyOqlEntity(dbEntity).GetName();

            return View(new string[] { 
                GetListAspx(area, group, dbEntity,Ref_Style),
                GetCardAspx(area, group, dbEntity,Ref_Style),
                
                            @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using DbEnt;
using MyOql;
using MyCon;
using MyBiz;;
using MyBiz.【if:HasArea】$Area$【else】App【fi】;

namespace MyWeb.Areas.【if:HasArea】$Area$【else】App【fi】.Controllers
{
    public partial class $Entity$Controller : MyMvcController
	{
        
    }
}
"
                .TmpIf()
                .Var("HasArea",()=>area.HasValue())
                .EndIf()
                    
                .Replace("$Area$",area)
                .Replace("$Entity$",entity)
                 , 
                @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using MyOql;
using MyCon;
using MyBiz;
using DbEnt;
using MyBiz.【if:HasArea】$Area$【else】App【fi】;

namespace MyWeb.Areas.【if:HasArea】$Area$【else】App【fi】.Controllers
{
	public partial class $Entity$Controller : MyMvcController
	{
        $$
    }
}
"
                .TmpIf()
                .Var("HasArea",()=>area.HasValue())
                .EndIf()

                .Replace("$Area$",area)
                .Replace("$Entity$",entity)
                .Replace("$$", GetListCs(area,group, entity) + GetEditCs(area,group, entity) ) , 
 
                @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using DbEnt;
using MyOql;
using MyBiz;


namespace MyBiz.$Area$
{
	public partial class $Entity$Biz 
	{
         
    }
}
"
                //.TmpIf()
                //.Var("HasGroup",()=>group.HasValue())
                //.EndIf()

                .Replace("$Group$",group)
                .Replace("$Area$",area.HasValue()? area :"App")
                .Replace("$Entity$",entity), 

          @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using DbEnt;
using MyOql;
using MyBiz;


namespace MyBiz.$Area$
{
	public partial class $Entity$Biz 
	{
        public class QueryModel: QueryBase
        {
            public string Name { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
        }

        $$
    }
}
"
                //.TmpIf()
                //.Var("HasGroup",()=>group.HasValue())
                //.EndIf()

                .Replace("$Group$",group)
                .Replace("$Area$",area.HasValue()?area:"App")
                .Replace("$Entity$",entity)
                .Replace("$$", GetBizCs(group,entity) ) 
            });


        }

    }
}
