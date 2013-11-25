<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<PmEnt.MenuRule.Entity>" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" runat="server">
    菜单编辑
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            jv.SetDetail("AppConfigDetail");

            jv.page().Edit_OK = function (ev) {
                if (!jv.chk()) {
                    return;
                }
                $.post(window.location.href, $(".Main:last").GetDivJson("Edit_"), function (res) {
                    //客户端返回的是Json 。 res = $.evalJSON(res);
                    if (jv.IsInBoxy(ev)) {
                        if (res.msg) alert(res.msg);
                        else {
                            jv.Hide(function () { $(".flexigrid", jv.boxdy()).getFlexi().populate(); });
                        }
                    }
                    else {
                        if (res.msg) alert(res.msg);
                        else alert("保存成功");
                    }
                });
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
    <div class="MyTool">
        <input class="submit" type="button" value="保存" onclick="jv.page().Edit_OK(event)" />
    </div>
    <div class="MyCard">
        <div>
            <div class="MyCardTitle">
                基本信息(必填)
            </div>
            <div class="kv">
                <span class="key" title="输入'-'表示菜单分隔符">显示名称:</span> <span class="val">
                    <%=Html.TextBox("Edit_Text", (Model.Text), new { chk = @"{1,50} " ,chk_msg = ""})%></span>
            </div>
            <%=Html.Hidden("Edit_Id", Model.Id)%>
            <%
                dbr.Menu.PopRadior("Edit_Pid",new UIPop
                {
                    KeyTitle = "父模块",
                    Value = Model.Pid.AsString(),
                    Display = Model.Pid > 0 ? dbr.Menu.NoPower().FindById(Model.Pid).Text : "根节点"
                })
                    .GenDiv();
            %>
            <div class="kv">
                <span class="key">网址:</span> <span class="val">
                    <%=Html.TextArea("Edit_Url", (Model.Url))%></span>
            </div>
        </div>
        <div class="FillHeight divSplit">
        </div>
        <div>
            <div class="MyCardTitle">
                扩展信息(选填)
            </div>
            <div class="kv">
                <span class="key">维护状态:</span> <span class="val">
                    <% Html.RegisteEnum("Edit_Status", Model.Status)
                           .Input()
                           .Radior();
                    %>
                </span>
            </div>
            <div class="kv">
                <span class="key">排序号:</span> <span class="val">
                    <%=Html.TextBox("Edit_SortID", (Model.SortID))%></span>
            </div>
            <div class="kv">
                <span class="key">更新时间:</span> <span class="val">
                    <%=Html.TextBox("Edit_AddTime",  Model.AddTime.AsMyDate(DateTime.Today), new { @class="MyDate", format="{yyyy}年{MM}月{dd}日"})%></span>
            </div>
        </div>
    </div>
</asp:Content>
