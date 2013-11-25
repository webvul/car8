using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MyCmn;
using MyOql;
using System.Diagnostics;
using System.Configuration;
using System.Web.Compilation;
using System.ComponentModel;
using System.Windows.Forms;


public class LogWriter : StreamWriter
{
    public LogWriter(string fileName) : base(fileName) { }

    public override Encoding Encoding
    {
        get { return Encoding.Default; }
    }

    public override void Write(string value)
    {
        var bytes = Encoding.GetBytes(value);
        Console.OpenStandardOutput().Write(bytes, 0, bytes.Length);
        base.Write(value);
        Flush();
    }

    public override void Write(object value)
    {
        var bytes = Encoding.GetBytes(value.AsString());
        Console.OpenStandardOutput().Write(bytes, 0, bytes.Length);
        base.Write(value);
        Flush();
    }

    public override void WriteLine(string value)
    {
        var bytes = Encoding.GetBytes(value + Environment.NewLine);
        Console.OpenStandardOutput().Write(bytes, 0, bytes.Length);
        base.WriteLine(value);
        Flush();
    }

    public override void WriteLine(object value)
    {
        var bytes = Encoding.GetBytes(value.AsString() + Environment.NewLine);
        Console.OpenStandardOutput().Write(bytes, 0, bytes.Length);
        base.WriteLine(value);
        Flush();
    }
    public override void WriteLine()
    {
        var bytes = Encoding.GetBytes(Environment.NewLine);
        Console.OpenStandardOutput().Write(bytes, 0, bytes.Length);
        base.WriteLine();
        Flush();
    }
}

public static class MyConsole
{
    public static void Write(string msg, ConsoleColor color)
    {
        var oriColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(msg);
        Console.ForegroundColor = oriColor;
    }
    public static void WriteLine(string msg, ConsoleColor color)
    {
        var oriColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(msg);
        Console.ForegroundColor = oriColor;
    }
}

public class BeginRepeatConsole
{
    private int Length;
    public void Write(string msg, ConsoleColor color)
    {
        Console.CursorVisible = false;
        ReLocation();
        Console.Write(new string(' ', this.Length));
        ReLocation();
        this.Length = System.Text.Encoding.Default.GetByteCount(msg) + msg.Count(o => o == '\t') * 3;
        var oriColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(msg);
        Console.ForegroundColor = oriColor;
        Console.CursorVisible = true;
    }

    private void ReLocation()
    {
        if (this.Length == 0) return;
        if (this.Length <= Console.CursorLeft)
        {
            Console.SetCursorPosition(Console.CursorLeft - this.Length, Console.CursorTop);
            return;
        }
        var length = this.Length - Console.CursorLeft;

        var offRow = length / (Console.BufferWidth + 1);
        var offCell = length % Console.BufferWidth;
        Console.SetCursorPosition(Console.BufferWidth - offCell, Console.CursorTop - offRow - 1);
    }
}

namespace MyTool
{
    class Program
    {
        public static MyCommandConfig _MyCmd;
        public static MyCommandConfig MyCmd
        {
            get
            {
                if (_MyCmd == null)
                {
                    _MyCmd = (MyCommandConfig)ConfigurationManager.GetSection("MyCmd");
                }
                return _MyCmd;
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            CmdArgs aa = new CmdArgs('-', args);
            var cmdName = args[0].Slice(1).AsString();
            foreach (MyCommandConfig.CommandCollection.CommandElement cmd in MyCmd.Commands)
            {
                if (cmd.Name == aa.Data.Keys.First())
                {
                    Run(aa, cmd);
                    return;
                }
            }

            Run(aa, new MyCommandConfig.CommandCollection.CommandElement() { Name = cmdName, Type = "MyTool." + args[0].Slice(1).AsString() + "Handler,MyTool" });

        }

        private static void Run(CmdArgs aa, MyCommandConfig.CommandCollection.CommandElement cmd)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var type = BuildManager.GetType(cmd.Type, true);
            var msg = string.Empty;

            if (type.IsSubclassOf(typeof(System.Windows.Forms.Form)))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Application.Run(Activator.CreateInstance(type) as Form);
                return;
            }
            Console.Title = cmd.Name;
            Console.WriteLine("『" + cmd.Name);

            (Activator.CreateInstance(type, new object[] { aa }) as ICommandHandler).Do();


            Console.ForegroundColor = ConsoleColor.Green;
            if (msg.HasValue())
            {
                Console.Write("\t");
                Console.WriteLine(msg);
            }
            sw.Stop();

            Console.Write("\t");
            Console.Write(string.Format(@"总共用时:   {0}", TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).GetSummary()));
            Console.ResetColor();

            Console.WriteLine("』");

        }
    }
}
