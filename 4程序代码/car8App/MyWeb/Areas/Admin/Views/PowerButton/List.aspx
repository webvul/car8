<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    按钮元数据列表
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
                Boxy.load("~/Admin/PowerButton/Detail/" + id + ".aspx", { filter: ".MyCard", modal: true, title: "查看详细" }, function (bxy) {
                    
                });
            };

            jv.page().flexiAdd = function () {
                Boxy.load("~/Admin/PowerButton/Add" + ".aspx", { filter: ".MyCard", modal: true, title: "添加" }, function (bxy) {
                    
                });
            };

            jv.page().flexiEdit = function (id) {
                Boxy.load("~/Admin/PowerButton/Update/" + id + ".aspx", { filter: ".MyCard", modal: true, title: "修改" }, function (bxy) {
                    

                    //Card页执行保存代码
                });
            };

            jv.page().flexiDel = function (ids) {
                //删除。
                $.post("~/Admin/PowerButton/Delete" + ".aspx", { query: ids.join(",") }, function (res) {
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
                title: "按钮元数据列表",
                url: "",
                useId: true,
                role: { id: "Id", name: "Descr", group: "Area,Controller,PowerController.Descr,Action,PowerAction.Descr" },  //id : 在被引用的时候,返回的Id值 ; name :在被引用的时候,返回的显示值.
                colModel: [
					{ display: "查看",  bind: false, name: "View", width: "auto",   html: '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID(),event);">查 看</a>' },
                    { display: "Id", name: "Id", width: "0", sortable: true },
                    { display: "子系统", name: "Area", width: "auto",  align: "left" },
                    { display: "模块", name: "Controller", width: "auto",  align: "left" },
                    { display: "模块描述", name: "PowerController.Descr", width: "auto",  align: "left" },
                    { display: "页面", name: "Action", width: "auto",  align: "left" },
                    { display: "页面描述", name: "PowerAction.Descr", width: "auto",  align: "left" },
                    { display: "按钮", name: "Button", width: "auto", sortable: true },
                    { display: "按钮描述", name: "Descr", width: "auto", sortable: true },
					{ display: "修改",  bind: false, name: "Edit", width: "auto",   html: '<a style="cursor:pointer" onclick="jv.page().flexiEdit($(this).getFlexiRowID(),event);">修 改</a>' }
					],
                dataType: "json",
                buttons: [
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

            myGrid.getFlexi().p.url = "~/Admin/PowerButton/Query" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
            myGrid.getFlexi().populate();
        }); 
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table class="divQuery" style="width: 100%">
        <tr>
            <td class="key">
                系统:
            </td>
            <td class="val">
                <input type="text" id="List_Area" />
            </td>
            <td class="key">
                模块名称:
            </td>
            <td class="val">
                <input type="text" id="List_Controller" />
            </td>
            <td class="key">
                模块描述:
            </td>
            <td class="val">
                <input type="text" id="List_ControllerDescr" />
            </td>
            <td class="key">
                页面:
            </td>
            <td class="val">
                <input type="text" id="List_Action" />
            </td>
        </tr>
        <tr>
            <td class="key">
                页面描述:
            </td>
            <td class="val">
                <input type="text" id="List_ActionDescr" />
            </td>
            <td class="key">
                按钮:
            </td>
            <td class="val">
                <input type="text" id="List_Button" />
            </td>
            <td class="key">
                按钮描述:
            </td>
            <td class="val">
                <input type="text" id="List_ButtonDescr" />
            </td>
            <td colspan="2" class="query_btn">
                <input class="submit" type="button" value="查询" onclick="jv.page(event).flexiQuery();" />
            </td>
        </tr>
    </table>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
