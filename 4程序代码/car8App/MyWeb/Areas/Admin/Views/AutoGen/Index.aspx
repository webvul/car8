<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    代码生成
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .con, .con div
        {
            padding: 10px;
            margin: 10px;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            $("#Group").TextHelper({ useSelect: false, url: "~/Admin/AutoGen/QueryGroup.aspx", post: "click", change: function () { $("#Entity").val("").ClearTextHelperData(); }
            });

            $("#Entity").TextHelper({ useSelect: false, url: "~/Admin/AutoGen/QueryEnt.aspx", post: "click", quote: "radio", paras: { Group: function () { return $("input[name=Group]").val(); } } });

            $("#Ref_Style").TextHelper({ useSelect: false, data: { Ref_Csm: "Csm引用样式", Ref_Pm: "PM引用样式" }, quote: "radio", paras: { Group: function () { return $("input[name=Group]").val(); } } });
        });
        function btnOK() {
            window.location = "~/Admin/AutoGen/ListCard.aspx?Area=" + ($("[name=Group]").val() || "Admin") +
                "&Group=" + $("[name=Group]").val() +
                "&Entity=" + $("[name=Entity]").val() +
                "&Ref_Style=" + $("[name=Ref_Style]").val();
        }
        function genAll() {
            if (Confirm("生成全部，将覆盖现有文件，请谨慎！！！") == false) {
                return;
            }
            else {
                var path = "";
                if (Confirm("自定义生成路径吗？") == true) {
                    path = prompt("自定义路径");
                }
                $.post("~/Admin/AutoGen/genAll.aspx", { path: path }, function (res) {
                    //客户端返回的是Json
                    alert(res.msg);
                });
            }
        }

        function genEnt() {
            $("form").attr("action", "~/Admin/AutoGen/Entity.aspx"); $("form").submit();
        }

        function genSql() {
            $("form").attr("action", "~/Admin/AutoGen/Sql/" + $("[name=db]:checked").val() + ".aspx"); $("form").submit();
        }

    </script>
</asp:Content>
 
 
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="MyTool">
 <button type="button" onclick='btnOK() ;'>
        生成所选</button>
    </div>
    <div class="MyCard" >
    <div class="kv">
        <span class="key" title="">实体Group:</span> <span class="val">
            <input type="text" id="Group" value="" />
        </span>
    </div>
    <div class="kv">
        <span class="key" title="">实体:</span> <span class="val">
            <input type="text" id="Entity" />
        </span>
    </div>
    <div class="kv">
        <span class="key" title="">引用样式:</span> <span class="val">
            <input type="text" id="Ref_Style" />
        </span>
    </div></div>
</asp:Content>
