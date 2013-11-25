
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
    public class ProductDetailController : MyMvcController
    {

        public ActionResult List()
        {
            return View();
        }

        public class QueryModel : QueryModelBase
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public ActionResult Query(string uid, QueryModel Query)
        {

            WhereClip where = dbr.ProductDetail.ProductID == uid;

            if (Query.Name.HasValue())
            {
                where &= dbr.ProductDetail.Key.Like("%" + Query.Name + "%");
            }
            if (Query.Value.HasValue())
            {
                where &= dbr.ProductDetail.Value.Like("%" + Query.Value + "%");
            }


            var d = dbr.ProductDetail.Select()
                //.LeftJoin(dbo., dbo.ProductDetailRule. == dbo..)
                .Skip(Query.skip)
                .Take(Query.take)
                .Where(where)
                .OrderBy(Query.sort)
                 .ToMyOqlSet();

            var keyIndex = d.Columns.IndexOf(o => o.Name == dbr.ProductDetail.Key.Name);
            var valueIndex = d.Columns.IndexOf(o => o.Name == dbr.ProductDetail.Value.Name);
            //var keyZhIndex = d.Columns.IndexOf(o => o.Name == dbr.ProductDetail.ZhKey.Name);
            //var valueZhIndex = d.Columns.IndexOf(o => o.Name == dbr.ProductDetail.ZhValue.Name);

            //d.Rows.All(o =>
            //{
            //    o[keyIndex] = MyHelper.Lang.GetRes(o[keyZhIndex], o[keyIndex]);
            //    o[valueIndex] = MyHelper.Lang.GetRes(o[valueZhIndex], o[valueIndex]);
            //    return true;
            //});


            return d.LoadFlexiGrid();
        }

        public ActionResult Delete(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();
            string[] delIds = collection["query"].Split(',');
            try
            {
                var ret = new ProductDetailRule().Delete(o => o.GetIdKey().In(delIds)).Execute();
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
            return View("Edit", dbr.ProductDetail.FindById(uid));
        }
        public ActionResult Add(string uid)
        {
            return View("Edit", new ProductDetailRule.Entity() { ProductID = uid.AsInt() });
        }
        [HttpPost]
        public ActionResult Add(string uid, ProductDetailRule.Entity ent)
        {
            //var query = collection.ToDictionary();
            //query.UpdateEnums(dbr.ProductDetail._);

            JsonMsg jm = new JsonMsg();
            try
            {
                ent.ProductID = uid.AsInt();
                ent.UserID = MySession.Get(MySessionKey.UserID);
                ent.AddTime = DateTime.Now;

                //query[dbr.ProductDetail.ProductID.Name] = uid;
                //query[dbr.ProductDetail.UserID.Name] = MySession.Get(MySessionKey.UserID);
                //query[dbr.ProductDetail.AddTime.Name] = DateTime.Now.ToString();
                if (dbr.ProductDetail.Insert(ent).Execute() == 1)
                {
                    dbr.ProductDetail.Update(o => o.SortID == o.Id * 10).Where(o => o.Id == ent.Id).Execute();
                }
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
            return View("Edit", dbr.ProductDetail.FindById(uid));
        }
        [HttpPost]
        public ActionResult Update(int uid, ProductDetailRule.Entity ent)
        {
            //var query = collection.ToDictionary();
            //query.UpdateEnums(dbr.ProductDetail._);

            JsonMsg jm = new JsonMsg();
            try
            {
                ent.UserID = MySessionKey.UserID.Get();
                ent.AddTime = DateTime.Now;
                ent.ProductID = dbr.ProductDetail.FindById(uid).ProductID;

                if (dbr.ProductDetail
                    .Update(ent)
                    .Where(o => o.Id == uid)
                    .Execute() == 1)
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
