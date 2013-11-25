<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.SiteShowRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SiteShow Edit
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
            
            $.post("~/Admin/SiteShow/" + jv.page()["action"] + "/" + (jv.page()["uid"] || "0") + ".aspx", $(".MyCard").GetDivJson("Edit_"), function (res) {
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
                <%=Html.TextBox("Edit_Id", (Model.Id))%>
            </td>
            <td class='tdSplit'>
            </td>
            <td class='tdEdit'>
                ProductID:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_ProductID", (Model.ProductID))%>
            </td>
        </tr>
        <tr>
            <td class='tdEdit'>
                Type:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_Type", (Model.Type))%>
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
                SortID:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_SortID", (Model.SortID))%>
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
                DeptID:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_DeptID", (Model.DeptID))%>
            </td>
        </tr>
        <tr>
            <td class='tdEdit'>
                IsValid:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_IsValid", (Model.IsValid))%>
            </td>
            <td class='tdSplit'>
            </td>
            <td class='tdEdit'>
                AddTime:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_AddTime", (Model.AddTime))%>
            </td>
        </tr>
    </table>
    <input class="submit" type="button" value="保存" onclick="Edit_OK(event)" />
</asp:Content>
