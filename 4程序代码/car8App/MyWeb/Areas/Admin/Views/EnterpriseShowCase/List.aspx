<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    EnterpriseShowCase列表
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
                

                Boxy.load("~/Admin/EnterpriseShowCase/Detail/" + id + ".aspx", { filter: ".Main", modal: true, title: "查看详细" }, function (bxy) {
                    

                    $(".Main").find(".submit").one("click", function () {
                        Boxy.get(".Main").hide();
                    });
                });
            };

            jv.page().flexiAdd = function () {
                
                Boxy.load("~/Admin/EnterpriseShowCase/Add" + ".aspx", { filter: ".Main", modal: true, title: "添加" }, function (bxy) {
                    
                });
            };

            jv.page().flexiEdit = function (id) {
                
                Boxy.load("~/Admin/EnterpriseShowCase/Update/" + id + ".aspx", { filter: ".Main", modal: true, title: "修改" }, function (bxy) {
                    

                    //Edit页执行保存代码
                });
            };

            jv.page().flexiDel = function (ids, grid) {
                //删除。
                $.post("~/Admin/EnterpriseShowCase/Delete.aspx", { query: ids.join(",") }, function (res) {
                    if (!res.msg) alert("删除成功 !");
                    else { alert(res.msg); }
                });
            };

            myGrid.flexigrid({
                title: "EnterpriseShowCase列表标题",
                url: "",
                colModel: [
                     { display: "查 看",  bind: false, name: "View", width: 30,  align: "center", html: '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID());">查 看</a>' },
                     { display: "ProductID", name: "ProductID", width: 80, sortable: true, align: "center", hide: true },
                     { display: "SortID", name: "SortID", width: 80, sortable: true, align: "center" },
                     { display: "BeginTime", name: "BeginTime", width: 80, sortable: true, align: "center" },
                     { display: "EndTime", name: "EndTime", width: 80, sortable: true, align: "center" },
                     { display: "DeptID", name: "DeptID", width: 80, sortable: true, align: "center", hide: true },
                     { display: "修 改",  bind: false, name: "Edit", width: 30,  align: "center", html: '<a style="cursor:pointer" onclick="jv.page().flexiEdit($(this).getFlexiRowID());">修 改</a>' }
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
            myGrid.getFlexi().p.url = "~/Admin/EnterpriseShowCase/Query" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
            myGrid.getFlexi().populate()
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
                    <input type="text" id="List_Code"  />
                </td>
                <td class="key">
                    查询条件二:
                </td>
                <td class="val">
                    <input type="text" id="List_Id"  />
                </td>
                <td class="key">
                    查询条件三:
                </td>
                <td class="val">
                    <input type="text" id="List_Name"  />
                </td>
                <td class="key">
                    查询条件四:
                </td>
                <td class="val">
                    <input type="text" id="List_OtherName"  />
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
