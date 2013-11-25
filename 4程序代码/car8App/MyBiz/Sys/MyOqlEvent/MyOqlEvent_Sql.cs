using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyOql;
using MyCmn;
using System.Data.Common;
using MyBiz;
using System.Configuration;
using DbEnt;


namespace MyBiz.Sys
{
    public partial class MyOqlEvent
    {

        bool MyOqlEvent_Executing(ContextClipBase arg)
        {
            var myCmd = arg.RunCommand;
            if (arg.ContainsConfig(ReConfigEnum.SkipLog))
            {
                LogTo(MyOqlActionEnum.All, myCmd.CurrentAction, InfoEnum.Info, () =>
                {
                    StringLinker msg = myCmd.Command.CommandText + Environment.NewLine;

                    foreach (DbParameter item in myCmd.Command.Parameters)
                    {
                        msg += "   " + item.ParameterName + ":" + item.Value.AsString();
                    }
                    if (myCmd.Command.Parameters.Count > 0) msg += Environment.NewLine;

                    return msg;
                });
            }
            return true;
        }
    }
}
