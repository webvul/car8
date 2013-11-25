using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using System.Data;
using System.Xml.Serialization;
using System.Xml;

namespace MyOql
{

    /// <summary>
    /// 查询子句.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public partial class SelectClip<T> : SelectClip
        where T : RuleBase
    {
        public SelectClip()
            : base()
        {
        }

        public SelectClip(T rule)
            : base(rule)
        {
        }



        public new MyOqlSet<T> ToMyOqlSet(SelectOption Option)
        {
            var set = base.ToMyOqlSet(Option);

            var retVal = new MyOqlSet<T>(this.CurrentRule);

            if (set == null) return retVal;

            retVal.Columns = set.Columns;
            retVal.Count = set.Count;
            retVal.Entity = set.Entity;
            retVal.OrderBy = set.OrderBy;
            retVal.Rows = set.Rows;
            return retVal;


            //var order = this.Dna.FirstOrDefault(o => o.Key == SqlKeyword.OrderBy) as OrderByClip;
            //if (order != null)
            //{
            //    retVal.OrderBy = order;
            //}

            //if (dbo.Event.OnReading(this) == false) return retVal;


            ////if (this.CurrentRule.GetBoxyCache() > 0)
            ////{
            ////    var cacheData = dbo.OnCacheFindSome(this);
            ////    if (cacheData != null)
            ////    {
            ////        return cacheData;
            ////    }
            ////}


            //retVal.Columns =
            //           Enumerable.Select(
            //               this.Dna.Where(o => o.Key == SqlKeyword.Column),
            //                o => (o as ColumnClip).Clone() as ColumnClip
            //                ).ToArray();

            //if (this.CurrentRule.GetDataBase().IsIn(DatabaseType.SqlServer2000, DatabaseType.Excel, DatabaseType.MsAccess, DatabaseType.Other))
            //{
            //    ToMyOqlSetUseOldPageMethod(Option, retVal);
            //}
            //else
            //{
            //    var myCmd = this.ToCommand();
            //    string cacheSqlKey = null;
            //    if (this.CurrentRule.GetCacheSqlTime() > 0 && !this.ContainsNoTableRule())
            //    {
            //        cacheSqlKey = this.GetCacheSql(myCmd.FullSql);
            //        var model = dbo.Event.OnCacheFindBySql(this, cacheSqlKey);
            //        if (model.HasData())
            //        {
            //            retVal.Rows.AddRange(model.Rows);
            //        }
            //    }

            //    if (retVal.Rows.Count == 0)
            //    {
            //        this.ExecuteReader(myCmd, o =>
            //        {
            //            retVal.Rows.Add(o.ToValueData());
            //            return true;
            //        });
            //    }

            //    retVal.Count = retVal.Rows.Count;
            //    if (this.CurrentRule.GetCacheSqlTime() > 0 && retVal.HasData() && cacheSqlKey.HasValue() && !this.ContainsNoTableRule())
            //    {
            //        dbo.Event.OnCacheAddBySql(this, cacheSqlKey, () => retVal);
            //    }


            //    if (Option == SelectOption.WithCount)
            //    {
            //        retVal.Count = GetQueryResultCount();
            //    }
            //}

            //dbo.Event.OnReaded(this);
            //return retVal;
        }

        public TModel ToEntity<TModel>(Func<T, TModel> NewModelFunc)
            where TModel : new()
        {
            return ToEntity<TModel>(() => NewModelFunc((T)this.CurrentRule));
        }

        public List<TModel> ToEntityList<TModel>(Func<T, TModel> NewModelFunc)
            where TModel : new()
        {
            return ToEntityList<TModel>(() => NewModelFunc((T)this.CurrentRule));
        }
        public new MyOqlSet<T> ToMyOqlSet()
        {
            return ToMyOqlSet(SelectOption.WithCount);
        }


        /// <summary>
        /// 添加 where 子句.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public SelectClip<T> Where(Func<T, WhereClip> func)
        {
            if (func != null)
            {
                return this.Where(func((T)(object)this.CurrentRule));
            }
            else return this;
        }

        /// <summary>
        /// 生成 having 子句.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public SelectClip<T> Having(Func<T, WhereClip> func)
        {
            if (func == null) return this;

            HavingClip having = new HavingClip();
            having.Where = func.Invoke((T)(object)this.CurrentRule);
            this.Dna.Add(having);
            return this;
        }


        /// <summary>
        /// 生成 group by 子句. 
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public SelectClip<T> GroupBy(Func<T, IEnumerable<ColumnClip>> func)
        {
            if (func == null) return this;
            this.GroupBy(func.Invoke((T)(object)this.CurrentRule).ToArray());
            return this;
        }


        /// <summary>
        /// 生成 skip 子句. 结果集会过滤指定的行数
        /// </summary>
        /// <param name="Num">表示过滤指定的行数</param>
        /// <returns></returns>
        public new SelectClip<T> Skip(int Num)
        {
            if (Num <= 0) return this;
            var skip = this.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Skip) as SkipClip;
            if (skip == null)
            {
                this.Dna.Add(new SkipClip() { SkipNumber = Num });
            }
            else
            {
                skip.SkipNumber = Num;
            }
            return this;
        }

        /// <summary>
        /// 生成 take 子句.结果集会只取指定数量的记录
        /// 仅在 &gt;= 0 时有意义。
        /// </summary>
        /// <param name="Num">表示要取的指定的记录数</param>
        /// <returns></returns>
        public new SelectClip<T> Take(int Num)
        {
            if (Num <= 0 || Num == int.MaxValue) return this;
            var take = this.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Take) as TakeClip;
            if (take == null)
            {
                this.Dna.Add(new TakeClip() { TakeNumber = Num });
            }
            else
            {
                take.TakeNumber = Num;
            }
            return this;
        }
    }
}

