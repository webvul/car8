/*
用法:
1. 在控件上应用约束. 
2. 在事件上调用方法: if ( jv.chk() == false ) return false ;

应用的约束示例:
1.简单通用的函数写法:
<input type="text" chk="function(val){ if ( val.length < 10 ) return '长度必须大于10 .' ;}" />
<input type="text" chk="function(val){ return MySelfFunction(val) ; }" />
2.简单通用的正则表达式写法 , 关于 \  ,写在HTML 元素的属性值上,不必使用两个 \\ , 使用一个 \ 表示即可.  但是如果在 JS 的 String 变量中, 必须使用两个 \\ 表示一个 \
必须指定 chkmsg ,语义为, 如果正则表达式命中, 则显示消息 .  命中条件可以是 == , 也可以是 !=
<input type="text" chk="reg == ^\d+$" chkmsg= "不能是数字." />
<input type="text" chk="reg != ^\d+$" chkmsg= "必须是数字." />
3.指定表达式,这种写法是应用的最常用的验证,代码量最少.
<input type="text" chk="int" />
<input type="text" chk="int{1,5}" />    指定长度范围
<input type="text" chk="int{1,5}(8,19999)" /> 指定长度范围和数值范围, 在这里长度范围就无意义了.
<input type="text" chk="int(8,199)" chkmsg="需要的数值范围是 8 - 199 "/> 如不指定消息 , 会弹出默认消息 .

<input type="text" chk="number" />  //可以是小数.
<input type="text" chk="date(2000-10-1,2010-10-1)" />  
<input type="text" chk="time(8:00,18:00)" />  
<input type="text" chk="datetime(8:00,18:00)" />  

<input type="text" chk="enum(提交,审批,通过,驳回,归档)" />  //枚举

<input type="text" chk="int"  chkmsg=""  chkval="" />
*/
//控件三属性: chk, chkval , chkmsg



/*
* Poshy Tip jQuery plugin v1.0
* http://vadikom.com/tools/poshy-tip-jquery-plugin-for-stylish-tooltips/
* Copyright 2010, Vasil Dinkov, http://vadikom.com/
*/

(function ($) {

    var tips = [],
		reBgImage = /^url\(["']?([^"'\)]*)["']?\);?$/i,
		rePNG = /\.png$/i,
		ie6 = $.browser.msie && $.browser.version == 6;

    // make sure the tips' position is updated on resize
    function handleWindowResize() {
        $.each(tips, function () {
            this.refresh(true);
        });
    }
    $(window).resize(handleWindowResize);

    $.Poshytip = function (elm, options) {
        this.$elm = $(elm);
        this.opts = $.extend({}, $.fn.poshytip.defaults, options);
        this.$tip = $(['<div class="', this.opts.className, '">',
				'<div class="tip-inner tip-bg-image"></div>',
				'<div class="tip-arrow tip-arrow-top tip-arrow-right tip-arrow-bottom tip-arrow-left"></div>',
			'</div>'].join(''));
        this.$arrow = this.$tip.find('div.tip-arrow');
        this.$inner = this.$tip.find('div.tip-inner');
        this.disabled = false;
        this.init();
    };

    $.Poshytip.prototype = {
        init: function () {
            tips.push(this);

            // save the original title and a reference to the Poshytip object
            this.$elm.data('title.poshytip', this.$elm.attr('title') || "")
				.data('poshytip', this);

            // hook element events
            switch (this.opts.showOn) {
                case 'hover':
                    this.$elm.bind({
                        'mouseenter.poshytip': $.proxy(this.mouseenter, this),
                        'mouseleave.poshytip': $.proxy(this.mouseleave, this)
                    });
                    if (this.opts.alignTo == 'cursor')
                        this.$elm.bind('mousemove.poshytip', $.proxy(this.mousemove, this));
                    if (this.opts.allowTipHover)
                        this.$tip.hover($.proxy(this.clearTimeouts, this), $.proxy(this.hide, this));
                    break;
                case 'focus':
                    this.$elm.bind({
                        'focus.poshytip': $.proxy(this.show, this),
                        'blur.poshytip': $.proxy(this.hide, this)
                    });
                    break;
            }
        },
        mouseenter: function (e) {
            if (this.disabled)
                return true;

            this.clearTimeouts();
            this.$elm.attr('title', '');
            this.showTimeout = setTimeout($.proxy(this.show, this), this.opts.showTimeout);
        },
        mouseleave: function () {
            if (this.disabled)
                return true;

            this.clearTimeouts();
            this.$elm.attr('title', this.$elm.data('title.poshytip'));
            this.hideTimeout = setTimeout($.proxy(this.hide, this), this.opts.hideTimeout);
        },
        mousemove: function (e) {
            if (this.disabled)
                return true;

            this.eventX = e.pageX;
            this.eventY = e.pageY;
            if (this.opts.followCursor && this.$tip.data('active')) {
                this.calcPos();
                this.$tip.css({ left: this.pos.l, top: this.pos.t });
                if (this.pos.arrow)
                    this.$arrow[0].className = 'tip-arrow tip-arrow-' + this.pos.arrow;
            }
        },
        show: function () {
            if (this.disabled || this.$tip.data('active'))
                return;

            this.reset();
            this.update();
            this.display();
        },
        hide: function () {
            if (this.disabled || !this.$tip.data('active'))
                return;

            this.display(true);
        },
        reset: function () {
            this.$tip.queue([]).detach().css('visibility', 'hidden').data('active', false);
            this.$inner.find('*').poshytip('hide');
            if (this.opts.fade)
                this.$tip.css('opacity', this.opacity);
            this.$arrow[0].className = 'tip-arrow tip-arrow-top tip-arrow-right tip-arrow-bottom tip-arrow-left';
        },
        update: function (content) {
            if (this.disabled)
                return;

            var async = content !== undefined;
            if (async) {
                if (!this.$tip.data('active'))
                    return;
            } else {
                content = this.opts.content;
            }

            this.$inner.contents().detach();
            var self = this;
            this.$inner.append(
				typeof content == 'function' ?
					content.call(this.$elm[0], function (newContent) {
					    self.update(newContent);
					}) :
					content == '[title]' ? this.$elm.data('title.poshytip') : content
			);

            this.refresh(async);
        },
        refresh: function (async) {
            if (this.disabled)
                return;

            if (async) {
                if (!this.$tip.data('active'))
                    return;
                // save current position as we will need to animate
                var currPos = { left: this.$tip.css('left'), top: this.$tip.css('top') };
            }

            // reset position to avoid text wrapping, etc.
            this.$tip.css({ left: 0, top: 0 }).appendTo(document.body);

            // save default opacity
            if (this.opacity === undefined)
                this.opacity = this.$tip.css('opacity');

            // check for images - this code is here (i.e. executed each time we show the tip and not on init) due to some browser inconsistencies
            var bgImage = this.$tip.css('background-image').match(reBgImage),
				arrow = this.$arrow.css('background-image').match(reBgImage);

            if (bgImage) {
                var bgImagePNG = rePNG.test(bgImage[1]);
                // fallback to background-color/padding/border in IE6 if a PNG is used
                if (ie6 && bgImagePNG) {
                    this.$tip.css('background-image', 'none');
                    this.$inner.css({ margin: 0, border: 0, padding: 0 });
                    bgImage = bgImagePNG = false;
                } else {
                    this.$tip.prepend('<table border="0" cellpadding="0" cellspacing="0"><tr><td class="tip-top tip-bg-image" colspan="2"><span></span></td><td class="tip-right tip-bg-image" rowspan="2"><span></span></td></tr><tr><td class="tip-left tip-bg-image" rowspan="2"><span></span></td><td></td></tr><tr><td class="tip-bottom tip-bg-image" colspan="2"><span></span></td></tr></table>')
						.css({ border: 0, padding: 0, 'background-image': 'none', 'background-color': 'transparent' })
						.find('.tip-bg-image').css('background-image', 'url("' + bgImage[1] + '")').end()
						.find('td').eq(3).append(this.$inner);
                }
                // disable fade effect in IE due to Alpha filter + translucent PNG issue
                if (bgImagePNG && !$.support.opacity)
                    this.opts.fade = false;
            }
            // IE arrow fixes
            if (arrow && !$.support.opacity) {
                // disable arrow in IE6 if using a PNG
                if (ie6 && rePNG.test(arrow[1])) {
                    arrow = false;
                    this.$arrow.css('background-image', 'none');
                }
                // disable fade effect in IE due to Alpha filter + translucent PNG issue
                this.opts.fade = false;
            }

            var $table = this.$tip.find('table');
            if (ie6) {
                // fix min/max-width in IE6
                this.$tip[0].style.width = '';
                $table.width('auto').find('td').eq(3).width('auto');
                var tipW = this.$tip.width(),
					minW = parseInt(this.$tip.css('min-width')),
					maxW = parseInt(this.$tip.css('max-width'));
                if (!isNaN(minW) && tipW < minW)
                    tipW = minW;
                else if (!isNaN(maxW) && tipW > maxW)
                    tipW = maxW;
                this.$tip.add($table).width(tipW).eq(0).find('td').eq(3).width('100%');
            } else if ($table[0]) {
                // fix the table width if we are using a background image
                $table.width('auto').find('td').eq(3).width('auto').end().end().width(this.$tip.width()).find('td').eq(3).width('100%');
            }
            this.tipOuterW = this.$tip.outerWidth();
            this.tipOuterH = this.$tip.outerHeight();

            this.calcPos();

            // position and show the arrow image
            if (arrow && this.pos.arrow) {
                this.$arrow[0].className = 'tip-arrow tip-arrow-' + this.pos.arrow;
                this.$arrow.css('visibility', 'inherit');

                //Udi ，右对齐。
                if (this.opts["arrow"] == "-right") {
                    this.$arrow.css("left", "auto").css("right", this.$arrow.css("marginLeft"));
                }
            }

            if (async)
                this.$tip.css(currPos).animate({ left: this.pos.l, top: this.pos.t }, 200);
            else
                this.$tip.css({ left: this.pos.l, top: this.pos.t });
        },
        display: function (hide) {
            var active = this.$tip.data('active');
            if (active && !hide || !active && hide)
                return;

            this.$tip.stop();
            if ((this.opts.slide && this.pos.arrow || this.opts.fade) && (hide && this.opts.hideAniDuration || !hide && this.opts.showAniDuration)) {
                var from = {}, to = {};
                // this.pos.arrow is only undefined when alignX == alignY == 'center' and we don't need to slide in that rare case
                if (this.opts.slide && this.pos.arrow) {
                    var prop, arr;
                    if (this.pos.arrow == 'bottom' || this.pos.arrow == 'top') {
                        prop = 'top';
                        arr = 'bottom';
                    } else {
                        prop = 'left';
                        arr = 'right';
                    }
                    var val = parseInt(this.$tip.css(prop));
                    from[prop] = val + (hide ? 0 : this.opts.slideOffset * (this.pos.arrow == arr ? -1 : 1));
                    to[prop] = val + (hide ? this.opts.slideOffset * (this.pos.arrow == arr ? 1 : -1) : 0);
                }
                if (this.opts.fade) {
                    from.opacity = hide ? this.$tip.css('opacity') : 0;
                    to.opacity = hide ? 0 : this.opacity;
                }
                this.$tip.css(from).animate(to, this.opts[hide ? 'hideAniDuration' : 'showAniDuration']);
            }
            hide ? this.$tip.queue($.proxy(this.reset, this)) : this.$tip.css('visibility', 'inherit');
            this.$tip.data('active', !active);
        },
        disable: function () {
            this.reset();
            this.disabled = true;
        },
        enable: function () {
            this.disabled = false;
        },
        destroy: function () {
            this.reset();
            this.$tip = $();
            this.$elm.unbind('poshytip').removeData('title.poshytip').removeData('poshytip');
            tips.splice($.inArray(this, tips), 1);
            this.$elm = $();
            this.disabled = true;
        },
        clearTimeouts: function () {
            if (this.showTimeout) {
                clearTimeout(this.showTimeout);
                this.showTimeout = 0;
            }
            if (this.hideTimeout) {
                clearTimeout(this.hideTimeout);
                this.hideTimeout = 0;
            }
        },
        calcPos: function () {
            var pos = { l: 0, t: 0, arrow: '' },
				$win = $(window),
				win = {
				    l: $win.scrollLeft(),
				    t: $win.scrollTop(),
				    w: $win.width(),
				    h: $win.height()
				}, xL, xC, xR, yT, yC, yB;
            if (this.opts.alignTo == 'cursor') {
                xL = xC = xR = this.eventX;
                yT = yC = yB = this.eventY;
            } else { // this.opts.alignTo == 'target'
                var elmOffset = this.$elm.offset(),
					elm = {
					    l: elmOffset.left,
					    t: elmOffset.top,
					    w: this.$elm.outerWidth(),
					    h: this.$elm.outerHeight()
					};
                xL = elm.l + (this.opts.alignX != 'inner-right' ? 0 : elm.w); // left edge
                xC = xL + Math.floor(elm.w / 2); 			// h center
                xR = xL + (this.opts.alignX != 'inner-left' ? elm.w : 0); // right edge
                yT = elm.t + (this.opts.alignY != 'inner-bottom' ? 0 : elm.h); // top edge
                yC = yT + Math.floor(elm.h / 2); 			// v center
                yB = yT + (this.opts.alignY != 'inner-top' ? elm.h : 0); // bottom edge
            }

            // keep in viewport and calc arrow position
            switch (this.opts.alignX) {
                case 'right':
                case 'inner-left':
                    pos.l = xR + this.opts.offsetX;
                    if (pos.l + this.tipOuterW > win.l + win.w)
                        pos.l = win.l + win.w - this.tipOuterW;
                    if (this.opts.alignX == 'right' || this.opts.alignY == 'center')
                        pos.arrow = 'left';
                    break;
                case 'center':
                    pos.l = xC - Math.floor(this.tipOuterW / 2);
                    if (pos.l + this.tipOuterW > win.l + win.w)
                        pos.l = win.l + win.w - this.tipOuterW;
                    else if (pos.l < win.l)
                        pos.l = win.l;
                    break;
                default: // 'left' || 'inner-right'
                    pos.l = xL - this.tipOuterW - this.opts.offsetX;
                    if (pos.l < win.l)
                        pos.l = win.l;
                    if (this.opts.alignX == 'left' || this.opts.alignY == 'center')
                        pos.arrow = 'right';
            }
            switch (this.opts.alignY) {
                case 'bottom':
                case 'inner-top':
                    pos.t = yB + this.opts.offsetY;
                    // 'left' and 'right' need priority for 'target'
                    if (!pos.arrow || this.opts.alignTo == 'cursor')
                        pos.arrow = 'top';
                    if (pos.t + this.tipOuterH > win.t + win.h) {
                        pos.t = yT - this.tipOuterH - this.opts.offsetY;
                        if (pos.arrow == 'top')
                            pos.arrow = 'bottom';
                    }
                    break;
                case 'center':
                    pos.t = yC - Math.floor(this.tipOuterH / 2);
                    if (pos.t + this.tipOuterH > win.t + win.h)
                        pos.t = win.t + win.h - this.tipOuterH;
                    else if (pos.t < win.t)
                        pos.t = win.t;
                    break;
                default: // 'top' || 'inner-bottom'
                    pos.t = yT - this.tipOuterH - this.opts.offsetY;
                    // 'left' and 'right' need priority for 'target'
                    if (!pos.arrow || this.opts.alignTo == 'cursor')
                        pos.arrow = 'bottom';
                    if (pos.t < win.t) {
                        pos.t = yB + this.opts.offsetY;
                        if (pos.arrow == 'bottom')
                            pos.arrow = 'top';
                    }
            }
            this.pos = pos;
        }
    };

    $.fn.poshytip = function (options) {
        if (typeof options == 'string') {
            return this.each(function () {
                var poshytip = $(this).data('poshytip');
                if (poshytip && poshytip[options])
                    poshytip[options]();
            });
        }

        var opts = $.extend({}, $.fn.poshytip.defaults, options);

        // generate CSS for this tip class if not already generated
        if (!$('#poshytip-css-' + opts.className)[0])
            $(['<style id="poshytip-css-', opts.className, '" type="text/css">',
				'div.', opts.className, '{visibility:hidden;position:absolute;top:0;left:0;}',
				'div.', opts.className, ' table, div.', opts.className, ' td{margin:0;font-family:inherit;font-size:inherit;font-weight:inherit;font-style:inherit;font-variant:inherit;}',
				'div.', opts.className, ' td.tip-bg-image span{display:block;font:1px/1px sans-serif;height:', opts.bgImageFrameSize, 'px;width:', opts.bgImageFrameSize, 'px;overflow:hidden;}',
				'div.', opts.className, ' td.tip-right{background-position:100% 0;}',
				'div.', opts.className, ' td.tip-bottom{background-position:100% 100%;}',
				'div.', opts.className, ' td.tip-left{background-position:0 100%;}',
				'div.', opts.className, ' div.tip-inner{background-position:-', opts.bgImageFrameSize, 'px -', opts.bgImageFrameSize, 'px;}',
				'div.', opts.className, ' div.tip-arrow{visibility:hidden;position:absolute;overflow:hidden;font:1px/1px sans-serif;}',
			'</style>'].join('')).appendTo('head');

        return this.each(function () {
            new $.Poshytip(this, opts);
        });
    }

    // default settings
    $.fn.poshytip.defaults = {
        content: '[title]', // content to display ('[title]', 'string', element, function(updateCallback){...}, jQuery)
        className: 'tipWrap', // class for the tips
        bgImageFrameSize: 10, 	// size in pixels for the background-image (if set in CSS) frame around the inner content of the tip
        showTimeout: 100, 	// timeout before showing the tip (in milliseconds 1000 == 1 second)
        hideTimeout: 500, 	// timeout before hiding the tip
        showOn: 'hover', // handler for showing the tip ('hover', 'focus', 'none') - use 'none' to trigger it manually
        alignTo: 'cursor', // align/position the tip relative to ('cursor', 'target')
        alignX: 'right', // horizontal alignment for the tip relative to the mouse cursor or the target element
        // ('right', 'center', 'left', 'inner-left', 'inner-right') - 'inner-*' matter if alignTo:'target'
        alignY: 'top', 	// vertical alignment for the tip relative to the mouse cursor or the target element
        // ('bottom', 'center', 'top', 'inner-bottom', 'inner-top') - 'inner-*' matter if alignTo:'target'
        offsetX: -22, 	// offset X pixels from the default position - doesn't matter if alignX:'center'
        offsetY: 18, 	// offset Y pixels from the default position - doesn't matter if alignY:'center'
        allowTipHover: true, 	// allow hovering the tip without hiding it onmouseout of the target - matters only if showOn:'hover'
        followCursor: false, 	// if the tip should follow the cursor - matters only if showOn:'hover' and alignTo:'cursor'
        fade: true, 	// use fade animation
        slide: true, 	// use slide animation
        slideOffset: 8, 	// slide animation offset
        showAniDuration: 300, 	// show animation duration - set to 0 if you don't want show animation
        hideAniDuration: 300		// hide animation duration - set to 0 if you don't want hide animation
    };

})(jQuery);

/*
用法:
1. 在控件上应用约束. 
2. 在事件上调用方法: if ( jv.chk() == false ) return false ;

应用的约束示例:
1.简单通用的函数写法:
<input type="text" chk="function(val){ if ( val.length < 10 ) return '长度必须大于10 .' ;}" />
<input type="text" chk="function(val){ return MySelfFunction(val) ; }" />
2.简单通用的正则表达式写法 , 关于 \  ,写在HTML 元素的属性值上,不必使用两个 \\ , 使用一个 \ 表示即可.  但是如果在 JS 的 String 变量中, 必须使用两个 \\ 表示一个 \
必须指定 chkmsg ,语义为, 如果正则表达式命中, 则显示消息 .  命中条件可以是 == , 也可以是 !=
<input type="text" chk="reg == ^\d+$" chkmsg= "不能是数字." />
<input type="text" chk="reg != ^\d+$" chkmsg= "必须是数字." />
3.指定表达式,这种写法是应用的最常用的验证,代码量最少.
<input type="text" chk="int" />
<input type="text" chk="int{1,5}" />    指定长度范围
<input type="text" chk="int{1,5}(8,19999)" /> 指定长度范围和数值范围, 在这里长度范围就无意义了.
<input type="text" chk="int(8,199)" chkmsg="需要的数值范围是 8 - 199 "/> 如不指定消息 , 会弹出默认消息 .

<input type="text" chk="number" />  //可以是小数.
<input type="text" chk="date(2000-10-1,2010-10-1)" />  
<input type="text" chk="time(8:00,18:00)" />  
<input type="text" chk="datetime(8:00,18:00)" />  

<input type="text" chk="enum(提交,审批,通过,驳回,归档)" />  //枚举

<input type="text" chk="int"  chkmsg=""  chkval="" />
*/
//控件三属性: chk, chkval , chkmsg
jv.chk_one = function (setting) {
    var Alert = function (alertOption) {
        var p = {
            con: false,
            msg: false
        };
        p = $.extend(p, alertOption);
        //        return alert(p.msg);
        if ($.fn.mytip) {
            var con = $(p.con);
            if (con.css("textAlign") == "right") { con.mytip(p.msg, { alignX: "inner-right", arrow: "-right" }); }
            else {
                con.mytip(p.msg);
            }
        }
        else {
            alert(p.msg);
        }
    };

    var UnAlert = function (obj) {
        $(obj).poshytip('destroy');
    };

    if (!setting) setting = {};

    var chkCon = setting.con || jv.GetDoer(), jchkCon = $(chkCon);

    if (chkCon.disabled || (chkCon.type == "hidden") || !jchkCon.is(":visible")) return true;

    var option = {
        con: chkCon,         //真正有用的参数
        callback: false,    //真正有用的参数
        chk: (jchkCon.attr("chk") || "").trim(),
        chkval: (jchkCon.attr("chkval") || "").trim(),
        chkmsg: (jchkCon.attr("chkmsg") || "").trim(),
        boxdy: false
    };
    option = $.extend(option, setting);

    //if (!option) option = {};

    //可以理角为 if regexp then regmsg end if ; 正则表达式,必须以字符串格式传入(不能添加""),也可以 Perl 格式 /\d/ .
    var regFunc = function (regOptions) {
        var p = {
            expchk: false,
            val: false,
            msg: false,
            con: false
        };
        p = $.extend(p, regOptions);
        var regLastTemp = p.expchk.slice(3).trim();
        var expSignEqual = regLastTemp.slice(0, 2) == "==";
        var expval = regLastTemp.slice(2).trim();

        var reg = new RegExp(expval);
        if (reg.test(p.val) == expSignEqual) {
            Alert(p);
            return false;
        }
        else {
            UnAlert(p.con);
        }
        return true;
    };

    var funcFunc = function (funcOptions) {
        var p = {
            expchk: false,
            val: false,
            msg: false,
            con: false
        };
        p = $.extend(p, funcOptions);


        var expval = p.expchk;
        var res = eval("eval (" + expval + ")")(p.val, p.msg, { originalEvent: true, target: p.con }); // eval("(" + expval + ")")(val, { originalEvent: true, target: jv.GetDoer() });
        if (res === true || jv.IsNull(res)) {
            UnAlert(p.con);
        }
        else if (res === false) {
            //如果在函数中调用 chk_one .
            return false;
        }
        else if (res.length > 0) {
            Alert({ con: p.con, msg: res });
            return false;
        }
        return true;
    };

    //expchk 是纯粹的约束
    var expLimit = function (expOptions) {
        var p = {
            dataType: false,
            expchk: false,
            val: false,
            msg: false,
            con: false
        };
        p = $.extend(p, expOptions);

        if (!p.expchk) return true;

        var getBetween = function (partStr, valProc) {
            var ranges = partStr.split(",");
            var from = valProc ? valProc(ranges[0]) : (ranges[0] || "");
            var to = valProc ? valProc(ranges[1]) : (ranges[1] || "");

            if (from.length && to.length) {
                if (from > to) {
                    var temp = from;
                    from = to;
                    to = temp;
                }
            }
            return { from: from, to: to };
        };
        var procLength = function (chkExp) {
            //先处理{}的情况 . 匹配以 } 结尾，前面有至少一个字符，再前面有{ 的正则。
            if (/\{.+\}/g.test(chkExp)) {
                //取出里面的内容
                var part = chkExp.match(/[^\{^\}]+/g)[0];

                var between = getBetween(part, parseInt), from = between.from, to = between.to, fromValue = parseInt(from), toValue = parseInt(to);

                var valLen = p.val.length;

                if (valLen < fromValue || (valLen > toValue)) {
                    Alert({ con: p.con, msg: p.msg || "长度限定在: " + chkExp + " 之间!" });
                    return false;
                }
                else if (valLen < fromValue) {
                    Alert({ con: p.con, msg: p.msg || "长度限定必须 ≥ " + from + "!" });
                    return false;
                }
                else if (valLen > toValue) {
                    Alert({ con: p.con, msg: p.msg || "长度限定必须 ≤ " + to + "!" });
                    return false;
                }
            }

            return true;
        };

        var procRange = function (chkExp) {
            //先处理（）的情况 . 匹配以 ） 结尾，前面有至少一个字符，再前面有（ 的正则。
            if (/\(.+\)/g.test(chkExp)) {
                //取出里面的内容
                var part = chkExp.match(/[^\(^\)]+/g)[0];

                var between = getBetween(part, jv.typeIsNumberType(p.dataType) ? parseFloat : null), from = between.from, to = between.to;


                if (p.dataType == "datetime" || p.dataType == "date") {
                    from = from.getDate();
                    to = to.getDate();
                    p.val = p.val.getDate();
                }
                else if (p.dataType == "time") {
                    from = ("2000-1-1 " + from).getDate();
                    to = ("2000-1-1 " + to).getDate();
                    p.val = ("2000-1-1 " + p.val).val.getDate();
                }
                else if (p.dataType == "int" || p.dataType == "Int32" || p.dataType == "UInt32") {
                    if (p.val.toString().length > 12) {
                        Alert({ con: p.con, msg: p.msg || "超出数字类型(" + p.dataType + ")的范围!" });
                        return false;
                    }
                    from = parseInt(from);
                    to = parseInt(to);
                    p.val = parseInt(p.val);
                }
                else if (p.dataType == "Int16" || p.dataType == "UInt16") {
                    if (p.val.toString().length > 6) {
                        Alert({ con: p.con, msg: p.msg || "超出数字类型(" + p.dataType + ")的范围!" });
                        return false;
                    }
                    from = parseInt(from);
                    to = parseInt(to);
                    p.val = parseInt(p.val);
                }
                else if (p.dataType == "number" || p.dataType == "Single") {
                    proc = function (v) { return parseFloat(v); };
                }
                else if (p.dataType == "Byte" || p.dataType == "SByte") {
                    if (p.val.toString().length > 3) {
                        Alert({ con: p.con, msg: p.msg || "超出数字类型(" + p.dataType + ")的范围!" });
                        return false;
                    }
                    proc = function (v) { return parseInt(v); };
                }
                else if (p.dataType == "Int64" || p.dataType == "Decimal" || p.dataType == "Double" || p.dataType == "UInt64" || p.dataType == "VarNumeric" || p.dataType == "Currency" ||
                    p.dataType == "Binary" || p.dataType == "Decimal" || p.dataType == "Guid" || p.dataType == "Object" || p.dataType == "Xml" || p.dataType == "Boolean") {
                    // js 本身处理不了大数据.
                    return true;
                }


                if (p.val < from || p.val > to) {
                    Alert({ con: p.con, msg: p.msg || "范围限定在: " + chkExp + " 之间!" });
                    return false;
                }
            }
            return true;
        };
        var sects = p.expchk.match(/\(|\)|\{|\}/g), retVal = true;
        $.each(sects, function (i, d) {
            if (i % 2 == 0) return;
            if (d == "}") {
                retVal &= procLength(p.expchk.match(/\{.+\}/g)[0]);
            }
            else if (d == ")") {
                retVal &= procRange(p.expchk.match(/\(.+\)/g)[0]);
            }

            if (!retVal) return retVal;
        });
        //-------------------------------------
        return retVal;
    };

    // 与 repFunc 不同, 它的表义是: 应该是该约束. 否则显示消息 .
    var expFunc = function (expOptions) {
        var p =
        {
            expchk: false,
            val: false,
            msg: false,
            con: false
        };
        p = $.extend(p, expOptions);

        // 类型:int,number,date,datetime,time,email,enum
        // 值范围 : (100,500) , "," 优先.
        // 长度范围 {1,2} , 正则语法
        // int{1,2}(9,17), int(9,17){1,2}, email{3,8},date(2000-8-9,2010-9-10),enum(Yes,No,None)
        var expDateTimeType = p.expchk.slice(0, 8).trim().toLowerCase();
        var exp6Type = expDateTimeType.slice(0, 6);
        var exp4Type = exp6Type.slice(0, 4);
        var exp3Type = exp4Type.slice(0, 3);
        var expval4 = p.expchk.slice(4).trim();

        if (expDateTimeType == "datetime") {
            var res = regFunc({ expchk: "reg!=^\\d{1,4}(\\-|\\/|\\.|年)\\d{1,2}(\\-|\\/|\\.|月)\\d{1,2}日? \\d{1,2}:\\d{1,2}:\\d{1,2}$", val: p.val, msg: p.msg || "不正确的日期时间格式!", con: p.con });
            if (res == false) return false;
            return expLimit({ dataType: expDateTimeType, expchk: p.expchk.slice(8).trim(), val: p.val, con: p.con, msg: p.msg });
        }
        else if (exp6Type == "number") {
            var res = regFunc({ expchk: "reg!=^[+-]?\\d*(\\.$|\\d*$|(\\.?\\d*$))", val: p.val, msg: p.msg || "不正确的数字格式!", con: p.con });
            if (res == false) return false;
            return expLimit({ dataType: exp6Type, expchk: p.expchk.slice(6).trim(), val: p.val, con: p.con, msg: p.msg });
        }
        else if (exp4Type == "email") {
            var res = regFunc({ expchk: "reg!=^\\w+((-\\w+)|(\\.\\w+))*\\@[A-Za-z0-9]+((\\.|-)[A-Za-z0-9]+)*\\.[A-Za-z0-9]+$", val: p.val, msg: p.msg || "不正确的电子邮件格式!", con: p.con });
            if (res == false) return false;
            return expLimit({ dataType: exp4Type, expchk: expval4, val: val, con: con, msg: p.msg });
        }
        else if (exp4Type == "date") {
            var res = regFunc({ expchk: "reg!=^\\d{1,4}(\\-|\\/|\\.|年)\\d{1,2}(\\-|\\/|\\.|月)\\d{1,2}日?$", val: p.val, msg: p.msg || "不正确的日期格式!", con: p.con });
            if (res == false) return false;
            return expLimit({ dataType: exp4Type, expchk: expval4, val: p.val, con: p.con, msg: p.msg });
        }
        else if (exp4Type == "time") {
            var res = regFunc({ expchk: "reg!=\\d{1,2}:\\d{1,2}:\\d{1,2}$", val: p.val, msg: p.msg || "不正确的时间格式!", con: p.con });
            if (res == false) return false;
            return expLimit({ dataType: exp4Type, expchk: expval4, val: p.val, con: p.con, msg: p.msg });
        }
        else if (exp4Type == "enum") {
            var expContent = p.expchk.split(new RegExp("[\(\)]"))[1].split(",");

            if (expContent.indexOf(p.val) < 0) {
                Alert({ con: p.con, msg: p.msg || "不在枚举范围内: " + p.expchk + " 之间!" });
                return false;
            }
        }
        else if (exp3Type == "int") {
            var res = regFunc({ expchk: "reg!=^[+-]?\\d+$", val: p.val, msg: p.msg || "需要整数!", con: p.con });
            if (res == false) return false;
            return expLimit({ dataType: exp3Type, expchk: p.expchk.slice(3).trim(), val: p.val, con: p.con, msg: p.msg });
        }
        else {
            // 没有类型限制的 字符串
            return expLimit({ expchk: p.expchk, val: p.val, con: p.con, msg: p.msg });
        }
    };

    var dbChk = function (expOptions) {
        var p = {
            expchk: false,
            val: false,
            msg: false,
            con: false
        };
        p = $.extend(p, expOptions);

        var dbType = p.con.getAttribute("dbtype");
        var maxLen = parseInt(p.con.getAttribute("dblen"));

        if (dbType == "Int32") {
            var res = regFunc({ expchk: "reg!=^[+-]?\\d+$", val: p.val, msg: p.msg || "需要32位整数!", con: p.con });
            if (res == false) return false;
            if (expLimit({ dataType: "int", expchk: "(-2147483648,2147483647)", val: p.val, con: p.con, msg: p.msg }) == false) return false;
        } else if (dbType == "Int16") {
            var res = regFunc({ expchk: "reg!=^[+-]?\\d+$", val: p.val, msg: p.msg || "需要16位整数!", con: p.con });
            if (res == false) return false;
            if (expLimit({ dataType: "int", expchk: "(-32768,32767)", val: p.val, con: p.con, msg: p.msg }) == false) return false;
        } else if (dbType == "Byte") {
            var res = regFunc({ expchk: "reg!=^[+]?\\d+$", val: p.val, msg: p.msg || "需要8位整数!", con: p.con });
            if (res == false) return false;
            if (expLimit({ dataType: "int", expchk: "(0,255)", val: p.val, con: p.con, msg: p.msg }) == false) return false;
        } else if (dbType == "SByte") {
            var res = regFunc({ expchk: "reg!=^[+-]?\\d+$", val: p.val, msg: p.msg || "需要8位整数!", con: p.con });
            if (res == false) return false;
            if (expLimit({ dataType: "int", expchk: "(-128,127)", val: p.val, con: p.con, msg: p.msg }) == false) return false;
        } else if (dbType == "UInt16") {
            var res = regFunc({ expchk: "reg!=^[+]?\\d+$", val: p.val, msg: p.msg || "需要16位整数!", con: p.con });
            if (res == false) return false;
            if (expLimit({ dataType: "int", expchk: "(-32768,32767)", val: p.val, con: p.con, msg: p.msg }) == false) return false;
        } else if (dbType == "UInt32") {
            var res = regFunc({ expchk: "reg!=^[+]?\\d+$", val: p.val, msg: p.msg || "需要32位整数!", con: p.con });
            if (res == false) return false;
            if (expLimit({ dataType: "int", expchk: "(0,4294967295)", val: p.val, con: p.con, msg: p.msg }) == false) return false;
        } else if (dbType == "Date") {
            var res = regFunc({ expchk: "reg!=^\\d{1,4}(\\-|\\/|\\.|年)\\d{1,2}(\\-|\\/|\\.|月)\\d{1,2}日?$", val: p.val, msg: p.msg || "不正确的日期格式!", con: p.con });
            if (res == false) return false;
        } else if (dbType == "DateTime" || dbType == "DateTime2" || dbType == "DateTimeOffset") {
            var res = regFunc({ expchk: "reg!=^\\d{1,4}(\\-|\\/|\\.|年)\\d{1,2}(\\-|\\/|\\.|月)\\d{1,2}日? \\d{1,2}:\\d{1,2}:\\d{1,2}$", val: p.val, msg: p.msg || "不正确的日期时间格式!", con: p.con });
            if (res == false) return false;
        } else if (dbType == "Time") {
            var res = regFunc({ expchk: "reg!=\\d{1,2}:\\d{1,2}:\\d{1,2}$", val: p.val, msg: p.msg || "不正确的时间格式!", con: p.con });
            if (res == false) return false;
        } else if (dbType == "AnsiString" || dbType == "AnsiStringFixedLength") {
            if (p.val.byteLen() > maxLen) {
                p.msg = p.msg || "超出了最大数据长度.";
                Alert(p);
                return false;
            }
        } else if (dbType == "String" || dbType == "StringFixedLength") {
            if (p.val.length > maxLen) {
                p.msg = p.msg || "超出了最大数据长度.";
                Alert(p);
                return false;
            }
        }

        p.expchk = p.expchk.slice(1);
        if (p.expchk && jv.chk_one(p) == false) return false;

        UnAlert(p.con);
        return true;
    };

    //    option.chk = option.chk || $(chkCon).attr("chk").trim();
    //    option.chkval = option.chkval || $(chkCon).attr("chkval") || "";
    //    option.chkmsg = option.chkmsg || $(chkCon).attr("chkmsg");

    var functionType = option.chk.slice(0, 8).toLowerCase();
    var regType = functionType.slice(0, 3).toLowerCase();
    var shortFunctionType = regType.slice(0, 1);
    var chkval;


    if (option.chkval) {
        if (option.chkval.slice(0, 8) == "function") {
            chkval = eval("eval (" + option.chkval + ")")();
        }
        else if (option.chkval.slice(0, 1) == ":") {
            chkval = eval("eval (function(){" + option.chkval.slice(1) + "})")();
        }
        else {
            var chk_val_con = $(option.chkval, jv.boxdy());
            if (!chk_val_con.length) {
                chk_val_con = $(option.chkval, jv.boxdy());
            }

            chkval = chk_val_con.val();
        }
    }
    else {
        chkval = $(chkCon, jv.boxdy()).val();
    }

    chkval = chkval.trim();

    if (shortFunctionType == ":") {
        option.chk = "function(val,msg){" + option.chk.slice(1) + "}";
    }

    if ((functionType == "function") || (shortFunctionType == ":")) {
        if (funcFunc({ expchk: option.chk, val: chkval, msg: option.chkmsg, con: chkCon }) == false)
            return false;
    }
    else if (regType == "reg") {
        if (regFunc({ expchk: option.chk, val: chkval, msg: option.chkmsg, con: chkCon }) == false)
            return false;
    }
    else if (shortFunctionType == "$") {
        if (dbChk({ expchk: option.chk, val: chkval, msg: option.chkmsg, con: chkCon }) == false) return false;
    }
    else {
        if (expFunc({ expchk: option.chk, val: chkval, msg: option.chkmsg, con: chkCon }) == false)
            return false;
    }

    if (option.callback) {
        var ret = option.callback(option);
        if (ret) {
            Alert({ con: option.con, msg: ret });
            return false;
        }
    }

    return true;
}

jv.chk = function (option) {
    if (!option) option = {};
    var container = option.boxdy || jv.boxdy();
    var retVal = true;
    var chkSenseWord = function (option) {
        var p = $.extend({ con: false, chkval: false }, option);
        if (!jv.SenseWord) return false;

        var senseWord = "";
        $(jv.SenseWord).each(function (i, d) {
            if (p.chkval.indexOf(d) >= 0) {
                senseWord = d; return false;
            }
        });
        if (senseWord) {
            return "包含关键词:" + senseWord;
        }
        else return false;
    };

    var chkOne = function (con) {
        var ro = jv.chk_one({ con: con, callback: chkSenseWord });

        if (ro) {
            $(con).poshytip('destroy');
        }
        return ro;
    };


    var firstChkCon;
    var $container = $(container);
    $container.filter("[chk]").add($container.find("[chk]")).each(function (i, d) {
        retVal &= chkOne(d);
        if (!firstChkCon && !retVal) {
            var jd = $(d);
            if (jd.attr("disabled") || (jd.attr("type") == "hidden") || !jd.is(":visible")) return;
            firstChkCon = d;
        }
    }).each(function (i, d) {
        $($(d).attr("chkval") || d).unbind("change.jvchk").bind("change.jvchk", function (ev) { chkOne(d); });
    });


    if (firstChkCon) {
        try {
            firstChkCon.focus();
        } catch (e) { }
    }

    if (!retVal) {
        jv.UnOneClick();
    }

    jv.RegisteUnload("chk", function () {
        jv.boxdy().find("[chk]").each(function (i, d) {
            $(d).poshytip('destroy');
        })
    });

    return retVal;
};
