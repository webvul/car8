using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using System.Data;
using System.ComponentModel;

namespace MyOql.MyOql_CodeGen
{
    /// <summary>
    /// 定义存储过程返回值格式.
    /// </summary>
    /// <remarks>
    ///      但要特别注意的是：如果果 Return 返回定义为空，则调用 ProcClip.Execute，即返回存储过程的返回值（存储过程中return Value 形式,如果没有定义return ，则返回影响行数）
    ///      而如果 Return 显式定义了返回数字类型，则调用 ProcClip.ExecuteReader ，即返回存储过程结果集的第一行第一列的值（select * from table）
    /// </remarks>
    /// <example>
    /// 参数名称=返回实体或返回类型:out
    ///	返回实体或返回类型包括：
    ///	    XmlDictionary&lt;T,V&gt;
    ///	    DataTable
    ///	    DataSet
    ///		实体：如  PersonRule.Entity。
    ///		实体数组： PersonRule.Entity[]。
    ///		值：须是DbType 枚举类型，如 Int32 , AnsiString。
    ///		值数组： 须是DbType 枚举类型数组。如 AnsiString[] ，系统返回对应的数据类型的数组。
    ///		void : 即使定义void ,也会返回存储过程影响行数.
    ///	对于 Oracle 来说 ,所有的存储过程都要在一个包下定义. 包定义见 MyOqlConfigSect 配置节.
    /// 由于Oracle存储过程不能直接返回结果集,但 SqlServer 可以返回结果集. 所以返回参数名在SqlServer中忽略.
    /// </example>
    [TypeConverter(typeof(MyParaReturnDefineConverter))]
    [Serializable]
    public class MyParaReturnDefine : ICloneable
    {
        public class MyParaReturnDefineConverter : TypeConverter
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
                if (value.IsDBNull()) return new MyParaReturnDefine(string.Empty);

                string val = value as string;
                if (val != null) return new MyParaReturnDefine(val);
                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                MyParaReturnDefine obj = value as MyParaReturnDefine;
                if (destinationType.FullName == "System.String") return obj.ToString();
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public string Name { get; set; }
        public string ReturnType { get; set; }

        public MyParaReturnDefine(string Define)
        {
            if (Define.HasValue() == false)
            {
                this.Name = string.Empty;
                this.ReturnType = string.Empty;
                return;
            }

            var eachPart = Define.Split("=:".ToCharArray());

            Name = eachPart[0];
            ReturnType = eachPart[1];
        }

        public override string ToString()
        {
            return Name + "=" + ReturnType + ":ret";
        }

        public static implicit operator MyParaReturnDefine(string Define)
        {
            return new MyParaReturnDefine(Define);
        }

        public bool ReturnIsArray()
        {
            return ReturnType.EndsWith("[]");
        }

        public bool ReturnIsDict()
        {
            return ReturnType.StartsWith("XmlDictionary[") || ReturnType.StartsWith("Dictionary[");
        }

        public bool ReturnIsVoid()
        {
            return ReturnType == "void" || (ReturnType.HasValue() == false);
        }
        //public bool ReturnIsEntity()
        //{
        //    return ReturnType.EndsWith(".Entity");
        //}
        public bool ReturnIsDataSet()
        {
            return ReturnType == "DataSet";
        }
        public bool ReturnIsDataTable()
        {
            return ReturnType == "DataTable";
        }

        public bool ReturnIsValue()
        {
            if (ReturnIsVoid() || ReturnIsDataSet() || ReturnIsDataTable() || ReturnIsDict()) return false;

            return Enum.IsDefined(typeof(DbType), GetReturnTypeWithoutArray()); //!isDict && !isEntity && !isDataTable && !isDataSet && (returnType != "void");
        }


        public bool ReturnIsModel()
        {
            if (ReturnIsVoid() || ReturnIsDataSet() || ReturnIsDataTable() || ReturnIsDict() || ReturnIsValue()) return false;
            return true;
        }


        public string GetReturnTypeWithoutArray()
        {
            return ReturnType.Replace("[]", "");
        }

        public object Clone()
        {
            return new MyParaReturnDefine(this.ToString());
        }
    }

    /// <summary>
    /// 存储过程的参数定义格式.
    /// </summary>
    /// <remarks>
    /// 参数名称＝DbType枚举值：in/out/inout/ret
    ///     其中 in  是指输入参数
    ///     out 是 输出参数，对应C#程序中的 out 参数
    ///     inout 是输入输出参数，对应C#程序中的 ref 参数
    ///     ret 是一个保留关键字。
    /// </remarks>
    [TypeConverter(typeof(MyParaNodeConverter))]
    [Serializable]
    public class MyParaNode : ICloneable
    {
        public class MyParaNodeConverter : TypeConverter
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
                if (value.IsDBNull()) return new MyParaNode(string.Empty);
                string str = value as string;
                if (str != null) return new MyParaNode(str);
                return base.ConvertFrom(context, culture, value);
            }
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                MyParaNode obj = value as MyParaNode;
                if (destinationType.FullName == "System.String") return obj.ToString();
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }


        public string Name { get; set; }
        public DbType DbType { get; set; }
        public bool IsNullable { get; set; }
        public ParameterDirection Direction { get; set; }

        public MyParaNode(string Define)
        {
            var eachPart = Define.Split("=:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            Direction = ParameterDirection.Input;

            if (eachPart.Length == 2) Direction = ParameterDirection.Input;
            else if (eachPart[2] == "out") Direction = ParameterDirection.Output;
            else if (eachPart[2] == "inout") Direction = ParameterDirection.InputOutput;
            else if (eachPart[2] == "ret") Direction = ParameterDirection.ReturnValue;


            Name = eachPart[0];

            if (eachPart[1].EndsWith("?"))
            {
                IsNullable = true;
                DbType = eachPart[1].Slice(0, -1).AsString().ToEnum<DbType>(true);
            }
            else
            {
                DbType = eachPart[1].ToEnum<DbType>(true);
            }
        }

        public override string ToString()
        {

            var direction = "in";
            if (Direction == ParameterDirection.Output)
            {
                direction = "out";
            }
            else if (Direction == ParameterDirection.InputOutput)
            {
                direction = "inout";
            }
            else if (Direction == ParameterDirection.ReturnValue)
            {
                direction = "ret";
            }
            return Name + "=" + DbType.ToString() + (IsNullable ? "?" : "") + ":" + direction;
        }

        public static implicit operator MyParaNode(string Define)
        {
            return new MyParaNode(Define);
        }

        public object Clone()
        {
            return new MyParaNode(this.ToString());
        }
    }


    [TypeConverter(typeof(MyParaDefineConverter))]
    [Serializable]
    public class MyParaDefine : List<MyParaNode>, ICloneable
    {
        public class MyParaDefineConverter : TypeConverter
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
                if (value.IsDBNull()) return new MyParaDefine(string.Empty);

                string val = value as string;
                if (val != null) return new MyParaDefine(val);
                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                MyParaDefine obj = value as MyParaDefine;
                if (destinationType.FullName == "System.String") return obj.ToString();
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public MyParaDefine(string Define)
            : base()
        {
            foreach (var item in Define.Replace(" ", "").Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                this.Add(new MyParaNode(item));
            }
        }

        public override string ToString()
        {
            return string.Join(",", this.Select(o => o.ToString()).ToArray());
        }


        public static implicit operator MyParaDefine(string Define)
        {
            return new MyParaDefine(Define);
        }

        public object Clone()
        {
            return new MyParaDefine(this.ToString());
        }
    }
}
