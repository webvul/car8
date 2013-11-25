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
        public static object GetPropertyValue(object obj, Type typeOfT, string PropertyName)
        {
            if (object.Equals(obj, null)) return false;
            if (typeOfT == null || typeOfT.FullName == "System.Object")
            {
                typeOfT = obj.GetType();
            }
            var methodInfo = typeOfT.GetMethod("get_" + PropertyName);
            return FastInvoke.GetMethodInvoker(methodInfo).Invoke(obj, new object[] { });
        }

        public static object SetPropertyValue(object obj, Type typeOfT, string PropertyName, object Value)
        {
            if (typeOfT == null || typeOfT.FullName == "System.Object")
            {
                typeOfT = obj.GetType();
            }

            var methodInfo = typeOfT.GetMethod("set_" + PropertyName);
            var paraType = methodInfo.GetParameters().First().ParameterType;
            if (paraType.IsEnum && !Value.AsString().HasValue())
            {
                return false;
            }

            FastInvoke.GetMethodInvoker(methodInfo).Invoke(obj, new object[] { ValueProc.AsType(paraType, Value) });
            return true;
        }

        /// <summary>
        /// 获取属性值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">调用对象。</param>
        /// <param name="typeOfT">如果为空或是 Object ， 则调用 obj.GetType 。传入该值是为了性能考虑。</param>
        /// <param name="methodInfo">对于属性取值来说，应该是 get_Property 方法。</param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(T obj, Type typeOfT, MethodInfo methodInfo)
        {
            GodError.Check(methodInfo == null, "方法信息不能为空，对于属性取值的方法来说是 get_Property 方法的方法信息。");
            if (object.Equals(obj, null)) return false;
            if (typeOfT == null || typeOfT.FullName == "System.Object")
            {
                typeOfT = obj.GetType();
            }
            return FastInvoke.GetMethodInvoker(methodInfo).Invoke(obj, new object[] { });
        }

        /// <summary>
        /// 设置属性值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="typeOfT">如果为空或是 Object ， 则调用 obj.GetType 。传入该值是为了性能考虑。</param>
        /// <param name="methodInfo">对于属性赋值来说，应该是 set_Property 方法。</param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool SetPropertyValue<T>(T obj, Type typeOfT, MethodInfo methodInfo, object Value)
        {
            GodError.Check(methodInfo == null,   "方法信息不能为空，对于属性赋值的方法来说是 set_Property 方法的方法信息。");
            if (object.Equals(obj, null)) return false;

            if (typeOfT == null || typeOfT.FullName == "System.Object")
            {
                typeOfT = obj.GetType();
            }

            var paraType = methodInfo.GetParameters().First().ParameterType;
            if (paraType.IsEnum && !Value.AsString().HasValue())
            {
                return false;
            }

            FastInvoke.GetMethodInvoker(methodInfo).Invoke(obj, new object[] { ValueProc.AsType(paraType, Value) });
            return true;
        }

        /// <summary>
        /// Emit动态生成的内部调用方法.
        /// </summary>
        /// <param name="target">目标对象.</param>
        /// <param name="paramters">方法参数</param>
        /// <returns>返回值.</returns>
        public delegate object FastInvokeHandler(object target, object[] paramters);

        //static object InvokeMethod(FastInvokeHandler invoke, object target, params object[] paramters)
        //{
        //    return invoke(null, paramters);
        //}

        public static FastInvokeHandler GetMethodInvoker(MethodInfo methodInfo)
        {
            //return CacheHelper.Get("FastInvoke" + methodInfo.Module.ModuleVersionId + "_" + methodInfo.MetadataToken, new TimeSpan(0, 0, 45), () =>
            //    {
            DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType.Module);
            ILGenerator il = dynamicMethod.GetILGenerator();
            ParameterInfo[] ps = methodInfo.GetParameters();
            Type[] paramTypes = new Type[ps.Length];
            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                    paramTypes[i] = ps[i].ParameterType.GetElementType();
                else
                    paramTypes[i] = ps[i].ParameterType;
            }
            LocalBuilder[] locals = new LocalBuilder[paramTypes.Length];

            for (int i = 0; i < paramTypes.Length; i++)
            {
                locals[i] = il.DeclareLocal(paramTypes[i], true);
            }
            for (int i = 0; i < paramTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_1);
                EmitFastInt(il, i);
                il.Emit(OpCodes.Ldelem_Ref);
                EmitCastToReference(il, paramTypes[i]);
                il.Emit(OpCodes.Stloc, locals[i]);
            }
            if (!methodInfo.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
            }
            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                    il.Emit(OpCodes.Ldloca_S, locals[i]);
                else
                    il.Emit(OpCodes.Ldloc, locals[i]);
            }
            if (methodInfo.IsStatic)
                il.EmitCall(OpCodes.Call, methodInfo, null);
            else
                il.EmitCall(OpCodes.Callvirt, methodInfo, null);
            if (methodInfo.ReturnType == typeof(void))
                il.Emit(OpCodes.Ldnull);
            else
                EmitBoxIfNeeded(il, methodInfo.ReturnType);

            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    EmitFastInt(il, i);
                    il.Emit(OpCodes.Ldloc, locals[i]);
                    if (locals[i].LocalType.IsValueType)
                        il.Emit(OpCodes.Box, locals[i].LocalType);
                    il.Emit(OpCodes.Stelem_Ref);
                }
            }

            il.Emit(OpCodes.Ret);
            FastInvokeHandler invoder = (FastInvokeHandler)dynamicMethod.CreateDelegate(typeof(FastInvokeHandler));
            return invoder;
            //});
        }

        private static void EmitCastToReference(ILGenerator il, System.Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }

        private static void EmitBoxIfNeeded(ILGenerator il, System.Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Box, type);
            }
        }

        private static void EmitFastInt(ILGenerator il, int value)
        {
            switch (value)
            {
                case -1:
                    il.Emit(OpCodes.Ldc_I4_M1);
                    return;
                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    return;
            }

            if (value > -129 && value < 128)
            {
                il.Emit(OpCodes.Ldc_I4_S, (SByte)value);
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4, value);
            }
        }
    }
}