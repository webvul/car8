<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Edit
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--        <div class="MyTool">
            <input class="submit" type="button" value="保存" onclick="Edit_OK(event)" /></div>
    --%>
    <div class="FillHeight">
        <ul class="tabs">
            <li onclick="jv.page().Tab('Table',event);return true ;"><a>表</a></li>
            <li onclick="jv.page().Tab('View' ,event);return true ;"><a>视图</a></li>
            <li onclick="jv.page().Tab('Proc',event);return true ;"><a>存储过程</a></li>
        </ul>
        <div id="Con" class="FillHeight">
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/flowplayer/tabs.js" type="text/javascript"></script>
    <link href="~/Res/flowplayer/tabs.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(function () {
            if (jv.boxdy()[0] != document) {
                //弹出情况.


                //                $($.makeArray(pobjs).reverse()).each(function (i, d) {
                //                    $(d).height(conHeight - $(d).offset().top);
                //                });
            }

            jv.page().Tab = function (tab, ev) {

                //                $(jv.GetDoer()).attr("href", "#" + tab);

                $("#Con").LoadView(
                { url: "~/Admin/AutoGen/Entity_{0}.aspx"
                        .format(tab)

                });

            };
            //            jv.page().tabs = $('.Tabs').tabs({fx:true});
            //            $(".Tabs >ul>li:eq(2)>a").trigger("click");
            var myTabs = $("ul.tabs").tabs(null, { api: true });

            myTabs.getCurrentTab().trigger("click");

            jv.SetDetail({ callback: function () { $("input[type=button]", jv.boxdy()).hide(); } });

            //$('#txtCreateDate').datepicker();
            //$("#txtStatus").TextHelper("array", "在岗,离岗");
            //$("#txtChannelManager").TextHelper("change", "~/C3_Shop/LoadStaff/?post=渠道经理", 500);

        });
      
    </script>
    <style type="text/css">
        .Create span, .Read span, .Update span, .Delete span, .Action span, .Button span
        {
            margin: 5px;
        }
    </style>
</asp:Content>
