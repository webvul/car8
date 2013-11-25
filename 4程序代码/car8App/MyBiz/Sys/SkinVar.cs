//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MyOql;
//using MyCmn;
//using System.IO;
//using System.Web;
//using System.Text.RegularExpressions;

//namespace MyBiz.Sys
//{
//    /*
//{   
//    "MasterPage":"~/Views/Shared/ShopSite.Master",
//    "Layout":"left",
//    "Corner":
//    {
//        "Header":15,
//        "InfoPad":10,
//        "List":5,
//        "Viewy":5,
//        "Search":6,
//        "ButtonLink":4,
//        "Content":15,
//        "MenuMap":0,
//        "MainSiteMap":4,
//        "Sect":0,
//        "MainItem":5,
//        "Cap":4
//    },
//    "EnImg":
//    {
//    },
//    "ZhImg":
//    {
//    },
//    "EnCss":[],
//    "ZhCss":[]
//}
//*/
//    public class SkinVar
//    {
//        //public string MasterPage { get; set; }
//        //public string Layout { get; set; }
//        public Dictionary<string, int> Corner { get; set; }
//        public Dictionary<string, string> EnImg { get; set; }
//        public Dictionary<string, string> ZhImg { get; set; }
//        public string[] EnCss { get; set; }
//        public string[] ZhCss { get; set; }
//        public string Js { get; set; }


//        public string ToString(LangEnum Lang)
//        {
//            List<string> listScript = new List<string>();
//            this.Corner.All(o => { listScript.Add(string.Format("jv.SetCorner('.{0}','{1}');", o.Key, o.Value)); return true; });

//            if (Lang == LangEnum.En)
//            {
//                this.EnImg.All(o => { listScript.Add(string.Format("$('.{0}').attr('src','{1}');", o.Key, o.Value)); return true; });
//                this.EnCss.All(o => { listScript.Add(string.Format("$('.{0}').addClass('{0}En').removeClass('{0}');", o)); return true; });
//            }
//            else
//            {
//                this.ZhImg.All(o => { listScript.Add(string.Format("$('.{0}').attr('src','{1}');", o.Key, o.Value)); return true; });
//                this.ZhCss.All(o => { listScript.Add(string.Format("$('.{0}').addClass('{0}Zh').removeClass('{0}');", o)); return true; });
//            }
//            listScript.Add(this.Js);
//            return string.Join("", listScript.ToArray());
//        }

//        public override string ToString()
//        {
//            return ToString(LangEnum.Zh);
//        }

//        public static string Load(int DeptID, LangEnum Lang)
//        {
//            SkinVar skin = new DictRule().FindScalar(o => o.Value, o => o.DeptID == DeptID & o.Key == DictKeyEnum.SkinVar).AsString().FromJson<SkinVar>();

//            var dept = DeptRule.FindByID(DeptID);

//            string skinName = dept.MySkin;
//            if (skinName.HasValue() == false) skinName = "Default";

//            if (skin == null)
//            {
//                using (StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~/App_Themes/" + skinName + "/SkinVar.js")))
//                {
//                    string str = sr.ReadToEnd();
//                    sr.Close();
//                    skin = Regex.Replace(str, "/\\*.*?\\*/", "", RegexOptions.Singleline).FromJson<SkinVar>();
//                }
//            }

//            skin.Js = new DictRule().FindScalar(o => o.Value, o => o.DeptID == DeptID & o.Key == DictKeyEnum.MySkinJs).AsString(null);

//            if (skin.Js == null)
//            {
//                using (StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~/App_Themes/" + skinName + "/MySkinJs.js")))
//                {
//                    string str = sr.ReadToEnd();
//                    sr.Close();
//                    skin.Js = Regex.Replace(str, "/\\*.*?\\*/", "", RegexOptions.Singleline);
//                }
//            }

//            return skin.ToString(Lang);
//        }

//        public static string Load(string Theme, LangEnum Lang)
//        {
//            SkinVar skin = new SkinVar();
//            using (StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~/App_Themes/" + Theme + "/SkinVar.js")))
//            {
//                string str = sr.ReadToEnd();
//                sr.Close();
//                skin = Regex.Replace(str, "/\\*.*?\\*/", "", RegexOptions.Singleline).FromJson<SkinVar>();
//            }
//            using (StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~/App_Themes/" + Theme + "/MySkinJs.js")))
//            {
//                string str = sr.ReadToEnd();
//                sr.Close();
//                skin.Js = Regex.Replace(str, "/\\*.*?\\*/", "", RegexOptions.Singleline);
//            }

//            return skin.ToString(Lang);
//        }
//    }
//}
