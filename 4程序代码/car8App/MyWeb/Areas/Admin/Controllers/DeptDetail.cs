
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using MyCmn;
//using MyOql;
//using MyCon;
//using MyBiz;
//using DbEnt;

//namespace MyWeb.Areas.Admin.Controllers
//{
//    public class DeptDetailController : MyMvcController
//    {
//        public ActionResult List()
//        {
//            return View();
//        }
//        public ActionResult Query(string uid, FormCollection collection)
//        {
//            Dictionary<string, string> query = collection["query"].FromJson<Dictionary<string, string>>();

//            string key = query.GetOrDefault("Key");
//            string value = query.GetOrDefault("Value");

//            string sortName = collection["sortname"];
//            string sortOrder = collection["sortorder"];


//            //if (sortName == "Key") sortName = MyHelper.GetRes("ZhKey", "Key");
//            //if (sortName == "Value") sortName = MyHelper.GetRes("ZhValue", "Value");

//            WhereClip where = dbr.DeptDetail.DeptID == MySession.Get(MySessionKey.DeptID);

//            if (key.HasValue())
//            {
//                where &= dbr.DeptDetail.Key.Like("%" + key + "%");
//            }

//            if (value.HasValue())
//            {
//                where &= dbr.DeptDetail.Value.Like("%" + value + "%");
//            }

//            OrderByClip order = new OrderByClip(sortName);
//            if (sortOrder == "desc") order.IsAsc = false;

//            var d = dbr.DeptDetail.Select()
//                //.LeftJoin(dbo., dbo.DeptDetailRule. == dbo..)
//                .Skip((collection["page"].AsInt() - 1) * collection["rp"].AsInt())
//                .Take(collection["rp"].AsInt())
//                .Where(where)
//                .OrderBy(order)
//                .ToMyOqlSet()
//                .LoadFlexiGrid();
//            return d;
//        }

//        public ActionResult Delete(string uid, FormCollection collection)
//        {
//            JsonMsg jm = new JsonMsg();
//            string[] delIds = collection["query"].Split(',');
//            try
//            {
//                var ret = new DeptDetailRule().Delete(o => o.GetIdKey().In(delIds)).Execute();
//                if (ret == delIds.Count())
//                {

//                }
//                else { jm.msg = string.Format("删除了 {0}/{1} 条记录.", ret, delIds.Count()); }
//            }
//            catch (Exception ee)
//            {
//                jm.msg = ee.Message.GetSafeValue();
//            }

//            return jm;
//        }

//        public ActionResult Detail(int uid)
//        {
//            return View("Edit", dbr.DeptDetail.FindById(uid));
//        }
//        public ActionResult Add(string uid)
//        {
//            return View("Edit", new DeptDetailRule.Entity() { SortID = 100, GroupKey = ShopMenu.Home });
//        }
//        [HttpPost]
//        public ActionResult Add(string uid, FormCollection collection)
//        {
//            var query = collection.ToDictionary();
//            query[dbr.DeptDetail.DeptID.Name] = MySession.Get(MySessionKey.DeptID);

//            JsonMsg jm = new JsonMsg();
//            try
//            {
//                if (new DeptDetailRule().Insert(query).Execute() == 1)
//                { }
//                else { jm.msg = "未新增记录"; }
//            }
//            catch (Exception ee)
//            {
//                jm.msg = ee.Message.GetSafeValue();
//            }

//            return jm;
//        }
//        public ActionResult Update(int uid)
//        {
//            return View("Edit", dbr.DeptDetail.FindById(uid));
//        }
//        [HttpPost]
//        public ActionResult Update(int uid, FormCollection collection)
//        {
//            var query = collection.ToDictionary();
//            //.ToDictionary(o => o.Key, o => o.Value.GetHtmlInnerText());
//            query.UpdateEnums(dbr.DeptDetail._);

//            JsonMsg jm = new JsonMsg();
//            try
//            {
//                if (new DeptDetailRule().Update(query).Execute() == 1)
//                { }
//                else { jm.msg = "未更新记录"; }
//            }
//            catch (Exception ee)
//            {
//                jm.msg = ee.Message.GetSafeValue();
//            }

//            return jm;
//        }
//    }
//}
