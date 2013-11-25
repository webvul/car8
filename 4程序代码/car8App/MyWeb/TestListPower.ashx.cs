using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyCmn;
using MyCon;
using MyOql;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Configuration;
using DbEnt;
using MyBiz;
using MyBiz.Sys;

namespace MyWeb
{
    /// <summary>
    /// TestPower 的摘要说明
    /// </summary>
    public class TestListPower : IHttpHandler, IReadOnlySessionState, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string area = context.Request.Form["area"];
            string con = context.Request.Form["controller"];
            string act = context.Request.Form["action"];
            string[] entitys = context.Request.Form["entity"].AsString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            var ps = Test(area, con, act, entitys, true);
            context.Response.ContentType = "application/json";
            context.Response.Write(ps.ToString());
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }



        private static PowerResult Test(string area, string con, string act, string[] entitys, bool TestCard)
        {
            PowerResult ps = new PowerResult();

            PowerJson pj = null;

            {
                if (MySession.IsSysAdmin())
                {
                    ps.msg = "SysAdmin";
                    return ps;
                }

                pj = MySession.GetMyPower();
                if (pj == null)
                {
                    ps.msg = MySession.GetLoginHtml();
                    return ps;
                }
            }

            if (pj == null)
            {
                ps.msg = "获取权限出错!";
                return ps;
            }

            if (dbr.View.VPowerAction.FindScalar(
                o => o.Count(),
                o => o.Area == area &
                o.Controller == con &
                o.Action == act).AsInt() > 0)
            {
                //先取出定义的所有按钮. 再从我的权限中取出我所拥有的权限, 相减 是我没有的权限. 返回之. 下同.udi

                var allButtons = dbr.PowerButton
                    .Select()
                    .Join(dbr.PowerAction, (a, b) => a.ActionID == b.Id)
                    .Join(dbr.PowerController, (a, b) => dbr.PowerAction.ControllerID == b.Id)
                    .Where(o => dbr.PowerController.Area == area & dbr.PowerController.Controller == con & dbr.PowerAction.Action == act)
                    .ToEntityList(o => o._)
                    .ToArray();

                var myButton = pj.Button.ToPositions().ToArray();

                ps.button = allButtons.Where(o => o.Id.IsIn(myButton) == false).Select(o => o.Button).ToArray();
            }

            //不启用 表列权限。 Udi
            //if (string.Equals(ConfigurationManager.AppSettings["MyDataPower"], "true", StringComparison.CurrentCultureIgnoreCase))
            //{

            //    if (dbr.PowerColumn
            //            .Select(o => o.Count())
            //            .Join(dbr.PowerTable, (a, b) => a.TableID == b.Id)
            //            .Where(o => dbr.PowerTable.Table.In(entitys) & o.Id.In(pj.Update.ToPositions().ToArray()))
            //            .ToScalar()
            //            .AsInt() > 0)
            //    {


            //        var allEditColumn = dbr.PowerColumn
            //            .Select()
            //            .Join(dbr.PowerTable, (a, b) => a.TableID == b.Id)
            //            .Where(o => dbr.PowerTable.Table.In(entitys))
            //            .ToEntityList(o => o._)
            //            .ToArray();

            //        var myEditColumn = pj.Update.ToPositions().ToArray();

            //        ps.edit = allEditColumn
            //            .Where(o => o.Id.IsIn(myEditColumn) == false)
            //            .Select(o => dbr.PowerTable.Table.Name + "." + o.Column)
            //            .ToArray();



            //        var allViewColumn = dbr.PowerColumn
            //            .Select()
            //            .Join(dbr.PowerTable, (a, b) => a.TableID == b.Id)
            //            .Where(o => dbr.PowerTable.Table.In(entitys))
            //            .ToEntityList(o => o._)
            //            .ToArray();

            //        var myViewColumn = pj.Read.ToPositions().ToArray();

            //        ps.view = allViewColumn
            //                .Where(o => o.Id.IsIn(myViewColumn) == false)
            //                .Select(o => dbr.PowerTable.Table.Name + "." + o.Column)
            //                .ToArray();
            //    }
            //}
            return ps;
        }


    }
}