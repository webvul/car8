using System;
using System.Threading;
using MyCmn;

namespace MyOql
{
    /// <summary>
    /// 设置当前线程的Myoql 配置
    /// </summary>
    /// <remarks>
    /// <example>
    /// 在以下代码中两个执行的操作，会跳过权限过滤。
    /// <code>
    ///     using ( var config = new MyOqlConfigScope( ReConfigEnum.SkipPower ) )
    ///     {
    ///         var ent =  dbr.Menu.FindById(12) ;
    ///         var usr = dbr.PLogin(ent.UserId , 'abc' ) ;
    ///     }
    /// </code>
    /// </example>
    /// </remarks>
    public class MyOqlConfigScope : IDisposable
    {
        private static ThreadLocal<ReConfigEnum> _Config { get; set; }
        private static ThreadLocal<ReConfigEnum> _Sync_Config = new ThreadLocal<ReConfigEnum>();

        public static ThreadLocal<ReConfigEnum> Config
        {
            get
            {
                if (_Config == null || !_Config.IsValueCreated)
                {
                    lock (_Sync_Config)
                    {
                        if (_Config == null || !_Config.IsValueCreated)
                        {
                            if (dbo.DefaultMyOqlConfig.HasValue())
                            {
                                _Config = new ThreadLocal<ReConfigEnum>() { Value = dbo.DefaultMyOqlConfig };
                            }
                        }
                    }
                }
                return _Config;
            }
            set
            {
                _Config = value;
            }
        }

        public MyOqlConfigScope(ReConfigEnum config)
        {
            if (config.Contains(ReConfigEnum.ReadPast) == false &&
                config.Contains(ReConfigEnum.NoLock) == false &&
                config.Contains(ReConfigEnum.WaitLock) == false)
            {
                config |= dbo.DefaultMyOqlConfig;
            }

            _Config = new ThreadLocal<ReConfigEnum>()
            {
                Value = config
            };

        }

        public void Dispose()
        {
            _Config = null;
        }
    }
}
