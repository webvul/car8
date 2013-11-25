using System;
using System.Data;
using System.Data.Common;
using MyCmn;
using System.Collections.Generic;
using System.Threading;

namespace MyOql
{
    public static partial class dbo
    {
        #region 扩展 权限,日志,缓存
        ///// <summary>
        ///// 强制使用权限控制机制.返回值会克隆一份返回以保证传入值不会改变
        ///// </summary>
        ///// <typeparam name="T">RuleBase</typeparam>
        ///// <param name="obj"></param>
        ///// <param name="UsePower"></param>
        ///// <returns></returns>
        //public static T UsePower<T>(this T obj, MyOqlActionEnum UsePower) where T : RuleBase
        //{
        //    T ret = (T)obj.Clone();
        //    ret.SetUsePower(UsePower);
        //    return ret;
        //}

        /// <summary>
        /// 强制忽略权限控制机制.返回值会克隆一份返回以保证传入值不会改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T SkipPower<T>(this T obj) where T : ContextClipBase
        {
            obj.AddConfig(ReConfigEnum.SkipPower);
            return obj;
        }

        ///// <summary>
        ///// 强制使用日志记录.返回值会克隆一份返回以保证传入值不会改变
        ///// </summary>
        ///// <typeparam name="T">RuleBase</typeparam>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public static T UseLog<T>(this T obj) where T : RuleBase
        //{
        //    T ret = (T)obj.Clone();
        //    ret.SetExtend(MyOqlExtendEnum.Log, true);
        //    return ret;
        //}

        /// <summary>
        /// 强制忽略日志记录.返回值会克隆一份返回以保证传入值不会改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T SkipLog<T>(this T obj) where T : ContextClipBase
        {
            obj.AddConfig(ReConfigEnum.SkipLog);
            return obj;
        }

        //public static T SetExtend<T>(this T obj, MyOqlExtendEnum extend, object Value) where T : RuleBase
        //{
        //    T ret = (T)obj.Clone();
        //    ret.SetExtend(extend, false);
        //    return ret;
        //}


        /// <summary>
        /// 强制忽略缓存策略.返回值会克隆一份返回以保证传入值不会改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T SkipCache<T>(this T obj) where T : ContextClipBase
        {
            obj.AddConfig(ReConfigEnum.SkipCache);
            return obj;
        }

        #endregion

        /// <summary>
        /// 尝试打开数据连接,并执行相应方法
        /// 该方法是安全的.保证使用完毕后自动关闭连接.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myCmd"></param>
        /// <param name="Context">如果上下文中存在事务, 则忽略连接</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T Open<T>(MyCommand myCmd, ContextClipBase Context, Func<T> func)
        {
            if (Context.Transaction != null &&
                Context.Transaction.Connection != null)
            {
                return func();
            }
            else if (Context.Connection != null)
            {
                return func();
            }
            else
            {
                GodError.Check(myCmd == null, "MyCommand 不能为 null");
                GodError.Check(myCmd.Command == null, "MyCommand 的 Command 不能为 null");
                GodError.Check(myCmd.Command.Connection == null, "MyCommand 的 Connection 不能为 null");

                var conn = myCmd.Command.Connection;

                return Open<T>(conn, func);

            }
        }

        public static T Open<T>(DbConnection conn, Func<T> func)
        {
            if (conn == null) return default(T);
            if (conn.ConnectionString.HasValue() == false) return default(T);
            var connString = conn.ConnectionString;
            T retVal = default(T);

            using (conn)
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                retVal = func();
            }

            //OleDb 在执行完之后, 连接字符串会丢失.
            conn.ConnectionString = connString;

            return retVal;
        }

        public static List<SqlOperator> Polymer { get; set; }
        public static Dictionary<SqlOperator, Func<DatabaseType, string>> MyFunction { get; set; }


        /// <summary>
        /// 值只能是 ReadPast,NoLock,WaitLock之一。
        /// </summary>
        public static ReConfigEnum DefaultMyOqlConfig { get; set; }


        public static string GetFullSql(this DbCommand Command)
        {
            string fullSql = Command.CommandText;
            if (fullSql.HasValue() == false) return fullSql;

            foreach (DbParameter item in Command.Parameters)
            {
                var val = item.Value.AsString();
                if (item.DbType.DbTypeIsNumber() == false || item.DbType == DbType.Boolean)
                {
                    val = "'" + val.Replace("'", "''") + "'";
                }

                var index = fullSql.IndexOf(item.ParameterName, 0, StringComparison.Ordinal,
                    c =>
                    {
                        if (c == ' ') return true;
                        if (char.IsLetterOrDigit(c)) return false;
                        if (c == '_') return false;
                        return true;
                    },
                    c =>
                    {
                        if (c == ' ') return true;
                        if (char.IsLetterOrDigit(c)) return false;
                        if (c == '_') return false;
                        return true;
                    });

                GodError.Check(index < 0, "在" + fullSql + "中找不到:" + item.ParameterName);

                fullSql = fullSql.Remove(index, item.ParameterName.Length).Insert(index, val);
            }
            return fullSql;
        }


        /// <summary>
        /// TF_Fees_{Comm}_{User}   TF_Fees_1001_01
        /// </summary>
        /// <remarks>
        /// DbName 中不能出现 { } 
        /// </remarks>
        /// <param name="tmpName"></param>
        /// <param name="DbName"></param>
        /// <returns></returns>
        public static StringDict GetVarTableVars(string tmpName, string DbName)
        {
            var ret = new StringDict();
            if (string.IsNullOrEmpty(tmpName) || string.IsNullOrEmpty(DbName)) return ret;

            int i = 0, j = 0;
            for (; i < tmpName.Length && j < DbName.Length; i++, j++)
            {
                if (tmpName[i] != DbName[j])
                {
                    if (tmpName[i] == '{' && DbName[j] == '{')
                    {
                        var endIndex = tmpName.IndexOf('}', i);
                        if (endIndex < 0) return null;

                        var key = tmpName.Substring(i + 1, endIndex - i - 1);

                        //到头了。
                        if (endIndex == tmpName.Length - 1)
                        {
                            var val = DbName.Substring(j + 1);
                            ret[key] = val;
                            break;
                        }
                        else
                        {
                            //没到头，后面还有。
                            var nextChar = tmpName[endIndex + 1];
                            var valEndIndex = DbName.IndexOf(nextChar, j);
                            if (valEndIndex < 0) return null;

                            var val = DbName.Substring(j + 1, valEndIndex - j - 1);

                            ret[key] = val;

                            //接着比
                            i = endIndex;
                            j = valEndIndex;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return ret;
        }

        public static string[] GetVarTableVars(string tmpName)
        {
            if (string.IsNullOrEmpty(tmpName)) return new string[0];

            var ret = new List<string>();
            for (int i = 0; i < tmpName.Length; i++)
            {

                if (tmpName[i] == '{')
                {
                    var endIndex = tmpName.IndexOf('}', i);
                    if (endIndex < 0) return null;

                    var key = tmpName.Substring(i + 1, endIndex - i - 1);

                    ret.Add(key);

                    i = endIndex;

                }
            }
            return ret.ToArray();
        }
    }
}
