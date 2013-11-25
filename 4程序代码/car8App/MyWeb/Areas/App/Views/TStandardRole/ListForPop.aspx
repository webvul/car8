<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/App/Views/Shared/App.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    标准角色列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());

            jv.page().flexiQuery = function () {
                var queryJDiv = $(".divQuery:last", jv.boxdy());
                myGrid.getFlexi().doSearch(queryJDiv.GetDivJson("List_"));
            };

            jv.page().flexiView = function (id) {
                Boxy.load("~/pm/Master/TStandardRole/Detail/" + id + ".aspx", { filter: ".divEdit:last", modal: true, title: "查看详细" }, function (bxy) {
                    bxy.resize(701);
                });
            };

            jv.page().flexiAdd = function () {
                Boxy.load("~/pm/Master/TStandardRole/Add" + ".aspx", { filter: ".divEdit:last", modal: true, title: "添加" }, function (bxy) {
                    bxy.resize(701);
                });
            };

            jv.page().flexiEdit = function (id) {
                Boxy.load("~/pm/Master/TStandardRole/Update/" + id + ".aspx", { filter: ".divEdit:last", modal: true, title: "修改" }, function (bxy) {
                    bxy.resize(701);

                    //Card页执行保存代码
                });
            };

            jv.page().flexiDel = function (ids) {
                //删除。
                $.post("~/pm/Master/TStandardRole/Delete" + ".aspx", { query: ids.join(",") }, function (res) {
                    if (res.msg) {
                        alert(res.msg);
                    }
                    else {
                        myGrid.getFlexi().populate();
                        alert("删除成功 !");
                    }
                });
            };

            //权限设置,udi
            jv.page().flexiPower = function (id) {
                var btnID = ".divEdit:last";
                Boxy.load("~/Admin/Power/Update.aspx?Type=TStandardRole&Value=" + id, { filter: btnID, modal: true, title: "修改" }, function (bxy) {
                    bxy.resize(880, 2000);

                    //Edit页执行保存代码
                }
                );
            };
            myGrid.flexigrid({
                title: "标准角色",
                url: "",
                role: { id: "StandardRoleId", name: "Name" },  //id : 在被引用的时候,返回的Id值 ; name :在被引用的时候,返回的显示值.
                colModel: [
                //{ display: "查 看", toggle: false, bind: false, name: "View", width: "auto", sortable: false, align: "left", html: '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID(),event);">查 看</a>' },
                  {display: "StandardRoleId", name: "StandardRoleId", width: "0", sortable: true, align: "left" },
                  { display: "编码", name: "Code", width: "auto", sortable: true, align: "left" },
                  { display: "名称", name: "Name", width: "auto", sortable: true, align: "left" },
                //{ display: "Type", name: "Type", width: "auto", sortable: true, align: "left" },
                //{ display: "ParentId", name: "ParentId", width: "auto", sortable: true, align: "left" },
                //{ display: "StandardOrganizationId", name: "StandardOrganizationId", width: "auto", sortable: true, align: "left" },
                //{ display: "OrderId", name: "OrderId", width: "auto", sortable: true, align: "left" },
                //{ display: "Status", name: "Status", width: "auto", sortable: true, align: "left" },
                  {display: "备注", name: "Remark", width: "auto", sortable: false, align: "left" },
                  { display: "更新时间", name: "UpdateDate", width: "auto", sortable: true, align: "left" },
                //{ display: "权限设置", toggle: false, bind: false, name: "Edit", width: "auto", sortable: false, align: "left", html: '<a style="cursor:pointer" onclick="jv.page().flexiPower($(this).getFlexiRowID(),event);">权限设置</a>' }
					],
                dataType: "json",
                resizable: true,
                buttons: [
//                { separator: true },
//                { name: "查询", bclass: "query", onpress: jv.page().flexiQuery },
                //{ separator: true },
                //{ name: '添加', bclass: 'add', onpress: jv.page().flexiAdd },
                //{ separator: true },
                //{ name: "删除", bclass: "delete", onpress: function (ids) { if (ids.length == 0) return; if (Confirm("确认删除这 " + ids.length + " 项吗？") == false) return; if (jv.page().flexiDel(ids) == false) return; } },
                { separator: true }
					],
                usepager: true,
                take:15,
                showTableToggleBtn: true,
                onSuccess: function (grid) { jv.TestRole(); }
            });

            myGrid.getFlexi().p.url = "~/pm/Master/TStandardRole/Query" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
            myGrid.getFlexi().populate();
        }); 
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divQuery">
        <div class="input">
            编&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;码:<input type="text" id="List_Code" style="width: 120px;" />
            &nbsp;&nbsp;&nbsp;&nbsp;
            名&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;称:<input type="text" id="List_Name" style="width: 120px;" />
        &nbsp;&nbsp;&nbsp;&nbsp;<input class="submit" type="button" value="查询" onclick="jv.page().flexiQuery()" />
        </div>

    </div>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
