using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyOql;
using System.Web.Mvc;
using MyBiz.Admin;
using MyCmn;
using DbEnt;
using System.Data.SqlClient;
using System.Data.Common;
using System.Xml;
using System.Web;
using System.Threading;
using System.Security.Cryptography;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

namespace MyBiz
{

    public static partial class MyBizHelper
    {


        /// <summary>
        /// 这里记录页面上要保存的非引用的值。
        /// </summary>
        public static KeepInfoModel[] KeepInfo
        {
            get
            {
                var col = new List<KeepInfoModel>();
                //客户名称
                col.Add(new KeepInfoModel("CustId,CustID,HandCustID", "CustName,HandCustName"));

                //可选房号。
                col.Add(new KeepInfoModel("car_room_select,SelRoomID,SelRoomId"));

                //房屋编号
                col.Add(new KeepInfoModel("RoomId,RoomID,HandRoomID", "HandRoomSign,RoomSign,RoomName"));

                //房间，客户中选择 公司/项目
                col.Add(new KeepInfoModel("QueryId", "CommCode"));
                //col.Add(new KeepInfoModel("CommID", "CommName"));

                return col.ToArray();
            }
        }

        public static KeepInfoModel GetKeyInfoModelFromKey(string Key)
        {
            for (int i = 0; i < KeepInfo.Length; i++)
            {
                var item = KeepInfo[i];
                if (item.Key.Contains(Key)) return item;
            }
            return null;
        }




        public static void ClearkeepInfo()
        {
            HttpContext.Current.Session[MySessionKey.KeepInfo.ToString()] = null;
        }
        public static void SetContext(HttpContext context)
        {
            if (HttpContext.Current == null) HttpContext.Current = context;
        }



        /// <summary>
        /// 顺序比较两个 Array的值，是否全等。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static bool ArrayIsEquals<T>(this IEnumerable<T> one, IEnumerable<T> two)
        {
            if (one == null) return false;
            if (two == null) return false;
            var length = one.Count();
            if (length != two.Count()) return false;

            for (int i = 0; i < length; i++)
            {
                if (one.ElementAt(i).Equals(two.ElementAt(i)) == false) return false;
            }

            return true;
        }


        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }

        //public static MyOqlSet LoadExcel<T>(T obj, int AnnexId, string SheetName) where T : RuleBase
        //{
        //    OleDbService.OleDbServiceClient client = new OleDbService.OleDbServiceClient();

        //    var annexEnt = dbr.Annex.FindById(AnnexId);
        //    var excelPath = HttpContext.Current.Server.MapPath(annexEnt.FullName);

        //    var data = client.ReadAsMyOqlSet(excelPath, SheetName);

        //    MyOqlSet set = new MyOqlSet(obj);
        //    set = data.FromJson<MyOqlSet>();
        //    set.Entity = obj;

        //    return set;
        //}

        public static void SendEmail(MailMessage email)
        {
            SmtpClient smtp = new SmtpClient();
            // 提供身份验证的用户名和密码 
            // 网易邮件用户可能为：username password 
            // Gmail 用户可能为：username@gmail.com password 
            smtp.SendCompleted += new SendCompletedEventHandler(SendMailCompleted);
            try
            {
                smtp.SendAsync(email, email);
            }
            catch (SmtpException ex)
            {
                new GodError("邮件：" + ex.Message, ex).ToLog();
            }
        }

        public static void SendMailCompleted(object sender, AsyncCompletedEventArgs e)
        {
            MailMessage mailMsg = (MailMessage)e.UserState;
            string subject = mailMsg.Subject;
            if (e.Cancelled) // 邮件被取消 
            {
                InfoEnum.Error.LogTo("邮件：" + subject + " 被取消。", string.Empty);
            }

            if (e.Error != null)
            {
                new GodError("邮件：" + subject + " 被取消。", e.Error).ToLog();
            }
        }

        public static void AddTableTotal(ref DataTable dTable, string strFieldName)
        {
            int count = dTable.Columns.Count;
            decimal num2 = -999999M;
            decimal[] numArray = new decimal[count];
            for (int i = 0; i < count; i++)
            {
                Type dataType = dTable.Columns[i].DataType;
                string columnName = dTable.Columns[i].ColumnName;
                switch (dataType.FullName.ToLower())
                {
                    case "system.decimal":
                    case "system.int32":
                        if (columnName.IndexOf("ID") == -1)
                        {
                            numArray[i] = 0M;
                        }
                        else
                        {
                            numArray[i] = num2;
                        }
                        break;

                    default:
                        numArray[i] = num2;
                        break;
                }
            }
            foreach (DataRow row in dTable.Rows)
            {
                for (int k = 0; k < count; k++)
                {
                    if (numArray[k] != num2)
                    {
                        try
                        {
                            numArray[k] += row[k].AsDecimal();
                        }
                        catch
                        {
                        }
                    }
                }
            }
            DataRow row2 = dTable.NewRow();
            row2[strFieldName] = "合计";
            for (int j = 0; j < count; j++)
            {
                if (numArray[j] != num2)
                {
                    row2[j] = numArray[j];
                }
            }
            dTable.Rows.Add(row2);
            dTable.AcceptChanges();
        }

        public static void CreateExcel(Page Pages, DataGrid DataGrid1, string FileName)
        {
            HttpResponse response = Pages.Response;
            response.Clear();
            response.Buffer = true;
            response.Charset = "GB2312";
            response.ContentEncoding = Encoding.GetEncoding("gb2312");
            response.AppendHeader("Content-Disposition", "attachment;filename=" + FileName);
            response.ContentType = "application/ms-excel";
            Pages.EnableViewState = false;
            response.Write("<META http-equiv=\"Content-Type\" content=\"text/html; charset=gb2312\">");
            CultureInfo formatProvider = new CultureInfo("ZH-CN", true);
            StringWriter writer = new StringWriter(formatProvider);
            HtmlTextWriter writer2 = new HtmlTextWriter(writer);
            DataGrid1.RenderControl(writer2);
            response.Write(writer.ToString());
            response.Flush();
            response.End();
        }
    }
}


