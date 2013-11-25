<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<DbEnt.ProductInfoRule.Entity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    商品详情
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            if (jv.page()["action"].toLowerCase() == "add") {
                $("#retUrl").css("display", "");
            }

            jv.SetDetail();
            $("#Edit_Descr").PopTextArea();


            $("#upFile_Logo").upFile({
                max: 1, listFile: null, onComplete: function (i, fileFullName, res) {
                    $("#MyImg_Logo").attr("src", res.url);
                    $("#Edit_Logo").val(res.id);
                    this.upFile._options._loaded = [];
                }
            });

            $("#upFile").upFile({
                max: 8,
                data: jv.page().annex,
                onSubmit: function () { },
                onComplete: function (id, fileFullName, qqFile) {
                }
            });
            //            jv.TestRole();

            //$('#txtCreateDate').datepicker();
            //$("#txtStatus").TextHelper("array", "在岗,离岗");
            //$("#txtChannelManager").TextHelper("change", "~/C3_Shop/LoadStaff/?post=渠道经理", 500);

            var myGrid = $(".myGrid", jv.boxdy());

            jv.page().Edit_OK = function (callback) {
                if (jv.chk() == false) return;

                var json = $(".MyCard").GetDivJson("Edit_");
                var Images = jv.GetJsonKeys($("#upFile").data("upFile")._options._loaded).join();
                $.post("~/Admin/ProductInfo/" + jv.page().action + (Images ? ("/" + Images) : "") + ".aspx", json, function (res) {
                    //客户端返回的是Json
                    if (res.msg) alert(res.msg);
                    else {
                        if (jv.page().action.toLowerCase() == "add") {
                            jv.page().action = "Update";
                            jv.page().uid = res.data;
                            $("#Edit_Id").val(res.data);
                            myGrid.getFlexi().p.url = "~/Admin/ProductDetail/Query" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";

                            if (typeof callback == "function") callback();
                            else
                                window.history.back();
                        }
                        else {
                            jv.Hide(function () { $(".myGrid:last").getFlexi().populate(); });
                        }
                    }
                });
            }


            jv.page().flexiQuery = function (ids, grid) {
                var queryJDiv = $(".divQuery:last", jv.boxdy());

                new Boxy(queryJDiv, { modal: true, title: "查询" }).resize(400, 100);
                queryJDiv.find(".submit").one("click", function () {
                    //查询。

                    grid.doSearch(queryJDiv.GetDivJson("List_"));
                    Boxy.get(queryJDiv).hide();
                });
            };

            jv.page().flexiView = function (id) {
                Boxy.load("~/Admin/ProductDetail/Detail/" + id + ".aspx", { filter: ".Main", modal: true, title: "查看详细" }, function (bxy) {


                    $(".Main").find(".submit").one("click", function () {
                        Boxy.get(".Main").hide();
                    });
                });
            };

            jv.page().flexiAdd = function () {
                if (!jv.page().uid) {
                    jv.page().Edit_OK(function () {
                        Boxy.load("~/Admin/ProductDetail/Add/" + jv.page()["uid"] + ".aspx", { filter: ".Main", modal: true, title: "添加商品参数" });
                    });
                }
                else {
                    Boxy.load("~/Admin/ProductDetail/Add/" + jv.page()["uid"] + ".aspx", { filter: ".Main", modal: true, title: "添加商品参数" });
                }
            };

            jv.page().flexiEdit = function (id) {

                Boxy.load("~/Admin/ProductDetail/Update/" + id + ".aspx", { filter: ".Main", modal: true, title: "修改" }, function (bxy) {


                    //Edit页执行保存代码
                });
            };

            jv.page().flexiDel = function (ids, grid) {
                //删除。
                $.post("~/Admin/ProductDetail/Delete.aspx", { query: ids.join(",") }, function (res) {
                    if (!res.msg) alert("删除成功 !");
                    else { alert(res.msg); }
                });
            };

            myGrid.flexigrid({
                title: "商品参数",
                url: "",
                role: { name: "Key" },
                colModel: [
                     { display: "查 看", bind: false, name: "View", width: 30, align: "center", html: '<a style="cursor:pointer" onclick="jv.page().flexiView($(this).getFlexiRowID());">查 看</a>' },
                     { display: "键", align: "center", name: "Key" },
                     { display: "值", align: "center", name: "Value" },
                     {
                         display: "是否是标题", name: "IsCaption", align: "center", format: function (rowData, rowIndex, grid, td, event) {
                             var txt = rowData.cell[this.indexOfBind];
                             if (txt == "True") return "是"; else return "";
                         }
                     },
                     { display: "排序", name: "SortID", align: "center" },
                     { display: "修 改", bind: false, name: "Edit", width: 30, align: "center", html: '<a style="cursor:pointer" onclick="jv.page().flexiEdit($(this).getFlexiRowID());">修 改</a>' }
                ],
                dataType: "json",
                buttons: [
                     //{ separator: true },
                     //{ name: "查询", bclass: "query", onpress: jv.page().flexiQuery },
                     { separator: true },
                     { name: '添加商品参数', bclass: 'add', onpress: jv.page().flexiAdd },
                     { separator: true },
                     { name: "删除商品参数", bclass: "delete", onpress: function (ids, grid) { if (ids.length == 0) return; if (confirm("确认删除这 " + ids.length + " 项吗？") == false) return; if (jv.page().flexiDel(ids, grid) == false) return; grid.populate(); } },
                     { separator: true }
                ],
                toolBar: false,
                usepager: true,
                useRp: true,
                rp: 15,
                showTableToggleBtn: true
            });

            if (jv.page().uid) {
                myGrid.getFlexi().p.url = "~/Admin/ProductDetail/Query" + (jv.page()["uid"] ? "/" + jv.page()["uid"] : "") + ".aspx";
                myGrid.getFlexi().populate();
            }

            $("#List_Trait").TextHelper({ datatype: "json", data: { Default: "描述", Caption: "标题项", Detail: "标题值" } });
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
                        <%= (Model.Id) %> <%=Html.Hidden("Edit_Id", (Model.Id))%>
                </div>
                <% 
                    var pt = Model.GetProductType();
                    Ronse += dbr.ProductType.PopRadior("Edit_ProductTypeID",
                    new UIPop()
                    {
                        area = "Admin",
                        KeyTitle = "产品类别",
                        require = true,
                        Value = Model.ProductTypeID.AsString(),
                        Display = pt != null ? pt.Name : ""
                    })
                    .GenDiv();
                %>
                <div class="kv">
                    <span class="key must" title="">商品名称:</span> <span class="val">
                        <input id="Edit_Name" value="<%=Model.Name %>" chk="function(val){if(!val.trim().length)return  '必须写商品名称'}" />
                    </span>
                </div>
                <div class="kv">
                    <span class="key" title="">商品描述:</span>
                    <span class="val">
                        <%=Html.TextArea("Edit_Descr", (Model.Descr))%>
                    </span>
                </div>
                <div class="kv">
                    <span class="key" title="">排序:</span> <span class="val">
                        <%= Html.TextBox("Edit_SortID", Model.SortID)%></span>
                </div>
                <%--            <div class="kv">
                <span class="key" title="">点击数量:</span> <span class="val">&nbsp;</span>
            </div>--%>
                <div class="kv">
                    <span class="key" title="">最近更新时间:</span> <span class="val">
                        <%= (Model.UpdateTime)%></span>
                </div>

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
                <div class="kv">
                    <span class="key">产品图片:</span>
                    <span class="val">
                        <div id="upFile"></div>
                    </span>
                </div>
            </div>
            <div class="FillHeight divSplit">
            </div>
            <div class="FillHeight">
                <table class="myGrid"></table>
            </div>
        </div>
    </div>
</asp:Content>
