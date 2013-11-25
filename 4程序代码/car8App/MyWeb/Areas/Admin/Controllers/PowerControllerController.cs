using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCon;
using MyCmn;
using DbEnt;
using MyOql;
using MyBiz.Admin;
using MyBiz.Sys;

namespace MyWeb.Areas.Admin.Controllers
{
    public class PowerControllerController : MyMvcController
    {
        [HttpPost]
        public ActionResult ImportOql(string uid)
        {
            JsonMsg jm = new JsonMsg();
            var ents = dbr.PowerController.SelectWhere(null).ToEntityList(o => o._);

            var type = typeof(MenuRule).Assembly.GetTypes();
            type.Where(o => o.Namespace == AutoGenController.EntityNameSpace && o.IsSubclassOf(typeof(RuleBase))).All(
                o =>
                {
                    var ent = o.Name.Remove(o.Name.Length - 4);
                    if (ents.Count(e => e.Controller == ent) == 0)
                        dbr.PowerController.Insert(c => c.Area == uid.ToEnum(AreaEnum.Admin) & c.Controller == ent).Execute();
                    return true;
                }
                );
            jm.msg = "OK";

            return jm;
        }
        public ActionResult List()
        {
            return View();
        }

        public ActionResult ListPower()
        {
            return View("List");
        }

        public ActionResult Query(string uid, PowerControllerBiz.QueryModel Query)
        {
            var retVal = PowerControllerBiz.Query(uid, Query).LoadFlexiGrid();
            UpdateSelectValue(uid, retVal);

            return retVal;
        }
        private void UpdateSelectValue(string uid, FlexiJson retVal)
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

                    if (power.Row != null && power.Row.View.ContainsKey(dbr.PowerController.GetDbName()))
                    {
                        var selIndex = retVal.PostColumns.IndexOf("Sel");
                        var menuPower = power.Row.View[dbr.PowerController.GetDbName()];
                        foreach (int act in menuPower.ToPositions())
                        {
                            retVal.Rows.ForEach(o =>
                                {
                                    if (o.Id.AsInt() == act)
                                    {
                                        o.Cell[selIndex] = Boolean.TrueString;
                                    }
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
                if (power.Row != null && power.Row.View.ContainsKey(dbr.PowerController.GetDbName()))
                {
                    menuPower = power.Row.View[dbr.PowerController.GetDbName()];
                }

                if (menuPower == null)
                {
                    menuPower = new MyBigInt();
                }

                if (menuPower.Max) return string.Empty;

                //权限字中不允许有Max. 待修改.

                menuPower |= MyBigInt.CreateByBitPositons(adds.Select(o => o.AsInt()));
                menuPower = menuPower.Minus(MyBigInt.CreateByBitPositons(minus.Select(o => o.AsInt())));


                power.Row.View[dbr.PowerController.GetDbName()] = menuPower;

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
                var ret = dbr.PowerController.Delete(o => o.GetIdKey().In(delIds)).Execute();
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
            return View("Edit", dbr.PowerController.FindById(uid));
        }
        public ActionResult Add(string uid)
        {
            return View("Edit", new PowerControllerRule.Entity());
        }
        [HttpPost]
        public ActionResult Add(string uid, FormCollection collection)
        {
            var query = collection.ToDictionary();

            query.UpdateEnums(dbr.PowerController._);

            JsonMsg jm = new JsonMsg();
            try
            {
                if (dbr.PowerController.Insert(query).Execute() == 1)
                { }
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
            return View("Edit", dbr.PowerController.FindById(uid));
        }
        [HttpPost]
        public ActionResult Update(Int32 uid, FormCollection collection)
        {
            var query = collection.ToDictionary();
            //.ToDictionary(o => o.Key, o => o.Value.GetHtmlInnerText());
            query.UpdateEnums(dbr.PowerController._);

            JsonMsg jm = new JsonMsg();
            try
            {
                if (dbr.PowerController.Update(query).Execute() == 1)
                { }
                else { jm.msg = "未更新记录"; }
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;
        }

    }
}
