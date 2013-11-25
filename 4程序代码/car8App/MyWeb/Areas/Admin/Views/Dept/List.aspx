<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    商户列表
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

                Boxy.load("~/Admin/Dept/Detail/" + id + ".aspx", { filter: ".Main", modal: true, title: "查看详细" }, function (bxy) {
                    
                });
            };

            jv.page().flexiAdd = function () {
                jv.goto("~/Admin/Dept/Add.aspx");
                //Boxy.load("~/Admin/Dept/Add.aspx", { filter: ".Main", modal: true, title: "添加" }, function (bxy) {
                //    
                //});
            };

            jv.page().flexiEdit = function (id) {

                Boxy.load("~/Admin/Dept/AdminUpdate/" + id + ".aspx", { filter: ".Main", modal: true, title: "修改",width: 900}, function (bxy) {
                    

                    //Edit页执行保存代码
                });
            };

  
            jv.page().flexiDel = function (ids, grid) {
                var grid = $(".flexigrid").getFlexi();
                var ids = grid.getSelectIds();

                if (ids.length == 0) return;
                if (Confirm("确认删除这 " + ids.length + " 项吗？") == false) return;
                //删除。
                $.post("~/Admin/Dept/Delete.aspx", { query: ids.join(",") }, function (res) {
                    if (!res.msg) {
                        grid.populate();
                        alert("删除成功 !");
                    }
                    else { alert(res.msg); }
                });
            };

            jv.page().flexiEdit_Power = function (rowData, rowIndex, grid, jdoer) {

                jv.Pop("~/Admin/Power/Update.aspx?Type=Dept&Value=" + rowData.cell[grid.getColModel("Id").indexOfBind]);
            };

            myGrid.flexigrid({
                title: "",
                url: "",
                colModel: [
                     { display: "查 看", bind: false, name: "View", width: 0, align: "center", html: '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID());">查 看</a>' },
                     { display: "ID", name: "Id", sortable: true, width: 80 },
                     { display: "名字", name: "Name", sortable: true },
                     { display: "WebName", name: "WebName", sortable: true },
                     { display: "地址", name: "Address", sortable: true },
                     { display: "联系人", name: "ContactPerson", sortable: true,width:0 },
                     { display: "联系人电话", name: "ContactMobile", sortable: true, width: 0 },
                     { display: "排序", name: "SortID", sortable: true, width: 50 },
                     { display: "添加时间", name: "AddTime", width: 120 },
                     //{ display: "结束时间", name: "EndTime", sortable: true },
                     //{ display: "设置权限",  bind: false, name: "Edit_Password", width: 50,   html: '<a style="cursor:pointer" onclick="jv.flexiRowEvent(jv.page().flexiEdit_Power,event);">设置权限</a>' },
                     { display: "操 作", bind: false, name: "Edit", width: 50, align: "center", html: '<a style="cursor:pointer" onclick="jv.page().flexiEdit($(this).getFlexiRowID());">修 改</a>' }
                ],
                dataType: "json",
                buttons: [
                ],
                usepager: true,
                useRp: true,
                rp: 15,
                resizable: false,
                showTableToggleBtn: true
            });

            myGrid.getFlexi().p.url = "~/Admin/Dept/Query" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
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
    <div class="MyTool">
        <input value="添加" type="button" onclick="jv.page().flexiAdd()" />
        <input value="删除" type="button" onclick="jv.page().flexiDel()" />
    </div>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
