using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Reflection;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using Microsoft.Win32;

namespace MyCmn
{
    /// <summary>
    /// 针对一些Web应用常用的处理助手函数.
    /// </summary>
    public static partial class CmnProc
    {
        /// <summary>
        /// 配置是否是Js，Css压缩输出。
        /// </summary>
        /// <returns></returns>
        public static bool IsConfigJsCssCompressed()
        {
            return false;
            //return CacheHelper.Get("__MyCmn_IsConfigJsCssCompressed__", new TimeSpan(2, 0, 0), delegate()
            //{
            //    return ConfigurationManager.AppSettings["JsCssComprese"].AsBool();
            //    //System.Configuration.Configuration configuration = WebConfigurationManager.OpenWebConfiguration(
            //    //    HttpContext.Current.Request.ApplicationPath
            //    //    );

            //    //SystemWebSectionGroup systemWeb = (SystemWebSectionGroup)configuration.GetSectionGroup("system.web");

            //    //return systemWeb.Compilation.Debug;
            //});
        }
        /// <summary>
        /// 通过访问注册表，得到指定类型的 ContentType 。 [★]
        /// </summary>
        /// <param name="strSuffix"></param>
        /// <returns></returns>
        public static string GetContentType(string strSuffix)
        {
            string strResult = string.Empty;
            try
            {
                RegistryKey key = Registry.LocalMachine;
                key = key.OpenSubKey("SOFTWARE\\Classes\\" + strSuffix);

                if (key != null)
                    strResult = key.GetValue("Content Type", "").ToString();
            }
            catch
            {
            }
            return strResult;
        }

        public static StringDict ToStringDict(this NameValueCollection NameValueCollectionAsDataSource)
        {
            var retVal = new StringDict();
            foreach (string key in NameValueCollectionAsDataSource.AllKeys)
            {
                retVal.Add(key, NameValueCollectionAsDataSource[key]);
            }

            return retVal;
        }


        /// <summary>
        /// 将 NameValueCollection 转换成 字典 的形式 [★]
        /// </summary>
        /// <param name="NameValueCollectionAsDataSource"></param>
        /// <returns></returns>
        public static XmlDictionary<string, T> ToDictionary<T>(this NameValueCollection NameValueCollectionAsDataSource)
        {
            XmlDictionary<string, T> retVal = new XmlDictionary<string, T>();
            foreach (string key in NameValueCollectionAsDataSource.AllKeys)
            {
                retVal.Add(key, (T)(object)NameValueCollectionAsDataSource[key]);
            }

            return retVal;
        }

        public static XmlDictionary<string, object> ToDictionary(this NameValueCollection NameValueCollectionAsDataSource)
        {
            return ToDictionary<object>(NameValueCollectionAsDataSource);
        }
        //public static T[] ToMyArray<T>(this IEnumerable<T> Value)
        //{
        //    if (Value == null) return null;

        //    List<T> list = new List<T>();

        //    IEnumerator<T> enr = Value.GetEnumerator();

        //    while (enr.MoveNext())
        //    {
        //        list.Add( enr.Current);
        //    }

        //    return list.ToArray();
        //}


        /// <summary>
        /// 得到 指定重复次数的 string 的表达式。 [★]
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="RepeatCount"></param>
        /// <returns></returns>
        public static string GetRepeatString(this string Value, int RepeatCount)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < RepeatCount; i++)
            {
                sb.Append(Value);
            }
            return sb.ToString();
        }


        ///// <summary>
        ///// 把 ICollection 转换为 List . 兼容模式. [★] .
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="TheCollection"></param>
        ///// <returns></returns>
        //[Obsolete("这一个，要明确用法。")]
        //public static List<T> ToMyList<T>(this ICollection TheCollection) 
        //{
        //    List<T> retVal = new List<T>();
        //    var ienum = TheCollection.GetEnumerator();
        //    while (ienum.MoveNext())
        //    {
        //        retVal.Add( ValueProc.Get<T>(ienum.Current ) ) ;
        //    }
        //    return retVal;
        //}

        /// <summary>
        /// 检测 DataSet 是否包含有效数据。 [★]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool DataTableHasData(this DataTable data)
        {
            if (data == null)
                return false;
            if (data.Rows == null)
                return false;
            if (data.Rows.Count == 0)
                return false;
            return true;
        }

        /// <summary>
        /// 检测 DataSet 是否包含有效数据。 [★]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool DataSetHasData(this DataSet data)
        {
            if (data == null)
                return false;
            if (data.Tables == null)
                return false;
            if (data.Tables.Count == 0)
                return false;
            return data.Tables[0].DataTableHasData();
        }

        /// <summary>
        /// 比较两个 Array 的值 是否相等。 [★]
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static bool ArrayIsEqual(this Array First, Array Second)
        {
            if (First == null && Second == null)
                return true;
            if (First == null || Second == null)
                return false;
            if (First.Length != Second.Length)
                return false;

            for (int i = 0; i < First.Length; i++)
            {
                if (First.GetValue(i).Equals(Second.GetValue(i)) == false)
                {
                    return false;
                }
            }
            return true;
        }



        /// <summary>
        /// 兼容 Cs,Bs , 获取路径.
        /// </summary>
        /// <remarks>
        /// 如果是 Bs , 获取相对服务器根位置的物理路径
        /// 如果是 Cs , 获取相对运行程序位置的物理路径.
        /// </remarks>
        /// <param name="myPath"></param>
        /// <returns></returns>
        [Obsolete("请使用 myPath.ResolveUrl()", true)]
        public static string GetPath(string myPath)
        {
            if (HttpContext.Current != null)
            {
                if (myPath.StartsWith("~/") || myPath.StartsWith("/") || myPath.StartsWith("../"))
                {
                    return HttpContext.Current.Server.MapPath(myPath);
                }
                else return myPath;
            }
            else if (myPath.StartsWith("~/"))
            {
                return new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName + Path.DirectorySeparatorChar.ToString() + myPath.Substring(2);
            }
            else if (myPath.IndexOf(":") > 0) return myPath;
            else
            {
                return new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName + Path.DirectorySeparatorChar.ToString() + myPath;
            }
        }

        /// <summary>
        /// 如果文件不存在,则创建文件,如果文件存在,则返回.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns>返回第一次是否成功摸到文件. 如果文件存在,返回true,否则返回false</returns>
        public static bool Touch(this FileInfo fileInfo)
        {
            if (fileInfo.Exists) return true;
            else
            {
                if (fileInfo.Directory.Exists == false)
                {
                    fileInfo.Directory.Create();
                }

                using (var fs = fileInfo.Create())
                {
                    fs.Close();
                }
            }
            return false;
        }
    }
}
