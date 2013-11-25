<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.NoticeTypeRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    公告类型
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            //TextHelper 要放在最前.
            //$("#txtStatus").TextHelper("array", "在岗,离岗");
            jv.SetDetail();

            //$('#txtCreateDate').datepicker();

            jv.page().Edit_OK = function (ev) {
                $.post("~/Admin/NoticeType/" + jv.page()["action"] + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx", $(".MyCard").GetDivJson("Edit_"), function (res) {
                    //客户端返回的是Json 。 res = $.evalJSON(res);

                    if (res.msg) alert(res.msg);
                    else {
                        jv.Hide(function () { $(".myGrid:last").getFlexi().populate(); });
                        alert("保存成功");
                        if (jv.IsInBoxy(ev)) { }
                    }
                });
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="MyTool">
        <input class="submit" type="button" value="保存" onclick="jv.page().Edit_OK(event)" />
    </div>
    <div class="MyCard">
        <%=Html.Hidden("Edit_DeptID", (Model.DeptID))%>
        <%=Html.Hidden("Edit_CategoryID", (Model.CategoryID))%>
        <div>
            <div class="MyCardTitle">
                基本信息(必填)
            </div>
            <div class="coline"></div>
            <div class="kv">
                <span class="key">ID:</span> <span class="val hard">
                    <%= (Model.Id)%><%=Html.Hidden("Edit_Id", (Model.Id))%></span>
            </div>
            <div style="display: none">
                <%= dbr.NoticeType.PopRadior("Edit_Pid",
            new UIPop()
            {
                KeyTitle = "父节点",
                area = "Admin",
                Value = Model.Pid.AsString(),
                query = new Dictionary<string, string>() {{"Wbs",Model.Wbs.AsString()} },
                Display = Model.Pid > 0 ? dbr.NoticeType.FindScalar(o => o.Name, o => o.Id == Model.Pid).AsString() : ""
            }).GenDiv()
                %>
            </div>
            <div class="kv">
                <span class="key">名称:</span> <span class="val">
                    <%=Html.TextBox("Edit_Name", (Model.Name))%></span>
            </div>
            <div class="kv">
                <span class="key">描述:</span> <span class="val">
                    <%=Html.TextBox("Edit_Descr", (Model.Descr))%></span>
            </div>
        </div>
        <div class="FillHeight divSplit">
        </div>
        <div>
            <div class="MyCardTitle">
                扩展信息
            </div>
            <div class="coline"></div>
            <div class="kv">
                <span class="key">排序号:</span> <span class="val">
                    <%=Html.TextBox("Edit_SortID", (Model.SortID))%></span>
            </div>
            <div class="kv">
                <span class="key">创建用户:</span> <span class="val hard"><span>
                    <%= Model.UserID %>&nbsp;</span> <%=Html.Hidden("Edit_UserID", (Model.UserID))%></span>
            </div>
            <div class="kv">
                <span class="key">创建时间:</span> <span class="val hard"><span>
                    <%= Model.AddTime %></span><%=Html.Hidden("Edit_AddTime", (Model.AddTime))%></span>
            </div>
            <div class="kv" style="display: none">
                <span class="key">WBS:</span> <span class="val hard"><span>
                    <%=Html.TextBox("Edit_Wbs", (Model.Wbs))%></span></span>
            </div>
        </div>
    </div>
</asp:Content>
