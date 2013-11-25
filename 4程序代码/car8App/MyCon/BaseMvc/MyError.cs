using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MyCmn;
using System.Web;
using System.Linq;
using MyBiz;

namespace MyCon
{
    public class MyError : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.StatusCode = 500;

            ProcException(filterContext, filterContext.Exception);
        }

        public static void ProcException(ControllerContext filterContext, Exception exp)
        {
            #region errorHtml
            string ErrorHtml = @"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head><title>
    出错了
</title>
    <link rel=""shortcut icon"" href=""~/favicon.ico"" type=""image/x-icon"" />
    <link rel=""stylesheet"" type=""text/css"" href=""~/Res/MyCss.css"" />
    <script type=""text/javascript"" src=""~/Res/MyJs.js""></script>
    <script type=""text/javascript"">$(function(){$InitJs$});</script>
    <link href=""~/App_Themes/Host/Css_Normal.css"" type=""text/css"" rel=""stylesheet"" />
    <link href=""~/App_Themes/Host/MyHost.css"" type=""text/css"" rel=""stylesheet"" />
    <script src=""~/Res/MyJs_Host.js"" type=""text/javascript""></script>
    <link rel=""icon"" href=""~/favicon.gif"" type=""image/gif"" />
    <link rel=""stylesheet"" type=""text/css"" href=""~/areas/host/res/css/css.css"" />
    <link rel=""stylesheet"" href=""~/areas/host/res/css/StyleSheet.css"" type=""text/css"" />
    <script type=""text/javascript"" src=""~/areas/host/res/js/coin-slider.js""></script>
</head>
<body>
    <div class=""b_wrap02"">
        <!-- HEAD STAR -->
        <div class=""head_apv02"">
            <div class=""head_apvright"">
                <ul>
                    <li><a href=""~/host/HelpCenter/index.aspx"" class=""dh_li01"" id=""menu_helpcenter_index"" title=""帮助指南"">
                        <img src=""~/areas/host/res/images/dh05.png"" /></a></li>
                    <li><a href=""~/host/Cinema/index.aspx"" class=""dh_li01"" id=""menu_cinema_index"" title=""橙天嘉禾影城"">
                        <img src=""~/areas/host/res/images/dh04.png"" /></a></li>
                    <li><a href=""~/host/scheduling/index.aspx"" class=""dh_li01"" id=""menu_scheduling_index"" title=""上映时间"">
                        <img src=""~/areas/host/res/images/dh03.png"" /></a></li>
                    <li><a href=""~/Host/Movie/Index.aspx"" class=""dh_li01"" id=""menu_movie_index"" title=""新片上映"">
                        <img src=""~/areas/host/res/images/dh02.png"" /></a></li>
                    <li><a href=""~/Index.aspx"" class=""dh_li01"" id=""menu_home"" title=""影城首页"">
                        <img src=""~/areas/host/res/images/dh01.png"" /></a></li>
                </ul>
            </div>
            <div class=""head_apvleft_left"">
                <ul>
                    <li class=""head_apvll_li01"">&nbsp; <a id=""prePageInfo"" href=""~/Index.aspx""></a></li>
                    <li>
                        <script type=""text/javascript"">                             document.write(jv.GetBreaks());</script>
                    </li>
                </ul>
            </div>
            <div class=""head_apvleft"">
                <a href=""javascript:void(0);"" id=""headRegionDiv""><b style=""color: #FE9B00; font-size: 15px;"">
                    我在<span id=""diqu_name"">北京</span></b>
                    <img src=""~/areas/host/res/images/arrow_up.png"" /></a>
                <!-- 地区切换弹出层START -->
                <div class=""head_apvlc"" id=""head_apvlcid"" style=""display: none;"">
                    <!-- Load RegionMap -->
                </div>
                <!-- 地区切换弹出层END -->
            </div>
        </div>
        <!-- HEAD END -->
        <div class=""login02"">
            <a href=""~/Index.aspx"">
                <img src=""~/areas/host/res/images/login02.png"" /></a>
            <span><a href='~/host/register/index.aspx'>注册</a> &nbsp; <a href='~/host/register/Login.aspx'>登录</a> </span>
        </div>
        <div class=""Main"">
    <div class=""error"">
        <img src=""~/areas/host/res/images/error.png"" />
        <span onclick=""javascript:window.location=window.location""></span>
        <pre style=""display: none"" id=""errorMsg"">
$ErrorMsg$
        </pre>
    </div>
        </div>
        <div class=""bottom"">
            <span>
                <img src=""~/areas/host/res/images/bottombg.png"">
            </span>
            <a target=""_blank"" href=""~/host/home/Coporation.aspx"">商业合作</a>
            |
            <a target=""_blank"" href=""~/Host/Home/Employee.aspx"">招聘信息</a>
            <br />
            COPYRIGHT2006-2011  橙天嘉禾影城版权所有  京ICP证110389号 京公网安备110105013376<br/>
        </div>
    </div>
</body>
</html>
";
            #endregion
            ErrorHtml = ErrorHtml.Replace("~/", MyUrl.GetUrlHeader())
                .Replace("$InitJs$", MyMvcPage.GetRequestJs())
                .Replace("$ErrorMsg$", exp != null ? exp.Message + Environment.NewLine + exp.StackTrace : "");
            //base.OnException(filterContext);
            //try
            //{
            //    LogInfo.To(InfoEnum.User | InfoEnum.Error, HttpContext.Current.Request.Url.ToString() + exp.Message, MySession.Get(MySessionKey.UserName).AsString(MySession.GetHost(HostSessionKey.MemberName)));
            //}
            //catch { }
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.Write(ErrorHtml);
            filterContext.HttpContext.Response.Flush();
            filterContext.HttpContext.Response.End();
            return;
        }
    } 
}
