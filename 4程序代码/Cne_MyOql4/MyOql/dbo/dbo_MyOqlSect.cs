using System;
using System.Configuration;
using MyCmn;
using MyOql.Provider;
using System.Data.Common;

namespace MyOql
{

    public partial class dbo
    {
        private static MyOqlConfigSect _MyOqlSet = null;

        public static MyOqlConfigSect MyOqlSect
        {
            get
            {
                if (_MyOqlSet == null)
                {
                    _MyOqlSet = (MyOqlConfigSect)ConfigurationManager.GetSection("MyOql");
                }

                dbo.Check(_MyOqlSet == null, "不能取得配置文件中的 MyOql 节点， 请检查配置文件！", null);
                return _MyOqlSet;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="DatabaseName"></param>
        /// <returns></returns>
        public static string GetProviderType(string DatabaseName)
        {
            if (MyOqlSect == null) return "MyOql.Provider.SqlServer,MyOql";
            var set = MyOqlSect.DbProviders.GetEnumerator();
            while (set.MoveNext())
            {
                var sect = set.Current as MyOql.MyOqlConfigSect.ProviderCollection.ProviderElement;
                if (sect.Name == DatabaseName)
                {
                    if (sect.Type.HasValue()) return sect.Type;
                    else break;
                }
            }
            throw new GodError(string.Format(@"未配置MyOql的 {0} 数据解析项", DatabaseName));
        }

        private static ConnectionStringSettingsCollection _Connections = null;
        internal static ConnectionStringSettingsCollection Connections
        {
            get
            {
                if (_Connections == null)
                {
                    _Connections = ConfigurationManager.ConnectionStrings;
                }
                return _Connections;
            }
        }

        public static string GetDbConnString(EntityName Entity)
        {
            return Connections[MyOqlSect.Entitys.GetConfig(Entity).db /* GetDbConfigName(Entity)*/].ConnectionString;
        }


        private static readonly object _syncObject = new object();

        private static XmlDictionary<DatabaseType, TranslateSql> _Provider = new XmlDictionary<DatabaseType, TranslateSql>();
        public static TranslateSql GetDbProvider(DatabaseType dbType)
        {
            if (_Provider.ContainsKey(dbType) == false)
            {
                lock (_syncObject)
                {
                    if (_Provider.ContainsKey(dbType) == false)
                    {
                        string ptype = dbo.GetProviderType(dbType.ToString());
                        if (_Provider.ContainsKey(dbType) == false)
                        {
                            _Provider.Add(dbType, Activator.CreateInstance(System.Web.Compilation.BuildManager.GetType(ptype, true)) as TranslateSql);
                        }
                    }

                }
            }
            return _Provider[dbType];
        }



        public static DbConnection GetDbConnection(EntityName EntityName)
        {
            var configName = MyOqlSect.Entitys.GetConfig(EntityName).db;
            return GetDbConnection(configName);

        }

        public static DbConnection GetDbConnection(string ConfigName)
        {
            return GetDbProvider(ConfigName).GetConnection(ConfigName);
        }

        public static TranslateSql GetDbProvider(string ConfigName)
        {
            return dbo.GetDbProvider(dbo.GetDatabaseType(ConfigName));
        }


        /// <summary>
        /// 通过实体找到所对应的数据库. 
        /// </summary>
        /// <param name="ConfigName"></param>
        /// <returns></returns>
        public static DatabaseType GetDatabaseType(string ConfigName)
        {
            return Connections[ConfigName].ProviderName.ToEnum(DatabaseType.SqlServer);
        }
    }
}
