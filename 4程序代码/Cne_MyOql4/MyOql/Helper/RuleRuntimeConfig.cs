using System;

namespace MyOql
{
    [Serializable]
    public class RuleRuntimeConfig : ICloneable
    {
        public string Owner { get; set; }
        public string Name { get; set; }
        public int CacheTime { get; set; }
        public int CacheSqlTime { get; set; }
        public string db { get; set; }
        public string UsePower { get; set; }
        public string Log { get; set; }
        public string OraclePKG { get; set; }
        public int CommandTimeout { get; set; }

        public RuleRuntimeConfig(MyOql.MyOqlConfigSect.EntityCollection.GroupCollection.EntityElement Entity)
        {
            this.Owner = Entity.Owner;
            this.Name = Entity.Name;
            this.CacheTime = Entity.CacheTime;
            this.CacheSqlTime = Entity.CacheSqlTime;
            this.db = Entity.db;
            this.UsePower = Entity.UsePower;
            this.Log = Entity.Log;
            this.OraclePKG = Entity.OraclePKG;
            this.CommandTimeout = Entity.Container.CommandTimeout;
        }

        public RuleRuntimeConfig() { }

        public object Clone()
        {
            var Entity = new RuleRuntimeConfig();

            Entity.Owner = this.Owner;
            Entity.Name = this.Name;
            Entity.CacheTime = this.CacheTime;
            Entity.CacheSqlTime = this.CacheSqlTime;
            Entity.db = this.db;
            Entity.UsePower = this.UsePower;
            Entity.Log = this.Log;
            Entity.OraclePKG = this.OraclePKG;
            Entity.CommandTimeout = this.CommandTimeout;

            return Entity;
        }
    }
}
