using System.Collections.Generic;
using MyOql.Provider;

namespace MyOql
{
    ///<summary>
    /// 用法 见 CaseWhen
    ///</summary>
    /// <remarks>
    ///  
    /// </remarks>
    public interface IRawOql
    {
        string ToRawSql(TranslateSql provider,bool withTableAlias);
    }
}
