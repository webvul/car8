<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.ProductDetailRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    产品详情项
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            jv.SetDetail();


            jv.page().style = function () {
                if ($("#Edit_IsCaption").prop("checked")) {
                    $("#Edit_Key").closest(".kv").find(".key").html("标题值");
                    $("#Edit_Value").closest(".kv").hide();
                } else {
                    $("#Edit_Key").closest(".kv").find(".key").html("属性值");
                    $("#Edit_Value").closest(".kv").show();
                }
            };

            $("#Edit_IsCaption").bind("click", jv.page().style);

            jv.page().style();

            //$('#txtCreateDate').datepicker();
            //$("#txtStatus").TextHelper("array", "在岗,离岗");
            //$("#txtChannelManager").TextHelper("change", "~/C3_Shop/LoadStaff/?post=渠道经理", 500);
            jv.page().Edit_OK = function (ev) {

                $.post("~/Admin/ProductDetail/" + jv.page()["action"] + "/" + (jv.page()["uid"] || "0") + ".aspx", $(".MyCard").GetDivJson("Edit_"), function (res) {
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
            <div class="kv">
                <span class="key">产品：</span>
                <span class="val"><% = Model.GetProductInfo().Name  %></span>
            </div>

            <div class="kv">
                <span class="key">属性：</span>
                <span class="val"><%=Html.TextBox("Edit_Key", (Model.Key))%></span>
            </div>
            <% if (!ActionIsAdd)
               {%>
            <div class="kv">
                <span class="key">排序:</span>
                <span class="val">
                    <input type="text" id="Edit_SortID" value="<%=Model.SortID %>" />
                </span>
            </div>
            <%} %>
        </div>
        <div class="FillHeight- divSplit">
        </div>
        <div>
            <div class="kv">
                <span class="key">特性:</span>
                <span class="val">
                    <input type="checkbox" id="Edit_IsCaption" <%=Model.IsCaption? "checked='checked'": "" %> style="width: auto;" />
                    <label for="Edit_IsCaption">是否是标题</label>
                </span>
            </div>
            <div class="kv">
                <span class="key">属性值：</span>
                <span class="val"><%=Html.TextArea("Edit_Value", (Model.Value))%></span>
            </div>
        </div>
    </div>


</asp:Content>
