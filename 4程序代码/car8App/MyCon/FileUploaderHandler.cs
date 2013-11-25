using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using MyCmn;
using System.IO;
using MyOql;
using MyBiz;
using System.Web.Mvc;
using DbEnt;

namespace MyCon
{
    public class FileJsonResult
    {
        public bool success { get; set; }

        /// <summary>
        /// 有消息就是错误消息。
        /// </summary>
        public string msg { get; set; }
        public string id { get; set; }
        public string url { get; set; }
        public string extraJs { get; set; }
        public string name { get; set; }
        public string size { get; set; }
        public override string ToString()
        {
            return this.ToJson();
        }
    }

    public class FileUploaderHandler : IHttpHandler, IReadOnlySessionState, IRequiresSessionState
    {
        public delegate void FileUploadCompleteDelegate(FileJsonResult uploadResult);

        public static event FileUploadCompleteDelegate FileUploadComplete;

        public void ProcessRequest(HttpContext context)
        {
            FileJsonResult jm = new FileJsonResult();
            jm.success = false;

            try
            {
                context.Response.ContentType = "text/plain";
                context.Response.Charset = "utf-8";
                string folder = context.Request.QueryString["folder"];


                //if (context.Request.Files["qqfile"] != null && context.Request.Files["qqfile"].InputStream.Length == 0)
                //{
                //    throw new FileLoadException("上传文件不能为空");
                //}

                //默认10Mb
                var limitSize = context.Request.QueryString["sizeLimit"].AsInt();//.HasValue(o => o.AsInt(), o => 10485760);
                if (limitSize > 0 && (context.Request.InputStream.Length > limitSize))
                {
                    throw new FileLoadException("文件太大, 最大限值: " + new CPUSize { Unit = CPUSizeEnum.Bytes, Value = limitSize }.ToFixedString());
                }


                if (folder.HasValue())
                {
                    if (MySession.Get(MySessionKey.WebName).HasValue())
                    {
                        folder = "~/Upload/" + MySession.Get(MySessionKey.WebName) + "/" + folder;
                    }
                    else
                    {
                        folder = "~/Upload/" + folder;
                    }
                }
                else
                {
                    if (MySession.Get(MySessionKey.WebName).HasValue())
                    {
                        folder = "~/Upload/" + MySession.Get(MySessionKey.WebName);
                    }
                    else
                    {
                        folder = "~/Upload/";
                    }
                }

                string strUploadPath = context.Server.MapPath(folder) + Path.DirectorySeparatorChar.ToString();

                var dy = new DirectoryInfo(strUploadPath);
                if (dy.Exists == false) dy.Create();

                var filename = HttpUtility.UrlDecode(context.Request.Headers["X-File-Name"]).AsString(null) ??
                            context.Request["qqfile"].AsString(null) ??
                            (context.Request.Files.Count > 0 ? context.Request.Files[0].FileName : "");

                var fi = new FileInfo(strUploadPath + GetFileName(strUploadPath, filename));

                using (BinaryWriter sw = new BinaryWriter(new FileStream(fi.FullName, FileMode.Append)))
                {
                    //每次读取1M
                    int readCount = 1024 * 1024;
                    byte[] fileContent = new byte[readCount];
                    int count = 0;

                    while (true)
                    {
                        if (context.Request.Files["qqfile"] == null)
                            count = context.Request.InputStream.Read(fileContent, 0, readCount);
                        else
                            count = context.Request.Files["qqfile"].InputStream.Read(fileContent, 0, readCount);
                        if (count == 0)
                        {
                            break;
                        }

                        sw.Write(fileContent, 0, count);
                    }
                    sw.Flush();
                    sw.Close();
                }


               // folder = MyUrl.GetUrlFullWithPrefix(folder);

                var insert = dbr.Annex.Insert(o =>
                    o.AddTime == DateTime.Now &
                    o.Ext == fi.Extension &
                    o.FullName == folder + "/" + fi.Name &
                    o.Name == fi.Name &
                    o.Size == fi.Length &
                    o.UserID == (MySession.Get(MySessionKey.UserID) ?? string.Empty) &
                    o.Path == folder+ "/" 
                    );

                if (insert.Execute() == 1)
                {
                    jm.success = true;
                    jm.id = insert.LastAutoID.AsString();
                }

                jm.url = (folder + "/" + fi.Name).ResolveUrl();
                jm.name = fi.Name;
                jm.size = fi.Length.ToString();

                if (FileUploadComplete != null)
                {
                    FileUploadComplete(jm);
                }
                //jm.data = listFiles;

                //输出格式: {"msg":"OK","data":[1,3]}
                context.Response.ContentType = "text/plain";
                //if (context.Request.Headers["User-Agent"].AsString().IndexOf("MSIE") > 0)
                // {
                //     context.Response.ContentType = "text/plain";
                // }
                // else
                // {
                //     context.Response.ContentType = "application/json";
                // }

                context.Response.Write(jm.ToString());
            }
            catch (Exception ee)
            {
                context.Response.ContentType = "text/plain";
                //if (context.Request.Headers["User-Agent"].AsString().IndexOf("MSIE") > 0)
                //{
                //}
                //else
                //{
                //    context.Response.ContentType = "application/json";
                //}

                jm.success = false;
                jm.msg = ee.Message.GetSafeValue();
                context.Response.Write(jm.ToString());
            }
        }

        private string GetFileName(string filePath, string name)
        {
            var FileName = name;
            if (File.Exists(filePath + FileName) == false) return FileName;

            FileName = DateTime.Today.ToString("yyyyMMdd") + "_" + name;
            if (File.Exists(filePath + FileName) == false) return FileName;

            FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + name;
            if (File.Exists(filePath + FileName) == false) return FileName;

            return Guid.NewGuid().AsString() + System.IO.Path.GetExtension(name);
        }
        /// <summary>
        /// 暂时没用
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        private bool CheckFileName(string FileName)
        {
            return true;
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
