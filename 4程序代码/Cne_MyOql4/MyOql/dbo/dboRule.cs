using System;
using System.Data;
using System.Data.Common;
using MyCmn;
using System.Collections.Generic;
using System.Threading;

namespace MyOql
{
    public static partial class dboRule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static DeleteClip<T> Delete<T>(this T obj, WhereClip where)
            where T : RuleBase, ITableRule 
        {
            DeleteClip<T> delete = new DeleteClip<T>();
            delete.CurrentRule = obj;
            if (where != null)
            {
                delete.Dna.Add(where);
            }
            return delete;
        }



        /// <summary>
        /// 生成删除子句。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static DeleteClip<T> Delete<T>(this T obj, Func<T, WhereClip> func)
            where T : RuleBase, ITableRule 
        {
            if (func == null) return Delete<T>(obj, (WhereClip)null);
            else return Delete<T>(obj, func(obj));
        }


        /// <summary>
        /// 显式使用事务及事务的连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Transaction"></param>
        /// <returns></returns>
        public static InsertClip<T> UseTransaction<T>(this InsertClip<T> Source, DbTransaction Transaction)
            where T : RuleBase
        {
            Source.Transaction = Transaction;
            return Source;
        }

        /// <summary>
        /// 显式使用事务及事务的连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Transaction"></param>
        /// <returns></returns>
        public static UpdationClip<T> UseTransaction<T>(this UpdationClip<T> Source, DbTransaction Transaction)
            where T : RuleBase
        {
            Source.Transaction = Transaction;
            return Source;
        }

        /// <summary>
        /// 显式使用事务及事务的连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Transaction"></param>
        /// <returns></returns>
        public static DeleteClip<T> UseTransaction<T>(this DeleteClip<T> Source, DbTransaction Transaction)
            where T : RuleBase
        {
            Source.Transaction = Transaction;
            return Source;
        }



        /// <summary>
        /// 显式使用事务及事务的连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Connection"></param>
        /// <returns></returns>
        public static SelectClip<T> UseConnection<T>(this SelectClip<T> Source, DbConnection Connection)
            where T : RuleBase
        {
            Source.Connection = Connection;
            return Source;
        }

        /// <summary>
        /// 显式使用事务及事务的连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Connection">强制使用数据连接,可以用于单机事务.</param>
        /// <returns></returns>
        public static InsertClip<T> UseConnection<T>(this InsertClip<T> Source, DbConnection Connection)
            where T : RuleBase
        {
            Source.Connection = Connection;
            return Source;
        }

        /// <summary>
        /// 显式使用事务及事务的连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Connection"></param>
        /// <returns></returns>
        public static UpdationClip<T> UseConnection<T>(this UpdationClip<T> Source, DbConnection Connection)
            where T : RuleBase
        {
            Source.Connection = Connection;
            return Source;
        }

        /// <summary>
        /// 显式使用事务及事务的连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Connection"></param>
        /// <returns></returns>
        public static DeleteClip<T> UseConnection<T>(this DeleteClip<T> Source, DbConnection Connection)
            where T : RuleBase
        {
            Source.Connection = Connection;
            return Source;
        }



        /// <summary>
        /// 直接取出条数。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="whereFunc"></param>
        /// <returns></returns>
        public static int Count<T>(this T obj, Func<T, WhereClip> whereFunc) where T : RuleBase
        {
            SelectClip<T> select = new SelectClip<T>(obj);
            select.CurrentRule = obj;
            select.Dna.Add(obj.Count());
            if (whereFunc != null)
            {
                var where = whereFunc(obj);
                if (where != null) select.Dna.Add(where);
            }
            return select.ToEntity(() => 0);
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="whereFunc"></param>
        /// <returns></returns>
        public static bool Exists<T>(this T obj, Func<T, WhereClip> whereFunc) where T : RuleBase
        {
            SelectClip<T> select = new SelectClip<T>(obj);
            select.CurrentRule = obj;
            select.Dna.Add(obj.Count());
            select.Dna.Add(new TakeClip() { TakeNumber = 1 });
            if (whereFunc != null)
            {
                var where = whereFunc(obj);
                if (where != null) select.Dna.Add(where);
            }
            return select.ToEntity(() => 0) > 0;
        }

        public static T WithReadPast<T>(T Rule)
    where T : RuleBase
        {
            var clone = Rule.Clone() as T;
            clone.SetReconfig(ReConfigEnum.ReadPast);
            return clone;
        }


        public static T WithNoLock<T>(T Rule)
            where T : RuleBase
        {
            var clone = Rule.Clone() as T;
            clone.SetReconfig(ReConfigEnum.NoLock);
            return clone;
        }


        /// <summary>
        /// 查找第一个实体。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="obj"></param>
        /// <param name="func"></param>
        /// <param name="NewModelFunc"></param>
        /// <returns></returns>
        public static TModel FindFirst<T, TModel>(this T obj, Func<T, WhereClip> func, Func<TModel> NewModelFunc)
            where T : RuleBase 
        {
            WhereClip where = null;
            if (func != null) where = func(obj);

            return FindFirst<T, TModel>(obj, where, NewModelFunc);
        }

        ///// <summary>
        ///// 查找第一个实体。
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="TModel"></typeparam>
        ///// <param name="obj"></param>
        ///// <param name="func"></param>
        ///// <param name="Model"></param>
        ///// <returns></returns>
        //public static TModel FindFirst<T, TModel>(this T obj, Func<T, WhereClip> func, TModel Model)
        //    where T : RuleBase, new()
        //    where TModel : new()
        //{
        //    return FindFirst<T, TModel>(obj, func, () => new TModel());
        //}


        /// <summary>
        /// 查找第一个实体。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="obj"></param>
        /// <param name="func"></param>
        /// <param name="Model"></param>
        /// <returns></returns>
        public static TModel FindFirst<T, TModel>(this T obj, Func<T, WhereClip> func, Func<T, TModel> Model)
            where T : RuleBase 
        {
            dbo.Check(Model == null, "实体委托不能为空", obj);

            WhereClip where = null;
            if (func != null) where = func(obj);

            return FindFirst<T, TModel>(obj, where, () => Model(obj));
        }


        public static TModel FindFirst<T, TModel>(this T obj, WhereClip where, Func<TModel> NewModelFunc)
            where T : RuleBase 
        {
            return obj.Select<T>()
                .Where(where)
                .ToEntity<TModel>(NewModelFunc);
        }

        public static object FindScalar<T>(this T obj, Func<T, ColumnClip> funcSelect, Func<T, WhereClip> funcWhere)
            where T : RuleBase
        {
            dbo.Check(funcSelect == null, "查询表达式不能为空.", obj);
            WhereClip where = null;
            if (funcWhere != null) where = funcWhere(obj);

            return obj.Select<T>(new ColumnClip[] { funcSelect(obj) }).Where(where).ToScalar();
        }

    }
}
