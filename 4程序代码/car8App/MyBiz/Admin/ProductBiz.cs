using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using MyOql;
using MyBiz;


namespace MyBiz.Admin
{
	public partial class ProductionBiz 
	{
        public class ProductTypeQueryModel : QueryModelBase
        {
            public string Name { get; set; }
            public string Wbs { get; set; }
        }

        public class ProductInfoQueryModel : QueryModelBase
        {
            public string Name { get; set; }
            public string ProductTypeID { get; set; }
        }   
    }
}
