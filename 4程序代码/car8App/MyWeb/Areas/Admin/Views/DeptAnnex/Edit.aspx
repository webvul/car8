<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.DeptAnnexRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    DeptAnnex Edit
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            jv.SetDetail();

            //$('#txtCreateDate').datepicker();
            $("#Edit_Key").TextHelper({ datatype: "json", data: { Home: "首页", Profile: "企业证书", AboutUs: "关于我们"} });

        
         jv.page().Edit_OK = function(ev) {
            
            $.post("~/Admin/DeptAnnex/" + jv.page()["action"] + "/" + (jv.page()["uid"] || "0") + ".aspx", $(".MyCard").GetDivJson("Edit_"), function (res) {
                //客户端返回的是Json
                if (res.msg) alert(res.msg);
                jv.Hide(function () { $(".flexigrid", jv.boxdy()).getFlexi().populate(); });
            });
        }
        });
        function AddImg() {
            if (jv.page()["action"] != "Add") return false;
            
            Boxy.load("~/Admin/Home/File.aspx?type=*.jpg;*.png;*.gif;*.jpeg&note=图片(jpg;png;gif;jpeg)&max=5&path=~/Upload/$WebName$&callback=window.parent.uploadedPost", { iframe: true, filter: "#divFile", modal: true, title: "照片上传", beforHide: function () {
                $("#up").uploadifyClearQueue();
            }
            }, function (bxy) { bxy.resize(400, 200); });
        }
        function uploadedPost(files) {
            $("<div>" + files + "</div>").insertAfter($("#Edit_AnnexID").val(files.join(",")));
            alert("上传照片成功,请保存.");
            Boxy.getOne().hide();
            //            $.post("~/Admin/DeptAnnex/Uploaded.aspx", { files: files }, function (res) {
            //                //客户端返回的是Json
            //                if ( !res.msg ) {
            //                    $(".flexigrid", jv.boxdy()).getFlexi().populate();
            //                    Boxy.getOne().hide();
            //                }
            //                else alert(res.msg);
            //            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divCard">
        <div class="MyTool">
            <input class="submit" type="button" value="保存" onclick="jv.page().Edit_OK(event)" />
        </div>
        <div class="MyCard">
            <div>
                <div class="MyCardTitle">
                    基本信息(必填)
                </div><div class="coline"> </div>
                <div class="kv">
                    <span class="key">ID:</span> <span class="val">
                        <%= Model.Id%>
                        <%=Html.Hidden("Edit_Id", (Model.Id))%></span>
                </div>
                <div class="kv">
                    <span class="key">照片类型:</span> <span class="val">
                        <%=Html.TextBox("Edit_Key", (Model.Key))%></span>
                </div>
                <div class="kv">
                    <span class="key">照片:</span> <span class="val">
                        <%=Html.BeginTag(HtmlTextWriterTag.Img,new {@class="myImg",width="200px",src= Model.AnnexID > 0 ? Model.GetAnnex().FullName : ""}) %>
                        <%=Html.Hidden("Edit_AnnexID", (Model.AnnexID))%></span>
                </div>
            </div>
            <div class="FillHeight divSplit">
            </div>
            <div>
                <div class="MyCardTitle">
                    其它
                </div><div class="coline"> </div>
                <div class="kv">
                    <span class="key">商户:</span> <span class="val">
                        <%
                            Ronse += MySession.Get(MySessionKey.DeptName);

                            Ronse += Html.Hidden("Edit_DeptID", MySession.Get(MySessionKey.DeptID));  %></span>
                </div>
                <div class="kv">
                    <span class="key">排序号:</span> <span class="val">
                        <%=Html.TextBox("Edit_SortID", (Model.SortID))%></span>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
