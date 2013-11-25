using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using System.Web;
using System.Data.Common;

namespace MyOql
{

    /// <summary>
    /// 更新子句. 
    /// 如果时间列值是 DateTime.MinTime 则忽略该列为Null 
    /// 若要忽略该列, 可使用 SetColumn 方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public partial class UpdationClip<T> : ContextClipBase
        where T : RuleBase
    {
        public UpdationClip()
        {
            this.Key = SqlKeyword.Update;
        }


        /// <summary>
        /// 多次追加 Update Set 值.
        /// </summary>
        /// <remarks> 可以对特殊值进行设定, 如 对DateTime 赋null值.
        /// 
        /// <code>
        ///  dbr.Log.Update(data).Set(o =&gt; o.AddTime == null).Execute();
        /// </code>
        /// </remarks>
        /// <param name="Model"></param>
        /// <returns></returns>
        public UpdationClip<T> Set(Func<T, IModel> Model)
        {
            var nModel = dbo.ModelToDictionary(this.CurrentRule, Model((T)this.CurrentRule));

            var model = this.Dna.First(o => object.Equals(o, null) == false && o.Key == SqlKeyword.Model) as ModelClip;
            if (model != null)
            {
                var objModel = model.Model;

                foreach (var key in nModel.Keys)
                {
                    if (key.EqualsNull() == false)
                    {
                        objModel[key as SimpleColumn] = nModel[key];
                    }
                }
            }
            else
            {
                this.Dna.Add(new ModelClip(this.CurrentRule, nModel));
            }

            return this;
        }

        /// <summary>
        /// 重新设定更新列.
        /// </summary>
        /// <example>
        /// <code>
        /// var Model = new TMemberRule.Entity Model() ;
        /// Model.UpdateMan = MySession.Get(MySessionKey.UserID);
        /// Model.UpdateTime = DateTime.Now;
        /// 
        /// var updateResult = dbr.App.TMember
        ///     .Update(Model)
        ///     .AppendColumns(o =&gt; o.GetColumns()
        ///                                 .Minus(new ColumnClip[] { o.Status, o.ValidateCode, o.LastLoginTime, o.Count, o.RegisterTime })
        ///                 )
        ///     .Execute();
        /// </code>
        /// </example>
        /// <param name="setColumnFunc"></param>
        /// <returns></returns>
        public UpdationClip<T> AppendColumns(Func<T, IEnumerable<ColumnClip>> setColumnFunc)
        {
            if (setColumnFunc != null)
            {
                var cols = setColumnFunc((T)(object)this.CurrentRule);
                if (cols != null)
                {
                    cols.All(o =>
                        {
                            this.Dna.Add(o);
                            return true;
                        });
                }
            }
            return this;
        }

        public UpdationClip<T> RemoveColumns(Func<T, IEnumerable<ColumnClip>> setColumnFunc)
        {
            if (setColumnFunc != null)
            {
                var cols = setColumnFunc((T)(object)this.CurrentRule);
                if (cols != null)
                {
                    cols.All(o =>
                    {
                        this.Dna.RemoveAll(c => c.IsColumn() && (c as ColumnClip).NameEquals(o));
                        return true;
                    });
                }
            }
            return this;
        }

        public UpdationClip<T> AppendColumn(Func<T, ColumnClip> setColumnFunc)
        {
            if (setColumnFunc != null)
            {
                var col = setColumnFunc((T)(object)this.CurrentRule);
                if (col.EqualsNull() == false)
                {
                    this.Dna.Add(col);
                }
            }
            return this;
        }

        public UpdationClip<T> RemoveColumn(Func<T, ColumnClip> setColumnFunc)
        {
            if (setColumnFunc != null)
            {
                var col = setColumnFunc((T)(object)this.CurrentRule);
                if (col.EqualsNull() == false)
                {
                    this.Dna.RemoveAll(c => c.IsColumn() && (c as ColumnClip).NameEquals(col));
                }
            }
            return this;
        }


        // <summary>
        /// 清除其它键，只保留客户端提交的键来设置插入列。
        /// </summary>
        /// <remarks>
        /// 当使用实体进行插入时，由于实体有默认值，而实际提交的Form信息并没有这些值，会导致插入信息的值不正确。
        /// 设置为true，将使用Request.Form.AllKeys 和Rue 的Name 并集对插入实体的 列 进行修正。
        /// </remarks>
        /// <returns></returns>
        public UpdationClip<T> UsePostKeys()
        {
            this.Dna.RemoveAll(o => o.IsColumn());
            if (HttpContext.Current != null && HttpContext.Current.Request.Form != null)
            {
                var keys = HttpContext.Current.Request.Form.AllKeys;
                foreach (var item in this.CurrentRule.GetColumns())
                {
                    if (keys.Contains(item.Name) || keys.Contains(item.DbName))
                    {
                        this.Dna.Add(item);
                    }
                }
            }
            return this;
        }


        public UpdationClip<T> Join<R>(SqlKeyword JoinType, R JoinRule, WhereClip onWhere)
            where R : RuleBase
        {
            onWhere.SetKey(SqlKeyword.OnWhere);
            this.Dna.Add(new JoinTableClip(JoinType) { Table = JoinRule, OnWhere = onWhere });
            return this;
        }

        public UpdationClip<T> Join<R>(R JoinRule, Func<T, R, WhereClip> FuncOnWhere)
            where R : RuleBase
        {
            return Join<R>(SqlKeyword.Join, JoinRule, FuncOnWhere == null ? null : FuncOnWhere((T)(object)this.CurrentRule, JoinRule));
        }

        public UpdationClip<T> Where(WhereClip where)
        {
            AppendWhere(where);
            return this;
        }

        public UpdationClip<T> Where(Func<T, WhereClip> func)
        {
            return Where(func((T)(object)this.CurrentRule));
        }


        public int Execute()
        {
            //自动补全Where条件。
            if (this.Dna.Count(o => o.Key == SqlKeyword.Where) == 0)
            {
                var model = this.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Model) as ModelClip;
                dbo.Check(model == null, "更新的Model 不能为空。", CurrentRule);

                Func<bool> AddWherePks = () =>
                {
                    //根据主键。
                    var pks = this.CurrentRule.GetPrimaryKeys();
                    //命中标志。
                    var hited = pks.Length > 0;
                    WhereClip where = new WhereClip();
                    //检查Model里是否存在主键值。
                    pks.All(o =>
                    {
                        var kvInModel = model.Model.FirstOrDefault(c => c.Key.NameEquals(o));
                        if (kvInModel.HasValue() == false)
                        {
                            hited = false;
                            return false;
                        }

                        where &= o == kvInModel.Value;

                        return true;
                    });

                    if (hited)
                    {
                        this.Where(where);
                    }

                    return hited;
                };


                Func<bool> AddWhereAutoKey = () =>
                    {
                        //根据主键。
                        var ak = this.CurrentRule.GetAutoIncreKey();
                        if (ak.EqualsNull()) return false;

                        var kvInModel = model.Model.FirstOrDefault(c => c.Key.NameEquals(ak));
                        //命中标志。
                        var hited = kvInModel.HasValue();

                        if (hited)
                        {
                            WhereClip where = ak == kvInModel.Value;
                            this.Where(where);
                        }

                        return hited;
                    };

                Func<bool> AddWhereUniKey = () =>
                {
                    //根据主键。
                    var ak = this.CurrentRule.GetUniqueKey();
                    if (ak.EqualsNull()) return false;

                    var kvInModel = model.Model.FirstOrDefault(c => c.Key.NameEquals(ak));
                    //命中标志。
                    var hited = kvInModel.HasValue();

                    if (hited)
                    {
                        WhereClip where = ak == kvInModel.Value;
                        this.Where(where);
                    }

                    return hited;
                };

                dbo.Check(
                    (AddWherePks()
                    || AddWhereAutoKey()
                    || AddWhereUniKey()
                    || Dna.Any(o => o.Key.IsIn(SqlKeyword.Join, SqlKeyword.LeftJoin, SqlKeyword.RightJoin))
                    ) == false,
                    "更新语句找不到所需的Where条件。",
                    this.CurrentRule);
            }

            if (dbo.Event.OnUpdating(this) == false)
            {
                return 0;
            }

            return Execute(this.ToCommand());
        }

        public override int Execute(MyCommand myCmd)
        {
            dbo.Check(myCmd == null, "MyCommand 不能为空!!", this.CurrentRule);
            dbo.Check(myCmd.Command == null, "MyCommand.Command 不能为空!!", this.CurrentRule);

            int retVal = ExecuteBase(myCmd);

            dbo.Event.OnUpdated(this);
            return retVal;
        }

        public override bool ExecuteReader(MyCommand myCmd, params Func<DbDataReader, bool>[] func)
        {
            dbo.Check(myCmd == null, "MyCommand 不能为空!!", this.CurrentRule);
            dbo.Check(myCmd.Command == null, "MyCommand.Command 不能为空!!", this.CurrentRule);

            var retVal = ExecuteReaderBase(myCmd, func);

            dbo.Event.OnUpdated(this);
            return retVal;

        }
    }
}