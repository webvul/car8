using System.Collections.Generic;
using MyOql.Provider;

namespace MyOql
{
    public interface IOperator
    {
        SqlOperator Operator { get; set; }
    }

    public class OperatorContext : IOperator
    {
        public SqlOperator Operator
        {
            get;
            set;
        }

        public OperatorContext(SqlOperator Operator)
        {
            this.Operator = Operator;
        }
    }
}
