using MyCmn;
using System.Xml;
using System;
using System.Data.Common;

namespace MyOql
{
    public class MyOqlError : GodError
    {
        public MyOqlError(string Msg ,string Detail)
            : base(Msg)
        {
            this.Type = "MyOqlGodError";
            this.Detail = Detail;
        }
         
    }
}
