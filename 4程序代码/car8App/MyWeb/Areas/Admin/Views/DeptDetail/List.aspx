<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    DeptDetail列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
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
                

                Boxy.load("~/Admin/DeptDetail/Detail/" + id + ".aspx", { filter: ".Main", modal: true, title: "查看详细" }, function (bxy) {
                    

                    $(".Main").find(".submit").one("click", function () {
                        Boxy.get(".Main").hide();
                    });
                });
            };

            jv.page().flexiAdd = function () {
                
                Boxy.load("~/Admin/DeptDetail/Add" + ".aspx", { filter: ".Main", modal: true, title: "添加" }, function (bxy) {
                    
                });
            };

            jv.page().flexiEdit = function (id) {
                
                Boxy.load("~/Admin/DeptDetail/Update/" + id + ".aspx", { filter: ".Main", modal: true, title: "修改" }, function (bxy) {
                    

                    //Edit页执行保存代码
                });
            };

            jv.page().flexiDel = function (ids, grid) {
                //删除。
                $.post("~/Admin/DeptDetail/Delete.aspx", { query: ids.join(",") }, function (res) {
                    if (!res.msg) alert("删除成功 !");
                    else { alert(res.msg); }
                });
            };

            myGrid.flexigrid({
                title: "DeptDetail列表标题",
                url: "",
                colModel: [
                     { display: "查 看",  bind: false, name: "View", width: 30,   html: '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID());">查 看</a>' },
                     { display: "ID", name: "Id", width: 80, sortable: true },
                     { display: "分组", name: "GroupKey", width: 80, sortable: true },
                     { display: "键", name: "Key", width: 80, sortable: true },
                     { display: "值", name: "Value", width: 80, sortable: true },
                     { display: "特征", name: "Trait", width: 80, sortable: true },
                     { display: "排序", name: "SortID", width: 80, sortable: true },
                     { display: "操 作",  bind: false, name: "Edit", width: 30,   html: '<a style="cursor:pointer" onclick="jv.page().flexiEdit($(this).getFlexiRowID());">修 改</a>' }
                     ],
                dataType: "json",
                buttons: [
                     { separator: true },
                     { name: "查询", bclass: "query", onpress: jv.page().flexiQuery },
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

            myGrid.getFlexi().p.url = "~/Admin/DeptDetail/Query" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
            myGrid.getFlexi().populate();
        }); 

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divQuery" style="display: none">
        <div class="input">
            查询条件一:<input type="text" id="List_Code" style="width: 120px;" />
            查询条件二:<input type="text" id="List_Id" style="width: 120px;" /><br />
            查询条件三:<input type="text" id="List_Name" style="width: 120px;" />
            查询条件四:<input type="text" id="List_OtherName" style="width: 120px;" /><br />
        </div>
        <br />
        <input class="submit" type="button" value="查询" />
    </div>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
