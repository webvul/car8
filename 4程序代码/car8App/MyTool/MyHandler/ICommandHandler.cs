using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCmn;
using System.Configuration;
using System.IO;

namespace MyTool
{
    public interface ICommandHandler
    {
        string Do();
    }
}
