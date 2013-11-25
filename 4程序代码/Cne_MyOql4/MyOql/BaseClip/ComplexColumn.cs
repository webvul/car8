using System;
using System.Linq;
using System.Data;
using MyCmn;
using System.Xml.Serialization;
using System.Xml;

namespace MyOql
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 不能使用 ColumnClip[].Contains ，因为重载了 ==
    /// 对于 Operator 的SQL关键字来说，比如： cast；或标准的SQL 函数来说，比如： IsNull , 也可以使用 复合列来表示。但仅限于只有两个列和一个sql关键字的形式。
    /// 如： 
    /// cast( col as varchar(8000))   ,这个可以是因为，cast 后只能跟as，而不能跟其它关键字。使用者可以调用  Column.CastAs(DbType.AnsiString) ,而不必传递 varchar(30) , 系统自动把它转换为: cast( column, varchar(8000) ) 
    /// isnull( col , '')
    /// 
    /// 对于 参数是 DbType 的来说， 系统只能把它转换为看似最合理的字符串格式。见：<see cref="TranslateSql.ToSqlType"/>
    /// </remarks>
    [Serializable]
    public partial class ComplexColumn : ColumnClip, IOperator
    {
        public ComplexColumn(string Name)
            : this()
        {
            // TODO: Complete member initialization
            this.Name = Name;
        }

        public ComplexColumn()
        {
            this.Key = SqlKeyword.Complex;
            //this.Extend = new ColumnExtend();
            // TODO: Complete member initialization
        }

        public ComplexColumn(SqlOperator Operator)
            : this()
        {
            this.Operator = Operator;
        }

        /// <summary>
        /// 表达式左数。
        /// </summary>
        public ColumnClip LeftExp { get; set; }

        /// <summary>
        /// 表达式操作符
        /// </summary>
        public SqlOperator Operator { get; set; }

        /// <summary>
        /// 表达式右数。
        /// </summary>
        public ColumnClip RightExp { get; set; }



        /// <summary>
        /// 递归所有叶子列.返回 false 停止
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        public bool Recusion(Func<SimpleColumn, bool> Func)
        {
            //改变 col 不会改变 this 变量。  udi test .
            if (this.LeftExp.IsComplex() == false)
            {
                if (Func(this.LeftExp as SimpleColumn) == false) return false;
            }

            if (this.RightExp.IsComplex() == false)
            {
                if (Func(this.RightExp as SimpleColumn) == false) return false;
            }

            if ((this.LeftExp as ComplexColumn).Recusion(Func) == false) return false;
            if ((this.RightExp as ComplexColumn).Recusion(Func) == false) return false;

            return true;
        }

        protected override object CloneClip()
        {
            var clone = new ComplexColumn();
            clone.Name = this.Name;
            clone.DbType = this.DbType;
            clone.Length = this.Length;
            clone.TableName = this.TableName;

            if (this.LeftExp.IsDBNull() == false)
            {
                clone.LeftExp = this.LeftExp.Clone() as ColumnClip;
            }

            clone.Operator = this.Operator;

            if (this.RightExp.IsDBNull() == false)
            {
                clone.RightExp = this.RightExp.Clone() as ColumnClip;
            }

            return clone;
        }

        public override ColumnName GetFullName()
        {
            if (this.TableName == null) return new ColumnName() { Column = Name };

            return new ColumnName() { Entity = TableName, Column = Name };
        }

        public override ColumnName GetFullDbName()
        {
            return new ColumnName() { Entity = TableName, Column = Name };
        }


    }
}
