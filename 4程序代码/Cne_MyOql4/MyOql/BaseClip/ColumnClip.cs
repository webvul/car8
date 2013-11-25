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
    /// </remarks>
    [Serializable]
    public abstract partial class ColumnClip : SqlClipBase, ICloneable, IAsable
    {
        /// <summary>
        /// Table的别名。
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 数据列的类型.
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        /// 数据长度,如果是Number,则为精度.
        /// </summary>
        public int Length { get; set; }

        ///// <summary>
        ///// 可以是 DbType, 简单类型,SqlClipBase ,对于不识别的类型，返回其 String 形式。
        ///// </summary>
        //public object Parameter { get; set; }

        ///// <summary>
        ///// 可以是 DbType, 简单类型,SqlClipBase
        ///// </summary>用.生成的Sql与出现的顺序相关.按顺序依次生成,如: Cast( M x (c l) a  int) , Max( cast( col as int ) ) 是不同的. 
        ///// </summary>
        //public ColumnExtend Extend { get; set; }

        /// <summary>
        /// 数据列. 也可为表达式。 转义后的名称.
        /// </summary>
        public string Name { get; set; }



        ///// <summary>
        ///// 小数位数, 用于生成 SQL.
        ///// </summary>
        //public int Scale { get; set; }
        /// <summary>
        /// 得到数据库里 列的全称(转义名称),为 表名.列名
        /// </summary>
        /// <returns></returns>
        public abstract ColumnName GetFullName();

        /// <summary>
        /// 得到数据库里 列的全称(非转义名称),为 表名.列名
        /// </summary>
        /// <returns></returns>
        public abstract ColumnName GetFullDbName();


        /// <summary>
        /// 得到列的别名.如果没有设置别名，则返回Name。
        /// </summary>
        /// <returns></returns>
        public string GetAlias()
        {
            return this.Name;
        }


        /// <summary>
        /// 返回的是 Name 属性.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }


        /// <summary>
        /// 是否是叶子。
        /// </summary>
        /// <returns></returns>
        public bool IsSimple()
        {
            return this.Key == SqlKeyword.Simple;
        }


        public bool IsComplex()
        {
            return this.Key == SqlKeyword.Complex;
        }

        public bool IsRaw()
        {
            return this.Key == SqlKeyword.Raw;
        }

        /// <summary>
        /// 由于重载了 ==,所以 == 不能比较两个 ColumnClip 是否相等，可果要比较两个 ColumnClip 可以使用：
        /// 如果要比较相等. 要使用 object.Equals(column ,object)
        /// 如果要比较是否为空,可使用 ColumnClip.IsNull(column)
        /// 当然你也可以使用 :  ColumnClip.Equlas(object) 比较。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var col = obj as ColumnClip;
            if (col.EqualsNull()) return false;
            return this.NameEquals(col);
        }


        ///// <summary>
        ///// 严格意义上的比较 , 相等的条件是 表名 DbName 和列名 DbName 相等,  忽略别名比较.
        ///// </summary>
        ///// <param name="one"></param>
        ///// <param name="two"></param>
        ///// <returns></returns>
        //public static bool ColumnEquals(ColumnClip one, ColumnClip two)
        //{
        //    if (one.EqualsNull()) return false;
        //    if (two.EqualsNull()) return false;
        //    if (one.Table == null) return false;
        //    if (two.Table == null) return false;
        //    if (one.DbName == two.DbName && one.Table.GetDbName() == two.Table.GetDbName()) return true;
        //    return false;
        //}



        //public static bool EqualObj(ColumnClip where, object obj)
        //{
        //    return object.Equals(where, obj);
        //}

        /// <summary>
        /// 判断 Column 是否为空.
        /// </summary>
        /// <param name="Column"></param>
        /// <returns></returns>
        //public static bool EqualsNull(ColumnClip Column)
        //{
        //    return object.Equals(Column, null);
        //}

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        /// <summary>
        /// 调用数据库函数 IsNull
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ComplexColumn IsNull<T>(T value)
        {
            var col = new ComplexColumn(this.Name.AsString("IsNull"));

            var tType = typeof(T);
            ColumnClip val = null;
            if (tType.IsEnum)
            {
                if (this.DbType.DbTypeIsNumber() || this.DbType == System.Data.DbType.Boolean)
                {
                    val = new ConstColumn(ValueProc.As<int>(value));
                }
                else
                {
                    val = new ConstColumn(value.AsString());
                }
            }
            else
            {
                var columnVal = value as ColumnClip;
                if (columnVal.EqualsNull() == false)
                {
                    val = columnVal.Clone() as ColumnClip;
                }
                else
                {
                    val = new ConstColumn(value);
                }
            }

            col.DbType = this.DbType;
            col.LeftExp = this.Clone() as ColumnClip;
            col.Operator = SqlOperator.IsNull;
            col.RightExp = val;
            return col;

        }


        /// <summary>
        /// 是否是 常数列。
        /// </summary>
        /// <returns></returns>
        public bool IsConst()
        {
            return this.Key == SqlKeyword.Const;
        }

        /// <summary>
        /// 是否是聚合函数
        /// </summary>
        /// <returns></returns>
        public bool IsPolymer()
        {
            if (IsSimple()) return false;

            if (this.IsComplex())
            {
                var complex = this as ComplexColumn;
                if (complex.Operator.IsIn(
                        SqlOperator.Sum,
                        SqlOperator.CountDistinct,
                        SqlOperator.Count,
                        SqlOperator.Max,
                        SqlOperator.Min,
                        SqlOperator.Avg))
                {
                    return true;
                }
                if (dbo.Polymer.Contains(complex.Operator)) return true;
            }
            else if (this.IsRaw())
            {
                var raw = this as RawColumn;
                if (dbo.Polymer.Contains(raw.Operator)) return true;
            }

            return false;
        }


        /// <summary>
        /// 生成 对列升序 的子句.
        /// </summary>
        public OrderByClip Asc
        {
            get { return new OrderByClip(this) { IsAsc = true }; }
        }

        /// <summary>
        /// 生成 对列降序 的子句.
        /// </summary>
        public OrderByClip Desc
        {
            get { return new OrderByClip(this) { IsAsc = false }; }
        }

        ///// <summary>
        ///// 判断该列是否是函数列。函数都是复合列，左为主，右为辅，只有一个Extend[SqlOperator.FunctionParameter]
        ///// </summary>
        ///// <returns></returns>
        //public bool IsFunction()
        //{
        //    return this.Extend.Select(o => o.Key).Intersect(
        //        new SqlOperator[]{
        //             SqlOperator.Add 
        //            ,SqlOperator.Cast
        //            ,SqlOperator.Avg   
        //            ,SqlOperator.Count   
        //            ,SqlOperator.CountDistinct 
        //            ,SqlOperator.Custom 
        //            ,SqlOperator.Divided 
        //            ,SqlOperator.Function  
        //            ,SqlOperator.IsNull  
        //            ,SqlOperator.Len 
        //            ,SqlOperator.Max  
        //            ,SqlOperator.Min
        //            ,SqlOperator.Minus
        //            ,SqlOperator.Mod  
        //            ,SqlOperator.Multiple 
        //            ,SqlOperator.StringIndex 
        //            ,SqlOperator.SubString
        //            ,SqlOperator.Sum
        //            ,SqlOperator.CaseWhen 
        //            ,SqlOperator.Year
        //            ,SqlOperator.Month
        //            ,SqlOperator.Day
        //            ,SqlOperator.DateDiff
        //            ,SqlOperator.IsNumeric
        //            ,SqlOperator.Left
        //            ,SqlOperator.Right
        //            ,SqlOperator.Reverse
        //            ,SqlOperator.AscII

        //        }).Any();
        //}

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }



        public override object Clone()
        {
            return this.CloneClip();
        }

        protected abstract object CloneClip();

        /// <summary>
        /// 调用前，要对对象进行克隆。
        /// </summary>
        /// <param name="Alias"></param>
        public void SetAlias(string Alias)
        {
            this.Name = Alias;
        }
    }
}
