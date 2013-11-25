<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    产品详情
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());

            jv.page().flexiQuery = function (ids, grid) {
                var queryJDiv = $(".divQuery:last", jv.boxdy());

                new Boxy(queryJDiv, { modal: true, title: "查询" }).resize(400, 100);
                queryJDiv.find(".submit").one("click", function () {
                    //查询。

                    grid.doSearch(queryJDiv.GetDivJson("List_"));
                    Boxy.get(queryJDiv).hide();
                });
            };

            jv.page().flexiView = function (id) {
                Boxy.load("~/Admin/ProductDetail/Detail/" + id + ".aspx", { filter: ".Main", modal: true, title: "查看详细" }, function (bxy) {


                    $(".Main").find(".submit").one("click", function () {
                        Boxy.get(".Main").hide();
                    });
                });
            };

            jv.page().flexiAdd = function () {

                Boxy.load("~/Admin/ProductDetail/Add/" + jv.page()["uid"] + ".aspx", { filter: ".Main", modal: true, title: "添加" }, function (bxy) {

                });
            };

            jv.page().flexiEdit = function (id) {

                Boxy.load("~/Admin/ProductDetail/Update/" + id + ".aspx", { filter: ".Main", modal: true, title: "修改" }, function (bxy) {


                    //Edit页执行保存代码
                });
            };

            jv.page().flexiDel = function (ids, grid) {
                //删除。
                $.post("~/Admin/ProductDetail/Delete.aspx", { query: ids.join(",") }, function (res) {
                    if (!res.msg) alert("删除成功 !");
                    else { alert(res.msg); }
                });
            };

            myGrid.flexigrid({
                title: "ProductDetail列表标题",
                url: "",
                role: { name: "Key" },
                colModel: [
                     { display: "查 看", bind: false, name: "View", width: 30, align: "center", html: '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID());">查 看</a>' },
                     { display: "ID", name: "ID", align: "center", hide: true },
                     { display: "键", sortable: true, align: "center", name: "Key" },
                     { display: "值", sortable: true, align: "center", name: "Value" },
                     { display: "是否是标题", name: "IsCaption", sortable: true, align: "center", format: function (rowData, rowIndex, grid, td, event) { var txt = rowData.cell[this.indexOfBind]; var d = { Caption: "标题项", Detail: "内容" }; return d[txt] || txt; } },
                     { display: "排序", name: "SortID", width: 80, sortable: true, align: "center" },
                     { display: "修 改", bind: false, name: "Edit", width: 30, align: "center", html: '<a style="cursor:pointer" onclick="jv.page().flexiEdit($(this).getFlexiRowID());">修 改</a>' }
                ],
                dataType: "json",
                buttons: [
                     { separator: true },
                     { name: "查询", bclass: "query", onpress: jv.page().flexiQuery },
                     { separator: true },
                     { name: '添加', bclass: 'add', onpress: jv.page().flexiAdd },
                     { separator: true },
                     { name: "删除", bclass: "delete", onpress: function (ids, grid) { if (ids.length == 0) return; if (confirm("确认删除这 " + ids.length + " 项吗？") == false) return; if (jv.page().flexiDel(ids, grid) == false) return; grid.populate(); } },
                     { separator: true }
                ],
                usepager: true,
                useRp: true,
                rp: 15,
                showTableToggleBtn: true
            });
            myGrid.getFlexi().p.url = "~/Admin/ProductDetail/Query" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
            myGrid.getFlexi().populate();

            $("#List_Trait").TextHelper({ datatype: "json", data: { Default: "描述", Caption: "标题项", Detail: "标题值" } });
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="divQuery" style="display: none">
        <div class="input">
            键:<input type="text" id="List_Key" />
            值:<input type="text" id="List_Value" /><br />
            特性:<input type="text" id="List_Trait" />
            <br />
        </div>
        <br />
        <input class="submit" type="button" value="查询" />
    </div>
    <table class="myGrid" style="display: none">
    </table>

</asp:Content>
