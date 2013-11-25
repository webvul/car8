<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    MyWork
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table>
        <tr valign="top">
            <td>
                <div class="c1">
                </div>
            </td>
            <td>
                <div class="c2">
                </div>
            </td>
        </tr>
    </table>
    <h2>
        MyWork</h2>
    <div class="c3">
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $(".c1:last").LoadView({ url: "~/Admin/PowerController/List.aspx", callback: function (jobj) {
                $.timer(500, function (timer) { jobj.find(".flexigrid").css("width", "460px"); timer.stop(); });
            }
            });
            $(".c2:last").LoadView({ url: "~/Admin/PowerAction/List.aspx", callback: function (jobj) {
                $.timer(500, function (timer) { jobj.find(".flexigrid").css("width", "460px"); timer.stop(); });
            }
            });
        })
    </script>
</asp:Content>
