using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using System.ComponentModel;

namespace MyOql.MyOql_CodeGen
{
    /// <summary>
    /// 外键定义格式
    /// </summary>
    [Serializable]
    public class MyFkNode : ICloneable
    {
        public class MyFkMapNode : MyFkNode
        {
            /// <summary>
            /// 当前表外键别名
            /// </summary>
            public string MapColumn { get; set; }

            /// <summary>
            /// 外键引用表别名
            /// </summary>
            public string MapRefTable { get; set; }

            /// <summary>
            /// 外键引用表引用列别名
            /// </summary>
            public string MapRefColumn { get; set; }

            /// <summary>
            /// 当前数据库表名
            /// </summary>
            public string Entity { get; set; }

            /// <summary>
            /// 当前数据库表别名
            /// </summary>
            public string MapEntity { get; set; }

            public MyFkMapNode() { }
            public MyFkMapNode(MyFkNode Node, string Entity)
            {
                this.Entity = Entity;
                this.MapEntity = Entity;

                this.Column = Node.Column;
                this.RefTable = Node.RefTable;
                this.RefColumn = Node.RefColumn;

                this.MapColumn = Node.Column;
                this.MapRefTable = Node.RefTable;
                this.MapRefColumn = Node.RefColumn;
            }

            public override object Clone()
            {
                return this.MemberwiseClone();
            }
        }

        public bool CascadeUpdate { get; set; }
        public bool CascadeDelete { get; set; }

        /// <summary>
        /// 外键引用表
        /// </summary>
        public string RefTable { get; set; }

        /// <summary>
        /// 外键引用表的引用列。
        /// </summary>
        public string RefColumn { get; set; }

        public string Column { get; set; }


        public MyFkNode() { }
        public MyFkNode(string Define)
        {
            var eachPart = Define.Split("=:".ToCharArray());

            RefColumn = string.Empty;

            Column = eachPart[0];

            if (Column.EndsWith(")"))
            {
                var leftIndex = Column.IndexOf('(');
                if (leftIndex > 0)
                {
                    var content = Column.Substring(leftIndex + 1, Column.Length - 2 - leftIndex);
                    this.Column = Column.Substring(0, leftIndex);

                    this.CascadeUpdate = content.Contains(o => o == 'u' || o == 'U');
                    this.CascadeDelete = content.Contains(o => o == 'd' || o == 'D');
                }
            }


            RefTable = eachPart[1];
            if (eachPart.Length > 2) RefColumn = eachPart[2];
        }

        public override string ToString()
        {
            var ret = Column;
            if (this.CascadeUpdate && this.CascadeDelete)
            {
                ret += "(";
                if (this.CascadeUpdate) ret += "u";
                if (this.CascadeDelete) ret += "d";
                ret += ")";
            }

            return ret + "=" + RefTable.ToString() + ":" + RefColumn;
        }

        public static implicit operator MyFkNode(string Define)
        {
            return new MyFkNode(Define);
        }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        public MyFkMapNode TranslateMapName(string tabName, MyOqlCodeGenSect.TableCollection tables)
        {
            var config = tables.GetConfig(tabName) as MyOql.MyOqlCodeGenSect.TableCollection.TableGroupCollection.TableElement;
            var refConfig = tables.GetConfig(this.RefTable) as MyOql.MyOqlCodeGenSect.TableCollection.TableGroupCollection.TableElement;
            MyFkMapNode map = new MyFkMapNode(this, tabName);

            if (config != null)
            {
                map.MapEntity = config.MapName;
                var define = config.Enums.FirstOrDefault(o => o.Name == map.Column);
                if (define != null)
                {
                    map.MapColumn = define.TranslateName;
                }
            }

            if (refConfig != null)
            {
                map.MapRefTable = refConfig.MapName;

                var define = refConfig.Enums.FirstOrDefault(o => o.Name == map.RefColumn);
                if (define != null)
                {
                    map.MapRefColumn = define.TranslateName;
                }

            }
            return map;
        }
    }


    [TypeConverter(typeof(MyFkDefineConverter))]
    [Serializable]
    public class MyFkDefine : List<MyFkNode>, ICloneable
    {
        public class MyFkDefineConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType.FullName == "System.String") return true;
                return base.CanConvertFrom(context, sourceType);
            }
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType.FullName == "System.String") return true;
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value.IsDBNull()) return new MyFkDefine(string.Empty);

                string val = value as string;
                if (val != null) return new MyFkDefine(val);
                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                var obj = value as MyFkDefine;
                if (destinationType.FullName == "System.String") return obj.ToString();
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public MyFkDefine() : base() { }
        public string Table { get; set; }
        public MyFkDefine(string Define)
            : base()
        {
            foreach (var item in Define.Replace(" ", "").Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                GodError.Check(this.Count(o => o.ToString() == item) > 0, "检测到重复的外键定义: " + item);
                this.Add(new MyFkNode(item));
            }
        }

        public override string ToString()
        {
            return string.Join(",", this.Select(o => o.ToString()).ToArray());
        }


        public static implicit operator MyFkDefine(string Define)
        {
            return new MyFkDefine(Define);
        }

        public object Clone()
        {
            return new MyFkDefine(this.ToString());
        }

        public List<MyFkNode.MyFkMapNode> TranslateMapName(MyOqlCodeGenSect sect, string Table)
        {
            List<MyFkNode.MyFkMapNode> list = new List<MyFkNode.MyFkMapNode>();
            this.All(o =>
                {
                    list.Add(o.TranslateMapName(Table, sect.Table));
                    return true;
                });
            return list;
        }
    }
}
