<%@ Page Title="" Inherits="MyShopPage<AboutModel>" Language="C#" MasterPageFile="~/Areas/Shop/Views/Shared/ShopSite.master"
    Theme="Default" %>

<%--<%@ OutputCache Duration="180" Location="Any" VaryByParam="None" %>--%>
<asp:Content ID="Content1" ContentPlaceHolderID="BigContent" runat="server">
    <pre class="Detail">
        <%   
            Ronse += Model.Dept.About;

            if (Model.Imgs.Count() > 0)
            {
                using (var div = new MyTag(HtmlTextWriterTag.Div, new { @class = "AboutImg" }))
                {
                    for (int i = 0; i < Model.Imgs.Length; i++)
                    {
                        Ronse += Html.MyTag(HtmlTextWriterTag.Img, "", new { src = Model.Imgs[i], width = "300px" });

                        if (i > 0 && i % 2 == 0)
                            Ronse += Html.MyTag(HtmlTextWriterTag.Br, null, null);
                    }
                }
            }
        %>
    </pre>
    <%-- <div class="NormalTxt Pin Desc" style="min-height: 300px;">
        <pre>
            <%
                 var showItem = Model.Info;

                 for (int i = 0; i < showItem.Length; i++)
                 {
                     if (showItem[i].Trait == DeptDetailTraitEnum.Caption)
                     {
                         Ronse += string.Format("<div class='Cap'><div class='CapInner'>{0}</div></div>", showItem[i].Value);
                     }
                     else if (showItem[i].Trait == DeptDetailTraitEnum.Detail)
                     {
                         Ronse += string.Format("<span class='KeySpan ProductInfoKey'>{0}</span><span class='ValueSpan ProductInfoValue'>{1}</span><div class='SplitLine'></div>"
                             , showItem[i].Key
                             , showItem[i].Value);
                     }
                 }

        %>

        </pre>
    </div>--%>

    <%Html.RenderPartial("SendMsg", Model.Dept); %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SmallContent" runat="server">
</asp:Content>
<asp:Content ID="titleContent" ContentPlaceHolderID="Title" runat="server">
    <%= "和易家社区服务平台 - " +  Model.Dept.Name + " " + ShopMenu.AboutUs.GetRes() + " " + Model.Dept.KeyWords%>
</asp:Content>
