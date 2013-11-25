
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using MyCmn;
//using MyOql;
//using MyCon;
//using DbEnt;

//namespace MyWeb.Areas.Admin.Controllers
//{
//    public class PersonAnnexController : MyMvcController
//    {

//        public ActionResult List()
//        {
//            return View();
//        }
//        public ActionResult Query(string uid, FormCollection collection)
//        {
//            Dictionary<string, string> query = collection["query"].FromJson<Dictionary<string, string>>();

//            string key = query.GetOrDefault(dbr.PersonAnnex.Key.Name);

//            string sortName = collection["sortname"];
//            string sortOrder = collection["sortorder"];

//            WhereClip where = new WhereClip();

//            if (key.HasValue())
//            {
//                where &= dbr.PersonAnnex.Key.Like("%" + key + "%");
//            }

 
//            OrderByClip order = new OrderByClip(sortName);
//            if (sortOrder == "desc") order.IsAsc = false;

//            var d = dbr.PersonAnnex.Select()
//                //.LeftJoin(dbo., dbo.PersonAnnexRule. == dbo..)
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
//                var ret = new PersonAnnexRule().Delete(o => o.GetIdKey().In(delIds)).Execute();
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
//            return View("Edit", dbr.PersonAnnex.FindById(uid));
//        }
//        public ActionResult Add(string uid)
//        {
//            return View("Edit", new PersonAnnexRule.Entity());
//        }
//        [HttpPost]
//        public ActionResult Add(string uid, FormCollection collection)
//        {
//            var query = collection.ToDictionary();
//            query.UpdateEnums(dbr.PersonAnnex._);

//            JsonMsg jm = new JsonMsg();
//            try
//            {
//                if (new PersonAnnexRule().Insert(query).Execute() == 1)
//                {  }
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
//            return View("Edit", dbr.PersonAnnex.FindById(uid));
//        }
//        [HttpPost]
//        public ActionResult Update(int uid, FormCollection collection)
//        {
//            var query = collection.ToDictionary();
//            query.UpdateEnums(dbr.PersonAnnex._);

//            JsonMsg jm = new JsonMsg();
//            try
//            {
//                if (new PersonAnnexRule().Update(query).Execute() == 1)
//                {  }
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
