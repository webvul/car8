//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Web;
//using System.Web.Mvc;
//using MyCmn;
//using MyCon;
//using MyOql;
//using MyWeb.Areas.Host.Models;
//using DbEnt;
//using MyBiz.Admin;
//namespace MyWeb.Areas.Host.Controllers
//{
//    public partial class ToolController : MyMvcController
//    {

//        public ActionResult ExportCustomerResult()
//        {


//            MyOqlSet set = dbr.TempRoomowner.Select().Where(o => o.Id < 0).ToMyOqlSet();
//            StringDict dic = new StringDict();

//            dic.Add("BuildingName", "楼宇名称");
//            dic.Add("FloorNum", "楼层号");
//            dic.Add("UnitNum", "单元号");
//            dic.Add("RoomCode", "房间编码");
//            dic.Add("ScBldArea", "建筑面积");
//            dic.Add("ScTnArea", "套内面积");
//            dic.Add("PropertyTakeoverDate", "物业接管时间");
//            dic.Add("LoginName", "登录名称");
//            dic.Add("OwnerName", "住户名称");
//            dic.Add("OwnerSex", "性别");
//            dic.Add("LinkMobile", "联系人电话");
//            dic.Add("IdNumber", "身份证号");
//            dic.Add("Email", "邮箱");
//            dic.Add("MailAddress", "邮寄地址");



//            return set.ExportToExcel("住户Excel模版.xls", dic);
//        }

//        public ActionResult LoadCustomer(string AnnexId, string SheetName, string CommID)
//        {
//            dbr.TempRoomowner.Delete(o => o.CommId == CommID).Execute();//删除以前的数据
//            var data = LoadExcel(AnnexId.AsInt(), SheetName);
//            MyOqlSet set = new MyOqlSet(dbr.TempRoomowner);
//            set = data.FromJson<MyOqlSet>();
//            JsonMsg jm = MyBiz.Admin.ToolBiz.AutoSaveCustomer(set, CommID.AsInt());
//            //set.Entity = dbr.TempRoomowner;
//            //set.Rows = set.Rows.Take(10).ToList();

//            //set.Count = 10;
//            //set.Columns = set.Columns.AddOne(new ConstColumn("CustomerID")).ToArray();

//            //set.Rows.All(o =>
//            //{
//            //    int i = 1;
//            //    o["CustomerID"] = i;
//            //    i++;
//            //    return true;
//            //});

//            //Mvc.Model["FlexiGrid_id"] = "CustomerID";

//            //Mvc.Model["FlexiGrid_cols"] = @"CustomerID,楼宇名称,楼层号,楼栋号,房间编码,建筑面积,套内面积,物业接管时间,登录名称,住户名称,性别,联系人电话,身份证号,邮箱,邮寄地址";

//            return jm;
//        }

//        public ActionResult QueryCustomer(ToolBiz.ExcelCustomerModel Query)
//        {
//            WhereClip where = new WhereClip();

//            Query.CommID.HasValue(o => where &= dbr.TempRoomowner.CommId == o);
//            var set = dbr.TempRoomowner.Select()
//                  .Where(where)
//                  .Skip(Query.skip)
//                  .Take(Query.take)
//                  .OrderBy(Query.sort)
//                  .ToMyOqlSet();

//            set.InsertColumn(new ConstColumn("IsMulti"), (rowIndex, rowData) => string.Empty);

//            set.Rows.All(o =>
//            {
//                if (ToolBiz.ValiMultiLoginName(o["LoginName"].AsString()))
//                    o["IsMulti"] = "Yes";
//                else
//                    o["IsMulti"] = "";
//                return true;
//            });

//            return set.LoadFlexiGrid();
//        }


//        [HttpPost]
//        public ActionResult ImportCustomer(string AnnexId, string SheetName, string CommID)
//        {
//            var data = LoadExcel(AnnexId.AsInt(), SheetName);

//            MyOqlSet set = new MyOqlSet(dbr.TempRoomowner);
//            set = data.FromJson<MyOqlSet>();
//            return MyBiz.Admin.ToolBiz.AutoSaveCustomer(set, CommID.AsInt());
//        }

//        [HttpPost]
//        public ActionResult RunSpImportRoomOwner(string CommID)
//        {
//            JsonMsg jm = new JsonMsg();
//            if (ToolBiz.IsHasMultiLoginName(CommID.AsInt()))
//            {
//                jm.msg = "存在相同登陆名的用户,请检查!";
//                return jm;
//            }
//           dbr.LongFor.SpImportRoomOwner(CommID);
//            return jm;
//        }



//        public string LoadExcel(int AnnexId, string SheetName)
//        {
//            var client = new OleDbServiceClient();

//            var annexEnt = dbr.Annex.FindById(AnnexId);
//            var excelPath = Server.MapPath("~/upload/" + annexEnt.Name);

//            try
//            {
//                return client.ReadAsMyOqlSet(excelPath, SheetName);
//            }
//            catch { throw new GodError("工作表不存在！请重新输入。"); }
//        }
//    }
//}
