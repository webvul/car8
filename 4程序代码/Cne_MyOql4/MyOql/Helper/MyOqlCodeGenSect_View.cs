using System.Configuration;
using System;
using System.Runtime.Serialization;
using MyOql.MyOql_CodeGen;
using MyCmn;
using System.Linq;

namespace MyOql
{
    public partial class MyOqlCodeGenSect : ConfigurationSection
    {
        /// <summary>
        /// 函数定义是在表定久的基础上添加了参数列表。要求在定义出完整的列定义Enums
        /// </summary>
        public class ViewCollection : TableCollection
        {
            protected override ConfigurationElement CreateNewElement()
            {
                return new ViewGroupCollection(this);
            }

            [Serializable]
            public class ViewGroupCollection : TableCollection.TableGroupCollection
            {
                public ViewGroupCollection(ViewCollection myOqlCodeGenViewsCollection)
                    : base(myOqlCodeGenViewsCollection)
                {
                }

                protected override ConfigurationElement CreateNewElement()
                {
                    return new ViewElement(this);
                }

                protected override object GetElementKey(ConfigurationElement element)
                {
                    ViewElement siteUser = element as ViewElement;
                    return siteUser.Name;
                }
                public class ViewElement : BaseTableElement, ICloneable
                {
                    [ConfigurationProperty("AutoTable", IsRequired = false)]
                    public string AutoTable
                    {
                        get
                        {
                            return (string)(this["AutoTable"]);
                        }
                        set { this["AutoTable"] = value; }
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

                    public string Table
                    {
                        get
                        {
                            return string.Join(",", this.MyTable.AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                .Union(this.AutoTable.AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                                .ToArray());
                        }
                    }

                    public ViewElement(TableGroupCollection myOqlCodeGenViewGroupCollection)
                        : base(myOqlCodeGenViewGroupCollection)
                    {
                    }

                    public override void GetObjectData(SerializationInfo info, StreamingContext context)
                    {
                        base.GetObjectData(info, context);
                        info.AddValue("AutoTable", this["AutoTable"]);
                        info.AddValue("MyTable", this["MyTable"]);
                    }

                    public override object Clone()
                    {
                        var ent = new ViewElement(this.Container);
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
                        //ent.RowTimestamp = this.RowTimestamp;

                        ent.AutoTable = this.AutoTable;
                        ent.MyTable = this.MyTable;
                        return ent;
                    }
                }
            }
        }
    }
}
