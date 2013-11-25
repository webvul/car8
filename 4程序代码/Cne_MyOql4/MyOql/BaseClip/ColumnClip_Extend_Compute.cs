using System.Data;
using MyCmn;

namespace MyOql
{
    /// <summary>
    /// 列子句
    /// </summary>
    public partial class ColumnClip
    {
        #region 重载运算符 .
        /// <summary>
        /// 对于两个数字列或左为数据列,右为数值类型来说,它是加法操作符.
        /// 其它情况,它是 连接符
        /// </summary>
        /// <remarks> Object 类型属SQL 注入型，当有任何一方是 Object类型，均不处理类型转换。</remarks>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static ComplexColumn operator +(ColumnClip left, ColumnClip right)
        {
            if (right.DbType.DbTypeIsNumber() && left.DbType.DbTypeIsNumber())
            {
                var col = new ComplexColumn();
                col.DbType = left.DbType;
                col.LeftExp = left.Clone() as ColumnClip;
                col.Operator = SqlOperator.Add;
                col.RightExp = right.Clone() as ColumnClip;
                return col;

                //return ColumnClip.Op(left, SqlOperator.Add, val);
            }

            if (left.DbType != DbType.Object && right.DbType != DbType.Object)
            {
                if (right.DbType.DbTypeIsNumber() != left.DbType.DbTypeIsNumber())
                {
                    var col = new ComplexColumn();
                    col.LeftExp = (left.Clone() as ColumnClip).Cast(DbType.String);
                    col.Operator = SqlOperator.Concat;
                    col.RightExp = (right.Clone() as ColumnClip).Cast(DbType.String);
                    return col;
                }
            }


            {
                var col = new ComplexColumn();
                col.LeftExp = left.Clone() as ColumnClip;
                col.Operator = SqlOperator.Concat;
                col.RightExp = right.Clone() as ColumnClip;
                return col;
            }
            //return ColumnClip.Op(left.Cast(DbType.AnsiString), SqlOperator.Contact, val.Cast(DbType.AnsiString));
        }

        public static ComplexColumn operator -(ColumnClip left, ColumnClip right)
        {
            var col = new ComplexColumn();
            col.DbType = left.DbType;
            col.LeftExp = left.Clone() as ColumnClip;
            col.Operator = SqlOperator.Minus;
            col.RightExp = right.Clone() as ColumnClip;
            return col;

            //return ColumnClip.Op(left, SqlOperator.Minus, right);
        }

        public static ComplexColumn operator *(ColumnClip left, ColumnClip right)
        {
            var col = new ComplexColumn();
            col.DbType = left.DbType;
            col.LeftExp = left.Clone() as ColumnClip;
            col.Operator = SqlOperator.Multiple;
            col.RightExp = right.Clone() as ColumnClip;
            return col;

            //return ColumnClip.Op(left, SqlOperator.Multiple, right);
        }


        public static ComplexColumn operator /(ColumnClip left, ColumnClip right)
        {
            var col = new ComplexColumn();
            col.DbType = left.DbType;
            col.LeftExp = left.Clone() as ColumnClip;
            col.Operator = SqlOperator.Divided;
            col.RightExp = right.Clone() as ColumnClip;
            return col;

            //return ColumnClip.Op(left, SqlOperator.Divided, right);
        }

        public static ComplexColumn operator %(ColumnClip left, ColumnClip right)
        {
            var col = new ComplexColumn();
            col.DbType = left.DbType;
            col.LeftExp = left.Clone() as ColumnClip;
            col.Operator = SqlOperator.Mod;
            col.RightExp = right.Clone() as ColumnClip;
            return col;

            //return ColumnClip.Op(left, SqlOperator.Mod, right);
        }


        /// <summary>
        /// 对于两个数字列或左为数据列,右为数值类型来说,它是加法操作符.
        /// 其它情况,它是 连接符
        /// </summary>
        /// <param name="left"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ComplexColumn operator +(ColumnClip left, int val)
        {
            return left + new ConstColumn(val);
            //if (left.DbType.DbTypeIsNumber())
            //{
            //    return ColumnClip.Op(left, SqlOperator.Add, val);
            //}
            //return ColumnClip.Op(left, SqlOperator.Contact, val);
        }

        public static ComplexColumn operator -(ColumnClip left, int right)
        {
            return left - new ConstColumn(right);
            //return ColumnClip.Op(left, SqlOperator.Minus, right);
        }

        public static ComplexColumn operator *(ColumnClip left, int right)
        {
            return left * new ConstColumn(right);
            //return ColumnClip.Op(left, SqlOperator.Multiple, right);
        }


        public static ComplexColumn operator /(ColumnClip left, int right)
        {
            return left / new ConstColumn(right);
            //return ColumnClip.Op(left, SqlOperator.Divided, right);
        }

        public static ComplexColumn operator %(ColumnClip left, int right)
        {
            return left % new ConstColumn(right);
            //return ColumnClip.Op(left, SqlOperator.Mod, right);
        }


        /// <summary>
        /// 对于两个数字列或左为数据列,右为数值类型来说,它是加法操作符.
        /// 其它情况,它是 连接符
        /// </summary>
        /// <param name="left"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ComplexColumn operator +(ColumnClip left, double val)
        {
            return left + new ConstColumn(val);
            //if (left.DbType.DbTypeIsNumber())
            //{
            //    return ColumnClip.Op(left, SqlOperator.Add, val);
            //}
            //return ColumnClip.Op(left, SqlOperator.Contact, val);
        }

        public static ComplexColumn operator -(ColumnClip left, double right)
        {
            return left - new ConstColumn(right);
            //return ColumnClip.Op(left, SqlOperator.Minus, right);
        }

        public static ComplexColumn operator *(ColumnClip left, double right)
        {
            return left * new ConstColumn(right);
            //return ColumnClip.Op(left, SqlOperator.Multiple, right);
        }


        public static ComplexColumn operator /(ColumnClip left, double right)
        {
            return left / new ConstColumn(right);
            //return ColumnClip.Op(left, SqlOperator.Divided, right);
        }

        public static ComplexColumn operator %(ColumnClip left, double right)
        {
            return left % new ConstColumn(right);
            //return ColumnClip.Op(left, SqlOperator.Mod, right);
        }

        /// <summary>
        /// 对于两个数字列或左为数据列,右为数值类型来说,它是加法操作符.
        /// 其它情况,它是 连接符
        /// </summary>
        /// <param name="left"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ComplexColumn operator +(ColumnClip left, decimal val)
        {
            return left + new ConstColumn(val);
            //if (left.DbType.DbTypeIsNumber())
            //{
            //    return ColumnClip.Op(left, SqlOperator.Add, val);
            //}
            //return ColumnClip.Op(left, SqlOperator.Contact, val);
        }

        public static ComplexColumn operator -(ColumnClip left, decimal right)
        {
            return left - new ConstColumn(right);
            //return ColumnClip.Op(left, SqlOperator.Minus, right);
        }

        public static ComplexColumn operator *(ColumnClip left, decimal right)
        {
            return left * new ConstColumn(right);
            //return ColumnClip.Op(left, SqlOperator.Multiple, right);
        }


        public static ComplexColumn operator /(ColumnClip left, decimal right)
        {
            return left / new ConstColumn(right);
            //return ColumnClip.Op(left, SqlOperator.Divided, right);
        }

        public static ComplexColumn operator %(ColumnClip left, decimal right)
        {
            return left % new ConstColumn(right);
            //return ColumnClip.Op(left, SqlOperator.Mod, right);
        }


        /// <summary>
        /// 对于两个数字列或左为数据列,右为数值类型来说,它是加法操作符.
        /// 其它情况,它是 连接符
        /// </summary>
        /// <param name="left"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ComplexColumn operator +(ColumnClip left, string val)
        {
            return left + new ConstColumn(val);
            //return ColumnClip.Op(left.Cast(DbType.AnsiString), SqlOperator.Contact, val);
        }

        #endregion

        #region 前面是 ConstColumn

        /// <summary>
        /// 对于两个数字列或左为数据列,右为数值类型来说,它是加法操作符.
        /// 其它情况,它是 连接符
        /// </summary>
        /// <param name="left"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ComplexColumn operator +(int left, ColumnClip val)
        {
            return new ConstColumn(left) + val;
            //if (val.DbType.DbTypeIsNumber())
            //{
            //    return ColumnClip.Op(new ConstColumn(left), SqlOperator.Add, val);
            //}
            //return ColumnClip.Op(new ConstColumn(left), SqlOperator.Contact, val);
        }

        public static ComplexColumn operator -(int left, ColumnClip right)
        {
            return new ConstColumn(left) - right;
            //return ColumnClip.Op(new ConstColumn(left), SqlOperator.Minus, right);
        }

        public static ComplexColumn operator *(int left, ColumnClip right)
        {
            return new ConstColumn(left) * right;
            //return ColumnClip.Op(new ConstColumn(left), SqlOperator.Multiple, right);
        }


        public static ComplexColumn operator /(int left, ColumnClip right)
        {
            return new ConstColumn(left) / right;
            //return ColumnClip.Op(new ConstColumn(left), SqlOperator.Divided, right);
        }

        public static ComplexColumn operator %(int left, ColumnClip right)
        {
            return new ConstColumn(left) % right;
            //return ColumnClip.Op(new ConstColumn(left), SqlOperator.Mod, right);
        }


        /// <summary>
        /// 对于两个数字列或左为数据列,右为数值类型来说,它是加法操作符.
        /// 其它情况,它是 连接符
        /// </summary>
        /// <param name="left"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ComplexColumn operator +(double left, ColumnClip val)
        {
            return new ConstColumn(left) + val;
            //if (val.DbType.DbTypeIsNumber())
            //{
            //    return ColumnClip.Op(new ConstColumn(left), SqlOperator.Add, val);
            //}
            //return ColumnClip.Op(new ConstColumn(left), SqlOperator.Contact, val);
        }

        public static ComplexColumn operator -(double left, ColumnClip right)
        {
            return new ConstColumn(left) - right;
            //return ColumnClip.Op(new ConstColumn(left), SqlOperator.Minus, right);
        }

        public static ComplexColumn operator *(double left, ColumnClip right)
        {
            return new ConstColumn(left) * right;
            //return ColumnClip.Op(new ConstColumn(left), SqlOperator.Multiple, right);
        }


        public static ComplexColumn operator /(double left, ColumnClip right)
        {
            return new ConstColumn(left) / right;
            //ColumnClip col = new ColumnClip();
            //col.Left = new ConstColumn(left);
            //col.Operator = SqlOperator.Divided;
            //col.Right = right.Clone();
            //return col;
            //return ColumnClip.Op(new ConstColumn(left), SqlOperator.Divided, right);
        }

        public static ComplexColumn operator %(double left, ColumnClip right)
        {
            return new ConstColumn(left) % right;
            //ColumnClip col = new ColumnClip();
            //col.Left = new ConstColumn(left);
            //col.Operator = SqlOperator.Mod;
            //col.Right = right.Clone();
            //return col;

            //return ColumnClip.Op(new ConstColumn(left), SqlOperator.Mod, right);
        }

        /// <summary>
        /// 对于两个数字列或左为数据列,右为数值类型来说,它是加法操作符.
        /// 其它情况,它是 连接符
        /// </summary>
        /// <param name="left"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ComplexColumn operator +(decimal left, ColumnClip val)
        {
            return new ConstColumn(left) + val;
            //if (val.DbType.DbTypeIsNumber())
            //{
            //    return ColumnClip.Op(new ConstColumn(left), SqlOperator.Add, val/*.Cast(DbType.Decimal)*/);
            //}
            //return ColumnClip.Op(new ConstColumn(left), SqlOperator.Contact, val);
        }

        public static ComplexColumn operator -(decimal left, ColumnClip right)
        {
            return new ConstColumn(left) - right;
            //return ColumnClip.Op(new ConstColumn(left), SqlOperator.Minus, right);
        }

        public static ComplexColumn operator *(decimal left, ColumnClip right)
        {
            return new ConstColumn(left) * right;
            //return ColumnClip.Op(new ConstColumn(left), SqlOperator.Multiple, right);
        }


        public static ComplexColumn operator /(decimal left, ColumnClip right)
        {
            return new ConstColumn(left) / right;
            //return ColumnClip.Op(new ConstColumn(left), SqlOperator.Divided, right);
        }

        public static ComplexColumn operator %(decimal left, ColumnClip right)
        {
            return new ConstColumn(left) % right;
            //return ColumnClip.Op(new ConstColumn(left), SqlOperator.Mod, right);
        }


        /// <summary>
        /// 对于两个数字列或左为数据列,右为数值类型来说,它是加法操作符.
        /// 其它情况,它是 连接符
        /// </summary>
        /// <param name="left"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ComplexColumn operator +(string left, ColumnClip val)
        {
            return new ConstColumn(left) + val;
            //return ColumnClip.Op(new ConstColumn(left), SqlOperator.Contact, val.Cast(DbType.AnsiString));
        }

        #endregion
    }
}
