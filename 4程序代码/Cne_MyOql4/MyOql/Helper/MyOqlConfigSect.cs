using System;
using System.Collections.Generic;
using System.Configuration;
using MyCmn;

namespace MyOql
{

    /// <summary>
    /// MyOql配置节点.
    /// </summary>
    /// <example>
    /// 注意事项:
    /// <div style="font-size:18px;font-weight:bold;border:solid 1px green;background-color:yellow;margin:10px;padding:10px;">
    /// 对于 权限控制 表, UsePower属性 一定为 false .
    /// 对于 日志 表, Log 属性一定为 false.
    /// </div>
    /// 对于所有人都可操作的表,如日志,字典, 建议设置 UsePower  为false 以提高性能.
    /// 对于系统访问频繁,是否访问并不关心的表, 建议设置 Log 为false 以减少日志冗余. 
    /// <code>
    /// <![CDATA[
    /// <configSections>
    ///    <section name="MyOql" type="MyOql.MyOqlConfigSect,MyOql"/>
    /// </configSections>
    /// <MyOql>
    ///     <Entitys CacheTime="120" db="dbo" UsePower="true" OraclePKG="PKG" Log="true">
    ///         <Entity Name="Dict" CacheTime="120" UsePower="false"/>
    ///         <Entity Name="Person" CacheTime="120" db="dbo"/>
    ///         <Entity Name="PowerTable" CacheTime="120" UsePower="false" Log="false"/>
    ///         <Entity Name="PowerColumn" CacheTime="120" UsePower="false" Log="false"/>
    ///         <Entity Name="PowerController" CacheTime="120" UsePower="false" Log="false"/>
    ///         <Entity Name="PowerAction" CacheTime="120" UsePower="false" Log="false"/>
    ///         <Entity Name="PowerButton" CacheTime="120" UsePower="false" Log="false"/>
    ///         <Entity Name="VPowerAction" CacheTime="120" UsePower="false" Log="false"/>
    ///         <Entity Name="VPowerData" CacheTime="120" UsePower="false" Log="false"/>
    ///         <Entity Name="Log" Log="false" UsePower="false"/>
    ///         <Entity Name="PLogin" UsePower="false" />
    ///     </Entitys>
    ///     <DbProviders>
    ///         <Provider Name="MySql" Type="MyOql.Provider.MySql,MyOql"/>
    ///         <Provider Name="SqlServer" Type="MyOql.Provider.SqlServer,MyOql"/>
    ///         <Provider Name="Oracle" Type="MyOql.Provider.Oracle,MyOql"/>
    ///     </DbProviders>
    /// </MyOql>
    /// ]]>
    /// </code>
    /// </example>
    public class MyOqlConfigSect : ConfigurationSection
    {
        [ConfigurationProperty("LogFile")]
        public string LogFile
        {
            get { return this["LogFile"] as string; }
        }

        [ConfigurationProperty("Entitys")]
        public EntityCollection Entitys
        {
            get { return this["Entitys"] as EntityCollection; }
        }

        [ConfigurationProperty("DbProviders")]
        public ProviderCollection DbProviders
        {
            get { return this["DbProviders"] as ProviderCollection; }
        }


        public class EntityCollection : ConfigurationElementCollection
        {
            public IEnumerable<GroupCollection.EntityElement> GetConfig(Func<GroupCollection.EntityElement, bool> func)
            {
                foreach (GroupCollection group in this)
                {
                    foreach (GroupCollection.EntityElement tab in group)
                    {
                        if (func(tab)) yield return tab;
                    }
                }
            }

            //public GroupCollection.EntityElement GetConfig(RuleBase Rule)
            //{
            //    //不要Owner。避免循环。
            //    return GetConfig(new EntityName { DbName = Rule.GetDbName(), Name = Rule.GetName() });// Rule.GetFullName());
            //}

            public GroupCollection.EntityElement GetConfig(EntityName Entity)
            {
                foreach (GroupCollection group in this)
                {
                    foreach (GroupCollection.EntityElement tab in group)
                    {
                        if (tab.Name == Entity.DbName || (tab.Name == Entity.Name)) return tab;
                    }
                }
                return new GroupCollection.EntityElement(new GroupCollection(this));
            }

            /// <summary>
            /// 数据库对象的所有者.
            /// </summary>
            [ConfigurationProperty("Owner", IsRequired = false)]
            public string Owner
            {
                get { return (string)(this["Owner"]); }
            }

            /// <summary>
            /// 表示全部组缓存主键或唯一键的默认时间.
            /// </summary>
            [ConfigurationProperty("CacheTime", IsRequired = false, DefaultValue = 0)]
            public int CacheTime
            {
                get { return this["CacheTime"].AsInt(); }
            }


            /// <summary>
            /// 表示全部组缓存SQL默认时间.
            /// </summary>
            [ConfigurationProperty("CacheSqlTime", IsRequired = false, DefaultValue = 0)]
            public int CacheSqlTime
            {
                get { return this["CacheSqlTime"].AsInt(); }
            }

            //[ConfigurationProperty("BoxyCache", IsRequired = false, DefaultValue = 0)]
            //public int BoxyCache
            //{
            //    get
            //    {
            //        return this["BoxyCache"].AsInt();
            //    }
            //}

            [ConfigurationProperty("db", IsRequired = false)]
            public string db
            {
                get
                {
                    var theDb = this["db"].AsString();
                    if (theDb.HasValue()) return theDb;
                    else return "dbo";
                }
            }

            [ConfigurationProperty("UsePower", IsRequired = false)]
            public string UsePower
            {
                get
                {
                    var thePower = this["UsePower"].AsString();

                    if (thePower.HasValue()) return thePower;
                    else return string.Empty;
                }
            }

            [ConfigurationProperty("Log", IsRequired = false)]
            public string Log
            {
                get
                {
                    var theLog = this["Log"].AsString();
                    if (theLog.HasValue()) return theLog;
                    else return string.Empty;
                }
            }

            [ConfigurationProperty("OraclePKG", IsRequired = false)]
            public string OraclePKG
            {
                get { return this["OraclePKG"] as string; }
            }

            ///// <summary>
            ///// 是否级联更新或级联删除，全局开关，默认为 false
            ///// </summary>
            //[ConfigurationProperty("Cascade", IsRequired = false)]
            //public string Cascade
            //{
            //    get { return (string)(this["Cascade"]); }
            //}

            [ConfigurationProperty("CommandTimeout", IsRequired = false)]
            public int CommandTimeout
            {
                get
                {
                    return this["CommandTimeout"].AsInt(3600);
                }
            }

            [ConfigurationProperty("CacheSql", IsRequired = false)]
            public int CacheSql
            {
                get
                {
                    return this["CacheSql"].AsInt(3600);
                }
            }

            protected override ConfigurationElement CreateNewElement()
            {
                return new GroupCollection(this);
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                GroupCollection siteUser = element as GroupCollection;
                return siteUser.Name;
            }

            public override ConfigurationElementCollectionType CollectionType
            {
                get
                {
                    return ConfigurationElementCollectionType.BasicMap;
                }
            }
            protected override string ElementName
            {
                get
                {
                    return "Group";
                }
            }


            public class GroupCollection : ConfigurationElementCollection
            {
                public EntityCollection Container { get; private set; }

                public GroupCollection(EntityCollection entityCollection)
                {
                    this.Container = entityCollection;
                }

                [ConfigurationProperty("Name")]
                public string Name
                {
                    get { return (string)this["Name"]; }
                }

                /// <summary>
                /// 数据库对象的所有者.
                /// </summary>
                [ConfigurationProperty("Owner", IsRequired = false)]
                public string Owner
                {
                    get
                    {
                        if (this["Owner"] == null) return Container.Owner;
                        else return this["Owner"].AsString();
                    }
                }

                /// <summary>
                /// 表示该组默认的缓存主键或唯一键的时间.
                /// </summary>
                [ConfigurationProperty("CacheTime", IsRequired = false, DefaultValue = -1)]
                public int CacheTime
                {
                    get
                    {
                        var theTime = this["CacheTime"].AsInt();
                        if (theTime >= 0)
                        {
                            return theTime;
                        }
                        else return Container.CacheTime;
                    }
                }

                /// <summary>
                /// 表示该组默认的缓存SQL的时间.
                /// </summary>
                [ConfigurationProperty("CacheSqlTime", IsRequired = false, DefaultValue = -1)]
                public int CacheSqlTime
                {
                    get
                    {
                        var theTime = this["CacheSqlTime"].AsInt();
                        if (theTime >= 0)
                        {
                            return theTime;
                        }
                        else return Container.CacheSqlTime;
                    }
                }
                //[ConfigurationProperty("BoxyCache", IsRequired = false, DefaultValue = -1)]
                //public int BoxyCache
                //{
                //    get
                //    {
                //        var theBc = this["BoxyCache"].AsInt();
                //        if (theBc >= 0)
                //        {
                //            return theBc;
                //        }
                //        else return Container.BoxyCache;
                //    }
                //}

                [ConfigurationProperty("db", IsRequired = false)]
                public string db
                {
                    get
                    {
                        var theDb = this["db"].AsString();
                        if (theDb.HasValue()) return theDb;
                        else return Container.db;
                    }
                }

                [ConfigurationProperty("UsePower", IsRequired = false)]
                public string UsePower
                {
                    get
                    {
                        var thePower = this["UsePower"].AsString();
                        if (thePower.HasValue()) return thePower;
                        else return Container.UsePower;
                    }
                }

                [ConfigurationProperty("Log", IsRequired = false)]
                public string Log
                {
                    get
                    {
                        var theLog = this["Log"].AsString();
                        if (theLog.HasValue()) return theLog;
                        else return Container.Log;
                    }
                }

                [ConfigurationProperty("OraclePKG", IsRequired = false)]
                public string OraclePKG
                {
                    get
                    {
                        var thePkg = this["OraclePKG"].AsString();
                        if (thePkg.HasValue()) return thePkg;
                        else return Container.OraclePKG;
                    }
                }

                [ConfigurationProperty("CommandTimeout", IsRequired = false)]
                public int CommandTimeout
                {
                    get
                    {
                        var theTime = this["CommandTimeout"].AsString();
                        if (theTime.HasValue()) return theTime.AsInt();
                        else return Container.CommandTimeout;
                    }
                }

                protected override ConfigurationElement CreateNewElement()
                {
                    return new EntityElement(this);
                }

                protected override object GetElementKey(ConfigurationElement element)
                {
                    EntityElement siteUser = element as EntityElement;
                    return siteUser.Name;
                }

                public override ConfigurationElementCollectionType CollectionType
                {
                    get
                    {
                        return ConfigurationElementCollectionType.BasicMap;
                    }
                }
                protected override string ElementName
                {
                    get
                    {
                        return "Entity";
                    }
                }



                public class EntityElement : ConfigurationElement
                {
                    public GroupCollection Container { get; private set; }

                    public EntityElement(GroupCollection groupCollection)
                    {
                        this.Container = groupCollection;
                    }

                    /// <summary>
                    /// 数据库对象的所有者.
                    /// </summary>
                    [ConfigurationProperty("Owner", IsRequired = false)]
                    public string Owner
                    {
                        get
                        {
                            if (this["Owner"] == null) return Container.Owner;
                            else return this["Owner"].AsString();
                        }
                    }

                    [ConfigurationProperty("Name")]
                    public string Name
                    {
                        get { return this["Name"] as string; }
                    }


                    /// <summary>
                    /// 表示缓存表主键或唯一键的时间
                    /// </summary>
                    [ConfigurationProperty("CacheTime", IsRequired = false, DefaultValue = -1)]
                    public int CacheTime
                    {
                        get
                        {
                            var theTime = this["CacheTime"].AsInt();
                            if (theTime >= 0)
                            {
                                return theTime;
                            }
                            else return Container.CacheTime;
                        }
                        set
                        {
                            this["CacheTime"] = value;
                        }
                    }

                    /// <summary>
                    /// 表示缓存表主键或唯一键的时间
                    /// </summary>
                    [ConfigurationProperty("CacheSqlTime", IsRequired = false, DefaultValue = -1)]
                    public int CacheSqlTime
                    {
                        get
                        {
                            var theTime = this["CacheSqlTime"].AsInt();
                            if (theTime >= 0)
                            {
                                return theTime;
                            }
                            else return Container.CacheSqlTime;
                        }
                        set
                        {
                            this["CacheSqlTime"] = value;
                        }
                    }

                    //[ConfigurationProperty("BoxyCache", IsRequired = false, DefaultValue = -1)]
                    //public int BoxyCache
                    //{
                    //    get
                    //    {
                    //        var theBc = this["BoxyCache"].AsInt();
                    //        if (theBc >= 0)
                    //        {
                    //            return theBc;
                    //        }
                    //        else return this.Container.BoxyCache;
                    //    }
                    //}

                    [ConfigurationProperty("db", IsRequired = false)]
                    public string db
                    {
                        get
                        {
                            var theDb = this["db"].AsString();
                            if (theDb.HasValue())
                            {
                                return theDb;
                            }
                            else return Container.db;
                        }
                    }

                    /// <summary>
                    /// CRUD=  *  . 其中= 表示是否启用行集权限. * 是所有权限.  另外.true 表示所有权限,  false 表示没有任何权限
                    /// 请使用: MyOqlActionEnumExtend.TranslateMyOqlAction 进行转换.
                    /// </summary>
                    [ConfigurationProperty("UsePower", IsRequired = false)]
                    public string UsePower
                    {
                        get
                        {
                            var thePower = this["UsePower"].AsString();
                            if (thePower.HasValue())
                            {
                                return thePower;
                            }
                            else return Container.UsePower;
                        }
                        set
                        {
                            this["UsePower"] = value;
                        }
                    }

                    /// <summary>
                    /// CRUD= * . 请使用: MyOqlActionEnumExtend.TranslateMyOqlAction 进行转换.
                    /// </summary>
                    [ConfigurationProperty("Log", IsRequired = false)]
                    public string Log
                    {
                        get
                        {
                            var theLog = this["Log"].AsString();
                            if (theLog.HasValue())
                            {
                                return theLog;
                            }
                            else return Container.Log;
                        }
                        set
                        {
                            this["Log"] = value;
                        }
                    }

                    [ConfigurationProperty("OraclePKG")]
                    public string OraclePKG
                    {
                        get
                        {
                            var thePkg = this["OraclePKG"].AsString();
                            if (thePkg.HasValue())
                            {
                                return thePkg;
                            }
                            else return Container.OraclePKG;
                        }
                    }
                }
            }
        }
        public class ProviderCollection : ConfigurationElementCollection
        {
            protected override ConfigurationElement CreateNewElement()
            {
                return new ProviderElement();
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                ProviderElement siteUser = element as ProviderElement;
                return siteUser.Name;
            }

            public override ConfigurationElementCollectionType CollectionType
            {
                get
                {
                    return ConfigurationElementCollectionType.BasicMap;
                }
            }
            protected override string ElementName
            {
                get
                {
                    return "Provider";
                }
            }
            public class ProviderElement : ConfigurationElement
            {

                [ConfigurationProperty("Name", IsRequired = true)]
                public string Name
                {
                    get { return this["Name"] as string; }
                }

                [ConfigurationProperty("Type", IsRequired = true)]
                public string Type
                {
                    get { return (string)(this["Type"]); }
                }
            }
        }
    }

}
