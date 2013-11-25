using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;

namespace MyOql.MyOql_CodeGen.SqlScriptGen
{
    /*
     CREATE TABLE  "WorkProcess"(
  "ID" integer  NOT NULL,
  "WfID" varchar2(50) NOT NULL,
  "StepID" integer NOT NULL,
  "WfType" varchar2(50) NULL,
  "State" varchar2(50) NULL,
  "Sugestion" varchar2(100) NULL,
  "SenderUserID" varchar2(50) NULL,
  "RecvRole" varchar2(50) NULL,
  "SendTime"  date NULL
) ;

alter table "WorkProcess"
  add constraint "PK_WorkProcess" primary key (ID) ;
  
alter table TxtRes
  add constraint I_TxtRes unique (KEY);

-- Create sequence 
create sequence "SEQ_WorkProcess"
minvalue 1
maxvalue 999999999
start with 1
increment by 1
cache 20
cycle;

 
alter table "Person"
add constraint "FK_Person_Dept" foreign key ("DeptID")
references "Dept" ("ID") on delete cascade;

commit ;
     
     */
    public class MySql
    {
        public static string Do(Type[] types)
        {
            //DatabaseType dbType = DatabaseType.Oracle;
            List<string> listDropContraintSql = new List<string>();
            List<string> listDropTableSql = new List<string>();
            List<string> listEnts = new List<string>();
            List<string> listFks = new List<string>();

            //MyOqlCodeGenSect sect = (MyOqlCodeGenSect)ConfigurationManager.GetSection("MyOqlCodeGen");

            string strDropContraintSql = @"
【for:fks】
Alter Table `$TableName$` DROP FOREIGN KEY `FK_$TableName$_$myCol$`  ;
【endfor】
";
            string strDropTableSql = @"
Drop Table `$TableName$` ;
";
            string strEnts = @"
Create Table IF NOT Exists `$TableName$` (
【for:cols】
    【if】,【fi】`$col$` $coltype$ 
【endfor】
)ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

ALTER Table `$TableName$` Add Primary KEY (`$PK$`);
【if:autoinc】ALTER TABLE `$TableName$` CHANGE COLUMN `$incKey$` `$incKey$` Int(11) Not NULL AUTO_INCREMENT , ADD UNIQUE INDEX `UN_$TableName$_$incKey$` (`$incKey$` ASC);【fi】

";

            string strFksSql = @"
【for:fks】
ALTER Table `$TableName$` ADD CONSTRAINT `FK_$TableName$_$myCol$` foreign key  (`$myCol$`) References `$refTable$` (`$refCol$`)  ON DELETE CASCADE ON UPDATE RESTRICT;
【endfor】
";
            //var entList = MyOql.Helper.MyOqlCodeGen.GetTablesFromConfig().ToList();

            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(RuleBase)) == false) continue;
                var obj = Activator.CreateInstance(type) as RuleBase;

                var list = obj.GetColumns();
                var fkDefine = dbo.Event.Idbr.GetFkDefines();// type.GetCustomAttributes(typeof(MyOqlFKAttribute), false).Select(o => o as MyOqlFKAttribute);
                var typeMap = dbo.GetDbProvider(DatabaseType.MySql);

                listDropContraintSql.Add(strDropContraintSql
                                          .TmpFor("fks", fkDefine)
                                          .DoFor("myCol", o => o.Column)
                                          .EndFor()
                                          .Replace("$TableName$", obj.GetDbName()));

                listDropTableSql.Add(strDropTableSql.Replace("$TableName$", obj.GetDbName()));

                listEnts.Add(strEnts
                              .TmpFor("cols", list)
                              .DoFor("col", o => o.DbName)
                              .DoFor("coltype", a =>
                {
                    return typeMap.ToSqlType(a.DbType).ToString() + "(" + a.Length + ")";

                })

                              .DoIf(
                                      new CodeGen.DoIfObject<SimpleColumn>(string.Empty,
                                      name => { return list.IndexOf(o => o.Name == name.Name) > 0; })
                                    )

                              .EndFor()

                              .TmpIf()
                              .Var("autoinc", () => obj.GetAutoIncreKey().EqualsNull() == false)
                              .EndIf()
                              .Replace("$TableName$", obj.GetDbName())
                              .Replace("$PK$", string.Join(",", obj.GetPrimaryKeys().Select(o => o.DbName).ToArray()))
                              .Replace("$incKey$", obj.GetAutoIncreKey().EqualsNull() ? "" : obj.GetAutoIncreKey().DbName)
                              );


                listFks.Add(strFksSql
                             .TmpFor("fks", fkDefine)
                             .DoFor("refTable", o => o.RefTable)
                             .DoFor("myCol", o => o.Column)
                             .DoFor("refCol", o => o.RefColumn)
                             .EndFor()
                             .Replace("$TableName$", obj.GetDbName())
                             );

            }

            return string.Join("", listDropContraintSql.ToArray()) +
                    Environment.NewLine + string.Join("", listDropTableSql.ToArray()) +
                    Environment.NewLine + string.Join("", listEnts.ToArray()) +
                    Environment.NewLine + string.Join("", listFks.ToArray());
        }
    }
}



