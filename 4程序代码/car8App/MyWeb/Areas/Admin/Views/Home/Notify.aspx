<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/cs/Views/Shared/csIndex.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    系统通知
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        h2
        {
            background-color: Gray;
            padding: 10px;
            color: White;
            margin-top: 100px;
            text-align: center;
            text-indent: -999px;
            -moz-user-select: none;
            -webkit-user-select: none;
            cursor: default;
        }
        h2 em
        {
            display: block;
            text-indent: 0;
            letter-spacing: -5px;
            font-size: 35px;
        }
        .msg
        {
            display: block;
            text-align: center;
            margin-top: 80px;
            font-size: 26px;
            line-height: 50px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divCard">
        <h2>
            <em><span>系统通知</span></em>
        </h2>
        <div class="msg">
            今天晚上 6：00 到 12：00 进行系统维护， 请相互转告。如果对此有问题，请联系运维人员：<br />
            李梦杰: 13439504367.
            <p style="text-align: right;">
                <%=DateTime.Today.ToString("yyyy年MM月dd日") %>
                <br />
                <a href="~/cs/TTaskTodo/Card.aspx">返回我的待办</a>
            </p>
        </div>
    </div>
</asp:Content>
