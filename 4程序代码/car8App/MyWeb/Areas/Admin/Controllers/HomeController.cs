using System;
using System.Linq;
using System.Web.Mvc;
using MyOql;
using MyCon;
using MyBiz;
//using Microsoft.AnalysisServices.AdomdClient;
using MyCmn;
using DbEnt;
using System.Web;
using System.Data;

namespace MyWeb.Areas.Admin.Controllers
{
    public class HomeController : MyMvcController
    {
        [HttpPost]
        public ActionResult CreateHtml(FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();

            var urls = string.Format(collection["Url"], MySession.Get(MySessionKey.WebName)).Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();


            for (int i = 0; i < urls.Count; i++)
            {
                urls[i] = MyUrl.GetUrlFullWithPrefix(urls[i]);

            }

            jm.data = urls.ToArray();
            return jm;
        }
        public ActionResult Test()
        {
            return View();
        }

        public ActionResult OlapTest()
        {
            //            using (AdomdConnection conn = new AdomdConnection(@"Provider=SQLNCLI10.1;Data Source=udi-PC;Persist Security Info=True;Password= ;User ID=udi-PC\udi;Initial Catalog=MyWebBI"))
            //            {
            //                conn.Open();
            //                AdomdCommand cmd = conn.CreateCommand();
            //                cmd.CommandText = @"
            //SELECT
            //NON EMPTY {  [Measures].[Clicks] } ON COLUMNS, 
            //NON EMPTY { (
            //[Product Clicks].[Year].[Year].ALLMEMBERS * 
            //[Product Clicks].[Month].[Month].ALLMEMBERS * 
            //[Product Clicks].[Product ID].[Product ID].ALLMEMBERS 
            //) } 
            //DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS 
            //FROM [My Web]
            //";

            //                CellSet cs = cmd.ExecuteCellSet();

            //                return View(cs);
            //            }

            return null;

        }
        public ActionResult File(string uid)
        {
            var files = Request.QueryString["path"];
            string path = Server.MapPath(files.GetUrlFull());
            string filename = System.IO.Path.GetFileName(path);
            DownloadResult result = new DownloadResult(uid, System.IO.File.ReadAllBytes(path));
            //Response.WriteFile(path);
            return result;
        }

        public ActionResult Default()
        {
            return Content(@"<!DOCTYPE html><html>
<head>
<meta name=""X-UA-Compatible"" value=""ie=EmulateIE8"" />
<script type=""text/javascript"">
window.onload = function() {
    var checkBrowser = function(){
        var ieMode=document.documentMode;
        var isIE=!!window.ActiveXObject;
        if ( !isIE ) return false;
        var isIE6=isIE&&!window.XMLHttpRequest;
        if ( isIE6 ) return false;
        var isIE7=isIE&&!isIE6&&!ieMode;
        if ( isIE7 ) return false ;
        var isIE8_7 = ieMode==7 ;
        if ( isIE8_7 ) return false ;
        var isIE9=isIE&&ieMode==9;
        if ( isIE9 ) return false ;
        var isIE8=isIE&&ieMode==8;
        if ( document.compatMode !== ""CSS1Compat"" ) return false ;
        return true ;
    };
    if( !checkBrowser() ) window.showModalDialog(""" + MyUrl.GetUrlHeader() + @"Web/IeError.html"",null,""center=yes;resizable=yes;dialogWidth=700px;dialogHeight=500px;"");

    self.moveTo(-4,-4);
    self.resizeTo(screen.availWidth+9,screen.availHeight+9);
    window.location='Admin/Home/Index.aspx" + @"';
}
</script>
</head>
</html>");
        }
        //
        // GET: /Admin/Home/

        [NoPowerFilter]
        public ActionResult Index()
        {
            if (MySessionKey.UserID.Get().HasValue() == false)
            {
                var login = ConfigKey.SSOLogin.Get().GetUrlFull();

                try
                {
                    return Redirect(ConfigKey.SSOLogin.Get());
                }
                catch
                {
                    return Content(MySession.GetLoginHtml());
                }
            }

            return Content(@"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Frameset//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head><title>和易家社区服务平台</title>
<meta http-equiv=""Content-Type"" content=""text/html;charset=UTF-8"" />
</head>
<frameset rows=""40,*"" border=""0px"" frameborder=""no"">
	<frame name=""banner"" id=""banner"" src=""{Root}Admin/Home/Title.aspx"" noResize=""noresize"" scrolling=""no"" frameborder=""no"" border=""0px"" ></frame>
	<frameset cols=""180,*"" id=""fm"" border=""0px"" frameborder=""no"">
        <frame name=""menu"" id=""menu""  noResize=""noresize""  src=""{Root}Admin/Home/Menu.aspx"" frameborder=""0""></frame>
		<frame name=""main"" frameborder=""no"" noResize src=""about:blank"" id=""main""></frame>
	</frameset>
</frameset>
<noframes><body>浏览器不支持FrameSet</body></noframes>
</html> 
"
                .Format(new StringDict() { { "Root", "~/".GetUrlFull() } })
                );
        }


        [NoPowerFilter]
        public ActionResult Title()
        {
            return View();
        }

        public ActionResult Menu()
        {
            return View();
        }

    }
}