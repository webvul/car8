
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
    public class EnterpriseShowCaseController : MyMvcController
    {

        public ActionResult List()
        {
            return View();
        }

        public class QueryModel : QueryModelBase
        {
            public string Name { get; set; }
        }

        public ActionResult Query(string uid,QueryModel Query)
        {
            EnterpriseShowCaseRule _EnterpriseShowCaseRule = new EnterpriseShowCaseRule();
            WhereClip where = new WhereClip();

             
            var d = _EnterpriseShowCaseRule.Select()
                //.LeftJoin(dbo., dbo.EnterpriseShowCaseRule. == dbo..)
                .Skip(Query.skip)
                .Take(Query.take)
                .Where(where)
                .OrderBy(Query.sort)
                .ToMyOqlSet()
                .LoadFlexiGrid();
            return d;
        }
     
        public ActionResult Delete(string uid,FormCollection collection)
        {
            JsonMsg jm = new JsonMsg() ;
            string[] delIds = collection["query"].Split(',');
            try
            {
                var ret = new EnterpriseShowCaseRule().Delete(o => o.GetIdKey().In(delIds)).Execute();
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
            return View("Edit", dbr.EnterpriseShowCase.FindByProductID(uid));
        }
        public ActionResult Add(string uid)
        {
            return View("Edit", new EnterpriseShowCaseRule.Entity() { SortID = 100 });
        }
        [HttpPost]
        public ActionResult Add(string uid,FormCollection collection)
        {
            var query = collection.ToDictionary();
            query.UpdateEnums(dbr.EnterpriseShowCase._);

            JsonMsg jm = new JsonMsg();
            try
            {
                if ( new EnterpriseShowCaseRule().Insert( query ).Execute() == 1 )
                {   }
                else {jm.msg = "未新增记录";}
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;
        }
        public ActionResult Update(int uid)
        {
            return View("Edit",dbr.EnterpriseShowCase.FindByProductID(uid));
        }
        [HttpPost]
        public ActionResult Update(int uid, FormCollection collection)
        {
            var query = collection.ToDictionary();
            query.UpdateEnums(dbr.EnterpriseShowCase._);

            JsonMsg jm = new JsonMsg();
            try
            {
                if ( new EnterpriseShowCaseRule().Update( query ).Execute() == 1 )
                {   }
                else {jm.msg = "未更新记录";}
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;
        }
}}
