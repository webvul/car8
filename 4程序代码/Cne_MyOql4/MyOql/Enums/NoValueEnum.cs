using System;

namespace MyOql
{
    /// <summary>
    /// Sql 操作符
    /// </summary>
    [Serializable]
    public enum NoValueEnum
    { 
        /// <summary>
        /// 不返回任何语句
        /// </summary>
        NoValue = 1,

        /// <summary>
        /// 忽略长度为0的数组，不生成该语句
        /// </summary>
        Ignore  =2,
    }
}
