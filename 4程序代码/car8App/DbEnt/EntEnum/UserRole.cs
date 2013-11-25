using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCmn;

namespace DbEnt
{
    [Flags()]
    public enum RoleEnum 
    {
        /// <summary>
        /// 系统管理员
        /// </summary>
        [MyDesc("系统管理员")]
        SysAdmin = 1,

        ///// <summary>
        ///// 系统管理员
        ///// </summary>
        //[MyDesc("站点管理员")]
        //Admin = 2,

        //[MyDesc("所有角色")]
        //All = 1023 ,

    }
}
