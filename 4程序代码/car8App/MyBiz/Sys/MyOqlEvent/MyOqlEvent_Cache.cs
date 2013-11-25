using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyOql;
using MyCmn;
using System.Data.Common;
using MyBiz;
using System.Configuration;
using DbEnt;


namespace MyBiz.Sys
{
    public partial class MyOqlEvent
    {
        public override void OnCacheRemoveById(ContextClipBase Context, string IdValue)
        {
            base.OnCacheRemoveById(Context, IdValue);
        }
        public override void OnCacheRemoveAll(ContextClipBase Context)
        {
            base.OnCacheRemoveAll(Context);
        }

        public override bool OnCacheAddById(ContextClipBase Context, string IdValue, XmlDictionary<string, object> Model)
        {
            return base.OnCacheAddById(Context, IdValue, Model);
        }
        public override CachedModel<T> OnCacheFindById<T>(ContextClipBase Context, Func<string> IdValueFunc, Func<T> NewModelFunc)
        {
            return base.OnCacheFindById<T>(Context, IdValueFunc, NewModelFunc);
        }

        public override void OnCacheRemoveBySql(ContextClipBase Context)
        {
            base.OnCacheRemoveBySql(Context);
        }

        public override CachedMyOqlSet OnCacheFindBySql(ContextClipBase Context, string cacheSqlKey)
        {
            return base.OnCacheFindBySql(Context, cacheSqlKey);
        }
    }
}
