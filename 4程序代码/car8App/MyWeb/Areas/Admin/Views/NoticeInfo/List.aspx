<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    公告列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());

            jv.page().flexiQuery = function () {
                var grid = myGrid.getFlexi();
                var ids = grid.getSelectIds();
                var queryJDiv = $(".divQuery:last", jv.boxdy());
                myGrid.getFlexi().doSearch(queryJDiv.GetDivJson("List_"));
            };

            jv.page().flexiView = function (id) {
                var isType = id[0] == "t";
                if (isType) {
                    id = id.slice(1);
                    url = "~/Admin/NoticeType/Detail/" + id + ".aspx";
                }
                else {
                    url = "~/Admin/NoticeInfo/Detail/" + id + ".aspx";
                }

                Boxy.load(url, { filter: ".Main", modal: true, title: isType ? "查看公告类别" : "查看公告信息" }, function (bxy) {

                    $(".Main").find(".submit").one("click", function () {
                        Boxy.get(".Main").hide();
                    });
                });
            };

            jv.page().flexiAdd = function () {
                Boxy.load("~/Admin/NoticeInfo/Add" + ".aspx", { filter: ".Main", modal: true, title: "添加", width: 900 });
            };

            jv.page().flexiAddType = function () {

                Boxy.load("~/Admin/NoticeType/Add" + ".aspx", { filter: ".Main", modal: true, title: "添加类别" }, function (bxy) {

                });
            };

            jv.page().flexiEdit = function (id) {
                var isType = id[0] == "t";

                if (isType) {
                    id = id.slice(1);
                    url = "~/Admin/NoticeType/Update/" + id + ".aspx";
                }
                else {
                    url = "~/Admin/NoticeInfo/Update/" + id + ".aspx";
                }

                Boxy.load(url, { filter: ".Main", modal: true, title: isType ? "修改类别信息" : "修改公告信息", width: 900 }, function (bxy) {


                    //Edit页执行保存代码
                });
            };

            jv.page().flexiDel = function () {
                var grid = myGrid.getFlexi();
                var ids = grid.getSelectIds();

                if (ids.length == 0) return;
                if (confirm("确认删除这 " + ids.length + " 项吗？") == false) return;

                //删除。
                $.post("~/Admin/NoticeInfo/Delete.aspx", { query: ids.join(",") }, function (res) {
                    grid.populate();
                    if (!res.msg) alert("删除成功 !");
                    else { alert(res.msg); }
                });
            };

            myGrid.flexigrid({
                title: "",
                url: "",
                role: { name: "Name" },
                colModel: [
                     { display: "查 看", bind: false, name: "View", width: 80, html: '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID(),event);">查 看</a>' },
                     { display: "ID", toggle: true, hide: false, name: "Id", width: 0, align: "center" },
                     { display: "名称", name: "Name", sortable: true },
                     { display: "描述", name: "Descr", sortable: true, align: "center" },
                     { display: "类别", name: "NoticeType.Name", sortable: true, align: "center" },
                     { display: "上次更新时间", name: "UpdateTime", sortable: true, align: "center" },
                     { display: "排序", name: "SortID", sortable: true, width: 80 },
                     { display: "修 改", bind: false, name: "Edit", align: "center", width: 80, html: '<a style="cursor:pointer" onclick="jv.page().flexiEdit($(this).getFlexiRowID(),event);">修 改</a>' }
                ],
                dataType: "json",
                usepager: true,
                useRp: true,
                rp: 15,
                showTableToggleBtn: true,
                onSuccess: function (grid) { jv.TestRole(); }
            });
            myGrid.getFlexi().p.url = "~/Admin/NoticeInfo/Query" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
            myGrid.getFlexi().populate();

            $("#List_NoticeTypeID").TextHelper({
                post: "click", url: "~/Admin/NoticeInfo/GetNoticeTypes.aspx", datatype: "json", callback: function (res) {
                    if (res.msg) { return alert(res.msg); }
                    else {
                        var data = {};
                        $(res.data).each(function (i, d) {
                            data[d.Id] = d.Name;
                        });
                        return data;
                    }
                }
            });


        });

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="MyTool">
        <input type="button" value="查询" onclick="jv.page().flexiQuery();" />
        <input type="button" value="添加公告" onclick="jv.page().flexiAdd();" />
        <input type="button" value="删除" title="删除列表所选择的公告" onclick="jv.page().flexiDel();" />

    </div>
    <table class="divQuery" style="width: 100%">
        <tr>
            <td class="key">名称:
            </td>
            <td class="val">
                <input type="text" id="List_Name" />
            </td>
            <td class="key">类别:
            </td>
            <td class="val">
                <input type="text" id="List_NoticeTypeID" />
            </td>
            <td class="key" style="background-color: transparent"></td>
            <td class="val" style="background-color: transparent"></td>
            <td class="key" style="background-color: transparent"></td>
            <td class="val" style="background-color: transparent"></td>
        </tr>
    </table>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
