<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            jv.SetDetail();
            jv.page().Edit_OK = function () {

                $.post("~/Admin/Home/CreateHtml.aspx", $(".Main").GetDivJson("Edit_"), function (res) {
                    //客户端返回的是Json
                    if (res.msg) {
                        alert(res.msg);
                        return false;
                    }

                    var cou = 0;
                    $(res.data).each(function (i, d) {
                        $.get(d, null, function (ss) {
                            cou++;
                            $("#urlList").append("<li style='list-style:none'>" + d + "</li>");
                            top.document.title = "还有：" + (res.data.length - cou) + " 个没有生成。";


                            if (res.data.length == cou) {
                                alert("创建成功");
                            }
                        });
                    });
                });

            }
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divCard FillHeight">
        <div class="MyTool">
            <input class="submit" type="button" value="创建" onclick="jv.page().Edit_OK(event)" />
        </div>
        <div class="MyCard FillHeight" style="text-align: center">
            将发布此次更新的全部内容至您的商家主页上，请确定创建…
            <ul id="urlList" style="list-style: none;text-align:left"></ul>

            <textarea id="Edit_Url" style="width: 80%; margin-top: 20px; margin-bottom: 20px; display: none" class="FillHeight">
~/Shop/Index/{0}.aspx?Html=True
~/Shop/Products/{0}.aspx?Html=True
~/Shop/Notice/{0}.aspx?Html=True
~/Shop/About/{0}.aspx?Html=True
~/Shop/ShowCase/{0}.aspx?Html=True
~/Shop/Index/{0}/En.aspx?Html=True
~/Shop/Products/{0}/En.aspx?Html=True
~/Shop/Notice/{0}/En.aspx?Html=True
~/Shop/About/{0}/En.aspx?Html=True
~/Shop/ShowCase/{0}/En.aspx?Html=True
</textarea>


        </div>
    </div>
</asp:Content>
