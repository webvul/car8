
namespace MyOql
{
    ///<summary>
    ///表示是视图的定义，不能插入、更新、删除。
    ///</summary>
    /// <remarks>
    /// ITableRule 与 IViewRule， IFunctionRule，IProcRule 互斥。
    /// </remarks>
    public interface IViewRule
    {
    }
}
