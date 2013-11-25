<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.ProductTypeRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    产品类型
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            //TextHelper 要放在最前.
            //$("#txtStatus").TextHelper("array", "在岗,离岗");
            jv.SetDetail();

            //$('#txtCreateDate').datepicker();

            jv.page().Edit_OK = function (ev) {
                $.post("~/Admin/ProductType/" + jv.page()["action"] + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx", $(".MyCard").GetDivJson("Edit_"), function (res) {
                    //客户端返回的是Json 。 res = $.evalJSON(res);

                    if (res.msg) alert(res.msg);
                    else {
                        jv.Hide(function () { $(".myGrid:last").getFlexi().populate(); });
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
                基本信息(必填)
            </div><div class="coline"> </div>
            <div class="kv">
                <span class="key">ID:</span> <span class="val hard">
                    <%= (Model.Id)%><%=Html.Hidden("Edit_Id", (Model.Id))%></span>
            </div>
            <div style="display:none">
            <%= dbr.ProductType.PopRadior("Edit_Pid",
            new UIPop(){ KeyTitle = "父节点",area = "Admin",
                    Value =  Model.Pid.AsString() , 
                    Display =  Model.Pid > 0 ?  dbr.ProductType.FindScalar(o=> o.Name ,o=>o.Id ==Model.Pid).AsString() : ""
            }).GenDiv()
            %>
                </div>
            <div class="kv">
                <span class="key">名称:</span> <span class="val">
                    <%=Html.TextBox("Edit_Name", (Model.Name))%></span>
            </div>
            <div class="kv">
                <span class="key">描述:</span> <span class="val">
                    <%=Html.TextBox("Edit_Descr", (Model.Descr))%></span>
            </div>
        </div>
        <div class="FillHeight divSplit">
        </div>
        <div>
            <div class="MyCardTitle">
                扩展信息
            </div><div class="coline"> </div>
            <div class="kv">
                <span class="key">排序号:</span> <span class="val">
                    <%=Html.TextBox("Edit_SortID", (Model.SortID))%></span>
            </div>
            <div class="kv">
                <span class="key">创建用户:</span> <span class="val hard"><span>
                    <%= Model.UserID %>&nbsp;</span> </span>
            </div>
            <div class="kv">
                <span class="key">创建时间:</span> <span class="val hard"><span>
                    <%= Model.AddTime %></span></span>
            </div>
        </div>
    </div>
</asp:Content>
