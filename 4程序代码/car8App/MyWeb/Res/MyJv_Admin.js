
(function () {

    //本页标志性含数 getDoer .
    if (jv.flexiRowEvent) return;

    history.Back = history.back;

    //重写系统的 history.back
    history.back = function () {
        if (document.referrer) { document.location = document.referrer; }
        else history.Back(arguments);
    };


    //兼容 FF,IE 下加载数据岛.
    jv.JqLoadXml = function (XmlID) {
        var xmlDoc = $("#" + XmlID);
        if (jv.IsNull(window.ActiveXObject) == true) {
            if (jv.IsNull(xmlDoc.attr("src")) == false) {
                xmlDoc = $(jv.loadXML(xmlDoc.attr("src")));
            }
        }
        return xmlDoc;
    };

    //兼容 FF,IE 加载XML文件.
    jv.LoadXML = jv.loadXML = function (xmlpath) {
        var xmlDoc = null;
        if (window.ActiveXObject) {
            xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
        } else if (document.implementation && document.implementation.createDocument) {
            xmlDoc = document.implementation.createDocument("", "", null);
        }
        else {
            alert('Your browser cannot handle this script.');
        }
        xmlDoc.async = false;
        xmlDoc.load(xmlpath);
        return xmlDoc;
    };

    //清除 Input type=file 的值 .
    jv.ClearFileValue = jv.clearFileValue = function (file) {
        //IE下默认 Input 的 File 的 Value 属性是只读的。
        //var file = document.getElementById(FildID);
        file.value = "";
        file.select();
        document.execCommand("Delete");
    };

    jv.GetHexColor = jv.getHexColor = function (Color) {
        if (Color == "transparent") return Color;
        var element = document.createElement("div"), cssProperty = "backgroundColor", mozillaEquivalentCSS = "background-color", actualColor;
        $(element).css("backgroundColor", Color);

        if (element.currentStyle) {
            actualColor = element.currentStyle[cssProperty];
            return actualColor;
        } else {
            var cs = document.defaultView.getComputedStyle(element, null);
            actualColor = cs.getPropertyValue(mozillaEquivalentCSS);
            cs = actualColor.substring(0, actualColor.length - 1).substring(4).mySplit(',');
            return "#" + parseInt(cs[0]).toString(16).padLeft(2, "0") + parseInt(cs[1]).toString(16).padLeft(2, "0") + parseInt(cs[2]).toString(16).padLeft(2, "0");
        }
    }



    jv.GetOffsetColor = jv.getOffsetColor = function (Color, Offset) {
        var c = parseInt("0x" + jv.GetHexColor(Color).substr(1));
        var r = (c >> 16) & 0xFF;
        r += parseInt(r * Offset / 255); r = r < 0 ? 0 : r > 255 ? 255 : r;
        var g = (c >> 8 & 0xFF) & 0xFF;
        g += parseInt(g * Offset / 255); g = g < 0 ? 0 : g > 255 ? 255 : g;
        var b = c & 0xFF;
        b += parseInt(b * Offset / 255); b = b < 0 ? 0 : b > 255 ? 255 : b;

        return "#" + r.toString(16).padLeft(2, "0") + g.toString(16).padLeft(2, "0") + b.toString(16).padLeft(2, "0");
    };
    jv.GetParentColor = jv.getParentColor = function (MyObj) {
        if (jv.IsNull(MyObj)) return "white";
        var Obj = $(MyObj)[0];
        if (Obj.tagName.toLowerCase() == "html") return "white";

        var p = $(Obj).parent();

        if (jv.IsNull(p)) return "white";

        var bg = p.css("background-color");
        if (bg == "transparent") {
            return jv.GetParentColor(p[0]);
        }
        return bg;
    }

    jv.FindParentsClass = jv.findParentsClass = function (Obj, ClassName) {
        if (jv.IsNull(Obj)) return null;
        var p = $(Obj).parent();
        if (p.length == 0) return null;
        if (p.filter('.' + ClassName).length > 0) return p;
        else { return jv.FindParentsClass(p, ClassName); }
        return null;
    }
    //在自定义行上注册方法。如 onclick= jv.flexiRowEvent(jv.page().FlexiView,event); 参数同：format,onpress.
    jv.FlexiRowEvent = jv.flexiRowEvent = function (callback, ev) {
        if (!callback) return;
        var jdoer = $(jv.GetDoer());
        var g = jdoer.getFlexi();
        var row = g.getjTr(jdoer);
        jdoer = jdoer.closest("td");
        callback(g.findRowData(g.data.rows, row.attr("id").slice(3)), row.index(), g, jdoer, ev);
    };

    jv.ReSortFlexi = jv.reSortFlexi = function (options) {
        var grid = options.table,
        SortColumn = options.SortColumn,
        callback = options.callback;

        $(grid.bDiv).find("table tr td.first").hover(function () {
            $(this).addClass('vDrag');
        }, function () { $(this).removeClass('vDrag'); });

        var startIndex, endIndex;

        $(grid.bDiv).find("table").tableDnD({
            dragHandle: "first", onDragClass: "tDnD_whileDrag",
            onDragStart: function (table, row) {
                startIndex = $(row).closest("tr").index();
            },
            onDrop: function (table, row) {
                //做Ajax回发,进行保存
                endIndex = $(row).index();
                var startRow = $(grid.bDiv).find("table tbody tr:eq(" + startIndex + ")");
                var endRow = $(grid.bDiv).find("table tbody tr:eq(" + endIndex + ")");

                var startSortID = grid.TableRowToData(startRow)[SortColumn];
                var endSortID = grid.TableRowToData(endRow)[SortColumn];

                callback(startSortID, endSortID);
            }
        });
    };




    jv.HighlightLogin = jv.highlightLogin = function (valMsg) {
        return valMsg.replace(/登录/g, "<a target=_top href=" + jv.Root + "Login.aspx>登录</a>");
    };


    jv.MyInitOn["Admin.Style"] = function () {
        var jbody = $(document.body);

        if (!window.top.frames.length) {
            var padLeft = parseInt(jbody.css("paddingRight"));
            if (padLeft) {
                jbody.css("paddingLeft", padLeft + "px");
            }
        }
    };

    jv.MyLoadOn["MyDate"] = function (container) {
        container = container || document;
        //处理三种情况. MyDate,MyTime,MyDateTime
        //之后整理一下所有依赖属性表示自动化处理的类.还有 FillHeight

        //    if (!$.fn.datepicker && $(".MyDate:first").length) {
        //        jv.LoadJsCss("css", jv.Root + "Res/jquery.ui/css/cupertino/jquery-ui-1.7.3.custom.css");
        //    }

        $(".MyDate", container).click(function () { WdatePicker({ $dpPath: jv.Root + "Res/My97/" }); });
        $(".MyTime", container).click(function () { WdatePicker({ dateFmt: 'HH:mm:ss' }); });
        $(".MyDateTime", container).click(function () { WdatePicker({ dateFmt: 'yyyy-MM-dd HH:mm:ss' }); });

        //        if ($.fn.datepicker) $(".MyDate", container).datepicker();
        //        if ($.fn.timepicker) $(".MyTime", container).timepicker({ timeOnly: true });
        //        $(".MyDateTime", container).MyDateTime();

        //    if ($.fn.datetimepicker) $(".MyDateTime").datetimepicker({ timeOnly: false });


        //    $(".MyDate").each(function (i, d) {
        //        if (!d.id) return true;
        //        Calendar.setup({
        //            inputField: d.id,
        //            trigger: d.id,
        //            onSelect: function () { this.hide() }
        //        });
        //    });

        //    $(".MyDateTime").each(function (i, d) {
        //        if (!d.id) return true;
        //        Calendar.setup({
        //            inputField: d.id,
        //            trigger: d.id,
        //            showTime: true,
        //            onSelect: function () { this.hide() }
        //        });
        //    });
    };


    jv.GetErrorFromPage = jv.getErrorFromPage = function (response) {
        var response = $(response), msg = response.filter("title").text();
        if (!msg) {
            for (var i = 0, responseLen = response.length; i < responseLen; i++) {
                msg = $(response[i]).text();
                if (!!msg) break;
            }
        }

        return msg || "";
    };

    $(function () {
        jv._AjaxTimer;
        jv._TimerCount = 0;

        //当有  ajax 时自动出现 正在加载, 或者可以通过  forceLoading 来指定.
        jv.StartLoading = jv.startLoading = function (forceLoading) {
            if (window.Boxy && Boxy.getOne()) return;

            if (!forceLoading) {
                if (jQuery.active !== 1) {
                    return;
                }

                var src = jv.getDoer(function (obj) { if (obj && (obj.type == "ajaxStart")) return false; });
                if (src && src.tagName && (src.tagName == "INPUT")) {
                }
                else return;
            }
            else {
                jv.PageLoading = true;
            }

            jv._TimerCount = 0;

            $.timer(500, function (timer) {
                timer.stop();

                if (forceLoading) {
                    if (!jv.PageLoading) return;
                }
                else if (jQuery.active == 0) {
                    return;
                }

                if ($(document.body).find(">.loadingDiv").length) return;
                //if (Boxy.getOne()) return;

                jv.loadingDiv();

                //            jv._TimerBoxy = Boxy.load(null, 
                //                { modal: true, title: "", opacity: 0.2, hideAnimateTime: 0, afterHide: function (bxy) {
                //                        if (jv._AjaxTimer) { jv._AjaxTimer.stop(); }
                //                    }
                //                }, 
                //                function(bxy) { 
                //                    $(bxy.boxy).addClass("Ignore") ; 
                //                }
                //            );

                //            var msg = jv._TimerBoxy.boxy
                //                .find(".boxy-inner")
                //                .append('<div style="text-align:center;padding:10px;"/>')
                //                .find("div:last");

                var $loadingDiv = $(document.body).find(">.loadingDiv");



                jv._AjaxTimer = $.timer(1000, function (tim) {
                    if (forceLoading && !jv.PageLoading) return;


                    jv._TimerCount++;
                    $loadingDiv.html(jv._TimerCount + "秒过去了...");
                });

            });
        };

        jv.StopLoading = jv.stopLoading = function (forceLoading) {
            if (forceLoading) {
                jv.PageLoading = false;
            }


            var $body = $(document.body);
            $body.find(">.loadingDiv").remove();
            $body.clearCover();

            if (jv._AjaxTimer && jv._AjaxTimer.stop) jv._AjaxTimer.stop();
            jv._TimerCount = 0;
            jv.PageLoading = false;
        };


        var body = $(document.body);
        if (body.data("events") && body.data("events")["ajaxStart"]) return;

        body.ajaxStart(function () {
            $(this).css("cursor", "wait");
            jv.StartLoading();
        })
        .ajaxStop(function (event, request, settings) {
            $($(document).data("OneClick") || []).removeAttr("disabled");
            $(document).data("OneClick", null);
            $(this).css("cursor", "default");

            jv.StopLoading();
        });


    });
})();