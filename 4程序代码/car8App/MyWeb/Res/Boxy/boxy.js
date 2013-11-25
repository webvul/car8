/**
* Boxy 0.1.4 - Facebook-style dialog, with frills
*
* (c) 2008 Jason Frame
* Licensed under the MIT License (LICENSE)
* jQuery plugin
*
* Options:
*   message: confirmation message for form submit hook (default: "Please confirm:")
* 
* Any other options - e.g. 'clone' - will be passed onto the boxy constructor (or
* Boxy.load for AJAX operations)
* 可以在弹出的页面里运行 Javascript脚本。但是不能引用外部文件。
*
*/
//jQuery.fn.boxy = function (options) {
//    options = options || {};
//    return this.each(function () {
//        var node = this.nodeName.toLowerCase(), self = this;
//        if (node == 'a') {
//            jQuery(this).click(function () {
//                var active = Boxy.linkedTo(this),
//                    href = this.getAttribute('href'),
//                    localOptions = jQuery.extend({ actuator: this, title: this.title }, options);

//                if (active) {
//                    active.show();
//                } else if (href.indexOf('#') >= 0) {
//                    var content = jQuery(href.substr(href.indexOf('#'))),
//                        newContent = content.clone(true);
//                    content.remove();
//                    localOptions.unloadOnHide = true;
//                    new Boxy(newContent, localOptions);
//                } else { // fall back to AJAX; could do with a same-origin check
//                    if (!localOptions.cache) localOptions.unloadOnHide = true;
//                    Boxy.load(this.href, localOptions);
//                }

//                return false;
//            });
//        } else if (node == 'form') {
//            jQuery(this).bind('submit.boxy', function () {
//                Boxy.confirm(options.message || 'Please confirm:', function () {
//                    jQuery(self).unbind('submit.boxy').submit();
//                });
//                return false;
//            });
//        }
//    });
//};

//
// Boxy Class
jv.FindNotTempDialogSrc = function (obj) {
    var retVal, container = jv.boxdy(obj);
    if (container.length == 1) return obj;


    container.each(function () {
        var self = $(this);
        //loadView 情况。
        if (!self.data('boxy')) return true;

        //Boxy.ask 情况
        if (self.data("boxy").options["tempDialog"]) {
            retVal = self.data("srcForBoxy");
            return false;
        }
        else {
            //如果 Boxy.load(url)
            return false;
        }
    });

    if (retVal) return retVal;
    else return obj;
}

function Boxy(element, options) {
    //当前窗体最大化.

    // 是否扩展到全屏开关. 
    //    if (false && top.document != document && !$(document).data("oriPos")) {
    //        $(document).data("oriPos", {
    //            offset: $(window.frameElement).offset(),
    //            size: { width: $(window.frameElement).width(), height: $(window.frameElement).height() },
    //            position: $(window.frameElement).css("position"),
    //            obj: this
    //        });
    //        var jtopbody = $(top.document.body),
    //        joffset =
    //        {
    //            paddingTop: parseInt(jtopbody.css("paddingTop")),
    //            paddingLeft: parseInt(jtopbody.css("paddingLeft")),
    //            paddingBottom: parseInt(jtopbody.css("paddingBottom")),
    //            paddingRight: parseInt(jtopbody.css("paddingRight")),
    //            marginTop: parseInt(jtopbody.css("marginTop")),
    //            marginLeft: parseInt(jtopbody.css("marginLeft")),
    //            marginBottom: parseInt(jtopbody.css("marginBottom")),
    //            marginRight: parseInt(jtopbody.css("marginRight"))
    //        };
    //        $(window.frameElement).width(0).height(0).css("position", "absolute")
    //                .offset({ top: Math.ceil(joffset.marginTop + joffset.paddingTop), left: Math.ceil(joffset.marginLeft + joffset.paddingLeft) })
    //                .width(jtopbody.width() + joffset.paddingLeft + joffset.paddingRight)
    //                .height($(top.document).height() - joffset.marginTop - joffset.marginBottom - joffset.paddingTop - joffset.paddingBottom)
    //                ;
    //    }

    //    element = $(element);
    //    element.data("boxyParent", element.parent());
    //    element.data("boxyParentIndex", element.index());

    //出现了循环引用。
    this.boxy = jQuery(Boxy.WRAPPER);
    jQuery.data(this.boxy[0], 'boxy', this);

    this.visible = false;
    this.options = jQuery.extend({}, Boxy.DEFAULTS, options || {});
    this.options["unloadOnHide"] = true;
    if (this.options.modal) {
        this.options = jQuery.extend(this.options, { center: true });
    }

    if (this.options.actuator) {
        jQuery.data(this.options.actuator, 'active.boxy', this);
    }

    if (options.width) this.boxy.find(".boxy-inner").width(options.width);
    if (options.height) this.boxy.find(".boxy-inner").height(options.height);

    this.setContent(element || "<div></div>");
    this._setupTitleBar();

    this.boxy.css('display', 'none').appendTo(document.body);
    //    if (this.options["nullRequest"] != false) {
    //    }
    //    else {
    //    }
    this.options["tempDialog"] = true;

    this.boxy.addClass("jvRequest_waite").data("srcForBoxy", jv.FindNotTempDialogSrc(jv.GetDoer()));
    jv.AddRequest({ boxy: this.boxy });

    this.toTop();
    //this.boxy.MyCorner({ radius: 3, backColor: "rgb(199,194,181)" });
    this.center();



    if (this.options.show) this.show();
};

Boxy.EF = function () { };

jQuery.extend(Boxy, {
    WRAPPER: "<div class='boxy-wrapper'><div class='boxy-inner'></div></div>",

    DEFAULTS: {
        opacity: 0.5,
        title: null,           // titlebar text. titlebar will not be visible if not set.
        closeable: true,           // display close link in titlebar?
        draggable: true,           // can this dialog be dragged?
        clone: false,          // clone content prior to insertion into dialog?
        actuator: null,           // element which opened this dialog
        center: true,           // center dialog in viewport?
        hideAnimateTime: 0,
        show: true,           // show dialog immediately?
        modal: false,          // make dialog modal?
        fixed: true,           // use fixed positioning, if supported? absolute positioning used otherwise
        closeText: '关闭',      // text to use for default close link
        closeButton: false,    //默认按钮, 即用户点关闭时, 要点击的按钮.
        unloadOnHide: true,          // should this dialog be removed from the DOM after being hidden? 该参数已被改造成 永远是 true 。
        clickToFront: false,          // bring dialog to foreground on any click (not just titlebar)?
        behaviours: Boxy.EF,        // function used to apply behaviours to all content embedded in dialog.
        afterDrop: Boxy.EF,        // callback fired after dialog is dropped. executes in context of Boxy instance.
        afterShow: Boxy.EF,        // callback fired after dialog becomes visible. executes in context of Boxy instance.
        beforeHide: Boxy.EF,
        afterHide: Boxy.EF,        // callback fired after dialog is hidden. executed in context of Boxy instance.
        beforeUnload: Boxy.EF,       // callback fired after dialog is unloaded. executed in context of Boxy instance.
        afterUnload: Boxy.EF         // callback fired after dialog is unloaded. executed in context of Boxy instance.
    },

    DEFAULT_X: 50,
    DEFAULT_Y: 50,
    dragConfigured: false, // only set up one drag handler for all boxys
    resizeConfigured: false,
    dragging: null,
    load: function () {
        //四参： url, options, LoadedCallBack, ReturnCallback,event
        var url, options, LoadedCallBack, ReturnCallback;
        if (arguments.length > 1) {
            url = arguments[0];
            options = arguments[1];
            LoadedCallBack = arguments[2];
            ReturnCallback = arguments[3];

            options.url = url;
            options.LoadedCallBack = LoadedCallBack;
            options.ReturnCallback = ReturnCallback;
        }
        else {
            options = arguments[0];
            url = options.url;
            LoadedCallBack = options.LoadedCallBack;
            ReturnCallback = options.ReturnCallback;
        }

        options = options || {};
        options["unloadOnHide"] = true;
        options["url"] = url;
        options["ReturnCallback"] = ReturnCallback;
        if (!("width" in options)) options["width"] = 701;
        //        options["nullRequest"] = true;

        var bxy = new Boxy($("<div><div class='loading'>加载中</div></div>"), $.extend({}, options, { width: false, height: false }));
        if (!url) {
            if (LoadedCallBack) LoadedCallBack(bxy);
            return bxy;
        }
        if (options.iframe) {
            window._BoxyIframeLoad_ = function () { LoadedCallBack(bxy, bxy.boxy.find("iframe")[0].contentDocument) };

            var maxViewPort = Boxy._viewport(options);

            bxy.moveToX(-3000).moveToY(-3000);

            bxy.setContent('<iframe src="' + url + '" style="padding:0px;margin:0px;border-width:0px;width:100%;height:99.9%;" onload="window._BoxyIframeLoad_();"/>', options);

            bxy.getContent()
                .width(options.width || maxViewPort.suitWidth)
                .height(options.height || maxViewPort.suitHeight)
                .css("maxHeight", maxViewPort.height + "px");

            bxy.center();

            return bxy;
        }

        //重新设置接收 Reqeust信号.
        bxy.boxy.addClass("jvRequest_waite");

        var ajax = {
            url: url, type: 'GET', dataType: 'html', cache: false,
            beforeSend: function (xhr) {
                //记录弹出Boxy 的源. 
                //bxy.boxy.data("srcForBoxy", jv.FindNotTempDialogSrc(jv.GetDoer()));
                jv.CreateEventSource(xhr, bxy.boxy[0]);
                xhr.setRequestHeader("MyTitle", "Json");
            },
            success: function (html) {
                if (jv.HasValue(html) == false) {
                    bxy.options["loaderror"] = true;
                    //                    bxy.boxy.removeClass("jvRequest_waite");
                    bxy.boxy.find(".boxy-content>div").html("<label style='color:red'>没有内容！</label>");
                    return;
                }
                else if (html.charAt(0) == '{') {
                    var res = null;
                    try
                    { res = $.parseJSON(html); }
                    catch (e) { }
                    if (jv.IsNull(res) == false && res.data == "power") {
                        bxy.options["loaderror"] = true;
                        //                        bxy.boxy.removeClass("jvRequest_waite");
                        var msg = "<div style='color:red'>" + res.msg + "</div><br /> 请联系管理员！";
                        bxy.setContent($("<div style='padding:20px'>" + msg + "</div>"), options);
                        bxy.center();
                        return;
                    }
                }

                if (options["loading"]) options["loading"](bxy);
                bxy.options["tempDialog"] = false;


                //                var div = document.createElement("div");
                //                document.body.appendChild(div);
                //                div.innerHTML = html;

                //                html = $(div.childNodes);

                html = $(html);


                var htmlFilter = jv.myFilter(html, options.filter);

                var maxViewPort = Boxy._viewport(options);

                bxy.moveToX(-3000).moveToY(-3000);

                bxy.setContent(htmlFilter, options);

                bxy.getContent()
                    .width(maxViewPort.suitWidth)
                    .height(maxViewPort.suitHeight)
                    .css("maxHeight", maxViewPort.height + "px");


                //                bxy.center();

                if (LoadedCallBack) LoadedCallBack(bxy);


                jv.execHtml(html, null, bxy.boxy[0]);
                bxy.center();
                //                bxy.boxy.find(".boxy-content").removeClass("FillHeight");

                jv.page().boxy = bxy.boxy;

            },
            error: function (ev) {
                bxy.options["loaderror"] = true;
                //                bxy.boxy.removeClass("jvRequest_waite");
                var msg = jv.getErrorFromPage(ev.responseText) || "服务器出现错误!";

                var loginIndex = msg.indexOf("登录");
                if (loginIndex >= 0) {
                    msg = msg.slice(0, loginIndex) + '<a class="Link" onclick="javascript:top.document.location=' + (jv.url().root) + 'Login.aspx"> 登录 </a>' + msg.slice(loginIndex + 2);
                }

                var msg = '<div><div class="question" style="color:red"><div class="icon error" /> <p class="Wrap">'
                + msg +
                '</p></div><div class="answers">请检查数据正确性,或联系运维管理员.</div></div>';

                bxy.setContent(msg, options);
                bxy.center();
            }
        };

        jQuery.each(['type', 'cache'], function () {
            if (this in options) {
                ajax[this] = options[this];
                delete options[this];
            }
        });

        jQuery.ajax(ajax);
    },

    // allows you to get a handle to the containing boxy instance of any element
    // e.g. <a href='#' onclick='alert(Boxy.get(this));'>inspect!</a>.
    // this returns the actual instance of the boxy 'class', not just a DOM element.
    // Boxy.get(this).hide() would be valid, for instance.
    get: function (ele) {
        var p = jQuery(ele);
        if (!p.hasClass("boxy-wrapper")) p = p.parents('.boxy-wrapper:first');
        return p.length ? jQuery.data(p[0], 'boxy') : null;
    },

    //模态获取最上层的 Boxy
    getOne: function (bod) {
        var mx = 0,
         mb = null;

        if (!bod) bod = window;
        var con = $(".boxy-wrapper");

        con.each(function () {
            if (this.style.display == "none") return;
            var self = $(this);
            if (self.css("zIndex") > mx) {

                if (self.hasClass("Ignore")) { return true; }

                mx = self.css("zIndex");
                mb = this;
            }
        });


        if (mb) return jQuery.data(mb, 'boxy');
        else if (bod.parent && bod != bod.parent) {
            return Boxy.getOne(bod.parent);
        } return null;
    },

    // returns the boxy instance which has been linked to a given element via the
    // 'actuator' constructor option.
    linkedTo: function (ele) {
        return jQuery.data(ele, 'active.boxy');
    },

    // displays an alert box with a given message, calling optional callback
    // after dismissal.
    alert: function (message, callback, options) {
        return Boxy.ask(message, ['OK'], callback, $.extend({ title: "!" }, options));
    },
    msg: function (message, liveTime, callback, options) {
        var bo = Boxy.ask(message, ['OK'], callback, options);

        if (parseInt(liveTime) > 0) {
            setTimeout(function () {
                bo.hide();
            }, parseInt(liveTime));
        }
        return bo;
    },

    // displays an alert box with a given message, calling after callback iff
    // user selects OK.
    confirm: function (message, after, options) {
        return Boxy.ask(message, ['OK', 'Cancel'], function (response) {
            if (response == 'OK') after();
        }, options);
    },

    // asks a question with multiple responses presented as buttons
    // selected item is returned to a callback method.
    // answers may be either an array or a json. if it's an array, the
    // the callback will received the selected value. if it's a json,
    // you'll get the corresponding key.
    ask: function (question, answers, callback, options) {
        return Boxy.html('<div class="question">' + question + '</div>', answers, callback, options);
    },
    html: function (htmlContent, answers, callback, options) {

        options = jQuery.extend({ modal: true, closeable: true, title: "?" },
                                options || {},
                                { show: true, unloadOnHide: true, center: true });
        var content = "";
        if (jv.getType(htmlContent) == "string") {
            content = htmlContent;
        }
        else if (htmlContent instanceof jQuery) {
            htmlContent.each(function () {
                content += this.outerHTML;
            });
        }
        else {
            content += htmlContent.outerHTML;
        }

        var body = '<div>' + content + "<div class='answers'>";

        // ick
        if ($.isArray(answers)) {
            var map = {};
            $(answers).each(function (i, d) { map[d] = d; });
            answers = map;
        }

        var buttons = [];
        for (var key in answers) {
            body += " <input type='button' value='{0}' class='button' {1}/>".format(answers[key], answers[key] == key ? "" : "data='{0}'".format(key));
        }

        body += "</div></div>";

        var boxy = new Boxy(body, options);
        $("input[type=button]", boxy.boxy).each(function (i, d) {
            $(d).bind("click", { bxy: boxy }, function (e) {
                var clicked = this;
                if (callback) callback(clicked.data || clicked.value, e.data.bxy, e);
                e.data.bxy.hide();
            });
        });
        $("input[type=button]:first", boxy.boxy).focus();
        //        if (options.width) { boxy.resize(options.width); }
        return boxy;
    },

    // returns true if a modal boxy is visible, false otherwise
    isModalVisible: function () {
        return jQuery('.boxy-modal-blackout').length > 0;
    },

    _u: function () {
        for (var i = 0, argLen = arguments.length; i < argLen; i++)
            if (typeof arguments[i] != 'undefined') return false;
        return true;
    },

    //    _handleResize: function (evt) {
    //        jQuery('.boxy-modal-blackout').css('display', 'none').css({
    //            height: document.documentElement.clientHeight
    //        }).css('display', 'block');
    //    },

    _handleDrag: function (evt) {
        var d;
        if (d = Boxy.dragging) {
            d[0].boxy.css({ left: evt.pageX - d[1], top: evt.pageY - d[2] });
        }
    },

    _nextZ: function () {
        if (!jv.zIndex) {
            jv.zIndex = 1337;
        }
        return ++jv.zIndex;
    },

    //content 的高度和宽度，要减去外面的间隙。
    _viewport: function (options) {
        var maxViewPort = jv.getEyeSize();

        if (options) {
            var nw, nh;

            maxViewPort.width -= 60;
            maxViewPort.height -= 80;


            if (options.width == "auto" || !options.width) {
                nw = "auto";
                //bxy.getInner().css("maxWidth", maxViewPort.width + "px");
            }
            else if (options.width < 0) {
                nw = Math.min(maxViewPort.width + options.width * 2, maxViewPort.width);
            }
            else {
                nw = Math.min(options.width, maxViewPort.width);
            }

            if (options.height == "auto" || !options.height) {
                nh = "auto";
                //bxy.getInner().css("maxHeight", maxViewPort.height + "px");
            }
            else if (options.height < 0) {
                nh = Math.min(maxViewPort.height + options.height * 2, maxViewPort.height);
            }
            else {
                nh = Math.min(options.height, maxViewPort.height);
            }

            maxViewPort.suitWidth = nw;
            maxViewPort.suitHeight = nh;
        }
        return maxViewPort;

        //以下代码移入 jv.getEyeSize
        //var d = document.documentElement, b = document.body, w = window;
        //return jQuery.extend(
        //    jQuery.browser.msie ?
        //        { left: b.scrollLeft || d.scrollLeft, top: b.scrollTop || d.scrollTop } :
        //        { left: w.pageXOffset, top: w.pageYOffset },
        //    !Boxy._u(w.innerWidth) ?
        //        { width: w.innerWidth, height: w.innerHeight } :
        //        (!Boxy._u(d) && !Boxy._u(d.clientWidth) && d.clientWidth != 0 ?
        //            { width: d.clientWidth, height: d.clientHeight } :
        //            { width: b.clientWidth, height: b.clientHeight }));
    }

});

Boxy.prototype = {

    // Returns the size of this boxy instance without displaying it.
    // Do not use this method if boxy is already visible, use getSize() instead.
    estimateSize: function () {
        this.boxy.css({ visibility: 'hidden', display: 'block' });
        var dims = this.getSize();
        this.boxy.css('display', 'none').css('visibility', 'visible');
        return dims;
    },

    // Returns the dimensions of the entire boxy dialog as [width,height]
    getSize: function () {
        return [this.boxy.width(), this.boxy.height()];
    },

    // Returns the dimensions of the content region as [width,height]
    //    getContentSize: function () {
    //        var c = this.getContent();
    //        return [c.width(), c.innerHeight()];
    //    },

    // Returns the position of this dialog as [x,y]
    getPosition: function () {
        var b = this.boxy[0];
        return [b.offsetLeft, b.offsetTop];
    },

    // Returns the center point of this dialog as [x,y]
    getCenter: function () {
        var p = this.getPosition(),
        s = this.getSize();
        return [Math.floor(p[0] + s[0] / 2), Math.floor(p[1] + s[1] / 2)];
    },

    // Returns a jQuery object wrapping the inner boxy region.
    // Not much reason to use this, you're probably more interested in getContent()
    getInner: function () {
        return jQuery('.boxy-inner', this.boxy);
    },

    // Returns a jQuery object wrapping the boxy content region.
    // This is the user-editable content area (i.e. excludes titlebar)
    getContent: function () {
        return jQuery('.boxy-content', this.boxy);
    },

    // Replace dialog content
    setContent: function (newContent) {
        if (newContent.jquery && newContent.html) newContent = newContent.html();
        newContent = jQuery(newContent);//.css({ display: 'block' }).addClass('boxy-content');
        if (this.options.clone) newContent = newContent.clone(true);
        var content = this.getContent();
        if (content.length) {
            content.empty().append(newContent);
        }
        else {
            this.getInner().append("<div class='boxy-content' style='display:block'></div>");
            this.getContent().append(newContent);
        }
        this._setupDefaultBehaviours(newContent);
        this.options.behaviours.call(this, newContent);
        return this;
    },

    // Move this dialog to some position, funnily enough
    moveTo: function (x, y) {
        this.moveToX(x).moveToY(y);
        return this;
    },

    // Move this dialog (x-coord only)
    moveToX: function (x) {
        if (typeof x == 'number') this.boxy.css({ left: x });
        else this.centerX();
        return this;
    },

    // Move this dialog (y-coord only)
    moveToY: function (y) {
        if (typeof y == 'number') this.boxy.css({ top: y });
        else this.centerY();
        return this;
    },

    // Move this dialog so that it is centered at (x,y)
    centerAt: function (x, y) {
        var s = this[this.visible ? 'getSize' : 'estimateSize']();
        if (typeof x == 'number') this.moveToX((x - s[0] / 2 - 8 < 0) ? 0 : (x - s[0] / 2 - 8));
        if (typeof y == 'number') this.moveToY((y - s[1] / 2  < 0) ? 0 : (y - s[1] / 2 ));
        return this;
    },

    centerAtX: function (x) {
        return this.centerAt(x, null);
    },

    centerAtY: function (y) {
        return this.centerAt(null, y);
    },

    // Center this dialog in the viewport
    // axis is optional, can be 'x', 'y'.
    center: function (axis) {
        this.moveTo(0, 0);
        if (this.options["center"]) {
            var v = jv.getEyeSize();
            //o = this.options.fixed ? [0, 0] : [v.left, v.top];
            if (!axis || axis == 'x') this.centerAt(v.width / 2, null);
            if (!axis || axis == 'y') this.centerAt(null, v.height / 2);
        }
        else {
            this.moveTo(
                Boxy._u(this.options.x) ? this.options.x : Boxy.DEFAULT_X,
                Boxy._u(this.options.y) ? this.options.y : Boxy.DEFAULT_Y
            );
        }
        return this;
    },

    // Center this dialog in the viewport (x-coord only)
    centerX: function () {
        return this.center('x');
    },

    // Center this dialog in the viewport (y-coord only)
    centerY: function () {
        return this.center('y');
    },

    // Resize the content region to a specific size
    resize: function (width, height, after) {
        if (!this.visible) return;
        if (width > 0) {
            //保持居中.
            this.getContent().css("width", width);
        }
        var bounds = this._getBoundsForResize(width, height);
        this.boxy.css({ left: bounds[0], top: bounds[1] || 0 });
        if (bounds[3] > 0) {
            this.getContent().css("height", bounds[3]);
        }
        if (after) after(this);
        return this;
    },

    // Tween the content region to a specific size
    tween: function (width, height, after) {
        if (!this.visible) return;
        var bounds = this._getBoundsForResize(width, height),
        self = this;
        this.boxy.stop().animate({ left: bounds[0], top: bounds[1] });
        this.getContent().stop().animate({ width: bounds[2], height: bounds[3] }, function () {
            if (after) after(self);
        });
        return this;
    },

    // Returns true if this dialog is visible, false otherwise
    isVisible: function () {
        return this.visible;
    },

    // Make this boxy instance visible
    show: function () {
        if (this.visible) return;
        if (this.options.modal) {
            var self = this;
            //            if ($.browser.msie && parseInt($.browser.version) < 7) { }
            //            else if (!Boxy.resizeConfigured) {
            //                Boxy.resizeConfigured = true;
            //                jQuery(window).resize(function () { Boxy._handleResize(); });
            //            }
            this.modalBlackout = jQuery('<div class="boxy-modal-blackout"></div>')
                .css({
                    zIndex: Boxy._nextZ()
                })
                .appendTo(document.body);
            //Boxy._handleResize();
            this.toTop();
            if (this.options.closeable) {
                jQuery(document.body).bind('keypress.boxy', function (evt) {
                    //                    var key = evt.which || evt.keyCode;
                    //                    if (key == 27) {
                    //                        self.hide();
                    //                        jQuery(document.body).unbind('keypress.boxy');
                    //                    }
                });
            }
        }
        this.boxy.stop().css({ opacity: 1 }).css("filter", "").show();
        //        this.boxy.find(".boxy-inner").css({ opacity: 1 });
        this.visible = true;
        this._fire('afterShow');
        this.boxy.bind("resize", function (ev) {
            Boxy.get(this).center();
        });
        return this;
    },

    // Hide this boxy instance
    hide: function (after) {
        var oriPos = $(document).data("oriPos");

        // 是否扩展到全屏开关.
        if (false && top.document != document && oriPos && oriPos.obj == this) {

            $(window.frameElement)
                    .width(0).height(0)
                    .css("position", oriPos.positon)
                    .offset({ top: Math.ceil(oriPos.offset.top), left: Math.ceil(oriPos.offset.left) })
                    .width(oriPos.size.width)
                    .height(oriPos.size.height)
            ;
            $(document).removeData("oriPos");
        }

        if (!this.visible) return;
        var self = this;
        if (self._fire('beforeHide') == false) return false;

        //清除 jv.Request.
        jv.UnloadRequest();

        //        if (!this.options["iframe"]) {
        //            if (!this.options["loaderror"]) {
        //                jv.UnloadRequest();
        //                //               for (var i = 0, requestLen = $(".jvRequest", this.boxy).length + 1; i < requestLen; i++) {
        //                //                }
        //            }
        //        }



        //        else if (this.options["nullRequest"]) {
        //            //udi mark.
        //        }

        var containerEv = { originalEvent: true, target: jv.boxdy().data("srcForBoxy") };


        var hide = function () {
            self.boxy.css({ display: 'none' });
            self.visible = false;
            self._fire('afterHide');
            if (self.options.modal) {
                jQuery(document.body).unbind('keypress.boxy');

                if (self.options.hideAnimateTime) {
                    self.modalBlackout.animate({ opacity: 0 }, self.options.hideAnimateTime, function () {
                        jQuery(this).remove();
                    });
                }
                else {
                    jQuery(self.modalBlackout).remove();
                }
            }
            if (self.options && self.options.unloadOnHide) self.unload();

            if (self.options.ReturnCallback) {
                var okval = self.ReturnValue;
                if ($.isFunction(okval)) { okval = okval(); }
                self.options.ReturnCallback(okval, containerEv);
            }
            if (after) after(self, containerEv);
        };

        if (self.options.hideAnimateTime) {
            this.boxy.stop().animate({ opacity: 0 }, self.options.hideAnimateTime, hide);
        }
        else {
            hide();
        }

        return this;
    },

    toggle: function () {
        this[this.visible ? 'hide' : 'show']();
        return this;
    },

    hideAndUnload: function (after) {
        this.options.unloadOnHide = true;
        this.hide(after);
        return this;
    },

    unload: function () {
        this._fire('beforeUnload');


        var content = this.boxy.find(".boxy-inner .boxy-content");
        //        var objParent = content.data("boxyParent"),
        //        objIndex = content.data("boxyParentIndex");


        //        if (objParent && objParent.length > 0 && objIndex >= 0) {
        //            content.removeClass("boxy-content").hide();
        //            if (objIndex == 0) {
        //                content.prependTo(objParent);
        //            }
        //            else {
        //                content.insertAfter(objParent.children(":eq(" + (objIndex - 1) + ")"));
        //            }
        //        }
        //        else 
        {
            this.boxy.remove();
        }

        if (this.options.actuator) {
            jQuery.data(this.options.actuator, 'active.boxy', false);
        }

        this._fire('afterUnload');
    },

    // Move this dialog box above all other boxy instances
    toTop: function () {
        this.boxy.css({ zIndex: Boxy._nextZ() });
        return this;
    },

    // Returns the title of this dialog
    getTitle: function () {
        return jQuery('> .title-bar h2', this.getInner()).html();
    },

    // Sets the title of this dialog
    setTitle: function (t) {
        jQuery('> .title-bar h2', this.getInner()).html(t);
        return this;
    },

    //
    // Don't touch these privates

    _getBoundsForResize: function (width, height) {
        var retHeight = height,
        inner = this.getContent(),
        csize = [inner.width(), inner.innerHeight()];

        if (!width) width = csize[0];
        if (!height) height = csize[1];
        var ch = document.documentElement.clientHeight - 80;
        if (height > ch) retHeight = height = ch;
        var delta = [width - csize[0], height - csize[1]], p = this.getPosition();
        return [Math.max(p[0] - delta[0] / 2, 0),
                Math.max(p[1] - delta[1] / 2, 0), width, retHeight];
    },

    _setupTitleBar: function () {
        if (this.options.title !== false) {
            var self = this,
            tb = jQuery("<div class='title-bar'><span class='title'>" + this.options.title + "</span><span class='BoxyTool'></span></div>"),
            tool = tb.find(".BoxyTool");

            tool.bind("mousedown", function (ev) { ev.stopPropagation(); });

            if (this.options.closeable) {
                $('<div class="close button view"></div>').appendTo(tb);
                //                tool.append(jQuery("<input type='button' class='close button view' value='" + this.options.closeText + "'  />"));
            }
            if (this.options.draggable) {
                tb[0].onselectstart = function () { return false; }
                tb[0].unselectable = 'on';
                tb[0].style.MozUserSelect = 'none';
                if (!Boxy.dragConfigured) {
                    jQuery(document).mousemove(Boxy._handleDrag);
                    Boxy.dragConfigured = true;
                }
                tb.mousedown(function (evt) {
                    $(document).bind("selectstart.boxy", function () { return false; });
                    self.toTop();
                    Boxy.dragging = [self, evt.pageX - self.boxy[0].offsetLeft, evt.pageY - self.boxy[0].offsetTop];
                    jQuery(this).addClass('dragging');
                }).mouseup(function () {
                    $(document).unbind("selectstart.boxy");
                    jQuery(this).removeClass('dragging');
                    Boxy.dragging = null;
                    self._fire('afterDrop');
                });
            }
            this.getInner().prepend(tb);
            this._setupDefaultBehaviours(tb);
        }
    },

    _setupDefaultBehaviours: function (root) {
        var self = this;
        if (this.options.clickToFront) {
            root.click(function () { self.toTop(); });
        }
        jQuery('.close', root).click(function () {

            //执行 closeButton

            if (self.options.closeButton) {
                var defBtnObj = self.options.closeButton;
                if ($.isFunction(defBtnObj)) {
                    var defBtn = defBtnObj(self.boxy);
                    defBtn.data("defBtned", true).trigger("click");
                }
            }

            self.hide();

            $(document.body).trigger("click");
            return;
        }).mousedown(function (evt) { $(document).click(); evt.stopPropagation(); });
    },

    _fire: function (event) {
        return this.options[event].call(this);
    }

};