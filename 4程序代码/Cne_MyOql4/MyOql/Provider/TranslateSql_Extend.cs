using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using MyCmn;
using System.Collections;
using System.Configuration;
using System.Data;
using MyOql;

namespace MyOql.Provider
{
    public abstract partial class TranslateSql
    {

        /// <summary>
        /// 处理常量表的情况.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public virtual string GetTableFullName(RuleBase table)
        {
            var fullName = table.GetFullName();
            if (fullName.AsString().HasValue() == false) return string.Empty;

            var retVal = string.Empty;

            if (fullName.Owner.HasValue())
            {
                retVal = GetMyName(fullName.Owner) + "." + GetMyName(fullName.DbName);
            }
            else
            {
                retVal = GetMyName(fullName.DbName);
            }

            var func = table as IFunctionRule;
            if (func != null)
            {
                //Context.ContainsFunction = true;
                List<string> list = new List<string>();
                func.GetParameters().All(o =>
                {
                    var v = func.GetParameterValue(o);

                    if (v == null)
                    {
                        throw new MyOqlError("函数实体 {Func} 的参数 {Para} 值不能为空。"
                            .Format(new StringDict { { "Func", func.ToString() }, { "Para", o } })
                            , null);
                    }

                    if (v.GetType().IsNumberType())
                    {
                        list.Add(v.AsString());
                    }
                    else
                    {
                        list.Add("'" + v.AsString().Replace("'", "''") + "'");
                    }
                    return true;
                });

                retVal += "(" + string.Join(",", list.ToArray()) + ")";
            }

            //if (Context.Rules.IndexOf(o => o.GetDbName() == table.GetDbName()) < 0)
            //{
            //    Context.Rules.Add(table);
            //}

            return retVal;
        }


        /// <summary>
        /// 如果任何一个参数值是数组,则将它们展开,合并到上级的参数数组.
        /// </summary>
        /// <param name="paraObjs"></param>
        /// <returns></returns>
        public object[] ToParaValues(params object[] paraObjs)
        {
            List<object> objs = new List<object>();
            foreach (var obj in paraObjs)
            {
                var itemArray = obj as object[];
                if (itemArray != null)
                {
                    objs.AddRange(itemArray);
                }
                else if (obj.IsDBNull() == false)
                {
                    objs.Add(obj);
                }
            }
            return objs.ToArray();
        }
        /// <summary>
        /// Select 子句查询表达式。
        /// </summary>
        /// <param name="Column"></param>
        /// <returns></returns>
        public virtual string GetSelectColumnExpression(ColumnClip Column)
        {
            var alias = Column.GetAlias();
            if (alias.HasValue())
            {
                return string.Format(GetOperator(SqlOperator.As), GetColumnExpression(Column), GetMyName(alias));
            }
            else return GetColumnExpression(Column);
        }

        public virtual string GetColumnExpression(ColumnClip Column)
        {
            return GetColumnExpression(Column, true);
        }


        /// <summary>
        /// 判断 该 DbType 的值是否能直接使用
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public virtual bool DbTypeValueMustBeWrap(DbType dtype)
        {
            if (dtype == DbType.Object) return false;
            else if (dtype == DbType.Guid) return true;
            else if (dtype.DbTypeIsDateTime()) return true;
            else if (dtype.DbTypeIsString()) return true;
            //else if (dtype.DbTypeIsNumber()) return false;
            return false;
        }
        /// <summary>
        /// 获取列的SQL表达式. 能够处理系统函数, 组合列表达式,
        /// </summary>
        /// <remarks>
        /// 从算法来看，可以参考 MyLinq GetQueryValue 方法进行重构。重构的参数是 把 ColumnClip 传递到每个 Func
        /// </remarks>
        /// <param name="Column"></param>
        /// <param name="withTableAlias">带表别名返回。</param>
        /// <returns></returns>
        public virtual string GetColumnExpression(ColumnClip Column, bool withTableAlias)
        {
            if (Column.EqualsNull()) return null;

            if (Column.IsRaw())
            {
                var rawObj = Column as RawColumn;

                if (rawObj.FunctionFormat.HasValue())
                {
                    return string.Format(rawObj.FunctionFormat, string.Format(GetOperator(rawObj.Operator), GetMyOqlRawParameterSql(rawObj)));
                }
                else return string.Format(GetOperator(rawObj.Operator), GetMyOqlRawParameterSql(rawObj));
            }

            if (Column.IsConst())
            {
                var cc = Column as ConstColumn;
                if (DbTypeValueMustBeWrap(Column.DbType)) return "'" + cc.DbName + "'";
                else return (withTableAlias && Column.TableName.HasValue()) ? (Column.TableName + "." + cc.DbName) : cc.DbName;
            }

            Func<string> GetSimpleColumnExpression = () =>
                {
                    var simple = Column as SimpleColumn;

                    if (withTableAlias)
                    {
                        return GetMyName(simple.TableName.AsString(simple.TableDbName)) + "." + GetMyName(simple.DbName);
                    }
                    else
                    {
                        return GetMyName(simple.DbName);
                    }
                };

            //Func<ColumnClip, string[]> GetFunctionParameter = c =>
            //    {
            //        List<string> list = new List<string>();
            //        //如果要自定义参数内容，可以使用 ColumnClip
            //        foreach (var ex in c.Extend)
            //        {
            //            if (ex.Key != SqlOperator.FunctionParameter) continue;

            //            var fp = ex.Value;

            //            var fpString = fp as string;
            //            if (fpString != null)
            //            {
            //                list.Add("'" + fpString + "'");
            //                continue;
            //            }

            //            var fpInt = fp as int?;
            //            if (fpInt.HasValue)
            //            {
            //                list.Add(fpInt.AsString());
            //                continue;
            //            }

            //            var fpColumn = fp as ColumnClip;
            //            if (fpColumn.EqualsNull() == false)
            //            {
            //                list.Add(GetColumnExpression(fpColumn));
            //                continue;
            //            }

            //            var fpColumns = fp as IEnumerable<ColumnClip>;
            //            if (fpColumns != null && fpColumns.Any())
            //            {
            //                list.AddRange(fpColumns.Select(o => GetColumnExpression(o)));
            //                continue;
            //            }

            //            var fpStringArray = fp as IEnumerable<string>;
            //            if (fpStringArray != null)
            //            {
            //                list.AddRange(fpStringArray.Select(o => "'" + o + "'"));
            //                continue;
            //            }

            //            var fpIntArray = fp as IEnumerable<int>;
            //            if (fpIntArray != null)
            //            {
            //                list.AddRange(fpIntArray.Select(o => o.AsString()));
            //                continue;
            //            }

            //            if (MyCmn.ValueProc.IsValueType<DbType>(fp))
            //            {
            //                var dbType = (DbType)fp;
            //                var sqlType = GetTypeMap().First(t => t.DbType == dbType).SqlType.ToString();

            //                if (dbType.DbTypeIsString())
            //                {
            //                    sqlType += "(4000)";
            //                }

            //                list.Add(sqlType);
            //                continue;
            //            }

            //            // DatePart 返回默认即可。

            //            list.Add(fp.AsString());
            //        }

            //        return list.ToArray();
            //    };

            Func<ColumnClip, string[]> _GetComplexExpression = null;
            Func<ColumnClip, string[]> GetComplexExpression = col =>
            {
                if (col.EqualsNull()) return null;

                if (col.IsComplex() == false)
                {
                    return new string[] { GetColumnExpression(col, withTableAlias) };
                }
                else
                {
                    var complex = col as ComplexColumn;

                    if (complex.Operator == SqlOperator.Parameter)
                    {
                        return new string[] { GetColumnExpression(complex.LeftExp, withTableAlias), GetColumnExpression(complex.RightExp, withTableAlias) };
                    }

                    return new string[]{ string.Format(GetOperator(complex),
                        ToParaValues(_GetComplexExpression(complex.LeftExp), _GetComplexExpression(complex.RightExp)/*, GetFunctionParameter(col)*/))
                    };
                }
            };
            _GetComplexExpression = GetComplexExpression;

            if (Column.IsSimple())
            {
                return GetSimpleColumnExpression();
            }
            else
            {
                return GetComplexExpression(Column)[0];
            }
        }

        private string GetMyOqlRawParameterSql(RawColumn col)
        {
            ValueMetaTypeEnum sqlOperator = col.ParameterValueType;
            object p = col.Parameter;
            if (sqlOperator == ValueMetaTypeEnum.DbType)
            {
                var dbType = (DbType)p;
                return this.ToSqlType(dbType) +
                    (dbType.IsIn(DbType.AnsiString, DbType.AnsiStringFixedLength, DbType.String, DbType.StringFixedLength) ? "(4000)" : string.Empty);
            }

            else if (sqlOperator == ValueMetaTypeEnum.NumberType) return p.ToString();
            else if (sqlOperator == ValueMetaTypeEnum.StringType) return p.ToString();
            else if (sqlOperator == ValueMetaTypeEnum.EnumType) return p.ToString();
            else if (sqlOperator == ValueMetaTypeEnum.MyOqlType)
            {
                var myoql = p as SqlClipBase;
                if (myoql.IsColumn())
                {
                    return GetColumnExpression((ColumnClip)p);
                }
                else if (myoql.Key == SqlKeyword.Where)
                {
                    return GetWhereText((WhereClip)p, true).ToFullSql();
                }
            }
            throw new GodError("RawColumn中出现不识别的参数类型：" + sqlOperator.ToString());
        }

        /// <summary>
        /// 仅返回 WereValue.其中CommandValue的Sql 和 Parameters是参数值.
        /// </summary>
        /// <param name="whe"></param>
        /// <returns></returns>
        public CommandValue GetOnlyWhereValue(WhereClip whe)
        {
            CommandValue value = new CommandValue();
            if (whe.Operator.IsIn(SqlOperator.In, SqlOperator.NotIn))
            {
                ContextClipBase selection = whe.Value as ContextClipBase;
                if (selection != null)
                {
                    //子查询不应用 权限，日志，缓存。
                    selection.SkipPower().SkipLog().SkipCache();

                    selection.Connection = this.Context.Connection;

                    var cmd = selection.ToCommand(this.Context).Command;
                    value.Sql = cmd.CommandText;
                    var paras = cmd.Parameters.ToMyList(o => (DbParameter)o).ToArray();
                    cmd.Parameters.Clear();
                    value.Parameters.AddRange(paras);
                }
                else
                {
                    IEnumerable list = whe.Value as IEnumerable;

                    if (list != null)
                    {
                        if (DbTypeValueMustBeWrap(whe.Query.DbType))
                        {
                            value.Sql = @"'" + string.Join("','", list.ToMyList(o => o.AsString()).ToArray()) + @"'";
                        }
                        else
                        {
                            value.Sql = string.Join(",", list.ToMyList(o => o.AsString()).ToArray());
                        }
                    }
                }
            }
            else if (whe.Operator.IsIn(SqlOperator.Between))
            {
                var values = whe.Value as object[];
                if (values != null && values.Length == 2)
                {
                    var sqlPara1 = GetParameter(whe.Query, values[0]);
                    var sqlPara2 = GetParameter(whe.Query, values[1]);

                    value.Sql = sqlPara1.Expression.ToString() + " And " + sqlPara2.Expression.ToString();

                    value.Parameters.Add(sqlPara1.Parameter);
                    value.Parameters.Add(sqlPara2.Parameter);
                }
                else
                {
                    throw new GodError(whe.Query.GetFullName().AsString() + "," + " Between 关键字需要两个值做为参数");
                }
            }
            else
            {
                ColumnClip col = whe.Value as ColumnClip;
                if (col.EqualsNull() == false)
                {
                    value.Sql = GetColumnExpression(col);
                }
                else
                {
                    var sqlPara = GetParameter(whe.Query, whe.Value);
                    value.Sql = sqlPara.Expression;

                    value.Parameters.Add(sqlPara.Parameter);
                }
            }
            return value;
        }

        [NonSerialized]
        protected StringDict DbConns = new StringDict();
        public virtual string GetConnectionString(string ConfigName)
        {
            if (DbConns.ContainsKey(ConfigName))
            {
                return DbConns[ConfigName];
            }

            var connString = "";
            if (ConfigName.HasValue() == false)
            {
                connString = ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1].ConnectionString;
            }
            else
            {
                var config = ConfigurationManager.ConnectionStrings[ConfigName];
                if (config == null)
                {
                    connString = ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1].ConnectionString;
                }
                else
                {
                    connString = config.ConnectionString;
                }
            }

            DbConns[ConfigName] = dbo.Event.OnDecrypte(connString);
            return DbConns[ConfigName];
        }

        public string GetOperator(SqlOperator Op)
        {
            return GetOperator(new OperatorContext(Op));
        }

        /// <summary>
        /// 以SqlServer 为标准.
        /// </summary>
        /// <param name="Operator"></param>
        /// <returns></returns>
        public virtual string GetOperator(IOperator Col)
        {
            SqlOperator Operator = Col.Operator;
            string retVal = "{0}";
            switch (Operator)
            {
                #region  第一部分： 关键字
                case SqlOperator.Blank:
                    retVal = "{0} {1}";
                    break;
                case SqlOperator.Enter:
                    retVal = @"{0}
{1}";
                    break;
                case SqlOperator.Concat:
                    retVal = "({0} + {1})";
                    break;
                case SqlOperator.Add:
                    retVal = "({0} + {1})";
                    break;
                case SqlOperator.Minus:
                    retVal = "({0} - {1})";
                    break;
                case SqlOperator.Multiple:
                    retVal = "{0} * {1}";
                    break;
                case SqlOperator.Divided:
                    retVal = "{0} / {1}";
                    break;
                case SqlOperator.As:
                    retVal = "{0} As {1}";
                    break;
                case SqlOperator.And:
                    retVal = "And";
                    break;
                case SqlOperator.Or:
                    retVal = "Or";
                    break;
                case SqlOperator.BitAnd:
                    retVal = "{0} & {1}";
                    break;
                case SqlOperator.BitOr:
                    retVal = "{0} | {1}";
                    break;
                case SqlOperator.Equal:
                    retVal = "{0} = {1}";
                    break;
                case SqlOperator.BigThan:
                    retVal = "{0} > {1}";
                    break;
                case SqlOperator.LessThan:
                    retVal = "{0} < {1}";
                    break;
                case SqlOperator.BigEqual:
                    retVal = "{0} >= {1}";
                    break;
                case SqlOperator.LessEqual:
                    retVal = "{0} <= {1}";
                    break;
                case SqlOperator.NotEqual:
                    retVal = "{0} != {1}";
                    break;
                case SqlOperator.In:
                    retVal = "{0} In ({1})";
                    break;
                case SqlOperator.NotIn:
                    retVal = "{0} Not In ({1})";
                    break;
                case SqlOperator.Like:
                    retVal = "{0} Like {1}";
                    break;
                case SqlOperator.Mod:
                    retVal = "{0} % {1}";
                    break;
                case SqlOperator.CaseWhen:
                    retVal = "Case {0} Else {1} End";
                    break;
                case SqlOperator.WhenThen:
                    retVal = "When {0} Then {1}";
                    break;
                case SqlOperator.Else:
                    retVal = "Else {0}";
                    break;
                case SqlOperator.Between:
                    retVal = "({0} Between {1})";
                    break;
                case SqlOperator.AndForBetween:
                    retVal = "{0} And {1}";
                    break;
                case SqlOperator.Cast:
                    retVal = "Cast({0} as {1})";
                    break;
                case SqlOperator.IsNull:
                    retVal = "IsNull({0},{1})";
                    break;
                case SqlOperator.Union:
                    retVal = @"{0}
Union
{1}";
                    break;
                case SqlOperator.UnionAll:
                    retVal = @"{0}
Union All
{1}";
                    break;
                #endregion

                #region 第二部分：字符串函数
                case SqlOperator.Len:
                    retVal = "Len({0})";
                    break;
                case SqlOperator.SizeOf:
                    retVal = "DataLength({0})";
                    break;
                case SqlOperator.Left:
                    retVal = "Left({0},{1})";
                    break;
                case SqlOperator.Right:
                    retVal = "Right({0},{1})";
                    break;
                case SqlOperator.Reverse:
                    retVal = "Reverse({0})";
                    break;
                case SqlOperator.AscII:
                    retVal = "AscII({0})";
                    break;
                case SqlOperator.Unicode:
                    retVal = "Unicode({0})";
                    break;
                case SqlOperator.Char:
                    retVal = "Char({0})";
                    break;
                case SqlOperator.NChar:
                    retVal = "NChar({0})";
                    break;
                case SqlOperator.StringIndex:
                    //三个字符串分别是： 总字符串, 查询字符串, 开始索引，后两个是一个复合列。
                    retVal = "charindex({0},{1},{2})";
                    break;
                case SqlOperator.SubString:
                    retVal = "SubString({0},{1})";
                    break;
                case SqlOperator.Stuff:
                    retVal = "Stuff({0},{2},{1})";
                    break;
                case SqlOperator.PatIndex:
                    retVal = "PatIndex({1},{0})";
                    break;
                case SqlOperator.Replace:
                    retVal = "Replace({0},{1},{2})";
                    break;
                case SqlOperator.IsNumeric:
                    retVal = "IsNumeric({0})";
                    break;
                case SqlOperator.Ltrim:
                    retVal = "Ltrim({0})";
                    break;
                case SqlOperator.Rtrim:
                    retVal = "Rtrim({0})";
                    break;
                case SqlOperator.Trim:
                    retVal = "Ltrim(Rtrim({0}))";
                    break;
                #endregion

                #region 第三部分：聚合函数
                case SqlOperator.Count:
                    retVal = "count({0})";
                    break;
                case SqlOperator.CountDistinct:
                    retVal = "count(distinct {0})";
                    break;
                case SqlOperator.Sum:
                    retVal = "sum({0})";
                    break;
                case SqlOperator.Max:
                    retVal = "max({0})";
                    break;
                case SqlOperator.Min:
                    retVal = "min({0})";
                    break;
                case SqlOperator.Avg:
                    retVal = "avg({0})";
                    break;
                #endregion

                #region 第四部分：时间函数
                case SqlOperator.IsDate:
                    retVal = "IsDate({0})";
                    break;
                case SqlOperator.Year:
                    retVal = "Year({0})";
                    break;
                case SqlOperator.Month:
                    retVal = "Month({0})";
                    break;
                case SqlOperator.Day:
                    retVal = "Day({0})";
                    break;
                case SqlOperator.DateDiff:
                    retVal = "DateDiff({2},{0},{1})";
                    break;
                case SqlOperator.DateAdd:
                    retVal = "DateAdd({2},{0},{1})";
                    break;
                #endregion

                #region 第五部分：
                case SqlOperator.Property:
                    retVal = "{0}.{1}";
                    break;
                case SqlOperator.Qualifier:
                    retVal = "[{0}]";
                    break;
                case SqlOperator.ConcatSql:
                    retVal = "{0} {1}";
                    break;
                case SqlOperator.CurrentIdentity:
                    retVal = "IDENT_CURRENT({0})";
                    break;
                case SqlOperator.Coalesce2:
                    retVal = "Coalesce({0},{1})";
                    break;
                case SqlOperator.Coalesce3:
                    retVal = "Coalesce({0},{1},{2})";
                    break;
                case SqlOperator.Coalesce4:
                    retVal = "Coalesce({0},{1},{2},{3})";
                    break;
                case SqlOperator.Coalesce5:
                    retVal = "Coalesce({0},{1},{2},{3},{4})";
                    break;
                //参数要单独处理。见 stringIndex.
                case SqlOperator.Parameter:
                    throw new GodError("参数不能取格式字符串");
                #endregion
                default:
                    break;
            }
            return retVal;
        }

        //[NonSerialized]
        //private Dictionary<DbType, string> _TypeMap;

        //public Dictionary<DbType, string> TypeMap { get { return _TypeMap; } protected set { _TypeMap = value; } }


        public abstract string ToSqlType(DbType dbType);
        public abstract DbType ToDbType(string sqlType);

        /// <summary>
        /// 批量插入,Context 里有两种Model , 一种是 MyOqlSet , 另一种是 Select 子句.
        /// </summary>
        /// <param name="myCmd"></param>
        public virtual void ToInsertSelectCommand(MyCommand myCmd)
        {
            var select = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Select) as SelectClip;

            select.Connection = this.Context.Connection;

            var selectCommand = select.ToCommand(this.Context);

            var listCol = new List<string>();

            if (select.Dna.Count(o => o.IsColumn()) == 0)
            {
                listCol.AddRange(select.CurrentRule.GetColumns().Select(o => o.GetAlias()));
            }
            else
            {
                listCol.AddRange(Enumerable.Select(select.Dna.Where(o => o.IsColumn()), o => (o as ColumnClip).GetAlias()));
            }


            string sql = "insert into " + GetTableFullName(Context.CurrentRule) + "(" +
              string.Join(",", listCol.ToArray()) +
              ") " + selectCommand.Command.CommandText;

            var paras = selectCommand.Command.Parameters.ToMyList(o => o as DbParameter).ToArray();
            selectCommand.Command.Parameters.Clear();
            myCmd.Command.Parameters.AddRange(paras);
            myCmd.Command.CommandText = sql;
            myCmd.Command.CommandType = System.Data.CommandType.Text;

            myCmd.ExecuteType = ExecuteTypeEnum.Execute;
            return;
        }


        /// <summary>
        /// 存储过程的名字.
        /// </summary>
        /// <returns></returns>
        public virtual string GetProcName()
        {
            var owner = Context.CurrentRule.GetConfig().Owner;
            if (owner.HasValue())
            {
                return string.Format(@"{0}.{1}",
                    GetMyName(owner),
                    GetMyName(Context.CurrentRule.GetDbName())
                    );
            }
            else
            {
                return GetMyName(Context.CurrentRule.GetName());
            }
        }
    }

}
