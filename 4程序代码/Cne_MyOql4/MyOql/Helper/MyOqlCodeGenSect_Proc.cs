using System;
using System.Collections.Generic;
using System.Configuration;
using MyCmn;
using MyOql.MyOql_CodeGen;
using System.Runtime.Serialization;
using System.Linq;

namespace MyOql
{
    /*
     
  <configSections>
    <section name="MyOqlCodeGen" type="MyOql.MyOqlCodeGenSect,MyOql"/>
  </configSections>

   <MyOqlCodeGen IgnoreTables="asp_net">
    <Tables>
      <Entity Name="Dict" ComputeKeys="Compute1,Compute2" AutoIncreKey="ID" PKs="ID,Name"></Entity>
    </Tables>
    <Views>
      <Entity Name="V_Dict" Table="Dict"></Entity>
    </Views>
    <Procs>
      <Entity Name="P_1" Return="string"></Entity>
    </Procs>
  </MyOqlCodeGen>
     */
    public partial class MyOqlCodeGenSect : ConfigurationSection
    {
        public class ProcCollection : ConfigurationElementCollection
        {
            public virtual ProcGroupCollection.ProcElement GetConfig(string Proc)
            {
                foreach (ProcCollection.ProcGroupCollection group in this)
                {
                    foreach (ProcGroupCollection.ProcElement proc in group)
                    {
                        if (proc.Name == Proc) return proc;
                    }
                }
                return null;
            }

            public virtual IEnumerable<ProcGroupCollection.ProcElement> GetConfig(Func<ProcGroupCollection.ProcElement, bool> func)
            {
                foreach (ProcGroupCollection group in this)
                {
                    foreach (ProcGroupCollection.ProcElement tab in group)
                    {
                        if (func == null || func(tab)) yield return tab;
                    }
                }
            }

            /// <summary>
            /// 数据库对象的所有者.
            /// </summary>
            [ConfigurationProperty("Owner", IsRequired = false)]
            public string Owner
            {
                get { return (string)(this["Owner"]); }
            }

            [ConfigurationProperty("db", IsRequired = false)]
            public string db
            {
                get { return (string)(this["db"]); }
            }

            protected override ConfigurationElement CreateNewElement()
            {
                return new ProcGroupCollection(this);
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                ProcGroupCollection siteUser = element as ProcGroupCollection;
                return siteUser.Name;
            }

            public override ConfigurationElementCollectionType CollectionType
            {
                get
                {
                    return ConfigurationElementCollectionType.BasicMap;
                }
            }
            protected override string ElementName
            {
                get
                {
                    return "Group";
                }
            }


            public new void BaseAdd(ConfigurationElement element)
            {
                base.BaseAdd(element);
            }


            /// <summary>
            /// 本程序集之外,需要操作Config 文件 之用. 
            /// </summary>
            /// <param name="Key"></param>
            public void BaseRemove(string Key)
            {
                base.BaseRemove(Key);
            }


            public class ProcGroupCollection : ConfigurationElementCollection, IConfigGroupSect, ISerializable
            {
                public ProcCollection Container;

                public ProcGroupCollection(ProcCollection myOqlCodeGenTablesCollection)
                {
                    // TODO: Complete member initialization
                    this.Container = myOqlCodeGenTablesCollection;
                }

                /// <summary>
                /// 数据库对象的所有者.
                /// </summary>
                [ConfigurationProperty("Owner", IsRequired = false)]
                public string Owner
                {
                    get
                    {
                        return this["Owner"].AsString(this.Container.Owner);
                    }
                }


                [ConfigurationProperty("db", IsRequired = false)]
                public string db
                {
                    get
                    {
                        return this["db"].AsString(this.Container.db);
                    }
                }

                [ConfigurationProperty("Name", IsRequired = false)]
                public string Name
                {
                    get { return (string)(this["Name"]); }
                }


                [ConfigurationProperty("Descr", IsRequired = false)]
                public string Descr
                {
                    get { return (string)(this["Descr"]); }
                }

                protected override ConfigurationElement CreateNewElement()
                {
                    return new ProcElement(this);
                }

                protected override object GetElementKey(ConfigurationElement element)
                {
                    ProcElement siteUser = element as ProcElement;
                    return siteUser.Name;
                }

                public override ConfigurationElementCollectionType CollectionType
                {
                    get
                    {
                        return ConfigurationElementCollectionType.BasicMap;
                    }
                }
                protected override string ElementName
                {
                    get
                    {
                        return "Entity";
                    }
                }


                public new void BaseAdd(ConfigurationElement element)
                {
                    base.BaseAdd(element);
                }


                /// <summary>
                /// 本程序集之外,需要操作Config 文件 之用. 
                /// </summary>
                /// <param name="Key"></param>
                public void BaseRemove(string Key)
                {
                    base.BaseRemove(Key);
                }

                public class ProcElement : ConfigurationElement, ISerializable, IConfigSect
                {
                    public ProcGroupCollection Container;

                    public ProcElement(ProcGroupCollection procGroupCollection)
                    {
                        // TODO: Complete member initialization
                        this.Container = procGroupCollection;
                    }


                    /// <summary>
                    /// 数据库对象的所有者.
                    /// </summary>
                    [ConfigurationProperty("Owner", IsRequired = false)]
                    public string Owner
                    {
                        get
                        {
                            return this["Owner"].AsString(this.Container.Owner);
                        }
                        set { this["Owner"] = value; }
                    }

                    /// <summary>
                    /// 该项配置不是针对存储过程的数据配置，因为生成存储过程本身不需要访问数据库，它是 ReturnModel 定义的表或视图所在的数据配置。
                    /// </summary>
                    [ConfigurationProperty("db", IsRequired = false)]
                    public string db
                    {
                        get
                        {
                            return this["db"].AsString(this.Container.db);
                        }
                        set { this["db"] = value; }
                    }

                    [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
                    public string Name
                    {
                        get { return this["Name"] as string; }
                        set { this["Name"] = value; }
                    }


                    /// <summary>
                    /// 映射到程序里的名字。
                    /// </summary>
                    [ConfigurationProperty("MapName", IsRequired = false)]
                    public string MapName
                    {
                        get
                        {
                            return this["MapName"].AsString(this.Name);
                        }
                        set { this["MapName"] = value; }
                    }

                    [ConfigurationProperty("Descr", IsRequired = false)]
                    public string Descr
                    {
                        get { return this["Descr"] as string; }
                        set { this["Descr"] = value; }
                    }

                    //[ConfigurationProperty("Paras", IsRequired = false)]
                    //public MyParaDefine Paras
                    //{
                    //    get { return (MyParaDefine)(this["Paras"].AsString().Replace(" ", "")); }
                    //    set { this["Paras"] = value; }
                    //}

                    [ConfigurationProperty("MyParas", IsRequired = false)]
                    public MyEnumDefine MyParas
                    {
                        get { return (MyEnumDefine)(this["MyParas"].AsString().Replace(" ", "")); }
                        set { this["MyParas"] = value; }
                    }

                    /// <summary>
                    /// 可提供两个属性即可：参数名称=数据类型
                    /// </summary>
                    [ConfigurationProperty("Return", IsRequired = false)]
                    public MyParaReturnDefine Return
                    {
                        get { return new MyParaReturnDefine(this["Return"].AsString().Replace(" ", "")); }
                        set { this["Return"] = value; }
                    }


                    /// <summary>
                    /// 定义为数据库里的表或视图。以表或视图的名称和列定义定义到Proc类里。作为存储过程返回结果定义
                    /// </summary>
                    /// <remarks>
                    /// DataTable , DataModel , DataModel Array , DbType ,void 
                    /// </remarks>
                    [ConfigurationProperty("ReturnDefine", IsRequired = false)]
                    public string ReturnDefine
                    {
                        get { return (string)(this["ReturnDefine"]); }
                        set { this["ReturnDefine"] = value; }
                    }

                    /// <summary>
                    /// 关联的更新插入实体。
                    /// </summary>
                    [ConfigurationProperty("MyTable", IsRequired = false)]
                    public string MyTable
                    {
                        get { return (string)(this["MyTable"]); }
                        set { this["MyTable"] = value; }
                    }

                    [ConfigurationProperty("AutoTable", IsRequired = false)]
                    public string AutoTable
                    {
                        get { return (string)(this["AutoTable"]); }
                        set { this["AutoTable"] = value; }
                    }


                    public string Table
                    {
                        get
                        {
                            return string.Join(",", this.MyTable.AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                .Union(this.AutoTable.AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                                .ToArray());
                        }
                    }

                    public void GetObjectData(SerializationInfo info, StreamingContext context)
                    {
                        info.AddValue("Owner", this["Owner"]);
                        info.AddValue("db", this["db"]);
                        info.AddValue("Name", this["Name"]);
                        info.AddValue("Descr", this["Descr"]);
                        info.AddValue("MapName", this["MapName"]);
                        info.AddValue("Paras", this["Paras"]);
                        info.AddValue("Return", this["Return"]);
                        info.AddValue("ReturnDefine", this["ReturnDefine"]);
                        info.AddValue("MyTable", this["MyTable"]);
                        info.AddValue("AutoTable", this["AutoTable"]);
                    }

                    public object Clone()
                    {
                        var ent = new ProcElement(this.Container);
                        ent.db = this.db;
                        ent.Descr = this.Descr;
                        ent.MapName = this.MapName;
                        ent.Name = this.Name;
                        ent.Owner = this.Owner;
                        //ent.Paras = (MyParaDefine)this.Paras.Clone();
                        ent.Return = (MyParaReturnDefine)this.Return.Clone();
                        ent.ReturnDefine = this.ReturnDefine;
                        ent.MyTable = this.MyTable;
                        ent.AutoTable = this.AutoTable;
                        ent.MyParas = this.MyParas;
                        return ent;
                    }
                }

                public void GetObjectData(SerializationInfo info, StreamingContext context)
                {
                    info.AddValue("Owner", this["Owner"]);
                    info.AddValue("db", this["db"]);
                    info.AddValue("Name", this["Name"]);
                    info.AddValue("Descr", this["Descr"]);
                }

                public object Clone()
                {
                    var group = new ProcGroupCollection(this.Container);
                    group.AddElementName = this.AddElementName;
                    group.ClearElementName = this.ClearElementName;
                    group["db"] = this.db;
                    group["Descr"] = this.Descr;
                    group["EmitClear"] = this.EmitClear;
                    group["Name"] = this.Name;

                    return group;
                }
            }
        }
    }
}
