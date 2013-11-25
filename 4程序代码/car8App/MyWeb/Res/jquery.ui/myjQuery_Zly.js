(function ($) {
    $.datepicker.dpDiv.hide();
    $.datepicker.regional['zh-CN'] = {
        showAnim: "slideDown",
        changeYear: true,
        showButtonPanel: true,
        closeText: '确定',
        prevText: '&#x3c;上月',
        nextText: '下月&#x3e;',
        currentText: '今天',
        monthNames: ['一月', '二月', '三月', '四月', '五月', '六月',
		'七月', '八月', '九月', '十月', '十一月', '十二月'],
        monthNamesShort: ['一', '二', '三', '四', '五', '六',
		'七', '八', '九', '十', '十一', '十二'],
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
    //    zeng.ly 2011-05-23 begin    
    /** 2种调用方式，默认不显示时刻
    $("#txtdate").datepicker($.extend($.datepicker._defaults,{showTime:true})); //显示时刻
    $("#txtdate").datepicker(); //默认调用不显示时刻
    **/

    if ($.datepicker._defaults.showTime == null) {
        $.datepicker._defaults = $.extend({
            showTime: false
        }, $.datepicker._defaults); //是否需要显示时刻
    }

    //重写_selectDate、_gotoToday、_attachments 方法
    $.datepicker._selectDate = function (id, dateStr) {

        var THIS = $.datepicker,
        target = $(id),
        inst = THIS._getInst(target[0]);

        dateStr = (dateStr != null ? dateStr : THIS._formatDate(inst));
        //解析日期
        if (dateStr == "") {
            var date = new Date();
            inst.selectedDay = date.getDate();
            inst.drawMonth = inst.selectedMonth = date.getMonth();
            inst.drawYear = inst.selectedYear = date.getFullYear();
            dateStr = $.datepicker._formatDate(inst, inst.selectDay, inst.selectMonth, inst.selectYear);
        }
        //解析时刻
        if (inst.input) {
            var newStr = dateStr, _time = { hour: "00", minute: "00" };
            if ($.datepicker._defaults.showTime) {
                var arr = $(".liyan_txt");
                $.each(arr, function (i, item) {
                    var _item = $(item).val(), _value = 0;
                    if (!isNaN(_item)) {
                        _value = parseInt(_item, 10);
                        if (i == 0) { //hour
                            if (_value < 0 || _value > 23) {
                                _value = 0;
                            }
                            if (_value < 10) {
                                _time.hour = "0" + _value;
                            }
                            else {
                                _time.hour = _value;
                            }
                        }
                        else { //minute
                            if (_value < 0 || _value > 59) {
                                _value = 0;
                            }
                            if (_value < 10) {
                                _time.minute = "0" + _value;
                            }
                            else {
                                _time.minute = _value;
                            }
                        }
                    }
                });
                newStr += " " + _time.hour + ":" + _time.minute;
            }

            inst.input.val(newStr);
        }
        THIS._updateAlternate(inst);
        var onSelect = THIS._get(inst, 'onSelect');
        if (onSelect) {
            onSelect.apply((inst.input ? inst.input[0] : null), [dateStr, inst]);
        }
        else if (inst.input) {
            inst.input.trigger('change');
        }
        if (inst.inline) {
            THIS._updateDatepicker(inst);
        }
        else {
            THIS._hideDatepicker();
            THIS._lastInput = inst.input[0];
            if (typeof (inst.input[0]) != 'object') {
                inst.input.focus();
            }
            THIS._lastInput = null;
        }
    };

    $.datepicker._gotoToday = function (id) {
        var THIS = $.datepicker;
        var target = $(id);
        var inst = THIS._getInst(target[0]);
        if (THIS._get(inst, 'gotoCurrent') && inst.currentDay) {
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
        //THIS._notifyChange(inst);
        //THIS._adjustDate(target); 
        var ii = $.datepicker._formatDate(inst, inst.selectDay, inst.selectMonth, inst.selectYear);
        THIS._selectDate(id, ii);
    };

    $.datepicker._attachments = function (input, inst) {
        var THIS = $.datepicker;
        var appendText = THIS._get(inst, 'appendText');
        var isRTL = THIS._get(inst, 'isRTL');
        if (inst.append)
            inst.append.remove();
        if (appendText) {
            inst.append = $('<span class="' + _this._appendClass + '">' + appendText + '</span>');
            input[isRTL ? 'before' : 'after'](inst.append);
        }
        if (THIS._defaults.showTime) {
            input.unbind("focus", THIS._showtimeDatepicker);
        }
        else {
            input.unbind('focus', THIS._showDatepicker);
        }
        if (inst.trigger)
            inst.trigger.remove();
        var showOn = THIS._get(inst, 'showOn');
        if (showOn == 'focus' || showOn == 'both') // pop-up date picker when in the marked field
        {
            if (THIS._defaults.showTime) {
                input.focus(this._showtimeDatepicker);
            }
            else {
                input.focus(this._showDatepicker);
            }
        }
        if (showOn == 'button' || showOn == 'both') { // pop-up date picker when button clicked
            var buttonText = THIS._get(inst, 'buttonText');
            var buttonImage = THIS._get(inst, 'buttonImage');
            inst.trigger = $(THIS._get(inst, 'buttonImageOnly') ?
				$('<img/>').addClass(THIS._triggerClass).
					attr({ src: buttonImage, alt: buttonText, title: buttonText }) :
				$('<button type="button"></button>').addClass(_this._triggerClass).
					html(buttonImage == '' ? buttonText : $('<img/>').attr(
					{ src: buttonImage, alt: buttonText, title: buttonText })));
            input[isRTL ? 'before' : 'after'](inst.trigger);
            inst.trigger.click(function () {
                if ($.datepicker._datepickerShowing && $.datepicker._lastInput == input[0])
                    $.datepicker._hideDatepicker();
                else {
                    if (THIS._defaults.showTime) {
                        $.datepicker._showtimeDatepicker(input[0]);
                    }
                    else {
                        $.datepicker._showDatepicker(input[0]);
                    }
                }
                return false;
            });
        }
    };
    //新增显示时刻的方法
    $.datepicker._showtimeDatepicker = function (input) {
        $.datepicker._showDatepicker(input);
        if ($.datepicker._defaults.showTime) {
            var THIS = $(this), arr = THIS.val().split(" "), _time = { hour: "00", minute: "00" };
            $.each(arr, function (i, item) {
                if (i > 0) {
                    var arrItem = item.split(":");
                    $.each(arrItem, function (k, childitem) {
                        if (k == 0) {
                            _time.hour = childitem;
                        }
                        else {
                            _time.minute = childitem;
                        }
                    });
                }
            });

            $(".date_liyan").html("");
            $(".ui-datepicker-buttonpane").html('<button onclick="$.datepicker._gotoToday(\'#' + THIS[0].id + '\')" class="ui-datepicker-current ui-state-default ui-priority-secondary ui-corner-all" type="button">' + $.datepicker._defaults.currentText + '</button><button onclick="$.datepicker._selectDate(\'#' + THIS[0].id + '\');" class="ui-datepicker-close ui-state-default ui-priority-primary ui-corner-all" type="button">' + $.datepicker._defaults.closeText + '</button>');
            $(".ui-datepicker-calendar", $.datepicker.dpDiv).after('<div class="date_liyan"> 时间：<input style="width:40px; text-align:center" value="' + _time.hour + '" class="liyan_txt" maxlength="2"/> 时 <input style="width:40px; text-align:center" value="' + _time.minute + '" class="liyan_txt" maxlength="2"/> 分 </div>');
        }
    };
    //    zeng.ly 2011-05-23 end    
})(jQuery);
