
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

namespace MyWeb.Areas.Admin.Controllers
{
    public class DeptAnnexController : MyMvcController
    {

        public ActionResult List()
        {
            return View();
        }
        public class QueryModel : QueryModelBase
        {
            public string Name { get; set; }
        }
        public ActionResult Query(string uid, QueryModel Query)
        {
            WhereClip where = dbr.DeptAnnex.DeptID == MySession.Get(MySessionKey.DeptID);


            var d = dbr.DeptAnnex.Select()
                .LeftJoin(dbr.Annex, (a, b) => a.AnnexID == b.Id, o => new ColumnClip[] { o.FullName.As("AnnexFullName") })
                //.LeftJoin(dbo., dbo.DeptAnnexRule. == dbo..)
                .Skip(Query.skip)
                .Take(Query.take)
                .Where(where)
                .OrderBy(Query.sort)
                .ToMyOqlSet();

            d.Rows.All(o =>
            {
                o["AnnexFullName"] = o["AnnexFullName"].AsString().GetUrlFull();
                return true;
            });

            return d.LoadFlexiGrid();
        }

        public ActionResult Delete(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();
            string[] delIds = collection["query"].Split(',');
            try
            {
                var ret = new DeptAnnexRule().Delete(o => o.GetIdKey().In(delIds)).Execute();
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
            return View("Edit", dbr.DeptAnnex.FindById(uid));
        }
        public ActionResult Add(string uid)
        {
            return View("Edit", new DeptAnnexRule.Entity() { SortID = 100, DeptID = MySession.Get(MySessionKey.DeptID).AsInt() });
        }
        [HttpPost]
        public ActionResult Add(string uid, FormCollection collection)
        {
            var query = collection.ToDictionary();

            query.UpdateEnums(dbr.DeptAnnex);

            JsonMsg jm = new JsonMsg();
            try
            {
                string files = query[dbr.DeptAnnex.AnnexID.Name].AsString();
                var flg = false;
                foreach (var item in files.Split(','))
                {
                    if (item.HasValue() == false) continue;
                    query[dbr.DeptAnnex.AnnexID.Name] = item;
                    if (dbr.DeptAnnex.Insert(query).Execute() == 1)
                    {
                        flg |= true;
                    }
                    else
                    {
                        jm.msg = "有未插入记录！";
                    }
                }
                if (flg == false) { jm.msg = "没有更新记录"; }
                else if (flg && jm.msg.HasValue() == false) { }

            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;
        }
        public ActionResult Update(int uid)
        {
            return View("Edit", dbr.DeptAnnex.FindById(uid));
        }
        [HttpPost]
        public ActionResult Update(int uid, FormCollection collection)
        {
            var query = collection.ToDictionary();
            query.UpdateEnums(dbr.DeptAnnex);

            JsonMsg jm = new JsonMsg();
            try
            {
                if (new DeptAnnexRule().Update(query).Execute() == 1)
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
