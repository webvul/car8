using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCmn;
using MyOql;
using MyBiz;
using DbEnt;
using System.Text;


namespace MyBiz.Admin
{
    public partial class ToolBiz
    {
        public class ExcelCustomerModel : QueryModelBase
        {
            public int CommID { get; set; }
        }

        public static bool ValiMultiLoginName(string LoginName)
        {
            return dbr.TRoomOwner.FindScalar(o => o.Count(), o => o.LoginName == LoginName).AsInt().AsBool();
        }

        public static bool IsHasMultiLoginName(int CommID )
        {
            var set = dbr.TempRoomowner.Select(o => o.LoginName).Where(o => o.CommId == CommID).ToDictList();

            foreach (var item in set)
            {
                if (ValiMultiLoginName(item["LoginName"].AsString()))
                    return true;
            }
            return false;
        }

        public static JsonMsg AutoSaveCustomer(MyOqlSet set, int CommID)
        {
            JsonMsg jm = new JsonMsg();
            if (!CommID.HasValue())
            {
                jm.msg = "超时,请重新载入页面!";
                return jm;
            }
            //var indexBuildName = set.Columns.IndexOf(o => o.Name == "楼宇名称");
            //var indexFloorNum = set.Columns.IndexOf(o => o.Name == "楼层号");
            //var indexUnitNum = set.Columns.IndexOf(o => o.Name == "楼栋号");
            //var indexRoomCode = set.Columns.IndexOf(o => o.Name == "房间编码");
            //var indexScBidArea = set.Columns.IndexOf(o => o.Name == "建筑面积");
            //var indexScTnArea = set.Columns.IndexOf(o => o.Name == "套内面积");
            //var indexPropertyTakeoverDate = set.Columns.IndexOf(o => o.Name == "物业接管时间");
            //var indexLoginName = set.Columns.IndexOf(o => o.Name == "登录名称");
            //var indexOwnerName = set.Columns.IndexOf(o => o.Name == "住户名称");
            //var indexOwnerSex = set.Columns.IndexOf(o => o.Name == "性别");
            //var indexLinkMobile = set.Columns.IndexOf(o => o.Name == "联系人电话");
            //var indexIdNumber = set.Columns.IndexOf(o => o.Name == "身份证号");
            //var indexEmail = set.Columns.IndexOf(o => o.Name == "邮箱");
            //var indexMailAddress = set.Columns.IndexOf(o => o.Name == "邮寄地址");


            string TipFormat = "第{0}行导入失败 失败原因:{1}<br/>";
            StringBuilder Sb = new StringBuilder();
            int RowNum = 0;

            foreach (var row in set.Rows)
            {
                RowNum++;
                var BuildName = row["楼宇名称"].AsString();
                var UnitNum = row["单元号"].AsString();
                var FloorNum = row["楼层号"].AsString();                
                var RoomCode = row["房间编码"].AsString();
                var ScBidArea = row["建筑面积"].AsString();
                var ScTnArea = row["套内面积"].AsString();
                var PropertyTakeoverDate = row["物业接管时间"].AsString();
                var LoginName = row["登录名称"].AsString();
                var OwnerName = row["住户名称"].AsString();
                var OwnerSex = row["性别"].AsString();
                var LinkMobile = row["联系人电话"].AsString();
                var IdNumber = row["身份证号"].AsString();
                var Email = row["邮箱"].AsString();
                var MailAddress = row["邮寄地址"].AsString();




                if (!BuildName.HasValue())
                {
                    Sb.Append(string.Format(TipFormat, RowNum.AsString(), "请填写楼宇名称"));
                    continue;
                }
                if (!FloorNum.HasValue())
                {
                    Sb.Append(string.Format(TipFormat, RowNum.AsString(), "请填写楼层号"));
                    continue;
                }

                if (!UnitNum.HasValue())
                {
                    Sb.Append(string.Format(TipFormat, RowNum.AsString(), "请填写单元号"));
                    continue;
                }
                if (!RoomCode.HasValue())
                {
                    Sb.Append(string.Format(TipFormat, RowNum.AsString(), "请填写房间编码"));
                    continue;
                }
                if (!ScBidArea.HasValue())
                {
                    Sb.Append(string.Format(TipFormat, RowNum.AsString(), "请建筑面积"));
                    continue;
                }


                if (!ScTnArea.HasValue())
                {
                    Sb.Append(string.Format(TipFormat, RowNum.AsString(), "请填写套内面积"));
                    continue;
                }
                if (!PropertyTakeoverDate.HasValue())
                {
                    Sb.Append(string.Format(TipFormat, RowNum.AsString(), "请填写物业接管时间"));
                    continue;
                }

                if (!LoginName.HasValue())
                {
                    Sb.Append(string.Format(TipFormat, RowNum.AsString(), "请填写登录名称"));
                    continue;
                }
                if (!OwnerName.HasValue())
                {
                    Sb.Append(string.Format(TipFormat, RowNum.AsString(), "请填写住户名称"));
                    continue;
                }
                if (!OwnerSex.HasValue())
                {
                    Sb.Append(string.Format(TipFormat, RowNum.AsString(), "请填写性别"));
                    continue;
                }


                if (!LinkMobile.HasValue())
                {
                    Sb.Append(string.Format(TipFormat, RowNum.AsString(), "请填写联系人电话"));
                    continue;
                }
                if (!IdNumber.HasValue())
                {
                    Sb.Append(string.Format(TipFormat, RowNum.AsString(), "请填写身份证号"));
                    continue;
                }

                if (!Email.HasValue())
                {
                    Sb.Append(string.Format(TipFormat, RowNum.AsString(), "请填写邮箱"));
                    continue;
                }
                if (!MailAddress.HasValue())
                {
                    Sb.Append(string.Format(TipFormat, RowNum.AsString(), "请填写邮寄地址"));
                    continue;
                }

                DbEnt.TempRoomownerRule.Entity TRoomOwnerEnti = new TempRoomownerRule.Entity();
                TRoomOwnerEnti.BuildingName = BuildName;
                TRoomOwnerEnti.FloorNum = Convert.ToSByte(FloorNum);
                TRoomOwnerEnti.UnitNum = Convert.ToSByte(UnitNum);
                TRoomOwnerEnti.RoomCode = RoomCode;
                TRoomOwnerEnti.ScBldArea = ScBidArea.AsDecimal();
                TRoomOwnerEnti.ScTnArea = ScTnArea.AsDecimal();
                TRoomOwnerEnti.PropertyTakeoverDate = PropertyTakeoverDate.AsDateTime();
                TRoomOwnerEnti.OwnerName = OwnerName;
                TRoomOwnerEnti.OwnerSex = OwnerSex;
                TRoomOwnerEnti.LoginName = LoginName;
                TRoomOwnerEnti.LinkMobile = LinkMobile;
                TRoomOwnerEnti.Email = Email;
                TRoomOwnerEnti.MailAddress = MailAddress;
                TRoomOwnerEnti.IdNumber = IdNumber;
                TRoomOwnerEnti.CommId = CommID;
                dbr.TempRoomowner.Insert(TRoomOwnerEnti).Execute();


            }
            jm.data = Sb.ToString();
            return jm;

        }
    }
}

