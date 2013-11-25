
 create function F_Gen_MyOql_Fk(@tab varchar(300) , @fk varchar(300))
 returns varchar(300)
 as
 begin
	declare @sql varchar(300) ;
	select  @sql = col_name(Fk.parent_object_id, Fk_Cl.parent_column_id) + '(' +
	case when Fk.update_referential_action = 1 then 'u' else '' end  +
	case when Fk.delete_referential_action = 1 then 'd' else '' end + ')=' + 
	TbR.name  +':' + 
	col_name(Fk.referenced_object_id, Fk_Cl.referenced_column_id)   
	from sys.foreign_keys Fk 
	join sys.tables TbR on TbR.object_id = Fk.referenced_object_id 
	inner join sys.foreign_key_columns Fk_Cl on Fk_Cl.constraint_object_id = Fk.object_id 
	where	Fk.parent_object_id = object_id(@tab)  and col_name(Fk.parent_object_id, Fk_Cl.parent_column_id) = @fk
	order by Fk.object_id, Fk_Cl.constraint_column_id 

	return @sql ;
 end ;
 
 go

 CREATE function [dbo].[F_To_DbType](@sqlType varchar(30),@precision int , @scale int )
returns varchar(30)
as
begin
 --- 与MyOql 规则保持一致
	if ( @sqlType = 'varchar')  return 'AnsiString' ;
	else if ( @sqlType = 'nvarchar' ) return 'String' ;
	else if ( @sqlType = 'char' ) return 'AnsiStringFixedLength' ;
	else if ( @sqlType = 'nchar' ) return 'StringFixedLength' ;
	else if ( @sqlType = 'text' ) return 'String' ;
	else if ( @sqlType = 'ntext' ) return 'String' ;
	else if ( @sqlType = 'Xml' ) return 'Xml' ;
	else if ( @sqlType = 'image' ) return 'Binary' ;
	else if ( @sqlType = 'binary' ) return 'Binary' ;
	else if ( @sqlType = 'uniqueidentifier' ) return 'Guid' ;
	else if ( @sqlType = 'datetime' ) return 'DateTime' ;
	else if ( @sqlType = 'date' ) return 'Date' ;
	else if ( @sqlType = 'time' ) return 'DateTime' ;
	else if ( @sqlType = 'datetime2' ) return 'DateTime' ;
	else if ( @sqlType = 'smalldatetime' ) return 'DateTime' ;
	else if ( @sqlType = 'timestamp' ) return 'DateTime' ;
	else if ( @sqlType = 'datetimeoffset' ) return 'DateTime' ;
	else if ( @sqlType = 'int' ) return 'Int32' ;
	else if ( @sqlType = 'bigint' ) return 'Int64' ;
	else if ( @sqlType = 'smallint' ) return 'Int16' ;
	else if ( @sqlType = 'tinyint' ) return 'Int16' ;
	else if ( @sqlType = 'bit' ) return 'Boolean' ;
	else if ( @sqlType = 'real' ) return 'Single' ;
	else if ( @sqlType = 'money' ) return 'Decimal' ;
	else if ( @sqlType = 'smallmoney' ) return 'Decimal' ;
	else if ( @sqlType = 'float' ) return 'Single' ;
	else if ( @sqlType = 'decimal' ) return 'Decimal' ;
	else if ( @sqlType = 'numeric' ) begin
		if ( @scale = 0 ) begin
			if ( @precision < 3 ) return 'Byte';
			if ( @precision < 5 ) return 'Int16';
			if ( @precision < 10 ) return 'Int32' ;
			if ( @precision < 19 ) return 'Int64';
		end
		else begin
			if ( @precision <=7 ) return 'Single';
			if ( @precision <=15 ) return 'Double';
		end
		return 'Decimal' ;
	end
	return @sqlType ;
end ;
go




