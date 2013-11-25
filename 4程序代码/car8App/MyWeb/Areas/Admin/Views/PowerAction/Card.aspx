<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.PowerActionRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    PowerAction Card
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            //TextHelper 要放在最前.
            jv.SetDetail();

            //$('#txtCreateDate').datepicker();

            jv.page().Edit_OK = function (ev) {
                $.post("~/Admin/PowerAction/" + jv.page()["action"] + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx", $(".MyCard", jv.boxdy()).GetDivJson("Edit_"), function (res) {
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
    <div class="divCard">
        <div class="MyTool">
            <input class="submit" type="button" value="保存" onclick="jv.page().Edit_OK(event)" />
        </div>
        <div class="MyCard">
            <div>
                <div class="MyCardTitle">
                    基本信息(必填)
                </div><div class="coline"> </div>
                <div class="kv">
                    <span class="key">Id:</span> <span class="val">
                        <%=Model.Id%>
                        <%=Html.Hidden("Edit_Id", (Model.Id))%></span>
                </div>
                <%
                    dbr.PowerController.PopRadior("Edit_ControllerID",new UIPop
                    {
                        KeyTitle = "模块",
                        Value = Model.ControllerID.HasValue() ? Model.ControllerID.AsString() : "",
                        Display = Model.ControllerID.HasValue() ? Model.GetPowerController().Descr.AsString() : "(空)"
                    }).GenDiv();
                    
                %>
            </div>
            <div class="FillHeight divSplit">
            </div>
            <div>
                <div class="MyCardTitle">
                    扩展信息(选填)
                </div><div class="coline"> </div>
                <div class="kv">
                    <span class="key">页面:</span> <span class="val">
                        <%=Html.TextBox("Edit_Action", (Model.Action))%></span>
                </div>
                <div class="kv">
                    <span class="key">页面描述:</span> <span class="val">
                        <%=Html.TextBox("Edit_Descr", (Model.Descr))%></span>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
