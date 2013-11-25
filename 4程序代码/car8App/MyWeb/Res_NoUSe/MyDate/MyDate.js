(function ($) {
    $.fn.MyDate = function (options) {
        var p = $.extend({
            callback: false, //每次选择之后的回调.
            clear: "清除", //清除显示的文本,如果没有, 则没有清除.
            preYear: "<",
            nextYear: ">",
            today: "今天"  //显示今天
        }, options);

        var g = {};
        g.p = p;
        g.thisYear = new Date();
        g.thisFullYear = g.thisYear.getFullYear();

        //只接收 2011-01-01 08:22:44  或 2011/01/01 08:22:44 或相应短格式
        g.getDate = function (con, DefaultValue) {
            var val = $(con).val();
            val = val.replace(/-/g, "/");
            if ((val == "0001/01/01 0:00:00") || (val == "0001/1/1 0:00:00") || (val == "0001/1/1 00:00:00") || (val == "0001/01/01 00:00:00")) return DefaultValue || Date.MinValue;
            var dt = new Date(val);
            //这里有显示的Bug,如果是 0001/01/01 会显示 1901/01/01 ,但不影响使用.就算这样吧.
            if (dt.valueOf() && (dt != Date.MinValue)) return dt;
            else return DefaultValue || Date.MinValue;
        };


        //取某年某月共有多少天.其中 "1"月表示 1月.
        g.GetDays = function (year, month) {
            //Js技巧, new Date(2011,0,0) 返回 2020年最后一天.
            //new Date(2011,10,1).valueOf() -   86400000 是 9 月份的天数
            year = parseInt(year);
            month = parseInt(month);
            var date = new Date(year, month, 1);
            date.setTime(date.valueOf() - 86400000);
            return date.getDate();
        };

        g.setDate = function (con, deepth) {
            var jcon = $(con), jContainer = $("#" + jcon.attr("MyDateId")),
            monthHtml = $(".Month", jContainer).html(),
            yearHtml = $(".Year", jContainer).html(),
            dateSpan = $(".Date", jContainer),
            dt = (yearHtml || "0001") +
                    "/" + (monthHtml || "1") +
                    "/" + (dateSpan.html() || "1") +
                    " " + ($(".Hour", jContainer).html() || "00") +
                    ":" + ($(".Minute", jContainer).html() || "00") +
                    ":" + ($(".Second", jContainer).html() || "00");

            if (!deepth &&
                (new Date(dt).getMonth() != (parseInt(monthHtml || 1) - 1))
            ) {

                var val = g.GetDays(yearHtml || "0001", monthHtml || "1");
                if (parseInt(dateSpan.html()) > val) {
                    dateSpan.html(val);
                    $("[name=" + dateSpan.attr("id") + "]").val(val);
                }


                g.setDate(con, 1);
            }
            $(con).val(dt);
        };

        g.formatCallback = function (format, val, jspan) {
            if (!format) return val;

            var GetVal = function (func) {
                if (val instanceof Date) return func() + "";
                else return val + "";
            }

            if (format == "yyyy") {
                val = GetVal(function () { return val.getFullYear(); }).PadLeft(4, "0");
                if (jspan) jspan.addClass("Year").html(val);
                return val;
            }
            else if (format == "MM") {
                val = GetVal(function () { return val.getMonth() + 1; }).PadLeft(2, "0");
                if (jspan) jspan.addClass("Month").html(val);
                return val;
            }
            else if (format == "M") {
                val = GetVal(function () { return val.getMonth() + 1; });
                if (jspan) jspan.addClass("Month").html(val);
                return val;
            }
            else if (format == "dd") {
                val = GetVal(function () { return val.getDate(); }).PadLeft(2, "0");
                if (jspan) jspan.addClass("Date").html(val);
                return val;
            }
            else if (format == "d") {
                val = GetVal(function () { return val.getDate(); });
                if (jspan) jspan.addClass("Date").html(val);
                return val;
            }
            else if (format == "hh") {
                val = GetVal(function () { return val.getHours(); }).PadLeft(2, "0");

                if (jspan) jspan.addClass("Hour").html(val);
                return val;
            }
            else if (format == "h") {
                val = GetVal(function () { return val.getHours(); });
                jspan.addClass("Hour").html(val).val(val);
            }
            else if (format == "mm") {
                val = GetVal(function () { return val.getMinutes(); }).PadLeft(2, "0");

                if (jspan) jspan.addClass("Minute").html(val);
                return val;
            }
            else if (format == "m") {
                val = GetVal(function () { return val.getMinutes(); });
                if (jspan) jspan.addClass("Minute").html(val);
                return val;
            }
            else if (format == "ss") {
                val = GetVal(function () { return val.getSeconds(); }).PadLeft(2, "0");
                if (jspan) jspan.addClass("Second").html(val);
                return val;
            }
            else if (format == "s") {
                val = GetVal(function () { return val.getSeconds(); });
                if (jspan) jspan.addClass("Second").html(val);
                return val;
            }
            else if (format == "yy") {
                val = GetVal(function () { return val.getFullYear(); }).slice(2);

                if (jspan) jspan.addClass("Year").html(val);
                return val;
            }
            else {
                throw new Error("不识别的时间格式");
            }
        };
        g.DisplayDate = function (con, jContainer) {
            var jcon = $(con);
            var dateSpan = $(".Date", jContainer);
            if (dateSpan.length == 0) return;
            var days = {}, dayIndex = 0;
            var yearHtml = $(".Year", jContainer).html(),
                monthHtml = $(".Month", jContainer).html();

            var OneWeek = new Date(yearHtml + "/" + monthHtml + "/" + 1).getDay();

            for (var i = 0; i < OneWeek; i++) {
                days["d_" + dayIndex++] = "";
            }

            var allDays = g.GetDays(yearHtml, monthHtml);
            for (var i = 1; i <= allDays; i++) {
                days["d_" + dayIndex++] = i;
            }


            dateSpan.TextHelper({
                data: days,
                checkByKey: false,
                checkByVal: true,
                change: eval("(" + jcon.attr("change") + ")"),
                click: function (val, src) {
                    var jsrc = $(src), realDate = $(jv.GetDoer()).html();
                    jsrc.html(g.formatCallback(jsrc.attr("format"), realDate));

                    $("[name=" + jsrc.attr("id") + "]").val(realDate);
                    g.setDate(con);

                    var hourSpan = $(".Hour", jContainer);

                    return hourSpan.length && hourSpan.trigger(hourSpan.data("TextHelper").p.triggerEvent).length;

                }
            });

            var datejCon = $("#" + dateSpan.data("TextHelper").divId);
            datejCon.find("button").remove();
            var sps = datejCon.find("span");
            sps.eq(6).after("<br />");
            sps.eq(13).after("<br />");
            sps.eq(20).after("<br />");
            sps.eq(27).after("<br />");
            sps.eq(34).after("<br />");
        };

        g.CreateHtml = function (con) {
            var jcon = $(con),
                ary = jv.SplitWithReg(jcon.attr("format"), new RegExp("\{\\w+\}", "g")),
                date = g.getDate(con),
                html = document.createDocumentFragment();

            $(ary).each(function (i, d) {
                if (d.Split) {
                    var span = document.createElement("span"), format = d.Value.replace("{", "").replace("}", ""),
                    jspan = $(span);

                    jspan
                    .attr("format", format)
                    .addClass("TextHelperInput");
                    g.formatCallback(format, date, jspan);
                    html.appendChild(span);
                }
                else {
                    var pre = document.createTextNode(d.Value);
                    html.appendChild(pre);
                }
            });
            var container = document.createElement("span");
            container.appendChild(html);
            var id = jv.MyGuid();
            $(container).attr("id", id).addClass("MyDateDisplay");
            jcon.hide().attr("MyDateId", id).after(container);
            return container;

        };
        g.GetYears = function (con, curYear) {
            var years = [];
            var year = curYear || g.getDate(con, g.thisYear).getFullYear();
            var jcon = $(con);
            //            jcon.data("curYear", year);

            var preCount = 8;
            years.push(p.preYear);

            if (p.today) {
                years.push(p.today);
            }
            else {
                years.push(year - preCount - 1);
            }

            years.push(g.thisFullYear);

            if (p.clear) {
                years.push(p.clear);
            }
            else {
                years.push(year + preCount + 2);
            }

            years.push(p.nextYear);
            var repearCount = 0;

            for (var i = year - preCount; i < year - preCount + 15; i++) {
                if (i == g.thisFullYear) {
                    repearCount++;
                    continue;
                }
                years.push(i);
            }

            for (var i = year - preCount + 15; i < year - preCount + 15 + repearCount; i++) {
                years.push(i);
            }

            //tidy
            for (var i = 0; i < years.length; i++) {
                years[i] = { key: "y_" + i, val: years[i] };
            }
            return years;
        };

        g.UnMyDate = function (con) {
            var jcon = $(con);
            jcon.data("MyDate", false);

            var span = jcon.next();
            if (span.hasClass("MyDateDisplay")) {
                span.remove();
                jcon.show();
            }
        };
        var _Date = function (con) {
            var jcon = $(con);
            if (jcon.data("MyDate")) { return true; }
            jcon.data("MyDate", g);



            var jContainer = $(g.CreateHtml(con));

            jContainer.find("span").andSelf().css("width", "auto").css("minWidth", "0px");

            var yearSpan = $(".Year", jContainer),
             monthSpan = $(".Month", jContainer),
             dateSpan = $(".Date", jContainer),
             hourSpan = $(".Hour", jContainer),
             minuteSpan = $(".Minute", jContainer),
             secondSpan = $(".Seconed", jContainer);

            var oriSel;
            if (yearSpan.length) {
                yearSpan.TextHelper({ data: g.GetYears(con),
                    checkByKey: false,
                    checkByVal: true,
                    change: eval("(" + jcon.attr("change") + ")"),
                    beforeClickInput: function (od) {
                        var jsrc = $(jv.GetDoer());
                        var html = jsrc.text();

                        if (html == p.preYear || (html == p.nextYear)) {
                            var minYear = jsrc.nextAll("br").first().next().text();
                            var maxYear = jsrc.nextAll("span:last").text();

                            var years = g.GetYears(con, html == p.preYear ? parseInt(minYear) - 7 : parseInt(maxYear) + 9), curIndex = 0;
                            var sps = jsrc.parents(".TextHelper:first").find("span");
                            sps.removeClass("check");
                            var curYear = g.getDate(con).getFullYear();
                            for (var i = 0; i < years.length; i++) {
                                var kv = years[i];
                                $(sps[curIndex]).attr("val", kv.key).html(jv.encode(kv.val));
                                if (kv.val == curYear) {
                                    $(sps[curIndex]).addClass("check");
                                }

                                curIndex++;
                            }


                            return false;
                        }
                    },
                    click: function (val, src) {
                        var jsrc = $(src), txt = $(jv.GetDoer()).text();
                        if (txt == p.clear) {
                            $(con).val("");
                            g.UnMyDate(con);
                            _Date(con);
                            return;
                        }
                        else if (txt == p.today) {
                            var now = new Date();
                            $(con).val(now.getFullYear() + "/" + (now.getMonth() + 1) + "/" + now.getDate() + " " + now.getHours() + ":" + now.getMinutes() + ":" + now.getSeconds());
                            g.UnMyDate(con);
                            _Date(con);
                            return;
                        }
                        jsrc.html(g.formatCallback(jsrc.attr("format"), txt));
                        g.setDate(con);
                        return monthSpan.length && monthSpan.trigger(monthSpan.data("TextHelper").p.triggerEvent).length;
                    }
                });

                $("#" + yearSpan.data("TextHelper").divId).find("button").remove();
                var sps = $("#" + $(".Year", jContainer).data("TextHelper").divId).find("span");
                sps.eq(4).after("<br />");
                sps.eq(9).after("<br />");
                sps.eq(14).after("<br />");
                sps.eq(0).width(28);
                sps.eq(4).width(28);
            }

            var monthData = [];
            for (var i = 1; i <= 12; i++) {
                monthData.push({ key: "m_" + i, val: i });
            }
            if (monthSpan.length) {
                monthSpan.TextHelper({
                    data: monthData,
                    checkByKey: false,
                    checkByVal: true,
                    change: eval("(" + jcon.attr("change") + ")"),
                    click: function (val, src) {
                        var jsrc = $(src), txt = $(jv.GetDoer()).text();
                        jsrc.html(g.formatCallback(jsrc.attr("format"), txt));
                        g.setDate(con);
                        g.DisplayDate(con, jContainer);
                        return dateSpan.length && dateSpan.trigger(dateSpan.data("TextHelper").p.triggerEvent).length;
                    }
                });

                $("#" + monthSpan.data("TextHelper").divId).find("button").remove();
                var sps = $("#" + $(".Month", jContainer).data("TextHelper").divId).find("span");
                sps.eq(3).after("<br />");
                sps.eq(7).after("<br />");
            }

            g.DisplayDate(con, jContainer);
            if (hourSpan.length) {
                hourSpan.TextHelper({
                    data: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23],
                    checkByKey: false,
                    checkByVal: true,
                    change: eval("(" + jcon.attr("change") + ")"),
                    click: function (val, src) {
                        var jsrc = $(src);
                        jsrc.html(g.formatCallback(jsrc.attr("format"), val));
                        g.setDate(con);
                        return minuteSpan.length && minuteSpan.trigger(minuteSpan.data("TextHelper").p.triggerEvent).length;

                    }
                });
                $("#" + hourSpan.data("TextHelper").divId).find("button").remove();
                var sps = $("#" + hourSpan.data("TextHelper").divId).find("span");
                sps.eq(7).after("<br />");
                sps.eq(15).after("<br />");
            }

            if (minuteSpan.length) {
                var minutes = {};
                for (var i = 0; i < 12; i++) {
                    minutes[i * 5 + ""] = i * 5;
                }

                minuteSpan.TextHelper({
                    data: minutes,
                    checkByKey: false,
                    checkByVal: true,
                    change: eval("(" + jcon.attr("change") + ")"),
                    click: function (val, src) {
                        var jsrc = $(src);
                        jsrc.html(g.formatCallback(jsrc.attr("format"), val));
                        g.setDate(con);
                        return secondSpan.length && secondSpan.trigger(secondSpan.data("TextHelper").p.triggerEvent).length;

                    }
                });

                $("#" + minuteSpan.data("TextHelper").divId).find("button").remove();
                var sps = $("#" + minuteSpan.data("TextHelper").divId).find("span");
                sps.eq(3).after("<br />");
                sps.eq(7).after("<br />");
            }

            if (secondSpan.length) {
                var minutes = [];
                for (var i = 0; i < 60; i++) {
                    minutes[i] = i;
                }
                secondSpan.TextHelper({
                    data: minutes,
                    checkByKey: false,
                    checkByVal: true,
                    change: eval("(" + jcon.attr("change") + ")"),
                    click: function (val, src) {
                        var jsrc = $(src);
                        jsrc.html(g.formatCallback(jsrc.attr("format"), val));
                        g.setDate(con);

                        return false;
                    }
                });

                $("#" + secondSpan.data("TextHelper").divId).find("button").remove();
                var sps = $("#" + secondSpan.data("TextHelper").divId).find("span");
                sps.eq(9).after("<br />");
                sps.eq(19).after("<br />");
                sps.eq(29).after("<br />");
                sps.eq(39).after("<br />");
                sps.eq(49).after("<br />");
            }
        };


        this.each(function (i, d) {
            _Date(d);
        });


        return this;
    };
})(jQuery);