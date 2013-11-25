<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Host/Views/Shared/Simple.Master"
    Inherits="MyCon.MyMvcPage" Theme="Host" %>

<asp:Content ID="dd" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            if ($.browser.msie && $.browser.version < 7) {
                top.window.location = "~/ie6Error.html";
            }

            $("#btnOK").OneClick();

            $(":text,:password").keyup(function (ev) {
                if (ev.keyCode == 13) {
                    $("#btnOK").click();
                }
            });
        });


        function btnOK_Click() {

            //$("form").submit();
            //var jj = $(".divEdit:last").find(":text,:selected,textarea,:hidden,:checked,:password");
            var json = $(document.body).GetDivJson("txt_");

            if (!json.UserName) return;

            json.CommId = jv.page().uid;
            $.post("~/Host/Home/Prove.aspx", json, function (res) {
                if (res.msg) alert(res.msg);
                else { top.window.location = "~/Admin/Home/Index.aspx"; }
            }).error(function (res) { if (res && res.msg) alert(res.msg); else alert(res); });
        }

        function btnExit_Click() {
            top.window.location = "<%=ConfigKey.HyjUrl.Get() %>";
        }
    </script>
    <style type="text/css">
        body {
            padding: 0;
            filter: progid:DXImageTransform.Microsoft.gradient(gradientType=0,startColorstr='#0e8775', endColorstr='#1dadac');
            background: -webkit-gradient(linear,0 0,0 100%, from(#0e8775), to(#1dadac) );
        }

        .LoginTitle {
            padding-top: 90px;
            color: #d1f8fe;
            font-size: 16px;
            text-align: center;
            width: 247px;
            height: 77px;
            position: absolute;
            left: 50%;
            top: 50%;
            margin-left: -124px;
            margin-top: -280px;
            background: url(~/Areas/Host/Res/img/login_Title.png) no-repeat;
        }

        .Sect {
            width: 516px;
            height: 291px;
            position: absolute;
            left: 50%;
            top: 50%;
            margin-left: -255px;
            margin-top: -160px;
            background: url(~/Areas/Host/Res/img/login_bg.png) no-repeat;
            border-width: 0;
        }

        .key {
            color: #5a656b;
            font-weight: bold;
            font-size: 16px;
            vertical-align: middle;
            line-height: 40px;
        }

        .val {
            line-height: 40px;
        }

            .val input {
                vertical-align: middle;
                font-weight: bold;
                font-size: 16px;
            }

        #btnOK {
            background-color: #2EA1EE;
            background-image: -webkit-gradient(linear, left top, left bottom, color-stop(0%, #2EA1EE), color-stop(50%, #2EA1EE), color-stop(50%, #0041B5), color-stop(100%, #004289));
            background-image: -webkit-linear-gradient(top, #2EA1EE 0%, #2EA1EE 50%, #006DB5 50%, #004289 100%);
            background-image: -moz-linear-gradient(top, #2EA1EE 0%, #2EA1EE 50%, #006DB5 50%, #004289 100%);
            background-image: -ms-linear-gradient(top, #2EA1EE 0%, #2EA1EE 50%, #006DB5 50%, #004289 100%);
            background-image: -o-linear-gradient(top, #2EA1EE 0%, #2EA1EE 50%, #006DB5 50%, #004289 100%);
            background-image: linear-gradient(top, #2EA1EE 0%, #2EA1EE 50%, #006DB5 50%, #004289 100%);
            border: 1px solid #000095;
            -webkit-border-radius: 5px;
            -moz-border-radius: 5px;
            -ms-border-radius: 5px;
            -o-border-radius: 5px;
            border-radius: 5px;
            -webkit-box-shadow: inset 0px 0px 0px 1px rgba(100, 112, 255, 0.4), 0 1px 3px #333333;
            -moz-box-shadow: inset 0px 0px 0px 1px rgba(100, 112, 255, 0.4), 0 1px 3px #333333;
            box-shadow: inset 0px 0px 0px 1px rgba(100, 112, 255, 0.4), 0 1px 3px #333333;
            color: #fff;
            padding: 5px 20px;
            text-align: center;
            text-shadow: 0px -1px 1px rgba(0, 0, 0, 0.8);
            font-size: 16px;
        }

            #btnOK:hover {
                background-color: #7387F3;
                background-image: -webkit-gradient(linear, left top, left bottom, color-stop(0%, #7387F3), color-stop(50%, #4D69DB), color-stop(50%, #0020CB), color-stop(100%, #011BA2));
                background-image: -webkit-linear-gradient(top, #7387F3 0%, #4D69DB 50%, #0020CB 50%, #011BA2 100%);
                background-image: -moz-linear-gradient(top, #7387F3 0%, #4D69DB 50%, #0020CB 50%, #011BA2 100%);
                background-image: -ms-linear-gradient(top, #7387F3 0%, #4D69DB 50%, #0020CB 50%, #011BA2 100%);
                background-image: -o-linear-gradient(top, #7387F3 0%, #4D69DB 50%, #0020CB 50%, #011BA2 100%);
                background-image: linear-gradient(top, #7387F3 0%, #4D69DB 50%, #0020CB 50%, #011BA2 100%);
                cursor: pointer;
            }

            #btnOK:active {
                background-color: #2851D4;
                background-image: -webkit-gradient(linear, left top, left bottom, color-stop(0%, #2851D4), color-stop(50%, #242FAD), color-stop(50%, #0C009C), color-stop(100%, #090070));
                background-image: -webkit-linear-gradient(top, #2851D4 0%, #242FAD 50%, #0C009C 50%, #090070 100%);
                background-image: -moz-linear-gradient(top, #2851D4 0%, #242FAD 50%, #0C009C 50%, #090070 100%);
                background-image: -ms-linear-gradient(top, #2851D4 0%, #242FAD 50%, #0C009C 50%, #090070 100%);
                background-image: -o-linear-gradient(top, #2851D4 0%, #242FAD 50%, #0C009C 50%, #090070 100%);
                background-image: linear-gradient(top, #2851D4 0%, #242FAD 50%, #0C009C 50%, #090070 100%);
                -webkit-box-shadow: inset 0px 0px 0px 1px rgba(100, 100, 255, 0.4);
                -moz-box-shadow: inset 0px 0px 0px 1px rgba(100, 100, 255, 0.4);
                box-shadow: inset 0px 0px 0px 1px rgba(100, 100, 255, 0.4);
            }

        #btnExit {
            background-color: #DADADA;
            background-image: -webkit-gradient(linear, left top, left bottom, color-stop(0%, #DADADA), color-stop(50%, #D2D3D5), color-stop(50%, #BEBDBD), color-stop(100%, #919191));
            background-image: -webkit-linear-gradient(top, #DADADA 0%, #D2D3D5 50%, #BEBDBD 50%, #919191 100%);
            background-image: -moz-linear-gradient(top, #DADADA 0%, #D2D3D5 50%, #BEBDBD 50%, #919191 100%);
            background-image: -ms-linear-gradient(top, #DADADA 0%, #D2D3D5 50%, #BEBDBD 50%, #919191 100%);
            background-image: -o-linear-gradient(top, #DADADA 0%, #D2D3D5 50%, #BEBDBD 50%, #919191 100%);
            background-image: linear-gradient(top, #DADADA 0%, #D2D3D5 50%, #BEBDBD 50%, #919191 100%);
            border: 1px solid #3F3F3F;
            -webkit-border-radius: 5px;
            -moz-border-radius: 5px;
            -ms-border-radius: 5px;
            -o-border-radius: 5px;
            border-radius: 5px;
            -webkit-box-shadow: inset 0px 0px 0px 1px rgba(255, 255, 255, 0.4), 0 1px 3px #333333;
            -moz-box-shadow: inset 0px 0px 0px 1px rgba(100, 112, 255, 0.4), 0 1px 3px #333333;
            box-shadow: inset 0px 0px 0px 1px rgba(255, 255, 255, 0.4), 0 1px 3px #333333;
            color: #000;
            padding: 5px 20px;
            text-align: center;
            text-shadow: 0px -1px 1px rgba(255, 255, 255, 0.8);
            font-size: 16px;
        }

            #btnExit:hover {
                background-color: #CECECE;
                background-image: -webkit-gradient(linear, left top, left bottom, color-stop(0%, #CECECE), color-stop(50%, #D8D8D8), color-stop(50%, #C5C5C5), color-stop(100%, #ACACAC));
                background-image: -webkit-linear-gradient(top, #CECECE 0%, #D8D8D8 50%, #C5C5C5 50%, #ACACAC 100%);
                background-image: -moz-linear-gradient(top, #CECECE 0%, #D8D8D8 50%, #C5C5C5 50%, #ACACAC 100%);
                background-image: -ms-linear-gradient(top, #CECECE 0%, #D8D8D8 50%, #C5C5C5 50%, #ACACAC 100%);
                background-image: -o-linear-gradient(top, #CECECE 0%, #D8D8D8 50%, #C5C5C5 50%, #ACACAC 100%);
                background-image: linear-gradient(top, #CECECE 0%, #D8D8D8 50%, #C5C5C5 50%, #ACACAC 100%);
                cursor: pointer;
            }

            #btnExit:active {
                background-color: #DADADA;
                background-image: -webkit-gradient(linear, left top, left bottom, color-stop(0%, #DADADA), color-stop(50%, #B3B3B3), color-stop(50%, #A0A0A0), color-stop(100%, #777));
                background-image: -webkit-linear-gradient(top, #DADADA 0%, #B3B3B3 50%, #A0A0A0 50%, #777 100%);
                background-image: -moz-linear-gradient(top, #DADADA 0%, #B3B3B3 50%, #A0A0A0 50%, #777 100%);
                background-image: -ms-linear-gradient(top, #DADADA 0%, #B3B3B3 50%, #A0A0A0 50%, #777 100%);
                background-image: -o-linear-gradient(top, #DADADA 0%, #B3B3B3 50%, #A0A0A0 50%, #777 100%);
                background-image: linear-gradient(top, #DADADA 0%, #B3B3B3 50%, #A0A0A0 50%, #777 100%);
                -webkit-box-shadow: inset 0px 0px 0px 1px rgba(255, 255, 255, 0.4);
                -moz-box-shadow: inset 0px 0px 0px 1px rgba(255, 255, 255, 0.4);
                box-shadow: inset 0px 0px 0px 1px rgba(255, 255, 255, 0.4);
            }

        input[type=button][disabled], input[type=button][disabled]:active, input[type=button][disabled]:hover {
            background-color: #DADADA !important;
            background-image: -webkit-gradient(linear, left top, left bottom, color-stop(0%, #DADADA), color-stop(50%, #D2D3D5), color-stop(50%, #BEBDBD), color-stop(100%, #919191)) !important;
            background-image: -webkit-linear-gradient(top, #DADADA 0%, #D2D3D5 50%, #D2D3D5 50%, #919191 100%) !important;
            background-image: -moz-linear-gradient(top, #DADADA 0%, #D2D3D5 50%, #D2D3D5 50%, #919191 100%) !important;
            background-image: -ms-linear-gradient(top, #DADADA 0%, #D2D3D5 50%, #D2D3D5 50%, #919191 100%) !important;
            background-image: -o-linear-gradient(top, #DADADA 0%, #D2D3D5 50%, #D2D3D5 50%, #919191 100%) !important;
            background-image: linear-gradient(top, #DADADA 0%, #D2D3D5 50%, #D2D3D5 50%, #919191 100%) !important;
            border: 1px solid #3F3F3F !important;
            -webkit-border-radius: 5px !important;
            -moz-border-radius: 5px !important;
            -ms-border-radius: 5px !important; 
            -o-border-radius: 5px !important;
            border-radius: 5px !important;
            -webkit-box-shadow: inset 0px 0px 0px 1px rgba(255, 255, 255, 0.4), 0 1px 3px #333333 !important;
            -moz-box-shadow: inset 0px 0px 0px 1px rgba(100, 112, 255, 0.4), 0 1px 3px #333333 !important;
            box-shadow: inset 0px 0px 0px 1px rgba(255, 255, 255, 0.4), 0 1px 3px #333333 !important;
            color: gray !important;
            padding: 5px 20px !important;
            text-align: center !important;
            text-shadow: 0px -1px 1px rgba(255, 255, 255, 0.8) !important;
            font-size: 16px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="LoginTitle">
        您的最佳社区服务平台
    </div>
    <div class="Sect Center ">
        <div style="text-align: center; margin-top: 50px;">
            <span class="key">用户名 ：</span><span class="val">
                <input type="text" name="txt_UserName" value="" style="ime-mode: disabled" />
            </span>
        </div>
        <br />
        <div style="text-align: center">
            <span class="key">密码 ：</span><span class="val">
                <input type="password" name="txt_Password" />
            </span>
        </div>
        <br />
        <div style="text-align: center;">
            <input type="button" id="btnOK" onclick="btnOK_Click(event);" style="margin: 20px;"
                value='登录' />
            <input type="button" id="btnExit" onclick="btnExit_Click(event);" style="margin: 20px; margin-left: 100px;"
                value='返回' />
        </div>
        <label id="msg">
        </label>
        <%  
#if DEBUG
               Ronse += "<label style='color:white;position:fixed;bottom:0;left:0;'> Debug:" ;
               var version = System.Reflection.Assembly.LoadFile(Server.MapPath("~/bin/MyWeb.dll")).GetName().Version;
               var dt = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
               Ronse += dt.ToString() + "</label>";
#endif
        %>
    </div>
</asp:Content>
