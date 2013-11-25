<%@ Page Language="C#" Inherits="MyCon.MyMvcPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Split</title>
    <link href="~/Css/Sliderbar.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(function () {

            $("#middlebar").click(function () {
                $(this).find("input").toggleClass(function () {
                    if ($(this).is(".left_btn")) {
                        $(top.document).find("#fm").attr("cols", "0,6,*");
                        $(this).removeClass("left_btn");
                        return "right_btn";
                    }
                    else {
                        $(top.document).find("#fm").attr("cols", "180,6,*");
                        $(this).removeClass("right_btn");
                        return "left_btn";
                    }
                });
            });
        });

     
    </script>
</head>
<body style="overflow: hidden">
    <div id="middlebar">
        <input name="button" type="submit" class="left_btn" value="" /></div>
</body>
</html>
