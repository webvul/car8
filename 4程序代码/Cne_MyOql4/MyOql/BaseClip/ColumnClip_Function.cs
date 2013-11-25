using System;
using MyCmn;
using System.Data;
using System.Linq;

namespace MyOql
{
    public partial class ColumnClip
    {
        /// <summary>
        /// 二进制 And ,生成Sql 为  &amp;
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public ComplexColumn BitAnd<T>(T Value) where T : IComparable, IFormattable, IConvertible
        {
            return BitAnd(Value.AsInt());
        }

        /// <summary>
        /// 二进制 And ,生成Sql 为  &amp;
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public ComplexColumn BitAnd(int Value)
        {
            if (this.DbType != System.Data.DbType.Object)
            {
                GodError.Check(this.DbType.DbTypeIsNumber() == false, "二进制操作要求列是数字类型!");
            }

            var col = new ComplexColumn(this.Name.AsString("BitAnd"));
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.BitAnd;
            col.DbType = System.Data.DbType.Int32;
            col.RightExp = new ConstColumn(Value);
            return col;
        }


        /// <summary>
        /// 二进制 Or , 生成SQL 为 &amp;
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public ComplexColumn BitOr<T>(T Value) where T : IComparable, IFormattable, IConvertible
        {
            return BitOr(Value.AsInt());
        }

        /// <summary>
        /// 二进制 Or , 生成SQL 为 &amp;
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public ComplexColumn BitOr(int Value)
        {
            if (this.DbType != System.Data.DbType.Object)
            {
                GodError.Check(this.DbType.DbTypeIsNumber() == false, "二进制操作要求列是数字类型! (" + this.GetFullName() + " BitOr " + Value.ToString() + ")");
            }

            var col = new ComplexColumn(this.Name.AsString("BitOr"));
            col.LeftExp = this.Clone() as ColumnClip;
            col.DbType = System.Data.DbType.Int32;
            col.Operator = SqlOperator.BitOr;
            col.RightExp = new ConstColumn(Value);
            return col;
        }


        #region 三参函数
        /// <summary>
        /// 生成 substring(column,1,2) 表达式。
        /// </summary>
        /// <returns></returns>
        public ColumnClip SubString(int StartIndex, int Length)
        {
            return SubString(new ConstColumn(StartIndex), Length);
            //var col = this.Clone();
            //col.DbType = System.Data.DbType.AnsiString;
            //col.Extend.Add(SqlOperator.SubString, new KeyValuePair<object, object>(StartIndex, Length));
            //return col;
        }

        /// <summary>
        /// 生成 substring(column,1,2) 表达式。
        /// </summary>
        /// <remarks>
        /// 例如表达式: dbr.Menu.Name.SubString('Text',1,2) .其结构如下:<br />
        /// <pre>
        ///                             ComplexColumn:返回的一个复合列
        ///                             ----------------------------
        ///     Left:dbr.Menu.Name       Operator:SqlOperator.SubString       Right:ComplexColumn     
        ///     -------------------------------------------------------------------
        ///                                                                      1      Parameter     2       
        /// </pre>
        /// </remarks>
        /// <param name="StartIndex">起始索引</param>
        /// <param name="Length">长度值</param>
        /// <returns></returns>
        public ComplexColumn SubString(ColumnClip StartIndex, ColumnClip Length)
        {
            var val = new ComplexColumn();
            val.Operator = SqlOperator.Parameter;
            val.LeftExp = StartIndex.Clone() as ColumnClip;
            val.RightExp = Length.Clone() as ColumnClip;

            var col = new ComplexColumn(this.Name.AsString("SubString"));
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.SubString;
            col.RightExp = val;
            return col;
        }

        public ComplexColumn SubString(int StartIndex, ColumnClip Length)
        {
            return SubString(new ConstColumn(StartIndex), Length);
        }

        public ComplexColumn SubString(ColumnClip StartIndex, int Length)
        {
            return SubString(StartIndex, new ConstColumn(Length));
        }

        /// <summary>
        /// 表示 Replace 函数。 
        /// </summary>
        /// <param name="StringPattern"></param>
        /// <param name="StringReplacement"></param>
        /// <returns></returns>
        public ComplexColumn Replace(ColumnClip StringPattern, ColumnClip StringReplacement)
        {
            var val = new ComplexColumn();
            val.Operator = SqlOperator.Parameter;
            val.LeftExp = StringPattern.Clone() as ColumnClip;
            val.RightExp = StringReplacement.Clone() as ColumnClip;

            var col = new ComplexColumn(this.Name.AsString("Replace"));
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Replace;
            col.RightExp = val;
            return col;
        }

        /// <summary>
        /// 表示 Replace 函数。 
        /// </summary>
        /// <param name="StringPattern"></param>
        /// <param name="StringReplacement"></param>
        /// <returns></returns>
        public ComplexColumn Replace(string StringPattern, string StringReplacement)
        {
            return Replace(new ConstColumn(StringPattern), new ConstColumn(StringReplacement));
        }


        /// <summary>
        /// 表示 PatIndex 函数
        /// </summary>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        public ComplexColumn PatIndex(string Pattern)
        {
            var col = new ComplexColumn(this.Name.AsString("PatIndex"));
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.PatIndex;
            col.RightExp = new ConstColumn(Pattern);
            return col;
        }


        /// <summary>
        /// 表示 DataLength 函数
        /// </summary>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        public ComplexColumn SizeOf()
        {
            var col = new ComplexColumn(this.Name.AsString("SizeOf"));
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.SizeOf;
            col.RightExp = null;
            return col;
        }

        public ComplexColumn IsDate()
        {
            var col = new ComplexColumn(this.Name.AsString("IsDate"));
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.IsDate;
            col.RightExp = null;
            return col;
        }

        public ComplexColumn Stuff(int startIndex, int length, ColumnClip insertExp)
        {
            var col = new ComplexColumn(this.Name.AsString("Stuff"));
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Stuff;
            col.RightExp = insertExp.Clone() as ColumnClip;

            var param = new ComplexColumn();
            param.Operator = SqlOperator.Parameter;
            param.LeftExp = new ConstColumn(startIndex);
            param.RightExp = new ConstColumn(length);

            return col;
        }

        public ComplexColumn Char()
        {
            var col = new ComplexColumn(this.Name.AsString("Char"));
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Char;
            col.RightExp = null;
            return col;
        }

        public ComplexColumn NChar()
        {
            var col = new ComplexColumn(this.Name.AsString("NChar"));
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.NChar;
            col.RightExp = null;
            return col;
        }

        public ComplexColumn Unicode()
        {
            var col = new ComplexColumn(this.Name.AsString("Unicode"));
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Unicode;
            col.RightExp = null;
            return col;
        }

        public ComplexColumn AscII()
        {
            var col = new ComplexColumn(this.Name.AsString("AscII"));
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.AscII;
            col.RightExp = null;
            return col;
        }

        public ComplexColumn Reverse()
        {
            var col = new ComplexColumn(this.Name.AsString("Reverse"));
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Reverse;
            col.RightExp = null;
            return col;
        }

        public ComplexColumn Right(int length)
        {
            return Right(new ConstColumn(length));
        }

        public ComplexColumn Right(ColumnClip lengthColumn)
        {
            var col = new ComplexColumn(this.Name.AsString("Right"));
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Right;
            col.RightExp = lengthColumn;
            return col;
        }

        public ComplexColumn Left(int length)
        {
            return Left(new ConstColumn(length));
        }

        public ComplexColumn Left(ColumnClip lengthColumn)
        {
            var col = new ComplexColumn(this.Name.AsString("Left"));
            col.DbType = System.Data.DbType.Int64;
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Left;
            col.RightExp = lengthColumn;
            return col;
        }

        /// <summary>
        /// 生成 charindex instr() 表达式。
        /// </summary>
        /// <returns></returns>
        public ComplexColumn StringIndex(string FindString)
        {
            return StringIndex(FindString, 1);
        }

        /// <summary>
        /// 生成 charindex instr() 表达式。
        /// </summary>
        /// <returns></returns>
        public ComplexColumn StringIndex(string FindString, int StartIndex)
        {
            return StringIndex(new ConstColumn(FindString), StartIndex);
        }


        /// <summary>
        /// 生成 charindex instr() 表达式。
        /// </summary>
        /// <returns></returns>
        public ComplexColumn StringIndex(ColumnClip FindString)
        {
            return StringIndex(FindString, 1);
        }

        /// <summary>
        /// 生成 charindex instr() 表达式。
        /// </summary>
        /// <returns></returns>
        public ComplexColumn StringIndex(ColumnClip FindString, int StartIndex)
        {
            var para = new ComplexColumn();
            para.Operator = SqlOperator.Parameter;
            para.LeftExp = this.Clone() as ColumnClip;
            para.RightExp = new ConstColumn(StartIndex);

            var col = new ComplexColumn(this.Name.AsString("StringIndex"));
            col.DbType = System.Data.DbType.Int32;
            col.LeftExp = FindString.Clone() as ColumnClip;
            col.Operator = SqlOperator.StringIndex;
            col.RightExp = para;
            return col;
        }
        #endregion

        #region 派生的业务函数
        public WhereClip Contains(ColumnClip FindString)
        {
            return StringIndex(FindString, 1) > 0;
        }

        public WhereClip Contains(string FindString)
        {
            return StringIndex(FindString, 1) > 0;
        }
        #endregion

        /// <summary>
        /// 生成 SQL 的Len 函数。
        /// </summary>
        /// <returns></returns>
        public ComplexColumn Len()
        {
            var col = new ComplexColumn(this.Name.AsString("Len"));
            col.DbType = System.Data.DbType.Int64;
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Len;
            col.RightExp = null;
            return col;
        }

        /// <summary>
        /// 生成 SQL 的 LTrim函数。
        /// </summary>
        /// <returns></returns>
        public ComplexColumn Ltrim()
        {
            var col = new ComplexColumn(this.Name.AsString("Ltrim"));
            col.DbType = this.DbType;
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Ltrim;
            col.RightExp = null;
            return col;
        }

        /// <summary>
        /// 生成 SQL 的 LTrim函数。
        /// </summary>
        /// <returns></returns>
        public ComplexColumn Rtrim()
        {
            var col = new ComplexColumn(this.Name.AsString("Rtrim"));
            col.DbType = this.DbType;
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Rtrim;
            col.RightExp = null;
            return col;
        }

        /// <summary>
        /// 生成 SQL 的 LTrim函数。
        /// </summary>
        /// <returns></returns>
        public ComplexColumn Trim()
        {
            var col = new ComplexColumn(this.Name.AsString("Trim"));
            col.DbType = this.DbType;
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Trim;
            col.RightExp = null;
            return col;
        }

        public WhereClip IsNumeric()
        {
            var col = new ComplexColumn(this.Name.AsString("IsNumeric"));
            col.DbType = System.Data.DbType.Byte;
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.IsNumeric;
            col.RightExp = null;
            return col == 1;
        }

        /// <summary>
        /// 类型转换.
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public ComplexColumn Cast(DbType dbType)
        {
            var col = new ComplexColumn(this.Name.AsString("Cast"));
            col.DbType = dbType;
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Cast;
            col.RightExp = new RawColumn(ValueMetaTypeEnum.DbType, dbType);
            return col;
        }

        #region 聚合函数
        /// <summary>
        /// 生成 count(1) 表达式。
        /// </summary>
        /// <returns></returns>
        public ComplexColumn Count()
        {
            var col = new ComplexColumn(this.Name.AsString("Count"));
            col.DbType = System.Data.DbType.Int64;
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Count;
            col.RightExp = null;
            return col;
        }

        /// <summary>
        /// 生成 count( distinct column) 表达式。
        /// </summary>
        /// <returns></returns>
        public ComplexColumn CountDistinct()
        {
            var col = new ComplexColumn(this.Name.AsString("CountDistinct"));
            col.DbType = System.Data.DbType.Int64;
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.CountDistinct;
            col.RightExp = null;
            return col;
        }


        /// <summary>
        /// 生成 sum(column) 表达式。
        /// </summary>
        /// <returns></returns>
        public ComplexColumn Sum()
        {
            var col = new ComplexColumn(this.Name.AsString("Sum"));
            col.DbType = System.Data.DbType.Int64;
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Sum;
            col.RightExp = null;
            return col;
        }

        /// <summary>
        /// 生成 max(column) 表达式。
        /// </summary>
        /// <returns></returns>
        public ComplexColumn Max()
        {
            var col = new ComplexColumn(this.Name.AsString("Max"));
            col.DbType = System.Data.DbType.Int64;
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Max;
            col.RightExp = null;
            return col;
        }

        /// <summary>
        /// 生成 min(column) 表达式。
        /// </summary>
        /// <returns></returns>
        public ComplexColumn Min()
        {
            var col = new ComplexColumn(this.Name.AsString("Min"));
            col.DbType = System.Data.DbType.Int64;
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Min;
            col.RightExp = null;
            return col;
        }

        /// <summary>
        /// 生成 avg(column) 表达式。
        /// </summary>
        /// <returns></returns>
        public ComplexColumn Avg()
        {
            var col = new ComplexColumn(this.Name.AsString("Avg"));
            col.DbType = System.Data.DbType.Int64;
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Avg;
            col.RightExp = null;
            return col;
        }

        #endregion


        public ComplexColumn Year()
        {
            var col = new ComplexColumn(this.Name.AsString("Year"));
            col.DbType = System.Data.DbType.Int64;
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Year;
            col.RightExp = null;
            return col;
        }

        public ComplexColumn Month()
        {
            var col = new ComplexColumn(this.Name.AsString("Month"));
            col.DbType = System.Data.DbType.Int64;
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Month;
            col.RightExp = null;
            return col;
        }

        public ComplexColumn Day()
        {
            var col = new ComplexColumn(this.Name.AsString("Day"));
            col.DbType = System.Data.DbType.Int64;
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.Day;
            col.RightExp = null;
            return col;
        }

        private ColumnClip getDateTimeColumn(ColumnClip myCol)
        {
            if (myCol.DbType.DbTypeIsString())
            {
                return Cast(System.Data.DbType.DateTime);
            }
            else if (myCol.DbType.DbTypeIsDateTime())
            {
                return myCol.Clone() as ColumnClip;
            }
            else
            {
                throw new GodError(myCol.GetFullDbName().AsString() + " 列不能转换到时间类型");
            }
        }

        public ComplexColumn DateDiff(DatePart part, DateTime dt)
        {
            var para = new ComplexColumn();
            para.Operator = SqlOperator.Parameter;
            para.LeftExp = new ConstColumn(dt.ToString());
            para.RightExp = new RawColumn(ValueMetaTypeEnum.EnumType, part);

            var col = new ComplexColumn(this.Name.AsString("DateDiff"));
            col.DbType = System.Data.DbType.Int64;
            col.LeftExp = getDateTimeColumn(this);
            col.Operator = SqlOperator.DateDiff;
            col.RightExp = para;

            return col;
        }

        public ComplexColumn DateDiff(DatePart part, ColumnClip dtColumn)
        {
            var para = new ComplexColumn();
            para.Operator = SqlOperator.Parameter;
            para.LeftExp = getDateTimeColumn(dtColumn);
            para.RightExp = new RawColumn(ValueMetaTypeEnum.EnumType, part);

            var col = new ComplexColumn(this.Name.AsString("DateDiff"));
            col.DbType = System.Data.DbType.Int64;
            col.LeftExp = getDateTimeColumn(this);
            col.Operator = SqlOperator.DateDiff;
            col.RightExp = para;

            return col;
        }


        public ComplexColumn DateAdd(DatePart part, Int64 dtValue)
        {
            var para = new ComplexColumn();
            para.Operator = SqlOperator.Parameter;
            para.LeftExp = new ConstColumn(dtValue);
            para.RightExp = new RawColumn(ValueMetaTypeEnum.EnumType, part);

            var col = new ComplexColumn(this.Name.AsString("DateAdd"));
            col.DbType = System.Data.DbType.Int64;
            col.LeftExp = getDateTimeColumn(this);
            col.Operator = SqlOperator.DateAdd;
            col.RightExp = para;
            return col;
        }

        public ComplexColumn DateAdd(DatePart part, ColumnClip dtValue)
        {
            var para = new ComplexColumn();
            para.Operator = SqlOperator.Parameter;
            para.LeftExp = getDateTimeColumn(dtValue);
            para.RightExp = new RawColumn(ValueMetaTypeEnum.EnumType, part);

            var col = new ComplexColumn(this.Name.AsString("DateAdd"));
            col.DbType = System.Data.DbType.Int64;
            col.LeftExp = getDateTimeColumn(this);
            col.Operator = SqlOperator.DateAdd;
            col.RightExp = para;
            return col;
        }

    }
}
