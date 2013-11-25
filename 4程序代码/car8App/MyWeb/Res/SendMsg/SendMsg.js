$(function () {
    jv.SendMsg = function (selector) {
        var con = $(selector).children("div").eq(1);
        if (con.filter(":visible").length > 0) {
            con.slideUp("fast");
            $(selector).find(".WinIcon").css("background-position", "0 0px");
        }
        else {
            con.slideDown("fast");
            $(selector).find(".WinIcon").css("background-position", "0 19px");
        }
    }

    $(".SendMsg .WinIcon").parent("div").click(function () {
        jv.SendMsg(".SendMsg");
    });
});