//系统自动生成的实体，不能修改。 By: UDI-PC.  At:2013-11-15 13:05:55
using System;
using MyOql;
using MyCmn;
using System.Linq;
using System.Data ;
using System.Data.Common ;
using System.Collections.Generic;
using System.Runtime.Serialization ;
using DbEnt;

namespace DbEnt
{

public partial class dbr_Proc
{

        
    public static ProcClip SpImportRoomOwnerClip( String commID )
    {
        var entName = "SP_Import_RoomOwner";
    
        ProcClip proc = new ProcClip(entName);
        var databaseType = dbo.GetDatabaseType(proc.CurrentRule.GetConfig().db);

        var provider = dbo.GetDbProvider(databaseType) ;
        var p_commID = provider.GetParameter("commID",DbType.AnsiString ,commID );
        p_commID.Direction = ParameterDirection.Input ;
        proc.AddParameter(p_commID);
        var p_ReturnValue = provider.GetParameter("ReturnValue", DbType.Int32, -1);
        p_ReturnValue.Direction = ParameterDirection.ReturnValue;
        proc.AddParameter(p_ReturnValue);

        return proc ;
    }

}

}