using System;
using System.Linq;
using MyCmn;
using System.Data;

namespace MyOql
{
    /// <summary>
    /// 自定义列.可以解析一些特殊的关键字.如 Case When Then Else End
    /// </summary>
    /// <remarks>
    /// 用处：
    /// 1. 在查询里引用子查询的列。
    /// 2. 自定义函数列 如： dbo.JoinStr 。 系统函数在复合列。
    /// 3. 
    /// 
    /// 
    /// RawColumn 必须是简单列。规范：
    /// 1. DbType 定义的是该列返回的列类型。
    /// 2. Name 别名
    /// 3. Extend中仅有一个 SqlOperator 
    /// </remarks>
    public class RawColumn : ColumnClip 
    {
        /// <summary>
        /// 操作标识，可能只是一个标识Int。 大于1024 表示自定义。如自定义聚合 dbo.JoinStr
        /// </summary>
        public SqlOperator Operator { get; set; }
        public ValueMetaTypeEnum ParameterValueType { get; set; }

        public string FunctionFormat { get; set; }

        public object Parameter { get; private set; }

        public void SetParameter(DbType value)
        {
            this.ParameterValueType = ValueMetaTypeEnum.DbType;
            this.Parameter = value;
        }
        public void SetParameter(Enum value)
        {
            this.ParameterValueType = ValueMetaTypeEnum.EnumType;
            this.Parameter = value;
        }
        public void SetParameter(decimal value)
        {
            this.ParameterValueType = ValueMetaTypeEnum.NumberType;
            this.Parameter = value;
        }

        public void SetParameter(string value)
        {
            this.ParameterValueType = ValueMetaTypeEnum.StringType;
            this.Parameter = value;
        }

        public void SetParameter(ColumnClip value)
        {
            this.ParameterValueType = ValueMetaTypeEnum.MyOqlType;
            this.Parameter = value;
        }

        public void SetParameter(WhereClip value)
        {
            this.ParameterValueType = ValueMetaTypeEnum.MyOqlType;
            this.Parameter = value;
        }

        public RawColumn()
        {
            //自定义的列名不能使用。
            this.Key = SqlKeyword.Raw;
            this.DbType = System.Data.DbType.Object;
        }

        public RawColumn(string Name)
            : this()
        {
            // TODO: Complete member initialization
            this.Name = Name;
        }

        public RawColumn(ValueMetaTypeEnum sqlOperator, object value)
            : this()
        {
            // TODO: Complete member initialization
            this.ParameterValueType = sqlOperator;
            this.Parameter = value;
        }

        public override ColumnName GetFullDbName()
        {
            return new ColumnName { Column = this.Name };
        }

        public override ColumnName GetFullName()
        {
            return new ColumnName { Column = this.Name };
        }

        protected override object CloneClip()
        {
            var clone = new RawColumn();
            clone.Name = this.Name;
            clone.DbType = this.DbType;
            clone.Length = this.Length;
            clone.TableName = this.TableName;

            if (this.ParameterValueType == ValueMetaTypeEnum.MyOqlType)
            {
                clone.Parameter = (this.Parameter as SqlClipBase).Clone();
            }
            else clone.Parameter = this.Parameter;

            clone.FunctionFormat = this.FunctionFormat;
            clone.Operator = this.Operator;
            clone.ParameterValueType = this.ParameterValueType;

            return clone;
        }
    }
}
