//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using MyCmn;
//using MyOql;
//using MyBiz;
//using MyOql.Helper;


//namespace MyBiz
//{
//    public enum ExcelTemplateEnum
//    {
//        撤销实收费用,
//        A,
//    }

//    public struct XmlExcelDefine
//    {
//        public XmlExcelColumnList ColumnMap { get; set; }
//        public ExcelTemplateEnum Type { get; set; }
//        public string TableDbName { get; set; }

//        /// <summary>
//        /// Key表示列映射的别名，Value表示数据库的列名。
//        /// </summary>
//        public string[] PrimaryKeys { get; set; }

//        /// <summary>
//        /// 外键定义，（由于数据库中可能不会定义外键，所以需要指定）其中，Key表示外键(别名)，Value借用了MyFkNode对象，Name表示引用表的显示列（DbName），RefTable表示引用表（DbName）RefColumn表示引用列（DbName）。
//        /// </summary>
//        public Dictionary<string, MyFkNode> ForeignKey { get; set; }
//    }

//    public struct XmlExcelColumn
//    {
//        public string Name { get; set; }
//        public string DbName { get; set; }
//        public string Caption { get; set; }
//    }

//    public class XmlExcelColumnList : List<XmlExcelColumn>
//    {
//        public void Add(string Name, string DbName, string Caption)
//        {
//            this.Add(new XmlExcelColumn { Name = Name, DbName = DbName, Caption = Caption });
//        }
//    }

//    public sealed partial class ExcelDefine
//    {
//        public List<XmlExcelDefine> list { get; set; }

//        private static ExcelDefine _ExcelDefineBase = new ExcelDefine();
//        public static ExcelDefine GetInstance() { return _ExcelDefineBase; }

//        private ExcelDefine()
//        {
//            list = new List<XmlExcelDefine>();
//            this.list.Add(GetA());
//        }
//    }
//}

