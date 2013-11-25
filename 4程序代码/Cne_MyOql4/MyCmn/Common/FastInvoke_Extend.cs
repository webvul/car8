using System;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


namespace MyCmn
{
    //需继续添加构造函数调用。
    /*
 * MethodInfo methodInfo = Typeof(Person).GetMethod("get_Txt");
 * object[] param = new object[] {  };
 * FastInvoke.FastInvokeHandler fastInvoker = FastInvoke.GetMethodInvoker(methodInfo);
 * fastInvoker(person, param);
 */
    /// <summary>
    /// 通过Emit方式操作对象.
    /// </summary>
    public static partial class FastInvoke
    {

        /// <summary>
        /// 把 Model 转为 字典，是一个和  ModelToDictionary(RuleBase Entity, IModel objModel) 相同算法的函数。
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        public static StringDict Model2StringDict(object objModel)
        {
            if (objModel == null) return null;

            StringDict dictModel = objModel as StringDict;
            if (dictModel != null)
            {
                return dictModel;
            }


            IDictionary dict = objModel as IDictionary;
            if (dict != null)
            {
                dictModel = new StringDict();

                foreach (var strKey in dict.Keys)
                {
                    dictModel.Add(strKey.AsString(), dict[strKey].AsString(null));
                }
                return dictModel;
            }

            dictModel = new StringDict();

            var entity = objModel as IReadEntity;
            if (entity != null)
            {
                foreach (var strKey in entity.GetProperties())
                {
                    if (strKey == null) continue;
                    dictModel.Add(strKey, entity.GetPropertyValue(strKey).AsString(null));
                }
                return dictModel;
            }


            Type type = objModel.GetType();

            GodError.Check(type.FullName == "MyOql.ColumnClip", "GodError", "ColumnClip 不能作为 Model", objModel.ToString());
            GodError.Check(type.IsPrimitive,"GodError", "基元类型不能做为 Model", objModel.ToString());

            foreach (PropertyInfo prop in type.GetProperties())
            {
                var methodInfo = type.GetMethod("get_" + prop.Name);
                if (methodInfo == null) continue;

                dictModel.Add(prop.Name, FastInvoke.GetPropertyValue(objModel, type, methodInfo).AsString(null));
            }

            return dictModel;
        }



        /// <summary>
        /// 把字典解析到 Model 类型的 Model 上。
        /// <remarks>
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
        public static T StringDict2Model<T>(this StringDict Dict, Func<T> NewModelFunc)
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

            //当是结构体时，只返回第一个列。如 Int。
            if (type.IsValueType && type.IsSimpleType())
            {
                return ValueProc.As<T>(Dict.ElementAt(0).Value);
            }

            if (type.FullName == "System.String" || type.FullName == "System.Text.StringBuilder" || type.FullName == "MyCmn.StringLinker")
            {
                return ValueProc.As<T>(Dict.ElementAt(0).Value);
            }

            else if (type.IsClass)
            {
                if (object.Equals(ret, default(T)))
                {
                    ret = NewModelFunc();
                }

                var retDict = ret as IDictionary;

                if (retDict != null)
                {
                    var genericTypes = type.getGenericType().GetGenericArguments();
                    if (genericTypes.Length == 2)
                    {
                        foreach (var kv in Dict)
                        {
                            retDict[ValueProc.AsType(genericTypes[0], kv.Key)] = ValueProc.AsType(genericTypes[1], kv.Value);
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

                var entity = ret as IEntity;

                if (entity != null)
                {
                    foreach (var prop in entity.GetProperties())
                    {
                        if (Dict.Keys.Select(o => o.AsString()).Contains(prop))
                        {
                            entity.SetPropertyValue(prop, Dict.First(d => d.Key.AsString() == prop).Value);
                        }
                    }
                }
                else
                {
                    Dict2DeepObj(Dict, ret, type);
                }
                return ret;
            }

            throw new GodError("不支持的Model类型！");
        }

        /// <summary>
        /// 可以对子对象赋值。
        /// </summary>
        /// <remarks>
        /// 子对象赋值： Key 如： book.Id 形式，则是给 model 的 book 对象赋 Id 值。
        /// model对象在默认构造函数时，应对 book 子对象初始化。
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dict"></param>
        /// <param name="TypeOfT"></param>
        /// <param name="model"></param>
        public static void Dict2DeepObj<T>(IDictionary Dict, T model, Type TypeOfT)
        {
            List<string> LeftKeys = new List<string>();
            foreach (var key in Dict.Keys)
            {
                if (key.HasValue() == false) continue;
                var val = Dict[key];
                if (val.IsDBNull()) continue;
                var strKey = key.AsString();

                if (strKey.Contains('.'))
                {
                    LeftKeys.Add(strKey);
                    continue;
                }

                var methodInfo = TypeOfT.GetMethod("set_" + strKey);
                if (methodInfo == null)
                {
                    continue;
                }

                FastInvoke.SetPropertyValue(model, TypeOfT, methodInfo, val);
            }

            Func<object, Type, string[], int, object> _FindBottomObj = null,
                FindBottomObj = (obj, type, Keys, currentLevel) =>
                {
                    if (obj.IsDBNull()) return null;
                    if (type == null) type = obj.GetType();

                    var curKey = Keys[currentLevel];
                    var methodInfo = type.GetMethod("get_" + curKey);
                    if (methodInfo == null) return null;

                    object curObj = FastInvoke.GetPropertyValue(obj, type, methodInfo);
                    if (curObj.IsDBNull())
                    {
                        //自动创建对象。
                        var setMethodInfo = type.GetMethod("set_" + curKey);
                        if (setMethodInfo == null) return null;

                        curObj = Activator.CreateInstance(methodInfo.ReturnType);
                        if (curObj.IsDBNull()) return null;

                        FastInvoke.SetPropertyValue(obj, type, setMethodInfo, curObj);
                    }


                    if (currentLevel < Keys.Length - 2)
                    {
                        return _FindBottomObj(curObj, null, Keys, currentLevel + 1);
                    }
                    else return curObj;
                };
            _FindBottomObj = FindBottomObj;

            if (LeftKeys.Count > 0)
            {
                foreach (var key in LeftKeys)
                {
                    var keys = key.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    var lastKey = keys.LastOrDefault();
                    if (lastKey.HasValue() == false) continue;
                    var obj = FindBottomObj(model, TypeOfT, keys.Slice(0, -1).ToArray(), 0);
                    if (obj.IsDBNull()) continue;

                    var objType = obj.GetType();
                    var methodInfo = objType.GetMethod("set_" + lastKey);
                    if (methodInfo == null) continue;


                    object oriKey = null;

                    foreach (var item in Dict.Keys)
                    {
                        if (item.AsString() == key) { oriKey = item; break; }
                    }

                    FastInvoke.SetPropertyValue(obj, objType, methodInfo, Dict[oriKey]);
                }
            }
        }

    }
}