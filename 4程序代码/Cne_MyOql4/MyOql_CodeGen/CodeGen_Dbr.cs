using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using System.Configuration;
using System.Reflection;
using System.Collections;

namespace MyOql.MyOql_CodeGen
{
    public partial class MyOqlCodeGen
    {
        private string GenDbr()
        {
            string strEnt = @"
///<summary>
///Database rule , 实体，视图，存储过程，函数的入口。
///</summary>
public partial class dbr:IDbr
{
    static dbr()
    {
【for:RootGroups】
        dbr.$Group$ = new $Group$GroupClass(); 
【endfor】

        //MyOql 实体信息
        MyOql_FkDefinesList = new List<FkColumn>() ;
【for:fks】
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = ""$TableDbName$"" , Column = ""$Property_DbName$"" , CascadeUpdate = $CascadeUpdate$ , CascadeDelete = $CascadeDelete$ , RefTable = ""$RefTable_DbName$"", RefColumn = ""$RefColumn_DbName$"" } );
【endfor】

        MyOql_ViewRelationTablesDict = new Dictionary<string, string[]>();
【for:AllRelationView】
        MyOql_ViewRelationTablesDict.Add( ""$DbName$"" , new string[] { $RelationTables$ } ) ;
【endfor】

        MyOql_ProcRelationTablesDict = new Dictionary<string, string[]>();
【for:AllRelationProc】
        MyOql_ProcRelationTablesDict.Add( ""$DbName$"" , new string[] { $RelationTables$ } ) ;
【endfor】

        MyOql_EntityDict = new Dictionary<string, RuleBase>();
【for:AllGroups】
    【for:entGroup】
        【if:HasVar】
        MyOql_VarTableDict.Add(""$DbEntVarName$"" , typeof($DbEnt$Rule) ); 
        【else】
        MyOql_EntityDict.Add(""$DbEnt$"" , dbr$FullGroup$.$Ent$ ); 
        【fi】
    【endfor】
【endfor】

    }
【for:RootGroups】
    public static $Group$GroupClass $Group$ { get ; private set ; }
【endfor】

【for:Groups】
    【for:SysEnts】
        【if:IsFunction】
    public static $Ent$Rule $Ent$(【for:Paras】【if:NotFirst】,【fi】$ParaType$ $Para$【endfor】) 
    {
        return new $Ent$Rule(【for:Paras】【if:NotFirst】,【fi】$Para$ 【endfor】) ;
    }
        【eif:IsVarTable】
    public static $Ent$Rule $Ent$(【for:Vars】【if:NotFirst】,【fi】string $Var$【endfor】) 
    {
        return new $Ent$Rule(【for:Vars】【if:NotFirst】,【fi】$Var$ 【endfor】) ;
    }

        【else】
    public static $Ent$Rule $Ent$ { get { return new $Ent$Rule() ; } }
        【fi】
    【endfor】
【endfor】

    //缓存 MyOql 实体信息
    public static Dictionary<string, RuleBase> MyOql_EntityDict { get; private set; }
    public static Dictionary<string, Type> MyOql_VarTableDict { get; private set; }
    public static Dictionary<string, string[]> MyOql_ViewRelationTablesDict { get; private set; }
    public static Dictionary<string, string[]> MyOql_ProcRelationTablesDict { get; private set; }
        
    /// <summary>
    /// 所有表的 外键定义信息
    /// </summary>
    /// <returns></returns>
    public static List<FkColumn> MyOql_FkDefinesList { get; private set; }

    /// <summary>
    /// 获取所有表的 外键定义信息
    /// </summary>
    /// <returns></returns>
    public List<FkColumn> GetFkDefines()
    {
        return MyOql_FkDefinesList;
    }

    /// <summary>
    /// 取 视图 关联的表
    /// </summary>
    /// <returns>Key 是视图的数据库名，Value 是关联表的数据库名</returns>
    public Dictionary<string, string[]> GetViewRelationTables()
    {
        return MyOql_ViewRelationTablesDict;
    }

    /// <summary>
    /// 取 存储过程 关联的表
    /// </summary>
    /// <returns>Key 是存储过程的数据库名，Value 是关联表的数据库名</returns>
    public Dictionary<string, string[]> GetProcRelationTables()
    {
        return MyOql_ProcRelationTablesDict;
    }

    /// <summary>
    /// 根据数据库名称取 表 或 视图
    /// </summary>
    /// <param name=""DbName"">可以是表名,视图名</param>
    /// <returns></returns>
        public RuleBase GetMyOqlEntity(string DbName)
        {
            if (MyOql_EntityDict.ContainsKey(DbName))
                return MyOql_EntityDict[DbName];

            RuleBase rule = null;
            MyOql_VarTableDict.All(o =>
                {
                    var str = dbo.GetVarTableVars(o.Key, DbName);
                    if (str != null)
                    {
                        //缓存对象
                        rule = CacheHelper.Get(""MyOql_VarTable_"" + DbName, TimeSpan.FromHours(8), () =>
                           {
                               rule = Activator.CreateInstance(o.Value, str.Values.ToArray()) as RuleBase;
                               return rule;
                           }
                        );
                        return false;
                    }
                    return true;
                });

            if (rule != null) return rule;
            return new ConstTable();
        }
}
";
            strEnt = TidyTemplate(strEnt);


            var groups = sect.Table.ToMyList(o => o as IConfigGroupSect)
                    .Concat(sect.View.ToMyList(o => o as IConfigGroupSect))
                    .Concat(sect.Function.ToMyList(o => o as IConfigGroupSect));

            var entGroups = sect.Table.ToMyList(o => o as IConfigGroupSect)
                    .Concat(sect.View.ToMyList(o => o as IConfigGroupSect));

            var allRelationProcs = new XmlDictionary<string, string[]>();
            var allRelationViews = new XmlDictionary<string, string[]>();
            sect.View.ToMyList(o => o as IConfigSect).Concat(sect.Proc.ToMyList(o => o as IConfigSect)).All(o =>
            {

                var ve = o as MyOqlCodeGenSect.ViewCollection.ViewGroupCollection;
                if (ve != null)
                {
                    ve.ToMyList(c => c as MyOqlCodeGenSect.ViewCollection.ViewGroupCollection.ViewElement).All(c =>
                    {
                        var tabs = c.Table.AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (tabs != null && tabs.Length > 0)
                        {
                            allRelationViews[c.Name] = tabs;
                        }
                        return true;
                    });
                    return true;
                }

                var pe = o as MyOqlCodeGenSect.ProcCollection.ProcGroupCollection;
                if (pe != null)
                {
                    pe.ToMyList(c => c as MyOqlCodeGenSect.ProcCollection.ProcGroupCollection.ProcElement).All(c =>
                    {
                        var procs = c.Table.AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (procs != null && procs.Length > 0)
                        {
                            allRelationProcs[c.Name] = procs;
                        }
                        return true;
                    });
                    return true;
                }

                return true;
            });

            var fks = new List<FkColumn>();

            sect.Table.ToMyList(o => o as MyOqlCodeGenSect.TableCollection.TableGroupCollection).All(o =>
            {
                o.ToMyList(t => t as MyOqlCodeGenSect.TableCollection.TableGroupCollection.TableElement).All(e =>
                {
                    if (e.FKs != null)
                    {
                        e.FKs.ForEach(n =>
                        {
                            fks.Add(new FkColumn()
                            {
                                Entity = e.Name,
                                Column = n.Column,
                                CascadeUpdate = n.CascadeUpdate,
                                CascadeDelete = n.CascadeDelete,
                                RefTable = n.RefTable,
                                RefColumn = n.RefColumn
                            });
                        });
                    }

                    return true;
                });
                return true;
            });

            var rootGroups = Enumerable.Select(groups.Where(o => o.Name.HasValue()), o => o.Name.Split(".".ToArray(), StringSplitOptions.RemoveEmptyEntries)[0]).Distinct();

            strEnt = strEnt
                .TmpFor("Groups", groups)
                .DoIf(new CodeGen.DoIfObject<IConfigGroupSect>("HasGroupName", o => o.Name.HasValue()))
                .DoFor("RootGroup", o => dbo.TranslateDbName(o.Name.Split('.')[0]))
                .DoEach((SysEnts, _SysEntsString) =>
                {
                    var ents = (SysEnts as MyOqlCodeGenSect.TableCollection.TableGroupCollection).ToMyList
                        (o => o as MyOqlCodeGenSect.TableCollection.TableGroupCollection.BaseTableElement);


                    return _SysEntsString
                        .TmpFor("SysEnts", ents)
                        .DoIf(new CodeGen.DoIfObject<MyOqlCodeGenSect.TableCollection.TableGroupCollection.BaseTableElement>
                            ("IsFunction", c => sect.Function.GetConfig(c.Name) != null),
                            new CodeGen.DoIfObject<MyOqlCodeGenSect.TableCollection.TableGroupCollection.BaseTableElement>
                            ("IsVarTable", c =>
                            {
                                var tabConfig = (c as MyOqlCodeGenSect.TableCollection.TableGroupCollection.TableElement);
                                if (tabConfig != null)
                                {
                                    return !string.IsNullOrEmpty(tabConfig.VarName);
                                }
                                return false;
                            }))
                        .DoFor("Ent", c => dbo.TranslateDbName(c.MapName))
                        .DoFor("DbEnt", c => c.Name)
                        .DoEach((Paras, _PrarsString) =>
                        {
                            var tabConfig = Paras as MyOqlCodeGenSect.TableCollection.TableGroupCollection.TableElement;
                            if (tabConfig == null) return _PrarsString;

                            var varTables = new string[0];
                            if (tabConfig != null)
                            {
                                varTables = dbo.GetVarTableVars(tabConfig.VarName);
                            }


                            return _PrarsString
                                .TmpFor("Paras", tabConfig.Paras)
                                 .DoIf("NotFirst", o => tabConfig.Paras.IndexOf(p => p.Name == o.Name) > 0)
                                 .DoFor("ParaType", o => o.DbType.GetCsType().FullName)
                                 .DoFor("Para", o => dbo.TranslateDbName(o.Name))
                                 .EndFor()
                                 .TmpFor("Vars", varTables)
                                    .DoIf("NotFirst", o => varTables.IndexOf(p => p == o) > 0)
                                    .DoFor("Var", o => o)
                                 .EndFor()
                                 ;
                        })
                        .EndFor();

                })
                .EndFor()

                .TmpFor("RootGroups", rootGroups)
                .DoFor("Group", o => dbo.TranslateDbName(o))
                .EndFor()

                .TmpFor("AllGroups", entGroups)
                .DoEach((SysEnts, _SysEntsString) =>
                {
                    var ents = (SysEnts as IEnumerable).ToMyList(o => o as IConfigSect);

                    //if (SysEnts.IsType<MyOqlCodeGenSect.TableCollection.TableGroupCollection>())
                    //{
                    //    ents.AddRange(
                    //        (SysEnts as MyOqlCodeGenSect.TableCollection.TableGroupCollection).ToMyList(o => o as IConfigGroupSect)
                    //        );
                    //}
                    //else if (SysEnts.IsType<MyOqlCodeGenSect.ViewCollection.ViewGroupCollection>())
                    //{
                    //    ents.AddRange(
                    //         (SysEnts as MyOqlCodeGenSect.ViewCollection.ViewGroupCollection).ToMyList(o => o as IConfigSect)
                    //         );
                    //}
                    //    var varTables = new string[0];
                    //var tabConfig = SysEnts as MyOqlCodeGenSect.TableCollection.TableGroupCollection.TableElement ;
                    //        if (tabConfig != null)
                    //        {
                    //            varTables = dbo.GetVarTableVars(tabConfig.VarName);
                    //        }


                    return _SysEntsString
                        .TmpFor("entGroup", ents)
                        .DoIf(new CodeGen.DoIfObject<IConfigSect>("HasVar", c =>
                        {
                            var tabConfig = c as MyOqlCodeGenSect.TableCollection.TableGroupCollection.TableElement;
                            if (tabConfig == null) return false;
                            return !string.IsNullOrEmpty(tabConfig.VarName);
                        }))
                        .DoFor("Ent", c => dbo.TranslateDbName(c.MapName))
                        .DoFor("DbEntVarName", c =>
                        {
                            var tabConfig = c as MyOqlCodeGenSect.TableCollection.TableGroupCollection.TableElement;
                            if (tabConfig == null) return string.Empty;
                            return tabConfig.VarName;
                        })
                        .DoFor("DbEnt", c => c.Name)
                        .DoFor("FullGroup", c => SysEnts.Name.AsString().Split(".".ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => "." + dbo.TranslateDbName(s)).Join(""))
                        .EndFor()
                        ;

                })
                .EndFor()

                //.TmpFor("AllRelation", allRelationViews.Keys.Union(allRelationProcs.Keys))
                //    .DoFor("DbName", o => o)
                //    .DoFor("RelationTables", o =>
                //    {
                //        if (allRelationViews.ContainsKey(o)) return string.Join(@""",""", allRelationViews[o]);
                //        if (allRelationProcs.ContainsKey(o)) return string.Join(@""",""", allRelationProcs[o]);
                //        return "";
                //    })
                //.EndFor()
                .TmpFor("AllRelationView", allRelationViews.Keys)
                    .DoFor("DbName", o => o)
                    .DoFor("RelationTables", o =>
                    {
                        if (allRelationViews.ContainsKey(o)) return @"""" + string.Join(@""",""", allRelationViews[o]) + @"""";
                        if (allRelationProcs.ContainsKey(o)) return @"""" + string.Join(@""",""", allRelationProcs[o]) + @"""";
                        return "";
                    })
                .EndFor()

                .TmpFor("AllRelationProc", allRelationProcs.Keys)
                    .DoFor("DbName", o => o)
                    .DoFor("RelationTables", o =>
                    {
                        if (allRelationViews.ContainsKey(o)) return @"""" + string.Join(@""",""", allRelationViews[o]) + @"""";
                        if (allRelationProcs.ContainsKey(o)) return @"""" + string.Join(@""",""", allRelationProcs[o]) + @"""";
                        return "";
                    })
                .EndFor()

                .TmpFor("fks", fks)
                .DoFor("TableDbName", o => o.Entity)
                .DoFor("Property_DbName", o => o.Column)
                .DoFor("CascadeUpdate", o => o.CascadeUpdate ? "true" : "false")
                .DoFor("CascadeDelete", o => o.CascadeDelete ? "true" : "false")
                .DoFor("RefTable_DbName", o => o.RefTable)
                .DoFor("RefColumn_DbName", o => o.RefColumn)
                .EndFor()

                ;
            return strEnt;
        }

    }
}
