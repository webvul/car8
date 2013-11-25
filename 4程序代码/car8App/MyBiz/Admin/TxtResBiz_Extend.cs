using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using MyOql;
using MyBiz;
using DbEnt;



namespace MyBiz.Admin
{
    public partial class TxtResBiz
    {
        public class QueryModel : QueryModelBase
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        //根据需要修改参数
        public static MyOqlSet Query(QueryModel query)
        {
            //WhereClip where = new WhereClip();

            //if (ResKey.HasValue())
            //{
            //    where &= dbr.ResKey.Key.Like("%" + ResKey + "%");
            //}

            //if (Value.HasValue())
            //{
            //    where &= dbr.ResValue.Value.Like("%" + Value + "%");
            //}



            var res = TxtResBiz.LoadRes(MyHelper.Lang)
                .Where(o =>
                {
                    return (query.Key.HasValue() == false || o.Key.IndexOf(query.Key, StringComparison.CurrentCultureIgnoreCase) >= 0) &&
                         (query.Value.HasValue() == false || (o.Value.HasValue() && o.Value.IndexOf(query.Value, StringComparison.CurrentCultureIgnoreCase) >= 0));
                });

            MyOqlSet set = new MyOqlSet(dbr.ResValue);
            set.Count = res.Count();

            if (query.sort.IsAsc)
            {
                set.Load(res.OrderBy(o => query.sort.Order.Name)
                .Skip(query.skip)
                .Take(query.take));

            }
            else
            {
                set.Load(res.OrderByDescending(o => query.sort.Order.Name)
                .Skip(query.skip)
                .Take(query.take)
                );

            }

            return set;
        }

        public static JsonMsg Update(VTxtResRule.Entity Model)
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
                if (Model.Id == 0)
                {
                    jm.msg = "更新对象,必须拥有唯一键,或主键!";
                    return jm;
                }

                //验证要更新的参数是否合法


                var updateResult = dbr.ResValue.Update(Model).Execute();
                if (updateResult == 0)
                {
                    jm.msg = "未更新记录";
                }
                else if (updateResult > 1)
                {
                    jm.msg = string.Format(@"更新了 {0} 条记录!", updateResult);
                    ClearCache(MyHelper.Lang);
                }
                else
                {
                    ClearCache(MyHelper.Lang);
                }
            }
            catch (Exception e)
            {
                //jm.msg = e.Message.GetSafeValue();
                e.ToLog();
                jm.msg = "修改失败,已记录到日志,详情请联系系统管理员.";
            }
            return jm;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public static int Add(VTxtResRule.Entity Model)
        {
            //验证要更新的参数是否合法

            var langRes = dbr.ResKey.Insert(o => o.Key == Model.Key & o.Group == Model.Group);
            langRes.Execute();

            return dbr.ResValue
                .Insert(o => o.ResID == langRes.LastAutoID & o.Lang == Model.Lang & o.Value == Model.Value)
                .Execute();
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
            var IdInDb = dbr.ResValue
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
                var delResult = dbr.ResValue.Delete(o => o.GetIdKey().In(IdArray)).Execute();
                if (delResult != IdArray.Length)
                {
                    jm.msg = string.Format("删除了 {0}/{1} 条记录.", delResult, IdArray.Length);
                }
            }
            catch (Exception ee)
            {
                ee.ToLog();
                jm.msg = "删除失败,已记录到日志,详情请联系系统管理员.";
            }

            return jm;
        }



    }
}
