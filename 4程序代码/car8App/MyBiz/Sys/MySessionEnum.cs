using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCmn;

namespace MyBiz
{
    [Flags]
    public enum MySessionKey
    {
        UserID,
        UserName,
        LoginName,
        Power,
        WebName,
        //UserRole,
        DeptID,
        //TStandardRole,
        DeptName,
        //Langs,
        DefaultLang,
        Account,
        KeepInfo,
    }

    public enum HostSessionKey
    {
        MemberName,
        RegionId,
        RegionName,
    }
}
