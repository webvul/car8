<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Shop/Views/Shared/ShopSite.master"
    Inherits="MyShopPage<NoticeInfoModel>" Theme="Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="BigContent" runat="server">
    <div class="Detail">
        <%   
            Ronse += Html.MyTag(HtmlTextWriterTag.Span, Model.NoticeInfo.Name, new { @class = "Title" });
            Ronse += "<br />";
        %>
        <pre><% 
                 Ronse += Model.NoticeInfo.Descr;
                 Ronse += "<br />";

        %></pre>
    </div>
    <%Html.RenderPartial("FixedMsg");%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SmallContent" runat="server">
</asp:Content>

<asp:Content ID="titleContent" ContentPlaceHolderID="Title" runat="server">
    <%= "和易家社区服务平台" + " " +   Model.Dept.Name  + " " + Model.Dept.KeyWords%>
</asp:Content>
