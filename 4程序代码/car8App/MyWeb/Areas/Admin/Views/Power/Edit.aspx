<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Main.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    权限
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ul class="tabs">
        <li onclick="jv.page().Tab('Power','PagePower',event);return true ;"><a>页面操作权限</a></li>
        <li onclick="jv.page().Tab('Menu', 'ListPower' ,event);return true ;"><a>菜单权限</a></li>
        <%--       <li class=".controller" onclick="jv.page().Tab('PowerController', 'ListPower' ,event);return true ;">
            <a>模块权限</a></li>--%>
        <%
                
            if (Request.QueryString["Data"].HasValue())
            {
        %>
        <li onclick="jv.page().Tab('Power','DataCreatePower',event);return true ;"><a>数据创建权限</a></li>
        <li onclick="jv.page().Tab('Power','DataDeletePower',event);return true ;"><a>数据删除权限</a></li>
        <li onclick="jv.page().Tab('Power','DataReadPower',event);return true ;"><a>数据读取权限</a></li>
        <li onclick="jv.page().Tab('Power','DataUpdatePower',event);return true ;"><a>数据更新权限</a></li>
        <%} %>
    </ul>
    <!--[if IE]><div style="height:2px"></div><![endif]-->
    <div id="Con" class="FillHeight" style="min-height: 300px; background-color: White;">
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/flowplayer/tabs.js" type="text/javascript"></script>
    <link href="~/Res/flowplayer/tabs.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(function () {
            jv.page().Tab = function (controller, tab, ev) {

                $("#Con").LoadView(
                {
                    url: "~/Admin/{0}/{1}/{2}.aspx?Type={3}&Value={4}&Mode={5}&_Ref_Type_=true"
                          .format(
                          controller,
                          tab,
                          jv.page().action,
                          jv.page().Type,
                          (jv.page().Value || ""),
                          jv.page().Mode || ""),
                    callback: function () {
                        var boxy = Boxy.getOne();
                        if (boxy) {
                            boxy.resize().center();
                        }
                    },
                    filter: ".Main"
                });

                return false;
            };
            var myTabs = $("ul.tabs").tabs(null, { api: true });

            //谷歌浏览器有错误, 传递的Event 不对.
            myTabs.getCurrentTab().trigger("click", { originalEvent: true, target: myTabs.getCurrentTab()[0] });

            jv.SetDetail({ callback: function () { $("input[type=button]", jv.boxdy()).hide(); } });

        });

    </script>
    <style type="text/css">
        .Create span, .Read span, .Update span, .Delete span, .Action span, .Button span {
            margin: 5px;
        }
    </style>
</asp:Content>
