using System;
using System.Data;
using System.Data.Common;
using MyCmn;
using System.Collections.Generic;
using System.Threading;

namespace MyOql
{
    public static partial class dboColumn
    {
        /// <summary>
        /// 判断两个列是否是同一列。(对于复合列，还要确认是否可以相等。)
        /// </summary>
        /// <param name="Column"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static bool NameEquals(this ColumnClip Column, ColumnClip two)
        {
            if (Column.EqualsNull() || two.EqualsNull()) return false;
            if (Column.Key != two.Key) return false;
            if (Column.DbType != two.DbType) return false;
            if (Column.Key == SqlKeyword.Simple)
            {
                var oneSimple = Column as SimpleColumn;
                var twoSimple = two as SimpleColumn;

                if (oneSimple.DbName == null) return false;
                if (oneSimple.DbName == string.Empty) return false;
                if (oneSimple.TableDbName != twoSimple.TableDbName) return false;
                if (oneSimple.DbName != twoSimple.DbName) return false;
                if (oneSimple.TableName != twoSimple.TableName) return false;
                return true;
            }
            else if (Column.Key == SqlKeyword.Complex)
            {
                var oneComplex = Column as ComplexColumn;
                var twoComplex = two as ComplexColumn;

                if (oneComplex.Operator != twoComplex.Operator) return false;
                if (oneComplex.LeftExp.NameEquals(twoComplex.LeftExp) == false) return false;
                if (oneComplex.RightExp.NameEquals(twoComplex.RightExp) == false) return false;
                if (oneComplex.TableName != twoComplex.TableName) return false;
                return true;
            }
            else if (Column.Key == SqlKeyword.Const)
            {
                if (Column.Name != two.Name) return false;
                if (Column.TableName != two.TableName) return false;
                return true;
            }
            else if (Column.Key == SqlKeyword.Raw)
            {
                var oneRaw = Column as RawColumn;
                var twoRaw = two as RawColumn;
                if (oneRaw.FunctionFormat != twoRaw.FunctionFormat) return false;
                if (object.Equals(oneRaw.Parameter, twoRaw.Parameter)) return false;
                return true;
            }
            return true;
        }

        ///// <summary>
        ///// 严格意义上的比较 , 相等的条件是 表名 DbName 和列名 DbName 相等,  忽略别名比较.
        ///// </summary>
        ///// <param name="Column"></param>
        ///// <param name="two"></param>
        ///// <returns></returns>
        //public static bool ColumnEquals(this ColumnClip Column, ColumnClip two)
        //{
        //    return ColumnClip.ColumnEquals(Column, two);
        //}
        /// <summary>
        /// 根据名称判断 列是否是同一列。
        /// </summary>
        /// <param name="Column"></param>
        /// <param name="ColumnName"></param>
        /// <param name="WithDbName">是否启用 真实列名的比较</param>
        /// <returns></returns>
        /// <remarks>
        /// 在 Insert , Update 时, 要使用 DbName 比较更有效,而别名可能无效.如:
        /// dbr.Menu.Insert(o=> new {NAME = "MyOql" } );
        /// 
        /// 而在应用层查询时,使用 Name 比较有效.如:
        /// dbr.Menu.Select(o=>o.Name.As("Name1"))
        ///     .Join(dbr.User,(a,b)=>a.Id == b.Id , o=>o.Name.As("Name2") )
        ///     ;
        /// 在这时, 比较 DbName 是不正确的.应该忽略对  DbName 的比较.
        /// </remarks>
        public static bool NameEquals(this ColumnClip Column, string ColumnName, bool WithDbName = false)
        {
            if (string.IsNullOrEmpty(ColumnName)) return false;
            if (Column.EqualsNull()) return false;
            if (Column.Name == ColumnName) return true;

            if (Column.Key == SqlKeyword.Simple)
            {
                var oneSimple = Column as SimpleColumn;

                if (WithDbName)
                {
                    //insert ,update 语句,需要使用  DbName 做匹配.
                    if (oneSimple.DbName == ColumnName) return true;
                }

                if (oneSimple.TableName + "." + oneSimple.Name == ColumnName) return true;
                if (oneSimple.TableDbName + "." + oneSimple.DbName == ColumnName) return true;
                return false;
            }

            return false;
        }

        /// <summary>
        /// 生成 As 语句，别名采用 转义后的数据库表名.数据库列名。
        /// </summary>
        /// <returns></returns>
        public static T AsFullName<T>(this T column) where T : ColumnClip
        {
            var Alias = column.GetFullName().AsString();

            var clone = column.Clone() as T;
            clone.Name = Alias;
            return clone;
        }

        public static T AsFullDbName<T>(this T column) where T : ColumnClip
        {
            var Alias = column.GetFullDbName().AsString();

            var clone = column.Clone() as T;
            clone.Name = Alias;
            return clone;
        }

        /// <summary>
        /// 从别名中引用列.
        /// </summary>
        /// <param name="Alias"></param>
        /// <returns></returns>
        public static T FromTable<T>(this T column, string Alias) where T : ColumnClip
        {
            if (Alias.HasValue() == false) return column;
            var clone = column.Clone() as T;
            clone.TableName = Alias;
            //clone.Extend.Add(SqlOperator.FromTable, Alias);
            return clone;
        }

    }
}
