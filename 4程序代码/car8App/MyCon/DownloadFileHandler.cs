using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using MyCmn;
using System.IO;
using System.Web.Mvc;
using DbEnt;

namespace MyCon
{
    public class DownloadFileHandler : IHttpHandler, IReadOnlySessionState, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            var id = context.Request.QueryString["Id"].AsInt();
            if (id.HasValue() == false)
            {
                procNoIdErrorPage(context);
                return;
            }
            var ent = dbr.Annex.FindById(id);
            if (ent == null)
            {
                procNoRecordErrorPage(context);
                return;
            }
            var realFile = context.Server.MapPath(ent.Path + ent.Name);
            if (File.Exists(realFile) == false)
            {
                procNoFileErrorPage(context);
                return;
            }

            context.Response.Clear();
            context.Response.BufferOutput = true;
            //http://www.learf.com/55.html
            //var header = context.Response.Headers;
            context.Response.AddHeader("Pragma", "public");//["Pragma"] = "public";
            context.Response.AddHeader("Last-Modified", DateTime.Now.ToString("r"));
            context.Response.AddHeader("Cache-Control", "no-store,no-cache,must-revalidate,pre-check=0,post-check=0,max-age=0");
            context.Response.AddHeader("Content-Transfer-Encoding", "binary");
            context.Response.AddHeader("Content-Encoding", "none");
            context.Response.AddHeader("Content-type", MyHtmlHelper.GetContentType(Path.GetExtension(ent.Name)));

            var renderFileName = ent.Name;

            if (context.Request.Headers["User-Agent"].AsString().IndexOf("Firefox") > 0)
            {
                renderFileName = renderFileName.Replace(" ", "");
            }
            else if (context.Request.Headers["User-Agent"].AsString().IndexOf("MSIE") > 0)
            {
                renderFileName = HttpUtility.UrlEncode(renderFileName);
            }
            else
            {
            }
            context.Response.AddHeader("Content-Disposition", "attachment; filename=" + renderFileName);

            var FileStream = File.ReadAllBytes(realFile);
            context.Response.AddHeader("Content-length", FileStream.Length.ToString());

            if (FileStream.Length != 0)
            {
                context.Response.OutputStream.Write(FileStream, 0, FileStream.Length);
                context.Response.Flush();
                context.Response.End();
                return;
            }
        }

        private void procNoFileErrorPage(HttpContext context)
        {
            context.Response.Write("找不到文件，请检查");
        }

        private void procNoRecordErrorPage(HttpContext context)
        {
            context.Response.Write("找不到该记录，请检查");
        }

        private void procNoIdErrorPage(HttpContext context)
        {
            context.Response.Write("参数不正确，请检查");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

}
