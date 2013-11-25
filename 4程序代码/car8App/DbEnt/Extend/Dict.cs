using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MyCmn;
using MyOql;

namespace DbEnt
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DictRule
    {
        public partial class Entity
        {
            public static bool SetValue(DictKeyEnum Key, string Value, DictTraitEnum Trait)
            {
                return dbr.Dict
                    .AutoSave(o => o.Key == Key & o.Trait == Trait & o.DeptID == 0 & o.Value == Value, o => o.Key)
                         == 1;
            }
        }
    }

    public partial class dbr
    {
        public static string[] GetDictValues(DictKeyEnum Key)
        {
            var dict = dbr.Dict.SelectWhere(o => o.Key == Key).SkipPower().ToEntityList(o => o._);
            var list = new List<string>();
            dict.All(o =>
            {
                if (o.Trait == DictTraitEnum.Array || o.Trait == DictTraitEnum.Enum)
                {
                    var vals = o.Value.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (vals != null)
                    {
                        list.AddRange(vals);
                    }
                }
                else if (o.Value.HasValue())
                {
                    list.Add(o.Value);
                }
                return true;
            });
            return list.ToArray();
        }

        public string GetValues(DictKeyEnum Key)
        {
            var dict = dbr.Dict.SelectWhere(o => o.Key == Key).SkipPower().ToEntityList(o => o._);
            var retVal = string.Empty;
            dict.All(o =>
                {
                    if (o.Trait == DictTraitEnum.Array)
                    {
                        var vals = o.Value.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (vals != null)
                        {
                            retVal = vals[0];
                            return false;
                        }
                    }
                    else if (o.Value.HasValue())
                    {
                        retVal = o.Value;
                        return false;
                    }
                    return true;
                });

            return retVal;
        }
    }
}
