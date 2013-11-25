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
using System.IO;
using DbEnt;
using MyOql;

namespace MyWeb.Areas.Admin.Controllers
{
    public partial class AutoGenController
    {
        public ActionResult Compress(string uid)
        {
            List<string> list = new List<string>();
            var jss = Directory.GetFiles(Server.MapPath("~/"), "*.js", SearchOption.AllDirectories);
            var csss = Directory.GetFiles(Server.MapPath("~/"), "*.css", SearchOption.AllDirectories);
            list.AddRange(jss);
            list.AddRange(csss);
            return View(list);
        }

        public ActionResult WebSite(string uid)
        {
            List<string> list = new List<string>();

            return View(list);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="DbEntity">数据库表名</param>
        /// <returns></returns>
        public string GetBizCs(string group, string DbEntity)
        {
            var obj = new dbr().GetMyOqlEntity(DbEntity);
            var idKey = obj.GetIdKey();// (ColumnClip)obj.GetType().GetMethod("GetIdKey").Invoke(obj, null);
            var autoKey = obj.GetAutoIncreKey();// (ColumnClip)obj.GetType().GetMethod("GetAutoIncreKey").Invoke(obj, null);
            var pks = obj.GetPrimaryKeys();// (ColumnClip[])obj.GetType().GetMethod("GetPrimaryKeys").Invoke(obj, null);
            var typeRule = obj.GetType();

            //string typeName = EntityNameSpace + "." + (group.HasValue() ? group + "Group." : "") + entity + "Rule+Entity";

            //var type = typeof(DictRule).Assembly.GetType(typeName);

            //Type typeRule = type.Assembly.GetType(EntityNameSpace + "." + (group.HasValue() ? group + "Group." : "") + entity + "Rule");

            #region template string
            string strCs = @"

        //根据需要修改参数
		public static MyOqlSet Query(QueryModel Query)
		{
			WhereClip where = new WhereClip();

			Query.Name.HasValue(o=> where &= dbr.$GroupInstanceDot$$Entity$.Name.Like(""%"" + o + ""%"") );
			Query.StartTime.HasValue(o=> where &= dbr.$GroupInstanceDot$$Entity$.StartTime.Like(""%"" + o + ""%"") );
			Query.EndTime.HasValue(o=> where &= dbr.$GroupInstanceDot$$Entity$.EndTime.Like(""%"" + o + ""%"") );

			return dbr.$GroupInstanceDot$$Entity$.Select()
				.Skip( Query.skip )
				.Take( Query.take )
				.Where(where)
				.OrderBy(Query.sort)
                .ToMyOqlSet(Query.option) ;
		}

		public static JsonMsg<int> Update($Entity$Rule.Entity Model)
		{
            //验证是否包含唯一约束列 或 主键 ； 是否包含其它必须参数.      如果是联合主键,判断方式会稍有不同,请自行修改.
【if:pks==1】
【if:id==number】
            if (Model.$IdKey$  == 0 ) 
【else】
            if (Model.$IdKey$.HasValue() == false ) 
【fi】
            {
                return new JsonMsg<int>(){ msg = ""更新对象,必须拥有唯一键,或主键!"" };
            }
【else】
            if ( Model.PK1 == 0 || Model.PK2.HasValue() == false)
            {
                return new JsonMsg<int>(){ msg = ""更新对象,必须拥有唯一键,或主键!"" };
            }
【fi】

            var jm = new JsonMsg<int>();
            try
            {
			    jm.value = dbr.$GroupInstanceDot$$Entity$.Update(Model).Execute() ;
            }
            catch(Exception e)
            {
                jm.msg = e.Message.GetSafeValue() ;
            }
            return jm ;
	    }

		public static JsonMsg<int> Add($Entity$Rule.Entity Model)
		{
【if:pks==1】
    【if:id!=auto】
        【if:id==number】
            if (Model.$IdKey$  == 0)
        【else】
            if (Model.$IdKey$.HasValue())
        【fi】
            {
                return new JsonMsg<int>(){ msg = ""创建对象必须拥有唯一键或主键!"" } ;
            }
    【fi】
【else】
            if ( Model.PK1 ==0  || Model.PK2.HasValue() == false)
            {
                return new JsonMsg<int>(){ msg = ""创建对象必须拥有主键!"" } ;
            }
【fi】
			var jm = new JsonMsg<int>();
            try
            {
                jm.value = dbr.$GroupInstanceDot$$Entity$.Insert(Model).Execute() ;
            }
            catch(Exception e)
            {
                jm.msg = e.Message.GetSafeValue();
            }
            return jm ;
		}

		//根据需要修改参数
        public static JsonMsg<int> Delete(string[] IdArray)
		{
            //判断要删除的Id 是否合法.
            if ( IdArray.Length == 0 ) 
            {
                return new JsonMsg<int>{ msg =  ""没有要删除的对象!"" } ;
            }
            var IdInDb = dbr.$GroupInstanceDot$$Entity$
                    .Select(o=>o.$IdKey$)
                    .Where( o=>o.$IdKey$.In( IdArray ))
                    .ToEntityList(string.Empty) ;

            var dbLoseId = IdArray.Minus( IdInDb ) ;
            if ( dbLoseId.Count() > 0 )
            {
                return new JsonMsg<int>()
                {
                    msg = string.Format(""要删除的数据Id ({0}) 在数据库中不存在, 可能数据已过时,请刷新后再试."", string.Join("","", dbLoseId.ToArray()))
                };
            }

            //添加业务判断。

            var jm = new JsonMsg<int>() ;
			try
			{
				var delResult = dbr.$GroupInstanceDot$$Entity$.Delete(o => o.$IdKey$.In(IdArray)).Execute();
                jm.value = delResult;
				if (delResult != IdArray.Length )
				{
				    jm.msg = string.Format(""删除了 {0}/{1} 条记录."", delResult, IdArray.Length); 
                }
			}
			catch (Exception ee)
			{
				jm.msg = ee.Message.GetSafeValue();
			}

			return jm;
		}
";
            #endregion

            strCs = strCs
                .TmpIf()
                //.Var("HasGroup", () => group.HasValue())
                .Var("pks==1", () => idKey.EqualsNull() == false)
                .Var("id!=auto", () => idKey.EqualsNull() == false && autoKey.EqualsNull() == false && idKey.Name != autoKey.Name)
                .Var("id==number", () => idKey.EqualsNull() == false && idKey.DbType.DbTypeIsNumber())
                .EndIf()

                .Replace("$Group$", group)
                .Replace("$GroupInstanceDot$", group.HasValue() ? group + "." : "")
                .Replace("$Entity$", obj.GetName())
                .Replace("$IdKey$", idKey.EqualsNull() ? "" : idKey.Name)
                ;


            return strCs;
        }

    }
}
