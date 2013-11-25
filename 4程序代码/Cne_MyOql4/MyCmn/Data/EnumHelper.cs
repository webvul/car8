using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
//using Microsoft.Practices.Unity;
using System.Configuration;

namespace MyCmn
{
    public interface IEnumEvent
    {
        //T ToEnum<T>(string obj) where T : IComparable, IFormattable, IConvertible;
        object ToEnum(Type EnumType, string obj, object DefaultValue);
    }
    /// <summary>
    /// 枚举辅助类
    /// </summary>
    public static partial class EnumHelper
    {
        public static string GetMyDesc<T>(this T myEnum) where T : IComparable, IFormattable, IConvertible
        {
            var type = typeof(T);
            if (type.IsEnum == false)
            {
                type = myEnum.GetType();
            }
            return GetMyDesc(type, myEnum.AsInt());
        }
        public static T GetMyEnumFromDesc<T>(string AnyDescr) where T : IComparable, IFormattable, IConvertible
        {
            //  MyDescAttribute retVal = new MyDescAttribute();
            FieldInfo[] fis = typeof(T).GetFields();
            if (fis.Length == 0)
            {
                return default(T);
            }
            foreach (FieldInfo item in fis)
            {
                if (item.GetType().FullName == "System.Reflection.RtFieldInfo") continue;
                var arrs = item.GetCustomAttributes(typeof(MyDescAttribute), false);
                if (arrs.Length == 0) continue;
                MyDescAttribute desc = arrs[0] as MyDescAttribute;
                if (string.Equals(desc.Desc.AsString(), AnyDescr, StringComparison.CurrentCultureIgnoreCase)) return item.Name.ToEnum<T>();
            }
            return default(T);
        }

        /// <summary>
        /// 按Short 来.
        /// </summary>
        /// <param name="EnumDefineType"></param>
        /// <param name="myEnumValue"></param>
        /// <returns></returns>
        public static string GetMyDesc(Type EnumDefineType, int myEnumValue)
        {
            List<string> retVal = new List<string>();
            List<int> val = myEnumValue.ToEnumList().ToList();
            Array oriData = Enum.GetValues(EnumDefineType);

            if (val.Count > 1)
            {
                val.Remove(0);
            }
            foreach (int eachVal in val)
            {
                foreach (var item in oriData)
                {
                    if (item.AsInt() == eachVal)
                    {
                        Object[] objList = EnumDefineType.GetField(item.AsString()).GetCustomAttributes(typeof(MyDescAttribute), false);
                        if (objList.Length == 0)
                        {
                            retVal.Add(item.AsString());
                        }
                        else
                        {
                            retVal.Add(((MyDescAttribute)objList[0]).ToString());
                        }
                        break;
                    }
                }
            }

            return string.Join(" , ", retVal.ToArray());
        }

        public static T ToEnum<T>(this string EnumString, T DefautValue)
            where T : IComparable, IFormattable, IConvertible
        {
            return ToEnum<T>(EnumString, DefautValue, false);
        }

        public static T ToEnum<T>(this string EnumString, bool ThrowError)
            where T : IComparable, IFormattable, IConvertible
        {
            return ToEnum<T>(EnumString, GetDefault<T>(), false);
        }

        /// <summary>
        /// 把指定的字符串形式的枚举值转换为枚举.如果转换失败,则尝试使用配置项EnumEvent指定的转换方法进行转换.
        /// </summary>
        /// <remarks>
        /// 如果要表示多个枚举组合，不应该是 Enable,Disable   这样的形式，而应该使用 3 这样的数字值。
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="EnumString"></param>
        /// <param name="DefaultValue"></param>
        /// <param name="ThrowError">如果抛出错误，则不使用 EnumEvent 进行转换。</param>
        /// <returns></returns>
        private static T ToEnum<T>(this string EnumString, T DefaultValue, bool ThrowError)
            where T : IComparable, IFormattable, IConvertible
        {
            if (EnumString.HasValue() == false) return DefaultValue;

            var typeT = typeof(T);
            if (typeT.IsEnum == false)
            {
                typeT = DefaultValue.GetType();
            }
            TypeConverter t = TypeDescriptor.GetConverter(typeT);

            try
            {
                return (T)t.ConvertFrom(EnumString);
            }
            catch
            {
                if (ThrowError) throw;
            }


            if (EnumEvent != null)
            {
                return (T)EnumEvent.ToEnum(typeT, EnumString, DefaultValue);
            }
            //IEnumEvent iee = null;
            //var ieeEvent = ConfigurationManager.AppSettings["EnumEvent"];
            //if (ieeEvent.HasValue())
            //{
            //    var type = Type.GetType(ieeEvent);
            //    if (type != null)
            //    {
            //        iee = Activator.CreateInstance(type) as IEnumEvent;


            //        if (iee != null)
            //        {
            //            var ieeValue = iee.ToEnum<T>(EnumString);

            //            if (ieeValue.AsInt() != 0)
            //            {
            //                return ieeValue;
            //            }
            //        }
            //    }
            //}
            return DefaultValue;
        }

        public static IEnumEvent EnumEvent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EnumString"></param>
        /// <param name="EnumType"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public static object ToEnum(this string EnumString, Type EnumType, object DefaultValue)
        {
            GodError.Check(EnumType.IsEnum == false, "GodError", "要求 " + EnumType.FullName + " 必须是枚举类型", null);

            if (EnumString.HasValue() == false) return ValueProc.AsType(EnumType, DefaultValue);


            TypeConverter t = TypeDescriptor.GetConverter(EnumType);

            try
            {
                return t.ConvertFrom(EnumString);
            }
            catch { }

            if (EnumEvent != null)
            {
                return EnumEvent.ToEnum(EnumType, EnumString, DefaultValue);
            }

            //IEnumEvent iee = null;
            //var ieeEvent = ConfigurationManager.AppSettings["EnumEvent"];
            //if (ieeEvent.HasValue())
            //{
            //    var type = Type.GetType(ieeEvent);
            //    if (type != null)
            //    {
            //        iee = Activator.CreateInstance(type) as IEnumEvent;


            //        if (iee != null)
            //        {
            //            var ieeValue = iee.ToEnum(EnumType, EnumString);

            //            if (ieeValue.AsInt() != 0)
            //            {
            //                return ieeValue;
            //            }
            //        }
            //    }
            //}

            return ValueProc.AsType(EnumType, DefaultValue);
        }


        /// <summary>
        /// 取默认值，当是枚举的时候，取空值。优先选择： 0,None,Default,-1,-2147483648
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetDefault<T>()
        {
            var enumType = typeof(T);
            if (enumType.IsNullableType())
            {
                enumType = Nullable.GetUnderlyingType(enumType);
            }

            if (enumType.IsEnum == false)
            {
                return default(T);
            }

            return (T)GetDefault(enumType);
        }


        /// <summary>
        /// 取默认值，当是枚举的时候，取空值。优先选择： 0,None,Default,-1,-2147483648
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static object GetDefault(Type enumType)
        {
            if (enumType.IsNullableType())
            {
                enumType = Nullable.GetUnderlyingType(enumType);
            }

            if (enumType.IsEnum == false)
            {
                return ValueProc.GetDefaultValue(enumType);
            }

            if (Enum.IsDefined(enumType, 0) == false)
            {
                return 0;
            }
            else
            {
                //查找 None,Default
                if (Enum.IsDefined(enumType, "None")) { return Enum.Parse(enumType, "None"); }
                else if (Enum.IsDefined(enumType, "Default")) { return Enum.Parse(enumType, "Default"); }
                else if (Enum.IsDefined(enumType, -1) == false) { return -1; }
                else if (Enum.IsDefined(enumType, int.MinValue) == false) { return int.MinValue; }
            }
            throw new GodError("找不到枚举：" + enumType.FullName + " 的默认值，已搜索：[0,None,Default,-1,-2147483648] 值。");
        }

        /// <summary>
        /// 把指定的字符串形式的枚举值转换为枚举.如果字符串表示多个枚举,用","分隔, 多个返回值用 逻辑或 表示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="EnumString"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string EnumString) where T : IComparable, IFormattable, IConvertible
        {
            return ToEnum<T>(EnumString, GetDefault<T>());
        }

        /// <summary>
        /// 把数字枚举值转换为枚举.多个返回值用 逻辑或 表示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="EnumValue"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this int EnumValue) where T : IComparable, IFormattable, IConvertible
        {
            GodError.Check(typeof(T).IsEnum == false, "不明确的枚举类型!");
            return (T)(object)EnumValue;
        }

        /// <summary>
        /// 把一个类型的枚举值转换为另一个类型的枚举值.多个返回值用 逻辑或 表示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="EnumValue"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this Enum EnumValue)
            where T : IComparable, IFormattable, IConvertible
        {
            return ToEnum<T>(EnumValue.AsInt());
        }
        /// <summary>
        /// 使用 Enum.GetValues 获得枚举各集合. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ToEnumList<T>() where T : IComparable, IFormattable, IConvertible
        {
            var type = typeof(T);
            GodError.Check(type.IsEnum == false, "不明确的枚举类型!");
            var fds = Enum.GetValues(type);

            foreach (var item in fds)
            {
                yield return (T)item;
            }
        }
        /// <summary>
        /// 得到枚举的可能单个枚举值列表.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="TheUnionEnum"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToEnumList<T>(this T TheUnionEnum) where T : IComparable, IFormattable, IConvertible
        {
            return ToEnumList<T>(TheUnionEnum, EnumType.All);
        }

        public static IEnumerable<T> ToEnumList<T>(this T TheUnionEnum, EnumType TheEnumType)
             where T : IComparable, IFormattable, IConvertible
        {
            var type = typeof(T);
            if (type.IsEnum == false)
            {
                type = TheUnionEnum.GetType();
            }
            return ToEnumList((Enum)Enum.ToObject(type, TheUnionEnum.AsInt()), type, TheEnumType).Select(o => (T)ValueProc.AsType(TheUnionEnum.GetType(), o));
        }
        /// <summary>
        /// 得到枚举的可能单个枚举值列表.(仅对标记 Flag 的枚举有效.) 也采用 Enum.GetValues 获取基本数据集合.
        /// </summary>
        /// <param name="TheUnionEnum"></param>
        /// <param name="Type"></param>
        /// <param name="TheEnumType">仅对标记 Flag 的枚举进行运算.</param>
        /// <returns>若枚举没有标记Flag,则返回自己,否则按位返回列表.</returns>
        public static IEnumerable<int> ToEnumList(this Enum TheUnionEnum, Type Type, EnumType TheEnumType)
        {
            var flagAttr = TheUnionEnum.GetType().GetCustomAttributes(typeof(FlagsAttribute), false);
            if (flagAttr == null || (flagAttr.Length == 0))
            {
                return new int[] { TheUnionEnum.AsInt() };
            }

            List<int> retVal = new List<int>();
            List<int> removeList = new List<int>();

            var fds = Enum.GetValues(Type);
            var EnumValue = TheUnionEnum.AsInt();
            foreach (var item in fds)
            {
                var val = item.AsInt();
                if ((EnumValue & val) == val)
                {
                    retVal.Add(val);
                }
            }

            if (TheEnumType.Contains(EnumType.All))
            {
                return retVal;
            }


            if (TheEnumType.Contains(EnumType.Short))
            {
                retVal.All(o =>
                    {
                        IEnumerable<int> eachOne = GetEachDefine(o.AsInt());

                        if (eachOne.Any())
                        {
                            retVal.All(s =>
                            {
                                if (o.Contains(s)) removeList.Add(s);
                                return true;
                            });
                            removeList.Remove(o);
                        }
                        return true;
                    });
            }

            if (TheEnumType.Contains(EnumType.NotZero))
            {
                retVal.Remove(0);
            }
            removeList = removeList.Distinct().ToList();

            retVal = retVal.Minus(removeList).ToList();
            return retVal;
        }

        /// <summary>
        /// 判断是否包含某值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="TheContainer"></param>
        /// <param name="TheOne"></param>
        /// <returns>不判断是否标记Flag,单纯的按二进制进行匹配.</returns>
        public static bool Contains<T>(this T TheContainer, T TheOne) where T : IComparable, IFormattable, IConvertible
        {
            int con = ValueProc.AsInt(TheContainer);
            int one = ValueProc.AsInt(TheOne);
            if (con == one) return true;
            if ((con & one) == one) return true;
            return false;
        }

        /// <summary>
        /// 得到枚举的可能单个枚举值的Int值列表. 算法和 枚举没有关系.
        /// </summary>
        /// <param name="EnumValue"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetEachDefine(int EnumValue)
        {
            EnumValue = EnumValue & int.MaxValue;
            int step = 0;
            while (EnumValue > 0)
            {
                int yu = EnumValue % 2;
                if (yu > 0)
                {
                    yield return 1 << step;
                }
                EnumValue = EnumValue >> 1;
                step++;
            }
        }


        /// <summary>
        /// 按位域得到各个Enum的值。
        /// </summary>
        /// <remarks>
        /// 关于位域，请参考：http://127.0.0.1:47873/help/1-5452/ms.help?method=page&amp;id=M%3aSYSTEM.FLAGSATTRIBUTE.%23CTOR&amp;topicversion=100&amp;topiclocale=ZH-CN&amp;SQM=1&amp;product=VS&amp;productVersion=100&amp;locale=ZH-CN
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="EnumValue"></param>
        /// <returns></returns>
        public static string GetEnumString<T>(this T EnumValue) where T : IComparable, IFormattable, IConvertible
        {
            return string.Join(",", EnumValue.ToEnumList().Select(o => o.ToString()).ToArray());
        }
    }

}