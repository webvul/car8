<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.ContactMsgRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    ContactMsg Edit
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            jv.SetDetail();

            //$('#txtCreateDate').datepicker();
            jv.page().Edit_OK = function(ev) {

                $.post("~/Admin/ContactMsg/" + jv.page()["action"] + "/" + (jv.page()["uid"] || "0") + ".aspx", $(".MyCard").GetDivJson("Edit_"), function (res) {
                    //客户端返回的是Json
                    if (res.msg) alert(res.msg);
                    jv.Hide(function () { $(".flexigrid", jv.boxdy()).getFlexi().populate(); });
                });
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="MyCard">
        <input class="submit" type="button" value="保存" onclick="jv.page().Edit_OK(event)" />
        <table width="100%">
            <tr>
                <td class='tdEdit'>ID:
                </td>
                <td class='tdVal'>
                    <%=Html.TextBox("Edit_Id", (Model.Id))%>
                </td>
                <td class='tdSplit'></td>
                <td class='tdEdit'>DeptID:
                </td>
                <td class='tdVal'>
                    <%=Html.TextBox("Edit_DeptID", (Model.DeptID))%>
                </td>
            </tr>
            <tr>
                <td class='tdEdit'>Subject:
                </td>
                <td class='tdVal'>
                    <%=Html.TextBox("Edit_Subject", (Model.Subject))%>
                </td>
                <td class='tdSplit'></td>
                <td class='tdEdit'>Msg:
                </td>
                <td class='tdVal'>
                    <%=Html.TextArea("Edit_Msg", (Model.Msg))%>
                </td>
            </tr>
            <tr>
                <td class='tdEdit'>SenderName:
                </td>
                <td class='tdVal'>
                    <%=Html.TextBox("Edit_SenderName", (Model.SenderName))%>
                </td>
                <td class='tdSplit'></td>
                <td class='tdEdit'>SenderUserID:
                </td>
                <td class='tdVal'>
                    <%=Html.TextBox("Edit_SenderUserID", (Model.SenderUserID))%>
                </td>
            </tr>
            <tr>
                <td class='tdEdit'>AddTime:
                </td>
                <td class='tdVal'>
                    <%=Html.TextBox("Edit_AddTime", (Model.AddTime))%>
                </td>
                <td class='tdSplit'></td>
                <td class='tdEdit'>Url：
                </td>
                <td class='tdVal'><%=Html.TextArea("Edit_Url", (Model.Url))%>
                </td>
            </tr>
        </table>

    </div>
</asp:Content>
