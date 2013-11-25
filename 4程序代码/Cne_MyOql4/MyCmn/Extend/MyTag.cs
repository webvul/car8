using System.Web.UI;


namespace System.Web.Mvc
{
    /// <summary>
    /// 生成自定义Tag标签.
    /// </summary>
    /// <example>
    /// <code>
    /// using (MyTag tag = new MyTag(HtmlTextWriterTag.A, new { href = "g.cn", id = "id" }))
    /// {
    ///      Ronse += "Google";
    /// }
    /// </code>
    /// </example>
    public class MyTag : IDisposable
    {
        protected HtmlTextWriterTag Tag { get; set; }
        public MyTag(HtmlTextWriterTag tag, object Attributes)
        {
            this.Tag = tag;
            HttpContext.Current.Response.Write(MyHelper.BeginTag(null, tag, Attributes));
        }

        public void Dispose()
        {
            HttpContext.Current.Response.Write(MyHelper.EndTag(null, this.Tag));
        }
    }
}
