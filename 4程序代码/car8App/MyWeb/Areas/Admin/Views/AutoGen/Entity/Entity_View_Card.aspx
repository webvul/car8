<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<MyOql.MyOqlCodeGenSect.MyOqlCodeGenViewsCollection.MyOqlCodeGenViewElement>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    视图
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {


            jv.SetDetail({ detail: "EntityViewDetail" });


            jv.page().Edit_OK = function (ev) {
                
                $.post("~/Admin/AutoGen/" + jv.page()["action"] + "/" + (jv.page()["uid"] || "0") + ".aspx", $(".Main").GetDivJson("Edit_"), function (res) {
                    //客户端返回的是Json

                    if (jv.IsInBoxy(ev)) {
                        if (res.msg) alert(res.msg);
                        else {
                            jv.Hide(function () {
                                $(".flexigrid", jv.boxdy()).getFlexi().populate();
                            });
                        }
                    }
                    else {
                        if (res.msg) alert(res.msg);
                        else alert("保存成功");
                    }
                });
            };


            jv.page().EnumKey = function (ev) {
                if (!($("#Edit_Name").val() && $("#Edit_db").val() && $("#Edit_Owner").val())) {
                    alert("请配置正确的配置项");
                    return;
                }

                var GetEnmValue = function (grid) {
                    var ret = [];
                    $(grid.gDiv).find("table tbody tr.trSelected").each(function (i, d) {
                        var tr = grid.TableRowToData(d, true);
                        ret.push((tr.Column + "=" + tr.Type + ":" + tr.MapName).trimAll());
                    });
                    return ret;
                };

                jv.PopList({ entity: "AutoGen", list: "Entity_Table_EnumKeyList", detail: "Entity_Table_EnumKeyList", query: { entity: $("#Edit_Name").val(), db: $("#Edit_db").val(), owner: $("#Edit_Owner").val() }, mode: "check",
                    callback: function (role, kv, add, minus, grid) {
                        var ToFksString = function (jObj) {
                            var fs = [];
                            $(jObj).each(function (i, d) {
                                fs.push(d.col + "=" + d.type + ":" + d.mapName);
                            });
                            return fs.join(",");
                        };

                        var enmValue = GetEnmValue(grid);


                        $("#Edit_Enums").val(enmValue.join(","));
                        jv.SetDisplayValue($("#Edit_Enums"), enmValue.join(","));
                    }
                });
            };
        });


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="MyTool">
        <input class="submit" type="button" value="保存" onclick="jv.page().Edit_OK(event)" /></div>
    <div class="MyCard">
        <div>
            <div class="MyCardTitle">
                基本信息(必填)
            </div><div class="coline"> </div>
            <div class="kv">
                <span class="key" title="">名称:</span> <span class="val hard">
                    <%=Html.TextBox("Edit_Name", (Model.Name), new { @readonly = "true" })%></span>
            </div>
            <div class="kv">
                <span class="key" title="">映射名称:</span> <span class="val hard">
                    <%=Html.TextBox("Edit_MapName", (Model.MapName)  )%></span>
            </div>
            <div class="kv">
                <span class="key" title="">描述:</span> <span class="val">
                    <%=Html.TextBox("Edit_Descr", (Model.Descr))%>
                </span>
            </div>
            <div class="kv">
                <span class="key" title="">关联主表:</span> <span class="val">
                    <%=Html.TextBox("Edit_Table", (Model.Table))%>
                </span>
            </div>
        </div>
        <div class="FillHeight divSplit">
        </div>
        <div>
            <div class="MyCardTitle">
                高级信息(选填)
            </div><div class="coline"> </div>
            <div class="kv">
                <span class="key" title="" style="color: Red">数据配置:</span> <span class="val">
                    <%=Html.TextBox("Edit_db", (Model.db))%>
                </span>
            </div>
            <div class="kv">
                <span class="key" title="" style="color: Red">所有者:</span> <span class="val">
                    <%=Html.TextBox("Edit_Owner", (Model.Owner))%>
                </span>
            </div>
            <div class="kv">
                <span class="key Link" onclick="jv.page().EnumKey(event);">列格式:
                    <% 
                        //var eumValue = string.Join(",", Model.Enums
                        //     .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        //     .Select(o => o.Split('=')[0])
                        //     .Where(o => o.HasValue())
                        //     .ToArray()
                        //     );
                        Ronse += Html.Hidden("Edit_Enums", Model.Enums).ToHtmlString(); 
                    %>
                </span><span class="val"><span>
                    <%=Model.Enums%></span></span>
            </div>
        </div>
    </div>
</asp:Content>
