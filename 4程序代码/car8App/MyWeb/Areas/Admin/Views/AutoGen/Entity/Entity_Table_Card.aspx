<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<MyOql.MyOqlCodeGenSect.MyOqlCodeGenTablesCollection.MyOqlCodeGenTableElement>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Dept Edit
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {



            $("#Edit_PKs").add($("#Edit_ComputeKeys"))
            .TextHelper({ post: "click", quote: "check", url: function () {
                return "~/Admin/AutoGen/GetColumns.aspx?Entity=" + $("#Edit_Name").val() +
                "&DbConfig=" + ($("#Edit_db").val() || jv.page().DbConfig) +
                "&Owner=" + ($("#Edit_Owner").val() || jv.page().Owner);
            }
            });

            $("#Edit_AutoIncreKey").add($("#Edit_UniqueKey"))
            .TextHelper({ post: "click", url: function () {
                return "~/Admin/AutoGen/GetColumns.aspx?Entity=" + $("#Edit_Name").val() +
                "&DbConfig=" + ($("#Edit_db").val() || jv.page().DbConfig) +
                "&Owner=" + ($("#Edit_Owner").val() || jv.page().Owner);
            }
            });


            jv.SetDetail({ detail: "EntityTableDetail" });



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

            jv.page().ForignKey = function (ev) {
                if (!($("#Edit_Name").val() && $("#Edit_db").val() && $("#Edit_Owner").val())) {
                    alert("请配置正确的配置项");
                    return;
                }

                var GetEnmValue = function (grid) {
                    var ret = [];
                    $(grid.gDiv).find("table tbody tr.trSelected").each(function (i, d) {
                        var tr = grid.TableRowToData(d, true);
                        ret.push((tr.Column + "=" + tr.RefTable + ":" + tr.RefColumn).trimAll());
                    });
                    return ret;
                };

                jv.PopList({ entity: "AutoGen", list: "Entity_Table_ForeignKeyList", detail: "Entity_Table_ForeignKeyList", query: { entity: $("#Edit_Name").val(), db: $("#Edit_db").val(), owner: $("#Edit_Owner").val() }, mode: "check",
                    callback: function (role, kv, add, minus, grid) {
                        var ToFksString = function (jObj) {
                            var fs = [];
                            $(jObj).each(function (i, d) {
                                fs.push(d.col + "=" + d.type + ":" + d.mapName);
                            });
                            return fs.join(",");
                        };

                        var enmValue = GetEnmValue(grid);


                        $("#Edit_FKs").val(enmValue.join(","));
                        jv.SetDisplayValue($("#Edit_FKs"), enmValue.join(","));
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
                <span class="key" title="">主键:</span> <span class="val">
                    <%=Html.TextBox("Edit_PKs", (Model.PKs))%>
                </span>
            </div>
            <div class="kv">
                <span class="key" title="">自增键:</span> <span class="val">
                    <%=Html.TextBox("Edit_AutoIncreKey", (Model.AutoIncreKey))%></span>
            </div>
            <div class="kv">
                <span class="key">唯一键:</span> <span class="val">
                    <%=Html.TextBox("Edit_UniqueKey", Model.UniqueKey)%>
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
                <div>
                    <span class="key Link" title="" onclick="jv.page().ForignKey(event);">外键:
                        <% 
                            //var fksValue = string.Join(",", Model.FKs
                            //     .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            //     .Select(o => o.Split('=')[0])
                            //     .Where(o => o.HasValue())
                            //     .ToArray()
                            //     );
                            //Ronse += Html.Hidden("Edit_FKs_Value", fksValue).ToHtmlString();
                            Ronse += Html.Hidden("Edit_FKs", Model.FKs).ToHtmlString(); 
                        %></span> <span class="val"><span>
                            <%=Model.FKs%></span></span>
                </div>
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
            <div class="kv">
                <span class="key">计算列:</span> <span class="val">
                    <%=Html.TextBox("Edit_ComputeKeys", Model.ComputeKeys)%>
                </span>
            </div>
        </div>
    </div>
</asp:Content>
