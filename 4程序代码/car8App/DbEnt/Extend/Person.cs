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
            private RoleRule.Entity[] _GetRoles = null;
            public RoleRule.Entity[] GetRoles()
            {
                return dbr.Role.SelectWhere(r =>
                r.Id.In(dbr.PersonRole.SelectWhere(
                    o => o.UserId == this.UserID)
                    ))
                    .ToEntityList(o => o._)
                    .ToArray();

            }
        }
    }
}
