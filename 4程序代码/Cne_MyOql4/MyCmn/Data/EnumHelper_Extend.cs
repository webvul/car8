using System;

namespace MyCmn
{
    /// <summary>
    /// 枚举的描述属性
    /// </summary>
    public class MyDescAttribute : Attribute
    {
        public MyDescAttribute()
        {
        }
        public MyDescAttribute(string SameValue)
        {
            this.Desc = SameValue;
        }

        public string Desc
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Desc;
        }
    }
    /// <summary>
    /// 枚举的辅助类， 在 数据库 列定义， UI 传值 规范， UI 列表列定义规范中经常会使用。
    /// </summary>
    public static partial class EnumHelper
    {
        public static Type WhichDefine(string EnumValue, params Type[] FindEnumType)
        {
            if (FindEnumType == null) return null;

            foreach (Type item in FindEnumType)
            {
                if (Enum.IsDefined(item, EnumValue) == true)
                    return item;
            }

            return null;
        }

        public enum EnumType
        {
            /// <summary>
            /// 逻辑运算.二进制数里只能有一个1
            /// </summary>
            Short = 1,
            /// <summary>
            /// 值大于0
            /// </summary>
            NotZero = 2,
            /// <summary>
            /// 返回枚举的所有定义.
            /// </summary>
            All = 4,
        }
    }
}
