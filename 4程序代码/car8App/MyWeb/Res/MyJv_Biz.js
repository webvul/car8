(function () {

    //本页标志性含数 getDoer .
    if (jv.FindPopConfig) return;

    jv.FindPopConfig = jv.findPopConfig = function (Dict, area, controller, action) {
        action = action || "";

        //额外的可以配置指定的List
        //Dict.cs["TContract/List"] = [600,0] ;

        //如果area 不存在,在根路径下找. 
        //参数方式: cs,Common,List
        //cs,Common
        //Common

        if (!controller && !action && area) {
            controller = area;
            area = "";
        }
        Dict[area] = Dict[area] || {};
        if (area) {
            return Dict[area][controller + "/" + action] || Dict[area][controller] || Dict[controller] || [];
        }
        else {
            for (var p in Dict) {
                return Dict[controller + "/" + action] || Dict[controller] || []
            }
        }

        return [];
    }
    jv.PopListConfig = jv.popListConfig = function (area, controller, action) {
        // 正数代表 宽度, 高度, 0 代表自适应.  负数代表 左右边距, 上下边距
        //第三个参数，表示打开方式： true：window.open, false：   window.showModelDialog (默认值) , 《Boxy：过滤器》： 使用Boxy 打开  , 字符串：open的name。
        var Dict = { App: {}, Admin: {}, Report: {}, Master: {}, Cost: {}, Property: {}, Sys: {} };
        Dict.Admin["Role"] = [350, -50];

        Dict.App["TRegion"] = [500, 0];

        Dict.Master["Pop/CostItem"] = [500, -50];
        Dict.Master["Pop/BuildRoomList"] = [800, -50];

        Dict["Notices"] = [-1, -1, true];
        Dict["BathRoom"] = [700, 425];

        return jv.FindPopConfig(Dict, area, controller, action);
    };

    jv.PopDetailConfig = jv.popDetailConfig = function (area, controller, action) {
        // 正数代表 宽度, 高度, 0 代表自适应.  负数代表 左右边距, 上下边距
        //第三个参数，表示打开方式： true：window.open, false：   window.showModelDialog (默认值) , 《Boxy：过滤器》： 使用Boxy 打开  , 字符串：open的name。
        var Dict = { App: {}, Admin: {}, Report: {}, cs: {} };
        Dict.Admin["Role"] = [550, 250, ".Main"];
        Dict.Admin["Menu"] = [710, 450, ""];

        Dict.App["TRegion"] = [650, 250, ".Main"];
        Dict["PopSearch"] = [450, 450];
        Dict["QuerySet"] = [605, 433];
        Dict["PopFeeDelReason"] = [450, 350];

        Dict["Recp"] = [900, 700, ""];
        Dict["Notices"] = [-1, -1, true];


        Dict["Power"] = [-50, -50, true];
        return jv.FindPopConfig(Dict, area, controller, action);
    };


})();