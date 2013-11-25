/*用法：
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

$(function () { $(".slide").SliderStart(); }); 
or
$(function () { $("#Div1").SliderStart(); });
*/
jQuery.fn.extend({
    MySliderStart: function () {
        $(this).each(function (index, d) {
            d.show = function (i, callback) {
                var cur = $(d).find(".cur");
                if (cur.length == 0) {
                    cur = $(d).find("div span img").eq(i).addClass("cur");
                    cur.css("top", "0px");
                    //                    cur.animate({ top: 0 }, 100).css('opacity', '1');
                    $(".this_pic", $(d)).attr("src", cur.attr("src")).fadeTo(400, 1, callback);
                }
                else {
                    cur.removeClass("cur");
                    cur.animate({ top: 10 }, 10).css('opacity', '0.8');
                    cur = $(d).find("div span img").eq(i).addClass("cur");

                    cur.animate({ top: 0 }, 100).css('opacity', '1');

                    $(".this_pic").fadeTo(300, 0.1, function () {
                        $(".this_pic", $(d)).attr("src", cur.attr("src")).fadeTo(400, 1, callback);
                    });
                }
            };

            d.ani = function (i) {
                d.timer = $.timer(4500, function (timer) {
                    d.timer = timer;
                    var len = $("div span img", $(d)).length;
                    if (len < 2) { timer.stop(); return; }
                    i = (i + 1) % len;
                    d.show(i);
                });
            }

            $(d).addClass("ImgSlider");
            var jImgs = $("div span img", $(d));
            if (jImgs.length == 0) return;
            var jInitOne = $(jImgs[0]);
//            var jInitOneHeight = jInitOne.height() || $(d).height();
//            var jInitOneWidth = jInitOne.width() || $(d).width();

//            var comh = $(d).width() * jInitOneHeight / jInitOneWidth;
//            if (comh < $(d).height()) {
//                $(d).css("height", comh);
//            }
//            else {
//                var comw = $(d).height() * jInitOneWidth / jInitOneHeight;
//                $(d).css("width", comw);
//            }
            $(d).prepend('<a><img class="this_pic"/></a>');
            if (jImgs.length == 1) {
                $(".this_pic", $(d)).attr("src", jInitOne.attr("src"));
                $(d).find("div").remove();
                return;
            }

            $('<div class="transparence"></div>').appendTo($(this));
            d.ani(0);

            d.show(0);

            jImgs.each(function (_i, _d) {
                $(_d).bind("mouseenter", function (ev) { d.timer.stop(); d.show(_i); });
                $(_d).bind("mouseleave", function () { d.ani(_i); });
            });
        });
    }
});
$(function () { $(".ImgSlider").MySliderStart(); });