(function ($) {
    $.fn.MyPucker = function (options) {
        options = $.extend({
            layout: "down",
            container: "body",
            groupLink: false,    //组是否可以链接，是否可选。
            click: function (obj, ev) { },
            rootclick: function (obj, ev) { }
        }, options);

        // 节点结构是: li>div>a>span
        var initIcon = function (obj) {
            var jObj = $(obj);
            jObj.find(">li").find("li").each(function (i, d) {
                var jd = $(d);
                if (jd.find(">ul").length > 0) {
                    //                    $(d).find(">div>a>span").addClass("hero");
                    jd.find(">div").addClass("minus");
                    jd.click(function (ev) {
                        var jthis = $(this), uls = jthis.find(">ul"), divs = jthis.find(">div");

                        if (divs.hasClass("add")) {

                            //如果展开的菜单不可见，则滚动条向下展开，把展开的菜单显示完整。
                            var jcon = $(options.container), con = jcon[0];
                            var expandValue = uls.height() + jthis.offset().top - con.scrollTop - jv.GetEyeSize().height;

                            uls.slideDown("fast", function () {
                                if (expandValue > 0) {
                                    con.scrollTop += expandValue;
                                }

                                divs.removeClass("add").addClass("minus");
                            });
                        }
                        else {
                            uls.slideUp("fast", function () {
                                divs.removeClass("minus").addClass("add");
                            });
                        }
                        ev.stopPropagation();
                    });


                    //                    }, function (ev) {
                    //                        
                    //                        ev.stopPropagation();
                    //                    });
                }
                else {
                    //                    $(d).find(">div>a>span").addClass("soldier");
                    jd.click(function (ev) {
                        ev.stopPropagation();
                    });
                }
            });
        },
        initLink = function (obj) {
            var jObj = $(obj), lis = jObj.find(">li");
            lis.find("li>div").each(function (i, d) {
                var jd = $(d);
                jd.hover(function () {
                    jd.addClass("over");
                }, function () {
                    jd.removeClass("over");
                });
            });

            lis.find("li>div>a").click(function (ev) {
                var jthis = $(this), uls = jthis.parent().next("ul");

                if (options.groupLink || !uls.length) {
                    $(".Pucker .sel").removeClass("sel");
                    $(this).closest("div").addClass("sel");
                    options.click(this, ev);
                    ev.stopPropagation();
                }
            });
        },
        initRoot = function (obj) {
            var jObj = $(obj);
            jObj.find(">li").each(function (i, d) {
                var jd = $(d);
                //根节点会改造成: li>div>div>a>span
                jd.find(">div a").wrap("<div />")

                var divs = jd.find(">div");

                divs.click(function (ev) {

                    var jthis = $(this), item = jthis.find(">div");
                    if (item.hasClass("root_add")) {
                        jthis.css("borderBottomWidth", "1px");
                        jthis.next("ul").slideDown("fast", function () {
                            item.removeClass("root_add").addClass("root_minus");
                        });
                    }
                    else {
                        jthis.next("ul").slideUp("fast", function () {
                            jthis.css("borderBottomWidth", "0px");
                            item.removeClass("root_minus").addClass("root_add");
                        });
                    }

                    ev.stopPropagation();
                });


                var jitem = divs.find(">div");


                jitem.addClass("root_minus").hover(function () { $(this).addClass("rootOver"); }, function () { $(this).removeClass("rootOver"); });

                jitem.find(">a").click(function (ev) {
                    options.rootclick(this, ev);
                    ev.stopPropagation();
                })
                    .find(">span").removeClass("hero").addClass("root");
            });
        };
        this.each(function (i, d) {
            //设置图标
            initIcon(d);
            initLink(d);
            initRoot(d);
        });
    };
})(jQuery);