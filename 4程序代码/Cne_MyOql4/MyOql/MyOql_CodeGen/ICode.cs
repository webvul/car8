using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Collections;

namespace MyOql.MyOql_CodeGen
{
    public enum CodeLanguage
    {
        /// <summary>
        /// C#.Net 语言
        /// </summary>
        CsNet,

        /// <summary>
        /// Vb.Net 语言
        /// </summary>
        VbNet,
    }


    /// <summary>
    /// 以后，生成器可以使用
    /// </summary>
    /// <param name="groupList"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public interface ICode
    {
        string ToCode(CodeLanguage Language = CodeLanguage.CsNet);
    }
}
