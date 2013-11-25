
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
    [NoPowerFilter]
    public class DictController : MyMvcController
    {
        public ActionResult GetKeys()
        {
            return Content(string.Join(",", EnumHelper.ToEnumList<DictKeyEnum>().Select(o => o.ToString()).ToArray()));
        }
        public ActionResult List()
        {
            return View();
        }

        public class QueryModel : QueryModelBase
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
        public ActionResult Query(string uid, QueryModel Query)
        {
            WhereClip where = new WhereClip();// dbr.Dict.PID == uid.GetInt();

            if (Query.Key.HasValue())
            {
                where &= dbr.Dict.Key.Like("%" + Query.Key + "%");
            }

            if (Query.Value.HasValue())
            {
                where &= dbr.Dict.Value.Like("%" + Query.Value + "%");
            }

            var d = dbr.Dict.Select()
                //.LeftJoin(dbo., dbo.DictRule. == dbo..)
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
                var ret = new DictRule().Delete(o => o.GetIdKey().In(delIds)).Execute();
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
            return View("Edit", dbr.Dict.FindById(uid));
        }
        public ActionResult Add()
        {
            return View("Edit", new DictRule.Entity() { SortID = 100 });
        }
        [HttpPost]
        public ActionResult Add(FormCollection collection)
        {
            var query = collection.ToDictionary();
            query.UpdateEnums(dbr.Dict._);

            JsonMsg jm = new JsonMsg();
            try
            {
                if (new DictRule().Insert(query).Execute() == 0)
                { jm.msg = "未新增记录"; }
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;
        }
        public ActionResult Update(int uid)
        {
            return View("Edit", dbr.Dict.FindById(uid));
        }
        [HttpPost]
        public ActionResult Update(int uid, FormCollection collection)
        {
            var query = collection.ToDictionary();
            query.UpdateEnums(dbr.Dict._);

            JsonMsg jm = new JsonMsg();
            try
            {
                if (new DictRule().Update(query).Execute() == 1)
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
