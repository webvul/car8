using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using MyCmn;

namespace MyOql
{

    /// <summary>
    /// 类型映射表. 
    /// </summary>
    /// <remarks>
    /// 注意: 
    ///     1. 生成 Command 时, Command 是按 DbType 的, 所以, DbType 一定要全.
    ///     2. 从任何一个实现,查找 DbType 对应的 CsType 是一样的.
    ///     3. 从任何一个实现,查找 CsType 对应的 Dbtype 是一样的.
    /// 另外：
    ///     1. DbType.String 与 DbType.AnsiString 在程序里,同样对待.
    ///     2. DbType.StringFixedLength 是固定字符长度
    ///     3. 并不能从 DbType 能转换到所有的 数据库类型, 如 MySqlDbType.Enum . 这些需要手动转换. 
    /// </remarks>
    //public class TypeMap
    //{
    //    public DbType DbType { get; set; }
    //    public Type CsType { get; set; }
    //    public Enum SqlType { get; set; }
    //    //public string SqlDb{ get; set; }

    //    public TypeMap(DbType Dbtype, Type Cstype, Enum Sqltype)
    //    {
    //        this.DbType = Dbtype;
    //        this.CsType = Cstype;
    //        this.SqlType = Sqltype;
    //    }
    //}

    //public class TypedDict<CSTYPE> : TypeMap
    //{
    //    //public DbType DbType { get; set; }
    //    //protected CSTYPE _CsType { get; set; }

    //    public TypedDict(DbType Dbtype, Enum Sqltype)
    //        : base(Dbtype, typeof(CSTYPE), Sqltype)
    //    {
    //        this.DbType = Dbtype;
    //        this.SqlType = Sqltype;
    //    }
    //}

    //public static class TypeMapHelper
    //{
    //    //public static TypeMap FirstSqlType(this IEnumerable<TypeMap> Source, Enum SqlType)
    //    //{
    //    //    return FirstSqlType(Source, SqlType.ToString());
    //    //}

    //    //public static TypeMap FirstSqlType(this IEnumerable<TypeMap> Source, string SqlType)
    //    //{
    //    //    var typeMap = Source.FirstOrDefault(t => string.Equals(t.SqlType.ToString(), SqlType, StringComparison.CurrentCultureIgnoreCase));

    //    //    if (typeMap != null) return typeMap;
    //    //    else typeMap = Source.FirstOrDefault(t => string.Equals(t.DbType.ToString(), SqlType, StringComparison.CurrentCultureIgnoreCase));

    //    //    if (typeMap != null) return typeMap;
    //    //    else throw new GodError("找不到数据类型:" + SqlType);
    //    //}

    //    //public static DbType FirstDbTypeByCsType(this IEnumerable<TypeMap> Source, Type CsType)
    //    //{
    //    //    var typeFullName = string.Empty ;
    //    //    if (CsType.IsNullableType())
    //    //    {
    //    //        typeFullName = CsType.GetGenericArguments()[0].FullName;
    //    //    }
    //    //    else
    //    //    {
    //    //        typeFullName = CsType.FullName;
    //    //    }

    //    //    var typeMap = Source.FirstOrDefault(t => t.CsType.FullName == typeFullName);

    //    //    if (typeMap != null) return typeMap.DbType;
    //    //    else throw new GodError("找不到数据类型:" + CsType.FullName);
    //    //}
    //}
}
