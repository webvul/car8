
namespace MyOql
{
    /// <summary>
    /// 查询选项.如果有分页查询,则有效.
    /// </summary>
    public enum SelectOption
    {
        /// <summary>
        /// 默认值.在存在 Take 或 Skip 选项时,执午额外的查询获取总记录数.
        /// </summary>
        WithCount = 1 ,
        NoCount,
    }


    public enum ReaderPositionEnum
    {
        BeforeSkip = 1 ,
        InTake,
        AfterTake,
    }
}
