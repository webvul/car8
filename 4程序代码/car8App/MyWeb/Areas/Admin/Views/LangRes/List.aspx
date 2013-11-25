<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    ResKey列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());

            jv.page().flexiQuery = function () {
                var queryJDiv = $(".divQuery:last", jv.boxdy());

                new Boxy(queryJDiv, { modal: true, title: "查询" }).resize(400, 100);
                queryJDiv.find(".submit").one("click", function () {
                    //查询。

                    myGrid.getFlexi().doSearch(queryJDiv.GetDivJson("List_"));
                    Boxy.get(queryJDiv).hide();
                });
            };

            jv.page().flexiView = function (id) {
                Boxy.load("~/Admin/ResKey/Detail/" + id + ".aspx", { filter: ".Main:last", modal: true, title: "查看详细" }, function (bxy) {
                    
                });
            };

            jv.page().flexiAdd = function () {
                Boxy.load("~/Admin/ResKey/Add" + ".aspx", { filter: ".Main:last", modal: true, title: "添加" }, function (bxy) {
                    
                });
            };

            jv.page().flexiEdit = function (id) {
                Boxy.load("~/Admin/ResKey/Update/" + id + ".aspx", { filter: ".Main:last", modal: true, title: "修改" }, function (bxy) {
                    

                    //Card页执行保存代码
                });
            };

            jv.page().flexiDel = function (ids) {
                //删除。
                $.post("~/Admin/ResKey/Delete" + ".aspx", { query: ids.join(",") }, function (res) {
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
                title: "语言资源",
                url: "",
                role: { id: "Id", name: "Key" },  //id : 在被引用的时候,返回的Id值 ; name :在被引用的时候,返回的显示值.
                colModel: [
					{ display: "查 看",  bind: false, name: "View", width: "auto",   html: '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID(),event);">查 看</a>' },
                    { display: "Id", name: "Id", width: "auto", sortable: true, hide: true },
                    { display: "Key", name: "Key", width: "auto", sortable: true },
					{ display: "操 作",  bind: false, name: "Edit", width: "auto",   html: '<a style="cursor:pointer" onclick="jv.page().flexiEdit($(this).getFlexiRowID(),event);">修 改</a>' }
					],
                dataType: "json",
                buttons: [
					{ separator: true },
					{ name: "查询", bclass: "query", onpress: jv.page().flexiQuery },
					{ separator: true },
					{ name: '添加', bclass: 'add', onpress: jv.page().flexiAdd },
					{ separator: true },
					{ name: "删除", bclass: "delete", onpress: function (ids) { if (ids.length == 0) return; if (Confirm("确认删除这 " + ids.length + " 项吗？")) { if (jv.page().flexiDel(ids) == false) return; } } },
					{ separator: true }
					],
                usepager: true,
                rp: 15,
                showTableToggleBtn: true,
                onSuccess: function (grid) { jv.TestRole(); }
            });

            myGrid.getFlexi().p.url = "~/Admin/ResKey/Query" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
            myGrid.getFlexi().populate();
        }); 
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divQuery" style="display: none">
           <table style="width: 100%">
            <tr>
                <td class="key">
                    查询条件一:
                </td>
                <td class="val">
                    <input type="text" id="List_Code" style="width: 120px;" />
                </td>
                <td class="key">
                    查询条件二:
                </td>
                <td class="val">
                    <input type="text" id="List_Id" style="width: 120px;" />
                </td>
                <td class="key">
                    查询条件三:
                </td>
                <td class="val">
                    <input type="text" id="List_Name" style="width: 120px;" />
                </td>
                <td class="key">
                    查询条件四:
                </td>
                <td class="val">
                    <input type="text" id="List_OtherName" style="width: 120px;" />
                </td>
            </tr>
            <tr>
                <td colspan="8" class="query_btn">
                           <input class="submit" type="button" value="查询" />
                </td>
            </tr>
        </table>
    </div>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
