
using System.Collections.Generic;
namespace MyOql
{
    ///<summary>
    /// 
    ///</summary>
    /// <remarks>
    ///  
    /// </remarks>
    public interface IDbr
    {
        ///// <summary>
        ///// 获取破坏缓存表的视图
        ///// </summary>
        ///// <returns></returns>
        //string[] GetViewsWithDestroyCache();



        /// <summary>
        /// 取 视图关联的表
        /// </summary>
        /// <param name="DbName"> 视图名 的数据库名称</param>
        /// <returns>Key 是视图的数据库名，Value 是关联表的数据库名</returns>
        Dictionary<string, string[]> GetViewRelationTables();


        /// <summary>
        /// 取 存储过程 关联的表
        /// </summary>
        /// <param name="ProcDbName">存储过程 的数据库名称</param>
        /// <returns>Key 是存储过程的数据库名，Value 是关联表的数据库名</returns>
        Dictionary<string, string[]> GetProcRelationTables();

        /// <summary>
        /// 根据数据库名称取 表 或 视图
        /// </summary>
        /// <param name="DbName">可以是表名,视图名,</param>
        /// <returns></returns>
        RuleBase GetMyOqlEntity(string DbName);

        /// <summary>
        /// 获取所有表的 外键定义信息
        /// </summary>
        /// <returns></returns>
        List<FkColumn> GetFkDefines();
    }
}
