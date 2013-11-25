using System;
using System.ComponentModel;
using MyCmn;

namespace MyOql
{
    [TypeConverter(typeof(MyOqlActionEnumConverter))]
    [Flags]
    public enum MyOqlActionEnum
    {
        Create = 1,
        Read = 2,
        Update = 4,
        Delete = 8,

        /// <summary>
        /// 目前在 Read , Update 中处理 .
        /// </summary>
        Row = 16,
        Proc = 32,
        All = 1023,
    }

    public static class MyOqlActionEnumExtend
    {
        public static string TraslateToString(this MyOqlActionEnum action)
        {
            StringLinker sl = new StringLinker();
            foreach (var item in action.ToEnumList())
            {
                switch (item)
                {
                    case MyOqlActionEnum.Create:
                        sl += "c";
                        break;
                    case MyOqlActionEnum.Read:
                        sl += "r";
                        break;
                    case MyOqlActionEnum.Update:
                        sl += "u";
                        break;
                    case MyOqlActionEnum.Delete:
                        sl += "d";
                        break;
                    case MyOqlActionEnum.Row:
                        sl += "=";
                        break;
                    case MyOqlActionEnum.Proc:
                        sl += "p";
                        break;
                    default:
                        break;
                }
            }
            return sl;
        }

        public static MyOqlActionEnum TranslateMyOqlAction(string PowerWord)
        {
            if (string.Equals(PowerWord, "false", StringComparison.CurrentCultureIgnoreCase)) return 0;
            //没有"," 的权限段.
            Type type = typeof(MyOqlActionEnum);
            Func<string, MyOqlActionEnum> OneType = Power =>
            {
                if (Enum.IsDefined(type, Power))
                {
                    return (MyOqlActionEnum)Enum.Parse(type, Power);
                }
                else
                {
                    MyOqlActionEnum retOne = new MyOqlActionEnum();
                    foreach (var item in Power.ToCharArray())
                    {
                        if (item == 'c') retOne |= MyOqlActionEnum.Create;
                        else if (item == 'r') retOne |= MyOqlActionEnum.Read;
                        else if (item == 'u') retOne |= MyOqlActionEnum.Update;
                        else if (item == 'd') retOne |= MyOqlActionEnum.Delete;
                        else if (item == 'p') retOne |= MyOqlActionEnum.Proc;
                        else if (item == '=') retOne |= MyOqlActionEnum.Row;
                    }
                    return retOne;
                }
            };

            MyOqlActionEnum retEnum = new MyOqlActionEnum();
            foreach (var EachSect in PowerWord.AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                retEnum |= OneType(EachSect);
            }
            return retEnum;
        }

    }

    public class MyOqlActionEnumConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            if (sourceType.FullName == "System.DBNull")
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((MyOqlActionEnum)value).TraslateToString();
            }


            return base.ConvertTo(context, culture, value, destinationType);
        }


        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null) return null;
            var sourceType = value.GetType();
            if (sourceType == typeof(string))
            {
                return MyOqlActionEnumExtend.TranslateMyOqlAction(value.AsString());
            }

            if (sourceType.FullName == "System.DBNull")
            {
                return default(MyOqlActionEnum); // new MyDate(DateTime.MinValue);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
