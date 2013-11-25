using System.IO;
using MyCmn;

namespace System.Web.Mvc
{

    public class DownloadResult : ActionResult
    {
        public string FileName { get; set; }
        public byte[] FileStream { get; set; }

        public DownloadResult(string FileName, string Content)
        {
            this.FileName = FileName;

            if (Content.HasValue())
            {
                this.FileStream = System.Text.Encoding.UTF8.GetBytes(Content);
            }
        }

        public DownloadResult(string FileName, byte[] FileStream)
        {
            this.FileName = FileName;
            this.FileStream = FileStream;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.BufferOutput = true;
            //http://www.learf.com/55.html
            //var header = context.HttpContext.Response.Headers;
            context.HttpContext.Response.AddHeader("Pragma", "public");//["Pragma"] = "public";
            context.HttpContext.Response.AddHeader("Last-Modified", DateTime.Now.ToString("r"));
            context.HttpContext.Response.AddHeader("Cache-Control", "no-store,no-cache,must-revalidate,pre-check=0,post-check=0,max-age=0");
            context.HttpContext.Response.AddHeader("Content-Transfer-Encoding", "binary");
            context.HttpContext.Response.AddHeader("Content-Encoding", "none");
            context.HttpContext.Response.AddHeader("Content-type", MyHtmlHelper.GetContentType(Path.GetExtension(FileName)));

            var renderFileName = FileName;

            if (context.HttpContext.Request.Headers["User-Agent"].AsString().IndexOf("Firefox") > 0)
            {
                renderFileName = renderFileName.Replace(" ", "");
            }
            else if (context.HttpContext.Request.Headers["User-Agent"].AsString().IndexOf("MSIE") > 0)
            {
                renderFileName = HttpUtility.UrlEncode(renderFileName);
            }
            else
            {
            }
            context.HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=" + renderFileName);

            if (this.FileStream.Length != 0)
            {
                context.HttpContext.Response.AddHeader("Content-length", this.FileStream.Length.ToString());
                context.HttpContext.Response.OutputStream.Write(this.FileStream, 0, this.FileStream.Length);
                context.HttpContext.Response.Flush();
                context.HttpContext.Response.End();
                return;
            }

            //header["Pragma"] = "public";
            //header["Last-Modified"] = DateTime.Now.ToString("r");// GMT 时间
            //header["Cache-Control"] = "no-store,no-cache,must-revalidate,pre-check=0,post-check=0,max-age=0";
            //header["Content-Transfer-Encoding"] = "binary";
            //header["Content-Encoding"] = "none";
            //header["Content-type"] = Path.GetExtension(FileName);
            //header["Content-Disposition"] = "attachment; filename=" + FileName; //文件名称
            // header["Content-length"] = FileStream.Length.ToString(); //文件大小
        }
    }
    public class DownloadFile
    {
        public string FileName { get; set; }
        public byte[] FileStream { get; set; }

        public DownloadFile(string FileName, string Content)
        {
            this.FileName = FileName;

            if (Content.HasValue())
            {
                this.FileStream = System.Text.Encoding.Default.GetBytes(Content);
            }
        }

        public DownloadFile(string FileName, byte[] FileStream)
        {
            this.FileName = FileName;
            this.FileStream = FileStream;
        }
        public void AddResponseHeader()
        {
            HttpContext.Current.Response.AddHeader("Pragma", "public");
            HttpContext.Current.Response.AddHeader("Last-Modified", DateTime.Now.ToString("r"));
            HttpContext.Current.Response.AddHeader("Content-Transfer-Encoding", "binary");
            HttpContext.Current.Response.AddHeader("Content-Encoding", "none");
            HttpContext.Current.Response.AddHeader("Content-type", Path.GetExtension(FileName));

            var renderFileName = FileName;

            if (HttpContext.Current.Request.Headers["User-Agent"].AsString().IndexOf("Firefox") > 0)
            {
                renderFileName = renderFileName.Replace(" ", "");
            }
            else if (HttpContext.Current.Request.Headers["User-Agent"].AsString().IndexOf("MSIE") > 0)
            {
                renderFileName = HttpUtility.UrlEncode(renderFileName);
            }
            else
            {
            }
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + renderFileName);

            if (this.FileStream != null)
            {
                HttpContext.Current.Response.AddHeader("Content-length", this.FileStream.Length.ToString());
                HttpContext.Current.Response.OutputStream.Write(this.FileStream, 0, this.FileStream.Length);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
            }
        }
    }



}
