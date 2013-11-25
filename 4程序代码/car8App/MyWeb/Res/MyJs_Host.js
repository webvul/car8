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

(function ($) {
    jv.CssHasBorderRadius = (typeof (document.createElement("div").style.borderRadius) == "string");
    jv.CssHasBoxShadow = (typeof (document.createElement("div").style.boxShadow) == "string");

    /*
    参数用法：1. Url 向服务器提交的Url ， 服务器返回 数组的 Json数据
    2. post ：change .每次改变 Text 都向服务器提交数据
    3.        click.仅第一次改变 Text 向服务器提交数据
    4. MaxWidth 和MaxHeight ， 最大宽度和最大高度。
    7. quote: 引用类型: radio ,check,none.       单选和多选,不选择
    8. click : 选择某项后的回调.
    9. postCallback : Ajax 成功之后的回调.
    10.不推荐使用hidden 参数 .
    */

    $.fn.TextHelper = function (setting) {
        //post: chang,click,none, url:
        var p = {
            beforeClickInput: false
            , callback: false         //同 postCallback
            , change: false           //同 changeCallback
            , changeCallback: false   //值改变的函数。包含 click。四个参数: selectValueinput  selectHtml event
            , clear: "清除"          //显示 清除 的文本，如果为空，则不显示。
            , checkByKey: true       // 显示选中的依据 ， 是否按Key选中。
            , checkByVal: false      // 显示选中的依据 ， 是否按Value选中。
            , click: false           //同 clickCallback
            , clickCallback: false   //返回值 表示 是否显示 TextHelper.
            , confirm: "确定"          //多选时 显示“确定”的文本， 如果为空，则不显示。
            , data: {}               //指定数据源。可以是数组，也可以是Json,推荐使用 :, 格式的字符串。数据类型自动识别。
            , display: false         //如果 post = 'click'  'change'  要显示的默认值.
            , hideCallback: false     //本控件隐藏的回调.
            , loadData: true         //预加载数据,即自动请求Url 加载数据.
            , post: "none"           //表示何时触发加载数据，可选值： none,click,change
            , quote: "radio"         //表示单选还是多选，可选值： radio,check
            , postCallback: false    //Post 之后的回调函数。 参数是Ajax返回的数据，返回值是处理过后的数据，返回false,则停止中断。
            , renderCallback: false  //渲染完成后的回调。
            , require: true          //是否必选。
            , selectIndex: null      //默认选中的索引，仅没有默认值时有意义。
            , selectValue: null      //默认选中的值，仅没有默认值时有意义。
            , triggerEvent: "mousedown" //触发显示的事件.
            , url: ""                //异步加数据数据的Url
            , useSelect: (jv.csm ? false : false)       //如果是不是Csm 强制使用select控件。 如果是多选，则使用 texthelper
            , width: 0               //强制指定弹出选择内容控件宽度， 为 0 表示自适应。
        }, THIS = this;
        p = $.extend(p, setting);

        //设置别名,当有 callback 时, 绑定到 postCallback 上.
        if (p.callback && !p.postCallback) { p.postCallback = p.callback; }

        //设置别名.,当有 change 时, 绑定到  changeCallBack 上
        if (p.change && !p.changeCallback) { p.changeCallback = p.change; }
        if (p.click && !p.clickCallback) { p.clickCallback = p.click; }

        if (!p.require) {
            p.clear = false;
            p.confirm = false;
        }

        //var lastTimer = null;
        p.post = p.post || "none";

        var RenderSelect = function (d, g, p) {
            if (!d) return d;
            if (d.tagName.toLowerCase() == "select") return d;
            var jd = $(d);
            var selAttr = [];
            for (var arrIndex in jd[0].attributes) {
                var arr = jd[0].attributes[arrIndex];
                if (!arr) continue;
                if (arr.name == "type") continue;
                else if (arr.name == "size") continue;
                else if (arr.nodeValue === false) continue;
                else if (arr.nodeValue === null) continue;
                else if (arr.name == "readonly") continue;
                else if ($.isFunction(arr.value)) continue;
                if (arr.name && arr.value) {
                    selAttr.push(arr.name + '="' + arr.value + '"');
                }
            }

            var selVaue = jd.val();
            var sel = jd.hide().after('<select {0}></select>'.format(selAttr.join(" "))).next();
            jd.remove();


            if (!p.require) {
                sel[0].options.add(new Option("", ""));
            }

            if (p.changeCallback) sel.bind("change", function (ev) { p.changeCallback(sel.val(), sel, ev) });
            if (p.clickCallback) sel.bind("change", function (ev) { p.clickCallback(sel.val(), sel, ev) });

            if (p.data) {
                for (var i = 0; i < p.data.length; i++) {
                    var obj = p.data[i];
                    sel[0].options.add(new Option(obj.val, obj.key));

                    if (selVaue && (selVaue == obj.key || selVaue == obj.val)) {
                        sel[0].selectedIndex = sel[0].length - 1;
                    }
                    else if (!selVaue && p.selectValue) {
                        if (obj.key == p.selectValue) {
                            sel[0].selectedIndex = sel[0].length - 1;
                        }
                    }
                }

                if (!selVaue && !jv.IsNull(p.selectIndex)) {
                    sel[0].selectedIndex = p.selectIndex;
                    sel.trigger("change");
                }
            }



            if (p.url) {
                var selPost = function () {
                    $.ajax({
                        url: $.isFunction(p.url) ? p.url() : p.url, data: {}, type: "post", success: function (res) {

                            res = g.tidyData(res);
                            p.data = res;
                            if (p.postCallback) {
                                var pcRes = p.postCallback(res);
                                if (pcRes === false) return false;
                                else if (jv.HasValue(pcRes)) {
                                    res = pcRes;
                                }
                            }

                            for (var i = 0; i < p.data.length; i++) {
                                var kv = p.data[i];
                                sel[0].options.add(new Option(kv.val, kv.key));

                                if (selVaue && (selVaue == kv.key || selVaue == kv.val)) {
                                    sel[0].selectedIndex = sel[0].length - 1;
                                }
                                else if (!selVaue && p.selectValue) {
                                    if (kv.key == p.selectValue) {
                                        sel[0].selectedIndex = sel[0].length - 1;
                                    }
                                }

                            };

                            if (!selVaue && !jv.IsNull(p.selectIndex)) {
                                sel[0].selectedIndex = p.selectIndex;
                                sel.trigger("change");
                            }

                            if (p.renderCallback) p.renderCallback(sel[0]);
                        }
                    });
                };
                if (p.loadData) {
                    selPost();
                }
                else {
                    sel.one("focus", selPost);
                }
            }

            return sel;
        };

        var retVal = $();
        this
            .UnTextHelper()
            .addClass("TextHelperInput")
            .each(function (oi, od) {
                //jod jQuery对象化的当前选择对象。
                var id = od.name || od.id, g = {}, jod, hidden, thDiv, div_content;
                //改变自身ID , 输入框只有ID,没有Name , Hidden 只有Name,没有ID , Hidden 用于提交. 

                if (!id) {
                    od.id = Math.random().toString(16).slice(2);
                    id = od.id;
                }

                var g = {
                    id: id,
                    p: p,
                    divId: id + "_Div_" + Math.random().toString(16).slice(2),

                    init: function () {
                        od.name = "";

                        //操作的都是 text 对象.
                        jod = $(od);
                        hidden = jod.next("input[type=hidden]");
                        if (hidden.attr("name") != id) {
                            hidden = $('<input type="hidden" name="{0}" />'.format(id));
                        }
                        if (p.selectValue && !jod.val()) { jod.val(p.selectValue); }

                        hidden = hidden.val(jod.val());
                        jod.after(hidden).attr("autocomplete", "off");

                        div_content = $("#" + g.divId, jv.boxdy());
                        if (div_content.length) {
                            div_content.html("");
                        }
                        else {
                            //<div class="TextHelperBorder"></div>
                            $('<div class="TextHelperWrap"><div id="' + g.divId + '"  class="TextHelper"></div></div>')
                            .appendTo(jv.boxdy(od, document.body));
                        }

                        if (!div_content.length) {
                            div_content = $("#" + g.divId, jv.boxdy());
                        }
                        thDiv = div_content.parent().hide();

                        //                        if ($.browser.msie) {
                        //                            thDiv.attr("onselectstart", "return false;");
                        //                        }


                        //                    .click(function(ev){
                        //                        jod.data("divClick", true);
                        //                    })
                        thDiv.add(jod)
                        .click(function (ev) {
                            ev.stopPropagation();
                        });
                    },

                    //返回 数组, 其中的每一项是 key val 对象，返回的是顺序字典。
                    //处理独特的 :, 格式 ,Json , Array，标准。三种格式。
                    tidyData: function (data) {
                        var retVal = [];
                        if (typeof (data) == "string") {
                            var sd = data.split(",");
                            $(sd).each(function (i, d) {
                                var kv = d.split(":");
                                retVal.push({ key: kv[0], val: kv[1] || kv[0] });
                            });
                            return retVal;
                        }

                        if ($.isPlainObject(data)) {
                            for (var d in data) {
                                if (!d && (d != 0)) continue;
                                if ($.isFunction(data[d])) continue;

                                retVal.push({ key: d, val: data[d] });
                            }
                        }
                        else if ($.isArray(data)) {
                            for (var i = 0, dataLen = data.length; i < dataLen; i++) {
                                //如果已是标准 键值对,则.
                                if ((data[i].key || (data[i].key == 0)) && (data[i].val || (data[i].val == 0))) {
                                    retVal.push(data[i]);
                                    continue;
                                }
                                retVal.push({ key: data[i], val: data[i] });
                            }
                        }

                        return retVal;
                    },
                    getSelectValues: function () {
                        var txts = [], vals = [];
                        div_content.find(">span.check")
                            .each(function (i, d) {
                                txts.push($(d).text());
                                vals.push($(d).attr("val"));
                            });

                        return { txts: txts, vals: vals };
                    },
                    displayCheck: function () {
                        var vals = [], keys = [];

                        vals = jod.val().split(',');
                        keys = hidden.val().split(',');

                        //为了不闪,只去除必要的 check.再添加 check
                        div_content.find(">span").each(function (i, d) {
                            var spanVal = $(this).attr("val").trim(), spanTxt = $(this).text().trim();

                            if ($(this).hasClass("check")) {
                                var atHome = true;
                                if (p.checkByKey && keys.indexOf(spanVal) < 0) {
                                    atHome = false;
                                }

                                if (!atHome) {
                                    if (p.checkByVal) {
                                        if (vals.indexOf(spanTxt) < 0) {
                                            atHome = false;
                                        }
                                        else atHome = true;
                                    }
                                }

                                if (!atHome) {
                                    $(this).removeClass("check");
                                }
                            }
                            else if ($(this).html()) {
                                if (p.checkByKey && keys.indexOf(spanVal) >= 0) {
                                    $(this).addClass("check");
                                    return;
                                }
                                if (p.checkByVal && vals.indexOf(spanTxt) >= 0) {
                                    $(this).addClass("check");
                                    return;
                                }
                            }
                        });
                    },

                    spanClick: function (span, ev) {
                        var jspan = $(span);
                        if (!jspan.html()) return false;
                        if (p.beforeClickInput) {
                            if (p.beforeClickInput(jod) == false) return false;
                        }

                        if (p.quote == "radio") {
                            var selectValue = jspan.attr("val");
                            //                                    var hidden = $("input[name=" + id + "]", jv.boxdy());
                            if (hidden.val() != selectValue) {
                                hidden.val(selectValue);
                                jod.val(jspan.text());
                                jod.trigger("change");
                                //                                if (p.selectCallback) p.selectCallback(selectValue, ev);
                                if (p.changeCallback) p.changeCallback(selectValue, jod, ev);

                                //触发Hidden 的Change 事件
                                hidden.trigger("change");
                                g.displayCheck();
                            }

                            if (p.clickCallback && p.clickCallback(selectValue, jod, ev)) {
                            }
                            else {
                                if (p.hideCallback) {
                                    p.hideCallback(jod, ev);
                                }
                                else {
                                    thDiv.hide();
                                }
                            }
                        }
                        else {
                            jspan.toggleClass("check");
                            var data = g.getSelectValues();

                            jod.val(data.txts.join(","));

                            hidden.val(data.vals.join(","));

                            jod.trigger("change");

                            if (p.changeCallback) p.changeCallback(data.vals.join(","), jod, ev);
                            if (p.clickCallback) p.clickCallback(data.vals.join(","), jod, ev);

                            //触发Hidden 的Change 事件
                            hidden.trigger("change");
                        }
                    },

                    loadDataToDiv: function () {
                        div_content.find("span").remove();
                        div_content.find("button").remove();


                        var spans = [], jodVal = hidden.val();
                        for (var i = 0; i < p.data.length; i++) {
                            var kv = p.data[i];
                            spans.push($('<span val="' + kv.key + '">' + jv.encode(kv.val) + "</span>")[0]);

                            if (p.quote == "radio" && (jodVal == kv.key || jodVal == kv.val)) {
                                hidden.val(kv.key);
                                jod.val(kv.val);
                            }
                        };

                        //定义完毕。

                        //if (lastTimer) { lastTimer.stop(); }


                        jod.data("data", p.data);

                        if (!p.data.length) return;

                        if (p.quote == "check") {
                            var displayValue = [];
                            $(jodVal.split(",")).each(function (i, d) {
                                if (!d) return true;
                                $(p.data)
                                    .filter(function (_i, _d) { return d == _d.key || d == _d.val; })
                                    .each(function (_i, _d) {
                                        displayValue.push(_d.val);
                                    })
                                ;
                            });

                            if (displayValue.length) {
                                jod.val(displayValue.join(","));
                            }
                        }

                        $(spans).each(function (i, d) {
                            $(d)
                            .appendTo(div_content)
                            .click(function (ev) {
                                g.spanClick(d, ev);
                            });
                        });

                        if (p.quote == "check" && p.confirm) {
                            $("<button></button")
                            .html(p.confirm)
                            .appendTo(div_content)
                            .bind("click", function (ev) {
                                var data = g.getSelectValues();

                                jod.val(data.txts.join(","));
                                hidden.val(data.vals.join(","));

                                if (p.hideCallback) {
                                    p.hideCallback(jod, ev);
                                }
                                else {
                                    thDiv.hide();
                                }

                                g.displayCheck();
                            });
                        }
                        else if (p.quote == "radio" && p.clear) {
                            $("<button></button")
                            .html(p.clear)
                            .appendTo(div_content)
                            .bind("click", function (ev) {

                                //                                var hidden = $("input[name=" + id + "]", jv.boxdy());
                                if (hidden.val()) {
                                    jod.val("");
                                    hidden.val("");

                                    jod.trigger("change");

                                    //触发Hidden 的Change 事件
                                    hidden.trigger("change");
                                    g.displayCheck();
                                }


                                if (p.changeCallback) p.changeCallback("", jod, ev);

                                if (p.hideCallback) {
                                    p.hideCallback(jod, ev);
                                }
                                else {
                                    thDiv.hide();
                                }
                                g.displayCheck();
                            });
                        }

                        g.displayCheck();
                    },

                    getValueFromInput: function () {
                        var valArrays = [], val = jod.val();
                        if (p.quote == "radio") {
                            //如果值存在, 则显示值.
                            if (p.checkByKey) {
                                valArrays =
                                $(p.data)
                                .filter(function (i, d) { return d.key == val; })
                                .each(function (i, d) { return d.val; })
                                .toArray();
                            }

                            if (valArrays.length) return valArrays;
                            else return [val];

                            if (p.checkByVal) {
                                valArrays =
                                $(p.data)
                                .filter(function (i, d) { return d.val == val; })
                                .each(function (i, d) { return d.val; })
                                .toArray();
                            }
                            if (valArrays.length) return valArrays;
                            else return [val];
                        }
                        else {
                            val = val.split(",");
                            //如果值存在, 则显示值.
                            if (p.checkByKey) {
                                $.merge(valArrays,
                                $(p.data)
                                .filter(function (i, d) { return val.indexOf(d.key) >= 0; })
                                .each(function (i, d) { return d.val; })
                                .toArray());
                            }

                            if (p.checkByVal) {
                                $.merge(valArrays,
                                $(p.data)
                                .filter(function (i, d) { return val.indexOf(d.key) >= 0; })
                                .each(function (i, d) { return d.val; })
                                .toArray());
                            }

                            return valArrays;
                        }

                        //                        var valArrays = [], checkSpans = {};

                        //                        valArrays = jod.val().split(',');


                        //                        //为了不闪,只去除必要的 check.再添加 check
                        //                        div_content.find(">span.check").each(function (i, d) {
                        //                            var spanVal = $(this).attr("val"), spanTxt = $(this).text();

                        //                            checkSpans[spanVal] = spanTxt;
                        //                        });


                        //                        var checkKeys = [], checkValues = [];
                        //                        for (var k in checkSpans) {
                        //                            checkKeys.push(k);
                        //                            checkValues.push(checkSpans[k]);
                        //                        }

                        //                        $(valArrays).each(function (i, d) {
                        //                            //如果输入的不在字典里. 
                        //                            if (checkKeys.indexOf(d) < 0 && checkValues.indexOf(d) < 0) {
                        //                                checkSpans[d] = d;
                        //                                checkKeys.push(d);
                        //                            }
                        //                        });

                        //                        return checkKeys;
                    },
                    txtClick: function (onlyLoad, ev) {
                        if (thDiv.length == 0) return;
                        jod.addClass("TextHelper_Loading");

                        if (p.hideCallback) {
                            p.hideCallback(jod, ev);
                        }
                        else {
                            thDiv.hide();
                        }


                        if ((p.post == "change" || p.post == "click") && (thDiv.find("span").length == 0)) {
                            g.post(onlyLoad ? null : g.showMe);
                        }
                        else if (!onlyLoad) {
                            g.showMe();
                        }
                    },
                    bindDivClick: function () {
                        jod.keyup(function (ev) {
                            var evCode = ev.witch || ev.keyCode;
                            if (evCode == 27) {
                                $(document).trigger("click");
                                return;
                            }
                            jod.addClass("TextHelper_Loading");
                            if (p.hideCallback) {
                                p.hideCallback(jod, ev);
                            }
                            else {
                                thDiv.hide();
                            }

                            g.displayCheck();

                            var val = g.getValueFromInput().join(",");
                            hidden.val(val);

                            if (p.changeCallback) p.changeCallback(val, jod, ev);

                            if (p.post == "change" || (p.post == "click")) {

                                jv.RestartTimer("TextHelper", function () {
                                    if (this.die) return false;
                                    g.post(function () { g.showMe(); })
                                });
                            }
                            else {
                                g.showMe();
                                return;
                            }
                        });

                        jod.bind(p.triggerEvent, function (ev) {
                            g.txtClick(false, ev);
                        });
                    },

                    post: function (Callback) {
                        var paras = jv.JsonValuer(p.paras || {});
                        paras["query"] = jod.val();

                        $.ajax({
                            url: $.isFunction(p.url) ? p.url() : p.url, data: paras, type: "post", success: function (res) {
                                jod.removeClass("Error").removeClass("TextHelper_Loading");

                                res = g.tidyData(res);
                                p.data = res;
                                if (p.postCallback) {
                                    var pcRes = p.postCallback(res);
                                    if (pcRes === false) return false;
                                    else if (jv.HasValue(pcRes)) {
                                        res = pcRes;
                                    }
                                }
                                if (res) {
                                    p.data = res;
                                    g.loadDataToDiv();
                                    //                                thDiv.css("left", -1000).css("top", -1000);

                                    if (Callback) {
                                        Callback();
                                    }
                                }

                                if (!jv.IsNull(p.selectIndex) && !jod.val()) {
                                    div_content.find(">span:eq(" + p.selectIndex + ")").trigger("click");
                                    p.selectIndex = null;
                                }
                                

                                if (p.renderCallback) {
                                    p.renderCallback(od, div_content[0], p);
                                }
                            }, error: function (res) {
                                jod.removeClass("TextHelper_Loading");
                                jod.addClass("Error");
                            }
                        });
                    },


                    showMe: function () {
                        if (jod.attr("disabled")) { return; }
                        if (p.hideCallback) {
                            p.hideCallback(jod);
                        }
                        else {
                            $(document).trigger("click");
                        }

                        g.displayCheck();

                        jod.smartPosition(thDiv, p.width);

                        /*
                        //以下是 CSM 算法
                        if (a.left <= body_width / 2) {
                        //a,d 两点
                        var outerMaxWidth = p.width || (body_width - a.left);

                        thDiv.css("maxWidth", outerMaxWidth - th_diff_width);

                        //如果在上半部分，或向下超出范围
                        var div_outerHeight = thDiv.outerHeight();


                        if (a.top <= body_height / 2) {
                        //d 点
                        thDiv.offset(d);
                        }
                        else if (d.top + div_outerHeight > body_height) {
                        //a 点
                        thDiv.offset({ left: a.left, top: a.top - div_outerHeight });
                        }
                        else {
                        //d 点
                        thDiv.offset(d);
                        }
                        }
                        else {
                        //b,c 两点
                        var outerMaxWidth = p.width || (body_width - a.left);

                        thDiv.css("maxWidth", b.left - th_diff_width);

                        //如果在上半部分，或向下超出范围
                        var div_outerHeight = thDiv.outerHeight();


                        if (a.top <= body_height / 2) {
                        //c 点
                        thDiv.offset({ left: c.left - thDiv.outerWidth(), top: c.top });
                        }
                        else if (d.top + div_outerHeight > body_height) {
                        //b 点
                        thDiv.offset({ left: c.left, top: b.top - div_outerHeight });

                        }
                        else {
                        //c 点
                        thDiv.offset({ left: c.left - thDiv.outerWidth(), top: c.top });
                        }

                        }
                        */



                        jod.removeClass("TextHelper_Loading");


                        $(document).one("click", function () {
                            //                    jod.removeClass("TextHelperInput");
                            thDiv.hide();
                        });

                        return;
                    }
                };

                if (p.data) {
                    p.data = g.tidyData(p.data);
                }

                if (p.useSelect) {
                    retVal = retVal.add(RenderSelect(od, g, p));
                    return true;
                }
                else retVal = retVal.add(od);


                g.init();
                g.loadDataToDiv();
                g.bindDivClick();

                //$("[name=" + id + "]", jv.boxdy()).val(g.getValueFromInput().join(","));


                p.display = p.display || $("#" + id, jv.boxdy()).attr("display");

                if (p.display) { $("#" + id, jv.boxdy()).val(p.display); }

                if (!jv.IsNull(p.selectIndex) && !jod.val()) {
                    div_content.find(">span:eq(" + p.selectIndex + ")").trigger("click");
                }
                else {
                    var srcObj = div_content.find("span.check");
                    var srcObjVal = srcObj.attr("val");
                    if (srcObjVal) {
                        if (p.changeCallback) p.changeCallback(srcObjVal, jod, { originalEvent: true, eventTarget: srcObj[0] });
                    }
                }

                jv.RegisteUnload("TextHelper", function () {
                    thDiv.hide();
                });

                //设置在Hidden上.
                $("#" + id, jv.boxdy()).data("TextHelper", g);


                if (p.url && p.loadData) {
                    g.txtClick(true, null);
                }

            });

        //        //临时解决.
        //        if (jv.TextHelperStyleCallBack) {
        //            jv.TextHelperStyleCallBack(this);
        //        }

        return retVal;
    }

    $.fn.UnTextHelper = function () {
        var retVal = $();
        this.each(function (i, od) {
            if (od.tagName.toLowerCase() == "select") {
                var jd = $(od);
                var selAttr = [];
                for (var arrIndex in jd[0].attributes) {
                    var arr = jd[0].attributes[arrIndex];
                    if (!arr) continue;
                    if (arr.name == "type") continue;
                    else if (arr.nodeValue === false) continue;
                    else if (arr.nodeValue === null) continue;
                    else if (arr.name == "readonly") continue;
                    if (arr.name && arr.value) {
                        selAttr.push(arr.name + '="' + arr.value + '"');
                    }
                }

                var val = jd.val();
                var inp = jd.after('<input {0} />'.format(selAttr.join(" "))).next();
                inp.val(val);

                jd.remove();

                retVal = retVal.add(inp);
                return true;
            }
            retVal = retVal.add(od);

            var config = $(od).data("TextHelper");
            if (!config) return true;

            //先还原 name.
            var hidden = $("input[name=" + config.id + "]", jv.boxdy());
            var val = hidden.val();
            hidden.remove();
            $("#" + config.divId, jv.boxdy()).remove();
            od.name = config.id;
            var jod = $(od);
            jod.val(val);

            //去除事件.
            jod.unbind()
                .data("TextHelper", false)
                .removeAttr("display");
        });
        return retVal;
    };

    $.fn.ClearTextHelperData = function () {
        this.each(function (i, od) {
            var config = $(od).data("TextHelper");
            if (!config || !config.divId) return true;

            //先还原 name.
            $("#" + config.divId).find("span").remove();
        });
        return this;
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
