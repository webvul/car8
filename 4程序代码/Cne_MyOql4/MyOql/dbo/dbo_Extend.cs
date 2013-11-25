using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using System.Data.Common;
using System.Configuration;

namespace MyOql
{
    public static partial class dbo
    {
        /// <summary>
        /// 给表设置别名.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rule"></param>
        /// <param name="Alias"></param>
        /// <returns></returns>
        public static T As<T>(this T rule, string Alias) where T : class,IAsable, ICloneable
        {
            if (Alias.HasValue() == false) return rule;

            // Select 子句不需要克隆
            var select = rule as SelectClip;
            if (select != null)
            {
                rule.SetAlias(Alias);
                return rule;
            }

            //表和列需要克隆。
            var clone = rule.Clone() as T;
            clone.SetAlias(Alias);

            var t = clone as RuleBase;
            if (t != null)
            {
                //该表的所有列，都添加别名。
                t.GetColumns().All(o =>
                    {
                        o.FromTable(Alias);
                        return true;
                    });
            }

            return clone;
        }








        public static CaseWhen CaseWhen(WhereClip Where, ColumnClip Column)
        {
            return new CaseWhen(Where, Column);
        }


        /// <summary>
        /// 对列集合进行别名设置，别名采用转义后的 数据库表名.数据库列名 （转义）。
        /// </summary>
        /// <param name="Columns"></param>
        /// <returns></returns>
        public static IEnumerable<ColumnClip> AsFullName(this IEnumerable<ColumnClip> Columns)
        {
            if (Columns == null) return null;
            return Enumerable.Select(Columns, o => "#" + o.AsFullName());
        }


        /// <summary>
        /// 对列集合进行别名设置，别名采用转义后的 数据库表名.数据库列名 （转义）。
        /// </summary>
        /// <param name="Columns"></param>
        /// <param name="Prefix">别名前缀</param>
        /// <returns></returns>
        public static IEnumerable<ColumnClip> AsFullName(this IEnumerable<ColumnClip> Columns, string Prefix)
        {
            if (Columns == null) return null;
            if (Prefix.StartsWith("#") == false)
            {
                Prefix = "#" + Prefix;
            }

            return Enumerable.Select(Columns, o => o.As(Prefix + o.GetFullName()));
        }


        public static IEnumerable<ColumnClip> AsFullDbName(this IEnumerable<ColumnClip> Columns)
        {
            if (Columns == null) return null;
            return Enumerable.Select(Columns, o => o.As("#" + o.GetFullDbName().AsString()));
        }

        /// <summary>
        /// 对列集合进行别名设置，别名采用数据库列名（不转义）
        /// </summary>
        /// <param name="Columns"></param>
        /// <returns></returns>
        public static IEnumerable<SimpleColumn> AsDbName(this IEnumerable<SimpleColumn> Columns)
        {
            if (Columns == null) return null;
            return Enumerable.Select(Columns, o => o.As("#" + o.DbName) as SimpleColumn);
        }

        /// <summary>
        /// 数据库系统函数，返回第一个非空值。
        /// </summary>
        /// <param name="cols"></param>
        /// <returns></returns>
        public static ComplexColumn Coalesce(ColumnClip one, ColumnClip two)
        {
            var col = new ComplexColumn("Coalesce");
            col.LeftExp = one.Clone() as ColumnClip;
            col.Operator = SqlOperator.Coalesce2;
            col.RightExp = two.Clone() as ColumnClip;
            return col;
        }

        public static ComplexColumn Coalesce(ColumnClip one, ColumnClip two, ColumnClip three)
        {
            var val = new ComplexColumn();
            val.Operator = SqlOperator.Parameter;
            val.LeftExp = two.Clone() as ColumnClip;
            val.RightExp = three.Clone() as ColumnClip;


            var col = new ComplexColumn("Coalesce");
            col.LeftExp = one.Clone() as ColumnClip;
            col.Operator = SqlOperator.Coalesce3;
            col.RightExp = val;
            //col.Extend[SqlOperator.FunctionParameter] = three.Clone();
            return col;
        }

        public static ComplexColumn Coalesce(ColumnClip one, ColumnClip two, ColumnClip three, ColumnClip four)
        {
            var val = new ComplexColumn(SqlOperator.Parameter);
            val.LeftExp = two.Clone() as ColumnClip;
            val.RightExp = three.Clone() as ColumnClip;


            var val2 = new ComplexColumn(SqlOperator.Parameter);
            val2.LeftExp = val;
            val2.RightExp = four.Clone() as ColumnClip;


            var col = new ComplexColumn("Coalesce");
            col.LeftExp = one.Clone() as ColumnClip;
            col.Operator = SqlOperator.Coalesce4;
            col.RightExp = val2;
            //col.Extend[SqlOperator.FunctionParameter] = three.Clone();
            //col.Extend[SqlOperator.FunctionParameter] = four.Clone();
            return col;
        }

        public static ComplexColumn Coalesce(ColumnClip one, ColumnClip two, ColumnClip three, ColumnClip four, ColumnClip five)
        {
            var val = new ComplexColumn(SqlOperator.Parameter);
            val.LeftExp = two.Clone() as ColumnClip;
            val.RightExp = three.Clone() as ColumnClip;


            var val2 = new ComplexColumn(SqlOperator.Parameter);
            val2.LeftExp = val;
            val2.RightExp = four.Clone() as ColumnClip;

            var val3 = new ComplexColumn(SqlOperator.Parameter);
            val3.LeftExp = val2;
            val3.RightExp = five.Clone() as ColumnClip;


            var col = new ComplexColumn("Coalesce");
            col.LeftExp = one.Clone() as ColumnClip;
            col.Operator = SqlOperator.Coalesce5;
            col.RightExp = val3;
            //col.Extend[SqlOperator.FunctionParameter] = three.Clone();
            //col.Extend[SqlOperator.FunctionParameter] = four.Clone();
            //col.Extend[SqlOperator.FunctionParameter] = five.Clone();
            return col;
        }

        /// <summary>
        /// 得到行唯一列表述。
        /// </summary>
        /// <param name="Seperator"></param>
        /// <param name="Columns"></param>
        /// <returns></returns>
        public static ColumnClip GetUniqueExpression(char Seperator, params ColumnClip[] Columns)
        {
            /* select (cast isnull(len(id1),0) as varchar(5)) + "," +  (cast isnull(len(id2),0) as varchar(5))
             * + "." 
             * + cast(id1 as varchar(3000)) + cast(id2 as varchar(3000)) + cast( isnull( len(id3),0) as varchar(4000) )
             */


            if (Columns == null || Columns.Length == 0)
            {
                return null;
            }
            else if (Columns.Length == 1) return Columns[0].Clone() as ColumnClip;
            else
            {
                List<ColumnClip> list = new List<ColumnClip>();

                for (int i = 0; i < Columns.Length - 1; i++)
                {
                    if (i > 0) list.Add(new ConstColumn(Seperator.ToString()));
                    list.Add(Columns[i].Len().IsNull(0).Cast(System.Data.DbType.AnsiString));
                }

                list.Add(new ConstColumn(Seperator.ToString()));

                for (int i = 0; i < Columns.Length; i++)
                {
                    list.Add(Columns[i].Cast(System.Data.DbType.AnsiString));
                }

                ColumnClip col = list[0];

                for (int i = 1; i < list.Count; i++)
                {
                    col = col + list[i];
                }

                return col;
            }
        }


        public static string GetUniqueExpression(char Seperator, params string[] Columns)
        {
            /* select (cast isnull(len(id1),0) as varchar(5)) + "," +  (cast isnull(len(id2),0) as varchar(5))
             * + "." 
             * + cast(id1 as varchar(3000)) + cast(id2 as varchar(3000)) + cast( isnull( len(id3),0) as varchar(4000) )
             */


            if (Columns == null || Columns.Length == 0)
            {
                return null;
            }
            else if (Columns.Length == 1) return Columns[0];
            else
            {
                List<string> list = new List<string>();

                for (int i = 0; i < Columns.Length - 1; i++)
                {
                    if (i > 0) list.Add(Seperator.ToString());
                    list.Add(Columns[i].Length.ToString());
                }

                list.Add(Seperator.ToString());

                for (int i = 0; i < Columns.Length; i++)
                {
                    list.Add(Columns[i]);
                }

                return string.Join("", list.ToArray());
            }
        }



        public static WhereClip GetOnWhere(RuleBase table, RuleBase refTable)
        {
            var fks = dbo.Event.Idbr.GetFkDefines();
            GodError.Check(fks == null, () => "自动表连接时发现 MyOql 没有定义外键关系:" + table.GetFullName() + "=>" + refTable.GetFullName());

            var fkList = fks.Where(o => o.Entity == table.GetDbName() && o.RefTable == refTable.GetDbName()).ToArray();
            if (fkList.Length == 0)
            {
                throw new GodError("自动表连接,没有发现主表和引用表的外键关系:" + table.GetFullName() + "=>" + refTable.GetFullName());
            }
            else if (fkList.Length > 1)
            {
                throw new GodError("自动表连接,需要主表和引用表仅有一个外键关系,但却定义了多个外键关系:" + string.Join(",", fkList.Select(o => o.ToString()).ToArray()));
            }

            var fk = fkList[0];
            return table.GetColumn(fk.Column) == refTable.GetColumn(fk.RefColumn);
        }



    }
}
