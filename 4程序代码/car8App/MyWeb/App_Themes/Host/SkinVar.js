/*
此Js是部分Json字符串格式,不能单独被解析
*/
(function ($) {
    $.fn.SkinVar = function (options) {
        var defaults = {
            Corner: {
                Sect: 15
            },
            LangImg: {},
            LangCss: []
        }

        if (!options) {
            options = {};
        }
        options.Corner = $.extend(defaults.Corner, options.Corner);
        options.LangImg = $.extend(defaults.LangImg, options.LangImg);
        options.LangCss = $.extend(defaults.LangCss, options.LangCss);

        this.each(function () {
            //实现代码
//            for (var p in options.Corner) {
//                jv.SetCorner('.' + p, options.Corner[p]);
//            }
            for (var p in options.LangImg) {
                $("." + p).attr('src', options.LangImg[p].format(jv.page()["Lang"]));
            }
            for (var p in options.LangCss) {
                $("." + p).addClass(p.format(jv.page()["Lang"]));
            }
        });
    };
})(jQuery);

$(function () { $("body").SkinVar(); });