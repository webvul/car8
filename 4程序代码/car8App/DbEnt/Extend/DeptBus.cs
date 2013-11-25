using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCmn;
using System.Text.RegularExpressions;
using MyOql;
using System.Web;

namespace DbEnt
{
    public partial class DeptRule
    {
        //public static DeptEntity Get(string DeptName)
        //{
        //    return DeptEntity.FindFirst(dbo.Dept.Name == DeptName) ;
        //}

        //public static string GetMyWebName(int DeptID)
        //{
        //    return CacheHelper.Get<string>(string.Format(@"{0}_{1}", CacheKey.ShopWebName.ToString(), DeptID), CacheTime.Hour, delegate()
        //           {
        //               DeptEntity dept = DeptEntity.FindById(DeptID);
        //               string webName = "ErrWebName";

        //               if (dept != null && dept.WebName.HasValue())
        //               {
        //                   webName = dept.WebName;
        //               }
        //               return webName;
        //           });
        //}
        //public static bool IsValid(int DeptID)
        //{
        //    return true;
        //    DeptEntity dept = DeptEntity.FindById(DeptID);
        //    if (dept.EndTime <= DateTime.Now)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}
        public static string[] GetSysName()
        {
            return dbr.Dict
                .Select(o => o.Value)
                .Where(o => o.Key == DictKeyEnum.SysName)
                .ToScalar().AsString().Split(',');
            //return CacheHelper.Get(CacheKey.SysName, delegate()
            //{
            //    DictRule.DictEntity dict = DictRule.DictEntity.FindFirst(dbo.Dict.Key == DictSysKeyEnum.SysName &
            //        dbo.Dict.PID.In(db.Select(dbo.Dict.ID).Where(dbo.Dict.Key == DictKeyEnum.Sys & dbo.Dict.PID == 0).ToSubQuery())
            //        );
            //    return dict.Value.AsString().Split(',');
            //});
        }

        public static bool IsValidateWebName(string DeptName, out string Msg)
        {
            if (DeptName.HasValue() == false)
            {
                Msg = "为空!";
                return false;
            }

            if (DeptName.Trim().Length < 4)
            {
                Msg = "长度小于4个字符!";
                return false;
            }

            if (DeptName.GetSafeValue() != DeptName)
            {
                Msg = "包含非法字符!";
                return false;
            }

            if (GetSysName().Contains(DeptName.Trim()))
            {
                Msg = "是系统保留用名!";
                return false;
            }
            if (Regex.IsMatch(DeptName, @"^[a-zA-Z0-9_]*$", RegexOptions.Compiled) == false)
            {
                Msg = "不是字母与数字的组合";
                return false;
            }

            Msg = "合法";
            return true;
        }

        public static LangEnum GetLangs(int DeptID)
        {
            LangEnum langs = LangEnum.Zh;

            dbr.Dept.FindScalar(o => o.Langs, o => o.Id == DeptID).AsString().Split(',')
                .All(o => { langs |= o.ToEnum(LangEnum.Zh); return true; });

            return langs;
        }


        public static LangEnum GetDefaultLang(int DeptID)
        {
            return dbr.Dept.FindById(DeptID).DefaultLang.AsString().ToEnum(LangEnum.Zh);
        }

        public static LangEnum GetLangs(DeptRule.Entity Dept)
        {
            if (Dept == null) return LangEnum.Zh;
            if (Dept.Langs.HasValue() == false) return LangEnum.Zh;

            LangEnum langs = LangEnum.Zh;

            Dept.Langs.AsString().Split(',')
                .All(o => { langs |= o.ToEnum(LangEnum.Zh); return true; });

            return langs;
        }

        //public static DeptRule.Entity GetSysAdminDept()
        //{
        //    return CacheHelper.Get<DeptRule.Entity>(CacheKey.SysAdminDept, new TimeSpan(0, 10, 0), delegate()
        //    {
        //        PersonRule.Entity per = dbr.Person.FindByUserID("SysAdmin");

        //        if (per != null) return per.
        //        else return null;
        //    });
        //}

        public partial class Entity
        {

        }
    }
}
