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
using MyOql.MyOql_CodeGen;

namespace MyTool
{

    public class MyOqlCodeGenHandler : ICommandHandler
    {
        public string CodeGen { get; set; }
        public string Log { get; set; }
        public string Namespace { get; set; }
        public string CodeHeader { get; set; }

        public MyOqlCodeGenHandler(CmdArgs arg)
        {
            arg.ToModel(this);
        }

        public string Do()
        {
            if (this.Log.HasValue())
            {
                var fi = new FileInfo(this.Log);
                if (fi.Exists) { fi.Delete(); }

                Console.SetOut(new LogWriter(this.Log));
            }


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(string.Format(@"{0,-40} {1,-15} {2,-5}", "数据库对象名称", "类型", "耗时（毫秒）"));
            Console.WriteLine("---------------------------------------------------------------------------");

            Console.ForegroundColor = ConsoleColor.White;

            var codeGen = new MyOqlCodeGen();


            BindTimer(codeGen);

            foreach (var item in codeGen.Do())
            {
                string fileName = CodeGen + Path.DirectorySeparatorChar.ToString() + "Ent_" + item.Key + ".cs";

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }


                var content = File.ReadAllText(this.CodeHeader)
                    .Replace("$Computer$", SystemInformation.ComputerName)
                    .Replace("$Author$", WindowsGroupUser.GetSingleUserInfo(SystemInformation.UserName, null).FullName)
                    .Replace("$DateTime$", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))

                    + @"
namespace " + this.Namespace + @"
{
"
                    + item.Value
                    +
                    @"
}";

                File.WriteAllText(fileName, content, Encoding.UTF8);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣");
            Console.ResetColor();

            return "";
        }

        private void BindTimer(MyOqlCodeGen codeGen)
        {
            Stopwatch sw = new Stopwatch();
            var prevGroup = string.Empty;
            codeGen.Doing += o =>
            {
                sw.Restart();

                var group = GetGroup(o);

                if (group != prevGroup)
                {
                    if (Console.ForegroundColor == ConsoleColor.White)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                    else if (Console.ForegroundColor == ConsoleColor.Cyan)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    else if (Console.ForegroundColor == ConsoleColor.Yellow)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.Title = "MyOql实体生成器－" + group;
                }
                prevGroup = group;
                Console.Write(string.Format("{0,-47} {1,-17} ", o.Name, group));
            };
            codeGen.Doed += o =>
            {
                Console.Write(string.Format("{0,-5}", sw.ElapsedMilliseconds));
                Console.WriteLine();
            };
        }

        private string GetGroup(IConfigSect o)
        {
            {
                var c = o as MyOqlCodeGenSect.ViewCollection.ViewGroupCollection.ViewElement;
                if (c != null)
                {
                    return "View" + c.Container.Name.HasValue(n => "." + n);
                }
            }
            {
                var c = o as MyOqlCodeGenSect.ProcCollection.ProcGroupCollection.ProcElement;
                if (c != null)
                {
                    return "Proc" + c.Container.Name.HasValue(n => "." + n);
                }
            }

            if (o.IsType<MyOqlCodeGenSect.TableCollection.TableGroupCollection.TableElement>(true))
            {
                var c = o as MyOqlCodeGenSect.TableCollection.TableGroupCollection.TableElement;
                if (c != null)
                {
                    if (c.Paras.Count > 0)
                    {
                        return "Function" + c.Container.Name.HasValue(n => "." + n);
                    }
                    else return "Table" + c.Container.Name.HasValue(n => "." + n);
                }
            }
            return string.Empty;
        }
    }

}
