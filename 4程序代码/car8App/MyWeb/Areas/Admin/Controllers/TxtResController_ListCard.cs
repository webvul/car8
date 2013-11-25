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
using MyBiz.Admin;
using DbEnt;


namespace MyWeb.Areas.Admin.Controllers
{
    public partial class TxtResController : MyMvcController
    {

        public ActionResult List()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Query(string uid, TxtResBiz.QueryModel Query)
        {
            return TxtResBiz.Query(Query).LoadFlexiGrid();
        }

        [HttpPost]
        public ActionResult Delete(string uid, FormCollection collection)
        {
            string[] delIds = collection["query"].Split(',');
            return TxtResBiz.Delete(delIds);
        }

        public ActionResult Detail(Int32 uid)
        {
            return View("Card", dbr.View.VTxtRes.FindFirst(o => o.ResID == uid, o => o._));
        }

        public ActionResult AddView()
        {
            return View("Card", new VTxtResRule.Entity());
        }

        /// <summary>
        /// 根据 ResID 添加. 
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult Add(string uid)
        {
            //uid 表示 ResID .
            var model = dbr.View.VTxtRes.FindFirst(o => o.ResID == uid, o => o._);
            return View("Card", model);
        }

        [HttpPost]
        public ActionResult AddView(VTxtResRule.Entity Entity)
        {//没有消息,就是好消息.
            JsonMsg jm = new JsonMsg();

            if (MySession.Get(MySessionKey.UserID).HasValue() == false)
            {
                jm.msg = "会话超时,请登录!";
                return jm;
            }


            try
            {
                var insertResult = TxtResBiz.Add(Entity);
                if (insertResult == 0)
                {
                    jm.msg = "未更新记录";
                }
                else
                {
                    TxtResBiz.ClearCache(MyHelper.Lang);
                }
            }
            catch (Exception e)
            {
                jm.msg = e.Message.GetSafeValue();
            }
            return jm;
        }


        [HttpPost]
        public ActionResult Add(string uid, VTxtResRule.Entity Entity)
        {
            JsonMsg jm = new JsonMsg();

            if (MySession.Get(MySessionKey.UserID).HasValue() == false)
            {
                jm.msg = "会话超时,请登录!";
                return jm;
            }


            try
            {
                var insertResult = dbr.ResValue.Insert(Entity).Execute();
                if (insertResult == 0)
                {
                    jm.msg = "未更新记录";
                }
                else
                {
                    TxtResBiz.ClearCache(MyHelper.Lang);
                }
            }
            catch (Exception e)
            {
                jm.msg = e.Message.GetSafeValue();
            }
            return jm;
        }

        /// <summary>
        /// 这里的ID是语言值ID
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult Update(Int32 uid)
        {
            //uid 表示 ResID .
            var ent = dbr.View.VTxtRes.FindFirst(o => o.ResID == uid, o => o._);
            return View("Card", ent);
        }

        [HttpPost]
        public ActionResult Update(Int32 uid, VTxtResRule.Entity Entity)
        {
            return TxtResBiz.Update(Entity);
        }
    }
}
