using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbEnt
{
    public enum PowerClientTypeEnum
    {
        Button = 1 , 
        View = 2 ,
        Edit = 6 ,  //包含OnlyEdit=4 ＋ View
   }

    public enum PowerOwnerEnum
    {
        User = 1,
        Dept = 2,
        //Role = 4,
        NotMine,

        /// <summary>
        /// 龙湖使用的 标准角色
        /// </summary>
        TStandardRole,
    }
}
