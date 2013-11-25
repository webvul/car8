using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using MyCmn;
using MyOql.MyOql_CodeGen;
using System.Runtime.Serialization;

namespace MyOql
{
    /*
     
  <configSections>
    <section name="MyOqlCodeGen" type="MyOql.MyOqlCodeGenSect,MyOql"/>
  </configSections>

   <MyOqlCodeGen IgnoreTables="asp_net">
    <Table>
      <Entity Name="Dict" ComputeKeys="Compute1,Compute2" AutoIncreKey="ID" PKs="ID,Name"></Entity>
    </Table>
    <View>
      <Entity Name="V_Dict" Table="Dict"></Entity>
    </View>
    <Proc>
      <Entity Name="P_1" Return="string"></Entity>
    </Proc>
  </MyOqlCodeGen>
     */
    public partial class MyOqlCodeGenSect : ConfigurationSection
    {
        public class TableCollection : ConfigurationElementCollection
        {
            public TableGroupCollection.TableElement GetConfig(string Name)
            {
                foreach (TableGroupCollection group in this)
                {
                    foreach (TableGroupCollection.TableElement tab in group)
                    {
                        if (tab.Name == Name) return tab;
                    }
                }
                return null;
            }

            public IEnumerable<TableGroupCollection.TableElement> GetConfig(Func<TableGroupCollection.TableElement, bool> func)
            {
                foreach (TableGroupCollection group in this)
                {
                    foreach (TableGroupCollection.TableElement tab in group)
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

            /// <summary>
            /// 对于值类型来说，是否生成 int? 类型，默认是 false 
            /// </summary>
            [ConfigurationProperty("Nullable", IsRequired = false)]
            public bool Nullable
            {
                get { return this["Nullable"].AsBool(); }
            }



            protected override ConfigurationElement CreateNewElement()
            {
                return new TableGroupCollection(this);
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                var siteUser = element as TableGroupCollection;
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

            [Serializable]
            public class TableGroupCollection : ConfigurationElementCollection, ISerializable, IConfigGroupSect, ICloneable
            {
                public TableCollection Container { get; set; }

                public TableGroupCollection(TableCollection myOqlCodeGenTablesCollection)
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

                /// <summary>
                /// 对于值类型来说，是否生成 int? 类型，默认是 false 
                /// </summary>
                [ConfigurationProperty("Nullable", IsRequired = false)]
                public bool Nullable
                {
                    get { return this["Nullable"].AsString().HasValue() ? (bool)(this["Nullable"]) : this.Container.Nullable; }
                }

                protected override ConfigurationElement CreateNewElement()
                {
                    return new TableElement(this);
                }

                protected override object GetElementKey(ConfigurationElement element)
                {
                    TableElement siteUser = element as TableElement;
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

                public void GetObjectData(SerializationInfo info, StreamingContext context)
                {
                    info.AddValue("db", this["db"]);
                    info.AddValue("Descr", this["Descr"]);
                    info.AddValue("Name", this["Name"]);
                    info.AddValue("Owner", this["Owner"]);
                    info.AddValue("Nullable", this["Nullable"]);
                }

                public abstract class BaseTableElement : ConfigurationElement, ISerializable, IConfigSect, ICloneable
                {
                    public List<MyFkNode.MyFkMapNode> GetChildrenDefine(TableCollection tables)
                    {
                        List<MyFkNode.MyFkMapNode> list = new List<MyFkNode.MyFkMapNode>();

                        foreach (MyOqlCodeGenSect.TableCollection.TableGroupCollection group in tables)
                        {
                            foreach (TableCollection.TableGroupCollection.BaseTableElement tab in group)
                            {
                                foreach (var fkitem in tab.FKs)
                                {
                                    if (fkitem.RefTable == this.Name)
                                    {
                                        if (list.Count(o => o.MapEntity == tab.MapName && o.Column == fkitem.Column && o.RefTable == fkitem.RefTable && o.RefColumn == fkitem.RefColumn) == 0)
                                        {
                                            list.Add(fkitem.TranslateMapName(tab.Name, tables));
                                        }
                                    }
                                }
                            }
                        }

                        return list;
                    }

                    public TableGroupCollection Container;

                    public BaseTableElement(TableGroupCollection myOqlCodeGenTableGroupCollection)
                    {
                        // TODO: Complete member initialization
                        this.Container = myOqlCodeGenTableGroupCollection;
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


                    [ConfigurationProperty("db", IsRequired = false)]
                    public string db
                    {
                        get
                        {
                            return this["db"].AsString(this.Container.db);
                        }
                        set { this["db"] = value; }
                    }

                    /// <summary>
                    /// 数据库里的表名。
                    /// </summary>
                    [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
                    public string Name
                    {
                        get { return this["Name"] as string; }
                        set { this["Name"] = value; }
                    }

                    /// <summary>
                    /// 手动映射成程序的友好名字
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

                    [ConfigurationProperty("ComputeKeys", IsRequired = false)]
                    public string ComputeKeys
                    {
                        get { return (string)(this["ComputeKeys"]); }
                        set { this["ComputeKeys"] = value; }
                    }

                    [ConfigurationProperty("AutoIncreKey", IsRequired = false)]
                    public string AutoIncreKey
                    {
                        get { return (string)(this["AutoIncreKey"]); }
                        set { this["AutoIncreKey"] = value; }
                    }

                    /// <summary>
                    /// 新的映射方法是子对象 pk，两种形式都兼容。
                    /// </summary>
                    [ConfigurationProperty("PKs", IsRequired = false)]
                    public string PKs
                    {
                        get { return (string)(this["PKs"]); }
                        set { this["PKs"] = value; }
                    }

                    [ConfigurationProperty("FKs", IsRequired = false)]
                    public MyFkDefine FKs
                    {
                        get { return (MyFkDefine)this["FKs"] ?? new MyFkDefine(); }
                        set { this["FKs"] = value; }
                    }

                    [ConfigurationProperty("Enums", IsRequired = false)]
                    public MyEnumDefine Enums
                    {
                        get
                        {
                            return (MyEnumDefine)(this["Enums"]) ?? new MyEnumDefine("");
                        }
                        set { this["Enums"] = value; }
                    }

                    /// <summary>
                    /// 仅支持单一唯一键，如果多个键是组合唯一键，会忽略其定义。
                    /// </summary>
                    [ConfigurationProperty("UniqueKey", IsRequired = false)]
                    public string UniqueKey
                    {
                        get
                        {
                            if (this["UniqueKey"].AsString().Contains(',')) return string.Empty;
                            return (string)(this["UniqueKey"]);
                        }
                        set { this["UniqueKey"] = value; }
                    }

                    /// <summary>
                    /// 对于值类型来说，是否生成 int? 类型，默认是 false 
                    /// </summary>
                    [ConfigurationProperty("Nullable", IsRequired = false)]
                    public bool Nullable
                    {
                        get { return this["Nullable"].AsString().HasValue() ? (bool)(this["Nullable"]) : this.Container.Nullable; }
                        set { this["Nullable"] = value; }
                    }

                    ///// <summary>
                    ///// 
                    ///// </summary>
                    //[ConfigurationProperty("RowTimestamp", IsRequired = false)]
                    //public string RowTimestamp
                    //{
                    //    get { return this["RowTimestamp"].AsString(); }
                    //    set { this["RowTimestamp"] = value; }
                    //}

                    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
                    {
                        info.AddValue("AutoIncreKey", this["AutoIncreKey"]);
                        info.AddValue("ComputeKeys", this["ComputeKeys"]);
                        info.AddValue("db", this["db"]);
                        info.AddValue("Descr", this["Descr"]);
                        info.AddValue("Enums", this["Enums"]);
                        info.AddValue("FKs", this["FKs"]);
                        info.AddValue("MapName", this["MapName"]);
                        info.AddValue("Name", this["Name"]);
                        info.AddValue("Owner", this["Owner"]);
                        info.AddValue("PKs", this["PKs"]);
                        info.AddValue("UniqueKey", this["UniqueKey"]);
                        info.AddValue("Paras", this["Paras"]);
                        info.AddValue("RowTimestamp", this["RowTimestamp"]);
                        info.AddValue("Nullable", this["Nullable"]);
                    }

                    public abstract object Clone();
                }

                public class TableElement : BaseTableElement
                {
                    public TableElement(TableGroupCollection myOqlCodeGenTableGroupCollection)
                        : base(myOqlCodeGenTableGroupCollection)
                    {
                    }

                    /// <summary>
                    /// 仅在 表值函数 时有效。
                    /// </summary>
                    [ConfigurationProperty("Paras", IsRequired = false)]
                    public MyParaDefine Paras
                    {
                        get { return (MyParaDefine)(this["Paras"]) ?? new MyParaDefine(""); }
                        set { this["Paras"] = value; }
                    }

                    /// <summary>
                    /// 表示变表的模板定义 如： TF_Fees_{Comm}
                    /// </summary>
                    [ConfigurationProperty("VarName", IsRequired = false)]
                    public string VarName
                    {
                        get { return (string)(this["VarName"]) ?? ""; }
                        set { this["VarName"] = value; }
                    }

                    public override void GetObjectData(SerializationInfo info, StreamingContext context)
                    {
                        base.GetObjectData(info, context);
                        info.AddValue("Paras", this["Paras"]);
                    }

                    public override object Clone()
                    {
                        var ent = new TableElement(this.Container);
                        ent.AutoIncreKey = this.AutoIncreKey;
                        ent.ComputeKeys = this.ComputeKeys;
                        ent.db = this.db;
                        ent.Descr = this.Descr;
                        ent.Enums = (MyEnumDefine)this.Enums.Clone();
                        ent.FKs = (MyFkDefine)this.FKs.Clone();
                        ent.MapName = this.MapName;
                        ent.Name = this.Name;
                        ent.Owner = this.Owner;
                        ent.PKs = this.PKs;
                        ent.UniqueKey = this.UniqueKey;
                        ent.Nullable = this.Nullable;
                        ent.VarName = this.VarName;
                        //ent.RowTimestamp = this.RowTimestamp;

                        ent.Paras = (MyParaDefine)this.Paras.Clone();
                        return ent;
                    }
                }

                public object Clone()
                {
                    var group = new TableGroupCollection(this.Container);
                    group.AddElementName = this.AddElementName;
                    group.ClearElementName = this.ClearElementName;
                    group.EmitClear = this.EmitClear;
                    group["db"] = this.db;
                    group["Descr"] = this.Descr;
                    group["Name"] = this.Name;
                    group["Owner"] = this.Owner;
                    group["Nullable"] = this.Nullable;
                    return group;
                }
            }
        }

    }
}
