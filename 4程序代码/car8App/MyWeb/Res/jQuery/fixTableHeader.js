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