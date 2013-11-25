using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyCon;
using DbEnt;
using MyOql;
using System.Web.Mvc;
using MyCmn;
using MyBiz.Admin;
namespace MyWeb.Areas.Admin.Controllers
{
    public class LogController : MyMvcController
    {
        #region 私有方法
        /// <summary>
        /// 获取日志类型
        /// </summary>
        /// <param name="typeName">日志类型名称</param>
        /// <returns></returns>
        /// <remarks>创建人员(日期):★ben★(101014 15:45)</remarks>
        public InfoEnum GetLogType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return 0;

            //"默认","消息","错误","系统","用户"

            else if (typeName.Equals("消息"))
                return InfoEnum.Info;
            else if (typeName.Equals("错误"))
                return InfoEnum.Error;
            else if (typeName.Equals("系统"))
                return InfoEnum.System;
            else if (typeName.Equals("用户"))
                return InfoEnum.User;
            else
                return 0;
        }

        #endregion
        public ActionResult List()
        {
            return View();
        }

        public class QueryModel : QueryModelBase
        {
            public string UserName { get; set; }
            public string BeginTime { get; set; }
            public string EndTime { get; set; }
            public string Type { get; set; }
        }

        public ActionResult Query(string uid, QueryModel Query)
        { 
            int totalCount = 0;

            LogClass lc = new LogClass();
            FlexiJson fj = dbr.Log.LoadFlexiGrid(
                lc.GetLogs(
                    Query.UserName,Query. BeginTime, Query. EndTime, GetLogType(Query. Type), Query.sort , Query.skip, Query.take, out totalCount), totalCount);

            fj.Totle = totalCount;
            fj.CurrentPage = Query.pageIndex;
            fj.Entity = lc.GetTableName();
            return fj;
        }
        //页面层不访问数据库，可是又不让写getbyid方法，真郁闷!
        //public ActionResult Detail(Int32 uid)
        //{
        //    //LogClass lc = new LogClass();
        //    return View("Edit", logs);
        //}
        public ActionResult Add(string uid)
        {
            return View("Edit", new LogRule.Entity());
        }


        public ActionResult Detail(string uid)
        {
            return View("Edit", dbr.Log.FindById(uid.AsInt()));
        }

        [HttpPost]
        public ActionResult Delete(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();
            string[] delIds = collection["query"].Split(',');
            try
            {

                var ret = dbr.Log.Delete(o => o.GetIdKey().In(delIds)).SkipLog().Execute();

            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;
        }
    }
}
