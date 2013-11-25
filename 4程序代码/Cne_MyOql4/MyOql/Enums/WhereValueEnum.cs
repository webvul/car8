
namespace MyOql
{
    /// <summary>
    /// Where表达式值的五种类型.
    /// </summary>
    public enum WhereValueEnum
    {
        /// <summary>
        /// 值类型，如：字符串，数字，时间，GUID，Null
        /// </summary>
        Value = 1,

        /// <summary>
        /// 值数组，用于 In,NotIn,Between 子句。
        /// </summary>
        ValueArray,

        /// <summary>
        /// 列类型。
        /// </summary>
        Column,

        ///// <summary>
        ///// 复合列类型。
        ///// </summary>
        //Complex,

        /// <summary>
        /// 子查询类型，用于 In,NotIn
        /// </summary>
        SubQuery,
    }
}
