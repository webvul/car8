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
    public partial class PersonBiz
    {
        public class QueryModel : QueryModelBase
        {
            public string UserID { get; set; }
            public string Name { get; set; }
            public string Mobile { get; set; }
        }

        public static MyOqlSet Query(QueryModel Query)
        {
            WhereClip where = new WhereClip();

            Query.Name.HasValue(o => where &= dbr.Person.Name.Like("%" + o + "%"));
            Query.UserID.HasValue(o => where &= dbr.Person.UserID.Like("%" + o + "%"));
            Query.Mobile.HasValue(o => where &= dbr.Person.Mobile.Like("%" + o + "%"));

            return dbr.Person.Select()
                .LeftJoin(dbr.Dept, (a, b) => a.DeptID == b.Id, o => o.Name.As("Dept"))
                .Skip(Query.skip)
                .Take(Query.take)
                .Where(where)
                .OrderBy(Query.sort)
                .ToMyOqlSet();
        }
    }
}

