<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.EnterpriseShowCaseRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    EnterpriseShowCase Edit
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            jv.SetDetail();

            //$('#txtCreateDate').datepicker();
            //$("#txtStatus").TextHelper("array", "在岗,离岗");
            //$("#txtChannelManager").TextHelper("change", "~/C3_Shop/LoadStaff/?post=渠道经理", 500);


            jv.page().Edit_OK = function (ev) {

                $.post("~/Admin/EnterpriseShowCase/" + jv.page()["action"] + "/" + (jv.page()["uid"] || "0") + ".aspx", $(".MyCard").GetDivJson("Edit_"), function (res) {
                    //客户端返回的是Json
                    if (res.msg) alert(res.msg);
                    jv.Hide(function () { $(".myGrid:last").getFlexi().populate(); });
                });
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="MyTool">
        <input class="submit" type="button" value="保存" onclick="jv.page().Edit_OK(event)" />
    </div>
    <div class="MyCard">
        <div>
            <div class="MyCardTitle">
                基本信息
            </div><div class="coline"> </div>
            <%=dbr.ProductInfo.PopRadior("Edit_ProductID", new UIPop()
{
    KeyTitle = "商品",
    area = "Admin",
    Value = Model.ProductID.AsString(),
    Display = Model.ProductID > 0 ? dbr.ProductInfo.FindById(Model.ProductID).Name + "&nbsp;" : "&nbsp;"
}).GenDiv() %>
            <div class="kv">
                <span class="key">商户:</span> <span class="val">
                    <%
                        if (Model.DeptID > 0)
                        {
                            Ronse += dbr.Dept.FindById(Model.DeptID).Name;
                        }
                        else
                        {
                            Ronse += "&nbsp;";
                        }
                    %></span>
            </div>
        </div>
        <div class="FillHeight divSplit">
        </div>
        <div>
            <div class="MyCardTitle">
                其它
            </div><div class="coline"> </div>
            <div class="kv">
                <span class="key">BeginTime:</span> <span class="val">
                    <%=Html.TextBox("Edit_BeginTime", (Model.BeginTime))%></span>
            </div>
            <div class="kv">
                <span class="key">EndTime:</span> <span class="val">
                    <%=Html.TextBox("Edit_EndTime", (Model.EndTime))%></span>
            </div>
            <div class="kv">
                <span class="key">SortID:</span> <span class="val">
                    <%=Html.TextBox("Edit_SortID", (Model.SortID))%></span>
            </div>
        </div>
    </div>
</asp:Content>
