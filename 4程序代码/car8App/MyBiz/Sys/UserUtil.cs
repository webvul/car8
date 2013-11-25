using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web;
using MyCmn;
using MyBiz.Sys;
using MyBiz;
using DbEnt;

namespace MyOql.Sys
{
    public class UserData
    {
        public PersonRule.Entity User { get; set; }
        public string MyPath { get; set; }
        public PowerJson Power { get; set; }
    }
    public class UserUtil
    {
        public static void Login(UserData userData, bool isPersistent)
        {
            if (userData == null)
            {
                throw new Exception("无效的用户数据！");
            }
            DateTime dt = isPersistent ? DateTime.Now.AddMonths(3) : DateTime.Now.AddMinutes(60);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1, // 票据版本号
                    userData.User.UserID, // 票据持有者
                    DateTime.Now, //分配票据的时间
                    dt, // 失效时间
                    isPersistent, // 需要用户的 cookie 
                    userData.ToJson(), // 用户数据，这里其实就是用户的角色
                    FormsAuthentication.FormsCookiePath);//cookie有效路径
            //使用机器码machine key加密cookie，为了安全传送
            string hash = FormsAuthentication.Encrypt(ticket);

            if (hash == null) throw new Exception("用户登录时加密失败！");
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash); //加密之后的cookie
            //将cookie的失效时间设置为和票据tikets的失效时间一致 
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            //添加cookie到页面请求响应中
            MySession.LogOut();

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

    }
}
