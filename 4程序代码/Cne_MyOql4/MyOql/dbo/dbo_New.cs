using System;
using System.Data;
using System.Data.Common;
using MyCmn;
using System.Collections.Generic;
using System.Threading;

namespace MyOql
{
    public static partial class dbo
    {

        ///// <summary>
        ///// 判断两个列是否是同一列,要求： 表,列，别名，都要一致。
        ///// </summary>
        ///// <param name="one"></param>
        ///// <param name="two"></param>
        ///// <returns></returns>
        //public static bool NameEquals(SimpleColumn one, SimpleColumn two)
        //{
        //    if (one.EqualsNull()) return false;
        //    if (two.EqualsNull()) return false;
        //    if (one.DbType != two.DbType) return false;

        //    if (one.TableDbName.HasValue() != two.TableDbName.HasValue()) return false;
        //    else if (one.TableDbName.HasValue() && two.TableDbName.HasValue())
        //    {
        //        if (one.TableDbName != two.TableDbName) return false;
        //    }

        //    if (one.Name != two.Name) return false;
        //    if (one.DbName != two.DbName) return false;
        //    if (one.GetAlias() != two.GetAlias()) return false;

        //    return true;
        //}

        ///// <summary>
        ///// 判断该列是否与 Name 是同一列。(对于复合列，还要确认是否可以相等。)
        ///// </summary>
        ///// <remarks>
        ///// 相等的规则:
        ///// 1.如果 Column.Name 匹配, 则相等.
        ///// 2.如果 Column.Alias() 匹配,则相等.
        ///// 3.如果 Table.DbName + "." + Column.DbName 匹配,则相等
        ///// 4.如果 Table.Name + "." + Column.Name  匹配,则相等.
        ///// 
        ///// 其它情况则不等.
        ///// </remarks>
        ///// <param name="one"></param>
        ///// <param name="Name"></param>
        ///// <returns></returns>
        //public static bool NameEquals(SimpleColumn one, string Name)
        //{
        //    if (one.EqualsNull()) return false;
        //    if (Name.HasValue() == false) return false;
        //    if (one.Name == Name) return true;
        //    if (one.DbName == Name) return true;
        //    if (one.GetAlias() == Name) return true;

        //    if (one.TableDbName.HasValue())
        //    {
        //        if (one.TableDbName + "." + one.DbName == Name) return true;
        //        if (dbo.Event.Idbr.GetMyOqlEntity(one.TableDbName).GetName() + "." + one.Name == Name) return true;
        //    }
        //    return false;
        //}

    }
}
