<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    语言资源列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {

            var myGrid = $(".myGrid", jv.boxdy());

            jv.page().flexiQuery = function () {
                var queryJDiv = $(".divQuery:last", jv.boxdy());

                myGrid.getFlexi().doSearch(queryJDiv.GetDivJson("List_"));
            };

            jv.page().flexiView = function (id) {
                Boxy.load("~/Admin/TxtRes/Detail/" + id + ".aspx", { filter: ".Main:last", modal: true, title: "查看详细" }, function (bxy) {
                    
                });
            };

            jv.page().flexiAdd = function () {
                Boxy.load("~/Admin/TxtRes/AddView.aspx", { filter: ".Main:last", modal: true, title: "添加" }, function (bxy) {
                    
                });
            };

            jv.page().flexiEdit = function (id) {
                var rowData = myGrid.getFlexi().TableRowToData();
                var url = "";
                if (parseInt(rowData.Id)) {
                    url = "~/Admin/TxtRes/Update/" + rowData.ResID + ".aspx";
                }
                else {
                    url = "~/Admin/TxtRes/Add/" + rowData.ResID + ".aspx";
                }

                Boxy.load(url, { filter: ".Main:last", modal: true, title: "修改" }, function (bxy) {
                    

                    //Card页执行保存代码
                });
            };

            jv.page().flexiDel = function (ids) {
                //删除。
                $.post("~/Admin/TxtRes/Delete" + ".aspx", { query: ids.join(",") }, function (res) {
                    if (res.msg) {
                        alert(res.msg);
                    }
                    else {
                        myGrid.getFlexi().populate();
                        alert("删除成功 !");
                    }
                });
            };

            myGrid.flexigrid({
                title: "TxtRes列表标题",
                url: "",
                useSelect:true,
                sortname: "Key",
                role: { id: "ResID", name: "Value" },  //id : 在被引用的时候,返回的Id值 ; name :在被引用的时候,返回的显示值.
                colModel: [
					{ display: "查 看",  bind: false, name: "View", width: "auto",   html: '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID(),event);">查 看</a>' },
                    { display: "Id", name: "Id", width: "0", sortable: true, hide: true },
                    { display: "ResID", name: "ResID", width: "0", sortable: true },
                    { display: "资源", name: "Key", width: "280", sortable: true },
                    { display: "语言值", name: "Value", width: "280", sortable: true },
					{ display: "操 作",  bind: false, name: "Edit", width: "auto",   html: '<a style="cursor:pointer" onclick="jv.page().flexiEdit($(this).getFlexiRowID(),event);">修 改</a>' }
					],
                dataType: "json",
                buttons: [
					{ separator: true },
                		{ name: '添加', bclass: 'add', onpress: jv.page().flexiAdd },

                //					{ separator: true },
                //					{ name: "删除", bclass: "delete", onpress: function (ids) { if (ids.length == 0) return; if (Confirm("确认删除这 " + ids.length + " 项吗？")){if (jv.page().flexiDel(ids) == false) return; } }},
					{separator: true }
					],
                useId: true,
                usepager: true,
                take:50,
                showTableToggleBtn: true,
                onSuccess: function (grid) { jv.TestRole(); }
            });


            myGrid.getFlexi().p.url = "~/Admin/TxtRes/Query.aspx";
            myGrid.getFlexi().populate();
        }); 
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table class="divQuery" style="width: 100%">
        <tr>
            <td class="key">
                资源:
            </td>
            <td class="val">
                <input id="List_Key" />
            </td>
            <td class="key">
                语言值:
            </td>
            <td class="val">
                <input id="List_Value" />
            </td>
            <td class="key">
            </td>
            <td class="val">
            </td>
            <td class="key">
            </td>
            <td class="val query_btn">
                <input class="submit" type="button" value="查询" onclick="jv.page(event).flexiQuery()" />
            </td>
        </tr>
    </table>
    <table class="myGrid" style="display: none;width:auto">
    </table>
</asp:Content>
