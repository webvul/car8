using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCmn;
using System.Configuration;
using System.IO;

namespace MyTool
{

    public class NewFileHandler : ICommandHandler
    {
        CmdArgs arg { get; set; }

        public string NewFile { get; set; }
        public bool Clear { get; set; }
        public string[] ListFiles { get; set; }
        public int CodePage { get; set; }

        public NewFileHandler(CmdArgs arg)
        {
            this.arg = arg;

            arg.ToModel(this);

            if (this.CodePage == 0)
            {
                this.CodePage = 65001;
            }
        }

        public string Do()
        {
            List<string> content = new List<string>();
            Encoding encode = Encoding.GetEncoding(CodePage);
            foreach (var item in ListFiles)
            {
                GodError.Check(File.Exists(item) == false, () =>
                {
                    Console.WriteLine("     找不到文件：" + item + "!!!");
                    return "找不到文件：" + item;
                }
               );
                content.Add(File.ReadAllText(item, encode));
            }

            if (Clear && File.Exists(NewFile))
            {
                File.Delete(NewFile);
            }

            using (StreamWriter sw = new StreamWriter(NewFile, false, encode))
            {
                sw.Write(string.Join(Environment.NewLine, content.ToArray()));
                sw.Close();
            }

            return "         ● 合并 《" + NewFile + "》 完成   !";
        }
    }

}
