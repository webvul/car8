using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyOql;
using DbEnt;
using MyCon;

namespace MyWeb.Areas.Shop.Models
{
    public class IndexModel:IShopModel
    {
        [Serializable]
        public class ShowCaseModel
        {
            public string Name { get; set; }
            public string Img { get; set; }
            public int ProductID { get; set; }
        }
        [Serializable]
        public class NoticeShowCaseModel
        {
            public string Name { get; set; }
            public string Img { get; set; }
            public int NoticeID { get; set; }
            public string NoticeTypeName { get; set; }
        }

        public DeptRule.Entity Dept
        {
            get;
            set;
        }

        public string[] Imgs { get; set; }

        //public string Detail { get; set; }

        public ShowCaseModel[] ShowCases { get; set; }
        public NoticeShowCaseModel[] NoticeShowCases { get; set; }
    }
}