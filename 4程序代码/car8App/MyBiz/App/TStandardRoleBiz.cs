using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using MyOql;
using DbEnt;




namespace MyBiz.App
{
    public partial class TStandardRoleBiz
    {

        public static void Clean(string[] StandardRoleIdList)
        {
            RoleRule.Entity[] roles = GetRoles(StandardRoleIdList);

            var actions = dbr.PowerAction.Select(o => o.Id).SkipPower().ToEntityList(0);
            var buttons = dbr.PowerButton.Select(o => o.Id).SkipPower().ToEntityList(0);
            var menus = dbr.Menu.Select(o => o.Id).SkipPower().ToEntityList(0);

            roles.All(o =>
                {
                    CleanOne(actions, buttons, menus, o);
                    return true;
                });
        }

        private static void CleanOne(List<int> actions, List<int> buttons, List<int> menus, RoleRule.Entity Ent)
        {
            var pj = new PowerJson(Ent.Power);
            pj.Action = MyBigInt.CreateBySqlRowIds(pj.Action.ToPositions().Intersect(actions));
            pj.Button = MyBigInt.CreateBySqlRowIds(pj.Button.ToPositions().Intersect(buttons));

            if (pj.Row.View.ContainsKey("Menu"))
            {
                pj.Row.View["Menu"] = MyBigInt.CreateBySqlRowIds(pj.Row.View["Menu"].ToPositions().Intersect(menus));
            }

            dbr.Role.Update(o => o.Power == pj.ToString(), o => o.Id == Ent.Id).Execute();
        }



        private static RoleRule.Entity[] GetRoles(string[] StandardRoleIdList)
        {
            var roleIds = StandardRoleIdList.Where(o => o.HasValue());

            if (roleIds.Count() == 0)
            {
                return dbr.Role.Select().ToEntityList(o => o._).ToArray();
            }
            else
            {
                return
                    dbr.Role
                    .SelectWhere(o => o.Id.In(StandardRoleIdList))
                    .ToEntityList(o => o._)
                    .ToArray();
            }
        }
    }
}
