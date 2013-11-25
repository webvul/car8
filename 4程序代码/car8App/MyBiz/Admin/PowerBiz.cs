using System;
using MyCmn;
using MyOql;
using MyBiz.Sys;
using DbEnt;


namespace MyBiz.Admin
{
    public partial class PowerBiz
    {
        /// <summary>
        /// 返回值代表错误消息 .
        /// </summary>
        /// <param name="PowerOwner"></param>
        /// <param name="Value"></param>
        /// <param name="Power"></param>
        /// <returns></returns>
        public static PowerJson GetTypePower(PowerOwnerEnum PowerOwner, string Value, ref string ErrorMsg)
        {
            switch (PowerOwner)
            {
                case PowerOwnerEnum.User:
                    var person = dbr.Person.FindByUserID(Value);
                    if (person == null) { ErrorMsg = "找不到用户！"; return null; }

                    return new PowerJson(person.Power);
                case PowerOwnerEnum.Dept:
                    var dept = dbr.Dept.FindById(Value.AsInt());
                    if (dept == null) { ErrorMsg = "找不到该部门！"; return null; }

                    return new PowerJson(dept.Power);
                //case PowerOwnerEnum.Role:
                //    var role = dbr.Role.FindById(Value.AsInt());
                //    if (role == null) { ErrorMsg = "找不到该角色！"; return null; }

                //    return new PowerJson(role.Power);

                case PowerOwnerEnum.TStandardRole:
                    var sr = dbr.TStandardRole.FindByStandardRoleId(new Guid(Value));
                    if (sr == null) { ErrorMsg = "找不到标准角色!"; return null; }
                    return new PowerJson(sr.Power);
                case PowerOwnerEnum.NotMine:
                    var personNotMine = dbr.Person.FindByUserID(Value);
                    if (personNotMine == null) { ErrorMsg = "找不到用户！"; return null; }

                    return new PowerJson(personNotMine.NotPower);
                default:
                    break;
            }
            ErrorMsg = "不识别的权限类型";
            return null;
        }


        public static string SaveTypePower(PowerOwnerEnum PowerOwner, string Value, PowerJson Power)
        {
            switch (PowerOwner)
            {
                case PowerOwnerEnum.User:
                    var person = dbr.Person.FindByUserID(Value);
                    if (person == null) { return "找不到用户！"; }

                    if (dbr.Person.Update(o => o.UserID == Value & o.Power == Power.ToString()).Execute() == 0)
                        return "未更新权限.";
                    return string.Empty;

                case PowerOwnerEnum.Dept:
                    var dept = dbr.Dept.FindById(Value.AsInt());
                    if (dept == null) { return "找不到该部门！"; }

                    if (dbr.Dept.Update(o => o.Id == Value & o.Power == Power.ToString()).Execute() == 0)
                        return "未更新权限.";
                    return string.Empty;

                //case PowerOwnerEnum.Role:
                //    var role = dbr.Role.FindById(Value.AsInt());
                //    if (role == null) { return "找不到该角色！"; }

                //    if (dbr.Role.Update(o => o.Id == Value & o.Power == Power.ToString()).Execute() == 0)
                //        return "未更新权限.";
                //    return string.Empty;
                case PowerOwnerEnum.NotMine:
                    var personNotMine = dbr.Person.FindByUserID(Value);
                    if (personNotMine == null) { return "找不到用户！"; }

                    if (dbr.Person.Update(o => o.UserID == Value & o.NotPower == Power.ToString()).Execute() == 0)
                        return "未更新权限.";
                    return string.Empty;

                case PowerOwnerEnum.TStandardRole:
                    var sr = dbr.TStandardRole.FindByStandardRoleId(new Guid(Value));
                    if (sr == null) { return "找不到标准角色!"; }


                    if (dbr.TStandardRole.Update(o => o.StandardRoleId == new Guid(Value) & o.Power == Power.ToString()).Execute() == 0)
                        return "未更新权限.";
                    return string.Empty;

                default:
                    break;
            }
            return "错误";
        }
    }
}
