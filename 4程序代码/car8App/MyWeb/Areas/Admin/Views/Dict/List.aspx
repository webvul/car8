<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    字典项列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());
            var me = jv.page();

            me.flexiQuery = function (ids, grid) {
                var queryJDiv = $(".divQuery:last", jv.boxdy());
                myGrid.getFlexi().doSearch(queryJDiv.GetDivJson("List_"));
            };

            me.flexiView = function (id) {
                

                Boxy.load("~/Admin/Dict/Detail/" + id + ".aspx", { filter: ".Main", modal: true, title: "查看详细" }, function (bxy) {
                    

                    $(".Main").find(".submit").one("click", function () {
                        Boxy.get(".Main").hide();
                    });
                });
            };

            me.flexiAdd = function () {
                
                Boxy.load("~/Admin/Dict/Add.aspx", { filter: ".Main", modal: true, title: "添加" }, function (bxy) {
                    
                });
            };

            me.flexiEdit = function (id) {
                
                Boxy.load("~/Admin/Dict/Update/" + id + ".aspx", { filter: ".Main", modal: true, title: "修改" }, function (bxy) {
                    

                    //Edit页执行保存代码
                });
            };

            me.flexiDel = function (ids, grid) {
                //删除。
                $.post("~/Admin/Dict/Delete.aspx", { query: ids.join(",") }, function (res) {
                    if (!res.msg) alert("删除成功 !");
                    else { alert(res.msg); }
                });
            };

            myGrid.flexigrid({
                title: "字典列表",
                url: "",
                role: { id: "Id", name: "Key" },
                colModel: [
                     { display: "查 看",  bind: false, name: "View", width: 30,   html: '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID());">查 看</a>' },
                     { display: "ID", name: "Id", width: 80, sortable: true, hide: true },
                     { display: "组", name: "Group", width: 80, sortable: true, toggle: true },
                     { display: "键", name: "Key", width: 150, sortable: true, toggle: true },
                     { display: "值", name: "Value", width: 80, sortable: true, toggle: true },
                     { display: "排序", name: "SortID", width: 80, sortable: true, toggle: true },
                     { display: "商户名称", name: "DeptName", width: 80, sortable: true },
                     { display: "操 作",  bind: false, name: "Edit", width: 30,   html: '<a style="cursor:pointer" onclick="jv.page().flexiEdit($(this).getFlexiRowID());">修 改</a>' }
                     ],
                dataType: "json",
                buttons: [
                     { separator: true },
                     { name: '添加', bclass: 'add', onpress: jv.page().flexiAdd },
                     { separator: true },
                     { name: "删除", bclass: "delete", onpress: function (ids, grid) { if (ids.length == 0) return; if (Confirm("确认删除这 " + ids.length + " 项吗？")) { if (jv.page().flexiDel(ids, grid) == false) return; grid.populate(); } } },
                     { separator: true }
                     ],
                usepager: true,
                useRp: true,
                rp: 15,
                showTableToggleBtn: true
            });
            myGrid.getFlexi().p.url = "~/Admin/Dict/Query" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
            myGrid.getFlexi().populate();
        }); 

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table class="divQuery" style="width: 100%">
        <tr>
            <td class="key">
                组:
            </td>
            <td class="val">
                <input type="text" id="List_Code" />
            </td>
            <td class="key">
                键:
            </td>
            <td class="val">
                <input type="text" id="List_Id" />
            </td>
            <td class="key">
                值:
            </td>
            <td class="val">
                <input type="text" id="List_Name" />
            </td>
            <td colspan="2" class="query_btn">
                <input class="submit" type="button" value="查询" onclick="jv.page(event).flexiQuery();" />
            </td>
        </tr>
    </table>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
