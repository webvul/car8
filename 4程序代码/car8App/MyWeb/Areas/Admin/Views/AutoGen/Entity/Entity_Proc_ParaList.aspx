<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    存储过程参数设置
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());

            jv.page().flexiAdd = function () {
                Boxy.ask(
                    jv.GenKv("para", "参数名", "") + jv.GenKv("type", "参数类型", "") + jv.GenKv("direction", "参数方向", "in"), ["确定"], function (res, bxy) {
                        if (res == "确定") {
                            myGrid.getFlexi().newRow({ id: bxy.boxy.find("#para").val(), cell:
                            ["", bxy.boxy.find("#para").val(), bxy.boxy.find("#type").val(), bxy.boxy.find("#direction").val()]
                            });
                        }
                    });

            };

            myGrid.flexigrid({
                title: "外键定义 " + jv.page().entity,
                url: "",
                role: { id: "Column", name: "Column" },
                treeOpen: true,
                colModel: [
                    { display: "选择", name: "Select", width: "auto",   html: "选择" },
                    { display: "参数名", name: "Name", width: "auto",  align: "left" },
                    { display: "参数类型", name: "Type", width: "auto",  align: "left" },
                    { display: "参数方向", name: "Direction", width: "auto",  align: "left" }
					],
                dataType: "json",
                buttons: [
                    { separator: true },
					{ name: '添加', bclass: 'pin add', onpress: jv.page().flexiAdd }
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
                            if (i == 1 || i == 2 || i == 3) {
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
