using System;
using System.Linq;
using System.Data;
using MyCmn;

namespace MyOql
{
    public partial class ColumnClip
    {
        private static object TranslateSuitValue(DbType suitDbType, object val)
        {
            if (suitDbType == DbType.Object) return val;
            if (val.IsDBNull()) return val;

            var cc = val as ColumnClip;
            if (!cc.EqualsNull()) return val;

            return ValueProc.AsType(suitDbType.GetCsType(), val);
        }


        /// <summary>
        /// 生成 column = value 表达式。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static WhereClip operator ==(ColumnClip left, object right)
        {
            WhereClip where = new WhereClip();
            where.Operator = SqlOperator.Equal;
            where.Query = left;
            where.Value = TranslateSuitValue(left.DbType, right);

            if (left.IsSimple() && (left.DbType == DbType.Guid))
            {
                var rightStringValue = right as string;
                if (rightStringValue != null)
                {
                    where.Value = rightStringValue.AsGuid();
                }
            }

            return where;
        }

        /// <summary>
        /// 生成 column != value 表达式。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static WhereClip operator !=(ColumnClip left, object right)
        {
            WhereClip where = new WhereClip();
            where.Operator = SqlOperator.NotEqual;
            where.Query = left;
            where.Value = TranslateSuitValue(left.DbType, right);

            return where;
        }




        /// <summary>
        /// 生成 column > value 表达式。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static WhereClip operator >(ColumnClip left, object right)
        {
            WhereClip where = new WhereClip();
            where.Operator = SqlOperator.BigThan;
            where.Query = left;
            where.Value = TranslateSuitValue(left.DbType, right);

            return where;
        }

        /// <summary>
        /// 生成 column &lt; value 表达式。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static WhereClip operator <(ColumnClip left, object right)
        {
            WhereClip where = new WhereClip();
            where.Operator = SqlOperator.LessThan;
            where.Query = left;
            where.Value = TranslateSuitValue(left.DbType, right);

            return where;
        }



        /// <summary>
        /// 生成 column >= value 表达式。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static WhereClip operator >=(ColumnClip left, object right)
        {
            WhereClip where = new WhereClip();
            where.Operator = SqlOperator.BigEqual;
            where.Query = left;
            where.Value = TranslateSuitValue(left.DbType, right);

            return where;
        }

        /// <summary>
        /// 生成 column &lt;= value 表达式。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static WhereClip operator <=(ColumnClip left, object right)
        {
            WhereClip where = new WhereClip();
            where.Operator = SqlOperator.LessEqual;
            where.Query = left;
            where.Value = TranslateSuitValue(left.DbType, right);

            return where;
        }


        /// <summary>
        /// 生成 column &amp; value 表达式。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static WhereClip operator &(ColumnClip left, int right)
        {
            WhereClip where = new WhereClip();
            where.Operator = SqlOperator.BitAnd;
            where.Query = left;
            where.Value = TranslateSuitValue(left.DbType, right);
            return where;
        }

        /// <summary>
        /// 生成 column | value 表达式。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static WhereClip operator |(ColumnClip left, int right)
        {
            WhereClip where = new WhereClip();
            where.Operator = SqlOperator.BitOr;
            where.Query = left;
            where.Value = TranslateSuitValue(left.DbType, right);
            return where;
        }

        public WhereClip NotIn<T>(params T[] Values) where T : IComparable, IConvertible
        {
            return NotIn<T>(NoValueEnum.NoValue, Values);
        }
        /// <summary>
        /// 生成 not in （值参数） 子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="option"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public WhereClip NotIn<T>(NoValueEnum option, params T[] Values) where T : IComparable, IConvertible
        {
            dbo.Check(typeof(T).FullName == "System.Char", "In，NotIn不能使用Char类型，请改用 String类型", null);

            WhereClip where = new WhereClip();
            if (Values == null || Values.Length == 0)
            {
                if (option == NoValueEnum.Ignore) return where;

                where.Operator = SqlOperator.Equal;
                where.Query = new ConstColumn(1);
                where.Value = new ConstColumn(0);
                return where;
            }
            else
            {
                where.Operator = SqlOperator.NotIn;
                where.Query = this;

                if (this.DbType.DbTypeIsNumber() || this.DbType == System.Data.DbType.Boolean)
                {
                    where.Value = Values.Select(o => o.AsDecimal()).ToArray();
                }
                else
                {
                    where.Value = Values;
                }
            }
            return where;
        }

        /// <summary>
        /// 生成 not in (select 语句) 子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Select"></param>
        /// <returns></returns>
        public WhereClip NotIn<T>(SelectClip<T> Select) where T : RuleBase
        {
            WhereClip where = new WhereClip();
            where.Operator = SqlOperator.NotIn;
            where.Query = this;
            where.Value = Select;
            return where;
        }


        /// <summary>
        /// 生成 in (值参数）子句。当没有数据时，生成SQL 语句的方式。默认生成 1=0,不会返回结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Values"></param>
        /// <returns></returns>
        public WhereClip In<T>(params T[] Values) where T : IComparable, IConvertible
        {
            return In<T>(NoValueEnum.NoValue, Values);
        }

        /// <summary>
        /// 生成 in (值参数）子句。
        /// </summary>
        /// <param name="option">当没有数据时，生成SQL 语句的方式。默认生成 1=0,不会返回结果</param>
        /// <typeparam name="T"></typeparam>
        /// <param name="Values"></param>
        /// <returns></returns>
        public WhereClip In<T>(NoValueEnum option, params T[] Values) where T : IComparable, IConvertible
        {
            //这两个枚举约束是表示ValueType

            GodError.Check(typeof(T).FullName == "System.Char", "MyOqlGodError", "In，NotIn不能使用Char类型，请改用 String类型", this.GetFullName() + " In ('" + Values.Join("','") + "')");

            if (Values == null || Values.Length == 0)
            {
                if (option == NoValueEnum.Ignore) return new WhereClip();

                WhereClip where = new WhereClip();
                where.Operator = SqlOperator.Equal;
                where.Query = new ConstColumn(1);
                where.Value = new ConstColumn(0);
                return where;
            }
            else
            {
                WhereClip where = new WhereClip();
                where.Operator = SqlOperator.In;
                where.Query = this;

                if (this.DbType.DbTypeIsNumber() || this.DbType == System.Data.DbType.Boolean)
                {
                    where.Value = Values.Select(o => o.AsDecimal()).ToArray();
                }
                else
                {
                    where.Value = Values;
                }

                return where;
            }
        }

        /// <summary>
        /// 生成 in (select 语句）子句。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Select"></param>
        /// <returns></returns>
        public WhereClip In<T>(SelectClip<T> Select) where T : RuleBase
        {
            WhereClip where = new WhereClip();
            where.Operator = SqlOperator.In;
            where.Query = this;
            where.Value = Select;
            return where;
        }

        /// <summary>
        /// 生成 like 表达式，会对值 ： [  _  进行转义。
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public WhereClip Like(string Expression)
        {
            //如果列是 guid , like 无意义, 报错. 

            GodError.Check(this.DbType == System.Data.DbType.Guid, "GUID 列 (" + this.GetFullName() +") 不能 Like. 请用 == ");

            return OriLike(Expression.Replace("[", "[[]").Replace("_", "[_]"));
        }

        /// <summary>
        /// 原生的 Like ，不对值进行转义。
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public WhereClip OriLike(string Expression)
        {
            //如果列是 guid , like 无意义, 报错. 

            GodError.Check(this.DbType == System.Data.DbType.Guid, "GUID 列 (" + this.GetFullName() + ") 不能 Like. 请用 == ");

            WhereClip where = new WhereClip();
            where.Operator = SqlOperator.Like;
            where.Query = this;
            where.Value = Expression;
            return where;
        }

        public WhereClip Like(ColumnClip Expression)
        {
            //如果列是 guid , like 无意义, 报错. 
            GodError.Check(this.DbType == System.Data.DbType.Guid, "GUID 列 (" + this.GetFullName() + ") 不能 Like. 请用 == ");

            WhereClip where = new WhereClip();
            where.Operator = SqlOperator.Like;
            where.Query = this;
            where.Value = Expression;
            return where;
        }


        /// <summary>
        /// Oracle 中 采用 Between And 的时间值不会查询到内容.目前禁用. Udi 2010-10-28
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public WhereClip Between(object value1, object value2)
        {
            //GodError.Check(this.DbType.IsIn(DbType.Date, DbType.DateTime, DbType.DateTime2, DbType.DateTimeOffset), "Oracle 中 采用 Between And 的时间值不会查询到内容.目前禁用!", InfoEnum.Error | InfoEnum.MyOql, this.GetFullName().ToString());

            var val = new ComplexColumn();
            val.Operator = SqlOperator.AndForBetween;
            val.LeftExp = new ConstColumn(value1);
            val.RightExp = new ConstColumn(value2);


            WhereClip where = new WhereClip();
            where.Operator = SqlOperator.Between;
            where.Query = this;
            where.Value = val;// new object[] { value1, value2 };

            return where;
        }




        ///// <summary>
        ///// 默认返回 Table 名。
        ///// </summary>
        ///// <returns></returns>
        //public string GetFromTableAlias()
        //{
        //    return this.Extend[SqlOperator.FromTable].AsString();
        //}
    }
}
