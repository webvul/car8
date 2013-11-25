<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage<Microsoft.AnalysisServices.AdomdClient.CellSet>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Dict列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyUploader/swfobject.js" type="text/javascript"></script>
    <script src="~/Res/MyUploader/jquery.uploadify.js" type="text/javascript"></script>
    <script src="~/Res/FlexiGrid/flexigrid.js" type="text/javascript"></script>
    <script src="~/Res/Boxy/boxy.js" type="text/javascript"></script>
    <script src="~/Res/TextHelper/TextHelper.js" type="text/javascript"></script>
    <link href="~/Res/TextHelper/TextHelper.css" rel="stylesheet" type="text/css" />
    <link href="~/Res/MyUploader/uploadify.css" rel="stylesheet" type="text/css" />
    <link href="~/Res/FlexiGrid/flexigrid.css" rel="stylesheet" type="text/css" />
    <link href="~/Res/Boxy/boxy.css" rel="stylesheet" type="text/css" />
    <script src="~/Res/DatePicker/jquery-ui.custom.js" type="text/javascript"></script>
    <link href="~/Res/DatePicker/jquery-ui.custom.css" rel="stylesheet" type="text/css" />
    <script src="~/Res/MyValidate/MyValidate.js" type="text/javascript"></script>
    <link href="~/Res/MyValidate/MyValidate.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <div id="divQuery" style="display: none">
            <div class="input">
                查询条件一:<input type="text" id="List_Code" style="width: 120px;" />
                查询条件二:<input type="text" id="List_Id" style="width: 120px;" /><br />
                查询条件三:<input type="text" id="List_Name" style="width: 120px;" />
                查询条件四:<input type="text" id="List_OtherName" style="width: 120px;" /><br />
            </div>
            <br />
            <input class="submit" type="button" value="查询" />
        </div>
        <%
            Ronse += "<table>";


            if (Model.Axes[1].Positions.Count > 0)
            {
                //头.
                Ronse += "<tr>";
                for (int i = 0; i < Model.Axes[1].Set.Tuples[0].Members.Count; i++)
                {
                    Ronse += "<td>.</td>";
                }
                for (int i = 0; i < Model.Axes[0].Positions.Count; i++)
                {
                    Ronse += "<td>" + Model.Axes[0].Positions[i].Members[0].Caption + "</td>";
                }
                Ronse += "</tr>";


                //体.
                for (int t = 0; t < Model.Cells.Count / Model.Axes[0].Positions.Count; t++)
                {
                    Ronse += "<tr>";
                    for (int i = 0; i < Model.Axes[1].Set.Tuples[0].Members.Count; i++)
                    {
                        Ronse += "<td>" + Model.Axes[1].Set.Tuples[t].Members[i].Caption + "</td>";
                    }

                    for (int i = 0; i < Model.Axes[0].Positions.Count; i++)
                    {
                        Ronse += "<td>" + Model.Cells[t * Model.Axes[0].Positions.Count + i].Value.AsString() + "</td>";

                    }
                    Ronse += "</tr>";
                }

            }

            Ronse += "</table>";
         
        %>
        <table id="myGrid" style="display: none">
        </table>
    </div>
</asp:Content>
