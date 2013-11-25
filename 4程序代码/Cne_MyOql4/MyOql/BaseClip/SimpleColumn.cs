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
    public partial class SimpleColumn : ColumnClip, ICloneable
    {
        /// <summary>
        /// 当前 数据实体对象（DbName）. 在OrderByClip的Order时，可以是 Null。因为只有 Name 才有意义。
        /// </summary>
        public string TableDbName { get; set; }

        /// <summary>
        /// 列是否可空。
        /// </summary>
        public bool DbNullable { get; set; }

        /// <summary>
        /// 数据库中的非转义名称.
        /// </summary>
        public string DbName { get; set; }



        public SimpleColumn()
        {
            this.Key = SqlKeyword.Simple;
            //Extend = new ColumnExtend();
        }

        public SimpleColumn(string TableDbName, string TableName, DbType type, string name, string dbName, bool DbNullable)
            : this()
        {
            this.TableDbName = TableDbName;
            this.TableName = TableName;
            this.Length = Length;
            this.DbType = type;
            this.Name = name;
            this.DbName = dbName;
            this.DbNullable = DbNullable;
            //this.Extend[SqlOperator.As] = this.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TabldDbName"></param>
        /// <param name="type"></param>
        /// <param name="Length"></param>
        /// <param name="name"></param>
        /// <param name="dbName">数据库中使用的名字</param>
        public SimpleColumn(string TabldDbName, string TableName, DbType type, int Length,/* int Scale, */string name, string dbName, bool DbNullable)
            : this(TabldDbName, TableName, type, name, dbName, DbNullable)
        {
            this.Length = Length;
            //this.Scale = Scale;
        }

        public SimpleColumn(RuleBase Rule, DbType type, int Length, string name, string dbName, bool DbNullable)
            : this(Rule.GetDbName(), Rule.GetName(), type, Length, name, dbName, DbNullable)
        {
        }


        protected override object CloneClip()
        {
            var clone = new SimpleColumn();
            clone.Name = this.Name;
            clone.DbName = this.DbName;
            clone.TableDbName = this.TableDbName;
            clone.TableName = this.TableName;
            clone.DbType = this.DbType;
            clone.Length = this.Length;
            return clone;
        }

        public override ColumnName GetFullName()
        {
            if (this.TableName == null) return new ColumnName() { Column = Name };
            return new ColumnName(this.TableName, Name);
        }

        public override ColumnName GetFullDbName()
        {
            if (this.TableDbName == null) return new ColumnName() { Entity = this.TableName, Column = DbName };
            return new ColumnName(this.TableDbName, DbName);
        }


        /// <summary>
        /// 如果DbName 有值，才有意义。
        /// </summary>
        /// <returns></returns>
        public SimpleColumn AsDbName()
        {
            if (this.DbName.HasValue() == false) return this;
            return this.As(this.DbName);
        }


    }
}
