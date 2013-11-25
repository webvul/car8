function TableFile(listFile, p) {
    var thead = document.createElement("thead");
    listFile.append(thead);

    listFile.append(document.createElement("tbody"));

    listFile.addClass("ListFile");

    var tr = document.createElement("tr");
    thead.appendChild(tr);

    {
        var th = document.createElement("th");
        tr.appendChild(th);

        var hid = document.createElement("input");
        hid["type"] = "hidden";
        hid["name"] = p.hidden;
        th.appendChild(hid);


        th.appendChild(hid);
        th.style.display = "none";
    }
    {
        var th = document.createElement("th");
        th.innerHTML = "文件说明";
        th.className = "fileName";
        tr.appendChild(th);
    }

    {
        var th = document.createElement("th");
        th.innerHTML = "大小（KB）";
        tr.appendChild(th);
    }

    {
        var th = document.createElement("th");
        th.innerHTML = "[下载]";
        tr.appendChild(th);
    }

    if (p.edit) {
        var th = document.createElement("th");
        th.innerHTML = "[删除]";
        tr.appendChild(th);
    }

    if (p.data) {
        for (var i = 0, length = p.data.length; i < length; i++) {
            var item = p.data[i];
            TableFile.AppendItem(listFile, p, item);
        }
    }
};

TableFile.AppendItem = function (table, p, res) {

    var tr = document.createElement("tr");
    tr.id = "row" + res.id;
    tr.setAttribute("qqFileId", res.qqFileId);

    $(table).find("tbody").append(tr);

    {
        var th = document.createElement("td");
        th.style.display = "none";
        th.innerHTML = res.id;
        tr.appendChild(th);
    }

    {
        var th = document.createElement("td");
        th.innerHTML = res.name;
        tr.appendChild(th);
    }

    {
        var th = document.createElement("td");
        th.innerHTML = parseInt(parseInt(res.size) / 1024);
        tr.appendChild(th);
    }

    {
        var th = document.createElement("td");
        th.innerHTML = '<a href="' + res.url + '" target="_blank"><div class="Inline DownloadIcon Icon16"></div></a>';
        tr.appendChild(th);
    }

    if (p.edit) {
        var th = document.createElement("td");
        th.innerHTML = '<div class="Inline DeleteIcon Icon16" onclick="javascript:return TableFile.DeleteFile(\'' + p.hidden + '\',event);" ></div>';
        tr.appendChild(th);
    }

    var hid = $("input[name=" + p.hidden + "]", table);
    var v = hid.val().mySplit(",", true);
    v.push(res.id);
    hid.val(v.join(","));
};

TableFile.DeleteFile = function (hidName, ev) {
    if (!confirm("确认删除该文件吗？")) return false;
    var jtr = $(jv.GetDoer()).closest("tr");
    var jtable = jtr.closest("table");
    var hid = jtable.find("thead tr th input[name=" + hidName + "]");
    var v = hid.val().mySplit(",", true);
    v.remove(jtr.attr("id").slice(3));
    hid.val(v.join(","));
    //这可能有问题，未测试，By Udi，at： 2013年4月18日
    this.upFile._remove(jtr.attr("qqFileId"));
    jtr.remove();
};