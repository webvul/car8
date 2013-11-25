<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<MyOql.MenuRule.Entity>>" %>
<script src="~/Res/fg-menu/fg.menu.js" type="text/javascript"></script>
<link href="~/Res/fg-menu/fg.menu.css" rel="stylesheet" type="text/css" />
<style type="text/css">
    .hidden
    {
        position: absolute;
        top: 0;
        left: -9999px;
        width: 1px;
        height: 1px;
        overflow: hidden;
    }
    .fg-button
    {
        padding: .4em 1em;
        text-decoration: none !important;
        cursor: pointer;
        position: relative;
        text-align: center;
        zoom: 1;
    }
    .fg-button .ui-icon
    {
        position: absolute;
        top: 50%;
        margin-top: -8px;
        left: 50%;
        margin-left: -8px;
    }
    a.fg-button
    {
        float: left;
        margin-left: 8px;
    }
    button.fg-button
    {
        width: auto;
        overflow: visible;
    }
    /* removes extra button width in IE */.fg-button-icon-left
    {
        padding-left: 2.1em;
    }
    .fg-button-icon-right
    {
        padding-right: 2.1em;
    }
    .fg-button-icon-left .ui-icon
    {
        right: auto;
        left: .2em;
        margin-left: 0;
    }
    .fg-button-icon-right .ui-icon
    {
        left: auto;
        right: .2em;
        margin-left: 0;
    }
    .fg-button-icon-solo
    {
        display: block;
        width: 8px;
        text-indent: -9999px;
    }
    /* solo icon buttons must have block properties for the text-indent to work */.fg-button.ui-state-loading .ui-icon
    {
        background: url(~/Res/fg-menu/spinner_bar.gif) no-repeat 0 0;
    }
    .menu-split
    {
        height: 1px;
        background-color: white;
        border-top: 1px solid wheat;
        margin: 3px 5px;
    }
</style>
<script type="text/javascript">
    $(function () {
        // BUTTONS
        $('.fg-button').hover(
    		function () { $(this).removeClass('ui-state-default').addClass('ui-state-focus'); },
    		function () { $(this).removeClass('ui-state-focus').addClass('ui-state-default'); }
    	);

        jv.page().menuClick = function (url) {
            var jwin = $("#frmMain");
            var jpc = jwin.parent();
            var newH = jpc.innerHeight() - parseInt(jpc.css("paddingTop")) - parseInt(jpc.css("paddingBottom")) - jwin.offset().top - 4;
            jwin.height(newH).attr("src", url);
            //            var win = jwin = jwin[0];
        }

        jv.page().loadMenu = function (id) {
            var jobj = $('[dbid=' + id + ']');

            if (jobj.data("loaded")) return;
            jobj.data("loaded", true);

            $.post('~/Admin/Menu/QueryMenu/' + id + '.aspx', function (data) {
                var d = jobj.menu({ content: data, flyOut: true });
                jobj.click();
            });
        };
    });
</script>
<% foreach (var item in Model)
   { %>
<a tabindex="0" class="fg-button fg-button-icon-right ui-widget ui-state-default ui-corner-all"
    dbid="<%= item.ID %>" onclick="jv.page().loadMenu($(this).attr('dbid'));"><span class="ui-icon ui-icon-triangle-1-s">
    </span>
    <%= item.Text %></a>
<%}
%>
