
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
    public class ContactMsgController : MyMvcController
    {

        public ActionResult List()
        {
            return View();
        }

        public class QueryModel : QueryModelBase
        {

        }


        public ActionResult Query(string uid,QueryModel Query)
        {
            ContactMsgRule _ContactMsgRule = new ContactMsgRule();
            WhereClip where = new WhereClip();

             var d = _ContactMsgRule.Select()
                //.LeftJoin(dbo., dbo.ContactMsgRule. == dbo..)
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
                var ret = new ContactMsgRule().Delete(o => o.GetIdKey().In(delIds)).Execute();
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
            return View("Edit", dbr.ContactMsg.FindById(uid));
        }
        public ActionResult Add(string uid)
        {
            return View("Edit", new ContactMsgRule.Entity() { AddTime = DateTime.Now });
        }
        [HttpPost]
        public ActionResult Add(string uid,FormCollection collection)
        {
            var query = collection.ToDictionary();//["query"].FromJson<Dictionary<string, string>>();
            query.UpdateEnums(dbr.ContactMsg._);

            JsonMsg jm = new JsonMsg();
            try
            {
                if ( new ContactMsgRule().Insert( query ).Execute() == 1 )
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
            return View("Edit",dbr.ContactMsg.FindById(uid));
        }
        [HttpPost]
        public ActionResult Update(int uid, FormCollection collection)
        {
            var query = collection.ToDictionary();//["query"].FromJson<Dictionary<string, string>>();
            query.UpdateEnums(dbr.ContactMsg._);

            JsonMsg jm = new JsonMsg();
            try
            {
                if ( new ContactMsgRule().Update( query ).Execute() == 1 )
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
