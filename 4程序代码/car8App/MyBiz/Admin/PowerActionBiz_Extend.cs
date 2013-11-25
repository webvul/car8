using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using MyOql;
using MyBiz;

using DbEnt;


namespace MyBiz.Admin
{
    public partial class PowerActionBiz
    {
        public class QueryModel : QueryModelBase
        {
            public string Area { get; set; }
            public string Controller { get; set; }
            public string ControllerDescr { get; set; }
            public string Action { get; set; }
            public string ActionDescr { get; set; }
        }

        //根据需要修改参数
        public static MyOqlSet Query(QueryModel Query)
        {
            WhereClip where = new WhereClip();

            Query.Area.HasValue(o => where &= dbr.PowerController.Area.Like("%" + o + "%"));
            Query.Controller.HasValue(o => where &= dbr.PowerController.Controller.Like("%" + o + "%"));
            Query.ControllerDescr.HasValue(o => where &= dbr.PowerController.Descr.Like("%" + o + "%"));
            Query.Action.HasValue(o => where &= dbr.PowerAction.Action.Like("%" + o + "%"));
            Query.ActionDescr.HasValue(o => where &= dbr.PowerAction.Descr.Like("%" + o + "%"));

            if (Query.sort.HasData() == false)
            {
                Query.sort = dbr.PowerController.Area.Asc;
                Query.sort &= dbr.PowerController.Controller.Asc;
                Query.sort &= dbr.PowerController.Descr.AsFullName().Asc;
                Query.sort &= dbr.PowerAction.Action.Asc;
            }
            return dbr.PowerAction.Select()
                .Join(dbr.PowerController, (a, b) => a.ControllerID == b.Id, o => new ColumnClip[] { o.Id.AsFullName(), o.Area.AsFullName(), o.Controller.AsFullName(), o.Descr.AsFullName() })
                .Skip(Query.skip)
                .Take(Query.take)
                .Where(where)
                .OrderBy(Query.sort)
                .ToMyOqlSet(Query.option);
        }

        public static JsonMsg Update(PowerActionRule.Entity Model)
        {
            //没有消息,就是好消息.
            JsonMsg jm = new JsonMsg();

            if (MySession.Get(MySessionKey.UserID).HasValue() == false)
            {
                jm.msg = "会话超时,请登录!";
                return jm;
            }


            try
            {
                //验证是否包含唯一约束列 或 主键 ； 是否包含其它必须参数.      如果是联合主键,判断方式会稍有不同,请自行修改.
                if (Model.Id == 0)
                {
                    jm.msg = "更新对象,必须拥有唯一键,或主键!";
                    return jm;

                }

                //验证要更新的参数是否合法


                var updateResult = dbr.PowerAction.Update(Model).Execute();
                if (updateResult == 0)
                {
                    jm.msg = "未更新记录";
                }
                else if (updateResult > 1)
                {
                    jm.msg = string.Format(@"更新了 {0} 条记录!", updateResult);
                }
            }
            catch (Exception e)
            {
                e.ToLog();
                jm.msg = "修改失败,已记录到日志,详情请联系系统管理员.";
            }
            return jm;
        }

        public static JsonMsg Add(PowerActionRule.Entity Model)
        {
            //没有消息,就是好消息.
            JsonMsg jm = new JsonMsg();

            if (MySession.Get(MySessionKey.UserID).HasValue() == false)
            {
                jm.msg = "会话超时,请登录!";
                return jm;
            }


            try
            {



                //验证要更新的参数是否合法


                var insertResult = dbr.PowerAction.Insert(Model).Execute();
                if (insertResult == 0)
                {
                    jm.msg = "未更新记录";
                }
            }
            catch (Exception e)
            {
                jm.msg = e.Message.GetSafeValue();
            }
            return jm;
        }

        //根据需要修改参数
        public static JsonMsg Delete(string[] IdArray)
        {
            //没有消息,就是好消息.
            JsonMsg jm = new JsonMsg();

            if (MySession.Get(MySessionKey.UserID).HasValue() == false)
            {
                jm.msg = "会话超时,请登录!";
                return jm;
            }

            //判断要删除的Id 是否合法.
            if (IdArray.Length == 0)
            {
                jm.msg = "没有要删除的对象!";
                return jm;
            }
            var IdInDb = dbr.PowerAction
                    .Select(o => o.GetIdKey())
                    .Where(o => o.GetIdKey().In(IdArray))
                    .ToEntityList(string.Empty);

            var dbLoseId = IdArray.Minus(IdInDb);
            if (dbLoseId.Count() > 0)
            {
                jm.msg = string.Format("要删除的数据Id ({0}) 在数据库中不存在, 可能数据已过时,请刷新后再试.",
                        string.Join(",", dbLoseId.ToArray())
                        );
                return jm;
            }

            //判断是否有权限删除


            try
            {
                var delResult = dbr.PowerAction.Delete(o => o.GetIdKey().In(IdArray)).Execute();
                if (delResult != IdArray.Length)
                {
                    jm.msg = string.Format("删除了 {0}/{1} 条记录.", delResult, IdArray.Length);
                }
            }
            catch (Exception ee)
            {
                ee.ToLog();
                jm.msg = "删除失败,已记录到日志,详情请联系系统管理员.";
            }

            return jm;
        }



    }
}
