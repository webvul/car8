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
using System.Web.Compilation;
using MyTool.MyHandler;

namespace MyTool
{

    public class NewProxyClassHandler : ICommandHandler
    {
        string useNameSpaces = @" 
using System;
using MyOql;
using MyCmn;
using System.Linq;
using System.Data ;
using System.Data.Common ;
using System.Collections.Generic;
using System.Runtime.Serialization ;
using PmEnt;
using PmEnt.Model;
using PmEnt.MasterGroup;
using PmEnt.PropertyGroup;
using PmEnt.CostGroup;
using PmEnt.SysGroup;
";

        public string Type { get; set; }
        public string propName { get; set; }
        public string Class { get; set; }
        public string File { get; set; }

        public NewProxyClassHandler(CmdArgs arg)
        {
            arg.ToModel(this);
        }

        public string Do()
        {
            Console.WriteLine("---------------------------------------------------------------------------");


            new NewProxyClassFrm().ShowDialog();



            Console.WriteLine("￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣￣");


            return "";
        }

    }

}
