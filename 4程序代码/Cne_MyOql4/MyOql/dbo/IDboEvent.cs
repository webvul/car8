using System;
using MyCmn;
using System.Collections.Generic;

namespace MyOql
{
    /// <summary>
    /// 缓存项包括: 根据id 缓存, 根据SQL 缓存 两种形式.
    /// </summary>
    /// <remarks>
    /// 术语定义：
    /// VCId  关联视图的根据Id的缓存项（仅一条）
    /// VCSql 关联视图的根据 SQL 的缓存项
    /// VCAll 关联视图的所有缓存项（所有根据Id缓存项 和 所有根据SQL缓存项）
    /// TCId  根据Id缓存表的缓存项 （仅一条）
    /// TCSql 根据SQL 缓存表的缓存项
    /// TCAll 该表的所有缓存项（所有根据Id缓存项 和 所有根据SQL缓存项）
    /// 
    /// 在 添加，删除，更新，执行存储过程时，清除相关联的缓存
    /// 添加：清除 TCSql ， VCAll
    /// 删除：清除 TCId ，TCSQL ，VCAll
    /// 更新：清除 TCId ， TCSQL，VCAll
    /// 存储过程： 清除 TCAll , VCAll
    /// 
    /// 分析得出： 
    /// 不会出现 单独清除 VCId ,VCSql 的情况。
    /// 清除 TCSql 时，一定要清除 VCAll
    /// 清除 TCAll 时，一定要清除 VCAll
    /// </remarks>
    public abstract partial class IDboEvent
    {
        public abstract IDbr Idbr { get; set; }
        /// <summary>
        /// 创建前事件。参数是：当前实体，创建的列，创建的Model，返回 DoingResult
        /// </summary>
        /// <remarks></remarks>
        public event Func<ContextClipBase, bool> Creating = null;

        ///// <summary>
        ///// 创建后事件。参数是：当前实体，创建的Model。
        ///// </summary>
        ///// <remarks>主键信息包含在Model 中</remarks>
        //public  event Action<RuleBase, ModelClip> Created = null;

        /// <summary>
        /// 读取前事件。参数是：当前实体，读取的Id值(Id值表示法，<see cref="WhereClip.GetIdValue"/>)，读取的列，返回值。
        /// </summary>
        public event Func<ContextClipBase, bool> Reading = null;

        ///// <summary>
        ///// 读取后事件。参数是：当前实体，读取Id的值(Id值表示法，<see cref="WhereClip.GetIdValue"/>)。
        ///// </summary>
        //public  event Action<RuleBase, string> Readed = null;

        /// <summary>
        /// 更新前事件.参数为:当前实体,要更新的Id值(Id值表示法，<see cref="WhereClip.GetIdValue"/>),更新的列，更新的Model，返回值。
        /// </summary>
        public event Func<ContextClipBase, bool> Updating = null;

        /// <summary>
        /// 更新后事件。参数是：当前实体，更新的Id值(Id值表示法，<see cref="WhereClip.GetIdValue"/>)，更新记录数，更新Model。
        /// </summary>
        /// <remarks>要更新的Id值，如果为null，则可能更新多行。</remarks>
        //public  event Action<RuleBase, string, int, ModelClip> Updated = null;

        /// <summary>
        /// 删除前事件。参数是：当前实体，要删除的Id值(Id值表示法，<see cref="WhereClip.GetIdValue"/>)，返回是否允许。
        /// </summary>
        /// <remarks>要删除的Id值，如果为null，则可能更新多行。</remarks>
        public event Func<ContextClipBase,  bool> Deleting = null;

        ///// <summary>
        ///// 删除后事件。参数是：当前实体，已删除的Id值(Id值表示法，<see cref="WhereClip.GetIdValue"/>)，删除记录数。
        ///// </summary>
        //public  event Action<RuleBase, string, int> Deleted = null;

        /// <summary>
        /// 生成SQL事件。参数是：生成SQL的数据库类型，当前命令。
        /// </summary>
        public event Action<DatabaseType, MyCommand> GenerateSqled = null;

        /// <summary>
        /// 存储过程执行前事件。参数是：当前实体，参数字典。
        /// </summary>
        public event Func<ContextClipBase, bool> Procing = null;

        ///// <summary>
        ///// 存储过程执行后事件。参数是：当前实体。
        ///// </summary>
        //public  event Action<RuleBase> Proced = null;

        /// <summary>
        /// 批量插入前事件。参数是：当前实体，插入的数据集，返回值。
        /// </summary>
        public event Func<ContextClipBase, bool> BulkInserting = null;

        /// <summary>
        /// 批量更新前事件。参数是：当前实体，更新的数据集，返回值。
        /// </summary>
        public event Func<ContextClipBase, bool> BulkUpdating = null;

        public event Func<ContextClipBase, bool> Executing = null;
    }
}
