using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Collections;

namespace MyOql.MyOql_CodeGen
{
    /// <summary>
    /// 代码生成.
    /// 存储过程中  Return 参数如果为空, 则返回影响行数.
    /// </summary>
    public static class MyOqlCodeGen_Static
    {
        static MyOqlCodeGen_Static()
        {
            EntityTemplate = MyOqlCodeGen.TidyTemplate(EntityTemplate);
            ProcTemplate = MyOqlCodeGen.TidyTemplate(ProcTemplate);
        }
        /// <summary>
        /// </summary>
        /// <remarks>
        /// 存储过程可返回:
        ///	    Dictionary&lt;T,V&gt;
        ///	    DataTable
        ///		实体：如  PersonRule.Entity。
        ///		实体数组： PersonRule.Entity[]。
        ///		值：须是 DbType 枚举类型，如 Int32 ，系统返回对应的 C#数据类型。
        ///		值数组： 须是DbType 枚举类型数组。如 Int32[] ，系统返回对应的数据类型的数组。
        /// </remarks>
        /// <example>
        /// <code>
        /// 配置节内容:
        /// <Entity Name="PLogin" Paras="UserID=varchar:in,PassWord=varchar:in" Return="result=Person:out"></Entity>
        /// </code>
        /// </example>
        /// 方向:in , out ,ret
        /// <param name="Proc"></param>
        /// <returns></returns>
        public static string ToPublicCode(this MyOqlCodeGenSect.ProcCollection.ProcGroupCollection.ProcElement ProcElement)
        {
            var Proc = ProcElement.Name;
            //var MapName = GetDbMapName(Proc);

            //【】返回类型: 列表.
            var provider = new Provider.SqlServer().GetTypeMap();
            var procOne = MyOqlCodeGen.sect.Proc.GetConfig(Proc);//.ToMyList().First(o => (o as MyOqlCodeGenSect.ProcCollection.ProcGroupCollection).Name == Proc);// GetProcOneFromConfig(Proc);

            DatabaseType dbType = dbo.GetDatabaseType(procOne.db);
            //var list = procOne.Paras.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

            var strProcTemp =
                ProcTemplate
                .TmpFor("paras", procOne.Paras)
                .DoIf((name) => procOne.Paras.IndexOf(o => o.Name == name.Name) > 0)
                .DoFor("paraDirection", o =>
                {
                    switch (o.Direction)
                    {
                        case ParameterDirection.Input:
                            return "";
                        case ParameterDirection.InputOutput:
                            return "ref ";
                        case ParameterDirection.Output:
                            return "ref ";
                        case ParameterDirection.ReturnValue:
                            return "ref ";
                        default:
                            break;
                    }
                    return "";
                })
                .DoFor("paraType", o =>
                {
                    return provider.First(t => t.DbType == o.DbType).CsType.FullName + (o.IsNullable ? "?" : "");
                })
                .DoFor("paraName", o => o.Name)
                .EndFor();


            var outPara = procOne.Paras.Where(o => o.Direction.IsIn(ParameterDirection.InputOutput, ParameterDirection.Output, ParameterDirection.ReturnValue)).ToArray();
            strProcTemp = strProcTemp.TmpFor("outpara", outPara)
                .DoIf(n => outPara.IndexOf(pn => pn.Name == n.Name) == 0)
                .DoFor("paraType", o =>
                {
                    if (procOne.MyParas.ContainsKey(o.Name)) return procOne.MyParas[o.Name].EnumType;
                    return provider.First(t => t.DbType == o.DbType).CsType.FullName + (o.IsNullable ? "?" : "");
                })
                .DoFor("paraName", o => o.Name)
                .EndFor();

            strProcTemp = strProcTemp.TmpFor("genParas", procOne.Paras)
                .DoFor("paraDbType", o =>
                {
                    if (procOne.MyParas.ContainsKey(o.Name))
                    {
                        return provider.FirstDbTypeByCsType(procOne.MyParas[o.Name].GetEnumType()).ToString();
                    }

                    return o.DbType.ToString();
                })
                .DoFor("paraName", o =>
                {
                    return o.Name;
                })
                .DoFor("paraNameValue", o =>
                {
                    return o.Name;
                })
                .DoFor("paraDirection", o =>
                {
                    return o.Direction.ToString();
                })
                .DoIf("OutVarParas", o =>
                {
                    return o.Direction != ParameterDirection.Input && o.DbType.IsIn(DbType.VarNumeric, DbType.String, DbType.AnsiString);
                })
                .EndFor();


            var returnSect = procOne.Return;

            strProcTemp = strProcTemp
               .TmpIf()
               .Var("IsArray", () => returnSect.ReturnIsArray())
               .Var("IsValue", () => returnSect.ReturnIsValue())
                //.Var("IsEntity", () => returnSect.ReturnIsEntity())
               .Var("IsDataSet", () => returnSect.ReturnIsDataSet())
               .Var("IsDataTable", () => returnSect.ReturnIsDataTable())
               .Var("IsDictionary", () => returnSect.ReturnIsDict())
               .Var("ReturnValue != 'void'", () => returnSect.ReturnIsVoid() == false)
               .Var("IsDictionary || IsModel", () => returnSect.ReturnIsDict() || returnSect.ReturnIsModel())
               .Var("ReturnValue == 'void'", () => returnSect.ReturnIsVoid())
               .Var("HasReturnModel", () => procOne.ReturnDefine.HasValue())
               .EndIf()
                ;

            strProcTemp = strProcTemp
                .Replace("$ProcReturn$", (returnSect.ReturnIsVoid() == false) && returnSect.ReturnIsValue() ? provider.First(o => o.DbType.ToString() == returnSect.GetReturnTypeWithoutArray()).CsType.FullName : returnSect.GetReturnTypeWithoutArray())
                .Replace("$ProcReturnDbType$", returnSect.ReturnIsValue() ? "DbType." + returnSect.GetReturnTypeWithoutArray() : returnSect.GetReturnTypeWithoutArray())
                .Replace("$returnPara$", returnSect.Name)
                .Replace("$ProcName$", procOne.Name)
                .Replace("$MapName$", dbo.TranslateDbName(procOne.MapName))
                .Replace("$ReturnModel$", procOne.ReturnDefine.HasValue() ? "<" + procOne.ReturnDefine + ">" : "")
             ;

            return strProcTemp;
        }


        public static string ToEntityCode(this MyOqlCodeGenSect.TableCollection.TableGroupCollection.BaseTableElement Config)
        {
            var Interface = string.Empty;
            {
                var type = Config.GetType();
                if (type == typeof(MyOqlCodeGenSect.TableCollection.TableGroupCollection.TableElement))
                {
                    Interface = "ITableRule";
                }
                else if (type == typeof(MyOqlCodeGenSect.ViewCollection.ViewGroupCollection.ViewElement))
                {
                    Interface = "IViewRule";
                }
                else if (type == typeof(MyOqlCodeGenSect.FunctionCollection.FunctionGroupCollection.FunctionElement))
                {
                    Interface = "IFunctionRule";
                }
            }

            DatabaseType dbType = MyOql.dbo.GetDatabaseType(Config.db);

            //string ReflectEnt = config.MapName.AsString(config.Name);// ReflectTable.HasValue() ? ReflectTable : Ent;
            var provider = MyOqlCodeGen.GetTypeMap(dbType);

            var EntMapName = Config.MapName;
            var dit = MyOqlCodeGen.FindColumns(Config, dbType, provider);


            GodError.Check(dit.Count == 0, "在数据库中找不到实体：" + Config.Name + " 的列信息，请检查.");

            var list = dit.Select(o => o.Name);

            var ems = Config.Enums;// MyOqlCodeGen.GetEnumKyesFromConfig(Ent);
            var descs = MyOqlCodeGen.GetColumnsDescriptionFromDb(dbType, Config.Name, Config.db, Config.Owner);
            var ent_desc = "";
            if (descs.ContainsKey("")) ent_desc = descs[""];

            //参考： MyEnumDefine.GetEnumType
            Func<string, Type> findCsType = (col) =>
            {
                var refEnt = ems;// GetEnumKyesFromConfig(ReflectEnt);
                if (refEnt.ContainsKey(col))
                {
                    return refEnt[col].GetEnumType();
                }

                var typ = provider.First(p => p.DbType == dit.First(d => d.Name == col).DbType).CsType;
                if (typ == typeof(DateTime))
                {
                    return typeof(MyDate);
                }
                else return typ;
            };



            Func<string, DbType> findDbType = (col) =>
            {
                return dit.First(d => d.Name == col).DbType;
            };

            //var fkOriDefine = config.FKs;// GetFKsFromConfig(ReflectEnt);
            var fkDefine = Config.FKs.TranslateMapName(MyOqlCodeGen.sect, Config.Name);

            var pks = Config.PKs.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);// GetPrimaryKeyFromConfig(ReflectEnt);

            var tabConfig = Config as MyOqlCodeGenSect.TableCollection.TableGroupCollection.TableElement;
            if (tabConfig != null && tabConfig.Paras.Count == 0)
            {
                GodError.Check(pks.Length == 0, "表 " + Config.Name + "不能没有主键!");
            }

            var uniqueIds = Config.UniqueKey.AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            var strEntTemp = EntityTemplate
                .TmpFor("Paras", tabConfig == null ? new MyParaDefine("") : tabConfig.Paras)
                .DoIf(o => tabConfig == null ? true : tabConfig.Paras.IndexOf(p => p.Name == o.Name) > 0)
                .DoFor("ParaType", o => provider.First(t => t.DbType == o.DbType).CsType.FullName)
                .DoFor("Para", o => o.Name)
                .EndFor()

                .TmpFor("propertys", list)
                .DoIf(new CodeGen.DoIfObject<string>("isFK", col =>
                {
                    return fkDefine.Any(o => o.Column == col);
                })
                )
                .DoIf("ColDescHasValue", col =>
                {
                    var col_Desc = descs.FirstOrDefault(o => o.Key == col);
                    if (col_Desc.Value != null) return col_Desc.Value.HasValue();
                    else return false;
                })
                .DoFor("RefTable", col =>
                {
                    var fkSect = fkDefine.FirstOrDefault(o => o.Column == col);
                    if (fkSect == null) throw new GodError("找不到外键：" + col); //return string.Empty;
                    else return dbo.TranslateDbName(fkSect.RefTable);
                })
                .DoFor("Col_Desc", col =>
                {
                    var col_Desc = descs.FirstOrDefault(o => o.Key == col);
                    if (col_Desc.Value != null) return col_Desc.Value;
                    else return "";
                })
                .DoFor("RefColumn", col =>
                {
                    var fkSect = fkDefine.FirstOrDefault(o => o.MapColumn == col);
                    if (fkSect == null) throw new GodError("找不到外键：" + col); //return string.Empty;
                    else return dbo.TranslateDbName(fkSect.RefColumn);
                })
                .DoFor("Property", col => dbo.TranslateDbName(ems.ContainsKey(col) ? ems[col].TranslateName : col))
                .DoFor("ColumnName", col => col)
                .DoFor("Type", o =>
                        ems.ContainsKey(o) && ems[o].EnumType.HasValue() ? ems[o].EnumType : findCsType(o).Name
                    )
                .DoFor("DbType", o =>
                      dit.First(d => d.Name == o).DbType.ToString()
                    )
                .DoFor("DataLength", o =>
                      dit.First(d => d.Name == o).Length.ToString()
                    )
                .DoFor("Triat", o =>
                {
                    var sl = new List<string>();
                    if (pks.Contains(o)) sl.Add("主键(" + string.Join(",", pks) + ")");
                    if (Config.AutoIncreKey == o) sl.Add("自增键");
                    if (uniqueIds.Contains(o)) sl.Add("唯一键(" + string.Join(",", uniqueIds) + ")");
                    if (Config.ComputeKeys.Split(",").Contains(o)) sl.Add("计算列");
                    if (fkDefine.Contains(f => f.Column == o)) sl.Add("外键(" + fkDefine.First(f => f.Column == o).ToString() + ")");
                    if (sl.Count == 0) return string.Empty;
                    return "[" + string.Join(",", sl.ToArray()) + "]";
                }
                )

                .EndFor();



            strEntTemp = strEntTemp.TmpFor("fkp", fkDefine)
                .DoIf("FKIsNullble", o =>
                {
                    //var oriOne = fkOriDefine.First(f => f.Name == o.Name);
                    var fkType = ems.ContainsKey(o.Column) && ems[o.Column].EnumType.HasValue() ? ems[o.Column].EnumType : findCsType(o.Column).Name;
                    return fkType.Contains("?");
                })
                .DoFor("FKName", o =>
                {
                    var fkType = ems.ContainsKey(o.MapColumn) && ems[o.MapColumn].EnumType.HasValue() ? ems[o.MapColumn].EnumType : findCsType(o.Column).Name;

                    return dbo.TranslateDbName(o.Column);
                })
               .DoFor("FK", o =>
               {
                   var fkType = ems.ContainsKey(o.MapColumn) && ems[o.MapColumn].EnumType.HasValue() ? ems[o.MapColumn].EnumType : findCsType(o.Column).Name;
                   if (fkType.Contains("?"))
                   {
                       return dbo.TranslateDbName(o.MapColumn) + ".Value";
                   }
                   else return dbo.TranslateDbName(o.MapColumn);
               })
                .DoFor("FullGroup", o =>
                {
                    var group = MyOqlCodeGen.sect.GetGroup(o.MapRefTable).Name;
                    if (group.HasValue()) return group + ".";
                    else return "";
                })
                .DoFor("RefGroup", o =>
                {
                    var group = MyOqlCodeGen.sect.GetGroup(o.MapRefTable).Name;
                    if (group.HasValue()) return group + "Group.";
                    else return "";
                })
                .DoFor("GroupEnt", o =>
                {
                    var group = MyOqlCodeGen.sect.GetGroup(o.MapRefTable).Name;
                    if (group.HasValue()) return group + "Group.";
                    else return "";
                })
                .DoFor("FKMethod", o =>
                {
                    if (fkDefine.Count(f => f.RefTable == o.MapRefTable) > 1)
                    {
                        return "By" + dbo.TranslateDbName(o.MapColumn);
                    }
                    else return "";
                })
                .DoFor("RefRule", o => dbo.TranslateDbName(o.MapRefTable)) // fkDefine.First(f => f.RefColumn == o).RefTable))
                .DoFor("RefRulePK", o => dbo.TranslateDbName(o.MapRefColumn)) //fkDefine.First(f => f.RefColumn == o).RefColumn))
                .EndFor();

            var fkcs = Config.GetChildrenDefine(MyOqlCodeGen.sect.Table);// GetFk_ChildrnEntitysFromConfig(ReflectEnt);

            strEntTemp = strEntTemp.TmpFor("fkcs", fkcs)
                .DoFor("FullGroup", o =>
                {
                    var group = MyOqlCodeGen.sect.GetGroup(o.Entity).Name;
                    if (group.HasValue()) return group + ".";
                    else return "";
                })
                .DoFor("GroupEnt", o =>
                {
                    var group = MyOqlCodeGen.sect.GetGroup(o.Entity).Name;
                    if (group.HasValue()) return group + "Group.";
                    else return "";
                })
                .DoFor("ByChildRuleFk", o =>
                {
                    if (fkcs.Count(f => f.Entity == o.Entity) > 1)
                    {
                        return "By" + dbo.TranslateDbName(o.MapColumn);
                    }
                    else
                    {
                        return "";
                    }
                })
                .DoFor("ChildRule", o => dbo.TranslateDbName(o.MapEntity))
                .DoFor("ChildRuleFk", o => dbo.TranslateDbName(o.MapColumn))
                .DoFor("MasterRuleRefKey", o => dbo.TranslateDbName(o.MapRefColumn))

                .DoFor("ChildRule_DbName", o => o.MapEntity)
                .DoFor("ChildRuleFk_DbName", o => o.MapColumn)
                .DoFor("MasterRuleRefKey_DbName", o => o.MapRefColumn)

                .DoFor("CascadeUpdate", o => o.CascadeUpdate ? "true" : "false")
                .DoFor("CascadeDelete", o => o.CascadeDelete ? "true" : "false")
                .EndFor();

            strEntTemp = strEntTemp.TmpFor("pks", pks)
                .DoIf(o => pks[0] != o)
                .DoFor("pktype", o =>
                        (ems.ContainsKey(o) && ems[o].EnumType.HasValue() ? ems[o].EnumType : findCsType(o).Name)
                    )
                    .DoFor("pk", o => ems.ContainsKey(o) ? ems[o].TranslateName : dbo.TranslateDbName(o))
                .EndFor();

            strEntTemp = strEntTemp.TmpFor("uniqueKeys", uniqueIds)
                .DoIf(o => uniqueIds[0] != o)
                .DoFor("uniqueKeyType", o =>
                        (ems.ContainsKey(o) && ems[o].EnumType.HasValue() ? ems[o].EnumType : findCsType(o).Name)
                    )
                    .DoFor("uniqueKey", o => ems.ContainsKey(o) ? ems[o].TranslateName : dbo.TranslateDbName(o))
                .EndFor();

            strEntTemp = strEntTemp.TmpIf()
             .Var("HasPara", () => tabConfig != null && tabConfig.Paras != null && tabConfig.Paras.Count > 0)

             .Var("pk1", delegate() { return pks.Length == 1; })
             .Var("isAutoId", delegate() { return Config.AutoIncreKey.HasValue(); })
             .Var("autoId", () =>
             {
                 if (Config.AutoIncreKey.HasValue() == false) return false;
                 else if (pks.Length == 1 && (Config.AutoIncreKey == pks[0])) return false;
                 else return true;
             })
             .Var("uniqueId1", () =>
             {
                 if (uniqueIds.Length != 1) return false;

                 if (pks.Length == 1 && uniqueIds[0] == pks[0]) return false;
                 else if (Config.AutoIncreKey == uniqueIds[0]) return false;
                 else return true;
             })
             .Var("pk2", delegate() { return pks.Length == 2; })
             .Var("pk3", delegate() { return pks.Length == 3; })
             .Var("pk1type==Number", () =>
             {
                 if (pks.Length == 0) return false;
                 if (dit.Count(d => d.Name == pks[0]) == 0) return false;
                 return findDbType(pks[0]).DbTypeIsNumber();
             })
              .Var("autoId==Number", () =>
              {
                  if (Config.AutoIncreKey.HasValue() == false) return false;
                  return findDbType(Config.AutoIncreKey).DbTypeIsNumber();
              })
             .Var("uniqueId1==Number", () =>
             {
                 if (uniqueIds.Length != 1) return false;
                 return findDbType(uniqueIds[0]).DbTypeIsNumber();

             })
             .Var("IsTableRule", () =>
             {
                 return Interface == "ITableRule";
             })
             .Var("pkn", () => pks.Length > 1)
             .Var("uniqueIdn", () =>
             {
                 if (uniqueIds.Length < 2) return false;
                 if (pks.OrderBy(o => o).ToArray() == uniqueIds.OrderBy(o => o).ToArray()) return false;
                 return true;
             })
             .EndIf();

            strEntTemp = strEntTemp
                .Replace("$Entity$", dbo.TranslateDbName(EntMapName))
                .Replace("$Interface$", Interface)
                .Replace("$Ent_Desc$", ent_desc)
                .Replace("$TableDbName$", Config.Name)
                .Replace("$Columns$", string.Join(",", list.Select(col => dbo.TranslateDbName(ems.ContainsKey(col) ? ems[col].TranslateName : col)).ToArray()))
                .Replace("$Propertys$", string.Join(",", list.Select(col => @"""" + dbo.TranslateDbName(ems.ContainsKey(col) ? ems[col].TranslateName : col) + @"""").ToArray()))
                .Replace("$PrimaryKeys$", string.Join(",", pks.Select(col => dbo.TranslateDbName(ems.ContainsKey(col) ? ems[col].TranslateName : col)).ToArray()))
                .Replace("$ComputeKeys$", string.Join(",", Config.ComputeKeys.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries) /*GetComputeKyesFromConfig(ReflectEnt)*/.Select(col => dbo.TranslateDbName(ems.ContainsKey(col) ? ems[col].TranslateName : col)).ToArray()))
                .Replace("$AutoIncreKey$", Config.AutoIncreKey.HasValue() ? dbo.TranslateDbName(ems.ContainsKey(Config.AutoIncreKey) ? ems[Config.AutoIncreKey].TranslateName : Config.AutoIncreKey) : "null")
                .Replace("$pk1$", pks.Length != 1 ? "" : dbo.TranslateDbName(ems.ContainsKey(pks[0]) ? ems[pks[0]].TranslateName : pks[0]))
                //.Replace("$autoId$", dbo.TranslateDbName(GetAutoIncreteKeysFromConfig(ReflectEnt)))
                .Replace("$uniqueId1$", dbo.TranslateDbName(ems.ContainsKey(Config.UniqueKey) ? ems[Config.UniqueKey].TranslateName : Config.UniqueKey))
                //.Replace("$pk2$", pks.Length > 1 ? dbo.TranslateDbName(ems.ContainsKey(pks[1]) ? ems[pks[1]].TranslateName : pks[1]) : "")
                //.Replace("$pk3$", pks.Length > 2 ? dbo.TranslateDbName(ems.ContainsKey(pks[2]) ? ems[pks[2]].TranslateName : pks[2]) : "")
                .Replace("$pk1type$",
                    pks.Length != 1 ? "" :
                   ems.ContainsKey(pks[0]) && ems[pks[0]].EnumType.HasValue() ? ems[pks[0]].EnumType : findCsType(pks[0]).Name
                         )
                .Replace("$autoIdType$", Config.AutoIncreKey.HasValue() ?

                (ems.ContainsKey(Config.AutoIncreKey) && ems[Config.AutoIncreKey].EnumType.HasValue() ? ems[Config.AutoIncreKey].EnumType : findCsType(Config.AutoIncreKey).Name)
                //provider.First(p => p.DbType == dit.First(d => d.Name == GetAutoIncreteKeys(ReflectEnt)).DbType).CsType.Name
                         : "")
                .Replace("$uniqueId1Type$", Config.UniqueKey.HasValue() ?
                   (ems.ContainsKey(Config.UniqueKey) ? ems[Config.UniqueKey].EnumType : findCsType(Config.UniqueKey).Name)
                //provider.First(p => p.DbType == dit.First(d => d.Name == GetUniqueKey(ReflectEnt)).DbType).CsType.Name
                         : "")
                //.Replace("$pk2type$", pks.Length > 1 ?
                // (ems.ContainsKey(pks[1]) && ems[pks[1]].EnumType.HasValue() ? ems[pks[1]].EnumType : findCsType(pks[1]).Name)
                //: "")
                //.Replace("$pk3type$", pks.Length > 2 ?
                //    (ems.ContainsKey(pks[2]) && ems[pks[2]].EnumType.HasValue() ? ems[pks[2]].EnumType : findCsType(pks[2]).Name)
                //            : "")
                .Replace("$TableRuleGroup$", new Func<string>(() =>
                {
                    var group = MyOqlCodeGen.sect.GetGroup(Config.Name).Name;
                    if (group.HasValue()) return group + ".";
                    else return "";
                }
                    )())
                .Replace("$TableRule$", dbo.TranslateDbName(Config.MapName.AsString(Config.Name)))
                .Replace("$UniqueKey$", Config.UniqueKey.HasValue() ? dbo.TranslateDbName(Config.UniqueKey) : "null")
                //.Replace("$RowTimestamp$", Config.RowTimestamp.HasValue() ? dbo.TranslateDbName(Config.RowTimestamp) : "null")
                ;

            return strEntTemp;
        }
        #region 实体模板

        static string EntityTemplate = @"
    /// <summary>
    /// $Ent_Desc$
    /// </summary>
    [Serializable]
    public sealed partial class $Entity$Rule : RuleBase, $Interface$,ICloneable
    {
【if:HasPara】
    【for:Paras】
            protected $ParaType$ _$Para$ ;
    【endfor】

            public string[] GetParameters() { return new string[] {
【for:Paras】【if】,【fi】 ""$Para$"" 
【endfor】}; }

            public object GetParameterValue(string Parameter)
            {
            【for:Paras】
                  if (Parameter == ""$Para$"")
                    return this._$Para$;
            【endfor】
                return null;
            }

            public $Entity$Rule(【for:Paras】【if】,【fi】$ParaType$ $Para$ 【endfor】) 
                : this ()
            {
            【for:Paras】
                    this._$Para$ = $Para$ ;
            【endfor】
                
                //函数表必须有别名。
                this.SetAlias(this.GetName());
            }
【fi】

        /// <summary>
        /// $Ent_Desc$ 
        /// </summary>
        public Entity _ { get { return new Entity (); } }
       
        /// <summary>
        /// $Ent_Desc$
        /// </summary>
        [Serializable]
        public sealed partial class Entity :IEntity
        {
【for:propertys】
 
            /// <summary>
            /// 【if:ColDescHasValue】$Col_Desc$【else】$ColumnName$【fi】$Triat$
            /// </summary>
            public $Type$ $Property$ { get; set; }
【endfor】

            public bool SetPropertyValue(string PropertyName, object Value)
            {
【for:propertys】
                if ( PropertyName == ""$Property$"" ) { this.$Property$ = ValueProc.As<$Type$>(Value) ; return true; }
【endfor】
                return false ;
           }

            public object GetPropertyValue(string PropertyName)
            {
【for:propertys】
                if ( PropertyName == ""$Property$"" ) { return this.$Property$ ; }
【endfor】
                return null ;
            }

            public string[]  GetProperties() { return new string[]{ $Propertys$ } ; }
            
            public object Clone()
            {
                return this.CloneIEntity() ;
            }

【for:fkp】
            private $RefRule$Rule.Entity _Get$RefRule$$FKMethod$ = null;
            public $RefRule$Rule.Entity Get$RefRule$$FKMethod$()
            {
                if (_Get$RefRule$$FKMethod$ != null) return _Get$RefRule$$FKMethod$;
                【if:FKIsNullble】
                if (this.$FKName$.HasValue == false) return null;
                【fi】
                _Get$RefRule$$FKMethod$ = dbr.$FullGroup$$RefRule$.FindBy$RefRulePK$(this.$FK$);
                return _Get$RefRule$$FKMethod$;
            }
【endfor】
【for:fkcs】
            private $ChildRule$Rule.Entity[] _Get$ChildRule$s$ByChildRuleFk$ = null;
            public $ChildRule$Rule.Entity[] Get$ChildRule$s$ByChildRuleFk$()
            {
                if (_Get$ChildRule$s$ByChildRuleFk$ != null) return _Get$ChildRule$s$ByChildRuleFk$;
                _Get$ChildRule$s$ByChildRuleFk$ = dbr.$FullGroup$$ChildRule$.SelectWhere(o => o.$ChildRuleFk$ == this.$MasterRuleRefKey$).ToEntityList<$ChildRule$Rule.Entity>().ToArray();
                return _Get$ChildRule$s$ByChildRuleFk$;
            }
【endfor】
         }

【if:HasPara】
        private 
【else】
        public 
【fi】 $Entity$Rule() : base(""$Entity$"")
        {
【for:propertys】
            this.$Property$ = new SimpleColumn(this, DbType.$DbType$, $DataLength$,""$Property$"",""$ColumnName$"");
【endfor】
        }

【for:propertys】
        /// <summary>
        /// 【if:ColDescHasValue】$Col_Desc$【else】$ColumnName$【fi】($DbType$)$Triat$
        /// </summary>
        public SimpleColumn $Property$ { get; set; }
【endfor】

        public override SimpleColumn[] GetColumns() {  return new SimpleColumn[] { $Columns$ }; }
        public override SimpleColumn[] GetPrimaryKeys() { return new SimpleColumn[] { $PrimaryKeys$ };  }
        public override SimpleColumn[] GetComputeKeys() { return new SimpleColumn[] { $ComputeKeys$ }; }
        public override SimpleColumn GetAutoIncreKey() {  return $AutoIncreKey$; }
        public override SimpleColumn GetUniqueKey() { return  $UniqueKey$; }
        public override string GetDbName() { return ""$TableDbName$""; }

【if:pk1】
        public Entity FindBy$pk1$($pk1type$ $pk1$)
        {
            【if:pk1type==Number】
             if ( $pk1$ <= 0 ) return null ;
            【else】
            if ( $pk1$.HasValue() == false ) return null ;
            【fi】
            return this.SelectWhere(o => o.$pk1$ == $pk1$).ToEntity<Entity>();
        }
【if:IsTableRule】
        public int DeleteBy$pk1$($pk1type$ $pk1$)
        {
            【if:pk1type==Number】
             if ( $pk1$ <= 0 ) return 0 ;
            【else】
            if ( $pk1$.HasValue() == false ) return 0 ;
            【fi】
            return this.Delete(o => o.$pk1$ == $pk1$).Execute() ;
        }
【fi】
【fi】
【if:autoId】
        public Entity FindBy$AutoIncreKey$($autoIdType$ $AutoIncreKey$)
        {
            【if:autoId==Number】
             if ( $AutoIncreKey$ <= 0 ) return null ;
            【else】
             if ( $AutoIncreKey$.HasValue() == false ) return null ;
            【fi】
            return this.SelectWhere(o => o.$AutoIncreKey$ == $AutoIncreKey$).ToEntity<Entity>();
        }
【if:IsTableRule】
        public int DeleteBy$AutoIncreKey$($autoIdType$ $AutoIncreKey$)
        {
            【if:autoId==Number】
             if ( $AutoIncreKey$ <= 0 ) return 0 ;
            【else】
             if ( $AutoIncreKey$.HasValue() == false ) return 0 ;
            【fi】
            return this.Delete(o => o.$AutoIncreKey$ == $AutoIncreKey$).Execute();
        }
【fi】
【fi】

【if:uniqueId1】
        public Entity FindBy$uniqueId1$($uniqueId1Type$ $uniqueId1$)
        {
            【if:uniqueId1==Number】
             if ( $uniqueId1$ <= 0 ) return null ;
            【else】
             if ( $uniqueId1$.HasValue() == false ) return null ;
            【fi】
            return this.SelectWhere(o => o.$uniqueId1$ == $uniqueId1$).ToEntity<Entity>();
        }
【if:IsTableRule】
        public int DeleteBy$uniqueId1$($uniqueId1Type$ $uniqueId1$)
        {
            【if:uniqueId1==Number】
             if ( $uniqueId1$ <= 0 ) return 0 ;
            【else】
             if ( $uniqueId1$.HasValue() == false ) return 0 ;
            【fi】
            return this.Delete(o => o.$uniqueId1$ == $uniqueId1$).Execute();
        }
【fi】
【fi】
【if:pkn】
        public Entity FindByPks(【for:pks】【if】,【fi】$pktype$ $pk$【endfor】)
        {
            return this.SelectWhere(o => 【for:pks】【if】&【fi】o.$pk$ == $pk$【endfor】).ToEntity<Entity>();
        }
【if:IsTableRule】
        public int DeleteByPks(【for:pks】【if】,【fi】$pktype$ $pk$【endfor】)
        {
            return  this.Delete(o => 【for:pks】【if】&【fi】o.$pk$ == $pk$【endfor】).Execute();
        }
【fi】
【fi】
【if:uniqueIdn】
        public Entity FindByUniqueKeys(【for:uniqueKeys】【if】,【fi】$uniqueKeyType$ $uniqueKey$【endfor】)
        {
            return  this.SelectWhere(o =>【for:uniqueKeys】【if】&【fi】o.$uniqueKey$ == $uniqueKey$【endfor】).ToEntity<Entity>();
        }
【if:IsTableRule】
        public int DeleteByUniqueKeys(【for:uniqueKeys】【if】,【fi】$uniqueKeyType$ $uniqueKey$【endfor】)
        {
            return  this.Delete(o =>【for:uniqueKeys】【if】&【fi】o.$uniqueKey$ == $uniqueKey$【endfor】).Execute();
        }
【fi】
【fi】

        public override object Clone()
        {
            var tab = new $Entity$Rule();
            if ( this._Config_ != null ) tab._Config_ = base._Config_.Clone() as RuleRuntimeConfig ;
            tab.SetAlias(base.Name);
            tab.SetReconfig(base.ReConfig);

【for:propertys】
            tab.$Property$ = this.$Property$.Clone() as SimpleColumn;
【endfor】
【for:Paras】
            tab._$Para$ = this._$Para$ ;
【endfor】

            return tab;
        }
    }
";

        #endregion


        #region 存储过程模板
        static string ProcTemplate = @"
【if:IsArray】
    public static 
    【if:ReturnValue == 'void'】int
    【eif:IsArray】List<$ProcReturn$>
    【else】$ProcReturn$
    【fi】 $MapName$(
            【for:paras】【if】,【fi】$paraDirection$ $paraType$ $paraName$ 【endfor】)
    {
        List<$ProcReturn$> retVal = new List<$ProcReturn$>();
          
            【if:IsDictionary || IsModel】
                $MapName$(【for:paras】【if】,【fi】$paraDirection$ $paraName$ 【endfor】,(reader,index)=>
                {
                    retVal.Add(dbo.ToEntity(reader,()=> new $ProcReturn$ () ));
                    return true;
                });
            【eif:IsValue】
                $MapName$(【for:paras】【if】,【fi】$paraDirection$ ,$paraName$ 【endfor】,(reader,index)=>
                {
                    retVal.Add(dbo.ToEntity(reader,$ProcReturn$ ));
                    return true;
                });
            【else】 在List 配置了非法的返回类型.
            【fi】

        return retVal ;
    }
【fi】


    
    public static 
    【if:ReturnValue == 'void'】int
    【eif:IsArray】void
    【else】$ProcReturn$
    【fi】 $MapName$(
        【for:paras】【if】,【fi】$paraDirection$ $paraType$ $paraName$ 【endfor】
    【if:IsArray】, Func<DbDataReader,int,bool> ReaderFunc
    【fi】)
    { 
    【for:outpara】  
        //$paraName$ = default($paraType$) ;
    【endfor】
        var proc = $MapName$Clip(【for:paras】【if】,【fi】 $paraName$ 【endfor】) ;
    【if:IsDataSet】
        DataSet retVal = proc.ToDataSet();
    【eif:IsDataTable】
        DataTable retVal = proc.ToDataTable();
    【eif:IsArray】
        int index = 0 ;
        proc.ExecuteReader((reader) =>
        {
            return ReaderFunc(reader,index++) ;
        });
    【eif:IsDictionary || IsModel】
        $ProcReturn$ retVal = default(  $ProcReturn$ ) ;
        proc.ExecuteReader(o =>
        {
            retVal =dbo.ToEntity(o,retVal);
            return false;
        });
    【eif:IsValue】
        $ProcReturn$ retVal = default(  $ProcReturn$ );
        proc.ExecuteReader(o =>
        {
            retVal = ValueProc.As<$ProcReturn$>(o.GetValue(0)) ; 
            return false;
        });
    【eif:ReturnValue == 'void'】
        int retVal = proc.Execute() ;
    【else】
配置了非法的返回类型.
    【fi】
    【for:outpara】  
    【if】
    ProcParameterClip paras = proc.Dna.FirstOrDefault(o => o.Key == SqlKeyword.ProcParameter) as ProcParameterClip;【fi】
    
    $paraName$ = ValueProc.As<$paraType$>(paras.Parameters.First(o => o.ParameterName == ""$paraName$"").Value) ;
    【endfor】
    【if:IsArray】
        return ;
    【else】
        return retVal;
    【fi】
    }
";
        #endregion

    }
}
