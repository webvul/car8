<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    首页展示公告
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


                Boxy.load("~/Admin/NoticeInfo/Detail/" + id + ".aspx", { filter: ".Main", modal: true, title: "查看详细" }, function (bxy) {
                    

                    $(".Main").find(".submit").one("click", function () {
                        Boxy.get(".Main").hide();
                    });
                });
            };

            jv.page().flexiAdd = function () {
                jv.PopList({
                    area: "Admin", entity: "NoticeInfo", mode: "check", callback: function (role, data) {
                        var ary = Array();
                        $(data).each(function (i, d) { ary.push(d.id); });
                        $.post("~/Admin/NoticeInfo/HotAdd.aspx", { NoticeIds: ary.join(",") }, function (res) {
                            $(".myGrid:last").getFlexi().populate();
                            if (res.msg) return alert(res.msg);
                        });
                    }
                });
            };

            jv.page().flexiEdit = function (id) {

                Boxy.load("~/Admin/NoticeInfo/Update/" + id + ".aspx", { filter: ".Main", modal: true, title: "修改" }, function (bxy) {
                    

                    //Edit页执行保存代码
                });
            };

            jv.page().flexiDel = function (ids, grid) {
                //删除。
                $.post("~/Admin/NoticeInfo/HotDelete.aspx", { query: ids.join(",") }, function (res) {
                    if (!res.msg) alert("删除成功 !");
                    else { alert(res.msg); }
                });
            };

            myGrid.flexigrid({
                title: "展示商品",
                url: "",
                colModel: [
                     { display: "查 看",  bind: false, name: "View", width: 30,  align: "center", html: '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID());">查 看</a>' },
                     { display: "ProductID",  hide: true, name: "ProductID", width: 80, sortable: true, align: "center" },
                //                     { display: "英文名", name: "Name", width: 80, sortable: true, align: "center" },
                //                     { display: "英文描述", name: "Descr", width: 80, sortable: true, align: "center" },
                     { display: "名称", name: "Name", width: 80, sortable: true, align: "center" },
                     { display: "描述", name: "Descr", width: 80, sortable: true, align: "center" },
                     { display: "类别", name: "ProductType", width: 80, sortable: true, align: "center" },
                     { display: "排序", name: "SortID", width: 80, sortable: true, align: "center" },
                     { display: "修 改",  bind: false, name: "Edit", width: 30,  align: "center", html: '<a style="cursor:pointer" onclick="jv.page().flexiEdit($(this).getFlexiRowID());">修 改</a>' }
                ],
                dataType: "json",
                buttons: [
                //                     { separator: true },
                //                     { name: "查询", bclass: "query", onpress: jv.page().flexiQuery },
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
            myGrid.getFlexi().p.url = "~/Admin/NoticeInfo/HotQuery/" + (jv.page()["uid"] || 0) + ".aspx";
            myGrid.getFlexi().populate();
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table class="divQuery" style="width: 100%;display:none">
        <tr>
            <td class="key"></td>
            <td class="val BtnQuery">
                <input class="submit" type="button" value="查询" onclick="jv.page(event).flexiQuery();" />
            </td>
        </tr>
    </table>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
