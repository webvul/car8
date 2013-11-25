<%@ Page Title="" Language="C#" Inherits="MyShopPage<MyOql.DeptRule.Entity>"
    Theme="Web" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE7" />
    <script src="~/Res/jquery.js" type="text/javascript"></script>
    <script src="~/Res/jquery.json.js" type="text/javascript"></script>
    <script src="~/Res/MyJs.js" type="text/javascript"></script>
    <script src="~/Res/MyJs_Extend.js" type="text/javascript"></script>
    <script src="~/Res/ContextMenu/jquery.contextmenu.r2.js" type="text/javascript"></script>
    <style type="text/css">
        body, div, ul, li
        {
            margin: 0;
            padding: 0;
        }
        
    </style>
    <script src="~/Res/ImgSlider/ImgSlider.js" type="text/javascript"></script>
    <link href="~/Res/ImgSlider/ImgSlider.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">



        $(function () { $(".slide").SliderStart(); });
 
        
    </script>
</head>
<body>
    <label id="log">
    </label>
    
    <div id="Div1" class="slide">
        <ul id="Ul1">
            <li><a>
                <img src="~/images/1.jpg" alt="" /></a></li>
            <li><a>
                <img src="~/images/2.jpg" alt="" /></a></li>
            <li><a>
                <img src="~/images/3.jpg" alt="" /></a></li>
            <li><a>
                <img src="~/images/4.jpg" alt="" /></a></li>
            <li><a>
                <img src="~/Res/Img/Help.png" />
                 </a></li>
        </ul>
    </div>
        <div id="Div2" class="slide">
        <ul id="Ul2">
            <li><a>
                <img src="~/images/1.jpg" alt="" /></a></li>
            <li><a>
                <img src="~/images/2.jpg" alt="" /></a></li>
            <li><a>
                <img src="~/images/3.jpg" alt="" /></a></li>
            <li><a>
                <img src="~/images/4.jpg" alt="" /></a></li>
            <li><a>
                <img src="~/images/5.jpg" alt="" /></a></li>
        </ul>
    </div>
</body>
</html>
