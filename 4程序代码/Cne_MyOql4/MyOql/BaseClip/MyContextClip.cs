using System;
using System.Data.Common;

namespace MyOql
{

    /// <summary>
    /// 绝对无用.自定义实例化  ContextClipBase。
    /// </summary>
    public sealed class MyContextClip : ContextClipBase
    {
        private MyContextClip() { }
        public static MyContextClip CreateByKey(SqlKeyword Key)
        {
            return new MyContextClip() { Key = Key };
        }
        
        public static MyContextClip CreateBySelect(RuleBase rule)
        {
            return new MyContextClip() { Key = SqlKeyword.Select, CurrentRule = rule };
        }

        public MyContextClip(ContextClipBase Context, RuleBase currentRule)
        {
            this.Key = Context.Key;
            this.AffectRow = Context.AffectRow;
            this.ClearedAllCacheTable = Context.ClearedAllCacheTable;
            this.ClearedSqlCacheTable = Context.ClearedSqlCacheTable;
            this.CurrentRule = currentRule;
            this.ParameterColumn = Context.ParameterColumn;
        }

        public MyContextClip UseTransaction(DbTransaction Tran)
        {
            this.Transaction = Tran;
            return this;
        }

        public MyContextClip UseConnection(DbConnection Conn)
        {
            this.Connection = Conn;
            return this;
        }

        public override int Execute(MyCommand myCmd)
        {
            return ExecuteBase(myCmd);
        }

        public override bool ExecuteReader(MyCommand myCmd, params Func<DbDataReader, bool>[] func)
        {
            return ExecuteReaderBase(myCmd, func);
        }
    }
}
