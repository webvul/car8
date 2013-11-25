using System;
using System.Data.Common;
using MyCmn;

namespace MyOql
{

    /// <summary>
    /// 不同的数据库进行解析Sql 时，返回的对象。
    /// </summary>
    public class MyCommand
    {
        /// <summary>
        /// 返回的 DbCommand ，里面有 CommandText , Parameters , Connection
        /// </summary>
        public DbCommand Command { get; set; }

        /// <summary>
        /// 当前动作子句。
        /// </summary>
        public ContextClipBase CurrentAction { get; set; }

        /// <summary>
        /// 数据执行方式，仅在是最外层 ContextClip 的 MyCommand 中有效。
        /// </summary>
        public ExecuteTypeEnum ExecuteType { get; set; }

        /// <summary>
        /// 仅在MyOql 内部使用.如果是插入的话, 返回的自增ID. 仅对数据库不能直接返回自增的情况有效. 
        /// </summary>
        /// <remarks>
        /// 对于一些OleDb数据库如 Excel, 不能执行多语句,也不能返回自增ID . 所以需要提前确定自增ID的值.
        /// 如果该值 大于 0 , 则使用该值返回结果 . 
        /// </remarks>
        public long LastAutoID { get; set; }

        private string _FullSql;
        public string FullSql
        {
            get
            {
                if (string.IsNullOrEmpty(_FullSql))
                {
                    _FullSql = dbo.GetFullSql(this.Command);
                }
                return _FullSql;
            }
            set
            {
                _FullSql = value;
            }
        }


        public MyCommand()
        {
            //this.ParameterColumn = new List<CommandParameter>();
        }
    }
}
