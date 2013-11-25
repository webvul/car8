using System;

namespace MyOql
{
    /// <summary>
    /// Column 的列表示.
    /// </summary>
    [Serializable]
    public class ColumnName:ICloneable
    {
        public string Entity { get; set; }
        public string Column { get; set; }

        public ColumnName()
        {
        }

        public ColumnName(string Entity, string Column)
        {
            this.Entity = Entity;
            this.Column = Column;
        }

        public override string ToString()
        {
            if (Entity == null) return Column;
            return Entity + "." + Column;
        }

        public virtual  object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    [Serializable]
    public class FkColumn:ColumnName 
    {
        public bool CascadeUpdate { get; set; }
        public bool CascadeDelete { get; set; }

        /// <summary>
        /// 外键引用表
        /// </summary>
        public string RefTable { get; set; }

        /// <summary>
        /// 外键引用表的引用列。
        /// </summary>
        public string RefColumn { get; set; }


        public override object Clone()
        {
            return this.MemberwiseClone();
        }

        public override string ToString()
        {
            //CorpStanID(ud)=TM_CorpCost_Standard:CorpStanID
            return string.Format(@"{0}{1}={2}:{3}", this.Column,
                (this.CascadeDelete || this.CascadeUpdate) ? "(" + (this.CascadeUpdate ? "u" : "") + (this.CascadeDelete ? "d" : "") + ")" : "",
                this.RefTable, this.RefColumn);
        }
    }
}
