//(function ($) {
//    $.fn.SkinVar = function (options) {
//        var defaults = {
//            Corner: {
//                Header: 5,
//                InfoPad: 0,
//                List: 0,
//                Viewy: 0,
//                Search: 5,
//                ButtonLink: 0,
//                Content: 0,
//                MenuMap: 0,
//                Sect: 0,
//                MainItem: 0,
//                Cap: 0,
//                MainSiteMap: 0
//            },
//            LangImg: { ContactImg: "~/{0}" },
//            LangCss: ["ContactImg{0}"]
//        }

//        if (!options) {
//            options = {};
//        }
//        options.Corner = $.extend(defaults.Corner, options.Corner);
//        options.LangImg = $.extend(defaults.LangImg, options.LangImg);
//        options.LangCss = $.extend(defaults.LangCss, options.LangCss);

//        this.each(function () {
//            //ÊµÏÖ´úÂë
//            //for (var p in options.Corner) {
//            //    jv.SetCorner('.' + p, options.Corner[p]);
//            //}
//            for (var p in options.LangImg) {
//                $("." + p).attr('src', options.LangImg[p].format(jv.page()["Lang"]));
//            }
//            for (var p in options.LangCss) {
//                $("." + p).addClass(p.format(jv.page()["Lang"]));
//            }

//            if (($(".ListPhoto").attr("src") || "").length == 0) {
//                $(".ListPhoto").parents(".Sect:first").hide();
//            }
//            if (($(".SkinTopBanner").attr("src") || "").length == 0) {
//                $(".SkinTopBanner").hide();
//            }
//        });
//    };
//})(jQuery);

//$(function () { $("body").SkinVar(); });