using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;

namespace MyOql.Provider
{
    /// <summary>
    /// 数据库翻译器.类似于 DbProvider.
    /// </summary>
    public abstract partial class TranslateSql
    {
        public abstract TranslateSql NewTranslation();

        public abstract DbDataAdapter GetDataAdapter();


        //private static Dictionary<string, DbConnection> dictConn = new Dictionary<string, DbConnection>();
        public abstract DbConnection GetConnection(string ConfigName);

        /// <summary>
        /// 变量标识符
        /// </summary>
        public abstract string VarId { get; }

        public abstract string GetWithLockSql(RuleBase rule);

        

        /// <summary>
        /// 生成 Select 子句Sql语句。分页机制可能存在不同的实现。
        /// </summary>
        /// <returns></returns>
        public abstract CommandValue GetSelectText(MyCommand myCmd);

        /// <summary>
        /// 生成指定的参数.
        /// </summary>
        /// <param name="ParameterName"></param>
        /// <param name="DbType"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public abstract DbParameter GetParameter(string ParameterName, DbType DbType, object Value);
        /// <summary>
        /// 根据DbType 生成 Parameter 对象。对于Oracle，生成 OracleDbType。同时处理Value 的时间值。
        /// </summary>
        /// <param name="Column">用它的DbType，如果是复合列，生成的参数类型按 Value 取。</param>
        /// <param name="Value"></param>
        /// <returns>返回的值可能只有一个表达式，而没有 DataParameter</returns>
        public abstract CommandParameter GetParameter(ColumnClip Column, object Value);
        //{
        //    return GetParameter(myCmd, Column, Value, null);
        //}

        //public CommandParameter GetParameter(ColumnClip Column, object Value, string ParameterName)
        //{
        //    return GetParameter(null, Column, Value, ParameterName);
        //}

        //public abstract CommandParameter GetParameter(MyCommand myCmd, ColumnClip Column, object Value, string ParameterName);

        /// <summary>
        /// 为了兼容各个数据库的自增.各数据库必须各自实现该方法 .
        /// </summary>
        /// <param name="myCmd"></param>
        protected abstract void ToInsertCommand(MyCommand myCmd);

        /// <summary>
        /// 生成Select子句.
        /// </summary>
        /// <param name="myCmd"></param>
        public abstract void ToSelectCommand(MyCommand myCmd);


        /// <summary>
        /// 批量插入,Context 里有两种Model , 一种是 MyOqlSet , 另一种是 Select 子句.
        /// </summary>
        /// <param name="myCmd"></param>
        protected abstract void ToBulkInsertCommand(MyCommand myCmd);




        ///// <summary>
        ///// 取别名.
        ///// </summary>
        ///// <param name="Alias"></param>
        ///// <returns></returns>
        //public abstract string GetAliasText(string Alias);


        /// <summary>
        /// 取类型映射字典.
        /// </summary>
        /// <returns></returns>
        //public abstract IEnumerable<TypeMap> GetTypeMap();

        //public abstract void GenFullSql(MyCommand myCmd);

        public abstract string GenSql(RuleBase rule);
    }
}
