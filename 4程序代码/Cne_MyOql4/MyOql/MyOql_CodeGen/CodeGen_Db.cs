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
    【if:IsArray】
    public
    【else】 
    private
    【fi】 static ProcClip$ReturnModel$ $MapName$Clip(
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
        public void Init()
        {
            Func<DatabaseType, string, List<StringDict>> GetColumns = (dbType, configName) =>
            {
                if (dbType == DatabaseType.SqlServer)
                {
                    //从 Sqlserver profileter 抓出SQL ， 发现： nvarchar , nchar 不对。
                    // varchar(max)   长度为 -1  视为 text
                    // nvarchar(max) 长度为  -1  视为 ntext 
                    // 此处返回的 text,ntext 只是 varchar(max)/nvarchar(max) 的标志
                    return dbo.ToDataTable(configName, @"
select '" + configName + @"' as ConfigName, OBJECT_NAME( c.object_id ) as TableName, c.name as ColumnName,
case 
    when t.name = 'varchar' and c.max_length = -1 then 'text' 
    when t.name = 'nvarchar' and c.max_length = -1 then 'ntext'
    else t.name
end as Type ,
c.Precision , 
CAST(
    CASE
        WHEN t.name IN (N'nchar', N'nvarchar') AND c.max_length <> -1 THEN c.max_length/2  
        when t.name in ('text','ntext') then -1 
        ELSE c.max_length 
        END 
AS int) as [Length], 
c.Scale,c.is_nullable as Nullable
from  sys.columns as c 
join sys.types as t on (c.user_type_id = t.user_type_id )
order by c.column_id ;
").ToEntityList<StringDict>();
                }
                else if (dbType == DatabaseType.Oracle)
                {
                    return dbo.ToDataTable(configName, @"
SELECT '" + configName + @"' as ConfigName, T.TABLE_NAME ""TableName"",T.COLUMN_NAME ""ColumnName"",  T.DATA_TYPE ""Type"", t.data_length ""Length"", T.data_precision ""Precision"" ,t.data_scale ""Scale""
FROM   USER_TAB_COLUMNS T 
order by column_id
").ToEntityList<StringDict>();
                }
                else if (dbType == DatabaseType.MySql)
                {
                    return dbo.ToDataTable(configName, @"
select  `" + configName + @"` as ConfigName,Table_Name as `TableName`,
Column_Name as `ColumnName` ,
Data_Type as `Type` , 
Numeric_Precision as `Precision`, 
Numeric_Scale as `Scale` , 
Column_Type as `ColType`
 from information_schema.columns 
").ToEntityList<StringDict>();
                }

                return new List<StringDict>();
            };
            Func<DatabaseType, string, List<StringDict>> GetColumnDescriptions = (dbType, configName) =>
            {
                if (dbType == DatabaseType.SqlServer)
                {
                    return dbo.ToDataTable(configName, @"
select '" + configName + @"' as ConfigName, OBJECT_NAME(p.major_id) as TableName , c.name as ColumnName , p.value as [Descr]
from sys.extended_properties as p
join sys.all_columns as c  on ( p.major_id = c.object_id and  p.minor_id = c.column_id )
where p.class  =1 
order by p.major_id,p.minor_id").ToEntityList<StringDict>();
                }


                return new List<StringDict>();
            };

            Func<DatabaseType, string, List<StringDict>> GetTableDescriptions = (dbType, configName) =>
            {
                if (dbType == DatabaseType.SqlServer)
                {
                    return dbo.ToDataTable(configName, @"
select '" + configName + @"' as ConfigName,OBJECT_NAME(p.major_id) as TableName  , p.value as [Descr]
from sys.extended_properties as p
join sys.tables as t  on ( p.major_id = t.object_id )
where p.class  = 1 and p.minor_id = 0
order by p.major_id,p.minor_id").ToEntityList<StringDict>();
                }


                return new List<StringDict>();
            };

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
                    ColumnDefines.AddRange(GetColumns(Config[key], key));

                    ColumnDescriptions.AddRange(GetColumnDescriptions(Config[key], key));
                    TableDescriptions.AddRange(GetTableDescriptions(Config[key], key));
                }
                catch { }
            }

            GodError.Check(ColumnDefines.Count == 0, () => "在数据库中找不到列信息，请检查数据库连接字符串配置。");

            //如果一个表在多个库上定义，则去重。
            //Columns = Columns.Distinct(new CommonEqualityComparer<StringDict>((a, b) => a.Key == b.Key)).ToList();
            //表说明，选字最多的那个。
            //TableDescriptions = TableDescriptions.GroupBy(o => o["TableName"]).Select(o =>
            //{
            //    var descrLen = o.Max(x => x["Descr"].Length);
            //    return o.First(x => x["Descr"].Length == descrLen);
            //}).ToList();

            ////列说明,选字最多的那个。
            //ColumnDescriptions = ColumnDescriptions.GroupBy(o => o["TableName"] + "." + o["ColumnName"]).Select(o =>
            //{
            //    var descrLen = o.Max(x => x["Descr"].Length);
            //    return o.First(x => x["Descr"].Length == descrLen);
            //}).ToList();

            Proc_Private = TidyTemplate(Proc_Private);
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
        /// <param name="table"></param>
        /// <param name="DbConfig"></param>
        /// <param name="Owner"></param>
        /// <returns></returns>
        public static List<ColumnClip> GetColumnsFromDb(DatabaseType dbType, string table, string DbConfig, string Owner)
        {
            List<ColumnClip> list = new List<ColumnClip>();
            var Rows = ColumnDefines.Where(o =>
                string.Equals(o["ConfigName"].AsString(), DbConfig, StringComparison.CurrentCultureIgnoreCase) &&
                string.Equals(o["TableName"].AsString(), table, StringComparison.CurrentCultureIgnoreCase)
                );

            if (dbType == DatabaseType.SqlServer)
            {
                var typeMap = new Provider.SqlServer().GetTypeMap();


                foreach (var row in Rows)
                {
                    if (row["Type"].AsString().Equals("Numeric", StringComparison.CurrentCultureIgnoreCase))
                    {
                        row["Length"] = row["Precision"];

                        if (row["Scale"].AsInt(-1) == 0)
                        {
                            if (row["Precision"].AsInt(38) < 3)
                            {
                                row["Type"] = "Byte";
                            }
                            else if (row["Precision"].AsInt(38) < 5)
                            {
                                row["Type"] = "Int16";
                            }
                            else if (row["Precision"].AsInt(38) < 10)
                            {
                                row["Type"] = "Int";
                            }
                            else if (row["Precision"].AsInt(38) < 19)
                            {
                                row["Type"] = "BigInt";
                            }
                            else
                            {
                                row["Type"] = "Decimal";
                            }
                        }
                        else
                        {
                            if (row["Precision"].AsInt(38) <= 7)
                                row["Type"] = "Float";
                            else if (row["Precision"].AsInt(38) <= 15)
                                row["Type"] = "Double";
                            else
                                row["Type"] = "Decimal";
                            //整数
                        }
                    }


                    list.Add(new SimpleColumn(string.Empty, string.Empty, typeMap.FirstSqlType(row["Type"].AsString()).DbType, row["Length"].AsInt(), row["ColumnName"].AsString(), row["ColumnName"].AsString()));
                }
                return list;
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

                var typeMap = dbo.GetDbProvider(DatabaseType.Oracle).GetTypeMap();

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
                    if (row["Type"].AsString().IsIn((a, b) => a.Equals(b, StringComparison.CurrentCultureIgnoreCase), "FLOAT", "NUMBER", "INTEGER", "SMALLINT"))
                    {
                        if (row["Type"].AsString().Equals("Float", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (row["Precision"].AsInt() <= 24)
                            {
                                row["Type"] = "Single";
                            }
                            else if (row["Precision"].AsInt() <= 54)
                            {
                                row["Type"] = "Double";
                            }
                            else
                                row["Type"] = "Decimal";
                        }
                        else if (row["Type"].AsString().Equals("Number", StringComparison.CurrentCultureIgnoreCase))
                        {
                            row["Length"] = row["Precision"];

                            if (row["Scale"].AsInt(-1) == 0)
                            {
                                if (row["Precision"].AsInt(38) <= 10)
                                    row["Type"] = "Int32";
                                else if (row["Precision"].AsInt(38) <= 19)
                                    row["Type"] = "Int64";
                                else
                                    row["Type"] = "Decimal";
                            }
                            else
                            {
                                if (row["Precision"].AsInt(38) <= 7)
                                    row["Type"] = "Float";
                                else if (row["Precision"].AsInt(38) <= 16)
                                    row["Type"] = "Double";
                                else
                                    row["Type"] = "Decimal";
                                //整数
                            }
                        }
                    }

                    list.Add(new SimpleColumn(string.Empty, string.Empty, typeMap.FirstSqlType(row["Type"].AsString()).DbType, row["Length"].AsInt(), row["ColumnName"].AsString(), row["ColumnName"].AsString()));
                }
                return list;
            }
            else if (dbType == DatabaseType.MySql)
            {
                var typeMap = dbo.GetDbProvider(DatabaseType.Oracle).GetTypeMap();


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
                    var colName = row["ColumnName"].AsString();
                    string type = row["Type"].AsString();
                    var precision = row["Precision"].AsInt();
                    var scale = row["Scale"].AsInt();
                    var Unsign = row["ColType"].AsString().Contains("unsigned");

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

                    list.Add(new SimpleColumn(string.Empty, string.Empty, typeMap.FirstSqlType(row["Type"].AsString()).DbType, row["Length"].AsInt(), dbo.TranslateDbName(row["ColumnName"].AsString()), row["ColumnName"].AsString()));
                }
                return list;
            }

            return null;
        }

        public static Dictionary<string, string> GetColumnsDescriptionFromDb(DatabaseType dbType, string table, string db, string Owner)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var Rows = ColumnDescriptions.Where(o =>
                string.Equals(o["ConfigName"].ToString(), db, StringComparison.CurrentCultureIgnoreCase) &&
                string.Equals(o["TableName"].AsString(), table, StringComparison.CurrentCultureIgnoreCase)
                );
            foreach (var row in Rows)
            {
                dict[row["ColumnName"].AsString()] = row["Descr"].AsString().GetSafeValue();
            }

            var tab = TableDescriptions.FirstOrDefault(o =>
                string.Equals(o["ConfigName"], db, StringComparison.CurrentCultureIgnoreCase) &&
                string.Equals(o["TableName"], table, StringComparison.CurrentCultureIgnoreCase)
                );
            if (tab != null)
            {
                //添加表 
                dict[""] = tab["Descr"].AsString();
            }
            return dict;

        }

    }
}
