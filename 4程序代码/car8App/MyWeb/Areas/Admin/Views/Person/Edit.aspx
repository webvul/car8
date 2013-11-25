<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.PersonRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Person Edit
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            jv.SetDetail();

            //$('#txtCreateDate').datepicker();
            //$("#txtStatus").TextHelper("array", "在岗,离岗");
            //$("#txtChannelManager").TextHelper("change", "~/C3_Shop/LoadStaff/?post=渠道经理", 500);
            $("textarea").PopTextArea();


            jv.page().Edit_OK = function (ev) {
                if (jv.chk() == false) return false;
                $.post("~/Admin/Person/" + jv.page()["action"] + ".aspx", $(".divCard").GetDivJson("Edit_"), function (res) {
                    //客户端返回的是Json
                    if (res.msg) return alert(res.msg);
                    if (jv.Hide(function () {
                        $(".flexigrid", jv.boxdy()).getFlexi().populate();
                    }) == false) alert("保存成功");
                });
            }

            $("#upFile").upFile({
                max: 1, listFile: null, onComplete: function (i, fileFullName, res) {
                    $("#MyImg").attr("src", res.url);
                    $("#Edit_Logo").val(res.id);
                    this.upFile._options._loaded = [];
                }
            });


            //jv.page().MyImgClick = function (ev) {
            //    jv.upload(
            //    {
            //        url: "~/Admin/Person/Uploaded/MyImg.aspx",
            //        paras: function (files) { return { files: files } },
            //        callback: function (res) {
            //            if (!res.msg) {
            //                $("#MyImg").attr("src", res.data);
            //                Boxy.getOne().hide();
            //            }
            //            else alert(res.msg);
            //        },
            //        title: "上传照片",
            //        max: 8,
            //        maxMsg: "每个仪器最多上传8张照片"
            //    });
            //}

        });
    </script>
    <style type="text/css">
        #imgMy {
            width: 120px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divCard">
        <div class="MyTool">
            <input class="submit" type="button" value="保存" onclick="jv.page().Edit_OK(event)" />
        </div>
        <div class="MyCard">
            <div>
                <div class="MyCardTitle">
                    基本信息
                    <%= Html.Hidden("Edit_Password",Model.Password) %>
                </div>
                <div class="coline"></div>
                <div class="kv">
                    <span class="key" title="">用户ID:</span> <span class="val hard">
                        <%
                            if (MySession.IsSysAdmin())
                            {
                                new MyTag(HtmlTextWriterTag.Input, new { id = "Edit_UserID", value = Model.UserID, chk = "{3}", chkmsg = "必填,长度要求大于3" });
                            }
                            else
                            {
                                new MyTag(HtmlTextWriterTag.Input, new { id = "Edit_UserID",Readonly="readonly", value = Model.UserID, chk = "{3}", chkmsg = "必填,长度要求大于3" });
                            }
                        %>
                        
                    </span>
                </div>
                <div class="kv">
                    <span class="key must" title="">名字:</span> <span class="val ">
                        <input id="Edit_Name" value="<%=Model.Name %>" chk="{3}" chkmsg="必填，长度要求大于3" />
                    </span>
                </div>

                <div class="kv">
                    <span class="key" title="">商户:</span> <span class="val ">                       
                        <%= Model.GetDept().Name %>
                        <input type="hidden" id="Edit_DeptID" value="<%= Model.DeptID %>" />
                    </span>
                </div>

              <!--  <%
                    
                    dbr.Dept.PopRadior("Edit_DeptID", new UIPop()
                    {
                        require = true,
                        area = "Admin",
                        KeyTitle = "商户",
                        Value = Model.DeptID.ToString(),
                        Display = Model.GetDept().Name
                    })
                     .GenDiv();
                            //{
                            //    var deptName = string.Empty;

                            //    if (Model.GetDepts() > 0)
                            //    {
                            //        var dept = dbr.Dept.FindById(Model.DeptID);
                            //        deptName = dept.Name;
                            //    }
                            //    Ronse += dbr.Dept.PopForCheck("Edit_DeptID",
                            //   "所属单位",
                            //   "Admin",
                            //   Model.DeptID.AsString(),
                            //   deptName);
                            //}
                %>
                -->
                <%--                <%
                    {
                        var roles = Model.GetRoles().Select(o => o.Role).Join(",");

                        Ronse += dbr.Role.PopChecker("Edit_Roles", new UIPop()
                        {
                            KeyTitle = "角色",
                            area = "Admin",
                            Value = Model.GetRoles().Select(o => o.Id).Join(","),
                            Display = roles
                        })
                            .GenDiv();
                    }
                %>--%>
                <div class="kv">
                    <span class="key" title="">性别:</span> <span class="val ">
                        <% Html.RegisteEnum("Edit_Sex", Model.Sex)
                               .Radior()
                               .Input();
                        %></span>
                </div>

            </div>
            <div class="FillHeight divSplit">
            </div>
            <div>
                <div class="MyCardTitle">
                    附加信息
                </div>
                <div class="coline"></div>


                <div class="kv">
                    <span class="key" title="">
                        <input type="hidden" id="Edit_Logo" value="<%=Model.Logo %>" />
                        <div id="upFile" style="float: right;">头像：</div>

                    </span>
                    <span class="val">
                        <img style="width: 150px;" alt="<%=Model.Name %>" id="MyImg" src="<%=Model.GetAnnex() == null ? "": Model.GetAnnex().GetUrlFull() %>" />

                    </span>
                </div>

                <%--  <div class="kv">
                    <span class="key" title="">MSN:</span> <span class="val ">
                        <%=Html.TextBox("Edit_MSN", (Model.Msn))%></span>
                </div>--%>
                <div class="kv">
                    <span class="key" title="">QQ:</span> <span class="val ">
                        <%=Html.TextBox("Edit_QQ", (Model.Qq))%></span>
                </div>
                <div class="kv">
                    <span class="key" title="">手机:</span> <span class="val ">
                        <%=Html.TextBox("Edit_Mobile", (Model.Mobile))%></span>
                </div>
                <div class="kv">
                    <span class="key" title="">邮件:</span> <span class="val ">
                        <%=Html.TextBox("Edit_Email", (Model.Email))%></span>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
