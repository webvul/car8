--请先切换数据库

 
--当前库的表及 Owner
declare @tab table( sc varchar(10) , Name varchar(200) )

--当前库表的列
declare @cols table( tab_Name varchar(200) , col_Name varchar(200) , order_id int , inPk int , is_AutoKeys int , is_computeKey int ,is_fk int ,is_uniqueKey int )

--当前库中每个表最短的唯一列
declare @uk table( tab_id  bigint , uk_id bigint, uk_name varchar(200) ) 

DECLARE @is_policy_automation_enabled bit
SET @is_policy_automation_enabled  = (SELECT CONVERT(bit, current_value)
      FROM msdb.dbo.syspolicy_configuration
      WHERE name = 'Enabled')
      

insert into @tab ( Name , sc )
SELECT
tbl.name AS [Name],
SCHEMA_NAME(tbl.schema_id) AS [Schema] 
 
FROM
master.sys.databases AS dtb,
sys.tables AS tbl
WHERE
(CAST(
 case 
    when tbl.is_ms_shipped = 1 then 1
    when (
        select 
            major_id 
        from 
            sys.extended_properties 
        where 
            major_id = tbl.object_id and 
            minor_id = 0 and 
            class = 1 and 
            name = N'microsoft_database_tools_support') 
        is not null then 1
    else 0
end          
             AS bit)='0')and (dtb.name=db_name() ) ;

---

with uk as (
select  ies.object_id as tab_id ,IES.index_id as uk_id, ies.name as uk_name,  
 COUNT(1) as cols_count
from sys.indexes as  ies
left join sys. index_columns as ic  on ( ies.object_id = ic.object_id and  ic.index_id = ies.index_id)
where ies.is_unique = 1 and ies.is_primary_key = 0
group by ies.object_id ,ies.index_id, ies.name
)

insert into @uk
select uk.tab_id , uk.uk_id, uk.uk_name
from uk 
join (
select tab_id   , MIN(cols_count) as min_cols_count
from uk
group by tab_id 
)
as s on ( uk.tab_id = s.tab_id and uk.cols_count = s.min_cols_count )


--


insert into @cols (tab_Name , col_Name  , order_id , inPk , is_AutoKeys, is_computeKey ,is_fk ,is_uniqueKey )
SELECT
tbl.name as Tab_Name,
clmns.name AS [Col_Name],
clmns.column_id as Order_id,
CAST(ISNULL(cik.index_column_id, 0) AS bit) as InPk,
clmns.is_identity as AutoIncreKey ,
clmns.is_computed as ComputeKeys,

CAST(ISNULL((select TOP 1 1 from sys.foreign_key_columns AS colfk where colfk.parent_column_id = clmns.column_id and colfk.parent_object_id = clmns.object_id), 0) AS bit) AS [IsForeignKey],


cast(isnull( (
select top 1 1
from  @uk  as ies
join sys.index_columns as ic on ( ies.tab_id = ic.object_id and ic.index_id = ies.uk_id)
where ies.tab_id = clmns.object_id and ic.column_id = clmns.column_id
),0) as bit ) as is_unique  
FROM
sys.tables AS tbl
INNER JOIN sys.all_columns AS clmns ON clmns.object_id=tbl.object_id
LEFT OUTER JOIN sys.indexes AS ik ON ik.object_id = clmns.object_id and 1=ik.is_primary_key
LEFT OUTER JOIN sys.index_columns AS cik ON cik.index_id = ik.index_id and cik.column_id = clmns.column_id and cik.object_id = clmns.object_id and 0 = cik.is_included_column
LEFT OUTER JOIN sys.types AS usrt ON usrt.user_type_id = clmns.user_type_id
LEFT OUTER JOIN sys.types AS baset ON (baset.user_type_id = clmns.system_type_id and baset.user_type_id = baset.system_type_id) or ((baset.system_type_id = clmns.system_type_id) and (baset.user_type_id = clmns.user_type_id) and (baset.is_user_defined = 0) and (baset.is_assembly_type = 1)) 
LEFT OUTER JOIN sys.xml_schema_collections AS xscclmns ON xscclmns.xml_collection_id = clmns.xml_collection_id
LEFT OUTER JOIN sys.schemas AS s2clmns ON s2clmns.schema_id = xscclmns.schema_id
WHERE
(
tbl.name +'.'+ SCHEMA_NAME(tbl.schema_id) in ( select Name +'.' + sc from @tab )
)

order by tbl.name asc ,clmns.column_id asc 

 
select tab_Name , 
'<Entity Name="' + tab_Name +
'" PKs="' + dbo.JoinStr( case when inPk = 1 then col_Name else null end  ,',') + 
'" AutoIncreKey="' + dbo.JoinStr( case when is_AutoKeys = 1 then col_Name else null end ,',') + 
'" ComputeKeys="' + dbo.JoinStr( case when is_computeKey = 1 then col_Name else null end ,',') + 
'" UniqueKey="' + dbo.JoinStr( case when is_uniqueKey = 1 then col_Name else null end ,',') + 
'" FKs="' + dbo.JoinStr( case when is_fk = 1 then dbo.F_Gen_MyOql_Fk(tab_Name, col_Name ) else null end ,',') + 
'"' + ' />'  
from @cols
where tab_Name like 'TB_%' or tab_Name in ('Annex','Dict','ResKey','ResValue','PowerController','PowerAction','PowerButton','Log','Menu','CacheTable','T_STANDARD_ROLE','TEMP_ROOMOWNER','T_ROOM_OWNER','TM_Community')
group by tab_Name


 