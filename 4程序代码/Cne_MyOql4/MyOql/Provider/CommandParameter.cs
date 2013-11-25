using System.Data.Common;

namespace MyOql.Provider
{
    public class CommandParameter
    {
        public DbParameter Parameter { get; set; }

        //public CommandParameter(DbParameter para, string VarID)//, int ParaIndex)
        //{
        //    this.Parameter = para;
        //    //this.CurrentIndex = ParaIndex;
        //    this.Expression = VarID + "p" + this.CurrentIndex.ToString();
        //    this.Parameter.ParameterName = this.Expression;
        //}

        public CommandParameter(DbParameter para, ColumnClip Column, string Expression)
        {
            this.Column = Column;
            this.Parameter = para;
            this.Expression = Expression;
            this.Parameter.ParameterName = this.Expression;
        }


        /// <summary>
        /// 可能是一个 类型转换表达式.
        /// </summary>
        public string Expression { get; set; }

        //public int CurrentIndex { get; set; }

        /// <summary>
        /// 该参数映射到哪个ColumnClip
        /// </summary>
        public ColumnClip Column { get; set; }
    }
}
