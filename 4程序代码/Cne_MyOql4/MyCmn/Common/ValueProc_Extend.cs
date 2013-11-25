using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;
using System.Reflection;

namespace MyCmn
{
    public delegate R Func<T1, T2, T3, T4, T5, R>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    public static partial class ValueProc
    {
        public static string Join<T>(this IEnumerable<T> Value, string separator)
        {
            return string.Join(separator, Value.Select(o => o.AsString()).ToArray());
        }

        /// <summary>
        /// 报告指定 one 在此集合中的第一个匹配项的索引。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source"></param>
        /// <param name="one"></param>
        /// <returns>如果找到该对象，则返回从零开始的索引位置；如果未找到，则返回 -1。</returns>
        public static int IndexOf<TSource>(this IEnumerable<TSource> Source, TSource one)
        {
            return IndexOf(Source, a => { return a.Equals(one); });
        }

        public static int IndexOf<TSource>(this IEnumerable<TSource> Source, Func<TSource, bool> func)
        {
            int retVal = -1;
            using (IEnumerator<TSource> e1 = Source.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    retVal++;
                    if (func(e1.Current))
                        return retVal;
                }

            }
            return -1;
        }

        /// <summary>
        /// 是 string.Split 的扩展版本.由于没有默认的按连续字符串分隔.
        /// </summary>
        /// <example>
        /// <code>
        ///     "hello&amp;nbsp;world".Split("&amp;nbsp;") ;
        /// </code>
        /// 返回 ["hello","word"]
        /// </example>
        /// <param name="Source"></param>
        /// <param name="splitString"></param>
        /// <returns></returns>
        public static IEnumerable<string> Split(this string Source, string splitString)
        {
            int currPos = 0;
            int nextPos = 0;

            while (true)
            {
                nextPos = Source.IndexOf(splitString, currPos);
                if (nextPos < 0)
                {
                    yield return Source.Substring(currPos, Source.Length - currPos);
                    yield break;
                }
                else
                {
                    yield return Source.Substring(currPos, nextPos - currPos);
                }

                currPos = nextPos + splitString.Length;
                if (currPos >= Source.Length) yield break;
            }

        }

        public class SplitRegResult
        {
            public string Value { get; set; }
            public bool Sucess { get; set; }
            public int Index { get; set; }
        }
        /// <summary>
        /// 用正则表达式分隔.
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="splitRegString"></param>
        /// <param name="Option"></param>
        /// <returns>返回所有.</returns>
        public static IEnumerable<SplitRegResult> SplitWithReg(this string Source, string splitRegString, RegexOptions Option)
        {
            Regex reg = new Regex(splitRegString, Option);

            var mstchs = reg.Matches(Source);
            int curPos = 0;
            foreach (Match item in mstchs)
            {
                if (item.Index > curPos)
                {

                    yield return new SplitRegResult()
                    {
                        Value = Source.Substring(curPos, item.Index - curPos),
                        Sucess = false
                    };
                }
                yield return new SplitRegResult()
                {
                    Value = item.Value,
                    Sucess = item.Success,
                    Index = item.Index
                };

                curPos = item.Index + item.Length;
            }

            if (curPos < Source.Length)
            {
                yield return new SplitRegResult()
                {
                    Value = Source.Substring(curPos),
                    Index = curPos,
                    Sucess = false
                };
            }
        }
        /// <summary>
        /// 把第二个集合,交替性插入到第一个集合中,并将第二个集合多余的部分追加到第一个集合末尾.
        /// </summary>
        /// <example>
        /// <code>
        ///     new int[]{1,3,5}.InsertAlternate(new int[]{2,4,6,8,10}) ;
        /// </code>
        /// 返回的结果是 1,2,3,4,5,6,8,10
        /// </example>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> InsertAlternate<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            using (IEnumerator<TSource> e1 = first.GetEnumerator())
            using (IEnumerator<TSource> e2 = second.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    yield return e1.Current;

                    if (e2.MoveNext())
                    {
                        yield return e2.Current;
                    }
                }

                while (e2.MoveNext())
                {
                    yield return e2.Current;
                }
            }
        }

        /// <summary>
        /// 按顺序返回相同个数的两个部分的交替组合.
        /// </summary>
        /// <example>
        /// <code>
        ///     new int[]{1,3,5}.IntersectAndAlternate(new int[]{2,4,6,8,10}) ;
        /// </code>
        /// 返回的结果是 1,2,3,4,5,6
        /// </example>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> IntersectAndAlternate<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            using (IEnumerator<TSource> e1 = first.GetEnumerator())
            using (IEnumerator<TSource> e2 = second.GetEnumerator())
                while (e1.MoveNext() && e2.MoveNext())
                {
                    yield return e1.Current;
                    yield return e2.Current;
                }
        }

        /// <summary>
        /// 如把 NameValueCollection 和 Hashtable 转换为 字典.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToMyList<T>(this IEnumerable source, Func<object, T> func)
        {
            var e = source.GetEnumerator();
            while (e.MoveNext())
            {
                yield return func(e.Current);
            }
        }


        ///// <summary>
        ///// 把 Byte 数据序列化为两位一个 Byte 的整齐的十六进制表示形式。和 GetBytesFromHexString 对应使用。 [★]
        ///// </summary>
        ///// <param name="ListBytes"></param>
        ///// <returns></returns>
        //public static string ToHexString(byte[] ListBytes)
        //{
        //    string strRet = "";
        //    foreach (byte byt in ListBytes)
        //    {
        //        strRet += string.Format("{0:X2}", byt);
        //    }
        //    return strRet;
        //}

        ///// <summary>
        ///// 把两位一个 Byte 的整齐的十六进制表示形式 转换为 Byte 数组。和 ToHexString 对应使用。 [★]
        ///// </summary>
        ///// <param name="strBytes"></param>
        ///// <returns></returns>
        //public static List<byte> GetBytesFromHexString(string strBytes)
        //{
        //    List<byte> li_Bytes = new List<byte>();
        //    for (int i = 0; i < strBytes.Length; i = i + 2)
        //    {
        //        byte byt = 0;
        //        byt = Convert.ToByte(Uri.FromHex(strBytes[i]) * 16 + Uri.FromHex(strBytes[i + 1]));
        //        li_Bytes.Add(byt);
        //    }
        //    return li_Bytes;
        //}

        /// <summary>
        /// 暗文（不显示字符）的 列分隔符。
        /// </summary>
        public const char SplitCell = (char)7;
        /// <summary>
        /// 暗文（不显示字符）的 行分隔符。
        /// </summary>
        public const char SplitLine = (char)8;
        /// <summary>
        /// 暗文（不显示字符）的 段分隔符。
        /// </summary>
        public const char SplitSect = (char)9;

        /// <summary>
        /// 明文的数组分隔符  ,
        /// </summary>
        public const char Comma = ',';

        /// <summary>
        /// 明文模板的字界符 %
        /// </summary>
        public const char Percent = '%';

        public static string Slice(this string source, int startIndex)
        {
            return Slice(source, startIndex, Int32.MaxValue);
        }

        public static string Slice(this string source, int startIndex, int endIndex)
        {
            if (startIndex < 0) return Slice(source, source.Length + startIndex, endIndex);
            if (endIndex < 0) return Slice(source, startIndex, source.Length + endIndex);

            return new string(Slice<char>(source, startIndex, endIndex).ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="one"></param>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsIn<T>(this T one, params T[] source)
        {
            return source.Contains(one);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="one"></param>
        /// <param name="func"></param>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsIn<T>(this T one, IEqualityComparer<T> func, params T[] source)
        {
            return source.Contains(one, func);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="one"></param>
        /// <param name="func">返回 true,表示找到该项. </param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsIn<T>(this T one, Func<T, T, bool> func, params T[] source)
        {
            return source.All(o =>
            {
                if (func(one, o)) return false;
                else return true;
            }) == false;
        }

        public static bool IsIn<T>(this T one, IEnumerable<T> source)
        {
            return source.Contains(one);
        }

        /// <summary>
        /// 从字符串里，取出第一个数值内容。实现类似 Javascript 的 parseInt 方法
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int TakeOutInt(this string value)
        {
            return TakeOutInt(value, 0);
        }

        /// <summary>
        /// 从字符串里，取出第一个数值内容。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int TakeOutInt(this string value, int defaultValue)
        {
            if (value.HasValue() == false) return defaultValue;

            //匹配不是URL转义的带有％3 这种形式的数字。 @"(?<!%)\d+"

            Regex rex = new Regex(@"[\-|+]?\d+", RegexOptions.Compiled);

            if (rex.IsMatch(value) == false) return defaultValue;

            return rex.Match(value).Value.AsInt();
        }

        /// <summary>
        /// 做指定类型的类型转换
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object AsType(Type type, object value)
        {
            if (value.IsDBNull()) return value;
            else if (type.IsEnum && (value.AsString().HasValue() == false)) return EnumHelper.GetDefault(type);

            Type valueType = value.GetType();

            //如果是可空类型，则取里面的类型。
            if (valueType.IsNullableType())
            {
                valueType = valueType.GetGenericArguments()[0];
            }

            if (type.IsClass || type.IsValueType)
            {
                if (valueType == type) return value;
                if (valueType.IsSubclassOf(type)) return value;
            }
            else if (type.IsInterface)
            {
                if (valueType.GetInterface(type.FullName, false) != null)
                {
                    return value;
                }
            }

            if (valueType.IsEnum)
            {
                if (type.IsNumberType()) value = value.AsInt();
            }

            string typeName = type.FullName;
            //lcc新加
            if (typeName == "System.Object") return value;
            else if (typeName == "System.String") return value.AsString();
            else if (typeName == "System.Int16") return value.AsShortInt();
            else if (typeName == "System.Int32") return value.AsInt();
            else if (typeName == "System.Int64") return ValueProc.AsLong(value);
            else if (typeName == "System.Decimal") return value.AsDecimal();
            else if (typeName == "System.Boolean") return value.AsBool();
            else if (typeName == "System.Single") return ValueProc.AsFloat(value);
            else if (typeName == "System.Double") return ValueProc.AsDouble(value);
            else if (typeName == "System.DateTime") return value.AsDateTime();
            else if (typeName == "System.UInt32") return ValueProc.AsUInt(value);
            else if (typeName == "System.UInt64")
            {
                ulong retVal = 0;
                ulong.TryParse(value.AsString(), out retVal);
                return retVal;
            }

            else if (typeName == "System.UInt16")
            {
                ushort retVal = 0;
                ushort.TryParse(value.AsString(), out retVal);
                return retVal;
            }
            else if (typeName == "System.Byte")
            {
                byte retVal = 0;
                byte.TryParse(value.AsString(), out retVal);
                return retVal;
            }
            else if (typeName == "System.Char")
            {
                char retVal = char.MinValue;
                char.TryParse(value.AsString(), out retVal);
                return retVal;
            }

            //枚举类
            //if (type.IsIEnm())
            //{

            //}


            if (object.Equals(value, null) == false)
            {
                if (type.IsEnum)
                {
                    if (valueType.IsNumberType())
                    {
                        return Enum.ToObject(type, value.AsInt());
                    }
                    else
                    {
                        return EnumHelper.ToEnum(value.AsString(), type, 0);
                    }
                }

                return changeType(type, value, valueType);
            }

            return value;
        }


        /// <summary>
        /// 精准判断是否是一个类型。基类不参与运算。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsType<T>(this object obj)
            where T : class
        {
            return IsType<T>(obj, false);
        }

        public static bool IsValueType<T>(this object obj)
            where T : struct
        {
            if (object.Equals(obj, null)) return false;
            var type = typeof(T);
            return type == obj.GetType();
        }


        /// <summary>
        /// 判断一个类型是否是可空类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableType(this Type type)
        {
            return (type.IsGenericType &&  type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static bool IsType<T>(this object obj, bool CompareBaseType)
            where T : class
        {
            if (Equals(obj, null)) return false;
            var tType = typeof(T);

            if (tType.IsSealed || CompareBaseType)
            {
                var t = obj as T;
                if (object.Equals(t, null)) return false;
                else return true;
            }
            else
            {
                return obj.GetType() == tType;
            }
        }

        private static object changeType(Type targetType, object value, Type valueType)
        {
            if (value.IsDBNull()) return value;
            if (valueType == null) valueType = value.GetType();

            TypeConverter typeConvertFrom = TypeDescriptor.GetConverter(targetType);

            if (typeConvertFrom.CanConvertFrom(valueType))
            {
                return typeConvertFrom.ConvertFrom(null, CultureInfo.CurrentCulture, value);
            }
            else
            {
                TypeConverter typeConvertTo = TypeDescriptor.GetConverter(valueType);
                if (typeConvertTo.CanConvertTo(targetType))
                {
                    return typeConvertTo.ConvertTo(value, targetType);
                }
            }

            var typeName = targetType.FullName;

            if (valueType.IsSubclassOf(typeof(Newtonsoft.Json.Linq.JToken)))
            {
                {
                    var jsonObj = value as Newtonsoft.Json.Linq.JValue;
                    if (jsonObj != null)
                    {
                        //处理简单类型 ，不用处理 String 。
                        if (typeName == "System.Int16") return value.ToString().AsShortInt();
                        if (typeName == "System.Int32") return value.ToString().AsInt();
                        if (typeName == "System.Int64") return ValueProc.AsLong(value.ToString());
                        if (typeName == "System.Decimal") return value.ToString().AsDecimal();
                        if (typeName == "System.Boolean") return value.ToString().AsBool();
                        if (typeName == "System.Single") return ValueProc.AsFloat(value.ToString());
                        if (typeName == "System.Double") return ValueProc.AsDouble(value.ToString());
                        if (typeName == "System.DateTime") return value.ToString().AsDateTime();
                        if (typeName == "System.UInt32") return ValueProc.AsUInt(value.ToString());
                    }
                }
                {
                    var jsonObj = value as Newtonsoft.Json.Linq.JContainer;
                    if (jsonObj != null)
                    {
                        if (jsonObj.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                        {
                            var aryObj = jsonObj as Newtonsoft.Json.Linq.JArray;

                            var typeIsAry = targetType.IsArray;
                            var typeIsList = !typeIsAry && (targetType.IsGenericType && targetType.GetInterface("IList") != null);
                            if (typeIsAry || typeIsList)
                            {
                                var type = typeIsAry ? targetType.GetElementType() : targetType.GetGenericArguments()[0];

                                var ary = Array.CreateInstance(type, aryObj.Count);

                                for (var i = 0; i < aryObj.Count; i++)
                                {
                                    ary.SetValue(ValueProc.AsType(type, aryObj[i]), i);
                                }

                                if (typeIsAry)
                                {
                                    return ary;
                                }
                                else
                                {

                                    var list = Activator.CreateInstance(
                                    typeof(List<>).MakeGenericType(new Type[] { type }),
                                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                                    null, null, null) as IList;

                                    for (int i = 0; i < ary.Length; i++)
                                    {
                                        list.Add(ary.GetValue(i));
                                    }
                                    return list;
                                }
                            }
                        }
                    }
                }
            }


            //处理一下当value 是空字符串时，返回targetType的默认值。
            if (valueType.FullName == "System.String")
            {
                return targetType.GetDefaultValue();
            }
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static XmlDictionary<TKey, TValue> ToXmlDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            return new XmlDictionary<TKey, TValue>(source.ToDictionary(o => o.Key, o => o.Value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keyFunc"></param>
        /// <param name="valueFunc"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TDescKey"></typeparam>
        /// <typeparam name="TDescValue"></typeparam>
        /// <returns></returns>
        public static XmlDictionary<TDescKey, TDescValue> ToXmlDictionary<TKey, TValue, TDescKey, TDescValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, Func<KeyValuePair<TKey, TValue>, TDescKey> keyFunc, Func<KeyValuePair<TKey, TValue>, TDescValue> valueFunc)
        {
            var xmlDict = new XmlDictionary<TDescKey, TDescValue>();
            source.All(o =>
                {
                    var key = (TDescKey)keyFunc(o);
                    GodError.Check(key == null, () => "将:" + o.Key.AsString() + "转换成:" + typeof(TDescKey).FullName + " 失败");
                    xmlDict[key] = (TDescValue)valueFunc(o);
                    return true;
                });
            return xmlDict;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="elementSelector"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <returns></returns>
        public static XmlDictionary<TKey, TElement> ToXmlDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return new XmlDictionary<TKey, TElement>(source.ToDictionary(keySelector, elementSelector));
        }

        /// <summary>
        /// 用反射的方式,把字典里的数据 枚举化. 枚举化规则采用 字典Key值 和 实体属性值相同.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static XmlDictionary<string, object> UpdateEnums<T>(this XmlDictionary<string, object> dict, T entity)
            where T : class,new()
        {
            Type type = typeof(T);
            foreach (var property in type.GetProperties())
            {
                if (property.PropertyType.IsEnum && dict.ContainsKey(property.Name))
                {
                    var valType = dict[property.Name].GetType();
                    if (valType.IsNumberType() ||
                        Enum.IsDefined(property.PropertyType, dict[property.Name].AsString()) == false)
                    {
                        dict[property.Name] = Enum.ToObject(property.PropertyType, dict[property.Name].AsInt());
                    }
                    else
                    {
                        dict[property.Name] = Enum.Parse(property.PropertyType, dict[property.Name].AsString());
                    }
                }
            }

            return dict;
        }

        ///// <summary>
        ///// 如果是 default(M)，则返回 NullFunc 回调内容，否则返回执行的 NotNullFunc
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="TM"></typeparam>
        ///// <param name="obj"></param>
        ///// <param name="nullFunc"></param>
        ///// <param name="notNullFunc"></param>
        ///// <returns></returns>
        //public static TM IfNoValue<T, TM>(this T obj, Func<TM> nullFunc, Func<T, TM> notNullFunc)
        //{
        //    if (object.Equals(obj, default(T)) || obj.Equals(default(T)))
        //    {
        //        if (nullFunc != null)
        //            return nullFunc();
        //        else return default(TM);
        //    }
        //    else return notNullFunc(obj);
        //}


        /// <summary>
        /// 使用IEntity 提供的方法进行对象属性拷贝.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="toEntity"></param>
        /// <returns></returns>
        public static T CopyTo<T>(this IEntity entity, T toEntity)
            where T : class, IEntity
        {
            GodError.Check(entity == null, "拷贝源不能为空");
            GodError.Check(toEntity == null, "拷贝目标对象不能为空");

            entity.GetProperties().All(o =>
            {
                toEntity.SetPropertyValue(o, entity.GetPropertyValue(o));
                return true;
            });
            return toEntity;
        }


        /// <summary>
        /// 超级IndexOf.
        /// </summary>
        /// <param name="value">数据源.</param>
        /// <param name="findValue">查找字符串</param>
        /// <param name="startIndex">开始查找的索引</param>
        /// <param name="comparison">比较枚举</param>
        /// <param name="prevCharFunc">找到字符串后再二次判断前面的字符串是否符合要求.</param>
        /// <param name="nextCharFunc">找到字符串后再三次判断前面的字符串是否符合要求.</param>
        /// <returns></returns>
        public static int IndexOf(this string value, string findValue, int startIndex, StringComparison comparison, Func<char, bool> prevCharFunc, Func<char, bool> nextCharFunc)
        {
            var index = value.IndexOf(findValue, startIndex, comparison);

            if (index < 0)
            {
                return index;
            }
            if (index == 0)
            {
                if (index + findValue.Length < value.Length && nextCharFunc(value[index + findValue.Length]))
                {
                    return index;
                }
            }
            else if (index + findValue.Length >= value.Length)
            {
                if (prevCharFunc(value[index - 1]))
                {
                    return index;
                }
            }
            else
            {
                if (prevCharFunc(value[index - 1]) && nextCharFunc(value[index + findValue.Length]))
                {
                    return index;
                }
            }


            return IndexOf(value, findValue, startIndex + findValue.Length, comparison, prevCharFunc, nextCharFunc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetSummary(this TimeSpan time)
        {
            Func<double, string> _GetTime = null, getTime = milliSecond =>
            {
                double seconed = 1000;
                double minutes = 60000;
                double hour = 3600000;
                double day = 86400000;
                double month = 2592000000;
                double year = 315360000000;

                if (milliSecond == 0) return "";

                if (milliSecond < seconed) return " " + milliSecond + "毫秒";
                if (milliSecond < minutes) return " " + (milliSecond / seconed).AsInt(0) + "秒" + _GetTime(milliSecond % seconed);
                if (milliSecond < hour) return " " + (milliSecond / minutes).AsInt(0) + "分" + _GetTime(milliSecond % minutes);
                if (milliSecond < day) return " " + (milliSecond / hour).AsInt(0) + "小时" + _GetTime(milliSecond % hour);
                if (milliSecond < month) return " " + (milliSecond / day).AsInt(0) + "天" + _GetTime(milliSecond % day);
                if (milliSecond < year) return " " + (milliSecond / month).AsInt(0) + "月" + _GetTime(milliSecond % month);
                return " " + (milliSecond / year).AsInt(0) + "年" + _GetTime(milliSecond % year);
            };
            _GetTime = getTime;

            return getTime(time.TotalMilliseconds);
        }

        /// <summary>
        /// 从 InnerException 中找出 GodError 
        /// </summary>
        /// <param name="exceptionError"></param>
        /// <returns></returns>
        public static GodError GetGodError(this Exception exceptionError)
        {
            var god = exceptionError as GodError;
            if (god != null) return god;
            if (exceptionError.InnerException == null) return null;
            return GetGodError(exceptionError.InnerException);
        }

        /// <summary>
        /// 字符串大小写是否一致（全大写或全小写）。
        /// </summary>
        /// <param name="value"></param>
        /// <returns>空值返回true</returns>
        public static bool IsSameCase(this string value)
        {
            if (value.HasValue() == false) return true;
            UnicodeCategory PrevCase = 0;
            UnicodeCategory CurCase = 0;
            bool firstRecu = true;

            value.All(o =>
            {
                if (char.IsLetter(o) == false) return true;
                if (firstRecu)
                {
                    firstRecu = false;
                    PrevCase = char.GetUnicodeCategory(o);
                    CurCase = PrevCase;
                    return true;
                }

                if (char.IsUpper(o))
                {
                    CurCase = UnicodeCategory.UppercaseLetter;

                    if (CurCase == PrevCase) { PrevCase = CurCase; return true; }
                    else return false;
                }
                else if (char.IsLower(o))
                {
                    CurCase = UnicodeCategory.LowercaseLetter;
                    if (CurCase == PrevCase) { PrevCase = CurCase; return true; }
                    else return false;
                }
                return true;
            });

            return PrevCase == CurCase;
        }



        /// <summary>
        /// 验证Html片断是否合法。
        /// </summary>
        /// <param name="HtmlSects"></param>
        /// <returns></returns>
        public static bool IsHtmlValidata(string HtmlSects)
        {
            try
            {
                new HtmlCharLoad(HtmlSects.ToCharArray()).Load(HtmlNodeProc.ProcType.None);
                return true;
            }
            catch { return false; }
        }

    }
}
