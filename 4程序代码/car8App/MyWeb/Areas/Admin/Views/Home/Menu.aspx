<%@ Page Title="Menu" Language="C#" Inherits="MyCon.MyMvcPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Menu </title>
    <meta http-equiv="Content-Type" content="text/html;charset=UTF-8" />
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <base target="main" />
    <script type="text/javascript">
        function openMenu() {
            $(".add").each(function (i, d) {
                var jd = $(d);
                jd.addClass("minus").removeClass("add");

                jd.next().show();
            });

            $(".root_add").each(function (i, d) {
                var jd = $(d);
                jd.addClass("root_minus").removeClass("root_add");
                var divs = jd.parent().css("borderBottomWidth", "1px");
                divs.next().show();
            });
        }

        function closeRoot() {
            $(".minus").each(function (i, d) {
                var jd = $(d);
                jd.addClass("add").removeClass("minus");

                jd.next().hide();
            });
        }

        function closeMenu() {
            closeRoot();
            $(".root_minus").each(function (i, d) {
                var jd = $(d);
                jd.addClass("root_add").removeClass("root_minus");
                var divs = jd.parent().css("borderBottomWidth", "0px");
                divs.next().hide();
            });

        }



        $(function () {
            $(".Pucker").MyPucker();

            //            $(".root_minus").each(function (i, d) {
            //                var jd = $(d);
            //                jd.addClass("root_add").removeClass("root_minus");
            //                var divs = jd.parent().css("borderBottomWidth", "0px");
            //                divs.next().hide();
            //            });


            //$(".Pucker div a").click(function (ev) {
            //    var self = $(this), menuId = self.attr("uid");
            //    if (menuId) {
            //        $.post("~/Host/Home/MenuClicks.aspx", { id: menuId });
            //    }
            //});

        });
    </script>
    <style type="text/css">
        *
        {
            padding: 0px;
            margin: 0px;
        }
        body
        {
            background: url("~/Css/images/menu-top-bg.jpg") repeat-x scroll left top #c9d1d8;
        }
        .Pucker .special
        {
            width: 15px;
            height: 15px;
            background: transparent url(~/Res/MyPucker/hong.jpg) no-repeat 0 center;
        }
        .Pucker .plan
        {
            width: 15px;
            height: 15px;
            background: transparent url(~/Res/MyPucker/bai.jpg) no-repeat 0 center;
        }
        #top
        {
            position: fixed;
            height: 12px;
            z-index: 9999;
            width: 100%;
            top: 0;
            left: 0; /*filter: progid:DXImageTransform.Microsoft.gradient(gradientType=0,startColorstr='#073650', endColorstr='#c9d1d8');
            background: -webkit-gradient(linear,0 0,0 100% , from(gray), to(#c9d1d8) );*/
        }
        .right_btn
        {
            background: url(~/Css/images/middle_bar_right.png) no-repeat 50% 50%;
        }
        .left_btn
        {
            background: url(~/Css/images/middle_bar_left.png) no-repeat 50% 50%;
        }
        .right_position
        {
            position: fixed;
            width: 7px;
            z-index: 999;
            right: 0;
            top: 0;
            height: 100%;
            background-color: #a1a7ad;
        }
        .left_position
        {
            position: fixed;
            width: 7px;
            z-index: 999;
            left: 0;
            top: 0;
            height: 100%;
            background-color: #a1a7ad;
        }
    </style>
    <link rel="stylesheet" type="text/css" href="~/Res/MyPucker/MyPucker.css" />
    <script type="text/javascript">
        $(function () {

            $("#middlebar").click(function () {
                $(this).toggleClass(function () {
                    if ($(this).is(".left_btn")) {
                        $(top.document).find("#fm").attr("cols", "8,*");
                        $(this).removeClass("left_btn").removeClass("right_position").addClass("left_position");
                        $(".Main").hide();
                        return "right_btn";
                    }
                    else {
                        $(top.document).find("#fm").attr("cols", "180,*");
                        $(this).removeClass("right_btn").removeClass("left_position").addClass("right_position");
                        $(".Main").show();
                        return "left_btn";
                    }
                });
            });
        });


    </script>
</head>
<body>
    <div class="Main">
        <div id="treeMenu" style="margin: 12px 7px; overflow: auto;">
            <div style="text-align: left; background-color: #EBEDF2; padding: 3px; margin: 0;
                border: solid 1px #4D597F;">
                <span onclick="openMenu()" style="cursor: pointer;">展开菜单</span> | <span onclick="closeMenu()"
                    style="cursor: pointer">折叠菜单</span></div>
            <ul class="Pucker">
                <%= Html.MyPucker(PuckerNode.LoadMenu(0).SubNode,true)%></ul>
        </div>
    </div>
    <div id="top">
    </div>
    <div id="middlebar" class="left_btn right_position">
    </div>
</body>
</html>
