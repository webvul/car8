<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.RoleRule.Entity>" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" runat="server">
    Role Edit
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            //TextHelper 要放在最前.
            //$("#txtStatus").TextHelper("array", "在岗,离岗");
            jv.SetDetail();

            //$('#txtCreateDate').datepicker();

            jv.page().flexiEdit_Power = function (id) {
                
                Boxy.load("~/Admin/Power/Update.aspx?Type=Role&Value=" + id, { filter: ".MyCard", modal: true, title: "修改" }, function (bxy) {
                    bxy.resize(880, 2000);

                    //Edit页执行保存代码
                }
                );
            };

            jv.page().Edit_OK = function (ev) {
                $.post("~/Admin/Role/" + jv.page()["action"] + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx", $(".MyCard").GetDivJson("Edit_"), function (res) {
                    //客户端返回的是Json 。 res = $.evalJSON(res);
                    if (jv.IsInBoxy(ev)) {
                        if (res.msg) alert(res.msg);
                        else {
                            jv.Hide(function () { $(".flexigrid", jv.boxdy()).getFlexi().populate(); });
                        }
                    }
                    else {
                        if (res.msg) alert(res.msg);
                        else alert("保存成功");
                    }
                });
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
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
                    <div>
                        <span class="key">ID:</span> <span class="val hard">
                            <%= (Model.Id)%><%=Html.Hidden("Edit_Id", (Model.Id))%></span>
                    </div>
                </div>
                <div class="kv">
                    <div>
                        <span class="key">Role:</span> <span class="val">
                            <%=Html.TextBox("Edit_Role", (Model.Role))%></span>
                    </div>
                </div>
            </div>
            <div class="FillHeight divSplit">
            </div>
            <div>
                <div class="MyCardTitle">
                    扩展信息(选填)
                </div><div class="coline"> </div>
                <div class="kv">
                    <div>
                        <span class="key Link" onclick="jv.page().flexiEdit_Power(<%=Model.Id %>) ;">Power:</span>
                        <span class="val">
                            <%=Html.TextBox("Edit_Power", (Model.Power))%></span>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
