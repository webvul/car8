<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/App/Views/Shared/App.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    人员列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            var myGrid = $(".myGrid", jv.boxdy());

            myGrid.flexigrid({
                title: "人员",
                url: "",
                role: { id: "Id", name: "UserName" ,group:"UserName" },  //id : 在被引用的时候,返回的Id值 ; name :在被引用的时候,返回的显示值.
                colModel: [
                  { display: "Id", name: "Id", width: "0", align: "left" },
                  { display: "姓名", name: "UserName", width: "auto", align: "left" },
                  { display: "公司-项目-角色", name: "Company", width: "auto", align: "left" }
					],
                dataType: "json",
                resizable: true,
                buttons: [
					],
                usepager: true,
                take: 15,
                showTableToggleBtn: true,
                onSuccess: function (grid) { jv.TestRole(); }
            });

            myGrid.getFlexi().p.url = "~/Master/TStandardRole/QueryUserByStanRole" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
            myGrid.getFlexi().populate();
        }); 
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table class="myGrid" style="display: none">
    </table>
</asp:Content>
