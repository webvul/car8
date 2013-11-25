<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Main.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    地产客服投诉分析表
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="top" style="height: 20px;">
    </div>
    <div id="treeMenu" style="margin-top: 7px; padding-left: 7px; margin-bottom: 7px;
        overflow: auto; width: 200px">
        <ul class="Pucker">
            <%= Html.MyPucker(PuckerNode.LoadMenu(0).SubNode,false)%></ul>
    </div>
    <div id="main" style="position: absolute; top: 20px; left: 200px; background-color: White;
        width: 500px; height: 500px;">
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="~/Res/MyPucker/MyPucker.css" />
    <script type="text/javascript">
        $(function () {
            $(".Pucker").MyPucker({ click: function (a) { $("#main").LoadView({ url: a.getAttribute("url") }); } });
        });
    </script>
</asp:Content>
