//系统自动生成的实体，不能修改。 By: UDI-PC.于新海  At:2013-12-15 11:12:12
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

public partial class dbr
{

        


    
    public  static PersonRule.Entity PLogin( System.String UserID , System.String PassWord )
    { 
        var proc = dbr_Proc.PLoginClip( UserID , PassWord ) ;
        PersonRule.Entity retVal = default(  PersonRule.Entity ) ;
        proc.ExecuteReader(o =>
        {
            retVal =dbo.ToEntity(o,retVal);
            return false;
        });
        return retVal;
    }

        


    
    public  static int PEditPassword( System.String UserID , System.String Passwrd )
    { 
        var proc = dbr_Proc.PEditPasswordClip( UserID , Passwrd ) ;
        int retVal = proc.Execute() ;
        return retVal;
    }


}

}