<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.DictRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    字典项
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">

        $(function () {
            jv.SetDetail();

            //$('#txtCreateDate').datepicker();
            $("#Edit_Key").TextHelper({ post: "click", quote: "check", url: "~/Admin/Dict/GetKeys.aspx", display: "Sys", width: 350 });
            //$("#txtChannelManager").TextHelper("change", "~/C3_Shop/LoadStaff/?post=渠道经理", 500);
            $("textarea").PopTextArea();
            jv.page().Edit_OK = function (ev) {
                if (jv.chk() == false) return;

                $.post("~/Admin/Dict/" + jv.page()["action"] + "/" + (jv.page()["uid"] || "0") + ".aspx", $(".MyCard").GetDivJson("Edit_"), function (res) {
                    //客户端返回的是Json
                    if (res.msg) alert(res.msg);
                    jv.Hide(function () { $(".flexigrid", jv.boxdy()).getFlexi().populate(); });
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
            <div class="kv">
                <span class="key">Id:</span> <span class="val">
                    <%=  ActionIsAdd ? "保存后生成" : Model.Id.AsString()  %>
                    <%=Html.Hidden("Edit_Id", (Model.Id))%></span>
            </div>
            <div class="kv">
                <span class="key" title="表示键值的组">所属组:</span> <span class="val">
                    <%=Html.TextBox("Edit_Group", (Model.Group))%></span>
            </div>
            <div class="kv">
                <span class="key">键:</span> <span class="val">
                    <%=Html.TextBox("Edit_Key", (Model.Key), new { chk="{1}"})
                    %></span>
            </div>
            <div class="kv">
                <span class="key">值:</span> <span class="val">
                    <%=Html.TextArea("Edit_Value", (Model.Value), new { chk = "{1}" })%></span>
            </div>
        </div>
        <div class="FillHeight divSplit">
        </div>
        <div>
            <div class="kv">
                <span class="key">数据类型:</span> <span class="val">
                    <%=Html.RegisteEnum("Edit_Trait", DictTraitEnum.String).Radior().Input() %>
                </span>
            </div>
            <div class="kv">
                <span class="key">商户:</span> <span class="val">
                    <input type="hidden" id="Edit_DeptID" value="<%=ActionIsAdd ? MySessionKey.DeptID.Get() : Model.DeptID.AsString() %>" />

                    <%=MySessionKey.DeptName.Get() %>
                </span>
            </div>
            <div class="kv">
                <span class="key">排序:</span> <span class="val">
                    <%=Html.TextBox("Edit_SortID", (Model.SortID))%></span>
            </div>
        </div>
    </div>
</asp:Content>
