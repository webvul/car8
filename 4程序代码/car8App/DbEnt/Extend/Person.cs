using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MyCmn;
using MyOql;

namespace DbEnt
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PersonRule
    {
        public partial class Entity
        {
            private TStandardRoleRule.Entity[] _GetRoles = null;
            public TStandardRoleRule.Entity[] GetRoles()
            {
                return dbr.TStandardRole.SelectWhere(o => o.Code == ConfigKey.StandardRoleCode.Get()).ToEntityList(o => o._).ToArray();
            }

        }
    }
}
