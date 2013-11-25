<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.NoticeInfoRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    公告详情
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/tiny_mce/jquery.tinymce.js"></script>
    <script src="~/Res/tiny_mce/Mytinymce.js"></script>
    <script type="text/javascript">
        $(function () {

            jv.SetDetail();
            $("#Edit_Descr").Mytinymce({ width: 712, height: 350 });


            $("#upFile_Logo").upFile({
                max: 1, listFile: null, onComplete: function (i, fileFullName, res) {
                    $("#MyImg_Logo").attr("src", res.url);
                    $("#Edit_Logo").val(res.id);
                    this.upFile._options._loaded = [];
                }
            });



            jv.page().Edit_OK = function (callback) {
                if (jv.chk() == false) return;

                var json = $(".MyCard").GetDivJson("Edit_");
                $.post("~/Admin/NoticeInfo/" + jv.page().action + ".aspx", json, function (res) {
                    if (res.msg) alert(res.msg);
                    else {
                        jv.Hide(function () { $(".myGrid:last").getFlexi().populate(); });
                        alert("保存成功");
                        if (jv.IsInBoxy(callback)) { }
                    }
                });
            }
        });

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="MyTool">
        <input class="submit" type="button" value="保存" onclick="jv.page().Edit_OK(event)" />
        <input class="submit" type="button" value="返回" id="retUrl" onclick="jv.back();" style="display: none;" />
    </div>
    <div class="divCard FillHeight-">
        <div class="MyCard FillHeight-">
            <div>
                <div class="MyCardTitle">
                    基本信息
                </div>
                <div class="coline"></div>
                <div class="kv">
                    <span class="key" title="">ID:</span> <span class="val">
                        <%= (Model.Id) %><%=Html.Hidden("Edit_Id", (Model.Id))%>
                </div>
                <% 
                    var pt = Model.GetNoticeType();
                    Ronse += dbr.NoticeType.PopRadior("Edit_NoticeTypeID",
                    new UIPop()
                    {
                        area = "Admin",
                        KeyTitle = "信息类别",
                        require = true,
                        Value = Model.NoticeTypeID.AsString(),
                        Display = pt != null ? pt.Name : ""
                    })
                    .GenDiv();
                %>
                <div class="kv">
                    <span class="key must" title="">信息主题:</span> <span class="val">
                        <input id="Edit_Name" value="<%=Model.Name %>" chk="function(val){if(!val.trim().length) return '必须写信息主题'} " />
                    </span>
                </div>
                <div class="kv">
                    <span class="key" title="">信息描述:</span>
                    <span class="val">
                        <%=Html.TextArea("Edit_Descr", (Model.Descr))%>
                    </span>
                </div>

                <div class="kv">
                    <span class="key" title="">排序:</span> <span class="val">
                        <%=Html.TextBox("Edit_SortID", (Model.SortID))%> </span>
                </div>

                <div class="kv" style="display: none">
                    <span class="key" title="">最近更新时间:</span> <span class="val">
                        <%= (Model.UpdateTime)%>  </span>
                </div>




            </div>
            <div class="FillHeight divSplit" style="display: none">
            </div>
            <div style="display: none">

                <div class="MyCardTitle">
                    附件信息
                </div>
                <div class="coline"></div>
                <div class="kv">
                    <span class="key" title="">
                        <input type="hidden" id="Edit_Logo" value="<%=Model.Logo %>" />
                        <div id="upFile_Logo" style="float: right;">缩略图:</div>

                    </span><span class="val">
                        <img id="MyImg_Logo" src="<%=Model.Id > 0 && Model.GetAnnex() != null ?   Model.GetAnnex().GetUrlFull() : string.Empty %>" />
                    </span>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
