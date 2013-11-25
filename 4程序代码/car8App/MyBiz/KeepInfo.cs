using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyOql;
using System.Web.Mvc;
using MyBiz.Admin;
using MyCmn;
using System.Data.SqlClient;
using System.Data.Common;
using System.Xml;
using System.Web;
using System.Threading;
using System.Security.Cryptography;
using System.IO;
using System.Data;

namespace MyBiz
{
    public class KeepInfoModel
    {
        public KeepInfoItem Key { get; set; }

        public KeepInfoItem Value { get; set; }

        public KeepInfoModel(string k , string v)
        {
            Key = new KeepInfoItem(k);
            Value = new KeepInfoItem(v);
        }

        public KeepInfoModel(string v)
        {
            this.Key = new KeepInfoItem(v);
        }
    }

    public class KeepInfoItem : List<string>
    {
 
        public KeepInfoItem(string v)
        {
            this.AddRange(v.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
        }
    }
}


