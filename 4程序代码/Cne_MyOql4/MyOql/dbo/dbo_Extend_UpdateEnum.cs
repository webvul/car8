using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using System.Data.Common;

namespace MyOql
{
    public static partial class dbo
    {
        /// <summary>
        /// 按 MyOqlSet 里的 Entity 进行更新。
        /// </summary>
        /// <param name="set"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static MyOqlSet<TE> UpdateEnum<TE>(this MyOqlSet<TE> set, Func<Enum, string> func)
            where TE : RuleBase
        {
            var mSet = set as MyOqlSet;
            UpdateEnum(mSet, func);
            return set;
        }


        /// <summary>
        /// 按 MyOqlSet 中的列 和 Entity 中的列进行比对（表列都需要一致）。对其中的枚举进行国际化。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="set"></param>
        /// <param name="Entity">枚举的实体容器</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static MyOqlSet<TE> UpdateEnum<T, TE>(this MyOqlSet<TE> set, T Entity, Func<Enum, string> func)
            where T : IReadEntity
            where TE : RuleBase
        {
            var mSet = set as MyOqlSet;
            UpdateEnum<T>(mSet, Entity, func);
            return set;
        }


        /// <summary>
        /// 使用指定的Type更新MyOqlSet
        /// </summary>
        /// <param name="set"></param>
        /// <param name="entityType"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static MyOqlSet<TE> UpdateEnum<TE>(this MyOqlSet<TE> set, Type entityType, Func<Enum, string> func)
            where TE : RuleBase
        {
            var mSet = set as MyOqlSet;
            UpdateEnum(mSet, entityType, func);
            return set;
        }
        /// <summary>
        /// 按 ColumnName 进行枚举化。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set"></param>
        /// <param name="columnName">显示的列别名，如果没有别名，按列名。</param>
        /// <param name="Enum">列别名所对应的枚举。</param>
        /// <param name="func">列别名枚举资源转换回调</param>
        /// <returns></returns>
        public static MyOqlSet<TE> UpdateEnum<T, TE>(this MyOqlSet<TE> set, string columnName, Func<T, string> func)
            where T : IComparable, IFormattable, IConvertible
            where TE : RuleBase
        {
            UpdateEnum<T>(set, columnName, func);
            return set;
        }



        /// <summary>
        /// 按 MyOqlSet 里的 Entity 进行更新。
        /// </summary>
        /// <param name="set"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static MyOqlSet UpdateEnum(this MyOqlSet set, Func<Enum, string> func)
        {
            GodError.Check(set.Entity == null, () => "MyOqlSet 的 Entity 不能为空");
            var entityType = set.Entity.GetType() + "+Entity";
            return UpdateEnum(set, System.Web.Compilation.BuildManager.GetType(entityType, false), func);
        }


        /// <summary>
        /// 按 MyOqlSet 中的列 和 Entity 中的列进行比对（表列都需要一致）。对其中的枚举进行国际化。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set"></param>
        /// <param name="Entity">枚举的实体容器</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static MyOqlSet UpdateEnum<T>(this MyOqlSet set, T Entity, Func<Enum, string> func)
            where T : IReadEntity
        {
            GodError.Check(Entity == null, () => "枚举的实体容器不能为空");

            Type type = Entity.GetType();

            return UpdateEnum(set, type, func);
        }


        /// <summary>
        /// 使用指定的Type更新MyOqlSet
        /// </summary>
        /// <param name="set"></param>
        /// <param name="entityType"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static MyOqlSet UpdateEnum(this MyOqlSet set, Type entityType, Func<Enum, string> func)
        {
            GodError.Check(entityType == null, () => "实体类型不能为空");
            var EnumColumns = new Dictionary<string, Type>();


            var ent = entityType.DeclaringType.Name.Slice(0, -4).AsString();

            var ps = entityType.GetProperties();
            for (int i = 0; i < set.Columns.Length; i++)
            {
                var col = set.Columns[i];
                var p = ps.FirstOrDefault(o =>
                {
                    if (o.Name == col.Name)
                    {
                        if (col.IsConst() == true)
                        {
                            return true;
                        }
                        if (col.IsSimple())
                        {
                            var simple = col as SimpleColumn;
                            if (simple.TableDbName.HasValue())
                            {
                                return ent == dbo.Event.Idbr.GetMyOqlEntity(simple.TableDbName).GetName();
                            }
                        }

                        //判断复合列. 如果里面只有一个 数据列. 则OK , 否则, 按常数列处理.
                        RuleBase table = null;
                        var complex = col as ComplexColumn;
                        complex.Recusion(c =>
                        {
                            if (table == null)
                            {
                                if (c.IsConst() == false)
                                {
                                    table = dbo.Event.Idbr.GetMyOqlEntity(c.TableDbName);
                                    return true;
                                }
                            }
                            else
                            {
                                table = null;
                                return false;
                            }
                            return false;
                        });

                        if (table.HasData())
                        {
                            return true;
                        }
                        else
                        {
                            throw new GodError();
                            //return ent == dbo.Event.Idbr.GetMyOqlEntity(col.TableDbName).GetName();
                        }

                    }
                    return false;
                });
                if (p != null && p.PropertyType.IsEnum)
                {
                    EnumColumns[col.Name] = p.PropertyType;
                }
            }

            foreach (var row in set.Rows)
            {
                foreach (var item in EnumColumns)
                {
                    var rowValue = row[item.Key].AsString();

                    if (rowValue.HasValue() == false)
                    {
                        continue;
                    }

                    object val = EnumHelper.ToEnum(rowValue, item.Value, 0);

                    if (func != null)
                    {
                        val = func((Enum)val);

                        set.Columns.First(o => o.NameEquals(item.Key, true)).DbType = System.Data.DbType.AnsiString;
                    }

                    row[item.Key] = val;

                }
            }

            return set;
        }
        /// <summary>
        /// 按 ColumnName 进行枚举化。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set"></param>
        /// <param name="columnName">显示的列别名，如果没有别名，按列名。</param>
        /// <param name="Enum">列别名所对应的枚举。</param>
        /// <param name="func">列别名枚举资源转换回调</param>
        /// <returns></returns>
        public static MyOqlSet UpdateEnum<T>(this MyOqlSet set, string columnName, Func<T, string> func)
            where T : IComparable, IFormattable, IConvertible
        {
            var columnIndex = set.Columns.IndexOf(o => o.GetAlias() == columnName);
            if (columnIndex < 0) return set;

            set.Columns[columnIndex].DbType = System.Data.DbType.AnsiString;


            foreach (var row in set.Rows)
            {
                var rowValue = row[columnName].AsString();

                //if (rowValue.HasValue() == false)
                //{
                //    continue;
                //}

                var val = EnumHelper.ToEnum<T>(rowValue);

                //if (val.AsString() == "0") continue;


                var resVal = string.Empty;

                if (func != null)
                {
                    resVal = func(val);
                }

                if (resVal != null)
                {
                    row[columnName] = resVal;
                }
            }

            return set;
        }

    }
}
