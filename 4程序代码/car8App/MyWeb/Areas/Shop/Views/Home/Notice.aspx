<%@ Page Title="" Inherits="MyShopPage<NoticeModel>" Language="C#" MasterPageFile="~/Areas/Shop/Views/Shared/ShopSite.master"
    Theme="Default" %>

<%--<%@ OutputCache Duration="180" Location="Any" VaryByParam="None" %>--%>
<asp:Content ID="Content1" ContentPlaceHolderID="BigContent" runat="server">
    <div class="PagerDiv">
        <div class="PagerInnerDiv Pin">
            <div style="float: right; vertical-align: middle;">
            </div>
            <span class="WebPager"></span>
        </div>
    </div>

    <% 
        
        for (int i = 0; i < Model.Notices.Count; i++)
        {
            if (i > 0)
            {
                Ronse += Html.MyTag(HtmlTextWriterTag.Div, null, new { @class = "split" });
            }
            string url = "~/Shop/NoticeInfo/" + Model.Dept.WebName + "/" + Model.Notices[i].Id + ".aspx";

            using (var d1 = new MyTag(HtmlTextWriterTag.Div, new { style = "padding:8px" }))
            {
                //using (var a = new MyTag(HtmlTextWriterTag.A, new { href = url, style = "float:left;padding:5px;" }))
                //{
                //    Ronse += Html.MyTag(HtmlTextWriterTag.Img, null, new { style = "border-width: 0px; width: 125px;", src = Model.Notices[i].Img });
                //}
                using (var a = new MyTag(HtmlTextWriterTag.Span, new { href = url, style = "float:left;padding:5px;" }))
                {
                    Ronse += "[" + Model.Notices[i].NoticeType + "]";
                }

                using (var d2 = new MyTag(HtmlTextWriterTag.Div, new { @class = "NormalTxt", style = "min-height:80px" }))
                {
                    Ronse += Html.MyTag(HtmlTextWriterTag.A,
                         "(" + (i + 1) + ") " + Model.Notices[i].Name,
                         new { href = url }
                         );

                    Ronse += "<br />";

                    var txt = Model.Notices[i].Descr.GetDisplayText();
                    Ronse += Html.MyTag(HtmlTextWriterTag.Span, txt.Text, new { title = txt.ToolTip, style = "display:inline" });
                }
            }
        }
    %>

    <div class="PagerDiv">
        <div class="PagerInnerDiv Pin">
            <span class="WebPager"></span>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="~/Res/MyPager/MyPager.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SmallContent" runat="server">
</asp:Content>
<asp:Content ID="titleContent" ContentPlaceHolderID="Title" runat="server">
    <%= "和易家社区服务平台" + " " + Model.Dept.Name + " " + ShopMenu.Notice.GetRes() + " " + Model.Dept.KeyWords%>
</asp:Content>
