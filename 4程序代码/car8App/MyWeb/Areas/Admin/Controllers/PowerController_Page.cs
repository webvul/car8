using System;
using System.Collections.Generic;
using System.Linq;
using MyCon;
using System.Web.Mvc;
using MyOql;
using MyCmn;
using MyBiz.Sys;
using MyBiz.Admin;
using DbEnt;

namespace MyWeb.Areas.Admin.Controllers
{

    /// <summary>
    /// 在URL 传递时, 采用  Type=User&Value=SysAdmin 的方式 , 这样 Key 是固定的. 
    /// 参数:
    /// Type=User,Dept,Role
    /// Value=SysAdmin,Guid,Id
    /// Data=True    启用数据权限
    /// Mode=Add,Minus 在批量处理时, 指定Post上来的数据是 批量添加权限还是批量减少权限.
    /// </summary>
    public partial class PowerController : MyMvcController
    {
        public class QueryModel : QueryModelBase
        {
            public string Controller { get; set; }
            public string Action { get; set; }
            public string Button { get; set; }
        }

        [HttpPost]
        public ActionResult PagePowerQuery(string Type, string Value, QueryModel Query)
        {
            PowerOwnerEnum updateType = Type.ToEnum<PowerOwnerEnum>();
            string updateValue = Value;
            bool IsSingleValue = Value.HasValue() && !Value.Contains(",");
            string ErrMsg = string.Empty;


            var retVal = GetPagePowers(
                Query.skip,
                Query.take,
                Query.Controller , Query.Action , Query.Button  
                );



            if (IsSingleValue)
            {
                PowerJson power = PowerBiz.GetTypePower(updateType, updateValue, ref ErrMsg);

                if (ErrMsg.HasValue())
                {
                    throw new GodError(ErrMsg);
                }

                foreach (int act in power.Action.ToPositions())
                {
                    retVal.Rows.All(c =>
                    {
                        c.Rows.All(a =>
                        {
                            if (a.Id == "a" + act)
                            {
                                a.Cell["Sel"] = true.AsString();
                            }
                            return true;
                        });

                        return true;
                    });

                }

                foreach (int btn in power.Button.ToPositions())
                {
                    retVal.Rows.All(c =>
                    {
                        c.Rows.All(a =>
                        {
                            a.Rows.All(b =>
                            {
                                if (b.Id == "b" + btn)
                                {
                                    b.Cell["Sel"] = true.AsString();
                                }
                                return true;
                            });
                            return true;
                        });

                        return true;
                    });
                }
            }

            return retVal;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="ControllerName">查询条件</param>
        /// <param name="ActionName">查询条件</param>
        /// <param name="ButtonName">查询条件</param>
        /// <returns></returns>
        private static FlexiTreeJson GetPagePowers(int skip, int take, string ControllerName, string ActionName, string ButtonName)
        {
            var cons = dbr.PowerController
                .Select()
                .Where(o => o.Descr.IsNull("").Like("%" + ControllerName + "%"))
                .Skip(skip)
                .Take(take)
                .ToEntityList(o => o._);

            var acts = dbr.PowerAction
                .Select()
                .Where(o => o.Descr.IsNull("").Like("%" + ActionName + "%"))
                .ToEntityList(o => o._)
                .Where(o => o.Action.HasValue())
                .ToList();

            var btns = dbr.PowerButton
                .Select()
                .Where(o => o.Descr.IsNull("").Like("%" + ButtonName + "%"))
                .ToEntityList(o => o._)
                .Where(o => o.Button.HasValue())
                .ToList();

            var retVal = new FlexiTreeJson(dbr.PowerAction.GetName(),
                dbr.PowerController.Select(o => o.Count()).Where(o => o.Descr.IsNull("").Like("%" + ControllerName + "%")).ToScalar().AsInt()
                );

            foreach (var con in cons)
            {
                var row = retVal.AddNewRootRow();
                row.Id = "c" + con.Id.AsString();
                row.Cell["id"] = row.Id;
                row.Cell["Name"] = con.Area + "." + con.Controller;
                row.Cell["Descr"] = con.Area + "." + con.Descr.AsString(null) ?? con.Controller;
                row.Cell["Data"] = "~/" + con.Area + "/" + con.Controller + "/";
                row.Cell["Sel"] = false.ToString();
                row.Cell["Type"] = "模块";
                foreach (var act in acts.Where(o => o.ControllerID == con.Id))
                {
                    var sub1row = row.AddNewRow();
                    sub1row.Id = "a" + act.Id;
                    sub1row.Cell["id"] = sub1row.Id;
                    sub1row.Cell["Name"] = act.Action;
                    sub1row.Cell["Descr"] = act.Descr.AsString(null) ?? act.Action;
                    sub1row.Cell["Data"] = "~/" + con.Area + "/" + con.Controller + "/" + act.Action + ".aspx";
                    sub1row.Cell["Sel"] = false.ToString();
                    sub1row.Cell["Type"] = "页面";

                    //row.Rows.Add(sub1row);
                    foreach (var btn in btns.Where(o => o.ActionID == act.Id))
                    {
                        var sub2row = sub1row.AddNewRow();
                        sub2row.Id = "b" + btn.Id;
                        sub2row.Cell["id"] = sub2row.Id;
                        sub2row.Cell["Name"] = btn.Button;
                        sub2row.Cell["Descr"] = btn.Descr.AsString(null) ?? btn.Button;
                        sub2row.Cell["Data"] = btn.Button;
                        sub2row.Cell["Sel"] = false.ToString();
                        sub2row.Cell["Type"] = "按钮";
                        //sub1row.Rows.Add(sub2row);
                    }
                }
                retVal.Rows.Add(row);
            }
            return retVal;
        }

        [HttpPost]
        public ActionResult PagePowerSave(string Type, string Value, string Mode, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();

            PowerOwnerEnum updateType = Type.ToEnum<PowerOwnerEnum>();

            if (Value.HasValue() == false)
            {
                Value = string.Join(",", GetAllValue(updateType));
            }

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

                //权限字中不允许有Max. 待修改.
                if (power.Action.Max == false)
                {
                    power.Action |= MyBigInt.CreateByBitPositons(adds.Where(o => o.StartsWith("a")).Select(o => o.Substring(1).AsInt()));
                    power.Action = power.Action.Minus(MyBigInt.CreateByBitPositons(minus.Where(o => o.StartsWith("a")).Select(o => o.Substring(1).AsInt())));
                }

                if (power.Button.Max == false)
                {
                    power.Button |= MyBigInt.CreateByBitPositons(adds.Where(o => o.StartsWith("b")).Select(o => o.Substring(1).AsInt()));
                    power.Button = power.Button.Minus(MyBigInt.CreateByBitPositons(minus.Where(o => o.StartsWith("b")).Select(o => o.Substring(1).AsInt())));
                }

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

        private string[] GetAllValue(PowerOwnerEnum updateType)
        {
            switch (updateType)
            {
                case PowerOwnerEnum.User:
                    return dbr.Person.Select(o => o.UserID).ToEntityList("").ToArray();
                case PowerOwnerEnum.Dept:
                    return dbr.Dept.Select(o => o.Id).ToEntityList("").ToArray();
                //case PowerOwnerEnum.Role:
                //    return dbr.Role.Select(o => o.Id).ToEntityList("").ToArray();
                case PowerOwnerEnum.NotMine:
                    return dbr.Person.Select(o => o.UserID).ToEntityList("").ToArray();
                case PowerOwnerEnum.TStandardRole:
                    return dbr.TStandardRole.Select(o => o.StandardRoleId).ToEntityList("").ToArray();
                default:
                    break;
            }

            return new string[] { };
        }
    }
}