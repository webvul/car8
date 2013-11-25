using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyOql;
using MyCmn;
using MyOql.MyOql_CodeGen;
using System.Configuration;

namespace MyOql.MyOql_CodeGen
{
    public enum DbrGroupType
    {
        Table,
        View,
        Function,
        Proc,
    }
    public class DbrGroupEnt
    {
        public DbrGroupType Type { get; set; }
        public string Name { get; set; }
        public string DbName { get; set; }

        public DbrGroupEnt(DbrGroupType type, IConfigSect ele)
        {
            this.Type = type;
            this.DbName = ele.Name;
            this.Name = dbo.TranslateDbName(ele.MapName.AsString(ele.Name));
        }
    }
    public class DbrGroup : ICode
    {
        public string Name { get; set; }
        public List<DbrGroupEnt> Entitys { get; set; }
        public List<DbrGroup> SubGroups { get; set; }

        public DbrGroup()
        {
            this.Entitys = new List<DbrGroupEnt>();
            this.SubGroups = new List<DbrGroup>();
        }

        public void InitGroup()
        {
            foreach (var group in MyOqlCodeGen.sect.Table.ToMyList(o => o as MyOqlCodeGenSect.TableCollection.TableGroupCollection))
            {
                if (group.Name.HasValue())
                {
                    DbrGroup g = GetGroup(group.Name);
                    foreach (var item in group.ToMyList(o => o as MyOqlCodeGenSect.TableCollection.TableGroupCollection.BaseTableElement).ToArray())
                    {
                        g.Entitys.Add(new DbrGroupEnt(DbrGroupType.Table, item));
                    }
                }
            }

            foreach (var group in MyOqlCodeGen.sect.View.ToMyList(o => o as MyOqlCodeGenSect.ViewCollection.TableGroupCollection))
            {
                if (group.Name.HasValue())
                {
                    DbrGroup g = GetGroup(group.Name);
                    foreach (var item in group.ToMyList(o => o as MyOqlCodeGenSect.ViewCollection.TableGroupCollection.BaseTableElement).ToArray())
                    {
                        g.Entitys.Add(new DbrGroupEnt(DbrGroupType.View, item));
                    }
                }
            }

            foreach (var group in MyOqlCodeGen.sect.Function.ToMyList(o => o as MyOqlCodeGenSect.FunctionCollection.TableGroupCollection))
            {
                if (group.Name.HasValue())
                {
                    DbrGroup g = GetGroup(group.Name);
                    foreach (var item in group.ToMyList(o => o as MyOqlCodeGenSect.FunctionCollection.TableGroupCollection.BaseTableElement).ToArray())
                    {
                        g.Entitys.Add(new DbrGroupEnt(DbrGroupType.Function, item));
                    }
                }
            }

            foreach (var group in MyOqlCodeGen.sect.Proc.ToMyList(o => o as MyOqlCodeGenSect.ProcCollection.ProcGroupCollection))
            {
                if (group.Name.HasValue())
                {
                    DbrGroup g = GetGroup(group.Name);
                    foreach (var item in group.ToMyList(o => o as MyOqlCodeGenSect.ProcCollection.ProcGroupCollection.ProcElement).ToArray())
                    {
                        g.Entitys.Add(new DbrGroupEnt(DbrGroupType.Proc, item));
                    }
                }
            }
        }

        private DbrGroup GetGroup(string groupName, DbrGroup group = null)
        {
            if (group == null)
            {
                group = this;
            }


            if (group.Name == groupName) return group;

            var each = groupName.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (group.SubGroups.Count > 0)
            {
                //逐级找。
                foreach (var item in group.SubGroups)
                {
                    if (item.Name == groupName)
                    {
                        return item;
                    }

                    var length = item.Name.Count(o => o == '.') + 1;
                    var p = each.Take(length).Join(".");

                    if (item.Name != p)
                    {
                        continue;
                    }
                    else
                    {
                        return GetGroup(groupName, item);
                    }
                }
            }

            //添加。
            DbrGroup pg = group;


            for (int i = 0; i < each.Length; i++)
            {
                var name = each.Take(i + 1).Join(".");
                var dg = new DbrGroup();
                dg.Name = name;
                pg.SubGroups.Add(dg);
                pg = dg;
            }

            return pg;
        }

        public string ToCode(CodeLanguage Language = CodeLanguage.CsNet)
        {
            if (Name.HasValue() == false)
            {
                return this.SubGroups.Select(o => o.ToCode()).Join("");
            }

            string strTemp = @"
public class $Group$GroupClass
{
【for:Ents】
【if:Ent】
    public $Entity$Rule $Entity$ { get { return new $Entity$Rule(); } }
【fi】
【if:Function】
    public $Entity$Rule $Entity$(【for:Paras】【if】,【fi】$ParaType$ $Para$【endfor】)
    {
        return new $Entity$Rule(【for:Paras】【if】,【fi】$Para$【endfor】);
    }
【fi】
【if:Proc】
    $ProcCode$ 
【fi】
【endfor】
【for:SubGroups】
    public $SubGroup$GroupClass $SubGroup$ { get; private set; }

    $SubGroupCode$
【endfor】
    public $Group$GroupClass()
    {
【for:SubGroups】
        this.$SubGroup$ = new $SubGroup$GroupClass() ;
【endfor】
    }
}
";

            return MyOqlCodeGen.TidyTemplate(strTemp)
                .TmpFor("Ents", this.Entitys)
                .DoIf(new CodeGen.DoIfObject<DbrGroupEnt>("Ent", o => o.Type == DbrGroupType.Table || o.Type == DbrGroupType.View))
                .DoIf(new CodeGen.DoIfObject<DbrGroupEnt>("Function", o => o.Type == DbrGroupType.Function))
                .DoIf(new CodeGen.DoIfObject<DbrGroupEnt>("Proc", o => o.Type == DbrGroupType.Proc))
                .DoEach((ent, tmp) =>
                {
                    if (ent.Type != DbrGroupType.Function)
                    {
                        return tmp;
                    }

                    var tabConfig = MyOqlCodeGen.sect.GetEntity(ent.DbName) as MyOqlCodeGenSect.FunctionCollection.TableGroupCollection.TableElement;

                    var provider = new Provider.SqlServer().GetTypeMap();

                    return tmp.TmpFor("Paras", tabConfig == null ? new MyParaDefine("") : tabConfig.Paras)
                        .DoIf(o => tabConfig == null ? true : tabConfig.Paras.IndexOf(p => p.Name == o.Name) > 0)
                        .DoFor("ParaType", o => provider.First(t => t.DbType == o.DbType).CsType.FullName)
                        .DoFor("Para", o => o.Name)
                        .EndFor();
                })
                .DoFor("Entity", o => o.Name)
                .DoFor("ProcCode", o =>
                    {
                        if (o.Type != DbrGroupType.Proc) return string.Empty;
                        var proc = MyOqlCodeGen.sect.GetEntity(o.DbName) as MyOqlCodeGenSect.ProcCollection.ProcGroupCollection.ProcElement;
                        return proc.ToPublicCode();
                    })
                .EndFor()

                .TmpFor("SubGroups", this.SubGroups)
                .DoFor("SubGroup", o => dbo.TranslateDbName(o.Name.Split(".".ToArray(), StringSplitOptions.RemoveEmptyEntries).Last()))
                .DoFor("SubGroupCode", o => o.ToCode())
                .EndFor()
                .Replace("$Group$", dbo.TranslateDbName(this.Name.Split(".".ToArray(), StringSplitOptions.RemoveEmptyEntries).Last()));
        }
    }
}
