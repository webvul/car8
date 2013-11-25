using System;
using System.Linq;
using System.Data.Common;
using MyCmn;
using System.Data;

namespace MyOql.Provider
{
    /// <summary>
    /// 数据库解析Sql基类。
    /// </summary>
    /// <remarks>
    /// 
    /// 由于参数不能同时被添加到多个参数集合中， 所以在添加完参数后， 清空参数。
    /// 否则报错：
    ///     “另一个 SqlParameterCollection 中已包含 SqlParameter。”
    ///     
    /// 但在执行完成后， 可以不清除。
    /// </remarks>
    [Serializable]
    public abstract partial class TranslateSql : ICloneable
    {
        //private int _ParaIndex = 0;
        //protected int ParaIndex
        //{
        //    get
        //    {
        //        if (_ParaIndex > 9999)
        //            _ParaIndex = 0;
        //        return _ParaIndex++;
        //    }
        //}
        [NonSerialized]
        private ContextClipBase _Context;

        public ContextClipBase Context { get { return _Context; } protected set { _Context = value; } }

        /// <summary>
        /// 解析入口。要根据合适的数据库类型进行调用.
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        public virtual MyCommand ToCommand(ContextClipBase Context, bool IsTop = true)
        {
            this.Context = Context;

            MyCommand myCmd = new MyCommand();
            myCmd.CurrentAction = Context;

            if (Context.Transaction != null && Context.Transaction.Connection != null)
            {
                myCmd.Command = Context.Transaction.Connection.CreateCommand();
                myCmd.Command.Transaction = Context.Transaction;
            }
            else if (Context.Connection != null)
            {
                myCmd.Command = Context.Connection.CreateCommand();
            }
            else
            {
                var conn = GetConnection(Context.CurrentRule.GetConfig().db);
                myCmd.Command = conn.CreateCommand();
            }

            myCmd.Command.CommandTimeout = Context.CurrentRule.GetConfig().CommandTimeout;

            if (Context.Key == SqlKeyword.Select)
            {
                if (IsTop == false)
                {
                    myCmd.ExecuteType = ExecuteTypeEnum.Select;
                }
                else if (MyOqlConfigScope.Config != null && MyOqlConfigScope.Config.IsValueCreated &&
                        MyOqlConfigScope.Config.Value.Contains(ReConfigEnum.DataReader))
                {
                    myCmd.ExecuteType = ExecuteTypeEnum.SelectWithSkip;
                }
                else myCmd.ExecuteType = ExecuteTypeEnum.Select;

                ToSelectCommand(myCmd);
            }
            else if (Context.Key == SqlKeyword.Delete)
            {
                myCmd.ExecuteType = ExecuteTypeEnum.Execute;
                ToDeleteCommand(myCmd);
            }
            else if (Context.Key == SqlKeyword.Insert)
            {
                myCmd.ExecuteType = ExecuteTypeEnum.Execute;
                if (Context.Dna.Any(o => o.Key == SqlKeyword.Select))
                {
                    ToInsertSelectCommand(myCmd);
                }
                else if (Context.Dna.Any(o => o.Key == SqlKeyword.Model))
                {
                    ToInsertCommand(myCmd);
                }
                else
                {
                    throw new GodError("不能解析的批量插入数据格式");
                }

            }
            else if (Context.Key == SqlKeyword.Update)
            {
                myCmd.ExecuteType = ExecuteTypeEnum.Execute;
                ToUpdateCommand(myCmd);
            }
            else if (Context.Key == SqlKeyword.Proc)
            {
                myCmd.Command.CommandType = CommandType.StoredProcedure;
                ToProcCommand(myCmd);
            }
            else if (Context.Key == SqlKeyword.BulkInsert)
            {
                myCmd.ExecuteType = ExecuteTypeEnum.Select;

                ToBulkInsertCommand(myCmd);
            }
            return myCmd;
        }

        protected void CheckSelectColumns()
        {
            var fullName = Context.CurrentRule.GetFullName().AsString();
            var repCol = Context.Dna
                .Where(o => o.IsColumn())
                .GroupBy(o =>
                {
                    return (o as ColumnClip).GetAlias();
                })
                .FirstOrDefault(o => o.Count() > 1);

            if (repCol != null)
            {
                throw new GodError(fullName + " 检测到重复列: " + repCol.Key + "，请检查！") { Detail = this.Context.CurrentRule.GetFullName().ToString() };
            }

            //检查子查询。
            //Context.Dna.Where(o => o.Key == SqlKeyword.Select).All(o =>
            //    {
            //        Check(o as ContextClipBase);
            //        return true;
            //    });
        }


        private void ToProcCommand(MyCommand myCmd)
        {
            var procParameters = (ProcParameterClip)Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.ProcParameter);

            myCmd.Command.CommandText = GetProcName();

            if (procParameters != null)
            {
                myCmd.Command.Parameters.AddRange(procParameters.Parameters.Where(o => o != null).ToArray());
            }
        }

        /// <summary>
        /// 解析为 Update 命令。
        /// </summary>
        /// <param name="myCmd"></param>
        /// <returns></returns>
        protected virtual void ToUpdateCommand(MyCommand myCmd)
        {
            if (Context == null)
                return;
            DbCommand command = myCmd.Command;
            CommandValue cmd = new CommandValue();
            cmd.Sql = @"Update " + GetMyName(Context.CurrentRule.GetName());

            XmlDictionary<ColumnClip, object> dictModel = null;
            var paras = TidyUpdateModel(myCmd, out dictModel);

            dbo.Check(paras.Count == 0, "要更新的列为空.", Context.CurrentRule);

            cmd.Sql += " Set " + string.Join(",", paras.Select(o => o.Key + "=" + o.Value.Expression).ToArray());
            cmd.Parameters.AddRange(paras.Select(o => o.Value.Parameter));

            cmd.Sql += GetFromText();


            //生成 Join 子句
            cmd &= GetJoinText();

            var @where = GetWhereText(Context.Dna.Where(o => o != null && o.Key == SqlKeyword.Where).Select(o => o as WhereClip).FirstOrDefault());
            if (@where.Sql.HasValue())
            {
                cmd.Sql += " Where " + @where.Sql;
                cmd.Parameters.AddRange(@where.Parameters);
            }
            else if (this.Context.Dna.Any(o => o.Key.IsIn(SqlKeyword.Join, SqlKeyword.LeftJoin, SqlKeyword.RightJoin)))
            {
            }
            else
            {
                Action UsePrimaryKeyUpdate = () =>
                    {
                        var pks = Context.CurrentRule.GetPrimaryKeys();

                        if (pks.All(o => { return dictModel.ContainsKey(o); }) == false)
                        {
                            throw new GodError("没有找到Model要更新的条件！");
                        }

                        WhereClip where2 = new WhereClip();
                        pks.All(o =>
                        {
                            WhereClip where1 = new WhereClip();
                            where1.Query = o;
                            where1.Operator = SqlOperator.Equal;
                            where1.Value = dictModel.First(d => d.Key.NameEquals(o)).Value;

                            where2 &= where1;

                            return true;
                        });
                        @where = GetWhereText(where2);
                        if (@where.Sql.HasValue())
                        {
                            cmd.Sql += " where " + @where.Sql;
                            cmd.Parameters.AddRange(@where.Parameters);
                        }
                    };

                var idKey = Context.CurrentRule.GetIdKey();
                if (idKey.EqualsNull() == false)
                {

                    var dm = dictModel.FirstOrDefault(o => o.Key.Name == idKey.Name);
                    if (dm.Key.EqualsNull() == false)
                    {
                        WhereClip where1 = new WhereClip();
                        where1.Query = idKey;
                        where1.Operator = SqlOperator.Equal;
                        where1.Value = dm.Value;
                        @where = GetWhereText(where1);
                        if (@where.Sql.HasValue())
                        {
                            cmd.Sql += " where " + @where.Sql;
                            cmd.Parameters.AddRange(@where.Parameters);
                        }
                    }
                    else
                    {
                        UsePrimaryKeyUpdate();
                    }
                }
                else
                {
                    UsePrimaryKeyUpdate();
                }
            }




            command.CommandText = cmd.Sql;

            command.Parameters.AddRange(cmd.Parameters.Where(o => o != null).ToArray());
            command.CommandType = System.Data.CommandType.Text;
            return;
        }

        /// <summary>
        /// 整理Update子句的Model
        /// </summary>
        /// <param name="myCmd"></param>
        /// <param name="Model"></param>
        /// <returns></returns>
        protected XmlDictionary<string, CommandParameter> TidyUpdateModel(MyCommand myCmd, out XmlDictionary<ColumnClip, object> Model)
        {
            XmlDictionary<string, CommandParameter> retVal = new XmlDictionary<string, CommandParameter>();
            var dict = (Context.Dna.First(o => o != null && o.Key == SqlKeyword.Model) as ModelClip).Model;
            //            InsertClip insert = Context as InsertClip;
            //            XmlDictionary<ColumnClip, object> dict = dbo.ModelToDictionary(Context.CurrentRule, (insert.Dna.First(o => o.Key == SqlKeyword.Object) as ModelClip).Model);

            Model = new XmlDictionary<ColumnClip, object>();
            foreach (var key in dict.Keys)
            {
                Model[key] = dict[key];
            }

            var setCols = Context.Dna.Where(o => o.IsColumn()).ToArray();

            string[] cols = null;
            if (setCols.Length > 0)
            {
                cols = setCols.Select(o => o.Key == SqlKeyword.Simple ? (o as SimpleColumn).DbName : (o as ConstColumn).DbName).ToArray();
            }
            else
            {
                cols = Context.CurrentRule.GetColumns().Select(o => o.DbName).ToArray();
            }

            dict.All(o =>
            {
                var dictDbName = o.Key.Key == SqlKeyword.Simple ? (o.Key as SimpleColumn).DbName : (o.Key as ConstColumn).DbName;
                if (Context.CurrentRule.GetComputeKeys().Any(p => dictDbName == p.DbName))
                    return true;
                var autoIncr = Context.CurrentRule.GetAutoIncreKey();
                if (autoIncr.EqualsNull() == false && autoIncr.Name == o.Key.Name)
                    return true;
                if (cols.Contains(dictDbName) == false)
                    return true;

                dbo.Check(o.Key.IsSimple() == false, "要插入或更新的Model,只能是简单列!", Context.CurrentRule);

                var colValue = o.Value;
                if (o.Key.DbType == DbType.Guid)
                {
                    var guid = (Guid)o.Value;
                    if (guid == Guid.Empty)
                    {
                        colValue = null;
                    }
                }
                else if (o.Key.DbType.IsIn(DbType.Date, DbType.DateTime, DbType.DateTime2, DbType.DateTimeOffset))
                {
                    if (o.Value != null && o.Value.AsDateTime() == DateTime.MinValue)
                    {
                        colValue = null;
                    }
                }

                //string paraFormat = VarId + "p" + curIndex.ToString();
                var para = GetParameter(o.Key, colValue);
                //                    listVal.Add(paraFormat);

                retVal.Add(GetColumnExpression(o.Key), para);
                //                    cmd.Parameters.Add(para);

                return true;
            });

            return retVal;
        }

        /// <summary>
        /// 整理Insert子句Model
        /// </summary>
        /// <param name="myCmd"></param>
        /// <returns></returns>
        protected XmlDictionary<string, CommandParameter> TidyInsertModel(MyCommand myCmd)
        {
            XmlDictionary<string, CommandParameter> retVal = new XmlDictionary<string, CommandParameter>();
            var cols = Context.CurrentRule.GetColumns().Select(o => o.DbName).ToArray();
            var dict = (Context.Dna.First(o => o != null && o.Key == SqlKeyword.Model) as ModelClip).Model;
            //            InsertClip insert = Context as InsertClip;
            //            XmlDictionary<ColumnClip, object> dict = dbo.ModelToDictionary(Context.CurrentRule, (insert.Dna.First(o => o.Key == SqlKeyword.Object) as ModelClip).Model);

            var setCols = Context.Dna.Where(o => o.IsColumn()).ToArray();

            if (setCols.Length > 0)
            {
                cols = setCols.Select(o => o.Key == SqlKeyword.Simple ? (o as SimpleColumn).DbName : (o as ConstColumn).DbName).ToArray();
            }

            dict.All(o =>
            {
                var dictDbName = o.Key.Key == SqlKeyword.Simple ? (o.Key as SimpleColumn).DbName : (o.Key as ConstColumn).DbName;

                if (Context.CurrentRule.GetComputeKeys().Any(p => dictDbName == p.DbName))
                    return true;
                var autoIncr = Context.CurrentRule.GetAutoIncreKey();
                if (autoIncr.EqualsNull() == false && autoIncr.Name == o.Key.Name)
                    return true;
                if (cols.Contains(dictDbName) == false)
                    return true;

                dbo.Check(o.Key.IsSimple() == false, "要插入或更新的Model,只能是简单列!", Context.CurrentRule);

                if (o.Key.DbType == DbType.Guid)
                {
                    var guid = (Guid)o.Value;
                    if (guid == Guid.Empty) return true;
                }

                if (o.Key.DbType.IsIn(DbType.Date, DbType.DateTime, DbType.DateTime2, DbType.DateTimeOffset))
                {
                    if (o.Value != null && o.Value.AsDateTime() == DateTime.MinValue)
                    {
                        return true;
                    }
                }




                //string paraFormat = VarId + "p" + curIndex.ToString();
                var para = GetParameter(o.Key, o.Value);
                //                    listVal.Add(paraFormat);

                para.Parameter.Direction = System.Data.ParameterDirection.Input;

                retVal.Add(GetColumnExpression(o.Key, false), para);
                //                    cmd.Parameters.Add(para);

                return true;
            });

            return retVal;
        }


        /// <summary>
        /// 解析Delete命令。
        /// </summary>
        /// <param name="myCmd"></param>
        /// <returns></returns>
        protected virtual void ToDeleteCommand(MyCommand myCmd)
        {
            DbCommand command = myCmd.Command;
            ContextClipBase delete = Context;
            if (delete == null)
                return;

            CommandValue cmd = new CommandValue();
            cmd.Sql = "delete " + GetTableFullName(delete.CurrentRule);

            var @where = GetWhereText(Context.Dna.Where(o => o.Key == SqlKeyword.Where).Select(o => o as WhereClip).FirstOrDefault(), false);
            if (@where.Sql.HasValue())
            {
                cmd.Sql += " where " + @where.Sql;
                cmd.Parameters.AddRange(@where.Parameters);
            }

            command.CommandText = cmd.Sql;
            command.Parameters.AddRange(cmd.Parameters.Where(o => o != null).ToArray());
            command.CommandType = System.Data.CommandType.Text;
            return;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// 给表列加标识符。
        /// </summary>
        /// <param name="MyObject"></param>
        /// <returns></returns>
        public string GetMyName(string MyObject)
        {
            if (MyObject.HasValue() == false) return MyObject.AsString();
            return string.Format(GetOperator(SqlOperator.Qualifier), MyObject);
        }


        /// <summary>
        /// 不必转义的标识符。
        /// </summary>
        public virtual string NonTranslateSign { get { return "#"; } }
    }
}
