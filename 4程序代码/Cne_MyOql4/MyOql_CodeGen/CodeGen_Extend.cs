using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using System.Configuration;
using System.Reflection;
using System.Xml;
using System.Data;

namespace MyOql.MyOql_CodeGen
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 对 Sqlserver 来说， varchar(max) = text , nvarchar(max) = ntext
    /// 对数据库来说，应该尽量使用 varchar(max) ，所以 MyOql 中的 text 在传递给数据库时，使用  varchar(max)  而没有了 text
    /// </remarks>
    public partial class MyOqlCodeGen
    {
        public class GroupModel
        {
            public string Name { get; set; }
            /// <summary>
            /// 父路径
            /// </summary>
            public string Path { get; set; }
            public GroupModel[] SubGroup { get; set; }
            public RuleBase[] SubEntity { get; set; }

            public GroupModel GetGroup(string fullGroupName)
            {
                if (this.ToString() == fullGroupName)
                {
                    return this;
                }
                else if (SubGroup != null)
                {
                    var ret = SubGroup.FirstOrDefault(o => o.GetGroup(fullGroupName) != null);
                    if (ret != null) return ret;
                }
                return null;
            }

            public override string ToString()
            {
                return Path.HasValue(o => Path + ".", o => string.Empty) + Name;
            }
        }


        /// <summary>
        /// 校验是否包含重复。 在外部调用。
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="RootName"></param>
        /// <returns></returns>
        public static string Check(string FileName, string RootName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(FileName);

            var root = doc.SelectSingleNode(@"/" + RootName);

            //先判断组名。
            var listGroup = new List<string>();

            //groups.Select(o => o.Name).GroupBy(o => o).Any(o =>
            //    {
            //        if (o.Count() > 1 && o.Key.HasValue()) throw new GodError("出现重复定义的组名：" + o.Key);
            //        return true;
            //    });

            //List<IConfigSect> list = new List<IConfigSect>();
            //groups.All(o =>
            //{
            //    list.AddRange((o as ConfigurationElementCollection).ToMyList(c => c as IConfigSect));
            //    return true;
            //});

            //list.Select(o => o.Name).GroupBy(o => o).Any(o =>
            //{
            //    if (o.Count() > 1) throw new GodError("出现重复定义的实体名：" + o.Key);
            //    return true;
            //});

            return string.Empty;
        }

        public static List<ColumnDefineDetail> FindColumns(MyOqlCodeGenSect.TableCollection.TableGroupCollection.BaseTableElement Config)
        {
            var tabConfig = Config as MyOqlCodeGenSect.TableCollection.TableGroupCollection.TableElement;


            //GetColumnsFromDb(dbType, Config.Name, Config.db, Config.Owner);

            //如果是函数，则从 Enum 定义中查找列信息。
            if (tabConfig != null && tabConfig.Paras.Count > 0)
            {
                //函数是少量群体，不必自动化实现动态类型识别。 参数函数和返回列类型必须写死。 
                var ret = new List<ColumnDefineDetail>();
                Config.Enums.All(o =>
                {
                    var enumDbType = DbType.String;
                    var type = Type.GetType(o.EnumType);
                    if (type != null)
                    {
                        enumDbType = type.GetDbType();
                    }
                    ret.Add(new ColumnDefineDetail()
                    {
                        ColumnDbName = o.Name,
                        db = tabConfig.db,
                        CsType = o.EnumType,
                        TableDbName = tabConfig.Name,
                        MapName = o.TranslateName,
                        DbType = enumDbType,
                        Nullable = o.EnumType.EndsWith("?")
                    });

                    return true;
                });

                return ret;
            }


            //默认都是从数据库取出来。
            var dit =
                ColumnDefines.Where(o =>
                string.Equals(o.db, Config.db, StringComparison.CurrentCultureIgnoreCase) &&
                string.Equals(o.TableDbName, Config.Name, StringComparison.CurrentCultureIgnoreCase)
                )
                .Distinct((a, b) => a.ColumnDbName == b.ColumnDbName).ToList();

            dit.All(o =>
            {
                GodError.Check(o.ColumnDbName.IsIn("_Alias_", "_Config_"), "数据库表中的列，不能是 _Alias_ , 或 _Config_，请改用其它名称 。");
                return true;
            });
            return dit;
        }


        public List<SimpleColumn> GetColumnsFromEntity(Type RuleType)
        {
            GodError.Check(RuleType.IsSubclassOf(typeof(RuleBase)) == false, "类型必须是从 RuleBase 继承的类.");

            var list = new List<SimpleColumn>();
            var obj = Activator.CreateInstance(RuleType);

            var colType = typeof(SimpleColumn);
            RuleType.GetProperties().All(o =>
            {
                if (o.PropertyType.Name != "SimpleColumn" || (o.PropertyType.IsSubclassOf(colType) == false))
                {
                    return true;
                }
                list.Add((SimpleColumn)RuleType.InvokeMember(o.Name, BindingFlags.GetProperty, null, obj, null));
                return true;
            });

            return list;
        }


    }
}
