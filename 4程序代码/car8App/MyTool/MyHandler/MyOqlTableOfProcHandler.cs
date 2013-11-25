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
    /// 单库分析存储过程的表依赖
    /// </summary>
    public class MyOqlTableOfProcHandler : ICommandHandler
    {
        /// <summary>
        /// 分析结果文件
        /// </summary>
        public string MyOqlTableOfProc { get; set; }

        /// <summary>
        /// 分析存储过程的文件
        /// </summary>
        public string ProcXml { get; set; }

        public string db { get; set; }

        public MyOqlTableOfProcHandler(CmdArgs arg)
        {
            arg.ToModel(this);
        }

        public string Do()
        {
            var all_procs = dbo.ToDataTable(this.db, @"select object_name(object_id) as Name, [Definition] 
from sys.sql_modules
where object_name(object_id) in ( '" +
                                     XElement.Load(ProcXml).Descendants("Entity").Select(o => o.Attribute("Name").Value).ToArray().Join("','")
                                     + "' )");

            var all_tables = dbo.ToDataTable(this.db,
                @"select distinct name from sys.tables");

            var list = new List<string>();
            foreach (DataRow row in all_procs.Rows)
            {
                list.Add(string.Format(@"<Entity Name=""{0}"" AutoTable=""{1}"" />", row["Name"].AsString()
                    , string.Join(",", Get_Proc_Tables(row["Name"].AsString(), all_procs, all_tables, new List<string> { row["Name"].AsString() }))
                    ));
            }

            File.WriteAllText(MyOqlTableOfProc, string.Format(@"<?xml version=""1.0""?>
<!-- （自动生成）分析在 存储过程 中可能会变更的 表对象，By: " + SystemInformation.ComputerName + "." + WindowsGroupUser.GetSingleUserInfo(SystemInformation.UserName, null).FullName + "(" + Environment.UserName + ")" + "   At:" + DateTime.Now.ToString() + @" -->
<MyOqlCodeGen>
    <Proc>
        <Group>
            {0}
        </Group>
    </Proc>
</MyOqlCodeGen>", string.Join(Environment.NewLine, list.ToArray())
                 )
                 );
            return "分析数据库存储过程依赖表完成   !";
        }

        private static List<string> Get_Proc_Tables(string viewName, DataTable all_procs, DataTable all_tables, List<string> procedProc)
        {
            var sqlD = all_procs.AsEnumerable().FirstOrDefault(o => string.Equals(o["Name"].AsString(), viewName, StringComparison.CurrentCultureIgnoreCase));
            if (sqlD == null)
            {
                MyConsole.Write("\t找不到：" + viewName + "。已跳过\t", ConsoleColor.Yellow);
                MyConsole.WriteLine("Stack:" + string.Join(",", procedProc.ToArray()), ConsoleColor.DarkGray);
                return new List<string>() { };
            }
            var sql = sqlD["Definition"].AsString();
            var reg = new Regex(@"insert into\s*\[?(\w*)\]?\.?\[?\b(\w+)\b\]?|update\s*\[?(\w*)\]?\.?\[?\b(\w+)\b\]?|exec\s*\[?(\w*)\]?\.?\[?\b(\w+)\b\]?", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(sql);

            var tables = new List<string>();
            var procs = new List<string>();
            do
            {
                if (reg.Success == false) break;
                var tab = reg.Groups[2].Value.AsString(null) ?? reg.Groups[1].Value.AsString(null)
                    ?? reg.Groups[4].Value.AsString(null) ?? reg.Groups[3].Value.AsString(null);
                if (tab.HasValue())
                {
                    tables.Add(tab);
                }

                var proc = reg.Groups[6].Value.AsString(null) ?? reg.Groups[5].Value.AsString(null);
                if (proc.HasValue())
                {
                    procs.Add(proc);
                }
            } while ((reg = reg.NextMatch()) != null);


            procs.ForEach(o =>
            {
                if (new string[] { "sp_executesql", "sp_cursoropen", "sp_cursorclose", "sp_cursorfetch" }.Contains(s => string.Equals(s, o, StringComparison.CurrentCultureIgnoreCase)))
                    return;

                if (procedProc.Contains(s => string.Equals(s, o, StringComparison.CurrentCultureIgnoreCase))) return;
                procedProc.Add(o);

                tables.AddRange(Get_Proc_Tables(o, all_procs, all_tables, procedProc));
            });

            //可能会有 其它的存储过程。执行存储过程，应该添加 exec
            //应该避免在存储过程中 对视图进行更改。 而应该对 表 进行更改。

            //检测是否有存储过程的办法是：找出数据库中所有的存储过程，依次进行配置。

            //var listFind = new List<string>();
            //all_procs.AsEnumerable().All(s =>
            //{
            //    if (procedProc.IndexOf(o => string.Equals(o, s["Name"].AsString(), StringComparison.CurrentCultureIgnoreCase)) >= 0)
            //        return true;

            //    listFind.Add(@"\b" + s["Name"].AsString() + @"\b");
            //    return true;
            //});

            //var regProc = new Regex(string.Join("|",listFind.ToArray()), RegexOptions.IgnoreCase | RegexOptions.Compiled).Matches(sql);
            //foreach(Match m in regProc)
            //{
            //    if (m.Success == false) continue;

            //    procedProc.Add(m.Value);
            //    //tables.Add("【" + s + "】");

            //    tables.AddRange(Get_Proc_Tables(m.Value, all_procs, all_tables, procedProc));
            //}

            //var procs = all_procs.AsEnumerable().Where(r => r["Name"].IsIn((a, b) => string.Equals(a.AsString(), b.AsString(), StringComparison.CurrentCultureIgnoreCase), tables));// ,r=>r["Name"].AsString()).ToArray() ;
            ////name in ('" + string.Join("','", tables.ToArray()) + "'").ToEntityList(() => "");

            //procs.All(o =>
            //{
            //    if (procedProc.Contains(s => string.Equals(s, o["Name"].AsString(), StringComparison.CurrentCultureIgnoreCase)))
            //        return true;
            //    procedProc.Add(o["Name"].AsString());
            //    tables.AddRange(Get_Proc_Tables(o["Name"].AsString(), all_procs, all_tables, procedProc));
            //    return true;
            //});

            var res = all_tables.AsEnumerable().Where(r => r["Name"].AsString().IsIn(
                (a, b) => string.Equals(a.AsString(), b.AsString(), StringComparison.CurrentCultureIgnoreCase),
                tables.ToArray())).Select(o => o["Name"].AsString()).ToList()
                ;

            if (tables.Minus(res).Any())
            {
                MyConsole.WriteLine(string.Format(@"<Entity Name=""{0}"" Tables=""{1}"" />" + Environment.NewLine, viewName,
                     string.Join(",", tables.Minus(res).ToArray())), ConsoleColor.Yellow);
            }
            return res;
        }
    }
}
