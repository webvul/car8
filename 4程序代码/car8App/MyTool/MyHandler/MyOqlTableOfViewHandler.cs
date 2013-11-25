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
using System.Text.RegularExpressions;
using System.Data;
using System.Xml.Linq;

namespace MyTool
{
    /// <summary>
    /// 单库分析 视图的表依赖
    /// </summary>
    public class MyOqlTableOfViewHandler : ICommandHandler
    {
        public string MyOqlTableOfView { get; set; }

        /// <summary>
        /// 分析存储过程的文件
        /// </summary>
        public string ViewXml { get; set; }

        public string db { get; set; }

        public MyOqlTableOfViewHandler(CmdArgs arg)
        {
            arg.ToModel(this);
        }

        public string Do()
        {
            var all_views = dbo.ToDataTable(this.db, @"select object_name(object_id) as Name, [Definition] 
from sys.sql_modules
where object_name(object_id) in ( '" +
                        XElement.Load(ViewXml).Descendants("Entity").Select(o => o.Attribute("Name").Value).ToArray().Join("','")
                        + "' )");

            var all_tables = dbo.ToDataTable(this.db,
                @"select distinct name from sys.tables");

            var list = new List<string>();
            foreach (DataRow row in all_views.Rows)
            {
                list.Add(string.Format(@"<Entity Name=""{0}"" AutoTable=""{1}"" />", row["Name"].AsString()
                    , string.Join(",", Get_View_Tables(row["Name"].AsString(), all_views, all_tables, new List<string> { row["Name"].AsString() }))
                    ));
            }

            File.WriteAllText(MyOqlTableOfView, string.Format(@"<?xml version=""1.0""?>
<!-- （自动生成）分析在 视图 中可能会变更的 表对象，By: " + SystemInformation.ComputerName + "." + WindowsGroupUser.GetSingleUserInfo(SystemInformation.UserName, null).FullName + "(" + Environment.UserName + ")" + "   At:" + DateTime.Now.ToString() + @" -->
<MyOqlCodeGen>
    <View>
        <Group Name=""View"">
            {0}
        </Group>
    </View>
</MyOqlCodeGen>", string.Join(Environment.NewLine, list.ToArray())
                 )
                 );
            return "分析数据库视图依赖表完成   !";
        }

        private static List<string> Get_View_Tables(string viewName, DataTable all_views, DataTable all_tables, List<string> procedView)
        {
            var sql = all_views.AsEnumerable().First(o => string.Equals(o["Name"].AsString(), viewName, StringComparison.CurrentCultureIgnoreCase))["Definition"].AsString();
            var reg = new Regex(@"from\s*\[?(\w*)\]?\.?\[?\b(\w+)\b\]?|join\s*\[?(\w*)\]?\.?\[?\b(\w+)\b\]?", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(sql);

            var tables = new List<string>();
            do
            {
                if (reg.Success == false) break;
                var tab = reg.Groups[2].Value.AsString(null) ?? reg.Groups[1].Value.AsString(null)
                    ?? reg.Groups[4].Value.AsString(null) ?? reg.Groups[3].Value.AsString(null);

                tables.Add(tab);
            } while ((reg = reg.NextMatch()) != null);


            //如果 tables中存在视图。

            var views = all_views.AsEnumerable().Where(r => r["Name"].IsIn((a, b) => string.Equals(a.AsString(), b.AsString(), StringComparison.CurrentCultureIgnoreCase), tables));// ,r=>r["Name"].AsString()).ToArray() ;
            //name in ('" + string.Join("','", tables.ToArray()) + "'").ToEntityList(() => "");

            views.All(o =>
            {
                if (procedView.Contains(s => string.Equals(s, o["Name"].AsString(), StringComparison.CurrentCultureIgnoreCase)))
                    return true;
                procedView.Add(o["Name"].AsString());
                tables.AddRange(Get_View_Tables(o["Name"].AsString(), all_views, all_tables, procedView));
                return true;
            });

            return all_tables.AsEnumerable().Where(r => r["Name"].AsString().IsIn(
                (a, b) => string.Equals(a.AsString(), b.AsString(), StringComparison.CurrentCultureIgnoreCase),
                tables.ToArray())).Select(o => o["Name"].AsString()).ToList()
                ;
        }

    }

}
