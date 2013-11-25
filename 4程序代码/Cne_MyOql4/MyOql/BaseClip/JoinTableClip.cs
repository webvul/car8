using System;
using MyCmn;

namespace MyOql
{
    /// <summary>
    /// 关系子句
    /// </summary>
    [Serializable]
    public class JoinTableClip : SqlClipBase
    {
        //[DataMember]

        private WhereClip _OnWhere;
        public WhereClip OnWhere
        {
            get
            {
                return _OnWhere;
            }
            set
            {
                //检查 是否是 自己 = 自己

                value.Recusion(w =>
                {
                    if (w.Query.IsSimple() && w.ValueType == WhereValueEnum.Column)
                    {
                        var v = w.Value as ColumnClip;
                        if (v.IsSimple())
                        {
                            if (w.Query.NameEquals(v))
                            {
                                throw new GodError("表连接中，不出现两个相同列的表达式:" + w.Query.GetFullName().ToString());
                            }
                        }
                    }
                    return true;
                });

                _OnWhere = value;
            }
        }
        //[DataMember]

        /// <summary>
        /// 与 SubSelect 互斥.
        /// </summary>
        public RuleBase Table { get; set; }

        public ContextClipBase SubSelect { get; set; }

        public JoinTableClip()
        {
            this.Key = SqlKeyword.Join;
        }

        /// <summary>
        /// 指定连接类型.
        /// </summary>
        /// <param name="JoinType">连接连接符必须是: Join,LeftJoin,RightJoin 其中之一!</param>
        public JoinTableClip(SqlKeyword JoinType)
        {
            dbo.Check(JoinType.IsIn(SqlKeyword.Join, SqlKeyword.LeftJoin, SqlKeyword.RightJoin) == false,
                "连接连接符必须是: Join,LeftJoin,RightJoin 其中之一!",
                this.Table);

            this.Key = JoinType;
        }

        //public JoinTableClip(SqlKeyword JoinType)
        //{
        //    this.Key = JoinType;
        //}

        public override object Clone()
        {
            var join = new JoinTableClip();
            join.Key = this.Key;
            join.OnWhere = this.OnWhere.Clone() as WhereClip;

            if (this.SubSelect != null)
            {
                join.SubSelect = this.SubSelect.Clone() as ContextClipBase;
            }

            if (this.Table != null)
            {
                join.Table = this.Table.Clone() as RuleBase;
            }

            return join;
        }
    }
}
