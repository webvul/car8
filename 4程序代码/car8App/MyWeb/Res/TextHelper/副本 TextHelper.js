
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
            , useSelect: (jv.csm ? false : true)       //如果是不是Csm 强制使用select控件。
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

        var lastTimer = null;
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
            var sel = jd.after('<select {0}></select>'.format(selAttr.join(" "))).next();
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
                    $.ajax({ url: $.isFunction(p.url) ? p.url() : p.url, data: {}, type: "post", success: function (res) {

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

                        if (lastTimer) { lastTimer.stop(); }


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
                                if (lastTimer) lastTimer.stop();
                                lastTimer = $.timer(500, function (timer) {
                                    timer.stop();
                                    g.post(function () { g.showMe(); });
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

                        $.ajax({ url: $.isFunction(p.url) ? p.url() : p.url, data: paras, type: "post", success: function (res) {
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

                        jod.smartPosition(thDiv,p.width);

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
})(jQuery);
