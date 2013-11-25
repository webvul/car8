<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Edit
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divQuery" style="display: none">
        <div class="kv Inline">
            <span class="key">编码:</span> <span class="val">
                <input type="text" id="List_Code" /></span></div>
        <div class="kv Inline" style="text-align: right">
            <span class="key">名称:</span> <span class="val">
                <input type="text" id="List_Name" /></span></div>
        <div class="query_btn">
            <input class="submit" type="button" value="查询" /></div>
    </div>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());

            jv.page().flexiQuery = function (ids, grid) {
                var queryJDiv = $(".divQuery:last", jv.boxdy());

                new Boxy(queryJDiv, { modal: true, title: "查询", width: 480 });
                queryJDiv.find(".submit").one("click", function () {
                    //查询。
                    console.debug($(".myGrid", jv.boxdy()));
                    //                    grid.doSearch(queryJDiv.GetDivJson("List_"));
                    //                    Boxy.get(queryJDiv).hide();
                });
            };

            jv.page().flexiSave = function (ids, grid) {
                var adds = [];
                var minus = [];
                //取Sel Index.
                var idIndex = myGrid.find("thead tr th[bindname=Id]").index();
                var selIndex = myGrid.find("thead tr th[bindname=Sel]").index();
                myGrid.find("tbody tr").each(function (ir, tr) {
                    if ($(tr).hasClass("trSelected")) {
                        if ($(tr.cells[selIndex]).text() == "False") {
                            adds.push(tr.id.slice(3));
                        }
                    }
                    else if ($(tr.cells[selIndex]).text() == "True") {
                        minus.push(tr.id.slice(3));
                    }
                });

                $.post("~/Admin/Power/" + jv.page().action + "Save.aspx?Type=" + jv.page().Type + "&Value=" + jv.page().Value + "&Mode=" + (jv.page().Mode || ""), { Adds: adds.join(","), Minus: minus.join(",") }, function (res) {
                    if (res.msg) {
                        alert(res.msg);
                    }

                    alert("保存成功.");
                });

            };

            myGrid.flexigrid({
                title: "数据列权限",
                url: "",
                useId: true,
                treeOpen: true,
                role: { id: "Id", name: "Column", select: "Sel" },
                colModel: [
                     { display: "ID", name: "Id", width: 0, sortable: true },
                     { display: "列名称", name: "Column", width: "auto", sortable: true },
                     { display: "Sel", name: "Sel", width: "0", sortable: true, format: function (row, rowIndex, g,td) {
                         if (row.cell[2] == "True") return "True";
                         else if (row.id.indexOf("T_") == 0) {
                             if (row.rows.lmFirst(function (i, d) { return d.cell[2] == "True"; })) {
                                 return "True";
                             }
                         }
                         return "False";
                     }
                     }
                     ],
                dataType: "json",
                buttons: [
                     { separator: true },
                     { name: "查询", bclass: "query", onpress: jv.page().flexiQuery },
                     { separator: true },
                     { name: "保存", bclass: "save", onpress: jv.page().flexiSave }
                    ],
                usepager: true,
                useRp: true,
                take:15,
                selectExtend: "up,down",
                showTableToggleBtn: true
            });


            myGrid.getFlexi().p.url = "~/Admin/Power/" + jv.page().action + "Query.aspx?Type=" + jv.page().Type + "&Value=" + jv.page().Value;
            myGrid.getFlexi().populate();


        });
    </script>
    <style type="text/css">
        .Create span, .Read span, .Update span, .Delete span, .Action span, .Button span
        {
            margin: 5px;
        }
    </style>
</asp:Content>
