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
using System.Reflection;
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
        public string GetEditCs(string Area, string Group, string DbEntity)
        {
            #region template string
            string strCs = @"
		public ActionResult Detail($EntityIdType$ uid)
		{
			return View(""Card"", dbr.$GroupInstanceDot$$Entity$.FindBy$EntityId$(uid));
		}
		public ActionResult Add(string uid)
		{
			return View(""Card"", new $Entity$Rule.Entity());
		}

		[HttpPost]
		public ActionResult Add(string uid,$Entity$Rule.Entity Entity)
		{
            //没有消息,就是好消息.
            var jm = $Entity$Biz.Add(Entity) ;
 
            return jm ;
		}

		public ActionResult Update($EntityIdType$ uid)
		{
			return View(""Card"",dbr.$GroupInstanceDot$$Entity$.FindBy$EntityId$(uid));
		}

		[HttpPost]
		public ActionResult Update($EntityIdType$ uid, $Entity$Rule.Entity Entity)
		{
            //没有消息,就是好消息.
            var jm = $Entity$Biz.Update(Entity) ;
 
            return jm ;
		}
";
            #endregion

            var objRule = new dbr().GetMyOqlEntity(DbEntity);
            //Type typeRule = objRule.GetType(); //typeEntity.Assembly.GetType(EntityNameSpace + "." + (Group.HasValue() ? Group + "Group." : "") + Entity + "Rule");
            //string typeName = EntityNameSpace + "." + (Group.HasValue() ? Group + "Group." : "") + Entity + "Rule+Entity";
            //var typeEntity = typeof(DictRule).Assembly.GetType(typeName);
            //var pks = (ColumnClip[])objRule.GetType().GetMethod("GetPrimaryKeys").Invoke(objRule, null);
            var idKey = objRule.GetIdKey();// (ColumnClip)objRule.GetType().GetMethod("GetIdKey").Invoke(objRule, null);

            strCs = strCs.TmpIf()
                .Var("HasGroup", () => Group.HasValue())
                .EndIf()
                .Replace("$Group$", Group)
                .Replace("$GroupInstanceDot$", Group.HasValue() ? Group + "." : "")
                .Replace("$GroupDefineDot$", Group.HasValue() ? Group + "Group." : "")
                .Replace("$Entity$", objRule.GetName())
                .Replace("$EntityId$", idKey.EqualsNull() ? "/* 由于是多主键,请自行修改 .*/" : idKey.Name)
                .Replace("$EntityIdType$", idKey.EqualsNull() ? "string" : idKey.DbType.GetCsType().Name);


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
        public string GetCardAspx(string Area, string Group, string DbEntity, string Ref_Style)
        {
            bool IsIIS6 = true;
            string Prefix = "Edit_";

            #region template string
            string strHtml = @"<%@ Page Language=""C#"" MasterPageFile=""~/Areas/$Area$/Views/Shared/$Area$.Master""  Inherits=""MyCon.MyMvcPage<" + EntityNameSpace + @".$GroupDefineDot$$Entity$Rule.Entity>"" %>
<asp:Content ID=""Content1"" ContentPlaceHolderID=""TitleContent"" runat=""server"">
	$EntitySmmary$ 信息
</asp:Content>
<asp:Content ID=""Content3"" ContentPlaceHolderID=""HeadContent"" runat=""server"">
    <script src=""~/Res/MyJs_Admin.js"" type=""text/javascript""></script>
	<script type=""text/javascript"">
		$(function () {
            var me = jv.page() ;
			jv.SetDetail();
 
		    me.Edit_OK = function(ev) {
			    $.post(jv.url(), $("".Main"",jv.boxdy()).GetDivJson(""$Prefix$"") , function (res) {
                    if (res.msg) alert(res.msg);
                    else {
                        jv.Hide(function () { 
                            $("".flexigrid"", jv.boxdy()).getFlexi().populate();
                        }); 
                        alert(""保存成功"");
                        if (jv.IsInBoxy(ev)) {}
                    }
			    });
		    }
		});
	</script>
</asp:Content>
<asp:Content ID=""Content2"" ContentPlaceHolderID=""MainContent"" runat=""server"">
        <div class=""MyTool"">
            <input class=""submit"" type=""button"" value=""保存"" onclick=""jv.page().Edit_OK(event)"" />
        </div>
        <div class=""MyCard"">
            <div>
                <div class=""MyCardTitle"">
                    基本信息
                </div>
【for:col】【if:isFK】
                <%= dbr.$RefTable$.PopRadior(""Edit_$ProKey$"", new UIPop{ 
                        Area = ""$FkArea$"",
                        KeyTitle= ""$SummaryKey$"" ,
                        Value = Model.$ProKey$.HasValue() ? Model.$ProKey$.AsString() : """",
                        Display =  Model.$ProKey$.HasValue() ? Model.Get$RefTable$().$RefColumn$.AsString() : ""(空)""
                }).GenDiv()
%>【eif:isEnum】
                <div class=""kv"">
                    <span class=""key"">$SummaryKey$:</span> <span class=""val"">
                        <%= Html.RegisteEnum(""Edit_$ProKey$"", Model.$ProKey$)
                                .Input()
                                .Radior()
                        %> </span>
                </div>【else】
                <div class=""kv"">
                    <span class=""key"">$SummaryKey$:</span>
                    <span class=""val【if】 hard【fi】"">【if】<span>  
                    <%
                        if (ActionIsAdd)
                        {
                            Ronse += ""添加后生成"";
                        }
                        else
                        {
                            Ronse += Model.Id.AsString();
                        }
                    %> </span><%=Html.Hidden(""$Prefix$$ProKey$"", Model.$ProKey$)%></span>
【else】<input id=""$Prefix$$ProKey$"" name=""$Prefix$$ProKey$"" value=""<%=Model.$ProKey$%>"" $Style$ maxlength=""$MaxLen$"" dbtype=""$DbType$"" dblen=""$DbLen$"" chk=""$""/></span>
【fi】                </div>
【fi】【endfor】
            </div>
            <div class=""FillHeight- divSplit"">
            </div>
            <div>
                <div class=""MyCardTitle"">
                    扩展信息
                </div>
                <div class=""kv"">
                        <span class=""key"">手动分布成两列: </span>
                        <span class=""val"">!!</span>
                </div>
            </div>
        </div>
</asp:Content>";
            #endregion

            var objRule = new dbr().GetMyOqlEntity(DbEntity) as RuleBase;
            var typeEntity = FastInvoke.GetPropertyValue(objRule, null, "_").GetType();
            string typeName = typeEntity.FullName;// EntityNameSpace + "." + (Group.HasValue() ? Group + "Group." : "") + Entity + "Rule+Entity";

            Type typeRule = objRule.GetType();
            //var objEntity = Activator.CreateInstance(typeEntity) ;
            var ps = typeEntity.GetProperties().Select(o => o.Name).ToArray();
            var autoKey = objRule.GetAutoIncreKey();// (typeRule.GetMethod("GetAutoIncreKey").Invoke(objRule, null) as ColumnClip);

            var GetFkFunc = new Func<string, FkColumn>((dbColumn) =>
            {
                return new dbr().GetFkDefines().FirstOrDefault(o => o.Entity == DbEntity && o.Column == dbColumn);
                //var fkAttrs = typeRule.GetProperty(p).GetCustomAttributes(typeof(MyOqlFKAttribute), false);
                //if (fkAttrs == null || fkAttrs.Length == 0) return null;
                //else return fkAttrs[0] as MyOqlFKAttribute;
            });

            strHtml = strHtml
                .TmpFor("col", ps.ToList())
                .DoIf(new CodeGen.DoIfObject<string>("", (col) => autoKey.EqualsNull() == false && col == autoKey.Name),
                    new CodeGen.DoIfObject<string>("isFK", (str) =>
                {
                    //反射取实体
                    return GetFkFunc(str) != null;
                }),
                new CodeGen.DoIfObject<string>("isEnum", (str) =>
                {
                    return typeEntity.GetProperty(str).PropertyType.IsEnum;
                })
                )
                .DoFor("ProKey", o => o)
                .DoFor("SummaryKey", o => GetSummary(typeRule, o))
                .DoFor("FkArea", o =>
                    {
                        var fk = GetFkFunc(o);
                        if (fk == null) return string.Empty;
                        var et = typeEntity.Assembly.GetTypes().First(a => a.FullName.Contains("." + fk.RefTable + "Rule+") ||
                            a.FullName.Contains("+" + fk.RefTable + "Rule+"));
                        var ret = et.Namespace.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last();
                        if (ret == EntityNameSpace) return string.Empty;
                        else return ret.AsString("Admin");
                    })
                .DoFor("RefTable", o =>
                    {
                        var fk = GetFkFunc(o);
                        if (fk == null) return string.Empty;
                        else return fk.RefTable;
                    })
                .DoFor("RefColumn", o =>
                    {
                        var fk = GetFkFunc(o);
                        if (fk == null) return string.Empty;
                        else return fk.RefTable;
                    })
                .DoFor("DbLen", o =>
                    {
                        return (objRule.GetType().GetProperty(o).GetValue(objRule, null) as ColumnClip).Length.ToString();
                    })
                .DoFor("MaxLen", o =>
                    {
                        var dbType = (objRule.GetType().GetProperty(o).GetValue(objRule, null) as ColumnClip).DbType;
                        var dbLen = (objRule.GetType().GetProperty(o).GetValue(objRule, null) as ColumnClip).Length;
                        switch (dbType)
                        {
                            case System.Data.DbType.AnsiString:
                            case System.Data.DbType.AnsiStringFixedLength:
                                return dbLen.ToString();
                            case System.Data.DbType.Binary:
                            case System.Data.DbType.Object:
                            case System.Data.DbType.Xml:
                            case System.Data.DbType.Double:
                            case System.Data.DbType.Single:
                            case System.Data.DbType.VarNumeric:
                                return "";
                            case System.Data.DbType.Boolean:
                                return "2";
                            case System.Data.DbType.Byte:
                                return "3";
                            case System.Data.DbType.Decimal:
                            case System.Data.DbType.Currency:
                                return "21";
                            case System.Data.DbType.Date:
                            case System.Data.DbType.DateTimeOffset:
                                return "11";
                            case System.Data.DbType.DateTime:
                            case System.Data.DbType.DateTime2:
                            case System.Data.DbType.Time:
                                return "9";
                            case System.Data.DbType.Guid:
                                return "36";
                            case System.Data.DbType.Int16:
                                return "6";
                            case System.Data.DbType.Int32:
                                return "11";
                            case System.Data.DbType.Int64:
                                return "20";
                            case System.Data.DbType.SByte:
                                return "4";
                            case System.Data.DbType.String:
                            case System.Data.DbType.StringFixedLength:
                                return dbLen.ToString();
                            case System.Data.DbType.UInt16:
                                return "5";
                            case System.Data.DbType.UInt32:
                                return "9";
                            case System.Data.DbType.UInt64:
                                return "19";
                            default:
                                break;
                        }
                        return "";
                    })
                .DoFor("DbType", o =>
                    {
                        return (objRule.GetType().GetProperty(o).GetValue(objRule, null) as ColumnClip).DbType.ToString();
                    })
                .EndFor()
                ;


            var Entity = objRule.GetName();

            strHtml = strHtml

                .Replace("$Group$", Group)
                .Replace("$Area$", Area.HasValue() ? Area : "App")
                .Replace("$GroupInstanceDot$", Group.HasValue() ? Group + "." : "")
                .Replace("$GroupDefineDot$", Group.HasValue() ? Group + "Group." : "")
                .Replace("$Entity$", Entity)
                .Replace("$EntitySmmary$", GetSummary(typeRule))
                .Replace("$EditPrefixLength$", Prefix.Length.ToString())
                .Replace("$Prefix$", Prefix)
                .Replace("$Style$", Ref_Style == "Pm" ? @"class=""ref""" : "")
                .Replace("$+IIS6$", IsIIS6 ? @"+"".aspx""" : "");


            return strHtml;
        }
    }
}
