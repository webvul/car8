<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<MyOqlCodeGenSect.TableCollection.TableGroupCollection.TableElement>"
    Theme="Admin" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" runat="server">
    菜单编辑
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            jv.SetDetail({ detail: "AppConfigDetail" });

            $("#cols").LoadView({ url: "~/Host/Tool/AppConfigColumnList.aspx" + jv.url().search, filter: ".Main" });

            jv.page().Edit_OK = function (ev) {
                if (!jv.chk()) {
                    return;
                }
                $.post(jv.url(), $(".Main:last").GetDivJson("Edit_"), function (res) {
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
    <div class="MyTool">
        <input class="submit" type="button" value="保存" onclick="jv.page().Edit_OK(event)" />
    </div>
    <div class="MyCardTitle">
        基本信息
    </div>
    <div class="MyCard">
        <div>
            <div class="kv">
                <span class="key">Name:</span> <span class="val">
                    <%=Html.TextBox("Edit_Name", (Model.Name))%></span>
            </div>
            <div class="kv">
                <span class="key">MapName:</span> <span class="val">
                    <%=Html.TextBox("Edit_MapName", (Model.MapName))%></span>
            </div>
            <div class="kv">
                <span class="key">Owner:</span> <span class="val">
                    <%=Html.TextBox("Edit_Owner", (Model.Owner))%></span>
            </div>
        </div>
        <div class="FillHeight divSplit">
        </div>
        <div>
            <div class="kv">
                <span class="key">db:</span> <span class="val">
                    <%=Html.TextBox("Edit_db", (Model.db))%></span>
            </div>
            <div class="kv">
                <span class="key">Descr:</span> <span class="val">
                    <%=Html.TextBox("Edit_Descr", (Model.Descr))%></span>
            </div>
        </div>
    </div>
    <div class="MyCardTitle">
        数据定义
    </div>
    <div id="cols">
    </div>
    <div class="MyCardTitle">
        参数定义
    </div>
</asp:Content>
