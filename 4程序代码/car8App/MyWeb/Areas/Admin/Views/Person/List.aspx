<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    人员列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());

            jv.page().flexiQuery = function () {

                var filters = $(".divQuery").GetDivJson("List_");
                myGrid.getFlexi().doSearch(filters);

            };

            jv.page().flexiView = function (id) {


                Boxy.load("~/Admin/Person/Detail/" + id + ".aspx", { filter: ".Main", modal: true, title: "查看详细" }, function (bxy) {
                    

                    $(".Main").find(".submit").one("click", function () {
                        Boxy.get(".Main").hide();
                    });
                });
            };

            jv.page().flexiAdd = function () {

                Boxy.load("~/Admin/Person/Add" + ".aspx", { filter: ".Main", modal: true, title: "添加" }, function (bxy) {
                    
                }
                );
            };
            jv.page().flexiEdit = function (id) {

                Boxy.load("~/Admin/Person/Update/" + id + ".aspx", { filter: ".Main", modal: true, title: "修改", width: 701 });
            };

            jv.page().flexiEdit_Power = function (id) {
                jv.Pop("~/Admin/Power/Update.aspx?Type=Dept&" + "Value=" + id);
            };

            jv.page().flexiEdit_Password = function (id) {
                //删除。
                var box = Boxy.ask("新密码：<input type='text' />", ["确定", "取消"], function (con, box) {
                    if (con == "确定") {
                        $.post("~/Admin/Person/EditPassword.aspx",
                        { UserID: box.userid, Password: $(":text:first", $(box.boxy)).val() },
                         function (res) { if (res.msg) alert(res.msg); else alert("更新成功"); });
                    }
                }, { title: "新密码" });
                box.userid = id;
            };

            jv.page().flexiEdit_Role = function (id) {

                jv.PopList({
                    entity: "Role", query: { UserID: id }, mode: "check", callback: function (idAry) {
                        $.post("~/Admin/Person/SaveRole.aspx?Type=Edit&UserID=" + id, { Role: idAry.join(",") }, function (res) {
                            if (res.msg) alert(res.msg);
                            else {
                            }
                        });
                    }
                });

            };

            jv.page().flexiDelConfirm = function () {
                var grid = $(".flexigrid").getFlexi();
                var ids = grid.getSelectIds();
                if (ids.length == 0) return;
                if (Confirm("确认删除这 " + ids.length + " 项吗？")) {
                    if (jv.page(jv.boxdy().last()).flexiDel(ids, grid) == false)
                        return false;
                }
            }

            jv.page().flexiDel = function (ids, grid) {
                //删除。
                $.post("~/Admin/Person/Delete.aspx", { query: ids.join(",") }, function (res) {
                    if (res.msg) { alert(res.msg); }
                    else {
                        myGrid.getFlexi().populate();
                        alert("删除成功 !");
                    }
                });
            };

            myGrid.flexigrid({
                title: "",
                url: "",
                useId: true,
                role: { id: "UserID", name: "Name" },
                colModel: [
                     { display: "用户ID", name: "UserID", width: 80, sortable: true, align: "center" },
                     { display: "名字", name: "Name", width: 'auto', sortable: true, align: "center" },
                     { display: "商户名称", name: "Dept", width: 'auto', sortable: true, align: "center" },
                     { display: "邮件", name: "Email", width: 'auto', sortable: true },
                     { display: "手机", name: "Mobile", width: 'auto', sortable: true, align: "right" },
                //{ display: "排序", name: "SortID", width: 30, sortable: true, align: "center" },
                     {
                         display: "修 改", bind: false, name: "Edit", width: "auto", align: "center", html:
                           '<a style="cursor:pointer" onclick="jv.page().flexiEdit($(this).getFlexiRowID(),event);">修 改</a> '
                     },
                     {
                         display: "查 看", bind: false, name: "View", width: "auto", align: "center", html:
                          '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID(),event);">查 看</a>'
                     },
                     {
                         display: "设置密码", bind: false, name: "设置密码", width: "auto", align: "center", html:
                        '<a style="cursor:pointer" onclick="jv.page().flexiEdit_Password($(this).getFlexiRowID(),event);">设置密码</a>'
                     }
                ],
                dataType: "json",
                buttons: [
                ],
                usepager: true,
                useRp: true,
                take: 50,
                showTableToggleBtn: true
            });
            myGrid.getFlexi().p.url = "~/Admin/Person/Query" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
            myGrid.getFlexi().populate();
        });

    </script>
    <style type="text/css">
        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table width="100%" class="divQuery">
        <tr>
            <td class="key">用户ID:
            </td>
            <td class="val">
                <input type="text" id="List_UserID" />
            </td>
            <td class="key">名字:
            </td>
            <td class="val ">
                <input type="text" id="List_Name" />
            </td>
            <td class="key">手机：
            </td>
            <td class="val">
                <input type="text" id="List_Mobile" />
            </td>
            <td class="key"></td>
            <td class="val query_btn"></td>
        </tr>
    </table>
    <div class="MyTool">
        <input class="submit" type="button" value="查询" onclick="jv.page().flexiQuery()" />
<%--        <input type="button" value="添加" onclick="jv.page().flexiAdd();" />
        <input type="button" value="删除" onclick="jv.page().flexiDelConfirm();" />--%>
    </div>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
