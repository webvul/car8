<%@ Control Language="C#" Inherits="MyCon.MyMvcVuc" %>
<%--<link href="~/Res/SendMsg/SendMsg.css" rel="stylesheet" type="text/css" />
<script src="~/Res/SendMsg/SendMsg.js" type="text/javascript"></script>
<script type="text/javascript">
    function sendMsg() {
        var msg = $(".SendMsg").GetDivJson("SendMsg_");
        if (!msg.Msg || !msg.Contract) { alert("请输入信息！"); return; }

        //if (jv.page().action == "ProductInfo") { msg.Url = document.location.href; }

        $.post("~/Shop/SendMsg/" + jv.page()["uid"] + ".aspx", msg, function (res) {
            if (res.msg) { alert(res.msg); }
            else {
                alert("消息发送成功。");
                jv.SendMsg(".SendMsg");
            }
        });
    }
</script>
<div style="right: 0pt; bottom: 0pt; padding: 0px; position: fixed; width: 245px;"
    class="SendMsg">
    <div class="Sect InfoPad  UpDiv">
        <div class="SectInner InfoPadInner UpDiv" style="padding: 3px;">
            <div class="WinIcon">
            </div>
            <%=MyHelper.Lang. GetRes("发消息给我")%>
        </div>
    </div>
    <div class="Sect Viewy DownDiv NoDiv">
        <div class="SectInner ViewyInner NoDiv" style="padding: 4px;">
            <textarea name="SendMsg_Msg" rows="6" style="width: 98%; overflow: auto;"></textarea><br />
            <%=MyHelper.Lang.GetRes("联系方式") %>
            ：<input style="width: 120px" name="SendMsg_Contract" />
            <button style="position: fixed; right: 0px; margin-right: 6px; width: 48px;" onclick="sendMsg();">
                <%=MyHelper.Lang.GetRes("发送") %>
            </button>
        </div>
    </div>
</div>--%>
