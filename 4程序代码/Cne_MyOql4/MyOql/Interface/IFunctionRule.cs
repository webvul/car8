
namespace MyOql
{
    ///<summary>
    ///表示是函数的定义，不能插入、更新、删除。
    ///</summary>
    /// <remarks>
    /// ITableRule 与 IViewRule， IFunctionRule，IProcRule 互斥。
    /// </remarks>
    public interface IFunctionRule
    {
        string[] GetParameters() ;
        object GetParameterValue(string Parameter);
    }
}
