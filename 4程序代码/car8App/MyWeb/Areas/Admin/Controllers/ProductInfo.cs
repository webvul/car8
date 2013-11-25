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
    public class ProductInfoController : MyMvcController
    {
        public ActionResult GetProductTypes()
        {
            var data = dbr.ProductType
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

        [HttpPost]
        public ActionResult HotDelete(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();
            string[] delIds = collection["query"].Split(',');
            try
            {
                var ret = dbr.EnterpriseShowCase.Delete(o => o.ProductID.In(delIds)).Execute();
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

            WhereClip where = dbr.EnterpriseShowCase.DeptID == MySession.Get(MySessionKey.DeptID);

            //var count = dbr.EnterpriseShowCase.FindScalar(o => o.Count(), o => { return where; }).GetInt();

            OrderByClip order = new OrderByClip(sortName);
            if (sortOrder == "desc") order.IsAsc = false;

            var d = dbr.EnterpriseShowCase.Select(o=>o.ProductID)
                .Join(dbr.ProductInfo, (a, b) => a.ProductID == b.Id, o => new ColumnClip[] { 
                    o.Id,
                    o.Name ,
                    o.Descr,
                    o.SortID
                })
                .LeftJoin(dbr.ProductType, (a, b) => dbr.ProductInfo.ProductTypeID == b.Id, o => new ColumnClip[]{
                    o.Name.As("ProductType")
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
            var ProductIds = collection["ProductIds"].Split(',');


            var insertIds = dbr.EnterpriseShowCase.Select(o => o.ProductID).Where(o => o.ProductID.In(ProductIds)).ToEntityList("");

            ProductIds = ProductIds.Where(o => insertIds.Contains(o) == false).ToArray();
            try
            {
                foreach (var item in ProductIds)
                {
                    dbr.EnterpriseShowCase.Insert(o => o.ProductID == item & o.DeptID == MySession.Get(MySessionKey.DeptID)).Execute();
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

        public ActionResult List()
        {
            return View();
        }
        public ActionResult Query(string uid, ProductionBiz.ProductInfoQueryModel Query)
        {
            WhereClip where = new WhereClip();
            var ProductTypeIDs = dbr.ProductType.Select(o => o.Id).Where(o => o.DeptID == MySessionKey.DeptID.Get() & o.UserID == MySessionKey.UserID.Get()).ToEntityList(0).ToArray();

            where &= dbr.ProductInfo.ProductTypeID.In(ProductTypeIDs);

            Query.Name.HasValue(o => where &= dbr.ProductInfo.Name.Like(string.Format("%{0}%", o)));
            Query.ProductTypeID.HasValue(o => where &= dbr.ProductInfo.ProductTypeID.In(o));

            return dbr.ProductInfo.Select(o => new ColumnClip[] { o.Id, o.Name, o.Descr, o.UpdateTime, o.SortID, ("t" + o.ProductTypeID).As("Type") })
                  .LeftJoin(dbr.ProductType, (a, b) => a.ProductTypeID == b.Id, o => new ColumnClip[] { o.Name.As(o.GetName() + ".Name") })
                  .LeftJoin(dbr.ProductClicks, (a, b) => a.Id == b.ProductID, o => o.Clicks.Count())
                  .Where(where)
                  .Skip(Query.skip)
                  .Take(Query.take)
                  .AutoGroup()
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
                new ProductDetailRule().Delete(o => o.ProductID.In(delIds)).Execute();
                var ret = new ProductInfoRule().Delete(o => o.GetIdKey().In(delIds)).Execute();
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
            var productId = uid.AsInt();
            RenderProductAnnexVar(productId);

            return View("Edit", dbr.ProductInfo.FindById(productId));
        }

        private void RenderProductAnnexVar(int productId)
        {
            var list = new List<string>();
            var annexInfos = dbr.Annex.SelectWhere(o => o.Id.In(dbr.ProductAnnex.Select(s => s.AnnexID).Where(s => s.ProductID == productId))).ToEntityList(o => o._);
            if (annexInfos != null)
            {
                foreach (var annex in annexInfos)
                {
                    list.Add(string.Format("{{ success :true, id: '{0}',url:'{1}',name:'{2}'}}", annex.Id, annex.GetUrlFull(), annex.Name));
                }
            }

            RenderJsVar("annex", new VarJsResult("[" + list.Join(",") + "]") { Type = JsType.Script });
        }
        public ActionResult Add(string uid)
        {
            return View("Edit", new ProductInfoRule.Entity() { SortID = 100 });
        }
        [HttpPost]
        public ActionResult Add(string uid, FormCollection collection)
        {
            var query = collection.ToDictionary();
            query.UpdateEnums(dbr.ProductInfo._);

            JsonMsg jm = new JsonMsg();
            try
            {
                if (query.ContainsKey("Logo") && query["Logo"].AsInt() == 0)
                {
                    query.Remove("Logo");
                }

                if (new ProductInfoRule().Insert(query).Execute() == 1)
                {
                    dbr.ProductInfo.Update(o => o.SortID == o.Id * 10).Where(o => o.Id == query[dbr.ProductInfo.Id.Name]).Execute();

                    jm.data = query[new ProductInfoRule().Id.Name];

                    //找出差异
                    if (query.ContainsKey("Images"))
                    {
                        var annexIds = query["Images"].AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(o => o.AsInt());

                        foreach (var item in annexIds)
                        {
                            dbr.ProductAnnex.Insert(o => o.AnnexID == item & o.ProductID == jm.data).Execute();
                        }
                    }

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
            RenderProductAnnexVar(uid);
            return View("Edit", dbr.ProductInfo.FindById(uid));
        }
        [HttpPost]
        public ActionResult Update(string uid, DbEnt.ProductInfoRule.Entity Entity)
        {


            JsonMsg jm = new JsonMsg();
            try
            {
                GodError.Check(Entity.Id.HasValue() == false, () => "找不到 产品实体 的主键信息");

                Entity.UpdateTime = DateTime.Now;
    

                if (Entity.Logo.HasValue())
                    dbr.ProductInfo.Update(Entity).Execute();
                else
                    dbr.ProductInfo.Update(o => o.ProductTypeID == Entity.ProductTypeID & o.Name == Entity.Name & o.Descr == Entity.Descr & o.UpdateTime == Entity.UpdateTime & o.Logo == null, o => o.Id == Entity.Id).Execute();

                //找出差异
                if (uid.HasValue())
                {
                    var annexIds = uid.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(o => o.AsInt());
                    var dbIds = dbr.ProductAnnex.Select(o => o.AnnexID).Where(o => o.ProductID == Entity.Id).ToEntityList(0);
                    var push = new DbPushModel<int>(dbIds, annexIds);


                    dbr.ProductAnnex.Delete(o => o.AnnexID.In(push.ToDelete.ToArray())).Execute();

                    foreach (var item in push.ToInsert)
                    {
                        dbr.ProductAnnex.Insert(o => o.AnnexID == item & o.ProductID == Entity.Id).Execute();
                    }

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
