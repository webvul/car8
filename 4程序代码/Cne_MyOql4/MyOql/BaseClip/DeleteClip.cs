using System;
using System.Data.Common;
using System.Linq;

namespace MyOql
{
    /// <summary>
    /// Delete 子句。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class DeleteClip<T> : ContextClipBase
        where T : RuleBase
    {
        public DeleteClip()
        {
            this.Key = SqlKeyword.Delete;
        }

        /// <summary>
        /// Delete 子句的 Where 条件 
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public DeleteClip<T> Where(Func<T, WhereClip> func)
        {
            AppendWhere(func.Invoke((T)(object)this.CurrentRule));
            return this;
        }

        /// <summary>
        /// 执行语句
        /// </summary>
        /// <returns>返回影响行数</returns>
        public int Execute()
        {
            if (dbo.Event.OnDeleting(this) == false)
                return 0;

            return Execute(this.ToCommand());
        }

        /// <summary>
        /// 执行语句。
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>返回影响行数</returns>
        public override int Execute(MyCommand cmd)
        {
            if (cmd == null) return 0;
            var retVal = ExecuteBase(cmd);

            dbo.Event.OnDeleted(this);
            return retVal;
        }


        /// <summary>
        /// 不会执行 破坏缓存，不会执行 OnDeleted  方法
        /// </summary>
        /// <param name="myCmd"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public override bool ExecuteReader(MyCommand myCmd, params Func<DbDataReader, bool>[] func)
        {
            var retVal = ExecuteReaderBase(myCmd, func);
            return retVal;
        }
    }
}
