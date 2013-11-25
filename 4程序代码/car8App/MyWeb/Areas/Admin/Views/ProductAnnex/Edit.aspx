<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.ProductAnnexRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    ProductAnnex Edit
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
            
            $.post("~/Admin/ProductAnnex/" + jv.page()["action"] + "/" + (jv.page()["uid"] || "0") + ".aspx", $(".MyCard").GetDivJson("Edit_"), function (res) {
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
                Key:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_Key", (Model.Key))%>
            </td>
        </tr>
        <tr>
            <td class='tdEdit'>
                ProductID:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_ProductID", (Model.ProductID))%>
            </td>
            <td class='tdSplit'>
            </td>
            <td class='tdEdit'>
                AnnexID:
            </td>
            <td class='tdVal'>
                <%=Html.TextBox("Edit_AnnexID", (Model.AnnexID))%>
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
            <td class='tdVal'>
            </td>
        </tr>
    </table>
    <input class="submit" type="button" value="保存" onclick="Edit_OK(event)" />
</asp:Content>
