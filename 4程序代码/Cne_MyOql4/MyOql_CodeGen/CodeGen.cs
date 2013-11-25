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
    /// <remarks>
    /// 规范数据库设计：
    /// 1. 尽量使用不能为空设计。
    /// 2. 
    /// 存储过程中  Return 参数如果为空, 则返回影响行数.
    /// </remarks>
    /// </summary>
    public partial class MyOqlCodeGen
    {
        //public enum CodeGenLogEnum
        //{
        //    Start,
        //    End,
        //}
        //public ILog Log { get; set; }

        public event Action<IConfigSect> Doing;
        public event Action<IConfigSect> Doed;

        public void OnDoing(IConfigSect config) { if (Doing != null) Doing(config); }
        public void OnDoed(IConfigSect config) { if (Doed != null) Doed(config); }

        /// <summary>
        /// 表说明，三列： ConfigName, TableName，Descr
        /// </summary>
        public static List<TableDefineInfo> TableDescriptions { get; set; }

        /// <summary>
        /// 从数据库选出的列。有五列： ConfigName, TableName，ColumnName，Type，Precision，Length，Scale。
        /// </summary>
        public static List<ColumnDefineDetail> ColumnDefines { get; set; }
        public static List<ProcParaDetail> ProcParaDefines { get; set; }
        //private static Dictionary<DatabaseType, IEnumerable<TypeMap>> TypeMaps { get; set; }
        public static MyOqlCodeGenSect sect { get; set; }


        static MyOqlCodeGen()
        {
            sect = (MyOqlCodeGenSect)ConfigurationManager.GetSection("MyOqlCodeGen");
            //TypeMaps = new Dictionary<DatabaseType, IEnumerable<TypeMap>>();
            ColumnDefines = new List<ColumnDefineDetail>();
            TableDescriptions = new List<TableDefineInfo>();
            ProcParaDefines = new List<ProcParaDetail>();
            Init();
        }

        public MyOqlCodeGen()
        {
        }

        //public static IEnumerable<TypeMap> GetTypeMap(DatabaseType db)
        //{
        //    if (TypeMaps.ContainsKey(db) == false)
        //    {
        //        TypeMaps[db] = dbo.GetDbProvider(db).GetTypeMap();
        //    }

        //    return TypeMaps[db];
        //}


        /// <summary>
        /// 处理一下模板格式.
        /// </summary>
        /// <param name="TemplateString"></param>
        /// <returns></returns>
        public static string TidyTemplate(string TemplateString)
        {
            TemplateString = Regex.Replace(TemplateString, string.Format(@"\r?\n[\t\x20]*({0}if:.*?{1})", CodeGen.LeftMark, CodeGen.RightMark), @"$1", RegexOptions.Compiled);
            TemplateString = Regex.Replace(TemplateString, string.Format(@"\r?\n[\t\x20]*({0}eif:.*?{1})", CodeGen.LeftMark, CodeGen.RightMark), @"$1", RegexOptions.Compiled);
            TemplateString = Regex.Replace(TemplateString, string.Format(@"\r?\n[\t\x20]*({0}else{1})", CodeGen.LeftMark, CodeGen.RightMark), @"$1", RegexOptions.Compiled);
            TemplateString = Regex.Replace(TemplateString, string.Format(@"\r?\n[\t\x20]*({0}fi{1})", CodeGen.LeftMark, CodeGen.RightMark), @"$1", RegexOptions.Compiled);
            TemplateString = Regex.Replace(TemplateString, string.Format(@"\r?\n[\t\x20]*({0}for:.*?{1})", CodeGen.LeftMark, CodeGen.RightMark), @"$1", RegexOptions.Compiled);
            TemplateString = Regex.Replace(TemplateString, string.Format(@"\r?\n[\t\x20]*({0}endfor{1})", CodeGen.LeftMark, CodeGen.RightMark), @"$1", RegexOptions.Compiled);

            return TemplateString;
        }


        public StringDict Do()
        {
            StringDict retVal = new StringDict();

            retVal["dbr"] = GenDbr();

            foreach (var group in sect.Table.ToMyList(o => o as MyOqlCodeGenSect.TableCollection.TableGroupCollection))
            {
                retVal[group.Name.AsString("Table")] = GenEnt(group, "ITableRule");
            }

            foreach (var group in sect.View.ToMyList(o => o as MyOqlCodeGenSect.ViewCollection.TableGroupCollection))
            {
                var groupName = group.Name.AsString("View");

                if (retVal.ContainsKey(groupName) == false)
                {
                    retVal[groupName] = string.Empty;
                }

                retVal[groupName] += GenEnt(group, "IViewRule");
            }

            foreach (var group in sect.Function.ToMyList(o => o as MyOqlCodeGenSect.FunctionCollection.FunctionGroupCollection))
            {
                var groupName = group.Name.AsString("Function");
                if (retVal.ContainsKey(groupName) == false)
                {
                    retVal[groupName] = string.Empty;
                }

                retVal[groupName] += GenEnt(group, "IFunctionRule");
            }

            foreach (var group in sect.Proc.ToMyList(o => o as MyOqlCodeGenSect.ProcCollection.ProcGroupCollection))
            {
                retVal[group.Name.AsString("Proc") + "_Private"] = @"
public partial class dbr_Proc
{
" + GenProcPrivate(group)
+ @"
}
"
              ;

                if (group.Name.HasValue() == false)
                {
                    var groupName = group.Name.AsString("Proc");
                    if (retVal.ContainsKey(groupName) == false)
                    {
                        retVal[groupName] = string.Empty;
                    }


                    retVal[groupName] += @"
public partial class dbr
{
" + GenProc(group)
+ @"
}
"
                ;
                }
            }

            var dg = new DbrGroup();
            dg.InitGroup();
            retVal["Group"] = dg.ToCode();
            return retVal;
        }

    }
}
