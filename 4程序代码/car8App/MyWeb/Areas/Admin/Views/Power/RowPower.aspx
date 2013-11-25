<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Main.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    行权限设置
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var myGrid = jv.boxdy().find(".myGrid");

            jv.page().flexiQuery = function (ids, grid) {
                var queryJDiv = $(".divQuery:last", jv.boxdy());

                new Boxy(queryJDiv, { modal: true, title: "查询" }).resize(400, 100);
                queryJDiv.find(".submit").one("click", function () {
                    //查询。

                    grid.doSearch(queryJDiv.GetDivJson("List_"));
                    Boxy.get(queryJDiv).hide();
                });
            };


            jv.page().flexiView = function (id) {
                
                var ent = $($(jv.GetDoer()).parents("tr:first")[0].cells[2]).text();

                jv.PopList({ url: jv.url().root + "Admin/" + ent + "/List.aspx", mode: "check" });
            };

            jv.page().flexiPower = function (id, RowType) {
                
                //                var ent = $($(jv.GetDoer()).parents("tr:first")[0].cells[2]).text();
                var query = "Type=" + jv.page().Type + "&Value=" + jv.page().Value + "&RowType=" + RowType + "&Mode=" + (jv.page().Mode || "");
                jv.PopList({ entity: id, mode: "check", callback: function (role, kv) {

                    $.post("~/Admin/Power/UpdateRowPower.aspx?" + query, { Entity: id, Power: jv.GetJsonKeys(kv).join(",") }, function (res) {
                        if (res.msg) alert(res.msg);
                        else {

                        }
                    });
                }
                });
            };


            myGrid.flexigrid({
                title: "行集权限管理",
                url: "",
                useId: true,
                colModel: [
                     { display: "查 看",  bind: false, name: "View", width: 32,   html: '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID());">查 看</a>' },
                     { display: "行集表", name: "Name", sortable: true },
                     { display: "表名", name: "TableName", sortable: true },
                     { display: "ViewPower", name: "ViewPower", width: 0, sortable: true },
                     { display: "EditPower", name: "EditPower", width: 0, sortable: true },
                     { display: "设置权限",  bind: false, name: "PowerView",   html: '<a style="cursor:pointer" onclick="jv.page().flexiPower($(this).getFlexiRowID(),&quot;View&quot;);">设置权限<input type="hidden" value="$ViewPower$" /></a><span class="val" style="display:none"></span>' }
                //                     { display: "修改权限",  bind: false, name: "PowerEdit",   html: '<a style="cursor:pointer" onclick="jv.page().flexiPower($(this).getFlexiRowID(),&quot;Edit&quot;);">修改权限<input type="hidden" value="$EditPower$" /></a><span class="val" style="display:none"></span>' }
                     ],
                dataType: "json",
                buttons: [
                     { separator: true },
                     { name: "查询", bclass: "query", onpress: jv.page().flexiQuery }
                     ],
                usepager: true,
                useRp: true,
                take:15,
                showTableToggleBtn: true
            });

            myGrid.getFlexi().p.url = "~/Admin/Power/RowPowerQuery" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx?Type=" + jv.page().Type + "&Value=" + jv.page().Value;
            myGrid.getFlexi().populate();

        }); 

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divQuery" style="display: none">
        <div class="input">
            查询条件一:<input type="text" id="List_Code" style="width: 120px;" />
            查询条件二:<input type="text" id="List_Id" style="width: 120px;" /><br />
            查询条件三:<input type="text" id="List_Name" style="width: 120px;" />
            查询条件四:<input type="text" id="List_OtherName" style="width: 120px;" /><br />
        </div>
        <br />
        <input class="submit" type="button" value="查询" />
    </div>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
