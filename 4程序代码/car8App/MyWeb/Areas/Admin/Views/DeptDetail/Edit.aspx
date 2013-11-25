<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.DeptDetailRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    DeptDetail Edit
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            jv.SetDetail();

            //$('#txtCreateDate').datepicker();
            $("#Edit_GroupKey").TextHelper({ data: { Home: "首页", Profile: "企业证书", AboutUs: "关于我们" } });
            $("#Edit_Trait").TextHelper({ data: { Default: "详细信息", Caption: "标题项", Detail: "标题值" } });
            $("#Edit_Value").PopTextArea();

            jv.page().Edit_OK = function (ev) {

                $.post("~/Admin/DeptDetail/" + jv.page()["action"] + "/" + (jv.page()["uid"] || "0") + ".aspx", $(".MyCard").GetDivJson("Edit_"), function (res) {
                    //客户端返回的是Json
                    if (res.msg) alert(res.msg);
                    jv.Hide(function () { $(".flexigrid", jv.boxdy()).getFlexi().populate(); });
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
                    <span class="key">ID:</span> <span class="val">
                        <%= (Model.Id)%>
                        <%=Html.Hidden("Edit_Id", (Model.Id))%></span>
                </div>
                <div class="kv">
                    <span class="key">分组:</span> <span class="val">
                        <%=Html.TextBox("Edit_GroupKey", (Model.GroupKey))%></span>
                </div>
                <div class="kv">
                    <span class="key">特征:</span> <span class="val">
                        <%=Html.TextBox("Edit_Trait", (Model.Trait))%></span>
                </div>

            </div>
            <div class="FillHeight divSplit">
            </div>
            <div>
                <div class="MyCardTitle">
                    语言信息
                </div><div class="coline"> </div>
                <div class="kv">
                    <span class="key">键:</span> <span class="val">
                        <%=Html.TextBox("Edit_Key", (Model.Key))%>
                    </span>
                </div>
                <div class="kv">
                    <span class="key">值:</span> <span class="val">
                        <%=Html.TextArea("Edit_Value", (Model.Value))%>
                    </span>
                </div>
                <div class="MyCardTitle">
                    其它
                </div><div class="coline"> </div>
                <div class="kv">
                    <span class="key">排序:</span> <span class="val">
                        <%=Html.TextBox("Edit_SortID", (Model.SortID))%></span>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
