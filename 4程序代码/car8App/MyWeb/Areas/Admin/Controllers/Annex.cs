
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using DbEnt;
using MyOql;
using MyCon;
using MyBiz;


namespace MyWeb.Areas.Admin.Controllers
{
    /// <summary>
    /// ffff
    /// </summary>
    public class AnnexController : MyMvcController
    {
        public ActionResult ImgList()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public class ImgQueryModel
        {
            public int page { get; set; }
            public string Name { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }

        public class ImgAnnexResult
        {
            public int id { get; set; }
            public string FullUrl { get; set; }
            public string name { get; set; }
        }

        public ActionResult ImgQuery(ImgQueryModel query)
        {
            var where = new WhereClip();
            query.Name.HasValue(o => where &= dbr.Annex.Name.Like("%" + o + "%"));
            query.StartTime.HasValue(o => where &= dbr.Annex.AddTime >= o);
            query.EndTime.HasValue(o => where &= dbr.Annex.AddTime <= o);

            var list = dbr.Annex.Select(o => new Columns() { o.Id, o.Name, o.FullName })
                .Where(o => o.UserID == MySessionKey.UserID.Get() & o.Ext.In(".jpg", ".bmp", ".png", ".gif", ".jpeg") & where)
                .Skip((query.page - 1) * 20)
                .Take(20)
                .OrderBy(o => o.AddTime.Desc)
                .ToMyOqlSet(SelectOption.WithCount);

            return list.ToJsonResult();
        }

        public class QueryModel : QueryModelBase
        {

        }
        public ActionResult Query(string uid, QueryModel Query)
        {
            AnnexRule _AnnexRule = new AnnexRule();
            WhereClip where = new WhereClip();

            var d = _AnnexRule.Select()
                //.LeftJoin(dbo., dbo.AnnexRule. == dbo..)
                .Skip(Query.skip)
                .Take(Query.take)
                .Where(where)
                .OrderBy(Query.sort)
                .ToMyOqlSet()
                .LoadFlexiGrid();
            return d;
        }

        public ActionResult Delete(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();
            string[] delIds = collection["query"].Split(',');
            try
            {
                var ret = new AnnexRule().Delete(o => o.GetIdKey().In(delIds)).Execute();
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
            return View("Edit", dbr.Annex.FindById(uid));
        }
        public ActionResult Add(string uid)
        {
            return View("Edit", new AnnexRule.Entity() { AddTime = DateTime.Now });
        }
        [HttpPost]
        public ActionResult Add(string uid, FormCollection collection)
        {
            var query = collection.ToDictionary();
            query.UpdateEnums(dbr.Annex._);

            JsonMsg jm = new JsonMsg();
            try
            {
                if (new AnnexRule().Insert(query).Execute() == 1)
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
            return View("Edit", dbr.Annex.FindById(uid));
        }
        [HttpPost]
        public ActionResult Update(int uid, FormCollection collection)
        {
            var query = collection.ToDictionary();
            query.UpdateEnums(dbr.Annex._);

            JsonMsg jm = new JsonMsg();
            try
            {
                if (new AnnexRule().Update(query).Execute() == 1)
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
