using System;
using MyCmn;
using System.Collections.Generic;
using System.Linq;

namespace MyOql
{
    public partial class IDboEvent
    {
        public virtual bool OnProcing(ContextClipBase Context)
        {
            if (Procing != null) Procing(Context);
            return true;
        }
        public virtual void OnProced(ContextClipBase Context)
        {
            var Entity = Context.CurrentRule;
            //清除关联视图缓存。

            var dbName = Entity.GetDbName();
            var dict = Idbr.GetProcRelationTables();
            if (dict.ContainsKey(dbName))
            {
                dict[dbName].All(o =>
                    {
                        OnCacheRemoveAll(new MyContextClip(Context, Idbr.GetMyOqlEntity(o)));
                        return true;
                    });
            }

            LogTo(MyOqlActionEnum.Proc, Context, InfoEnum.Info, () => string.Format("成功成执存储过程: {0}", Entity.GetFullName()));
        }
        public virtual bool OnCreating(ContextClipBase Context)
        {
            //OnPreModel(Context);

            //if (Context.Dna.Any(o => o.Key == SqlKeyword.Select) == false)
            //{
            //    if (Context.GetUsePower().Contains(MyOqlActionEnum.Create) == false) return true;
            //}

            if (Creating != null) return Creating(Context);
            return true;
        }

        public virtual bool OnBulkInserting(ContextClipBase Context)
        {
            if (BulkInserting == null) return true;
            if (Context.ContainsConfig(ReConfigEnum.SkipPower)) return true;

            if (BulkInserting != null) return BulkInserting(Context);
            return true;
        }

        public virtual bool OnBulkUpdating(ContextClipBase Context)
        {
            if (BulkUpdating == null) return true;
            if (Context.ContainsConfig(ReConfigEnum.SkipPower)) return true;

            if (BulkUpdating != null) return BulkUpdating(Context);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Context"></param>
        /// <returns>返回 false 说明没有查询权限. 返回 true ,表示有查询权限,并已处理查询列.</returns>
        public virtual bool OnReading(ContextClipBase Context)
        {
            //以下并不能保证子查询里没有行集表
            //var entPower = Context.GetUsePower();
            //if (entPower.Contains(MyOqlActionEnum.Read) == false &&
            //    entPower.Contains(MyOqlActionEnum.Row) == false)
            //    return true;

            if (Reading != null) return Reading(Context);
            return true;
        }
        public virtual void OnReaded(ContextClipBase Context)
        {
            //if (Readed == null) return;

            Func<string> idFunc = () =>
            {
                var where = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Where) as WhereClip;
                if (where.IsNull() == false)
                {
                    return where.GetIdValue(Context.CurrentRule);
                }
                return null;
            };

            //Readed(Context.CurrentRule, idFunc());
            var Id = idFunc();
            LogTo(MyOqlActionEnum.Read, Context, InfoEnum.Info, () => string.Format("成功读取Id值是 {0} 的 {1} 表数据", Id, Context.CurrentRule.GetFullName()));
        }


        public virtual void OnDeleted(ContextClipBase Context)
        {
            //if (Deleted == null) return;

            Func<string> idFunc = () =>
            {
                var where = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Where) as WhereClip;
                if (where.IsNull() == false)
                {
                    return where.GetIdValue(Context.CurrentRule);
                }
                return null;
            };

            if (Context.AffectRow == 0)
            {
                LogTo(MyOqlActionEnum.Delete, Context, InfoEnum.Info, () => string.Format("未删除 {0} 表, Id:", Context.CurrentRule.GetFullName(), idFunc()));
            }
            else
            {
                var Id = idFunc();
                OnCacheRemoveById(Context, Id);
                OnCacheRemoveBySql(Context);

                ////清除关联视图缓存。

                //GetCacheViewBy(Context.CurrentRule).All(o =>
                //    {
                //        var ent = Idbr.GetMyOqlEntity(o);
                //        var cont = new MyContextClip(Context, ent);
                //        OnCacheRemoveBySql(cont);
                //        OnCacheRemoveById(cont, Id);
                //        return true;
                //    });

                LogTo(MyOqlActionEnum.Delete, Context, InfoEnum.Info, () => string.Format("成功删除Id值是 {0} 的 {1} 表数据", Id, Context.CurrentRule.GetFullName()));
            }


            //级联删除式破坏缓存
            dbo.Event.Idbr.GetFkDefines()
                .Where(o => o.RefTable == Context.CurrentRule.GetDbName() && o.CascadeDelete)
                .All(o =>
                    {
                        var ent = dbo.Event.Idbr.GetMyOqlEntity(o.Entity);
                        if (ent != null)
                        {
                            OnDeleted(new MyContextClip(Context, ent));
                        }
                        return true;
                    });
        }

        public virtual void OnCreated(ContextClipBase Context)
        {
            //if (Created == null) return;

            OnCacheRemoveBySql(Context);

            ////清除关联视图缓存。
            //GetCacheViewBy(Context.CurrentRule).All(o =>
            //    {
            //        OnCacheRemoveBySql(new MyContextClip(Context, Idbr.GetMyOqlEntity(o)));
            //        return true;
            //    });


            if (Context.Dna.Exists(o => !o.IsDBNull() && o.Key == SqlKeyword.Model))
            {

                LogTo(MyOqlActionEnum.Create, Context, InfoEnum.Info, () =>
                {
                    var model = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Model) as ModelClip;

                    var IdValue = model.GetIdValue();

                    return string.Format("成功创建Id值是 {0} 的 {1} 表数据", IdValue, Context.CurrentRule.GetFullName());
                });
            }
            else
            {
                LogTo(MyOqlActionEnum.Create, Context, InfoEnum.Info, () => string.Format("批量创建了 {0} 的表数据", Context.CurrentRule.GetFullName()));
            }
        }

        public virtual bool OnDeleting(ContextClipBase Context)
        {
            //var where = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Where) as WhereClip;
            //if (where != null)
            //{
            //    //Delete 语句，没有 Where In (Select)子句，没有启用删除权限，且没有启用行集权限的情况下，直接执行。
            //    if (where.HasInQuery() == false &&
            //        Context.GetUsePower().Contains(MyOqlActionEnum.Delete) == false &&
            //        Context.GetUsePower().Contains(MyOqlActionEnum.Row) == false)
            //    {
            //        return true;
            //    }
            //}

            if (Deleting != null) return Deleting(Context);
            return true;
        }

        public virtual bool OnUpdating(ContextClipBase Context)
        {

            //OnPreModel(Context);


            //if (Context.Dna.Any(o => o.Key.IsIn(SqlKeyword.Join, SqlKeyword.Join, SqlKeyword.RightJoin)) == false)
            //{
            //    //以下并不能保证子查询里没有行集表。
            //    //既没启用更新，也没启用行集。

            //    var where = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Where) as WhereClip;

            //    //Where 是不能为Null 的。
            //    if (where != null)
            //    {
            //        if (where.HasInQuery() == false &&
            //            Context.GetUsePower().Contains(MyOqlActionEnum.Update) == false &&
            //            Context.GetUsePower().Contains(MyOqlActionEnum.Read) == false &&
            //            Context.GetUsePower().Contains(MyOqlActionEnum.Row) == false
            //            ) return true;
            //    }
            //}

            if (Updating != null) return Updating(Context);
            return true;
        }
        public virtual void OnUpdated(ContextClipBase Context)
        {
            //if (Updated == null) return;

            Func<string> IdFunc = () =>
            {
                var where = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Where) as WhereClip;
                if (where.IsNull() == false)
                {
                    return where.GetIdValue(Context.CurrentRule);
                }
                return null;
            };

            var model = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Model) as ModelClip;

            //Updated(Context.CurrentRule, IdFunc(), Context.AffectRow, model);
            if (Context.AffectRow == 0)
            {
                LogTo(MyOqlActionEnum.Update, Context, InfoEnum.Info, () => string.Format("未更新 {0} 表, Id:", Context.CurrentRule.GetFullName(), IdFunc()));
            }
            else
            {
                var Id = IdFunc();
                OnCacheRemoveById(Context, Id);
                OnCacheRemoveBySql(Context);

                ////清除关联视图缓存。
                //GetCacheViewBy(Context.CurrentRule).All(o =>
                //{
                //    var ent = Idbr.GetMyOqlEntity(o);
                //    var cout = new MyContextClip(Context, ent);
                //    OnCacheRemoveBySql(cout);
                //    OnCacheRemoveById(cout, Id);
                //    return true;
                //});


                if (Id.HasValue())
                {
                    LogTo(MyOqlActionEnum.Update, Context, InfoEnum.Info, () => string.Format("成功更新Id值是 {0} 的 {1} 表数据", Id, Context.CurrentRule.GetFullName()));
                }
                else
                {
                    LogTo(MyOqlActionEnum.Update, Context, InfoEnum.Info, () => string.Format("成功批量更新 {0} 表的 {1} 条数据", Context.CurrentRule.GetFullName(), Context.AffectRow));
                }
            }

            //级联删除式破坏缓存
            dbo.Event.Idbr.GetFkDefines()
                .Where(o => o.RefTable == Context.CurrentRule.GetDbName() && o.CascadeUpdate)
                .All(o =>
                {
                    var ent = dbo.Event.Idbr.GetMyOqlEntity(o.Entity);
                    if (ent != null)
                    {
                        OnUpdated(new MyContextClip(Context, ent));
                    }
                    return true;
                });

        }

        public virtual void OnGenerateSqled(DatabaseType dbType, MyCommand myCmd)
        {
            if (GenerateSqled == null) return;

            GenerateSqled(dbType, myCmd);
        }

        public virtual bool OnExecute(ContextClipBase Context)
        {
            if (Executing == null) return true;

            return Executing(Context);
        }

        /// <summary>
        /// 解密数据库连接字符串的事件。 
        /// </summary>
        /// <example>
        /// 应该在 Application_Start 事件中注册如下事件的代码
        /// <code>
        /// dbo.DecrypteEvent += (arg) =>
        /// {
        ///     return MyCmn.Security.DecrypteString(arg);
        /// };
        /// </code>
        /// </example>
        public virtual event Func<string, string> DecrypteEvent = null;

        /// <summary>
        /// 解密数据库连接字符串的方法。 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public virtual string OnDecrypte(string Value)
        {
            if (DecrypteEvent != null)
            {
                return DecrypteEvent(Value);
            }
            else return Value;
        }

        public virtual void LogTo(MyOqlActionEnum Action, ContextClipBase Context, InfoEnum LogType, Func<string> LogFunc)
        {
            if (Action == 0) return;
            var Entity = Context.CurrentRule;
            if ((Context.GetUseLog() & Action) == 0) return;
            if (LogFunc == null) return;

            var msg = LogFunc();
            if (msg.HasValue() == false) return;

            Log.To(InfoEnum.MyOql | LogType, Entity.GetFullName().DbName + ":" + msg);
        }

        //public virtual void LogToEntity(MyOqlActionEnum Action, RuleBase Entity, InfoEnum LogType, Func<string> LogFunc)
        //{
        //    if (Action == 0) return;
        //    if ((Entity.GetUseLog() & Action) == 0) return;
        //    if (LogFunc == null) return;

        //    var msg = LogFunc();
        //    if (msg.HasValue() == false) return;

        //    Log.To(InfoEnum.MyOql | LogType, Entity.GetFullName().DbName + ":" + msg, "MyOql");
        //}


    }

    public enum InfoEnum
    {
        Info = 1,
        Error = 2,
        System = 4,
        MyOql = 8,
        User = 16,
        Warn = 32,
        Sql = 64,
        MyCmn = 128,
        Database = 256,
        Extern = 512,
        App = 1024,
        Unkown = 2048
    }
}
