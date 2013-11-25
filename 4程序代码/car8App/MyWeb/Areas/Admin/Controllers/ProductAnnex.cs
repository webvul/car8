
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using MyOql;
using MyCon;
using DbEnt;

namespace MyWeb.Areas.Admin.Controllers
{
    public class ProductAnnexController : MyMvcController
    {
        
        public ActionResult Uploaded(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();
            var productId = collection["ProductID"].AsInt();
            int[] Files = collection["files"].Split(',').Select(o => o.AsInt()).Where(o => o > 0).ToArray();
            //string imgtype = collection["imgtype"];
            try
            {
                foreach (int item in Files)
                {
                    var dict = new XmlDictionary<ColumnClip, object>();
                    dict[dbr.ProductAnnex.AnnexID] = item;
                    dict[dbr.ProductAnnex.ProductID] = productId;
                    dict[dbr.ProductAnnex.SortID] = 10;
                    if (dbr.ProductAnnex.Insert(dict).Execute() < 0)
                    {
                        jm.msg = "插入记录失败.";
                        return jm;
                    }
                }


                //if (dbr.ProductAnnex.FindScalar(o => o.Count(), o => o.ProductID == uid & o.Key == ProductAnnexKeyEnum.MinImg).GetInt() == 0)
                //{
                //    dbr.ProductAnnex.Update(o => o.Key == ProductAnnexKeyEnum.MinImg).Where(o => o.ID == dict[o.ID]).Execute();
                //}
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }
            return jm;
        }

        public ActionResult GetDeptWebName(string uid)
        {
            JsonMsg jm = new JsonMsg();
            jm.data = dbr.ProductInfo.Select(o => { return (ColumnClip)null; })
                .Join(dbr.ProductType, (a, b) => a.ProductTypeID == b.Id)
                .Join(dbr.Dept, (a, b) => dbr.ProductType.DeptID == b.Id, o => o.WebName)
                .Where(o => o.Id == uid)
                .ToScalar().AsString();



            return jm;
        }

        public ActionResult List(string uid)
        {
            return View();
        }

        public class QueryModel : QueryModelBase
        {
        }

        public ActionResult Query(string uid, QueryModel Query)
        {
            WhereClip where = dbr.ProductAnnex.ProductID == uid;

            var d = dbr.ProductAnnex.Select()
                .LeftJoin(dbr.Annex, (a, b) => a.AnnexID == b.Id, o => new ColumnClip[] { o.FullName.As("AnnexFullName") })
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

        public ActionResult Delete(int uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();
            string[] delIds = collection["query"].Split(',');
            try
            {
                List<string> listID = new List<string>();

                foreach (var item in delIds)
                {
                    listID.Add(dbr.ProductAnnex.FindById(item.AsInt()).GetAnnex().FullName);
                }

                var ret = new ProductAnnexRule().Delete(o => o.GetIdKey().In(delIds)).Execute();
                if (ret == delIds.Count())
                {
                    foreach (var item in listID)
                    {
                        System.IO.File.SetAttributes(Server.MapPath(item), System.IO.FileAttributes.Normal);
                        System.IO.File.Delete(Server.MapPath(item));
                    }

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
            return View("Edit", dbr.ProductAnnex.FindById(uid));
        }
        public ActionResult Add(string uid)
        {
            return View("Edit", new ProductAnnexRule.Entity() { ProductID = uid.AsInt() });
        }
        [HttpPost]
        public ActionResult Add(string uid, FormCollection collection)
        {
            var query = collection.ToDictionary();
            query.UpdateEnums(dbr.ProductAnnex._);

            JsonMsg jm = new JsonMsg();
            try
            {
                if (new ProductAnnexRule().Insert(query).Execute() == 1)
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
            return View("Edit", dbr.ProductAnnex.FindById(uid));
        }
        [HttpPost]
        public ActionResult Update(int uid, FormCollection collection)
        {
            var query = collection.ToDictionary();
            query.UpdateEnums(dbr.ProductAnnex._);

            JsonMsg jm = new JsonMsg();
            try
            {
                if (new ProductAnnexRule().Update(query).Execute() == 1)
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
