using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using System.ComponentModel;
using System.Data;

namespace MyOql.MyOql_CodeGen
{
    /// <summary>
    /// 代码生成器中的Enums定义的每个节点.
    /// </summary>
    [Serializable]
    public class MyEnumNode
    {
        /// <summary>
        /// 数据库的列名。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 枚举类型，如果是表值函数，则表示具体的列的 C# 类型
        /// </summary>
        /// <remarks>
        /// 如果自定义了类型, 那么需要实现 TypeConverter , 且要实现 CanConvertFrom,ConvertFrom  或 CanConvertTo,ConvertTo.(类型转换需要.)
        /// </remarks>
        public string EnumType { get; set; }

        /// <summary>
        /// 数据库中原始的类型
        /// </summary>
        //public DbType DbType { get; set; }

        private string _TranslateName { get; set; }
        /// <summary>
        /// 数据库列的别名。
        /// </summary>
        public string TranslateName { get { return dbo.TranslateDbName(_TranslateName.AsString(this.Name)); } set { this._TranslateName = value; } }

        //public Type GetEnumType()
        //{
        //    if (EnumType == "?")
        //    {
        //        return typeof(Nullable<>);
        //    }
        //    else if (EnumType.EndsWith("?") == false)
        //    {
        //        return System.Web.Compilation.BuildManager.GetType(EnumType, false) ??
        //            System.Web.Compilation.BuildManager.GetType("MyOql." + EnumType, false) ??
        //            System.Web.Compilation.BuildManager.GetType("MyCmn." + EnumType, false) ??
        //            System.Web.Compilation.BuildManager.GetType("System." + EnumType, false);
        //    }
        //    else
        //    {
        //        var t = EnumType.Substring(0, EnumType.Length - 1);
        //        return typeof(Nullable<>).MakeGenericType(System.Web.Compilation.BuildManager.GetType(t, false) ??
        //            System.Web.Compilation.BuildManager.GetType("MyOql." + t, false) ??
        //            System.Web.Compilation.BuildManager.GetType("MyCmn." + t, false) ??
        //            System.Web.Compilation.BuildManager.GetType("System." + t, false));
        //    }
        //}

        public MyEnumNode(string Define)
        {
            var eachPart = Define.Split("=:".ToCharArray());

            Name = eachPart[0];
            EnumType = eachPart[1];
            if (eachPart.Length > 2) TranslateName = eachPart[2];
            else TranslateName = Name;
        }

        public override string ToString()
        {
            return Name + "=" + EnumType.ToString() + ":" + TranslateName;
        }

        public static implicit operator MyEnumNode(string Define)
        {
            return new MyEnumNode(Define);
        }
    }


    [TypeConverter(typeof(MyEnumDefineConverter))]
    [Serializable]
    public class MyEnumDefine : List<MyEnumNode>, ICloneable
    {
        public class MyEnumDefineConverter : TypeConverter
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
                if (value.IsDBNull()) return new MyEnumDefine(string.Empty);

                string val = value as string;
                if (val != null) return new MyEnumDefine(val);
                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                var obj = value as MyEnumDefine;
                if (destinationType.FullName == "System.String") return obj.ToString();
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
        public MyEnumNode this[string key]
        {
            get
            {
                var cou = this.Count(o => o.Name == key);
                if (cou == 0)
                {
                    return null;
                }
                else return this.First(o => o.Name == key);
            }
        }

        public MyEnumDefine(string Define)
            : base()
        {
            foreach (var item in Define.Replace(" ", "").Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                this.Add(new MyEnumNode(item));
            }
        }

        public override string ToString()
        {
            return string.Join(",", this.Select(o => o.ToString()).ToArray());
        }


        public static implicit operator MyEnumDefine(string Define)
        {
            return new MyEnumDefine(Define);
        }

        public bool ContainsKey(string col)
        {
            return this.Count(o => o.Name == col) > 0;
        }

        public object Clone()
        {
            return new MyEnumDefine(this.ToString());
        }
    }
}
