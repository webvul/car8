/*
重新定义：
1.利用 CSS3 标准，对元素增加属性 border-radius ，指定左上、左下、右上、右下。  如 border-radius=5,0,0,5 。数值表示半径。
该Div对象需定义为：
1. position= absolute or relative
2. 去除 overflow 属性。
3. display = block or inline-block
4. 尽量不要有 line-height ， 在IE7下会显示上下错位。
*/

(function ($) {
    jv.CssHasBorderRadius = (typeof (document.createElement("div").style.borderRadius) == "string");
    /*
    指定圆角半径大小，单数值，或多数值。
    参数：{radius：,func:,unCorner:,backColor:}
    */
    $.fn.MyCorner = function (setting) {
        var setting = $.extend({}, setting);
        var _GetCorner = function (jod, radius) {
            if (!radius) {
                radius = jod.attr("border-radius");
            }
            if (!radius) {
                radius = setting.radius;
            }

            var _GetNotAutoSetting = function (radius) {
                var Corner = Array();
                if (radius instanceof Array) return radius;
                if ((radius + "").indexOf(",") > 0) {
                    Corner = radius.split(',');
                    for (var i = 0; i < 4; i++) {
                        Corner[i] = parseInt(Corner[i]);
                    }
                }
                else if ((radius + "").indexOf(" ") > 0) {
                    Corner = radius.split(' ');
                    for (var i = 0; i < 4; i++) {
                        Corner[i] = parseInt(Corner[i]);
                    }
                }
                else {
                    for (var i = 0; i < 4; i++) {
                        Corner.push(parseInt(radius));
                    }
                }
                return Corner;
            },
            _GetAutoSetting = function (jobj) {
                if (jobj.filter("body").length > 0) return [0, 0, 0, 0];
                if (jobj.filter("[border-radius]").length > 0) {
                    if (jobj.attr("border-radius") == "auto") {
                        var pr = _GetAutoSetting(jobj.parent());
                        for (var i = 0; i < 4; i++) {
                            pr[i] -= parseInt(jobj.parent().css("borderTopWidth"));
                            if (pr[i] < 0) pr[i] = 0;
                        }

                        jobj.attr("border-radius", pr);
                        return pr;
                    }
                    else return _GetNotAutoSetting(jobj.attr("border-radius"));
                }
                else return [0, 0, 0, 0];
            };
            if (radius && radius != "auto") return _GetNotAutoSetting(radius);
            else return _GetAutoSetting(jod);
        };

        var THIS = this;
        var setCssRadius = function () {
            $(THIS).each(function () {
                var self = $(this);
                if (jv.GetInt(self.css("borderRadius")) == 0) {
                    var radiusAry = _GetCorner(self);
                    $(radiusAry).each(function (_i, _d) {
                        radiusAry[_i] = _d + "px";
                    });

                    self.css("borderRadius", radiusAry.join(" "));
                }
            });
            return THIS;
        };
        if (jv.CssHasBorderRadius) {
            return setCssRadius();
        }
        else if (!window.G_vmlCanvasManager) {
            return setCssRadius();
        }
        else if ($.browser.mozilla) {
            if (parseInt($.browser.version) > 2) {
                return setCssRadius();
            }
        }
        else if ($.browser.safari) {
            return setCssRadius();
        }


        var _unCorner = function (jod, unCorner) {
            if (unCorner != true) return false;
            if (!jod.data("myCorner")) return true;
            $(jod.data("myCorner")).each(function () {
                $(this).remove();
            });
            jod.data("myCorner", null);
        },
        _corner = function (jod, setting) {
            var p = $.extend({}, setting, p),
            radius = p.radius, func = p.func, unCorner = p.unCorner;

            if (_unCorner(jod, unCorner) == true) return;

            var Corner = _GetCorner(jod, radius),

            isValid = false;
            $(Corner).each(function (oi, od) {
                if (od > 0) { isValid = true; return false; }
            });
            if (isValid == false) {
                return false;
            }
            if (jod.data("myCorner")) return true;

            if (["relative", "absolute", "fixed"].indexOf(jod.css("position")) < 0) {
                jod.css("position", "relative"); // absolute 也可以。
            }

            if (["block", "inline-block", "none"].indexOf(jod.css("display")) < 0) {
                jod.css("display", "block"); // absolute 也可以。
            }

            if (jod.css("overflow")) {
                jod.css("overflow", "");
            }
            var borderTopWidth = parseInt(jod.css("borderTopWidth")) || 0,
            borderBottomWidth = parseInt(jod.css("borderBottomWidth")) || 0,
            borderLeftWidth = parseInt(jod.css("borderLeftWidth")) || 0,
            borderRightWidth = parseInt(jod.css("borderRightWidth")) || 0,

            v0 = document.createElement("canvas"),
            v1 = document.createElement("canvas"),
            v2 = document.createElement("canvas"),
            v3 = document.createElement("canvas");

            $(v0).css("top", "0px").css("left", "0px").css("marginLeft", 0 - borderLeftWidth).css("marginTop", 0 - borderTopWidth);
            $(v1).css("top", "0px").css("right", "0px").css("marginRight", 0 - borderRightWidth).css("marginTop", 0 - borderTopWidth);
            $(v2).css("bottom", "0px").css("right", "0px").css("marginRight", 0 - borderRightWidth).css("marginBottom", 0 - borderBottomWidth);
            $(v3).css("bottom", "0px").css("left", "0px").css("marginLeft", 0 - borderLeftWidth).css("marginBottom", 0 - borderBottomWidth);


            var jvs = $([v0, v1, v2, v3]);
            //            var outerWidth = parseInt(jod.css("outlineWidth"));
            //var div = $('<div style="display: inherit;" />').appendTo(jod);
            jvs.each(function (oi, od) {
                if (Corner[oi] <= 0) return;

                if (window.G_vmlCanvasManager) {
                    od = window.G_vmlCanvasManager.initElement(od);
                }

                var jvod = $(od),
                borderWidth = (oi % 2 == 0) ? borderTopWidth : borderBottomWidth;

                if (borderWidth == 0) return;
                jvod.css("position", "absolute");
                //                jvod.css("margin", 0 - borderWidth);


                //                var size = borderWidth > Corner[oi] ? Corner[oi] / 2 + borderWidth : Corner[oi] + borderWidth / 2;
                //                jvod.attr("width", size);
                //                jvod.attr("height", size);
                jvod.appendTo(jod);
                var ary = (jod.data("myCorner") || []);
                ary.push(od);
                jod.data("myCorner", ary);
            });

            jod.data("backColor", p.backColor);
            if (Corner[0] > 0 && borderTopWidth > 0 && borderLeftWidth > 0) {
                var v = v0,
                 c = Corner[0],
                 b = Math.min(borderTopWidth, borderLeftWidth),
                 borderColor = borderTopWidth >= borderLeftWidth ? jod.css("borderTopColor") : jod.css("borderLeftColor");
                $(v).attr("width", c + b / 2).attr("height", c + b / 2);

                _Arc({
                    canvas: v,
                    x: c,
                    y: c,
                    radius: c,
                    start: Math.PI * 1.5,
                    p1: { x: 0 - c, y: c },
                    p2: { x: 0 - c, y: 0 - c },
                    p3: { x: c, y: 0 - c },
                    pjdiv: jod,
                    func: func,
                    borderColor: borderColor,
                    borderWidth: b
                });
            }
            if (Corner[1] > 0 && borderTopWidth > 0 && borderRightWidth > 0) {
                var v = v1,
                 c = Corner[1],
                 b = Math.min(borderTopWidth, borderRightWidth),
                 borderColor = borderTopWidth >= borderRightWidth ? jod.css("borderTopColor") : jod.css("borderRightColor");

                $(v).attr("width", c).attr("height", c);

                _Arc({
                    canvas: v,
                    x: 0,
                    y: c,
                    radius: c,
                    start: Math.PI * 2,
                    p1: { x: 0, y: 0 - c },
                    p2: { x: c * 2, y: 0 - c },
                    p3: { x: c * 2, y: c },
                    pjdiv: jod,
                    func: func,
                    borderColor: borderColor,
                    borderWidth: b
                });
            }
            if (Corner[2] > 0 && borderBottomWidth > 0 && borderRightWidth > 0) {
                var v = v2,
                c = Corner[2],
                b = Math.min(borderBottomWidth, borderRightWidth),
                borderColor = borderBottomWidth >= borderRightWidth ? jod.css("borderBottomColor") : jod.css("borderRightColor");

                $(v).attr("width", c).attr("height", c);


                _Arc({
                    canvas: v,
                    x: 0,
                    y: 0,
                    radius: c,
                    start: Math.PI * 0.5,
                    p1: { x: c * 2, y: 0 },
                    p2: { x: c * 2, y: c * 2 },
                    p3: { x: 0, y: c * 2 },
                    pjdiv: jod,
                    func: func,
                    borderColor: borderColor,
                    borderWidth: b
                });
            }
            if (Corner[3] > 0 && borderBottomWidth > 0 && borderLeftWidth > 0) {
                var v = v3,
                 c = Corner[3],
                 b = Math.min(borderBottomWidth, borderLeftWidth),
                 borderColor = borderBottomWidth >= borderLeftWidth ? jod.css("borderBottomColor") : jod.css("borderLeftColor");

                $(v).attr("width", c).attr("height", c);

                _Arc({
                    canvas: v,
                    x: c,
                    y: 0,
                    radius: c,
                    start: Math.PI,
                    p1: { x: c, y: 2 * c },
                    p2: { x: 0, y: 2 * c },
                    p3: { x: 0 - c, y: 0 },
                    pjdiv: jod,
                    func: func,
                    borderColor: borderColor,
                    borderWidth: b
                });
            }
            if ($.browser.msie && $.browser.version < 9) {
                jvs.each(function () {
                    var self = $(this);
                    self.find("div").width(self.width()).height(self.height());
                });
            }
            return true;
        },



        /* 利用 */
        _Arc = function (setting) { //outColor, borderColor, borderWidth) {
            var canvas = setting.canvas,
                x = setting.x,  //X坐标
                y = setting.y,  //Y坐标
                radius = setting.radius,    //半径
                start = setting.start,      // 起始角度
                p1 = setting.p1,
                p2 = setting.p2,
                p3 = setting.p3,
                pjdiv = setting.pjdiv,
                borderWidth = setting.borderWidth,
                borderColor = setting.borderColor,

                outColor = pjdiv.data("backColor") || jv.GetParentColor(pjdiv);
            borderColor = borderColor || "white";
            //            var borderWidth = parseInt(pjdiv.css("borderTopWidth"));

            var bigBgFunc = function (v) {
                if (v > 0) v += borderWidth * 2;
                else v -= borderWidth * 2;
                return v;
            };
            p1.x = bigBgFunc(p1.x);
            p1.y = bigBgFunc(p1.y);
            p2.x = bigBgFunc(p2.x);
            p2.y = bigBgFunc(p2.y);
            p3.x = bigBgFunc(p3.x);
            p3.y = bigBgFunc(p3.y);
            //setting.p1 = p1; setting.p2 = p2; setting.p3 = p3;

            var ctx = canvas.getContext('2d');

            if (radius.func) {
                if (radius.func(ctx, radius) == false) return;
            }
            ctx.beginPath();
            if (borderWidth > 0) {
                ctx.lineWidth = borderWidth * 2;
                ctx.strokeStyle = borderColor;
                ctx.arc(x, y, radius, start, start - Math.PI / 2.0, true);  //逆时针
                ctx.stroke();
            }

            ctx.lineWidth = 0;
            ctx.lineTo(p1.x, p1.y);
            ctx.lineTo(p2.x, p2.y);
            ctx.lineTo(p3.x, p3.y);
            ctx.fillStyle = parseInt(pjdiv.css("outlineWidth")) > 0 ? borderColor : outColor;
            ctx.fill();
        }

        this.each(function () {
            _corner($(this), setting);
        });
        return this;
    };

    $.fn.MyShade = function (setting) {

    }
    $.fn.MyCardTitle = function (setting) {
        var _shade = function (setting) {
            var height = 8,
            cv = setting.canvas,
            jod = $(cv);
            //            jod.parent().height(height);
            jod.attr("width", setting.width).attr("height", height);


            if ($.browser.msie && parseInt($.browser.version) < 9) {
                if (window.G_vmlCanvasManager) {
                    cv = window.G_vmlCanvasManager.initElement(cv);
                }
            }

            var ctx = cv.getContext('2d'),
            lg = ctx.createLinearGradient(0, 0, setting.width, 0);
            lg.addColorStop(0, setting.startColor);
            lg.addColorStop(1, setting.endColor);
            ctx.fillStyle = lg;
            ctx.fillRect(0, 0, setting.width, height);
            return true;
        };

        this.each(function () {
            var self = $(this);
            if (self.data("MyCardTitle") == true) return;
            self.data("MyCardTitle", true);
            if ($.browser.msie && (parseInt($.browser.version) <= 7)) {
                self.css("display", "inline");
            } else {
                self.css("display", "inline-block");
            }
            var pod = self.wrap("<div class='MyCardTitleWrap'></div>").parent().css("position", "relative"),
            cv = document.createElement("canvas");
            $(cv).css("position", "absolute").appendTo(pod);
            var container = pod.parent(),
            width = container.width() - self.width() - 3;
            if (width < 0) return;
            var endOpacity = parseInt(self.attr("endopacity") || -50);
            if (endOpacity < 0) { width = 100 * width / (100 - endOpacity); }
            $(cv).css("bottom", 0); //.css("left", $(od).width() + parseInt($(od).css("paddingLeft")));
            _shade({ canvas: cv, startColor: self.css("backgroundColor"), endColor: jv.GetParentColor(pod), width: width, endOpacity: endOpacity });
        });
        return this;
    }
})(jQuery);
