using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCmn;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace MyTool
{

    public class GenSqlHandler : ICommandHandler
    {
        CmdArgs arg { get; set; }

        public string GenSql { get; set; }
        public string Dll { get; set; }
        public MyOql.DatabaseType DataBase { get; set; }


        public GenSqlHandler(CmdArgs arg)
        {
            this.arg = arg;

            arg.ToModel(this);
        }

        public string Do()
        {
            string content = "";
            switch (DataBase)
            {
                case MyOql.DatabaseType.DB2:
                    break;
                case MyOql.DatabaseType.Excel:
                    break;
                case MyOql.DatabaseType.MsAccess:
                    break;
                case MyOql.DatabaseType.MySql:
                    content = MyOql.MyOql_CodeGen.SqlScriptGen.MySql.Do(Assembly.LoadFile(new FileInfo(this.Dll).FullName).GetTypes());
                    break;
                case MyOql.DatabaseType.Oracle:
                    //content = MyOql.Helper.SqlScriptGen.Oracle.Do(Assembly.LoadFile(new FileInfo(this.Dll).FullName).GetTypes());
                    break;
                case MyOql.DatabaseType.Other:
                    break;
                case MyOql.DatabaseType.PostgreSql:
                    break;
                case MyOql.DatabaseType.SqlLite:
                    break;
                case MyOql.DatabaseType.SqlServer:
                    break;
                case MyOql.DatabaseType.SqlServer2000:
                    break;
                default:
                    break;
            }


            File.WriteAllText(GenSql, content);

            return "         ● 合并 　 完成   !";
        }
    }

}
