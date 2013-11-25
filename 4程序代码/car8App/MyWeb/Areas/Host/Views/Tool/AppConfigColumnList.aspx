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

            var btns = [];

            myGrid.flexigrid({
                title: "列定义（部分）",
                url: "",
                role: { id: "Name", name: "Name", select: "Sel" },
                treeOpen: true,
                useId: true,
                useSelect: true,
                colModel: [
                    { display: "名称", name: "Name", width: "auto", sortable: false, align: "left" },
                    { display: "映射名称", name: "MapName", width: "auto", sortable: false, align: "left" },
                    { display: "主键", name: "Pk", width: "auto", sortable: false, align: "left" },
                    { display: "自增", name: "AutoIncre", width: "auto", sortable: false, align: "left" },
                    { display: "唯一键", name: "Unique", width: "auto", sortable: false, align: "left" },
                    { display: "外键", name: "Fk", width: "auto", sortable: false, align: "left" },
                    { display: "指定类型", name: "Enum", width: "auto", sortable: false, align: "left" }
					],

                buttons: btns,
                usepager: true,
                FillHeight:false,
                take: 50
            });

            var myFlexi = myGrid.getFlexi();
            myFlexi.p.url = "~/Host/Tool/AppConfigColumnQuery.aspx" + jv.url().search;
            myFlexi.populate();
        }); 
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="Main">
        <table class="myGrid" style="width: auto">
        </table>
    </div>
</asp:Content>
