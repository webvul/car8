//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using MyCmn;
//using MyOql;
//using MyCon;
//using DbEnt;
//using MyBiz.App;


//namespace MyWeb.Areas.App.Controllers
//{
//    public partial class TStandardRoleController : MyMvcController
//    {

//        [HttpPost]
//        public ActionResult Clean(string uid, FormCollection collection)
//        {
//            JsonMsg jm = new JsonMsg();

//            TStandardRoleBiz.Clean(collection["query"].Split(','));

//            return jm;
//        }


//        public ActionResult ListForMenuQuery(FormCollection collection)
//        {
//            var queryDict = collection["query"].FromJson<XmlDictionary<string, string>>();
//            string menuQuery = queryDict["Menu"];
//            string standardRoleQuery = queryDict["StandardRole"];

//            int skip = collection["skip"].AsInt();
//            int take = collection["take"].AsInt();

//            string order = "Menu";
//            var isAsc = collection["sortorder"] == "asc";

//            var roles = dbr.TStandardRole
//                .Select()
//                .Where(o => o.Name.Like("%" + standardRoleQuery + "%"))
//                .ToDictList();

//            var menus = new List<XmlDictionary<string, string>>();
//            var index = 0;

//            roles.All(o =>
//                {

//                    var power = new PowerJson(o[dbr.TStandardRole.Power.Name].AsString()).Row.View.GetOrDefault("Menu");

//                    if (power != null)
//                    {
//                        foreach (var pos in power.ToPositions())
//                        {
//                            var menu = dbr.Menu.FindFirst(m => m.Id == pos, () => new XmlDictionary<string, object>());
//                            if (menu != null)
//                            {
//                                var dict = new XmlDictionary<string, string>();
//                                dict[dbr.TStandardRole.StandardRoleId.Name] = o[dbr.TStandardRole.StandardRoleId.Name].AsString();
//                                dict[dbr.TStandardRole.Name.Name] = o[dbr.TStandardRole.Name.Name].AsString();

//                                dict["Id"] = (index++).AsString();

//                                dict["Menu"] = menu.GetOrDefault("Text").AsString();


//                                menus.Add(dict);
//                            }
//                        }
//                    }
//                    return true;
//                });



//            if (menuQuery.HasValue())
//            {
//                menus = menus.Where(o => o["Menu"].AsString().Contains(menuQuery)).ToList();
//            }

//            if (standardRoleQuery.HasValue())
//            {
//                menus = menus.Where(o => o["Name"].AsString().Contains(standardRoleQuery)).ToList();
//            }


//            int count = menus.Count;

//            //客户端列表的列名，Name 需要是 "Name", "Menu" 
//            if (order.HasValue())
//            {
//                if (isAsc)
//                {
//                    menus.Sort((a, b) => StringComparer.CurrentCulture.Compare(a[order], b[order]));
//                }
//                else
//                {
//                    menus.Sort((a, b) => StringComparer.CurrentCulture.Compare(b[order], a[order]));
//                }
//            }


//            menus = menus.Skip(skip).Take(take).ToList();


//            return dbr.TStandardRole.LoadFlexiGrid(menus, count);

//        }
//    }
//}
