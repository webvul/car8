<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    生成视图
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());

            jv.page().flexiQuery = function (ids, grid) {
                var queryJDiv = $(".divQuery:last", jv.boxdy());

                new Boxy(queryJDiv, { modal: true, title: "查询" });
                queryJDiv.find(".submit").one("click", function () {
                    //查询。

                    grid.doSearch(queryJDiv.GetDivJson("List_"));
                    Boxy.get(queryJDiv).hide();
                });
            };

            jv.page().flexiView = function (id) {
                Boxy.load("~/Admin/Menu/Detail/" + id + ".aspx", { filter: ".Main:last", modal: true, title: "查看详细" }, function (bxy) {
                    
                });
            };

            jv.page().flexiAdd = function () {
                Boxy.ask(
                    jv.GenKv("tab", "存储过程名", "") + jv.GenKv("config", "数据库配置", "dbo") + jv.GenKv("owner", "所有者", "dbo"), ["确定"], function (res, bxy) {
                        if (res == "确定") {
                            Boxy.load(
                            "~/Admin/AutoGen/EntityProcAdd/" + bxy.boxy.find("#tab").val() + ".aspx?DbConfig=" + bxy.boxy.find("#config").val() + "&Owner=" + bxy.boxy.find("#owner").val(),
                                { filter: ".Main:last", modal: true, title: "添加" }, function (bxy) {
                                    bxy.resize(601);
                                });

                        }
                    }, { title: "必填项" });
            };

            jv.page().flexiEdit = function (id) {
                Boxy.load("~/Admin/AutoGen/EntityProcUpdate/" + id + ".aspx", { filter: ".Main:last", modal: true, title: "修改" }, function (bxy) {
                    

                    //Edit页执行保存代码
                });
            };

            jv.page().flexiDel = function (ids, grid) {
                //删除。
                $.post("~/Admin/AutoGen/EntityProcDelete.aspx", { query: ids.join(",") }, function (res) {
                    if (!res.msg) alert("删除成功 !");
                    else { alert(res.msg); }
                });
            };

            myGrid.flexigrid({
                title: "生成存储过程",
                url: "",
                role: { id: "Name", name: "Name", select: "Sel" },
                treeOpen: true,
                colModel: [
                //                    { display: "模块ID", name: "Id", width: "auto",  align: "left" },
                    {display: "描述", name: "Descr", width: 80,  align: "left" },
                    { display: "名称", name: "Name", width: 80,  align: "left" },
                    { display: "参数", name: "Paras", width: 280,  align: "left" },
                    { display: "返回值", name: "Return", width: 80,  align: "left" },
                    { display: "所有者", name: "Owner", width: 80,  align: "left" },
                    { display: "数据库", name: "db", width: 80,  align: "left" },
                    { display: "操 作",  bind: false, name: "Edit", width: 30,   html: '<a style="cursor:pointer" onclick="jv.page().flexiEdit($(this).getFlexiRowID(),event);">修 改</a>' }
 					],
                dataType: "json",
                buttons: [
                    { separator: true },
					{ name: '查询', bclass: 'query', onpress: jv.page().flexiQuery },
					{ separator: true },
					{ name: '添加', bclass: 'add', onpress: jv.page().flexiAdd },
					{ separator: true },
					{ name: "删除", bclass: "delete", onpress: function (ids, grid) { if (ids.length == 0) return; if (Confirm("确认删除这 " + ids.length + " 项吗？")) { if (jv.page().flexiDel(ids, grid) == false) return; } } },
					{ separator: true }
					],
                usepager: true,
                selectExtend: "up,down",
                take:15,
                showTableToggleBtn: true
            });

            myGrid.getFlexi().p.url = "~/Admin/AutoGen/EntityProcQuery" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
            myGrid.getFlexi().populate();
        }); 
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divQuery" style="display: none">
        <div class="kv Inline">
            <span class="key">表名:</span> <span class="val fix120">
                <input type="text" id="List_Name" /></span></div>
        <div class="kv Inline" style="text-align: right">
            <span class="key">描述:</span> <span class="val fix120">
                <input type="text" id="List_Descr" /></span></div>
        <div class="query_btn">
            <input class="submit" type="button" value="查询" /></div>
    </div>
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
