using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyOql;
using DbEnt;
using MyCon;

namespace MyWeb.Areas.Shop.Models
{
    public class ProductInfoModel:IShopModel
    {
        public DeptRule.Entity Dept
        {
            get;
            set;
        }

        public ProductInfoRule.Entity ProductInfo { get; set; }
        public int Clicks { get; set; }
        public string[] Imgs { get; set; }
    }

    public class NoticeInfoModel : IShopModel
    {
        public DeptRule.Entity Dept
        {
            get;
            set;
        }

        public NoticeInfoRule.Entity NoticeInfo { get; set; }
        public string[] Imgs { get; set; }
    }

}