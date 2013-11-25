
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using MyOql;
using MyCon;
using MyBiz;
using MyBiz.Sys;
using System.Configuration;
using DbEnt;
using MyWeb.Areas.Admin.Models;
using System.Transactions;
using MyBiz.Admin;
using DbEnt;

namespace MyWeb.Areas.Admin.Controllers
{
    public class PersonController : MyMvcController
    {
        public ActionResult Uploaded(string uid, string UserId, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();
            if (uid == "MyImg")
            {
                Dictionary<ColumnClip, object> dict = new Dictionary<ColumnClip, object>();
                UserId = UserId.AsString(null) ?? MySession.Get(MySessionKey.UserID);
                try
                {
                    //dbr.PersonAnnex.AutoSave(o =>
                    //    {
                    //        dict[o.AnnexID] = collection["files"].AsInt();
                    //        dict[o.UserID] = UserId;
                    //        dict[o.Key] = "MyImg";
                    //        return dict;
                    //    }, o => new ColumnClip[] { o.UserID, o.Key });

                    //jm.data = dbr.Annex.FindById(dict[dbr.PersonAnnex.AnnexID].AsInt()).FullName.GetUrlFull();
                }
                catch (Exception ee) { jm.msg = ee.Message.GetSafeValue(); }
                return jm;
            }
            return jm;
        }

        [HttpPost]
        public ActionResult EditPassword(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();
            if (dbr.PEditPassword(collection["UserID"], collection["Password"]) == 1)
            {

                return jm;
            }
            else
            {
                jm.msg = "没有更新记录.";
                return jm;
            }
        }

        [HttpPost]
        public ActionResult UpdatePassword(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();
            var dict = collection.ToDictionary<string>();

            if (dbr.PLogin(dict["UserId"], dict["OriPassword"]) == null)
            {
                jm.msg = "您输入的原始密码不正确,请重新输入.";
                return jm;
            }

            if (dbr.PEditPassword(collection["UserId"], collection["Password"]) == 1)
            {
                return jm;
            }
            else
            {
                jm.msg = "没有更新记录.";
                return jm;
            }
        }
        public ActionResult List()
        {
            return View();
        }
        public ActionResult Query(string uid, PersonBiz.QueryModel query)
        {
            return PersonBiz.Query(query).LoadFlexiGrid();
        }
        public ActionResult Delete(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();
            string[] delIds = collection["query"].Split(',');
            try
            {
                var ret = new PersonRule().Delete(o => o.GetIdKey().In(delIds)).Execute();
                if (ret == delIds.Count())
                {

                }
                else { jm.msg = string.Format("删除了 {0}/{1} 条记录.", ret, delIds.Count()); }
            }
            catch (Exception ee)
            {
                //jm.msg = ee.Message.GetSafeValue();
                jm.msg = "请先清除人员与区域与角色的关系";
            }

            return jm;
        }

        public ActionResult Detail(string uid)
        {
            return View("Edit", dbr.Person.FindByUserID(uid));
        }
        public ActionResult Add(string uid)
        {
            return View("Edit", new PersonRule.Entity());
        }
        [HttpPost]
        public ActionResult Add(string uid, FormCollection collection)
        {
            var query = collection.ToDictionary();
            query.UpdateEnums(dbr.Person._);

            //if (query["RelateType"].AsString() == "Region")
            //{
            //    query["CinemaID"] = null;
            //}
            //else
            //{
            //    query["RegionID"] = null;
            //}
            if (query.ContainsKey("Logo") && query["Logo"].AsInt() == 0) query.Remove("Logo");

            JsonMsg jm = new JsonMsg();
            try
            {
                using (var tran = new TransactionScope())
                {
                    var exci = dbr.Person.Select(o => o.UserID).Where(o => o.UserID == query["UserID"].AsString()).ToEntityList(o => o._);
                    if (exci.Count > 0)
                    {
                        //throw new GodError("ID重复,未新增记录");
                        jm.msg = "ID重复,未新增记录";
                        return jm;
                    }
                    if (new PersonRule().Insert(query).Execute() == 0)
                    {
                        //throw new GodError("未新增记录");
                        jm.msg = "未新增记录";
                        return jm;
                    }

                    dbr.PEditPassword(query[dbr.Person.UserID.Name].AsString(), ConfigurationManager.AppSettings[ConfigKey.InitPassword.AsString()]);
                    jm.msg = "用户： " + query[dbr.Person.UserID.Name].AsString() + " 已添加 ,初始密码为：" + ConfigurationManager.AppSettings[ConfigKey.InitPassword.AsString()];
                    jm.data = "1";


                    var user = query["UserID"].AsString();


                    //var role = query["Roles"].AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    ////更新角色
                    //if (role.Length > 0)
                    //{
                    //    UserRoleRule.Entity urole = new UserRoleRule.Entity();

                    //    urole.UserID = user;
                    //    int countRole = 0;
                    //    role.All(o =>
                    //    {
                    //        urole.RoleID = o.AsInt();
                    //        countRole += dbr.UserRole.Insert(urole).Execute();

                    //        return true;
                    //    });
                    //    if (countRole != role.Length)
                    //    {
                    //        //throw new GodError("未新增记录");
                    //        jm.msg = "部分用户角色信息未更新";
                    //        return jm;
                    //    }
                    //}


                    //var dept = query["Dept"].AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    //if (dept.Length > 0)
                    //{
                    //    var udept = new UserDeptRule.Entity();
                    //    udept.UserID = user;

                    //    int countDept = 0;
                    //    dept.All(o =>
                    //          {
                    //              udept.DeptID = o.AsInt();
                    //              countDept += dbr.UserDept.Insert(udept).Execute();
                    //              return true;
                    //          });

                    //    if (countDept != dept.Length)
                    //    {
                    //        jm.msg = "部分用户部门信息未更新";
                    //        return jm;
                    //    }
                    //}

                    tran.Complete();
                }
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }
            return jm;
        }
        public ActionResult Update(string uid)
        {
            var UserId = uid.AsString(null) ?? MySession.Get(MySessionKey.UserID);
            if (UserId.HasValue() == false) return new ContentResult() { Content = string.Format("<a href='{0}' target='_top'>会话超时，请登录!</a>", "~/Login.aspx".GetUrlFull()) };
            RenderJsVar("UserId", UserId);
            //RenderJsVar("IsAdmin", MySession.GetMyRole().Contains(o => o.Role.Contains("Admin")).AsString());
            return View("Edit", dbr.Person.FindByUserID(UserId));
        }
        [HttpPost]
        public ActionResult Update(string uid, DbEnt.PersonRule.Entity Enti)
        {


            JsonMsg jm = new JsonMsg();
            try
            {
                if (Enti.Logo.HasValue())
                    dbr.Person.Update(Enti).Execute();
                else
                    dbr.Person.Update(o => o.Name == Enti.Name & o.Sex == Enti.Sex &o.Mobile == Enti.Mobile  & o.Logo == null & o.Qq == Enti.Qq & o.Msn == Enti.Msn & o.Email == Enti.Email, o => o.UserID == Enti.UserID).Execute();                
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;
        }


        ///// <summary>
        ///// 如果一个Region 是另一个Region 的子项,则去除该Region.
        ///// </summary>
        ///// <param name="region"></param>
        ///// <returns></returns>
        //private int[] TidyRegin(string[] region)
        //{
        //    List<int> list = new List<int>();
        //    Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();
        //    region.All(o =>
        //        {
        //            dict[o.AsInt()] = dbr.FGetAllRegions(o.AsInt()).Select(r => r.Id).ToEntityList(0);
        //            return true;
        //        });

        //    dict.All(o =>
        //        {
        //            if (dict.All(s =>
        //             {
        //                 if (s.Key == o.Key) return true;

        //                 if (s.Value.Contains(o.Key))
        //                 {
        //                     return false;
        //                 }
        //                 else return true;
        //             }))
        //            {
        //                list.Add(o.Key);
        //            }
        //            return true;
        //        });

        //    return list.ToArray();
        //}

        //[HttpPost]
        //public ActionResult UpdatePower(string uid, FormCollection collection)
        //{
        //    JsonMsg jm = new JsonMsg();


        //    var data = collection["power"].FromJson<Dictionary<string, int[]>>();
        //    PowerJson power = new PowerJson(data, null,null);


        //    dbr.Person.Update(o => o.Power == power.ToString()).Where(o => o.UserID == uid).Execute();

        //    return jm;
        //}

        [HttpPost]
        public ActionResult SaveRole(string UserID, FormCollection collection)
        {
            var roleIds = collection["Role"].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(o => o.AsInt());
            JsonMsg jm = new JsonMsg();
            //if (dbr.Person
            //    .Update(o => o.Roles == MyBigInt.CreateByBitPositons(roleIds).ToString())
            //    .Where(o => o.UserID == UserID)
            //    .Execute() != 1)
            //{
            //    jm.msg = "未更新用户的角色.";
            //}

            return jm;
        }
    }
}
