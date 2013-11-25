
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using DbEnt;
using MyOql;
using MyCon;
using System.Text;
using MyWeb.Areas.Admin.Models;
using MyBiz.Admin;
using MyBiz;
using MyBiz.Sys;
using DbEnt;
namespace MyWeb.Areas.Admin.Controllers
{
    public class MenuController : MyMvcController
    {
        /// <summary>
        /// 根据父节点加载子节点
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult QueryMenu(string uid, FormCollection collection)
        {
            WhereClip where = new WhereClip();
            where &= (dbr.Menu.Wbs + ",").Like("%." + uid + ".%");
            where &= dbr.Menu.Status != IsAbleEnum.Disable;//开启
            OrderByClip order = new OrderByClip(dbr.Menu.Wbs.Name);
            order.IsAsc = true;
            OrderByClip orderSortId = new OrderByClip(dbr.Menu.SortID.Name);
            order.IsAsc = true;

            var d = dbr.Menu.Select()
             .Where(where)
             .OrderBy(order)
             .OrderBy(orderSortId)
             .OrderBy(o => o.Id.Asc)
             .SkipPower()
             .ToEntityList<MenuRule.Entity>();

            MenuNodeCollection menu = new MenuNodeCollection();

            menu.Bind(d, uid.AsInt());
            return Content(menu.ToString());
            //  return new ContentResult() { Content = content.ToString() };
        }
        public string MyRoleMenu()//根据权限获取显示的值        
        {//除系统管理模块外其他所有模块功能页面
            // State = YesNoEnum.Yes//开启状态
            //  , RootPath ="like %"+ myRole+"%"//
            return string.Empty;
        }
        ///// <summary>
        ///// 加载跟节点,0
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult Index()
        //{
        //    WhereClip where = new WhereClip();
        //    where &= dbr.Menu.PID == 0;//根节点
        //    where &= dbr.Menu.State == YesNoEnum.None;//开启

        //    OrderByClip order = new OrderByClip("RootPath");
        //    order.IsAsc = true;
        //    OrderByClip orderSortId = new OrderByClip("SortID");
        //    order.IsAsc = true;

        //    var d = dbr.Menu.Select()
        //     .Where(where)
        //     .OrderBy(order)
        //       .OrderBy(orderSortId)
        //     .ToEntityList<MenuRule.Entity>();

        //    return View(d);
        //}
        public ActionResult Detail(Int32 uid)
        {
            return View("Edit", dbr.Menu.SelectWhere(o => o.Id == uid).SkipPower().ToEntity(o => o._));
        }
        public ActionResult Add(string uid)
        {
            return View("Edit", new MenuRule.Entity() { Status = IsAbleEnum.Enable });
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(string uid, FormCollection collection)
        {
            var query = collection.ToDictionary();
            if (query[dbr.Menu.Pid.Name].AsInt() != 0)
            {
                query.Add(dbr.Menu.Wbs.Name, dbr.Menu
                    .SelectWhere(o => o.Id == query[dbr.Menu.Pid.Name].AsInt())
                    .SkipPower()
                    .ToEntity(o => o._)
                    .Wbs.AsString() + "," + query[dbr.Menu.Pid.Name]);

            }
            else
            {
                query.Add(dbr.Menu.Wbs.Name, "0");
            }
            query.Add("AddUser", MySession.Get(MySessionKey.UserID));//添加用户
            query.UpdateEnums(dbr.Menu._);


            JsonMsg jm = new JsonMsg();
            try
            {
                if (dbr.Menu.Insert(query).SkipPower().Execute() == 1)
                {
                    if (query.ContainsKey("SortID") && query["SortID"].AsInt() == 0)
                    {
                        dbr.Menu
                            .Update(o => o.SortID == query[dbr.Menu.Id.Name].AsInt(), o => o.Id == query[dbr.Menu.Id.Name].AsInt())
                            .SkipPower()
                            .Execute();
                    }
                }
                else
                {
                    jm.msg = "未新增记录";
                }
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;
        }
        public ActionResult Update(Int32 uid)
        {
            var ent = dbr.Menu.SelectWhere(o => o.Id == uid).SkipPower().ToEntity(o => o._);
            return View("Edit", ent);
        }
        [HttpPost]
        public ActionResult Update(Int32 uid, FormCollection collection)
        {
            var query = collection.ToDictionary();
            if (query[dbr.Menu.Pid.Name].AsInt() != 0)
            {
                query.Add(dbr.Menu.Wbs.Name, dbr.Menu
                    .SelectWhere(o => o.Id == query[dbr.Menu.Pid.Name].AsInt())
                    .SkipPower()
                    .ToEntity(o => o._).Wbs.AsString() + "," + query[dbr.Menu.Pid.Name]);

            }
            else
            {
                query.Add(dbr.Menu.Wbs.Name, "0");
            }
            query.Add("AddUser", MySession.Get(MySessionKey.UserID));//添加用户

            if (query.Count(o => o.Key == dbr.Menu.Status.Name) > 0)
            {
                query[dbr.Menu.Status.Name] = query[dbr.Menu.Status.Name].AsString().ToEnum<IsAbleEnum>();
            }

            JsonMsg jm = new JsonMsg();
            try
            {
                if (dbr.Menu.Update(query).SkipPower().Execute() == 0)
                {
                    jm.msg = "未更新记录";
                }
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;
        }
        public ActionResult Edit()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }

        public ActionResult ListPower()
        {
            return View("List");
        }

        public class QueryModel : QueryModelBase
        {

        }

        public ActionResult Query(string uid, QueryModel Query)
        {
            OrderByClip orderRootPath = new OrderByClip("RootPath");
            orderRootPath.IsAsc = true;
            OrderByClip orderSortId = new OrderByClip("SortID");
            orderRootPath.IsAsc = true;

            WhereClip where = new WhereClip();

            //如果是引用。
            if (uid == "ListPower")
            {
                where &= dbr.Menu.Status == IsAbleEnum.Enable;
            }


            //根据根节点分页
            using (var scope = new MyOqlConfigScope(MySession.IsSysAdmin() ? ReConfigEnum.SkipPower : (ReConfigEnum)0))
            {
                var count = dbr.Menu.Select(o => o.Count()).Where(o => where & dbr.Menu.Pid == 0).ToScalar().AsInt();
                var Ids = dbr.Menu
                    .Select(o => o.Id)
                    .Skip(Query.skip)
                    .Take(Query.take)
                    .Where(o => where & o.Pid == 0)
                    .OrderBy(Query.sort)
                    .SmartPager(count)
                    .ToEntityList(0);



                var allData = dbr.Menu.Select().SkipPower().Where(o => where).ToDictList();

                //如果禁用了中间某一个,要检测每一个的父级是否存在。
                for (int i = 0; i < allData.Count; i++)
                {
                    var item = allData[i];
                    var sects = item["Wbs"].AsString().Split(",.".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Minus(new string[] { "0" }).ToArray();
                    if (allData.Count(o => o["Id"].AsString().IsIn(sects))
                        != sects.Length)
                    {
                        allData.RemoveAt(i);
                        i--;
                    }
                }

                var d = allData.Where(o =>
                    {
                        if (o[dbr.Menu.Wbs.Name].AsString().Split(',').Select(id => id.AsInt()).Intersect(Ids).Count() > 0 ||
                            Ids.Contains(o[dbr.Menu.Id.Name].AsInt()))
                        {
                            return true;
                        }
                        else return false;
                    })
                    ;

                //foreach (var item in d)
                //{
                //    if (ValueProc.As<IsAbleEnum>(item[dbr.Menu.Status.Name]) == IsAbleEnum.Disable)
                //    {
                //        item[dbr.Menu.Status.Name] = "禁用";
                //    }
                //    else
                //    {
                //        item[dbr.Menu.Status.Name] = "启用";
                //    }
                //}



                var retVal = dbr.Menu.LoadFlexiTreeGrid(d.Select(o => o.ToDictionary(k => k.Key, v => v.Value.AsString()).ToXmlDictionary()).ToList(), count, dbr.Menu.Id.Name, dbr.Menu.Pid.Name);
                RenderJsVar("VerId", "1");
                UpdateSelectValue(uid, retVal);

                return retVal;
            }
        }

        private void UpdateSelectValue(string uid, FlexiTreeJson retVal)
        {
            if (uid == "ListPower")
            {
                var Type = Request.QueryString["Type"];
                var Value = Request.QueryString["Value"];

                if (Type.HasValue() == false) return;

                PowerOwnerEnum updateType = Type.ToEnum<PowerOwnerEnum>();
                string updateValue = Value;
                bool IsSingleValue = Value.HasValue() && !Value.Contains(",");
                string ErrMsg = string.Empty;


                if (IsSingleValue)
                {
                    PowerJson power = PowerBiz.GetTypePower(updateType, updateValue, ref ErrMsg);

                    if (ErrMsg.HasValue())
                    {
                        throw new GodError(ErrMsg);
                    }

                    if (power.Row != null && power.Row.View.ContainsKey(dbr.Menu.GetDbName()))
                    {
                        var menuPower = power.Row.View[dbr.Menu.GetDbName()];
                        foreach (int act in menuPower.ToPositions())
                        {
                            new Recursion<FlexiTreeJson.FlexiTreeRowData>().Execute(retVal.Rows,
                                o => o.Rows, o =>
                                    {
                                        if (o.Id.AsInt() == act)
                                        {
                                            o.Cell["Sel"] = true.AsString();
                                        }
                                        return RecursionReturnEnum.Go;
                                    });

                        }
                    }
                }
            }
        }

        [HttpPost]
        public ActionResult PowerSave(string Type, string Value, string Mode, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();

            PowerOwnerEnum updateType = Type.ToEnum<PowerOwnerEnum>();
            string updateValue = Value;
            bool IsSingleValue = !Value.Contains(",");
            string ErrMsg = string.Empty;

            var query = collection.ToDictionary();
            var adds = query.GetOrDefault("Adds").AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var minus = query.GetOrDefault("Minus").AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //var userID = Request["Value"];


            if (ErrMsg.HasValue())
            {
                jm.msg = ErrMsg;
                return jm;
            }


            Func<string, string> SaveOneValuePower = val =>
            {
                PowerJson power = PowerBiz.GetTypePower(updateType, val, ref ErrMsg);
                if (ErrMsg.HasValue()) return ErrMsg;

                MyBigInt menuPower = null;
                if (power.Row != null && power.Row.View.ContainsKey(dbr.Menu.GetDbName()))
                {
                    menuPower = power.Row.View[dbr.Menu.GetDbName()];
                }

                if (menuPower == null)
                {
                    menuPower = new MyBigInt();
                }

                if (menuPower.Max) return string.Empty;


                //权限字中不允许有Max. 待修改.
                if (menuPower.Max == false)
                {
                    menuPower |= MyBigInt.CreateByBitPositons(adds.Select(o => o.AsInt()));
                    menuPower = menuPower.Minus(MyBigInt.CreateByBitPositons(minus.Select(o => o.AsInt())));
                }

                power.Row.View[dbr.Menu.GetDbName()] = menuPower;

                return PowerBiz.SaveTypePower(updateType, val, power);
            };
            if (IsSingleValue)
            {
                jm.msg = SaveOneValuePower(updateValue);
            }
            else if (Mode == "Add")
            {
                updateValue.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).All(o =>
                {
                    jm.msg = SaveOneValuePower(o);
                    if (jm.msg.HasValue())
                    {
                        jm.msg = "批量更新时遇到了一个错误而中止： " + jm.msg;
                        return false;
                    }
                    return true;
                });
            }
            else if (Mode == "Minus")
            {
                minus = adds;
                adds = new string[0];

                updateValue.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).All(o =>
                {
                    jm.msg = SaveOneValuePower(o);
                    if (jm.msg.HasValue())
                    {
                        jm.msg = "批量更新时遇到了一个错误而中止： " + jm.msg;
                        return false;
                    }
                    return true;
                });
            }

            return jm;
        }


        public ActionResult Delete(string uid, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();
            string[] delIds = collection["query"].Split(',');
            try
            {

                var ret = dbr.Menu.Delete(o => o.GetIdKey().In(delIds)).SkipPower().Execute();
                if (ret != delIds.Count())
                {
                    jm.msg = string.Format("删除了 {0}/{1} 条记录.", ret, delIds.Count());
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
