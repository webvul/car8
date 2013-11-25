//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MyCmn;
//using MyBiz;
//using System.IO;
//using System.Text.RegularExpressions;
//using System.Web;
 

//namespace MyBiz.Sys
//{

//    public class HostSkinHelper
//    {
//        public class HostSkinVar
//        {
//            public HostSkinVar()
//            {
//                this.EnSrcImg = new Dictionary<string, string>();
//                this.ZhSrcImg = new Dictionary<string, string>();
//                this.EnBackgroundcImg = new Dictionary<string, string>();
//                this.ZhBackgroundcImg = new Dictionary<string, string>();
//                this.Corner = new Dictionary<string, string>();
//            }

//            public Dictionary<string, string> Corner { get; set; }
//            public Dictionary<string, string> EnSrcImg { get; set; }
//            public Dictionary<string, string> ZhSrcImg { get; set; }
//            public Dictionary<string, string> EnBackgroundcImg { get; set; }
//            public Dictionary<string, string> ZhBackgroundcImg { get; set; }
//            public string MasterPage { get; set; }
//            public string WebName { get; set; }
//            //public string Layout { get; set; }
//            public string ExecuteJs { get; set; }
//        }

//        public static HostSkinVar LoadMySkinVar()
//        {
//            return CacheHelper.Get(CacheKey.HostSkinVar, CacheTime.Hour, delegate()
//            {
//                string skinName = GetMyTheme();
//                using (StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~/App_Themes/" + skinName + "/SkinVar.js")))
//                {
//                    string str = sr.ReadToEnd();
//                    sr.Close();
//                    return Regex.Replace(str, @"/\*.*?\*/", "", RegexOptions.Singleline).FromJson<HostSkinVar>();
//                }
//            });
//        }

//        private static string GetMyTheme()
//        {
//            return "Host";
//            //DictEntity dict = DictEntity.FindFirst(
//            //    dbo.Dict.DeptID == DeptBus.GetSysAdminDept().ID &&
//            //    dbo.Dict.Key == DictHiddenKeyEnum.Skin);

//            //if (dict != null) return dict.Value;
//            //else return "Host";
//        }

//    }
//}
