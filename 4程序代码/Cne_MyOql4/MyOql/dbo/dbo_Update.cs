using System;
using System.Collections.Generic;
using MyCmn;
using System.Web;
using System.Linq;

namespace MyOql
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class dboRule
    {


        /// <summary>
        /// 更新三种 Model： 1. 实体类 2.Dictionary&lt;string,object> or Dictionary&lt;string,string>  3. WhereClip
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="Model"></param>
        /// <param name="where">如果where 为空，则按主键进行。</param>
        /// <param name="setColumnFunc">设置更新列.</param>
        /// <returns></returns>
        public static UpdationClip<T> Update<T>(this T obj, IModel Model, WhereClip where, Func<T, IEnumerable<ColumnClip>> setColumnFunc)
            where T : RuleBase, ITableRule
        {
            dbo.Check(object.Equals(Model, null), "要更新的实体不能为空", obj);

            UpdationClip<T> update = new UpdationClip<T>();
            update.CurrentRule = obj;

            if (where != null) update.Dna.Add(where);

            update.Dna.Add(new ModelClip(obj, Model));

            if (setColumnFunc != null)
            {
                update.AppendColumns(setColumnFunc);
            }
            return update;
        }


        /// <summary>
        /// 根据主键进行更新. 如果没有主键,则会出错. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="SetValueExpressionFunc"></param>
        /// <returns></returns>
        public static UpdationClip<T> Update<T>(this T obj, Func<T, IModel> SetValueExpressionFunc)
            where T : RuleBase, ITableRule 
        {
            GodError.Check(SetValueExpressionFunc == null, "要更新实体不能为空");

            return Update<T>(obj, SetValueExpressionFunc(obj), (WhereClip)null);
        }



        /// <summary>
        /// 生成 Update 子句。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="SetValueExpressionFunc"></param>
        /// <param name="WhereFunc"></param>
        /// <returns></returns>
        public static UpdationClip<T> Update<T>(this T obj, Func<T, IModel> SetValueExpressionFunc, Func<T, WhereClip> WhereFunc)
            where T : RuleBase, ITableRule 
        {
            return Update(obj, SetValueExpressionFunc, WhereFunc, null);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="SetValueExpressionFunc">更新的Model回调</param>
        /// <param name="WhereFunc">更新条件</param>
        /// <param name="setColumnFunc"></param>
        /// <returns></returns>
        public static UpdationClip<T> Update<T>(this T obj, Func<T, IModel> SetValueExpressionFunc, Func<T, WhereClip> WhereFunc, Func<T, IEnumerable<ColumnClip>> setColumnFunc)
            where T : RuleBase, ITableRule 
        {
            dbo.Check(SetValueExpressionFunc == null, "要更新实体不能为空", obj);

            WhereClip where = null;
            if (WhereFunc != null) where = WhereFunc(obj);

            return Update<T>(obj, SetValueExpressionFunc(obj), where, setColumnFunc);
        }




        /// <summary>
        /// 根据主键更新。
        /// </summary>
        /// <example>
        /// <code>
        /// var model = new UserRule.Entity();
        /// model.DeptID = 1 ;
        /// model.Name = "张三" ;
        /// model.AddTime = DateTime.Now ;
        /// model.ID = 15 ;
        /// 
        /// var retval = dbr.User
        ///             .Update(model) // 根据主键更新.
        ///             .Execute() ;
        /// </code>
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="Model"></param>
        /// <returns></returns>
        public static UpdationClip<T> Update<T>(this T obj, IModel Model)
            where T : RuleBase, ITableRule
        {
            WhereClip where = null;
            return Update(obj, Model, where);
        }


        /// <summary>
        /// 用 Model 更新记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="Model"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static UpdationClip<T> Update<T>(this T obj, IModel Model, Func<T, WhereClip> func)
            where T : RuleBase, ITableRule
        {
            dbo.Check(object.Equals(Model, null), "要更新的实体不能为空", obj);
            WhereClip where = null;
            if (func != null) where = func(obj);

            return Update(obj, Model, where);
        }

        /// <summary>
        /// 用 Model 更新记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="Model"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static UpdationClip<T> Update<T>(this T obj, IModel Model, Func<T, IEnumerable<ColumnClip>> func)
            where T : RuleBase, ITableRule
        {
            dbo.Check(object.Equals(Model, null), "要更新的实体不能为空", obj);

            var dict = dbo.ModelToDictionary(obj, Model);
            WhereClip where = new WhereClip();
            if (func != null)
            {
                var cols = func.Invoke(obj);
                foreach (var item in cols)
                {
                    where &= item == dict[item];
                }
            }
            return Update(obj, dict, where);
        }

        /// <summary>
        /// 用 Model 更新记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="Model"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static UpdationClip<T> Update<T>(this T obj, IModel Model, Func<T, ColumnClip> func)
            where T : RuleBase, ITableRule
        {
            dbo.Check(object.Equals(Model, null), "要更新的实体不能为空", obj);

            ColumnClip[] columns = null;
            if (func != null)
            {
                columns = new ColumnClip[1];
                columns[0] = func(obj);
            }

            return Update(obj, Model, o => { return columns; });
        }

        /// <summary>
        /// 更新三种 Model： 1. 实体类 2.Dictionary&lt;string,object> or Dictionary&lt;string,string>  3. WhereClip
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="Model"></param>
        /// <param name="where">如果where 为空，则按主键进行。</param>
        /// <returns></returns>
        public static UpdationClip<T> Update<T>(this T obj, IModel Model, WhereClip where)
            where T : RuleBase, ITableRule
        {
            return Update<T>(obj, Model, where, null);
        }
    }
}
