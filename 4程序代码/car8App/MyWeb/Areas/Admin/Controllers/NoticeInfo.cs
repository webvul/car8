
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using MyOql;
using MyCon;
using MyBiz;
using DbEnt;
using MyBiz.Admin;

namespace MyWeb.Areas.Admin.Controllers
{
    public class NoticeInfoController : MyMvcController
    {
        public ActionResult GetNoticeTypes()
        {
            var data = dbr.NoticeType
                .Select(o => new ColumnClip[] { o.Id, o.Name })
                .Where(o => o.DeptID == MySession.Get(MySessionKey.DeptID))
                .ToDictList();

            var strDict = new StringDict();
            data.All(o =>
            {
                strDict[o["Id"].ToString()] = o["Name"].ToString();
                return true;
            });

            return new TextHelperJsonResult(strDict);
        }

        public ActionResult List()
        {
            return View();
        }
        public ActionResult Query(string uid, NoticeBiz.NoticeInfoQueryModel Query)
        {
            WhereClip where = new WhereClip();
            var NoticeTypeIDs = dbr.NoticeType.Select(o => o.Id).Where(o => o.DeptID == MySessionKey.DeptID.Get() & o.UserID == MySessionKey.UserID.Get()).ToEntityList(0).ToArray();

            where &= dbr.NoticeInfo.NoticeTypeID.In(NoticeTypeIDs);

            Query.NoticeTypeID.HasValue(o => where &= dbr.NoticeInfo.NoticeTypeID == o);
            Query.Name.HasValue(o => where &= dbr.NoticeInfo.Name.Like(string.Format("%{0}%", o)));
            return dbr.NoticeInfo.Select(o => new ColumnClip[] { o.Id, o.Name, o.Descr, o.UpdateTime, o.SortID, ("t" + o.NoticeTypeID).As("Type") })
                      .LeftJoin(dbr.NoticeType, (a, b) => a.NoticeTypeID == b.Id, o => new ColumnClip[] { o.Name.As(o.GetName() + ".Name") })
                      .Skip(Query.skip)
                      .Take(Query.take)
                      .Where(where)
                      .OrderBy(Query.sort)
                      .ToMyOqlSet()
                      .LoadFlexiGrid();




        }

        public ActionResult Delete(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();
            string[] delIds = collection["query"].Split(',');
            try
            {
                var ret = new NoticeInfoRule().Delete(o => o.GetIdKey().In(delIds)).Execute();
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

        public ActionResult Detail(string uid)
        {
            var noticeId = uid.AsInt();
            //RenderProductAnnexVar(noticeId);

            return View("Edit", dbr.NoticeInfo.FindById(noticeId));
        }

        public ActionResult Add(string uid)
        {
            return View("Edit", new NoticeInfoRule.Entity() { SortID = 100 });
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(string uid, DbEnt.NoticeInfoRule.Entity Enti)
        {
            Enti.UpdateTime = DateTime.Now;

            JsonMsg jm = new JsonMsg();
            try
            {
                if (Enti.Logo.HasValue())
                    if (dbr.NoticeInfo.Insert(Enti).Execute() > 0)
                    { }
                    else { jm.msg = "未新增记录"; }
                else
                {
                    if (dbr.NoticeInfo.Insert(o => o.NoticeTypeID == Enti.NoticeTypeID & o.Name == Enti.Name & o.Descr == Enti.Descr & o.SortID == Enti.SortID).Execute() > 0)
                    { }
                    else { jm.msg = "未新增记录"; }
                }
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;

        }
        public ActionResult Update(int uid)
        {
            return View("Edit", dbr.NoticeInfo.FindById(uid));
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(string uid, DbEnt.NoticeInfoRule.Entity Enti)
        {
            JsonMsg jm = new JsonMsg();
            try
            {
                Enti.UpdateTime = DateTime.Now;
                if (Enti.Logo.HasValue())
                    if (dbr.NoticeInfo.Update(Enti).Execute() > 0)
                    { }
                    else { jm.msg = "未新增记录"; }
                else
                {
                    if (dbr.NoticeInfo.Update(o => o.NoticeTypeID == Enti.NoticeTypeID & o.Name == Enti.Name & o.Descr == Enti.Descr & o.SortID == Enti.SortID & o.UpdateTime == Enti.UpdateTime, o => o.Id == Enti.Id).Execute() > 0)
                    { }
                    else { jm.msg = "未新增记录"; }
                }

            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;

        }

        public ActionResult Hot()
        {
            return View();
        }

        [HttpPost]
        public ActionResult HotDelete(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();
            string[] delIds = collection["query"].Split(',');
            try
            {
                var ret = dbr.NoticeShowCase.Delete(o => o.NoticeID.In(delIds)).Execute();
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

        [HttpPost]
        public ActionResult HotQuery(string uid, FormCollection collection)
        {
            string sortName = collection["sortname"];
            string sortOrder = collection["sortorder"];

            WhereClip where = dbr.NoticeShowCase.DeptID == MySession.Get(MySessionKey.DeptID);

            //var count = dbr.EnterpriseShowCase.FindScalar(o => o.Count(), o => { return where; }).GetInt();

            OrderByClip order = new OrderByClip(sortName);
            if (sortOrder == "desc") order.IsAsc = false;

            var d = dbr.NoticeShowCase.Select(o => o.NoticeID)
                .Join(dbr.NoticeInfo, (a, b) => a.NoticeID == b.Id, o => new ColumnClip[] { 
                    o.Id,
                    o.Name ,
                    o.Descr,
                    o.SortID
                })
                .LeftJoin(dbr.NoticeType, (a, b) => dbr.NoticeInfo.NoticeTypeID == b.Id, o => new ColumnClip[]{
                    o.Name.As("NoticeType")
                })
                .Where(where)
                .OrderBy(order)
                .ToMyOqlSet()
                .LoadFlexiGrid();
            return d;
        }
        [HttpPost]
        public ActionResult HotAdd(FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();

            //1.查出不在数据库的部分.
            var NoticeIds = collection["NoticeIds"].Split(',');


            var insertIds = dbr.NoticeShowCase.Select(o => o.NoticeID).Where(o => o.NoticeID.In(NoticeIds)).ToEntityList("");

            NoticeIds = NoticeIds.Where(o => insertIds.Contains(o) == false).ToArray();
            try
            {
                foreach (var item in NoticeIds)
                {
                    dbr.NoticeShowCase.Insert(o => o.NoticeID == item & o.DeptID == MySession.Get(MySessionKey.DeptID)).Execute();
                }

            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }
            return jm;
        }

    }
}
