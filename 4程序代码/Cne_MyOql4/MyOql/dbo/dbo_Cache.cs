using System;
using MyCmn;
using System.Linq;

namespace MyOql
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 对于外部系统如存储过程更新或删除数据.要用触发器更新元数据表. 这样为了让程序及时得到缓存破坏信息. 
    /// 在 dbo.OnReadingRow 方法前,检测元数据表.如果元数据表标识有变, 则提前清除缓存.
    /// 缓存只针对单表单记录读取.
    /// 
    /// <code>
    /// 元数据表:
    /// 触发器:
    /// Oracle:
    /// Sqlserver:
    /// </code>
    /// </remarks>
    public partial class dbo
    {
        public static IDboEvent Event { get; set; }


        /// <summary>
        /// 根据 表名，找出和该表有关系的视图
        /// </summary>
        /// <param name="TableDbName">表的数据库名</param>
        /// <returns>和该表有关系的视图</returns>
        public static string[] GetRelationViewsByTable(string TableDbName)
        {
            return Event.Idbr.GetViewRelationTables()
                .Where(o => o.Value.Contains(TableDbName))
                .Select(o => o.Key)
                .ToArray();
        }

        //public static bool HasValue(this DictListModel Model)
        //{
        //    if (Model == null) return false;
        //    if (Model.Data == null) return false;
        //    if (Model.Data.Count == 0) return false;
        //    return true;
        //}
    }
}
