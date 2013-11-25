using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyOql;

namespace MyWeb.Areas.Host.Models
{
    public class HostIndexModel
    {

    }

    public class MyOqlCacheModel
    {
        public string Type { get; set; }
        public string Object { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}