(function ($) {
    //$(dom).MyPager({total:6,page:1,display:12,href:'/Html/Products/zxld/{0}.html'});
    $.fn.MyPager = function (setting) {
        var p = $.extend({
            total: 1,   //总页数。
            page: 1,    //当前页。
            display: 7, //显示的总页码个数
            href: ""    //链接格式字符串
        }, setting);

        p.total = parseInt(p.total);
        p.page = parseInt(p.page);
        p.display = parseInt(p.display);


        this.empty().each(function () {
            $(this).append("第 {0} 页,共 {1} 页.".format(p.page, p.total));
            p.display = Math.max(p.display, 3);

            var min = p.page, max = p.page;
            for (var i = 1, len = Math.min(p.total, p.display) ; i <= len; i++) {
                if (i % 2) {
                    if (max == p.total) { min--; }
                    else {
                        max++;
                    }
                }
                else {
                    if (min < 1) { max++; }
                    else {
                        min--;
                    }
                }
            }

            if (min < 1) min = 1;
            if (max > p.total) max = p.total;

            for (var i = min ; i <= max  ; i++) {
                var currentPage = i;
                if ((i == min) && (min !== 1)) {
                    currentPage = 1;
                }


                if (currentPage == p.page) {
                    $(this).append('<a class="sel">{0}</a>'.format(currentPage));
                }
                else {
                    if (p.href) {
                        $(this).append('<a href="{1}">{0}</a>'.format(currentPage, p.href.format(currentPage)));
                    }
                    else {
                        $(this).append('<a>{0}</a>'.format(currentPage));
                    }
                    if (p.onclick) {
                        $(this).find("a").click(function () {
                            p.onclick(parseInt(jv.getDoer().innerHTML));
                        });
                    }
                }
            }
        });
    };
})(jQuery);