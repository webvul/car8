using System;
using MyCmn;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MyOql
{
    public class CachedMyOqlSet
    {
        public bool HasValue { get; set; }
        public MyOqlSet Set { get; set; }
    }

    public class CachedModel<T>
    {
        public bool HasValue { get; set; }
        public T Model { get; set; }
    }


    public abstract partial class IDboEvent
    {
        /// <summary>
        /// 移除一条缓存数据.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="IdValue">如果为空，则移除所有</param>
        public virtual void OnCacheRemoveById(ContextClipBase Context, string IdValue)
        {
            var Entity = Context.CurrentRule;
            if (Entity == null) return;
            if (Entity.GetDbName().HasValue() == false) return;

            if (Entity.GetCacheTime() == 0)
            {
                return;
            }
            if (IdValue.HasValue())
            {
                CacheHelper.Remove(Entity.GetFullName().GetCacheKey(IdValue));
                LogTo(0, Context, InfoEnum.Info, () => "已清除缓存:" + IdValue);
            }
            else
            {
                OnCacheRemoveAll(Context);
            }
        }


        /// <summary>
        /// 单条数据查找.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="IdValueFunc"></param>
        /// <param name="NewModelFunc"></param>
        /// <returns></returns>
        public virtual CachedModel<T> OnCacheFindById<T>(ContextClipBase Context, Func<string> IdValueFunc, Func<T> NewModelFunc)
        {
            var jm = new CachedModel<T>();

            if (Context.ContainsConfig(ReConfigEnum.SkipCache)) return jm;

            if (IdValueFunc == null) return jm;

            var changedTable = GetChangedTable();
            if (changedTable != null && changedTable.Length > 0)
            {
                if (changedTable.All(o =>
                   {
                       if (o == Context.CurrentRule.GetName() || o == Context.CurrentRule.GetDbName())
                       {
                           OnCacheRemoveAll(Context);
                           ClearedCacheTable(o);
                           return false;
                       }
                       return true;
                   }) == false)
                {
                    return jm;
                }
            }

            var idVal = IdValueFunc();

            if (idVal.HasValue() == false)
            {
                LogTo(0, Context, InfoEnum.Info, () => "找不到缓存项");
                return jm;
            }

            var cacheKey = Context.CurrentRule.GetFullName().GetCacheKey(idVal);
            jm.HasValue = CacheHelper.IsExists(cacheKey);

            if (jm.HasValue == false) return jm;

            var data = CacheHelper.Get<object[]>(cacheKey);

            var retVal = CacheDataToDict(Context.CurrentRule, data);


            LogTo(0, Context, InfoEnum.Info, () =>
                retVal != null ? "找到缓存项" : "找不到缓存项");

            if (retVal == null) return jm;

            Type type = typeof(T);

            if (Context != null && (type.IsValueType || type.FullName == "System.String") &&
                Context.Dna.Count(o => o.IsColumn()) == 1)
            {
                var col = Context.Dna.First(o => o.IsColumn()) as ColumnClip;
                if (retVal.ContainsKey(col.Name))
                {
                    jm.Model = ValueProc.As<T>(retVal[col.Name]);
                    return jm;
                }
            }

            jm.Model = dbo.DictionaryToFuncModel(retVal, NewModelFunc);
            return jm;
        }

        /// <summary>
        /// 把单条数据转换为 Dict . 单条数用 object[] 来存储, 是为了减少存储量.
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public XmlDictionary<string, object> CacheDataToDict(RuleBase Entity, object[] Data)
        {
            if (Data == null) return null;
            if (
                Entity.GetColumns().Length != Data.Length) return null;
            //string.Format("缓存实体 {0} ,要求列数要和实体定义一致.", Entity.GetFullName()),
            //InfoEnum.MyOql | InfoEnum.System | InfoEnum.Error,
            //    Entity.GetFullName().AsString());

            XmlDictionary<string, object> retVal = new XmlDictionary<string, object>();
            for (int i = 0; i < Data.Length; i++)
            {
                retVal[Entity.GetColumns()[i].Name] = Data[i];
            }
            return retVal;
        }

        /// <summary>
        /// 把 XmlDict 转化为 object[]. 单条数用 object[] 来存储, 是为了减少存储量.
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="Dict"></param>
        /// <returns></returns>
        public object[] CacheDictToData(RuleBase Entity, XmlDictionary<ColumnClip, object> Dict)
        {
            return CacheDictToData(Entity, Dict.ToXmlDictionary(o => o.Key.GetAlias(), o => o.Value));
        }

        /// <summary>
        /// 把 XmlDict 转化为 object[]. 单条数用 object[] 来存储, 是为了减少存储量.
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="Dict"></param>
        /// <returns></returns>
        public object[] CacheDictToData(RuleBase Entity, XmlDictionary<string, object> Dict)
        {
            if (Dict == null) return null;
            if (Entity.GetColumns().Length != Dict.Count) return null;
            //string.Format("缓存实体 {0} ,要求列数要和实体定义一致.", Entity.GetFullName()),
            //InfoEnum.MyOql | InfoEnum.System | InfoEnum.Error,
            //    Entity.GetFullName().AsString());

            List<object> retVal = new List<object>();
            if (Dict == null) return null;
            for (int i = 0; i < Dict.Count; i++)
            {
                var name = Entity.GetColumns()[i].Name;
                if (Dict.ContainsKey(name) == false) return null;
                //string.Format(@"要缓存的实体 {0} 中不存在列 {1}", Entity.GetFullName(), name),
                //InfoEnum.MyOql | InfoEnum.System | InfoEnum.Error,
                //Entity.GetFullName().AsString());

                retVal.Add(Dict[name]);
            }
            return retVal.ToArray();
        }


        /// <summary>
        /// 更新某一个缓存实体,如果更新不成功，会删除所有的缓存项。
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="IdValue"></param>
        /// <param name="Model"></param>
        /// <returns>成功更新缓存，返回true，如果没有更新，返回false</returns>
        public virtual bool OnCacheAddById(ContextClipBase Context, string IdValue, XmlDictionary<string, object> Model)
        {
            var Entity = Context.CurrentRule;
            //更新缓存项。
            if (Entity.GetCacheTime() > 0)
            {
                if (IdValue.HasValue() && !IdValue.StartsWith("#") /*&& Model != null*/)
                {
                    CacheHelper.Get(Entity.GetFullName().GetCacheKey(IdValue), new TimeSpan(0, 0, dbo.MyOqlSect.Entitys.GetConfig(new EntityName { DbName = Entity.GetDbName(), Name = Entity.GetName() }).CacheTime), () => CacheDictToData(Entity, Model));
                    LogTo(0, Context, InfoEnum.Info, () => "已添加缓存:" + IdValue);
                    return true;
                }
                else
                {
                    OnCacheRemoveBySql(Context);
                    //CacheHelper.Remove(Entity.GetFullName().GetCacheBoxKey());
                }
            }
            return false;
        }

        /// <summary>
        /// 获取外部接口改变的数据表.
        /// </summary>
        /// <remarks>如果返回有值,则会清空关联该值的所有缓存.</remarks>
        /// <returns></returns>
        public abstract string[] GetChangedTable();


        /// <summary>
        /// 已清除缓存表的事件通知.
        /// </summary>
        /// <param name="Table"></param>
        public abstract void ClearedCacheTable(string Table);

        /// <summary>
        /// 对多条查询的数据,根据Sql进行查找.
        /// </summary>
        /// <param name="Context">上下文.</param>
        /// <param name="cacheSqlKey"></param>
        /// <returns></returns>
        public virtual CachedMyOqlSet OnCacheFindBySql(ContextClipBase Context, string cacheSqlKey)
        {
            var ret = new CachedMyOqlSet();

            if (Context.ContainsConfig(ReConfigEnum.SkipCache)) return ret;

            if (cacheSqlKey.HasValue() == false) return ret;
            if (Context.CurrentRule.GetCacheSqlTime() <= 0) return ret;
            if (Context.ContainsFunctionRule()) return ret;

            var rules = Context.Rules;
            string[] RelateTable = rules.Select(o => o.GetDbName()).ToArray();

            var changedTable = GetChangedTable();
            if (changedTable != null && changedTable.Length > 0)
            {
                bool HitChanged = false;
                changedTable.Intersect(RelateTable).All(o =>
                    {
                        HitChanged = true;
                        var entity = rules.First(e => e.GetDbName() == o);
                        OnCacheRemoveAll(new MyContextClip(Context, entity));
                        ClearedCacheTable(o);
                        return true;
                    });

                if (HitChanged)
                {
                    return ret;
                }
            }

            //T 可以是四个类型: List<XmlDictionary<string, object>> , MyOqlSet, 单值 string ,object[]

            ret.HasValue = CacheHelper.IsExists(cacheSqlKey);

            if (ret.HasValue == false) return ret;

            var dict = CacheHelper.Get<MyOqlSet>(cacheSqlKey);

            if (dict == null) return ret;

            ret.Set = dict.Clone() as MyOqlSet;
            return ret;
        }

        /// <summary>
        /// 对多条查询的数据进行缓存.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="cacheSqlKey"></param>
        /// <param name="GetModelFunc"></param>
        public void OnCacheAddBySql(ContextClipBase Context, string cacheSqlKey, Func<MyOqlSet> GetModelFunc)
        {
            if (GetModelFunc == null) return;

            var cacheSqlTime = int.MaxValue;
            Context.Rules.All(o =>
                {
                    cacheSqlTime = Math.Min(cacheSqlTime, o.GetCacheSqlTime());
                    return true;
                });

            CacheHelper.Get<MyOqlSet>(cacheSqlKey,
                new TimeSpan(0, 0, cacheSqlTime), () =>
                {
                    var model = GetModelFunc();
                    if (model == null) return null;
                    return model.Clone() as MyOqlSet;
                }
                );
        }

        private bool CacheSqlKeyContainsTable(string sqlKey, RuleBase Entity)
        {
            if (sqlKey.StartsWith("MyOql|Sql|") == false) return false;
            var last1Index = sqlKey.IndexOf('|', 10);
            if (last1Index < 0) return false;
            var ents = sqlKey.Slice(10, last1Index).AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            return ents.Contains(Entity.GetDbName());
        }

        /// <summary>
        /// 清除根据SQL 的缓存项，同时清除 和该表相关联 视图的缓存项  <see cref="MyOql.IDboEvent"/>
        /// </summary>
        /// <param name="Context"></param>
        public virtual void OnCacheRemoveBySql(ContextClipBase Context)
        {
            var Entity = Context.CurrentRule;
            //可能是存储过程。直接返回不处理。
            if (Entity == null) return;
            var dbName = Entity.GetDbName();
            if (dbName.HasValue() == false) return;

            if (
                (Context.ClearedAllCacheTable.Contains(dbName)
                ||
                Context.ClearedSqlCacheTable.Contains(dbName)) == false
                )
            {
                CacheHelper.Remove(o => CacheSqlKeyContainsTable(o, Entity));
                Context.ClearedSqlCacheTable.Add(dbName);
            }

            //清除 关联的视图缓存
            dbo.GetRelationViewsByTable(dbName).All(o =>
                {
                    OnCacheRemoveAll(new MyContextClip(Context, Idbr.GetMyOqlEntity(o)));
                    return true;
                });

            //{
            //    if (o.StartsWith("MyOql|Sql|") == false) return false;
            //    var last1Index = o.IndexOf('|', 10);
            //    if (last1Index < 0) return true;
            //    var ents = o.Slice(10, last1Index).AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //    if (ents.IndexOf(Entity.GetDbName()) >= 0) return true;
            //    else return false;
            //});
        }

        /// <summary>
        /// 移除和该表相关的所有项，并清除和该表相关的视图的所有项 <see cref="MyOql.IDboEvent"/>
        /// </summary>
        /// <param name="Context"></param>
        public virtual void OnCacheRemoveAll(ContextClipBase Context)
        {
            var Entity = Context.CurrentRule;
            if (Entity == null) return;
            var dbName = Entity.GetDbName();
            if (dbName.HasValue() == false) return;

            if (Context.ClearedAllCacheTable.Contains(dbName) == false)
            {
                if (Entity.GetCacheTime() > 0)
                {
                    CacheHelper.Remove(o => o.StartsWith(Entity.GetFullName().GetCacheKey("")));
                    LogTo(0, Context, InfoEnum.Info, () => "已清除全部单项缓存.");
                }

                if (Entity.GetCacheSqlTime() > 0)
                {
                    if (Context.ClearedSqlCacheTable.Contains(dbName) == false)
                    {
                        CacheHelper.Remove(o => CacheSqlKeyContainsTable(o, Entity));
                        Context.ClearedSqlCacheTable.Add(dbName);
                    }
                }

                Context.ClearedAllCacheTable.Add(dbName);
            }

            //清除关联的视图的缓存
            dbo.GetRelationViewsByTable(dbName).All(o =>
            {
                OnCacheRemoveAll(new MyContextClip(Context, Idbr.GetMyOqlEntity(o)));
                return true;
            });
        }
    }
}
