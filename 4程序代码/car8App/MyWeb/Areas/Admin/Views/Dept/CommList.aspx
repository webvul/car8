<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    小区列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());

            jv.page().flexiQuery = function (ids, grid) {
                var queryJDiv = $(".divQuery:last", jv.boxdy());
                var searchOption = queryJDiv.GetDivJson("List_");
                myGrid.getFlexi().doSearch(searchOption);
            };


            myGrid.flexigrid({
                title: "",
                url: "",
                role: { name: "CommName", id: "CommID" },
                colModel: [
                     { display: "CommID", name: "CommID", sortable: true, width: 0 },
                     { display: "小区名称", name: "CommName", sortable: true },
                ],
                dataType: "json",
                usepager: true,
                useRp: true,
                rp: 15,
                resizable: false,
                showTableToggleBtn: true
            });

            myGrid.getFlexi().p.url = "~/Admin/Dept/CommunityQuery.aspx";
            jv.page().flexiQuery();
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="MyTool">
        <input type="button" value="查询" onclick="jv.page(event).flexiQuery();" />

    </div>

    <table class="divQuery" style="width: 100%">
        <tr>
            <td class="key">小区名称:
            </td>
            <td class="val">
                <input type="text" id="List_CommName" style="width: 120px;" />
            </td>
            <td class="key"></td>
            <td class="val"></td>
            <td class="key"></td>
            <td class="val"></td>
            <td class="key"></td>
            <td class="val "></td>
        </tr>
    </table>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
