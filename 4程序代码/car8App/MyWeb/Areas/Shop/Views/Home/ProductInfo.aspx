<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Shop/Views/Shared/ShopSite.master"
    Inherits="MyShopPage<ProductInfoModel>" Theme="Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="BigContent" runat="server">
    <div class="Detail">
        <div class="ImgSlider">
            <div>
                <%
                    for (int i = 0; i < Model.Imgs.Length; i++)
                    {
                        Ronse += string.Format(@"<span><a><img src=""{0}"" /></a></span>", Model.Imgs[i]);
                    } 
                %>
            </div>
        </div>
        <%   
            Ronse += Html.MyTag(HtmlTextWriterTag.Div, Model.ProductInfo.Name, new { @class = "Title" });
        %>
        <pre><% 
                 Ronse += Model.ProductInfo.Descr;
        %><br /></pre>
    </div>
    <div class="tabInfo">
        <%
            var showItem = Model.ProductInfo.GetProductDetails().OrderBy(o => o.SortID).ToArray();

            for (int i = 0; i < showItem.Length; i++)
            {
                if (showItem[i].IsCaption)
                {
                    Ronse += string.Format("<div class='Cap'>{0}</div>", showItem[i].Key);
                }
                else
                {
                    Ronse += string.Format("<span class='key'>{0}</span><span class='val'>{1}</span>"
                        , showItem[i].Key
                        , showItem[i].Value);
                    Ronse += "<div class='SplitLine'></div>";
                }
            }

        %>
    </div>
    <%Html.RenderPartial("FixedMsg");%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="~/Res/ImgSlider/ImgSlider.js" type="text/javascript"></script>
    <link href="~/Res/ImgSlider/ImgSlider.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SmallContent" runat="server">
</asp:Content>

<asp:Content ID="titleContent" ContentPlaceHolderID="Title" runat="server">
    <%= "和易家社区服务平台" + " " +   Model.Dept.Name  + " " + Model.Dept.KeyWords%>
</asp:Content>
