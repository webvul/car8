/**
 * editor_plugin_src.js
 *
 * Copyright 2009, Moxiecode Systems AB
 * Released under LGPL License.
 *
 * License: http://tinymce.moxiecode.com/license
 * Contributing: http://tinymce.moxiecode.com/contributing
 */

(function () {
    tinymce.create('tinymce.plugins.UploadImgPlugin', {
        init: function (ed, url) {
            // Register commands
            ed.addCommand('uploadImg', function () {
                // Internal image object like a flash placeholder
                if (ed.dom.getAttrib(ed.selection.getNode(), 'class', '').indexOf('mceItem') != -1)
                    return;

                ed.windowManager.open({
                    file: url + '/image.htm',
                    width: 400,
                    height: 400,
                    inline: 1
                }, {
                    plugin_url: url
                });
            });

            // Register buttons
            ed.addButton('uploadImg', {
                title: '上传图片',
                cmd: 'uploadImg'
            });
        },

        getInfo: function () {
            return {
                longname: 'uploadImg',
                author: 'udi',
                authorurl: 'http://tinymce.moxiecode.com',
                infourl: 'http://wiki.moxiecode.com/index.php/TinyMCE:Plugins/advimage',
                version: tinymce.majorVersion + "." + tinymce.minorVersion
            };
        }
    });

    // Register plugin
    tinymce.PluginManager.add('uploadImg', tinymce.plugins.UploadImgPlugin);
})();