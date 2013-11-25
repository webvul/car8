<%@ Page Title="" Theme="Default" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    图片列表
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyPager/MyPager.js"></script>
    <script type="text/javascript">
<% 
        var total = dbr.Annex.Select(o => o.Count()).Where(o => o.UserID == MySessionKey.UserID.Get() & o.Ext.In(".jpg", ".bmp", ".png", ".gif", ".jpeg")).ToEntity(0);
%>

        $(function () {
            jv.page().Query = function () {

            };
            jv.page().load = function (pageIndex) {
                var json = $(".divQuery").getDivJson("List_");
                json.page = pageIndex;
                $.post("~/Admin/Annex/ImgQuery.aspx", json, function (res) {
                    var $con = $(".con");
                    $con.empty();
                    for (var i = 0, len = res.Rows.length ; i < len ; i++) {
                        var item = res.Rows[i];
                        var template = "<div class='Item Inline' dbid='{2}'><img src='{0}' /><span class='Wrap'>{1}</span></div>";
                        $con.append(template.format(item[2], item[1], item[0]));
                    }

                    jv.page().bind();
                    $('.WebPager').MyPager({ total: Math.ceil(res.Count / 20.0), page: pageIndex, onclick: jv.page().load });
                });
            }

            jv.page().bind = function () {
                $(".con .Item")
                    .hover(function () {
                        $(this).addClass("hover");
                    }, function () {
                        $(this).removeClass("hover");
                    });


                //目前只支持单选
                if (jv.page()._Ref_Type_ == "radio") {
                    $(".con .Item").dblclick(function () {
                        var $doer = $(this);
                        if ($doer.hasClass("Item")) {
                            if (window.opener._Ref_Callback_) {
                                //IE下不能跨页面传递事件源，所以显式指定。
                                var dbid = $doer.attr("dbid");
                                var name = $doer.find("span").text();
                                var fullUrl = $doer.find("img").attr("src");
                                jv.Hide(function () { window.opener._Ref_Callback_({ id: "id", name: "name" }, [{ id: dbid, name: name, FullUrl: fullUrl }], null, { originalEvent: true, target: $doer[0] }); });
                            }

                        }
                    });
                }
            };

            jv.page().bind();




            $('.WebPager').MyPager({ total: "<%=Math.Ceiling( total/20.0) %>", page: "<%=Mvc.Model["uid"].AsInt()+1 %>", onclick: jv.page().load });
        })
    </script>
    <style type="text/css">
        body {
            background-image: none;
        }

        .Item {
            vertical-align: top;
            margin: 5px;
            border: solid 1px green;
            width: 180px;
        }

            .Item.hover {
                outline: solid 4px #ffd800;
            }

            .Item img {
                width: 100%;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table class="divQuery" style="width: 100%">
        <tr>
            <td class="key">图片名称:
            </td>
            <td class="val">
                <input id="List_Name" />
            </td>
            <td class="key">上传开始时间:
            </td>
            <td class="val">
                <input id="List_StartTime" class="MyDate" />
            </td>
            <td class="key">上传结束时间:
            </td>
            <td class="val">
                <input id="List_EndTime" class="MyDate" />
            </td>
            <td class="key"></td>
            <td class="val query_btn">
                <input class="submit" type="button" value="查询" onclick="jv.page(event).Query()" />
            </td>
        </tr>
    </table>
    <div class="divQuery" style="display: none">
        <br />
        <input class="submit" type="button" value="查询" />
    </div>
    <div class="PagerDiv">
        <div class="PagerInnerDiv Pin">
            <span class="WebPager"></span>
        </div>
    </div>
    <div class="FillHeight con">
        <%        
            var list = dbr.Annex.SelectWhere(o => o.UserID == MySessionKey.UserID.Get() & o.Ext.In(".jpg", ".bmp", ".png", ".gif", ".jpeg")).Take(20).OrderBy(o => o.AddTime.Desc).ToEntityList(o => o._);

            foreach (var file in list)
            {
                Ronse += string.Format("<div class='Item Inline' dbid='{2}'><img src='{0}' /><span class='Wrap'>{1}</span></div>", file.GetUrlFull(), file.Name, file.Id);
            }
        
        %>
    </div>
</asp:Content>
