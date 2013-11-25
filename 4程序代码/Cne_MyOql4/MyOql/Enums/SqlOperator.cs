using System;

namespace MyOql
{
    /// <summary>
    /// Sql 操作符 , 1024以上属自定义函数 。
    /// </summary>
    [Serializable]
    public enum SqlOperator
    {

        #region 第一部分： 关键字。
        Blank = 1, //表示空白。不同于 0
        
        Enter , //回车

        /// <summary>
        /// 字符串, 表达式 连接符.
        /// </summary>
        Concat,
        /// <summary>
        /// 加法
        /// </summary>
        Add,
        /// <summary>
        /// 减法
        /// </summary>
        Minus,
        /// <summary>
        /// 乘法
        /// </summary>
        Multiple,
        /// <summary>
        /// 除法
        /// </summary>
        Divided,

        /// <summary>
        /// 列别名已转移到 列的 Name 上。
        /// </summary>
        As,
        /// <summary>
        /// And 关键字
        /// </summary>
        And,
        /// <summary>
        /// Or 关键字
        /// </summary>
        Or,
        /// <summary>
        /// 二进制 And 关键字
        /// </summary>
        BitAnd,
        /// <summary>
        /// 二进制 Or 关键字
        /// </summary>
        BitOr,
        /// <summary>
        /// 判断相等
        /// </summary>
        Equal,
        /// <summary>
        /// 判断 大于
        /// </summary>
        BigThan,
        /// <summary>
        /// 判断 小于
        /// </summary>
        LessThan,
        /// <summary>
        /// 判断 大于等于
        /// </summary>
        BigEqual,
        /// <summary>
        /// 判断 小于等于
        /// </summary>
        LessEqual,
        /// <summary>
        /// 判断 不等于
        /// </summary>
        NotEqual,
        /// <summary>
        /// In 
        /// </summary>
        In,
        NotIn,
        Like,
        /// <summary>
        /// 取模
        /// </summary>
        Mod,
        /// <summary>
        ///  
        /// </summary>
        CaseWhen,
        /// <summary>
        /// 只帮助生成SQL用。
        /// </summary>
        WhenThen,
        /// <summary>
        /// 只帮助生成SQL
        /// </summary>
        Else,

        Between,
        AndForBetween,

        Cast,
        IsNull,

        Union,
        UnionAll,
        #endregion


        #region 第二部分： 字符串函数
        /// <summary>
        ///  长度函数，是字符长度。Len(MsSql)、char_Length(MySql)
        /// </summary>
        Len,
        /// <summary>
        /// 表示 DataLength(MsSql)、 函数
        /// </summary>
        SizeOf,

        /// <summary>
        /// 表示 Left 函数
        /// </summary>
        Left,

        /// <summary>
        /// 表示 Right 函数
        /// </summary>
        Right,
        /// <summary>
        /// 表示 反转函数
        /// </summary>
        Reverse,

        /// <summary>
        /// 表示 AscII 函数
        /// </summary>
        AscII,

        /// <summary>
        /// 表示 Unicode  函数
        /// </summary>
        Unicode,

        /// <summary>
        /// 表示 Char 函数。
        /// </summary>
        Char,

        /// <summary>
        /// 表示 NChar 函数
        /// </summary>
        NChar,

        /// <summary>
        /// 生成一个 复合列表达式.
        /// </summary>
        StringIndex,
        SubString,

        /// <summary>
        /// 表示 Stuff 函数
        /// </summary>
        Stuff,

        /// <summary>
        /// 表示 PatIndex 函数
        /// </summary>
        PatIndex,

        /// <summary>
        /// 表示 Replace 函数
        /// </summary>
        Replace,
        /// <summary>
        /// 是否是数字。
        /// </summary>
        IsNumeric,

        Ltrim,
        Rtrim,
        Trim,
        #endregion

        #region 第三部分： 聚合函数
        /// <summary>
        /// Count(column) 函数
        /// </summary>
        Count,

        /// <summary>
        /// Count( distinct column ) 函数 
        /// </summary>
        CountDistinct,

        /// <summary>
        /// sum(column) 函数
        /// </summary>
        Sum,
        /// <summary>
        /// Max 函数
        /// </summary>
        Max,
        Min,
        Avg,
        #endregion

        #region 第四部分：时间函数

        /// <summary>
        /// 表示 IsDate 函数
        /// </summary>
        IsDate,
        Year,
        Month,
        Day,
        DateDiff,
        DateAdd,
        #endregion

        #region 第五部分： 其它
        /// <summary>
        /// 属性连接符，即点号
        /// </summary>
        Property,

        /// <summary>
        /// 限定符，即 []
        /// </summary>
        Qualifier,
        /// <summary>
        /// 连接SQL
        /// </summary>
        ConcatSql,

        /// <summary>
        /// 当前要使用的自增值.(插入记录要返回的自增值)
        /// </summary>
        CurrentIdentity,

        /// <summary>
        /// 表示 Coalesce函数，返回第一个非空值,2个参数
        /// </summary>
        Coalesce2,
        Coalesce3,
        Coalesce4,
        Coalesce5,

        /// <summary>
        /// 参数连接符，即逗号 
        /// </summary>
        Parameter,
        #endregion

    }


    /// <summary>
    /// 描述值的类型的枚举
    /// </summary>
    public enum ValueMetaTypeEnum
    {
        /// <summary>
        /// Raw 所使用的,表示 参数内容的类型是 DbType,下同。
        /// </summary>
        DbType,
        EnumType,
        NumberType,
        StringType,
        /// <summary>
        /// Raw 所使用的,表示 参数内容的类型是 SqlClipBase。
        /// </summary>
        MyOqlType,
    }
}
