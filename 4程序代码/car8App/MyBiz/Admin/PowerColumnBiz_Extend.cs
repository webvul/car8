//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using MyCmn;
//using MyOql;
//using MyBiz;
//using DbEnt;


//namespace MyBiz.Admin
//{
//    public partial class PowerColumnBiz
//    {
//        public class QueryModel : QueryBase
//        {
//            public string Table { get; set; }
//            public string Column { get; set; }
//            public string Descr { get; set; }
//        }



//        //根据需要修改参数
//        public static MyOqlSet Query(QueryModel Query)
//        {
//            WhereClip where = new WhereClip();

//            Query.Table.HasValue(o => where &= dbr.PowerTable.Table.Like("%" + o + "%"));
//            Query.Column.HasValue(o => where &= dbr.PowerColumn.Column.Like("%" + o + "%"));
//            Query.Descr.HasValue(o => where &= dbr.PowerColumn.Descr.Like("%" + o + "%"));

//            if (Query.sort.HasValue() == false)
//            {
//                Query.sort = dbr.PowerTable.Table.Asc;
//            }

//            return dbr.PowerColumn.Select()
//                .Join(dbr.PowerTable, (a, b) => a.TableID == b.Id, o => o.Table)
//                .Skip(Query.skip)
//                .Take(Query.take)
//                .Where(where)
//                .OrderBy(o => Query.sort)
//                .ToMyOqlSet(Query.option);
//        }

//        public static JsonMsg<int> Update(PowerColumnRule.Entity Model)
//        {
//            //验证是否包含唯一约束列 或 主键 ； 是否包含其它必须参数.      如果是联合主键,判断方式会稍有不同,请自行修改.


//            if (Model.Id == 0)
//            {
//                return new JsonMsg<int>() { msg = "更新对象,必须拥有唯一键,或主键!" };
//            }


//            var jm = new JsonMsg<int>();
//            try
//            {
//                jm.value = dbr.PowerColumn.Update(Model).Execute();
//            }
//            catch (Exception e)
//            {
//                jm.msg = e.Message.GetSafeValue();
//            }
//            return jm;
//        }

//        public static JsonMsg<int> Add(PowerColumnRule.Entity Model)
//        {





//            var jm = new JsonMsg<int>();
//            try
//            {
//                jm.value = dbr.PowerColumn.Insert(Model).Execute();
//            }
//            catch (Exception e)
//            {
//                jm.msg = e.Message.GetSafeValue();
//            }
//            return jm;
//        }

//        //根据需要修改参数
//        public static JsonMsg<int> Delete(string[] IdArray)
//        {
//            //判断要删除的Id 是否合法.
//            if (IdArray.Length == 0)
//            {
//                return new JsonMsg<int> { msg = "没有要删除的对象!" };
//            }
//            var IdInDb = dbr.PowerColumn
//                    .Select(o => o.GetIdKey())
//                    .Where(o => o.GetIdKey().In(IdArray))
//                    .ToEntityList(string.Empty);

//            var dbLoseId = IdArray.Minus(IdInDb);
//            if (dbLoseId.Count() > 0)
//            {
//                return new JsonMsg<int>()
//                {
//                    msg = string.Format("要删除的数据Id ({0}) 在数据库中不存在, 可能数据已过时,请刷新后再试.", string.Join(",", dbLoseId.ToArray()))
//                };
//            }

//            //添加业务判断。

//            var jm = new JsonMsg<int>();
//            try
//            {
//                var delResult = dbr.PowerColumn.Delete(o => o.GetIdKey().In(IdArray)).Execute();
//                jm.value = delResult;
//                if (delResult != IdArray.Length)
//                {
//                    jm.msg = string.Format("删除了 {0}/{1} 条记录.", delResult, IdArray.Length);
//                }
//            }
//            catch (Exception ee)
//            {
//                jm.msg = ee.Message.GetSafeValue();
//            }

//            return jm;
//        }



//    }
//}
