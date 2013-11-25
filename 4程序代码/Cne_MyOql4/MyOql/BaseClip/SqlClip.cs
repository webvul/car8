using System;
using MyCmn;

namespace MyOql
{

    /// <summary>
    /// SqlClip 表示一个 Sql 语句段的基类。如From 子句 ， Where 子句 。
    /// 其中包含 ContextClipBase 表示一个Sql动作。如 Select 子句，Insert 子句，Update 子句。
    /// MyOql 内部存存结构即是很多不同的 SqlClip。
    /// </summary>
    [Serializable]
    public abstract class SqlClipBase : ICantToValueType ,ICloneable
    {
        //[DataMember]
        /// <summary>
        /// Sql语句段的关键字
        /// </summary>
        public SqlKeyword Key { get; protected set; }

        public SqlClipBase()
        {
        }

        public SqlClipBase(SqlKeyword Key)
        {
            this.Key = Key;
        }

        public bool IsColumn()
        {
            return (int)this.Key < 5;
        }

        public abstract object Clone();
    }
}
