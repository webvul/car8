using System;

namespace MyOql
{
    [Flags]
    public enum MyOqlRuleEnum
    {
        /// <summary>
        /// 实体表。
        /// </summary>
        Table = 1,

        /// <summary>
        /// 视图
        /// </summary>
        View = 2,

        /// <summary>
        /// 表变量
        /// </summary>
        VarTable = 4,

        /// <summary>
        /// 临时表
        /// </summary>
        TempTable = 8,
    }
}
