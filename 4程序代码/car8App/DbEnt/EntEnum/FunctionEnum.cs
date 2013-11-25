using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCmn;

namespace DbEnt
{
    public enum FunctionEnum
    {
        [MyDesc("添加")]
        Add ,

        [MyDesc("删除")]
        Del ,

        [MyDesc("保存")]
        Save ,

    }
}
