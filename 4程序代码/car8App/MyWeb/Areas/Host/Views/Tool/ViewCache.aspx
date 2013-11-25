<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Host/Views/Shared/Simple.Master"
    Theme="Admin" Inherits="MyCon.MyMvcPage<IEnumerable<MyWeb.Areas.Host.Models.MyOqlCacheModel>>" %>

<asp:Content ID="dd" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/Canvas/excanvas.js" type="text/javascript"></script>
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <style type="text/css">
        body {
            background-color: #f0f0f0;
            height: 100%;
        }

        .Center {
            width: 450px;
            height: 300px;
            position: absolute;
            left: 50%;
            top: 50%;
            margin-left: -225px;
            margin-top: -160px;
        }

        table {
            width: 100%;
            border-collapse: collapse;
        }

            table tr td, table tr th {
                padding: 5px;
                border: 1px solid rgb(222, 231, 215);
            }

            table tr th {
                background-color: rgb(238, 227, 205);
                font-size: 14px;
            }

        tr.sql {
            color: blue;
        }

        tr.oqlid {
            color: red;
        }

        button {
            margin: 0 10px;
            background-color: #8f0000;
            color: White;
            padding: 5px 15px;
            font-weight: bold;
        }
    </style>
    <script type="text/javascript">
        function delCache() {
            var doer = jv.getDoer();
            var key = $(doer).closest("td").find(".key").html();
            $.post("~/Host/Tool/DeleteCache.aspx", { key: key }, function (res) {
                jv.goto(window.location);
            });
        }
        function viewCache() {
            var doer = jv.getDoer();
            var key = $(doer).closest("td").find(".key").html();
            var val = $(doer).closest("td").find(".val").html();

            var html = "<pre>{0}</pre><br /><pre>{1}</pre>".format(key, val);
            Boxy.alert(html).resize(600).center();
        }

        function delCacheObj() {
            var doer = jv.getDoer();
            $.post("~/Host/Tool/DeleteCacheObj.aspx?obj=" + $("#obj").val(), { key: $(doer).closest("td").next().attr("title") }, function (res) {
                jv.goto("~/Host/Tool/ViewCache.aspx");
            });
        }

        function delAll() {
            $.post("~/Host/Tool/ClearCache.aspx", function (res) {
                jv.goto("~/Host/Tool/ViewCache.aspx");
            });
        }

        $(function () {
            $("#btnOK").click(function () {
                var url = jv.url();
                url.attr("obj", $("#obj").val());

                jv.goto(url.toString());
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table>
        <tr>
            <td colspan="2">
                <button id="btnDelObj" onclick="delCacheObj()">
                    删除查询结果</button>
            </td>
            <td class="key"></td>
            <td class="val"></td>
            <td class="key">关键对象：
            </td>
            <td class="val">
                <%=Html.TextBox("obj", Request.QueryString["obj"])  %>
            </td>
            <td colspan="2">
                <button id="btnOK">
                    查询</button>
            </td>
        </tr>
    </table>
    <table>
        <thead>
            <tr>
                <th></th>
                <th>缓存键(共<%=Model.Count() %>个)
                </th>
                <th>关键对象
                </th>
                <th>缓存值
                </th>
            </tr>
        </thead>
        <tbody>
            <% 
                var obj = Request.QueryString["obj"];
                foreach (var kv in Model)
                {
                    Ronse += string.Format(@"<tr class='{0}'>
<td>
<input type='button' value='删除' onclick=""delCache(event);"" />
<input type='button' value='查看' onclick='viewCache(event);' />
<textarea class='key' style='display:none'>{5}</textarea>
<textarea class='val' style='display:none'>{6}</textarea>
</td>
<td title=""{1}""  class='Wrap'>{2}</td><td>{3}</td><td class='Wrap' title='{7}'>{4}</td></tr>",
                        kv.Type,
                        kv.Key.GetSafeValue(),
                        kv.Key.Slice(0, 80).AsString(),
                        kv.Object,
                        kv.Value.Slice(0, 80).AsString(),
                        kv.Key,
                        kv.Value,
                        kv.Value.GetSafeValue()
                        );
                }

            %>
        </tbody>
    </table>
</asp:Content>
