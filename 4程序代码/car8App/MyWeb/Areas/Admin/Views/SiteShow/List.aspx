<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SiteShow列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function flexiQuery(ids, grid) {
            

            new Boxy($(".Main"), { modal: true, title: "查询" }).resize(400, 100);
            $(".Main").find(".submit").one("click", function () {
                //查询。

                $(".myGrid:last")[0].grid.doSearch($(".Main").GetDivJson("List_"));
                Boxy.get(".Main").hide();
            });
        }
        function flexiView(id) {
            

            Boxy.load("~/Admin/SiteShow/Detail/" + id + ".aspx", { filter: ".Main", modal: true, title: "查看详细" }, function (bxy) {
                

                $(".Main").find(".submit").one("click", function () {
                    Boxy.get(".Main").hide();
                });
            });
        }
        function flexiAdd() {
            
            Boxy.load("~/Admin/SiteShow/Add" + ".aspx", { filter: ".Main", modal: true, title: "添加" }, function (bxy) {
                
            });
        }

        function flexiEdit(id) {
            
            Boxy.load("~/Admin/SiteShow/Update/" + id + ".aspx", { filter: ".Main", modal: true, title: "修改" }, function (bxy) {
                

                //Edit页执行保存代码
            });
        }

        function flexiDel(ids, grid) {
            //删除。
            $.post("~/Admin/SiteShow/Delete.aspx", { query: ids.join(",") }, function (res) {
                if ( !res.msg ) alert("删除成功 !");
                else { alert(res.msg); }
            });
        }

        $(function () {
            $(".myGrid:last").flexigrid({
                title: "SiteShow列表标题",
                url: "",
                role: { name: "ProductID" },
                colModel: [
                     { display: "查 看",  bind: false, name: "View", width: 30,  align: "center", html: '<a style="cursor:pointer" onclick="flexiView($(this).parents(\'tr\')[0].id.slice(3));">查 看</a>' },
                     { display: "ID", name: "ID", width: 80, sortable: true, align: "center", hide: true },
                     { display: "ProductID", name: "ProductID", width: 80, sortable: true, align: "center" },
                     { display: "Type", name: "Type", width: 80, sortable: true, align: "center" },
                     { display: "CategoryID", name: "CategoryID", width: 80, sortable: true, align: "center" },
                     { display: "SortID", name: "SortID", width: 80, sortable: true, align: "center" },
                     { display: "BeginTime", name: "BeginTime", width: 80, sortable: true, align: "center" },
                     { display: "EndTime", name: "EndTime", width: 80, sortable: true, align: "center" },
                     { display: "DeptID", name: "DeptID", width: 80, sortable: true, align: "center" },
                     { display: "IsValid", name: "IsValid", width: 80, sortable: true, align: "center" },
                     { display: "AddTime", name: "AddTime", width: 80, sortable: true, align: "center" },
                     { display: "修 改",  bind: false, name: "Edit", width: 30,  align: "center", html: '<a style="cursor:pointer" onclick="flexiEdit($(this).parents(\'tr\')[0].id.slice(3));">修 改</a>' }
                     ],
                dataType: "json",
                buttons: [
                     { separator: true },
                     { name: "查询", bclass: "query", onpress: flexiQuery },
                     { separator: true },
                     { name: '添加', bclass: 'add', onpress: flexiAdd },
                     { separator: true },
                     { name: "删除", bclass: "delete", onpress: function (ids, grid) { if (ids.length == 0) return; if (confirm("确认删除这 " + ids.length + " 项吗？") == false) return; if (flexiDel(ids, grid) == false) return; grid.populate(); } },
                     { separator: true }
                     ],
                usepager: true,
                useRp: true,
                rp: 15,
                showTableToggleBtn: true
            });
            $(".myGrid:last").getFlexi().p.url = "~/Admin/SiteShow/Query" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
            $(".myGrid:last").getFlexi().getFlexi().populate();
        }); 

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="divQuery" style="display: none">
       <table style="width: 100%">
            <tr>
                <td class="key">
                    用 户 名:
                </td>
                <td class="val">
                    <input type="text" id="List_UserName"  />
                </td>
                <td class="key">
                    类 型:
                </td>
                <td class="val">
                   <%=Html.EnumRadior("List_Type", MyCmn.InfoEnum.Info)%>
                </td>
                <td class="key">
                    开始时间:
                </td>
                <td class="val">
                    <input type="text" readonly="readonly" id="List_BeginTime"  />
                </td>
                <td class="key">
                    结束时间:
                </td>
                <td class="val">
                    <input type="text" readonly="readonly" id="List_EndTime"  />
                </td>
            </tr>
            <tr>
                <td colspan="8" class="query_btn">
                           <input class="submit" type="button" value="查询" />
                </td>
            </tr>
        </table>
    </div>
    <table id="myGrid" style="display: none">
    </table>
</asp:Content>
