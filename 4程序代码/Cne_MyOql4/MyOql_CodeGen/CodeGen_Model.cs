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
    public interface ISqlType
    {
        string SqlType { get; set; }
        int Precision { get; set; }
        int Length { get; set; }
        int Scale { get; set; }

        DbType DbType { get; set; }
        string CsType { get; set; }
    }

    public class TableDefineInfo
    {
        public string db { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ColumnDefineDetail : ISqlType
    {
        public string TableDbName { get; set; }
        public string ColumnDbName { get; set; }
        public string SqlType { get; set; }
        public bool Nullable { get; set; }
        public int Precision { get; set; }
        public int Length { get; set; }
        public int Scale { get; set; }
    
        /// <summary>
        /// 冗余字段。
        /// </summary>
        public string db { get; set; }
        public string Description { get; set; }

        public string MapName { get; set; }

        public string CsType { get; set; }
        public DbType DbType { get; set; }
    }


    public class ProcParaDetail : ISqlType
    {
        public string ProcDbName { get; set; }
        public string ParaDbName { get; set; }
        public string SqlType { get; set; }
        public bool Nullable { get; set; }
        public int Precision { get; set; }
        public int Length { get; set; }
        public int Scale { get; set; }

        public ParameterDirection Direction { get; set; }

        /// <summary>
        /// 冗余字段。
        /// </summary>
        public string db { get; set; }
        public string Description { get; set; }

        public string MapName { get; set; }

        public string CsType { get; set; }
        public DbType DbType { get; set; }
    }
}
