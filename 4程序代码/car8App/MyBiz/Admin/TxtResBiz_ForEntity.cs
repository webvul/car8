using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using MyOql;
using DbEnt;

namespace MyBiz.Admin
{
    public partial class TxtResBiz
    {
        ///// <summary>
        ///// 自定义实体,如果父类中继承IEntity,那么父类的每级子类都需要继承 IEntity,并且显示实体之.
        ///// </summary>
        //public class TxtResModel : TxtResRule.Entity, IEntity
        //{
        //    public string Key { get; set; }
        //    public string Group { get; set; }

        //    string[] IEntity.GetProperties()
        //    {
        //        var list = base.GetProperties().ToList();
        //        list.Add("Key");
        //        list.Add("Group");
        //        return list.ToArray();
        //    }

        //    object IEntity.GetPropertyValue(string PropertyName)
        //    {
        //        if (PropertyName == "Key") return this.Key;
        //        else if (PropertyName == "Group") return this.Group;
        //        else return base.GetPropertyValue(PropertyName);
        //    }

        //    bool IEntity.SetPropertyValue(string PropertyName, object Value)
        //    {
        //        if (PropertyName == "Key")
        //        {
        //            this.Key = Value.AsString();
        //            return true;
        //        }
        //        else if (PropertyName == "Group")
        //        {
        //            this.Group = Value.AsString();
        //            return true;
        //        }
        //        else
        //        {
        //            return base.SetPropertyValue(PropertyName, Value);
        //        }
        //    }
        //}

        public static List<VTxtResRule.Entity> LoadRes(LangEnum Lang)
        {
            return CacheHelper.Get("ResValue." + Lang.AsString(), new TimeSpan(0, 10, 0), () =>
            {
                return dbr.View.VTxtRes
                    .SelectWhere(o => o.Lang.IsNull(Lang) == Lang)
                    .ToEntityList(o => o._);
            });
        }

        public static StringLinker GetRes(LangEnum Lang, string Key)
        {
            //var kv = new GroupKey(Res);

            var retVal = GetResFromDb(Lang, Key);

            if (retVal.HasValue()) return retVal;
            else
            {
                var key = dbr.ResKey.FindFirst(o => o.Key == Key, o => o._);
                if (key == null)
                {
                    dbr.ResKey.Insert(o => o.Key == Key).Execute();
                    ClearCache(Lang);
                }
                return Key;
            }
        }

        private static string GetResFromDb(LangEnum Lang, string Key)
        {
            return LoadRes(Lang)
                .Where(o => o.Lang == Lang && string.Equals(o.Key, Key, StringComparison.CurrentCultureIgnoreCase))
                .Select(o => o.Value)
                .FirstOrDefault();
        }

        public static void ClearCache(LangEnum Lang)
        {
            CacheHelper.Remove(o => o.StartsWith("ResValue." + Lang.AsString()));

            dbo.Event.OnCacheRemoveAll(MyContextClip.CreateBySelect(dbr.View.VTxtRes));
        }
    }
}
