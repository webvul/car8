using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using MyOql.Sys;
using MyOql;
using MyCmn;
using System.Web.Compilation;
using System.Web.Security;
using System.Collections;
using MyBiz.Sys;
using System.Net;
using System.IO;
using System.Configuration;
using DbEnt;
using System.Web.Mvc;

namespace MyBiz
{
    public static class MySession
    {
        /// <summary>
        /// 加密格式: "UserName\nPassword\nDateTime"
        /// </summary>
        /// <param name="OrgOID"></param>
        /// <returns></returns>
        public static string CreateOnceLink(string UserID, string Password)
        {
            string res = UserID + "\n" + Password + "\n" + DateTime.Now.ToString();

            return Security.EncryptString(res);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Pwd"></param>
        /// <param name="PwdEncrypt">密码是否已编码</param>
        /// <returns></returns>
        public static bool Login(string UserID, string Pwd)
        {
            using (var conf = new MyOqlConfigScope(ReConfigEnum.SkipPower))
            {
                var usr = dbr.PLogin(UserID, Pwd);

                if (usr == null)
                {
                    if (SsoLogin().HasValue()) return true;
                    return false;
                }

                SetSession(usr);

                return true;
            }
        }

        private static void SetSession(PersonRule.Entity usr)
        {
            if (usr == null) return;

            HttpContext.Current.Session["Account"] = usr.UserID;
            HttpContext.Current.Session["Login"] = usr;
            //HttpContext.Current.Session[MySessionKey.TStandardRole.AsString()] =
            //    dbr.UserRole.Select(o => o.RoleID).Where(o => o.UserID == usr.UserID).ToEntityList("");

            HttpContext.Current.Session[MySessionKey.WebName.AsString()] = usr.GetDept().WebName;
            HttpContext.Current.Session[MySessionKey.Power.AsString()] = GetMyFullPower(usr);

            HttpContext.Current.Session[MySessionKey.UserID.AsString()] = usr.UserID;
            HttpContext.Current.Session[MySessionKey.UserName.AsString()] = usr.Name;
            HttpContext.Current.Session[MySessionKey.LoginName.AsString()] = usr.UserID;
            HttpContext.Current.Session[MySessionKey.DeptID.ToString()] = usr.GetDept().Id;
            HttpContext.Current.Session[MySessionKey.DeptName.ToString()] = usr.GetDept().Name;
            HttpContext.Current.Session[MySessionKey.DefaultLang.ToString()] = usr.GetDept().DefaultLang;

            SetCookie();
        }

        public static void SetCookie()
        {
            return;
            var accountCookie = new HttpCookie("account", HttpContext.Current.Session[MySessionKey.UserID.ToString()].AsString());

            accountCookie.Expires = DateTime.Now.AddMonths(1);

            HttpContext.Current.Response.Cookies.Add(accountCookie);
        }


        public static void LogOut()
        {
            //HttpContext.Current.Request.Cookies.Clear();

            //HttpContext.Current.Request.Cookies.AllKeys.All(o =>
            //{
            //    if (o == "hyj_account" || o == "bbsmax_user")
            //    {
            //        if (HttpContext.Current.Response.Cookies.AllKeys.Contains(o))
            //        {
            //            HttpContext.Current.Response.Cookies[o].Expires = new DateTime(2000, 1, 1);
            //        }
            //        else
            //        {
            //            HttpCookie cookie = new HttpCookie(o);
            //            cookie.Expires = new DateTime(2000, 1, 1);
            //            HttpContext.Current.Response.Cookies.Add(cookie);
            //        }
            //    }
            //    return true;
            //});

            HttpContext.Current.Session.Clear();
        }

        //public static bool IsLogin()
        //{
        //    return HttpContext.Current.Session["Login"] != null;
        //}

        public static PersonRule.Entity GetMyEntity()
        {
            var org = HttpContext.Current.Session["Login"] as PersonRule.Entity;
            if (org == null)
            {
                var account = SsoLogin();
                if (account.HasValue())
                {
                    org = org = HttpContext.Current.Session["Login"] as PersonRule.Entity;
                }

                if (org == null)
                {
                    throw new GodError("Session丢失或超时,请重新登录!");
                }
            }

            return org;
        }

        public static string OnlyGet(this MySessionKey key)
        {
            return Get(key, false);
        }

        public static string Get(this MySessionKey key, bool UseSSO = true)
        {
            if (HttpContext.Current == null) return null;
            if (HttpContext.Current.Session == null) return null;

            var org = HttpContext.Current.Session["Login"] as PersonRule.Entity;

            if (org == null && UseSSO)
            {
                string account = SsoLogin();

                if (account.HasValue())
                {
                    org = HttpContext.Current.Session["Login"] as PersonRule.Entity;
                }
            }

            if (org == null)
            {
                return string.Empty;
            }

            switch (key)
            {
                case MySessionKey.UserID:
                    return org.UserID.AsString();
                case MySessionKey.UserName:
                    return org.Name;
                case MySessionKey.LoginName:
                    return org.UserID;
                case MySessionKey.DeptID:
                    return org.DeptID.ToString();
                //case MySessionKey.UserRole:
                //    return org.Roles.ToString();
                //case MySessionKey.DeptID:
                //    return "AAA30AAC-67EE-4B51-B58E-35C0DE7D16E8";
                case MySessionKey.WebName:
                    return HttpContext.Current.Session[MySessionKey.WebName.AsString()].AsString();
                case MySessionKey.DeptName:
                    return org.GetDept().Name;
                //case MySessionKey.Langs:
                //    return org.GetDept().Langs;
                case MySessionKey.DefaultLang:
                    return org.GetDept().DefaultLang;
                default:
                    break;
            }
            return string.Empty;
        }

        private static string SsoLogin()
        {
            return string.Empty;
            string account = GetCookie("account");

            CookieContainer mycookiecontainer = new CookieContainer();
            string requestUrl = string.Empty;

            if (account.HasValue())
            {
                string res = GetResponseValue(string.Format(ConfigurationManager.AppSettings["SSOValidate"], account));

                if (res.HasValue())
                {
                    var usr = dbr.Person.FindFirst(o => o.UserID == res, o => o._);
                    if (usr == null) return null;

                    SetSession(usr);
                }

                return res;
            }
            return account;
        }
        public static bool IsSysAdmin()
        {
            var loginName = MySessionKey.LoginName.Get().AsString(MySessionKey.UserName.Get());
            if (loginName.HasValue() == false) return false;
            return string.Equals(loginName, ConfigKey.SysAdmin.Get(), StringComparison.CurrentCultureIgnoreCase);
        }

        private static string GetResponseValue(string requestUrl)
        {
            WebClient web = new WebClient();
            var retVal = string.Empty;

            using (Stream stream = web.OpenRead(requestUrl.StartsWith("~/") ? MyUrl.GetUrlFullWithPrefix(requestUrl) : requestUrl))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    retVal = reader.ReadToEnd();
                    reader.Close();
                }
                stream.Close();
            }

            return retVal;
        }

        private static string GetCookie(string cookie)
        {
            //只能获取同域下的Cookie 。
            var cookieValue = HttpContext.Current.Request.Cookies[cookie];
            if (cookieValue != null) return cookieValue.Value;
            else return null;


            //var mycookiecontainer = new CookieContainer();
            //HttpWebRequest web = (HttpWebRequest)WebRequest.Create(requestUrl);

            //string indata = "aa=hello" ;
            ////web.ClientCertificates
            //web.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;// = (System.Security.Cryptography.X509Certificates.X509CertificateCollection)CredentialCache.DefaultCredentials;
            //web.CookieContainer = mycookiecontainer;
            //web.ContentType = "application/x-www-form-urlencoded";
            //web.Method = "Get";

            //web.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";




            //var response = web.GetResponse();

            //var requestCookie = mycookiecontainer.GetCookies(new Uri(requestUrl))[cookie];

            //if (requestCookie != null)
            //{
            //    return requestCookie.Value;
            //}
            //else return null;

        }

        public static PowerJson GetMyPower()
        {
            // Current ＝＝ null 说明是服务。
            if (HttpContext.Current == null) return PowerJson.MaxValue;
            if (HttpContext.Current.Session == null) return PowerJson.MaxValue;

            // Current ＝＝ null 说明是服务。
            if (HttpContext.Current == null) return PowerJson.MaxValue;
            if (HttpContext.Current.Session == null) return PowerJson.MaxValue;

            if (MyHelper.Area == "Host") return PowerJson.MaxValue;
            if (HttpContext.Current.Session["Login"] == null)
            {
                //return null;
                var account = SsoLogin();

                if (account.HasValue() == false)
                {
                    return null;
                }
            }
            return (PowerJson)HttpContext.Current.Session[MySessionKey.Power.AsString()];
        }


        public static int[] GetMyRoles()
        {
            return GetMyEntity().GetRoles().Select(o => o.Id).ToArray();
        }

        /// <summary>
        /// 取用户的所有权限
        /// </summary>
        /// <param name="usr"></param>
        /// <param name="dept"></param>
        /// <returns></returns>
        public static PowerJson GetMyFullPower(PersonRule.Entity usr)
        {
            if (usr == null) return null;

            if (ConfigKey.SysAdmin.Get() == usr.UserID) return PowerJson.MaxValue;

            var power = new PowerJson(usr.Power);


            power |= new PowerJson(usr.GetDept().Power);


            foreach (var role in usr.GetRoles())
            {
                power |= new PowerJson(role.Power);
            }

            var notPower = new PowerJson(usr.NotPower);

            power.Action.Minus(notPower.Action);
            power.Button.Minus(notPower.Button);
            power.Create.Minus(notPower.Create);
            power.Delete.Minus(notPower.Delete);
            power.Read.Minus(notPower.Read);
            power.Update.Minus(notPower.Update);

            power.Row.View.Keys.Intersect(notPower.Row.View.Keys).All(o =>
            {
                power.Row.View[o].Minus(notPower.Row.View[o]);
                return true;
            });

            power.Row.Edit.Keys.Intersect(notPower.Row.Edit.Keys).All(o =>
            {
                power.Row.View[o].Minus(notPower.Row.View[o]);
                return true;
            });
            return power;

        }

        public static string GetLoginHtml()
        {
            return string.Format("<a href='{0}' target='_top'>会话超时，请登录!</a>", ConfigKey.SSOLogin.Get().GetUrlFull());
        }
    }
}
