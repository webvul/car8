using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyOql;
using DbEnt;
using MyCon;
using MyCmn;
using System.Web.Mvc;

namespace MyWeb.Areas.Shop.Models
{
    public class ProductsModel : IShopModel
    {

        [Serializable]
        public class ProductsDetail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Descr { get; set; }

            public string ProductType { get; set; }
            public string Img { get; set; }
        }

        public DeptRule.Entity Dept
        {
            get;
            set;
        }

        public IList<ProductsDetail> Products { get; set; }

        public int Count { get; set; }

        public ProductsModel()
        {
            this.Dept = new DeptRule.Entity();
        }
    }
}