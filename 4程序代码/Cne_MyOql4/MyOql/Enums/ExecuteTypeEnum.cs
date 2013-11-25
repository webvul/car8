
namespace MyOql
{
    /// <summary>
    /// 数据执行方式.
    /// </summary>
    public enum ExecuteTypeEnum
    {
        /// <summary>
        /// 有返回值,用专用连接.
        /// </summary>
        Select = 1,

        /// <summary>
        /// 有分页结果 , 使用了 MyOqlConfigScope.DataReader
        /// </summary>
        SelectWithSkip = 2,
        /// <summary>
        /// 直接执行.可以用共享连接.
        /// </summary>
        Execute = 4,
    }
}
