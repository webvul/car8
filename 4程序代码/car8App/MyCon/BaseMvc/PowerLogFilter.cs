using System;
using System.Web.Mvc;


/// <summary>
/// 如果设置了该过滤器, 则过滤权限检查. (默认启用权限检查.)
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class NoPowerFilter : FilterAttribute, IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext filterContext)
    {

    }

    public void OnActionExecuting(ActionExecutingContext filterContext)
    {

    }
}


/// <summary>
/// 如果设置了该过滤器, 则过滤日志. (默认记录日志.)
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class NoLogFilter : FilterAttribute, IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext filterContext)
    {

    }

    public void OnActionExecuting(ActionExecutingContext filterContext)
    {

    }
}