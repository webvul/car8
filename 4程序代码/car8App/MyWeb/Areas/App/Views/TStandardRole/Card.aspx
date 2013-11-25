<%@ Page Language="C#" MasterPageFile="~/Areas/App/Views/Shared/App.Master" Inherits="MyCon.MyMvcPage<DbEnt.TStandardRoleRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    TStandardRole Card
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            //TextHelper 要放在最前.
            jv.SetDetail();

            //$('#txtCreateDate').datepicker();

            jv.page().Edit_OK = function (ev) {
                $.post("~/pm/Master/TStandardRole/" + jv.page()["action"] + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx", $(".divEdit:last").GetDivJson("Edit_"), function (res) {
                    if (res.msg) alert(res.msg);
                    else {
                        jv.Hide(function () {
                            $(".flexigrid", jv.boxdy()).getFlexi().populate();
                        });
                        alert("保存成功");
                        if (jv.IsInBoxy(ev)) { }
                    }
                });
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="_Is_Detail_ divEdit listView">
        <div class="MyTool">
            <input class="submit" type="button" value="保存" onclick="jv.page().Edit_OK(event)" />
        </div>
        <div class="MyCard">
            <div>
                <div class="MyCardTitle">
                    基本信息(必填)
                </div>
                <div class="kv">
                    <span class="key">StandardRoleId:</span> <span class="val">
                        <%=Html.TextBox("Edit_StandardRoleId", (Model.StandardRoleId))%></span>
                </div>
                <div class="kv">
                    <span class="key">Code:</span> <span class="val">
                        <%=Html.TextBox("Edit_Code", (Model.Code))%></span>
                </div>
                <div class="kv">
                    <span class="key">Name:</span> <span class="val">
                        <%=Html.TextBox("Edit_Name", (Model.Name))%></span>
                </div>
                <div class="kv">
                    <span class="key">Type:</span> <span class="val">
                        <%=Html.TextBox("Edit_Type", (Model.Type))%></span>
                </div>
                <div class="kv">
                    <span class="key">ParentId:</span> <span class="val">
                        <%=Html.TextBox("Edit_ParentId", (Model.ParentId))%></span>
                </div>
                <div class="kv">
                    <span class="key">StandardOrganizationId:</span> <span class="val">
                        <%=Html.TextBox("Edit_StandardOrganizationId", (Model.StandardOrganizationId))%></span>
                </div>
                <div class="kv">
                    <span class="key">OrderId:</span> <span class="val">
                        <%=Html.TextBox("Edit_OrderId", (Model.OrderId))%></span>
                </div>
                <div class="kv">
                    <span class="key">Status:</span> <span class="val">
                        <%=Html.TextBox("Edit_Status", (Model.Status))%></span>
                </div>
                <div class="kv">
                    <span class="key">Remark:</span> <span class="val">
                        <%=Html.TextBox("Edit_Remark", (Model.Remark))%></span>
                </div>
                <div class="kv">
                    <span class="key">UpdateDate:</span> <span class="val">
                        <%=Html.TextBox("Edit_UpdateDate", (Model.UpdateDate))%></span>
                </div>
            </div>
            <div class="FillHeight divSplit">
            </div>
            <div>
                <div class="MyCardTitle">
                    扩展信息(选填)
                </div>
                <div class="kv">
                    <span class="key">手动分布成两列: </span><span class="val">!!</span>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
