<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/App/Views/Shared/App.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    标准角色列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/flowplayer/tabs.js" type="text/javascript"></script>
    <link href="~/Res/flowplayer/tabs.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());

            jv.page().flexiQuery = function () {
                var queryJDiv = $(".divQuery:last", jv.boxdy());

                myGrid.getFlexi().doSearch(queryJDiv.GetDivJson("List_"));
            };

            jv.page().flexiClean = function (ids) {
                var grid = myGrid.getFlexi();
                var ids = grid.getSelectIds();

                if (Confirm("数据清洗,是针对元数据删除产生的冗余数据清理.在删除元数据后，需要进行数据清洗。请谨慎.确认吗？") == false) return;
                $.post("~/Master/TStandardRole/Clean.aspx", { query: ids.join(",") }, function (res) {
                    if (res.msg) {
                        alert(res.msg);
                    }
                    else {
                        myGrid.getFlexi().populate();
                        alert("数据清洗成功 !");
                    }
                });
            };
            jv.page().flexiView = function (id) {
                Boxy.load("~/Master/TStandardRole/Detail/" + id + ".aspx", { filter: ".divEdit:last", modal: true, title: "查看详细" }, function (bxy) {
                    bxy.resize(701);
                });
            };

            jv.page().flexiAdd = function () {
                Boxy.load("~/Master/TStandardRole/Add" + ".aspx", { filter: ".divEdit:last", modal: true, title: "添加" }, function (bxy) {
                    bxy.resize(701);
                });
            };

            jv.page().flexiEdit = function (id) {
                Boxy.load("~/Master/TStandardRole/Update/" + id + ".aspx", { filter: ".divEdit:last", modal: true, title: "修改" }, function (bxy) {
                    bxy.resize(701);

                    //Card页执行保存代码
                });
            };

            jv.page().flexiDel = function () {
                var grid = myGrid.getFlexi();
                var ids = grid.getSelectIds();

                if (ids.length == 0) return;
                if (onfirm("确认删除这 " + ids.length + " 项吗？") == false) return;


                //删除。
                $.post("~/Master/TStandardRole/Delete" + ".aspx", { query: ids.join(",") }, function (res) {
                    if (res.msg) {
                        return alert(res.msg);
                    }
                    else {
                        myGrid.getFlexi().populate();
                        alert("删除成功 !");
                    }
                });
            };

            jv.page().flexiSetPower = function (ids) {
                var grid = myGrid.getFlexi();
                var ids = grid.getSelectIds();
                if (ids.length == 0) {
                    Alert("请选择 标准角色!");
                    return false;
                }
                if (Confirm("对多个标准角色设置权限，不会显示这些人共有权限，请谨慎操作！确认吗？") == false) return false;

                jv.Pop("~/Admin/Power/Update.aspx?Type=TStandardRole&Value=" + ids.join(",") + "&Mode=Add");
            };

            jv.page().flexiClearPower = function (ids) {
                var grid = myGrid.getFlexi();
                var ids = grid.getSelectIds();
                if (ids.length == 0) {
                    Alert("请选择 标准角色!");
                    return false;
                }
                if (Confirm("对多个标准角色设置权限，不会显示这些人共有权限，请谨慎操作！确认吗？") == false) return false;

                jv.Pop("~/Admin/Power/Update.aspx?Type=TStandardRole&Value=" + ids.join(",") + "&Mode=Minus");

            };

            jv.page().flexiPop = function (id) {
                jv.PopList({
                    list: 'PopForUser', area: 'Master', entity: 'TStandardRole', mode: 'none',
                    callback: function (role, data) {
                    }, uid: id, width: 400
                });
            };

            //权限设置,udi
            jv.page().flexiPower = function (id) {
                jv.PopDetail({ url: "~/Admin/Power/Update.aspx?Type=TStandardRole&Value=" + id, entity: "Power", data: { code: true } });
                //                var btnID = ".divEdit:last";
                //                Boxy.load("~/Admin/Power/Update.aspx?Type=TStandardRole&Value=" + id, { filter: btnID, modal: true, title: "修改", width: 880, height: 2000 }
                //                );
            };
            myGrid.flexigrid({
                title: "",
                url: "",
                useId: true,
                role: { id: "StandardRoleId", name: "Name" },  //id : 在被引用的时候,返回的Id值 ; name :在被引用的时候,返回的显示值.
                colModel: [
                //{ display: "查 看", toggle: false, bind: false, name: "View", width: "auto", sortable: false, align: "left", html: '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID(),event);">查 看</a>' },
                  { display: "StandardRoleId", name: "StandardRoleId", width: "0", sortable: true, align: "left" },
                  { display: "编码", name: "Code", width: "auto", sortable: true, align: "left" },
                  { display: "名称", name: "Name", width: "auto", sortable: true, align: "left", format: '<a style="cursor:pointer;color:blue;" onclick="jv.page().flexiPop($(this).getFlexiRowID(),event);">$#$</a>' },
                //{ display: "Type", name: "Type", width: "auto", sortable: true, align: "left" },
                //{ display: "ParentId", name: "ParentId", width: "auto", sortable: true, align: "left" },
                //{ display: "StandardOrganizationId", name: "StandardOrganizationId", width: "auto", sortable: true, align: "left" },
                //{ display: "OrderId", name: "OrderId", width: "auto", sortable: true, align: "left" },
                //{ display: "Status", name: "Status", width: "auto", sortable: true, align: "left" },
                  { display: "备注", name: "Remark", width: "auto", sortable: true, align: "left" },
                  { display: "更新时间", name: "UpdateDate", width: "auto", sortable: true, align: "left" },
                  { display: "操 作", toggle: false, bind: false, name: "Edit", width: "auto", sortable: false, align: "left", html: '<a style="cursor:pointer" onclick="jv.page().flexiPower($(this).getFlexiRowID(),event);">权限设置</a>' }
                ],
                dataType: "json",
                buttons: [
                ],
                FillHeight:true,
                usepager: true,
                take: 15,
                rpOptions: [5, 10, 15, 20, 25, 40, 800],
                showTableToggleBtn: true,
                onSuccess: function (grid) { jv.TestRole(); }
            });

            myGrid.getFlexi().p.url = "~/App/TStandardRole/Query" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
            myGrid.getFlexi().populate();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table class="divQuery" style="width: 100%">
        <tr>
            <td class="key">标准角色编码:
            </td>
            <td class="val">
                <input type="text" id="List_Code" />
            </td>
            <td class="key">标准角色名称:
            </td>
            <td class="val">
                <input type="text" id="List_Name" />
            </td>
            <%=
                dbr.Menu
                    .PopRadior("List_Menu",new UIPop{ area = "Admin", KeyTitle="菜单"} )
                    .GenTd()
            %>
            <td class="key"></td>
            <td class="val query_btn"></td>
        </tr>
    </table>
    <div class="MyTool">
        <input class="submit" type="button" value="查询" onclick="jv.page(event).flexiQuery()" />
        <input class="submit" type="button" value="删除" onclick="jv.page(event).flexiDel()" />
        <input class="submit" type="button" value="设置所选权限" onclick="jv.page(event).flexiSetPower()" />
        <input class="submit" type="button" value="清除所选权限" onclick="jv.page(event).flexiClearPower()" />
        <input class="submit" type="button" value="数据清洗" onclick="jv.page(event).flexiClean()" />
    </div>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
