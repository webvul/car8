<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.PowerColumnRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    列元数据 信息
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            jv.SetDetail();

            jv.page().Edit_OK = function (ev) {
                $.post("~/Admin/PowerColumn/" + jv.page()["action"] + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx", $(".Main", jv.boxdy()).GetDivJson("Edit_"), function (res) {
                    if (res.msg) alert(res.msg);
                    else {
                        jv.Hide(function () {
                            $(".flexigrid", jv.boxdy()).getFlexi().populate();
                        });
                        alert("保存成功");
                        if (jv.IsInBoxy(ev)) { }
                    }
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
            <div class="kv">
                <span class="key">Id:</span> <span class="val hard"><span>
                    <%= (Model.Id)%>
                </span>
                    <%=Html.Hidden("Edit_Id", Model.Id)%></span>
            </div>
            <% dbr.PowerTable.PopRadior("Edit_TableID",new UIPop
                   {
                       KeyTitle = "表ID",
                       Value = Model.TableID.HasValue() ? Model.TableID.AsString() : "",
                       Display = Model.TableID.HasValue() ? Model.GetPowerTable().Table.AsString() : "(空)"
                   }
                ).GenDiv(); %>
            <div class="kv">
                <span class="key">列名:</span> <span class="val">
                    <%=Html.TextBox("Edit_Column", (Model.Column))%></span>
            </div>
            <div class="kv">
                <span class="key">描述:</span> <span class="val">
                    <%=Html.TextBox("Edit_Descr", (Model.Descr))%></span>
            </div>
        </div>
        <div class="FillHeight- divSplit">
        </div>
        <div>
            <div class="MyCardTitle">
                扩展信息
            </div><div class="coline"> </div>
            <div class="kv">
                <span class="key">手动分布成两列: </span><span class="val">!!</span>
            </div>
        </div>
    </div>
</asp:Content>
