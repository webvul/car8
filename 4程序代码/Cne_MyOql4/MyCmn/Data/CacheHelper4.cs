using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using System.Configuration;


namespace MyCmn
{
    public interface ICache
    {
        void Add(string key, object Data, TimeSpan Time);

        bool IsExists(string key);

        void Remove(string Key);

        IEnumerable<string> GetKeys();

        object Get(string key);
    }

    internal class RuntimeCache : ICache
    {
        public void Add(string key, object Data, TimeSpan Time)
        {
            if (Data == null)
                return;

            CacheDependency FileDependency = new CacheDependency(CacheHelper.WebConfigPath);

            HttpRuntime.Cache.Add(key, Data,
                FileDependency,
                System.Web.Caching.Cache.NoAbsoluteExpiration,
                Time,
                CacheItemPriority.Default,
                null
                );
        }


        public bool IsExists(string key)
        {
            GodError.Check(key.HasValue() == false, "CacheKey 不能为空");

            return HttpRuntime.Cache[key] != null;
        }


        public void Remove(string Key)
        {
            HttpRuntime.Cache.Remove(Key);
        }


        public IEnumerable<string> GetKeys()
        {
            var enm = HttpRuntime.Cache.GetEnumerator();
            while (enm.MoveNext())
            {
                yield return enm.Key.ToString();
            }
        }

        public object Get(string key)
        {
            return HttpRuntime.Cache[key];
        }
    }


    /// <summary>
    /// 缓存管理器，用于 缓存 服务器 IIS 端数据。使用的是 HttpRuntime.Cache类. 仅 .Net 4.0 支持.
    /// </summary>
    /// <example>
    /// CacheHelper.Get 的作用是指当数据项失效后， 调用指定的委托来添加数据项，以保证该项缓存的持续性。
    /// 该示例用来演示最简的 代码风格， 描述 取出在缓存中维护数据。
    /// <code>
    /// var retVal = CacheHelper.Get(CacheKey.OrgUserID, delegate() {
    ///        var htState = new List&lt; string>();
    ///        htState.Add("sys");
    ///        htState.Add("org");
    ///        htState.Add("user");
    ///        htState.Add("none");
    ///        htState.Add("web");
    ///        return htState ;
    ///    });
    /// return retVal ;
    /// </code>
    /// </example>
    public static class CacheHelper
    {
        private static string _WebConfigPath;
        public static string WebConfigPath
        {
            get
            {
                if (_WebConfigPath == null)
                {
                    lock (Sync)
                    {
                        if (_WebConfigPath == null)
                        {
                            if (HttpContext.Current != null)
                            {
                                _WebConfigPath = HttpContext.Current.Server.MapPath("~/Web.config");
                            }
                            else
                            {
                                _WebConfigPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
                            }
                        }

                    }
                }
                return _WebConfigPath;
            }
        }


        private static object Sync = new object();
        private static ICache _Cache = null;
        private static ICache GetCacheEvent()
        {
            if (_Cache == null)
            {
                lock (Sync)
                {
                    if (_Cache == null)
                    {
                        var logEvent = ConfigurationManager.AppSettings["CacheEvent"];
                        if (logEvent.HasValue())
                        {
                            var type = Type.GetType(logEvent);
                            if (type != null)
                            {
                                _Cache = Activator.CreateInstance(type) as ICache;
                                return _Cache;
                            }
                        }
                    }

                    if (_Cache == null)
                    {
                        _Cache = new RuntimeCache();
                    }
                }
            }
            return _Cache;
        }

        //public static void Add<T>(string key, TimeSpan time, T Value)
        //{
        //    Get(key, time, () => Value);
        //}
        /// <summary>
        /// 如果该项先前存储在 Cache 中，则为 Object，否则为 null。
        /// </summary>
        /// <param name="key">缓存 Key，可以自已定义， 但是 Key 的 String 在 缓存管理器里不能有重复。</param>
        /// <param name="Data">要缓存到 Key 里的 数据。</param>
        /// <param name="Time">要缓存的时间</param>
        /// <returns>返回 添加的 Data </returns>
        private static void Add(string key, object Data, TimeSpan Time)
        {
            GetCacheEvent().Add(key, Data, Time);
        }


        /// <summary>
        /// 判断是否存在指定的 Key 的缓存项
        /// </summary>
        /// <param name="key">缓存 Key，可以自已定义， 但是 Key 的 String 在 缓存管理器里不能有重复。</param>
        /// <returns>布尔类型的值 ，存在返回 true ， 不存在返回 false 。</returns>
        public static bool IsExists(string key)
        {
            return GetCacheEvent().IsExists(key);
        }

        /// <summary>
        /// 从 Cache 移除的项。如果未找到键参数中的值，则返回 null
        /// </summary>
        /// <param name="Key"></param>
        /// <returns>从 Cache 移除的项。如果未找到键参数中的值，则返回 null </returns>
        public static void Remove(Enum Key)
        {
            Remove(Key.ToString());
        }

        public static void Remove(string Key)
        {
            GetCacheEvent().Remove(Key);
        }

        public static IEnumerable<string> GetKeys()
        {
            return GetCacheEvent().GetKeys();
        }


        public static void Remove(Func<string, bool> KeyFunc)
        {
            if (KeyFunc != null)
            {
                var enm = GetKeys();
                List<string> removeKeys = new List<string>();
                foreach (var key in GetKeys())
                {
                    if (KeyFunc(key))
                    {
                        Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// 用最简单的方式来取一个缓存的值 。
        /// </summary>
        /// <typeparam name="T">缓存值的类型。</typeparam>
        /// <param name="key">缓存的 Key</param>
        /// <returns>得到的缓存值。</returns>
        public static T Get<T>(Enum key)
        {
            return Get<T>(key.ToString());
        }

        /// <summary>
        /// 用最简单的方式来取一个缓存的值 。
        /// </summary>
        /// <typeparam name="T">缓存值的类型。</typeparam>
        /// <param name="key">缓存的 Key</param>
        /// <returns>得到的缓存值。</returns>
        public static T Get<T>(string key)
        {
            return (T)GetCacheEvent().Get(key);
        }



        /// <summary>
        /// 该方法用于取 指定 Key 的缓存数据项.<see cref="CacheHelper" />
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <typeparam name="T">缓存数据项的类型</typeparam>
        /// <param name="CacheKey">要缓存的 Key</param>
        /// <param name="time"></param>
        /// <param name="CachSet">当该数据项失效后， 要调用的用来添加数据项委托</param>
        /// <returns>要取的 缓存 Key 的数据</returns>
        public static T Get<T>(Enum CacheKey, TimeSpan time, Func<T> CachSet)
        {
            return Get<T>(CacheKey.ToString(), time, CachSet);
        }

        /// <summary>
        /// 该方法用于取 指定 Key 的缓存数据项.<see cref="CacheHelper" />
        /// </summary>
        /// <typeparam name="T">缓存数据项的类型</typeparam>
        /// <param name="CacheKey">要缓存的 Key</param>
        /// <param name="time">缓存时间</param>
        /// <param name="CachSet">当该数据项失效后， 要调用的用来添加数据项委托</param>
        /// <returns>要取的 缓存 Key 的数据</returns>
        public static T Get<T>(string CacheKey, TimeSpan time, Func<T> CachSet)
        {
            T retVal = default(T);

            if (IsExists(CacheKey) == false)
            {
                retVal = CachSet.Invoke();
                Add(CacheKey, retVal, time);
                return retVal;
            }
            else return Get<T>(CacheKey);
        }
    }
}
