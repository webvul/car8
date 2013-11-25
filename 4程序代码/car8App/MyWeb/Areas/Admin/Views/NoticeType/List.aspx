<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    公告类型列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());

            jv.page().flexiQuery = function (ids, grid) {
                var queryJDiv = $(".divQuery:last", jv.boxdy());
                var searchOption = queryJDiv.GetDivJson("List_");
                searchOption.RefId = jv.page()["Wbs"] || "";
                myGrid.getFlexi().doSearch(searchOption);
            };

            jv.page().flexiView = function (id) {


                Boxy.load("~/Admin/NoticeType/Detail/" + id + ".aspx", { filter: ".Main", modal: true, title: "查看详细" }, function (bxy) {
                    

                    $(".Main").find(".submit").one("click", function () {
                        Boxy.get(".Main").hide();
                    });
                });
            }
            jv.page().flexiAdd = function () {

                Boxy.load("~/Admin/NoticeType/Add" + ".aspx", { filter: ".Main", modal: true, title: "添加", width: 701 });
            };

            jv.page().flexiEdit = function (id) {

                Boxy.load("~/Admin/NoticeType/Update/" + id + ".aspx", { filter: ".Main", modal: true, title: "修改", width: 701 });
            };

            jv.page().flexiDel = function (ids, grid) {
                var grid = myGrid.getFlexi();
                var ids = grid.getSelectIds();
                if (ids.length == 0) return;
                if (confirm("确认删除这 " + ids.length + " 项吗？") == false) return;


                //删除。
                $.post("~/Admin/NoticeType/Delete.aspx", { IDs: ids.join(",") }, function (res) {
                    if (!res.msg) {
                        grid.populate();
                        return alert("删除成功 !");
                    }
                    else { alert(res.msg); }
                });
            };


            myGrid.flexigrid({
                title: "",
                url: "",
                colModel: [
                     { display: "查 看", bind: false, name: "View", width: 30, align: "center", html: '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID());">查 看</a>' },
                     { display: "ID", name: "Id", width: 80, sortable: true, align: "center" },
                     { display: "名称", name: "Name", width: 80, sortable: true, align: "center" },
                //                     { display: "父种类", name: "PID", width: 80, sortable: true, align: "center", format: '<a target="_blank" href="~/Admin/ProductType/List/$#$.aspx">$#$</a>' },
                     { display: "描述", name: "Descr", width: 80, sortable: true, align: "center" },
                     { display: "SortID", name: "SortID", width: 80, sortable: true, align: "center" },
                     { display: "添加时间", name: "AddTime", width: 100, sortable: true, align: "center" },
                     { display: "修 改", bind: false, name: "Edit", width: 30, align: "center", html: '<a style="cursor:pointer" onclick="jv.page().flexiEdit($(this).getFlexiRowID());">修 改</a>' }
                ],
                dataType: "json",
                usepager: true,
                useRp: true,
                rp: 15,
                showTableToggleBtn: true,
                onSuccess: function (grid) { jv.TestRole(); }
            });
            myGrid.getFlexi().p.url = "~/Admin/NoticeType/Query.aspx";
            jv.page().flexiQuery();
        });

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="MyTool">
        <input type="button" value="查询" onclick="jv.page(event).flexiQuery();" />
        <input type="button" value="添加" onclick="jv.page(event).flexiAdd();" />
        <input type="button" value="删除" onclick="jv.page(event).flexiDel();" />

    </div>
    <table class="divQuery" style="width: 100%">
        <tr>
            <td class="key">类 别 名:
            </td>
            <td class="val">
                <input type="text" id="List_Name" />
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
