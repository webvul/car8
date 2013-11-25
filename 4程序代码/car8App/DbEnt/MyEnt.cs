using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyOql;
using System.Web.Mvc;
using MyCmn;
using System.Configuration;
using System.Data;

namespace DbEnt
{
    /// <summary>
    /// 
    /// </summary>
    public static class MyEnt
    {
        public static void Main()
        {
            
        }

        public static DataTable GetLockDetail<T>(this T entity)
            where T : RuleBase
        {
            var dt = dbo.ToDataTable(entity.GetConfig().db, @"
select
a.session_id as SessionId, 
s.[Text] as [Sql]  ,
t.resource_type as ResouceType, 
request_mode as RequestMode, 
resource_description as ResouceDescription, 
resource_associated_entity_id as EntityId, 
request_status  as [Status]
from sys.dm_exec_connections a 
cross apply sys.dm_exec_sql_text(a.most_recent_sql_handle) as s 
join sys.dm_tran_locks as t on ( a.session_id = t.request_session_id )
where t.request_owner_type = 'TRANSACTION'
");
            return dt;
        }


        public static void ToLog(this Exception exp, string Detail = null)
        {
            var godError = exp as GodError;
            if (godError != null)
            {
                Log.To("GodError", exp.Message, Detail.AsString(godError.Detail), exp.StackTrace, 0M);
            }
            else
            {
                Log.To("Exception", exp.Message, Detail.AsString(exp.Source), exp.StackTrace, 0M);
            }
        }

        public static void LogTo(this InfoEnum key, string Msg, string Detail)
        {
            Log.To(key, Msg, Detail);
        }


        public static string Get(this ConfigKey key)
        {
            return ConfigurationManager.AppSettings[key.ToString()];
        }




        public static ColumnClip JoinStr(this ColumnClip column)
        {
            var retVal = new ComplexColumn();
            retVal.Operator = (SqlOperator)1025;
            retVal.DbType = System.Data.DbType.AnsiString;
            retVal.LeftExp = column.Clone() as ColumnClip;
            retVal.RightExp = new ConstColumn(",");
            return retVal;
        }

        public static ColumnClip GetDateString(this ColumnClip column)
        {
            var para = new ComplexColumn();
            para.Operator = SqlOperator.Property;
            para.LeftExp = column.Clone() as ColumnClip;
            para.RightExp = new ConstColumn(",");

            var col = new ComplexColumn(column.Name.AsString("FN_GetDateString"));
            col.Operator = (SqlOperator)1026;
            col.DbType = System.Data.DbType.AnsiString;

            if (column.IsSimple())
            {
                var simple = column as SimpleColumn;

                col.LeftExp = new RawColumn(ValueMetaTypeEnum.StringType, simple.TableName + "." + simple.DbName);
            }
            else
            {
                col.LeftExp = new ConstColumn(column.Name);
            }


            return col;
        }

    }

    public enum WorkFlowTypeEnum
    {
        /// <summary>
        /// 计划确认
        /// </summary>
        PlanConfirm,

    }

    public class WFInput
    {
        public YesNoEnum Action { get; set; }

        public string Sender { get; set; }
        /// <summary>
        /// 发送到某人
        /// </summary>
        public string Recver { get; set; }

        public WorkFlowTypeEnum WorkType { get; set; }

        //public int BizID { get; set; }
        ///// <summary>
        ///// 绑定数据库中 Role 的名称列表.
        ///// </summary>
        //public string Role { get; set; }

        ///// <summary>
        ///// 绑定数据库中 UserID 的列表.
        ///// </summary>
        //public string User { get; set; }

        public string Sugestion { get; set; }
    }
}
