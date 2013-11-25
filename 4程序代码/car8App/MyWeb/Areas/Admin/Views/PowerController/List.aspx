<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    模块元数据列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());

            jv.page().flexiQuery = function (ids, grid) {
                var queryJDiv = $(".divQuery:last", jv.boxdy());
                myGrid.getFlexi().doSearch(queryJDiv.GetDivJson("List_"));
            };


            jv.page().flexiView = function (id) {
                

                Boxy.load("~/Admin/PowerController/Detail/" + id + ".aspx", { filter: ".Main", modal: true, title: "查看详细" }, function (bxy) {
                    

                    $(".Main").find(".submit").one("click", function () {
                        Boxy.get(".Main").hide();
                    });
                });
            };

            jv.page().flexiAdd = function () {
                
                Boxy.load("~/Admin/PowerController/Add" + ".aspx", { filter: ".Main", modal: true, title: "添加" }, function (bxy) {
                    
                });
            };

            jv.page().flexiImport = function (ids, grid) {
                $.post("~/Admin/PowerController/ImportOql/Admin.aspx", {}, function (res) {
                    if (!res.msg) {
                        grid.populate();
                    }
                    else {
                    }
                });
            };

            jv.page().flexiEdit = function (id) {
                
                Boxy.load("~/Admin/PowerController/Update/" + id + ".aspx", { filter: ".Main", modal: true, title: "修改" }, function (bxy) {
                    

                    //Edit页执行保存代码
                });
            };

            jv.page().flexiDel = function (ids, grid) {
                //删除。
                $.post("~/Admin/PowerController/Delete" + ".aspx", { query: ids.join(",") }, function (res) {
                    if (!res.msg) alert("删除成功 !");
                    else { alert(res.msg); }
                });
            };

            jv.page().adds = [];
            jv.page().minus = [];


            jv.page().flexiSave = function (ids, grid) {
                $.post("~/Admin/PowerController/PowerSave" + ".aspx" + jv.url().search, { Adds: jv.page().adds.join(","), Minus: jv.page().minus.join(",") }, function (res) {
                    if (res.msg) {
                        alert(res.msg);
                    }

                    alert("保存成功.");
                });

            };

            var btns;

            if (jv.page().action == "ListPower") {
                btns = [{ separator: true },
					{ name: '保存', bclass: 'save pin', onpress: jv.page().flexiSave}];
            }
            else {
                btns = [
					 { separator: true },
					 { name: '添加', bclass: 'add', onpress: jv.page().flexiAdd },
					 { name: '导入所有MyOql模块', bclass: 'dot', onpress: jv.page().flexiImport },
					 { separator: true },
					 { name: "删除", bclass: "delete", onpress: function (ids, grid) { if (ids.length == 0) return; if (Confirm("确认删除这 " + ids.length + " 项吗？")) { if (jv.page().flexiDel(ids, grid) == false) return; grid.populate(); } } },
					 { separator: true }
					 ];
            }

            myGrid.flexigrid({
                title: "模块元数据列表",
                url: "",
                role: { name: "Descr", select: "Sel" },
                colModel: [
                    { display: "ID", name: "Id", width: 80, sortable: true, hide: true },
                    { display: "系统", name: "Area", width: 80, sortable: true },
                    { display: "模块", name: "Controller", width: 120, sortable: true },
                    { display: "模块描述", name: "Descr", width: "auto", sortable: true },
                    { display: "Sel", name: "Sel", width: 0,  css: "NoWrap" },
                    { display: "",  bind: false, name: "Edit",  align: "center", html: '<a style="cursor:pointer" onclick="jv.page().flexiEdit($(this).getFlexiRowID());">修 改</a>' },
                    { display: "",  bind: false, name: "View",  align: "center", html: '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID());">查 看</a>' }
                ],
                dataType: "json",
                buttons: btns,
                usepager: true,
                keepQuote: true,
                useRp: true,
                onSelected: function (tr, g) {
                    if (g.isSelected(tr)) {
                        jv.page().adds.push(tr.id.slice(3));
                    } else {
                        jv.page().minus.push(tr.id.slice(3));
                    }
                },
                rp: 15
            });

            myGrid.getFlexi().p.url = "~/Admin/PowerController/Query" + (jv.page().action == "ListPower" ? "/ListPower" : "") + ".aspx" + jv.url().search;
            myGrid.getFlexi().populate();
        }); 

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table class="divQuery" style="width: 100%">
        <tr>
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
                <input type="text" id="List_Descr" />
            </td>
            <td class="key">
            </td>
            <td class="val">
            </td>
            <td class="key">
            </td>
            <td class="val query_btn">
                <input class="submit" type="button" value="查询" onclick="jv.page(event).flexiQuery();" />
            </td>
        </tr>
    </table>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
