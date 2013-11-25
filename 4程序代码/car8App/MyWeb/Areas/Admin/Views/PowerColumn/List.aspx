<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    列元数据列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());

            jv.page().flexiQuery = function () {
                //                var queryJDiv = $(".divQuery:last", jv.boxdy());
                //                myGrid.getFlexi().doSearch(queryJDiv.GetDivJson("List_"));
            };



            jv.page().flexiDetail = function (id) {
                Boxy.load("~/Admin/PowerColumn/Detail/" + id + ".aspx",
                    { filter: ".Main:last", modal: true, title: "查看详细", width: 701 },
                    function (bxy) { }
                );
            };

            jv.page().flexiAdd = function () {
                Boxy.load("~/Admin/PowerColumn/Add" + ".aspx",
                    { filter: ".Main:last", modal: true, title: "添加", width: 701 },
                    function (bxy) { }
                );
            };

            jv.page().flexiUpdate = function (id) {
                Boxy.load("~/Admin/PowerColumn/Update/" + id + ".aspx",
                    { filter: ".Main:last", modal: true, title: "修改", width: 701 },
                    function (bxy) { }
                );
            };

            jv.page().flexiDel = function (ids) {
                //删除。
                $.post("~/Admin/PowerColumn/Delete" + ".aspx", { query: ids.join(",") }, function (res) {
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
                title: "列元数据列表标题",
                url: "",
                role: { id: "Id", name: "Id", group: "Table", group: "Table" },  //id : 在被引用的时候,返回的Id值 ; name :在被引用的时候,返回的显示值.
                useId: true,
                colModel: [
                    { display: "Id", name: "Id", width: "0", align: "center" },
					{ display: "查看",  bind: false, name: "View", width: "auto",  align: "center", html:
                        '<a style="cursor:pointer" onclick="jv.page().flexiDetail($(this).getFlexiRowID(),event);">查 看</a>'
					},
                    { display: "表名", name: "Table", sortable: true,  width: "auto", align: "center" },
                    { display: "列名", name: "Column", sortable: true,width: "auto", align: "center" },
                    { display: "描述", name: "Descr", width: "auto", align: "center",format:"$#$" },
					{ display: "修改",  bind: false, name: "Edit", width: "auto",  align: "center", html:
                        '<a style="cursor:pointer" onclick="jv.page().flexiUpdate($(this).getFlexiRowID(),event);">修 改</a>'
					}
					],
                dataType: "json",
                buttons: [
					{ separator: true },
					{ name: '添加', bclass: 'add', onpress: jv.page().flexiAdd },
					{ separator: true },
					{ name: "删除", bclass: "delete", onpress: function (ids) { if (ids.length == 0) return; if (Confirm("确认删除这 " + ids.length + " 项吗？")) { if (jv.page(jv.boxdy().last()).flexiDel(ids, grid) == false) return false; } } },
					{ separator: true }
					],
                usepager: true,
                rp: 15,
                showTableToggleBtn: true,
                onSuccess: function (grid) { jv.TestRole(); }
            });

            myGrid.getFlexi().p.url = "~/Admin/PowerColumn/Query" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
            myGrid.getFlexi().populate();
        }); 
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table style="width: 100%">
        <tr>
            <td class="key">
                表名称:
            </td>
            <td class="val">
                <input id="List_Table" />
            </td>
            <td class="key">
                列名称:
            </td>
            <td class="val">
                <input id="List_Column" />
            </td>
            <td class="key">
                列描述:
            </td>
            <td class="val">
                <input id="List_Descr" />
            </td>
            <td class="key">
            </td>
            <td class="val BtnQuery">
                <input class="submit" type="button" value="查询" onclick="jv.page(event).flexiQuery()" />
            </td>
        </tr>
    </table>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
