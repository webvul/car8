<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    菜单列表
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

            jv.page().flexiView = function (rowData, rowIndex, grid) {
                var id = rowData.id;
                Boxy.load("~/Admin/Menu/Detail/" + id + ".aspx", { filter: ".Main:last", modal: true, title: "查看详细" }, function (bxy) {

                });
            };

            jv.page().flexiAdd = function () {
                Boxy.load("~/Admin/Menu/Add" + ".aspx", { filter: ".Main:last", modal: true, title: "添加" }, function (bxy) {

                });
            };

            jv.page().flexiEdit = function (rowData, rowIndex, grid) {
                var id = rowData.id;
                Boxy.load("~/Admin/Menu/Update/" + id + ".aspx", { filter: ".Main:last", modal: true, title: "修改" }, function (bxy) {


                    //Edit页执行保存代码
                });
            };

            jv.page().flexiDelete = function (rowData, rowIndex, grid) {

                if (!Confirm("确认删除该项吗？ ")) return;

                var id = rowData.id;
                //删除。
                $.post("~/Admin/Menu/Delete.aspx", { query: id }, function (res) {
                    if (res.msg) { alert(res.msg); }
                    else {
                        myGrid.getFlexi().populate();
                        alert("删除成功 !");
                    }
                });
            };

            jv.page().adds = [];
            jv.page().minus = [];


            jv.page().flexiSave = function (ids, grid) {
                $.post("~/Admin/Menu/PowerSave" + ".aspx" + jv.url().search, { Adds: jv.page().adds.join(","), Minus: jv.page().minus.join(",") }, function (res) {
                    if (res.msg) {
                        alert(res.msg);
                    }

                    alert("保存成功.");
                });

            };


            var btns;

            if (jv.page().action == "ListPower") {
                btns = [{ separator: true },
					{ name: '保存', bclass: 'save pin', onpress: jv.page().flexiSave }];
            }
            else {
                btns = [
					{ separator: true },
					{ name: '添加', bclass: 'add', onpress: jv.page().flexiAdd },
					{ separator: true },
					{ name: "删除", bclass: "delete", onpress: function (ids, grid) { if (ids.length == 0) return; if (Confirm("确认删除这 " + ids.length + " 项吗？")) { if (jv.page().flexiDel(ids, grid) == false) return; } } },
					{ separator: true }
                ];
            }
            myGrid.flexigrid({
                title: "菜单列表",
                url: "",
                role: { id: "Id", name: "Text", select: "Sel" },
                treeOpen: false,
                useId: 2,
                keepQuote: true,
                FillHeight: true,
                colModel: [
                    { display: "Id", name: "Id", width: 0, hide: true },
                    { display: "PID", name: "Pid", width: 0, hide: true },
                    { display: "根路径", name: "RootPath", width: 0, align: "left" },
                    { display: function () { return "显示名称" }, name: "Text", width: 200, align: "left" },
                    { display: "网址", toggle: true, name: "Url", css: "Wrap" },
                    { display: "排序号", toggle: true, name: "SortID", align: "left" },
                    { display: "Status", name: "Status", width: 0, align: "left" },
                    {
                        display: "状态", toggle: true, name: "Status_Display", css: "pin", bind: false, align: "center",
                        format: function (row, rowIndex, g, td) {
                            var data = g.getJson(row), imgUrl = "", alt = "";
                            if (data["Status"] == "Enable") {
                                imgUrl = "~/Img/OK_CBlue.gif";
                                alt = "启用";
                            }
                            else {
                                imgUrl = "~/Img/Stop_CYellow.gif";
                                alt = "禁用";
                            }

                            return '<img width="24px" src="{0}" alt="{1}" title="{1}"/>'.format(imgUrl, alt);
                        }
                    },
                    { display: "Sel", name: "Sel", width: 0, css: "NoWrap" },
					{
					    display: "&nbsp;", bind: false, name: "View", css: "NoWrap", align: "center",
					    html: '<a style="cursor:pointer" onclick="jv.flexiRowEvent(jv.page().flexiView,event);">查 看</a>'
					},
					{
					    display: "&nbsp;", bind: false, name: "Edit", css: "NoWrap", align: "center",
					    html: '<a style="cursor:pointer" onclick="jv.flexiRowEvent(jv.page().flexiEdit,event);">修 改</a>'
					},
                    {
                        display: "&nbsp;", bind: false, name: "Delete", css: "NoWrap", align: "center",
                        html: '<a style="cursor:pointer" onclick="jv.flexiRowEvent(jv.page().flexiDelete,event);">删 除</a>'
                    }
                ],

                //useSelect: (jv.page().uid == "UpdateMain" || (jv.page().uid == "Update")) ? { width: 10, display: "", colIndex: "before"} : -1,
                buttons: btns,
                usepager: true,
                selectExtend: (jv.page().Mode == "Minus" ? "down" : "up,down"),
                onSelected: function (tr, g) {
                    var id = tr.id.slice(3);
                    if (g.isSelected(tr)) {
                        if (jv.page().minus.indexOf(id) >= 0) jv.page().minus.remove(id);
                        if (jv.page().adds.indexOf(id) < 0) jv.page().adds.push(id);
                    } else {
                        if (jv.page().adds.indexOf(id) >= 0) jv.page().adds.remove(id);
                        if (jv.page().minus.indexOf(id) < 0) jv.page().minus.push(id);
                    }
                },
                resizable: false,
                take: 5
            });


            myGrid.getFlexi().p.url = "~/Admin/Menu/Query" + (jv.page().action == "ListPower" ? "/ListPower" : "") + ".aspx" + jv.url().search;
            myGrid.getFlexi().populate();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="divQuery" style="display: none">
        <div class="input">
            显示名称:<input type="text" id="List_Text" style="width: 120px;" />
        </div>
        <br />
        <input class="submit" type="button" value="查询" />
    </div>
    <table class="myGrid" style="display: none;">
    </table>
</asp:Content>
