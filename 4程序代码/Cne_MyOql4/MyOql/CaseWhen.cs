using System;
using System.Collections.Generic;
using System.Linq;

namespace MyOql
{

    //[DataContract]
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 该类不记录CRUD事件. 在子类中记录事件
    /// </remarks>
    [Serializable]
    public partial class CaseWhen : IRawOql
    {
        public Dictionary<WhereClip, ColumnClip> WhenData { get; set; }
        public ColumnClip ElseData { get; set; }

        public CaseWhen(WhereClip Where, ColumnClip Column)
        {
            WhenData = new Dictionary<WhereClip, ColumnClip>();
            WhenData.Add(Where, Column.Clone() as ColumnClip);
        }

        public CaseWhen WhenThen(WhereClip Where, ColumnClip Column)
        {
            WhenData.Add(Where, Column.Clone() as ColumnClip);
            return this;
        }

        public ColumnClip ElseEnd(ColumnClip Column)
        {
            ElseData = Column.Clone() as ColumnClip;
            return ToColumn();
        }

        public ComplexColumn ToColumn()
        {
            var retVal = new ComplexColumn();
            retVal.DbType = ElseData.DbType;
            retVal.Operator = SqlOperator.CaseWhen;
            retVal.LeftExp = GetWhenThen(0, null);
            retVal.RightExp = ElseData;
            return retVal;
        }

        /// <summary>
        /// 返回的 RightExp 永远是下一个 WhenThen 复合表达式
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private ComplexColumn GetWhenThen(int level, ComplexColumn oriColumn)
        {
            if (WhenData.Count <= level) return oriColumn;
            var item = WhenData.ElementAt(level);
            var retVal = new ComplexColumn();
            retVal.DbType = ElseData.DbType;
            if (dbo.EqualsNull(oriColumn))
            {
                retVal.Operator = SqlOperator.WhenThen;
                retVal.LeftExp = new RawColumn(ValueMetaTypeEnum.MyOqlType, item.Key);
                retVal.RightExp = item.Value;
                return GetWhenThen(level + 1, retVal);
            }
            else
            {
                var current = new ComplexColumn();
                current.DbType = ElseData.DbType;
                current.Operator = SqlOperator.WhenThen;
                current.LeftExp = new RawColumn(ValueMetaTypeEnum.MyOqlType, item.Key);
                current.RightExp = item.Value;


                retVal.Operator = SqlOperator.Blank;
                retVal.LeftExp = oriColumn;
                retVal.RightExp = current;
                return GetWhenThen(level + 1, retVal);
            }
        }

        public string ToRawSql(Provider.TranslateSql provider, bool withTableAlias)
        {
            var sl = new List<string>();
            this.WhenData.All(w =>
            {
                sl.Add(string.Format(provider.GetOperator(SqlOperator.WhenThen),
                    provider.ToParaValues(provider.GetWhereText(w.Key).ToFullSql(), provider.GetColumnExpression(w.Value))
                    ));
                return true;
            });

            var ret = string.Format(provider.GetOperator(SqlOperator.CaseWhen), string.Join(" ", sl.ToArray()),
                this.ElseData.EqualsNull() ? string.Empty : string.Format(provider.GetOperator(SqlOperator.Else), provider.GetColumnExpression(this.ElseData))
                );
            return ret;
        }
    }
}
