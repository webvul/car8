
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
using System.Transactions;
using DbEnt;

namespace MyWeb.Areas.Admin.Controllers
{
    public class DeptController : MyMvcController
    {
        [HttpPost]
        public ActionResult R(string id)
        {
            return new TextHelperJsonResult(id, ".", DateTime.Now.ToString());
        }

        public ActionResult Upload(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();
            var file = collection["files"].Split(',')[0].AsInt();
            var deptID = collection["DeptID"].AsInt();

            //yy 注释 20111112 加班
            //DeptAnnexKeyEnum key = 0;

            //if (uid == "MyLogo")
            //{
            //    key = DeptAnnexKeyEnum.Logo;
            //}
            //else if (uid == "MyTitle")
            //{
            //    key = DeptAnnexKeyEnum.Title;
            //}

            //if (dbr.DeptAnnex
            //        .Insert(o => o.AnnexID == file & o.Key == key & o.DeptID == deptID & o.SortID == 100)
            //        .Execute() == 1)
            //{


            //    return jm;
            //}
            //else jm.msg = "未插入值.";

            return jm;
        }

        public ActionResult GetSkins()
        {
            var ret = dbr.GetDictValues(DictKeyEnum.SkinEnum);
            return Json(ret);
        }

        public ActionResult List()
        {
            return View();
        }


        public class CommunityQueryModel : QueryModelBase
        {
            public string CommName { get; set; }
        }

        public ActionResult CommunityQuery(string uid, CommunityQueryModel Query)
        {
            WhereClip where = new WhereClip();

            Query.CommName.HasValue(o => where &= dbr.Community.CommName.Like(string.Format("%{0}%", o)));


            return dbr.Community.Select(o => new ColumnClip[] { o.CommID, o.CommName })
                  .Where(where)
                  .Skip(Query.skip)
                  .Take(Query.take)
                  .OrderBy(Query.sort)
                  .ToMyOqlSet()
                  .LoadFlexiGrid();
        }

        public class QueryModel : QueryModelBase
        {
            public string Name { get; set; }
        }
        public ActionResult Query(string UserId, string MyUser, string ooo, QueryModel Query)
        {


            WhereClip where = new WhereClip();

            if (Query.Name.HasValue())
            {
                where &= dbr.Dept.Name.Like("%" + Query.Name + "%");
            }

            if (MyUser.AsBool())
            {
                UserId = MySession.Get(MySessionKey.UserID);
            }

            if (UserId.HasValue())
            {
                where &= dbr.Dept.Id.In(dbr.Person.Select(o => o.DeptID).Where(o => o.UserID == UserId));
            }

            //var count = dbr.Dept.FindScalar(o => o.Count(), o => { return where; }).GetInt();


            return dbr.Dept.Select()
                //.LeftJoin(dbo., dbo.DeptRule. == dbo..)
                .Skip(Query.skip)
                .Take(Query.take)
                .Where(where)
                .OrderBy(Query.sort)
                .ToMyOqlSet(SelectOption.WithCount)
                .LoadFlexiGrid();
        }

        public ActionResult Delete(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();
            string[] delIds = collection["query"].Split(',');
            try
            {
                new PersonRule().Delete(o => o.DeptID.In(delIds)).Execute();
                var ret = new DeptRule().Delete(o => o.GetIdKey().In(delIds)).Execute();
                dbr.DeptCommunity.Delete(o => o.DeptID.In(delIds)).Execute();
                if (ret == delIds.Count())
                {

                }
                else { jm.msg = string.Format("删除了 {0}/{1} 条记录.", ret, delIds.Count()); }
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;
        }

        public ActionResult Detail(int uid)
        {
            var CommIDs = dbr.DeptCommunity.Select(o => o.CommID).Where(o => o.DeptID == MySessionKey.DeptID.Get() & o.RefUser == MySessionKey.UserID.Get()).ToEntityList(0).Distinct().ToArray();
            var CommNames = dbr.Community.Select(o => o.CommName).Where(o => o.CommID.In(CommIDs)).ToEntityList("").Distinct().Join(",");
            RenderJsVar("CommNames", CommNames);
            return View("Edit", dbr.Dept.FindById(uid));
        }
        public ActionResult Add(string uid)
        {
            return View("Edit", new DeptRule.Entity() { SortID = 100, EndTime = DateTime.Now.AddYears(1) });
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(string uid, DeptRule.Entity dept)
        {
            JsonMsg jm = new JsonMsg();
            try
            {
                var MaxDeptID = dbr.Dept.Select(o => (o.Id.Max().IsNull(0) + 1)).ToEntity(0).AsInt();
                dept.Id = MaxDeptID;
                if (dbr.Dept.Insert(dept)
                    .UsePostKeys()
                    .AppendColumn(o => o.Pid).RemoveColumns(o =>
                {
                    var list = new List<ColumnClip>();
                    if (dept.Logo == 0)
                    {
                        list.Add(o.Logo);
                    }
                    if (dept.Title == 0)
                    {
                        list.Add(o.Title);
                    }
                    if (dept.TitleExtend == 0)
                    {
                        list.Add(o.TitleExtend);
                    }
                    return list;
                }).Execute() == 1)
                {
                    #region 添加小区关联
                    foreach (var CommID in uid.Split(","))
                    {
                        dbr.DeptCommunity.Insert(o => o.CommID == CommID & o.DeptID == MaxDeptID & o.RefUser == dept.Name & o.RefTime == DateTime.Now & o.IfPushComm == 1).Execute();
                    }
                    #endregion

                    dbr.Person.Insert(o => o.UserID == dept.WebName & o.Name == dept.WebName & o.DeptID == MaxDeptID).Execute();
                    dbr.PEditPassword(dept.WebName, ConfigKey.InitPassword.Get());
                    jm.data = ConfigKey.InitPassword.Get();
                }
                else { jm.msg = "未新增记录"; }
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;
        }
        public ActionResult Update(string uid)
        {
            if (uid.HasValue() == false)
            {
                MyHelper.RequestContext.RouteData.Values["uid"] = MySession.Get(MySessionKey.DeptID).AsString();
            }
            var CommIDs = dbr.DeptCommunity.Select(o => o.CommID).Where(o => o.DeptID == MySessionKey.DeptID.Get() & o.RefUser == MySessionKey.DeptName.Get()).ToEntityList(0).Distinct().ToArray();
            var CommNames = dbr.Community.Select(o => o.CommName).Where(o => o.CommID.In(CommIDs)).ToEntityList("").Distinct();
            RenderJsVar("CommNames", CommNames.Join(","));
            RenderJsVar("CommIDs", CommIDs.Join(","));

            return View("Edit", dbr.Dept.FindById(
                 (uid.AsString(null) ?? MySession.Get(MySessionKey.DeptID)).AsInt()
                 ));
        }

        public ActionResult AdminUpdate(string uid)
        {
            if (uid.HasValue() == false)
            {
                MyHelper.RequestContext.RouteData.Values["uid"] = MySession.Get(MySessionKey.DeptID).AsString();
            }
            var CommIDs = dbr.DeptCommunity.Select(o => o.CommID).Where(o => o.DeptID == uid).ToEntityList(0).Distinct().ToArray();
            var CommNames = dbr.Community.Select(o => o.CommName).Where(o => o.CommID.In(CommIDs)).ToEntityList("").Distinct();
            RenderJsVar("CommNames", CommNames.Join(","));
            RenderJsVar("CommIDs", CommIDs.Join(","));

            return View("Edit", dbr.Dept.FindById(
                 (uid.AsString(null) ?? MySession.Get(MySessionKey.DeptID)).AsInt()
                 ));
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(string uid, DeptRule.Entity ent)
        {
            JsonMsg jm = new JsonMsg();
            try
            {
                if (dbr.Dept
                    .Update(ent)
                    .UsePostKeys()
                    .RemoveColumn(o => ent.Logo == 0 ? o.Logo : null)
                    .RemoveColumn(o => ent.Title == 0 ? o.Title : null)
                    .RemoveColumn(o => ent.TitleExtend == 0 ? o.TitleExtend : null)
                    .Execute() == 1)
                {
                    #region 添加小区关联
                    dbr.DeptCommunity.Delete(o => o.DeptID == ent.Id).Execute();
                    foreach (var CommID in uid.Split(","))
                    {
                        dbr.DeptCommunity.Insert(o => o.CommID == CommID & o.DeptID == ent.Id & o.RefUser == ent.Name & o.RefTime == DateTime.Now & o.IfPushComm == 1).Execute();
                    }
                    #endregion
                }
                else { jm.msg = "未更新记录"; }
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdminUpdate(string uid, DeptRule.Entity ent)
        {
            JsonMsg jm = new JsonMsg();
            try
            {
                if (dbr.Dept
                    .Update(ent)
                    .UsePostKeys()
                    .RemoveColumn(o => ent.Logo == 0 ? o.Logo : null)
                    .RemoveColumn(o => ent.Title == 0 ? o.Title : null)
                    .RemoveColumn(o => ent.TitleExtend == 0 ? o.TitleExtend : null)
                    .Execute() == 1)
                {
                    #region 添加小区关联
                    dbr.DeptCommunity.Delete(o => o.DeptID == ent.Id).Execute();
                    foreach (var CommID in uid.Split(","))
                    {
                        dbr.DeptCommunity.Insert(o => o.CommID == CommID & o.DeptID == ent.Id & o.RefUser == ent.Name & o.RefTime == DateTime.Now & o.IfPushComm == 1).Execute();
                    }
                    #endregion
                }
                else { jm.msg = "未更新记录"; }
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;
        }


        [HttpPost]
        public ActionResult UpdatePower(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();


            var data = collection["power"].FromJson<Dictionary<string, int[]>>();
            PowerJson power = new PowerJson(data, null, null, null);

            dbr.Dept.Update(o => o.Power == power.ToString()).Where(o => o.Id == uid).Execute();

            return jm;
        }
    }
}
