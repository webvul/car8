<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="MyCon.MyMvcPage<string[]>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    代码生成
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        
    </style>
    <script type="text/javascript">
        $(function () {
            //            $(".key").click(function (ev) {
            //                jv.GetValueDisplay({ obj: this })
            //                .css("width", "100%")
            //                .css("height", "100%")
            //                .css("position", "absolute")
            //                .css("left", "0px")
            //                .css("top", "0px");
            //            });

            $("textarea").PopTextArea();
        });

        function toFile() {
            if (Confirm("将覆盖 List.ASPX,Card.aspx, Controller_ListCard.cs 以及 Biz_Extend.cs 文件，请谨慎！！！") == false) {
                return false;
            }
            else {
                $.post("~/Admin/AutoGen/WriteFile.aspx?Area=" + jv.url().kv.Area +
                    "&Group=" + jv.url().kv.Group +
                    "&Entity=" + jv.url().kv.Entity +
                    "&Ref_Style=" + jv.url().kv.Ref_Style, function (res) {
                        //客户端返回的是Json
                        if (res.msg) alert(res.msg);
                        else alert("生成成功");
                    });
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="MyTool">
        <input type="button" value="写入文件" onclick="toFile()" />

    </div>
    <div class="MyCard">
        <div>
            <div class="kv">
                <span class="key">List_Aspx:</span> <span class="val">
                    <textarea style="height: 200px"><%=Model[0]%></textarea></span>
            </div>
            <div class="kv">
                <span class="key">Card_Aspx: </span><span class="val">
                    <textarea style="height: 200px"><%=Model[1]%></textarea>
                </span>
            </div>
        </div>
        <div class="FillHeight- divSplit">
        </div>
        <div>
            <div class="kv">
                <span class="key">List_Card_Controller_Cs :</span> <span class="val">
                    <textarea style="height: 200px"><%=Model[3]%></textarea></span>
            </div>
            <div class="kv">
                <span class="key">Biz_Extend_Cs : </span><span class="val">
                    <textarea style="height: 200px"><%=Model[5]%></textarea></span>
            </div>
        </div>
    </div>
</asp:Content>
