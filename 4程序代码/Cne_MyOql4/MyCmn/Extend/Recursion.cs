using System;
using System.Collections.Generic;

namespace MyCmn
{
    /// <summary>
    /// 递归返回类型.
    /// </summary>
    public enum RecursionReturnEnum
    {
        /// <summary>
        /// 继续
        /// </summary>
        Go,

        /// <summary>
        /// 停止向下执行递归.
        /// </summary>
        StopSub,


        /// <summary>
        /// 终止递归.
        /// </summary>
        Abord,
    }

    public class Recursion<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Container"></param>
        /// <param name="Subs"></param>
        /// <param name="Exec"></param>
        /// <returns></returns>
        public bool Execute(IEnumerable<T> Container, Func<T, IEnumerable<T>> Subs, Func<T, RecursionReturnEnum> Exec)
        {
            foreach (T item in Container)
            {
                var ret = Exec(item);

                if (ret == RecursionReturnEnum.StopSub) continue;
                else if (ret == RecursionReturnEnum.Abord) return false;

                if (Execute(Subs(item), Subs, Exec) == false)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 递归执行
        /// </summary>
        /// <param name="Container"></param>
        /// <param name="Subs"></param>
        /// <param name="Exec"></param>
        /// <param name="InitLevel">初始Level</param>
        /// <returns></returns>
        public bool Execute(IEnumerable<T> Container, Func<T, IEnumerable<T>> Subs, Func<T, int, RecursionReturnEnum> Exec, int InitLevel = 0)
        {
            foreach (T item in Container)
            {
                var ret = Exec(item, InitLevel);

                if (ret == RecursionReturnEnum.StopSub) continue;
                else if (ret == RecursionReturnEnum.Abord) return false;

                if (Execute(Subs(item), Subs, Exec, InitLevel + 1) == false)
                    return false;
            }
            return true;
        }

        public R Get<R>(IEnumerable<T> Container, Func<T, IEnumerable<T>> Subs, Func<T, R> Exec)
        {
            foreach (T item in Container)
            {
                R retVal = Exec(item);
                if (retVal.HasValue()) return retVal;
                retVal = Get(Subs(item), Subs, Exec);
                if (retVal.HasValue())
                    return retVal;

            }
            return default(R);
        }
    }
}
