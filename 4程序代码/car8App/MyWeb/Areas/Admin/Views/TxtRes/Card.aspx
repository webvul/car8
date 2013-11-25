<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.ViewGroup.VTxtResRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    TxtRes Card
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            //TextHelper 要放在最前.
            jv.SetDetail();

            //$('#txtCreateDate').datepicker();

            jv.page().Edit_OK = function (ev) {
                $.post("~/Admin/TxtRes/" + jv.page()["action"] + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx", $(".MyCard").GetDivJson("Edit_"), function (res) {
                    if (res.msg) alert(res.msg);
                    else {
                        jv.Hide(function () {
                            $(".flexigrid", jv.boxdy()).getFlexi().populate();
                        });
                        if (jv.IsInBoxy(ev)) { }
                    }
                });
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divCard">
        <div class="MyTool">
            <input class="submit" type="button" value="保存" onclick="jv.page().Edit_OK(event)" />
        </div>
        <div class="MyCard">
            <div>
                <div class="MyCardTitle">
                    基本信息(必填)
                </div><div class="coline"> </div>
                <div class="kv">
                    <span class="key">Id:</span> <span class="val">
                        <%=Html.Hidden("Edit_Id", Model.Id)%>
                        <%= Model.Id%>
                    </span>
                </div>
                <div class="kv">
                    <span class="key">语言:</span> <span class="val">
                        <%= MyHelper.Lang.GetRes()%>
                    </span>
                </div>
            </div>
            <div class="FillHeight divSplit">
                <%=  Html.Hidden("Edit_ResID",Model.ResID) %>
                <%=  Html.Hidden("Edit_Lang",MyHelper.Lang) %>
            </div>
            <div>
                <div class="MyCardTitle">
                    扩展信息(选填)
                </div><div class="coline"> </div>
                <div class="kv">
                    <span class="key">资源:</span><span class="val">
                        <%=Html.TextBox("Edit_Key", (Model.Key))%>
                    </span>
                </div>
                <div class="kv">
                    <span class="key">语言值:</span> <span class="val">
                        <%=Html.TextBox("Edit_Value", (Model.Value))%></span>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
