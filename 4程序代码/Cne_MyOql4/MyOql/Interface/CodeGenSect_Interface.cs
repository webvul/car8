using System;

namespace MyOql
{
    public interface IConfigGroupSect : ICloneable
    {
        string Name { get; }
    }

    public interface IConfigSect:ICloneable
    {
        string Name { get;}
        string MapName { get;  }
    }
}
