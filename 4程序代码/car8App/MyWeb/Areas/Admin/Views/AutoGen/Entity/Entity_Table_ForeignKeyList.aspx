<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    外键
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());


            myGrid.flexigrid({
                title: "外键定义 " + jv.page().entity,
                url: "",
                role: { id: "Column", name: "Column" },
                treeOpen: true,
                colModel: [
                    { display: "选择", name: "Select", width: "auto",   html: "选择" },
                    { display: "外键", name: "Column", width: "auto",  align: "left" },
                    { display: "引用表", name: "RefTable", width: "auto",  align: "left" },
                    { display: "引用列", name: "RefColumn", width: "auto",  align: "left" }
					],
                dataType: "json",
                buttons: [
					],
                usepager: true,
                selectExtend: "up,down",
                take:15,
                onSuccess: function (grid) {
                    $(grid.gDiv).find("table tbody tr").each(function (i, r) {

                        if (grid.p._Quote_Value_ && grid.p._Quote_Value_.code) {

                            $(grid.p._Quote_Value_.code.split(",")).each(function (i, d) {
                                var col = $(jv.SplitWithReg(d, "=|:")).filter(function (i, d) { return !d.Split; });  //.split("=")[0];
                                if (r.id.slice(3) == col[0].Value) {
                                    $(r).addClass("trSelected");
                                    if (col[1].Value) {
                                        $(r.cells[2]).find(">div").text(col[1].Value);
                                    }
                                    if (col[2]) {
                                        $(r.cells[3]).find(">div").text(col[2].Value);
                                    }
                                }
                            });

                        }

                        $(r).find("td").each(function (i, d) {
                            if (i == 2 || i == 1) {
                                $(d).find(">div")
                                    .css("border", "solid 1px green")
                                    .attr("contentEditable", "true");
                            }
                        });
                    });
                },
                onSelect: function (tr) {
                    var tds = $(jv.GetDoer()).parents("td:first");
                    if (tds.length == 0) return;

                    var tdIndex = tds[0].cellIndex;
                    if (tdIndex == 2 || tdIndex == 3) {
                        $(tr).toggleClass("trSelected");
                    }
                },
                showTableToggleBtn: true
            });


            myGrid.getFlexi().p.url = "~/Admin/AutoGen/ForeignKeyQuery.aspx?entity="
                + jv.page().entity + "&db=" + jv.page().db + "&owner=" + jv.page().owner;
            myGrid.getFlexi().populate();
        }); 
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divQuery" style="display: none">
        <div class="input">
            显示名称:<input type="text" id="List_Text" style="width: 120px;" />
        </div>
        <br />
        <input class="submit" type="button" value="查询" />
    </div>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
