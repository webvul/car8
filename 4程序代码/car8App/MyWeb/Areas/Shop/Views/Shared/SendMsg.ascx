<%@ Control Language="C#" Inherits="MyMvcVuc<DeptRule.Entity>" %>
<%--<div class="Sect NullLine">
    <div class="SectInner SendMsgDiv">
        <div style="font-size: 18px;">
            <%=GetRes("给我消息") %>
        </div>
        <div class="SendMsgTxtDiv">
            <textarea id="txtMsg" style="width: 100%; height: 120px;"></textarea>
            <div class="TipTxt">
                <%=GetRes("键入20到250个汉字") %>
            </div>
            <div style="width: 100%; text-align: center;">
                <br />
                <script type="text/javascript">
                    function send_click() {
                        $.post(jv.Root + "Shop/SendMsg/" + jv.page().uid + ".aspx"
                        , { Msg: $("#txtMsg").val() }
                        , function (res) {
                            if (res.msg) return alert(res.msg);
                            $("#txtMsg").val("");
                            $("#aj_res").text("发送成功");

                        }
		                , "json");
                    }
                </script>
                <span id="aj_res" style="float: right; color: #363636; font-size: 12px;"></span>
                <button type="button" class="buttonSkinA" style="width: 85px;" onclick="send_click();">
                    <%=GetRes("给我消息") %>
                </button>
            </div>
        </div>
        <div class="SendMsgFlash">
            <%=Html.BeginTag(HtmlTextWriterTag.Img,new{
                        @class="myImg",
                        alt=Model.Name,
                        src= (Model.GetPersons() != null && Model.GetPersons().Length > 0 && Model.GetPersons()[0].GetAnnex() != null ? 
                        Model.GetPersons()[0].GetAnnex().FullName :
                        "")
                        }) 
            %>
            <div style="color: #2e3d52; font-size: 13px; font-weight: bold;">
                <%=    Model.GetPersons() != null && Model.GetPersons().Length > 0  ? Model.GetPersons()[0].Name :"" %>
            </div>
        </div>
    </div>
</div>--%>
