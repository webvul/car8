<%@ Page Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="MyCon.MyMvcPage<MyOql.MyOqlCodeGenSect.MyOqlCodeGenProcsCollection.MyOqlCodeGenProcElement>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    视图
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {


            jv.SetDetail({ detail: "EntityProcDetail" });


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
                        ret.push((tr.Column + "=" + tr.Type + ":" + tr.Direction).trimAll());
                    });
                    return ret;
                };

                jv.PopList({ entity: "AutoGen", list: "Entity_Proc_ParaList", detail: "Entity_Proc_ParaList", query: { entity: $("#Edit_Name").val(), db: $("#Edit_db").val(), owner: $("#Edit_Owner").val() }, mode: "check",
                    dataSource: function () {
                        var data = {};
                        data.rows = [];
                        data.title = "存储过程" + jv.page().uid + "参数设置";

                        $($("#Edit_Paras").val().split(",")).each(function (i, paras_val) {
                            if (!paras_val) return;
                            var paras = $(jv.SplitWithReg(paras_val, "=|:"))
                                .filter(function (i, d) { return !d.Split; })
                                .map(function (i, d) { return d.Value; });

                            data.rows.push({ id: paras[0], cell: paras });
                        });

                        data.total = data.rows.length;
                        return data;
                    },
                    callback: function (role, kv, add, minus, grid) {
                        var ToFksString = function (jObj) {
                            var fs = [];
                            $(jObj).each(function (i, d) {
                                fs.push(d.col + "=" + d.type + ":" + d.mapName);
                            });
                            return fs.join(",");
                        };

                        var enmValue = GetEnmValue(grid);


                        $("#Edit_Paras").val(enmValue.join(","));
                        jv.SetDisplayValue($("#Edit_Paras"), enmValue.join(","));
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
                <span class="key" title="">返回值:</span> <span class="val">
                    <%=Html.TextBox("Edit_Return", (Model.Return))%>
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
                <span class="key Link" onclick="jv.page().EnumKey(event);">参数格式:
                    <% 
                        //var eumValue = string.Join(",", Model.Enums
                        //     .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        //     .Select(o => o.Split('=')[0])
                        //     .Where(o => o.HasValue())
                        //     .ToArray()
                        //     );
                        Ronse += Html.Hidden("Edit_Paras", Model.Paras).ToHtmlString(); 
                    %>
                </span><span class="val"><span>
                    <%=Model.Paras%></span></span>
            </div>
        </div>
    </div>
</asp:Content>
