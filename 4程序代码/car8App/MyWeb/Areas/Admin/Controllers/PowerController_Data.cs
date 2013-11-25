using System;
using System.Linq;
using MyCon;
using System.Web.Mvc;
using DbEnt;
using MyOql;
using MyCmn;
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


        public ActionResult DataCreatePower(string Type, string User)
        {
            return View("TablePower");
        }
        public ActionResult DataDeletePower(string Type, string User)
        {
            return View("TablePower");
        }
        public ActionResult DataReadPower(string Type, string User)
        {
            return View("ColumnsPower");
        }

        //[HttpPost]
        //public ActionResult DataReadPowerQuery(string Type, string Value, FormCollection collection)
        //{
        //    var query = collection["query"].FromJson<XmlDictionary<string, string>>();

        //    PowerOwnerEnum updateType = Type.ToEnum<PowerOwnerEnum>();
        //    string updateValue = Value;
        //    bool IsSingleValue = !Value.Contains(",");
        //    string ErrMsg = string.Empty;

        //    GodError.Check(IsSingleValue == false, typeof(PowerController), () => "目前只支持单值设置权限");
        //    PowerJson power = PowerBiz.GetTypePower(updateType, updateValue, ref ErrMsg);

        //    GodError.Check(ErrMsg.HasValue(), typeof(PowerController), () => ErrMsg);

        //    var set = PowerBiz.GetColumnsPowerData(
        //        (collection["page"].AsInt() - 1) * collection["rp"].AsInt(),
        //        collection["rp"].AsInt(),
        //        query.GetOrDefault("TableName"),
        //        query.GetOrDefault("ColumnName"),
        //        power.Read
        //        );

        //    var retVal = set.LoadFlexiTreeGrid(dbr.PowerColumn.Id.Name, dbr.PowerColumn.TableID.Name);

        //    return retVal;
        //}




        public ActionResult DataUpdatePower(string Type, string Value)
        {
            return View("ColumnsPower");
        }

        //[HttpPost]
        //public ActionResult DataUpdatePowerQuery(string Type, string Value, FormCollection collection)
        //{
        //    var query = collection["query"].FromJson<XmlDictionary<string, string>>();

        //    PowerOwnerEnum updateType = Type.ToEnum<PowerOwnerEnum>();
        //    string updateValue = Value;
        //    bool IsSingleValue = !Value.Contains(",");
        //    string ErrMsg = string.Empty;

        //    GodError.Check(IsSingleValue == false, typeof(PowerController), () => "目前只支持单值设置权限");
        //    PowerJson power = PowerBiz.GetTypePower(updateType, updateValue, ref ErrMsg);

        //    GodError.Check(ErrMsg.HasValue(), typeof(PowerController), () => ErrMsg);

        //    var set = PowerBiz.GetColumnsPowerData(
        //        (collection["page"].AsInt() - 1) * collection["rp"].AsInt(),
        //        collection["rp"].AsInt(),
        //        query.GetOrDefault("TableName"),
        //        query.GetOrDefault("ColumnName"),
        //        power.Update
        //        );

        //    var retVal = set.LoadFlexiTreeGrid(dbr.PowerColumn.Id.Name, dbr.PowerColumn.TableID.Name);

        //    return retVal;

        //}


        [HttpPost]
        public ActionResult DataUpdatePowerSave(string Type, string Value, FormCollection collection)
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

                //权限字中不允许有Max. 待修改.
                if (power.Action.Max == false)
                {
                    power.Update |= MyBigInt.CreateByBitPositons(adds.Where(o => o.StartsWith("C_")).Select(o => o.Substring(2).AsInt()));
                    power.Update.Minus(MyBigInt.CreateByBitPositons(minus.Where(o => o.StartsWith("C_")).Select(o => o.Substring(2).AsInt())));
                }

                return PowerBiz.SaveTypePower(updateType, val, power);
            };
            if (IsSingleValue)
            {
                jm.msg = SaveOneValuePower(updateValue);
            }
            else
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

            return jm;
        }

        [HttpPost]
        public ActionResult DataReadPowerSave(string Type, string Value, FormCollection collection)
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

                //权限字中不允许有Max. 待修改.
                if (power.Action.Max == false)
                {
                    power.Read |= MyBigInt.CreateByBitPositons(adds.Where(o => o.StartsWith("C_")).Select(o => o.Substring(2).AsInt()));
                    power.Read.Minus(MyBigInt.CreateByBitPositons(minus.Where(o => o.StartsWith("C_")).Select(o => o.Substring(2).AsInt())));
                }

                return PowerBiz.SaveTypePower(updateType, val, power);
            };
            if (IsSingleValue)
            {
                jm.msg = SaveOneValuePower(updateValue);
            }
            else
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

            return jm;
        }


        //[HttpPost]
        //public ActionResult DataCreatePowerQuery(string Type, string Value, string Mode, FormCollection collection)
        //{
        //    PowerOwnerEnum updateType = Type.ToEnum<PowerOwnerEnum>();
        //    string updateValue = Value;
        //    bool IsSingleValue = !Value.Contains(",");
        //    string ErrMsg = string.Empty;

        //    GodError.Check(IsSingleValue == false, typeof(PowerController), () => "目前只支持单值设置权限");

        //    var skip = (collection["page"].AsInt() - 1) * collection["rp"].AsInt();
        //    var take = collection["rp"].AsInt();
        //    var data = dbr.PowerTable.NoPower().Select()
        //        .Skip(skip)
        //        .Take(take)
        //        .ToDictList();

        //    var count = dbr.PowerTable
        //        .Select(o => o.Count()).ToScalar().AsInt();

        //    if (IsSingleValue)
        //    {
        //        var myPower = PowerBiz.GetTypePower(updateType, updateValue, ref ErrMsg);


        //        data.All(o =>
        //        {
        //            o["Sel"] = myPower.Create.ToPositions().Contains(o["Id"].AsInt()).ToString();

        //            return true;
        //        });

        //    }

        //    return dbr.PowerTable.LoadFlexiGrid(data, count);
        //}

        [HttpPost]
        public ActionResult DataCreatePowerSave(string Type, string Value, string Mode, FormCollection collection)
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

                //权限字中不允许有Max. 待修改.
                if (power.Action.Max == false)
                {
                    power.Create |= MyBigInt.CreateByBitPositons(adds.Select(o => o.AsInt()));
                    power.Create.Minus(MyBigInt.CreateByBitPositons(minus.Select(o => o.AsInt())));
                }

                return PowerBiz.SaveTypePower(updateType, val, power);
            };
            if (IsSingleValue)
            {
                jm.msg = SaveOneValuePower(updateValue);
            }
            else
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

            return jm;
        }

        //[HttpPost]
        //public ActionResult DataDeletePowerQuery(string Type, string Value, string Mode, FormCollection collection)
        //{
        //    PowerOwnerEnum updateType = Type.ToEnum<PowerOwnerEnum>();
        //    string updateValue = Value;
        //    bool IsSingleValue = !Value.Contains(",");
        //    string ErrMsg = string.Empty;

        //    GodError.Check(IsSingleValue == false, typeof(PowerController), () => "目前只支持单值设置权限");

        //    var skip = (collection["page"].AsInt() - 1) * collection["rp"].AsInt();
        //    var take = collection["rp"].AsInt();
        //    var data = dbr.PowerTable.NoPower().Select()
        //        .Skip(skip)
        //        .Take(take)
        //        .ToDictList();

        //    var count = dbr.PowerTable
        //        .Select(o => o.Count()).ToScalar().AsInt();

        //    if (IsSingleValue)
        //    {
        //        var myPower = PowerBiz.GetTypePower(updateType, updateValue, ref ErrMsg);


        //        data.All(o =>
        //        {
        //            o["Sel"] = myPower.Delete.ToPositions().Contains(o["Id"].AsInt()).ToString();

        //            return true;
        //        });

        //    }

        //    return dbr.PowerTable.LoadFlexiGrid(data, count);
        //}

        [HttpPost]
        public ActionResult DataDeletePowerSave(string Type, string Value, string Mode, FormCollection collection)
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

                //权限字中不允许有Max. 待修改.
                if (power.Action.Max == false)
                {
                    power.Delete |= MyBigInt.CreateByBitPositons(adds.Select(o => o.AsInt()));
                    power.Delete.Minus(MyBigInt.CreateByBitPositons(minus.Select(o => o.AsInt())));
                }

                return PowerBiz.SaveTypePower(updateType, val, power);
            };
            if (IsSingleValue)
            {
                jm.msg = SaveOneValuePower(updateValue);
            }
            else
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

            return jm;
        }
    }
}