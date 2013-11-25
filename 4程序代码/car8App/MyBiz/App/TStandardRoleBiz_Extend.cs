using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using MyOql;
using DbEnt;
using MyBiz;



namespace MyBiz.App
{
    public partial class TStandardRoleBiz
    {
        //根据需要修改参数
        public static MyOqlSet Query(int Skip, int Take, string sortName, bool IsAsc, SelectOption Option,
            string Code, string Name, int MenuId)
        {
            WhereClip where = new WhereClip();

            if (Code.HasValue())
            {
                where &= dbr.TStandardRole.Code.Like("%" + Code + "%");
            }

            if (Name.HasValue())
            {
                where &= dbr.TStandardRole.Name.Like("%" + Name + "%");
            }

            OrderByClip order = new OrderByClip(sortName);
            order.IsAsc = IsAsc;


            //如果有 MenuId 则再进行一次权限过滤。
            if (MenuId > 0)
            {
                var set = dbr.TStandardRole.Select()
                    .Where(where)
                    .OrderBy(order)
                    .ToMyOqlSet(Option);

                var takeIds = new List<int>();
                var rowIndex = -1;
                set.Rows.All(o =>
                {
                    rowIndex++;


                    //if (takeIds.Count > Skip + Take) return false;

                    var power = new PowerJson(o[dbr.TStandardRole.Power.Name].AsString());
                    //if (power.IsMax)
                    //{
                    //    takeIds.Add(rowIndex);
                    //    return true;
                    //}

                    //if (power.Row.IsMax)
                    //{
                    //    takeIds.Add(rowIndex);
                    //    return true;
                    //}

                    if (power.Row.View.ContainsKey(dbr.Menu.GetDbName()))
                    {
                        var menuPower = power.Row.View[dbr.Menu.GetDbName()];
                        if (menuPower.ToPositions().Contains(MenuId))
                        {
                            takeIds.Add(rowIndex);
                            return true;
                        }
                    }
                    return true;
                });

                set.Count = takeIds.Count;
                var takeRow = new List<RowData>();

                takeIds.All(o =>
                    {
                        takeRow.Add(set.Rows[o]);
                        return true;
                    });

                set.Rows = takeRow;
                return set;
            }
            else
            {
                return dbr.TStandardRole.Select()
                    .Skip(Skip)
                    .Take(Take)
                    .Where(where)
                    .OrderBy(order)
                    .ToMyOqlSet(Option);

            }
        }

        public static JsonMsg Update(TStandardRoleRule.Entity Model)
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
                if (Model.StandardRoleId.HasValue() == false)
                {
                    jm.msg = "更新对象,必须拥有唯一键,或主键!";
                    return jm;

                }

                //验证要更新的参数是否合法


                var updateResult = dbr.TStandardRole.Update(Model).Execute();
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
                jm.msg = e.Message.GetSafeValue();
            }
            return jm;
        }

        public static JsonMsg Add(TStandardRoleRule.Entity Model)
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


                var insertResult = dbr.TStandardRole.Insert(Model).Execute();
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
            var IdInDb = dbr.TStandardRole
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
                var delResult = dbr.TStandardRole.Delete(o => o.GetIdKey().In(IdArray)).Execute();
                if (delResult != IdArray.Length)
                {
                    jm.msg = string.Format("删除了 {0}/{1} 条记录.", delResult, IdArray.Length);
                }
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;
        }

        //public static MyOqlSet QueryUserByStanRole(int Skip, int Take, string sortName, bool IsAsc, SelectOption Option,
        //    string StandardRoleId)
        //{
        //    WhereClip where = new WhereClip();

        //    if (StandardRoleId.HasValue())
        //    {
        //        where &= dbr.View.VwUserRole.StandardRoleId == StandardRoleId;
        //    }

        //    OrderByClip order = new OrderByClip(sortName);
        //    order.IsAsc = IsAsc;

        //    return dbr.View.VwUserRole.Select(o => new ColumnClip[] { (o.UserId + "," + o.RoleId).As("Id"), o.UserName, o.RoleName.As("Company") })
        //        //.Join(dbr.TOrganization.As("COM"),(a,b)=>a.CompanyCode==b.Code.FromTable("COM"),o=>o.Name.FromTable("COM").As("Company"))
        //        //.Join(dbr.TOrganization,(a,b)=>a.Code==b.Code,o=>o.Name.As("Project"))
        //        //.Join(dbr.TRole,(a,b)=>a.RoleId==b.RoleId,o=>o.RoleName.As())
        //        .Skip(Skip)
        //        .Take(Take)
        //        .Where(where)
        //        .Distinct()
        //        .OrderBy(o => o.UserName.Asc)
        //        .ToMyOqlSet(Option);
        //}
    }
}
