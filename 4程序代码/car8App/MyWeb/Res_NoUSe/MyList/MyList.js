(function ($) {
    $.fn.MyList = function (options) {
        var p = $.extend({
            data: false,
            count: 0,
            cols: false,
            fom: '',
            url: ''
        }, options);

        var g = {};

        g.CreateStandardItem = function () {
            if (g.standardItem) return g.standardItem.clone()[0];
            g.standardItem = $('<li></li>');

            g.standardItem.find(".abc").click(function () { alert("123") });

            return g.standardItem.clone()[0];
        };

        g.ToHtml = function (row) {
            var tt = p.fom;
            for (var col in p.cols) {
                var cell = row[p.cols[col]];
                if (!cell) continue;
                var gg = new RegExp("{" + p.cols[col] + "}", "g");
                tt = tt.replace(gg, cell)
            }
            var result = $(g.CreateStandardItem());         
            return result.append($(tt));
        };

        var _List = function (con) {
            var uls = $('<ul></ul>');

            if (p.url) {
                $.post(p.url, function (res) {
                    p.data = eval(res.data);
                    for (var i = 0; i < p.data.length; i++) {
                        var tempD = p.data[i];
                        if (!tempD) continue;

                        uls.append(g.ToHtml(tempD));
                    }
                    $(con).append(uls);
                });
            } else {
                for (var i = 0; i < p.data.length; i++) {
                    var tempD = p.data[i];
                    if (!tempD) continue;

                    uls.append(g.ToHtml(tempD));
                }
                $(con).append(uls);
            }
        };


        this.each(function (i, d) {
            if ($(d).data("MyList")) { return true; }
            $(d).data("MyList", g);

            _List(d);
        });

        return this;
    };
})(jQuery);