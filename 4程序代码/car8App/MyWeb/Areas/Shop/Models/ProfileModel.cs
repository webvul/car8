using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyOql;
using DbEnt;
using MyCon;

namespace MyWeb.Areas.Shop.Models
{
    public class NoticeModel:IShopModel
    {
        [Serializable]
        public class NoticeDetail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Descr { get; set; }

            public string NoticeType { get; set; }
            public string Img { get; set; }            
        }

        public DeptRule.Entity Dept
        {
            get;
            set;
        }

        public IList<NoticeDetail> Notices { get; set; }

        public int Count { get; set; }

        public NoticeModel()
        {
            this.Dept = new DeptRule.Entity();
        }
    }
}