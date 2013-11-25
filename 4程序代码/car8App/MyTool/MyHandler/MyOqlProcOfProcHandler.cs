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
using System.Data;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MyTool
{
    /// <summary>
    /// 单库分析存储过程依赖的存储过程
    /// </summary>
    public class MyOqlProcOfProcHandler : ICommandHandler
    {
        public string MyOqlProcOfProc { get; set; }
        /// <summary>
        /// 分析存储过程的文件
        /// </summary>
        public string ProcXml { get; set; }


        public string db { get; set; }

        public MyOqlProcOfProcHandler(CmdArgs arg)
        {
            arg.ToModel(this);
        }

        public string Do()
        {
            var all_procs = dbo.ToDataTable(this.db, @"select object_name(object_id) as Name, [Definition] ,'' as [Proc]
from sys.sql_modules
where object_name(object_id) in ( '" +
                        XElement.Load(ProcXml).Descendants("Entity").Select(o => o.Attribute("Name").Value).ToArray().Join("','")
                        + "' )");

            var dict = new StringDict();

            var brc = new BeginRepeatConsole();
            var st = Stopwatch.StartNew();
            var maxTimeEnt = "";
            long maxTime = 0;
            for (var i = 0; i < all_procs.Rows.Count; i++)
            {
                st.Restart();
                DataRow row = all_procs.Rows[i];
                dict[row["Name"].AsString()] = Gen_Proc_Procs(row["Name"].AsString(), all_procs, new List<string> { row["Name"].AsString() });

                if (st.ElapsedMilliseconds > maxTime)
                {
                    maxTime = st.ElapsedMilliseconds;
                    maxTimeEnt = row["Name"].AsString();
                }

                brc.Write("\t进度：" + i + "/" + all_procs.Rows.Count + "\t(" + maxTimeEnt + "=" + TimeSpan.FromMilliseconds(maxTime).GetSummary() + ")", ConsoleColor.Yellow);
            }


            File.WriteAllText(MyOqlProcOfProc, string.Format(@"<?xml version=""1.0""?>
<!-- （自动生成）分析在 存储过程 中调用的其它 存储过程，By: " + SystemInformation.ComputerName + "." + WindowsGroupUser.GetSingleUserInfo(SystemInformation.UserName, null).FullName + "(" + Environment.UserName + ")" + "   At:" + DateTime.Now.ToString() + @" -->
<MyOqlCodeGen>
    <Proc>
        <Group>
            {0}
        </Group>
    </Proc>
</MyOqlCodeGen>", string.Join(Environment.NewLine, dict.Where(o => o.Value.HasValue()).Select(o => string.Format(@"<Entity Name=""{0}"" Proc=""{1}"" />", o.Key, o.Value)).ToArray())
                 )
                 );
            return "● 分析数据库存储过程依赖表完成   !";
        }

        private string Gen_Proc_Procs(string proc, DataTable all_procs, List<string> procedProc)
        {
            var row = all_procs.AsEnumerable().First(o => o["Name"].AsString() == proc);

            var define = row["Definition"].AsString();

            var list = new List<string>();

            all_procs.AsEnumerable().All(o =>
                {
                    var item = o["Name"].AsString();
                    if (procedProc.Contains(item)) return true;
                    var index = define.IndexOf(item);
                    if (index > 0 && define[index - 1].IsIn(' ', '\t', '\r', '\n', '[', '"'))
                    {
                        if (index < define.Length)
                        {
                            if (define[index + item.Length].IsIn(' ', '\t', '\r', '\n', ']', '"'))
                            {
                                list.Add(item);
                            }
                        }
                        else list.Add(item);
                    }
                    return true;
                });

            row["Proc"] = string.Join(",", list.ToArray());

            return row["Proc"].AsString();
        }
    }
}
