(function () {

    //本页标志性含数 getDoer .
    if (jv.popPrint) return;

    jv.PopPrint = jv.popPrint = function (urlSetting) {
        urlSetting = $.extend({ url: jv.Root + "ReportWeb/" + jv.page().controller + "/" + jv.page().action + ".aspx", query: false }, urlSetting);
        if (urlSetting.query) {
            urlSetting.url += "?" + urlSetting.query;
        }

        jv.PopDetail({ url: urlSetting.url, data: { code: true } });
        return false;
    };

    jv.PopSearch = jv.popSearch = function (querySelector) {
        if (querySelector) {
            window.PopSearch = function () { $(querySelector).trigger("click"); };
        }

        jv.PopDetail({ url: jv.Root + "html/PopSearch.html", data: { code: true }, entity: "PopSearch" });
        return false;
    };


})();