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

    public class CheckIModelHandler : ICommandHandler
    {
        public string[] dlls { get; set; }

        public CheckIModelHandler(CmdArgs arg)
        {
            arg.ToModel(this);
        }

        public string Do()
        {
            foreach (var item in dlls)
            {
                GodError.Check(File.Exists(item) == false, () =>
                {
                    Console.WriteLine("     找不到文件：" + item + "!!!");
                    return "找不到文件：" + item;
                }
               );

                var types = Assembly.LoadFrom(item).GetTypes();
                foreach (var type in types)
                {
                    if (type.GetInterface(typeof(IModel).FullName, true) != null)
                    {
                        try
                        {
                            var instance = Activator.CreateInstance(type) as IModel;

                            if (instance == null) continue;

                            LogPropErr(type, instance.GetProperties());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }
                }
            }

            return "         ● 分析 IModel 完成   !";
        }

        private void LogPropErr(Type type, string[] p)
        {
            var props = type.GetProperties().Select(o => o.Name).ToArray();
            var m1 = props.Minus(p).ToArray();
            var m2 = p.Minus(props).ToArray();
            if (m1.Length > 0 && m2.Length == 0)
            {
                MyConsole.Write(" " + type.FullName, ConsoleColor.Green);
                Console.Write(" 少定义了:");

                var index = 0;
                m1.All(o =>
                    {
                        if (index++ > 0) { Console.Write(","); }
                        var prop = type.GetProperty(o);
                        if (prop.PropertyType.IsSimpleType() || (prop.PropertyType.GetInterface(typeof(IEnm).FullName, false)) != null)
                        {
                            MyConsole.Write(o, ConsoleColor.Red);
                        }
                        else MyConsole.Write(o, ConsoleColor.Yellow);
                        return true;
                    });
                Console.WriteLine();
            }
            else if (m1.Length == 0 && m2.Length > 0)
            {
                MyConsole.Write(" " + type.FullName, ConsoleColor.Green);
                Console.Write(" 多定义了:");

                var index = 0;
                m2.All(o =>
                    {
                        if (index++ > 0) { Console.Write(","); }

                        MyConsole.Write(o, ConsoleColor.Red);

                        return true;
                    });
                Console.WriteLine();
            }
            else if (m1.Length > 0 && m2.Length > 0)
            {
                MyConsole.Write(" " + type.FullName, ConsoleColor.Green);
                Console.Write(" 少定义了:");

                var index = 0;
                m1.All(o =>
                {
                    if (index++ > 0) { Console.Write(","); }
                    var prop = type.GetProperty(o);
                    if (prop.PropertyType.IsSimpleType() || (prop.PropertyType.GetInterface(typeof(IEnm).FullName, false)) != null)
                    {
                        MyConsole.Write(o, ConsoleColor.Red);
                    }
                    else MyConsole.Write(o, ConsoleColor.Yellow);
                    return true;
                });

                Console.Write(",多定义了:");
                index = 0;
                m2.All(o =>
                {
                    if (index++ > 0) { Console.Write(","); }

                    if (m1.Length == 0)
                    {
                        MyConsole.Write(o, ConsoleColor.Red);
                    }
                    else
                    {
                        MyConsole.Write(o, ConsoleColor.Yellow);
                    }

                    return true;
                });

                Console.WriteLine();
            }
        }
    }

}
