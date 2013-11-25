<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.PowerActionRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    PowerAction Edit
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            jv.SetDetail();

            if (jv.page().uid) {
                $(".grid:last").LoadView({ url: "~/Admin/PowerButton/List/" + jv.page().uid + ".aspx" });
            }
            //$('#txtCreateDate').datepicker();
            //$("#txtStatus").TextHelper("array", "在岗,离岗");
            //$("#txtChannelManager").TextHelper("change", "~/C3_Shop/LoadStaff/?post=渠道经理", 500);

        });
        jv.page().Edit_OK = function (ev) {
            

            $.post("~/Admin/PowerAction/" + jv.page()["action"] + "/" + (jv.page()["uid"] || "0") + ".aspx", $(".MyCard").GetDivJson("Edit_"), function (res) {
                //客户端返回的是Json 。 res = $.evalJSON(res);
                if (res.msg) alert(res.msg);
                else {
                    jv.Hide(function () { $(".flexigrid", jv.boxdy()).getFlexi().populate(); });
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divCard">
        <table width="100%">
            <tr>
                <td class='tdEdit'>
                    ID:
                </td>
                <td class='tdVal'>
                    <%= Model.Id %>
                    <%=Html.Hidden("Edit_Id", (Model.Id))%>
                </td>
                <td class='tdSplit'>
                </td>
                <td class='tdEdit'>
                    Action:
                </td>
                <td class='tdVal'>
                    <%=Html.TextBox("Edit_Action", Model.Action)%>
                </td>
            </tr>
            <tr>
                <td class='tdEdit'>
                    ControllerID:
                </td>
                <td class='tdVal'>
                    <%  dbr.PowerController.PopRadior("Edit_ControllerID", new UIPop
                        {
                            KeyTitle = "...",
                            Value = Model.ControllerID.AsString(),
                            Display = Model.ControllerID.AsString()
                        }).GenTd();
                    %>
                </td>
                <td class='tdSplit'>
                </td>
                <td class='tdVal'>
                </td>
            </tr>
        </table>
        <input class="submit" type="button" value="保存" onclick="jv.page().Edit_OK(event)" />
        <div class="grid">
        </div>
    </div>
</asp:Content>
