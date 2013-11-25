<%@ Control Language="C#" Inherits="MyMvcVuc<DeptRule.Entity>" %>
<div class="CorpInfo">

    <div class="Title">商户信息</div>
    <div class="split"></div>
    <div class="kv">
        <span class="key">商户名称</span>
        <span class="val"><%=Model.Name   %></span>
    </div>
    <div class="kv">
        <span class="key">业务类型</span>
        <span class="val"><%= string.Join(" , ",  Model.BizType .ToEnumList().Select(o=>o.GetRes().AsString()).ToArray())%></span>
    </div>

    <div class="kv">
        <span class="key">商户电话</span>
        <span class="val"><%=  Model.Phone%></span>
    </div>

    <div class="kv">
        <span class="key">手机</span>
        <span class="val"><%=Model.GetPersons().FirstOrDefault().Mobile%></span>
    </div>
    <div class="CorpTail">
        <div class="kv">
            <img class="QQImage" src="~/Img/QQ.png" />
            <%
            
                Response.Write(Html.MyTag(HtmlTextWriterTag.A,
                    Model.GetPersons().FirstOrDefault().Qq,
                    new { target = "_blank", href = "tencent://message/?uin=" + Model.GetPersons().FirstOrDefault().Qq + "&Site=和易家社区服务平台&Menu=yes" }
                    ));
                 
            %>
        </div>
        <div class="kv">
            <img class="EmailImage" src="~/Img/Email.png" />
            <a target="_blank" href='mailto:<%=Model.GetPersons().FirstOrDefault().Email %>'>
                <%=Model.GetPersons().FirstOrDefault().Email%></a>
        </div>

    </div>
</div>
