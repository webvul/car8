
using System;
using MyCmn;
using System.Linq;

namespace MyOql
{
    /// <summary>
    /// 当有Table的时候，表示指定表里的列。 当没有Table的时候，表示常数列。
    /// </summary>
    [Serializable]
    public class ConstColumn : ColumnClip
    {
        public string DbName { get; set; }

        private ConstColumn()
        {
            this.Key = SqlKeyword.Const;
        }

        /// <summary>
        /// 如果以 # 开头，则DbName会去掉第一个#
        /// </summary>
        /// <param name="DbName"></param>
        public ConstColumn(string DbName)
            : this()
        {
            this.DbName = DbName;
            this.Name = this.DbName;
            this.DbType = System.Data.DbType.AnsiString;

            if (this.DbName != null && this.DbName.StartsWith("#"))
            {
                this.DbName = this.DbName.Substring(1);
            }
        }

        public ConstColumn(int DbName)
            : this()
        {
            this.DbName = DbName.ToString();
            this.Name = this.DbName;
            this.DbType = System.Data.DbType.Int32;
        }

        public ConstColumn(double DbName)
            : this()
        {
            this.DbName = DbName.ToString();
            this.Name = this.DbName;
            this.DbType = System.Data.DbType.Double;
        }

        public ConstColumn(long DbName)
            : this()
        {
            this.DbName = DbName.ToString();
            this.Name = this.DbName;
            this.DbType = System.Data.DbType.Int64;
        }

        public ConstColumn(decimal DbName)
            : this()
        {
            this.DbName = DbName.ToString();
            this.Name = this.DbName;
            this.DbType = System.Data.DbType.Decimal;
        }


        internal ConstColumn(object DbName)
            : this()
        {
            this.DbName = DbName.ToString();
            this.Name = this.DbName;

            this.DbType = DbName.GetType().GetDbType();
        }

        /// <summary>
        /// 直接使用SQL,尽量不要使用.
        /// </summary>
        /// <param name="DbName"></param>
        /// <returns></returns>
        public static ConstColumn CreateSystemColumn(string DbName)
        {
            ConstColumn retVal = new ConstColumn(DbName);
            retVal.DbType = System.Data.DbType.Object;
            return retVal;
        }

        public ConstColumn(DateTime DbName)
            : this()
        {
            this.DbName = DbName.ToString("yyyy-MM-dd HH:mm:ss");
            this.Name = this.DbName;
            this.DbType = System.Data.DbType.Date;
        }

        public override ColumnName GetFullDbName()
        {
            return new ColumnName { Column = this.DbName };
        }

        public override ColumnName GetFullName()
        {
            return new ColumnName { Column = this.DbName };
        }

        protected override object CloneClip()
        {
            var clone = new ConstColumn();
            clone.DbName = this.DbName;
            clone.Name = this.Name;
            clone.DbType = this.DbType;
            clone.Length = this.Length;

            return clone;
        }
    }
}
