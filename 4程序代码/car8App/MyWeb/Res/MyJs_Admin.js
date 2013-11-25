
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
/* Copyright (c) 2009 Mustafa OZCAN (http://www.mustafaozcan.net)
* Dual licensed under the MIT (http://www.opensource.org/licenses/mit-license.php)
* and GPL (http://www.opensource.org/licenses/gpl-license.php) licenses.
* Version: 1.0.2
* Requires: jquery.1.3+
*/
jQuery.fn.fixTableHeader = function (options) {
    var settings = jQuery.extend({
        size: 1,
        scrollObj: document.body
    },
    options);
    this.each(function () {
        var $tbl = $(this);
        var $tblhfixed = $tbl.find("tr:lt(" + settings.size + ")");
        var headerelement = "th";
        var $header = $tblhfixed.find(headerelement);
        if ($header.length == 0) {
            headerelement = "td";
            $header = $tblhfixed.find(headerelement);
        }
        if ($header.length === 0) {
            return;
        }

        var $clonedTable = $tbl.clone();

        //去除样式标记
        $clonedTable[0].className = "";

        $clonedTable.find("thead,tbody,tfoot").empty();


        $clonedTable.css({
            "position": "fixed",
            "top": $(settings.scrollObj).offset().top,
            "left": $tbl.offset().left
        })
            .addClass("fixedTableHeader")
            .width($tbl.outerWidth())
            .hide();

        if ($clonedTable.find("thead").length == 0) {
            $clonedTable.append(document.createElement("thead"));
        }

        var $rows = $tblhfixed.clone();

        $rows.each(function (index) {
            var oriCells = $tblhfixed.eq(index).find(headerelement);
            $(this).find(headerelement).each(function (i) {
                $(this).width($(oriCells[i]).width());
            });
        });

        $clonedTable.find("thead").append($rows);

        $tbl.after($clonedTable);

        $((settings.scrollObj == document || settings.scrollObj == document.body) ? window : settings.scrollObj).scroll(function () {
            if (jQuery.browser.msie && jQuery.browser.version == "6.0") $clonedTable.css({
                "position": "absolute",
                "top": $(settings.scrollObj).offset().top,
                "left": $tbl.offset().left
            });
            else $clonedTable.css({
                "position": "fixed",
                "top": $(settings.scrollObj).offset().top,
                "left": $tbl.offset().left - $(window).scrollLeft()
            });
            var sctop = $(window).scrollTop();
            var elmtop = $tblhfixed.offset().top;
            if (sctop > elmtop && sctop <= (elmtop + $tbl.height() - $tblhfixed.height())) $clonedTable.show();
            else $clonedTable.hide();
        });
        $(window).resize(function () {
            if ($clonedTable.outerWidth() != $tbl.outerWidth()) {
                $tblhfixed.find(headerelement).each(function (index) {
                    var self = $(this);
                    var w = self.width();
                    self.css("width", w);
                    $clonedTable.find(headerelement).eq(index).css("width", w);
                });
                $clonedTable.width($tbl.outerWidth());
            }
            $clonedTable.css("left", $tbl.offset().left);
        });

    });

};
/*
* Flexigrid for jQuery - New Wave Grid
*
* Copyright (c) 2008 Paulo P. Marinas (webplicity.net/flexigrid)
* Dual licensed under the MIT (MIT-LICENSE.txt)
* and GPL (GPL-LICENSE.txt) licenses.
*
* $Date: 2008-07-14 00:09:43 +0800 (Tue, 14 Jul 2008) $

修改记录：
1. moreCol:用于复合表头。
2. colModel 列定义，Json数组，每项Json除 bind 属性外，其它属性，都支持回调。 format 和 html 是运行时回调（有行数据参数），其它是静态回调（即没有参数）

display ： 字符串， 表示显示名称
toggle  ： 布尔， 表示是否可切换显示列。
bind    ： 布尔， 不支持回调， 表示是否是数据绑定列
name    :  字符串 ， 表示唯一name
 
css   ： 字符串，表示列的class
sortable    ： 布尔，表示是否可排序，刷新排序
align   ：字符串（left,right,center）,表示对齐方式
width   ：字符串（可设 auto）,数字 ，表示列宽度。 如果为 0 ， 表示隐藏。
format  ：function，模板定义，表示单元格内容。 参数： rowData,rowIndex,grid,td,event           －－－－－之后重构参数，参数只有一个对象 grid ，但 grid 包含当前行，当前列索引，和取得Json数据等方法。
html    ：同 format
maxWidth    :字符串，数据，表示列的最大宽度
hide    : 布尔，表示是否隐藏该列
onpress ：click 事件。参数同 format

format: 用于格式化显示方式，有两种方式，
1).$#$ 静态方式： <a href="~/$Id$.aspx" >$#$</a> 用相应的列替换。
2).  colModel . callback(cells,index) 回调， 传入的是从服务器返回的数据的该行的 cells，以及，该单元格所在 Cells 的索引。
3. Url 中的 _Ref_Type_ 有两种值： radio , check 
4. 如果打开页的Body 中有 _Ref_Value_  (JSON对象{code:"1",name:"张三"}) . 调用该对象,绑定 .trSelect 及 QueteValue 类.
moreCol: function () {
var tr = document.createElement('tr');
{
var th1 = document.createElement('th');
$(th1).html("Hello");
$(th1).attr("colspan", "6");
$(tr).append(th1);
}
{
var th1 = document.createElement('th');
$(th1).html("Hellorrrr");
$(th1).attr("colspan", "6");
$(tr).append(th1);
}

$("th", $(tr)).each(function (i, d) {
$(d).css("border", "1px solid #cccccc");
$(d).css("text-align", "center");
});
return [tr];
},
5. name  可以是 Text , 也可以是 function 类型.进行动态绑定 .
*/

// p.colModel 运行时属性 , 表头的每个列, 绑定数据"col"
// headDisplay

(function ($) {
    $.addFlex = function (t, p) {
        if (t.grid) return false; //return if already exist	
        var jt = $(t);
        //如果是多主键,则键可以用 "," 分隔, 返回多选格式为:  1+3+4,2+4+2,29+48+49
        p.role = $.extend({ id: "Id", name: "Name", select: "IsSelected" }, p.role);
        p.role.tree = p.role.tree || p.role.name;
        p._Ref_Type_ = p._Ref_Type_ || jv.url().attr("_Ref_Type_");

        // apply default properties
        p = $.extend({
            ajaxUrl: false, //异步加载子节点时调用.
            autoload: false,
            blankHtml: "&nbsp;",
            buttons: [], // 可以指定 id , style ,bclass ,onpress .
            cgwidth: false,
            colModel: [],   //列定义，每一项内容是Json数据 
            dataType: 'json', // type of data loaded
            defwidth: 300,
            dbclick: function (Tr) {
                if (p._Ref_Type_ == "radio") {
                    if (p.onSelect && (p.onSelect(Tr, g) === false)) return false;
                    if (p.onSelected) { p.onSelected(Tr, g); }

                    //一个页面一次只允计调用一个弹出页.
                    if (p.RefCallback) {
                        //IE下不能跨页面传递事件源，所以显式指定。

                        jv.Hide(function () { p.RefCallback(p.role, p.PageQuote, g, { originalEvent: true, target: Tr }); });
                    }
                }
            },
            draggable: false,
            errormsg: '发生异常',
            FillHeight: true,    //flexigrid容器和 bDiv 容器是否应用 FillHeight
            fixTableHeader: false,
            height: jv.oriCss(t, "height") || 'auto', //default height
            hideOnSubmit: true,
            IdName: "序号",
            keepQuote: false, //保持跨页面选中。
            method: 'POST', // data sending method
            minColToggle: 1, //minimum allowed column to be hidden
            minwidth: 20, //min width of columns
            maxwidth: 1200, //max width of columns
            minheight: 80, //min height of table.
            moreCol: false,
            newButton: function (btnDefine, grid) {
                //参数
                if (!btnDefine) return false;
                if (!this.container) return false;
                if (btnDefine.separator) return false;

                var con = $(this.container, jv.boxdy());

                var btn = document.createElement("input");
                btn.type = "button";
                btn.className = btnDefine.bclass;
                btn.value = btnDefine.name;
                btn.onclick = function (ev) {
                    btnDefine.onpress(grid.getSelectIds(), grid, ev);
                }

                con.append(btn);

                return true;
            },
            newp: 1,
            nohresize: false,
            nomsg: '没有数据',
            //nowrap: true, 
            novstripe: false,
            onChangeSort: false,

            //onlyHeadtd: true,    //只有td 模式，不产生td里的 Div。
            onSelect: false,    //三个参数： tr ,grid , 是否是全选触发
            onSelected: false,
            onSubmit: false, // using a custom populate function
            onSuccess: false,
            onToggleCol: false,
            multiSelect: true,
            page: 1, //current page
            pages: 1,
            params: {},
            procmsg: '正在处理，请稍候...',
            pagestat: '显示第 {from} 到 {to} 条记录，共{total}条记录。',
            preProcess: false,
            qtype: '',
            query: '',
            repeatHeader: false,
            resizable: false, //resizable table
            role: { id: "Id", name: "Name", tree: "Name", select: "IsSelected", group: "", groupStyle: "1px solid green" },  //如果是树，仅叶子节点可以分组。按组先排序。
            take: 15, // results per page
            rpFixed: false,
            rpOptions: [5, 10, 15, 20, 25, 40, 50],
            selectExtend: "",   //树型时，选择某一行同时，是向上选，还是向下选: 可选值： up,down
            checkSelectName: "<div class='inpbox check Inline'></div>全选",
            radioSelectName: "选择",
            selectType: "all",   //全选时，点击头部时，是全选，还是反选。要用值： all,un
            showHeader: true,
            showToggleBtn: false, //show or hide column toggle popup
            sortorder: "asc",
            striped: true, //apply odd even stripes
            thead: false,   //自定义额外的Header,可以是Function，返回数据是 Json， Json的数组（多行）
            tfoot: false,   //自定义Footer,可以是Function，返回数据是 Json， Json的数组（多行）
            toolBar: [".BoxyTool", ".PageTool", ".MyTool"],    //依次查找在定义的工具条。如果找到，则采用系统的工具条。如果找不到，采用flexigrid的工具条。
            tooltip: true, //默认ToolTip
            total: 0, //total pages
            treeOpen: true, //默认树是否打开. 
            //title: false,
            useId: false,
            useSelect: false,   //使用全选，可用值，布尔，before，after,int(列索引)， 如果为true=before
            url: false, //ajax url
            usepager: false, //
            width: jv.oriCss(t, "width") || 'auto', //auto width
            //wrapTd: true,
            //            cellDefaultMaxWidth: 500,   //默认单元格列的最大宽度.
            //-------------------------------------
            _Ref_Type_: false,       //单选还是多选 .
            //            resizeCallback: function () {
            //                $(g.hDiv).width(jt.width());
            //                var hrow = $(g.hDiv).find("table tr.nulRow th");
            //                var trow = jt.find("tr.nulRow th");
            //                $.each(g.keys, function (i, key) {
            //                    var col = g.cols[key];

            //                    if (col.width == "auto") {
            //                        hrow.eq(i).width(trow.eq(i).width());
            //                    }
            //                });
            //            },
            //            scrollCallback: function () {
            //                $(g.hDiv).scrollLeft(jt.parent().scrollLeft());
            //            },
            //            _Ref_Value_: false,      //引用值. 字符串格式
            RefCallback: false,   //引用回调. 单引和多引参数不同.
            PageQuote: [],   //是一个Json数组。表示本页面的要返回的值。
            DataSource: false      //指定数据源.而不进行 Ajax 进行数据绑定 , 有三个参数： (cols, param, g) ，也可以是FlexiJson数据。
            //            InitMinus: []            //多选时,点击清空,初始化 差 值.
        }, p);

        //        $(window).bind("resize", p.resizeCallback);
        //        jt.parent().bind("scroll", p.scrollCallback); //scroll 没有调试好。
        //p.pages = 总页数； p.page ＝ 当前页 ； p.newp ＝ 分页时，新的页码。

        if (!p.take && p.rp) { p.take = p.rp }


        p.role = $.extend({ id: "Id", name: "Name", tree: "Name", select: "IsSelected", group: "", groupStyle: "1px solid green" }, p.role);

        //        if (p.FillHeight) {
        //            jv.boxdy(null, window).bind("resize", function () { if (window.onresize) { window.onresize(); } });
        //        }


        if (p._Ref_Type_) {
            if (p._Ref_Type_ == "radio") {
                p.multiSelect = false;
                p.selectExtend = "";
            }
            //  PageQuoteCode 是程序在本页可操作的引用值.
            p.PageQuote = (p.PageQuote.length ? p.PageQuote : (window.opener && window.opener._PageQuote_) ) || [];

            p.RefCallback = p.RefCallback || (window.opener && window.opener._Ref_Callback_);

            p.PageQuote = jQuery.extend([], p.PageQuote);

            if (p._Ref_Type_.length > 1) {
                for (var i = 0; i < p.colModel.length; i++) {
                    var model = p.colModel[i];
                    if (!model) continue;
                    if (model.bind === false && (model.name != "View")) {
                        if (!model.css || ("," + model.css + ",").indexOf(",pin,") < 0) {
                            model.hide = true;
                        }
                    }
                }
            }
            //有DataSource 则用DataSource 的数据源. 而不进行 Ajax  
            p.DataSource = jv.boxdy()[0].DataSource ||
                        (window.opener && window.opener.DataSource);
        }

        if (p.toolBar && p.toolBar.length) {
            $.each(p.toolBar, function (i, d) {
                var con = $(d, jv.boxdy());
                if (!con.length) return;
                if (con[0].style.display == "none") return;
                if (!p.container) {
                    p.container = d;
                }

                if (p._Ref_Type_ && (p._Ref_Type_.length > 1)) {
                    con.find("input").each(function () {
                        var self = this;
                        if (self.style.display == "none") return;
                        if (jv.nodeHasClass(self, "query")) return;
                        if (jv.nodeHasClass(self, "pin")) return;
                        self.style.display = "none";
                    });

                    //处理一下 p.buttons 的定义
                    for (var i = 0, len = p.buttons.length ; i < len; i++) {
                        var item = p.buttons[i];
                        var bc = " " + item.bclass + " ";
                        if (bc.indexOf("query") >= 0) return;
                        if (bc.indexOf("pin") >= 0) return;
                        p.buttons.removeAt(i);
                        i--;
                        len--;
                    }
                }
            });
        }





        //添加当前页码
        if (p.rpOptions.indexOf(p.take) < 0) {
            p.rpOptions.push(p.take);
            p.rpOptions.sort(function compare(a, b) { return a - b; });
        }

        //改变当前执行代码的事件发生源. 不改变也没有问题. 
        //在LoadView 时, 事件源是 容器.
        //jv.CreateEventSource(p, t);

        //分解 grid 对象中的私有方法。
        var tm = {
            newTreeRows: function (rowData, rowLevel, newRowFunc, evt) {
                if (rowData === true) return [];
                if (!rowLevel) rowLevel = 0;

                var row = newRowFunc();
                g.dataToRow(rowData,
                         row,
                         rowLevel,
                         rowData.rows === true ? true : (rowData.rows ? rowData.rows.length : 0));


                if (g.data.isTree) {
                    g.addTreeProp(row);


                    if (rowData.rows !== true && rowData.rows) {

                        for (var i = 0, len = rowData.rows.length; i < len; i++) {
                            var d = rowData.rows[i];
                            if (d === true) continue;
                            tm.newTreeRows(d, rowLevel + 1, newRowFunc, { originalEvent: true, target: evt || jv.getDoer() });
                        }
                    }
                }

            },

            //分页时清除选中数据。
            removePageQuoteData: function () {
                if (p._Ref_Type_ == "check" || p._Ref_Type_ == "radio" || p.keepQuote) {
                }
                else {
                    g.clearSelectRow();
                }
            },

            updateHeaderChk: function () {
                if (p.useSelect) {
                    //大部分是 chkpart
                    //
                    var hasSelected = null, hasUnSelected = null;

                    for (var i = 0, len = t.tBodies.length; i < len; i++) {
                        if (hasSelected !== null && (hasUnSelected !== null)) break;
                        var body = t.tBodies[i];
                        for (var j = 0, rl = body.children.length; j < rl; j++) {
                            if (hasSelected !== null && (hasUnSelected !== null)) break;
                            var row = body.children[j];
                            var rowClass = " " + row.className + " ";
                            if (rowClass.indexOf(" trSelected ") >= 0) {
                                hasSelected = true;
                            }
                            else {
                                hasUnSelected = true;
                            }
                        }
                    }

                    if (!tm.chk || !tm.chk.length) {
                        tm.chk = tm.chk || jt.find("thead tr .inpbox");
                    }

                    if (hasSelected && hasUnSelected) {
                        tm.chk.removeClass("chked").addClass("chkpart");
                    }
                    else if (hasSelected) {
                        tm.chk.removeClass("chkpart").addClass("chked");
                    }
                    else {
                        tm.chk.removeClass("chked").removeClass("chkpart");
                    }
                }
            },
            tempReplaceFunc: function (lmth, find, replaceFunc) {
                var toFind = "$" + find + "$";
                if (lmth.indexOf(toFind) >= 0) {
                    var curVal = replaceFunc();
                    lmth = lmth.replace(new RegExp("\\$" + find + "\\$", "g"), curVal);
                }
                return lmth;
            },

            decideBtnClick: function (srcObj) {
                srcObj = srcObj || jv.GetDoer();
                var srcTag = srcObj.tagName.toLowerCase();
                var retVal = srcTag == "a" || (srcTag == "button") || (srcTag == "input");
                if (retVal) return retVal;
                //找父节点是否是
                srcObj = srcObj.parentNode;
                if (!srcObj) return false;
                srcTag = srcObj.tagName.toLowerCase();
                if (srcTag == "div" || srcTag == "td" || srcTag == "tr") return false;
                return tm.decideBtnClick(srcObj);
            },
            setUpSelect: function (tr) {
                var jtr = $(tr), level = jtr.data("rowLevel");
                var selectExtend = (p.selectExtend || "").mySplit(",", true);
                var isSel = g.isSelected(jtr);

                if (!g.data) return;
                if ((g.data.isTree && isSel) && (selectExtend.indexOf("up") > -1)) {


                    //向上遍历
                    while (true) {
                        jtr = jtr.prev();
                        if (jtr.length == 0) break;

                        var theLevel = jtr.data("rowLevel");
                        if (level > theLevel) {

                            //执行onsel ect 事件
                            if (p.onSelect && p.onSelect(jtr[0], g) === false) break;

                            g.setSelectRow(jtr);

                            //执行onsel ected 事件
                            if (p.onSelected) p.onSelected(jtr[0], g);

                            level = theLevel;
                        }
                    }
                }
            },
            setDownSelect: function (tr) {
                var jtr = $(tr), level = jtr.data("rowLevel");
                var selectExtend = (p.selectExtend || "").mySplit(",", true);
                var isSel = g.isSelected(jtr);

                if (!g.data) return;
                if (g.data.isTree && (selectExtend.indexOf("down") > -1)) {

                    //向下遍历.
                    while (true) {
                        jtr = jtr.next();
                        if (jtr.length == 0) break;
                        var theLevel = jtr.data("rowLevel");
                        if (level >= theLevel) break;

                        //执行onsel ect 事件
                        if (p.onSelect && p.onSelect(jtr[0], g) === false) break;

                        if (isSel) {
                            g.setSelectRow(jtr);
                        }
                        else {
                            g.setUnSelectRow(jtr);
                        }
                        if (p.onSelected) {
                            p.onSelected(jtr[0], g);
                        }
                    }
                }
            },
            btnPress: function (ev) {
                var jDoer = $(jv.GetDoer());
                if (jDoer.attr("disabled")) {
                    return false;
                }
                var fbtn = jDoer.closest(".fbutton");
                if (fbtn.attr("disabled")) return false;
                if (fbtn.find(".submit").length) jv.SetDisable(fbtn);

                var res = ev.data.btn.onpress(g.getSelectIds(), g, ev);
                if (res === false) {
                    if (jDoer.hasClass(".fbutton")) jDoer.UnOneClick();
                    else { jv.UnOneClick(jDoer.closest(".fbutton")); }
                }
                return res;
            },
            headerOver: function (thInd, thData) {
                var jthis = $(this).addClass('thOver');
                var col = g.cols[g.keys[this.cellIndex]];

                if (col.sortable) {
                    if (col.name != p.sortname) {
                        jthis.addClass('s' + p.sortorder);
                    }
                    else {
                        var no = '';
                        if (p.sortorder == 'asc') no = 'desc';
                        else no = 'asc';
                        jthis.removeClass('s' + p.sortorder).addClass('s' + no);
                    }
                }
            },
            headerOut: function () {
                var jthis = $(this).removeClass('thOver');

                var col = g.cols[g.keys[this.cellIndex]];

                if (col.sortable) {
                    if (col.name == p.sortname) {
                        var no = '';
                        if (p.sortorder == 'asc') no = 'desc';
                        else no = 'asc';

                        jthis.addClass('s' + p.sortorder).removeClass('s' + no);
                    }
                    else {
                        jthis.removeClass('s' + p.sortorder);
                    }
                }


                //                if (g.colCopy) {
                //                    $(g.cdropleft).remove();
                //                    $(g.cdropright).remove();
                //                    g.dcolt = null;
                //                }
            },
            findPOne: function (onejTr) {
                onejTr = $(onejTr);
                if (!onejTr.length) return null;
                else return onejTr.closest("tr");
            },
            btnRadioOkPress: function (ids, grid, ev) {

                //必须从当前选中页面，点确定，才能正确返回。
                var sels = $(".trSelected", grid.bDiv);

                if (sels.length == 1) {
                    p.dbclick(sels[0]);
                }
                else if (sels.length > 1) {
                    alert("单选状态下，请选择一个返回。");
                    return;
                }
                else {
                    var ret = p.PageQuote;
                    if (!ret.length) {
                        ret = [{}];
                        ret.IsEmpty = true;
                    }

                    jv.Hide(function () { p.RefCallback(p.role, ret, g, { originalEvent: true, target: jv.GetDoer() }); });

                    //                    if (p.PageQuote[0] && (p.PageQuote[0][p.role.id] == p.PageQuote[0].id)) {
                    //                        return;
                    //                    }
                    //                    else {
                    //                        jv.Hide(function () { p.RefCallback(p.role, [{}], { originalEvent: true, target: jv.GetDoer() }); });
                    //                    }
                }
            },
            btnCheckOkPress: function (ids, grid, ev) {
                var ret = p.PageQuote;
                if (!ret.length) ret.IsEmpty = true;
                jv.Hide(function () { p.RefCallback(p.role, ret, g, { originalEvent: true, target: jv.GetDoer() }); });

            },
            subMenuPress: function (ev) {
                var jsubDiv = $(".flexi_subMenu:first");
                if (jsubDiv.length == 0) {
                    $("body").append('<div class="flexigrid flexi_subMenu" />');
                    jsubDiv = $(".flexi_subMenu:first");
                }
                jsubDiv.html("");


                var btnDiv_source = $(jv.GetDoer()).closest(".subBtn"),
                                    data = ev.data["btns"];
                for (j = 0, dataLen = data.length; j < dataLen; j++) {
                    if (data[j].separator) {
                        $("<div class='menu-split'></div>").appendTo(jsubDiv);
                        continue;
                    }
                    var subbtn = data[j],
                                        menuItem = $('<span>' + g.getName(subbtn.name) + '</span>');

                    if (subbtn.bclass) menuItem.addClass(subbtn.bclass); // class="' + (subbtn.bclass || '') + '"
                    if (subbtn.id) menuItem.attr("id", subbtn.id);
                    if (subbtn.style) menuItem.attr("style", subbtn.style);
                    menuItem.css("paddingLeft", 20).css("paddingRight", 20);

                    menuItem
                                        .wrap('<div class="menuitem"></div>')
                                        .parent()
                                        .bind("click", { btn: btnDiv_source, press: subbtn.onpress }, function (ev) {
                                            jsubDiv.hide();
                                            if (ev.data.press) {
                                                ev.data.press(g.getSelectIds(), g, ev);
                                            }
                                        })
                                        .appendTo(jsubDiv);
                }
                jsubDiv.css({ "left": btnDiv_source.offset().left, "top": btnDiv_source.offset().top + btnDiv_source.outerHeight() - 1 });
                if (jsubDiv.width() < 60) jsubDiv.width(60);

                jsubDiv.show();

                ev.stopPropagation();
                $("body").one("click", function () {
                    jsubDiv.hide();
                });
            },

            newAjaxRows: function (jRootTr, rowData, rowLevel, jPrevTr) {
                if (jv.IsNull(rowLevel)) rowLevel = jRootTr.data("rowLevel") + 1;
                if (jv.IsNull(jPrevTr)) jPrevTr = jRootTr[0];

                //记录一下,和上一个节点相比, 相差级数.正数是降级. 负数是升级 . 0 是同级.
                var trs = [],
                    row = document.createElement("tr");

                var jrow = $(row).insertAfter(jPrevTr);

                g.dataToRow(rowData, row, rowLevel, rowData.rows === true ? true : (rowData.rows ? rowData.rows.length : 0));

                g.addTreeProp(jrow);

                if (rowData.rows != true && !!rowData.rows) {
                    $.each(rowData.rows, function (i, rowData) {
                        if (d === true) return;
                        newAjaxRows(jRootTr, rowData, rowLevel + 1, jrow);
                    });
                }
            },

            ajaxNodes: function ($addon) {
                $addon.one("click.ajaxNode", function (ev) {
                    var tr = jv.getParentTag($addon[0], "TR");


                    g.populate({ url: p.ajaxUrl.format(tr.id.slice(3)) }, tm.addAjaxNode, tr, true);
                });
            },

            addAjaxNode: function (data, tr) {

                //把当前数据加入到 g.data
                g.updateData(data, tr.id.slice(3));

                $(g.block).remove();
                g.loading = false;

                if (!data || !data.rows || !data.rows.length) return;

                var currentIndex = tr.rowIndex + 1;
                //先遍历每一行.
                if (data.isTree) {
                    var jtr = $(tr);

                    $.each(data.rows, function (i, rowData) {
                        if (rowData === true) return;
                        tm.newAjaxRows(jtr, rowData, jtr.data("rowLevel") + 1);
                    });
                }
                else {
                    //理论上讲, Ajax加载,都是Ajax加载树节点, 不会到这的. 
                    throw Error("Ajax加载要求是加载树节点.请检查代码.");
                }
            },
            //设置引用数据，支持多个行设置。
            setPageQuoteFromRow: function (jTr) {
                //更新 PageQuoteCode
                //if (!p.keepQuote && !p._Ref_Type_) return;

                $.each(g.getjTr(jTr), function () {
                    var jtr = $(this);
                    var trVal = jtr.attr("id").slice(3);
                    if (!trVal) return;

                    g.setPageQuoteValue(trVal, g.isSelected(jtr));
                });
            }
        }; //   临时的Model 数据。 分担 g 对象压力。不对外。


        //create grid class,标准对象！
        var g = {
            hset: {},
            rePosDrag: function () {
                return;
                var prevPos = 0, hideDrag = false;
                if ($('thead tr:last th:visible', jt).length < 2) {
                    hideDrag = true;
                }

                $('thead tr:last th', jt).each
				(
			 	    function (i, d) {
			 	        var jd = $(d), n = jd.index(),
			 	        dragDiv = $('div:eq(' + n + ')', g.cDrag);
			 	        if (!hideDrag && jd.is(":visible")) {

			 	            var curPos = prevPos + jd.width() +
			 	            parseInt(jd.css("paddingLeft")) +
			 	            parseInt(jd.css("paddingRight")) +
                            parseInt(jd.css("borderLeftWidth")) +
                            parseInt(jd.css("borderRightWidth"));


			 	            //经调试,这里差一个像素.
			 	            dragDiv.css({ left: curPos - 1 });
			 	            prevPos = curPos;

			 	            //下面两个++ 很奇怪.
			 	            if ($.browser.msie) {
			 	                prevPos = prevPos + 2;
			 	            }
			 	        }
			 	        else {
			 	            dragDiv.css({ left: "0px" });
			 	            dragDiv.hide();
			 	        }
			 	    }
				);
            },
            fixHeight: function (newH) {
                return;
                newH = newH || p.height;
                //                var hdHeight = $(this.hDiv).height();
                //$('div', this.cDrag).each(
                //		function () {
                //		    $(this).height(newH/* + hdHeight */ - 18); //有滚动条的时候
                //		}
                //	);

                $(g.bDiv).height(newH);

                //var hrH;
                //if (g.vDiv && (p.height != 'auto') && p.resizable) {
                //    hrH = g.vDiv.offsetTop;
                //}
                //else {
                //    hrH = g.bDiv.offsetTop + newH;
                //}
                //$(g.rDiv).css({ "height": hrH });
            },

            dragStart: function (dragtype, e, obj) { //default drag function start

                if (dragtype == 'colresize') //column resize
                {
                    $(g.nDiv).hide(); $(g.nBtn).hide();
                    var n = $('div', this.cDrag).index(obj),
                    oth = $('thead tr:last th:eq(' + n + ')', jt),
                    ow = oth.width() - jv.GetInt(oth.css("borderLeftWidth")) - jv.GetInt(oth.css("borderRightWidth"))
                            - jv.GetInt(oth.css("paddingLeft")) - jv.GetInt(oth.css("paddingRight"))
                    //假设 oth 内的DivMargin 为0.
                    ;


                    $(obj).addClass('dragging'); //.siblings().hide();
                    $(obj).prevUntil("div:visible").eq(0).addClass('dragging').show();

                    this.colresize = { startX: e.pageX, ol: parseInt(obj.style.left), ow: ow, n: n };
                    $('body').css('cursor', 'col-resize');
                }
                else if (dragtype == 'vresize') //table resize
                {
                    var hgo = false;
                    $('body').css('cursor', 'row-resize');
                    if (obj) {
                        hgo = true;
                        $('body').css('cursor', 'col-resize');
                    }

                    if (p.width == 'auto') {
                        p.width = $(this.gDiv).width();
                    }

                    this.vresize = { h: p.height, sy: e.pageY, w: p.width, sx: e.pageX, hgo: hgo };

                }

                else if (dragtype == 'colMove') //column header drag
                {
                    if (p.draggable) {
                        $(g.nDiv).hide(); $(g.nBtn).hide();
                        this.hset = $(this.bDiv).offset();
                        var jhead = $('table:not(.fixedTableHeader) thead', this.bDiv);
                        this.hset.right = this.hset.left + jhead.width();
                        this.hset.bottom = this.hset.top + jhead.height();
                        this.dcol = obj;
                        this.dcoln = jhead.find("th").index(obj);

                        this.colCopy = document.createElement("div");
                        this.colCopy.className = "colCopy";
                        this.colCopy.innerHTML = obj.innerHTML;
                        if ($.browser.msie) {
                            this.colCopy.className = "colCopy ie";
                        }


                        $(this.colCopy).css({ "position": 'absolute', "float": 'left', "display": 'none', "textAlign": obj.align });
                        $('body').append(this.colCopy);
                        $(this.cDrag).hide();
                    }
                }

                $('body').noSelect();

            },
            dragMove: function (e) {

                if (this.colresize) //column resize
                {
                    var n = this.colresize.n,
                    diff = e.pageX - this.colresize.startX,
                    nleft = this.colresize.ol + diff,
                    nw = this.colresize.ow + diff;

                    if (nw > p.minwidth) {
                        $('div:eq(' + n + ')', this.cDrag).css('left', nleft);
                        this.colresize.nw = nw;
                    }
                    else { }
                }
                else if (this.vresize) //table resize
                {
                    var v = this.vresize,
                    y = e.pageY,
                    diff = y - v.sy;

                    if (!p.defwidth) p.defwidth = 300;

                    if (!p.nohresize && v.hgo) {
                        var x = e.pageX,
                        xdiff = x - v.sx,
                        newW = v.w + xdiff;
                        if (newW > p.defwidth) {
                            this.gDiv.style.width = newW + 'px';
                            p.width = newW;
                        }
                    }

                    var newH = v.h + diff;
                    if ((newH > p.minheight || p.height < p.minheight) && !v.hgo) {
                        this.bDiv.style.height = newH + 'px';
                        p.height = newH;
                        this.fixHeight(newH);
                    }
                    v = null;
                }
                else if (this.colCopy) {
                    $(this.dcol).addClass('thMove').removeClass('thOver');
                    if (e.pageX > this.hset.right || e.pageX < this.hset.left || e.pageY > this.hset.bottom || e.pageY < this.hset.top) {
                        $('body').css('cursor', 'move');
                    }
                    else
                        $('body').css('cursor', 'pointer');
                    $(this.colCopy).css({ "top": e.pageY + 10, "left": e.pageX + 20, "display": 'block' });
                }

            },
            dragEnd: function () {
                g.standardRow = null;

                if (this.colresize) {
                    var n = this.colresize.n,
                    nw = this.colresize.nw;

                    var setWidth = function (nWidth, isWithTd) {
                        var th = $('thead tr:last th:eq(' + n + ')', jt);

                        th.find('>div').css('width', nWidth);

                        if (isWithTd) th.css('width', nWidth);

                        $('tbody tr', jt).each(
									    function () {
									        var td = $('td:eq(' + n + ')', $(this));

									        td.find(">div").css('width', nWidth);
									        if (isWithTd) td.css('width', nWidth);
									    }
								    );
                    };

                    setWidth(nw, true);

                    p.colModel[n].width = nw;
                    $('.dragging', this.cDrag).removeClass('dragging');
                    this.rePosDrag();

                    this.fixHeight();
                    this.colresize = false;
                }
                else if (this.vresize) {
                    this.vresize = false;
                }
                else if (this.colCopy) {
                    $(this.colCopy).remove();
                    if (this.dcolt != null) {


                        if (this.dcoln > this.dcolt)
                            $('thead tr:last th:eq(' + this.dcolt + ')', jt).before(this.dcol);
                        else
                            $('thead tr:last th:eq(' + this.dcolt + ')', jt).after(this.dcol);



                        this.switchCol(this.dcoln, this.dcolt);
                        $(this.cdropleft).remove();
                        $(this.cdropright).remove();


                    }

                    this.dcol = null;
                    this.hset = null;
                    this.dcoln = null;
                    this.dcolt = null;
                    this.colCopy = null;

                    this.rePosDrag();
                    $('.thMove', this.bDiv).removeClass('thMove');
                    $(this.cDrag).show();
                }
                $('body').css('cursor', 'default');
                $('body').noSelect(false);
            },
            toggleCol: function (name, visible) {
                var bDiv = this.bDiv;
                var n = $("thead tr:last th[bindName=" + name + "]", bDiv).index();
                //                var isCheck = $('input[value=' + name + ']:checked', g.nDiv).length > 0;

                //                visible = (visible || isCheck);


                //                if ($('input:checked', g.nDiv).length < p.minColToggle && !visible) return false;


                var headRowLen = 1;
                $('tr', t).each
							(
								function () {
								    //复合表头, 不能切换显示.

								    if (visible) {
								        if (headRowLen > 0) {
								            headRowLen--;
								            $("th:eq(" + n + ")", this).css("display", "");
								        }
								        else {
								            $('td:eq(' + n + ')', this).css("display", "");
								        }
								        $(".cDrag>div:eq(" + n + ")", bDiv).show();
								        p.colModel[n].headDisplay = true;
								    }
								    else {
								        if (headRowLen > 0) {
								            headRowLen--;
								            $("th:eq(" + n + ")", this).hide();
								        }
								        else {
								            $('td:eq(' + n + ')', this).hide();
								        }

								        $(".cDrag>div:eq(" + n + ")", bDiv).hide();
								        p.colModel[n].headDisplay = false;
								    }
								}
							);

                this.rePosDrag();

                if (p.onToggleCol) p.onToggleCol(cid, visible);

                return false;
            },
            switchCol: function (cdrag, cdrop) { //switch columns

                $('tbody tr', t).each
					(
						function () {
						    if (cdrag > cdrop)
						        $('td:eq(' + cdrop + ')', this).before($('td:eq(' + cdrag + ')', this));
						    else
						        $('td:eq(' + cdrop + ')', this).after($('td:eq(' + cdrag + ')', this));
						}
					);

                //switch order in nDiv
                if (cdrag > cdrop)
                    $('tr:eq(' + cdrop + ')', this.nDiv).before($('tr:eq(' + cdrag + ')', this.nDiv));
                else
                    $('tr:eq(' + cdrop + ')', this.nDiv).after($('tr:eq(' + cdrag + ')', this.nDiv));

                //                if ($.browser.msie && $.browser.version < 7.0) 
                //                    $('tr:eq(' + cdrop + ') input', this.nDiv)[0].checked = true;

            },
            scroll: function () {
            },
            //row: 是指Json中的行数据. col: 是指列定义.
            getColContent: function (row, col, td) {
                var oriVal = row.cell[g.getColModel(col.name)["indexOfBind"]], lmth; // html 的倒写.
                if (col.format) {
                    if ($.isFunction(col.format)) {
                        //传参: 行数据,行索引,全部数据
                        lmth = col.format(row, tm.rowIndex, g, td, oriVal) + "";

                        if (jv.IsNull(lmth)) {
                            var colIndex = g.getColModel(col.name)["indexOfBind"];
                            lmth = row.cell[colIndex];
                        }
                    }
                    else {
                        lmth = col.format + "";
                    }

                    $.each(g.keys, function (_i, k) {
                        var m = g.cols[k];
                        lmth = tm.tempReplaceFunc(lmth, k, function () { return row.cell[m["indexOfBind"]]; });
                    });

                    lmth = tm.tempReplaceFunc(lmth, "#", function () { return row.cell[g.cols[col.name]["indexOfBind"]]; });
                }
                else {
                    lmth = oriVal;
                }

                return lmth || p.blankHtml;
            },
            //设置选中行的值  trVal = idOrjTrOrRowData
            setPageQuoteValue: function (trVal, isSet) {
                var rjv = g.getJson(trVal);

                //                    if (!("id" in rjv)) {
                //                        rjv["id"] = rjv[p.role.id];

                //                        if (!("name" in rjv)) {
                //                            rjv["name"] = rjv[p.role.name];
                //                        }
                //                    }

                if (p._Ref_Type_ == "radio") {
                    p.PageQuote = [rjv];

                    //                        if (p.role.refName.length) {
                    //                            var refVal = [];
                    //                            p.role.refName.each(function (i, d) {
                    //                                refVal.push(rjv[d]);
                    //                                p.PageQuoteRefValue = refVal;
                    //                            });
                    //                        }
                }
                else {
                    var quoteIndex = -1;
                    $.each(p.PageQuote, function (index) {
                        var n = this;

                        if ("id" in n) {
                            if ((n["id"] == rjv.id) || n["id"] == rjv[p.role.id]) {
                                quoteIndex = index;
                                return false;
                            }
                        }
                        if (p.role.id in n) {
                            if ((n[p.role.id] == rjv.id) || n[p.role.id] == rjv[p.role.id]) {
                                quoteIndex = index;
                                return false;
                            }
                        }
                    });

                    if (isSet) {
                        if (quoteIndex < 0) {
                            p.PageQuote.push(rjv);
                        } else {
                            var keys = jv.GetJsonKeys(rjv);
                            for (var i = 0, len = keys.length; i < len; i++) {
                                var k = keys[i];
                                p.PageQuote[quoteIndex][k] = rjv[k];
                            }
                        }
                    }
                    else {
                        if (quoteIndex >= 0) {
                            jv.remove(p.PageQuote, function (n) {
                                if ("id" in n) return n["id"] == trVal;
                                else if (p.role.id in n) return n[p.role.id] == trVal;
                            });
                        }
                    }
                }
            },
            getRowCount: function (rowData) {
                if (!rowData) return 0;
                if (!rowData.length) return 0;
                var rc = rowData.length;
                $.each(rowData, function () {
                    rc += g.getRowCount(this);
                });
                return rc;
            },

            //设置在引用时选中的样式，仅在第一次加载时执行。

            setQuoteSelect: function (rows) {
                g.setQuoteData(rows);
                g.setQuoteStyle(rows);
            },
            setQuoteData: function (rows) {
                if (p.PageQuote) {
                    rows = rows || g.data.rows;
                    if (rows === true) return;
                    if (!rows.length) return;
                    var celIndex = g.getColModel(p.role.name).indexOfCell;
                    var idValIndex = g.getColModel(p.role.id).indexOfBind;

                    //如果引用数据量大，则循环该页的行数据。
                    //if (refData.length > g.getRowCount(rows)) 

                    $.each(rows, function () {
                        if (this.rows) g.setQuoteData(this.rows);
                        var rowData = this;
                        var refIndex = jv.indexOf(p.PageQuote, function (n) {
                            return n.id == rowData.cell[idValIndex] || n.id == rowData.id;
                        });

                        if (refIndex >= 0) {
                            var tr = document.getElementById("row" + rowData.id);
                            if (!tr) return;
                            g.setSelectRow(tr);
                        }
                    });
                }
            },
            setQuoteStyle: function (rows) {
                if (p.PageQuote) {
                    rows = rows || g.data.rows;
                    if (rows === true) return;
                    if (!rows.length) return;
                    var celIndex = g.getColModel(p.role.name).indexOfCell;
                    var idValIndex = g.getColModel(p.role.id).indexOfBind;

                    //如果引用数据量大，则循环该页的行数据。
                    //if (refData.length > g.getRowCount(rows)) 

                    $.each(rows, function () {
                        if (this.rows) g.setQuoteStyle(this.rows);
                        var rowData = this;
                        var refIndex = jv.indexOf(p.PageQuote, function (n) {
                            return n.id == rowData.cell[idValIndex] || n.id == rowData.id;
                        });

                        if (refIndex >= 0) {
                            var tr = document.getElementById("row" + rowData.id);
                            if (!tr) return;


                            var nameTd = tr.cells[celIndex];
                            if (nameTd) {
                                var nameDivs = jv.getNodes(nameTd, function (node) { return node.tagName == "DIV"; });
                                if (nameDivs.length) {
                                    nameTd = nameDivs[0];
                                }

                                nameTd.style.outline = "dotted 1px coral";
                            }
                        }
                    });
                }
            },
            addTreeProp: function (jtr) {
                jtr = $(jtr);
                if (g.data.isTree && p.role.tree) {

                    var node = jtr.find(".addon");
                    node.css("width", (jtr.data("rowLevel") || 0) * 12 - 3 + 8 + "px");
                    var bindClick = function () {
                        $(jv.getParentTag(node[0], "TD")).click(function (ev) {
                            var srcObj = jv.GetDoer(ev);
                            if ($(srcObj).hasClass("addon")) {
                                g.toggleTree(srcObj, ev.altKey);
                                ev.stopPropagation();
                            }
                        });
                    };

                    var hasChild = jtr.data("hasChild");
                    if (hasChild === true) {
                        node.html("±");
                        node.data("status", "close");

                        tm.ajaxNodes(node);
                        bindClick();
                    }
                    else if (hasChild) {
                        node.html("-");
                        node.data("status", "open");
                        bindClick();
                    }


                }
            },

            //把行数据添加到表格.不支持树。 rowIndex 默认是表格最后一行。对外使用 addRow
            dataToRow: function (rowData, row, rowLevel, hasChild) {
                if (rowData === true) return null;
                tm.rowIndex++;


                if (tm.rowIndex % 2 && p.striped) row.setAttribute("class", 'erow');
                row.id = 'row' + rowData.id;
                var jrow = $(row);
                //选中列的内容。
                var selText;

                var first = false;
                for (var i = 0, len = g.keys.length; i < len; i++) {
                    var col = g.cols[g.keys[i]];
                    if (!col) return;

                    var td = document.createElement("td");
                    row.appendChild(td);

                    var tdClass = [];
                    if (col["css"]) { tdClass.push(col["css"]); }


                    if (!col.headDisplay) td.style.display = "none";

                    //设置属性
                    //                    if (col.width > 0) {
                    //                        td.style.width = col.width + "px";
                    //                    }

                    //                    if (col.maxWidth) {
                    //                        td.style.maxWidth = col.maxWidth + "px";
                    //                    }

                    if (col.align) {
                        td.setAttribute("align", col.align);
                    }

                    if (col.sortable && p.sortname && (p.sortname == col.name)) {
                        tdClass.push('sorted');
                    }


                    if (!first && !p.useSelect && col.headDisplay) {
                        tdClass.push("first");
                        first = true;
                    }

                    if (tdClass.length) td.setAttribute("class", tdClass.join(" "));


                    var lmth = g.getColContent(rowData, col, td);


                    var addOn = "";
                    if (g.data.isTree && (col.name == p.role.tree)) {
                        addOn = '<span class="addon"></span>';

                        jrow.data("rowLevel", rowLevel || 0).data("hasChild", hasChild || false);
                    }


                    if (col.align) {
                        td.align = col.align;
                    }

                    td.innerHTML = addOn + lmth;



                    if (col.name == p.role.select) {
                        selText = lmth;

                    }

                    if (col.onpress) {
                        td.onclick = function (ev) {
                            return col.onpress(rowData, tm.rowIndex, g, jd, ev);
                        };
                    }


                } //end each

                if ((selText == "true" || selText == "True" || selText == "是" || parseInt(selText) > 0)) {
                    if (p.onSelect && (p.onSelect(row, g) === false)) return false;

                    g.setSelectRow(row);

                    var nameTd = row.cells[g.getColModel(p.role.name)["indexOfCell"]];
                    if (nameTd) {
                        var nameDivs = jv.getNodes(nameTd, function (node) { return node.tagName == "DIV"; });
                        if (nameDivs.length) {
                            nameTd = nameDivs[0];
                        }

                        nameTd.style.outline = "dotted 1px coral";
                    }

                    if (p.onSelected) p.onSelected(row, g);
                }
            },

            //添加新行,支持树(如果是树，则必须是根节点). 对外API. rowData 是 Json 数据 {id:1,cells:[]}
            addRow: function (rowData, rowLevel) {
                if (!rowData) return;
                if (!g.data) {
                    g.data = {};
                    g.data.rows = [];
                    g.data.total = 0;
                }
                $(g.block).remove();

                g.data.rows.push(rowData);
                g.data.total += 1;

                var jtbody = jt.find(">tbody");
                if (jtbody.length == 0) {
                    jt.append("<tbody />");
                    jtbody = jt.find(">tbody");
                }

                //更新行数信息. 未实现. 
                tm.newTreeRows(rowData, null, function () {
                    var tr = document.createElement("tr");
                    jtbody[0].appendChild(tr);
                    return tr;
                });


                //更新 tfooter.
                g.createFooter();

                //                if ((jt.oriCss("height") || "auto") == "auto") {
                //                    jv.boxdy(null, window).trigger("resize");
                //                }
            },
            //支持树（如果是树，则必须是根节点）,对外API
            deleteRow: function (rowId) {
                if (!rowId) return;
                if (!g.data) return;

                $(g.block).remove();

                g.data.rows.Recursion(
                    function (con, i, item) {
                        return item.rows;
                    },
                    function (con, i, item) {
                        if (item.id == rowId) {
                            con.removeAt(i);
                            return false;
                        }
                    }
                    );
                g.data.total -= 1;

                // UI 删除节点及子节点。
                var theRow = $("#row" + rowId);
                if (g.data.isTree) {
                    var rowLevel = theRow.data("rowLevel");
                    if (!jv.IsNull(rowLevel)) {
                        var rowIndex = theRow[0].rowIndex - jt.find(">thead>tr").length;
                        var count = jt.find(">tbody>tr").length - rowIndex;
                        theRow.remove();
                        for (var i = rowIndex; i < count - 1; i++) {
                            var nextRow = jt.find(">tbody>tr:eq(" + rowIndex + ")");
                            if (nextRow.data("rowLevel") <= rowLevel) break;
                            nextRow.remove();
                        }
                    }
                }
                else {
                    theRow.remove();
                }
                tm.rowIndex--;
                //更新 tfooter.
                g.createFooter();

                //                if ((jt.oriCss("height") || "auto") == "auto") {
                //                    jv.boxdy(null, window).trigger("resize");
                //                }
            },
            //只更新单行。
            updateRow: function (rowId, rowData) {
                if (!rowData) return;
                if (!g.data) return;
                $(g.block).remove();

                g.data.rows.Recursion(
                    function (con, i, item) {
                        return item.rows;
                    },
                    function (con, i, item) {
                        if (item.id == rowId) {
                            con[i] = rowData;
                            return false;
                        }
                    }
                    );


                var oldRow = jt.find(">tbody>tr#row" + rowId);
                var oldRowIndex = oldRow.index();
                oldRow.rmove();

                //更新行数信息. 未实现. 
                var row = document.createElement("tr");
                jt.find(">tbody")[0].appendChild(row);
                g.dataToRow(rowData, row);

                //更新 tfooter.
                g.createFooter();
            },
            setGroup: function () {
                var groups = p.role.group.mySplit(",", true);
                if (!groups || !groups.length) return;
                //还原头底线样式。
                jt.find("thead tr:last th").css("borderBottomWidth", "0px");
                //设置组样式。
                var prevColIndex = -1;
                var res = $.grep(g.data.sort.mySplit(",", true), function (n, i) { return $.inArray(n, groups) > -1; });

                $.each(res, function (i, d) {
                    if (!d) return;
                    var model = g.getColModel(d);
                    if (!model) return;

                    var colIndex = model["indexOfCell"];
                    if (prevColIndex < 0) {
                        var prevVIndex = -1;
                        jt.find(">tbody>tr").each(function (i, tr) {
                            if (prevVIndex < 0) {
                                prevVIndex = $(tr).find("td:eq(" + colIndex + ")").prevAll("td:visible:first").index();
                            }
                            var prevTd = $(tr).find("td:eq(" + prevVIndex + ")").css("borderRight", p.role.groupStyle);
                            if ($.browser.msie) {
                                prevTd.css("borderRightWidth", "2px");
                            }
                        });
                    }

                    g.setGroupColumnStyle(colIndex, prevColIndex);
                    prevColIndex = colIndex;
                });
            },
            //colIndex,表示要设置的列索引， prevColIndex表示该组的父组列索引，为空，表示colIndex是第一组。
            //画线只能画 Right 和 Bottom 的边框。因为：BorderRight 本身就是 td 的边框线。 为何用 Bottom 本身也没弄清。可能是 table-border 样式的原因。 udi
            setGroupColumnStyle: function (colIndex, prevColIndex) {
                if (!colIndex) return;

                var prevTd, prevText, firstTd, prevPwbs = "", rowspan = 1;


                jt.find(">tbody>tr").each(function (i, d) {
                    var jtr = $(d),
                    jtd = jtr.find(">td:eq(" + colIndex + ")"),
                    text = jtd.text(),
                    pwbs = "";



                    if (prevColIndex >= 0) {
                        pwbs = jtr.find(">td:eq(" + prevColIndex + ")").attr("wbs");
                    }


                    if (i === 0) {
                        firstTd = jtd;
                        jtd.css("borderTop", p.role.groupStyle)
                            .css("borderRight", p.role.groupStyle).css("borderBottom", p.role.groupStyle)
                            .css("backgroundColor", "white").css("backgroundImage", "none")
                            .css("verticalAlign", "middle");
                    }


                    else if (i > 0) {
                        if (prevText == text && (pwbs == prevPwbs)) {
                            //隐藏
                            jtd.hide();
                            rowspan++;
                        }
                            //组尾行。
                        else {
                            prevTd.css("borderBottom", p.role.groupStyle);


                            if (rowspan > 1) {
                                firstTd.attr("rowspan", rowspan);
                            }

                            firstTd = jtd;

                            firstTd.css("borderRight", p.role.groupStyle).css("borderBottom", p.role.groupStyle)
                                .css("backgroundColor", "white").css("backgroundImage", "none")
                                .css("verticalAlign", "middle");


                            rowspan = 1;
                        }
                    }

                    var pTrIndex = jv.getParentTag(firstTd[0], "TR").rowIndex;
                    if (prevColIndex >= 0) {
                        jtd.attr("wbs", pwbs + "." + pTrIndex);
                    }
                    else {
                        jtd.attr("wbs", pTrIndex);
                    }

                    prevPwbs = pwbs;
                    prevText = jtd.text();
                    prevTd = jtd;

                });

                if (rowspan > 1) {
                    firstTd.attr("rowspan", rowspan);
                }

            },

            //更新客户端缓存数据，更新 p.PageQuote 
            updateCache: function (data) {
                data = data || g.data;

                if (!p.PageQuote || !p.PageQuote.length) return;

                if (!data || !data.rows) return;
                //循环
                $.each(data.rows, function () {
                    var row = this;
                    var quoteRow = $.grep(p.PageQuote, function (n, i) { return n.id == row.id })[0];
                    if (quoteRow) {
                        g.setPageQuoteValue(row, true);
                    }
                });
            },

            loadFromData: function (data) {
                if (!data.entity) data.entity = "";

                g.standardRow = null;
                tm.rowIndex = -1;

                if (p.preProcess)
                    data = p.preProcess(data);

                g.data = null;
                g.updateData(data);


                if (this.pDiv) {
                    $('.pReload', this.pDiv).removeClass('loading');
                }

                $(".inpbox", g.bDiv).removeClass("chked").removeClass("chkpart");
                $(g.block).remove();
                if (!data || data.ErrorMsg || data.msg) {
                    if (this.pDiv) {
                        $('.pPageStat', this.pDiv).html('<div class="flexiErrMsg Wrap">' + jv.highlightLogin(data.msg || data.ErrorMsg || data.msg || p.errormsg) + '</div>');
                    }
                    else {
                        jt.append('<tbody><tr><td colspan="100" class="flexiErrMsg Wrap">' + jv.highlightLogin(data.ErrorMsg || data.msg || p.errormsg) + "</td></tr></tbody>");
                    }
                    tm.removePageQuoteData();
                    this.loading = false;
                    return false;
                }


                jt.attr("entity", data.entity);

                if (p.dataType == 'xml') {
                    p.total = +$('rows total', data).text();
                } else {
                    p.total = data.total;
                    //p.title = data.title || p.title || "&nbsp";
                    //$(this.mDiv).find(".ftitle").html(p.title);
                }

                if ((p.total == 0) ||
                    //最后一页
                    (p.total && !data.rows.length)) {
                    $('tr, a, td, div', t).unbind();
                    jt.find(">tbody").remove();
                    jt.find(">tfoot").remove();

                    p.pages = 1;
                    p.page = 1;
                    this.buildpager();

                    if (this.pDiv) {
                        $('.pPageStat', this.pDiv).html($("<span class='Wrap flexiErrMsg'>" + jv.highlightLogin(data.ErrorMsg || p.nomsg) + "</div>"));
                    }
                    if (p.onSuccess) {

                        //不必捕获错误，因为回传的数据很关键。
                        if (g.data.ExtraJs) {
                            jv.ExecJs(g.data.ExtraJs);
                        }

                        p.onSuccess(this, t);
                    }

                    this.loading = false;
                    return false;
                }


                p.pages = Math.ceil(p.total / p.take);

                if (p.dataType == 'xml')
                    p.page = +$('rows page', data).text();
                else
                    p.page = data.page || p.newp;

                this.buildpager();

                $('tr', t).unbind();
                jt.find(">tbody").remove();
                tm.removePageQuoteData();

                //build new body
                var tbody = document.createElement('tbody');
                jt.append(tbody);

                if (p.dataType == 'json') {
                    //先遍历每一行.
                    $.each(g.data.rows, function () {
                        var rows = tm.newTreeRows(this, null, function () {
                            var row = document.createElement("tr");
                            tbody.appendChild(row);
                            return row;
                        });
                    });

                } else if (p.dataType == 'xml') {
                    alert("请用 Json 格式!");
                }


                g.appendNulHeader();
                g.createFooter();

                if (g.data.isTree && !p.treeOpen) {
                    jt.find(">tbody>tr:visible").each(function () {
                        g.toggleTree($(this).find("div>span.addon"));
                    });
                }


                //this.fixHeight($(this.bDiv).height());

                this.rePosDrag();

                g.setGroup();

                //固定表头。

                if (p.resizeCallback) p.resizeCallback();

                if (p.onSuccess) {

                    //不必捕获错误，因为回传的数据很关键。
                    if (g.data.ExtraJs) {
                        jv.ExecJs(g.data.ExtraJs);
                    }

                    p.onSuccess(this, t);
                }
                tbody = null; data = null; i = null;

                if (p.hideOnSubmit) $(g.block).remove();

                if ($.browser.opera) jt.css('visibility', 'visible');
                g.fixBrower();

                //                if ((jt.oriCss("height") || "auto") == "auto") {
                //                    jv.boxdy(null, window).trigger("resize");
                //                }

                //绑定已选择的数据.
                if (p._Ref_Type_ == "check" || p._Ref_Type_ == "radio" || p.keepQuote) {
                    //只能选择当页.

                    g.setQuoteSelect();

                    //更新全选头。
                    tm.All_Selected = tm.updateHeaderChk();


                    g.updateRefTile();
                }
                this.loading = false;
            }, // end loadFromData
            updateRefTile: function () {
                if (p._Ref_Type_ != "check" && p._Ref_Type_ != "radio") return;
                if (g.refTitled) return;
                window.document.title = "{0}-{1} (引用{2}条)({3})".format(
                    (p._Ref_Type_ == "check" ? "【多选】" : (p._Ref_Type_ == "radio" ? "单选" : "")),
                    window.document.title,
                    p.PageQuote.length,
                    $.map(p.PageQuote, function (n) { return n.name; }).join(",")
                );
                g.refTitled = true;
            },
            appendNulHeader: function () {
                return;
                var nulRow = document.createElement("tr");
                nulRow.className = "nulRow";
                $(jt).find(">tbody").append(nulRow);

                for (var i = 0, len = g.keys.length; i < len; i++) {
                    var col = g.cols[g.keys[i]];
                    if (!col) continue;

                    var td = document.createElement("td");
                    nulRow.appendChild(td);

                    if (!col.headDisplay) td.style.display = "none";
                } //end each
            },
            createFooter: function () {
                //设置tFoot
                if (p.tfoot) {
                    jt.find("tfoot").remove();
                    var tfoot = document.createElement("tfoot");
                    jt.append(tfoot);

                    var ftr = document.createElement("tr");
                    tfoot.appendChild(ftr);

                    for (var i = 0; i < g.keys.length; i++) {
                        var col = g.cols[g.keys[i]];
                        if (!col) continue;

                        var td = document.createElement("td");
                        ftr.appendChild(td);


                        //div = document.createElement("div");

                        if (col.align) {
                            //div.style.textAlign = col.align;
                            td.align = col.align;
                        }

                        if (!col.headDisplay) td.style.display = "none";


                    } //end each


                    tfoot.appendChild(ftr);

                    for (var r in p.tfoot) {
                        var cellIndex = g.getCellIndex(r);
                        var cellVal;
                        if ($.isFunction(p.tfoot[r])) {
                            var cells = [];
                            $(jt.find(">tbody>tr")).each(function (i, d) {
                                cells.push(d.cells[cellIndex]);
                            });
                            cellVal = p.tfoot[r](cells, jt, g);
                        }
                        else {
                            cellVal = p.tfoot[r];
                        }

                        var cel = ftr.cells[cellIndex];
                        if (cel.childNodes.length) {
                            cel = cel.childNodes[0];
                        }

                        cel.innerHTML = cellVal;
                    }
                }
            },
            changeSort: function (th, ctrlKey) { //change sortorder
                if (this.loading) return true;

                var jth = $(th);
                $(g.nDiv).hide();
                $(g.nBtn).hide();
                var col = g.cols[g.keys[th.cellIndex]];

                if (!ctrlKey) {
                    if (p.sortname == col.name) {
                        if (p.sortorder == 'asc') p.sortorder = 'desc';
                        else if (p.sortorder == "desc") {
                            p.sortname = "";
                            p.sortorder = "asc";


                            jth.removeClass('sorted').siblings().removeClass('sorted');
                            $('.sdesc', jt).removeClass('sdesc');
                            $('.sasc', jt).removeClass('sasc');

                            if (p.onChangeSort)
                                p.onChangeSort(p.sortname, p.sortorder);
                            else
                                this.populate();

                            return;
                        }
                        else p.sortorder = 'asc';
                    }
                    else {
                        p.sortname = col.name;
                    }

                    jth.addClass('sorted').siblings().removeClass('sorted');
                    $('.sdesc', jt).removeClass('sdesc');
                    $('.sasc', jt).removeClass('sasc');
                    jth.addClass('s' + p.sortorder);
                }
                else {
                    //目前也是一样. 待处理 . udi.
                    if (p.sortname == col.name) {
                        if (p.sortorder == 'asc') p.sortorder = 'desc';
                        else p.sortorder = 'asc';
                    }


                    jth.addClass('sorted').siblings().removeClass('sorted');
                    $('.sdesc', jt).removeClass('sdesc');
                    $('.sasc', jt).removeClass('sasc');
                    jth.addClass('s' + p.sortorder);
                    p.sortname = col.name;
                }

                if (p.onChangeSort)
                    p.onChangeSort(p.sortname, p.sortorder);
                else
                    this.populate();

            },
            buildpager: function () { //rebuild pager based on new properties
                if (this.pDiv) {
                    $('.pcontrol input', this.pDiv).removeAttr("disabled").val(p.page);
                    $('.pcontrol span', this.pDiv).html(p.pages);

                    if (p.pages == 1) { $('.pcontrol input', this.pDiv).attr("disabled", "true"); }

                    var r1 = (p.page - 1) * p.take + 1,
                r2 = r1 + p.take - 1,
                stat = p.pagestat;

                    if (p.total < r2 || r2 <= 0) r2 = p.total;

                    stat = stat.replace(/{from}/, r1);
                    stat = stat.replace(/{to}/, r2);
                    stat = stat.replace(/{total}/, p.total);

                    $('.pPageStat', this.pDiv).html(stat);
                }

            },
            populate: function (param, successCallback, otherPara, isAjax) { //get latest data

                var oriUrl = p.url;
                p = $.extend(p, param);

                var url = p.url;
                if (isAjax) { p.url = oriUrl };

                if (!url && !p.DataSource) return false;

                if (this.loading) return true;
                this.loading = true;

                if (p.onSubmit) {
                    var gh = p.onSubmit();
                    if (gh === false) return false;
                }

                if (this.pDiv) {
                    $('.pPageStat', this.pDiv).html(p.procmsg);
                    $('.pReload', this.pDiv).addClass('loading');
                }

                $(g.block).css({ marginBottom: 0 - g.bDiv.offsetHeight, height: g.bDiv.offsetHeight });

                if (p.hideOnSubmit) $(this.gDiv).prepend(g.block); //$(t).hide();

                if ($.browser.opera) jt.css('visibility', 'hidden');

                if (!p.newp) p.newp = 1;

                if (p.page > p.pages) p.page = p.pages;

                var query = p.query || {};
                query.FlexiGrid_Page = p.newp;
                query.FlexiGrid_Skip = (parseInt(p.newp || 1) - 1) * p.take;
                query.FlexiGrid_Take = p.take;
                query.FlexiGrid_SortName = (p.sortname == "__AutoID__" ? "" : p.sortname) || "";
                query.FlexiGrid_SortOrder = p.sortorder || "";
                query.FlexiGrid_Cols = g.bindKeys.join(",");
                query.FlexiGrid_Id = p.role.id;
                query.FlexiGrid_RefId = $.map(p.PageQuote || [], function (n) { return n.id; });

                if ($.isEmptyObject(p.params)) {
                    var keys = jv.GetJsonKeys(p.params);
                    for (var pi = 0, paraLen = keys.length; pi < paraLen; pi++)
                        var k = keys[pi];
                    query[k] = p.params[k];
                }

                if (p.DataSource) {
                    g.loadFromData($.isFunction(p.DataSource) ? p.DataSource(g.bindKeys, query, g) : p.DataSource);


                    if (g.pDiv) {
                        $('.pReload', g.pDiv).removeClass('loading');
                        g.loading = false;
                    }


                    p.DataSource = null;
                    return;
                }


                var url = jv.url(url);
                url.attr("_", jv.Random());
                url = url.tohref();

                $.ajax({
                    type: p.method,
                    url: url,
                    data: query,
                    dataType: p.dataType,
                    //                    beforeSend: function (xhr) {
                    //                        jv.CreateEventSource(xhr, jv.getDoer() || g.gDiv);
                    //                    },
                    success: function (data) {
                        successCallback ? successCallback(data, otherPara) : g.loadFromData(data, otherPara);
                    },
                    error: function (data) {
                        if (g.pDiv) {
                            $('.pReload', g.pDiv).removeClass('loading');
                            g.loading = false;
                            var errorMsg = "（格式错误）";
                            try {
                                if (/^(?:\{.*\}|\[.*\])$/.test(data.responseText)) {
                                    var rtm = $.parseJSON(data.responseText) || {};

                                    errorMsg = rtm.msg || rtm.ErrorMsg || rtm.error || "服务器错误，但并没有返回内容";
                                }
                                else {
                                    errorMsg = $(data.responseText).filter("title").text();
                                }
                            }
                            catch (e) { }
                            $('.pPageStat', g.pDiv).html("<label class='Wrap flexiErrMsg'>数据加载错误: </label>" + jv.highlightLogin(errorMsg));
                        }

                        try { if (p.onError) p.onError(data); } catch (e) { }
                    }
                });
            },
            doSearch: function (query) {
                if (query instanceof String) {
                    p.query = { query: query };
                }
                else {
                    p.query = query;
                }

                p.newp = 1;

                //查询时，重新设置引用项。
                p.PageQuote = jQuery.extend(p.PageQuote, p.PageQuote);

                g.populate();
            },
            changePage: function (ctype) { //change page

                if (this.loading) return true;

                switch (ctype) {
                    case 'first': p.newp = 1; break;
                    case 'prev': if (p.page > 1) p.newp = parseInt(p.page) - 1; break;
                    case 'next': if (p.page < p.pages) p.newp = parseInt(p.page) + 1; break;
                    case 'last': p.newp = p.pages; break;
                    case 'input':
                        var nv = parseInt($('.pcontrol input', this.pDiv).val());
                        if (isNaN(nv)) nv = 1;
                        if (nv < 1) nv = 1;
                        else if (nv > p.pages) nv = p.pages;
                        $('.pcontrol input', this.pDiv).val(nv);
                        p.newp = nv;
                        break;
                }

                if (p.newp == p.page) return false;

                if (p.onChangePage)
                    p.onChangePage(p.newp);
                else
                    this.populate();
            },
            //在 rows 中查找某一个rowData 对象。
            findRowData: function (rows, id) {
                var fdRow;
                if (rows === true) return null;
                $.each(rows, function (i, d) {
                    if (d.id == id) { fdRow = d; return false; }

                    if (d.rows) {
                        fdRow = g.findRowData(d.rows, id);
                        if (fdRow) return false;
                    }
                });
                return fdRow;
            },
            //得到当页的所有Json数据。
            getAllJsons: function (IsGetFromHtml) {
                if (!g.data) IsGetFromHtml = true;

                var ret = [];

                if (IsGetFromHtml) {
                    for (var r = 0, rl = jt.find(">tbody>tr").length; r < rl; r++) {
                        var jTr = jt.find(">tbody>tr:eq(" + r + ")");
                        var retVal = {};
                        for (var i = 0, colLen = g.bindKeys.length; i < colLen; i++) {
                            var bindName = g.bindKeys[i];
                            var cellIndex = g.cols[bindName]["indexOfCell"];
                            var val = jTr.children("td:eq(" + cellIndex + ")").text();

                            retVal[bindName] = val;
                        }

                        ret.push(retVal);
                    }
                }
                else {
                    for (var r = 0, rl = g.data.rows.length; r < rl; r++) {
                        var retVal = {};
                        for (var i = 0, colLen = g.bindKeys.length; i < colLen; i++) {
                            var bindName = g.bindKeys[i];
                            var cellIndex = g.cols[bindName]["indexOfBind"];
                            var rowdata = g.data.rows[r];
                            var val = jv.decode(rowdata.cell[cellIndex]);

                            retVal[g.bindKeys[i]] = val;   //可能有特殊字符。如： ·
                        }
                        ret.push(retVal);
                    }
                }

                return ret;
            },
            //通过 tr 里的对象，找出Json数据。
            //idOrjTrOrRowData 可以是Id，可以是tr 里的对象，也可以是 RowData， 如果为 Null，是查找 jv.GetDoer()
            //bindNameCallback 表示只找一个值。参数：bindName ,当前值,列索引，返回false停止查找。
            getRowJson: function (idOrjTrOrRowData, IsGetFromHtml, bindNameCallback) {
                var jTr, id, isRowData, isJson, idType = jv.getType(idOrjTrOrRowData);

                //如果是 dom ,则： idType 是：dom

                if (idOrjTrOrRowData && idOrjTrOrRowData.id && (idType == "object")) {
                    isRowData = !!idOrjTrOrRowData.cell;

                    if (!isRowData) isJson = true;
                }

                if (isRowData || isJson) {
                    id = idOrjTrOrRowData.id;
                    jTr = $("#row" + id);
                }
                else if (idType == "string") {
                    id = idOrjTrOrRowData;
                    jTr = $("#row" + id);
                } else {
                    jTr = g.getjTr(idOrjTrOrRowData);
                    id = jTr.attr("id").slice(3);
                }
                if (!id) return null;

                var retVal = {};

                if (!g.data) IsGetFromHtml = true;

                if (IsGetFromHtml) {

                    for (var i = 0, colLen = g.bindKeys.length; i < colLen; i++) {
                        var bindName = g.bindKeys[i];
                        if (bindName) {
                            var cellIndex = g.cols[bindName]["indexOfCell"];
                            var val = jTr.children("td:eq(" + cellIndex + ")").text();
                            if (bindNameCallback && (bindNameCallback(bindName, val, cellIndex) === false)) {
                                return val;
                            }
                            else {
                                retVal[bindName] = val;
                            }
                        }
                    }
                }
                else {

                    var rowdata = isRowData ? idOrjTrOrRowData : g.findRowData(g.data.rows, id);

                    if (jv.IsNull(rowdata)) {
                        //先从 PageQuote  中查找。如果找不到。再从当前页的数据里查找。 
                        retVal = $.grep(p.PageQuote, function (n, i) {
                            return n.id == id || n[p.role.id] == id;
                        })[0];

                        //如果仅得到 id,name 说明是引用值，则继续查找。
                        if (retVal) {
                            var keys = jv.GetJsonKeys(retVal);
                            if (bindNameCallback) {
                                var val;
                                $.each(keys, function (i, d) {
                                    val = retVal[d];
                                    if (bindNameCallback(i, val, d) == false) {
                                        return false;
                                    }
                                });
                                return val;
                            }

                            if (g.bindKeys.minus(keys).length) {
                                if (bindNameCallback) {
                                    var val;
                                    $.each(keys, function (i, d) {
                                        val = retVal[d];
                                        if (bindNameCallback(i, val, d) == false) {
                                            return false;
                                        }
                                    });
                                    return val;
                                }
                                return retVal;
                            }
                        }
                    }

                    retVal = {};
                    for (var i = 0, colLen = g.bindKeys.length; i < colLen; i++) {
                        var bindName = g.bindKeys[i];
                        var cellIndex = g.cols[bindName]["indexOfBind"];
                        var val = jv.decode(rowdata.cell[cellIndex]);
                        if (bindNameCallback && (bindNameCallback(bindName, val, cellIndex) === false)) {
                            return val;
                        }
                        else {
                            retVal[g.bindKeys[i]] = val;   //可能有特殊字符。如： ·
                        }
                    }
                }

                if (!(p.role.id in retVal)) {
                    retVal[p.role.id] = id;
                }


                if (!("id" in retVal)) {
                    retVal["id"] = id;
                }

                if (!("name" in retVal)) {
                    retVal["name"] = retVal[p.role.name];
                }

                return retVal;
            },
            //从 Json 数据中查找指定名称的值。 id = idOrjTrOrRowData
            getJsonValue: function (idOrjTrOrRowData, bindName) {
                var id, isRowData, isJson, idType = jv.getType(idOrjTrOrRowData);

                //如果是 dom ,则： idType 是：dom

                if (idOrjTrOrRowData && idOrjTrOrRowData.id && (idType == "object")) {
                    isRowData = !!idOrjTrOrRowData.cell;

                    if (!isRowData) isJson = true;
                }

                if (isJson && (bindName in idOrjTrOrRowData)) return idOrjTrOrRowData[bindName];

                if (isRowData) {
                    return idOrjTrOrRowData.cell[g.getColModel(bindName)["indexOfBind"]];
                }


                if (idType == "string") {
                    id = idOrjTrOrRowData;
                } else {
                    id = g.getjTr(idOrjTrOrRowData).attr("id").slice(3);
                }

                if (!id) return null;

                return g.findRowData(g.data.rows, id).cell[g.getColModel(bindName)["indexOfBind"]];
            },
            //得到行的详细信息 id = idOrjTrOrRowData
            getRowDetail: function (id) {
                var name, n_model = g.getColModel(g.p.role.name);
                if (!n_model) {
                    //找第一个可见列。
                    $.each(g.cols, function () {
                        if (!this.headDisplay) return;
                        n_model = this;
                        return false;
                    });
                }

                name = n_model.display;

                var val = g.getJsonValue(id, n_model.name);
                return { name: name, value: val, toString: function () { return this.name + " : " + this.value; } };
            },
            getCellIndex: function (bindName) {
                var model = g.getColModel(bindName);
                if (model) return model.indexOfCell;
                return -1;
                //return jt.find(".hDiv > tr:last th[bindName=" + name + "] ").index();
            },
            getName: function (name) {
                if ($.isFunction(name)) {
                    return name();
                }
                else return name;
            },

            //spanObj 是指 span 标签对象。forceStatus是指定的状态:open,close。 isDeep 是指是否进行递归处理。
            toggleTree: function (spanObj, altKey) {
                spanObj = $(spanObj);
                if (spanObj.length == 0) return;
                var status = spanObj.data("status");
                if (status != "open" && status != "close") return;

                var rows = jv.getParentTag(spanObj[0], "TABLE").rows,
                    celIndex = jv.getParentTag(spanObj[0], "TD").cellIndex,
                    row = jv.getParentTag(spanObj[0], "TR"),
                    rowIndex = row.rowIndex,
                    rowLevel = $(row).data("rowLevel") || 0,
                    pos = rowIndex,
                    posStatus = "";

                //如果是最后一位.
                //if (pos >= rows.length - 1) return;

                for (var i = rowIndex + 1, rowLen = rows.length; i < rowLen; i++) {
                    var jcurRow = $(rows[i]),
                    curRowLevel = jcurRow.data("rowLevel") || 0;

                    if (curRowLevel > rowLevel) {
                        if (status == "open") {
                            jcurRow.hide();

                            if (altKey) {
                                if (jcurRow.data("hasChild")) {
                                    jcurRow.find("span.addon").data("status", "close").html("+");
                                }
                            }
                        }
                        else if (status == "close") {
                            if (altKey) {
                                jcurRow.show();
                                if (jcurRow.data("hasChild")) {
                                    jcurRow.find("span.addon").data("status", "open").html("−");
                                }
                            }
                            else {
                                if (curRowLevel - rowLevel == 1) {
                                    jcurRow.show();
                                    posStatus = jcurRow.find("span.addon").data("status");
                                    continue;
                                }
                                else if (posStatus == "open") {
                                    jcurRow.show();
                                    var subPosStatus = jcurRow.find("span.addon").data("status");
                                    if (subPosStatus) posStatus = subPosStatus;
                                    continue;
                                }
                            }
                        }
                    }
                    else break;
                }


                if (status == "open") {
                    spanObj.data("status", "close").html("+");
                }
                else {
                    spanObj.data("status", "open").html("−");
                }
            },

            //把Ajax数据更新到 g.data , 同时更新缓存数据
            updateData: function (data, id) {
                if (!g.data) {
                    g.data = data;
                    g.updateCache(data);
                    return;
                }


                //查找 相应的 id 对象.
                if (g.data.rows.Recursion) {
                    if (g.data.rows.Recursion(
                    function (con, i, d) { return d.rows; },
                    function (con, i, d) {
                        if (d.id == id) {

                            d.rows = data.rows;

                            return false;
                    }
                    })) {
                        //出错了, 怎么办?
                    }
                }

                g.updateCache(data);
            },


            getCellDim: function (obj) // get cell prop for editable event
            {
                var jobj = $(obj),
                 ht = parseInt(jobj.height()),
                 pht = parseInt(jobj.parent().height()),
                 wt = parseInt(obj.style.width),
                 pwt = parseInt(jobj.parent().width()),
                 top = obj.offsetParent.offsetTop,
                 left = obj.offsetParent.offsetLeft,
                 pdl = parseInt(jobj.css('paddingLeft')),
                 pdt = parseInt(jobj.css('paddingTop'));

                return { ht: ht, wt: wt, top: top, left: left, pdl: pdl, pdt: pdt, pht: pht, pwt: pwt };
            },

            //只调用一次.
            addRowProp: function () {

                $('tbody tr', jt)
                .die("click.ori")
				.live("click.ori",
					function (e) {
					    //点击 thead 也会触发该事件。很奇怪。
					    var ptr = this.parentNode;
					    if (ptr.tagName != "TBODY" && ptr.tagName != "TABLE") {
					        return;
					    }

					    //判断该行是否是 t 的子级
					    if (ptr.parentNode != t) {
					        var ptr = jv.getParentTag(ptr, "TR");
					        if (ptr) {
					            $(ptr).trigger("click");
					        }
					        return false;
					    }

					    var jThis = $(this);

					    if (g.data && (g.data.ErrorMsg || g.data.msg)) return;
					    if (p.onSelect && (p.onSelect(this, g) === false)) {
					        return false;
					    }

					    var isSel = g.isSelected(jThis);

					    if (tm.decideBtnClick()) {
					        g.setSelectRow(this);
					        tm.setUpSelect(this);

					        if (p.onSelected) {
					            p.onSelected(this, g);
					        }

					        return;
					    }

					    if (!p.multiSelect) {
					        g.clearSelectRow();
					        g.setSelectRow(jThis);
					        isSel = true;
					    }
					    else {
					        if (isSel) {
					            g.setUnSelectRow(jThis);
					        }
					        else {
					            g.setSelectRow(jThis);
					        }
					        isSel = !isSel;
					    }

					    tm.setUpSelect(this);
					    tm.setDownSelect(this);


					    var trIndex = jThis.index();
					    if (p.multiSelect && (e.shiftKey || e.altKey) && (tm.LastSelectedRowIndex === 0 || tm.LastSelectedRowIndex)) {
					        var fromIndex = Math.min(tm.LastSelectedRowIndex, trIndex),
                            toIndex = Math.max(tm.LastSelectedRowIndex, trIndex);

					        for (var i = fromIndex; i < toIndex; i++) {
					            g.setSelectRow($(">tbody>tr:eq(" + i + ")", jt));
					        }
					    }

					    //更新Header标志。
					    tm.updateHeaderChk();

					    //后续应用 p.multiSelect
					    if (p.onSelected) {
					        p.onSelected(this, g);
					    }

					    tm.LastSelectedRowIndex = trIndex;
					}
				);

                if (p.dbclick) {
                    $('tbody tr', jt).die("dblclick.ori").live("dblclick.ori", function (ev) {
                        var ptr = this.parentNode;
                        if (ptr.tagName != "TBODY" && ptr.tagName != "TABLE") {
                            return;
                        }

                        //判断该行是否是 t 的子级
                        if (ptr.parentNode != t) {
                            var ptr = jv.getParentTag(ptr, "TR");
                            if (ptr) {
                                $(ptr).trigger("click");
                            }
                            return false;
                        }

                        p.dbclick(this);
                    });
                }
            },
            getSelectIds: function () {
                return $.map(p.PageQuote, function (n) { return n[p.role.id]; })
                //                var dt = [];
                //                $(".trSelected", jt).each(function (i, d) { dt[dt.length] = d.id.slice(3); });
                //                return dt;
            },
            addNewButton: function (btn) {
                if (!btn.separator) {
                    var btnDiv = document.createElement('div'), jbtnDiv = $(btnDiv);
                    btnDiv.className = 'fbutton';
                    var btnName = g.getName(btn.name);
                    btnDiv.innerHTML = "<div><span>" + btnName + "</span></div>";
                    btnDiv.onpress = btn.onpress;


                    //方便前台取Name
                    btnDiv.name = btnName;
                    //添加属性, 给权限控制使用.
                    jbtnDiv.attr("value", btnName).attr("title", p.title || "");

                    if (btn.id) btnDiv.id = btn.id;
                    if (btn.style) $(btnDiv).css(btn.style);
                    if (btn.onpress) {
                        jbtnDiv.bind("click", { btn: btn }, tm.btnPress);
                    }

                    if (btn.disabled) {
                        btnDiv.disabled = true;
                        jbtnDiv.css("color", "gray");
                    }
                    if (btn.bclass) {
                        var sp_btn = $('span', btnDiv)
            				.addClass(btn.bclass)
            				.css({ "paddingLeft": 20 })
                        ;

                        //                        if (sp_btn.hasClass("submit")) {
                        //                            jbtnDiv.OneClick();
                        //                        }
                    }

                    return btnDiv;
                }
            },
            createHeader: function () {
                //create model if any
                //                var ht = document.createElement("table");
                //                g.hDiv.appendChild(ht);

                var thead = document.createElement('thead');
                thead.className = "hDiv";
                t.appendChild(thead);
                var jthead = $(thead);

                var tr = document.createElement("tr");
                thead.appendChild(tr);
                tr.className = "row";

                for (i = 0, colModelLen = g.keys.length; i < colModelLen; i++) {
                    //处理colModel 定义时，多一个 “,” 的情况
                    var cm = g.getColModel(g.keys[i]);

                    var th = document.createElement('th');
                    tr.appendChild(th);
                    var cmName = g.getName(cm.name);
                    var div = document.createElement("div");
                    if (cm.align) {
                        th.setAttribute("align", cm.align);
                        div.style.textAlign = cm.align;
                    }

                    //                    if (cm.process) {
                    //                        th.process = cm.process;
                    //                    }


                    //                    if (cm.width > 0) {
                    //                        jth.width(cm.width);
                    //                    }
                    //                    else 

                    // 宽度。
                    if (!cm.headDisplay) {
                        th.style.display = "none";
                    }
                    else {
                        if (cm.width > 0) {
                            div.style.width = cm.width + "px";
                        }

                        if (cm.maxWidth) {
                            div.style.maxWidth = cm.maxWidth + "px";
                        }

                        if (cm.minWidth) {
                            div.style.minWidth = cm.minWidth + "px";
                        }
                    }


                    //事件
                    if (cm.sortable) {
                        $(th).click(function (e) {
                            g.changeSort(this, e.ctrlKey);
                        })
						.hover(tm.headerOver, tm.headerOut); //wrap content


                        if (cm.sortable && cmName == p.sortname) {
                            div.className = 'sorted';
                            th.className = 's' + p.sortorder;
                        }
                    }

                    //                    if (p.onlyHeadtd) {
                    //                        $(th).html(cm.display);
                    //                    }
                    //                    else {

                    th.appendChild(div);
                    div.innerHTML = cm.display;
                    //                    th.innerHTML = "<div>" + cm.display + "</div>";
                    //                    }

                    //                    if (cm.maxWidth) {
                    //                        var setWidthControl = jth.find(">div").andSelf();
                    //                        setWidthControl.css("maxWidth", jv.GetInt(cm.maxWidth) + "px");

                    //                        if (cm.width == "auto" && $.browser.msie && (parseInt($.browser.version) < 8)) {
                    //                            setWidthControl.width(jv.GetInt(cm.maxWidth) + "px");
                    //                        }
                    //                    }

                    //th.setAttribute("bindName", cmName); // .attr("bindName", cmName);

                    //                    jth.attr("toggle", cm.toggle || false /*默认值是 false*/);
                }


                if (p.moreCol && thead) {
                    $.each(p.moreCol(), function () { jthead.prepend($(this).addClass("moreCol").addClass("row")); });
                    p.showToggleBtn = false;
                }

                //setup thead			



                var nulRow = document.createElement("tr");
                nulRow.className = "nulRow";
                thead.appendChild(nulRow);

                for (var i = 0, len = g.keys.length; i < len; i++) {
                    var col = g.cols[g.keys[i]];
                    if (!col) continue;

                    var td = document.createElement("th");
                    nulRow.appendChild(td);

                    td.appendChild(document.createElement("hr"));

                    var inDiv = document.createElement("div");

                    td.appendChild(inDiv);

                    if (!col.headDisplay) td.style.display = "none";
                    else {
                        if (col.width > 0) {
                            td.style.width = col.width + "px";
                        }

                        if (col.maxWidth) {
                            td.style.maxWidth = col.maxWidth + "px";
                        }

                        if (col.minWidth) {
                            td.style.minWidth = col.minWidth + "px";
                        }
                    }
                } //end each



                $(nulRow).find("th div")
                .each(function () {
                    var pos = {};
                    $(this).bindResize({
                        down: function (el, e) {
                            var $self = $(el);
                            var header_height = $(g.bDiv).find(".hDiv").height();

                            $self.addClass("moving")
                                .css("left", "0px")
                                .css("right", "auto")
                                .css("top", (0 - header_height) + "px")
                                .css("height", header_height + "px");

                            pos.x = e.clientX;
                            pos.width = $self.width();

                        }, move: function (el, e) {
                            var w = pos.width + e.clientX - pos.x;
                            $(el).width(w).html(w);
                        }, up: function (el, e) {
                            var $self = $(el);
                            var width = $self.width();
                            var $hr = $self.prev(), $th = $hr.parent();
                            $hr.width(width);
                            $th.width(width);
                            $self.removeClass("moving");
                            el.style.width = "";
                            el.style.right = "";
                            $self.html("");

                            //重调高度
                            //由于表格宽度 100% 时，会自动设置单元格宽度。
                            //                        var widthIng = $th.width();
                            //                        $hr.width(widthIng);
                            //                        $th.width(widthIng);
                            var header_height = $(g.bDiv).find(".hDiv").height();
                            $self.parents(".nulRow:first").find("th div")
                                    .css("left", "")
                                    .css("height", header_height + "px")
                                    .css("top", (0 - header_height) + "px");
                        }
                    });
                });

                g.updateColWidth();

                if (!p.showHeader) {
                    jthead.hide();
                }
            },
            updateColWidth: function () {
                return;
                var nulRow = $(g.bDiv).find("tbody tr.nulRow")[0];
                $(g.bDiv).find("thead.hDiv tr.nulRow th").each(function (i) {
                    var rowItem = jv.getNode(nulRow, i);
                    rowItem.style.display = this.style.display;

                    var $self = $(this),
                    width = $self.width();


                    rowItem.style.width = width + "px";
                });

            },
            createContainer: function () {
                g.gDiv = document.createElement('div'); //create global container

                //set gDiv
                //                g.gDiv.className = 'flexigrid';

                var jgDiv = $(g.gDiv).addClass("flexigrid");
                if (p.FillHeight) {
                    jgDiv.addClass("FillHeight-");
                }

                //add conditional classes
                if ($.browser.msie)
                    jgDiv.addClass('ie');

                jgDiv.attr("ofvh", jt.attr("ofvh"));


                if (p.novstripe)
                    jgDiv.addClass('novstripe');


                jgDiv.width(p.width || "");



                //                jgDiv.hooked("width",function () {
                //                    top.document.title += "!";
                //                });

                jt.before(g.gDiv);
                jgDiv.append(t);
            },
            //支持多个Tr对象的查找。
            getjTr: function (jTr) {
                jTr = $(jTr);
                if (!jTr.length) return $(jv.getParentTag(jv.GetDoer(), "TR"));

                var tr = jTr[0];
                if (tr.tagName == "TR") return jTr;
                return $(jv.getParentTag(tr, "TR")); // jTr.closest("tr"); //.filter(function (i, d) { return $(d).is("tr"); }).add(jTr.filter(function (i, d) { return !$(d).is("tr"); }).parents("tr:first"));
            },
            //判断该行是否已选中。不支持多行。多行取第一个。尽量其它代码地方不出现类 .trSelected
            isSelected: function (jTr) {
                return g.getjTr(jTr).hasClass("trSelected");
            },
            //设置行的选中状态，并设置引用数据，可设置多个行。
            setSelectRow: function (jTr, sel) {
                if (sel === false) return g.setUnSelectRow(jTr);

                g.getjTr(jTr).each(function (i, r) {
                    var jr = $(r);
                    if (g.isSelected(jr)) return;
                    jr.addClass("trSelected");
                    tm.setPageQuoteFromRow(jr);
                });
            },
            //设置行非选中状态，并设置引用数据，可设置多个行。
            setUnSelectRow: function (jTr) {
                g.getjTr(jTr).each(function (i, r) {
                    var jr = $(r);
                    if (!g.isSelected(jr)) return;
                    jr.removeClass("trSelected");
                    tm.setPageQuoteFromRow(jr);
                });
            },
            clearSelectRow: function (grid) {
                $((grid || g).bDiv).find(".trSelected").removeClass("trSelected");
                p.PageQuote = [];
            },
            createDivs: function () {        //init divs
                //                g.mDiv = document.createElement('div'); //create title container
                //                g.hDiv = document.createElement('div'); //create header container
                //                g.hDiv.className = "hDiv hDivTop";
                //                g.hDiv.style.position = "absolute";
                //                jt.before(g.hDiv);

                g.bDiv = document.createElement('div'); //create body container
                jt.before(g.bDiv);

                if (p.resizable && !p.FillHeight) {
                    g.vDiv = document.createElement('div'); //create grip
                    $(g.bDiv).after(g.vDiv);
                }

                g.rDiv = document.createElement('div'); //create horizontal resizer
                g.cDrag = document.createElement('div'); //create column drag
                g.block = document.createElement('div'); //creat blocker

                g.nBtn = document.createElement('div'); //create column show/hide button
                //g.tDiv = document.createElement('div'); //create toolbar
                //g.mDiv = g.tDiv;

                if (p.usepager) {
                    g.pDiv = document.createElement('div'); //create pager container
                    $(g.bDiv).after(g.pDiv);
                }




                g.cDrag.className = 'cDrag';

                var jbDiv = $(g.bDiv);
                jbDiv.prepend(g.cDrag);

                if (p.FillHeight) jbDiv.addClass("FillHeight-");
                //set bDiv
                //                g.bDiv.className = 'bDiv';
                jbDiv
                    .addClass("bDiv")
		            .css({ "height": (p.height == 'auto') ? 'auto' : p.height + "px" })
                //.scroll(function (e) { g.scroll() })
		            .append(t)
                ;


                //根据 p.worpTd 添加整体样式
                //                if (p.wrapTd) { jbDiv.addClass("wrapTd"); }
                //                else { jbDiv.addClass("nowrapTd"); }


                if (p.height == 'auto') {
                    $('table:not(.fixedTableHeader)', g.bDiv).addClass('autoht');
                }


                //                for (var i = 0, len = g.keys.length; i < len; i++) {
                //                    var col = g.cols[g.keys[i]];
                //                    if (!col) continue;

                //                    var inDiv = document.createElement("div");
                //                    g.cDrag.appendChild(inDiv);
                //                } //end each


            },
            createButtons: function () {
                if (p._Ref_Type_) {
                    //先从 p.toolBar  里找，找不到再自行创建 tDiv

                    if (p._Ref_Type_ == "radio") {

                        p.buttons.push({ name: "确 定", bclass: "return", title: "单选确定,返回单选选择的对象", onpress: tm.btnRadioOkPress });


                        p.buttons.push({
                            name: "清 除", bclass: "minus", title: "清除所选数据", onpress: function (ids, grid, ev) {
                                g.clearSelectRow(grid);
                            }
                        });

                        p.buttons.push({
                            name: "返回空", bclass: "return_nul", title: "返回空值,双击返回数据!", onpress: function (ids, grid, ev) {

                                var emp = [{}];
                                emp.IsEmpty = true;
                                jv.Hide(function () { p.RefCallback(p.role, emp, g, { originalEvent: true, target: jv.GetDoer() }); });
                            }
                        });

                    }
                    else if (p._Ref_Type_ == "check") {

                        p.buttons.push({
                            name: "确 定", bclass: "return", title: "确定返回当前面的改变数据,仅改变当前页!", onpress: tm.btnCheckOkPress
                        });

                        p.buttons.push({
                            name: "清 除", bclass: "minus", title: "清除所选数据", onpress: function (ids, grid, ev) {
                                g.clearSelectRow();
                            }
                        });

                        p.buttons.push({
                            name: "返回空", bclass: "return_nul", title: "返回空值!", onpress: function (ids, grid, ev) {
                                g.clearSelectRow();

                                var emp = [];
                                emp.IsEmpty = true;
                                jv.Hide(function () { p.RefCallback(p.role, emp, g, { originalEvent: true, target: jv.GetDoer() }); });

                            }
                        });

                    }
                }


                //set toolbar
                if (p.buttons && p.buttons.length) {
                    if (p.container) {

                        //创建button
                        $.each(p.buttons, function () {
                            p.newButton(this, g);
                        });
                    }
                    else {
                        g.tDiv = document.createElement('div');
                        g.tDiv.className = 'tDiv';

                        if (p.title) {
                            $(g.tDiv).append("<div class='ftitle'>" + p.title + "</div>");
                        }


                        var btnLen = p.buttons.length || 0, i = btnLen;


                        while (true) {

                            i--;
                            if (i < 0) break;


                            var btn = p.buttons[i];
                            if (!btn) continue;
                            if (!btn.separator) {
                                var btnDiv = g.addNewButton(btn),
                                jbtnDiv = $(btnDiv);
                                $(g.tDiv).append(btnDiv);


                                //                            if ($.browser.msie && $.browser.version < 7.0) {
                                //                                jbtnDiv.hover(function () { $(this).addClass('fbOver'); }, function () { $(this).removeClass('fbOver'); });
                                //                            }

                                if (btn.buttons) {
                                    jbtnDiv.addClass("subBtn");
                                    jbtnDiv.bind("click", { btns: btn.buttons }, tm.subMenuPress);
                                }

                            } else {
                                $(g.tDiv).append($("<div class='btnseparator'></div>").css(btn.style || {}));
                            }
                        }

                        $(g.gDiv)
                            .prepend(g.tDiv)
                            .css("borderTopWidth", "0px");
                    }
                }

            },
            createDragDiv: function () {
                return;
                var cdheight = $(g.bDiv).height();
                $('thead tr:last th', jt).each
			        (
			 	        function (i, d) {
			 	            var cgDiv = document.createElement('div');
			 	            if ($(d).is(":visible")) {
			 	                $(cgDiv).css({ left: $(d).offset().left + $(d).width() });
			 	            }
			 	            else {
			 	                $(cgDiv).hide();
			 	            }
			 	            $(g.cDrag).append(cgDiv);
			 	            if (!p.cgwidth) p.cgwidth = $(cgDiv).width();
			 	            $(cgDiv).css({ "height": cdheight })
						        .mousedown(function (e) { g.dragStart('colresize', e, this); })
			 	            ;
			 	            //			 	            if ($.browser.msie && $.browser.version < 7.0) {
			 	            //			 	                g.fixHeight($(g.gDiv).height());
			 	            //			 	                $(cgDiv).hover(
			 	            //								        function () {
			 	            //								            g.fixHeight();
			 	            //								            $(this).addClass('dragging')
			 	            //								        },
			 	            //								        function () { if (!g.colresize) $(this).removeClass('dragging') }
			 	            //							        );
			 	            //			 	            }
			 	        }
			        );
                g.cdropleft = document.createElement('span');
                g.cdropleft.className = 'cdropleft';
                g.cdropright = document.createElement('span');
                g.cdropright.className = 'cdropright';
            },
            createResizeDiv: function () {
                if (!p.resizable) return;

                if (g.vDiv) {
                    g.vDiv.className = 'vGrip';
                    $(g.vDiv)
                        .mousedown(function (e) { g.dragStart('vresize', e) })
                        .html('<span></span>');
                }

                if (!p.nohresize) {
                    g.rDiv.className = 'hGrip';
                    $(g.rDiv)
		                .mousedown(function (e) { g.dragStart('vresize', e, true); })
		                .html('<span></span>')
		                .css('height', $(g.gDiv).height())
                    ;
                    //                    if ($.browser.msie && $.browser.version < 7.0) {
                    //                        $(g.rDiv).hover(function () { $(this).addClass('hgOver'); }, function () { $(this).removeClass('hgOver'); });
                    //                    }
                    $(g.gDiv).append(g.rDiv);
                }
            },
            createPager: function () {
                // add pager
                if (p.usepager) {

                    g.pDiv.className = 'pDiv';
                    g.pDiv.innerHTML = '<div class="pDiv2"></div>';
                    var html = "";
                    if (p.take > 0) {
                        var html = ' <div class="pGroup"> <div class="pFirst pButton"><span></span></div><div class="pPrev pButton"><span></span></div> </div> <div class="btnseparator"></div> <div class="pGroup"><span class="pcontrol">第<input type="text" size="3" value="1" />页，共<span>1</span>页</span></div> <div class="btnseparator"></div> <div class="pGroup"> <div class="pNext pButton"><span></span></div><div class="pLast pButton"><span></span></div> </div> ';
                    }
                    html += '<div class="btnseparator"></div> <div class="pGroup"> <div class="pReload pButton"><span></span></div> </div> <div class="btnseparator"></div> <div class="pGroup"><span class="pPageStat"></span></div>';
                    $('div', g.pDiv).html(html);

                    $('.pReload', g.pDiv).click(function () { g.populate() });
                    $('.pFirst', g.pDiv).click(function () { g.changePage('first') });
                    $('.pPrev', g.pDiv).click(function () { g.changePage('prev') });
                    $('.pNext', g.pDiv).click(function () { g.changePage('next') });
                    $('.pLast', g.pDiv).click(function () { g.changePage('last') });
                    $('.pcontrol input', g.pDiv).keydown(function (e) {
                        if (e.keyCode == 13) {
                            g.changePage('input');
                            e.preventDefault();
                            e.stopPropagation();
                            return false;
                        }
                    }).keypress(function (e) {
                        if (e.keyCode == 13) {
                            e.preventDefault();
                            e.stopPropagation();
                            return false;
                        }
                    }).keyup(function (e) {
                        if (e.keyCode == 13) {
                            e.preventDefault();
                            e.stopPropagation();
                            return false;
                        }
                    });

                    if (p.take > 0) {
                        var sel_random = jv.Random();
                        if (p.useTextHelper) {
                            $('.pDiv2', g.pDiv).prepend("<div class='pGroup'><input type='text' id='" + sel_random + "' name='take' readonly='readonly' style='width:27px' value='" + p.take + "'/>条</div> <div class='btnseparator'></div>");

                            if (!p.rpFixed) {
                                $('#' + sel_random).TextHelper({
                                    data: p.rpOptions, change: function (newValue) {
                                        p.take = parseInt(newValue);
                                        g.changePage("first");
                                        g.populate();
                                    }
                                });
                            }
                        }
                        else {
                            $('.pDiv2', g.pDiv).prepend("<div class='pGroup'><select id='" + sel_random + "' name='take'></select></div>");
                            $.each(p.rpOptions, function (i, n) {
                                $("#" + sel_random)[0].options.add(new Option(n, n));
                            });

                            $("#" + sel_random)
                                .val(p.take)
                                .bind("change", function () {
                                    var newValue = this.value;
                                    p.take = parseInt(newValue);
                                    g.changePage("first");
                                    g.populate();
                                });
                        }
                        $('.pDiv2', g.pDiv).prepend("<div class='pGroup' style='margin-left:10px;'>每页</div>");
                    }
                }
            },


            createBlockDiv: function () {
                //add block
                g.block.className = 'gBlock';
            },
            createDivEvents: function () {
                //                $(g.bDiv)
                //                	.hover(function (e) {
                //                	    if (jv.getParentTag(jv.GetDoer(), "THEAD")) {
                //                	        $(g.nDiv).hide(); $(g.nBtn).hide();
                //                	    }
                //                	})
                //                	;
                //                $(g.gDiv)
                //		            .hover(function () { }, function () { $(g.nDiv).hide(); $(g.nBtn).hide(); })
                //		            ;

                //add document events
                //                $(document)
                //		            .mousemove(function (e) { g.dragMove(e) })
                //		            .mouseup(function (e) { g.dragEnd() })
                //		            .hover(function () { }, function () { g.dragEnd() })
                //		            ;
            },
            fixBrower: function () {
                $(g.bDiv).each(function () { jv.fixIe7(this) });

                return;
                //browser adjustments
                if ($.browser.msie && $.browser.version < 7.0) {
                    $('.bDiv,.mDiv,.pDiv,.vGrip,.tDiv, .sDiv', g.gDiv)
			            .css({ "width": '100%' });
                    $(g.gDiv).addClass('ie6');
                    if (p.width != 'auto') $(g.gDiv).addClass('ie6fullwidthbug');
                }

                if ($.browser.msie && $.browser.version < 7.0)
                    $('tr', g.nDiv).hover
				        (
				 	        function () { $(this).addClass('ndcolover'); },
					        function () { $(this).removeClass('ndcolover'); }
				        );
            },

            //行的反选.
            ToggleSelect: function () {
                if (p._Ref_Type_ == "radio") return;

                jt.find("tbody tr").each(function (i, d) {
                    var jd = $(d);
                    if (!jd.data("hasChild")) {
                        jd.trigger("click");
                    }
                });
            }

            //---------------------------------------------------------------------
        };
        //end g

        //设置别名
        g.TableRowToData = g.getJson = g.getRowJson;
        g.newRow = g.addRow;

        var addModel = function (modelName, model) {

            if (modelName == "after") {
                p.colModel = p.colModel.insertAt(p.colModel.length, model);
            }
            else if (jv.getType(modelName) == "number") {
                if (modelName >= 0) {
                    p.colModel = p.colModel.insertAt(modelName, model);
                }
            }
            else if (modelName == "before") {
                p.colModel = p.colModel.insertAt(0, model);
            }
            else {
                model = $.extend(model, modelName);
                if (modelName.colIndex == "after") {
                    p.colModel = p.colModel.insertAt(p.colModel.length, model);
                }
                else if (jv.getType(modelName.colIndex) == "number") {
                    p.colModel = p.colModel.insertAt(modelName.colIndex, model);
                }
                else if (modelName.colIndex == "before") {
                    p.colModel = p.colModel.insertAt(0, model);
                }
            }
        };

        if (p.useId) {
            var selModel = { display: p.IdName, toggle: false, bind: false, name: "__AutoID__", width: 36, sortable: false, css: "pin", align: "left", format: function (row, rowIndex) { return rowIndex + 1; } };

            if (!$.grep(p.colModel, function (n, i) { return n && n.name == "__AutoID__"; }).length) {
                addModel(p.useId, selModel);
            }
        }

        //多种格式设置 
        if (p.useSelect) {
            var selModel = {
                display: (p.multiSelect ? p.checkSelectName : p.radioSelectName), toggle: false, bind: false, name: "__Select__", css: "tdSelect pin " + (p.multiSelect ? "check" : "radio"), sortable: false, align: "center", width: (p.multiSelect ? 45 : 35),
                format: function (rowData, rowIndex, grid, td, event) {
                    return "<div>&nbsp</div>";
                }
            };

            if (!$.grep(p.colModel, function (n, i) { return n && n.name == "__Select__"; }).length) {
                addModel(p.useSelect, selModel);
            }
        }





        //不要调用它，使用 cols 数据。
        //        g.cols = {};
        //        g.keys = [];
        //        g.bindKeys = [];

        g.configKeys = function () {
            g.cols = {};
            g.keys = [];
            g.bindKeys = [];

            var bindIndex = -1;
            $.each(p.colModel, function (_i, cm) {
                //处理colModel 定义时，多一个 “,” 的情况
                if (!cm) return;
                var theBindIndex;
                if (cm.bind !== false) {
                    ++bindIndex;
                    theBindIndex = bindIndex;
                }
                if (!cm.align) { cm.align = ""; }

                //设置的宽度值:  auto,不设置,null,undefined, 30,30px 0,0px
                //标准化之后的结果: auto（默认即不设置）, int,*(填充宽度,最后统一设置)
                if (cm.hide) cm.width = 0;
                else if (cm.width === "auto") {
                    cm.headDisplay = true;
                }
                else if (cm.width === "*") {
                    cm.headDisplay = true;
                }
                else if (jv.IsNull(cm.width)) {
                    cm.width = "auto";
                    cm.headDisplay = true;
                }
                else {
                    cm.width = parseInt(cm.width);

                    if (cm.width > 0) {
                        cm.headDisplay = true;
                    }
                    else {
                        cm.headDisplay = false;
                    }
                }


                //标准化
                if (cm.maxwidth) cm.maxWidth = cm.maxwidth;
                if (cm.html) cm.format = cm.html;

                if (!cm.name) return;
                var name = g.getName(cm.name);
                var model = jv.JsonValuer(cm, function (p, val) { if ((p == "format") || (p == "html") || (p == "onpress")) return false; return g; }, false);

                if ((model.bind !== false) && model.name) {
                    g.bindKeys.push(name);
                }


                //在Html UI 中的索引
                model["indexOfCell"] = _i;

                //在Json Data中的索引
                model["indexOfBind"] = theBindIndex;
                g.keys.push(name);
                g.cols[name] = model;
            });
        };

        g.getColModel = function (bindName) {
            if (bindName in g.cols) return g.cols[bindName];
            if (bindName == p.role.id) return {};
        };

        g.configKeys();

        //        p._Ref_Value_ = {};


        //开始
        jt
        .css("position", "relative")
		.attr({ cellpadding: 5, cellspacing: 0, border: 0 })  //remove padding and spacing
		.show() //show if hidden
        ;




        g.createContainer();



        g.createDivs();

        g.createHeader();

        g.createButtons();
        g.createDragDiv();
        g.createResizeDiv();
        g.createPager();

        g.createBlockDiv();

        //g.createDivEvents();

        //g.fixBrower();

        g.addRowProp();


        if (p.height == "auto") {
            var newH = 0;
            if (p.FillHeight) {
                //jv.MyOnLoad();
                jv.fixHeight(g.bDiv);
                //            if (pone[0].tagName == "BODY") {
                //                newH = $(document).height();
                //                newH = newH - parseInt(pjd.css("marginTop")) - parseInt(pjd.css("marginBottom"));
                //            }
                //            else {
                //                if (pone.hasClass("FillHeight")) { jv.MyOnLoad(); }
                //                newH = pone.height() - 5; // 5 并不奇怪,是底边的高度.
                //            }
                //            newH = newH - (one.offset().top - pone.offset().top);


                //if ((p.buttons && p.buttons.length) || p.title) {
                //    newH -= 31;
                //}

                //if (p.usepager) {
                //    newH -= 31;
                //}


                //if (p.resizable) {
                //    newH -= 6;
                //}


                //if (newH < p.minheight) newH = p.minheight;
                //p.height = newH;
            }
        }

        //if (p.FillHeight) {
        //    g.fixHeight();
        //}

        //        if (p._Ref_Type_ && p.RefCallback) {
        //            var fhDiv = $(g.gDiv).closest(".FillHeighted");
        //            if (fhDiv.length == 1) {
        //                fhDiv.css("height", "100%");
        //            }
        //        }

        //make grid functions accessible
        g.p = p;
        //        t.p = p;
        t.grid = g;

        // load data
        if (p.url.length > 0 && p.autoload) {
            g.populate();
        }

        //绑定反选事件
        if (p.multiSelect && p.useSelect) {
            var cellIndex = g.getCellIndex("__Select__");
            if (cellIndex >= 0) {
                $($(g.bDiv).find("table thead tr.row:last")[0].cells[cellIndex]).click(function (e) {

                    if (p.selectType == "all") {
                        if (tm.All_Selected) {
                            tm.All_Selected = false;
                        }
                        else {
                            tm.All_Selected = true;
                        }
                    }

                    var trigerRowClick = function (row, set) {
                        if (p.onSelect && (p.onSelect(row, g, true) === false)) {
                            return;
                        }

                        if (set) g.setSelectRow(row);
                        else g.setUnSelectRow(row);


                        if (p.onSelected) {
                            p.onSelected(row, g, true);
                        }
                    }

                    $(g.bDiv).find("table:not(.fixedTableHeader) tbody tr").each(function () {
                        if (p.selectType == "all") {
                            trigerRowClick(this, tm.All_Selected);
                        }
                        else {
                            trigerRowClick(this, !$(this).hasClass("trSelected"));
                        }
                    });
                    tm.updateHeaderChk();
                });
            }
        }



        return t;

    };

    //    var docloaded = false;

    //    $(function () {
    //        docloaded = true
    //    });

    //在当前行对象上调用。
    $.fn.getFlexiRowID = function () {
        return jv.getParentTag(this[0], "TR").id.slice(3); //$(this).closest('tr').attr("id").slice(3); // $(this).parents(".flexigrid:first");
    }

    //当前须是 
    $.fn.getFlexi = function () {
        var down = $(this).filter(".flexigrid").add($(this).find(".flexigrid:first")).find(".bDiv>table:not(.fixedTableHeader)");
        if (down.length > 0) return down[0].grid;
        else {
            var up = $(this).closest(".flexigrid").find(".bDiv>table:not(.fixedTableHeader)");
            if (up.length > 0) {
                return up[0].grid;
            }
            else return { p: {}, populate: function () { } };
        }
    }

    $.fn.flexigrid = function (p) {
        this.grid = this.last().grid;

        return this.each(function () {
            //            if (!docloaded) {
            //                $(this).hide();
            //                var t = this;
            //                $(document).ready
            //					(
            //						function () {
            //						    $.addFlex(t, p);
            //						}
            //					);
            //            } else {
            $.addFlex(this, p);
            //            }
        });

    }; //end flexigrid

    $.fn.noSelect = function (p) { //no select plugin by me :-)

        if (p == null)
            prevent = true;
        else
            prevent = p;

        if (prevent) {

            return this.each(function () {
                if ($.browser.msie || $.browser.safari) $(this).bind('selectstart', function () { return false; });
                else if ($.browser.mozilla) {
                    $(this).css('MozUserSelect', 'none');
                    $('body').trigger('focus');
                }
                else if ($.browser.opera) $(this).bind('mousedown', function () { return false; });
                else $(this).attr('unselectable', 'on');
            });

        } else {


            return this.each(function () {
                if ($.browser.msie || $.browser.safari) $(this).unbind('selectstart');
                else if ($.browser.mozilla) $(this).css('MozUserSelect', 'inherit');
                else if ($.browser.opera) $(this).unbind('mousedown');
                else $(this).removeAttr('unselectable', 'on');
            });

        }

    }; //end noSelect

})(jQuery);
/**
* TableDnD plug-in for JQuery, allows you to drag and drop table rows
* You can set up various options to control how the system will work
* Copyright (c) Denis Howlett <denish@isocra.com>
* Licensed like jQuery, see http://docs.jquery.com/License.
*
* Configuration options:
* 
* onDragStyle
*     This is the style that is assigned to the row during drag. There are limitations to the styles that can be
*     associated with a row (such as you can't assign a border--well you can, but it won't be
*     displayed). (So instead consider using onDragClass.) The CSS style to apply is specified as
*     a map (as used in the jQuery css(...) function).
* onDropStyle
*     This is the style that is assigned to the row when it is dropped. As for onDragStyle, there are limitations
*     to what you can do. Also this replaces the original style, so again consider using onDragClass which
*     is simply added and then removed on drop.
* onDragClass
*     This class is added for the duration of the drag and then removed when the row is dropped. It is more
*     flexible than using onDragStyle since it can be inherited by the row cells and other content. The default
*     is class is tDnD_whileDrag. So to use the default, simply customise this CSS class in your
*     stylesheet.
* onDrop
*     Pass a function that will be called when the row is dropped. The function takes 2 parameters: the table
*     and the row that was dropped. You can work out the new order of the rows by using
*     table.rows.
* onDragStart
*     Pass a function that will be called when the user starts dragging. The function takes 2 parameters: the
*     table and the row which the user has started to drag.
* onAllowDrop
*     Pass a function that will be called as a row is over another row. If the function returns true, allow 
*     dropping on that row, otherwise not. The function takes 2 parameters: the dragged row and the row under
*     the cursor. It returns a boolean: true allows the drop, false doesn't allow it.
* scrollAmount
*     This is the number of pixels to scroll if the user moves the mouse cursor to the top or bottom of the
*     window. The page should automatically scroll up or down as appropriate (tested in IE6, IE7, Safari, FF2,
*     FF3 beta
* dragHandle
*     This is the name of a class that you assign to one or more cells in each row that is draggable. If you
*     specify this class, then you are responsible for setting cursor: move in the CSS and only these cells
*     will have the drag behaviour. If you do not specify a dragHandle, then you get the old behaviour where
*     the whole row is draggable.
* 
* Other ways to control behaviour:
*
* Add class="nodrop" to any rows for which you don't want to allow dropping, and class="nodrag" to any rows
* that you don't want to be draggable.
*
* Inside the onDrop method you can also call $.tableDnD.serialize() this returns a string of the form
* <tableID>[]=<rowID1>&<tableID>[]=<rowID2> so that you can send this back to the server. The table must have
* an ID as must all the rows.
*
* Other methods:
*
* $("...").tableDnDUpdate() 
* Will update all the matching tables, that is it will reapply the mousedown method to the rows (or handle cells).
* This is useful if you have updated the table rows using Ajax and you want to make the table draggable again.
* The table maintains the original configuration (so you don't have to specify it again).
*
* $("...").tableDnDSerialize()
* Will serialize and return the serialized string as above, but for each of the matching tables--so it can be
* called from anywhere and isn't dependent on the currentTable being set up correctly before calling
*
* Known problems:
* - Auto-scoll has some problems with IE7  (it scrolls even when it shouldn't), work-around: set scrollAmount to 0
* 
* Version 0.2: 2008-02-20 First public version
* Version 0.3: 2008-02-07 Added onDragStart option
*                         Made the scroll amount configurable (default is 5 as before)
* Version 0.4: 2008-03-15 Changed the noDrag/noDrop attributes to nodrag/nodrop classes
*                         Added onAllowDrop to control dropping
*                         Fixed a bug which meant that you couldn't set the scroll amount in both directions
*                         Added serialize method
* Version 0.5: 2008-05-16 Changed so that if you specify a dragHandle class it doesn't make the whole row
*                         draggable
*                         Improved the serialize method to use a default (and settable) regular expression.
*                         Added tableDnDupate() and tableDnDSerialize() to be called when you are outside the table
* Version 0.6: 2011-12-02 Added support for touch devices
*/
// Determine if this is a touch device
var hasTouch = 'ontouchstart' in document.documentElement,
	startEvent = hasTouch ? 'touchstart' : 'mousedown',
	moveEvent = hasTouch ? 'touchmove' : 'mousemove',
	endEvent = hasTouch ? 'touchend' : 'mouseup';

jQuery.tableDnD = {
    /** Keep hold of the current table being dragged */
    currentTable: null,
    /** Keep hold of the current drag object if any */
    dragObject: null,
    /** The current mouse offset */
    mouseOffset: null,
    /** Remember the old value of Y so that we don't do too much processing */
    oldY: 0,


    /** Actually build the structure */
    build: function (options) {
        // Set up the defaults if any

        this.each(function () {
            // This is bound to each matching table, set up the defaults and override with user options
            this.tableDnDConfig = jQuery.extend({
                onDragStyle: null,
                onDropStyle: null,
                // Add in the default class for whileDragging
                onDragClass: "tDnD_whileDrag",
                onDrop: null,
                onDragStart: null,
                scrollAmount: 5,

                serializeRegexp: /[^\-]*$/, // The regular expression to use to trim row IDs
                serializeParamName: null, // If you want to specify another parameter name instead of the table ID
                dragHandle: null // If you give the name of a class here, then only Cells with this class will be draggable
            }, options || {});
            // Now make the rows draggable
            jQuery.tableDnD.makeDraggable(this);
        });

        // Don't break the chain
        return this;
    },

    /** This function makes all the rows on the table draggable apart from those marked as "NoDrag" */
    makeDraggable: function (table) {

        var config = table.tableDnDConfig;
        if (config.dragHandle) {
            // We only need to add the event to the specified cells
            var cells = jQuery("td." + table.tableDnDConfig.dragHandle, table);
            cells.each(function () {
                // The cell is bound to "this"
                jQuery(this).bind(startEvent, function (ev) {
                    jQuery.tableDnD.initialiseDrag(this.parentNode, table, this, ev, config);
                    return false;
                });
            })
        } else {
            // For backwards compatibility, we add the event to the whole row
            var rows = jQuery("tr", table); // get all the rows as a wrapped set
            rows.each(function () {
                // Iterate through each row, the row is bound to "this"
                var row = jQuery(this);
                if (!row.hasClass("nodrag")) {
                    row.bind(startEvent, function (ev) {
                        if (ev.target.tagName == "TD") {
                            jQuery.tableDnD.initialiseDrag(this, table, this, ev, config);
                            return false;
                        }
                    }).css("cursor", "move"); // Store the tableDnD object
                }
            });
        }
    },

    initialiseDrag: function (dragObject, table, target, evnt, config) {
        jQuery.tableDnD.dragObject = dragObject;
        jQuery.tableDnD.currentTable = table;
        jQuery.tableDnD.mouseOffset = jQuery.tableDnD.getMouseOffset(target, evnt);
        jQuery.tableDnD.originalOrder = jQuery.tableDnD.serialize();
        // Now we need to capture the mouse up and mouse move event
        // We can use bind so that we don't interfere with other event handlers
        jQuery(document)
            .bind(moveEvent, jQuery.tableDnD.mousemove)
            .bind(endEvent, jQuery.tableDnD.mouseup);
        if (config.onDragStart) {
            // Call the onDragStart method if there is one
            config.onDragStart(table, target);
        }
    },

    updateTables: function () {
        this.each(function () {
            // this is now bound to each matching table
            if (this.tableDnDConfig) {
                jQuery.tableDnD.makeDraggable(this);
            }
        })
    },

    /** Get the mouse coordinates from the event (allowing for browser differences) */
    mouseCoords: function (ev) {
        if (ev.pageX || ev.pageY) {
            return { x: ev.pageX, y: ev.pageY };
        }
        return {
            x: ev.clientX + document.body.scrollLeft - document.body.clientLeft,
            y: ev.clientY + document.body.scrollTop - document.body.clientTop
        };
    },

    /** Given a target element and a mouse event, get the mouse offset from that element.
    To do this we need the element's position and the mouse position */
    getMouseOffset: function (target, ev) {
        ev = ev || window.event;

        var docPos = this.getPosition(target);
        var mousePos = this.mouseCoords(ev);
        return { x: mousePos.x - docPos.x, y: mousePos.y - docPos.y };
    },

    /** Get the position of an element by going up the DOM tree and adding up all the offsets */
    getPosition: function (e) {
        var left = 0;
        var top = 0;
        /** Safari fix -- thanks to Luis Chato for this! */
        if (e.offsetHeight == 0) {
            /** Safari 2 doesn't correctly grab the offsetTop of a table row
            this is detailed here:
            http://jacob.peargrove.com/blog/2006/technical/table-row-offsettop-bug-in-safari/
            the solution is likewise noted there, grab the offset of a table cell in the row - the firstChild.
            note that firefox will return a text node as a first child, so designing a more thorough
            solution may need to take that into account, for now this seems to work in firefox, safari, ie */
            e = e.firstChild; // a table cell
        }

        while (e.offsetParent) {
            left += e.offsetLeft;
            top += e.offsetTop;
            e = e.offsetParent;
        }

        left += e.offsetLeft;
        top += e.offsetTop;

        return { x: left, y: top };
    },

    mousemove: function (ev) {
        if (jQuery.tableDnD.dragObject == null) {
            return;
        }
        if (ev.type == 'touchmove') {
            // prevent touch device screen scrolling
            event.preventDefault();
        }

        var dragObj = jQuery(jQuery.tableDnD.dragObject);
        var config = jQuery.tableDnD.currentTable.tableDnDConfig;
        var mousePos = jQuery.tableDnD.mouseCoords(ev);
        var y = mousePos.y - jQuery.tableDnD.mouseOffset.y;
        //auto scroll the window
        var yOffset = window.pageYOffset;
        if (document.all) {
            // Windows version
            //yOffset=document.body.scrollTop;
            if (typeof document.compatMode != 'undefined' &&
	             document.compatMode != 'BackCompat') {
                yOffset = document.documentElement.scrollTop;
            }
            else if (typeof document.body != 'undefined') {
                yOffset = document.body.scrollTop;
            }

        }

        if (mousePos.y - yOffset < config.scrollAmount) {
            window.scrollBy(0, -config.scrollAmount);
        } else {
            var windowHeight = window.innerHeight ? window.innerHeight
                    : document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body.clientHeight;
            if (windowHeight - (mousePos.y - yOffset) < config.scrollAmount) {
                window.scrollBy(0, config.scrollAmount);
            }
        }


        if (y != jQuery.tableDnD.oldY) {
            // work out if we're going up or down...
            var movingDown = y > jQuery.tableDnD.oldY;
            // update the old value
            jQuery.tableDnD.oldY = y;
            // update the style to show we're dragging
            if (config.onDragClass) {
                dragObj.addClass(config.onDragClass);
            } else {
                dragObj.css(config.onDragStyle);
            }
            // If we're over a row then move the dragged row to there so that the user sees the
            // effect dynamically
            var currentRow = jQuery.tableDnD.findDropTargetRow(dragObj, y);
            if (currentRow) {
                // TODO worry about what happens when there are multiple TBODIES
                if (movingDown && jQuery.tableDnD.dragObject != currentRow) {
                    jQuery.tableDnD.dragObject.parentNode.insertBefore(jQuery.tableDnD.dragObject, currentRow.nextSibling);
                } else if (!movingDown && jQuery.tableDnD.dragObject != currentRow) {
                    jQuery.tableDnD.dragObject.parentNode.insertBefore(jQuery.tableDnD.dragObject, currentRow);
                }
            }
        }

        return false;
    },

    /** We're only worried about the y position really, because we can only move rows up and down */
    findDropTargetRow: function (draggedRow, y) {
        var rows = jQuery.tableDnD.currentTable.rows;
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            var rowY = this.getPosition(row).y;
            var rowHeight = parseInt(row.offsetHeight) / 2;
            if (row.offsetHeight == 0) {
                rowY = this.getPosition(row.firstChild).y;
                rowHeight = parseInt(row.firstChild.offsetHeight) / 2;
            }
            // Because we always have to insert before, we need to offset the height a bit
            if ((y > rowY - rowHeight) && (y < (rowY + rowHeight))) {
                // that's the row we're over
                // If it's the same as the current row, ignore it
                if (row == draggedRow) { return null; }
                var config = jQuery.tableDnD.currentTable.tableDnDConfig;
                if (config.onAllowDrop) {
                    if (config.onAllowDrop(draggedRow, row)) {
                        return row;
                    } else {
                        return null;
                    }
                } else {
                    // If a row has nodrop class, then don't allow dropping (inspired by John Tarr and Famic)
                    var nodrop = jQuery(row).hasClass("nodrop");
                    if (!nodrop) {
                        return row;
                    } else {
                        return null;
                    }
                }
                return row;
            }
        }
        return null;
    },

    mouseup: function (e) {
        if (jQuery.tableDnD.currentTable && jQuery.tableDnD.dragObject) {
            // Unbind the event handlers
            jQuery(document)
	            .unbind(moveEvent, jQuery.tableDnD.mousemove)
	            .unbind(endEvent, jQuery.tableDnD.mouseup);
            var droppedRow = jQuery.tableDnD.dragObject;
            var config = jQuery.tableDnD.currentTable.tableDnDConfig;
            // If we have a dragObject, then we need to release it,
            // The row will already have been moved to the right place so we just reset stuff
            if (config.onDragClass) {
                jQuery(droppedRow).removeClass(config.onDragClass);
            } else {
                jQuery(droppedRow).css(config.onDropStyle);
            }
            jQuery.tableDnD.dragObject = null;
            var newOrder = jQuery.tableDnD.serialize();
            if (config.onDrop && (jQuery.tableDnD.originalOrder != newOrder)) {
                // Call the onDrop method if there is one
                config.onDrop(jQuery.tableDnD.currentTable, droppedRow);
            }
            jQuery.tableDnD.currentTable = null; // let go of the table too
        }
    },

    serialize: function () {
        if (jQuery.tableDnD.currentTable) {
            return jQuery.tableDnD.serializeTable(jQuery.tableDnD.currentTable);
        } else {
            return "Error: No Table id set, you need to set an id on your table and every row";
        }
    },

    serializeTable: function (table) {
        var result = "";
        var tableId = table.id;
        var rows = table.rows;
        for (var i = 0; i < rows.length; i++) {
            if (result.length > 0) result += "&";
            var rowId = rows[i].id;
            if (rowId && rowId && table.tableDnDConfig && table.tableDnDConfig.serializeRegexp) {
                rowId = rowId.match(table.tableDnDConfig.serializeRegexp)[0];
            }

            result += tableId + '[]=' + rowId;
        }
        return result;
    },

    serializeTables: function () {
        var result = "";
        this.each(function () {
            // this is now bound to each matching table
            result += jQuery.tableDnD.serializeTable(this);
        });
        return result;
    }

};


jQuery.fn.extend(
	{
	    tableDnD: jQuery.tableDnD.build,
	    tableDnDUpdate: jQuery.tableDnD.updateTables,
	    tableDnDSerialize: jQuery.tableDnD.serializeTables
	}
);
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

/*
* jQuery UI 1.7.3
*
* Copyright (c) 2009 AUTHORS.txt (http://jqueryui.com/about)
* Dual licensed under the MIT (MIT-LICENSE.txt)
* and GPL (GPL-LICENSE.txt) licenses.
*
* http://docs.jquery.com/UI
*/
; jQuery.ui || (function ($) {

    var _remove = $.fn.remove,
	isFF2 = $.browser.mozilla && (parseFloat($.browser.version) < 1.9);

    //Helper functions and ui object
    $.ui = {
        version: "1.7.3",

        // $.ui.plugin is deprecated.  Use the proxy pattern instead.
        plugin: {
            add: function (module, option, set) {
                var proto = $.ui[module].prototype;
                for (var i in set) {
                    proto.plugins[i] = proto.plugins[i] || [];
                    proto.plugins[i].push([option, set[i]]);
                }
            },
            call: function (instance, name, args) {
                var set = instance.plugins[name];
                if (!set || !instance.element[0].parentNode) { return; }

                for (var i = 0; i < set.length; i++) {
                    if (instance.options[set[i][0]]) {
                        set[i][1].apply(instance.element, args);
                    }
                }
            }
        },

        contains: function (a, b) {
            return document.compareDocumentPosition
			? a.compareDocumentPosition(b) & 16
			: a !== b && a.contains(b);
        },

        hasScroll: function (el, a) {

            //If overflow is hidden, the element might have extra content, but the user wants to hide it
            if ($(el).css('overflow') == 'hidden') { return false; }

            var scroll = (a && a == 'left') ? 'scrollLeft' : 'scrollTop',
			has = false;

            if (el[scroll] > 0) { return true; }

            // TODO: determine which cases actually cause this to happen
            // if the element doesn't have the scroll set, see if it's possible to
            // set the scroll
            el[scroll] = 1;
            has = (el[scroll] > 0);
            el[scroll] = 0;
            return has;
        },

        isOverAxis: function (x, reference, size) {
            //Determines when x coordinate is over "b" element axis
            return (x > reference) && (x < (reference + size));
        },

        isOver: function (y, x, top, left, height, width) {
            //Determines when x, y coordinates is over "b" element
            return $.ui.isOverAxis(y, top, height) && $.ui.isOverAxis(x, left, width);
        },

        keyCode: {
            BACKSPACE: 8,
            CAPS_LOCK: 20,
            COMMA: 188,
            CONTROL: 17,
            DELETE: 46,
            DOWN: 40,
            END: 35,
            ENTER: 13,
            ESCAPE: 27,
            HOME: 36,
            INSERT: 45,
            LEFT: 37,
            NUMPAD_ADD: 107,
            NUMPAD_DECIMAL: 110,
            NUMPAD_DIVIDE: 111,
            NUMPAD_ENTER: 108,
            NUMPAD_MULTIPLY: 106,
            NUMPAD_SUBTRACT: 109,
            PAGE_DOWN: 34,
            PAGE_UP: 33,
            PERIOD: 190,
            RIGHT: 39,
            SHIFT: 16,
            SPACE: 32,
            TAB: 9,
            UP: 38
        }
    };

    // WAI-ARIA normalization
    if (isFF2) {
        var attr = $.attr,
		removeAttr = $.fn.removeAttr,
		ariaNS = "http://www.w3.org/2005/07/aaa",
		ariaState = /^aria-/,
		ariaRole = /^wairole:/;

        $.attr = function (elem, name, value) {
            var set = value !== undefined;

            return (name == 'role'
			? (set
				? attr.call(this, elem, name, "wairole:" + value)
				: (attr.apply(this, arguments) || "").replace(ariaRole, ""))
			: (ariaState.test(name)
				? (set
					? elem.setAttributeNS(ariaNS,
						name.replace(ariaState, "aaa:"), value)
					: attr.call(this, elem, name.replace(ariaState, "aaa:")))
				: attr.apply(this, arguments)));
        };

        $.fn.removeAttr = function (name) {
            return (ariaState.test(name)
			? this.each(function () {
			    this.removeAttributeNS(ariaNS, name.replace(ariaState, ""));
			}) : removeAttr.call(this, name));
        };
    }

    //jQuery plugins
    //$.fn.extend({
    //	remove: function(selector, keepData) {
    //		return this.each(function() {
    //			if ( !keepData ) {
    //				if ( !selector || $.filter( selector, [ this ] ).length ) {
    //					$( "*", this ).add( this ).each(function() {
    //						$( this ).triggerHandler( "remove" );
    //					});
    //				}
    //			}
    //			return _remove.call( $(this), selector, keepData );
    //		});
    //	},

    //	enableSelection: function() {
    //		return this
    //			.attr('unselectable', 'off')
    //			.css('MozUserSelect', '')
    //			.unbind('selectstart.ui');
    //	},

    //	disableSelection: function() {
    //		return this
    //			.attr('unselectable', 'on')
    //			.css('MozUserSelect', 'none')
    //			.bind('selectstart.ui', function() { return false; });
    //	},

    //	scrollParent: function() {
    //		var scrollParent;
    //		if(($.browser.msie && (/(static|relative)/).test(this.css('position'))) || (/absolute/).test(this.css('position'))) {
    //			scrollParent = this.parents().filter(function() {
    //				return (/(relative|absolute|fixed)/).test($.curCSS(this,'position',1)) && (/(auto|scroll)/).test($.curCSS(this,'overflow',1)+$.curCSS(this,'overflow-y',1)+$.curCSS(this,'overflow-x',1));
    //			}).eq(0);
    //		} else {
    //			scrollParent = this.parents().filter(function() {
    //				return (/(auto|scroll)/).test($.curCSS(this,'overflow',1)+$.curCSS(this,'overflow-y',1)+$.curCSS(this,'overflow-x',1));
    //			}).eq(0);
    //		}

    //		return (/fixed/).test(this.css('position')) || !scrollParent.length ? $(document) : scrollParent;
    //	}
    //});


    //Additional selectors
    $.extend($.expr[':'], {
        data: function (elem, i, match) {
            return !!$.data(elem, match[3]);
        },

        focusable: function (element) {
            var nodeName = element.nodeName.toLowerCase(),
			tabIndex = $.attr(element, 'tabindex');
            return (/input|select|textarea|button|object/.test(nodeName)
			? !element.disabled
			: 'a' == nodeName || 'area' == nodeName
				? element.href || !isNaN(tabIndex)
				: !isNaN(tabIndex))
            // the element and all of its ancestors must be visible
            // the browser may report that the area is hidden
			&& !$(element)['area' == nodeName ? 'parents' : 'closest'](':hidden').length;
        },

        tabbable: function (element) {
            var tabIndex = $.attr(element, 'tabindex');
            return (isNaN(tabIndex) || tabIndex >= 0) && $(element).is(':focusable');
        }
    });


    // $.widget is a factory to create jQuery plugins
    // taking some boilerplate code out of the plugin code
    function getter(namespace, plugin, method, args) {
        function getMethods(type) {
            var methods = $[namespace][plugin][type] || [];
            return (typeof methods == 'string' ? methods.split(/,?\s+/) : methods);
        }

        var methods = getMethods('getter');
        if (args.length == 1 && typeof args[0] == 'string') {
            methods = methods.concat(getMethods('getterSetter'));
        }
        return ($.inArray(method, methods) != -1);
    }

    $.widget = function (name, prototype) {
        var namespace = name.split(".")[0];
        name = name.split(".")[1];

        // create plugin method
        $.fn[name] = function (options) {
            var isMethodCall = (typeof options == 'string'),
			args = Array.prototype.slice.call(arguments, 1);

            // prevent calls to internal methods
            if (isMethodCall && options.substring(0, 1) == '_') {
                return this;
            }

            // handle getter methods
            if (isMethodCall && getter(namespace, name, options, args)) {
                var instance = $.data(this[0], name);
                return (instance ? instance[options].apply(instance, args)
				: undefined);
            }

            // handle initialization and non-getter methods
            return this.each(function () {
                var instance = $.data(this, name);

                // constructor
                (!instance && !isMethodCall &&
				$.data(this, name, new $[namespace][name](this, options))._init());

                // method call
                (instance && isMethodCall && $.isFunction(instance[options]) &&
				instance[options].apply(instance, args));
            });
        };

        // create widget constructor
        $[namespace] = $[namespace] || {};
        $[namespace][name] = function (element, options) {
            var self = this;

            this.namespace = namespace;
            this.widgetName = name;
            this.widgetEventPrefix = $[namespace][name].eventPrefix || name;
            this.widgetBaseClass = namespace + '-' + name;

            this.options = $.extend({},
			$.widget.defaults,
			$[namespace][name].defaults,
			$.metadata && $.metadata.get(element)[name],
			options);

            this.element = $(element)
			.bind('setData.' + name, function (event, key, value) {
			    if (event.target == element) {
			        return self._setData(key, value);
			    }
			})
			.bind('getData.' + name, function (event, key) {
			    if (event.target == element) {
			        return self._getData(key);
			    }
			})
			.bind('remove', function () {
			    return self.destroy();
			});
        };

        // add widget prototype
        $[namespace][name].prototype = $.extend({}, $.widget.prototype, prototype);

        // TODO: merge getter and getterSetter properties from widget prototype
        // and plugin prototype
        $[namespace][name].getterSetter = 'option';
    };

    $.widget.prototype = {
        _init: function () { },
        destroy: function () {
            this.element.removeData(this.widgetName)
			.removeClass(this.widgetBaseClass + '-disabled' + ' ' + this.namespace + '-state-disabled')
			.removeAttr('aria-disabled');
        },

        option: function (key, value) {
            var options = key,
			self = this;

            if (typeof key == "string") {
                if (value === undefined) {
                    return this._getData(key);
                }
                options = {};
                options[key] = value;
            }

            $.each(options, function (key, value) {
                self._setData(key, value);
            });
        },
        _getData: function (key) {
            return this.options[key];
        },
        _setData: function (key, value) {
            this.options[key] = value;

            if (key == 'disabled') {
                this.element
				[value ? 'addClass' : 'removeClass'](
					this.widgetBaseClass + '-disabled' + ' ' +
					this.namespace + '-state-disabled')
				.attr("aria-disabled", value);
            }
        },

        enable: function () {
            this._setData('disabled', false);
        },
        disable: function () {
            this._setData('disabled', true);
        },

        _trigger: function (type, event, data) {
            var callback = this.options[type],
			eventName = (type == this.widgetEventPrefix
				? type : this.widgetEventPrefix + type);

            event = $.Event(event);
            event.type = eventName;

            // copy original event properties over to the new event
            // this would happen if we could call $.event.fix instead of $.Event
            // but we don't have a way to force an event to be fixed multiple times
            if (event.originalEvent) {
                for (var i = $.event.props.length, prop; i; ) {
                    prop = $.event.props[--i];
                    event[prop] = event.originalEvent[prop];
                }
            }

            this.element.trigger(event, data);

            return !($.isFunction(callback) && callback.call(this.element[0], event, data) === false
			|| event.isDefaultPrevented());
        }
    };

    $.widget.defaults = {
        disabled: false
    };


    /** Mouse Interaction Plugin **/

    $.ui.mouse = {
        _mouseInit: function () {
            var self = this;

            this.element
			.bind('mousedown.' + this.widgetName, function (event) {
			    return self._mouseDown(event);
			})
			.bind('click.' + this.widgetName, function (event) {
			    if (self._preventClickEvent) {
			        self._preventClickEvent = false;
			        event.stopImmediatePropagation();
			        return false;
			    }
			});

            // Prevent text selection in IE
            if ($.browser.msie) {
                this._mouseUnselectable = this.element.attr('unselectable');
                this.element.attr('unselectable', 'on');
            }

            this.started = false;
        },

        // TODO: make sure destroying one instance of mouse doesn't mess with
        // other instances of mouse
        _mouseDestroy: function () {
            this.element.unbind('.' + this.widgetName);

            // Restore text selection in IE
            ($.browser.msie
			&& this.element.attr('unselectable', this._mouseUnselectable));
        },

        _mouseDown: function (event) {
            // don't let more than one widget handle mouseStart
            // TODO: figure out why we have to use originalEvent
            event.originalEvent = event.originalEvent || {};
            if (event.originalEvent.mouseHandled) { return; }

            // we may have missed mouseup (out of window)
            (this._mouseStarted && this._mouseUp(event));

            this._mouseDownEvent = event;

            var self = this,
			btnIsLeft = (event.which == 1),
			elIsCancel = (typeof this.options.cancel == "string" ? $(event.target).parents().add(event.target).filter(this.options.cancel).length : false);
            if (!btnIsLeft || elIsCancel || !this._mouseCapture(event)) {
                return true;
            }

            this.mouseDelayMet = !this.options.delay;
            if (!this.mouseDelayMet) {
                this._mouseDelayTimer = setTimeout(function () {
                    self.mouseDelayMet = true;
                }, this.options.delay);
            }

            if (this._mouseDistanceMet(event) && this._mouseDelayMet(event)) {
                this._mouseStarted = (this._mouseStart(event) !== false);
                if (!this._mouseStarted) {
                    event.preventDefault();
                    return true;
                }
            }

            // these delegates are required to keep context
            this._mouseMoveDelegate = function (event) {
                return self._mouseMove(event);
            };
            this._mouseUpDelegate = function (event) {
                return self._mouseUp(event);
            };
            $(document)
			.bind('mousemove.' + this.widgetName, this._mouseMoveDelegate)
			.bind('mouseup.' + this.widgetName, this._mouseUpDelegate);

            // preventDefault() is used to prevent the selection of text here -
            // however, in Safari, this causes select boxes not to be selectable
            // anymore, so this fix is needed
            ($.browser.safari || event.preventDefault());

            event.originalEvent.mouseHandled = true;
            return true;
        },

        _mouseMove: function (event) {
            // IE mouseup check - mouseup happened when mouse was out of window
            if ($.browser.msie && !event.button) {
                return this._mouseUp(event);
            }

            if (this._mouseStarted) {
                this._mouseDrag(event);
                return event.preventDefault();
            }

            if (this._mouseDistanceMet(event) && this._mouseDelayMet(event)) {
                this._mouseStarted =
				(this._mouseStart(this._mouseDownEvent, event) !== false);
                (this._mouseStarted ? this._mouseDrag(event) : this._mouseUp(event));
            }

            return !this._mouseStarted;
        },

        _mouseUp: function (event) {
            $(document)
			.unbind('mousemove.' + this.widgetName, this._mouseMoveDelegate)
			.unbind('mouseup.' + this.widgetName, this._mouseUpDelegate);

            if (this._mouseStarted) {
                this._mouseStarted = false;
                this._preventClickEvent = (event.target == this._mouseDownEvent.target);
                this._mouseStop(event);
            }

            return false;
        },

        _mouseDistanceMet: function (event) {
            return (Math.max(
				Math.abs(this._mouseDownEvent.pageX - event.pageX),
				Math.abs(this._mouseDownEvent.pageY - event.pageY)
			) >= this.options.distance
		);
        },

        _mouseDelayMet: function (event) {
            return this.mouseDelayMet;
        },

        // These are placeholder methods, to be overriden by extending plugin
        _mouseStart: function (event) { },
        _mouseDrag: function (event) { },
        _mouseStop: function (event) { },
        _mouseCapture: function (event) { return true; }
    };

    $.ui.mouse.defaults = {
        cancel: null,
        distance: 1,
        delay: 0
    };

})(jQuery);
/*
* jQuery UI Datepicker 1.7.3
*
* Copyright (c) 2009 AUTHORS.txt (http://jqueryui.com/about)
* Dual licensed under the MIT (MIT-LICENSE.txt)
* and GPL (GPL-LICENSE.txt) licenses.
*
* http://docs.jquery.com/UI/Datepicker
*
* Depends:
*	ui.core.js
*/

(function ($) { // hide the namespace

    $.extend($.ui, { datepicker: { version: "1.7.3"} });

    var PROP_NAME = 'datepicker';

    /* Date picker manager.
    Use the singleton instance of this class, $.datepicker, to interact with the date picker.
    Settings for (groups of) date pickers are maintained in an instance object,
    allowing multiple different settings on the same page. */

    function Datepicker() {
        this.debug = false; // Change this to true to start debugging
        this._curInst = null; // The current instance in use
        this._keyEvent = false; // If the last event was a key event
        this._disabledInputs = []; // List of date picker inputs that have been disabled
        this._datepickerShowing = false; // True if the popup picker is showing , false if not
        this._inDialog = false; // True if showing within a "dialog", false if not
        this._mainDivId = 'ui-datepicker-div'; // The ID of the main datepicker division
        this._inlineClass = 'ui-datepicker-inline'; // The name of the inline marker class
        this._appendClass = 'ui-datepicker-append'; // The name of the append marker class
        this._triggerClass = 'ui-datepicker-trigger'; // The name of the trigger marker class
        this._dialogClass = 'ui-datepicker-dialog'; // The name of the dialog marker class
        this._disableClass = 'ui-datepicker-disabled'; // The name of the disabled covering marker class
        this._unselectableClass = 'ui-datepicker-unselectable'; // The name of the unselectable cell marker class
        this._currentClass = 'ui-datepicker-current-day'; // The name of the current day marker class
        this._dayOverClass = 'ui-datepicker-days-cell-over'; // The name of the day hover marker class
        this.regional = []; // Available regional settings, indexed by language code
        this.regional[''] = { // Default regional settings
            clearText: 'Clear', //udi add.
            closeText: 'Done', // Display text for close link
            prevText: 'Prev', // Display text for previous month link
            nextText: 'Next', // Display text for next month link
            currentText: 'Today', // Display text for current month link
            monthNames: ['January', 'February', 'March', 'April', 'May', 'June',
			'July', 'August', 'September', 'October', 'November', 'December'], // Names of months for drop-down and formatting
            monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'], // For formatting
            dayNames: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'], // For formatting
            dayNamesShort: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'], // For formatting
            dayNamesMin: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'], // Column headings for days starting at Sunday
            dateFormat: 'mm/dd/yy', // See format options on parseDate
            firstDay: 0, // The first day of the week, Sun = 0, Mon = 1, ...
            isRTL: false // True if right-to-left language, false if left-to-right
        };
        this._defaults = { // Global defaults for all the date picker instances
            showOn: 'focus', // 'focus' for popup on focus,
            // 'button' for trigger button, or 'both' for either
            showAnim: 'show', // Name of jQuery animation for popup
            showOptions: {}, // Options for enhanced animations
            defaultDate: null, // Used when field is blank: actual date,
            // +/-number for offset from today, null for today
            appendText: '', // Display text following the input box, e.g. showing the format
            buttonText: '...', // Text for trigger button
            buttonImage: '', // URL for trigger button image
            buttonImageOnly: false, // True if the image appears alone, false if it appears on a button
            hideIfNoPrevNext: false, // True to hide next/previous month links
            // if not applicable, false to just disable them
            navigationAsDateFormat: false, // True if date formatting applied to prev/today/next links
            gotoCurrent: false, // True if today link goes back to current selection instead
            changeMonth: false, // True if month can be selected directly, false if only prev/next
            changeYear: false, // True if year can be selected directly, false if only prev/next
            showMonthAfterYear: false, // True if the year select precedes month, false for month then year
            yearRange: '-10:+10', // Range of years to display in drop-down,
            // either relative to current year (-nn:+nn) or absolute (nnnn:nnnn)
            showOtherMonths: false, // True to show dates in other months, false to leave blank
            calculateWeek: this.iso8601Week, // How to calculate the week of the year,
            // takes a Date and returns the number of the week for it
            shortYearCutoff: '+10', // Short year values < this are in the current century,
            // > this are in the previous century,
            // string value starting with '+' for current year + value
            minDate: null, // The earliest selectable date, or null for no limit
            maxDate: null, // The latest selectable date, or null for no limit
            duration: 'normal', // Duration of display/closure
            beforeShowDay: null, // Function that takes a date and returns an array with
            // [0] = true if selectable, false if not, [1] = custom CSS class name(s) or '',
            // [2] = cell title (optional), e.g. $.datepicker.noWeekends
            beforeShow: null, // Function that takes an input field and
            // returns a set of custom settings for the date picker
            onSelect: null, // Define a callback function when a date is selected
            onChangeMonthYear: null, // Define a callback function when the month or year is changed
            onClose: null, // Define a callback function when the datepicker is closed
            numberOfMonths: 1, // Number of months to show at a time
            showCurrentAtPos: 0, // The position in multipe months at which to show the current month (starting at 0)
            stepMonths: 1, // Number of months to step back/forward
            stepBigMonths: 12, // Number of months to step back/forward for the big links
            altField: '', // Selector for an alternate field to store selected dates into
            altFormat: '', // The date format to use for the alternate field
            constrainInput: true, // The input is constrained by the current date format

            showClearButton: true, //udi add 
            showButtonPanel: false // True to show button panel, false to not show it
        };
        $.extend(this._defaults, this.regional['']);
        this.dpDiv = $('<div id="' + this._mainDivId + '" class="ui-datepicker ui-widget ui-widget-content ui-helper-clearfix ui-corner-all ui-helper-hidden-accessible"></div>');
    }

    $.extend(Datepicker.prototype, {
        /* Class name added to elements to indicate already configured with a date picker. */
        markerClassName: 'hasDatepicker',

        /* Debug logging (if enabled). */
        log: function () {
            if (this.debug)
                console.log.apply('', arguments);
        },

        /* Override the default settings for all instances of the date picker.
        @param  settings  object - the new settings to use as defaults (anonymous object)
        @return the manager object */
        setDefaults: function (settings) {
            extendRemove(this._defaults, settings || {});
            return this;
        },

        /* Attach the date picker to a jQuery selection.
        @param  target    element - the target input field or division or span
        @param  settings  object - the new settings to use for this date picker instance (anonymous) */
        _attachDatepicker: function (target, settings) {
            // check for settings on the control itself - in namespace 'date:'
            var inlineSettings = null;
            for (var attrName in this._defaults) {
                var attrValue = target.getAttribute('date:' + attrName);
                if (attrValue) {
                    inlineSettings = inlineSettings || {};
                    try {
                        inlineSettings[attrName] = eval(attrValue);
                    } catch (err) {
                        inlineSettings[attrName] = attrValue;
                    }
                }
            }
            var nodeName = target.nodeName.toLowerCase();
            var inline = (nodeName == 'div' || nodeName == 'span');
            if (!target.id)
                target.id = 'dp' + (++this.uuid);
            var inst = this._newInst($(target), inline);
            inst.settings = $.extend({}, settings || {}, inlineSettings || {});
            if (nodeName == 'input') {
                this._connectDatepicker(target, inst);
            } else if (inline) {
                this._inlineDatepicker(target, inst);
            }
        },

        /* Create a new instance object. */
        _newInst: function (target, inline) {
            var id = target[0].id.replace(/([:\[\]\.])/g, '\\\\$1'); // escape jQuery meta chars
            return { id: id, input: target, // associated target
                selectedDay: 0, selectedMonth: 0, selectedYear: 0, // current selection
                drawMonth: 0, drawYear: 0, // month being drawn
                inline: inline, // is datepicker inline or not
                dpDiv: (!inline ? this.dpDiv : // presentation div
			$('<div class="' + this._inlineClass + ' ui-datepicker ui-widget ui-widget-content ui-helper-clearfix ui-corner-all"></div>'))
            };
        },

        /* Attach the date picker to an input field. */
        _connectDatepicker: function (target, inst) {
            var input = $(target);
            inst.append = $([]);
            inst.trigger = $([]);
            if (input.hasClass(this.markerClassName))
                return;
            var appendText = this._get(inst, 'appendText');
            var isRTL = this._get(inst, 'isRTL');
            if (appendText) {
                inst.append = $('<span class="' + this._appendClass + '">' + appendText + '</span>');
                input[isRTL ? 'before' : 'after'](inst.append);
            }
            var showOn = this._get(inst, 'showOn');
            if (showOn == 'focus' || showOn == 'both') // pop-up date picker when in the marked field
                input.focus(this._showDatepicker);
            if (showOn == 'button' || showOn == 'both') { // pop-up date picker when button clicked
                var buttonText = this._get(inst, 'buttonText');
                var buttonImage = this._get(inst, 'buttonImage');
                inst.trigger = $(this._get(inst, 'buttonImageOnly') ?
				$('<img/>').addClass(this._triggerClass).
					attr({ src: buttonImage, alt: buttonText, title: buttonText }) :
				$('<button type="button"></button>').addClass(this._triggerClass).
					html(buttonImage == '' ? buttonText : $('<img/>').attr(
					{ src: buttonImage, alt: buttonText, title: buttonText })));
                input[isRTL ? 'before' : 'after'](inst.trigger);
                inst.trigger.click(function () {
                    if ($.datepicker._datepickerShowing && $.datepicker._lastInput == target)
                        $.datepicker._hideDatepicker();
                    else
                        $.datepicker._showDatepicker(target);
                    return false;
                });
            }
            input.addClass(this.markerClassName).keydown(this._doKeyDown).keypress(this._doKeyPress).
			bind("setData.datepicker", function (event, key, value) {
			    inst.settings[key] = value;
			}).bind("getData.datepicker", function (event, key) {
			    return this._get(inst, key);
			});
            $.data(target, PROP_NAME, inst);
        },

        /* Attach an inline date picker to a div. */
        _inlineDatepicker: function (target, inst) {
            var divSpan = $(target);
            if (divSpan.hasClass(this.markerClassName))
                return;
            divSpan.addClass(this.markerClassName).append(inst.dpDiv).
			bind("setData.datepicker", function (event, key, value) {
			    inst.settings[key] = value;
			}).bind("getData.datepicker", function (event, key) {
			    return this._get(inst, key);
			});
            $.data(target, PROP_NAME, inst);
            this._setDate(inst, this._getDefaultDate(inst));
            this._updateDatepicker(inst);
            this._updateAlternate(inst);
        },

        /* Pop-up the date picker in a "dialog" box.
        @param  input     element - ignored
        @param  dateText  string - the initial date to display (in the current format)
        @param  onSelect  function - the function(dateText) to call when a date is selected
        @param  settings  object - update the dialog date picker instance's settings (anonymous object)
        @param  pos       int[2] - coordinates for the dialog's position within the screen or
        event - with x/y coordinates or
        leave empty for default (screen centre)
        @return the manager object */
        _dialogDatepicker: function (input, dateText, onSelect, settings, pos) {
            var inst = this._dialogInst; // internal instance
            if (!inst) {
                var id = 'dp' + (++this.uuid);
                this._dialogInput = $('<input type="text" id="' + id +
				'" size="1" style="position: absolute; top: -100px;"/>');
                this._dialogInput.keydown(this._doKeyDown);
                $('body').append(this._dialogInput);
                inst = this._dialogInst = this._newInst(this._dialogInput, false);
                inst.settings = {};
                $.data(this._dialogInput[0], PROP_NAME, inst);
            }
            extendRemove(inst.settings, settings || {});
            this._dialogInput.val(dateText);

            this._pos = (pos ? (pos.length ? pos : [pos.pageX, pos.pageY]) : null);
            if (!this._pos) {
                var browserWidth = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
                var browserHeight = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
                var scrollX = document.documentElement.scrollLeft || document.body.scrollLeft;
                var scrollY = document.documentElement.scrollTop || document.body.scrollTop;
                this._pos = // should use actual width/height below
				[(browserWidth / 2) - 100 + scrollX, (browserHeight / 2) - 150 + scrollY];
            }

            // move input on screen for focus, but hidden behind dialog
            this._dialogInput.css('left', this._pos[0] + 'px').css('top', this._pos[1] + 'px');
            inst.settings.onSelect = onSelect;
            this._inDialog = true;
            this.dpDiv.addClass(this._dialogClass);
            this._showDatepicker(this._dialogInput[0]);
            if ($.blockUI)
                $.blockUI(this.dpDiv);
            $.data(this._dialogInput[0], PROP_NAME, inst);
            return this;
        },

        /* Detach a datepicker from its control.
        @param  target    element - the target input field or division or span */
        _destroyDatepicker: function (target) {
            var $target = $(target);
            var inst = $.data(target, PROP_NAME);
            if (!$target.hasClass(this.markerClassName)) {
                return;
            }
            var nodeName = target.nodeName.toLowerCase();
            $.removeData(target, PROP_NAME);
            if (nodeName == 'input') {
                inst.append.remove();
                inst.trigger.remove();
                $target.removeClass(this.markerClassName).
				unbind('focus', this._showDatepicker).
				unbind('keydown', this._doKeyDown).
				unbind('keypress', this._doKeyPress);
            } else if (nodeName == 'div' || nodeName == 'span')
                $target.removeClass(this.markerClassName).empty();
        },

        /* Enable the date picker to a jQuery selection.
        @param  target    element - the target input field or division or span */
        _enableDatepicker: function (target) {
            var $target = $(target);
            var inst = $.data(target, PROP_NAME);
            if (!$target.hasClass(this.markerClassName)) {
                return;
            }
            var nodeName = target.nodeName.toLowerCase();
            if (nodeName == 'input') {
                target.disabled = false;
                inst.trigger.filter('button').
				each(function () { this.disabled = false; }).end().
				filter('img').css({ opacity: '1.0', cursor: '' });
            }
            else if (nodeName == 'div' || nodeName == 'span') {
                var inline = $target.children('.' + this._inlineClass);
                inline.children().removeClass('ui-state-disabled');
            }
            this._disabledInputs = $.map(this._disabledInputs,
			function (value) { return (value == target ? null : value); }); // delete entry
        },

        /* Disable the date picker to a jQuery selection.
        @param  target    element - the target input field or division or span */
        _disableDatepicker: function (target) {
            var $target = $(target);
            var inst = $.data(target, PROP_NAME);
            if (!$target.hasClass(this.markerClassName)) {
                return;
            }
            var nodeName = target.nodeName.toLowerCase();
            if (nodeName == 'input') {
                target.disabled = true;
                inst.trigger.filter('button').
				each(function () { this.disabled = true; }).end().
				filter('img').css({ opacity: '0.5', cursor: 'default' });
            }
            else if (nodeName == 'div' || nodeName == 'span') {
                var inline = $target.children('.' + this._inlineClass);
                inline.children().addClass('ui-state-disabled');
            }
            this._disabledInputs = $.map(this._disabledInputs,
			function (value) { return (value == target ? null : value); }); // delete entry
            this._disabledInputs[this._disabledInputs.length] = target;
        },

        /* Is the first field in a jQuery collection disabled as a datepicker?
        @param  target    element - the target input field or division or span
        @return boolean - true if disabled, false if enabled */
        _isDisabledDatepicker: function (target) {
            if (!target) {
                return false;
            }
            for (var i = 0; i < this._disabledInputs.length; i++) {
                if (this._disabledInputs[i] == target)
                    return true;
            }
            return false;
        },

        /* Retrieve the instance data for the target control.
        @param  target  element - the target input field or division or span
        @return  object - the associated instance data
        @throws  error if a jQuery problem getting data */
        _getInst: function (target) {
            try {
                return $.data(target, PROP_NAME);
            }
            catch (err) {
                throw 'Missing instance data for this datepicker';
            }
        },

        /* Update or retrieve the settings for a date picker attached to an input field or division.
        @param  target  element - the target input field or division or span
        @param  name    object - the new settings to update or
        string - the name of the setting to change or retrieve,
        when retrieving also 'all' for all instance settings or
        'defaults' for all global defaults
        @param  value   any - the new value for the setting
        (omit if above is an object or to retrieve a value) */
        _optionDatepicker: function (target, name, value) {
            var inst = this._getInst(target);
            if (arguments.length == 2 && typeof name == 'string') {
                return (name == 'defaults' ? $.extend({}, $.datepicker._defaults) :
				(inst ? (name == 'all' ? $.extend({}, inst.settings) :
				this._get(inst, name)) : null));
            }
            var settings = name || {};
            if (typeof name == 'string') {
                settings = {};
                settings[name] = value;
            }
            if (inst) {
                if (this._curInst == inst) {
                    this._hideDatepicker(null);
                }
                var date = this._getDateDatepicker(target);
                extendRemove(inst.settings, settings);
                this._setDateDatepicker(target, date);
                this._updateDatepicker(inst);
            }
        },

        // change method deprecated
        _changeDatepicker: function (target, name, value) {
            this._optionDatepicker(target, name, value);
        },

        /* Redraw the date picker attached to an input field or division.
        @param  target  element - the target input field or division or span */
        _refreshDatepicker: function (target) {
            var inst = this._getInst(target);
            if (inst) {
                this._updateDatepicker(inst);
            }
        },

        /* Set the dates for a jQuery selection.
        @param  target   element - the target input field or division or span
        @param  date     Date - the new date
        @param  endDate  Date - the new end date for a range (optional) */
        _setDateDatepicker: function (target, date, endDate) {
            var inst = this._getInst(target);
            if (inst) {
                this._setDate(inst, date, endDate);
                this._updateDatepicker(inst);
                this._updateAlternate(inst);
            }
        },

        /* Get the date(s) for the first entry in a jQuery selection.
        @param  target  element - the target input field or division or span
        @return Date - the current date or
        Date[2] - the current dates for a range */
        _getDateDatepicker: function (target) {
            var inst = this._getInst(target);
            if (inst && !inst.inline)
                this._setDateFromField(inst);
            return (inst ? this._getDate(inst) : null);
        },

        /* Handle keystrokes. */
        _doKeyDown: function (event) {
            var inst = $.datepicker._getInst(event.target);
            var handled = true;
            var isRTL = inst.dpDiv.is('.ui-datepicker-rtl');
            inst._keyEvent = true;
            if ($.datepicker._datepickerShowing)
                switch (event.keyCode) {
                case 9: $.datepicker._hideDatepicker(null, '');
                    break; // hide on tab out
                case 13: var sel = $('td.' + $.datepicker._dayOverClass +
							', td.' + $.datepicker._currentClass, inst.dpDiv);
                    if (sel[0])
                        $.datepicker._selectDay(event.target, inst.selectedMonth, inst.selectedYear, sel[0]);
                    else
                        $.datepicker._hideDatepicker(null, $.datepicker._get(inst, 'duration'));
                    return false; // don't submit the form
                    break; // select the value on enter
                case 27: $.datepicker._hideDatepicker(null, $.datepicker._get(inst, 'duration'));
                    break; // hide on escape
                case 33: $.datepicker._adjustDate(event.target, (event.ctrlKey ?
							-$.datepicker._get(inst, 'stepBigMonths') :
							-$.datepicker._get(inst, 'stepMonths')), 'M');
                    break; // previous month/year on page up/+ ctrl
                case 34: $.datepicker._adjustDate(event.target, (event.ctrlKey ?
							+$.datepicker._get(inst, 'stepBigMonths') :
							+$.datepicker._get(inst, 'stepMonths')), 'M');
                    break; // next month/year on page down/+ ctrl
                case 35: if (event.ctrlKey || event.metaKey) $.datepicker._clearDate(event.target);
                    handled = event.ctrlKey || event.metaKey;
                    break; // clear on ctrl or command +end
                case 36: if (event.ctrlKey || event.metaKey) $.datepicker._gotoToday(event.target);
                    handled = event.ctrlKey || event.metaKey;
                    break; // current on ctrl or command +home
                case 37: if (event.ctrlKey || event.metaKey) $.datepicker._adjustDate(event.target, (isRTL ? +1 : -1), 'D');
                    handled = event.ctrlKey || event.metaKey;
                    // -1 day on ctrl or command +left
                    if (event.originalEvent.altKey) $.datepicker._adjustDate(event.target, (event.ctrlKey ?
									-$.datepicker._get(inst, 'stepBigMonths') :
									-$.datepicker._get(inst, 'stepMonths')), 'M');
                    // next month/year on alt +left on Mac
                    break;
                case 38: if (event.ctrlKey || event.metaKey) $.datepicker._adjustDate(event.target, -7, 'D');
                    handled = event.ctrlKey || event.metaKey;
                    break; // -1 week on ctrl or command +up
                case 39: if (event.ctrlKey || event.metaKey) $.datepicker._adjustDate(event.target, (isRTL ? -1 : +1), 'D');
                    handled = event.ctrlKey || event.metaKey;
                    // +1 day on ctrl or command +right
                    if (event.originalEvent.altKey) $.datepicker._adjustDate(event.target, (event.ctrlKey ?
									+$.datepicker._get(inst, 'stepBigMonths') :
									+$.datepicker._get(inst, 'stepMonths')), 'M');
                    // next month/year on alt +right
                    break;
                case 40: if (event.ctrlKey || event.metaKey) $.datepicker._adjustDate(event.target, +7, 'D');
                    handled = event.ctrlKey || event.metaKey;
                    break; // +1 week on ctrl or command +down
                default: handled = false;
            }
            else if (event.keyCode == 36 && event.ctrlKey) // display the date picker on ctrl+home
                $.datepicker._showDatepicker(this);
            else {
                handled = false;
            }
            if (handled) {
                event.preventDefault();
                event.stopPropagation();
            }
        },

        /* Filter entered characters - based on date format. */
        _doKeyPress: function (event) {
            var inst = $.datepicker._getInst(event.target);
            if ($.datepicker._get(inst, 'constrainInput')) {
                var chars = $.datepicker._possibleChars($.datepicker._get(inst, 'dateFormat'));
                var chr = String.fromCharCode(event.charCode == undefined ? event.keyCode : event.charCode);
                return event.ctrlKey || (chr < ' ' || !chars || chars.indexOf(chr) > -1);
            }
        },

        /* Pop-up the date picker for a given input field.
        @param  input  element - the input field attached to the date picker or
        event - if triggered by focus */
        _showDatepicker: function (input) {
            input = input.target || input;
            if (input.nodeName.toLowerCase() != 'input') // find from button/image trigger
                input = $('input', input.parentNode)[0];
            if ($.datepicker._isDisabledDatepicker(input) || $.datepicker._lastInput == input) // already here
                return;
            var inst = $.datepicker._getInst(input);
            var beforeShow = $.datepicker._get(inst, 'beforeShow');
            extendRemove(inst.settings, (beforeShow ? beforeShow.apply(input, [input, inst]) : {}));
            $.datepicker._hideDatepicker(null, '');
            $.datepicker._lastInput = input;
            $.datepicker._setDateFromField(inst);
            if ($.datepicker._inDialog) // hide cursor
                input.value = '';
            if (!$.datepicker._pos) { // position below input
                $.datepicker._pos = $.datepicker._findPos(input);
                $.datepicker._pos[1] += input.offsetHeight; // add the height
            }
            var isFixed = false;
            $(input).parents().each(function () {
                isFixed |= $(this).css('position') == 'fixed';
                return !isFixed;
            });
            if (isFixed && $.browser.opera) { // correction for Opera when fixed and scrolled
                $.datepicker._pos[0] -= document.documentElement.scrollLeft;
                $.datepicker._pos[1] -= document.documentElement.scrollTop;
            }
            var offset = { left: $.datepicker._pos[0], top: $.datepicker._pos[1] };
            $.datepicker._pos = null;
            inst.rangeStart = null;
            // determine sizing offscreen
            inst.dpDiv.css({ position: 'absolute', display: 'block', top: '-1000px' });
            $.datepicker._updateDatepicker(inst);
            // fix width for dynamic number of date pickers
            // and adjust position before showing
            offset = $.datepicker._checkOffset(inst, offset, isFixed);
            inst.dpDiv.css({ position: ($.datepicker._inDialog && $.blockUI ?
			'static' : (isFixed ? 'fixed' : 'absolute')), display: 'none',
                left: offset.left + 'px', top: offset.top + 'px'
            });
            if (!inst.inline) {
                var showAnim = $.datepicker._get(inst, 'showAnim') || 'show';
                var duration = $.datepicker._get(inst, 'duration');
                var postProcess = function () {
                    $.datepicker._datepickerShowing = true;
                    if ($.browser.msie && parseInt($.browser.version, 10) < 7) // fix IE < 7 select problems
                        $('iframe.ui-datepicker-cover').css({ width: inst.dpDiv.width() + 4,
                            height: inst.dpDiv.height() + 4
                        });
                };
                if ($.effects && $.effects[showAnim])
                    inst.dpDiv.show(showAnim, $.datepicker._get(inst, 'showOptions'), duration, postProcess);
                else
                    inst.dpDiv[showAnim](duration, postProcess);
                if (duration == '')
                    postProcess();
                if (inst.input[0].type != 'hidden')
                    inst.input[0].focus();
                $.datepicker._curInst = inst;
            }
        },

        /* Generate the date picker content. */
        _updateDatepicker: function (inst) {
            var dims = { width: inst.dpDiv.width() + 4,
                height: inst.dpDiv.height() + 4
            };
            var self = this;
            inst.dpDiv.empty().append(this._generateHTML(inst))
			.find('iframe.ui-datepicker-cover').
				css({ width: dims.width, height: dims.height })
			.end()
			.find('button, .ui-datepicker-prev, .ui-datepicker-next, .ui-datepicker-calendar td a')
				.bind('mouseout', function () {
				    $(this).removeClass('ui-state-hover');
				    if (this.className.indexOf('ui-datepicker-prev') != -1) $(this).removeClass('ui-datepicker-prev-hover');
				    if (this.className.indexOf('ui-datepicker-next') != -1) $(this).removeClass('ui-datepicker-next-hover');
				})
				.bind('mouseover', function () {
				    if (!self._isDisabledDatepicker(inst.inline ? inst.dpDiv.parent()[0] : inst.input[0])) {
				        $(this).parents('.ui-datepicker-calendar').find('a').removeClass('ui-state-hover');
				        $(this).addClass('ui-state-hover');
				        if (this.className.indexOf('ui-datepicker-prev') != -1) $(this).addClass('ui-datepicker-prev-hover');
				        if (this.className.indexOf('ui-datepicker-next') != -1) $(this).addClass('ui-datepicker-next-hover');
				    }
				})
			.end()
			.find('.' + this._dayOverClass + ' a')
				.trigger('mouseover')
			.end();
            var numMonths = this._getNumberOfMonths(inst);
            var cols = numMonths[1];
            var width = 17;
            if (cols > 1) {
                inst.dpDiv.addClass('ui-datepicker-multi-' + cols).css('width', (width * cols) + 'em');
            } else {
                inst.dpDiv.removeClass('ui-datepicker-multi-2 ui-datepicker-multi-3 ui-datepicker-multi-4').width('');
            }
            inst.dpDiv[(numMonths[0] != 1 || numMonths[1] != 1 ? 'add' : 'remove') +
			'Class']('ui-datepicker-multi');
            inst.dpDiv[(this._get(inst, 'isRTL') ? 'add' : 'remove') +
			'Class']('ui-datepicker-rtl');
            if (inst.input && inst.input[0].type != 'hidden' && inst == $.datepicker._curInst)
                $(inst.input[0]).focus();
        },

        /* Check positioning to remain on screen. */
        _checkOffset: function (inst, offset, isFixed) {
            var dpWidth = inst.dpDiv.outerWidth();
            var dpHeight = inst.dpDiv.outerHeight();
            var inputWidth = inst.input ? inst.input.outerWidth() : 0;
            var inputHeight = inst.input ? inst.input.outerHeight() : 0;
            var viewWidth = (window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth) + $(document).scrollLeft();
            var viewHeight = (window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight) + $(document).scrollTop();

            offset.left -= (this._get(inst, 'isRTL') ? (dpWidth - inputWidth) : 0);
            offset.left -= (isFixed && offset.left == inst.input.offset().left) ? $(document).scrollLeft() : 0;
            offset.top -= (isFixed && offset.top == (inst.input.offset().top + inputHeight)) ? $(document).scrollTop() : 0;

            // now check if datepicker is showing outside window viewport - move to a better place if so.
            offset.left -= (offset.left + dpWidth > viewWidth && viewWidth > dpWidth) ? Math.abs(offset.left + dpWidth - viewWidth) : 0;
            offset.top -= (offset.top + dpHeight > viewHeight && viewHeight > dpHeight) ? Math.abs(offset.top + dpHeight + inputHeight * 2 - viewHeight) : 0;

            return offset;
        },

        /* Find an object's position on the screen. */
        _findPos: function (obj) {
            while (obj && (obj.type == 'hidden' || obj.nodeType != 1)) {
                obj = obj.nextSibling;
            }
            var position = $(obj).offset();
            return [position.left, position.top];
        },

        /* Hide the date picker from view.
        @param  input  element - the input field attached to the date picker
        @param  duration  string - the duration over which to close the date picker */
        _hideDatepicker: function (input, duration) {
            var inst = this._curInst;
            if (!inst || (input && inst != $.data(input, PROP_NAME)))
                return;
            if (inst.stayOpen)
                this._selectDate('#' + inst.id, this._formatDate(inst,
				inst.currentDay, inst.currentMonth, inst.currentYear));
            inst.stayOpen = false;
            if (this._datepickerShowing) {
                duration = (duration != null ? duration : this._get(inst, 'duration'));
                var showAnim = this._get(inst, 'showAnim');
                var postProcess = function () {
                    $.datepicker._tidyDialog(inst);
                };
                if (duration != '' && $.effects && $.effects[showAnim])
                    inst.dpDiv.hide(showAnim, $.datepicker._get(inst, 'showOptions'),
					duration, postProcess);
                else
                    inst.dpDiv[(duration == '' ? 'hide' : (showAnim == 'slideDown' ? 'slideUp' :
					(showAnim == 'fadeIn' ? 'fadeOut' : 'hide')))](duration, postProcess);
                if (duration == '')
                    this._tidyDialog(inst);
                var onClose = this._get(inst, 'onClose');
                if (onClose)
                    onClose.apply((inst.input ? inst.input[0] : null),
					[(inst.input ? inst.input.val() : ''), inst]);  // trigger custom callback
                this._datepickerShowing = false;
                this._lastInput = null;
                if (this._inDialog) {
                    this._dialogInput.css({ position: 'absolute', left: '0', top: '-100px' });
                    if ($.blockUI) {
                        $.unblockUI();
                        $('body').append(this.dpDiv);
                    }
                }
                this._inDialog = false;
            }
            this._curInst = null;
        },

        /* Tidy up after a dialog display. */
        _tidyDialog: function (inst) {
            inst.dpDiv.removeClass(this._dialogClass).unbind('.ui-datepicker-calendar');
        },

        /* Close date picker if clicked elsewhere. */
        _checkExternalClick: function (event) {
            if (!$.datepicker._curInst)
                return;
            var $target = $(event.target);
            if (($target.parents('#' + $.datepicker._mainDivId).length == 0) &&
				!$target.hasClass($.datepicker.markerClassName) &&
				!$target.hasClass($.datepicker._triggerClass) &&
				$.datepicker._datepickerShowing && !($.datepicker._inDialog && $.blockUI))
                $.datepicker._hideDatepicker(null, '');
        },

        /* Adjust one of the date sub-fields. */
        _adjustDate: function (id, offset, period) {
            var target = $(id);
            var inst = this._getInst(target[0]);
            if (this._isDisabledDatepicker(target[0])) {
                return;
            }
            this._adjustInstDate(inst, offset +
			(period == 'M' ? this._get(inst, 'showCurrentAtPos') : 0), // undo positioning
			period);
            this._updateDatepicker(inst);
        },

        /* Action for current link. */
        _gotoToday: function (id) {
            var target = $(id);
            var inst = this._getInst(target[0]);
            if (this._get(inst, 'gotoCurrent') && inst.currentDay) {
                inst.selectedDay = inst.currentDay;
                inst.drawMonth = inst.selectedMonth = inst.currentMonth;
                inst.drawYear = inst.selectedYear = inst.currentYear;
            }
            else {
                var date = new Date();
                inst.selectedDay = date.getDate();
                inst.drawMonth = inst.selectedMonth = date.getMonth();
                inst.drawYear = inst.selectedYear = date.getFullYear();
            }
            this._notifyChange(inst);
            this._adjustDate(target);
        },

        /* Action for selecting a new month/year. */
        _selectMonthYear: function (id, select, period) {
            var target = $(id);
            var inst = this._getInst(target[0]);
            inst._selectingMonthYear = false;
            inst['selected' + (period == 'M' ? 'Month' : 'Year')] =
		inst['draw' + (period == 'M' ? 'Month' : 'Year')] =
			parseInt(select.options[select.selectedIndex].value, 10);
            this._notifyChange(inst);
            this._adjustDate(target);
        },

        /* Restore input focus after not changing month/year. */
        _clickMonthYear: function (id) {
            var target = $(id);
            var inst = this._getInst(target[0]);
            if (inst.input && inst._selectingMonthYear && !$.browser.msie)
                inst.input[0].focus();
            inst._selectingMonthYear = !inst._selectingMonthYear;
        },

        /* Action for selecting a day. */
        _selectDay: function (id, month, year, td) {
            var target = $(id);
            if ($(td).hasClass(this._unselectableClass) || this._isDisabledDatepicker(target[0])) {
                return;
            }
            var inst = this._getInst(target[0]);
            inst.selectedDay = inst.currentDay = $('a', td).html();
            inst.selectedMonth = inst.currentMonth = month;
            inst.selectedYear = inst.currentYear = year;
            if (inst.stayOpen) {
                inst.endDay = inst.endMonth = inst.endYear = null;
            }
            this._selectDate(id, this._formatDate(inst,
			inst.currentDay, inst.currentMonth, inst.currentYear));
            if (inst.stayOpen) {
                inst.rangeStart = this._daylightSavingAdjust(
				new Date(inst.currentYear, inst.currentMonth, inst.currentDay));
                this._updateDatepicker(inst);
            }
        },

        /* Erase the input field and hide the date picker. */
        _clearDate: function (id) {
            var target = $(id);
            var inst = this._getInst(target[0]);
            inst.stayOpen = false;
            inst.endDay = inst.endMonth = inst.endYear = inst.rangeStart = null;
            this._selectDate(target, '');
        },

        /* Update the input field with the selected date. */
        _selectDate: function (id, dateStr) {
            var target = $(id);
            var inst = this._getInst(target[0]);
            dateStr = (dateStr != null ? dateStr : this._formatDate(inst));
            if (inst.input)
                inst.input.val(dateStr);
            this._updateAlternate(inst);
            var onSelect = this._get(inst, 'onSelect');
            if (onSelect)
                onSelect.apply((inst.input ? inst.input[0] : null), [dateStr, inst]);  // trigger custom callback
            else if (inst.input)
                inst.input.trigger('change'); // fire the change event
            if (inst.inline)
                this._updateDatepicker(inst);
            else if (!inst.stayOpen) {
                this._hideDatepicker(null, this._get(inst, 'duration'));
                this._lastInput = inst.input[0];
                if (typeof (inst.input[0]) != 'object')
                    inst.input[0].focus(); // restore focus
                this._lastInput = null;
            }
        },

        /* Update any alternate field to synchronise with the main field. */
        _updateAlternate: function (inst) {
            var altField = this._get(inst, 'altField');
            if (altField) { // update alternate field too
                var altFormat = this._get(inst, 'altFormat') || this._get(inst, 'dateFormat');
                var date = this._getDate(inst);
                dateStr = this.formatDate(altFormat, date, this._getFormatConfig(inst));
                $(altField).each(function () { $(this).val(dateStr); });
            }
        },

        /* Set as beforeShowDay function to prevent selection of weekends.
        @param  date  Date - the date to customise
        @return [boolean, string] - is this date selectable?, what is its CSS class? */
        noWeekends: function (date) {
            var day = date.getDay();
            return [(day > 0 && day < 6), ''];
        },

        /* Set as calculateWeek to determine the week of the year based on the ISO 8601 definition.
        @param  date  Date - the date to get the week for
        @return  number - the number of the week within the year that contains this date */
        iso8601Week: function (date) {
            var checkDate = new Date(date.getFullYear(), date.getMonth(), date.getDate());
            var firstMon = new Date(checkDate.getFullYear(), 1 - 1, 4); // First week always contains 4 Jan
            var firstDay = firstMon.getDay() || 7; // Day of week: Mon = 1, ..., Sun = 7
            firstMon.setDate(firstMon.getDate() + 1 - firstDay); // Preceding Monday
            if (firstDay < 4 && checkDate < firstMon) { // Adjust first three days in year if necessary
                checkDate.setDate(checkDate.getDate() - 3); // Generate for previous year
                return $.datepicker.iso8601Week(checkDate);
            } else if (checkDate > new Date(checkDate.getFullYear(), 12 - 1, 28)) { // Check last three days in year
                firstDay = new Date(checkDate.getFullYear() + 1, 1 - 1, 4).getDay() || 7;
                if (firstDay > 4 && (checkDate.getDay() || 7) < firstDay - 3) { // Adjust if necessary
                    return 1;
                }
            }
            return Math.floor(((checkDate - firstMon) / 86400000) / 7) + 1; // Weeks to given date
        },

        /* Parse a string value into a date object.
        See formatDate below for the possible formats.

        @param  format    string - the expected format of the date
        @param  value     string - the date in the above format
        @param  settings  Object - attributes include:
        shortYearCutoff  number - the cutoff year for determining the century (optional)
        dayNamesShort    string[7] - abbreviated names of the days from Sunday (optional)
        dayNames         string[7] - names of the days from Sunday (optional)
        monthNamesShort  string[12] - abbreviated names of the months (optional)
        monthNames       string[12] - names of the months (optional)
        @return  Date - the extracted date value or null if value is blank */
        parseDate: function (format, value, settings) {
            if (format == null || value == null)
                throw 'Invalid arguments';
            value = (typeof value == 'object' ? value.toString() : value + '');
            if (value == '')
                return null;
            var shortYearCutoff = (settings ? settings.shortYearCutoff : null) || this._defaults.shortYearCutoff;
            var dayNamesShort = (settings ? settings.dayNamesShort : null) || this._defaults.dayNamesShort;
            var dayNames = (settings ? settings.dayNames : null) || this._defaults.dayNames;
            var monthNamesShort = (settings ? settings.monthNamesShort : null) || this._defaults.monthNamesShort;
            var monthNames = (settings ? settings.monthNames : null) || this._defaults.monthNames;
            var year = -1;
            var month = -1;
            var day = -1;
            var doy = -1;
            var literal = false;
            // Check whether a format character is doubled
            var lookAhead = function (match) {
                var matches = (iFormat + 1 < format.length && format.charAt(iFormat + 1) == match);
                if (matches)
                    iFormat++;
                return matches;
            };
            // Extract a number from the string value
            var getNumber = function (match) {
                lookAhead(match);
                var origSize = (match == '@' ? 14 : (match == 'y' ? 4 : (match == 'o' ? 3 : 2)));
                var size = origSize;
                var num = 0;
                while (size > 0 && iValue < value.length &&
					value.charAt(iValue) >= '0' && value.charAt(iValue) <= '9') {
                    num = num * 10 + parseInt(value.charAt(iValue++), 10);
                    size--;
                }
                if (size == origSize)
                    throw 'Missing number at position ' + iValue;
                return num;
            };
            // Extract a name from the string value and convert to an index
            var getName = function (match, shortNames, longNames) {
                var names = (lookAhead(match) ? longNames : shortNames);
                var size = 0;
                for (var j = 0; j < names.length; j++)
                    size = Math.max(size, names[j].length);
                var name = '';
                var iInit = iValue;
                while (size > 0 && iValue < value.length) {
                    name += value.charAt(iValue++);
                    for (var i = 0; i < names.length; i++)
                        if (name == names[i])
                            return i + 1;
                    size--;
                }
                throw 'Unknown name at position ' + iInit;
            };
            // Confirm that a literal character matches the string value
            var checkLiteral = function () {
                if (value.charAt(iValue) != format.charAt(iFormat))
                    throw 'Unexpected literal at position ' + iValue;
                iValue++;
            };
            var iValue = 0;
            for (var iFormat = 0; iFormat < format.length; iFormat++) {
                if (literal)
                    if (format.charAt(iFormat) == "'" && !lookAhead("'"))
                        literal = false;
                    else
                        checkLiteral();
                else
                    switch (format.charAt(iFormat)) {
                    case 'd':
                        day = getNumber('d');
                        break;
                    case 'D':
                        getName('D', dayNamesShort, dayNames);
                        break;
                    case 'o':
                        doy = getNumber('o');
                        break;
                    case 'm':
                        month = getNumber('m');
                        break;
                    case 'M':
                        month = getName('M', monthNamesShort, monthNames);
                        break;
                    case 'y':
                        year = getNumber('y');
                        break;
                    case '@':
                        var date = new Date(getNumber('@'));
                        year = date.getFullYear();
                        month = date.getMonth() + 1;
                        day = date.getDate();
                        break;
                    case "'":
                        if (lookAhead("'"))
                            checkLiteral();
                        else
                            literal = true;
                        break;
                    default:
                        checkLiteral();
                }
            }
            if (year == -1)
                year = new Date().getFullYear();
            else if (year < 100)
                year += new Date().getFullYear() - new Date().getFullYear() % 100 +
				(year <= shortYearCutoff ? 0 : -100);
            if (doy > -1) {
                month = 1;
                day = doy;
                do {
                    var dim = this._getDaysInMonth(year, month - 1);
                    if (day <= dim)
                        break;
                    month++;
                    day -= dim;
                } while (true);
            }
            var date = this._daylightSavingAdjust(new Date(year, month - 1, day));
            if (date.getFullYear() != year || date.getMonth() + 1 != month || date.getDate() != day)
                throw 'Invalid date'; // E.g. 31/02/*
            return date;
        },

        /* Standard date formats. */
        ATOM: 'yy-mm-dd', // RFC 3339 (ISO 8601)
        COOKIE: 'D, dd M yy',
        ISO_8601: 'yy-mm-dd',
        RFC_822: 'D, d M y',
        RFC_850: 'DD, dd-M-y',
        RFC_1036: 'D, d M y',
        RFC_1123: 'D, d M yy',
        RFC_2822: 'D, d M yy',
        RSS: 'D, d M y', // RFC 822
        TIMESTAMP: '@',
        W3C: 'yy-mm-dd', // ISO 8601

        /* Format a date object into a string value.
        The format can be combinations of the following:
        d  - day of month (no leading zero)
        dd - day of month (two digit)
        o  - day of year (no leading zeros)
        oo - day of year (three digit)
        D  - day name short
        DD - day name long
        m  - month of year (no leading zero)
        mm - month of year (two digit)
        M  - month name short
        MM - month name long
        y  - year (two digit)
        yy - year (four digit)
        @ - Unix timestamp (ms since 01/01/1970)
        '...' - literal text
        '' - single quote

        @param  format    string - the desired format of the date
        @param  date      Date - the date value to format
        @param  settings  Object - attributes include:
        dayNamesShort    string[7] - abbreviated names of the days from Sunday (optional)
        dayNames         string[7] - names of the days from Sunday (optional)
        monthNamesShort  string[12] - abbreviated names of the months (optional)
        monthNames       string[12] - names of the months (optional)
        @return  string - the date in the above format */
        formatDate: function (format, date, settings) {
            if (!date)
                return '';
            var dayNamesShort = (settings ? settings.dayNamesShort : null) || this._defaults.dayNamesShort;
            var dayNames = (settings ? settings.dayNames : null) || this._defaults.dayNames;
            var monthNamesShort = (settings ? settings.monthNamesShort : null) || this._defaults.monthNamesShort;
            var monthNames = (settings ? settings.monthNames : null) || this._defaults.monthNames;
            // Check whether a format character is doubled
            var lookAhead = function (match) {
                var matches = (iFormat + 1 < format.length && format.charAt(iFormat + 1) == match);
                if (matches)
                    iFormat++;
                return matches;
            };
            // Format a number, with leading zero if necessary
            var formatNumber = function (match, value, len) {
                var num = '' + value;
                if (lookAhead(match))
                    while (num.length < len)
                        num = '0' + num;
                return num;
            };
            // Format a name, short or long as requested
            var formatName = function (match, value, shortNames, longNames) {
                return (lookAhead(match) ? longNames[value] : shortNames[value]);
            };
            var output = '';
            var literal = false;
            if (date)
                for (var iFormat = 0; iFormat < format.length; iFormat++) {
                    if (literal)
                        if (format.charAt(iFormat) == "'" && !lookAhead("'"))
                            literal = false;
                        else
                            output += format.charAt(iFormat);
                    else
                        switch (format.charAt(iFormat)) {
                        case 'd':
                            output += formatNumber('d', date.getDate(), 2);
                            break;
                        case 'D':
                            output += formatName('D', date.getDay(), dayNamesShort, dayNames);
                            break;
                        case 'o':
                            var doy = date.getDate();
                            for (var m = date.getMonth() - 1; m >= 0; m--)
                                doy += this._getDaysInMonth(date.getFullYear(), m);
                            output += formatNumber('o', doy, 3);
                            break;
                        case 'm':
                            output += formatNumber('m', date.getMonth() + 1, 2);
                            break;
                        case 'M':
                            output += formatName('M', date.getMonth(), monthNamesShort, monthNames);
                            break;
                        case 'y':
                            output += (lookAhead('y') ? date.getFullYear() :
								(date.getYear() % 100 < 10 ? '0' : '') + date.getYear() % 100);
                            break;
                        case '@':
                            output += date.getTime();
                            break;
                        case "'":
                            if (lookAhead("'"))
                                output += "'";
                            else
                                literal = true;
                            break;
                        default:
                            output += format.charAt(iFormat);
                    }
                }
            return output;
        },

        /* Extract all possible characters from the date format. */
        _possibleChars: function (format) {
            var chars = '';
            var literal = false;
            for (var iFormat = 0; iFormat < format.length; iFormat++)
                if (literal)
                    if (format.charAt(iFormat) == "'" && !lookAhead("'"))
                        literal = false;
                    else
                        chars += format.charAt(iFormat);
                else
                    switch (format.charAt(iFormat)) {
                    case 'd': case 'm': case 'y': case '@':
                        chars += '0123456789';
                        break;
                    case 'D': case 'M':
                        return null; // Accept anything
                    case "'":
                        if (lookAhead("'"))
                            chars += "'";
                        else
                            literal = true;
                        break;
                    default:
                        chars += format.charAt(iFormat);
                }
            return chars;
        },

        /* Get a setting value, defaulting if necessary. */
        _get: function (inst, name) {
            return inst.settings[name] !== undefined ?
			inst.settings[name] : this._defaults[name];
        },

        /* Parse existing date and initialise date picker. */
        _setDateFromField: function (inst) {
            var dateFormat = this._get(inst, 'dateFormat');
            var dates = inst.input ? inst.input.val() : null;
            inst.endDay = inst.endMonth = inst.endYear = null;
            var date = defaultDate = this._getDefaultDate(inst);
            var settings = this._getFormatConfig(inst);
            try {
                date = this.parseDate(dateFormat, dates, settings) || defaultDate;
            } catch (event) {
                this.log(event);
                date = defaultDate;
            }
            inst.selectedDay = date.getDate();
            inst.drawMonth = inst.selectedMonth = date.getMonth();
            inst.drawYear = inst.selectedYear = date.getFullYear();
            inst.currentDay = (dates ? date.getDate() : 0);
            inst.currentMonth = (dates ? date.getMonth() : 0);
            inst.currentYear = (dates ? date.getFullYear() : 0);
            this._adjustInstDate(inst);
        },

        /* Retrieve the default date shown on opening. */
        _getDefaultDate: function (inst) {
            var date = this._determineDate(this._get(inst, 'defaultDate'), new Date());
            var minDate = this._getMinMaxDate(inst, 'min', true);
            var maxDate = this._getMinMaxDate(inst, 'max');
            date = (minDate && date < minDate ? minDate : date);
            date = (maxDate && date > maxDate ? maxDate : date);
            return date;
        },

        /* A date may be specified as an exact value or a relative one. */
        _determineDate: function (date, defaultDate) {
            var offsetNumeric = function (offset) {
                var date = new Date();
                date.setDate(date.getDate() + offset);
                return date;
            };
            var offsetString = function (offset, getDaysInMonth) {
                var date = new Date();
                var year = date.getFullYear();
                var month = date.getMonth();
                var day = date.getDate();
                var pattern = /([+-]?[0-9]+)\s*(d|D|w|W|m|M|y|Y)?/g;
                var matches = pattern.exec(offset);
                while (matches) {
                    switch (matches[2] || 'd') {
                        case 'd': case 'D':
                            day += parseInt(matches[1], 10); break;
                        case 'w': case 'W':
                            day += parseInt(matches[1], 10) * 7; break;
                        case 'm': case 'M':
                            month += parseInt(matches[1], 10);
                            day = Math.min(day, getDaysInMonth(year, month));
                            break;
                        case 'y': case 'Y':
                            year += parseInt(matches[1], 10);
                            day = Math.min(day, getDaysInMonth(year, month));
                            break;
                    }
                    matches = pattern.exec(offset);
                }
                return new Date(year, month, day);
            };
            date = (date == null ? defaultDate :
			(typeof date == 'string' ? offsetString(date, this._getDaysInMonth) :
			(typeof date == 'number' ? (isNaN(date) ? defaultDate : offsetNumeric(date)) : date)));
            date = (date && date.toString() == 'Invalid Date' ? defaultDate : date);
            if (date) {
                date.setHours(0);
                date.setMinutes(0);
                date.setSeconds(0);
                date.setMilliseconds(0);
            }
            return this._daylightSavingAdjust(date);
        },

        /* Handle switch to/from daylight saving.
        Hours may be non-zero on daylight saving cut-over:
        > 12 when midnight changeover, but then cannot generate
        midnight datetime, so jump to 1AM, otherwise reset.
        @param  date  (Date) the date to check
        @return  (Date) the corrected date */
        _daylightSavingAdjust: function (date) {
            if (!date) return null;
            date.setHours(date.getHours() > 12 ? date.getHours() + 2 : 0);
            return date;
        },

        /* Set the date(s) directly. */
        _setDate: function (inst, date, endDate) {
            var clear = !(date);
            var origMonth = inst.selectedMonth;
            var origYear = inst.selectedYear;
            date = this._determineDate(date, new Date());
            inst.selectedDay = inst.currentDay = date.getDate();
            inst.drawMonth = inst.selectedMonth = inst.currentMonth = date.getMonth();
            inst.drawYear = inst.selectedYear = inst.currentYear = date.getFullYear();
            if (origMonth != inst.selectedMonth || origYear != inst.selectedYear)
                this._notifyChange(inst);
            this._adjustInstDate(inst);
            if (inst.input) {
                inst.input.val(clear ? '' : this._formatDate(inst));
            }
        },

        /* Retrieve the date(s) directly. */
        _getDate: function (inst) {
            var startDate = (!inst.currentYear || (inst.input && inst.input.val() == '') ? null :
			this._daylightSavingAdjust(new Date(
			inst.currentYear, inst.currentMonth, inst.currentDay)));
            return startDate;
        },

        /* Generate the HTML for the current state of the date picker. */
        _generateHTML: function (inst) {
            var today = new Date();
            today = this._daylightSavingAdjust(
			new Date(today.getFullYear(), today.getMonth(), today.getDate())); // clear time
            var isRTL = this._get(inst, 'isRTL');
            var showButtonPanel = this._get(inst, 'showButtonPanel');
            var hideIfNoPrevNext = this._get(inst, 'hideIfNoPrevNext');
            var navigationAsDateFormat = this._get(inst, 'navigationAsDateFormat');
            var numMonths = this._getNumberOfMonths(inst);
            var showCurrentAtPos = this._get(inst, 'showCurrentAtPos');
            var stepMonths = this._get(inst, 'stepMonths');
            var stepBigMonths = this._get(inst, 'stepBigMonths');
            var isMultiMonth = (numMonths[0] != 1 || numMonths[1] != 1);
            var currentDate = this._daylightSavingAdjust((!inst.currentDay ? new Date(9999, 9, 9) :
			new Date(inst.currentYear, inst.currentMonth, inst.currentDay)));
            var minDate = this._getMinMaxDate(inst, 'min', true);
            var maxDate = this._getMinMaxDate(inst, 'max');
            var drawMonth = inst.drawMonth - showCurrentAtPos;
            var drawYear = inst.drawYear;
            if (drawMonth < 0) {
                drawMonth += 12;
                drawYear--;
            }
            if (maxDate) {
                var maxDraw = this._daylightSavingAdjust(new Date(maxDate.getFullYear(),
				maxDate.getMonth() - numMonths[1] + 1, maxDate.getDate()));
                maxDraw = (minDate && maxDraw < minDate ? minDate : maxDraw);
                while (this._daylightSavingAdjust(new Date(drawYear, drawMonth, 1)) > maxDraw) {
                    drawMonth--;
                    if (drawMonth < 0) {
                        drawMonth = 11;
                        drawYear--;
                    }
                }
            }
            inst.drawMonth = drawMonth;
            inst.drawYear = drawYear;
            var prevText = this._get(inst, 'prevText');
            prevText = (!navigationAsDateFormat ? prevText : this.formatDate(prevText,
			this._daylightSavingAdjust(new Date(drawYear, drawMonth - stepMonths, 1)),
			this._getFormatConfig(inst)));
            var prev = (this._canAdjustMonth(inst, -1, drawYear, drawMonth) ?
			'<a class="ui-datepicker-prev ui-corner-all" onclick="DP_jQuery.datepicker._adjustDate(\'#' + inst.id + '\', -' + stepMonths + ', \'M\');"' +
			' title="' + prevText + '"><span class="ui-icon ui-icon-circle-triangle-' + (isRTL ? 'e' : 'w') + '">' + prevText + '</span></a>' :
			(hideIfNoPrevNext ? '' : '<a class="ui-datepicker-prev ui-corner-all ui-state-disabled" title="' + prevText + '"><span class="ui-icon ui-icon-circle-triangle-' + (isRTL ? 'e' : 'w') + '">' + prevText + '</span></a>'));
            var nextText = this._get(inst, 'nextText');
            nextText = (!navigationAsDateFormat ? nextText : this.formatDate(nextText,
			this._daylightSavingAdjust(new Date(drawYear, drawMonth + stepMonths, 1)),
			this._getFormatConfig(inst)));
            var next = (this._canAdjustMonth(inst, +1, drawYear, drawMonth) ?
			'<a class="ui-datepicker-next ui-corner-all" onclick="DP_jQuery.datepicker._adjustDate(\'#' + inst.id + '\', +' + stepMonths + ', \'M\');"' +
			' title="' + nextText + '"><span class="ui-icon ui-icon-circle-triangle-' + (isRTL ? 'w' : 'e') + '">' + nextText + '</span></a>' :
			(hideIfNoPrevNext ? '' : '<a class="ui-datepicker-next ui-corner-all ui-state-disabled" title="' + nextText + '"><span class="ui-icon ui-icon-circle-triangle-' + (isRTL ? 'w' : 'e') + '">' + nextText + '</span></a>'));
            var currentText = this._get(inst, 'currentText');
            var gotoDate = (this._get(inst, 'gotoCurrent') && inst.currentDay ? currentDate : today);
            currentText = (!navigationAsDateFormat ? currentText :
			this.formatDate(currentText, gotoDate, this._getFormatConfig(inst)));
            var controls = (!inst.inline ? '<button type="button" class="ui-datepicker-close ui-state-default ui-priority-primary ui-corner-all" onclick="DP_jQuery.datepicker._hideDatepicker();">' + this._get(inst, 'closeText') + '</button>' : '');

            //udi add 2012年7月25日16:49:53
            var clearControl = '<button type="button" class="ui-datepicker-close ui-state-default ui-priority-primary ui-corner-all" onclick="DP_jQuery.datepicker._clearDate(\'#' + inst.id + '\');">' + this._get(inst, 'clearText') + '</button>';


            var buttonPanel = (showButtonPanel) ? '<div class="ui-datepicker-buttonpane ui-widget-content">' + (isRTL ? controls : '') +
			(this._isInRange(inst, gotoDate) ? '<button type="button" class="ui-datepicker-current ui-state-default ui-priority-secondary ui-corner-all" onclick="DP_jQuery.datepicker._gotoToday(\'#' + inst.id + '\');"' +
			'>' + currentText + '</button>' : '') + (isRTL ? '' : controls) +
            //udi add 2012年7月25日16:49:01
            (this._get(inst, 'showClearButton') ? clearControl : '') +
             '</div>' : '';
            var firstDay = parseInt(this._get(inst, 'firstDay'), 10);
            firstDay = (isNaN(firstDay) ? 0 : firstDay);
            var dayNames = this._get(inst, 'dayNames');
            var dayNamesShort = this._get(inst, 'dayNamesShort');
            var dayNamesMin = this._get(inst, 'dayNamesMin');
            var monthNames = this._get(inst, 'monthNames');
            var monthNamesShort = this._get(inst, 'monthNamesShort');
            var beforeShowDay = this._get(inst, 'beforeShowDay');
            var showOtherMonths = this._get(inst, 'showOtherMonths');
            var calculateWeek = this._get(inst, 'calculateWeek') || this.iso8601Week;
            var endDate = inst.endDay ? this._daylightSavingAdjust(
			new Date(inst.endYear, inst.endMonth, inst.endDay)) : currentDate;
            var defaultDate = this._getDefaultDate(inst);
            var html = '';
            for (var row = 0; row < numMonths[0]; row++) {
                var group = '';
                for (var col = 0; col < numMonths[1]; col++) {
                    var selectedDate = this._daylightSavingAdjust(new Date(drawYear, drawMonth, inst.selectedDay));
                    var cornerClass = ' ui-corner-all';
                    var calender = '';
                    if (isMultiMonth) {
                        calender += '<div class="ui-datepicker-group ui-datepicker-group-';
                        switch (col) {
                            case 0: calender += 'first'; cornerClass = ' ui-corner-' + (isRTL ? 'right' : 'left'); break;
                            case numMonths[1] - 1: calender += 'last'; cornerClass = ' ui-corner-' + (isRTL ? 'left' : 'right'); break;
                            default: calender += 'middle'; cornerClass = ''; break;
                        }
                        calender += '">';
                    }
                    calender += '<div class="ui-datepicker-header ui-widget-header ui-helper-clearfix' + cornerClass + '">' +
					(/all|left/.test(cornerClass) && row == 0 ? (isRTL ? next : prev) : '') +
					(/all|right/.test(cornerClass) && row == 0 ? (isRTL ? prev : next) : '') +
					this._generateMonthYearHeader(inst, drawMonth, drawYear, minDate, maxDate,
					selectedDate, row > 0 || col > 0, monthNames, monthNamesShort) + // draw month headers
					'</div><table class="ui-datepicker-calendar"><thead>' +
					'<tr>';
                    var thead = '';
                    for (var dow = 0; dow < 7; dow++) { // days of the week
                        var day = (dow + firstDay) % 7;
                        thead += '<th' + ((dow + firstDay + 6) % 7 >= 5 ? ' class="ui-datepicker-week-end"' : '') + '>' +
						'<span title="' + dayNames[day] + '">' + dayNamesMin[day] + '</span></th>';
                    }
                    calender += thead + '</tr></thead><tbody>';
                    var daysInMonth = this._getDaysInMonth(drawYear, drawMonth);
                    if (drawYear == inst.selectedYear && drawMonth == inst.selectedMonth)
                        inst.selectedDay = Math.min(inst.selectedDay, daysInMonth);
                    var leadDays = (this._getFirstDayOfMonth(drawYear, drawMonth) - firstDay + 7) % 7;
                    var numRows = (isMultiMonth ? 6 : Math.ceil((leadDays + daysInMonth) / 7)); // calculate the number of rows to generate
                    var printDate = this._daylightSavingAdjust(new Date(drawYear, drawMonth, 1 - leadDays));
                    for (var dRow = 0; dRow < numRows; dRow++) { // create date picker rows
                        calender += '<tr>';
                        var tbody = '';
                        for (var dow = 0; dow < 7; dow++) { // create date picker days
                            var daySettings = (beforeShowDay ?
							beforeShowDay.apply((inst.input ? inst.input[0] : null), [printDate]) : [true, '']);
                            var otherMonth = (printDate.getMonth() != drawMonth);
                            var unselectable = otherMonth || !daySettings[0] ||
							(minDate && printDate < minDate) || (maxDate && printDate > maxDate);
                            tbody += '<td class="' +
							((dow + firstDay + 6) % 7 >= 5 ? ' ui-datepicker-week-end' : '') + // highlight weekends
							(otherMonth ? ' ui-datepicker-other-month' : '') + // highlight days from other months
							((printDate.getTime() == selectedDate.getTime() && drawMonth == inst.selectedMonth && inst._keyEvent) || // user pressed key
							(defaultDate.getTime() == printDate.getTime() && defaultDate.getTime() == selectedDate.getTime()) ?
                            // or defaultDate is current printedDate and defaultDate is selectedDate
							' ' + this._dayOverClass : '') + // highlight selected day
							(unselectable ? ' ' + this._unselectableClass + ' ui-state-disabled' : '') +  // highlight unselectable days
							(otherMonth && !showOtherMonths ? '' : ' ' + daySettings[1] + // highlight custom dates
							(printDate.getTime() >= currentDate.getTime() && printDate.getTime() <= endDate.getTime() ? // in current range
							' ' + this._currentClass : '') + // highlight selected day
							(printDate.getTime() == today.getTime() ? ' ui-datepicker-today' : '')) + '"' + // highlight today (if different)
							((!otherMonth || showOtherMonths) && daySettings[2] ? ' title="' + daySettings[2] + '"' : '') + // cell title
							(unselectable ? '' : ' onclick="DP_jQuery.datepicker._selectDay(\'#' +
							inst.id + '\',' + drawMonth + ',' + drawYear + ', this);return false;"') + '>' + // actions
							(otherMonth ? (showOtherMonths ? printDate.getDate() : '&#xa0;') : // display for other months
							(unselectable ? '<span class="ui-state-default">' + printDate.getDate() + '</span>' : '<a class="ui-state-default' +
							(printDate.getTime() == today.getTime() ? ' ui-state-highlight' : '') +
							(printDate.getTime() >= currentDate.getTime() && printDate.getTime() <= endDate.getTime() ? // in current range
							' ui-state-active' : '') + // highlight selected day
							'" href="#">' + printDate.getDate() + '</a>')) + '</td>'; // display for this month
                            printDate.setDate(printDate.getDate() + 1);
                            printDate = this._daylightSavingAdjust(printDate);
                        }
                        calender += tbody + '</tr>';
                    }
                    drawMonth++;
                    if (drawMonth > 11) {
                        drawMonth = 0;
                        drawYear++;
                    }
                    calender += '</tbody></table>' + (isMultiMonth ? '</div>' +
							((numMonths[0] > 0 && col == numMonths[1] - 1) ? '<div class="ui-datepicker-row-break"></div>' : '') : '');
                    group += calender;
                }
                html += group;
            }
            html += buttonPanel + ($.browser.msie && parseInt($.browser.version, 10) < 7 && !inst.inline ?
			'<iframe src="javascript:false;" class="ui-datepicker-cover" frameborder="0"></iframe>' : '');
            inst._keyEvent = false;
            return html;
        },

        /* Generate the month and year header. */
        _generateMonthYearHeader: function (inst, drawMonth, drawYear, minDate, maxDate,
			selectedDate, secondary, monthNames, monthNamesShort) {
            minDate = (inst.rangeStart && minDate && selectedDate < minDate ? selectedDate : minDate);
            var changeMonth = this._get(inst, 'changeMonth');
            var changeYear = this._get(inst, 'changeYear');
            var showMonthAfterYear = this._get(inst, 'showMonthAfterYear');
            var html = '<div class="ui-datepicker-title">';
            var monthHtml = '';
            // month selection
            if (secondary || !changeMonth)
                monthHtml += '<span class="ui-datepicker-month">' + monthNames[drawMonth] + '</span> ';
            else {
                var inMinYear = (minDate && minDate.getFullYear() == drawYear);
                var inMaxYear = (maxDate && maxDate.getFullYear() == drawYear);
                monthHtml += '<select class="ui-datepicker-month" ' +
				'onchange="DP_jQuery.datepicker._selectMonthYear(\'#' + inst.id + '\', this, \'M\');" ' +
				'onclick="DP_jQuery.datepicker._clickMonthYear(\'#' + inst.id + '\');"' +
			 	'>';
                for (var month = 0; month < 12; month++) {
                    if ((!inMinYear || month >= minDate.getMonth()) &&
						(!inMaxYear || month <= maxDate.getMonth()))
                        monthHtml += '<option value="' + month + '"' +
						(month == drawMonth ? ' selected="selected"' : '') +
						'>' + monthNamesShort[month] + '</option>';
                }
                monthHtml += '</select>';
            }
            if (!showMonthAfterYear)
                html += monthHtml + ((secondary || changeMonth || changeYear) && (!(changeMonth && changeYear)) ? '&#xa0;' : '');
            // year selection
            if (secondary || !changeYear)
                html += '<span class="ui-datepicker-year">' + drawYear + '</span>';
            else {
                // determine range of years to display
                var years = this._get(inst, 'yearRange').split(':');
                var year = 0;
                var endYear = 0;
                if (years.length != 2) {
                    year = drawYear - 10;
                    endYear = drawYear + 10;
                } else if (years[0].charAt(0) == '+' || years[0].charAt(0) == '-') {
                    year = drawYear + parseInt(years[0], 10);
                    endYear = drawYear + parseInt(years[1], 10);
                } else {
                    year = parseInt(years[0], 10);
                    endYear = parseInt(years[1], 10);
                }
                year = (minDate ? Math.max(year, minDate.getFullYear()) : year);
                endYear = (maxDate ? Math.min(endYear, maxDate.getFullYear()) : endYear);
                html += '<select class="ui-datepicker-year" ' +
				'onchange="DP_jQuery.datepicker._selectMonthYear(\'#' + inst.id + '\', this, \'Y\');" ' +
				'onclick="DP_jQuery.datepicker._clickMonthYear(\'#' + inst.id + '\');"' +
				'>';
                for (; year <= endYear; year++) {
                    html += '<option value="' + year + '"' +
					(year == drawYear ? ' selected="selected"' : '') +
					'>' + year + '</option>';
                }
                html += '</select>';
            }
            if (showMonthAfterYear)
                html += (secondary || changeMonth || changeYear ? '&#xa0;' : '') + monthHtml;
            html += '</div>'; // Close datepicker_header
            return html;
        },

        /* Adjust one of the date sub-fields. */
        _adjustInstDate: function (inst, offset, period) {
            var year = inst.drawYear + (period == 'Y' ? offset : 0);
            var month = inst.drawMonth + (period == 'M' ? offset : 0);
            var day = Math.min(inst.selectedDay, this._getDaysInMonth(year, month)) +
			(period == 'D' ? offset : 0);
            var date = this._daylightSavingAdjust(new Date(year, month, day));
            // ensure it is within the bounds set
            var minDate = this._getMinMaxDate(inst, 'min', true);
            var maxDate = this._getMinMaxDate(inst, 'max');
            date = (minDate && date < minDate ? minDate : date);
            date = (maxDate && date > maxDate ? maxDate : date);
            inst.selectedDay = date.getDate();
            inst.drawMonth = inst.selectedMonth = date.getMonth();
            inst.drawYear = inst.selectedYear = date.getFullYear();
            if (period == 'M' || period == 'Y')
                this._notifyChange(inst);
        },

        /* Notify change of month/year. */
        _notifyChange: function (inst) {
            var onChange = this._get(inst, 'onChangeMonthYear');
            if (onChange)
                onChange.apply((inst.input ? inst.input[0] : null),
				[inst.selectedYear, inst.selectedMonth + 1, inst]);
        },

        /* Determine the number of months to show. */
        _getNumberOfMonths: function (inst) {
            var numMonths = this._get(inst, 'numberOfMonths');
            return (numMonths == null ? [1, 1] : (typeof numMonths == 'number' ? [1, numMonths] : numMonths));
        },

        /* Determine the current maximum date - ensure no time components are set - may be overridden for a range. */
        _getMinMaxDate: function (inst, minMax, checkRange) {
            var date = this._determineDate(this._get(inst, minMax + 'Date'), null);
            return (!checkRange || !inst.rangeStart ? date :
			(!date || inst.rangeStart > date ? inst.rangeStart : date));
        },

        /* Find the number of days in a given month. */
        _getDaysInMonth: function (year, month) {
            return 32 - new Date(year, month, 32).getDate();
        },

        /* Find the day of the week of the first of a month. */
        _getFirstDayOfMonth: function (year, month) {
            return new Date(year, month, 1).getDay();
        },

        /* Determines if we should allow a "next/prev" month display change. */
        _canAdjustMonth: function (inst, offset, curYear, curMonth) {
            var numMonths = this._getNumberOfMonths(inst);
            var date = this._daylightSavingAdjust(new Date(
			curYear, curMonth + (offset < 0 ? offset : numMonths[1]), 1));
            if (offset < 0)
                date.setDate(this._getDaysInMonth(date.getFullYear(), date.getMonth()));
            return this._isInRange(inst, date);
        },

        /* Is the given date in the accepted range? */
        _isInRange: function (inst, date) {
            // during range selection, use minimum of selected date and range start
            var newMinDate = (!inst.rangeStart ? null : this._daylightSavingAdjust(
			new Date(inst.selectedYear, inst.selectedMonth, inst.selectedDay)));
            newMinDate = (newMinDate && inst.rangeStart < newMinDate ? inst.rangeStart : newMinDate);
            var minDate = newMinDate || this._getMinMaxDate(inst, 'min');
            var maxDate = this._getMinMaxDate(inst, 'max');
            return ((!minDate || date >= minDate) && (!maxDate || date <= maxDate));
        },

        /* Provide the configuration settings for formatting/parsing. */
        _getFormatConfig: function (inst) {
            var shortYearCutoff = this._get(inst, 'shortYearCutoff');
            shortYearCutoff = (typeof shortYearCutoff != 'string' ? shortYearCutoff :
			new Date().getFullYear() % 100 + parseInt(shortYearCutoff, 10));
            return { shortYearCutoff: shortYearCutoff,
                dayNamesShort: this._get(inst, 'dayNamesShort'), dayNames: this._get(inst, 'dayNames'),
                monthNamesShort: this._get(inst, 'monthNamesShort'), monthNames: this._get(inst, 'monthNames')
            };
        },

        /* Format the given date for display. */
        _formatDate: function (inst, day, month, year) {
            if (!day) {
                inst.currentDay = inst.selectedDay;
                inst.currentMonth = inst.selectedMonth;
                inst.currentYear = inst.selectedYear;
            }
            var date = (day ? (typeof day == 'object' ? day :
			this._daylightSavingAdjust(new Date(year, month, day))) :
			this._daylightSavingAdjust(new Date(inst.currentYear, inst.currentMonth, inst.currentDay)));
            return this.formatDate(this._get(inst, 'dateFormat'), date, this._getFormatConfig(inst));
        }
    });

    /* jQuery extend now ignores nulls! */
    function extendRemove(target, props) {
        $.extend(target, props);
        for (var name in props)
            if (props[name] == null || props[name] == undefined)
                target[name] = props[name];
        return target;
    };

    /* Determine whether an object is an array. */
    function isArray(a) {
        return (a && (($.browser.safari && typeof a == 'object' && a.length) ||
		(a.constructor && a.constructor.toString().match(/\Array\(\)/))));
    };

    /* Invoke the datepicker functionality.
    @param  options  string - a command, optionally followed by additional parameters or
    Object - settings for attaching new datepicker functionality
    @return  jQuery object */
    $.fn.datepicker = function (options) {

        /* Initialise the date picker. */
        if (!$.datepicker.initialized) {
            $(document).mousedown($.datepicker._checkExternalClick).
			find('body').append($.datepicker.dpDiv);
            $.datepicker.initialized = true;
        }

        var otherArgs = Array.prototype.slice.call(arguments, 1);
        if (typeof options == 'string' && (options == 'isDisabled' || options == 'getDate'))
            return $.datepicker['_' + options + 'Datepicker'].
			apply($.datepicker, [this[0]].concat(otherArgs));
        if (options == 'option' && arguments.length == 2 && typeof arguments[1] == 'string')
            return $.datepicker['_' + options + 'Datepicker'].
			apply($.datepicker, [this[0]].concat(otherArgs));
        return this.each(function () {
            typeof options == 'string' ?
			$.datepicker['_' + options + 'Datepicker'].
				apply($.datepicker, [this].concat(otherArgs)) :
			$.datepicker._attachDatepicker(this, options);
        });
    };

    $.datepicker = new Datepicker(); // singleton instance
    $.datepicker.initialized = false;
    $.datepicker.uuid = new Date().getTime();
    $.datepicker.version = "1.7.3";

    // Workaround for #4055
    // Add another global to avoid noConflict issues with inline event handlers
    window.DP_jQuery = $;

})(jQuery);


/*
 * jQuery UI Slider 1.7.3
 *
 * Copyright (c) 2009 AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT (MIT-LICENSE.txt)
 * and GPL (GPL-LICENSE.txt) licenses.
 *
 * http://docs.jquery.com/UI/Slider
 *
 * Depends:
 *	ui.core.js
 */

(function($) {

$.widget("ui.slider", $.extend({}, $.ui.mouse, {

	_init: function() {

		var self = this, o = this.options;
		this._keySliding = false;
		this._handleIndex = null;
		this._detectOrientation();
		this._mouseInit();

		this.element
			.addClass("ui-slider"
				+ " ui-slider-" + this.orientation
				+ " ui-widget"
				+ " ui-widget-content"
				+ " ui-corner-all");

		this.range = $([]);

		if (o.range) {

			if (o.range === true) {
				this.range = $('<div></div>');
				if (!o.values) o.values = [this._valueMin(), this._valueMin()];
				if (o.values.length && o.values.length != 2) {
					o.values = [o.values[0], o.values[0]];
				}
			} else {
				this.range = $('<div></div>');
			}

			this.range
				.appendTo(this.element)
				.addClass("ui-slider-range");

			if (o.range == "min" || o.range == "max") {
				this.range.addClass("ui-slider-range-" + o.range);
			}

			// note: this isn't the most fittingly semantic framework class for this element,
			// but worked best visually with a variety of themes
			this.range.addClass("ui-widget-header");

		}

		if ($(".ui-slider-handle", this.element).length == 0)
			$('<a href="#"></a>')
				.appendTo(this.element)
				.addClass("ui-slider-handle");

		if (o.values && o.values.length) {
			while ($(".ui-slider-handle", this.element).length < o.values.length)
				$('<a href="#"></a>')
					.appendTo(this.element)
					.addClass("ui-slider-handle");
		}

		this.handles = $(".ui-slider-handle", this.element)
			.addClass("ui-state-default"
				+ " ui-corner-all");

		this.handle = this.handles.eq(0);

		this.handles.add(this.range).filter("a")
			.click(function(event) {
				event.preventDefault();
			})
			.hover(function() {
				if (!o.disabled) {
					$(this).addClass('ui-state-hover');
				}
			}, function() {
				$(this).removeClass('ui-state-hover');
			})
			.focus(function() {
				if (!o.disabled) {
					$(".ui-slider .ui-state-focus").removeClass('ui-state-focus'); $(this).addClass('ui-state-focus');
				} else {
					$(this).blur();
				}
			})
			.blur(function() {
				$(this).removeClass('ui-state-focus');
			});

		this.handles.each(function(i) {
			$(this).data("index.ui-slider-handle", i);
		});

		this.handles.keydown(function(event) {

			var ret = true;

			var index = $(this).data("index.ui-slider-handle");

			if (self.options.disabled)
				return;

			switch (event.keyCode) {
				case $.ui.keyCode.HOME:
				case $.ui.keyCode.END:
				case $.ui.keyCode.UP:
				case $.ui.keyCode.RIGHT:
				case $.ui.keyCode.DOWN:
				case $.ui.keyCode.LEFT:
					ret = false;
					if (!self._keySliding) {
						self._keySliding = true;
						$(this).addClass("ui-state-active");
						self._start(event, index);
					}
					break;
			}

			var curVal, newVal, step = self._step();
			if (self.options.values && self.options.values.length) {
				curVal = newVal = self.values(index);
			} else {
				curVal = newVal = self.value();
			}

			switch (event.keyCode) {
				case $.ui.keyCode.HOME:
					newVal = self._valueMin();
					break;
				case $.ui.keyCode.END:
					newVal = self._valueMax();
					break;
				case $.ui.keyCode.UP:
				case $.ui.keyCode.RIGHT:
					if(curVal == self._valueMax()) return;
					newVal = curVal + step;
					break;
				case $.ui.keyCode.DOWN:
				case $.ui.keyCode.LEFT:
					if(curVal == self._valueMin()) return;
					newVal = curVal - step;
					break;
			}

			self._slide(event, index, newVal);

			return ret;

		}).keyup(function(event) {

			var index = $(this).data("index.ui-slider-handle");

			if (self._keySliding) {
				self._stop(event, index);
				self._change(event, index);
				self._keySliding = false;
				$(this).removeClass("ui-state-active");
			}

		});

		this._refreshValue();

	},

	destroy: function() {

		this.handles.remove();
		this.range.remove();

		this.element
			.removeClass("ui-slider"
				+ " ui-slider-horizontal"
				+ " ui-slider-vertical"
				+ " ui-slider-disabled"
				+ " ui-widget"
				+ " ui-widget-content"
				+ " ui-corner-all")
			.removeData("slider")
			.unbind(".slider");

		this._mouseDestroy();

	},

	_mouseCapture: function(event) {

		var o = this.options;

		if (o.disabled)
			return false;

		this.elementSize = {
			width: this.element.outerWidth(),
			height: this.element.outerHeight()
		};
		this.elementOffset = this.element.offset();

		var position = { x: event.pageX, y: event.pageY };
		var normValue = this._normValueFromMouse(position);

		var distance = this._valueMax() - this._valueMin() + 1, closestHandle;
		var self = this, index;
		this.handles.each(function(i) {
			var thisDistance = Math.abs(normValue - self.values(i));
			if (distance > thisDistance) {
				distance = thisDistance;
				closestHandle = $(this);
				index = i;
			}
		});

		// workaround for bug #3736 (if both handles of a range are at 0,
		// the first is always used as the one with least distance,
		// and moving it is obviously prevented by preventing negative ranges)
		if(o.range == true && this.values(1) == o.min) {
			closestHandle = $(this.handles[++index]);
		}

		this._start(event, index);

		self._handleIndex = index;

		closestHandle
			.addClass("ui-state-active")
			.focus();
		
		var offset = closestHandle.offset();
		var mouseOverHandle = !$(event.target).parents().andSelf().is('.ui-slider-handle');
		this._clickOffset = mouseOverHandle ? { left: 0, top: 0 } : {
			left: event.pageX - offset.left - (closestHandle.width() / 2),
			top: event.pageY - offset.top
				- (closestHandle.height() / 2)
				- (parseInt(closestHandle.css('borderTopWidth'),10) || 0)
				- (parseInt(closestHandle.css('borderBottomWidth'),10) || 0)
				+ (parseInt(closestHandle.css('marginTop'),10) || 0)
		};

		normValue = this._normValueFromMouse(position);
		this._slide(event, index, normValue);
		return true;

	},

	_mouseStart: function(event) {
		return true;
	},

	_mouseDrag: function(event) {

		var position = { x: event.pageX, y: event.pageY };
		var normValue = this._normValueFromMouse(position);
		
		this._slide(event, this._handleIndex, normValue);

		return false;

	},

	_mouseStop: function(event) {

		this.handles.removeClass("ui-state-active");
		this._stop(event, this._handleIndex);
		this._change(event, this._handleIndex);
		this._handleIndex = null;
		this._clickOffset = null;

		return false;

	},
	
	_detectOrientation: function() {
		this.orientation = this.options.orientation == 'vertical' ? 'vertical' : 'horizontal';
	},

	_normValueFromMouse: function(position) {

		var pixelTotal, pixelMouse;
		if ('horizontal' == this.orientation) {
			pixelTotal = this.elementSize.width;
			pixelMouse = position.x - this.elementOffset.left - (this._clickOffset ? this._clickOffset.left : 0);
		} else {
			pixelTotal = this.elementSize.height;
			pixelMouse = position.y - this.elementOffset.top - (this._clickOffset ? this._clickOffset.top : 0);
		}

		var percentMouse = (pixelMouse / pixelTotal);
		if (percentMouse > 1) percentMouse = 1;
		if (percentMouse < 0) percentMouse = 0;
		if ('vertical' == this.orientation)
			percentMouse = 1 - percentMouse;

		var valueTotal = this._valueMax() - this._valueMin(),
			valueMouse = percentMouse * valueTotal,
			valueMouseModStep = valueMouse % this.options.step,
			normValue = this._valueMin() + valueMouse - valueMouseModStep;

		if (valueMouseModStep > (this.options.step / 2))
			normValue += this.options.step;

		// Since JavaScript has problems with large floats, round
		// the final value to 5 digits after the decimal point (see #4124)
		return parseFloat(normValue.toFixed(5));

	},

	_start: function(event, index) {
		var uiHash = {
			handle: this.handles[index],
			value: this.value()
		};
		if (this.options.values && this.options.values.length) {
			uiHash.value = this.values(index);
			uiHash.values = this.values();
		}
		this._trigger("start", event, uiHash);
	},

	_slide: function(event, index, newVal) {

		var handle = this.handles[index];

		if (this.options.values && this.options.values.length) {

			var otherVal = this.values(index ? 0 : 1);

			if ((this.options.values.length == 2 && this.options.range === true) && 
				((index == 0 && newVal > otherVal) || (index == 1 && newVal < otherVal))){
 				newVal = otherVal;
			}

			if (newVal != this.values(index)) {
				var newValues = this.values();
				newValues[index] = newVal;
				// A slide can be canceled by returning false from the slide callback
				var allowed = this._trigger("slide", event, {
					handle: this.handles[index],
					value: newVal,
					values: newValues
				});
				var otherVal = this.values(index ? 0 : 1);
				if (allowed !== false) {
					this.values(index, newVal, ( event.type == 'mousedown' && this.options.animate ), true);
				}
			}

		} else {

			if (newVal != this.value()) {
				// A slide can be canceled by returning false from the slide callback
				var allowed = this._trigger("slide", event, {
					handle: this.handles[index],
					value: newVal
				});
				if (allowed !== false) {
					this._setData('value', newVal, ( event.type == 'mousedown' && this.options.animate ));
				}
					
			}

		}

	},

	_stop: function(event, index) {
		var uiHash = {
			handle: this.handles[index],
			value: this.value()
		};
		if (this.options.values && this.options.values.length) {
			uiHash.value = this.values(index);
			uiHash.values = this.values();
		}
		this._trigger("stop", event, uiHash);
	},

	_change: function(event, index) {
		var uiHash = {
			handle: this.handles[index],
			value: this.value()
		};
		if (this.options.values && this.options.values.length) {
			uiHash.value = this.values(index);
			uiHash.values = this.values();
		}
		this._trigger("change", event, uiHash);
	},

	value: function(newValue) {

		if (arguments.length) {
			this._setData("value", newValue);
			this._change(null, 0);
		}

		return this._value();

	},

	values: function(index, newValue, animated, noPropagation) {

		if (arguments.length > 1) {
			this.options.values[index] = newValue;
			this._refreshValue(animated);
			if(!noPropagation) this._change(null, index);
		}

		if (arguments.length) {
			if (this.options.values && this.options.values.length) {
				return this._values(index);
			} else {
				return this.value();
			}
		} else {
			return this._values();
		}

	},

	_setData: function(key, value, animated) {

		$.widget.prototype._setData.apply(this, arguments);

		switch (key) {
			case 'disabled':
				if (value) {
					this.handles.filter(".ui-state-focus").blur();
					this.handles.removeClass("ui-state-hover");
					this.handles.attr("disabled", "disabled");
				} else {
					this.handles.removeAttr("disabled");
				}
			case 'orientation':

				this._detectOrientation();
				
				this.element
					.removeClass("ui-slider-horizontal ui-slider-vertical")
					.addClass("ui-slider-" + this.orientation);
				this._refreshValue(animated);
				break;
			case 'value':
				this._refreshValue(animated);
				break;
		}

	},

	_step: function() {
		var step = this.options.step;
		return step;
	},

	_value: function() {

		var val = this.options.value;
		if (val < this._valueMin()) val = this._valueMin();
		if (val > this._valueMax()) val = this._valueMax();

		return val;

	},

	_values: function(index) {

		if (arguments.length) {
			var val = this.options.values[index];
			if (val < this._valueMin()) val = this._valueMin();
			if (val > this._valueMax()) val = this._valueMax();

			return val;
		} else {
			return this.options.values;
		}

	},

	_valueMin: function() {
		var valueMin = this.options.min;
		return valueMin;
	},

	_valueMax: function() {
		var valueMax = this.options.max;
		return valueMax;
	},

	_refreshValue: function(animate) {

		var oRange = this.options.range, o = this.options, self = this;

		if (this.options.values && this.options.values.length) {
			var vp0, vp1;
			this.handles.each(function(i, j) {
				var valPercent = (self.values(i) - self._valueMin()) / (self._valueMax() - self._valueMin()) * 100;
				var _set = {}; _set[self.orientation == 'horizontal' ? 'left' : 'bottom'] = valPercent + '%';
				$(this).stop(1,1)[animate ? 'animate' : 'css'](_set, o.animate);
				if (self.options.range === true) {
					if (self.orientation == 'horizontal') {
						(i == 0) && self.range.stop(1,1)[animate ? 'animate' : 'css']({ left: valPercent + '%' }, o.animate);
						(i == 1) && self.range[animate ? 'animate' : 'css']({ width: (valPercent - lastValPercent) + '%' }, { queue: false, duration: o.animate });
					} else {
						(i == 0) && self.range.stop(1,1)[animate ? 'animate' : 'css']({ bottom: (valPercent) + '%' }, o.animate);
						(i == 1) && self.range[animate ? 'animate' : 'css']({ height: (valPercent - lastValPercent) + '%' }, { queue: false, duration: o.animate });
					}
				}
				lastValPercent = valPercent;
			});
		} else {
			var value = this.value(),
				valueMin = this._valueMin(),
				valueMax = this._valueMax(),
				valPercent = valueMax != valueMin
					? (value - valueMin) / (valueMax - valueMin) * 100
					: 0;
			var _set = {}; _set[self.orientation == 'horizontal' ? 'left' : 'bottom'] = valPercent + '%';
			this.handle.stop(1,1)[animate ? 'animate' : 'css'](_set, o.animate);

			(oRange == "min") && (this.orientation == "horizontal") && this.range.stop(1,1)[animate ? 'animate' : 'css']({ width: valPercent + '%' }, o.animate);
			(oRange == "max") && (this.orientation == "horizontal") && this.range[animate ? 'animate' : 'css']({ width: (100 - valPercent) + '%' }, { queue: false, duration: o.animate });
			(oRange == "min") && (this.orientation == "vertical") && this.range.stop(1,1)[animate ? 'animate' : 'css']({ height: valPercent + '%' }, o.animate);
			(oRange == "max") && (this.orientation == "vertical") && this.range[animate ? 'animate' : 'css']({ height: (100 - valPercent) + '%' }, { queue: false, duration: o.animate });
		}

	}
	
}));

$.extend($.ui.slider, {
	getter: "value values",
	version: "1.7.3",
	eventPrefix: "slide",
	defaults: {
		animate: false,
		delay: 0,
		distance: 0,
		max: 100,
		min: 0,
		orientation: 'horizontal',
		range: false,
		step: 1,
		value: 0,
		values: null
	}
});

})(jQuery);

/*
http://trentrichardson.com/examples/timepicker/js/jquery-ui-timepicker-addon.js
* jQuery timepicker addon
* By: Trent Richardson [http://trentrichardson.com]
* Version 0.9.9
* Last Modified: 02/05/2012
* 
* Copyright 2012 Trent Richardson
* Dual licensed under the MIT and GPL licenses.
* http://trentrichardson.com/Impromptu/GPL-LICENSE.txt
* http://trentrichardson.com/Impromptu/MIT-LICENSE.txt
* 
* HERES THE CSS:
* .ui-timepicker-div .ui-widget-header { margin-bottom: 8px; }
* .ui-timepicker-div dl { text-align: left; }
* .ui-timepicker-div dl dt { height: 25px; margin-bottom: -25px; }
* .ui-timepicker-div dl dd { margin: 0 10px 10px 65px; }
* .ui-timepicker-div td { font-size: 90%; }
* .ui-tpicker-grid-label { background: none; border: none; margin: 0; padding: 0; }
*/

(function ($) {

    $.extend($.ui, { timepicker: { version: "0.9.9"} });

    /* Time picker manager.
    Use the singleton instance of this class, $.timepicker, to interact with the time picker.
    Settings for (groups of) time pickers are maintained in an instance object,
    allowing multiple different settings on the same page. */

    function Timepicker() {
        this.regional = []; // Available regional settings, indexed by language code
        this.regional[''] = { // Default regional settings
            currentText: 'Now',
            closeText: 'Done',
            ampm: false,
            amNames: ['AM', 'A'],
            pmNames: ['PM', 'P'],
            timeFormat: 'hh:mm tt',
            timeSuffix: '',
            timeOnlyTitle: 'Choose Time',
            timeText: 'Time',
            hourText: 'Hour',
            minuteText: 'Minute',
            secondText: 'Second',
            millisecText: 'Millisecond',
            timezoneText: 'Time Zone'
        };
        this._defaults = { // Global defaults for all the datetime picker instances
            showButtonPanel: true,
            timeOnly: false,
            showHour: true,
            showMinute: true,
            showSecond: false,
            showMillisec: false,
            showTimezone: false,
            showTime: true,
            stepHour: 1,
            stepMinute: 1,
            stepSecond: 1,
            stepMillisec: 1,
            hour: 0,
            minute: 0,
            second: 0,
            millisec: 0,
            timezone: '+0000',
            hourMin: 0,
            minuteMin: 0,
            secondMin: 0,
            millisecMin: 0,
            hourMax: 23,
            minuteMax: 59,
            secondMax: 59,
            millisecMax: 999,
            minDateTime: null,
            maxDateTime: null,
            onSelect: null,
            hourGrid: 0,
            minuteGrid: 0,
            secondGrid: 0,
            millisecGrid: 0,
            alwaysSetTime: true,
            separator: ' ',
            altFieldTimeOnly: true,
            showTimepicker: true,
            timezoneIso8609: false,
            timezoneList: null,
            addSliderAccess: false,
            sliderAccessArgs: null
        };
        $.extend(this._defaults, this.regional['']);
    };

    $.extend(Timepicker.prototype, {
        $input: null,
        $altInput: null,
        $timeObj: null,
        inst: null,
        hour_slider: null,
        minute_slider: null,
        second_slider: null,
        millisec_slider: null,
        timezone_select: null,
        hour: 0,
        minute: 0,
        second: 0,
        millisec: 0,
        timezone: '+0000',
        hourMinOriginal: null,
        minuteMinOriginal: null,
        secondMinOriginal: null,
        millisecMinOriginal: null,
        hourMaxOriginal: null,
        minuteMaxOriginal: null,
        secondMaxOriginal: null,
        millisecMaxOriginal: null,
        ampm: '',
        formattedDate: '',
        formattedTime: '',
        formattedDateTime: '',
        timezoneList: null,

        /* Override the default settings for all instances of the time picker.
        @param  settings  object - the new settings to use as defaults (anonymous object)
        @return the manager object */
        setDefaults: function (settings) {
            extendRemove(this._defaults, settings || {});
            return this;
        },

        //########################################################################
        // Create a new Timepicker instance
        //########################################################################
        _newInst: function ($input, o) {
            var tp_inst = new Timepicker(),
			inlineSettings = {};

            for (var attrName in this._defaults) {
                var attrValue = $input.attr('time:' + attrName);
                if (attrValue) {
                    try {
                        inlineSettings[attrName] = eval(attrValue);
                    } catch (err) {
                        inlineSettings[attrName] = attrValue;
                    }
                }
            }
            tp_inst._defaults = $.extend({}, this._defaults, inlineSettings, o, {
                beforeShow: function (input, dp_inst) {
                    if ($.isFunction(o.beforeShow))
                        return o.beforeShow(input, dp_inst, tp_inst);
                },
                onChangeMonthYear: function (year, month, dp_inst) {
                    // Update the time as well : this prevents the time from disappearing from the $input field.
                    tp_inst._updateDateTime(dp_inst);
                    if ($.isFunction(o.onChangeMonthYear))
                        o.onChangeMonthYear.call($input[0], year, month, dp_inst, tp_inst);
                },
                onClose: function (dateText, dp_inst) {
                    if (tp_inst.timeDefined === true && $input.val() != '')
                        tp_inst._updateDateTime(dp_inst);
                    if ($.isFunction(o.onClose))
                        o.onClose.call($input[0], dateText, dp_inst, tp_inst);
                },
                timepicker: tp_inst // add timepicker as a property of datepicker: $.datepicker._get(dp_inst, 'timepicker');
            });
            tp_inst.amNames = $.map(tp_inst._defaults.amNames, function (val) { return val.toUpperCase() });
            tp_inst.pmNames = $.map(tp_inst._defaults.pmNames, function (val) { return val.toUpperCase() });

            if (tp_inst._defaults.timezoneList === null) {
                var timezoneList = [];
                for (var i = -11; i <= 12; i++)
                    timezoneList.push((i >= 0 ? '+' : '-') + ('0' + Math.abs(i).toString()).slice(-2) + '00');
                if (tp_inst._defaults.timezoneIso8609)
                    timezoneList = $.map(timezoneList, function (val) {
                        return val == '+0000' ? 'Z' : (val.substring(0, 3) + ':' + val.substring(3));
                    });
                tp_inst._defaults.timezoneList = timezoneList;
            }

            tp_inst.hour = tp_inst._defaults.hour;
            tp_inst.minute = tp_inst._defaults.minute;
            tp_inst.second = tp_inst._defaults.second;
            tp_inst.millisec = tp_inst._defaults.millisec;
            tp_inst.ampm = '';
            tp_inst.$input = $input;

            if (o.altField)
                tp_inst.$altInput = $(o.altField)
				.css({ cursor: 'pointer' })
				.focus(function () { $input.trigger("focus"); });

            if (tp_inst._defaults.minDate == 0 || tp_inst._defaults.minDateTime == 0) {
                tp_inst._defaults.minDate = new Date();
            }
            if (tp_inst._defaults.maxDate == 0 || tp_inst._defaults.maxDateTime == 0) {
                tp_inst._defaults.maxDate = new Date();
            }

            // datepicker needs minDate/maxDate, timepicker needs minDateTime/maxDateTime..
            if (tp_inst._defaults.minDate !== undefined && tp_inst._defaults.minDate instanceof Date)
                tp_inst._defaults.minDateTime = new Date(tp_inst._defaults.minDate.getTime());
            if (tp_inst._defaults.minDateTime !== undefined && tp_inst._defaults.minDateTime instanceof Date)
                tp_inst._defaults.minDate = new Date(tp_inst._defaults.minDateTime.getTime());
            if (tp_inst._defaults.maxDate !== undefined && tp_inst._defaults.maxDate instanceof Date)
                tp_inst._defaults.maxDateTime = new Date(tp_inst._defaults.maxDate.getTime());
            if (tp_inst._defaults.maxDateTime !== undefined && tp_inst._defaults.maxDateTime instanceof Date)
                tp_inst._defaults.maxDate = new Date(tp_inst._defaults.maxDateTime.getTime());
            return tp_inst;
        },

        //########################################################################
        // add our sliders to the calendar
        //########################################################################
        _addTimePicker: function (dp_inst) {
            var currDT = (this.$altInput && this._defaults.altFieldTimeOnly) ?
				this.$input.val() + ' ' + this.$altInput.val() :
				this.$input.val();

            this.timeDefined = this._parseTime(currDT);
            this._limitMinMaxDateTime(dp_inst, false);
            this._injectTimePicker();
        },

        //########################################################################
        // parse the time string from input value or _setTime
        //########################################################################
        _parseTime: function (timeString, withDate) {
            var regstr = this._defaults.timeFormat.toString()
				.replace(/h{1,2}/ig, '(\\d?\\d)')
				.replace(/m{1,2}/ig, '(\\d?\\d)')
				.replace(/s{1,2}/ig, '(\\d?\\d)')
				.replace(/l{1}/ig, '(\\d?\\d?\\d)')
				.replace(/t{1,2}/ig, this._getPatternAmpm())
				.replace(/z{1}/ig, '(z|[-+]\\d\\d:?\\d\\d)?')
				.replace(/\s/g, '\\s?') + this._defaults.timeSuffix + '$',
			order = this._getFormatPositions(),
			ampm = '',
			treg;

            if (!this.inst) this.inst = $.datepicker._getInst(this.$input[0]);

            if (withDate || !this._defaults.timeOnly) {
                // the time should come after x number of characters and a space.
                // x = at least the length of text specified by the date format
                var dp_dateFormat = $.datepicker._get(this.inst, 'dateFormat');
                // escape special regex characters in the seperator
                var specials = new RegExp("[.*+?|()\\[\\]{}\\\\]", "g");
                regstr = '^.{' + dp_dateFormat.length + ',}?' + this._defaults.separator.replace(specials, "\\$&") + regstr;
            }

            treg = timeString.match(new RegExp(regstr, 'i'));

            if (treg) {
                if (order.t !== -1) {
                    if (treg[order.t] === undefined || treg[order.t].length === 0) {
                        ampm = '';
                        this.ampm = '';
                    } else {
                        ampm = $.inArray(treg[order.t].toUpperCase(), this.amNames) !== -1 ? 'AM' : 'PM';
                        this.ampm = this._defaults[ampm == 'AM' ? 'amNames' : 'pmNames'][0];
                    }
                }

                if (order.h !== -1) {
                    if (ampm == 'AM' && treg[order.h] == '12')
                        this.hour = 0; // 12am = 0 hour
                    else if (ampm == 'PM' && treg[order.h] != '12')
                        this.hour = (parseFloat(treg[order.h]) + 12).toFixed(0); // 12pm = 12 hour, any other pm = hour + 12
                    else this.hour = Number(treg[order.h]);
                }

                if (order.m !== -1) this.minute = Number(treg[order.m]);
                if (order.s !== -1) this.second = Number(treg[order.s]);
                if (order.l !== -1) this.millisec = Number(treg[order.l]);
                if (order.z !== -1 && treg[order.z] !== undefined) {
                    var tz = treg[order.z].toUpperCase();
                    switch (tz.length) {
                        case 1: // Z
                            tz = this._defaults.timezoneIso8609 ? 'Z' : '+0000';
                            break;
                        case 5: // +hhmm
                            if (this._defaults.timezoneIso8609)
                                tz = tz.substring(1) == '0000'
						   ? 'Z'
						   : tz.substring(0, 3) + ':' + tz.substring(3);
                            break;
                        case 6: // +hh:mm
                            if (!this._defaults.timezoneIso8609)
                                tz = tz == 'Z' || tz.substring(1) == '00:00'
						   ? '+0000'
						   : tz.replace(/:/, '');
                            else if (tz.substring(1) == '00:00')
                                tz = 'Z';
                            break;
                    }
                    this.timezone = tz;
                }

                return true;

            }
            return false;
        },

        //########################################################################
        // pattern for standard and localized AM/PM markers
        //########################################################################
        _getPatternAmpm: function () {
            var markers = [];
            o = this._defaults;
            if (o.amNames)
                $.merge(markers, o.amNames);
            if (o.pmNames)
                $.merge(markers, o.pmNames);
            markers = $.map(markers, function (val) { return val.replace(/[.*+?|()\[\]{}\\]/g, '\\$&') });
            return '(' + markers.join('|') + ')?';
        },

        //########################################################################
        // figure out position of time elements.. cause js cant do named captures
        //########################################################################
        _getFormatPositions: function () {
            var finds = this._defaults.timeFormat.toLowerCase().match(/(h{1,2}|m{1,2}|s{1,2}|l{1}|t{1,2}|z)/g),
			orders = { h: -1, m: -1, s: -1, l: -1, t: -1, z: -1 };

            if (finds)
                for (var i = 0; i < finds.length; i++)
                    if (orders[finds[i].toString().charAt(0)] == -1)
                        orders[finds[i].toString().charAt(0)] = i + 1;

            return orders;
        },

        //########################################################################
        // generate and inject html for timepicker into ui datepicker
        //########################################################################
        _injectTimePicker: function () {
            var $dp = this.inst.dpDiv,
			o = this._defaults,
			tp_inst = this,
            // Added by Peter Medeiros:
            // - Figure out what the hour/minute/second max should be based on the step values.
            // - Example: if stepMinute is 15, then minMax is 45.
			hourMax = parseInt((o.hourMax - ((o.hourMax - o.hourMin) % o.stepHour)), 10),
			minMax = parseInt((o.minuteMax - ((o.minuteMax - o.minuteMin) % o.stepMinute)), 10),
			secMax = parseInt((o.secondMax - ((o.secondMax - o.secondMin) % o.stepSecond)), 10),
			millisecMax = parseInt((o.millisecMax - ((o.millisecMax - o.millisecMin) % o.stepMillisec)), 10),
			dp_id = this.inst.id.toString().replace(/([^A-Za-z0-9_])/g, '');

            // Prevent displaying twice
            //if ($dp.find("div#ui-timepicker-div-"+ dp_id).length === 0) {
            if ($dp.find("div#ui-timepicker-div-" + dp_id).length === 0 && o.showTimepicker) {
                var noDisplay = ' style="display:none;"',
				html = '<div class="ui-timepicker-div" id="ui-timepicker-div-' + dp_id + '"><dl>' +
						'<dt class="ui_tpicker_time_label" id="ui_tpicker_time_label_' + dp_id + '"' +
						((o.showTime) ? '' : noDisplay) + '>' + o.timeText + '</dt>' +
						'<dd class="ui_tpicker_time" id="ui_tpicker_time_' + dp_id + '"' +
						((o.showTime) ? '' : noDisplay) + '></dd>' +
						'<dt class="ui_tpicker_hour_label" id="ui_tpicker_hour_label_' + dp_id + '"' +
						((o.showHour) ? '' : noDisplay) + '>' + o.hourText + '</dt>',
				hourGridSize = 0,
				minuteGridSize = 0,
				secondGridSize = 0,
				millisecGridSize = 0,
				size;

                // Hours
                html += '<dd class="ui_tpicker_hour"><div id="ui_tpicker_hour_' + dp_id + '"' +
						((o.showHour) ? '' : noDisplay) + '></div>';
                if (o.showHour && o.hourGrid > 0) {
                    html += '<div style="padding-left: 1px"><table class="ui-tpicker-grid-label"><tr>';

                    for (var h = o.hourMin; h <= hourMax; h += parseInt(o.hourGrid, 10)) {
                        hourGridSize++;
                        var tmph = (o.ampm && h > 12) ? h - 12 : h;
                        if (tmph < 10) tmph = '0' + tmph;
                        if (o.ampm) {
                            if (h == 0) tmph = 12 + 'a';
                            else if (h < 12) tmph += 'a';
                            else tmph += 'p';
                        }
                        html += '<td>' + tmph + '</td>';
                    }

                    html += '</tr></table></div>';
                }
                html += '</dd>';

                // Minutes
                html += '<dt class="ui_tpicker_minute_label" id="ui_tpicker_minute_label_' + dp_id + '"' +
					((o.showMinute) ? '' : noDisplay) + '>' + o.minuteText + '</dt>' +
					'<dd class="ui_tpicker_minute"><div id="ui_tpicker_minute_' + dp_id + '"' +
							((o.showMinute) ? '' : noDisplay) + '></div>';

                if (o.showMinute && o.minuteGrid > 0) {
                    html += '<div style="padding-left: 1px"><table class="ui-tpicker-grid-label"><tr>';

                    for (var m = o.minuteMin; m <= minMax; m += parseInt(o.minuteGrid, 10)) {
                        minuteGridSize++;
                        html += '<td>' + ((m < 10) ? '0' : '') + m + '</td>';
                    }

                    html += '</tr></table></div>';
                }
                html += '</dd>';

                // Seconds
                html += '<dt class="ui_tpicker_second_label" id="ui_tpicker_second_label_' + dp_id + '"' +
					((o.showSecond) ? '' : noDisplay) + '>' + o.secondText + '</dt>' +
					'<dd class="ui_tpicker_second"><div id="ui_tpicker_second_' + dp_id + '"' +
							((o.showSecond) ? '' : noDisplay) + '></div>';

                if (o.showSecond && o.secondGrid > 0) {
                    html += '<div style="padding-left: 1px"><table><tr>';

                    for (var s = o.secondMin; s <= secMax; s += parseInt(o.secondGrid, 10)) {
                        secondGridSize++;
                        html += '<td>' + ((s < 10) ? '0' : '') + s + '</td>';
                    }

                    html += '</tr></table></div>';
                }
                html += '</dd>';

                // Milliseconds
                html += '<dt class="ui_tpicker_millisec_label" id="ui_tpicker_millisec_label_' + dp_id + '"' +
					((o.showMillisec) ? '' : noDisplay) + '>' + o.millisecText + '</dt>' +
					'<dd class="ui_tpicker_millisec"><div id="ui_tpicker_millisec_' + dp_id + '"' +
							((o.showMillisec) ? '' : noDisplay) + '></div>';

                if (o.showMillisec && o.millisecGrid > 0) {
                    html += '<div style="padding-left: 1px"><table><tr>';

                    for (var l = o.millisecMin; l <= millisecMax; l += parseInt(o.millisecGrid, 10)) {
                        millisecGridSize++;
                        html += '<td>' + ((l < 10) ? '0' : '') + l + '</td>';
                    }

                    html += '</tr></table></div>';
                }
                html += '</dd>';

                // Timezone
                html += '<dt class="ui_tpicker_timezone_label" id="ui_tpicker_timezone_label_' + dp_id + '"' +
					((o.showTimezone) ? '' : noDisplay) + '>' + o.timezoneText + '</dt>';
                html += '<dd class="ui_tpicker_timezone" id="ui_tpicker_timezone_' + dp_id + '"' +
							((o.showTimezone) ? '' : noDisplay) + '></dd>';

                html += '</dl></div>';
                $tp = $(html);

                // if we only want time picker...
                if (o.timeOnly === true) {
                    $tp.prepend(
					'<div class="ui-widget-header ui-helper-clearfix ui-corner-all">' +
						'<div class="ui-datepicker-title">' + o.timeOnlyTitle + '</div>' +
					'</div>');
                    $dp.find('.ui-datepicker-header, .ui-datepicker-calendar').hide();
                }

                this.hour_slider = $tp.find('#ui_tpicker_hour_' + dp_id).slider({
                    orientation: "horizontal",
                    value: this.hour,
                    min: o.hourMin,
                    max: hourMax,
                    step: o.stepHour,
                    slide: function (event, ui) {
                        tp_inst.hour_slider.slider("option", "value", ui.value);
                        tp_inst._onTimeChange();
                    }
                });


                // Updated by Peter Medeiros:
                // - Pass in Event and UI instance into slide function
                this.minute_slider = $tp.find('#ui_tpicker_minute_' + dp_id).slider({
                    orientation: "horizontal",
                    value: this.minute,
                    min: o.minuteMin,
                    max: minMax,
                    step: o.stepMinute,
                    slide: function (event, ui) {
                        tp_inst.minute_slider.slider("option", "value", ui.value);
                        tp_inst._onTimeChange();
                    }
                });

                this.second_slider = $tp.find('#ui_tpicker_second_' + dp_id).slider({
                    orientation: "horizontal",
                    value: this.second,
                    min: o.secondMin,
                    max: secMax,
                    step: o.stepSecond,
                    slide: function (event, ui) {
                        tp_inst.second_slider.slider("option", "value", ui.value);
                        tp_inst._onTimeChange();
                    }
                });

                this.millisec_slider = $tp.find('#ui_tpicker_millisec_' + dp_id).slider({
                    orientation: "horizontal",
                    value: this.millisec,
                    min: o.millisecMin,
                    max: millisecMax,
                    step: o.stepMillisec,
                    slide: function (event, ui) {
                        tp_inst.millisec_slider.slider("option", "value", ui.value);
                        tp_inst._onTimeChange();
                    }
                });

                this.timezone_select = $tp.find('#ui_tpicker_timezone_' + dp_id).append('<select></select>').find("select");
                $.fn.append.apply(this.timezone_select,
				$.map(o.timezoneList, function (val, idx) {
				    return $("<option />")
						.val(typeof val == "object" ? val.value : val)
						.text(typeof val == "object" ? val.label : val);
				})
			);
                this.timezone_select.val((typeof this.timezone != "undefined" && this.timezone != null && this.timezone != "") ? this.timezone : o.timezone);
                this.timezone_select.change(function () {
                    tp_inst._onTimeChange();
                });

                // Add grid functionality
                if (o.showHour && o.hourGrid > 0) {
                    size = 100 * hourGridSize * o.hourGrid / (hourMax - o.hourMin);

                    $tp.find(".ui_tpicker_hour table").css({
                        width: size + "%",
                        marginLeft: (size / (-2 * hourGridSize)) + "%",
                        borderCollapse: 'collapse'
                    }).find("td").each(function (index) {
                        $(this).click(function () {
                            var h = $(this).html();
                            if (o.ampm) {
                                var ap = h.substring(2).toLowerCase(),
								aph = parseInt(h.substring(0, 2), 10);
                                if (ap == 'a') {
                                    if (aph == 12) h = 0;
                                    else h = aph;
                                } else if (aph == 12) h = 12;
                                else h = aph + 12;
                            }
                            tp_inst.hour_slider.slider("option", "value", h);
                            tp_inst._onTimeChange();
                            tp_inst._onSelectHandler();
                        }).css({
                            cursor: 'pointer',
                            width: (100 / hourGridSize) + '%',
                            textAlign: 'center',
                            overflow: 'hidden'
                        });
                    });
                }

                if (o.showMinute && o.minuteGrid > 0) {
                    size = 100 * minuteGridSize * o.minuteGrid / (minMax - o.minuteMin);
                    $tp.find(".ui_tpicker_minute table").css({
                        width: size + "%",
                        marginLeft: (size / (-2 * minuteGridSize)) + "%",
                        borderCollapse: 'collapse'
                    }).find("td").each(function (index) {
                        $(this).click(function () {
                            tp_inst.minute_slider.slider("option", "value", $(this).html());
                            tp_inst._onTimeChange();
                            tp_inst._onSelectHandler();
                        }).css({
                            cursor: 'pointer',
                            width: (100 / minuteGridSize) + '%',
                            textAlign: 'center',
                            overflow: 'hidden'
                        });
                    });
                }

                if (o.showSecond && o.secondGrid > 0) {
                    $tp.find(".ui_tpicker_second table").css({
                        width: size + "%",
                        marginLeft: (size / (-2 * secondGridSize)) + "%",
                        borderCollapse: 'collapse'
                    }).find("td").each(function (index) {
                        $(this).click(function () {
                            tp_inst.second_slider.slider("option", "value", $(this).html());
                            tp_inst._onTimeChange();
                            tp_inst._onSelectHandler();
                        }).css({
                            cursor: 'pointer',
                            width: (100 / secondGridSize) + '%',
                            textAlign: 'center',
                            overflow: 'hidden'
                        });
                    });
                }

                if (o.showMillisec && o.millisecGrid > 0) {
                    $tp.find(".ui_tpicker_millisec table").css({
                        width: size + "%",
                        marginLeft: (size / (-2 * millisecGridSize)) + "%",
                        borderCollapse: 'collapse'
                    }).find("td").each(function (index) {
                        $(this).click(function () {
                            tp_inst.millisec_slider.slider("option", "value", $(this).html());
                            tp_inst._onTimeChange();
                            tp_inst._onSelectHandler();
                        }).css({
                            cursor: 'pointer',
                            width: (100 / millisecGridSize) + '%',
                            textAlign: 'center',
                            overflow: 'hidden'
                        });
                    });
                }

                var $buttonPanel = $dp.find('.ui-datepicker-buttonpane');
                if ($buttonPanel.length) $buttonPanel.before($tp);
                else $dp.append($tp);

                this.$timeObj = $tp.find('#ui_tpicker_time_' + dp_id);

                if (this.inst !== null) {
                    var timeDefined = this.timeDefined;
                    this._onTimeChange();
                    this.timeDefined = timeDefined;
                }

                //Emulate datepicker onSelect behavior. Call on slidestop.
                var onSelectDelegate = function () {
                    tp_inst._onSelectHandler();
                };
                this.hour_slider.bind('slidestop', onSelectDelegate);
                this.minute_slider.bind('slidestop', onSelectDelegate);
                this.second_slider.bind('slidestop', onSelectDelegate);
                this.millisec_slider.bind('slidestop', onSelectDelegate);

                // slideAccess integration: http://trentrichardson.com/2011/11/11/jquery-ui-sliders-and-touch-accessibility/
                if (this._defaults.addSliderAccess) {
                    var sliderAccessArgs = this._defaults.sliderAccessArgs;
                    setTimeout(function () { // fix for inline mode
                        if ($tp.find('.ui-slider-access').length == 0) {
                            $tp.find('.ui-slider:visible').sliderAccess(sliderAccessArgs);

                            // fix any grids since sliders are shorter
                            var sliderAccessWidth = $tp.find('.ui-slider-access:eq(0)').outerWidth(true);
                            if (sliderAccessWidth) {
                                $tp.find('table:visible').each(function () {
                                    var $g = $(this),
									oldWidth = $g.outerWidth(),
									oldMarginLeft = $g.css('marginLeft').toString().replace('%', ''),
									newWidth = oldWidth - sliderAccessWidth,
									newMarginLeft = ((oldMarginLeft * newWidth) / oldWidth) + '%';

                                    $g.css({ width: newWidth, marginLeft: newMarginLeft });
                                });
                            }
                        }
                    }, 0);
                }
                // end slideAccess integration

            }
        },

        //########################################################################
        // This function tries to limit the ability to go outside the
        // min/max date range
        //########################################################################
        _limitMinMaxDateTime: function (dp_inst, adjustSliders) {
            var o = this._defaults,
			dp_date = new Date(dp_inst.selectedYear, dp_inst.selectedMonth, dp_inst.selectedDay);

            if (!this._defaults.showTimepicker) return; // No time so nothing to check here

            if ($.datepicker._get(dp_inst, 'minDateTime') !== null && $.datepicker._get(dp_inst, 'minDateTime') !== undefined && dp_date) {
                var minDateTime = $.datepicker._get(dp_inst, 'minDateTime'),
				minDateTimeDate = new Date(minDateTime.getFullYear(), minDateTime.getMonth(), minDateTime.getDate(), 0, 0, 0, 0);

                if (this.hourMinOriginal === null || this.minuteMinOriginal === null || this.secondMinOriginal === null || this.millisecMinOriginal === null) {
                    this.hourMinOriginal = o.hourMin;
                    this.minuteMinOriginal = o.minuteMin;
                    this.secondMinOriginal = o.secondMin;
                    this.millisecMinOriginal = o.millisecMin;
                }

                if (dp_inst.settings.timeOnly || minDateTimeDate.getTime() == dp_date.getTime()) {
                    this._defaults.hourMin = minDateTime.getHours();
                    if (this.hour <= this._defaults.hourMin) {
                        this.hour = this._defaults.hourMin;
                        this._defaults.minuteMin = minDateTime.getMinutes();
                        if (this.minute <= this._defaults.minuteMin) {
                            this.minute = this._defaults.minuteMin;
                            this._defaults.secondMin = minDateTime.getSeconds();
                        } else if (this.second <= this._defaults.secondMin) {
                            this.second = this._defaults.secondMin;
                            this._defaults.millisecMin = minDateTime.getMilliseconds();
                        } else {
                            if (this.millisec < this._defaults.millisecMin)
                                this.millisec = this._defaults.millisecMin;
                            this._defaults.millisecMin = this.millisecMinOriginal;
                        }
                    } else {
                        this._defaults.minuteMin = this.minuteMinOriginal;
                        this._defaults.secondMin = this.secondMinOriginal;
                        this._defaults.millisecMin = this.millisecMinOriginal;
                    }
                } else {
                    this._defaults.hourMin = this.hourMinOriginal;
                    this._defaults.minuteMin = this.minuteMinOriginal;
                    this._defaults.secondMin = this.secondMinOriginal;
                    this._defaults.millisecMin = this.millisecMinOriginal;
                }
            }

            if ($.datepicker._get(dp_inst, 'maxDateTime') !== null && $.datepicker._get(dp_inst, 'maxDateTime') !== undefined && dp_date) {
                var maxDateTime = $.datepicker._get(dp_inst, 'maxDateTime'),
				maxDateTimeDate = new Date(maxDateTime.getFullYear(), maxDateTime.getMonth(), maxDateTime.getDate(), 0, 0, 0, 0);

                if (this.hourMaxOriginal === null || this.minuteMaxOriginal === null || this.secondMaxOriginal === null) {
                    this.hourMaxOriginal = o.hourMax;
                    this.minuteMaxOriginal = o.minuteMax;
                    this.secondMaxOriginal = o.secondMax;
                    this.millisecMaxOriginal = o.millisecMax;
                }

                if (dp_inst.settings.timeOnly || maxDateTimeDate.getTime() == dp_date.getTime()) {
                    this._defaults.hourMax = maxDateTime.getHours();
                    if (this.hour >= this._defaults.hourMax) {
                        this.hour = this._defaults.hourMax;
                        this._defaults.minuteMax = maxDateTime.getMinutes();
                        if (this.minute >= this._defaults.minuteMax) {
                            this.minute = this._defaults.minuteMax;
                            this._defaults.secondMax = maxDateTime.getSeconds();
                        } else if (this.second >= this._defaults.secondMax) {
                            this.second = this._defaults.secondMax;
                            this._defaults.millisecMax = maxDateTime.getMilliseconds();
                        } else {
                            if (this.millisec > this._defaults.millisecMax) this.millisec = this._defaults.millisecMax;
                            this._defaults.millisecMax = this.millisecMaxOriginal;
                        }
                    } else {
                        this._defaults.minuteMax = this.minuteMaxOriginal;
                        this._defaults.secondMax = this.secondMaxOriginal;
                        this._defaults.millisecMax = this.millisecMaxOriginal;
                    }
                } else {
                    this._defaults.hourMax = this.hourMaxOriginal;
                    this._defaults.minuteMax = this.minuteMaxOriginal;
                    this._defaults.secondMax = this.secondMaxOriginal;
                    this._defaults.millisecMax = this.millisecMaxOriginal;
                }
            }

            if (adjustSliders !== undefined && adjustSliders === true) {
                var hourMax = parseInt((this._defaults.hourMax - ((this._defaults.hourMax - this._defaults.hourMin) % this._defaults.stepHour)), 10),
                minMax = parseInt((this._defaults.minuteMax - ((this._defaults.minuteMax - this._defaults.minuteMin) % this._defaults.stepMinute)), 10),
                secMax = parseInt((this._defaults.secondMax - ((this._defaults.secondMax - this._defaults.secondMin) % this._defaults.stepSecond)), 10),
				millisecMax = parseInt((this._defaults.millisecMax - ((this._defaults.millisecMax - this._defaults.millisecMin) % this._defaults.stepMillisec)), 10);

                if (this.hour_slider)
                    this.hour_slider.slider("option", { min: this._defaults.hourMin, max: hourMax }).slider('value', this.hour);
                if (this.minute_slider)
                    this.minute_slider.slider("option", { min: this._defaults.minuteMin, max: minMax }).slider('value', this.minute);
                if (this.second_slider)
                    this.second_slider.slider("option", { min: this._defaults.secondMin, max: secMax }).slider('value', this.second);
                if (this.millisec_slider)
                    this.millisec_slider.slider("option", { min: this._defaults.millisecMin, max: millisecMax }).slider('value', this.millisec);
            }

        },


        //########################################################################
        // when a slider moves, set the internal time...
        // on time change is also called when the time is updated in the text field
        //########################################################################
        _onTimeChange: function () {
            var hour = (this.hour_slider) ? this.hour_slider.slider('value') : false,
			minute = (this.minute_slider) ? this.minute_slider.slider('value') : false,
			second = (this.second_slider) ? this.second_slider.slider('value') : false,
			millisec = (this.millisec_slider) ? this.millisec_slider.slider('value') : false,
			timezone = (this.timezone_select) ? this.timezone_select.val() : false,
			o = this._defaults;

            if (typeof (hour) == 'object') hour = false;
            if (typeof (minute) == 'object') minute = false;
            if (typeof (second) == 'object') second = false;
            if (typeof (millisec) == 'object') millisec = false;
            if (typeof (timezone) == 'object') timezone = false;

            if (hour !== false) hour = parseInt(hour, 10);
            if (minute !== false) minute = parseInt(minute, 10);
            if (second !== false) second = parseInt(second, 10);
            if (millisec !== false) millisec = parseInt(millisec, 10);

            var ampm = o[hour < 12 ? 'amNames' : 'pmNames'][0];

            // If the update was done in the input field, the input field should not be updated.
            // If the update was done using the sliders, update the input field.
            var hasChanged = (hour != this.hour || minute != this.minute
				|| second != this.second || millisec != this.millisec
				|| (this.ampm.length > 0
				    && (hour < 12) != ($.inArray(this.ampm.toUpperCase(), this.amNames) !== -1))
				|| timezone != this.timezone);

            if (hasChanged) {

                if (hour !== false) this.hour = hour;
                if (minute !== false) this.minute = minute;
                if (second !== false) this.second = second;
                if (millisec !== false) this.millisec = millisec;
                if (timezone !== false) this.timezone = timezone;

                if (!this.inst) this.inst = $.datepicker._getInst(this.$input[0]);

                this._limitMinMaxDateTime(this.inst, true);
            }
            if (o.ampm) this.ampm = ampm;

            //this._formatTime();
            this.formattedTime = $.datepicker.formatTime(this._defaults.timeFormat, this, this._defaults);
            if (this.$timeObj) this.$timeObj.text(this.formattedTime + o.timeSuffix);
            this.timeDefined = true;
            if (hasChanged) this._updateDateTime();
        },

        //########################################################################
        // call custom onSelect. 
        // bind to sliders slidestop, and grid click.
        //########################################################################
        _onSelectHandler: function () {
            var onSelect = this._defaults.onSelect;
            var inputEl = this.$input ? this.$input[0] : null;
            if (onSelect && inputEl) {
                onSelect.apply(inputEl, [this.formattedDateTime, this]);
            }
        },

        //########################################################################
        // left for any backwards compatibility
        //########################################################################
        _formatTime: function (time, format) {
            time = time || { hour: this.hour, minute: this.minute, second: this.second, millisec: this.millisec, ampm: this.ampm, timezone: this.timezone };
            var tmptime = (format || this._defaults.timeFormat).toString();

            tmptime = $.datepicker.formatTime(tmptime, time, this._defaults);

            if (arguments.length) return tmptime;
            else this.formattedTime = tmptime;
        },

        //########################################################################
        // update our input with the new date time..
        //########################################################################
        _updateDateTime: function (dp_inst) {
            dp_inst = this.inst || dp_inst;
            var dt = $.datepicker._daylightSavingAdjust(new Date(dp_inst.selectedYear, dp_inst.selectedMonth, dp_inst.selectedDay)),
			dateFmt = $.datepicker._get(dp_inst, 'dateFormat'),
			formatCfg = $.datepicker._getFormatConfig(dp_inst),
			timeAvailable = dt !== null && this.timeDefined;
            this.formattedDate = $.datepicker.formatDate(dateFmt, (dt === null ? new Date() : dt), formatCfg);
            var formattedDateTime = this.formattedDate;
            if (dp_inst.lastVal !== undefined && (dp_inst.lastVal.length > 0 && this.$input.val().length === 0))
                return;

            if (this._defaults.timeOnly === true) {
                formattedDateTime = this.formattedTime;
            } else if (this._defaults.timeOnly !== true && (this._defaults.alwaysSetTime || timeAvailable)) {
                formattedDateTime += this._defaults.separator + this.formattedTime + this._defaults.timeSuffix;
            }

            this.formattedDateTime = formattedDateTime;

            if (!this._defaults.showTimepicker) {
                this.$input.val(this.formattedDate);
            } else if (this.$altInput && this._defaults.altFieldTimeOnly === true) {
                this.$altInput.val(this.formattedTime);
                this.$input.val(this.formattedDate);
            } else if (this.$altInput) {
                this.$altInput.val(formattedDateTime);
                this.$input.val(formattedDateTime);
            } else {
                this.$input.val(formattedDateTime);
            }

            this.$input.trigger("change");
        }

    });

    $.fn.extend({
        //########################################################################
        // shorthand just to use timepicker..
        //########################################################################
        timepicker: function (o) {
            o = o || {};
            var tmp_args = arguments;

            if (typeof o == 'object') tmp_args[0] = $.extend(o, { timeOnly: true });

            return $(this).each(function () {
                $.fn.datetimepicker.apply($(this), tmp_args);
            });
        },

        //########################################################################
        // extend timepicker to datepicker
        //########################################################################
        datetimepicker: function (o) {
            o = o || {};
            var $input = this,
		tmp_args = arguments;

            if (typeof (o) == 'string') {
                if (o == 'getDate')
                    return $.fn.datepicker.apply($(this[0]), tmp_args);
                else
                    return this.each(function () {
                        var $t = $(this);
                        $t.datepicker.apply($t, tmp_args);
                    });
            }
            else
                return this.each(function () {
                    var $t = $(this);
                    $t.datepicker($.timepicker._newInst($t, o)._defaults);
                });
        }
    });

    //########################################################################
    // format the time all pretty... 
    // format = string format of the time
    // time = a {}, not a Date() for timezones
    // options = essentially the regional[].. amNames, pmNames, ampm
    //########################################################################
    $.datepicker.formatTime = function (format, time, options) {
        options = options || {};
        options = $.extend($.timepicker._defaults, options);
        time = $.extend({ hour: 0, minute: 0, second: 0, millisec: 0, timezone: '+0000' }, time);

        var tmptime = format;
        var ampmName = options['amNames'][0];

        var hour = parseInt(time.hour, 10);
        if (options.ampm) {
            if (hour > 11) {
                ampmName = options['pmNames'][0];
                if (hour > 12)
                    hour = hour % 12;
            }
            if (hour === 0)
                hour = 12;
        }
        tmptime = tmptime.replace(/(?:hh?|mm?|ss?|[tT]{1,2}|[lz])/g, function (match) {
            switch (match.toLowerCase()) {
                case 'hh': return ('0' + hour).slice(-2);
                case 'h': return hour;
                case 'mm': return ('0' + time.minute).slice(-2);
                case 'm': return time.minute;
                case 'ss': return ('0' + time.second).slice(-2);
                case 's': return time.second;
                case 'l': return ('00' + time.millisec).slice(-3);
                case 'z': return time.timezone;
                case 't': case 'tt':
                    if (options.ampm) {
                        if (match.length == 1)
                            ampmName = ampmName.charAt(0);
                        return match.charAt(0) == 'T' ? ampmName.toUpperCase() : ampmName.toLowerCase();
                    }
                    return '';
            }
        });

        tmptime = $.trim(tmptime);
        return tmptime;
    }

    //########################################################################
    // the bad hack :/ override datepicker so it doesnt close on select
    // inspired: http://stackoverflow.com/questions/1252512/jquery-datepicker-prevent-closing-picker-when-clicking-a-date/1762378#1762378
    //########################################################################
    $.datepicker._base_selectDate = $.datepicker._selectDate;
    $.datepicker._selectDate = function (id, dateStr) {
        var inst = this._getInst($(id)[0]),
		tp_inst = this._get(inst, 'timepicker');

        if (tp_inst) {
            tp_inst._limitMinMaxDateTime(inst, true);
            inst.inline = inst.stay_open = true;
            //This way the onSelect handler called from calendarpicker get the full dateTime
            this._base_selectDate(id, dateStr);
            inst.inline = inst.stay_open = false;
            this._notifyChange(inst);
            this._updateDatepicker(inst);
        }
        else this._base_selectDate(id, dateStr);
    };

    //#############################################################################################
    // second bad hack :/ override datepicker so it triggers an event when changing the input field
    // and does not redraw the datepicker on every selectDate event
    //#############################################################################################
    $.datepicker._base_updateDatepicker = $.datepicker._updateDatepicker;
    $.datepicker._updateDatepicker = function (inst) {

        // don't popup the datepicker if there is another instance already opened
        var input = inst.input[0];
        if ($.datepicker._curInst &&
	   $.datepicker._curInst != inst &&
	   $.datepicker._datepickerShowing &&
	   $.datepicker._lastInput != input) {
            return;
        }

        if (typeof (inst.stay_open) !== 'boolean' || inst.stay_open === false) {

            this._base_updateDatepicker(inst);

            // Reload the time control when changing something in the input text field.
            var tp_inst = this._get(inst, 'timepicker');
            if (tp_inst) tp_inst._addTimePicker(inst);
        }
    };

    //#######################################################################################
    // third bad hack :/ override datepicker so it allows spaces and colon in the input field
    //#######################################################################################
    $.datepicker._base_doKeyPress = $.datepicker._doKeyPress;
    $.datepicker._doKeyPress = function (event) {
        var inst = $.datepicker._getInst(event.target),
		tp_inst = $.datepicker._get(inst, 'timepicker');

        if (tp_inst) {
            if ($.datepicker._get(inst, 'constrainInput')) {
                var ampm = tp_inst._defaults.ampm,
				dateChars = $.datepicker._possibleChars($.datepicker._get(inst, 'dateFormat')),
				datetimeChars = tp_inst._defaults.timeFormat.toString()
								.replace(/[hms]/g, '')
								.replace(/TT/g, ampm ? 'APM' : '')
								.replace(/Tt/g, ampm ? 'AaPpMm' : '')
								.replace(/tT/g, ampm ? 'AaPpMm' : '')
								.replace(/T/g, ampm ? 'AP' : '')
								.replace(/tt/g, ampm ? 'apm' : '')
								.replace(/t/g, ampm ? 'ap' : '') +
								" " +
								tp_inst._defaults.separator +
								tp_inst._defaults.timeSuffix +
								(tp_inst._defaults.showTimezone ? tp_inst._defaults.timezoneList.join('') : '') +
								(tp_inst._defaults.amNames.join('')) +
								(tp_inst._defaults.pmNames.join('')) +
								dateChars,
				chr = String.fromCharCode(event.charCode === undefined ? event.keyCode : event.charCode);
                return event.ctrlKey || (chr < ' ' || !dateChars || datetimeChars.indexOf(chr) > -1);
            }
        }

        return $.datepicker._base_doKeyPress(event);
    };

    //#######################################################################################
    // Override key up event to sync manual input changes.
    //#######################################################################################
    $.datepicker._base_doKeyUp = $.datepicker._doKeyUp;
    $.datepicker._doKeyUp = function (event) {
        var inst = $.datepicker._getInst(event.target),
		tp_inst = $.datepicker._get(inst, 'timepicker');

        if (tp_inst) {
            if (tp_inst._defaults.timeOnly && (inst.input.val() != inst.lastVal)) {
                try {
                    $.datepicker._updateDatepicker(inst);
                }
                catch (err) {
                    $.datepicker.log(err);
                }
            }
        }

        return $.datepicker._base_doKeyUp(event);
    };

    //#######################################################################################
    // override "Today" button to also grab the time.
    //#######################################################################################
    $.datepicker._base_gotoToday = $.datepicker._gotoToday;
    $.datepicker._gotoToday = function (id) {
        var inst = this._getInst($(id)[0]),
		$dp = inst.dpDiv;
        this._base_gotoToday(id);
        var now = new Date();
        var tp_inst = this._get(inst, 'timepicker');
        if (tp_inst && tp_inst._defaults.showTimezone && tp_inst.timezone_select) {
            var tzoffset = now.getTimezoneOffset(); // If +0100, returns -60
            var tzsign = tzoffset > 0 ? '-' : '+';
            tzoffset = Math.abs(tzoffset);
            var tzmin = tzoffset % 60;
            tzoffset = tzsign + ('0' + (tzoffset - tzmin) / 60).slice(-2) + ('0' + tzmin).slice(-2);
            if (tp_inst._defaults.timezoneIso8609)
                tzoffset = tzoffset.substring(0, 3) + ':' + tzoffset.substring(3);
            tp_inst.timezone_select.val(tzoffset);
        }
        this._setTime(inst, now);
        $('.ui-datepicker-today', $dp).click();
    };

    //#######################################################################################
    // Disable & enable the Time in the datetimepicker
    //#######################################################################################
    $.datepicker._disableTimepickerDatepicker = function (target, date, withDate) {
        var inst = this._getInst(target),
	tp_inst = this._get(inst, 'timepicker');
        $(target).datepicker('getDate'); // Init selected[Year|Month|Day]
        if (tp_inst) {
            tp_inst._defaults.showTimepicker = false;
            tp_inst._updateDateTime(inst);
        }
    };

    $.datepicker._enableTimepickerDatepicker = function (target, date, withDate) {
        var inst = this._getInst(target),
	tp_inst = this._get(inst, 'timepicker');
        $(target).datepicker('getDate'); // Init selected[Year|Month|Day]
        if (tp_inst) {
            tp_inst._defaults.showTimepicker = true;
            tp_inst._addTimePicker(inst); // Could be disabled on page load
            tp_inst._updateDateTime(inst);
        }
    };

    //#######################################################################################
    // Create our own set time function
    //#######################################################################################
    $.datepicker._setTime = function (inst, date) {
        var tp_inst = this._get(inst, 'timepicker');
        if (tp_inst) {
            var defaults = tp_inst._defaults,
            // calling _setTime with no date sets time to defaults
			hour = date ? date.getHours() : defaults.hour,
			minute = date ? date.getMinutes() : defaults.minute,
			second = date ? date.getSeconds() : defaults.second,
			millisec = date ? date.getMilliseconds() : defaults.millisec;

            //check if within min/max times..
            if ((hour < defaults.hourMin || hour > defaults.hourMax) || (minute < defaults.minuteMin || minute > defaults.minuteMax) || (second < defaults.secondMin || second > defaults.secondMax) || (millisec < defaults.millisecMin || millisec > defaults.millisecMax)) {
                hour = defaults.hourMin;
                minute = defaults.minuteMin;
                second = defaults.secondMin;
                millisec = defaults.millisecMin;
            }

            tp_inst.hour = hour;
            tp_inst.minute = minute;
            tp_inst.second = second;
            tp_inst.millisec = millisec;

            if (tp_inst.hour_slider) tp_inst.hour_slider.slider('value', hour);
            if (tp_inst.minute_slider) tp_inst.minute_slider.slider('value', minute);
            if (tp_inst.second_slider) tp_inst.second_slider.slider('value', second);
            if (tp_inst.millisec_slider) tp_inst.millisec_slider.slider('value', millisec);

            tp_inst._onTimeChange();
            tp_inst._updateDateTime(inst);
        }
    };

    //#######################################################################################
    // Create new public method to set only time, callable as $().datepicker('setTime', date)
    //#######################################################################################
    $.datepicker._setTimeDatepicker = function (target, date, withDate) {
        var inst = this._getInst(target),
		tp_inst = this._get(inst, 'timepicker');

        if (tp_inst) {
            this._setDateFromField(inst);
            var tp_date;
            if (date) {
                if (typeof date == "string") {
                    tp_inst._parseTime(date, withDate);
                    tp_date = new Date();
                    tp_date.setHours(tp_inst.hour, tp_inst.minute, tp_inst.second, tp_inst.millisec);
                }
                else tp_date = new Date(date.getTime());
                if (tp_date.toString() == 'Invalid Date') tp_date = undefined;
                this._setTime(inst, tp_date);
            }
        }

    };

    //#######################################################################################
    // override setDate() to allow setting time too within Date object
    //#######################################################################################
    $.datepicker._base_setDateDatepicker = $.datepicker._setDateDatepicker;
    $.datepicker._setDateDatepicker = function (target, date) {
        var inst = this._getInst(target),
	tp_date = (date instanceof Date) ? new Date(date.getTime()) : date;

        this._updateDatepicker(inst);
        this._base_setDateDatepicker.apply(this, arguments);
        this._setTimeDatepicker(target, tp_date, true);
    };

    //#######################################################################################
    // override getDate() to allow getting time too within Date object
    //#######################################################################################
    $.datepicker._base_getDateDatepicker = $.datepicker._getDateDatepicker;
    $.datepicker._getDateDatepicker = function (target, noDefault) {
        var inst = this._getInst(target),
		tp_inst = this._get(inst, 'timepicker');

        if (tp_inst) {
            this._setDateFromField(inst, noDefault);
            var date = this._getDate(inst);
            if (date && tp_inst._parseTime($(target).val(), tp_inst.timeOnly)) date.setHours(tp_inst.hour, tp_inst.minute, tp_inst.second, tp_inst.millisec);
            return date;
        }
        return this._base_getDateDatepicker(target, noDefault);
    };

    //#######################################################################################
    // override parseDate() because UI 1.8.14 throws an error about "Extra characters"
    // An option in datapicker to ignore extra format characters would be nicer.
    //#######################################################################################
    $.datepicker._base_parseDate = $.datepicker.parseDate;
    $.datepicker.parseDate = function (format, value, settings) {
        var date;
        try {
            date = this._base_parseDate(format, value, settings);
        } catch (err) {
            if (err.indexOf(":") >= 0) {
                // Hack!  The error message ends with a colon, a space, and
                // the "extra" characters.  We rely on that instead of
                // attempting to perfectly reproduce the parsing algorithm.
                date = this._base_parseDate(format, value.substring(0, value.length - (err.length - err.indexOf(':') - 2)), settings);
            } else {
                // The underlying error was not related to the time
                throw err;
            }
        }
        return date;
    };

    //#######################################################################################
    // override formatDate to set date with time to the input
    //#######################################################################################
    $.datepicker._base_formatDate = $.datepicker._formatDate;
    $.datepicker._formatDate = function (inst, day, month, year) {
        var tp_inst = this._get(inst, 'timepicker');
        if (tp_inst) {
            if (day)
                var b = this._base_formatDate(inst, day, month, year);
            tp_inst._updateDateTime(inst);
            return tp_inst.$input.val();
        }
        return this._base_formatDate(inst);
    };

    //#######################################################################################
    // override options setter to add time to maxDate(Time) and minDate(Time). MaxDate
    //#######################################################################################
    $.datepicker._base_optionDatepicker = $.datepicker._optionDatepicker;
    $.datepicker._optionDatepicker = function (target, name, value) {
        var inst = this._getInst(target),
		tp_inst = this._get(inst, 'timepicker');
        if (tp_inst) {
            var min, max, onselect;
            if (typeof name == 'string') { // if min/max was set with the string
                if (name === 'minDate' || name === 'minDateTime')
                    min = value;
                else if (name === 'maxDate' || name === 'maxDateTime')
                    max = value;
                else if (name === 'onSelect')
                    onselect = value;
            } else if (typeof name == 'object') { //if min/max was set with the JSON
                if (name.minDate)
                    min = name.minDate;
                else if (name.minDateTime)
                    min = name.minDateTime;
                else if (name.maxDate)
                    max = name.maxDate;
                else if (name.maxDateTime)
                    max = name.maxDateTime;
            }
            if (min) { //if min was set
                if (min == 0)
                    min = new Date();
                else
                    min = new Date(min);

                tp_inst._defaults.minDate = min;
                tp_inst._defaults.minDateTime = min;
            } else if (max) { //if max was set
                if (max == 0)
                    max = new Date();
                else
                    max = new Date(max);
                tp_inst._defaults.maxDate = max;
                tp_inst._defaults.maxDateTime = max;
            }
            else if (onselect)
                tp_inst._defaults.onSelect = onselect;
        }
        if (value === undefined)
            return this._base_optionDatepicker(target, name);
        return this._base_optionDatepicker(target, name, value);
    };

    //#######################################################################################
    // jQuery extend now ignores nulls!
    //#######################################################################################
    function extendRemove(target, props) {
        $.extend(target, props);
        for (var name in props)
            if (props[name] === null || props[name] === undefined)
                target[name] = props[name];
        return target;
    };

    $.timepicker = new Timepicker(); // singleton instance
    $.timepicker.version = "0.9.9";

})(jQuery);

/*
http://jqueryui.com/download
采用

组件包括:
Datepicker
主题: cupertino
Version: 1.7.3

添加样式表：
.ui-datepicker
{
z-index: 9999;
}
*/

(function ($) {
    if ($.datepicker) {
        $.datepicker.dpDiv.hide();
        $.datepicker.regional['zh-CN'] = {
            showAnim: "slideDown",
            changeYear: true,
            showButtonPanel: true,
            closeText: '确定',
            clearText: '清除',
            prevText: '&#x3c;上月',
            nextText: '下月&#x3e;',
            currentText: '今天',
            monthNames: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
            monthNamesShort: ['一', '二', '三', '四', '五', '六', '七', '八', '九', '十', '十一', '十二'],
            dayNames: ['星期日', '星期一', '星期二', '星期三', '星期四', '星期五', '星期六'],
            dayNamesShort: ['周日', '周一', '周二', '周三', '周四', '周五', '周六'],
            dayNamesMin: ['日', '一', '二', '三', '四', '五', '六'],
            weekHeader: '周',
            dateFormat: 'yy-mm-dd',
            firstDay: 1,
            isRTL: false,
            showMonthAfterYear: true,
            yearSuffix: '年'
        };
        $.datepicker.setDefaults($.datepicker.regional['zh-CN']);
    }

    if ($.timepicker) {
        $.timepicker.regional['zh-CN'] = {
            currentText: '现在',
            closeText: '确定',
            ampm: false,
            amNames: ['上午', 'A'],
            pmNames: ['下午', 'P'],
            timeFormat: 'hh:mm tt',
            timeSuffix: '',
            timeOnlyTitle: '选择时间',
            timeText: '时间',
            hourText: '时',
            minuteText: '分',
            secondText: '秒',
            millisecText: '毫秒',
            timezoneText: '时区'
        };
        $.timepicker.setDefaults($.timepicker.regional['zh-CN']);
    }
})(jQuery);

/**
* http://github.com/Valums-File-Uploader/file-uploader
*
* Multiple file upload component with progress-bar, drag-and-drop.
*
* Have ideas for improving this JS for the general community?
* Submit your changes at: https://github.com/Valums-File-Uploader/file-uploader
* Readme at https://github.com/valums/file-uploader/blob/2.0/readme.md
*
* VERSION 2.2-SNAPSHOT
* Original version: 1.0 © 2010 Andrew Valums ( andrew(at)valums.com )
* Current Maintainer (2.0+): © 2012, Ray Nicholus ( fineuploader(at)garstasio.com )
*
* Licensed under MIT license, GNU GPL 2 or later, GNU LGPL 2 or later, see license.txt.
*/

//
// Helper functions
//

var qq = qq || {};

/**
* Adds all missing properties from second obj to first obj
*/
qq.extend = function (first, second) {
    return $.extend(true, first, second);
    //    for (var prop in second) {
    //        first[prop] = second[prop];
    //    }
};

/**
* Searches for a given element in the array, returns -1 if it is not present.
* @param {Number} [from] The index at which to begin the search
*/
qq.indexOf = function (arr, elt, from) {
    if (arr.indexOf) return arr.indexOf(elt, from);

    from = from || 0;
    var len = arr.length;

    if (from < 0) from += len;

    for (; from < len; from++) {
        if (from in arr && arr[from] === elt) {
            return from;
        }
    }
    return -1;
};

qq.getUniqueId = (function () {
    var id = 0;
    return function () { return id++; };
})();

//
// Browsers and platforms detection

qq.ie = function () { return navigator.userAgent.indexOf('MSIE') != -1; }
qq.safari = function () { return navigator.vendor != undefined && navigator.vendor.indexOf("Apple") != -1; }
qq.chrome = function () { return navigator.vendor != undefined && navigator.vendor.indexOf('Google') != -1; }
qq.firefox = function () { return (navigator.userAgent.indexOf('Mozilla') != -1 && navigator.vendor != undefined && navigator.vendor == ''); }
qq.windows = function () { return navigator.platform == "Win32"; }

//
// Events

/** Returns the function which detaches attached event */
qq.attach = function (element, type, fn) {
    if (element.addEventListener) {
        element.addEventListener(type, fn, false);
    } else if (element.attachEvent) {
        element.attachEvent('on' + type, fn);
    }
    return function () {
        qq.detach(element, type, fn)
    }
};
qq.detach = function (element, type, fn) {
    if (element.removeEventListener) {
        element.removeEventListener(type, fn, false);
    } else if (element.attachEvent) {
        element.detachEvent('on' + type, fn);
    }
};

qq.preventDefault = function (e) {
    if (e.preventDefault) {
        e.preventDefault();
    } else {
        e.returnValue = false;
    }
};

//
// Node manipulations

/**
* Insert node a before node b.
*/
qq.insertBefore = function (a, b) {
    b.parentNode.insertBefore(a, b);
};
qq.remove = function (element) {
    element.parentNode.removeChild(element);
};

qq.contains = function (parent, descendant) {
    // compareposition returns false in this case
    if (parent == descendant) return true;

    if (parent.contains) {
        return parent.contains(descendant);
    } else {
        return !!(descendant.compareDocumentPosition(parent) & 8);
    }
};

/**
* Creates and returns element from html string
* Uses innerHTML to create an element
*/
qq.toElement = (function () {
    var div = document.createElement('div');
    return function (html) {
        div.innerHTML = html;
        var element = div.firstChild;
        div.removeChild(element);
        return element;
    };
})();

//
// Node properties and attributes

/**
* Sets styles for an element.
* Fixes opacity in IE6-8.
*/
qq.css = function (element, styles) {
    if (styles.opacity != null) {
        if (typeof element.style.opacity != 'string' && typeof (element.filters) != 'undefined') {
            styles.filter = 'alpha(opacity=' + Math.round(100 * styles.opacity) + ')';
        }
    }
    qq.extend(element.style, styles);
};
qq.hasClass = function (element, name) {
    var re = new RegExp('(^| )' + name + '( |$)');
    return re.test(element.className);
};
qq.addClass = function (element, name) {
    if (!qq.hasClass(element, name)) {
        element.className += ' ' + name;
    }
};
qq.removeClass = function (element, name) {
    var re = new RegExp('(^| )' + name + '( |$)');
    element.className = element.className.replace(re, ' ').replace(/^\s+|\s+$/g, "");
};
qq.setText = function (element, text) {
    element.innerText = text;
    element.textContent = text;
};

//
// Selecting elements

qq.children = function (element) {
    var children = [],
        child = element.firstChild;

    while (child) {
        if (child.nodeType == 1) {
            children.push(child);
        }
        child = child.nextSibling;
    }

    return children;
};

qq.getByClass = function (element, className) {
    if (element.querySelectorAll) {
        return element.querySelectorAll('.' + className);
    }

    var result = [];
    var candidates = element.getElementsByTagName("*");
    var len = candidates.length;

    for (var i = 0; i < len; i++) {
        if (qq.hasClass(candidates[i], className)) {
            result.push(candidates[i]);
        }
    }
    return result;
};

/**
* obj2url() takes a json-object as argument and generates
* a querystring. pretty much like jQuery.param()
*
* how to use:
*
*    `qq.obj2url({a:'b',c:'d'},'http://any.url/upload?otherParam=value');`
*
* will result in:
*
*    `http://any.url/upload?otherParam=value&a=b&c=d`
*
* @param  Object JSON-Object
* @param  String current querystring-part
* @return String encoded querystring
*/
qq.obj2url = function (obj, temp, prefixDone) {
    var uristrings = [],
        prefix = '&',
        add = function (nextObj, i) {
            var nextTemp = temp
                ? (/\[\]$/.test(temp)) // prevent double-encoding
                ? temp
                : temp + '[' + i + ']'
                : i;
            if ((nextTemp != 'undefined') && (i != 'undefined')) {
                uristrings.push(
                    (typeof nextObj === 'object')
                        ? qq.obj2url(nextObj, nextTemp, true)
                        : (Object.prototype.toString.call(nextObj) === '[object Function]')
                        ? encodeURIComponent(nextTemp) + '=' + encodeURIComponent(nextObj())
                        : encodeURIComponent(nextTemp) + '=' + encodeURIComponent(nextObj)
                );
            }
        };

    if (!prefixDone && temp) {
        prefix = (/\?/.test(temp)) ? (/\?$/.test(temp)) ? '' : '&' : '?';
        uristrings.push(temp);
        uristrings.push(qq.obj2url(obj));
    } else if ((Object.prototype.toString.call(obj) === '[object Array]') && (typeof obj != 'undefined')) {
        // we wont use a for-in-loop on an array (performance)
        for (var i = 0, len = obj.length; i < len; ++i) {
            add(obj[i], i);
        }
    } else if ((typeof obj != 'undefined') && (obj !== null) && (typeof obj === "object")) {
        // for anything else but a scalar, we will use for-in-loop
        for (var i in obj) {
            add(obj[i], i);
        }
    } else {
        uristrings.push(encodeURIComponent(temp) + '=' + encodeURIComponent(obj));
    }

    return uristrings.join(prefix)
        .replace(/^&/, '')
        .replace(/%20/g, '+');
};

//
//
// Uploader Classes
//
//

var qq = qq || {};

/**
* Creates upload button, validates upload, but doesn't create file list or dd.
*/
qq.FileUploaderBasic = function (o) {
    var that = this;
    this._options = {
        // set to true to see the server response
        debug: false,
        _loaded: {},
        customList: false,
        action: jv.url().root + 'Res/FileUploader/FileUploader.ashx',
        params: {},
        customHeaders: {},
        button: null,
        multiple: true,
        maxFile: 3,
        disableCancelForFormUploads: false,
        autoUpload: true,
        forceMultipart: false,
        // validation
        allowedExtensions: [],
        acceptFiles: null, 	// comma separated string of mime-types for browser to display in browse dialog
        sizeLimit: 0,
        minSizeLimit: 0,
        stopOnFirstInvalidFile: true,
        // events
        // return false to cancel submit
        onSubmit: function (id, fileName) { },
        onComplete: function (id, fileName, responseJSON) { },
        onCancel: function (id, fileName) { },
        onUpload: function (id, fileName, xhr) { },
        onProgress: function (id, fileName, loaded, total) { },
        onError: function (id, fileName, reason) { },
        // messages
        messages: {
            typeError: "{file} 具有非法的扩展名. 只允许: {extensions}",
            sizeError: "{file} 文件太大, 最大限值: {sizeLimit}",
            minSizeError: "{file} 文件太小, 最小限值: {minSizeLimit}",
            emptyError: "{file} 文件为空, 请另行选择",
            onLeave: "文件正在上传,现在退出会取消上传.",
            maxFile: "已超出文件限额,最多只能上传 {maxFile} 个",
            noFilesError: "No files to upload."
        },
        showMessage: function (message) {
            alert(message);
        },
        inputName: 'qqfile'
    };

    qq.extend(this._options, o);
    this._wrapCallbacks();
    qq.extend(this, qq.DisposeSupport);

    // number of files being uploaded
    this._filesInProgress = 0;

    this._storedFileIds = [];

    this._handler = this._createUploadHandler();

    //udi add
    this._remove = function (dbId) {
        var qqFileId = $("li[dbid=" + dbId + "]", this._listElement).attr("qqfileid");
        if (qqFileId) {
            if (this._handler._files) {
                this._handler._files.removeAt(qqFileId);
                $("li:eq(" + qqFileId + ")", this._listElement).remove();
            }
            if (this._handler._queue) this._handler._queue.remove(qqFileId);
            if (this._options._loaded) delete this._options._loaded[qqFileId];
            if (this._handler._params) {
                delete this._handler._params[qqFileId];
                var seq_id = qqFileId.toString();
                if (seq_id.indexOf("qq-upload-handler-iframe") >= 0) {
                    seq_id = seq_id.slice("qq-upload-handler-iframe".length);
                }

                $("li:eq(" + seq_id + ")", this._listElement).remove();
            }
        }
        if (dbId) {
            $("li[dbid=" + dbId + "]", this._listElement).remove();
            if (this._options._loaded) delete this._options._loaded[dbId];
        }

    };

    if (this._options.button) {
        this._button = this._createUploadButton(this._options.button);
    }

    this._preventLeaveInProgress();
};

qq.FileUploaderBasic.prototype = {
    log: function (str) {
        if (this._options.debug && window.console) console.log('[uploader] ' + str);
    },
    setParams: function (params) {
        this._options.params = params;
    },
    getInProgress: function () {
        return this._filesInProgress;
    },
    uploadStoredFiles: function () {
        while (this._storedFileIds.length) {
            this._filesInProgress++;
            this._handler.upload(this._storedFileIds.shift(), this._options.params);
        }
    },
    clearStoredFiles: function () {
        this._storedFileIds = [];
    },
    _createUploadButton: function (element) {
        var self = this;

        var button = new qq.UploadButton({
            element: element,
            multiple: this._options.multiple && qq.UploadHandlerXhr.isSupported(),
            acceptFiles: this._options.acceptFiles,
            onChange: function (input) {
                self._onInputChange(input);
            }
        });

        if (this._options.css)
            $(button._element).css(this._options.css);
     
        this.addDisposer(function () { button.dispose(); });
        return button;
    },
    _createUploadHandler: function () {
        var self = this,
            handlerClass;

        if (qq.UploadHandlerXhr.isSupported()) {
            handlerClass = 'UploadHandlerXhr';
        } else {
            handlerClass = 'UploadHandlerForm';
        }

        var handler = new qq[handlerClass]({
            debug: this._options.debug,
            sizeLimit: this._options.sizeLimit,
            action: this._options.action,
            forceMultipart: this._options.forceMultipart,
            maxFile: this._options.maxFile,
            customHeaders: this._options.customHeaders,
            inputName: this._options.inputName,
            demoMode: this._options.demoMode,
            onProgress: function (id, fileName, loaded, total) {
                self._onProgress(id, fileName, loaded, total);
                self._options.onProgress(id, fileName, loaded, total);
            },
            onComplete: function (id, fileName, result) {
                self._onComplete(id, fileName, result);
                self._options.onComplete(id, fileName, result);
            },
            onCancel: function (id, fileName) {
                self._onCancel(id, fileName);
                self._options.onCancel(id, fileName);
            },
            onError: self._options.onError,
            onUpload: function (id, fileName, xhr) {
                self._onUpload(id, fileName, xhr);
                self._options.onUpload(id, fileName, xhr);
            }
        });

        return handler;
    },
    _preventLeaveInProgress: function () {
        var self = this;

        this._attach(window, 'beforeunload', function (e) {
            if (!self._filesInProgress) { return; }

            var e = e || window.event;
            // for ie, ff
            e.returnValue = self._options.messages.onLeave;
            // for webkit
            return self._options.messages.onLeave;
        });
    },
    _onSubmit: function (id, fileName) {
        if (this._options.autoUpload) {
            this._filesInProgress++;
        }
    },
    _onProgress: function (id, fileName, loaded, total) {
    },
    _onComplete: function (id, fileName, result) {
        this._filesInProgress--;

        if (!result.success) {
            var errorReason = result.error ? result.error : "Upload failure reason unknown";
            this._options.onError(id, fileName, errorReason);
        }
    },
    _onCancel: function (id, fileName) {
        var storedFileIndex = qq.indexOf(this._storedFileIds, id);
        if (this._options.autoUpload || storedFileIndex < 0) {
            this._filesInProgress--;
        }
        else if (!this._options.autoUpload) {
            this._storedFileIds.splice(storedFileIndex, 1);
        }
    },
    _onUpload: function (id, fileName, xhr) {
    },
    _onInputChange: function (input) {
        var fileCount = this._options && this._options._loaded && jv.GetJsonKeys(this._options._loaded).length;

        if ((input.files ? input.files.length : 1) + fileCount > this._options.maxFile) {
            this._error("maxFile");
            return;
        }

        if (this._handler instanceof qq.UploadHandlerXhr) {
            this._uploadFileList(input.files);
        } else {
            if (this._validateFile(input)) {
                this._uploadFile(input);
            }
        }
        this._button.reset();
    },
    _uploadFileList: function (files) {
        if (files.length > 0) {
            for (var i = 0; i < files.length; i++) {
                if (this._validateFile(files[i])) {
                    this._uploadFile(files[i]);
                } else {
                    if (this._options.stopOnFirstInvalidFile) {
                        return;
                    }
                }
            }
        }
        else {
            this._error('noFilesError', "");
        }
    },
    _uploadFile: function (fileContainer) {
        var id = this._handler.add(fileContainer);
        var fileName = this._handler.getName(id);

        if (this._options.onSubmit(id, fileName) !== false) {
            this._onSubmit(id, fileName);
            if (this._options.autoUpload) {
                this._handler.upload(id, this._options.params);
            }
            else {
                this._storeFileForLater(id);
            }
        }
    },
    _storeFileForLater: function (id) {
        this._storedFileIds.push(id);
    },
    _validateFile: function (file) {
        var name, size;

        if (file.value) {
            // it is a file input
            // get input value and remove path to normalize
            name = file.value.replace(/.*(\/|\\)/, "");
        } else {
            // fix missing properties in Safari 4 and firefox 11.0a2
            name = (file.fileName !== null && file.fileName !== undefined) ? file.fileName : file.name;
            size = (file.fileSize !== null && file.fileSize !== undefined) ? file.fileSize : file.size;
        }

        if (!this._isAllowedExtension(name)) {
            this._error('typeError', name);
            return false;

        } else if (size === 0) {
            this._error('emptyError', name);
            return false;

        } else if (size && this._options.sizeLimit && size > this._options.sizeLimit) {
            this._error('sizeError', name);
            return false;

        } else if (size && size < this._options.minSizeLimit) {
            this._error('minSizeError', name);
            return false;
        }

        return true;
    },
    _error: function (code, fileName) {
        var message = this._options.messages[code];
        function r(name, replacement) {
            if (!name) return message;
            message = message.replace(name, replacement);
        }

        var extensions = this._options.allowedExtensions.join(', ');

        r('{file}', this._formatFileName(fileName));
        r('{extensions}', extensions);
        r('{sizeLimit}', this._formatSize(this._options.sizeLimit));
        r('{minSizeLimit}', this._formatSize(this._options.minSizeLimit));
        r('{maxFile}', this._formatFileName(this._options.maxFile));

        this._options.onError(null, fileName, message);
        this._options.showMessage(message);
    },
    _formatFileName: function (name) {
        if (!name) return "";
        if (name.length > 33) {
            name = name.slice(0, 19) + '...' + name.slice(-13);
        }
        return name;
    },
    _isAllowedExtension: function (fileName) {
        var ext = (-1 !== fileName.indexOf('.'))
            ? fileName.replace(/.*[.]/, '').toLowerCase()
            : '';
        var allowed = this._options.allowedExtensions;

        if (!allowed.length) { return true; }

        for (var i = 0; i < allowed.length; i++) {
            if (allowed[i].toLowerCase() == ext) { return true; }
        }

        return false;
    },
    _formatSize: function (bytes) {
        var i = -1;
        do {
            bytes = bytes / 1024;
            i++;
        } while (bytes > 99);

        return Math.max(bytes, 0.1).toFixed(1) + ['kB', 'MB', 'GB', 'TB', 'PB', 'EB'][i];
    },
    _wrapCallbacks: function () {
        var self, safeCallback;

        self = this;

        safeCallback = function (callback, args) {
            try {
                return callback.apply(self, args);
            }
            catch (exception) {
                self.log("Caught " + exception + " in callback: " + callback);
            }
        }

        for (var prop in this._options) {
            if (/^on[A-Z]/.test(prop)) {
                (function () {
                    var oldCallback = self._options[prop];
                    self._options[prop] = function () {
                        return safeCallback(oldCallback, arguments);
                    }
                } ());
            }
        }
    }
};


/**
* Class that creates upload widget with drag-and-drop and file list
* @inherits qq.FileUploaderBasic
*/
qq.FileUploader = function (o) {
    // call parent constructor
    qq.FileUploaderBasic.apply(this, arguments);

    // additional options
    qq.extend(this._options, {
        element: null,
        // if set, will be used instead of qq-upload-list in template
        listElement: null,
        dragText: '拖拽上传',
        extraDropzones: [],
        hideDropzones: true,
        disableDefaultDropzone: false,
        uploadButtonText: '上传',
        cancelButtonText: '取消',
        failUploadText: '上传失败',

        template: '<div class="qq-uploader">' +
            (!this._options.disableDefaultDropzone ? '<div class="qq-upload-drop-area"><span>{dragText}</span></div>' : '') +
            (!this._options.button ? '<div class="qq-upload-button">{uploadButtonText}</div>' : '') +
            (!this._options.listElement ? '<ul class="qq-upload-list"></ul>' : '') +
            '</div>',

        // template for one item in file list
        fileTemplate: '<li>' +
            '<div class="qq-progress-bar"></div>' +
            '<span class="qq-upload-spinner"></span>' +
            '<span class="qq-upload-finished"></span>' +
            '<span class="qq-upload-file"></span>' +
            '<span class="qq-upload-size"></span>' +
            '<a class="qq-upload-cancel" href="#">{cancelButtonText}</a>' +
            '<span class="qq-upload-failed-text">{failUploadtext}</span>' +
            '</li>',

        classes: {
            // used to get elements from templates
            button: 'qq-upload-button',
            drop: 'qq-upload-drop-area',
            dropActive: 'qq-upload-drop-area-active',
            dropDisabled: 'qq-upload-drop-area-disabled',
            list: 'qq-upload-list',
            progressBar: 'qq-progress-bar',
            file: 'qq-upload-file',
            spinner: 'qq-upload-spinner',
            finished: 'qq-upload-finished',
            size: 'qq-upload-size',
            cancel: 'qq-upload-cancel',
            failText: 'qq-upload-failed-text',

            // added to list item <li> when upload completes
            // used in css to hide progress spinner
            success: 'qq-upload-success',
            fail: 'qq-upload-fail',

            successIcon: null,
            failIcon: null
        },
        extraMessages: {
            formatProgress: "{percent}% of {total_size}",
            tooManyFilesError: "You may only drop one file"
        },
        failedUploadTextDisplay: {
            mode: 'default', //default, custom, or none
            maxChars: 50,
            responseProperty: 'error',
            enableTooltip: true
        }
    });
    // overwrite options with user supplied
    qq.extend(this._options, o);
    this._wrapCallbacks();

    qq.extend(this._options.messages, this._options.extraMessages);

    // overwrite the upload button text if any
    // same for the Cancel button and Fail message text
    this._options.template = this._options.template.replace(/\{dragText\}/g, this._options.dragText);
    this._options.template = this._options.template.replace(/\{uploadButtonText\}/g, this._options.uploadButtonText);    
    this._options.fileTemplate = this._options.fileTemplate.replace(/\{cancelButtonText\}/g, this._options.cancelButtonText);
    this._options.fileTemplate = this._options.fileTemplate.replace(/\{failUploadtext\}/g, this._options.failUploadText);

    this._element = this._options.element;
    this._element.innerHTML = this._options.template;
    this._listElement = this._options.listElement || this._find(this._element, 'list');

    this._classes = this._options.classes;

    if (!this._button) {
        this._button = this._createUploadButton(this._find(this._element, 'button'));
    }

    this._bindCancelEvent();
    this._setupDragDrop();

    this._options._loaded = {};
    //
    if (this._options.data) {
        for (var i = 0, len = this._options.data.length; i < len; i++) {
            var item = this._options.data[i];
            if (!item) continue;
            this._options._loaded[item.id] = item.name;
        }
    }

};

// inherit from Basic Uploader
qq.extend(qq.FileUploader.prototype, qq.FileUploaderBasic.prototype);

qq.extend(qq.FileUploader.prototype, {
    clearStoredFiles: function () {
        qq.FileUploaderBasic.prototype.clearStoredFiles.apply(this, arguments);
        this._listElement.innerHTML = "";
    },
    addExtraDropzone: function (element) {
        this._setupExtraDropzone(element);
    },
    removeExtraDropzone: function (element) {
        var dzs = this._options.extraDropzones;
        for (var i in dzs) if (dzs[i] === element) return this._options.extraDropzones.splice(i, 1);
    },
    _leaving_document_out: function (e) {
        return ((qq.chrome() || (qq.safari() && qq.windows())) && e.clientX == 0 && e.clientY == 0) // null coords for Chrome and Safari Windows
            || (qq.firefox() && !e.relatedTarget); // null e.relatedTarget for Firefox
    },
    _storeFileForLater: function (id) {
        qq.FileUploaderBasic.prototype._storeFileForLater.apply(this, arguments);
        var item = this._getItemByFileId(id);
        this._find(item, 'spinner').style.display = "none";
    },
    /**
    * Gets one of the elements listed in this._options.classes
    **/
    _find: function (parent, type) {
        var element = qq.getByClass(parent, this._options.classes[type])[0];
        if (!element) {
            throw new Error('element not found ' + type);
        }

        return element;
    },
    _setupExtraDropzone: function (element) {
        this._options.extraDropzones.push(element);
        this._setupDropzone(element);
    },
    _setupDropzone: function (dropArea) {
        var self = this;

        var dz = new qq.UploadDropZone({
            element: dropArea,
            onEnter: function (e) {
                qq.addClass(dropArea, self._classes.dropActive);
                e.stopPropagation();
            },
            onLeave: function (e) {
                //e.stopPropagation();
            },
            onLeaveNotDescendants: function (e) {
                qq.removeClass(dropArea, self._classes.dropActive);
            },
            onDrop: function (e) {
                if (self._options.hideDropzones) {
                    dropArea.style.display = 'none';
                }
                qq.removeClass(dropArea, self._classes.dropActive);
                if (e.dataTransfer.files.length > 1 && !self._options.multiple) {
                    self._error('tooManyFilesError', "");
                }
                else {
                    self._uploadFileList(e.dataTransfer.files);
                }
            }
        });

        this.addDisposer(function () { dz.dispose(); });

        if (this._options.hideDropzones) {
            dropArea.style.display = 'none';
        }
    },
    _setupDragDrop: function () {
        var self = this;

        if (!this._options.disableDefaultDropzone) {
            var dropArea = this._find(this._element, 'drop');
            this._options.extraDropzones.push(dropArea);
        }

        var dropzones = this._options.extraDropzones;
        var i;
        for (i = 0; i < dropzones.length; i++) {
            this._setupDropzone(dropzones[i]);
        }

        // IE <= 9 does not support the File API used for drag+drop uploads
        // Any volunteers to enable & test this for IE10?
        if (!this._options.disableDefaultDropzone && !qq.ie()) {
            this._attach(document, 'dragenter', function (e) {
                if (qq.hasClass(dropArea, self._classes.dropDisabled)) return;

                dropArea.style.display = 'block';
                for (i = 0; i < dropzones.length; i++) { dropzones[i].style.display = 'block'; }

            });
        }
        this._attach(document, 'dragleave', function (e) {
            // only fire when leaving document out
            if (self._options.hideDropzones && qq.FileUploader.prototype._leaving_document_out(e)) {
                for (i = 0; i < dropzones.length; i++) {
                    dropzones[i].style.display = 'none';
                }
            }
        });
        qq.attach(document, 'drop', function (e) {
            if (self._options.hideDropzones) {
                for (i = 0; i < dropzones.length; i++) {
                    dropzones[i].style.display = 'none';
                }
            }
            e.preventDefault();
        });
    },
    _onSubmit: function (id, fileName) {
        qq.FileUploaderBasic.prototype._onSubmit.apply(this, arguments);
        this._addToList(id, fileName);
    },
    // Update the progress bar & percentage as the file is uploaded
    _onProgress: function (id, fileName, loaded, total) {
        qq.FileUploaderBasic.prototype._onProgress.apply(this, arguments);

        var item = this._getItemByFileId(id);

        if (loaded === total) {
            var cancelLink = this._find(item, 'cancel');
            cancelLink.style.display = 'none';
        }

        var size = this._find(item, 'size');
        size.style.display = 'inline';

        var text;
        var percent = Math.round(loaded / total * 100);

        if (loaded != total) {
            // If still uploading, display percentage
            text = this._formatProgress(loaded, total);
        } else {
            // If complete, just display final size
            text = this._formatSize(total);
        }

        // Update progress bar <span> tag
        this._find(item, 'progressBar').style.width = percent + '%';

        qq.setText(size, text);
    },
    _onComplete: function (id, fileName, result) {
        qq.FileUploaderBasic.prototype._onComplete.apply(this, arguments);

        var item = this._getItemByFileId(id);

        qq.remove(this._find(item, 'progressBar'));

        if (!this._options.disableCancelForFormUploads || qq.UploadHandlerXhr.isSupported()) {
            qq.remove(this._find(item, 'cancel'));
        }
        qq.remove(this._find(item, 'spinner'));

        if (result.success) {
            qq.addClass(item, this._classes.success);
            if (this._classes.successIcon) {
                this._find(item, 'finished').style.display = "inline-block";
                qq.addClass(item, this._classes.successIcon)
            }
            this._options._loaded[result.id] = fileName;
        } else {
            qq.addClass(item, this._classes.fail);
            if (this._classes.failIcon) {
                this._find(item, 'finished').style.display = "inline-block";
                qq.addClass(item, this._classes.failIcon)
            }
            this._controlFailureTextDisplay(item, result);
        }
    },
    _onUpload: function (id, fileName, xhr) {
        qq.FileUploaderBasic.prototype._onUpload.apply(this, arguments);

        var item = this._getItemByFileId(id);

        if (qq.UploadHandlerXhr.isSupported()) {
            this._find(item, 'progressBar').style.display = "block";
        }

        var spinnerEl = this._find(item, 'spinner');
        if (spinnerEl.style.display == "none") {
            spinnerEl.style.display = "inline-block";
        }
    },
    _addToList: function (id, fileName) {
        var item = qq.toElement(this._options.fileTemplate);
        if (this._options.disableCancelForFormUploads && !qq.UploadHandlerXhr.isSupported()) {
            var cancelLink = this._find(item, 'cancel');
            qq.remove(cancelLink);
        }

        item.qqFileId = id;

        var fileElement = this._find(item, 'file');
        qq.setText(fileElement, this._formatFileName(fileName));
        this._find(item, 'size').style.display = 'none';
        if (!this._options.multiple) this._clearList();
        this._listElement.appendChild(item);
    },
    _clearList: function () {
        this._listElement.innerHTML = '';
        this.clearStoredFiles();
    },
    _getItemByFileId: function (id) {
        var item = this._listElement.firstChild;

        // there can't be txt nodes in dynamically created list
        // and we can  use nextSibling
        while (item) {
            if (item.qqFileId == id) return item;
            item = item.nextSibling;
        }
    },
    /**
    * delegate click event for cancel link
    **/
    _bindCancelEvent: function () {
        var self = this,
            list = this._listElement;

        this._attach(list, 'click', function (e) {
            e = e || window.event;
            var target = e.target || e.srcElement;

            if (qq.hasClass(target, self._classes.cancel)) {
                qq.preventDefault(e);

                var item = target.parentNode;
                while (item.qqFileId == undefined) {
                    item = target = target.parentNode;
                }

                self._handler.cancel(item.qqFileId);
                qq.remove(item);
            }
        });
    },
    _formatProgress: function (uploadedSize, totalSize) {
        var message = this._options.messages.formatProgress;
        function r(name, replacement) { message = message.replace(name, replacement); }

        r('{percent}', Math.round(uploadedSize / totalSize * 100));
        r('{total_size}', this._formatSize(totalSize));
        return message;
    },
    _controlFailureTextDisplay: function (item, response) {
        var mode, maxChars, responseProperty, failureReason, shortFailureReason;

        mode = this._options.failedUploadTextDisplay.mode;
        maxChars = this._options.failedUploadTextDisplay.maxChars;
        responseProperty = this._options.failedUploadTextDisplay.responseProperty;

        if (mode === 'custom') {
            var failureReason = response[responseProperty];
            if (failureReason) {
                if (failureReason.length > maxChars) {
                    shortFailureReason = failureReason.substring(0, maxChars) + '...';
                }
                qq.setText(this._find(item, 'failText'), shortFailureReason || failureReason);

                if (this._options.failedUploadTextDisplay.enableTooltip) {
                    this._showTooltip(item, failureReason);
                }
            }
            else {
                this.log("'" + responseProperty + "' is not a valid property on the server response.");
            }
        }
        else if (mode === 'none') {
            qq.remove(this._find(item, 'failText'));
        }
        else if (mode !== 'default') {
            this.log("failedUploadTextDisplay.mode value of '" + mode + "' is not valid");
        }
    },
    //TODO turn this into a real tooltip, with click trigger (so it is usable on mobile devices).  See case #355 for details.
    _showTooltip: function (item, text) {
        item.title = text;
    }
});

qq.UploadDropZone = function (o) {
    this._options = {
        element: null,
        onEnter: function (e) { },
        onLeave: function (e) { },
        // is not fired when leaving element by hovering descendants
        onLeaveNotDescendants: function (e) { },
        onDrop: function (e) { }
    };
    qq.extend(this._options, o);
    qq.extend(this, qq.DisposeSupport);

    this._element = this._options.element;

    this._disableDropOutside();
    this._attachEvents();
};

qq.UploadDropZone.prototype = {
    _dragover_should_be_canceled: function () {
        return qq.safari() || (qq.firefox() && qq.windows());
    },
    _disableDropOutside: function (e) {
        // run only once for all instances
        if (!qq.UploadDropZone.dropOutsideDisabled) {

            // for these cases we need to catch onDrop to reset dropArea
            if (this._dragover_should_be_canceled) {
                qq.attach(document, 'dragover', function (e) {
                    e.preventDefault();
                });
            } else {
                qq.attach(document, 'dragover', function (e) {
                    if (e.dataTransfer) {
                        e.dataTransfer.dropEffect = 'none';
                        e.preventDefault();
                    }
                });
            }

            qq.UploadDropZone.dropOutsideDisabled = true;
        }
    },
    _attachEvents: function () {
        var self = this;

        self._attach(self._element, 'dragover', function (e) {
            if (!self._isValidFileDrag(e)) return;

            var effect = qq.ie() ? null : e.dataTransfer.effectAllowed;
            if (effect == 'move' || effect == 'linkMove') {
                e.dataTransfer.dropEffect = 'move'; // for FF (only move allowed)
            } else {
                e.dataTransfer.dropEffect = 'copy'; // for Chrome
            }

            e.stopPropagation();
            e.preventDefault();
        });

        self._attach(self._element, 'dragenter', function (e) {
            if (!self._isValidFileDrag(e)) return;

            self._options.onEnter(e);
        });

        self._attach(self._element, 'dragleave', function (e) {
            if (!self._isValidFileDrag(e)) return;

            self._options.onLeave(e);

            var relatedTarget = document.elementFromPoint(e.clientX, e.clientY);
            // do not fire when moving a mouse over a descendant
            if (qq.contains(this, relatedTarget)) return;

            self._options.onLeaveNotDescendants(e);
        });

        self._attach(self._element, 'drop', function (e) {
            if (!self._isValidFileDrag(e)) return;

            e.preventDefault();
            self._options.onDrop(e);
        });
    },
    _isValidFileDrag: function (e) {
        // e.dataTransfer currently causing IE errors
        // IE9 does NOT support file API, so drag-and-drop is not possible
        // IE10 should work, but currently has not been tested - any volunteers?
        if (qq.ie()) return false;

        var dt = e.dataTransfer,
        // do not check dt.types.contains in webkit, because it crashes safari 4
            isSafari = qq.safari();

        // dt.effectAllowed is none in Safari 5
        // dt.types.contains check is for firefox
        return dt && dt.effectAllowed != 'none' &&
            (dt.files || (!isSafari && dt.types.contains && dt.types.contains('Files')));

    }
};

qq.UploadButton = function (o) {
    this._options = {
        element: null,
        // if set to true adds multiple attribute to file input
        multiple: false,
        acceptFiles: null,
        // name attribute of file input
        name: 'file',
        onChange: function (input) { },
        hoverClass: 'qq-upload-button-hover',
        focusClass: 'qq-upload-button-focus'
    };

    qq.extend(this._options, o);
    qq.extend(this, qq.DisposeSupport);

    this._element = this._options.element;

    // make button suitable container for input
    qq.css(this._element, {
        position: 'relative',
        overflow: 'hidden',
        // Make sure browse button is in the right side
        // in Internet Explorer
        direction: 'ltr'
    });

    this._input = this._createInput();
};

qq.UploadButton.prototype = {
    /* returns file input element */
    getInput: function () {
        return this._input;
    },
    /* cleans/recreates the file input */
    reset: function () {
        if (this._input.parentNode) {
            qq.remove(this._input);
        }

        qq.removeClass(this._element, this._options.focusClass);
        this._input = this._createInput();
    },
    _createInput: function () {
        var input = document.createElement("input");

        if (this._options.multiple) {
            input.setAttribute("multiple", "multiple");
        }

        if (this._options.acceptFiles) input.setAttribute("accept", this._options.acceptFiles);

        input.setAttribute("type", "file");
        input.setAttribute("name", this._options.name);

        qq.css(input, {
            position: 'absolute',
            // in Opera only 'browse' button
            // is clickable and it is located at
            // the right side of the input
            right: 0,
            top: 0,
            fontFamily: 'Arial',
            // 4 persons reported this, the max values that worked for them were 243, 236, 236, 118
            fontSize: '118px',
            margin: 0,
            padding: 0,
            cursor: 'pointer',
            opacity: 0
        });

        this._element.appendChild(input);

        var self = this;
        this._attach(input, 'change', function () {
            self._options.onChange(input);
        });

        this._attach(input, 'mouseover', function () {
            qq.addClass(self._element, self._options.hoverClass);
        });
        this._attach(input, 'mouseout', function () {
            qq.removeClass(self._element, self._options.hoverClass);
        });
        this._attach(input, 'focus', function () {
            qq.addClass(self._element, self._options.focusClass);
        });
        this._attach(input, 'blur', function () {
            qq.removeClass(self._element, self._options.focusClass);
        });

        // IE and Opera, unfortunately have 2 tab stops on file input
        // which is unacceptable in our case, disable keyboard access
        if (window.attachEvent) {
            // it is IE or Opera
            input.setAttribute('tabIndex', "-1");
        }

        return input;
    }
};

/**
* Class for uploading files, uploading itself is handled by child classes
*/
qq.UploadHandlerAbstract = function (o) {
    // Default options, can be overridden by the user
    this._options = {
        debug: false,
        action: '/upload.php',
        // maximum number of concurrent uploads
        maxFile: 999,
        onProgress: function (id, fileName, loaded, total) { },
        onComplete: function (id, fileName, response) { },
        onCancel: function (id, fileName) { },
        onUpload: function (id, fileName, xhr) { }
    };
    qq.extend(this._options, o);

    this._queue = [];

    // params for files in queue
    this._params = {};
};
qq.UploadHandlerAbstract.prototype = {
    log: function (str) {
        if (this._options.debug && window.console) console.log('[uploader] ' + str);
    },
    /**
    * Adds file or file input to the queue
    * @returns id
    **/
    add: function (file) { },
    /**
    * Sends the file identified by id and additional query params to the server
    */
    upload: function (id, params) {
        var len = this._queue.push(id);

        var copy = {};
        qq.extend(copy, params);
        this._params[id] = copy;

        // if too many active uploads, wait...
        if (len <= this._options.maxFile) {
            this._upload(id, this._params[id]);
        }
    },
    /**
    * Cancels file upload by id
    */
    cancel: function (id) {
        this._cancel(id);
        this._dequeue(id);
    },
    /**
    * Cancells all uploads
    */
    cancelAll: function () {
        for (var i = 0; i < this._queue.length; i++) {
            this._cancel(this._queue[i]);
        }
        this._queue = [];
    },
    /**
    * Returns name of the file identified by id
    */
    getName: function (id) { },
    /**
    * Returns size of the file identified by id
    */
    getSize: function (id) { },
    /**
    * Returns id of files being uploaded or
    * waiting for their turn
    */
    getQueue: function () {
        return this._queue;
    },
    /**
    * Actual upload method
    */
    _upload: function (id) { },
    /**
    * Actual cancel method
    */
    _cancel: function (id) { },
    /**
    * Removes element from queue, starts upload of next
    */
    _dequeue: function (id) {
        var i = qq.indexOf(this._queue, id);
        this._queue.splice(i, 1);

        var max = this._options.maxFile;

        if (this._queue.length >= max && i < max) {
            var nextId = this._queue[max - 1];
            this._upload(nextId, this._params[nextId]);
        }
    }
};

/**
* Class for uploading files using form and iframe
* @inherits qq.UploadHandlerAbstract
*/
qq.UploadHandlerForm = function (o) {
    qq.UploadHandlerAbstract.apply(this, arguments);

    this._inputs = {};
    this._detach_load_events = {};
};
// @inherits qq.UploadHandlerAbstract
qq.extend(qq.UploadHandlerForm.prototype, qq.UploadHandlerAbstract.prototype);

qq.extend(qq.UploadHandlerForm.prototype, {
    add: function (fileInput) {
        fileInput.setAttribute('name', this._options.inputName);
        var id = 'qq-upload-handler-iframe' + qq.getUniqueId();

        this._inputs[id] = fileInput;

        // remove file input from DOM
        if (fileInput.parentNode) {
            qq.remove(fileInput);
        }

        return id;
    },
    getName: function (id) {
        // get input value and remove path to normalize
        return this._inputs[id].value.replace(/.*(\/|\\)/, "");
    },
    _cancel: function (id) {
        this._options.onCancel(id, this.getName(id));

        delete this._inputs[id];
        delete this._detach_load_events[id];

        var iframe = document.getElementById(id);
        if (iframe) {
            // to cancel request set src to something else
            // we use src="javascript:false;" because it doesn't
            // trigger ie6 prompt on https
            iframe.setAttribute('src', 'javascript:false;');

            qq.remove(iframe);
        }
    },
    _upload: function (id, params) {
        this._options.onUpload(id, this.getName(id), false);
        var input = this._inputs[id];

        if (!input) {
            throw new Error('file with passed id was not added, or already uploaded or cancelled');
        }

        var fileName = this.getName(id);
        params[this._options.inputName] = fileName;

        var iframe = this._createIframe(id);
        var form = this._createForm(iframe, params);
        form.appendChild(input);

        var self = this;
        this._attachLoadEvent(iframe, function () {
            self.log('iframe loaded');

            var response = self._getIframeContentJSON(iframe);

            if (response.extraJs) jv.execJs(response.extraJs);

            self._options.onComplete(id, fileName, response);
            self._dequeue(id);

            delete self._inputs[id];
            // timeout added to fix busy state in FF3.6
            setTimeout(function () {
                self._detach_load_events[id]();
                delete self._detach_load_events[id];
                qq.remove(iframe);
            }, 1);
        });

        form.submit();
        qq.remove(form);

        return id;
    },
    _attachLoadEvent: function (iframe, callback) {
        this._detach_load_events[iframe.id] = qq.attach(iframe, 'load', function () {
            // when we remove iframe from dom
            // the request stops, but in IE load
            // event fires
            if (!iframe.parentNode) {
                return;
            }

            try {
                // fixing Opera 10.53
                if (iframe.contentDocument &&
                    iframe.contentDocument.body &&
                    iframe.contentDocument.body.innerHTML == "false") {
                    // In Opera event is fired second time
                    // when body.innerHTML changed from false
                    // to server response approx. after 1 sec
                    // when we upload file with iframe
                    return;
                }
            }
            catch (error) {
                //IE may throw an "access is denied" error when attempting to access contentDocument on the iframe in some cases
            }

            callback();
        });
    },
    /**
    * Returns json object received by iframe from server.
    */
    _getIframeContentJSON: function (iframe) {
        //IE may throw an "access is denied" error when attempting to access contentDocument on the iframe in some cases
        try {
            // iframe.contentWindow.document - for IE<7
            var doc = iframe.contentDocument ? iframe.contentDocument : iframe.contentWindow.document,
                response;

            var innerHTML = doc.body.innerHTML;
            this.log("converting iframe's innerHTML to JSON");
            this.log("innerHTML = " + innerHTML);
            //plain text response may be wrapped in <pre> tag
            if (innerHTML.slice(0, 5).toLowerCase() == '<pre>' && innerHTML.slice(-6).toLowerCase() == '</pre>') {
                innerHTML = doc.body.firstChild.firstChild.nodeValue;
            }
            response = eval("(" + innerHTML + ")");
        } catch (err) {
            response = { success: false };
        }

        return response;
    },
    /**
    * Creates iframe with unique name
    */
    _createIframe: function (id) {
        // We can't use following code as the name attribute
        // won't be properly registered in IE6, and new window
        // on form submit will open
        // var iframe = document.createElement('iframe');
        // iframe.setAttribute('name', id);

        var iframe = qq.toElement('<iframe src="javascript:false;" name="' + id + '" />');
        // src="javascript:false;" removes ie6 prompt on https

        iframe.setAttribute('id', id);

        iframe.style.display = 'none';
        document.body.appendChild(iframe);

        return iframe;
    },
    /**
    * Creates form, that will be submitted to iframe
    */
    _createForm: function (iframe, params) {
        // We can't use the following code in IE6
        // var form = document.createElement('form');
        // form.setAttribute('method', 'post');
        // form.setAttribute('enctype', 'multipart/form-data');
        // Because in this case file won't be attached to request
        var protocol = this._options.demoMode ? "GET" : "POST"
        var form = qq.toElement('<form method="' + protocol + '" enctype="multipart/form-data"></form>');

        var queryString = qq.obj2url(params, this._options.action);

        form.setAttribute('action', queryString);
        form.setAttribute('target', iframe.name);
        form.style.display = 'none';
        document.body.appendChild(form);

        return form;
    }
});

/**
* Class for uploading files using xhr
* @inherits qq.UploadHandlerAbstract
*/
qq.UploadHandlerXhr = function (o) {
    qq.UploadHandlerAbstract.apply(this, arguments);

    this._files = [];
    this._xhrs = [];

    // current loaded size in bytes for each file
    // udi 修改为： 加载了的文件信息。
    //this._loaded = {};
};

// static method
qq.UploadHandlerXhr.isSupported = function () {
    var input = document.createElement('input');
    input.type = 'file';

    return (
        'multiple' in input &&
            typeof File != "undefined" &&
            typeof FormData != "undefined" &&
            typeof (new XMLHttpRequest()).upload != "undefined");
};

// @inherits qq.UploadHandlerAbstract
qq.extend(qq.UploadHandlerXhr.prototype, qq.UploadHandlerAbstract.prototype)

qq.extend(qq.UploadHandlerXhr.prototype, {
    /**
    * Adds file to the queue
    * Returns id to use with upload, cancel
    **/
    add: function (file) {
        if (!(file instanceof File)) {
            throw new Error('Passed obj in not a File (in qq.UploadHandlerXhr)');
        }

        return this._files.push(file) - 1;
    },
    getName: function (id) {
        var file = this._files[id];
        // fix missing name in Safari 4
        //NOTE: fixed missing name firefox 11.0a2 file.fileName is actually undefined
        return (file.fileName !== null && file.fileName !== undefined) ? file.fileName : file.name;
    },
    getSize: function (id) {
        var file = this._files[id];
        return file.fileSize != null ? file.fileSize : file.size;
    },
    /**
    * Returns uploaded bytes for file identified by id
   
    getLoaded: function (id) {
    return this._loaded[id] || 0;
    }, */
    /**
    * Sends the file identified by id and additional query params to the server
    * @param {Object} params name-value string pairs
    */
    _upload: function (id, params) {
        this._options.onUpload(id, this.getName(id), true);

        var file = this._files[id],
            name = this.getName(id),
            size = this.getSize(id);


        var xhr = this._xhrs[id] = new XMLHttpRequest();
        var self = this;

        xhr.upload.onprogress = function (e) {
            if (e.lengthComputable) {
                //self._loaded[id] = e.loaded;
                self._options.onProgress(id, name, e.loaded, e.total);
            }
        };

        var input = jv.getDoer();
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4) {
                self._onComplete(id, xhr, { originalEvent: true, target: input });
            }
        };

        // build query string
        params = params || {};
        params[this._options.inputName] = name;

        params['sizeLimit'] = this._options.sizeLimit;

        var queryString = qq.obj2url(params, this._options.action);

        var protocol = this._options.demoMode ? "GET" : "POST";
        xhr.open(protocol, queryString, true);
        xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
        xhr.setRequestHeader("X-File-Name", encodeURIComponent(name));
        xhr.setRequestHeader("Cache-Control", "no-cache");
        if (this._options.forceMultipart) {
            var formData = new FormData();
            formData.append(this._options.inputName, file);
            file = formData;
        } else {
            xhr.setRequestHeader("Content-Type", "application/octet-stream");
            //NOTE: return mime type in xhr works on chrome 16.0.9 firefox 11.0a2
            xhr.setRequestHeader("X-Mime-Type", file.type);
        }
        for (key in this._options.customHeaders) {
            xhr.setRequestHeader(key, this._options.customHeaders[key]);
        };
        xhr.send(file);
    },
    _onComplete: function (id, xhr) {
        //"use strict";
        // the request was aborted/cancelled
        if (!this._files[id]) { return; }

        var name = this.getName(id);
        var size = this.getSize(id);
        var response; //the parsed JSON response from the server, or the empty object if parsing failed.

        //this._options.onProgress(id, name, size, size);

        this.log("xhr - server response received");
        this.log("responseText = " + xhr.responseText);

        try {
            if (typeof JSON.parse === "function") {
                response = JSON.parse(xhr.responseText);
            } else {
                response = eval("(" + xhr.responseText + ")");
            }

        } catch (err) {
            response = {};
        }

        try {
            if (response.extraJs) { jv.execJs(response.extraJs); }
        }
        catch (e) { }


        if (xhr.status !== 200) {
            this._options.onError(id, name, "XHR returned response code " + xhr.status);
        }

        this._options.onComplete(id, name, response);

        this._xhrs[id] = null;
        this._dequeue(id);
    },
    _cancel: function (id) {
        this._options.onCancel(id, this.getName(id));

        this._files[id] = null;

        if (this._xhrs[id]) {
            this._xhrs[id].abort();
            this._xhrs[id] = null;
        }
    }
});

/**
* A generic module which supports object disposing in dispose() method.
* */
qq.DisposeSupport = {
    _disposers: [],

    /** Run all registered disposers */
    dispose: function () {
        var disposer;
        while (disposer = this._disposers.shift()) {
            disposer();
        }
    },

    /** Add disposer to the collection */
    addDisposer: function (disposeFunction) {
        this._disposers.push(disposeFunction);
    },

    /** Attach event handler and register de-attacher as a disposer */
    _attach: function () {
        this.addDisposer(qq.attach.apply(this, arguments));
    }
};

/*查看文件控件
调用方式:
<div id="fileView" />

$("#fileView").ListFile(
{
data: [
{ name: "文件一", icon: "w", id: 1, url: "/Admin/File.aspx?id=1" },
{ name: "文件二", icon: "", id: 2, url: "/Admin/File.aspx?id=3" }
],
edit: true,
removeOKCallback: function (deleteIds,Ids,) { info("Oaldksfj;lkasdjfasdfasdfwwer wrejlk23o4  h2kj3h4lk23h4kl32 jh4lk23jh4lk23jh4kjlK"); ; }
});

生成如下内容:
<span><span class="Link File" ><img src="ico" />文件一</span><span class="DelBtn">删除<span></span>    绑定数据 file : { name:"文件一", icon: "" , url: "Admin/File.aspx?id=1"}
<span><span class="Link File" ><img src="ico" />文件二</span><span class="DelBtn">删除<span></span>    绑定数据 file : { name:"文件二", icon: "" , url: "Admin/File.aspx?id=3"}

 
*/


//暴露的API对象。传入关键对象后，Render。
function ListFile(elem, options) {
    //设置公共属性
    options = ListFile.GenOption(options);

    var jcon = $(jv.getType(elem) == "string" ? "#" + elem : elem);

    jcon.addClass("ListFile").empty().data("listFile", this);

    $(options.data).each(function (i, d) {
        if (i > 0) jcon.append(options.separator || " , ");
        jcon.append(ListFile.CreateItem(options, d));
    });

    var okBtn = $('<button style="display:none">确定</button>');
    jcon.append(okBtn);

    okBtn.click(function () {
        if (p.removeOKCallback) {
            var ids = [];
            $(this).parent().find(".FileItem.DelFile").each(function (i, d) {
                ids.push($(d).attr("dbid"));
            });

            var allIds = ListFile.GetIds(jcon);
            jcon.find(">input[name=" + options.hidden + "]").val(allIds.join(","));

            p.removeOKCallback(ids, allIds);
        }
    });

    if (!jcon.find(">input[name=" + options.hidden + "]").length) {
        var hid = document.createElement("input");
        hid["type"] = "hidden";
        hid["name"] = options.hidden;
        jcon.append(hid);
    }
};

ListFile.GenOption = function (option) {
    return $.extend({
        data: false,
        canRecv: false,
        //            callback: function (name, url) {
        //                document.location = jv.url(url).href;
        //            },
        edit: false,
        link: "{url}",
        target: "blank",
        removeOKCallback: function (deleteIds, Ids) { },
        removeCancel: function (data) { }
    }, option);
};
//静态方法当作 API
ListFile.Get = function (elem) {
    return ListFile.GetContainer(elem).data("listFile");
}

ListFile.GetIds = function (elem) {
    var ids = [];
    var jelem = $(jv.getType(elem) == "string" ? "#" + elem : elem);

    jelem.find(".FileItem").each(function (i, d) {
        var jd = $(d);
        if (jd.hasClass("DelFile")) return;
        var dbid = jd.attr("dbid");
        if (dbid !== "") {
            ids.push(dbid);
        }
    });
    return ids;
};

//与UI相关的API.
ListFile.GetContainer = function (elem) {
    var jelem = $(jv.getType(elem) == "string" ? "#" + elem : elem);

    return jelem.closest(".ListFile");
};

ListFile.CreateLink = function (p, eachDefine) {
    p = ListFile.GenOption(p);
    var jcon = $(document.createElement("a"));
    jcon.attr("target", "_blank");
    if (p.link) { jcon.attr("href", jv.url(p.link.formatEx(eachDefine)).toString()); }

    if (eachDefine.icon) {
        jcon.append($('<img src="' + jv.url(eachDefine.icon).href + '" />'));
    }

    jcon.append(eachDefine.name);

    if (p.callback) {
        jcon.click(function () {
            p.callback(eachDefine);
        });
    }

    return jcon;
};
ListFile.SyncDisplay = function (ekem) {
    var container = ListFile.GetContainer(elem);
    var hasDelete = false;
    if (container.find(".FileItem.DelFile").length > 0) {
        container.find("button").show();
    }
    else {
        container.find("button").hide();
    }
};


ListFile.CreateItem = function (p, eachDefine) {
    p = ListFile.GenOption(p);
    var jcon = $('<span class="FileItem" dbid="' + eachDefine.id + '" qqfileid="' + ((eachDefine.qqFileId === 0 || eachDefine.qqFileId) ? eachDefine.qqFileId : "") + '"></span>');
    jcon.append(ListFile.CreateLink(p, eachDefine));
    if (p.edit) {
        jcon.append(ListFile.CreateDel(p, eachDefine));
    }
    return jcon;
};

ListFile.AppendItem = function (elem, p, resJson) {
    p = ListFile.GenOption(p);
    var jelem = $(jv.getType(elem) == "string" ? "#" + elem : elem);
    if (resJson.id) {
        var jbtn = jelem.find("button");
        if (jelem.find(".FileItem").length) {
            $(document.createTextNode(p.separator || " , ")).insertBefore(jbtn);
        }
        ListFile.CreateItem(p, resJson).insertBefore(jbtn);
    }


    var allIds = ListFile.GetIds(elem);
    jelem.find(">input[name=" + p.hidden + "]").val(allIds.join(","));
};


ListFile.CreateDel = function (p, eachDefine) {
    p = ListFile.GenOption(p);
    if (!p.edit) return null;
    var jcon = $('<div class="DelBtn Inline Icon16"></div>');

    jcon.click(function () {
        var jme = $(this);
        var jelem = ListFile.GetContainer(jme);

        if (p.canRecv) {
            var pn = jme.parent();
            if (pn.hasClass("DelFile")) {
                pn.removeClass("DelFile");
                ListFile.SyncDisplay(this);
            }
            else {
                pn.addClass("DelFile");
                ListFile.GetContainer(this).find("button").show();
            }
        }
        else {
            var comma = this.parentNode.previousSibling;
            if (!comma || (comma.nodeType != 3)) {
                comma = this.parentNode.nextSibling;
            }

            if (comma && (comma.nodeType == 3)) comma.nodeValue = "";

            var spanItem = jv.getParentTag(this, "SPAN");
            var listFile = jv.getParentTag(spanItem, "DIV");

            p.upFile._remove(spanItem.getAttribute("dbId"));
            $(spanItem).remove();

        }

        var allIds = ListFile.GetIds(jelem);
        jelem.find(">input[name=" + p.hidden + "]").val(allIds.join(","));

    });
    return jcon;
};
function TableFile(listFile, p) {
    var thead = document.createElement("thead");
    listFile.append(thead);

    listFile.append(document.createElement("tbody"));

    listFile.addClass("ListFile");

    var tr = document.createElement("tr");
    thead.appendChild(tr);

    {
        var th = document.createElement("th");
        tr.appendChild(th);

        var hid = document.createElement("input");
        hid["type"] = "hidden";
        hid["name"] = p.hidden;
        th.appendChild(hid);


        th.appendChild(hid);
        th.style.display = "none";
    }
    {
        var th = document.createElement("th");
        th.innerHTML = "文件说明";
        th.className = "fileName";
        tr.appendChild(th);
    }

    {
        var th = document.createElement("th");
        th.innerHTML = "大小（KB）";
        tr.appendChild(th);
    }

    {
        var th = document.createElement("th");
        th.innerHTML = "[下载]";
        tr.appendChild(th);
    }

    if (p.edit) {
        var th = document.createElement("th");
        th.innerHTML = "[删除]";
        tr.appendChild(th);
    }

    if (p.data) {
        for (var i = 0, length = p.data.length; i < length; i++) {
            var item = p.data[i];
            TableFile.AppendItem(listFile, p, item);
        }
    }
};

TableFile.AppendItem = function (table, p, res) {

    var tr = document.createElement("tr");
    tr.id = "row" + res.id;
    tr.setAttribute("qqFileId", res.qqFileId);

    $(table).find("tbody").append(tr);

    {
        var th = document.createElement("td");
        th.style.display = "none";
        th.innerHTML = res.id;
        tr.appendChild(th);
    }

    {
        var th = document.createElement("td");
        th.innerHTML = res.name;
        tr.appendChild(th);
    }

    {
        var th = document.createElement("td");
        th.innerHTML = parseInt(parseInt(res.size) / 1024);
        tr.appendChild(th);
    }

    {
        var th = document.createElement("td");
        th.innerHTML = '<a href="' + res.url + '" target="_blank"><div class="Inline DownloadIcon Icon16"></div></a>';
        tr.appendChild(th);
    }

    if (p.edit) {
        var th = document.createElement("td");
        th.innerHTML = '<div class="Inline DeleteIcon Icon16" onclick="javascript:return TableFile.DeleteFile(\'' + p.hidden + '\',event);" ></div>';
        tr.appendChild(th);
    }

    var hid = $("input[name=" + p.hidden + "]", table);
    var v = hid.val().mySplit(",", true);
    v.push(res.id);
    hid.val(v.join(","));
};

TableFile.DeleteFile = function (hidName, ev) {
    if (!confirm("确认删除该文件吗？")) return false;
    var jtr = $(jv.GetDoer()).closest("tr");
    var jtable = jtr.closest("table");
    var hid = jtable.find("thead tr th input[name=" + hidName + "]");
    var v = hid.val().mySplit(",", true);
    v.remove(jtr.attr("id").slice(3));
    hid.val(v.join(","));
    //这可能有问题，未测试，By Udi，at： 2013年4月18日
    this.upFile._remove(jtr.attr("qqFileId"));
    jtr.remove();
};