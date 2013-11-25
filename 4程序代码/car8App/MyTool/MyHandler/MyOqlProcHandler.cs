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
using System.Xml;

namespace MyTool
{
    public class MyOqlProcHandler : ICommandHandler
    {
        public string MyOqlProc { get; set; }
        public string ExtFile { get; set; }

        public MyOqlProcHandler(CmdArgs arg)
        {
            arg.ToModel(this);
        }

        public string Do()
        {
            if (ExtFile.HasValue() == false) return MyOqlProc;
            if (MyOqlProc.HasValue() == false) return ExtFile;

            var xmlExt = new XmlDocument();
            xmlExt.Load(ExtFile);

            var procs = xmlExt.SelectNodes(@"/ns:MyOqlCodeGen/ns:Proc/ns:Group/ns:Entity", xmlExt.GetNameSpaceManager("ns"));

            foreach (XmlNode item in procs)
            {
                if (item.Attr("Proc").HasValue())
                {
                    item.Attr("MyTable", string.Join(",", item.Attr("MyTable").AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Union(
                        GetProcTables(procs.ToMyList(o => o as XmlNode), item.Attr("Proc").AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries), new List<string>() { item.Attr("Name") })
                        )));
                }

                item.Attributes.RemoveNamedItem("Proc");
            }


            xmlExt.Save(MyOqlProc);

            return "● 处理存储过程依赖表完成   !";
        }

        private IEnumerable<string> GetProcTables(IEnumerable<XmlNode> allProcEntity, string[] procs, List<string> procedProc)
        {
            var list = new List<string>();
            procs.All(o =>
                {
                    if (procedProc.Contains(o)) return true;
                    procedProc.Add(o);
                    list.AddRange(GetProcTableOne(allProcEntity, o, procedProc));
                    return true;
                });

            return list;
        }

        private IEnumerable<string> GetProcTableOne(IEnumerable<XmlNode> allProcEntity, string proc, List<string> procedProc)
        {
            var list = new List<string>();
            if (procedProc.Contains(proc)) return list;

            var ent = allProcEntity.First(o => o.Attr("Name") == proc);

            list.AddRange(ent.Attr("AutoTable").AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            list.AddRange(ent.Attr("MyTable").AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

            if (ent.Attr("Proc").AsString().HasValue())
            {
                procedProc.Add(proc);
                list.AddRange(GetProcTables(allProcEntity, ent.Attr("Proc").AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries), procedProc));
            }

            return list;
        }
    }
}
