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