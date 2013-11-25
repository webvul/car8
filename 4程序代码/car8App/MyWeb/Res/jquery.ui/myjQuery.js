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
