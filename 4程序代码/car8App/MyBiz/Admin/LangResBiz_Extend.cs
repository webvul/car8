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
    public partial class LangResBiz
    {


        //根据需要修改参数
        public static MyOqlSet Query(int Skip, int Take, OrderByClip Order, SelectOption Option, string Name)
        {
            WhereClip where = new WhereClip();

            if (Name.HasValue())
            {
                //where &= dbr.ResKey.Key.Like("%" + Name + "%");
            }



            return dbr.ResKey.Select()
                .Skip(Skip)
                .Take(Take)
                .Where(where)
                .OrderBy(Order)
                .ToMyOqlSet(Option);
        }

        public static JsonMsg Update(ResKeyRule.Entity Model)
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
                if (Model.Key.HasValue() == false)
                {
                    jm.msg = "更新对象,必须拥有唯一键,或主键!";
                    return jm;

                }

                //验证要更新的参数是否合法


                var updateResult = dbr.ResKey.Update(Model).Execute();
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

        public static JsonMsg Add(ResKeyRule.Entity Model)
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

                if (Model.Key.HasValue())
                {
                    jm.msg = "创建对象必须拥有唯一键或主键!";
                    return jm;
                }


                //验证要更新的参数是否合法


                var insertResult = dbr.ResKey.Insert(Model).Execute();
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
            var IdInDb = dbr.ResKey
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
                var delResult = dbr.ResKey.Delete(o => o.GetIdKey().In(IdArray)).Execute();
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



    }
}
