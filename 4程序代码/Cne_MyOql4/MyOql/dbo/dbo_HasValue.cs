using System;
using System.Collections;
using System.Collections.Generic;
using MyCmn;

namespace MyOql
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class dbo
    {
        /// <summary>
        /// 判断 MyOqlSet 是否有数据。
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public static bool HasData(this MyOqlSet set)
        {
            if (set == null) return false;
            if (set.Rows == null) return false;
            if (set.Columns.Length == 0) return false;
            if (set.Rows.Count == 0) return false;
            return true;
        }


        public static bool HasData(this RuleBase rule)
        {
            if (rule == null) return false;
            if (rule.GetDbName().HasValue() == false) return false;
            return true;
        }

        /// <summary>
        /// 是否为有实际排序。
        /// </summary>
        /// <param name="Order"></param>
        /// <returns></returns>
        public static bool HasData(this OrderByClip Order, Action<OrderByClip> HasValueAction)
        {
            if (HasData(Order))
            {
                HasValueAction(Order);
                return true;
            }
            else return false;
        }


        /// <summary>
        /// 在 C# 中 ,与 null 进行值比较.
        /// </summary>
        /// <param name="Column"></param>
        /// <returns></returns>
        public static bool EqualsNull(this ColumnClip Column)
        {
            return object.Equals(Column, null);
        }


        /// <summary>
        /// 只判断当前，不判断Next数据。
        /// </summary>
        /// <param name="Order"></param>
        /// <returns></returns>
        public static bool HasData(this OrderByClip Order)
        {
            if (Order == null) return false;
            if (Order.PostOrderName.HasValue()) return true;
            if (Order.Order.EqualsNull()) return false;
            return Order.Order.Name.HasValue();
        }

        public static bool HasData(this ColumnClip col)
        {
            return !col.EqualsNull();
        }

        /// <summary>
        /// 判断Where 是否为空.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public static bool IsNull(this WhereClip where)
        {
            return where == null || (where.Query.EqualsNull() && IsNull(where.Child));
        }

        public static bool HasData(this WhereClip where)
        {
            return !where.IsNull();
        }
    }
}
