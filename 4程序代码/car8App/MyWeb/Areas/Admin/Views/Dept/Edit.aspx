<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.DeptRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    商户
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/tiny_mce/jquery.tinymce.js"></script>
    <script src="~/Res/tiny_mce/Mytinymce.js"></script>
    <script type="text/javascript">
        $(function () {
            jv.SetDetail();
            //$("textarea").PopTextArea();

            if (jv.page().action.toLowerCase() == "add") $("#retUrl").show();
            if (jv.page().action.toLowerCase() == "update") {
                $("#Edit_Name").attr("disabled", true);
                $("#Edit_CommID").parent().next().find("input:first").attr("disabled", true);
            }


            if (jv.page()["CommNames"])
                $("#Edit_CommID").parent().next().find("input").val(jv.page()["CommNames"]);
            if (jv.page()["CommIDs"])
                $("#Edit_CommID").val(jv.page()["CommIDs"]);


            //事件
            //            $("#Edit_MySkin").TextHelper({ post: "click", url: "~/Admin/Dept/GetSkins.aspx", datatype: "array" });
            $(":hidden[name=Edit_MySkin]").val("GreenMenu");//添加默认站点皮肤
            jv.page().Edit_OK = function (ev) {
                if (jv.chk() == false) return;

                var model = $(".divCard").GetDivJson("Edit_");


                $.post("~/Admin/Dept/" + jv.page()["action"] + "/" + model.CommID + ".aspx", model, function (res) {
                    //客户端返回的是Json

                    if (jv.IsInBoxy(ev)) {
                        if (res.msg) alert(res.msg);
                        else {
                            if (res.data) {
                                Boxy.load("~/Admin/Person/Update/" + model.WebName + ".aspx", { title: "编辑用户", width: 701 })
                                return;
                            }

                            jv.Hide(function () {
                                $(".flexigrid", jv.boxdy()).getFlexi().populate();
                            });
                        }
                    }
                    else {
                        if (res.msg) alert(res.msg);
                        else {
                            alert("保存成功");
                            if (jv.page()["action"] == "Add")
                                window.history.back();
                        }
                    }
                });
            };

            $("#upFile_Logo").upFile({
                css: { width: "100px" },
                max: 1, listFile: null, onComplete: function (i, fileFullName, res) {
                    $("#MyImg_Logo").attr("src", res.url);
                    $("#Edit_Logo").val(res.id);
                    this.upFile._options._loaded = [];
                }
            });
            $("#upFile_Title").upFile({
                css: { width: "130px" },
                max: 1, listFile: null, onComplete: function (i, fileFullName, res) {
                    $("#MyImg_Title").attr("src", res.url);
                    $("#Edit_Title").val(res.id);
                    this.upFile._options._loaded = [];
                }
            });

            $("#upFile_TitleExtend").upFile({
                css: { width: "150px" },
                max: 1, listFile: null, onComplete: function (i, fileFullName, res) {
                    $("#MyImg_TitleExtend").attr("src", res.url);
                    $("#Edit_TitleExtend").val(res.id);
                    this.upFile._options._loaded = [];
                }
            });

            jv.page().MyLogo_Click = function (ev) {

                if (Confirm("将上传新的图片替换掉目前的图片，确定吗？") == false) return false;

                $("#upFile").upFile({
                    max: 1, listFile: null, onComplete: function (i, fileFullName, res) {
                        $("#MyImg").attr("src", res.url);
                        $("#Edit_Icon").val(res.id);
                        this.upFile._options._loaded = [];
                    }
                });


                $("#Edit_Name").TextHelper({ post: "change", url: "~/Admin/Dept/R.aspx", change: function (val) { } });

                //jv.Upload(
                //{
                //    callback: function (fileRes) {

                //        var url = "~/Admin/Dept/Upload/MyLogo.aspx";
                //        var ids = [];
                //        $(fileRes).each(function (i, d) {
                //            ids.push(d.id);
                //        });

                //        $.post(url, { DeptID: jv.page().uid, files: ids.join(",") }, function (res) {

                //            if (!res.msg) {
                //                $("#MyLogo").attr("src", res.id);
                //                Boxy.getOne().hide();
                //            }
                //            else alert(res.msg);
                //        });
                //    },
                //    title: "上传Logo照片",
                //    max: 1,
                //    maxMsg: "最多上传1张Logo照片"
                //});

                return;
            }

            jv.page().MyTitle_Click = function (ev) {
                if (Confirm("将上传新的图片替换掉目前的图片，确定吗？") == false) return false;

                jv.Upload(
                {

                    callback: function (fileRes) {

                        var url = "~/Admin/Dept/Uploaded/MyTitle.aspx";
                        var ids = [];
                        $(fileRes).each(function (i, d) {
                            ids.push(d.id);
                        });

                        $.post(url, { DeptID: jv.page().uid, files: ids.join(",") }, function (res) {

                            if (!res.msg) {
                                $("#MyTitle").attr("src", res.data);
                                Boxy.getOne().hide();
                            }
                            else alert(res.msg);
                        });

                    },
                    title: "上传标题照片",
                    max: 1,
                    maxMsg: "最多上传1张标题照片"
                });

                return;
            }

            $("#Edit_Detail").Mytinymce({ width: 712, height: 350 });
            $("#Edit_About").Mytinymce({ width: 712, height: 350 });

            $.timer(200, function (timer) { timer.stop(); if (jv.IsInBoxy()) { Boxy.getOne().center(); } });
        });

    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="MyTool">
        <input class="submit" type="button" value="保存" onclick="jv.page().Edit_OK(event);" />
        <input class="submit" type="button" value="返回" id="retUrl" onclick="jv.back();" style="display: none;" />


    </div>
    <div class="divCard">
        <div class="MyCard">
            <div>
                <div class="MyCardTitle">
                    基本信息(必填)
                </div>
                <div class="coline"></div>

                <div class="kv">
                    <span class="key" title="">ID:</span> <span class="val hard">
                        <%= (Model.Id)%>
                        <%=Html.Hidden("Edit_Id", (Model.Id))%></span>
                </div>
                <div class="kv">
                    <span class="key must">商户名称:</span> <span class="val">
                        <input id="Edit_Name" value="<%=Model.Name %>" chk="{2}" chkmsg="必填" />
                    </span>
                </div>
                <div class="kv">
                    <span class="key must" title="">站点简称:</span> <span class="val">
                        <%= ActionIsAdd ? Html.TextBox("Edit_WebName", Model.WebName, new { chk="{3}", chkmsg="长度必须大于3位" }).ToHtmlString() : Model.WebName%>
                    </span>
                </div>

                <div class="kv">
                    <span class="key must">关联小区：<input id="Edit_CommID" type="hidden" value="0" /></span>
                    <span class="val">
                        <input readonly="readonly" class="ref" onclick="jv.PopList({ mode: 'check', area: 'Admin', entity: 'Dept', list: 'CommList' }, event);" chk="function(val){ if ( val == '0' || val.length == 0 ) return '必须选择一个值!'; }" chkval="#Edit_CommID" value="" />
                    </span>
                </div>

                <div class="kv">
                    <span class="key" title="">商户电话:</span> <span class="val">
                        <%=Html.TextBox("Edit_Phone", (Model.Phone))%>
                    </span>
                </div>
                <div class="kv">
                    <span class="key" title="">商户类型:</span> <span class="val">
                        <%  Html.RegisteEnum("Edit_BizType", Model.BizType)
                            .Radior(new UITextHelper { require = true })
                            .Input();%>
                    </span>
                </div>
                <div class="kv">
                    <span class="key" title="">关键字:</span> <span class="val">
                        <%=Html.TextArea("Edit_KeyWords", (Model.KeyWords))%></span>
                </div>
                <%--            <div class="kv">
                <span class="key Link" onclick="Boxy.alert($(this).find('div').html()).resize(400);"
                    title="">Google地图:</span>
                <div style="display: none">
                    坐标(格式：（1,2）),打开:http://ditu.google.cn/ ,在地图中心的找出商户位置,在地址栏打入:<br />
                    javascript:void(prompt('',gApplication.getMap().getCenter()));
                    <br />
                    把显示值填入.
                </div>
                <span class="val">
                    <%=Html.TextArea("Edit_GisPos", (Model.GisPos))%>
                </span>
            </div>--%>
                <div class="kv" style="display: none">
                    <span class="key">站点皮肤:</span> <span class="val">
                        <%
                            Html.Registe("Edit_MySkin")
                                .Radior(new UITextHelper { url = "~/Admin/Dept/GetSkins.aspx", require = true, selectValue = Model.MySkin, post = TextHelperPostEnum.Click })
                                .Input(); %>
                    </span>

                </div>

                <div class="kv">
                    <span class="key">地址:</span> <span class="val">
                        <%=Html.TextArea("Edit_Address", (Model.Address))%>
                    </span>
                </div>

            </div>
            <div class="FillHeight divSplit">
            </div>
            <div>
                <div class="MyCardTitle">
                    附件信息
                </div>
                <div class="coline"></div>
                <div class="kv">
                    <span class="key" title="">
                        <input type="hidden" id="Edit_Logo" value="<%=Model.Logo %>" />
                        <div id="upFile_Logo" style="float: right;">Logo(50 × 50):</div>
                    </span>
                    <span class="val">
                        <img id="MyImg_Logo" style="width: 50px;" src="<%=Model.Id > 0 && Model.GetAnnexByLogo() != null ?   Model.GetAnnexByLogo().GetUrlFull() : string.Empty %>" />
                    </span>
                </div>
                <div class="kv">
                    <span class="key">
                        <input type="hidden" id="Edit_Title" value="<%=Model.Title %>" />
                        <div id="upFile_Title" style="float: right;">商户标题(450 × 60):</div>
                    </span>
                    <span class="val">
                        <img id="MyImg_Title" style="height: 100px" src="<%=Model.Id > 0 && Model.GetAnnexByTitle() != null ?   Model.GetAnnexByTitle().GetUrlFull() : string.Empty %>" />
                    </span>
                </div>

                <div class="kv">
                    <span class="key">
                        <input type="hidden" id="Edit_TitleExtend" value="<%=Model.TitleExtend %>" />
                        <div id="upFile_TitleExtend" style="float: right;">商铺推广图片(300 × 300):</div>
                    </span>
                    <span class="val">
                        <img id="MyImg_TitleExtend" style="height: 100px" src="<%=Model.Id > 0 && Model.GetAnnexByTitleExtend() != null ?   Model.GetAnnexByTitleExtend().GetUrlFull() : string.Empty %>" />
                    </span>
                </div>


                <%--<div class="MyCardTitle">
                附加信息
            </div>
            <div class="coline"></div>
            <div class="kv">
                <span class="key">添加时间:</span> <span class="val"><span>
                    <%= Model.AddTime%></span> </span>
            </div>
            <div class="kv">
                <span class="key">结束时间:</span> <span class="val">
                    <%
                        var name = "Edit_EndTime";
                        Ronse += Html.TextBox(name, (Model.EndTime), new { @class = "MyDateTime" });
                    %>
                </span>
            </div>
            <div class="kv">
                <span class="key">排序:</span> <span class="val">
                    <%=Html.TextBox("Edit_SortID", (Model.SortID))%>
                </span>
            </div>--%>
            </div>

        </div>
        <div style="margin: 10px; text-align: center;">
            <span class="key" style="display: block; text-align: left;">公司信息:</span>
            <span class="val">
                <%=Html.TextArea("Edit_Detail", (Model.Detail))%>
            </span>
        </div>
        <div style="margin: 10px; text-align: center;">
            <span class="key" style="display: block; text-align: left;">关于我们:</span> <span class="val">
                <%=Html.TextArea("Edit_About", (Model.About))%>
            </span>
        </div>
    </div>


</asp:Content>
