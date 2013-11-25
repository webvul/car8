<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;

public class Handler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {

        context.Response.ContentType = "text/plain";

        string result = "";

        if (context.Request.Params["key"] != null && context.Request.Params["key"].Length < 5)
        {
            string key = context.Request.Params["key"];
            if (context.Cache.Get(key) != null)
                result = context.Cache.Get(key).ToString();
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    result += key + i.ToString() + ";";
                }
                context.Cache.Add(
                    key,
                    result,
                    null,
                    DateTime.Now.AddMinutes(3),
                    System.Web.Caching.Cache.NoSlidingExpiration,
                    System.Web.Caching.CacheItemPriority.Default,
                    null);
            }
            context.Response.Write(result);
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}