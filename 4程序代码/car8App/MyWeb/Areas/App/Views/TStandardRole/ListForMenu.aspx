<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/App/Views/Shared/App.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    菜单标准角色
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());

            jv.page().flexiQuery = function () {
                var queryJDiv = $(".divQuery:last", jv.boxdy());
                myGrid.getFlexi().doSearch(queryJDiv.GetDivJson("List_"));
            };

            jv.page().flexiPop = function (id) {
                jv.PopList({ list: 'PopForUser', area: 'Master', entity: 'TStandardRole', mode: 'none',
                    callback: function (role, data) {

                    }, uid: id, width: 400

                });
            };
            jv.page().flexiExport = function (id) {
                document.location = "~/Master/TStandardRole/InsertToExcel.aspx?Menu=" + encodeURI($("#List_Menu").val()) + "&StandardRole=" + encodeURI($("#List_StandardRole").val());
            };


            myGrid.flexigrid({
                title: "菜单标准角色列表",
                url: "",
                role: { id: "StandardRoleId", name: "Name" },  //id : 在被引用的时候,返回的Id值 ; name :在被引用的时候,返回的显示值.
                colModel: [
                  { display: "StandardRoleId", name: "StandardRoleId", width: "0", align: "left" },
                  { display: "菜单", name: "Menu", width: "auto", align: "left" },
                  { display: "标准角色", name: "Name", width: "auto", align: "left", format: '<a style="cursor:pointer;color:blue;" onclick="jv.page().flexiPop($(this).getFlexiRowID(),event);">$#$</a>' },
					],
                dataType: "json",
                resizable: true,
                buttons: [
                { separator: true },
                { name: "导出", bclass: "query", onpress: jv.page().flexiExport },
                { separator: true }
					],
                usepager: true,
                take:15,
                showTableToggleBtn: true,
                onSuccess: function (grid) { jv.TestRole(); }
            });

            myGrid.getFlexi().p.url = "~/Master/TStandardRole/ListForMenuQuery" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
            //            myGrid.getFlexi().populate();
        }); 
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table class="divQuery" style="width: 100%">
        <tr>
            <td class="key">
                菜单：
            </td>
            <td class="val">
                <input type="text" id="List_Menu" />
            </td>
            <td class="key">
                标准角色：
            </td>
            <td class="val">
                <input type="text" id="List_StandardRole" />
            </td>
            <td class="key">
            </td>
            <td class="val">
            </td>
            <td class="key">
            </td>
            <td class="val query_btn">
                <input class="submit" type="button" value="查询" onclick="jv.page().flexiQuery()" />
            </td>
        </tr>
    </table>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
