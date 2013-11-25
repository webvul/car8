
namespace MyOql
{
    ///<summary>
    ///表示是否是可以  As 的对象。 表，字段，子查询，都可以 As
    ///</summary>
    /// <remarks>
    /// </remarks>
    public interface IAsable
    {
        /// <summary>
        /// 使用 SetAlias 之前，需要对对象进行克隆。
        /// </summary>
        /// <param name="Alias"></param>
        void SetAlias(string Alias);
        string GetAlias();
    }
}
