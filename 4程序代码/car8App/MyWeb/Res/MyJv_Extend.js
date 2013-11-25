window.Alert = window.alert;
window.Confirm = window.confirm;

//扩展到 JQuery
(function ($) {
    //本页标志性含数 $.fn.LoadView
    if ($.fn.LoadView) return;

    $(window).load(function () {
        jv.LoadJsCss("js", jv.Root + "Res/My97/wdatepicker.js");
        jv.MyOnLoad();
    });

    if ($.browser.msie) {
        var unloadEvent = function () {


            //http://fogtower.iteye.com/blog/617997
            $("form,button,input,select,textarea,a,img").unbind().removeData();
            //,table,tr,td
            //添加这一句，将减少大部分内存泄露
            $.cache = null;
            CollectGarbage();

            window.detachEvent('onunload', unloadEvent);



            //https://github.com/mootools/mootools-core/issues/2329
            //http://com.hemiola.com/2009/11/23/memory-leaks-in-ie8/
            //http://blog.csdn.net/guo_rui22/article/details/3165320

        };

        window.attachEvent("onunload", unloadEvent);
    }


    //是否定义了某个事件, 所有的对象是否全部都定义了该事件. 返回最后一个对象定义的事件。
    $.fn.IsBindEvent = $.fn.isBindEvent = function (eventName) {
        var found = false;
        this.each(function () {
            if (this["on" + eventName]) {
                found = this["on" + eventName];
            }
            else {
                var self = $(this);
                found = self.data("events") && self.data("events")[eventName];
            }
            return found;
        });

        return found;
    };

    //offset 的另一实现,请参考: http://kb.cnblogs.com/a/1710726/
    //$.fn.Offset = $.fn.offset = function (options) {
    //    var elem = this[0];
    //    if (options) {
    //        return this.each(function (i) {
    //            jQuery.offset.setOffset(this, options, i);
    //        });
    //    }
    //    if (!elem || !elem.ownerDocument) {
    //        return null;
    //    }

    //    //特殊处理body  
    //    if (elem === elem.ownerDocument.body) {
    //        return jQuery.offset.bodyOffset(elem);
    //    }

    //    //获得关于offset相关的浏览器特征  
    //    jQuery.offset.initialize();

    //    var offsetParent = elem.offsetParent, prevOffsetParent = elem,
    //    doc = elem.ownerDocument, computedStyle, docElem = doc.documentElement,
    //    body = doc.body, defaultView = doc.defaultView,
    //    prevComputedStyle = defaultView ? defaultView.getComputedStyle(elem, null) : elem.currentStyle,
    //    top = elem.offsetTop, left = elem.offsetLeft;

    //    while ((elem = elem.parentNode) && elem !== body && elem !== docElem) {
    //        //HTML，BODY，以及不具备CSS盒子模型的元素及display为fixed的元素没有offsetParent属性  
    //        if (jQuery.offset.supportsFixedPosition && prevComputedStyle.position === "fixed") {
    //            break;
    //        }
    //        computedStyle = defaultView ? defaultView.getComputedStyle(elem, null) : elem.currentStyle;
    //        top -= elem.scrollTop;
    //        left -= elem.scrollLeft;
    //        if (elem === offsetParent) {
    //            top += elem.offsetTop;
    //            left += elem.offsetLeft;

    //            //offset应该返回的是border-box，但在一些表格元素却没有计算它们的border值，需要自行添加  
    //            //在IE下表格元素的display为table,table-row与table-cell  
    //            if (jQuery.offset.doesNotAddBorder && !(jQuery.offset.doesAddBorderForTableAndCells && /^t(able|d|h)$/i.test(elem.nodeName))) {
    //                top += parseFloat(computedStyle.borderTopWidth) || 0;
    //                left += parseFloat(computedStyle.borderLeftWidth) || 0;
    //            }
    //            prevOffsetParent = offsetParent, offsetParent = elem.offsetParent;
    //        }

    //        //修正safari的错误  
    //        if (jQuery.offset.subtractsBorderForOverflowNotVisible && computedStyle.overflow !== "visible") {
    //            top += parseFloat(computedStyle.borderTopWidth) || 0;
    //            left += parseFloat(computedStyle.borderLeftWidth) || 0;
    //        }
    //        prevComputedStyle = computedStyle;
    //    }

    //    //最后加上body的偏移量  
    //    if (prevComputedStyle.position === "relative" || prevComputedStyle.position === "static") {
    //        top += body.offsetTop;
    //        left += body.offsetLeft;
    //    }

    //    //使用固定定位，可能出现滚动条，我们要获得取大的滚动距离  
    //    if (jQuery.offset.supportsFixedPosition && prevComputedStyle.position === "fixed") {
    //        top += Math.max(docElem.scrollTop, body.scrollTop);
    //        left += Math.max(docElem.scrollLeft, body.scrollLeft);
    //    }

    //    return { top: top, left: left };
    //};


    //模拟 lambda.select
    $.fn.out = function (callback) {
        var newObj = [];
        if (callback) {
            this.each(function (_i, _d) { newObj.push(callback(_i, _d)); });
            return $(newObj);
        }
        else return this;
    };
    //转到 Jquery，加入 jv.boxdy 范围。如果页面同时有 id,name ,  则按name . 
    //prefix 前缀，约定： 如果为 true ，表示自动截取 $ _ 最后部分。 否则不截取，默认为 空字符串 表示不截取。
    $.fn.GetDivJson = $.fn.getDivJson = function (prefix, option) {
        if ($.isPlainObject(prefix) && !option) {
            option = prefix;
            prefix = "";
        }
        if (prefix !== true) prefix = prefix || "";

        var p = $.extend({
            valueProc: function (val) { return (val || "").trim(); },
            keyProc: function (key) {
                if (!key) return key;
                if (prefix === true) {
                    return key.slice(Math.max(key.lastIndexOf("$"), key.lastIndexOf("_")) + 1);
                }
                else {
                    if (!prefix) return key;
                    if (key.indexOf(prefix) != 0) return "";
                    return key.slice(prefix.length);
                }
            }
        }, option);

        //        var idValues = {}, nameValues = {};
        var jsonVal = {};

        //keyFunc 表示有key 值 存在时key的回调， valFunc 表示有key值存在时val的回调
        var addVal = function (key, val, keyFunc, valFunc) {
            if (jsonVal[key]) {
                var ary = jsonVal[key].mySplit(",", true);
                ary.push(val);
                jsonVal[key] = ary.join(",");
            }
            else {
                jsonVal[key] = val;
            }
        };


        var setJsonValue = function (inps) {

            for (var i = 0, length = inps.length; i < length; i++) {
                var c = inps[i];
                var nameKey = p.keyProc(c.name || c.id || c.getAttribute("name"));
                if (!nameKey) continue;

                if (c.tagName == "SELECT") {
                    if (c.multiple) {
                        var selCount = 0;
                        for (var j = 0, ol = c.options.length; j < ol; j++) {
                            var op = c.options[j];
                            if (op.selected) {
                                addVal(nameKey + "[" + (selCount++) + "]", p.valueProc(op.value));
                            }
                        }
                    }
                    else {
                        jsonVal[nameKey] = p.valueProc(c.value);
                    }
                    continue;
                }
                else if (c.tagName == "INPUT") {
                    if (c["type"] == "checkbox") {
                        var checkValues = {};

                        if (c.checked) {
                            var chkCount = 0;
                            for (var k in jsonVal) {
                                if (k.indexOf(nameKey + "[") == 0) { chkCount++; }
                            }
                            addVal(nameKey + "[" + chkCount + "]", p.valueProc(c.value));
                        }
                        continue;
                    }
                    else if (c["type"] == "radio") {
                        if (c.checked) {
                            jsonVal[nameKey] = p.valueProc(c.value);
                        }
                        continue;
                    }

                    //在 .MyDateTime 时，要使用 $.fn.val 方法获取下拉列表里的值。
                    var jc = $(c);
                    jsonVal[nameKey] = p.valueProc(jc.val());
                    continue;
                }
                else if ("BUTTON" == c.tagName) {
                    jsonVal[nameKey] = p.valueProc(c.value);
                }
                else if ("TEXTAREA" == c.tagName) {
                    if ((" " + c.className + " ").indexOf(" tinymce ") >= 0) {
                        //tinymce 3.x 需要通过　jQuery的　val 取值。
                        jsonVal[nameKey] = $(c).val();
                    } else {
                        jsonVal[nameKey] = p.valueProc(c.value);
                    }
                }
                else if (jv.isLeaf(c)) {
                    jsonVal[nameKey] = p.valueProc(c.innerHTML);
                }
            }
        };


        this.each(function () {
            var inps = jv.findInput(this);
            if (p.extDom && p.extDom) {
                var extinps = p.extDom();
                for (var i = 0; i < extinps.length ; i++) {
                    inps.push(extinps[i]);
                }
            }

            var idInputs = [], nameInputs = [];

            $.each(inps, function (i, d) {
                if (d.id) { idInputs.push(d); }
                else nameInputs.push(d);
            });


            setJsonValue(idInputs);
            setJsonValue(nameInputs);
        });

        return jsonVal;
    };



    //prefix 前缀，约定： 如果为 true ，表示自动截取 $ _ 最后部分。 否则不截取，默认为 空字符串 表示不截取。
    $.fn.SetDivJson = $.fn.setDivJson = function (prefix, JsonValue, option) {
        if ($.isPlainObject(prefix) && !option) {
            option = (JsonValue || null);
            JsonValue = prefix;
            prefix = "";
        }

        if (prefix !== true) prefix = prefix || "";
        var container = this;
        if (container.length == 0) { return; }
        var p = $.extend({
            refMark: "::",   //1::张三 ,表示设置Id为1，显示的名字是张三。
            clearChecked: true,
            findCallback: function (name, val) {
                if ((name.slice(0, 1) == "#") || (name.slice(0, 2) == "\\#")) {
                    if (name.slice(0, 1) == "#") name = name.slice(1);
                    else name = name.slice(2);

                    if (prefix === true) {
                        return $("#" + name, container)[0] || $("[id$=_" + name + "]", container)[0];
                    }
                    else return $("#" + prefix + name, container)[0];
                }
                if (prefix === true) {
                    //优先使用没有前缀的。

                    var jObj;
                    jObj = $("[name=" + name + "]", container);
                    if (jObj.length) return jObj[0];

                    jObj = $("#" + name, container);
                    if (jObj.length) return jObj[0];

                    jObj = $("[name$=_" + name + "]", container);
                    if (jObj.length) return jObj[0];

                    return $("[id$=_" + name + "]", container)[0];
                }
                else {
                    var cons = $("[name=" + prefix + name + "]", container[0]);//container[0].ownerDocument.getElementsByName(prefix + name);
                    if (cons.length > 1) {
                        if (jv.IsNull(val) == false) {
                            //查找某个value值的控件
                            for (var i = 0, length = cons.length; i < length; i++) {
                                var n = cons[i];
                                if (n.value == val && jv.inContainer(n, container[0])) return n;
                            }
                        }
                    }
                    if (cons && cons.length && jv.inContainer(cons[0], container[0])) return cons[0];

                    cons = $("#" + prefix + name, container[0]);//container[0].ownerDocument.getElementById(prefix + name);
                    if (jv.inContainer(cons, container[0])) return cons;
                    return null;
                }
            }
        }, option);

        var setValue = function (cobj, value) {
            var tagName = cobj.tagName;
            if (tagName == "INPUT") {
                var type = cobj["type"];
                if (type == "checkbox" || (type == "radio")) {
                    cobj.checked = true;
                    return;
                }
                else {
                    var self = $(cobj);
                    if (value == "0001/01/01 00:00" || value == "0001/1/1 0:00:00") {
                        self.val("");
                    }
                    else {
                        //对于 .MyDateTime 来说，要使用 $.fn.val 方法进行设置。
                        self.val(value);
                    }
                    return;
                }
            }
            else if (tagName == "OPTION") {
                cobj.selected = true;
                return;
            }
            else if (tagName == "TEXTAREA") {
                cobj.value = value;
                return;
            }
            else if (["SELECT", "BUTTON"].indexOf(tagName) >= 0) {
                cobj.value = value;
            }
            else if (jv.isLeaf(cobj)) {
                cobj.innerHTML = value;
            }
        };

        //清除选中状态。
        if (p.clearChecked) {
            {
                var chks = container[0].getElementsByTagName("input");
                for (var i = 0, length = chks.length; i < length; i++) {
                    var n = chks[i];

                    if (n.checked) {
                        n.checked = false;
                    }
                }
            }
            {
                var chks = container[0].getElementsByTagName("select");
                for (var i = 0, length = chks.length; i < length; i++) {
                    var n = chks[i];

                    n.selectedIndex = -1;
                }
            }
        }


        for (var jkey in JsonValue) {
            //把 $ 替换成 \$ ； 把 # 替换成 \# , 因为它们在 jQuery 选择器中表示特殊的含义 。

            var val = JsonValue[jkey];

            var key = jkey.replace(/\$/g, "\\$").replace(/\#/g, "\\#");

            var leftKIndex = key.indexOf('['), rightKIndex = key.indexOf(']'),
                isCheck = (key.slice(-1) == ']' && (leftKIndex < rightKIndex));
            var checkIndex = -1;
            if (isCheck) {
                checkIndex = parseInt(key.slice(leftKIndex + 1, rightKIndex));
                key = key.slice(0, leftKIndex);
            }

            var cobj = p.findCallback(key, isCheck ? val : null);

            if (cobj) {
                if (prefix === true && val.indexOf("::") > 0) {
                    if ($(cobj).is(":hidden")) {
                        var sect = val.split("::");
                        setValue(cobj, sect[0]);

                        //查找 name
                        jv.SetDisplayValue({ obj: cobj }, null, [sect[1]]);
                    }
                }
                else {
                    setValue(cobj, val);
                }
            }
        }
        return this;
    };

    // 工具栏中的  .view 不会被去除.
    $.fn.SetDetail = $.fn.setDetail = function (options) {
        var p = $.extend({ callback: false, detail: "Detail", container: jv.boxdy() }, options);

        //弹出页面没有导航栏.
        if (jv.IsInBoxy()) {
            $(".Navigate", p.contaier).hide();
        }



        //var procEdit = function () {
        //    jv.PopTextArea($("textarea", p.container));
        //};

        if ((jv.page()["action"] || "").toLowerCase() != p.detail.toLowerCase()) {
            //procEdit();
            return this;
        }


        var _setDetail = function (con) {
            var container = jv.boxdy(con);

            //设置属性和数据.在弹出列表时,方便查找.
            //container
            //    .addClass("jvDetail")
            //    .data("jvDetail", p);
            //处理权限
            //jv.TestRole();        //卡片先不处理权限.

            //            var jcon = $($(".Main:last", jcon)[0] || jcon);



            container.find(":text,textarea").filter(":visible").each(function () {
                var jd = $(this), val = jd.val();

                jd.closest(".val").addClass("Wrap");

                var content = $("<span></span>")
                    .css("height", "100%")
                //                    .css("word-wrap", "break-word")
                //                    .css("white-space", "normal")
                //                    .css("width", jd.width() + parseInt(jd.css("paddingRight")) + +parseInt(jd.css("paddingLeft")))
                    .html(jv.encode(val))
                    .insertAfter(jd)
                ;

                var id = jd.attr("id"), name = jd.attr("name");

                jd.remove();

                var hid = $("<input type='hidden' value='" + val + "' />");

                if (id) hid.attr("id", id);
                if (name) hid.attr("name", name);

                hid.insertAfter(content);
            });

            container.find(":checked").each(function (i) {
                var jd = $(this);
                if (i > 0) {
                    jd.after(" , ");
                }
                jd.remove();
            });

            container.find(":radio,:checkbox").each(function () {
                $("label[for=" + this.id + "]").remove();
                $(this).remove();
            });

            //                container.find(".MyTool").remove();
            container.find(".boxy-inner .BoxyTool").children().not(".view").remove();
            container.find(".PageTool").children().not(".view").remove();


            var myTool = container.find(".MyTool,.myTool");
            myTool.children().not(".view").remove();
            if (!myTool.children().length) { myTool.remove(); }

            container.find(":file").each(function () {
                $(this).remove();
            });

            $(".MyDateDisplay", container).find("span").unbind();


            container.find(".kv .key.Link").unbind("click").removeClass("Link");
            //Boxy.getOne().resize();


            if (p.callback) p.callback();
        };

        this.each(function () {
            _setDetail(this);
        });
    };

    //加 ed ，是因为钩子在最后执行。
    $.fn.Hooked = $.fn.hooked = function (props, func) {
        //props 为空的容错。
        props = props.mySplit(",", true);
        $(props).each(function (i, d) {
            if (d == "" || d == "*") {
                props = props.removeAt(i);
            }
        });
        return this.each(function (i, d) {
            var id = d.id || d.name || d.getAttribute("name"),
            jod = $(d),
            fnc = function (ev, prop) {
                $(props).filter(function () {
                    if (prop == this || prop.indexOf(this + ".") == 0) { func(ev, prop); return false; }
                });
            };

            if (typeof (this.onpropertychange) == "object") {
                jod.bind("propertychange." + id, function (ev) {
                    if (jv.GetDoer(ev) != d) return;
                    fnc(ev, event.propertyName);
                });
            }
            else if ($.browser.mozilla) {
                jod.bind("DOMAttrModified." + id, function (ev) {
                    if (jv.GetDoer(ev) != d) return;
                    var prop = ev.attrName;
                    if (prop == "style") {
                        if (jv.HasValue(ev.newValue)) {
                            prop += "." + ev.newValue.mySplit(":", true)[0];
                        }
                        else if (jv.HasValue(ev.prevValue)) {
                            prop += "." + ev.prevValue.mySplit(":", true)[0];
                        }
                    }
                    fnc(ev, prop);
                });
            }
            else
                //待测其它浏览器。
                return;
        });
    };
    $.fn.OneClick = $.fn.oneClick = function (timeout) {

        this.each(function () {
            if (!this.oned) {
                this.oned = true;

                var jd = $(this);
                jd.data("oriclick", this.onclick);

                var joclick = [];
                if (jd.data("events") && jd.data("events")["click"]) {
                    var clicks = jd.data("events")["click"];
                    for (var i = 0; i < clicks.length; i++) {
                        joclick.push(clicks[i]);
                    }
                }
                jd.data("orijclick", joclick);

                jd.unbind("click");

                this.onclick = function (ev) {
                    var ret, jobj = $(this);
                    jv.SetDisable(jobj, timeout);


                    var keyClick = jobj.data("oriclick"),
                        jclick = jobj.data("orijclick");

                    if (keyClick) ret = keyClick(ev);
                    if (ret === false) return false;
                    if (jclick && jclick.length) {
                        for (var i = 0; i < jclick.length; i++) {
                            if (jclick[i].handler) ret = jclick[i].handler(ev);
                            if (ret === false) return false;
                        }
                    }


                };
            }
        });

        return this;
    };

    $.fn.SkinVar = $.fn.skinVar = function (options) {
        //后面的定义会覆盖前面，忽略值为0的值。
        var defaults = {
            Corner: {},
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
            //jv.SetCorner(options.Corner);

            for (var p in options.LangImg) {
                $("." + p).attr('src', options.LangImg[p].format(jv.page()["Lang"] || "Zh"));
            }

            $(options.LangCss).each(function () {
                $("." + this.format("")).addClass(this.format(jv.page()["Lang"] || "Zh"))
            });

            if (($(".ListPhoto").attr("src") || "").length == 0) {
                $(".ListPhoto").parents(".Sect:first").hide();
            }
            if (($(".SkinTopBanner").attr("src") || "").length == 0) {
                $(".SkinTopBanner").hide();
            }
            if ($(".deptTitleImg").length > 0) {
                if ($(".deptTitleImg").height() > 100) { $(".deptTitleImg").height(100); }
                if ($(".deptTitleImg").width() > 600) { $(".deptTitleImg").css("height", null); $(".deptTitleImg").width(600); }
            }
        });
    };

    $.fn.LoadView = $.fn.loadView = function (setting) {
        //errror : 出错时的 CallBack , 参数依次表示: Response, 容器. error返回值含义表示是否继续执行默认出错处理.
        var p = $.extend(true, { url: false, postType: "GET", param: { LoadView: true }, filter: false, callback: false, error: false }, setting);

        p.url = jv.url(p.url);

        var theUrl = jv.url(window.location);
        if (theUrl.attr("Html") == "True") {
            p.url.attr("Html", "True");
        }
        else {
            p.url.attr("History", "False"); //强制 History 为False . Udi.
        }

        if (theUrl.url.slice(-5) == ".html") {
            var search = "";
            //转到 HTML
            for (var key in p.url.kv) {
                if (key == "_") continue;
                if (key == "Html") continue;
                if (key == "History") continue;
                search += "." + key + (p.url.kv[key] ? "." + p.url.kv[key] : "");
            }


            p.url.url = p.url.url.slice(0, -5) + search + ".html";
            p.url.search = "";
            p.url.kv = {};
        }

        p.url = p.url.tohref();

        parg = arguments[0];
        parg.originalEvent = true;
        parg.target = this[0];


        var LoadOne = function (_d) {
            var jd = $(_d), timer = $.timer(1000, function (timer) {
                timer.stop();
                if (!jd.attr("class")) { jd.empty(); jd.html("正在加载,请稍等...."); }
            });

            //                    .width(jd.width() + "px")
            //                    .height( jd.height() + "px")
            //                    .addClass("LoadingWillRemove")


            if (jd.hasClass("jvRequest")) {
                jv.UnloadRequest();
            }

            var ajax = {
                url: p.url, type: p.postType, dataType: 'html', cache: false, data: p.param,
                beforeSend: function (xhr) {
                    jv.CreateEventSource(xhr, _d);
                },
                success: function (html) {
                    if (timer) timer.stop();

                    if (html[0] == "{") {
                        var res = null;
                        try
                        { res = $.parseJSON(html); }
                        catch (e) { }
                        if (jv.IsNull(res) == false && res.data == "power") {
                            jd.html(res.msg);
                            return;
                        }
                    }

                    html = /^(?:[^#<]*(<[\w\W]+>)[^>]*$|#([\w\-]*)$)/.exec(html);
                    html = html[1].replace(/<(?!area|br|col|embed|hr|img|input|link|meta|param)(([\w:]+)[^>]*)\/>/ig, "<$1></$2>");

                    var div = document.createElement("div");
                    div.style.display = "none";
                    document.body.appendChild(div);
                    div.innerHTML = "div<div>" + html + "</div>";

                    html = $(div.lastChild.childNodes);

                    var htmlFilter = jv.myFilter(html, p.filter);
                    var inscript = [];

                    htmlFilter.each(function () {
                        var ins = this.getElementsByTagName("script");
                        for (var i = 0, len = ins.length; i < len; i++) {
                            var s = ins[0];
                            inscript.push(s);
                            s.parentNode.removeChild(s);
                        }
                    });

                    jd.addClass("jvRequest_waite").empty();
                    jd.prepend(htmlFilter);
                    //.find(">.LoadingWillRemove").remove() ;
                    jv.execHtml(html.add(inscript), null, _d);
                    if (p.callback) { p.callback(_d); }

                    document.body.removeChild(div);
                },
                error: function (ev) {
                    if (timer) timer.stop();
                    if (!p.error || (p.error && (p.error(ev, jd) === false))) {
                        var response = $(ev.responseText),
                                msg = response.filter("title").text();
                        if (!msg) {
                            for (var i = 0; i < response.length; i++) {
                                //                            if ( !response[i].text ) continue ;
                                msg = $(response[i]).text();
                                if (!!msg) break;
                            }
                        }

                        var msg = "<div style='color:red'>" +
                                msg +
                                "</div><br />请刷新后再试";

                        jd.html(msg);
                    }
                }
            };
            jQuery.ajax(ajax);
        };

        this
        //.css("position","relative")
            .each(function () {
                LoadOne(this, { originalEvent: true, target: this });
            });
        return this;
    };


    $.fn.Mytip = $.fn.mytip = function (msg, options) {

        var THIS = this,
        p = {
            content: msg,
            //showOn: "focus",
            alignTo: 'target',
            alignX: 'inner-left',
            offsetX: 0,
            offsetY: 5
        };
        p = $.extend(p, options),
        //if ($(obj).data("events") && $(obj).data("events")["focus"] && !$(obj).data("poshytip")) { showType = "hover"; }

        poshytipCallBack = function () {
            THIS.poshytip("destroy").poshytip(p).poshytip(msg ? "show" : "hide");
            var tip = THIS.data("poshytip");
            if (tip) {
                tip.$tip.dblclick(function (a, b) {
                    $(this).hide();
                });
            };
        };

        if (this.poshytip) {
            poshytipCallBack();
        }
        else { jv.MyLoadOn["jv.chk.url"](true, poshytipCallBack); }
    };

    $.fn.CoverDiv = $.fn.coverDiv = function () {
        this.find(">div.cover").remove();
        this.each(function () {
            var $self = $(this);
            var h = $self.height();
            var cover = document.createElement("div");
            cover.setAttribute("class", "cover");

            cover.style.marginBottom = 0 - h + "px";
            cover.style.height = h + "px";

            $self.prepend(cover);
        });

        return this;
    };

    $.fn.ClearCover = $.fn.clearCover = function () {
        this.find(">.cover").remove();
        return this;
    };


    $.fn.UpFile = $.fn.upFile = function (option) {
        var jTHIS = $(this);
        var listFile;
        if (!option) option = {};
        //修复参数：
        if (!option.maxFile && option.max) {
            option.maxFile = option.max;
            delete option.max;
        }


        this.each(function () {

            //更多参数，详见： fileuploader.js 。
            var p = $.extend({
                sizeLimit: 0,
                maxFile: 1,
                data: false, //数据源。[{name:"文件一",id:2,url:"",size:"" }]
                element: this,
                hidden: this.id,
                edit: true,
                multiple: true,
                onSubmit: false,
                onComplete: false,
                listFile: "list"
            }, option);

            var self = $(this);
            var btnText = self.text();
            if (btnText) {
                p.uploadButtonText = btnText;
            }

            //参数
            p.on_Submit = p.onSubmit;
            p.on_Complete = p.onComplete;

            p.onSubmit = function (id, fileFullName) {
                var qqFile = this;
                var fs = fileFullName.mySplit(/\\|\//g, true);
                var name = fs[fs.length - 1].mySplit(".", true)[0];
                var res = name;
                //var res = window.showModalDialog(jv.Root + "html/FileName.html", name, "dialogWidth:380px;dialogHeight:140px;center:yes;resizable:yes;");
                //if (!res) {
                //    this._remove(id);
                //    return false;
                //}

                jv.StartLoading(true);
                qqFile._options.params["FileName"] = res;

                if (p.on_Submit) p.on_Submit(id, fileFullName, qqFile);
            };

            p.onComplete = function (i, fileFullName, res) {
                var qqFile = this;
                jv.StopLoading(true);

                if (res.msg) {
                    $(qqFile._element).trigger("removeOneFile");
                    alert(res.msg);
                    return;
                }

                res.qqFileId = i;

                if (p.listFile == "grid") {
                    TableFile.AppendItem(listFile, p, res);
                }
                else if (p.listFile == "list") {
                    ListFile.AppendItem(listFile, p, res);
                }

                if (p.on_Complete) p.on_Complete(i, fileFullName, res);
            };

            var instance = new qq.FileUploader(p);
            p.upFile = instance;
            self.data("upFile", instance);
            if (p.listFile == "grid") {
                var listFile = $(document.createElement("table")).insertAfter(this);
                new TableFile(listFile, p);
            }
            else if (p.listFile == "list") {
                var listFile = $(document.createElement("div")).insertAfter(this);
                new ListFile(listFile, p);
            }
        });

        $(".qq-upload-list", this).hide();
    };


    //强制触发。触发Hidden的change，input[readonly] 的change 等。不断完善。
    $.fn.EnforceTrigger = $.fn.enforceTrigger = function (eventName) {
        var check = function (jcon) {
            var con = jcon[0], tagName = con.tagName.toLowerCase();
            if (eventName == "change") {
                if (tagName == "input") {
                    if (jcon.attr("type") == "hidden") {
                        return true;
                    }
                    else if (jcon.attr("readonly")) {
                        return true;
                    }
                }
            }
            return false;
        };

        this.each(function () {
            var jd = $(this);
            if (check(jd)) jd.trigger(eventName);
        });

        return this;
    };

    $.fn.ReSet = $.fn.reSet = function () {
        var list = this.find("input");
        if (this.is("input")) { list.add(this);}
        for (var i = 0, len = list.length ; i < len; i++) {
            var self = list[i];
            var type = self.type;
            if (type == "button") continue;
            if (type == "hidden") continue;
            self.value = "";

            if (type == "file") {
                self.select();
                document.execCommand("Delete");
            }
        }

        var list = this.find("select");
        if (this.is("select")) { list.add(this); }
        for (var i = 0, len = list.length ; i < len; i++) {
            list[i].selectedIndex = 0;
        }
    };

    $.fn.MyCardTitle = $.fn.myCardTitle = function (setting) {
        var _shade = function (setting) {
            var height = 8,
            cv = setting.canvas,
            jod = $(cv);
            //            jod.parent().height(height);
            jod.attr("width", setting.width).attr("height", height);


            if ($.browser.msie && parseInt($.browser.version) < 9) {
                if (window.G_vmlCanvasManager) {
                    cv = window.G_vmlCanvasManager.initElement(cv);
                } else return false;
            }

            var ctx = cv.getContext('2d'),
            lg = ctx.createLinearGradient(0, 0, setting.width, 0);
            lg.addColorStop(0, setting.startColor);
            lg.addColorStop(1, setting.endColor);
            ctx.fillStyle = lg;
            ctx.fillRect(0, 0, setting.width, height);
            return true;
        };

        this.each(function (oi, od) {
            if ($(od).data("MyCardTitle") == true) return;
            $(od).data("MyCardTitle", true);
            if ($.browser.msie && (parseInt($.browser.version) <= 7)) {
                $(od).css("display", "inline");
            } else {
                $(od).css("display", "inline-block");
            }
            var pod = $(od).wrap("<div class='MyCardTitleWrap'></div>").parent().css("position", "relative"),
            cv = document.createElement("canvas");
            $(cv).css("position", "absolute").appendTo(pod);
            var container = pod.parent(),
            width = container.width() - $(od).width() - 3;
            if (width < 0) return;
            var endOpacity = parseInt($(od).attr("endopacity") || -50);
            if (endOpacity < 0) { width = 100 * width / (100 - endOpacity); }
            $(cv).css("bottom", 0).css("left", $(od).width() + parseInt($(od).css("paddingLeft")));
            _shade({ canvas: cv, startColor: $(od).css("backgroundColor"), endColor: jv.GetParentColor(pod), width: width, endOpacity: endOpacity });
        });
        return this;
    };


    $.fn.SmartPosition = $.fn.smartPosition = function ($target, width) {
        //弹出的 TextHelper position 须为  absolute
        //以 左上角为起点，顺时针计算依次为: a,b,c,d 四点。
        var jod = this;
        var thDiv = $target;
        var a = jod.offset(),
        c = { left: a.left + jod.outerWidth(), top: a.top + jod.outerHeight() },
        b = { left: c.left, top: a.top },
        d = { left: a.left, top: c.top };


        var $body = $(document.body);
        var body_width = $body.width();
        var body_height = $body.height();


        //对于TextHelper 来说， 内外宽度之差是固定的。
        //                        var th_diff_width= 
        //                            jv.GetInt( thDiv.css("marginLeft") ) +
        //                            jv.GetInt( thDiv.css("borderLeftWidth")) +
        //                            jv.GetInt( thDiv.css("paddingLeft")) +
        //                            jv.GetInt( thDiv.css("paddingRight")) + 
        //                            jv.GetInt( thDiv.css("borderRightWidth")) +
        //                            jv.GetInt ( thDiv.css("marginRight"))  ;
        var th_diff_width = 10;
        var th_diff_height = 10;

        //offset 会不断下移。

        thDiv.css("left", -3000).css("top", -3000).show();

        //智能算法， Input所在区间与offset 对应关系： 
        //如果在第一区间，肯定是d点
        //如果在第二区间，按d点设置宽度，获取高度，依次计算是否满足d,c
        //哪果在第三区间，按d点设置宽度，获取高度，依次计算是否满足 d,c,b
        //哪果在第四区间，按d点设置宽度，获取高度，依次计算是否满足 d,a

        if (a.left <= body_width / 2) {
            //第一区间
            if (a.top <= body_height / 2) {
                var outerMaxWidth = width || (body_width - a.left);
                thDiv.css("maxWidth", outerMaxWidth - th_diff_width);
                thDiv.offset(d);

                thDiv.css("maxHeight", body_height - d.top);
            }
                //第四区间
            else {
                var outerMaxWidth = width || (body_width - a.left);
                thDiv.css("maxWidth", outerMaxWidth - th_diff_width);

                var th_outerHeight = thDiv.outerHeight();
                if (d.top + th_outerHeight < body_height) {
                    thDiv.offset(d);

                    thDiv.css("maxHeight", body_height - d.top);
                } else {
                    //a点
                    var top = a.top - th_outerHeight;
                    thDiv.offset({ left: a.left, top: top > 0 ? top : 0 });

                    thDiv.css("maxHeight", a.top);
                }
            }
        }
        else {
            //第二区间：
            if (a.top <= body_height / 2) {
                var outerMaxWidth = width || (body_width - a.left);
                thDiv.css("maxWidth", outerMaxWidth - th_diff_width);

                //修改逻辑。 当在第二区间，按d点不出现折行时， 锁定d点。否则按c点。
                //if (d.top + thDiv.outerHeight() < body_height) {
                if (thDiv.outerWidth() < outerMaxWidth - th_diff_width) {
                    thDiv.offset(d);

                    thDiv.css("maxHeight", body_height - d.top);
                } else {
                    outerMaxWidth = width || d.left;
                    thDiv.css("maxWidth", outerMaxWidth - th_diff_width);

                    //c点
                    var th_outerHeight = thDiv.outerWidth();
                    var left = c.left - th_outerHeight;
                    thDiv.offset({ left: left > 0 ? left : 0, top: c.top });

                    thDiv.css("maxHeight", body_height - d.top);
                }
            }
                //第三区间
            else {
                var outerMaxWidth = width || (body_width - a.left);
                thDiv.css("maxWidth", outerMaxWidth - th_diff_width);

                //修改逻辑。 当在第三区间，按d点不出现折行时， 锁定d点。否则。。。
                //if (d.top + thDiv.outerHeight() < body_height) {
                if (thDiv.outerWidth() < outerMaxWidth - th_diff_width) {
                    thDiv.offset(d);

                    thDiv.css("maxHeight", body_height - d.top);
                } else {
                    //判断是否满足 c
                    outerMaxWidth = width || c.left;
                    thDiv.css("maxWidth", outerMaxWidth - th_diff_width);

                    var th_outerHeight = thDiv.outerHeight();

                    //修改逻辑。 当在第三区间，按c点不出现折行时， 锁定c点。否则。。。
                    //if (d.top + th_outerHeight < body_height) {
                    var th_outerWidth = thDiv.outerWidth();
                    if (th_outerWidth < outerMaxWidth - th_diff_width) {
                        //c点
                        var left = c.left - th_outerWidth;
                        thDiv.offset({ left: left > 0 ? left : 0, top: c.top });

                        thDiv.css("maxHeight", body_height - d.top);
                    } else {
                        //b点。
                        var left = b.left - th_outerWidth,
                        top = b.top - th_outerHeight;
                        thDiv.offset({
                            left: left > 0 ? left : 0,
                            top: top > 0 ? top : 0
                        });

                        thDiv.css("maxHeight", b.top);
                    }
                }
            }
        }
    };



    $.fn.PopTextArea = $.fn.popTextArea = function (jTextAreas) {
        this.focus(function () {
            var self = $(this);
            var size = jv.getEyeSize();
            var ta = '<textarea style="width:100%;padding:0px;margin:0px;height:' + (size.height - 240) + 'px">' + this.value + "</textarea>";
            var boxy = Boxy.html(ta, ["确 定", "取 消"], function (v, bxy) {
                if (v == "确 定") {
                    self.val(bxy.boxy.find("textarea").val());
                }
            }, { width: size.width - 200, title: "输入文本 - " + (self.closest(".kv").find(".key").text() || "") });

            boxy.boxy.find("textarea").focus();
            boxy.boxy.find(".answers .button[value='确 定']").addClass("large");
        });
    };

    $.fn.BindResize = $.fn.bindResize = function (events) {
        var self = this[0];
        //移动事件
        var mouseMove = function (e) {
            if (events.move) {
                events.move(self, e);
            }
        };

        //停止事件
        var mouseUp = function (e) {
            //在支持 releaseCapture 做些东东
            if (self.releaseCapture) {
                //释放焦点
                self.releaseCapture();
                //移除事件
                self.onmousemove = self.onmouseup = null;
            }
            else {
                //卸载事件
                $(document).unbind("mousemove", mouseMove).unbind("mouseup", mouseUp);
            }

            if (events.up) {
                events.up(self, e);
            }
        };


        this.mousedown(function (e) {
            //在支持 setCapture 做些东东
            if (self.setCapture) {
                //捕捉焦点
                self.setCapture();
                //设置事件
                self.onmousemove = function (ev) {
                    mouseMove(ev || event)
                };
                self.onmouseup = mouseUp;
            } else {
                //绑定事件
                $(document).bind("mousemove", mouseMove).bind("mouseup", mouseUp);
            }
            //防止默认事件发生
            e.preventDefault();

            if (events.down) {
                events.down(self, e);
            }
        });
    }

})(jQuery);


