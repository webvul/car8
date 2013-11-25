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
//    public partial class ExcelDefine
//    {
//        private XmlExcelDefine GetA()
//        {
//            var colMap = new XmlExcelColumnList { 
//                                            {"CustName","", "客户名称"},
//                                            {"RoomSign","", "房屋编号"},
//                                            {"ChargeDate","", "收款时间"},
//                                            {"BillsSign","", "收据号码"},
//                                            {"FeesDueDate","", "费用日期"},
//                                            {"FeesStateDate","", "开始日期"},
//                                            {"FeesEndDate","", "结束日期"},
//                                            {"CostName","", "费用名称"},
//                                            {"DueAmount","", "收款金额"},
//                                            {"LateFeeAmount","", "合同违约金"},
//                                            {"UserName","", "收款人"},
//                                            {"FeesMemo","", "备注"},
//                                            {"AuditDate","", "撤销时间"},
//                                            {"ReferReason","", "撤销原因"}
//                                            };

//            var fks = new Dictionary<string, MyFkNode>();

//            fks["UserName"] = new MyFkNode() { Column = "User_Name", RefTable = "T_User", RefColumn = "User_ID" };

//            return new XmlExcelDefine()
//            {
//                ColumnMap = colMap,
//                ForeignKey = fks,
//                PrimaryKeys = new string[] { "CustName" },
//                TableDbName = "TP_dfd",
//                Type = ExcelTemplateEnum.撤销实收费用
//            };

//        }

//    }
//}

