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

    public class RenamePentaxFolderAsYearHandler : ICommandHandler
    {
        public string RenamePentaxFolderAsYear { get; set; }

        public RenamePentaxFolderAsYearHandler(CmdArgs arg)
        {
            arg.ToModel(this);
        }

        public string Do()
        {
            Console.WriteLine("---------------------------------------------------------------------------");

            DoItem(RenamePentaxFolderAsYear);
            Console.WriteLine("￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣");

            return "         ● 重命名完成   !";
        }

        private void DoItem(string pentaxPath)
        {
            var each = pentaxPath.Split('\\').Last().Split('_');
            if (each.Length == 2)
            {
                if (each[0].AsInt() > 0 && each[1].AsInt() > 0)
                {
                    var di = System.IO.Directory.GetFiles(pentaxPath, "*.jpg");
                    if (di.Any() == false) return;
                    var fi = di[0];

                    var info = new Goheer.EXIF.EXIFextractor(fi, "", "");

                    var dt = info["DTDigitized"].AsString().Split(' ')[0].Split(':')[0].AsInt();

                    if (dt.HasValue())
                    {
                        var pi = new DirectoryInfo(pentaxPath);
                        var newPath = pi.Parent.FullName + System.IO.Path.DirectorySeparatorChar + dt + "_" + each[1];
                        if (pentaxPath != newPath)
                        {
                            try
                            {
                                System.IO.Directory.Move(pentaxPath, newPath);
                                MyConsole.WriteLine(pentaxPath + " =>  " + newPath, ConsoleColor.Green);
                                pentaxPath = newPath;
                            }
                            catch (Exception e)
                            {
                                MyConsole.WriteLine(pentaxPath + "  " + e.Message, ConsoleColor.Red);
                            }
                        }
                    }
                }
            }

            foreach (var item in System.IO.Directory.GetDirectories(pentaxPath))
            {
                DoItem(item);
            }
        }

    }

}
