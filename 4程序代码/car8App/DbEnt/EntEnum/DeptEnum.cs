using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCmn;

namespace DbEnt
{
    [Flags]
    public enum DeptBizTypeEnum
    {
        Production =1,
        Trade = 2,
        Service = 4,
    }

    [Flags]
    public enum DeptPayEnum
    {
        Normal = 1 ,
        Sliver = 2,
        Gold = 4 ,
    }
}
