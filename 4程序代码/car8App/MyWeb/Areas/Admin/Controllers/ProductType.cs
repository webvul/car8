
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
    public class ProductTypeController : MyMvcController
    {

        public ActionResult List()
        {
            return View();
        }
        public ActionResult Query(string uid, ProductionBiz.ProductTypeQueryModel Query)
        {

            WhereClip where = dbr.ProductType.DeptID == MySession.Get(MySessionKey.DeptID);
            where &= dbr.ProductType.UserID == MySession.Get(MySessionKey.UserID);

            Query.Name.HasValue(o => where &= dbr.ProductType.Name.Like(string.Format("%{0}%", o)));


            return dbr.ProductType.Select(o => new ColumnClip[] { o.Id, o.Pid, o.Name, o.Descr, o.SortID, o.AddTime })
                   .Skip(Query.skip)
                   .Take(Query.take)
                   .Where(where)
                   .OrderBy(Query.sort)
                   .ToMyOqlSet()
                   .LoadFlexiGrid();
            //.LoadFlexiTreeGrid(dbr.NoticeType.Id.Name, dbr.NoticeType.Pid.Name);
        }

        public ActionResult Delete(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();
            string[] delIds = collection["query"].Split(',');
            try
            {
                var ret = new ProductTypeRule().Delete(o => o.GetIdKey().In(delIds)).Execute();
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
            return View("Edit", dbr.ProductType.FindById(uid));
        }
        public ActionResult Add(string uid)
        {
            return View("Edit", new ProductTypeRule.Entity() { SortID = 100, AddTime = DateTime.Now });
        }

        [HttpPost]
        public ActionResult Add(string uid, ProductTypeRule.Entity productType)
        {
            productType.UserID = MySession.Get(MySessionKey.UserID);
            productType.DeptID = MySession.Get(MySessionKey.DeptID).AsInt();
            productType.AddTime = DateTime.Now.AsString().AsDateTime();

            //更新Wbs
            if (productType.Pid > 0)
            {
                var ppt = dbr.ProductType.FindById(productType.Pid);
                productType.Wbs = ppt.Wbs + "," + ppt.Id;
            }
            else { productType.Wbs = "0"; }

            JsonMsg jm = new JsonMsg();
            try
            {
                if (new ProductTypeRule().Insert(productType).Execute() > 0)
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
            return View("Edit", dbr.ProductType.FindById(uid));
        }
        [HttpPost]
        public ActionResult Update(int uid, FormCollection collection)
        {
            var query = collection.ToDictionary();
            query.UpdateEnums(dbr.ProductType._);

            JsonMsg jm = new JsonMsg();
            try
            {
                if (new ProductTypeRule().Update(query).Execute() == 1)
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
