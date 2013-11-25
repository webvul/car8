using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCmn;

namespace DbEnt
{
    /// <summary>
    /// Dict 是 字典表， 是所有程序里公共用的字典表。
    /// ExtDesc 内容是 枚举内容。
    /// </summary>
    public enum DictKeyEnum
    {
        /// <summary>
        /// 系统字典。
        /// </summary>
        Sys = 1,
        Bus = 2,
        SkinEnum,
        SysName,
        DisplayLevel,
        ListBrief,
        MySkinJs,
    }

    public enum DictTraitEnum
    {
        String,
        Enum,
        Int,
        Number,
        Json,
        Xml,
        Time,
        Date,
        DateTime,
        Bool,
        Array,
        Object,
    }

    //public enum DictLayOutEnum
    //{
    //    none,
    //    /// <summary>
    //    /// 系统字典。
    //    /// </summary>
    //    [MyDesc("left")]
    //    left ,
    //    /// <summary>
    //    /// 系统字典。
    //    /// </summary>
    //    [MyDesc("right")]
    //    right ,
    //}
}

