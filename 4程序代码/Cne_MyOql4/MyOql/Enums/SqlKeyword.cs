using System;

namespace MyOql
{
    /// <summary>
    /// Sql 关键字。
    /// </summary>
    [Serializable]
    public enum SqlKeyword
    {
        /// <summary>
        /// 简单列
        /// </summary>
        Simple = 1,

        /// <summary>
        /// 复合列
        /// </summary>
        Complex,

        /// <summary>
        /// 原始列
        /// </summary>
        Raw,

        /// <summary>
        /// 常数列
        /// </summary>
        Const,

        //None,
        Select,
        Update,
        Delete,
        Insert,
        BulkInsert,
        //BulkUpdate,


        Expresstion,
        LeftJoin,
        RightJoin,
        Join,
        OnWhere,
        Where,
        GroupBy,
        OrderBy,
        Having,
        Distinct,
        Take,
        Func,
        Proc,
        ProcName,
        ProcParameter,
        Skip,

        /// <summary>
        /// 表示 Model
        /// </summary>
        Model,

        ///// <summary>
        ///// 它不能单独存在，它存放在 ColumnClip 的 Extend 中
        ///// </summary>
        //Complex,
        MyOqlSet,
        As,
        ProcVar,
    }
}
