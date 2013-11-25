
using System;


namespace MyOql
{
    /// <summary>
    /// 配置MyOql作用域
    /// </summary>
    [Flags]
    public enum ReConfigEnum
    {
        SkipPower = 0x1,
        SkipCache = 0x2,
        SkipLog = 0x4,


        /// <summary>
        /// 设置不对数据库名进行转义。
        /// </summary>
        NoEscapeDbName = 0x10,

        /// <summary>
        /// 脏读模式，不读取未提交信息。
        /// </summary>
        ReadPast = 0x20,

        /// <summary>
        /// 脏读模式，读取未提交信息
        /// </summary>
        NoLock = 0x40,

        /// <summary>
        /// 默认的等待事务完成模式。
        /// </summary>
        WaitLock = 0x80,

        /// <summary>
        /// 使用 RowNumber 分页
        /// </summary>
        RowNumber = 0x100,

        /// <summary>
        /// 使用 DataReader 分页
        /// </summary>
        DataReader = 0x200,
    }
}
