<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Main.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    页面权限设置
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table class="divQuery" width="100%">
        <tr>
            <td class="key">
                模块名称:
            </td>
            <td class="val">
                <input type="text" id="List_Controller" />
            </td>
            <td class="key">
                页面名称:
            </td>
            <td class="val">
                <input type="text" id="List_Action" />
            </td>
        </tr>
        <tr>
            <td class="key">
                按钮名称:
            </td>
            <td class="val">
                <input type="text" id="List_Button" />
            </td>
            <td class="key">
            </td>
            <td class="val query_btn">
                <input class="submit" type="button" value="查询" onclick="jv.page().flexiQuery()" />
            </td>
        </tr>
    </table>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());

            jv.page().flexiQuery = function () {
                var queryJDiv = $(".divQuery:last", jv.boxdy());


                myGrid.getFlexi().doSearch(queryJDiv.GetDivJson("List_"));

            };

            jv.page().adds = [];
            jv.page().minus = [];


            jv.page().flexiSave = function (ids, grid) {
                $.post("~/Admin/Power/PagePowerSave" + ".aspx" + jv.url().search, { Adds: jv.page().adds.join(","), Minus: jv.page().minus.join(",") }, function (res) {
                    if (res.msg) {
                        alert(res.msg);
                    }

                    alert("保存成功.");
                });

            };

            myGrid.flexigrid({
                title: "页面操作权限",
                url: "",
                useId: true,
                treeOpen: true,
                role: { id: "id", name: "Name", select: "Sel" },
                colModel: [
                     { display: "ID", name: "Id", width: 0, sortable: true, hide: true },
                     { display: "Sel", name: "Sel", width: 0, sortable: true },
                     { display: "类型", name: "Type", width: "80px", sortable: true },
                     { display: "名称", name: "Name", width: "auto", sortable: true },
                     { display: "描述", name: "Descr", width: "auto", sortable: true },
                     { display: "数据", name: "Data", width: "auto", sortable: true }
                     ],
                dataType: "json",
                buttons: [
                //                     { separator: true },
                //                     { name: "查询", bclass: "query", onpress: jv.page().flexiQuery },
                //                     { separator: true },
                     {name: "保存", bclass: "save pin", onpress: jv.page().flexiSave }
                    ],
                usepager: true,
                useRp: true,
                selectExtend: (jv.page().Mode == "Minus" ? "down" : "up,down"),
                onSelected: function (tr, g) {
                    if (g.isSelected(tr)) {
                        jv.page().adds.push(tr.id.slice(3));
                    } else {
                        jv.page().minus.push(tr.id.slice(3));
                    }
                },
                take: 15
            });


            myGrid.getFlexi().p.url = "~/Admin/Power/PagePowerQuery.aspx?Type=" + (jv.page().Type || "") + "&Value=" + (jv.page().Value || "");
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
