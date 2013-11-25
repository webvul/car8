<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Host/Views/Shared/Host.Master"
    Theme="Admin" Inherits="MyCon.MyMvcPage" %>

<asp:Content ID="dd" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="~/Res/MyJs_Admin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {



            var myGrid = $(".myGrid", jv.boxdy());

            $("#upFile").upFile(
                {
                    max: 1,
                    messagesmaxFile: "每次导入最多上传1个Excel",
                    allowedExtensions: ["xls", "xml"]
                });

            jv.page().ExportCustomerResult = function () {
                window.open("~/Host/Tool/ExportCustomerResult.aspx")
            };

            jv.page().ImportCustomer = function () {
                if (!jv.page()["CommID"])
                    alert("超时,请重新载入页面!");
                if (!jv.chk()) return;
                var fileIds = $("input[name=upFile]", jv.boxdy()).val();
                if (!fileIds) {
                    alert("请选择要读取的本地文件!");
                    return;
                }
                var sheetName = $("#List_SheetName", jv.boxdy()).val();
                var CommID = $("#Edit_CommID").val();

                //$.post("~/Host/Tool/ImportCustomer.aspx", { AnnexId: fileIds, SheetName: sheetName, CommID: CommID }, function (res) {
                //    if (res.msg) alert(res.msg);
                //    else {
                //        if (res.data)
                //            $("#tipBar").css("display", "").empty().append(res.data);
                //        else
                $.post("~/Host/Tool/RunSpImportRoomOwner.aspx", { CommID: jv.page()["CommID"] }, function (jm) {
                    if (jm.msg)
                        alert(jm.msg);
                    else
                        alert("导入成功");
                });

                //    }
                //});
            };

            jv.page().LoadCustomer = function (ev) {

                if (!jv.chk()) return;
                var fileIds = $("input[name=upFile]", jv.boxdy()).val();
                if (!fileIds) {
                    alert("请选择要读取的本地文件!");
                    return;
                }
                var sheetName = $("#List_SheetName", jv.boxdy()).val();

                $.post("~/Host/Tool/LoadCustomer.aspx", { AnnexId: fileIds, SheetName: sheetName, CommID: jv.page()["CommID"] }, function (jm) {
                    if (jm.msg)
                        alert(jm.msg)
                    else {
                        myGrid.getFlexi().p.url = "~/Host/Tool/QueryCustomer.aspx";
                        myGrid.getFlexi().doSearch({ CommID: jv.page()["CommID"] });
                    }
                });

            };

            myGrid.flexigrid({
                title: "",
                url: "",
                role: { id: "Id", name: "Id" },
                colModel: [
                     { display: "Id", name: "Id", width: "0", align: "left" },
                     { display: "楼宇名称", name: "BuildingName", align: "left" },
                     { display: "单元号", name: "UnitNum", align: "left" },
                     { display: "楼层号", name: "FloorNum", align: "left" },
                     { display: "房间编码", name: "RoomCode", align: "left", html: function (a, b, c, d) { return "<span style='color:red;'>" + c.TableRowToData(a.cell[0]).RoomCode + "</span>"; } },
                     { display: "建筑面积", name: "ScBldArea", align: "left" },
                     { display: "套内面积", name: "ScTnArea", align: "left" },
                     { display: "物业接管时间", name: "PropertyTakeoverDate", align: "left" },
                     { display: "登录名称", name: "LoginName", align: "left", html: function (a, b, c, d) { return "<span style='color:red;'>" + c.TableRowToData(a.cell[0]).LoginName + "</span>"; } },
                     { display: "住户名称", name: "OwnerName", align: "left" },
                     { display: "性别", name: "OwnerSex", align: "left" },
                     { display: "联系人电话", name: "LinkMobile", align: "left" },
                     { display: "身份证号", name: "IdNumber", align: "left" },
                     { display: "邮箱", name: "Email", align: "left" },
                { display: "邮寄地址", name: "MailAddress", align: "left" },
                { display: "IsMulti", name: "IsMulti", align: "left", width: 0 }


                ],
                dataType: "json",
                buttons: [],
                usepager: true,
                FillHeight: false,
                height: 300,
                ShowTableToggleBtn: true,
                onSuccess: function (a, b, c, d) {
                    for (var i = 0 ; i < b.rows.length; i++) {
                        if (i > 1) {
                            var $row = a.TableRowToData(b.rows[i]);
                            if ($row.IsMulti) {
                                b.rows[i].style.color = "red";
                                b.rows[i].title = "存在相同登录名称的用户";
                            }
                        }
                    }
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div style="text-align: center;" class="FillHeight">
        <div id="divStep2">
            <div class="MyTitle">
                第一步：上传文件
            </div>
            <span id="upFile"></span>
            <input id="List_SheetName" style="display: none" chk="function(val){if(!val) return '请输入Excel表的工作表名!';}"
                value="Sheet1" /><input type="button" value="读取" onclick="return jv.page().LoadCustomer();" />
            <input style="position: absolute; top: 30px; right: 200px;" type="button" onclick="window.open('~/住户Excel模版.xls')" value="下载住户Excel模版" />
        </div>
        <div id="divStep3">
            <div class="MyTitle">
                第二步：确定导入
            </div>
            <div class="kv">
                <%--     <span class=" must">关联小区：<input id="Edit_CommID" type="hidden" value="0" /></span>

                <input readonly="readonly" class="ref" onclick="jv.PopList({ mode: 'radio', area: 'Admin', entity: 'Dept', list: 'CommList', callback: function (a, b, c, d) { if (b[0].CommID) $('#Edit_CommID').val(b[0].CommID) } }, event);" chk="function(val){ if ( val == '0' || val.length == 0 ) return '请选择小区'; }" chkval="#Edit_CommID" value="" />
                --%>
                <input type="button" id="btnImport" value="确定导入" onclick="return jv.page().ImportCustomer();" />

            </div>
        </div>
        <div id="tipBar" style="height: 100px; overflow-y: scroll; text-align: left; display: none;">
        </div>
        <table class="myGrid" style="display: none;">
        </table>
    </div>
</asp:Content>
