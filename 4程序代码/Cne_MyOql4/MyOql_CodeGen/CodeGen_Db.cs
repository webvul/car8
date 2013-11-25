using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using System.Configuration;
using System.Reflection;

namespace MyOql.MyOql_CodeGen
{
    public partial class MyOqlCodeGen
    {


        #region 存储过程私有方法
        static string Proc_Private = @"
    public static ProcClip$ReturnModel$ $MapName$Clip(
            【for:paras】【if】,【fi】 $paraType$ $paraName$ 【endfor】)
    {
        var entName = ""$ProcName$"";
    
        ProcClip$ReturnModel$ proc = new ProcClip$ReturnModel$(entName);
        var databaseType = dbo.GetDatabaseType(proc.CurrentRule.GetConfig().db);

        var provider = dbo.GetDbProvider(databaseType) ;
    【for:genParas】
        var p_$paraName$ = provider.GetParameter(""$paraName$"",DbType.$paraDbType$ ,$paraNameValue$ );
        p_$paraName$.Direction = ParameterDirection.$paraDirection$ ;
【if:OutVarParas】
        p_$paraName$.Size = 4000 ;
【fi】
        proc.AddParameter(p_$paraName$);
    【endfor】
    【if:ReturnValue != 'void'】
        DbParameter p_result = null ;
        if (databaseType == DatabaseType.Oracle)
        {
            p_result = provider.GetParameter(【if:IsValue】 ""$returnPara$"" , $ProcReturnDbType$ 【else】 ""$returnPara$"" , (DbType)121 【fi】,null) ;
            p_result.Direction = ParameterDirection.Output;
            p_result.Size = 4000 ;

            proc.AddParameter(p_result);
        }
    【fi】
        var p_ReturnValue = provider.GetParameter(""ReturnValue"", DbType.Int32, -1);
        p_ReturnValue.Direction = ParameterDirection.ReturnValue;
        proc.AddParameter(p_ReturnValue);

        return proc ;
    }";
        #endregion

        /// <summary>
        /// 从数据库中取出部分元数据
        /// </summary>
        public static void Init()
        {
            Dictionary<string, DatabaseType> Config = new Dictionary<string, DatabaseType>();

            for (int i = 0; i < System.Configuration.ConfigurationManager.ConnectionStrings.Count; i++)
            {
                var config = System.Configuration.ConfigurationManager.ConnectionStrings[i];

                var dbType = EnumHelper.ToEnum<DatabaseType>(config.ProviderName);
                if (dbType.HasValue() == false) continue;
                Config.Add(config.Name, dbType);
            }

            //设置数据配置项的数据库类型.
            foreach (var key in Config.Keys.ToArray())
            {
                try
                {
                    ColumnDefines.AddRange(Init_GetColumns(Config[key], key));

                    TableDescriptions.AddRange(Init_GetTableDescriptions(Config[key], key));

                    ProcParaDefines.AddRange(Init_ProcParas(Config[key], key));
                }
                catch { }
            }

            GodError.Check(ColumnDefines.Count == 0, () => "在数据库中找不到列信息，请检查数据库连接字符串配置。");


            Proc_Private = TidyTemplate(Proc_Private);
        }

        private static ProcParaDetail[] Init_ProcParas(DatabaseType databaseType, string configName)
        {
            var dict = new List<ProcParaDetail>();

            if (databaseType == DatabaseType.SqlServer)
            {
                dict = dbo.ToDataTable(configName, @"
declare @tab table(name varchar(300) ) ;

DECLARE @is_policy_automation_enabled bit
SET @is_policy_automation_enabled  = (SELECT CONVERT(bit, current_value)
  FROM msdb.dbo.syspolicy_configuration
  WHERE name = 'Enabled')

insert into @tab
SELECT
sp.name AS [Name]
FROM
master.sys.databases AS dtb,
sys.all_objects AS sp
LEFT OUTER JOIN sys.database_principals AS ssp ON ssp.principal_id = ISNULL(sp.principal_id, (OBJECTPROPERTY(sp.object_id, 'OwnerId')))
LEFT OUTER JOIN sys.sql_modules AS smsp ON smsp.object_id = sp.object_id
LEFT OUTER JOIN sys.system_sql_modules AS ssmsp ON ssmsp.object_id = sp.object_id
WHERE
(sp.type = 'P' OR sp.type = 'RF' OR sp.type='PC')and(CAST(
 case 
    when sp.is_ms_shipped = 1 then 1
    when (
        select 
            major_id 
        from 
            sys.extended_properties 
        where 
            major_id = sp.object_id and 
            minor_id = 0 and 
            class = 1 and 
            name = N'microsoft_database_tools_support') 
        is not null then 1
    else 0
end          
             AS bit)='0')and( dtb.name=db_name() )
ORDER BY [Name] ASC

select '" + configName + @"' as db, 
sp.Name as ProcDbName,
stuff(param.name,1,1,'') as ParaDbName,
usrt.name as SqlType,
param.max_length as [Length],
param.Precision ,
param.Scale ,
case when param.is_output = 1 then 'Output' else 'Input' end  as Direction
FROM
sys.all_objects AS sp
INNER JOIN sys.all_parameters AS param ON param.object_id=sp.object_id
LEFT OUTER JOIN sys.types AS usrt ON usrt.user_type_id = param.user_type_id
LEFT OUTER JOIN sys.types AS baset ON (baset.user_type_id = param.system_type_id and baset.user_type_id = baset.system_type_id) or ((baset.system_type_id = param.system_type_id) and (baset.user_type_id = param.user_type_id) and (baset.is_user_defined = 0) and (baset.is_assembly_type = 1)) 
WHERE
(sp.type = 'P' OR sp.type = 'RF' OR sp.type='PC')and
(
sp.name  in ( select * from @tab )
);
").ToEntityList<ProcParaDetail>();

            }


            FixedSqlType(databaseType, dict);
            return dict.ToArray();
        }

        public static ColumnDefineDetail[] Init_GetColumns(DatabaseType dbType, string configName)
        {
            var dict = new List<StringDict>();
            if (dbType == DatabaseType.SqlServer)
            {
                //从 Sqlserver profileter 抓出SQL ， 发现： nvarchar , nchar 不对。
                // varchar(max)   长度为 -1  视为 text
                // nvarchar(max) 长度为  -1  视为 ntext 
                // 此处返回的 text,ntext 只是 varchar(max)/nvarchar(max) 的标志
                dict = dbo.ToDataTable(configName, @"
select '" + configName + @"' as db, OBJECT_NAME( c.object_id ) as TableDbName, c.name as ColumnDbName,
t.name as SqlType ,
c.Precision , 
c.max_length  as [Length], 
c.Scale,c.is_nullable as Nullable, p.value as [Description]
from  sys.columns as c 
join sys.types as t on (c.user_type_id = t.user_type_id )
left join sys.extended_properties as p on ( p.major_id = c.object_id and  p.minor_id = c.column_id )
order by c.column_id ;
").ToEntityList<StringDict>()
        ;


            }
            else if (dbType == DatabaseType.Oracle)
            {
                dict = dbo.ToDataTable(configName, @"
SELECT '" + configName + @"' as db, T.TABLE_NAME ""TableDbName"",T.COLUMN_NAME ""ColumnDbName"",  T.DATA_TYPE ""SqlType"", t.data_length ""Length"", T.data_precision ""Precision"" ,t.data_scale ""Scale""
FROM   USER_TAB_COLUMNS T 
order by column_id
").ToEntityList<StringDict>();
            }
            else if (dbType == DatabaseType.MySql)
            {
                dict = dbo.ToDataTable(configName, @"
select  `" + configName + @"` as db,Table_Name as `TableDbName`,
Column_Name as `ColumnDbName` ,
Data_Type as `SqlType` , 
Numeric_Precision as `Precision`, 
Numeric_Scale as `Scale` , 
Column_Type as `ColType`
 from information_schema.columns 
").ToEntityList<StringDict>();
            }



            var ret = Enumerable.Select(dict, o => dbo.DictionaryToModel(o, new ColumnDefineDetail())).ToArray();

            FixedSqlType(dbType, ret);

            return ret;
        }


        /// <summary>
        /// 原则是程序表示的数值范围尽可能包容下数据库里的值。如数据库定义 Numberic(3) 时，映射到程度是 Int16 而不是 Byte.(如256)
        /// </summary>
        /// <remarks>
        /// 对于 Sqlserver 来说：
        /// 数据库里的数据长度表示的是实际十进制的数字个数。 Numberic(18,2) 表示：数据总长度是18，其中有2个小数位
        /// C#中：
        /// byte 的数值最大长度为 3  位 , 所以该类型可以对应 Numberic(m) 中的  m 小于 3 时
        /// Int16 的数值最大长度为 5  位 , 所以该类型可以对应 Numberic(m) 中的  m 小于 5 时
        /// Int32 的数值最大长度为 10 位 , 所以该类型可以对应 Numberic(m) 中的  m 小于 10 时
        /// Int64 的数值最大长度为 19 位 , 所以该类型可以对应 Numberic(m) 中的  m 小于 19 时
        /// Decimal的数值最大长度为 29 位, 所以该类型可以对应 Numberic(m) 中的  m 小于 29 时
        /// 数据库里的数值最大长度为 38 位, 大于 29 位的数值在C#中只能使用 Decimal，暂时不考虑大数字类型。
        /// 注意上面使用的规则是 m 小于 类型位，如：byte 最大值  255 ， 是3 位， 但数据库中可能会存储 256 ，也是3 位，为了最大限度的让 C# 包容数据库里的数据，所以使用了 小于。
        /// 
        /// 单精度浮点数值，精度只有 7  位十进制有效数字，但内部可能维护 9 位，：http://msdn.microsoft.com/zh-cn/library/system.single.aspx
        /// 双精度浮点数值，精度只有 15 位十进制有效数字，但内部可能维护 17 位，：http://msdn.microsoft.com/zh-cn/library/system.double.aspx
        /// 
        /// by udi at 2013年4月12日
        /// 
        /// 对于 MySql 来说：
        /// 
        /// 对于 Oracle 来说：
        /// </remarks>
        /// <param name="dbType"></param>
        /// <param name="table">TableDbName</param>
        /// <param name="DbConfig"></param>
        /// <param name="Owner">目前没有使用</param>
        /// <returns></returns>
        public static void FixedSqlType<T>(DatabaseType dbType, IEnumerable<T> Rows) where T : ISqlType
        {
            if (dbType == DatabaseType.SqlServer)
            {
                foreach (var row in Rows)
                {
                    if (row.SqlType.Equals("Numeric", StringComparison.CurrentCultureIgnoreCase))
                    {
                        row.Length = row.Precision;

                        if (row.Scale == 0)
                        {
                            if (row.Precision < 3)
                            {
                                row.SqlType = "Byte";
                            }
                            else if (row.Precision < 5)
                            {
                                row.SqlType = "Int16";
                            }
                            else if (row.Precision < 10)
                            {
                                row.SqlType = "Int";
                            }
                            else if (row.Precision < 19)
                            {
                                row.SqlType = "BigInt";
                            }
                            else
                            {
                                row.SqlType = "Decimal";
                            }
                        }
                        else
                        {
                            if (row.Precision <= 7)
                                row.SqlType = "Float";
                            else if (row.Precision <= 15)
                                row.SqlType = "Double";
                            else
                                row.SqlType = "Decimal";
                            //整数
                        }
                    }

                        //2013.10.26 by udi 
                    else if (row.SqlType.Equals("varchar", StringComparison.CurrentCultureIgnoreCase) &&
                        row.Length == -1)
                    {
                        row.SqlType = "text";
                    }
                    else if (row.SqlType.Equals("nvarchar", StringComparison.CurrentCultureIgnoreCase) &&
                        row.Length == -1)
                    {
                        row.SqlType = "ntext";
                    }


                    if (row.SqlType.IsIn((a, b) => string.Equals(a, b, StringComparison.CurrentCultureIgnoreCase), "text", "ntext"))
                    {
                        //数据库中的 Length 是存储的字节数，但　nvarchar 时，ASCII 码也会占会两个字节，所以它是实际的字符个数。
                        row.Length = -1;
                    }
                }
            }
            else if (dbType == DatabaseType.Oracle)
            {
                /*
                // 未实现 Owner.
                //这里要出现 OracleDbType 枚举类型.
                //Oracle 的 SQL Developer 定义的数据类型长度是错误的. 
                //如: Int = 22 , float = 22.126 , single =22.126
                //事实是  int = 10 , int64 = 19 , float=10
                 * 
                 * Oracle 的 float 的 precision , 它是二进制单位 .
                 */

                foreach (var row in Rows)
                {
                    //FLOAT,NUMBER,INTEGER,SMALLINT, (real不会出现,会出现float)
                    /*
 * 比较复杂,因为SQLDeveloper 是错误的, 不能按它的算. 
 * float
 * integer : dataType=Number, length = 22 , precision = null , scale = 0 . 
 * smallint : dataType=Number,length =22 , precision = null , scale = 0 .
 * 
 * 按正确的计算方式:(C# 的 decimal 是28 位, 小于 oracle 的 decimal 38 位.)
 * Number 的 Length 永远是22 , 主要是看如果是整数. precision , 1-10 之间是 int , 10-19 是int64 , > 19 是 decimal .
 * 如果是float:按整数算(float 精度是7位, double 精度是16位 , 这里的精度是 十进制精度, 二进制精度* 0.3 = 十进制精度)
 * precison , 1-7 (十进制) = 1 - 24(二进制) 是 float , 24-54 是 double , > 54 是 decimal
 */
                    if (row.SqlType.IsIn((a, b) => a.Equals(b, StringComparison.CurrentCultureIgnoreCase), "FLOAT", "NUMBER", "INTEGER", "SMALLINT"))
                    {
                        if (row.SqlType.Equals("Float", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (row.Precision <= 24)
                            {
                                row.SqlType = "Single";
                            }
                            else if (row.Precision <= 54)
                            {
                                row.SqlType = "Double";
                            }
                            else
                                row.SqlType = "Decimal";
                        }
                        else if (row.SqlType.Equals("Number", StringComparison.CurrentCultureIgnoreCase))
                        {
                            row.Length = row.Precision;

                            if (row.Scale == 0)
                            {
                                if (row.Precision <= 10)
                                    row.SqlType = "Int32";
                                else if (row.Precision <= 19)
                                    row.SqlType = "Int64";
                                else
                                    row.SqlType = "Decimal";
                            }
                            else
                            {
                                if (row.Precision <= 7)
                                    row.SqlType = "Float";
                                else if (row.Precision <= 16)
                                    row.SqlType = "Double";
                                else
                                    row.SqlType = "Decimal";
                                //整数
                            }
                        }
                    }
                }
            }
            else if (dbType == DatabaseType.MySql)
            {
                foreach (var row in Rows)
                {
                    //FLOAT,NUMBER,INTEGER,SMALLINT, (real不会出现,会出现float)
                    /*
 * 比较复杂,因为SQLDeveloper 是错误的, 不能按它的算. 
 * float
 * integer : dataType=Number, length = 22 , precision = null , scale = 0 . 
 * smallint : dataType=Number,length =22 , precision = null , scale = 0 .
 * 
 * 按正确的计算方式:(C# 的 decimal 是28 位, 小于 oracle 的 decimal 38 位.)
 * Number 的 Length 永远是22 , 主要是看如果是整数. precision , 1-10 之间是 int , 10-19 是int64 , > 19 是 decimal .
 * 如果是float:按整数算(float 精度是7位, double 精度是16位 , 这里的精度是 十进制精度, 二进制精度* 0.3 = 十进制精度)
 * precison , 1-7 (十进制) = 1 - 24(二进制) 是 float , 24-54 是 double , > 54 是 decimal
 */
                    string type = row.SqlType;
                    var precision = row.Precision;
                    var scale = row.Scale;
                    var Unsign = row.SqlType.Contains("unsigned");//这里有问题，以后使用Oracle 再改。

                    if (type == "Float") type = "Single";

                    if (scale == 0)
                    {
                        if (type.Equals("SmallInt", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (Unsign) type = "UInt16";
                            else type = "Int16";
                        }
                        else if (type.Equals("TinyInt", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (Unsign) type = "UByte";
                            else type = "Byte";
                        }
                        else if (type.Equals("MediumnInt", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (Unsign) type = "UInt32";
                            else type = "Int32";
                        }
                        else if (type.Equals("BigInt", StringComparison.CurrentCultureIgnoreCase))
                        {
                            type = "Decimal";
                        }


                        if (type.IsIn((a, b) => string.Equals(a, b, StringComparison.CurrentCultureIgnoreCase), "Numeric", "Decimal"))
                        {
                            if (precision <= 10) type = "Int32";
                            else if (precision <= 19) type = "Int64";
                            else type = "Decimal";
                        }
                    }
                    else
                    {
                        if (type.IsIn((a, b) => string.Equals(a, b, StringComparison.CurrentCultureIgnoreCase), "Numeric", "Decimal"))
                        {
                            if (precision <= 7) type = "Single";
                            else if (precision <= 16) type = "Double";
                            else type = "Decimal";
                        }
                    }

                    row.SqlType = type;
                }
            }

            var provider = dbo.GetDbProvider(dbType);

            foreach (var item in Rows)
            {
                item.DbType = provider.ToDbType(item.SqlType);

                item.CsType = item.DbType.GetCsType().Name;
            }
        }

        public static TableDefineInfo[] Init_GetTableDescriptions(DatabaseType dbType, string configName)
        {
            var dict = new List<StringDict>();
            if (dbType == DatabaseType.SqlServer)
            {
                dict = dbo.ToDataTable(configName, @"
select '" + configName + @"' as db,OBJECT_NAME(p.major_id) as Name  , p.value as [Description]
from sys.extended_properties as p
join sys.tables as t  on ( p.major_id = t.object_id )
where p.class  = 1 and p.minor_id = 0
order by p.major_id,p.minor_id").ToEntityList<StringDict>();
            }


            return Enumerable.Select(dict, o => dbo.DictionaryToModel(o, new TableDefineInfo())).ToArray();
        }



    }
}
