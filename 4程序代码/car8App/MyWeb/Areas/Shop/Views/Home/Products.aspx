<%@ Page Title="" Inherits="MyShopPage<ProductsModel>" Language="C#" MasterPageFile="~/Areas/Shop/Views/Shared/ShopSite.master"
    Theme="Default" %>

<%--<%@ OutputCache Duration="180" Location="Any" VaryByParam="None" %>--%>
<asp:Content ID="Content1" ContentPlaceHolderID="BigContent" runat="server">
    <div class="PagerDiv">
        <div class="PagerInnerDiv Pin">
            <div style="float: right; vertical-align: middle;">
                <!--   <%if (Request.QueryString["IsList"] == "true")
                         { %>
                <img src="~/Img/display_type_gallery_current.gif" />
                <% 
                             Ronse += Html.MyTag(HtmlTextWriterTag.A, "平铺方式查看", new { href = "~/Shop/Products/" + Model.Dept.WebName + ".aspx" });
                         }
                         else
                         { %>
                <img src="~/Img/display_type_list_current.gif" />
                <%
                             Ronse += Html.MyTag(HtmlTextWriterTag.A, "列表方式查看", new { href = "~/Shop/Products/" + Model.Dept.WebName + ".aspx?IsList=true" });
                         } %>
             -->
            </div>
            <span class="WebPager"></span>
        </div>
    </div>
    <%
        for (int i = 0; i < Model.Products.Count; i++)
        {
            if (i > 0)
            {
                Ronse += Html.MyTag(HtmlTextWriterTag.Div, null, new { @class = "SplitLine" });
            }
            string url = "~/Shop/ProductInfo/" + Model.Dept.WebName + "/" + Model.Products[i].Id + ".aspx";

            using (var d1 = new MyTag(HtmlTextWriterTag.Div, new { style = "padding:8px" }))
            {
                using (var a = new MyTag(HtmlTextWriterTag.A, new { href = url, style = "float:left;padding:5px;" }))
                {
                    Ronse += Html.MyTag(HtmlTextWriterTag.Img, null, new { style = "border-width: 0px; width: 125px;", src = Model.Products[i].Img });
                }

                using (var d2 = new MyTag(HtmlTextWriterTag.Div, new { @class = "NormalTxt", style = "min-height:200px" }))
                {
                    Ronse += Html.MyTag(HtmlTextWriterTag.A,
                         "(" + (i + 1) + ") " + Model.Products[i].Name,
                         new { href = url }
                         );

                    Ronse += "<br />";

                    var txt = Model.Products[i].Descr.GetDisplayText();
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
    <%Html.RenderPartial("FixedMsg"); %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="~/Res/MyPager/MyPager.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SmallContent" runat="server">
</asp:Content>

<asp:Content ID="titleContent" ContentPlaceHolderID="Title" runat="server">
    <%= "和易家社区服务平台" + " " +  Model.Dept.Name + " " + Model.Dept.KeyWords %>
</asp:Content>
