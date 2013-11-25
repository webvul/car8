(function () {
    jv.tree = {}

    jv.tree.Init = function (VVar) {
        jv.tree.InitRoot(VVar);

        $(".tv>dd dt").append("<div class='divcap'/>");
        $(".tv>dd dt>a").each(function (i) {
            $(this).appendTo($(this).parent().find(">div"));
        });

        $(".tv dt + dd").each(function (i) {
            var dt = $(this).prev("dt").find(">div").parent();

            dt.attr("layout", VVar["layout"]);

            jv.tree.InitEvent(VVar, dt);
        });

        jv.tree.InitRootLine(VVar);
        jv.tree.InitCapLine(VVar);

        $(".tv").attr("selimg", VVar["sel"]);
        jv.tree.Sel($(".tv .sel>div>a")[0]);

        //修正DD里第一项Dl－》Dt－》Div－》A
        if ($.browser.msie && $.browser.version <8) {
            $(".tv").find("dd").each(function (i, d) {
                var a = $(d).find("dt>div>a:first");
                a.offset({ left: a.offset().left - 40, top: a.offset().top });
            })
        };
    }
    jv.tree.InitRoot = function (VVar) {
        //先给第一层添加背景图.
        $(".tv>dt").css("background-image", "url(" + VVar["root_bg"] + ")");
        $(".tv > dt").append("<div class='divtit' />");
        $(".tv>dt>a").each(function (i) {
            $(this).appendTo($(this).parent().find(">div"));
        });

        $(".tv>dt + dd").each(function (i) {
            var div = $(this).prev("dt").find(">div");
            var layout = $(this).prev("dt").attr("layout");
            if (layout == "down") {
                div.css("background-image", "url(" + VVar["green_add"] + ")");
            }
            else {
                div.css("background-image", "url(" + VVar["red_minus"] + ")");
            }
        });
    }

    jv.tree.Sel = function (evt, LinkObj) {
        var sel = $(".tv .sel");
        if (sel.attr("bkimg") != null && sel.attr("bkimg").length > 0) {
            sel.css("background-image", sel.attr("bkimg"));
        }
        else {
            sel.css("background-image", "");
        }
        sel.removeClass("sel");
        $(LinkObj).parent().addClass("sel");
        $(LinkObj).parent().attr("bkimg", $(LinkObj).parent().css("background-image"));
        $(LinkObj).parent().css("background-image", "url(" + $(".tv").attr("selimg") + ")");
        $(LinkObj).parent().css("background-repeat", "no-repeat");
    }

    jv.tree.InitRootLine = function (VVar) {
        var prevlay = "up";
        $(".tv>dt").each(function (i) {
            if (i == 0) { $(this).css("border-top-width", "0px"); }
            else { $(this).css("border-top", VVar["root_border"]); }

            prevlay = $(this).attr("layout");
            if (prevlay == "down") { $(this).css("border-bottom", VVar["root_border"]); }
            else { $(this).css("border-bottom", "0px"); }
        });
    }

    jv.tree.InitCapLine = function (VVar) {
        var prevlay = "up";
        $(".tv>dd dt+dd").each(function (i) {
            var dt = $(this).prev("dt");
            if (dt.attr("layout") == "down") {
                dt.find("div").addClass("divcapsel");
                dt.find("div").css("background-image", "url(" + VVar["cap_red_minus"] + ")");
            }
            else {
                dt.find("div").css("background-image", "url(" + VVar["cap_add"] + ")");
            }
        });
    }

    jv.tree.InitEvent = function (VVar, JObj) {
        var dt = JObj;
        dt.click(function () {
            var isRoot = $(this).find(".divtit").parent().length > 0;
            if ($(this).attr("layout") == "down") {
                $(this).attr("layout", "up");
                var dt = $(this);
                $(this).next("dd").slideUp("fast");

                if (isRoot == true) {
                    dt.css("border-bottom", "0px");
                    dt.find("div").css("background-image", "url(" + VVar["green_add"] + ")");
                }
                else {
                    dt.find("div").removeClass("divcapsel");
                    dt.find("div").css("background-image", "url(" + VVar["cap_add"] + ")");
                }
            }
            else {
                $(this).attr("layout", "down");
                $(this).next("dd").slideDown("fast");

                if (isRoot == true) {
                    $(this).css("border-bottom", VVar["root_border"]);
                    $(this).find("div").css("background-image", "url(" + VVar["red_minus"] + ")");
                }
                else {
                    $(this).find("div").addClass("divcapsel");
                    $(this).find("div").css("background-image", "url(" + VVar["cap_red_minus"] + ")");
                }
            }
        });
    }
})();