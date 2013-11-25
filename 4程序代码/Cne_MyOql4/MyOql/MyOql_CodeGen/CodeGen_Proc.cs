using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using System.Configuration;
using System.Reflection;
using System.Data;

namespace MyOql.MyOql_CodeGen
{
    public partial class MyOqlCodeGen
    {
        private string GenProc(MyOqlCodeGenSect.ProcCollection.ProcGroupCollection group)
        {
            var strProc = @"
            【for:Proc】
        $ProcCode$
            【endfor】
";

            strProc = TidyTemplate(strProc);

            strProc = strProc
                .TmpFor("Proc", group.ToMyList(o => o as MyOqlCodeGenSect.ProcCollection.ProcGroupCollection.ProcElement))
                .DoFor("ProcCode", o =>
                {
                    OnDoing(o);

                    var strProcTemp = o.ToPublicCode();

                    OnDoed(o);
                    return strProcTemp;
                })
                .EndFor(); 

            return strProc;
        }

        private string GenProcPrivate(MyOqlCodeGenSect.ProcCollection.ProcGroupCollection group)
        {
            var strProc = @"
            【for:Proc】
        $ProcCode$
            【endfor】
";

            strProc = TidyTemplate(strProc);

            strProc = strProc
                .TmpFor("Proc", group.ToMyList(o => o as MyOqlCodeGenSect.ProcCollection.ProcGroupCollection.ProcElement))
                .DoFor("ProcCode", o =>
                {
                    return DoProcPrivate(o);
                })
                .EndFor(); 

            return strProc;
        }
         
 
        public string DoProcPrivate(MyOqlCodeGenSect.ProcCollection.ProcGroupCollection.ProcElement ProcElement)
        {
            OnDoing(ProcElement);
            var Proc = ProcElement.Name;
            //var MapName = GetDbMapName(Proc);

            //【】返回类型: 列表.
            var provider = new Provider.SqlServer().GetTypeMap();
            var procOne = sect.Proc.GetConfig(Proc);//.ToMyList().First(o => (o as MyOqlCodeGenSect.ProcCollection.ProcGroupCollection).Name == Proc);// GetProcOneFromConfig(Proc);

            DatabaseType dbType = dbo.GetDatabaseType(procOne.db);
            //var list = procOne.Paras.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

            var strProcTemp = Proc_Private.TmpFor("paras", procOne.Paras)
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

            OnDoed(ProcElement);
            return strProcTemp;
        }

    
    }
}
