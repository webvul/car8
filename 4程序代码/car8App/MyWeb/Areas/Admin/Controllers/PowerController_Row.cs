using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyCon;
using System.Web.Mvc;
using DbEnt;
using MyOql;
using MyCmn;
using MyWeb.Areas.Admin.Models;
using MyBiz.Sys;
using MyBiz;
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
        [HttpPost]
        public ActionResult RowPowerQuery(string Type, string Value, FormCollection collection)
        {
            List<Dictionary<string, string>> dict = new List<Dictionary<string, string>>();

            foreach (MyOqlConfigSect.EntityCollection.GroupCollection group in dbo.MyOqlSect.Entitys)
            {
                foreach (MyOqlConfigSect.EntityCollection.GroupCollection.EntityElement ent in group)
                {
                    var entPower = ent.UsePower.ToEnum<MyOqlActionEnum>();// MyOql.MyOqlSectHelper.GetUsePower(ent.Name);
                    if (entPower.Contains(MyOqlActionEnum.Row))
                    {
                        dict.Add(new Dictionary<string, string>());
                        dict.Last()["Name"] = ent.Name;
                        dict.Last()["TableName"] = ent.Name;
                    }
                }
            }


            //var enm = MyOql.MyOqlSectHelper.MyOqlSet.Entitys.GetEnumerator();
            //while (enm.MoveNext())
            //{
            //    var ent = (enm.Current as MyOqlSect.EntityCollection.GroupCollection.EntityElement);
            //    var entPower = ent.UsePower.ToEnum<MyOqlActionEnum>();// MyOql.MyOqlSectHelper.GetUsePower(ent.Name);
            //    if (entPower.Contains(MyOqlActionEnum.Row))
            //    {
            //        dict.Add(new Dictionary<string, string>());
            //        dict.Last()["Name"] = ent.Name;
            //        dict.Last()["TableName"] = ent.Name;
            //    }
            //}

            PowerOwnerEnum updateType = Type.ToEnum<PowerOwnerEnum>();// Request.QueryString["Type"].ToEnum(PowerOwnerEnum.None);
            string updateValue = Value;// Request.QueryString["Value"];
            bool IsSingleValue = Value.HasValue() && !Value.Contains(",");
            string ErrMsg = string.Empty;

            if (IsSingleValue)
            {
                PowerJson power = PowerBiz.GetTypePower(updateType, updateValue, ref ErrMsg);

                dict.All(o =>
                {
                    var ent = o["Name"];
                    if (power.Row.View.ContainsKey(ent))
                    {
                        o["ViewPower"] = string.Join(",", power.Row.View[ent].ToPositions().Select(p => p.AsString()).ToArray());
                    }

                    if (power.Row.Edit.ContainsKey(ent))
                    {
                        o["EditPower"] = string.Join(",", power.Row.Edit[ent].ToPositions().Select(p => p.AsString()).ToArray());
                    }

                    return true;
                });
            }

            return new ConstTable().LoadFlexiGrid(dict, dict.Count);
        }



        [HttpPost]
        public ActionResult UpdateRowPower(string Type, string Value, string RowType, string Mode, FormCollection collection)
        {
            PowerOwnerEnum updateType = Type.ToEnum<PowerOwnerEnum>();

            if (Value.HasValue() == false)
            {
                Value = string.Join(",", GetAllValue(updateType));
            }

            string updateValue = Value;
            bool IsSingleValue = !updateValue.Contains(",");
            string ErrMsg = string.Empty;
            var Entity = collection["Entity"];
            var NewPowerPositions = collection["Power"].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(o => o.AsInt());
            var NewPower = MyBigInt.CreateByBitPositons(NewPowerPositions);



            Func<string, string> SaveOneRowPower = (val) =>
            {
                PowerJson power = PowerBiz.GetTypePower(updateType, val, ref ErrMsg);


                if (RowType == "View")
                {
                    power.Row.View[Entity] = NewPower;
                }
                else if (RowType == "Edit")
                {
                    power.Row.Edit[Entity] = NewPower;
                }
                else
                {
                    throw new GodError("更新行集权限时不识别类型:" + RowType);
                }


                return PowerBiz.SaveTypePower(updateType, val, power);

            };

            Func<string, string, string> SaveOneRowPowerWithSomeRole = (val, mode) =>
            {
                PowerJson power = PowerBiz.GetTypePower(updateType, val, ref ErrMsg);


                MyBigInt ObjPower = null;

                if (RowType == "View")
                {
                    ObjPower = power.Row.View.GetOrDefault(Entity);
                }
                else if (RowType == "Edit")
                {
                    ObjPower = power.Row.Edit.GetOrDefault(Entity);
                }

                if (ObjPower == null) ObjPower = new MyBigInt();


                if (RowType == "Edit" || RowType == "View")
                {
                    if (mode == "Add")
                    {
                        ObjPower |= NewPower;
                    }
                    else
                    {
                        var FanPower = MyBigInt.Fill1(ObjPower.GetPositionLength());

                        NewPower.ToPositions().All(o =>
                        {
                            FanPower.SetFlag(o, false);
                            return true;
                        });

                        ObjPower &= FanPower;
                    }
                }

                else
                {
                    throw new GodError("更新行集权限时不识别类型:" + RowType);
                }



                if (RowType == "View")
                {
                    power.Row.View[Entity] = ObjPower;
                }
                else if (RowType == "Edit")
                {
                    power.Row.Edit[Entity] = ObjPower;
                }

                return PowerBiz.SaveTypePower(updateType, val, power);
            };

            var jm = new JsonMsg();

            if (IsSingleValue)
            {
                jm.msg = SaveOneRowPower(updateValue);
            }
            else
            {
                updateValue.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).All(o =>
                {
                    jm.msg = SaveOneRowPowerWithSomeRole(o, Mode);
                    if (jm.msg.HasValue())
                    {
                        jm.msg = "批量更新行集权限时出错而中止： " + jm.msg;
                        return false;
                    }
                    return true;
                });
            }

            return jm;
        }



    }
}