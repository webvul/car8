
using System.Collections.Generic;
using System;
using System.Linq;

namespace MyCmn
{
    public static partial class ValueProc
    {
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> Comparer)
        {
            return source.Distinct(new CommonEqualityComparer<T>(Comparer));
        }

        /// <summary>
        /// 给数组添加一个值。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="data"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> AddOne<TSource>(this IEnumerable<TSource> data, TSource Value)
        {
            if (data == null) return new TSource[1] { Value };
            return data.Concat(new TSource[1] { Value });
        }

        /// <summary>
        /// 集合减法.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="data"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> Minus<TSource>(this IEnumerable<TSource> data, IEnumerable<TSource> other)
        {
            if (other != null)
            {
                return data.Where(o => (o.IsIn(other.ToArray()) == false));
            }
            else return data;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="data"></param>
        /// <param name="otherFunc">返回true 表示相等，要减去。返回false 不减。</param>
        /// <returns></returns>
        public static IEnumerable<TSource> Minus<TSource>(this IEnumerable<TSource> data, Func<TSource, bool> otherFunc)
        {
            if (otherFunc != null)
            {
                return data.Where(o => !otherFunc(o));
            }
            else return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="equalFunc"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static bool Contains<TSource>(this IEnumerable<TSource> data, Func<TSource, bool> equalFunc)
        {
            return !data.All(o => !equalFunc(o));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TSource> Slice<TSource>(this IEnumerable<TSource> source, int startIndex)
        {
            return Slice<TSource>(source, startIndex, int.MaxValue);
        }

        /// <summary>
        /// 
        ///类似于 javascript.slice 。 取出集合的一部分 new int[1,2,3].Slice(0,2) 返回 [1,2]
        /// </summary>
        /// <remarks>
        /// slice 方法一直复制到 end 所指定的元素，但是不包括该元素。
        /// 如果 start 为负，将它作为 length + start处理，此处 length 为数组的长度。
        /// 如果 end 为负，就将它作为 length + end 处理，此处 length 为数组的长度。
        /// 如果省略 end ，那么 slice 方法将一直复制到 arrayObj 的结尾。
        /// 如果 end 出现在 start 之前，不复制任何元素到新数组中。
        /// 示例:
        /// <code>
        ///  new int[1,2,3].Slice(0,-1) 返回 1,2
        ///  new int[1,2,3].Slice(1,0)  返回 空.
        ///  new int[1,2,3].Slice(-100,2) 返回 1,2
        ///  new int[1,2,3].Slice(-2,-1) 返回 3
        /// </code>
        /// </remarks>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="startIndex"> 表示要截取的字符串的开始索引 . 如果 start 为负，将它作为 length + start处理，此处 length 为数组的长度。 </param>
        /// <param name="endIndex">
        /// 表示要截取的字符串的结束索引,不包括该元素.
        /// 如果 end 为负，就将它作为 length + end 处理，此处 length 为数组的长度。
        /// 如果省略 end ，那么 slice 方法将一直复制到 arrayObj 的结尾。
        /// 如果 end 出现在 start 之前，不复制任何元素到新数组中。
        /// </param>
        /// <returns></returns>
        public static IEnumerable<TSource> Slice<TSource>(this IEnumerable<TSource> source, int startIndex, int endIndex)
        {
            if (startIndex < 0) return Slice<TSource>(source, source.Count() + startIndex, endIndex);
            if (endIndex < 0) return Slice<TSource>(source, startIndex, source.Count() + endIndex);

            return source.Where((value, index) =>
            {
                return index >= startIndex && index < endIndex;
            });
        }
    }
}
