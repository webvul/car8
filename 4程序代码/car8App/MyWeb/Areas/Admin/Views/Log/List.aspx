<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Log列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());
            $('#List_BeginTime').datepicker();
            $('#List_EndTime').datepicker();
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
                Boxy.load("~/Admin/Log/Detail/" + id + ".aspx", { filter: ".MyCard", modal: true, title: "查看详细" }, function (bxy) {
                    

                    $(".MyCard").find(".submit").one("click", function () {
                        Boxy.get(".Main").hide();
                    });
                });
            };

            jv.page().flexiAdd = function () {
                Boxy.load("~/Admin/Log/Add" + ".aspx", { filter: ".MyCard", modal: true, title: "添加" }, function (bxy) {
                    
                });
            };

            jv.page().flexiEdit = function (id) {
                Boxy.load("~/Admin/Log/Update/" + id + ".aspx", { filter: ".MyCard", modal: true, title: "修改" }, function (bxy) {
                    

                    //Edit页执行保存代码
                });
            };

            jv.page().flexiDel = function (ids, grid) {
                //删除。
                $.post("~/Admin/Log/Delete" + ".aspx", { query: ids.join(",") }, function (res) {
                    if (!res.msg) {
                        alert("删除成功 !");
                        myGrid.getFlexi().populate();
                    }
                    else { alert(res.msg); }
                });
            };

            myGrid.flexigrid({
                title: "日志列表",
                url: "",
                colModel: [
					{ display: "查 看",  bind: false, name: "View", width: 30,   html: '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID(),event);">查 看</a>' },
                    { display: "ID", name: "Id", width: 30, sortable: true },
                    { display: "类型", name: "Type", width: 50, sortable: true },
                    { display: "用户", name: "UserName", width: 120, sortable: true },
                    { display: "消息", name: "Msg", maxWidth: "600", sortable: true },
                    { display: "添加时间", name: "AddTime", width: 80, sortable: true }
					],
                dataType: "json",
                buttons: [
					{ separator: true },
					{ name: "查询", bclass: "query", onpress: jv.page().flexiQuery },
					{ separator: true },
					{ name: "删除", bclass: "delete", onpress: jv.page().flexiDel }
					],
                usepager: true,
                useRp: true,
                rp: 15,
                showTableToggleBtn: true
            });

            myGrid.getFlexi().p.url = "~/Admin/Log/Query" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
            myGrid.getFlexi().populate();
        }); 
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divQuery" style="display: none">
        <div class="input">
            用 户 名:<input type="text" id="List_UserName" style="width: 120px;" />
            类 型:<%
                    Html.RegisteEnum("List_Type", MyCmn.InfoEnum.Info)
                        .Input()
                        .Radior();
            %><br />
            开始时间:<input type="text" readonly="readonly" id="List_BeginTime" style="width: 120px;" />
            结束时间:<input type="text" readonly="readonly" id="List_EndTime" style="width: 120px;" />
        </div>
        <br />
        <input class="submit" type="button" value="查询" />
    </div>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
