using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyOql;
using DbEnt;
using MyCon;

namespace MyWeb.Areas.Shop.Models
{
    public class AboutModel:IShopModel
    {
        public DeptRule.Entity Dept
        {
            get;
            set;
        }

        //public string Detail { get; set; }
        public string[] Imgs { get; set; }

        //public DeptDetailRule.Entity[] Info { get; set; }
    }
}