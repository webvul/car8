<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.StockRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Stock Edit
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            jv.SetDetail();

            //$('#txtCreateDate').datepicker();
            //$("#txtStatus").TextHelper("array", "在岗,离岗");
            //$("#txtChannelManager").TextHelper("change", "~/C3_Shop/LoadStaff/?post=渠道经理", 500);

        });
        function Edit_OK(ev) {
            
            $.post("~/Admin/Stock/" + jv.page()["action"] + "/" + (jv.page()["uid"] || "0") + ".aspx", $(".MyCard").GetDivJson("Edit_"), function (res) {
                //客户端返回的是Json
                if (res.msg) alert(res.msg);
                jv.Hide(function () { $(".myGrid:last").getFlexi().populate(); });
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table width="100%">
        <tr>
            <td class='tdEdit'>
                ID:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_ID", (Model.ID))%>
            </td>
            <td class='tdSplit'>
            </td>
            <td class='tdEdit'>
                CategoryID:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_CategoryID", (Model.CategoryID))%>
            </td>
        </tr>
        <tr>
            <td class='tdEdit'>
                DeptID:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_DeptID", (Model.DeptID))%>
            </td>
            <td class='tdSplit'>
            </td>
            <td class='tdEdit'>
                Title:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_Title", (Model.Title))%>
            </td>
        </tr>
        <tr>
            <td class='tdEdit'>
                Detail:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_Detail", (Model.Detail))%>
            </td>
            <td class='tdSplit'>
            </td>
            <td class='tdEdit'>
                Number:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_Number", (Model.Number))%>
            </td>
        </tr>
        <tr>
            <td class='tdEdit'>
                AddTime:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_AddTime", (Model.AddTime))%>
            </td>
            <td class='tdSplit'>
            </td>
            <td class='tdEdit'>
                BeginTime:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_BeginTime", (Model.BeginTime))%>
            </td>
        </tr>
        <tr>
            <td class='tdEdit'>
                EndTime:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_EndTime", (Model.EndTime))%>
            </td>
            <td class='tdSplit'>
            </td>
            <td class='tdEdit'>
                SortID:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_SortID", (Model.SortID))%>
            </td>
        </tr>
        <tr>
            <td class='tdEdit'>
                Status:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_Status", (Model.Status))%>
            </td>
            <td class='tdSplit'>
            </td>
            <td class='tdEdit'>
                Lang:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_Lang", (Model.Lang))%>
            </td>
        </tr>
    </table>
    <input class="submit" type="button" value="保存" onclick="Edit_OK(event)" />
</asp:Content>
