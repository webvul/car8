using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using System.Data.Common;
using MyCmn;
using System.IO;
using DbEnt.SysEnum;
using MyCmn;
using System.Web.Mvc;

namespace DbEnt
{
    public partial class AnnexRule
    {
        public partial class Entity
        {
            public string GetUrlFull()
            {
                return this.FullName.GetUrlFull();
            }
        }
    }
}
