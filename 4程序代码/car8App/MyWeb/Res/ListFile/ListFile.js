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