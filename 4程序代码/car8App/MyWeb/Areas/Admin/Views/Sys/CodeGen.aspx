<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyMvcPage<string[]>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    CodeGen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form action="" method="post">
    <div class="idTabs">
        <ul>
            <li><a href="#one">生成实体</a></li>
            <li><a href="#two">生成列表</a></li>
            <li><a href="#three">生成编辑</a></li>
        </ul>
        <div class="items">
            <div id="one">
                <input type="button" value="生成实体" onclick="toEnt();"/>
            </div>
            <div id="two">
                You can do anything with <b>idTabs</b>.</div>
            <div id="three">
                Anything at all.</div>
        </div>
    </div>
    <div>
        <input type="radio" name="GenType" id="gen2" value="Ent" /><label for="gen2">生成实体</label>
        <input type="radio" name="GenType" id="gen1" value="List" /><label for="gen1">生成列表</label>
        <input type="radio" name="GenType" id="gen3" value="Edit" /><label for="gen3">生成Edit</label>
        <div>
            实体：
            <%=Html.TextHelper("ent", Model,500) %>
        </div>
        <button type="button" onclick='javascript:$("form").attr("action","~/Admin/Sys/" + $("[name=GenType]:checked").val() +"/"+$("#ent").val());$("form").submit();'>
            提交</button>
    </div>
    </form>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <link href="~/Res/TextHelper/TextHelper.css" rel="stylesheet" type="text/css" />
    <script src="~/Res/TextHelper/TextHelper.js" type="text/javascript"></script>
    <link href="~/Res/tab/jquery-ui-1.8.custom.css" rel="stylesheet" type="text/css" />
    <script src="~/Res/tab/jquery-ui-1.8.custom.min.js" type="text/javascript"></script>
    <style type="text/css">
        *
        {
            margin: 6px;
            padding: 6px;
        }
    </style>
    <script type="text/javascript">
        function toEnt() {
            window.location = "~/Admin/Sys/Ent";
        }
        $(function () {
            $("[name=GenType]").click(function () {
                var pre = $("[name=GenType]:checked").val();
                $("#prefix").val(pre);
            });

            $(".idTabs").tabs();
        });
    </script>
</asp:Content>
