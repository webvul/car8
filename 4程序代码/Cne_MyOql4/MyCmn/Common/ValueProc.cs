using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Collections;

namespace MyCmn
{
    /// <summary>
    /// 一些类型转换的函数。
    /// </summary>
    /// <remarks>
    /// <pre style='line-height:30px;font-size:14px;font-family: 微软雅黑,宋体'>
    /// 最基本的操作是类型转换.它们大部分是扩展方法,方便使用.
    /// 比如方法调用形式
    ///     A.B().C().D().E(); 的可读性要比:  E(D(C(A.B()))) ;  可读性好.
    ///     估且把第一种称为 链式调用. 把第二种称为 层级调用.
    ///     该类的很多方法是把层级调用转换为链式调用.
    /// 它们是在 .net framework 之上再次封装,在实际业务中,用于替换 .net framework 的方法.
    /// <b style="color:red">应用实例</b>
    /// 1. AsString 是 ToString 的替代方案 
    ///     32.AsString();         //等同于 32.ToString();
    ///     null.AsString();       //返回 null;
    ///     new char[]{'h','e','l','l','o'}.AsString();    //返回 "hello"
    ///     对于枚举, 推荐使用 GetEnumString
    /// 2. 同类方法包括:
    ///     GetInt 是  Convert.ToInt 和 int.Parse 的替代方案
    ///     GetBool
    ///     GetDateTime 
    ///     GetDecimal 
    ///     GetFloat 
    ///     GetGuid 
    ///     GetLong 
    ///     GetUInt 
    /// 3. Split 是 string.Split 的扩展版本.由于没有默认的按连续字符串分隔.
    ///     "hello&amp;nbsp;world".Split("&amp;nbsp;") ;    //返回 ["hello","word"]
    /// 4. TakeOutInt 实现类似 Javascript 的 parseInt 方法
    ///     "width:12px".TakeOutInt();              // 返回 12
    /// 5. HasValue 判断对象是否有值. 是  string.IsEmpty 的替换方案.
    /// 6. GetSub 是 IEnumerable&lt;TSource&gt;.Where((value,index,retval)=> return index &gt;= startIndex &amp;&amp; index &lt;=endIndex) 的替代方案
    /// 7. GetOrDefault 是取出字典值.当字典不存在时,返回默认值.而不是报错.(直接取字典值报错.)
    ///     字典是高效的,易于使用的数据结构,它是 Hashtable 的替代方案. Hashtable 需要装箱,拆箱.
    /// 8. IsIn 判断是否存在于集合中.
    ///     3.IsIn( new int[]{1,2,3,4} ) ;  等效于 new int[]{1,2,3,4}.Contains(3);
    /// 9. TrimWithPair 结队去除.
    ///     "&lt;a&gt;hello&lt;/a&gt;".TrimWithPair("&lt;a&gt;","&lt;/a&gt;") ;     //返回 hello.
    /// 10. SplitSect , SplitLine ,SplitCell 暗文,须要保证文本里都是可见字符.
    ///     当进行转义性替换时,$表示变量时,具有特殊的意义,就要用两个 $$ 表示一个 $ : 
    ///     "you cost $$:$money$"
    ///         .Replace("$$",ValueProc.SplitSect.AsString())
    ///         .FindNextNode(o=&gt;o.Replace("money", "123"))
    ///         .Replace(ValueProc.SplitSect.AsString(),"$$");
    ///         
    ///     //返回 you cost $:123
    ///<hr></hr>
    /// </pre>
    /// </remarks>
    public static partial class ValueProc
    {


        /// <summary>
        /// 执行像 StringBuilder.Append("abc").Append("def") ;
        /// StringBuilder.ReturnSelf(o=o.Append("abc")).ReturnSelf(o=>o.Append("def")) ;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="Func"></param>
        /// <returns></returns>
        public static T ReturnSelf<T>(this T obj, Action<T> Func)
        {
            Func(obj);
            return obj;
        }


        /// <summary>
        /// 用它和 default(T) 进行比较，如果等于默认值，则返回false, 否则:
        /// 
        /// 1. 时间最小值 ,返回 false
        /// 2. float 非法值 最小值 ,返回 false
        /// 3. dbouble 非法值 最小值,返回false
        /// 4. decimal 最小值, 返回 false
        /// </summary>
        /// <typeparam name="T">检测对象类型</typeparam>
        /// <typeparam name="R">当有值时，回调返回值类型</typeparam>
        /// <param name="Value"></param>
        /// <param name="HasValueFunc">当有值时，执行的回调</param>
        /// <returns></returns>
        public static R HasValue<T, R>(this T Value, Func<T, R> HasValueFunc)
        {
            if (HasValue(Value))
            {
                if (HasValueFunc != null) return HasValueFunc(Value);
            }

            return default(R);
        }

        public static R HasValue<T, R>(this T Value, Func<T, R> HasValueFunc, Func<T, R> NoValueFunc)
        {
            if (HasValue(Value))
            {
                if (HasValueFunc != null) return HasValueFunc(Value);
            }
            else
            {
                if (NoValueFunc != null) return NoValueFunc(Value);
            }

            return default(R);
        }

        /// <summary>
        /// 判断一个对象是否为 null 或 DBNull
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool IsDBNull<T>(this T Value)
        {
            if (object.Equals(Value, null)) return true;
            else
            {
                return object.Equals(Value, DBNull.Value);
            }
        }

        public static bool HasData(this ICollection collection)
        {
            if (object.Equals(collection, null)) return false;
            return collection.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Between<T>(this T value, T startValue, T endValue) where T : IComparable
        {
            if (Equals(value, null)) return false;
            if (Equals(startValue, null)) return false;
            if (Equals(endValue, null)) return false;

            if (value.CompareTo(startValue) < 0) return false;
            if (value.CompareTo(endValue) > 0) return false;
            return true;
        }

        /// <summary>
        ///	去除开头的字符串。 
        /// </summary>
        /// <param name="value">
        /// </param>
        /// <param name="removeStart">
        /// </param>
        /// <returns>
        /// </returns>
        public static string TrimStart(this string value, string removeStart)
        {
            if (value.StartsWith(removeStart))
            {
                value = value.Substring(removeStart.Length);

                if (value.StartsWith(removeStart))
                {
                    value = value.TrimStart(removeStart);
                }
            }

            return value;
        }

        /// <summary>
        /// 去除结尾的字符串
        /// </summary>
        /// <param name="value">
        /// </param>
        /// <param name="removeEnd">
        /// </param>
        /// <returns>
        /// </returns>
        public static string TrimEnd(this string value, string removeEnd)
        {
            if (value.EndsWith(removeEnd))
            {
                value = value.Remove(value.Length - removeEnd.Length);

                if (value.EndsWith(removeEnd))
                {
                    value = value.TrimEnd(removeEnd);
                }
            }

            return value;
        }

        public static string TrimWithPair(this string value, string removeStart, string removeEnd)
        {
            return TrimWithPair(value, removeStart, removeEnd, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// 结队去除，比如去除 () {} [] , 或 &lt;a>t&lt;/a> 等
        /// </summary>
        /// <param name="value">
        /// </param>
        /// <param name="removeStart">
        /// </param>
        /// <param name="removeEnd">
        /// </param>
        /// <param name="compare"></param>
        /// <returns>
        /// </returns>
        public static string TrimWithPair(this string value, string removeStart, string removeEnd,
                                          StringComparison compare)
        {
            if (value.HasValue() == false || removeStart.HasValue() == false || removeEnd.HasValue() == false)
                return value;

            value = value.Trim();
            removeStart = removeStart.Trim();
            removeEnd = removeEnd.Trim();

            if (value.StartsWith(removeStart, compare) && value.EndsWith(removeEnd, compare))
            {
                value = value.Substring(removeStart.Length);
                value = value.Remove(value.Length - removeEnd.Length);

                if (value.StartsWith(removeStart) && value.EndsWith(removeEnd))
                {
                    value = TrimWithPair(value, removeStart, removeEnd, compare);
                }
            }

            return value.Trim();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetValueOrDefault<T>(this T? value) where T : struct
        {
            if (value.HasValue) return value.Value;
            else return default(T);
        }

        /// <summary>
        /// 替换 内容有: 单引号, 双引号, 回车, -- , 大于号, 小于号,反斜线[★]
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string GetSafeValue(this string msg)
        {
            if (string.IsNullOrEmpty(msg))
                return msg;
            return
                msg.Replace("'", "＇").Replace("\"", "＂").Replace(Environment.NewLine, " ").Replace("--", "－－").Replace(
                    "\r", "　").Replace("\n", " ").Replace("<", "＜").Replace(">", "＞").Replace("\\", "＼").Trim();
        }

        //public static T GetDefault<T>()
        //{
        //    int
        //}

        /// <summary>
        /// 如果没能取出Key ， 则返回 default()， 不会报错。  
        /// </summary>
        /// <remarks>
        /// <pre>
        /// GetOrDefault 是取出字典值.当字典不存在时,返回默认值.而不是报错.(直接取字典值报错.)
        ///     字典是高效的,易于使用的数据结构,它是 Hashtable 的替代方案. Hashtable 需要装箱,拆箱.
        /// </pre>
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="theDict"></param>
        /// <param name="one"></param>
        /// <returns></returns>
        public static R GetOrDefault<T, R>(this IDictionary<T, R> theDict, T one)
        {
            if (theDict == null) return default(R);
            if (theDict.ContainsKey(one) == false)
                return default(R);
            return theDict[one];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theDict"></param>
        /// <param name="func"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <returns></returns>
        public static R GetOrDefault<T, R>(this IDictionary<T, R> theDict, Func<T, bool> func)
        {
            if (theDict == null) return default(R);
            object retVal = null;
            if (theDict.Any(o =>
                                {
                                    if (func(o.Key))
                                    {
                                        retVal = o.Value;
                                        return true;
                                    }
                                    else return false;
                                }))
            {
                return (R)retVal;
            }
            else
                return default(R);
        }


        /// <summary>
        /// 不区分大小写的比较两个字符.
        /// </summary>
        /// <param name="charValue"> </param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool EqualsNoMatter(this char charValue, char other)
        {
            if (charValue.Equals(other)) return true;
            if (char.IsLetter(charValue) && char.IsLetter(other))
            {
                return char.ToLower(charValue).Equals(char.ToLower(other));
            }
            else return false;
        }

        /// <summary>
        /// 根据种子得到某个索引的值.
        /// </summary>
        /// <remarks>
        /// <code>
        /// 把 十进制 100 , 转为 16 进制.
        /// var num100 = GetSequence("123456789abcdef", 100).PadLeft(8, 'a')
        /// </code>
        /// </remarks>
        /// <param name="seed">种子</param>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public static string GetSequence(string seed, int index)
        {
            GodError.Check(index < 0, "索引值(" + index + ")不能小于0");

            int Length = seed.Length;

            var list = new List<char>();

            int De = index;
            int Yu = 0;
            do
            {
                Yu = De % Length;
                De = De / Length;

                list.Add(seed[Yu]);
            } while (De != 0);

            list.Reverse();
            return new string(list.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetSequenceIndex(string seed, string value)
        {
            GodError.Check(value.HasValue() == false, "Value不能为空.");
            int length = seed.Length;

            var list = new List<int>();
            int retVal = 0;

            int pos = 0;
            value.Reverse().All(o =>
                                    {
                                        int index = seed.IndexOf(o);
                                        GodError.Check(index < 0, "Value的值: " + o + ",必须全部出现在Seed中.");
                                        list.Add(index);

                                        retVal += index * Math.Pow(length, pos).AsInt();
                                        pos++;
                                        return true;
                                    });

            return retVal;
        }

        /// <summary>
        /// 得到 Excel 的列名.
        /// </summary>
        /// <param name="Seed"></param>
        /// <param name="Index"></param>
        /// <returns></returns>
        public static string GetExcelSequence(string Seed, int Index)
        {
            GodError.Check(Index < 0, "索引值(" + Index + ")不能小于0");
            var chars = new List<char>();
            int Length = Seed.Length;
            do
            {
                if (chars.Count > 0) Index--;
                chars.Insert(0, Seed[Index % Length]);
                Index = (Index - Index % Length) / Length;
            } while (Index > 0);

            return chars.AsString();
        }

        public static int GetExcelSequenceIndex(string Seed, string Value)
        {
            GodError.Check(Value.HasValue() == false, "Value不能为空.");

            int Length = Seed.Length;
            double count = -1;

            for (int i = 0; i < Value.Length; i++)
            {
                count += (Seed.IndexOf(Value[i]) + 1) * Math.Pow(Length, Value.Length - 1 - i);
            }

            return count.AsInt();
        }


        //public static string Format<T>(this string Source, T JsonObject)
        //    where T : class
        //{
        //    if (JsonObject == default(T)) return Source;

        //    Type type = typeof(T);
        //    Dictionary<string, string> dict = new Dictionary<string, string>();
        //    type.GetProperties().All(o =>
        //        {
        //            dict[o.Name] = o.GetValue(JsonObject, null).AsString();
        //            return true;
        //        });
        //    return Format(Source, '{', '}', dict);
        //}


        /// <summary>
        /// Dict 连写方式 : new StringDict { { "k1","v1" },{"k2","v2"} } .
        /// </summary>
        /// <example>
        /// <code>
        /// "内容如下\n {Name1}:{Value1}, {Name2}:{Value2}".Format(new StringDict { { "Name1", "Value1" }, { "Name2", "Value2" } })
        /// </code>
        /// </example>
        /// <param name="Source"></param>
        /// <param name="JsonObject"></param>
        /// <returns></returns>
        public static string Format(this string Source, StringDict JsonObject)
        {
            return Format(Source, "{}", JsonObject);
        }

        public static string Format(this StringLinker Source, StringDict JsonObject)
        {
            return Format(Source.AsString(), JsonObject);
        }


        /// <summary>
        /// 增强型Format.避免用数字索引进行格式化.
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="LeftMark"></param>
        /// <param name="RightMar"></param>
        /// <param name="Dict"></param>
        /// <returns></returns>
        public static string Format(this string Source, string Marks, StringDict Dict)
        {
            //Source = Source.Replace("{{", SplitSect.ToString()).Replace("}}", SplitLine.ToString());
            Marks.HasValue(o => Marks = "{}");
            var LeftMark = Marks[0];
            var RightMark = LeftMark;
            if (Marks.Length > 1) RightMark = Marks[1];

            foreach (string item in Dict.Keys)
            {
                Source = Source.Replace(LeftMark + item + RightMark, Dict[item]);
            }
            return Source;
            //return Source = Source.Replace(SplitSect.ToString(), "{{").Replace(SplitLine.ToString(), "}}");
        }

        /// <summary>
        /// string.Format 
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Paras"></param>
        /// <returns></returns>
        public static string Format(this string Source, params string[] Paras)
        {
            return string.Format(Source, Paras);
        }


        public struct SimilarResult
        {
            public int s1Index { get; set; }
            public int s2Index { get; set; }
            public string Value { get; set; }
        }
        /// <summary>
        /// 获取两个字符串的最大公共部分
        /// </summary>
        /// <remarks>
        /// http://hi.baidu.com/tangguoshequ/blog/item/d587dc170878c8946538dbd1.html
        /// </remarks>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="compareWithCase">是否区分大小写</param>
        /// <returns></returns>
        public static SimilarResult[] GetMaxCommStrings(string s1, string s2, bool compareWithCase)
        {
            if (String.IsNullOrEmpty(s1) || String.IsNullOrEmpty(s2))
                return new SimilarResult[] { };
            else if (s1 == s2)
                return new SimilarResult[] { new SimilarResult() { s1Index = 0, s2Index = 0, Value = s1 } };

            var d = new int[s1.Length, s2.Length];

            var length = 0;

            for (int i = 0; i < s1.Length; i++)
            {
                for (int j = 0; j < s2.Length; j++)
                {
                    // 左上角值
                    var n = i - 1 >= 0 && j - 1 >= 0 ? d[i - 1, j - 1] : 0;

                    // 当前节点值 = "1 + 左上角值" : "0"
                    d[i, j] = (compareWithCase ? s1[i] == s2[j] : s1[i].EqualsNoMatter(s2[j])) ? 1 + n : 0;

                    // 如果是最大值，则记录该值和行号
                    if (d[i, j] > length)
                    {
                        length = d[i, j];
                    }
                }
            }

            if (length == 0) { return new SimilarResult[] { }; }
            var list = new List<SimilarResult>();
            for (int i = 0; i < s1.Length; i++)
            {
                for (int j = 0; j < s2.Length; j++)
                {
                    // 如果是最大值，则记录该值和行号
                    if (d[i, j] == length)
                    {
                        list.Add(new SimilarResult() { s1Index = i - length + 1, s2Index = j - length + 1, Value = s1.Substring(i - length + 1, length) });
                    }
                }
            }
            return list.ToArray();
        }

        public enum SimilarEnum
        {
            MaxLength = 0,
            First = 1,
            Second = 2,
        }

        /// <summary>
        /// 获取两个字符串相似度。（最大公约数法的扩展）
        /// </summary>
        /// <remarks>
        ///       人 民 共 和 时 代 华 人
        ///    中 0, 0, 0, 0, 0, 0, 0, 0
        ///    华 0, 0, 0, 0, 0, 0, 1, 0
        ///    人 1, 0, 0, 0, 0, 0, 0, 2
        ///    民 0, 2, 0, 0, 0, 0, 0, 0
        ///    共 0, 0, 3, 0, 0, 0, 0, 0
        ///    和 0, 0, 0, 4, 0, 0, 0, 0
        ///    国 0, 0, 0, 0, 0, 0, 0, 0
        /// 
        /// 得到 人民共和  和 华人 两个单词
        /// 其中 人民共和 是最大公约数
        /// 按 人民共和 把两个词条分隔。 左左， 右右，再递归求最大公约数
        /// 参照长度 默认为： Max( s1.Length + s2.Length) 
        /// 相似度 =   最大公约数.Length / 参照长度   + 
        ///            （左最大公约数/ 左参照长度   +  右最大公约数.Length / 右参照长度 ） /参照长度
        ///            
        /// 1  表示， 整体的相似度 介于 四个连字到五个连字之间。
        /// </remarks>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="compareWithCase">是否区分大小写</param>
        /// <param name="lengthType">参照算法</param>
        /// <returns></returns>
        public static double GetSimilar(string s1, string s2, bool compareWithCase, SimilarEnum lengthType)
        {
            var allLength = 1.0;

            switch (lengthType)
            {
                case SimilarEnum.First:
                    allLength = s1.Length * 1.0;
                    break;
                case SimilarEnum.Second:
                    allLength = s1.Length * 1.0;
                    break;
                default:
                    allLength = Math.Max(s1.Length, s2.Length) * 1.0;
                    break;
            }

            var allSimilar = new List<double>();

            GetMaxCommStrings(s1, s2, compareWithCase).All(o =>
                {
                    var leftSimilar = 0.0;
                    if (o.s1Index > 0 && o.s2Index > 0)
                    {
                        leftSimilar = GetSimilar(s1.Substring(0, o.s1Index), s2.Substring(0, o.s2Index), compareWithCase, lengthType);
                    }
                    var rightSimilar = 0.0;
                    if (o.s1Index + o.Value.Length < s1.Length &&
                        o.s2Index + o.Value.Length < s2.Length)
                    {
                        rightSimilar = GetSimilar(s1.Substring(o.s1Index + o.Value.Length), s2.Substring(o.s2Index + o.Value.Length), compareWithCase, lengthType);
                    }

                    var theSimilar = o.Value.Length / allLength +
                         (leftSimilar + rightSimilar) / allLength
                         ;

                    allSimilar.Add(theSimilar);

                    // return true 最准确，但是最慢。
                    return false;
                });

            if (allSimilar.Count == 0) return 0.0;
            else return allSimilar.Max();

        }

    }
}