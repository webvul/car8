using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCmn;
using System.Configuration;
using System.IO;

namespace MyTool
{

    public class DeleteFolderHandler : ICommandHandler
    {
        CmdArgs arg { get; set; }

        public string DeleteFolder { get; set; }

        /// <summary>
        /// 和 Empty 互斥
        /// </summary>
        public string Filter { get; set; }
        public bool Empty { get; set; }

        public bool NoConfirm { get; set; }

        public DeleteFolderHandler(CmdArgs arg)
        {
            this.arg = arg;

            arg.ToModel(this);
        }

        public class Tree<T>
        {
            public List<T> SubTree { get; set; }

            public Tree()
            {
                this.SubTree = new List<T>();
            }
        }

        public class Folder : Tree<Folder>
        {
            public string Name { get; set; }
            public int Files { get; set; }
            public int Total { get; set; }
        }

        public string Do()
        {
            if (this.Filter.HasValue())
            {
                return GetFilterFolder();
            }
            else
            {
                return GetEmptyFolder();
            }
        }

        private string GetFilterFolder()
        {
            var di = new DirectoryInfo(DeleteFolder).GetDirectories(this.Filter, SearchOption.AllDirectories);

            di.All(o =>
            {
                Console.WriteLine(o.FullName);
                return true;
            });


            if (GetConfirm())
            {
                di.All(o =>
                    {
                        try
                        {
                            o.Delete(true);
                            //Console.WriteLine("删除文件夹:{0}!", o.FullName);
                        }
                        catch
                        {
                            Console.WriteLine("{0,-60}失败!", o.FullName);
                        }
                        return true;
                    });


                return "已删除目录!";
            }

            return "未删除目录,已退出!";
        }

        private bool GetConfirm()
        {
            if (this.NoConfirm) return true;
            Console.Write("请确认删除上述目录![回车或Y键继续]\t\t");
            var key = Console.ReadKey();
            Console.WriteLine();
            if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Y)
            { return true; }
            else return false;
        }

        private string GetEmptyFolder()
        {
            Func<DirectoryInfo, Folder> _ProcOne = null;
            Func<DirectoryInfo, Folder> ProcOne = di =>
            {
                Folder f = new Folder();
                f.Name = di.FullName;
                f.Files = di.GetFiles().Count();

                di.GetDirectories("*", SearchOption.TopDirectoryOnly).All(o =>
                {
                    f.SubTree.Add(_ProcOne(o));
                    return true;
                });
                return f;
            };
            _ProcOne = ProcOne;

            var con = ProcOne(new DirectoryInfo(DeleteFolder));

            new Recursion<Folder>().Execute(new Folder[] { con }, o => o.SubTree, o =>
            {
                if (o.Total == 0)
                {
                    o.Total = o.Files + o.SubTree.Select(s => s.Files).Sum();
                }
                return RecursionReturnEnum.Go;
            });

            new Recursion<Folder>().Execute(new Folder[] { con }, o => o.SubTree, o =>
            {
                if (this.Empty && o.Total == 0)
                {
                    Console.WriteLine(o.Name);
                    return RecursionReturnEnum.StopSub;
                }

                return RecursionReturnEnum.Go;
            });



            if (GetConfirm())
            {
                new Recursion<Folder>().Execute(new Folder[] { con }, o => o.SubTree, o =>
                {
                    if (this.Empty && o.Total == 0)
                    {
                        Directory.Delete(o.Name, true);
                        return RecursionReturnEnum.StopSub;
                    }

                    return RecursionReturnEnum.Go;
                });

                return "已删除上述目录!";
            }

            return "未删除上述目录,已退出!";
        }
    }

}
