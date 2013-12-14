//系统自动生成的实体，不能修改。 By: UDI-PC.于新海  At:2013-12-09 19:32:05
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

        
    public static ProcClip PLoginClip( String UserID , String PassWord )
    {
        var entName = "P_LoginShop";
    
        ProcClip proc = new ProcClip(entName);
        var databaseType = dbo.GetDatabaseType(proc.CurrentRule.GetConfig().db);

        var provider = dbo.GetDbProvider(databaseType) ;
        var p_UserID = provider.GetParameter("UserID",DbType.AnsiString ,UserID );
        p_UserID.Direction = ParameterDirection.Input ;
        proc.AddParameter(p_UserID);
        var p_PassWord = provider.GetParameter("PassWord",DbType.String ,PassWord );
        p_PassWord.Direction = ParameterDirection.Input ;
        proc.AddParameter(p_PassWord);
        DbParameter p_result = null ;
        if (databaseType == DatabaseType.Oracle)
        {
            p_result = provider.GetParameter( "result" , (DbType)121 ,null) ;
            p_result.Direction = ParameterDirection.Output;
            p_result.Size = 4000 ;

            proc.AddParameter(p_result);
        }
        var p_ReturnValue = provider.GetParameter("ReturnValue", DbType.Int32, -1);
        p_ReturnValue.Direction = ParameterDirection.ReturnValue;
        proc.AddParameter(p_ReturnValue);

        return proc ;
    }
        
    public static ProcClip PEditPasswordClip( String UserID , String Passwrd )
    {
        var entName = "P_EditPassword";
    
        ProcClip proc = new ProcClip(entName);
        var databaseType = dbo.GetDatabaseType(proc.CurrentRule.GetConfig().db);

        var provider = dbo.GetDbProvider(databaseType) ;
        var p_UserID = provider.GetParameter("UserID",DbType.AnsiString ,UserID );
        p_UserID.Direction = ParameterDirection.Input ;
        proc.AddParameter(p_UserID);
        var p_Passwrd = provider.GetParameter("Passwrd",DbType.AnsiString ,Passwrd );
        p_Passwrd.Direction = ParameterDirection.Input ;
        proc.AddParameter(p_Passwrd);
        var p_ReturnValue = provider.GetParameter("ReturnValue", DbType.Int32, -1);
        p_ReturnValue.Direction = ParameterDirection.ReturnValue;
        proc.AddParameter(p_ReturnValue);

        return proc ;
    }

}

}