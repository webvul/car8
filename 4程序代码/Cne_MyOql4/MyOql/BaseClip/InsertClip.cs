using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using System.Data;
using System.Data.Common;
using System.Web;

namespace MyOql
{
    /// <summary>
    /// 插入子句.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InsertClip<T> : ContextClipBase
    {
        /// <summary>
        /// 返回插入表记录的自增ID.
        /// </summary>
        public long LastAutoID { get; set; }

        public InsertClip()
        {
            this.Key = SqlKeyword.Insert;
        }

        /// <summary>
        /// 指定更新列。如果为空，更新所有列。
        /// </summary>
        private List<ColumnClip> Columns { get; set; }

        public InsertClip<T> AppendColumns(Func<T, IEnumerable<ColumnClip>> setColumnFunc)
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

        public InsertClip<T> RemoveColumns(Func<T, IEnumerable<ColumnClip>> setColumnFunc)
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

        public InsertClip<T> AppendColumn(Func<T, ColumnClip> setColumnFunc)
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

        public InsertClip<T> RemoveColumn(Func<T, ColumnClip> setColumnFunc)
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

        /// <summary>
        /// 执行插入,如果有自增列,或计算列, 则更新Model.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public int Execute()
        {
            if (dbo.Event.OnCreating(this) == false) return 0;
            return Execute(this.ToCommand());
        }

        /// <summary>
        /// 执行插入,如果有自增列,或计算列, 则更新Model. 
        /// </summary>
        /// <param name="myCmd">
        /// A <see cref="MyCommand"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public override int Execute(MyCommand myCmd)
        {
            var cmd = myCmd.Command;

            if (cmd == null) return 0;
            var ret = 0;


            if (dbo.Event.OnExecute(this) == false) return 0;

            //当没有Model时，是 Insert Tab (cols) Select cols from Tab 语句
            if (this.Dna.Exists(o => !o.IsDBNull() && o.Key == SqlKeyword.Model) == false)
            {
                ret = ExecuteBase(myCmd);
                dbo.Event.OnCreated(this);
                return ret;
            }

            /*
                   * 在有自增列时，如果参数里含有 LastAutoID， 取之。 否则取Scalar。兼容了一下 Oracle.
                   */
            var mc = this.Dna.First(o => !o.IsDBNull() && o.Key == SqlKeyword.Model) as ModelClip;
            var OriModel = mc.OriginalModel;
            var dictModel = mc.Model;


            if (myCmd.ExecuteType == ExecuteTypeEnum.Select)
            {
                dbo.Open(myCmd, this, () =>
                {
                    if (this.CurrentRule.GetAutoIncreKey().EqualsNull() == false)
                    {
                        this.LastAutoID = cmd.ExecuteScalar().AsLong();
                        myCmd.LastAutoID = this.LastAutoID;

                        if (this.LastAutoID > 0)
                        {
                            dbo.UpdateOneProperty(OriModel, this.CurrentRule.GetAutoIncreKey(), this.LastAutoID);
                            dbo.UpdateOneProperty(dictModel, this.CurrentRule.GetAutoIncreKey(), this.LastAutoID);
                            ret = 1;
                        }

                        UpdateComputeKeys(OriModel, dictModel);
                    }
                    else
                    {
                        UpdateTriggerKeys(cmd, OriModel, dictModel);

                        ret = 1;
                    }

                    //cmd.Parameters.Clear();
                    return true;
                });
            }
            else if (myCmd.ExecuteType == ExecuteTypeEnum.Execute)
            {
                ret = ExecuteBase(myCmd);

                ////更新自增ID,好像没有用到,可能OleDb用到。 2012年6月4日10:44:00
                //if (myCmd.LastAutoID > 0)
                //{
                //    this.LastAutoID = myCmd.LastAutoID;
                //    dbo.UpdateOneProperty(OriModel, this.CurrentRule.GetAutoIncreKey(), myCmd.LastAutoID);
                //    dbo.UpdateOneProperty(dictModel, this.CurrentRule.GetAutoIncreKey(), myCmd.LastAutoID);
                //}


                UpdateComputeKeys(OriModel, dictModel);
            }

            dbo.Event.OnCreated(this);
            return ret;
        }

        /// <summary>
        /// 不执行破坏缓存，不执行  OnCreated 方法
        /// </summary>
        /// <param name="myCmd"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public override bool ExecuteReader(MyCommand myCmd, params Func<DbDataReader, bool>[] func)
        {
            var retVal = ExecuteReaderBase(myCmd, func);
            return retVal;
        }


        private int UpdateReturnValueParameter(DbCommand cmd, IModel OriModel, XmlDictionary<ColumnClip, object> dictModel)
        {
            for (int i = cmd.Parameters.Count - 1; i >= 0; i--)
            {
                if (cmd.Parameters[i].Direction == ParameterDirection.ReturnValue)
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                        this.LastAutoID = cmd.Parameters[i].Value.AsLong();
                        if (this.LastAutoID > 0)
                        {
                            dbo.UpdateOneProperty(OriModel, this.CurrentRule.GetAutoIncreKey(), this.LastAutoID);
                            dbo.UpdateOneProperty(dictModel, this.CurrentRule.GetAutoIncreKey(), this.LastAutoID);
                            return 1;
                        }
                        else return 0;
                    }
                    catch (Exception e)
                    {
                        e.ThrowError(cmd);
                    }
                }
            }
            return 0;
        }


        private void UpdateTriggerKeys(DbCommand cmd, IModel OriModel, XmlDictionary<ColumnClip, object> dictModel)
        {
            //else 的判断条件是：
            /*
            if (Context.CurrentRule.GetUniqueKey().EqualsNull())
            {
                var computeKyes = Context.CurrentRule.GetComputeKeys();
                if (computeKyes.Length > 0)
                {
                    var interC = computeKyes.Intersect(Context.CurrentRule.GetPrimaryKeys()).Count();
                    if (interC > 0)
                    {
                        myCmd.ExecuteType = ExecuteTypeEnum.Select;
                    }
                }
            }
            */
            //更新的是计算列中的主键

            if (this.CurrentRule.GetUniqueKey().EqualsNull())
            {
                var computeKyes = this.CurrentRule.GetComputeKeys();
                if (computeKyes.Length > 0)
                {
                    var interCol = computeKyes.Intersect(this.CurrentRule.GetPrimaryKeys()).ToArray();

                    //如果有触发器返回结果，那么应该一次性的把计算列全部返回。
                    if (interCol.Length == 1)
                    {
                        var cp = cmd.ExecuteScalar();
                        if (cp.IsDBNull() == false)
                        {
                            dbo.UpdateOneProperty(OriModel, interCol[0], cp);
                            dbo.UpdateOneProperty(dictModel, interCol[0], cp);
                        }
                    }
                    else if (interCol.Length > 1)
                    {

                        using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (reader.Read())
                            {
                                foreach (var col in interCol)
                                {
                                    var cvi = reader.GetOrdinal(col.DbName);
                                    if (cvi < 0) cvi = reader.GetOrdinal(col.Name);
                                    if (cvi < 0) continue;

                                    var cv = reader.GetValue(cvi);
                                    if (cv.IsDBNull()) continue;

                                    dbo.UpdateOneProperty(OriModel, col, cv);
                                    dbo.UpdateOneProperty(dictModel, col, cv);
                                }
                                break;
                            }
                        }
                    }
                    else throw new GodError("当返回Select命令时，要求该表有自增列，或，该表触发器有返回结果（计算列中包含主键），表：" + this.CurrentRule.GetFullName());
                }
            }
        }


        private void UpdateComputeKeys(IModel OriModel, XmlDictionary<ColumnClip, object> dictModel)
        {
            if (this.CurrentRule.GetComputeKeys().Length > 0)
            {
                WhereClip where = new WhereClip();
                var idKey = this.CurrentRule.GetIdKey();
                if (dbo.EqualsNull(idKey) == false)
                {
                    where &= idKey == dictModel.First(o => o.Key.NameEquals(idKey.DbName,true)).Value;
                }
                else
                {
                    foreach (var item in this.CurrentRule.GetPrimaryKeys())
                    {
                        where &= item == dictModel.First(o => o.Key.NameEquals(item.DbName,true)).Value; ;
                    }
                }


                var selectCmd = this.CurrentRule.Select().Where(where).ToCommand();

                if (ExecuteReader(selectCmd, o =>
                {
                    dbo.ToEntity(o, () => OriModel);
                    var model = dbo.ModelToDictionary(this.CurrentRule, OriModel);

                    foreach (var item in model)
                    {
                        dictModel[item.Key as SimpleColumn] = item.Value;
                    }
                    return false;
                }))
                {

                }
            }
        }

        /// <summary>
        /// 清除其它键，只保留客户端提交的键来设置插入列。
        /// </summary>
        /// <remarks>
        /// 当使用实体进行插入时，由于实体有默认值，而实际提交的Form信息并没有这些值，会导致插入信息的值不正确。
        /// 设置为true，将使用Request.Form.AllKeys 和Rue 的Name 并集对插入实体的 列 进行修正。
        /// </remarks>
        /// <returns></returns>
        public InsertClip<T> UsePostKeys()
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

    }
}
