using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCon;
using MyOql;
using MyCmn;
using MyBiz.Sys;
using MyBiz;
using DbEnt;

namespace MyWeb.Areas.Admin.Controllers
{
    public class RoleController : MyMvcController
    {
        public ActionResult List()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Query(string uid, string Type, string UserID, FormCollection collection)
        {
            var query = collection["query"].FromJson<XmlDictionary<string, string>>();

            string name = query.GetOrDefault("Name");

            string sortName = collection["sortname"];
            string sortOrder = collection["sortorder"];

            WhereClip where = new WhereClip();



            //var eachIDs = new MyBigInt(dbr.Person.FindByUserID(UserID).Roles).ToPositions();
            //if (Type == "View" && UserID.HasValue())
            //{
            //    where &= dbr.Role.ID.In(eachIDs.ToArray());
            //}

            OrderByClip order = new OrderByClip(sortName);
            if (sortOrder == "desc") order.IsAsc = false;

            var d = dbr.Role
                .Select(o => o.GetColumns())
                ._(new ConstColumn(0).As("Sel"))
                .Skip((collection["page"].AsInt() - 1) * collection["rp"].AsInt())
                .Take(collection["rp"].AsInt())
                .Where(where)
                .OrderBy(order)
                .ToMyOqlSet()
 ;

            ////uid 是指用户ID
            //if (uid.HasValue())
            //{
            //    var usr = dbr.Person.FindByUserID(uid);
            //    if (usr == null) throw new GodError("用户 ID 不能为空.");

            //    var selColIndex = d.Columns.Length - 1;
            //    var idColIndex = d.Columns.IndexOf(o => ColumnClip.NameEquals(o, dbr.Role.Id));
            //    new MyBigInt(usr.GetRoles().Select(o=>o.Power)).ToPositions().All(o =>
            //        {
            //            d.Rows.First(r => r[idColIndex].AsInt() == o)[selColIndex] = 1;
            //            return true;
            //        });

            //}


            return d.LoadFlexiGrid();
        }

        public ActionResult Delete(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();
            string[] delIds = collection["query"].Split(',');
            try
            {
                var ret = dbr.Role.Delete(o => o.GetIdKey().In(delIds)).Execute();
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

        public ActionResult Detail(Int32 uid)
        {
            return View("Edit", dbr.Role.FindById(uid));
        }

        public ActionResult Add(string uid)
        {
            return View("Edit", new RoleRule.Entity());
        }

        [HttpPost]
        public ActionResult Add(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();

            var query = collection.ToDictionary();
            query["Power"] = new PowerJson(query.GetOrDefault("Power").AsString().FromJson<Dictionary<string, int[]>>(), null, null, null).ToString();
            query.UpdateEnums(dbr.Role._);

            try
            {
                if (dbr.Role.Insert(query).Execute() == 1)
                {

                    jm.data = query[dbr.Role.Id.Name];
                }
                else { jm.msg = "未新增记录"; }
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;
        }

        public ActionResult Update(Int32 uid)
        {
            return View("Edit", dbr.Role.FindById(uid));
        }

        [HttpPost]
        public ActionResult Update(Int32 uid, FormCollection collection)
        {
            var query = collection.ToDictionary();
            query.UpdateEnums(dbr.Role._);
            query.Remove("Power");

            JsonMsg jm = new JsonMsg();
            try
            {
                if (dbr.Role.Update(query).Execute() == 1)
                { }
                else { jm.msg = "未更新记录"; }
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;
        }

        [HttpPost]
        public ActionResult UpdatePower(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();


            var data = collection["power"].FromJson<Dictionary<string, int[]>>();
            PowerJson power = new PowerJson(data, null, null, null);

            dbr.Role.Update(o => o.Power == power.ToString()).Where(o => o.Id == uid).Execute();

            return jm;
        }
    }
}
