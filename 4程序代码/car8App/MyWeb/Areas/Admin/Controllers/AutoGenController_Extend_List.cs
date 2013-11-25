using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DbEnt;
using MyOql;
using MyCmn;
using MyCon;
using System.Text;
using DbEnt;
using MyOql;

namespace MyWeb.Areas.Admin.Controllers
{
    public partial class AutoGenController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Area"></param>
        /// <param name="Group"></param>
        /// <param name="DbEntity">数据库表名</param>
        /// <returns></returns>
        public string GetListCs(string Area, string Group, string DbEntity)
        {
            #region template string
            string strCs = @"
		public ActionResult List()
		{
			return View();
		}
        
        [HttpPost]
		public ActionResult Query(string uid,$Entity$Biz.QueryModel Query)
		{
			return $Entity$Biz.Query(Query).LoadFlexiGrid();
		}
	 
        [HttpPost]
		public ActionResult Delete(string uid,FormCollection collection)
		{           
            	string[] delIds = collection[""query""].Split(',');
			return $Entity$Biz.Delete(delIds);
		}
";
            #endregion

            var objRule = new dbr().GetMyOqlEntity(DbEntity);
            //string typeName = EntityNameSpace + "." + (Group.HasValue() ? Group + "Group." : "") + Entity + "Rule+Entity";

            //var type = typeof(DictRule).Assembly.GetType(typeName);

            //Type typeRule = type.Assembly.GetType(EntityNameSpace + "." + (Group.HasValue() ? Group + "Group." : "") + Entity + "Rule");
            //var obj = Activator.CreateInstance(typeRule);
            //var ar = (ColumnClip[])obj.GetType().GetMethod("GetPrimaryKeys").Invoke(obj, null);

            strCs = strCs
                .Replace("$Entity$", objRule.GetName())
                //.Replace("$EntityId$", ar[0].Name)
                ;

            return strCs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Area"></param>
        /// <param name="Group"></param>
        /// <param name="DbEntity">数据库的表名称</param>
        /// <param name="Ref_Style"></param>
        /// <returns></returns>
        public string GetListAspx(string Area, string Group, string DbEntity, string Ref_Style)
        {
            const bool isIis6 = true;
            const string prefix = "List_";

            #region template string
            string strHtml = @"<%@ Page Title="""" Language=""C#"" MasterPageFile=""~/Areas/$Area$/Views/Shared/$Area$.Master"" Inherits=""MyCon.MyMvcPage"" %>
<asp:Content ID=""Content1"" ContentPlaceHolderID=""TitleContent"" runat=""server"">
	$EntitySmmary$列表
</asp:Content>
<asp:Content ID=""Content3"" ContentPlaceHolderID=""HeadContent"" runat=""server""> 
	<script type=""text/javascript"">
		$(function () {
            var me = jv.page() ;
			var myGrid= $("".myGrid"",jv.boxdy());

            me.flexiQuery = function () {
                var queryJDiv = $("".divQuery:last"", jv.boxdy());
                myGrid.getFlexi().doSearch(queryJDiv.GetDivJson(""List_""));
            };


			me.flexiDetail = function (rowData,rowIndex,grid,jtd) {
				Boxy.load(""~/$Area$/$Entity$/Detail/"" + rowData.id $+IIS6$, 
                    { filter: "".Main:last"", modal: true, title: ""查看详细"", width:701 }, 
                    function (bxy) { }
                );
			};

			me.flexiAdd = function () {
				Boxy.load(""~/$Area$/$Entity$/Add"" $+IIS6$, 
                    { filter: "".Main:last"", modal: true, title: ""添加"" ,width:701 },
                    function (bxy) { }
                );
			};

			me.flexiUpdate = function (rowData,rowIndex,grid,jtd) {
				Boxy.load(""~/$Area$/$Entity$/Update/"" + rowData.id $+IIS6$, 
                    { filter: "".Main:last"", modal: true, title: ""修改"" ,width:701 },
                    function (bxy) { }
                );
			};

			me.flexiDel = function (ids) {
				//删除。
				    $.post(""~/$Area$/$Entity$/Delete"" $+IIS6$, { query: ids.join("","") }, function (res) {
                    if (res.msg) 
                    {
                        alert(res.msg);
                    }
                    else { 
                        myGrid.getFlexi().populate();
                        alert(""删除成功 !""); 
                    }
				});
			};

            myGrid.flexigrid({
				title: ""$EntitySmmary$列表标题"",
				url: """",
                role: { id: ""$IdKey$"", name: ""$IdName$"" },  //id : 在被引用的时候,返回的Id值 ; name :在被引用的时候,返回的显示值.
                useId : true ,
				colModel: [
					{ display: ""查看"", toggle: false, bind: false, name: ""View"", width: ""auto"", sortable: false, align: ""center"", html:
                        '<a style=""cursor:pointer"" onclick=""jv.flexiRowEvent(me.flexiDetail,event);"">查 看</a>' }
$Columns$
				    ,{ display: ""修改"", toggle: false, bind: false, name: ""Edit"", width: ""auto"", sortable: false, align: ""center"", html:
                        '<a style=""cursor:pointer"" onclick=""jv.flexiRowEvent(me.flexiUpdate,event);"">修 改</a>' }
					],
				buttons: [
					{ separator: true },
					{ name: '添加', bclass: 'add', onpress: jv.page().flexiAdd },
					{ separator: true },
					{ name: ""删除"", bclass: ""delete"", onpress: function (ids) { if (ids.length == 0) return; if (Confirm(""确认删除这 "" + ids.length + "" 项吗？"")) {if (jv.page(jv.boxdy().last()).flexiDel(ids, grid) == false) return false;} } },
					{ separator: true }
					],
				rp: 15,
				showTableToggleBtn: true,
                onSuccess: function (grid) { jv.TestRole(); }
			});  

            myGrid.getFlexi().p.url = ""~/$Area$/$Entity$/Query"" + (me[""uid""] ? ""/"" + me[""uid""] : """") $+IIS6$;

            me.flexiQuery() ;
		}); 
		</script>
</asp:Content>
<asp:Content ID=""Content2"" ContentPlaceHolderID=""MainContent"" runat=""server"">
    <table style=""width: 100%"">
        <tr>
            <td class=""key""> 名称:</td>
            <td class=""val""> <input id=""List_Name"" />  </td>
            <td class=""key""> 开始日期: </td>
            <td class=""val""> <input id=""List_StartTime"" class=""MyDate""/> </td>
            <td class=""key""> 结束日期: </td>
            <td class=""val""> <input id=""List_EndTime"" class=""MyDate"" /> </td>
            <td class=""key""> </td>
            <td class=""val BtnQuery"">
                <input class=""submit"" type=""button"" value=""查询"" onclick=""jv.page(event).flexiQuery()"" />
            </td>
       </tr>
    </table>
    <table class=""myGrid"" style=""display: none"">
    </table>
</asp:Content>
";
            #endregion

            var objRule = new dbr().GetMyOqlEntity(DbEntity);
            var typeRule = objRule.GetType();
            //string typeName = EntityNameSpace + "." + (Group.HasValue() ? Group + "Group." : "") + Entity + "Rule+Entity";

            var ps = typeRule.GetProperties().Select(o => o.Name).ToArray();

            //var ruleType = typeof(DictRule).Assembly.GetType(EntityNameSpace + "." + (Group.HasValue() ? Group + "Group." : "") + Entity + "Rule");

            var idKey = objRule.GetIdKey();
            //var _IdKey = (typeRule.InvokeMember("GetIdKey", System.Reflection.BindingFlags.InvokeMethod, null, Activator.CreateInstance(ruleType), null) as ColumnClip);
            //if (_IdKey.EqualsNull())
            //{
            //    idKey.AddRange(
            //        typeRule.InvokeMember("GetPrimaryKeys", System.Reflection.BindingFlags.InvokeMethod, null, Activator.CreateInstance(ruleType), null) as ColumnClip[]
            //        );
            //}
            //else
            //{
            //    idKey.Add(_IdKey);
            //}

            var nameKeyProperty = objRule.GetColumns().FirstOrDefault(o => o.Name.ToLower().IsIn("text", "value", "name"));// typeRule.GetProperties().FirstOrDefault(o => o.Name.ToLower().EndsWith("text") || o.Name.ToLower() == "value" || o.Name.ToLower().EndsWith("name"));
            string NameKey = "";
            if (nameKeyProperty.EqualsNull() == false)
            {
                NameKey = nameKeyProperty.Name;
            }
            else
            {
                NameKey = "Name";
            }

            //string NameKey = string.Empty;
            //if (nameKeyProperty == null) NameKey = string.Join(",", idKey.Select(o => o.Name).ToArray());
            //else NameKey = nameKeyProperty.Name;


            var Entity = objRule.GetName();

            strHtml = strHtml

                .Replace("$Group$", Group)
                .Replace("$Area$", Area.HasValue() ? Area : "App")
                .Replace("$Entity$", Entity)
                .Replace("$EntitySmmary$", GetSummary(typeRule))
                .Replace("$Prefix$", prefix)
                .Replace("$PrefixLength$", prefix.Length.ToString())
                .Replace("$EditPrefixLength$", prefix.Length.ToString())
                .Replace("$IdKey$", idKey.Name)
                .Replace("$IdName$", NameKey)
                .Replace("$Columns$", string.Join(Environment.NewLine, ps.Select(
                    o => string.Format(@"                    ,{{ display: ""{1}"", name: ""{0}"", width: ""auto"", align: ""center"" }}", o, GetSummary(typeRule, o))
                    ).ToArray()
                    ))
                    .Replace("$+IIS6$", isIis6 ? @"+"".aspx""" : "");



            return strHtml;
        }

    }
}
