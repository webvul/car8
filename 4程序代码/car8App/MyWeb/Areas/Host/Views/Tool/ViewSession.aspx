<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Host/Views/Shared/Simple.Master"
    Theme="Admin" Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="dd" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/Canvas/excanvas.js" type="text/javascript"></script>
    <script src="/pm/Res/MyJs_Admin.js" type="text/javascript"></script>
    <style type="text/css">
        body
        {
            background-color: #f0f0f0;
            height: 100%;
        }
        
        .Center
        {
            width: 450px;
            height: 300px;
            position: absolute;
            left: 50%;
            top: 50%;
            margin-left: -225px;
            margin-top: -160px;
        }
        table
        {
            width: 100%;
            border-collapse: collapse;
        }
        table tr td, table tr th
        {
            padding: 5px;
            border: 1px solid rgb(222, 231, 215);
        }
        
        table tr th
        {
            background-color: rgb(238, 227, 205);
            font-size: 14px;
        }
        
        tr.sql
        {
            color: blue;
        }
        
        tr.oqlid
        {
            color: red;
        }
        
        button
        {
            margin: 0 10px;
            background-color: #8f0000;
            color: White;
            padding: 5px 15px;
            font-weight: bold;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table>
        <thead>
            <th>
                Session键(共<%=Session.Count %>个)
            </th>
            <th>
                Session值
            </th>
        </thead>
        <tbody>
            <% 
                
                foreach (string k in Session.Keys)
                {
                    Ronse += string.Format(@"<tr class='{0}'>
<td  class='Wrap'>{1}</td><td class='Wrap' >{2}</td></tr>",
                        "",
                        k,
                        Session[k] != null ? Session[k].ToJson().GetSafeValue() : ""
                        );
                }

            %></tbody>
    </table>
    <hr />
    <table>
    <%var coinfo = HttpContext.Current.Request.Cookies; %>
        <thead>
            <th>
                Cookie键(共<%=coinfo.Count%>个)
            </th>
            <th>
                Cookie值
            </th>
        </thead>
        <tbody>
            <% 
                
                foreach (string k in coinfo.Keys)
                {
                    Ronse += string.Format(@"<tr class='{0}'>
<td  class='Wrap'>{1}</td><td class='Wrap' >{2}</td></tr>",
                        "",
                        k,
                        coinfo[k] != null ? coinfo[k].ToJson().GetSafeValue() : ""
                        );
                }

            %></tbody>
    </table>
</asp:Content>
