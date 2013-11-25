<%@ Page Language="C#" Inherits="MyShopPage<IndexModel>" MasterPageFile="~/Areas/Shop/Views/Shared/ShopSite.master"
    Theme="Default" %>

<%--<%@ OutputCache Duration="180" Location="Any" VaryByParam="*" %>--%>
<asp:Content ID="titleContent" ContentPlaceHolderID="Title" runat="server">
    <%= "和易家社区服务平台"  + " " +   Model.Dept.Name  + " " + ShopMenu.Home.GetRes() + " " + Model.Dept.KeyWords%>
</asp:Content>
<asp:Content ContentPlaceHolderID="head" ID="headContent" runat="server">
    <script src="~/Res/ImgSlider/ImgSlider.js" type="text/javascript"></script>
    <link href="~/Res/ImgSlider/ImgSlider.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .marquee {
            border: 0px solid #f1f1f1;
        }

            .marquee span {
                margin: 8px;
                text-align: center;
            }
    </style>
</asp:Content>
<asp:Content ContentPlaceHolderID="SmallContent" ID="SmallContentContent" runat="server">
</asp:Content>
<asp:Content ContentPlaceHolderID="BigContent" ID="BigContentContent" runat="server">
    <div id="body_Show">

        <div class="Title">
            <a class="more" href="~/Shop/ShowCase/<%=Model.Dept.WebName %>.aspx"></a>
            <%-- <div class="MainItemInner">
                    <a style="float: right;" class="OtherTxt">
                        <img src="~/Img/icon_arrow.gif" style="border-width: 0px; padding-right: 5px;" alt='<%=GetRes("更多","see more") +"..." %>' />
                    </a> </div>--%>
                    展示商品  
               
        </div>

        <div class="marquee">
            <%
                for (var i = 0; i < Model.ShowCases.Length; i++)
                {
                    Ronse += string.Format(@"<span><a href=""~/Shop/ProductInfo/{3}/{2}.aspx""><img src=""{0}"" width=""160px"" /><br /><label>{1}</label></a></span>"
                        , Model.ShowCases[i].Img
                        , Model.ShowCases[i].Name
                        , Model.ShowCases[i].ProductID
                        , Model.Dept.WebName);
                }
                        
            %>
        </div>

    </div>
    <div id="body_Info">
        <pre class="Detail"><% Ronse += Model.Dept.Detail; %> </pre>
        <div class="corpExtend">
            <ul style="list-style: none; margin: 0px">
                <%
                    for (var i = 0; i < Model.NoticeShowCases.Length; i++)
                    {
                        Response.Write(string.Format("<li>[{0}] <a href='~/Shop/NoticeInfo/{2}/{1}.aspx'>{3}</a></li>"
                            , Model.NoticeShowCases[i].NoticeTypeName
                            , Model.NoticeShowCases[i].NoticeID
                            , Model.Dept.WebName
                            , Model.NoticeShowCases[i].Name));
                    }
                %>
            </ul>
        </div>
    </div>
    <%Html.RenderPartial("SendMsg", Model.Dept); %>
</asp:Content>
