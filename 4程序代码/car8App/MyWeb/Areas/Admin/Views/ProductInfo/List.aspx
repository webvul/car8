<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    产品列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());

            jv.page().flexiQuery = function () {
                var grid = myGrid.getFlexi();
                var ids = grid.getSelectIds();
                var queryJDiv = $(".divQuery:last", jv.boxdy());
                var searchOption = queryJDiv.GetDivJson("List_");
                myGrid.getFlexi().doSearch(searchOption);
            };

            jv.page().flexiView = function (id) {
                var isType = id[0] == "t";
                if (isType) {
                    id = id.slice(1);
                    url = "~/Admin/ProductType/Detail/" + id + ".aspx";
                }
                else {
                    url = "~/Admin/ProductInfo/Detail/" + id + ".aspx";
                }

                Boxy.load(url, { filter: ".Main", modal: true, title: isType ? "查看产品类别" : "查看产品信息", width: 9999, height: 9999 });
            };

            jv.page().flexiAdd = function () {
                jv.goto("~/Admin/ProductInfo/Add.aspx");
            };

            jv.page().flexiAddType = function () {

                Boxy.load("~/Admin/ProductType/Add" + ".aspx", { filter: ".Main", modal: true, title: "添加类别" }, function (bxy) {

                });
            };

            jv.page().flexiEdit = function (id) {
                var isType = id[0] == "t";

                if (isType) {
                    id = id.slice(1);
                    url = "~/Admin/ProductType/Update/" + id + ".aspx";
                }
                else {
                    url = "~/Admin/ProductInfo/Update/" + id + ".aspx";
                }

                Boxy.load(url, { filter: ".Main", modal: true, title: isType ? "修改类别信息" : "修改商品信息" }, function (bxy) {


                    //Edit页执行保存代码
                });
            };

            jv.page().flexiDel = function () {
                var grid = myGrid.getFlexi();
                var ids = grid.getSelectIds();

                if (ids.length == 0) return;
                if (confirm("确认删除这 " + ids.length + " 项吗？") == false) return;

                //删除。
                $.post("~/Admin/ProductInfo/Delete.aspx", { query: ids.join(",") }, function (res) {
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
                     { display: "类别", name: "ProductType.Name", sortable: true, align: "center" },
                     { display: "点击次数", name: "Clicks", sortable: true, align: "center" },
                     { display: "上次更新时间", name: "UpdateTime", sortable: true, align: "center" },
                     { display: "排序", name: "SortID", sortable: true, width: 80 },
                     { display: "修 改", bind: false, name: "Edit", align: "center", width: 80, html: '<a style="cursor:pointer" onclick="jv.page().flexiEdit($(this).getFlexiRowID(),event);">修 改</a>' }
                ],
                dataType: "json",
                buttons: [
                     //{ separator: true },
                     //{ name: "查询", bclass: "query", onpress: jv.page().flexiQuery },
                     //{
                     //    name: "操作", bclass: "dot", buttons: [
                     //     {
                     //         name: "更新到企业首页", bclass: "dot", onpress: function (ids, grid) {
                     //             var i = 0;
                     //         }
                     //     },
                     //     { separator: true },
                     //     { name: "推荐到网站首页", bclass: "dot", onpress: jv.page().flexiQuery }
                     //    ]
                     //},
                     //{ separator: true },
                     //{ name: '添加商品', bclass: 'add', onpress: jv.page().flexiAdd },
                     ////{ name: '添加类别', bclass: 'add', onpress: jv.page().flexiAddType },
                     //{ separator: true },
                     //{ name: "删除", bclass: "delete", onpress: function (ids, grid) {  },
                     //{ separator: true }
                ],
                usepager: true,
                useRp: true,
                rp: 15,
                showTableToggleBtn: true,
                onSuccess: function (grid) { jv.TestRole(); }
            });
            myGrid.getFlexi().p.url = "~/Admin/ProductInfo/Query" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
            myGrid.getFlexi().populate();

            $("#List_ProductTypeID").TextHelper({
                post: "click", url: "~/Admin/ProductInfo/GetProductTypes.aspx", datatype: "json", callback: function (res) {
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
        <input type="button" value="添加商品" onclick="jv.page().flexiAdd();" />
        <input type="button" value="删除" title="删除列表所选择的商品" onclick="jv.page().flexiDel();" />

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
                <input type="text" id="List_ProductTypeID" />
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
