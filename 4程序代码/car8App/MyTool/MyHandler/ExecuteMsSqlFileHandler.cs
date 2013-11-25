using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCmn;
using System.Configuration;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using MyOql;
using System.Windows.Forms;
using System.Web.Compilation;
using MyTool.MyHandler;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;

namespace MyTool
{
    public class ExecuteMsSqlFileHandler : ICommandHandler
    {
        public string ExecuteMsSqlFile { get; set; }
        public string db { get; set; }

        public ExecuteMsSqlFileHandler(CmdArgs arg)
        {
            arg.ToModel(this);
        }

        public string Do()
        {
            var sql = File.ReadAllText(ExecuteMsSqlFile);
            if (sql.HasValue() == false) return "文件里没有内容。";

            try
            {
                //执行脚本
                SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[this.db].ConnectionString);
                Microsoft.SqlServer.Management.Smo.Server server = new Server(new ServerConnection(conn));
                int i = server.ConnectionContext.ExecuteNonQuery(sql);

                if (i == 1)
                {
                    return "执行完成。";
                }
                else
                {
                    return "@_@ 执行过程中失败了。";
                }

            }
            catch (Exception es)
            {
                return es.Message;
            }

            return "";
        }

    }

}
