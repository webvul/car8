<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.LogRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Log Edit
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            jv.SetDetail();
            
            //$('#txtCreateDate').datepicker();
            //$("#txtStatus").TextHelper("array", "在岗,离岗");

        });
        jv.page().Edit_OK = function (ev) {
            $.post("~/Admin/Log/" + jv.page()["action"] + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx", $(".MyCard").GetDivJson("Edit_"), function (res) {
                //客户端返回的是Json 。 res = $.evalJSON(res);
                if (res.msg ) alert(res.msg);
                else {
                    jv.Hide(function () { $(".flexigrid", jv.boxdy()).getFlexi().populate(); });
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divCard"> 
        <div class="MyTool"> <input class="submit" type="button" value="保存" onclick="jv.page().Edit_OK(event)" /></div>
       
        <div class="MyCard">
        <table width="100%">
            <tr>
                <td class='tdEdit'>
                    ID:
                </td>
                <td class='tdVal'>
                    <%= Model.Id %><%=Html.Hidden("Edit_Id", Model.Id)%>
                </td>
                <td class='tdSplit'>
                </td>
                <td class='tdEdit'>
                    Type:
                </td>
                <td class='tdVal'>
                    <%=Html.TextBox("Edit_Type", Model.Type)%>
                </td>
            </tr>
            <tr>
                <td class='tdEdit'>
                    UserName:
                </td>
                <td class='tdVal'>
                    <%=Html.TextBox("Edit_UserName", Model.UserName)%>
                </td>
                <td class='tdSplit'>
                </td>
                <td class='tdEdit'>
                    Msg:
                </td>
                <td class='tdVal'>
                    <%=Html.TextArea("Edit_Msg", Model.Msg)%>
                </td>
            </tr>
            <tr>
                <td class='tdEdit'>
                    AddTime:
                </td>
                <td class='tdVal'>
                    <%=Html.TextBox("Edit_AddTime", Model.AddTime)%>
                </td>
                <td class='tdSplit'>
                </td>
                <td class='tdVal'>
                </td>
            </tr>
        </table></div>
       
    </div>
</asp:Content>
