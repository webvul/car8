<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="db.aspx.cs" Inherits="MyWeb.db" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
    input,body
    {
        margin:6px;
        padding:6px;
    }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="lab" runat="server" Text="Label">数据库字符串</asp:Label>
        :<br /><br />
        <asp:TextBox ID="txtContent" runat="server" TextMode="MultiLine" Height="101px" Width="443px"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="btnEncrypte" runat="server" Text="加 密" OnClick="btnEncrypte_Click" />
        &nbsp;&nbsp;
        <asp:Button ID="btyDecrypte" runat="server" Text="解 密" 
            onclick="btyDecrypte_Click" />
        <br />
        <br />
        <asp:TextBox ID="txtResult" runat="server" TextMode="MultiLine" Height="101px" Width="443px"></asp:TextBox>
    </div>
    </form>
</body>
</html>
