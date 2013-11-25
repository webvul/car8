using System;
using System.Collections;
using System.Collections.Generic;
using MyCmn;
using System.Linq;
using System.Web;

namespace MyOql
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class dboRule
    {


        /// <summary>
        /// 三种Model，1.实体类， 2. Dictionary&lt;string,object&gt; or Dictionary&lt;string,string&gt; 3.WhereClip
        /// </summary>
        /// <remarks>
        ///  Model的形式可以是多种多样的.可以是 三种Model，1.实体类， 2. Dictionary&lt;string,object&gt; or Dictionary&lt;string,string&gt; 3.WhereClip .
        ///  只要类的属性和数据实体属性名相同,即可插入记录.同时忽略不匹配的属性名.
        /// </remarks>
        /// <example>
        /// <code>
        ///  var model = new UserRule.Entity();
        ///  model.DeptID = 1 ;
        ///  model.Name = "张三" ;
        ///  model.AddTime = DateTime.Now ;
        /// 
        ///  var retval = dbr.User
        ///               .Insert(model)
        ///               .Execute() ;
        ///  </code>    
        ///  <code>
        /// var dict = new XmlDictionary&lt;string,object&gt;
        /// dict["DeptID"] = 1 ;
        /// dict[dbr.User.Name.Name] = "张三" ;
        /// dict[dbr.User.AddTime.Name] = DateTime.Now ;
        /// 
        /// var retval = dbr.User
        ///             .Insert(dict)
        ///             .Execute();
        /// </code>
        /// Model也可认是Where表达式,但不能以Where表达式理解,Where表达式只用来表示数据存储,这样写法更简便.
        /// 这种方法不能返回即插入对象值,如ID值例如:
        ///<code>
        ///var retval = dbr.User
        ///            .Insert(o=&gt;o.DeptID == 1 &amp; o.Name == "张三" &amp; o.AddTime == DateTime.Now) 
        ///            .Execute(); 
        /// </code>
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="Model"></param>
        /// <returns></returns>
        public static InsertClip<T> Insert<T>(this T obj, IModel Model)
            where T : RuleBase, ITableRule
        {
            dbo.Check(object.Equals(Model, null), "插入的实体不能为空", obj);

            InsertClip<T> insert = new InsertClip<T>();
            insert.CurrentRule = obj;


            var model = new ModelClip(obj, Model);
            if (model.Model == null)
            {
                var modelClip = Model as ContextClipBase;

                //udi,待完成.
                dbo.Check(modelClip == null, "插入的实体为空,或不是Select子句.", obj);
                insert.Dna.Add(modelClip);
            }
            else
            {
                insert.Dna.Add(model);
            }
            return insert;
        }

        /// <summary>
        /// 批量插入并执行.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="obj"></param>
        /// <param name="Model"></param>
        /// <returns></returns>
        public static InsertClip<T> Insert<T, T2>(this T obj, SelectClip<T2> Model)
            where T : RuleBase, ITableRule
            where T2 : RuleBase
        {
            dbo.Check(object.Equals(Model, null), "插入的实体不能为空", obj);

            InsertClip<T> insert = new InsertClip<T>();
            insert.CurrentRule = obj;

            //批量插入时使用.
            insert.Dna.Add(Model);

            return insert;
        }


        /// <summary>
        /// 返回的 object ，仅限三种类型： 1.实体类。2.Dictionary , 3.WhereClip
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="SetValueExpressionFunc"></param>
        /// <returns></returns>
        public static InsertClip<T> Insert<T>(this T obj, Func<T, IModel> SetValueExpressionFunc)
            where T : RuleBase, ITableRule 
        {
            dbo.Check(SetValueExpressionFunc == null, "要插入的实体不能为空", obj);
            return Insert<T>(obj, SetValueExpressionFunc(obj));
        }


        /// <summary>
        /// 自动保存。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="FuncModel"></param>
        /// <param name="FuncWhereColumn"></param>
        /// <returns></returns>
        public static int AutoSave<T>(this T obj, Func<T, IModel> FuncModel, Func<T, ColumnClip> FuncWhereColumn)
            where T : RuleBase, ITableRule
        {
            return AutoSave<T>(obj, FuncModel, FuncWhereColumn, null);
        }



        /// <summary>
        /// 无则插入,有则更新.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="FuncModel"></param>
        /// <param name="FuncWhereColumn">如果ColumnClip为空,则用主键判断是否存在记录.</param>
        /// <param name="SetColumnsFunc"></param>
        /// <returns></returns>
        public static int AutoSave<T>(this T obj, Func<T, IModel> FuncModel, Func<T, ColumnClip> FuncWhereColumn, Func<T, IEnumerable<ColumnClip>> SetColumnsFunc)
            where T : RuleBase, ITableRule
        {
            IModel model = null;
            if (FuncModel != null) model = FuncModel(obj);

            ColumnClip col = null;
            if (FuncWhereColumn != null) col = FuncWhereColumn(obj);

            return AutoSave<T>(obj, model, new ColumnClip[] { col }, SetColumnsFunc);
        }

        /// <summary>
        /// 自动保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="FuncModel"></param>
        /// <param name="FuncWhereColumns"></param>
        /// <returns></returns>
        public static int AutoSave<T>(this T obj, Func<T, IModel> FuncModel, Func<T, IEnumerable<ColumnClip>> FuncWhereColumns)
            where T : RuleBase, ITableRule
        {
            return AutoSave<T>(obj, FuncModel, FuncWhereColumns, (Func<T, IEnumerable<ColumnClip>>)null);
        }
        /// <summary>
        /// 无则插入,有则更新.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="FuncModel"></param>
        /// <param name="FuncWhereColumns">如果ColumnClip为空,则用主键判断是否存在记录.</param>
        /// <param name="SetColumnsFunc"></param>
        /// <returns></returns>
        public static int AutoSave<T>(this T obj, Func<T, IModel> FuncModel, Func<T, IEnumerable<ColumnClip>> FuncWhereColumns, Func<T, IEnumerable<ColumnClip>> SetColumnsFunc)
            where T : RuleBase, ITableRule
        {
            IModel model = null;
            if (FuncModel != null) model = FuncModel(obj);

            IEnumerable<ColumnClip> cols = null;
            if (FuncWhereColumns != null) cols = FuncWhereColumns(obj);

            return AutoSave<T>(obj, model, cols, SetColumnsFunc);
        }


        /// <summary>
        /// 单条数的 数据推送(无则插入,有则更新).尽量不要使用,它用生成两次操作. 一次是查询是否存在, 一次是执行.
        /// 如果查询出多条，则会抛出异常。
        /// </summary>
        /// <example>
        /// var model = new UserRule.Entity();
        /// model.DeptID = 1 ;
        /// model.Name = "张三" ;
        /// model.AddTime = DateTime.Now ;
        /// model.ID = 15 ;
        /// 
        /// var retval = dbr.User
        ///             .AutoSave(model,o=&lt;o.ID);
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="Model"></param>
        /// <param name="WhereColumns">如果为空,则用Model里主键,唯一键,或自增键判断是否存在记录.</param>
        /// <param name="UpdateSetColumnsFunc">更新实体时，更新的字段</param>
        /// <returns></returns>
        private static int AutoSave<T>(this T obj, IModel Model, IEnumerable<ColumnClip> WhereColumns, Func<T, IEnumerable<ColumnClip>> UpdateSetColumnsFunc)
            where T : RuleBase, ITableRule
        {
            dbo.Check(object.Equals(Model, null), "要保存的实体不能为空", obj);

            var dit = dbo.ModelToDictionary(obj, Model);


            Func<WhereClip> GetWhere = () =>
            {
                WhereClip _where = new WhereClip();

                if (obj.GetColumns().All(o =>
                {
                    if (dit.Any(c => c.Key.NameEquals(o)) == false)
                    {
                        return false;
                    }
                    _where &= o == dit.First(c => c.Key.NameEquals(o));
                    return true;
                }
                    ))
                {
                    return _where;
                }


                {
                    var IdKey = obj.GetUniqueKey();
                    if (IdKey.EqualsNull() == false)
                    {
                        if (dit.Any(c => c.Key.NameEquals(IdKey)))
                        {
                            return IdKey == dit.First(o => o.Key.NameEquals(IdKey));
                        }
                    }
                }

                {
                    var IdKey = obj.GetAutoIncreKey();
                    if (IdKey.EqualsNull() == false)
                    {
                        if (dit.Any(c => c.Key.NameEquals(IdKey)))
                        {
                            return IdKey == dit.First(o => o.Key.NameEquals(IdKey));
                        }
                    }
                }


                throw new GodError("AutoSave 找不到Where条件.");
            };

            WhereClip where = null;


            if (WhereColumns == null || !WhereColumns.Any())
            {
                where = GetWhere();
            }
            else
            {
                foreach (var col in WhereColumns)
                {
                    if (col.EqualsNull()) continue;
                    where &= col == dit.First(o => o.Key.Name == col.Name).Value;
                }
            }

            var cou = obj.FindScalar<T>(o => { return obj.Count(); }, o => { return where; }).AsInt();
            if (cou == 0)
            {
                return Insert(obj, dit).Execute();
            }
            else if (cou == 1)
            {
                return Update(obj, dit, where).AppendColumns(UpdateSetColumnsFunc).Execute();
            }
            else
            {
                throw new GodError("AutoSave 从数据库中找到多条记录.");
            }
        }

    }
}
