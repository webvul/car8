<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Host/Views/Shared/Simple.Master"
    Inherits="MyCon.MyMvcPage<Dictionary<string,string>>" Theme="Admin" %>

<asp:Content ID="dd" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/Canvas/excanvas.js" type="text/javascript"></script>
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <style type="text/css">
        body
        {
            background-color: #f0f0f0;
        }
    </style>
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

            jv.page().flexiView = function (rowData, rowIndex, grid, jtd) {
                jv.page().PopCard(rowData, grid, "Detail");
            };

            jv.page().PopCard = function (rowData, grid, action) {
                var json = grid.getJson(rowData);
                if (json.Type == "Root") {
                    Boxy.load("~/Host/Tool/AppConfigRoot" + action + ".aspx?Name=" + json["Name"], { filter: ".Main", modal: true, title: "查看", width: 701 });
                }
                else if (json.Type == "Group") {
                    Boxy.load("~/Host/Tool/AppConfigGroup" + action + ".aspx?Name=" + json["Name"], { filter: ".Main", modal: true, title: "查看", width: 701 });
                }
                else {
                    Boxy.load("~/Host/Tool/AppConfig" + action + ".aspx?Name=" + json["Name"], { filter: ".Main", modal: true, title: "查看", width: 701 });
                }
            },

            jv.page().flexiEdit = function (rowData, rowIndex, grid, jtd) {
                jv.page().PopCard(rowData, grid, "Edit");
            };

            var btns = [
					{ separator: true },
					{ name: '添加', bclass: 'add', onpress: jv.page().flexiAdd },
					{ separator: true },
					{ name: "删除", bclass: "delete", onpress: function (ids, grid) { if (ids.length == 0) return; if (Confirm("确认删除这 " + ids.length + " 项吗？")) { if (jv.page().flexiDel(ids, grid) == false) return; } } },
					{ separator: true }
					];

            myGrid.flexigrid({
                title: "实体列表",
                url: "",
                role: { id: "Id", name: "Name", select: "Sel" },
                treeOpen: true,
                useId: true,
                useSelect: true,
                colModel: [
                    { display: "&nbsp;", bind: false, name: "View", width: "auto", sortable: false, align: "center",
                        html: '<a style="cursor:pointer" onclick="jv.flexiRowEvent(jv.page().flexiView,event);">查 看</a>'
                    },
                    { display: "Id", name: "Id", width: 0, sortable: false, align: "left", hide: true },
                    { display: "PID", name: "Pid", width: 0, sortable: false, align: "left", hide: true },
                    { display: "类型", name: "Type", width: "0", sortable: false, align: "left" },
                    { display: "实体名称", name: "Name", width: "400", sortable: false, align: "left" },
					{ display: "&nbsp;", bind: false, name: "Edit", width: "auto", sortable: false, align: "center",
					    html: '<a style="cursor:pointer" onclick="jv.flexiRowEvent(jv.page().flexiEdit,event);">修 改</a>'
					}
					],

                buttons: btns,
                usepager: true,
                take: 50
            });

            var myFlexi = myGrid.getFlexi();
            myFlexi.p.url = "~/Host/Tool/AppConfigQuery.aspx";
            myFlexi.populate();
        }); 
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table class="myGrid" style="width: auto">
    </table>
</asp:Content>
