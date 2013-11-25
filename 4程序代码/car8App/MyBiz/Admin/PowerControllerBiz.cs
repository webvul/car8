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
    public partial class PowerControllerBiz
    {
        public class QueryModel : QueryModelBase
        {
            public string Controller { get; set; }
            public int Descr { get; set; }
        }

        public static MyOqlSet Query(string uid,QueryModel Query)
        {
            WhereClip where = new WhereClip();

            Query.Controller.HasValue(o => where &= dbr.PowerController.Controller.Like("%" + o + "%"));
            Query.Descr.HasValue(o => where &= dbr.PowerController.Descr.Like("%" + o + "%"));

            //如果是引用。
            //if (uid == "ListPower")
            //{
            //    where &= dbr.PowerController.Status == IsAbleEnum.Enable;
            //}

            using (var scope = new MyOqlConfigScope(MySession.IsSysAdmin() ? ReConfigEnum.SkipPower : (ReConfigEnum)0))
            {
                return dbr.PowerController.Select()
                    .Skip(Query.skip)
                    .Take(Query.take)
                    .Where(where)
                    .OrderBy(Query.sort)
                    .ToMyOqlSet();
            }
        }


    }
}
