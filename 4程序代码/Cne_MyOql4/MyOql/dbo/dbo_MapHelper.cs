using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using MyCmn;

namespace MyOql
{
    public static partial class dbo
    {
        /// <summary>
        /// 把 Model 转为 字典，是一个和  ModelToDictionary(RuleBase Entity, IModel objModel) 相同算法的函数。
        /// </summary>
        /// <remarks>功能和 FastInvoke 是一样的。针对 WhereClip 会有优化。</remarks>
        /// <param name="objModel"></param>
        /// <returns></returns>
        public static StringDict ModelToStringDict(object objModel)
        {
            WhereClip where = objModel as WhereClip;
            if (where != null)
            {
                var dictModel = new StringDict();
                do
                {
                    if (dbo.EqualsNull(where.Query)) break;

                    dictModel[where.Query.ToString()] = where.Value.AsString(null);

                } while ((where = where.Next) != null);
                return dictModel;
            }

            return FastInvoke.Model2StringDict(objModel);
        }
        /// <summary>
        /// 把 Model 转为 字典。核心函数。
        /// <remarks>
        /// 数据实体推入到数据库时使用,解析如下类型： 
        /// String
        /// Dictionary&lt;ColumnClip, object&gt;
        /// Dictionary&lt;string, object&gt;
        /// Dictionary&lt;string, string&gt;
        /// 类
        /// 值类型结构体
        /// </remarks>
        /// </summary>
        /// <param name="Entity">如果为空,则生成 ConstColumn </param>
        /// <param name="objModel"></param>
        /// <returns></returns>
        public static XmlDictionary<ColumnClip, object> ModelToDictionary(RuleBase Entity, IModel objModel)
        {
            XmlDictionary<ColumnClip, object> dictModel = objModel as XmlDictionary<ColumnClip, object>;
            if (dictModel != null)
            {
                return dictModel;
            }

            Func<string, ColumnClip> GetColumn = colKey =>
            {
                if (Entity == null) return new ConstColumn(colKey);
                var retVal = Entity.GetColumn(colKey);
                if (retVal.IsDBNull()) return new ConstColumn(colKey);
                else return retVal;
            };

            IDictionary dict = objModel as IDictionary;
            if (dict != null)
            {
                dictModel = new XmlDictionary<ColumnClip, object>();

                var objTypes = objModel.GetType().getGenericType().GetGenericArguments();

                //Key 可能是 string,Column 没其它的.
                if (objTypes[0] == typeof(string))
                {
                    foreach (string strKey in dict.Keys)
                    {
                        var theCol = GetColumn(strKey);
                        if (dbo.EqualsNull(theCol) == false)
                        {
                            dictModel.Add(theCol, dict[strKey]);
                        }

                    }
                    return dictModel;
                }
                else if (objTypes[0] == typeof(ColumnClip))
                {
                    foreach (ColumnClip colKey in dict.Keys)
                    {
                        dictModel.Add(colKey, dict[colKey]);
                    }
                    return dictModel;
                }

            }

            WhereClip where = objModel as WhereClip;
            if (where != null)
            {
                dictModel = new XmlDictionary<ColumnClip, object>();
                do
                {
                    if (dbo.EqualsNull(where.Query)) break;

                    dictModel[where.Query] = where.Value;

                } while ((where = where.Next) != null);
                return dictModel;
            }

            dictModel = new XmlDictionary<ColumnClip, object>();

            IEntity entity = objModel as IEntity;
            if (entity != null)
            {
                foreach (var strKey in entity.GetProperties())
                {
                    var theCol = GetColumn(strKey);
                    if (theCol.EqualsNull()) continue;
                    dictModel.Add(theCol, entity.GetPropertyValue(strKey));
                }
                return dictModel;
            }


            GodError.Check((objModel as ContextClipBase) != null, "Model 不能是 MyOql 子句");


            Type type = objModel.GetType();
            GodError.Check(type.FullName == "MyOql.ColumnClip", "ColumnClip 不能作为 Model");
            foreach (PropertyInfo prop in type.GetProperties())
            {
                var methodInfo = type.GetMethod("get_" + prop.Name);
                if (methodInfo == null) continue;

                var theCol = GetColumn(prop.Name);
                dictModel.Add(theCol, FastInvoke.GetPropertyValue(objModel, type, methodInfo));
            }

            return dictModel;
        }


        ///// <summary>
        ///// 将一个对象转化为 XmlDictionary
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="objModel"></param>
        ///// <returns></returns>
        //public static XmlDictionary<string, object> ModelToDictionary<T>(T objModel)
        //    where T : class , new()
        //{
        //    Type type = typeof(T);
        //    var IsDict = type.GetInterface("System.Collections.IDictionary") != null;
        //    XmlDictionary<string, object> dict = new XmlDictionary<string, object>();

        //    if (IsDict == false)
        //    {
        //        var entity = objModel as IEntity;
        //        if (entity != null)
        //        {
        //            foreach (var key in entity.GetProperties())
        //            {
        //                dict[key] = entity.GetPropertyValue(key);
        //            }
        //        }
        //        else
        //        {
        //            foreach (var pi in type.GetProperties())
        //            {
        //                var methodInfo = type.GetMethod("get_" + pi.Name);
        //                if (methodInfo == null) continue;

        //                dict[pi.Name] = FastInvoke.GetPropertyValue(objModel, type, methodInfo);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        var objDict = objModel as IDictionary;

        //        foreach (var key in objDict.Keys)
        //        {
        //            dict[key.AsString()] = objDict[key];
        //        }
        //    }

        //    return dict;
        //}

        public static T DictionaryToModel<TKey, TValue, T>(this Dictionary<TKey, TValue> Dict, T NewModel)
        {
            return DictionaryToFuncModel<TKey, TValue, T>(Dict, () => NewModel);
        }

        /// <summary>
        /// 把字典解析到 Model 类型的 Model 上。
        /// <remarks>
        ///  逻辑同 FastInvoke.StringDictToModel
        /// 从数据库返回数据实体时使用,解析如下类型： 
        /// String
        /// IDictionary
        /// 类(支持递归赋值。如果第一级属性找不到，则查找第二级非基元属性，依次向下查找。)
        /// Json树格式，如果在HTML中Post Json对象，如 cols[id][sid] = 10 则可以映射到合适的对象上。
        /// 值类型结构体,主要适用于 数值，Enum类型。对于结构体，把 结果集第一项值 强制类型转换为该结构体类型，所以尽量避免使用自定义结构体。
        /// </remarks>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dict"></param>
        /// <param name="NewModelFunc">关键是 泛型！Model可以为null</param>
        /// <returns></returns>
        public static T DictionaryToFuncModel<TKey, TValue, T>(this IDictionary<TKey, TValue> Dict, Func<T> NewModelFunc)
        {
            if (Dict == null)
            {
                return default(T);
            }

            Type type = typeof(T);

            T ret = default(T);

            //Object 表示没有反射出 T 类型，  IsInterface 时亦然。
            if (type.FullName == "System.Object" || type.IsInterface)
            {
                ret = NewModelFunc();
                type = ret.GetType();
            }

            var retDict = ret as IDictionary;

            if (retDict != null)
            {
                var genericTypes = type.getGenericType().GetGenericArguments();
                if (genericTypes.Length == 2)
                {
                    Func<Type, object, object> getKey = (_type, _value) =>
                    {
                        if (_type.FullName == "MyOql.ColumnClip")
                        {
                            return new ConstColumn(_value);
                        }
                        return ValueProc.AsType(_type, _value);
                    };

                    foreach (KeyValuePair<TKey, TValue> kv in Dict)
                    {
                        retDict[getKey(genericTypes[0], kv.Key)] = ValueProc.AsType(genericTypes[1], kv.Value);
                    }

                }
                else
                {
                    foreach (var kv in Dict)
                    {
                        retDict[kv.Key] = kv.Value;
                    }
                }
                return (T)(object)retDict;
            }

            return FastInvoke.StringDict2Model(StringDict.LoadFrom(Dict), NewModelFunc);
        }

        /// <summary>
        /// 设置字典的属性。
        /// </summary>
        /// <param name="dict">数据源。如果该数据源没有相应字典，则会自动创建</param>
        /// <param name="key">Html Post 的Key，如：  cols[id][sid] </param>
        /// <param name="value">值</param>
        private static void setDictValue(XmlDictionary<string, object> dict, string key, object value)
        {
            var allDeep = key.Count(c => c == '[');

            var curKey_LastIndex = 0;
            var curKey = string.Empty;
            //取当前级别的Object
            XmlDictionary<string, object> curObject = dict;
            for (int i = 0; i < allDeep; i++)
            {
                curKey_LastIndex = key.IndexOf('[', curKey_LastIndex + 1);

                //计算当前深度下的Key  , 如：cols[cid][sid]，深度为0 ， 返回 cid ，为1返回 sid
                curKey = key.Slice(curKey_LastIndex + 1, key.IndexOf(']', curKey_LastIndex + 1)).AsString();

                if (i < (allDeep - 1))
                {
                    if (curObject.ContainsKey(curKey) == false) curObject[curKey] = new XmlDictionary<string, object>();
                    curObject = curObject[curKey] as XmlDictionary<string, object>;
                }
            }
            curObject[curKey] = value;
        }


        /// <summary>
        /// 更新Model的一个属性值。Model可以是 字典,Where,类.
        /// </summary>
        /// <param name="objModel">可以是 字典,Where,类.</param>
        /// <param name="Column">要更新的属性</param>
        /// <param name="Value">要更新的值</param>
        /// <returns></returns>
        public static bool UpdateOneProperty(object objModel, ColumnClip Column, object Value)
        {
            if (Column.IsDBNull()) return false;
            Type type = objModel.GetType();
            IDictionary dictModel = objModel as IDictionary;
            if (dictModel != null)
            {
                var objTypes = type.getGenericType().GetGenericArguments();

                //Key 可能是 string,Column 没其它的.
                if (objTypes[0] == typeof(string))
                {
                    foreach (string strKey in dictModel.Keys)
                    {
                        if (Column.NameEquals(strKey, true))
                        {
                            dictModel[strKey] = ValueProc.AsType(objTypes[1], Value);
                            return true;
                        }
                    }
                    dictModel[Column.Name] = ValueProc.AsType(objTypes[1], Value);
                    return true;
                }
                else if (objTypes[0] == typeof(ColumnClip))
                {
                    foreach (ColumnClip colKey in dictModel.Keys)
                    {
                        if (Column.NameEquals(colKey))
                        {
                            dictModel[colKey] = ValueProc.AsType(objTypes[1], Value);
                            return true;
                        }
                    }
                    dictModel[Column] = ValueProc.AsType(objTypes[1], Value);
                    return true;
                }
            }


            WhereClip whereModel = objModel as WhereClip;

            if (whereModel != null)
            {

                var curWhere = whereModel;
                while (curWhere.IsNull() == false && dbo.EqualsNull(curWhere.Query) == false)
                {
                    if (curWhere.Query.NameEquals(Column.Name, true))
                    {
                        curWhere.Value = Value;
                        return true;
                    }


                    if (curWhere.Next.IsNull())
                    {
                        break;
                    }
                    else curWhere = curWhere.Next;
                }

                if (curWhere.IsNull())
                {
                    curWhere = curWhere.Next = new WhereClip();
                }
                curWhere.Linker = SqlOperator.And;
                curWhere.Query = Column;
                curWhere.Operator = SqlOperator.Equal;
                curWhere.Value = Value;
                return true;
            }

            dictModel = new XmlDictionary<string, object>();

            var entity = objModel as IEntity;
            if (entity != null)
            {
                entity.SetPropertyValue(Column.Name, Value);
            }
            else
            {
                FastInvoke.SetPropertyValue(objModel, type, type.GetMethod("set_" + Column.Name), Value);
            }
            //foreach (PropertyInfo item in type.GetProperties())
            //{
            //    if (item.Name == Column.Name)
            //    {
            //        FastInvoke.SetPropertyValue(objModel, Column.Name, Value);
            //        return true;
            //    }
            //}
            return false;
        }


        /// <summary>
        /// Reader 转为 字典 函数，核心函数。
        /// </summary>
        /// <param name="openReader"></param>
        /// <returns></returns>
        public static RowData ToDictionary(this DbDataReader openReader)
        {
            var dict = new RowData();

            for (int i = 0; i < openReader.FieldCount; i++)
            {
                if (openReader.GetName(i).StartsWith("__IgNoRe__")) continue;
                dict[openReader.GetName(i)] = openReader.GetValue(i);
            }
            return dict;
        }

        /// <summary>
        /// DataRow 转为 字典 函数，核心函数。
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static XmlDictionary<string, object> ToDictionary(this DataRow row)
        {
            XmlDictionary<string, object> dict = new XmlDictionary<string, object>();
            for (int i = 0; i < row.Table.Columns.Count; i++)
            {
                if (row.Table.Columns[i].ColumnName.StartsWith("__IgNoRe__")) continue;
                dict[row.Table.Columns[i].ColumnName] = row[i];
            }
            return dict;
        }


        public static object[] ToValueData(this DbDataReader openReader)
        {
            return ToValueData(openReader, null);
        }

        public static object[] ToValueData(this DbDataReader openReader, params string[] Columns)
        {
            if (Columns == null || Columns.Length == 0)
            {

                List<object> list = new List<object>();
                //如果第一列为行号。则忽略掉一列。

                for (int i = 0; i < openReader.VisibleFieldCount; i++)
                {
                    if (openReader.GetName(i).StartsWith("__IgNoRe__"))
                    {
                        continue;
                    }

                    list.Add(openReader.GetValue(i));
                }
                return list.ToArray();
            }
            else
            {
                List<object> list = new List<object>();
                foreach (var col in Columns)
                {
                    list.Add(openReader.GetValue(openReader.GetOrdinal(col)));
                }
                return list.ToArray();
            }
        }

        /// <summary>
        /// 把 DataTable 转为实体列表。由于DataTable 是一次装载,不对 DataRow 应用权限控制机制
        /// </summary>
        /// <param name="table"></param>
        /// <param name="NewModelFunc"></param>
        /// <returns></returns>
        public static List<T> ToEntityList<T>(this DataTable table, Func<T> NewModelFunc)
        {
            List<T> list = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                list.Add(ToEntity(row, NewModelFunc));
            }
            return list;
        }


        //public static List<TModel> ToEntityList<T,TModel>(this SelecteClip<T> select )
        //    where T:RuleBase
        //    where TModel:struct
        //{
        //    return select.ToEntityList(() => new TModel());
        //}

        /// <summary>
        /// 把 DataTable 转为实体列表。转换过程不应用权限过滤机制.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> ToEntityList<T>(this DataTable table) where T : new()
        {
            List<T> list = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                list.Add(ToEntity<T>(row));
            }
            return list;
        }

        /// <summary>
        /// 把 DataReader 转为实体。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="openReader"></param>
        /// <returns></returns>
        public static T ToEntity<T>(this DbDataReader openReader) where T : new()
        {
            return ToEntity(openReader, () => new T());
        }

        public static T ToEntity<T>(this DbDataReader openReader, T Model) where T : new()
        {
            return ToEntity(openReader, () => new T());
        }

        ///// <summary>
        ///// Reader 转为 字典 函数，核心函数之一。用权限过滤。
        ///// </summary>
        ///// <param name="openReader"></param>
        ///// <returns></returns>
        //public static XmlDictionary<string, object> ToDictionary(this DbDataReader openReader)
        //{
        //    return ToDictionary(openReader, true);
        //}

        /// <summary>
        /// 启用权限的写法。如果不启用权限请用 DictionaryToModel(ToDictionary(openReader,false), Model)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="openReader"></param>
        /// <param name="NewModelFunc"></param>
        /// <returns></returns>
        public static T ToEntity<T>(this DbDataReader openReader, Func<T> NewModelFunc)
        {
            return DictionaryToFuncModel(ToDictionary(openReader), NewModelFunc);
        }


        /// <summary>
        /// 把 DataRow 转为实体。由于DataTable 是一次装载,不对 DataRow 应用权限控制机制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T ToEntity<T>(this DataRow row) where T : new()
        {
            return (T)ToEntity(row, () => new T());
        }

        /// <summary>
        /// 把 DataRow 转为实体。由于DataTable 是一次装载,不对 DataRow 应用权限控制机制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="NewModelFunc">生成默认Model构造器的回调.</param>
        /// <returns></returns>
        public static T ToEntity<T>(this DataRow row, Func<T> NewModelFunc)
        {
            return DictionaryToFuncModel(ToDictionary(row), NewModelFunc);
        }

        public static string ToEntity(this DataRow row, string DefaultValue)
        {
            return DictionaryToFuncModel(ToDictionary(row), () => DefaultValue);
        }
        /// <summary>
        /// 把 DbName 转换为 程序 所使用的名称.
        /// </summary>
        /// <remarks>
        /// 如果数据库名称中出现以下 指定的分隔字符 , 则按以下分隔字符 进行分隔, 并把每个部分首字母大写之后进行联接. 分隔字符有:
        /// 1.空格
        /// 2.下划线 _
        /// 3.加号 +
        /// 4.减号 -
        /// 5.#
        /// 6.&amp;
        /// 7.竖线 |
        /// 8.冒号 :
        /// 9.分号 ;
        /// 10.小于号 &lt;
        /// 11.大于号 &gt;
        /// 12.逗号 , 
        /// 13.点号 .
        /// 14.$
        /// 15.左括号
        /// 16.右括号
        /// 
        /// 如果没有出现上述分隔符, 如果数据库名称 全大写,或全小写, 则按首字母大写转换, 否则 返回数据库名称.
        /// </remarks>
        /// <param name="DbName"></param>
        /// <returns></returns>
        public static string TranslateDbName(string DbName)
        {
            if (MyOqlConfigScope.Config != null && MyOqlConfigScope.Config.IsValueCreated && MyOqlConfigScope.Config.Value.Contains(ReConfigEnum.NoEscapeDbName)) return DbName;

            if (DbName.HasValue() == false) return DbName;

            //在Sqlserver中，列名可以以 # 开头，MyOql规定，以#开头的列名，不转义。
            if (DbName.StartsWith("#")) return DbName.Substring(1);

            var ents = DbName.Split(" _+-&|:;<>,.$(){}".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);


            Func<string, string> Format = o =>
            {
                if (o.HasValue() == false) return string.Empty;
                else if (o.Length == 1) return o.ToUpperInvariant();
                else if (o.IsSameCase()) return o.First().ToString().ToUpperInvariant() + o.Substring(1).ToLower();
                else return o.First().ToString().ToUpperInvariant() + o.Substring(1);
            };

            return string.Join("", ents.Select(o => Format(o)).ToArray());

        }

        /// <summary>
        /// 把一个 DataTable 转换为 MyOqlSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <param name="Rule"></param>
        /// <returns></returns>
        public static MyOqlSet ToMyOqlSet<T>(this DataTable dataTable, T Rule)
            where T : RuleBase
        {
            return new MyOqlSet(Rule).Load(dataTable);
        }


        public static MyOqlSet ToMyOqlSet<T>(this List<XmlDictionary<string, object>> dictList, T Rule)
            where T : RuleBase
        {
            return new MyOqlSet(Rule).Load(dictList);
        }
    }
}