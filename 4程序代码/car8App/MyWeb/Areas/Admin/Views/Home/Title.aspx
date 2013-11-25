<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Main.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    和易家
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            if ($.browser.msie && $.browser.version < 7) {
                top.window.location = "~/ie6Error.html";
            }
            toHelp = function () {
                top.main.window.location = "~/Shop/Index/<%=MySessionKey.WebName.Get()%>.aspx";
            };

        });
    </script>
    <div id="header" style="position: relative">
        <div style="font-size: 14pt; padding: 0 10px 0 10px; font-weight: bold; color: #EDF8FD;"
            class="Inline">
            和易家 －
            <% =MySession.Get(MySessionKey.DeptName).HasValue() ? MySession.Get(MySessionKey.DeptName) : "()"%>
            商铺管理系统
        </div>
        <div class="btnInMenu Inline"><a target="main" href="~/Shop/Index/<%=MySessionKey.WebName.Get()%>.aspx">预览</a></div>
        <div style="position: absolute; right: 20px; top: 0px; font-size: 14px; font-weight: bold; font-family: 微软雅黑">
            <div class="BigBtnBorder Inline">
                <div class="BigBtn" onclick="top.location='<%=ConfigKey.HyjUrl.Get() %>';">
                    和易家
                </div>
            </div>
            <div style="margin-left: 30px;" class="Inline">
                你好，<%=MySessionKey.UserName.Get() %>
            </div>
            <div class="btnInMenu Inline" onclick="jv.postOpen('~/Host/Home/Logout.aspx', {}, { self: true, callback: function (res) { top.window.location ='~/Login.aspx'; } });"><a>退出</a> </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="~/Css/Title.css" />
    <style type="text/css">
        #limrq ul {
            position: relative;
        }

        .BigBtn {
            font-size: 16px;
            color: white;
            font-weight: bold;
            font-family: 微软雅黑;
            line-height: 40px;
            vertical-align: middle;
            background: url(~/Css/images/BigBtn.jpg);
            border: solid 1px #044468;
            border-width: 0 1px;
            padding: 0 15px;
            cursor: pointer;
        }

        .BigBtnBorder {
            border: solid 1px #3c96c8;
            border-width: 0 1px;
        }

        .btnInMenu {
            padding: 3px 10px;
            background-color: #2A6A90;
            margin-left: 10px;
            cursor: pointer;
            height: 25px;
            line-height: 25px;
        }

            .btnInMenu a {
                color: White;
                font-size: 16px;
                font-family: 微软雅黑;
            }
    </style>
</asp:Content>
