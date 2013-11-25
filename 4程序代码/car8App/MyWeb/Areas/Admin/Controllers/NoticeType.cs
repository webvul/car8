
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
    public class NoticeTypeController : MyMvcController
    {

        public ActionResult List()
        {
            return View();
        }
        public ActionResult Query(string uid, NoticeBiz.NoticeTypeQueryModel Query)
        {

            WhereClip where = dbr.NoticeType.DeptID == MySession.Get(MySessionKey.DeptID);
            where &= dbr.NoticeType.UserID == MySession.Get(MySessionKey.UserID);


            Query.Name.HasValue(o => where &= dbr.NoticeType.Name.Like(string.Format("%{0}%", o)));

            return dbr.NoticeType.Select(o => new ColumnClip[] { o.Id, o.Pid, o.Name, o.Descr, o.SortID, o.AddTime })
                   .Skip(Query.skip)
                   .Take(Query.take)
                   .Where(where)
                   .OrderBy(Query.sort)
                   .ToMyOqlSet()
                   .LoadFlexiGrid();
            //.LoadFlexiTreeGrid(dbr.NoticeType.Id.Name, dbr.NoticeType.Pid.Name);

        }

        public ActionResult Delete(string uid, string IDs)
        {
            JsonMsg jm = new JsonMsg();
            string[] delIds = IDs.Split(',');
            try
            {
                var ret = new NoticeTypeRule().Delete(o => o.GetIdKey().In(delIds)).Execute();
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
            return View("Edit", dbr.NoticeType.FindById(uid));
        }
        public ActionResult Add(string uid)
        {
            return View("Edit", new NoticeTypeRule.Entity() { SortID = 100, AddTime = DateTime.Now });
        }
        [HttpPost]
        public ActionResult Add(string uid, NoticeTypeRule.Entity Enti)
        {
            Enti.UserID = MySession.Get(MySessionKey.UserID);
            Enti.DeptID = MySession.Get(MySessionKey.DeptID).AsInt();
            Enti.AddTime = DateTime.Now.AsDateTime();

            if (Enti.Pid > 0)
            {
                var pnt = dbr.NoticeType.FindById(Enti.Pid);
                Enti.Wbs = pnt.Wbs + "," + pnt.Id;
            }
            else
            {
                Enti.Wbs = "0";
            }

            JsonMsg jm = new JsonMsg();
            try
            {
                if (dbr.NoticeType.Insert(Enti).Execute() > 0)
                { }
                else { jm.msg = "未新增记录"; }
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;

        }
        public ActionResult Update(int uid)
        {
            return View("Edit", dbr.NoticeType.FindById(uid));
        }
        [HttpPost]
        public ActionResult Update(string uid, NoticeTypeRule.Entity Enti)
        {


            var Pid = Enti.Pid;
            if (Pid.HasValue())
            {
                var PWbs = dbr.NoticeType.FindById(Pid).Wbs;
                Enti.Wbs = PWbs + "," + Enti.Id;
            }
            else
                Enti.Wbs = Enti.Id.AsString();

            JsonMsg jm = new JsonMsg();
            try
            {
                if (dbr.NoticeType.Update(Enti).Execute() == 1)
                { }
                else { jm.msg = "未更新记录"; }
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;

        }
    }
}
