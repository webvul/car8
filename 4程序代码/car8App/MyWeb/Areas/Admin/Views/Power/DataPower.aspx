<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage<MyWeb.Areas.Admin.Models.PowerModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Edit
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <input class="submit" type="button" value="保存" onclick="Edit_OK(event)" />
    <input type="button" value="全选" onclick="$('input[type=checkbox]').attr('checked',1);" />
    <input type="button" value="反选" onclick="$('input[type=checkbox]').each(function(i,d){if ($(d).attr('checked')) $(d).removeAttr('checked'); else $(d).attr('checked',1);});" />
    <div class="MyCardTitle">
        数据库-创建表</div><div class="coline"> </div>
    <div class="Create">
        <%
            if (Model.Create != null)
            {
                for (int i = 0; i < Model.Create.Count; i++)
                {
                    Ronse += string.Format(@"<span><input type=""checkbox"" id=""Edit_Create_{0}"" value=""{0}"" {2}/><label for=""Edit_Create_{0}"">{1}</label></span>",
                        Model.Create.Keys.ElementAt(i),
                        Model.Create.Values.ElementAt(i),
                        Model.Create.Keys.ElementAt(i).IsIn(Model.MyCreate) ? "checked" : ""
                       );
                }
            }
        %>
    </div>
    <div class="MyCardTitle">
        数据库-读取列</div><div class="coline"> </div>
    <div class="Read">
        <%
            if (Model.Read != null)
            {
                for (int i = 0; i < Model.Read.Count; i++)
                {
                    Ronse += string.Format(@"<span><input type=""checkbox"" id=""Edit_Read_{0}"" value=""{0}"" {2}/><label for=""Edit_Read_{0}"">{1}</label></span>",
                        Model.Read.Keys.ElementAt(i),
                        Model.Read.Values.ElementAt(i),
                        Model.Read.Keys.ElementAt(i).IsIn(Model.MyRead) ? "checked" : ""
                       );
                }
            }
        %>
    </div>
    <div class="MyCardTitle">
        数据库-更新列</div><div class="coline"> </div>
    <div class="Update">
        <%
            if (Model.Update != null)
            {
                for (int i = 0; i < Model.Update.Count; i++)
                {
                    Ronse += string.Format(@"<span><input type=""checkbox"" id=""Edit_Update_{0}"" value=""{0}"" {2}/><label for=""Edit_Update_{0}"">{1}</label></span>",
                        Model.Update.Keys.ElementAt(i),
                        Model.Update.Values.ElementAt(i),
                        Model.Update.Keys.ElementAt(i).IsIn(Model.MyUpdate) ? "checked" : ""
                       );
                }
            }
        %>
    </div>
    <div class="MyCardTitle">
        数据库-删除表</div><div class="coline"> </div>
    <div class="Delete">
        <%
            if (Model.Delete != null)
            {
                for (int i = 0; i < Model.Delete.Count; i++)
                {
                    Ronse += string.Format(@"<span><input type=""checkbox"" id=""Edit_Delete_{0}"" value=""{0}"" {2}/><label for=""Edit_Delete_{0}"">{1}</label></span>",
                        Model.Delete.Keys.ElementAt(i),
                        Model.Delete.Values.ElementAt(i),
                        Model.Delete.Keys.ElementAt(i).IsIn(Model.MyDelete) ? "checked" : ""
                       );
                }
            }
        %>
    </div>
    <div class="MyCardTitle">
        页面-页面权限</div><div class="coline"> </div>
    <div class="Action">
        <%
            if (Model.Action != null)
            {
                for (int i = 0; i < Model.Action.Count; i++)
                {
                    var mod = Model.Action;
                    Ronse += string.Format(@"<span><input type=""checkbox"" id=""Edit_Action_{0}"" value=""{0}"" {2}/><label for=""Edit_Action_{0}"">{1}</label></span>",
                        mod.Keys.ElementAt(i),
                        mod.Values.ElementAt(i),
                        mod.Keys.ElementAt(i).IsIn(Model.MyAction) ? "checked" : ""
                       );
                }
            }
        %>
    </div>
    <div class="MyCardTitle">
        页面-操作权限</div><div class="coline"> </div>
    <div class="Button">
        <%
            if (Model.Button != null)
            {
                for (int i = 0; i < Model.Button.Count; i++)
                {
                    Ronse += string.Format(@"<span><input type=""checkbox"" id=""Edit_Button_{0}"" value=""{0}"" {2}/><label for=""Edit_Button_{0}"">{1}</label></span>",
                        Model.Button.Keys.ElementAt(i),
                        Model.Button.Values.ElementAt(i),
                        Model.Button.Keys.ElementAt(i).IsIn(Model.MyButton) ? "checked" : ""
                       );
                }
            }
        %>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            jv.SetDetail({ callback: function () { $("input[type=button]", jv.boxdy()).hide(); } });

            //$('#txtCreateDate').datepicker();
            //$("#txtStatus").TextHelper("array", "在岗,离岗");
            //$("#txtChannelManager").TextHelper("change", "~/C3_Shop/LoadStaff/?post=渠道经理", 500);

        });
        function Edit_OK(ev) {
            var val = { Create: [], Read: [], Update: [], Delete: [], Action: [], Button: [] };
            var bxy = Boxy.getOne();
            $(".Create").find("input[type=checkbox]:checked").each(function (i, d) { val.Create.push($(d).val()); });
            $(".Read").find("input[type=checkbox]:checked").each(function (i, d) { val.Read.push($(d).val()); });
            $(".Update").find("input[type=checkbox]:checked").each(function (i, d) { val.Update.push($(d).val()); });
            $(".Delete").find("input[type=checkbox]:checked").each(function (i, d) { val.Delete.push($(d).val()); });
            $(".Action").find("input[type=checkbox]:checked").each(function (i, d) { val.Action.push($(d).val()); });
            $(".Button").find("input[type=checkbox]:checked").each(function (i, d) { val.Button.push($(d).val()); });
            bxy.ReturnValue = val;
            bxy.hide();
        }
         
    </script>
    <style type="text/css">
        .Create span, .Read span, .Update span, .Delete span, .Action span, .Button span
        {
            margin: 5px;
        }
    </style>
</asp:Content>
