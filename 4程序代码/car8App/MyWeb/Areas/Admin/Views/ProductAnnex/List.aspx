<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    ProductAnnex列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
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

            var _img_Type = "";


            jv.page().flexiUplad = function () {

                jv.Upload(
                {
                    url: "~/Admin/ProductAnnex/Uploaded.aspx",
                    paras: function (files) { return { ProductID: jv.page().uid, files: files } },
                    callback: function (res) {
                        if (res.msg) {
                            alert(res.msg);
                            return;
                        }

                        myGrid[0].grid.populate();
                        jv.Hide();
                    },
                    title: "上传照片",
                    max: 8,
                    maxMsg: "每个仪器最多上传8张照片"
                });
            };



            jv.page().flexiMinImg = function (id) {

                $.post("~/Admin/ProductAnnex/MinImg/" + id + ".aspx", {}, function (res) {
                    //客户端返回的是Json
                    if (res.msg) { alert(res.msg); }
                    else {
                        $(".myGrid:last").getFlexi().populate();
                    }
                });
            };

            jv.page().flexiView = function (id) {


                Boxy.load("~/Admin/ProductAnnex/Detail/" + id + ".aspx", { filter: ".Main", modal: true, title: "查看详细" }, function (bxy) {
                    

                    $(".Main").find(".submit").one("click", function () {
                        Boxy.get(".Main").hide();
                    });
                });
            };

            jv.page().flexiAdd = function () {

                Boxy.load("~/Admin/ProductAnnex/Add" + ".aspx", { filter: ".Main", modal: true, title: "添加" }, function (bxy) {
                    
                });
            };

            jv.page().flexiEdit = function (id) {

                Boxy.load("~/Admin/ProductAnnex/Update/" + id + ".aspx", { filter: ".Main", modal: true, title: "修改" }, function (bxy) {
                    

                    //Edit页执行保存代码
                });
            };

            jv.page().flexiDel = function (ids, grid) {
                //删除。
                $.post("~/Admin/ProductAnnex/Delete.aspx", { query: ids.join(",") }, function (res) {
                    if (!res.msg) alert("删除成功 !");
                    else { alert(res.msg); }
                });
            };

            myGrid.flexigrid({
                title: "ProductAnnex列表标题",
                url: "",
                role: { name: "AnnexFullName" },
                colModel: [
                     { display: "",  bind: false, name: "View", width: 30,  align: "center", html: '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID());">查 看</a>' },
                     { display: "ID", name: "ID", width: 80, sortable: true, align: "center" },
                     { display: "类型", name: "Key", width: 80, sortable: true, align: "center", format: function (row, rowIndex, g) { var txt = row.cell[1]; if (txt == "Img") return "图片"; if (txt == "MinImg") return "缩略图"; return txt; } },
                     { display: "附件", name: "AnnexFullName", width: 140, sortable: true, align: "center", format: '<img src="$#$" style="width:120px"/>' },
                     { display: "排序", name: "SortID", width: 80, sortable: true, align: "center" },
                     { display: "",  bind: false, name: "Edit", width: 30,  align: "center", html: '<a style="cursor:pointer" onclick="jv.page().flexiEdit($(this).getFlexiRowID());">修 改</a>' },
                     { display: "",  bind: false, name: "Edit", width: 70,  align: "center", html: '<a style="cursor:pointer" onclick="jv.page().flexiMinImg($(this).getFlexiRowID());">设为缩略图</a>' }
                ],
                dataType: "json",
                buttons: [
                     { separator: true },
                     { name: "查询", bclass: "query", onpress: jv.page().flexiQuery },
                     { separator: true },
                     { name: '添加', bclass: 'add', onpress: jv.page().flexiUplad },
                     { separator: true },
                     { name: "删除", bclass: "delete", onpress: function (ids, grid) { if (ids.length == 0) return; if (confirm("确认删除这 " + ids.length + " 项吗？") == false) return; if (jv.page().flexiDel(ids, grid) == false) return; grid.populate(); } },
                     { separator: true }
                ],
                usepager: true,
                useRp: true,
                rp: 15,
                showTableToggleBtn: true
            });
            myGrid.getFlexi().p.url = "~/Admin/ProductAnnex/Query" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
            myGrid.getFlexi().populate();
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divQuery" style="display: none">
        <table style="width: 100%">
            <tr>
                <td class="key">用 户 名:
                </td>
                <td class="val">
                    <input type="text" id="List_UserName" />
                </td>
                <td class="key">类 型:
                </td>
                <td class="val">
                    <%=Html.RegisteEnum("List_Type", MyCmn.InfoEnum.Info).Radior().Input()%>
                </td>
                <td class="key">开始时间:
                </td>
                <td class="val">
                    <input type="text" readonly="readonly" id="List_BeginTime" />
                </td>
                <td class="key">结束时间:
                </td>
                <td class="val">
                    <input type="text" readonly="readonly" id="List_EndTime" />
                </td>
            </tr>
            <tr>
                <td colspan="8" class="query_btn">
                    <input class="submit" type="button" value="查询" />
                </td>
            </tr>
        </table>
    </div>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
