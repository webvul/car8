//using System;
//using System.Collections.Generic;
//using System.Linq;
//using MyCmn;
//using System.ComponentModel;

//namespace MyOql.MyOql_CodeGen
//{
//    public class TableDefineInfo
//    {
//        public string db { get; set; }
//        public string Name { get; set; }
//        public string Description { get; set; }
//    }

//    public class ColumnDefineInfo
//    {
//        public string TableName { get; set; }
//        public string ColumnName { get; set; }
//        public string SqlType { get; set; }
//        public bool Nullable { get; set; }
//        public int Precision { get; set; }
//        public int Length { get; set; }
//        public int Scale { get; set; }
//    }

//    public class ColumnDefineDetail : ColumnDefineInfo
//    {
//        /// <summary>
//        /// 冗余字段。
//        /// </summary>
//        public string db { get; set; }
//        public string Description { get; set; }
//    }
//}
