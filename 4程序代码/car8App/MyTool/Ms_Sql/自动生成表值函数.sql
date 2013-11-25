--请先切换数据库

declare @tab table( sc varchar(10) , Name varchar(200) , Ft int )


DECLARE @is_policy_automation_enabled bit
SET @is_policy_automation_enabled  = (SELECT CONVERT(bit, current_value)
  FROM msdb.dbo.syspolicy_configuration
  WHERE name = 'Enabled')
      

insert into @tab (Name , sc , Ft )
SELECT
udf.name AS [Name],
SCHEMA_NAME(udf.schema_id) AS [Schema],

(case when 'FN' = udf.type then 1 when 'FS' = udf.type then 1 when 'IF' = udf.type then 3 when 'TF' = udf.type then 2 when 'FT' = udf.type then 2 else 0 end) AS [FunctionType]
FROM
master.sys.databases AS dtb,
sys.all_objects AS udf
LEFT OUTER JOIN sys.database_principals AS sudf ON sudf.principal_id = ISNULL(udf.principal_id, (OBJECTPROPERTY(udf.object_id, 'OwnerId')))
WHERE
(udf.type in ('TF', 'FN', 'IF', 'FS', 'FT'))and(((case when 'FN' = udf.type then 1 when 'FS' = udf.type then 1 when 'IF' = udf.type then 3 when 'TF' = udf.type then 2 when 'FT' = udf.type then 2 else 0 end)='3' or (case when 'FN' = udf.type then 1 when 'FS' = udf.type then 1 when 'IF' = udf.type then 3 when 'TF' = udf.type then 2 when 'FT' = udf.type then 2 else 0 end)='2') and CAST(
 case 
    when udf.is_ms_shipped = 1 then 1
    when (
        select 
            major_id 
        from 
            sys.extended_properties 
        where 
            major_id = udf.object_id and 
            minor_id = 0 and 
            class = 1 and 
            name = N'microsoft_database_tools_support') 
        is not null then 1
    else 0
end          
             AS bit)='0') and( dtb.name=db_name() )

 
---------------------------------------------------------------------------------------
SELECT
udf.name as Obj_Name ,
--dbo.JoinStr(udf.name,',') , 

'<Entity Name="' + udf.name +'" Paras="' + 

dbo.JoinStr(
substring( param.name ,2,8000) +'=' + dbo.F_To_DbType( usrt.name,param.precision , param.scale ) +':' + 
case when param.is_output = 1 then 'out' else 'in' end  ,',')

+
'" />'

FROM
sys.all_objects AS udf
INNER JOIN sys.all_parameters AS param ON (param.is_output = 0) AND (param.object_id=udf.object_id)
LEFT OUTER JOIN sys.types AS usrt ON usrt.user_type_id = param.user_type_id
LEFT OUTER JOIN sys.types AS baset ON (baset.user_type_id = param.system_type_id and baset.user_type_id = baset.system_type_id) or ((baset.system_type_id = param.system_type_id) and (baset.user_type_id = param.user_type_id) and (baset.is_user_defined = 0) and (baset.is_assembly_type = 1)) 
WHERE
(udf.type in ('TF', 'FN', 'IF', 'FS', 'FT'))and
(
udf.name +'.' +  SCHEMA_NAME(udf.schema_id) in ( select Name +'.' + sc from @tab )
)

group by udf.name
order by udf.name asc  

