--请先切换数据库


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



SELECT
sp.Name as Proc_Name,
'<Entity Name="' + sp.Name + '"  Paras="' +
dbo.JoinStr( substring(param.name , 2,8000) +'='+ dbo.F_To_DbType(usrt.name,param.precision , param.scale) +':' +
case when param.is_output = 1 then 'out' else 'in' end  ,',') +'" />'
FROM
sys.all_objects AS sp
INNER JOIN sys.all_parameters AS param ON param.object_id=sp.object_id
LEFT OUTER JOIN sys.types AS usrt ON usrt.user_type_id = param.user_type_id
LEFT OUTER JOIN sys.types AS baset ON (baset.user_type_id = param.system_type_id and baset.user_type_id = baset.system_type_id) or ((baset.system_type_id = param.system_type_id) and (baset.user_type_id = param.user_type_id) and (baset.is_user_defined = 0) and (baset.is_assembly_type = 1)) 
WHERE
(sp.type = 'P' OR sp.type = 'RF' OR sp.type='PC')and
(
sp.name  in ( select * from @tab )
)
group by sp.Name
