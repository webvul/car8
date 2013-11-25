<%@ Page Language="C#" MasterPageFile="~/Areas/Host/Views/Shared/Host.Master"
    Inherits="MyCon.MyHostPage<List<DeptRule.Entity>>" Title="和易家社区服务平台" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="~/Areas/Host/Res/MyCss.css" rel="stylesheet" />
    <style type="text/css">
        .ShopItem {
            width: 131px;
            border-width: 0;
            margin: 3px;
            padding: 1px;
            text-align: center;
        }

            .ShopItem img {
                border: solid 1px #a0a0a0;
                width: 131px;
                margin: 0;
                height: 80px;
                display: block;
            }

            .ShopItem span {
                font-size: 14px;
                font-weight: bold;
            }
    </style>
    <script type="text/javascript">
        $(function () {
            var shops = $("#hid_shopCount").val();
            for (var i = 0; i < shops; i++) {
                var txt = $("#d" + i).text();

                $("#d" + i).after(document.createTextNode(txt));
                $("#d" + i).remove();
            }
        });
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <div id="top">
    </div>
    <div class="toplogo">
        和易家
    </div>
    <div class="favorite" onclick="javascript:jv.AddToFavorite(window.location,'和易家','和易家社区服务平台');">
        添加到收藏夹<div class="fav Inline">
        </div>
    </div>

    <div style="height: 80px;"></div>
    <%
        
        var bizType = Model.Select(o => o.BizType).Distinct();
        foreach (var t in bizType)
        {
    %>
    <div class="Sect" style="text-align: left; margin: 5px;">
        <div class="Title">
            <a><%=t.GetRes() %></a>
        </div>
        <div style="text-align: center;">

            <%
            var i = -1;
            var shops = Model.Where(o => o.BizType == t);
            Ronse += "<input type='hidden' id='hid_shopCount' value='" + shops.Count() + "' />";
            foreach (var s in shops)
            {
                i++;
            %>
            <div style="vertical-align: top; min-height: 150px; text-align: left;">
                <div class="ShopItem" style="float: left">
                    <a href="~/Shop/Index/<%=s.WebName %>.aspx">
                        <img src="<%=s.GetAnnexByTitleExtend().GetUrlFull() %>" /><span><%=s.Name %></span></a>
                </div>
                <div id="d<%=i %>"><%=s.Detail %></div>
            </div>
            <%} %>
        </div>
    </div>
    <%} %>
</asp:Content>
