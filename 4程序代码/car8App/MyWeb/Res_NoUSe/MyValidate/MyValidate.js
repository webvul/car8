/* <reference path="jquery.js"/>
用法：
my_check="type=int,minlen=0,maxlen=9,check=JQuery的表单对象列表,minvalue=2,maxvalue=5"

*/
(function ($) {
    $.extend($.fn, {
        my_check: function (sele) {
            defBlur = function (jObj) {
                if ((jObj.attr("formycheck") || false) == false) {
                    jObj.change(function () {
                        check1(jObj);
                    });
                    jObj.attr("formycheck", "true");
                }
            };
            check1 = function (jObj) {
                jObj.removeClass("MyCheckBg");
                jObj.next().filter(".MyCheckTip").remove();

                var ret = {};
                var d = jObj[0];
                var setting = {};
                var ary = jObj.attr("my_check").split(',');
                $.each(ary, function (_i, _d) {
                    var kv = _d.split('=');
                    setting[kv[0]] = kv[1];
                });

                if (!setting["check"]) { setting["check"] = "this" }

                setting["value"] = jObj.val();

                if (setting["value"].length < setting["minlen"]) {
                    ret.msg = "长度太小";
                    ret.obj = d;
                }
                else if (setting["value"].length > setting["maxlen"]) {
                    ret.msg = "长度太大";
                    ret.obj = d;
                }
                else if (setting["value"] > setting["maxvalue"]) {
                    ret.msg = "值太大";
                    ret.obj = d;
                }
                else if (setting["value"] < setting["minvalue"]) {
                    ret.msg = "值太小";
                    ret.obj = d;
                }
                else {
                    if (setting["type"] == "int") {
                        var valid = new RegExp("^\\d+(\\.\\d+)?$").test(setting["value"]);
                        if (valid == false) {
                            ret.msg = "应该是数值型";
                            ret.obj = d;
                        }

                    }
                    else if (setting["type"] == "date") {
                        var valid = new RegExp("^\\d{4}(\\-|\\/|\.)\\d{1,2}\\1\\d{1,2}$").test(setting["value"]);
                        if (valid == false) {
                            ret.msg = "应该是时间型";
                            ret.obj = d;
                        }
                    }
                    else if (setting["type"] == "email") {
                        var valid = new RegExp("^\\w+((-\\w+)|(\\.\\w+))*\\@[A-Za-z0-9]+((\\.|-)[A-Za-z0-9]+)*\\.[A-Za-z0-9]+$").test(setting["value"]);
                        if (valid == false) {
                            ret.msg = "应该是邮件格式";
                            ret.obj = d;
                        }
                    }
                }

                if (ret["obj"]) {
                    procError(ret);
                    return false;
                } else return true;
            };
            procError = function (ret) {
                if (ret["obj"]) {
                    $(ret.obj).addClass("MyCheckBg");
                    $('<label class="MyCheckTip">' + ret.msg + '</label>').insertAfter($(ret.obj));
                    return false;
                }
            };
            getSetting = function (setting) {
                var ret = Array();
                if (setting["type"]) ret.push("type=" + setting["type"]);
                if (setting["minlen"]) ret.push("minlen=" + setting["minlen"]);
                if (setting["maxlen"]) ret.push("maxlen=" + setting["maxlen"]);
                if (setting["minvalue"]) ret.push("minvalue=" + setting["minvalue"]);
                if (setting["maxvalue"]) ret.push("maxvalue=" + setting["maxvalue"]);
                return ret.join(",");
            };

            sele = sele || "body";
            var isValidate = true;
            $(".MyCheckBg", $(sele)).removeClass("MyCheckBg");
            $(".MyCheckTip", $(sele)).remove();

            $("[my_check]:visible", $(sele)).each(function (i, d) {
                var jObj = $(d);
                var setting = Object();
                var ary = jObj.attr("my_check").split(',');
                $.each(ary, function (_i, _d) {
                    var kv = _d.split('=');
                    setting[kv[0]] = kv[1];
                });

                if (!setting["check"]) { setting["check"] = "this" }

                if (setting["check"] == "this") {
                    defBlur(jObj);
                    isValidate &= check1(jObj);
                }
                else {
                    $(":" + setting["check"], jObj).each(function (_i, _d) {
                        $(_d).attr("my_check", getSetting(setting));
                        defBlur($(_d));
                        isValidate &= check1($(_d));
                    });
                    jObj.removeAttr("my_check");
                }
            });
            return isValidate;
        }
    });
})(jQuery);