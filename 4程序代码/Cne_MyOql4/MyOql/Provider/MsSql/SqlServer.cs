using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;
using MyCmn;
using System.Data;
using System.Transactions;

namespace MyOql.Provider
{
    public partial class SqlServer : TranslateSql
    {
        public SqlServer()
        {
        }

        public override string ToSqlType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString: return "varchar";
                case DbType.AnsiStringFixedLength: return "char";
                case DbType.Boolean: return "bit";
                case DbType.Byte: return "tinyint";
                case DbType.Currency: return "decimal";
                case DbType.Date: return "date";
                case DbType.DateTime: return "datetime";
                case DbType.DateTime2: return "datetime2";
                case DbType.DateTimeOffset: return "datetimeoffset";
                case DbType.Decimal: return "decimal";
                case DbType.Double: return "bigint";
                case DbType.Guid: return "uniqueidentifier";
                case DbType.Int16: return "smallint";
                case DbType.Int32: return "int";
                case DbType.Int64: return "bigint";
                case DbType.SByte: return "tinyint";
                case DbType.Single: return "float";
                case DbType.String: return "nvarchar";
                case DbType.StringFixedLength: return "nchar";
                case DbType.Time: return "time";
                case DbType.UInt16: return "smallint";
                case DbType.UInt32: return "int";
                case DbType.UInt64: return "bigint";
                case DbType.VarNumeric: return "decimal";
                case DbType.Xml: return "xml";
                case DbType.Binary: return "varbinary";
                case DbType.Object: return "image";
            }

            return "varchar";
        }

        public override DbType ToDbType(string sqlType)
        {
            var type = sqlType.ToLower();
            switch (type)
            {
                case "image": return DbType.Object;
                case "text": return DbType.AnsiString;
                case "uniqueidentifier": return DbType.Guid;
                case "date": return DbType.Date;
                case "time": return DbType.Time;
                case "datetime2": return DbType.DateTime2;
                case "datetimeoffset": return DbType.DateTimeOffset;
                case "tinyint": return DbType.SByte;
                case "smallint": return DbType.Int16;
                case "int": return DbType.Int32;
                case "smalldatetime": return DbType.DateTime;
                case "real": return DbType.Single;
                case "money": return DbType.Decimal;
                case "datetime": return DbType.DateTime;
                case "float": return DbType.Single;
                case "sql_variant": return DbType.Object;
                case "ntext": return DbType.String;
                case "bit": return DbType.Boolean;
                case "decimal": return DbType.Decimal;
                case "numeric": return DbType.Decimal;
                case "smallmoney": return DbType.Decimal;
                case "bigint": return DbType.Int64;
                case "hierarchyid": return DbType.Object;
                case "geometry": return DbType.Object;
                case "geography": return DbType.Object;
                case "varbinary": return DbType.Binary;
                case "varchar": return DbType.AnsiString;
                case "binary": return DbType.Binary;
                case "char": return DbType.AnsiStringFixedLength;
                case "timestamp": return DbType.DateTimeOffset;
                case "nvarchar": return DbType.String;
                case "nchar": return DbType.StringFixedLength;
                case "xml": return DbType.Xml;

                //nvarchar(128) 
                case "sysname": return DbType.StringFixedLength;
                default:
                    break;
            } return DbType.String;
        }


        public override DbDataAdapter GetDataAdapter()
        {
            return new SqlDataAdapter();
        }

        /// <summary>
        /// 获取指定的 DbConnection.
        /// </summary>
        /// <param name="ConfigName">如果ConfigName 为空,返回默认值.</param>
        /// <returns></returns>
        public override DbConnection GetConnection(string ConfigName)
        {
            if (Context != null && Context.Transaction != null && Context.Transaction.Connection != null)
                return Context.Transaction.Connection;

            else if (Context != null && Context.Connection != null) return Context.Connection;
            else
            {
                return new SqlConnection(GetConnectionString(ConfigName));
            }
        }

        public override string VarId
        {
            get { return "@"; }
        }


        public override CommandValue GetSelectText(MyCommand myCmd)
        {
            CommandValue command = new CommandValue();

            StringBuilder sbSql = new StringBuilder("select");

            if (Context.Dna.Any(o => o.Key == SqlKeyword.Distinct))
            {
                sbSql.Append(" distinct");
            }

            if (UseRowNumber == false)
            {
                var take = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Take) as TakeClip;
                if (take != null)
                {
                    var skipNumber = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Skip) as SkipClip;
                    if (skipNumber == null || skipNumber.SkipNumber <= 0)
                    {
                        sbSql.Append(" top " + (
                                take.TakeNumber
                                ).ToString());
                    }
                }
            }

            if (Context.Dna.Any(o => o.IsColumn()) == false)
            {
                sbSql.Append(" *");
            }
            else
            {
                string columns = string.Join(",", Context.Dna.Where(o => o.IsColumn()).Select(o => GetSelectColumnExpression(o as ColumnClip)).ToArray()
                    );
                sbSql.Append(" " + columns);
            }

            command.Sql = sbSql.ToString();
            return command;
        }

        public override DbParameter GetParameter(string ParameterName, DbType DbType, object Value)
        {
            var retVal = new SqlParameter(ParameterName, DbType);
            if (Value.IsDBNull())
            {
                retVal.Value = DBNull.Value;
            }
            else
            {
                var dbType = DbType.GetCsType();
                if (dbType.FullName == "MyCmn.MyDate")
                {
                    dbType = typeof(DateTime);
                }

                if (dbType.FullName == "System.DateTime")
                {
                    var val = ValueProc.As<DateTime>(Value);
                    if (val.HasValue())
                    {
                        retVal.Value = val;
                    }
                    else
                    {
                        retVal.Value = DBNull.Value;
                    }
                }
                else
                {
                    retVal.Value = ValueProc.AsType(dbType, Value);
                }
            }
            return retVal;
        }

        /// <summary>
        /// 重写基类方法. 
        /// </summary>
        /// <param name="Column">如果是复合列, 则按 Value 的类型确定.</param>
        /// <param name="Value"></param>
        /// <returns>
        /// A <see cref="CommandParameter"/>
        /// </returns>
        public override CommandParameter GetParameter(ColumnClip Column, object Value)//, string ParameterName)
        {
            dbo.Check(Column.EqualsNull(), "生成参数时,列 不能 为 null!", null);

            CommandParameter retVal = null;

            var colName = Column.Name;
            if (colName.HasValue() == false)
            {
                colName = "Var";
            }
            else
            {
                //参数只保留数字，字母。
                colName = colName.Where(o => char.IsLetterOrDigit(o)).AsString();
            }

            if (colName.HasValue() == false) colName = "Var";

            if (Context != null)
            {
                var pc = Context.ParameterColumn.Count(
                        o => o.Parameter != null &&
                        o.Parameter.ParameterName.StartsWith(VarId + colName + "_")
                        );
                if (pc == 0)
                {
                    retVal = new CommandParameter(new SqlParameter(), Column, VarId + colName + "_");
                }
                else
                {
                    retVal = new CommandParameter(new SqlParameter(), Column, VarId + colName + "_" + pc.AsString());
                }

                Context.ParameterColumn.Add(retVal);
            }
            else
            {
                retVal = new CommandParameter(new SqlParameter(), Column, VarId + colName);
            }
            //if (ParameterName.HasValue())
            //{
            //    retVal = new CommandParameter(new SqlParameter(), ParameterName);
            //}
            //else
            //{
            //    retVal = new CommandParameter(new SqlParameter(), VarId, ParaIndex);
            //}

            retVal.Parameter.Size = Column.Length;
            retVal.Parameter.DbType = Column.DbType;

            //修改Bug
            if (Column.DbType.IsIn(DbType.UInt16))
            {
                retVal.Parameter.DbType = DbType.Int32;
            }

            retVal.Parameter.Value = Value;

            //如果值是 Null 或 DbNull
            if (Value.IsDBNull())
            {
                retVal.Parameter.Value = DBNull.Value;
                return retVal;
            }

            //如果值是列
            var col_Val = Value as ColumnClip;
            if (col_Val.EqualsNull() == false)
            {
                //生成 Expression 表达式，Parameter 置为Null
                retVal.Parameter = null;
                retVal.Expression = GetColumnExpression(col_Val);

                return retVal;
            }

            //其余的, 要依次兼容, 列类型是 : 枚举,时间,Guid
            var valType = Value.GetType();

            if (valType.IsEnum)
            {
                if (retVal.Parameter.DbType.DbTypeIsNumber() || retVal.Parameter.DbType == DbType.Boolean)
                {
                    retVal.Parameter.Value = Value.AsInt();
                }
                else
                {
                    retVal.Parameter.Value = Value.AsString();
                }

                return retVal;
            }

            if (retVal.Parameter.DbType.DbTypeIsDateTime())
            {
                if (retVal.Parameter.Value.AsString() != string.Empty)
                {
                    var dtValue = retVal.Parameter.Value.AsDateTime();
                    retVal.Parameter.Value = dtValue;
                    if (dtValue == DateTime.MinValue)
                    {
                        retVal.Parameter.Value = DBNull.Value;
                    }
                }
            }
            //情景 , 列是 Guid  == string.
            else if (retVal.Parameter.DbType == DbType.Guid)
            {
                if (valType.FullName == "System.String")
                {
                    retVal.Parameter.Value = retVal.Parameter.Value.AsString();
                }
            }
            //情况: 列是 varchar == Guid
            else if (retVal.Parameter.DbType.IsIn(DbType.AnsiString, DbType.AnsiStringFixedLength, DbType.String, DbType.StringFixedLength))
            {
                if (valType.FullName == "System.Guid")
                {
                    retVal.Parameter.DbType = DbType.AnsiString;
                    retVal.Parameter.Value = retVal.Parameter.Value.AsString();
                }
                else
                {
                    var strVal = Value as string;
                    if (strVal == null)
                    {
                        retVal.Parameter.DbType = DbType.AnsiString;
                        retVal.Parameter.Value = retVal.Parameter.Value.AsString();
                    }
                }
            }
            else if (retVal.Parameter.DbType.DbTypeIsNumber() && valType.IsNumberType())
            {
                //Int32时，参数化有Bug,会造成查询非常慢。而使用SQL却很快。后来，发现，把数字的DbType的类型增大，就可以。
                retVal.Parameter.DbType = DbType.Decimal;
            }
            //其它情况, 按 值 的类型 来确定 参数类型.
            else
            {
                //如果执行了： id = dbo.func() ，则 生成 ： id = @func  @func的DbType 是  value 的 DbType

                //使用公共转换。 udi@2013年10月20日2时1分
                //var types = new MyOql.Provider.SqlServer().GetTypeMap();
                //var mapType = types.FirstDbTypeByCsType(valType);

                retVal.Parameter.DbType = valType.GetDbType();// mapType;

                //转换自定义类型.
                //retVal.Parameter.Value = ValueProc.AsType(types.First(o => o.DbType == retVal.Parameter.DbType).CsType, retVal.Parameter.Value);
            }

            return retVal;
        }

        /// <summary>
        /// 解析为 Insert 命令
        /// </summary>
        /// <param name="myCmd"></param>
        /// <returns></returns>
        protected override void ToInsertCommand(MyCommand myCmd)
        {
            DbCommand command = myCmd.Command;
            //            List<string> listCol = new List<string>();
            //            List<string> listVal = new List<string>();
            //XmlDictionary<ColumnClip, object> dict = dbo.ModelToDictionary(Context.CurrentRule, (Context.Dna.First(o => o.Key == SqlKeyword.Object) as ModelClip).Model);

            var paras = TidyInsertModel(myCmd);

            bool HasIncre = Context.CurrentRule.GetAutoIncreKey().EqualsNull() == false;

            string strSelect = "";

            if (HasIncre)
            {
                strSelect = "select SCOPE_IDENTITY();";

                //如果有计算列, 则再进行一次查询.
                myCmd.ExecuteType = ExecuteTypeEnum.Select;
            }
            else
            {
                //如果没有自增列。而且，没有唯一列。 而且，要命的是，计算列是主键。
                //这时，需要在触发器中返回计算列的值。
                if (Context.CurrentRule.GetUniqueKey().EqualsNull())
                {
                    var computeKyes = Context.CurrentRule.GetComputeKeys();
                    if (computeKyes.Length > 0)
                    {
                        var interC = computeKyes.Intersect(Context.CurrentRule.GetPrimaryKeys()).Any();
                        //GodError.Check(interC > 1, () => "主键中的动计算列只能有一个");
                        if (interC)
                        {
                            myCmd.ExecuteType = ExecuteTypeEnum.Select;
                        }
                    }
                }
            }

            //int curParaIndex = ParaIndex;

            var listCol = paras.Select(o => o.Key);
            var listVal = paras.Select(o => o.Value.Expression);


            string sql = "insert into " + GetTableFullName(Context.CurrentRule) + "(" +
                string.Join(",", listCol.ToArray()) +
                ")values(" +
                string.Join(",", listVal.ToArray()) +
                ");" + strSelect;

            command.Parameters.AddRange(paras.Select(o => o.Value.Parameter).Where(o => o != null).ToArray());
            command.CommandText = sql;
            command.CommandType = System.Data.CommandType.Text;
            return;
        }

        public override TranslateSql NewTranslation()
        {
            return new SqlServer();
        }


        /// <summary>
        /// Sqlserver 使用 rowNumber 分页。
        /// </summary>
        private bool UseRowNumber { get; set; }


        /// <summary>
        /// 解析Select命令。
        /// </summary>
        /// <param name="myCmd"></param>
        /// <returns></returns>
        public override void ToSelectCommand(MyCommand myCmd)
        {
            CheckSelectColumns();

            SelectClip curSelect = Context as SelectClip;
            var first = true;
            string linkerFormat = null;
            while (curSelect != null)
            {
                this.Context = curSelect;

                UseRowNumber = isUseRowNumber(myCmd);

                if (UseRowNumber)
                {
                    var orderClip = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.OrderBy) as OrderByClip;
                    //如果没有排序，且是单表，按主键排序
                    if (orderClip.HasData() == false && Context.Dna.Any(o => o.Key.IsIn(SqlKeyword.Join, SqlKeyword.LeftJoin, SqlKeyword.RightJoin)) == false)
                    {
                        Context.CurrentRule.GetPrimaryKeys().All(o =>
                        {
                            (this.Context as SelectClip).OrderBy(o.Asc);
                            return true;
                        });

                        orderClip = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.OrderBy) as OrderByClip;
                    }

                    var order = GetOrderExp();

                    order.ForEach(o =>
                    {
                        AutoAddOrderSql(Context, o, true);
                    });
                }

                //Action<CommandValue> skipFunc = FormatSkipSql(myCmd);

                CommandValue cmd = new CommandValue();
                var select = GetSelectText(myCmd);
                var from = GetFromText();
                var join = GetJoinText();
                var group = GetGroupText();
                var having = GetHavingText(myCmd);

                cmd &= select;
                cmd &= from;
                cmd &= join;

                var hasDistinct = Context.Dna.Any(o => o.Key == SqlKeyword.Distinct) ||
                    Context.TableHasPolymer();

                var where = GetWhereText(Context.Dna.Where(o => o.Key == SqlKeyword.Where).Select(o => o as WhereClip).FirstOrDefault());
                if (where.Sql.HasValue())
                {
                    where.Sql = " where " + where.Sql;
                    cmd &= where;
                }

                //顺序是  group by  ... having ...  order by ... 

                cmd &= group;
                cmd &= having;




                //使用分页包装器
                PagerWrapper(myCmd, cmd);

                if (first)
                {
                    myCmd.Command.CommandText = cmd.Sql;
                }
                else
                {
                    myCmd.Command.CommandText = string.Format(linkerFormat,
                        myCmd.Command.CommandText
                            .Replace('{', ValueProc.SplitLine)
                            .Replace('}', ValueProc.SplitCell)
                            + ";"
                        , cmd.Sql
                            .Replace('{', ValueProc.SplitLine)
                            .Replace('}', ValueProc.SplitCell)
                            + ";"
                        );

                    myCmd.Command.CommandText = myCmd.Command.CommandText
                            .Replace(ValueProc.SplitLine, '{')
                            .Replace(ValueProc.SplitCell, '}');
                }

                myCmd.Command.Parameters.AddRange(cmd.Parameters.Where(o => o != null).ToArray());

                if (first) first = false;


                if (curSelect.Next == null) break;

                linkerFormat = GetOperator(curSelect.Linker);
                curSelect.Next.ParameterColumn = Context.ParameterColumn;
                //curSelect.Next.Rules = Context.Rules;

                curSelect = curSelect.Next;
            }
            myCmd.Command.CommandType = System.Data.CommandType.Text;



            return;
        }

        private void PagerWrapper(MyCommand myCmd, CommandValue cmd)
        {
            if (UseRowNumber == false)
            {
                cmd &= GetOrderText();
                return;
            }

            var skipNumber = 0;
            var skip = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Skip) as SkipClip;

            if (skip != null && skip.SkipNumber >= 0)
            {
                skipNumber = skip.SkipNumber;
            }

            //有 Skip 一定要有 Take
            var take = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Take) as TakeClip;
            var takeCount = 0;
            if (take != null)
            {
                takeCount = take.TakeNumber;
                //Context.Dna.RemoveAll(o => o.Key == SqlKeyword.Take);
            }
            else
            {
                takeCount = int.MaxValue - skipNumber;
            }

            //如果是 insert select  子句 或 join ( select ) 子句， 其中 select 子句有分页，需要使用  row_number

            var orderClip = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.OrderBy) as OrderByClip;
            string strSql = string.Empty;

            var rowCount = (this.Context as SelectClip).SmartRowCount;

            var smartPager = false;
            if (rowCount > 0)
            {
                smartPager = skipNumber > rowCount / 2;
            }




            if (orderClip.HasData())
            {
                strSql = @"Select * From 
( Select Row_Number() Over ($RowNumberOrderby$) As [__IgNoRe__AutoId], * From 
( 
$OriSql$
) As [__SubQuery__] ) As [___SubQuery___]
where [__IgNoRe__AutoId] between $BeginRowNumber$ and $EndRowNumber$
$OrderbyInResult$";

                //准备各个参数：$RowNumberOrderby$,$BeginRowNumber$,$EndRowNumber$,$OrderbyInResult$
                var order = GetOrderExp();

                if (smartPager)
                {
                    var rownumberOrderby = order.Select(o => o.Order.Name + (o.IsAsc ? " desc" : " asc")).Join(",");
                    var orderbyInResult = order.Select(o => o.Order.Name + (o.IsAsc ? " asc" : " desc")).Join(",");

                    cmd.Sql = strSql.Replace("$RowNumberOrderby$", "Order by " + rownumberOrderby)
                        .Replace("$BeginRowNumber$", (rowCount - skipNumber - takeCount + 1).ToString())
                        .Replace("$EndRowNumber$", (rowCount - skipNumber).ToString())
                        .Replace("$OrderbyInResult$", "Order by " + orderbyInResult)
                        .Replace("$OriSql$", cmd.Sql);
                }
                else
                {
                    var rownumberOrderby = order.Select(o => o.Order.Name + (o.IsAsc ? " asc" : " desc")).Join(",");

                    cmd.Sql = strSql.Replace("$RowNumberOrderby$", "Order by " + rownumberOrderby)
                        .Replace("$BeginRowNumber$", (skipNumber + 1).ToString())
                        .Replace("$EndRowNumber$", (skipNumber + takeCount).ToString())
                        .Replace("$OrderbyInResult$", string.Empty)
                        .Replace("$OriSql$", cmd.Sql);
                }
            }
            else
            {
                strSql = @"Select * From 
( Select Row_Number() Over ($RowNumberOrderby$) As [__IgNoRe__AutoId], * From 
( Select 1 as [__IgNoRe__SubId] , * From 
( 
$OriSql$
) As [_SubQuery_] 
) As [__SubQuery__] 
) As [___SubQuery___]
where [__IgNoRe__AutoId] between $BeginRowNumber$ and $EndRowNumber$
$OrderbyInResult$";

                ////准备各个参数：$RowNumberOrderby$,$BeginRowNumber$,$EndRowNumber$,$OrderbyInResult$
                //if (smartPager)
                //{
                //    cmd.Sql = strSql.Replace("$RowNumberOrderby$", "Order By [__IgNoRe__SubId] desc")
                //        .Replace("$BeginRowNumber$", (rowCount - skipNumber - takeCount + 1).ToString())
                //        .Replace("$EndRowNumber$", (rowCount - skipNumber).ToString())
                //        .Replace("$OrderbyInResult$", "Order By [__IgNoRe__SubId] asc")
                //        .Replace("$OriSql$", cmd.Sql);
                //}
                //else
                {
                    cmd.Sql = strSql.Replace("$RowNumberOrderby$", "Order By [__IgNoRe__SubId] asc")
                        .Replace("$BeginRowNumber$", (skipNumber + 1).ToString())
                        .Replace("$EndRowNumber$", (skipNumber + takeCount).ToString())
                        .Replace("$OrderbyInResult$", string.Empty)
                        .Replace("$OriSql$", cmd.Sql);
                }
            }
        }

        /// <summary>
        /// 如果有Skip，处理 Dna
        /// </summary>
        private KeyValuePair<string, bool> AutoAddOrderSql(ContextClipBase Context, OrderByClip orderByValue, bool IsTop)
        {
            //由于嵌套子查询不能有  OrderBy ,需要把 OrderBy 中且查询列中不存在的，添加到选择列。同时给 OrderBy列添加别名。

            if (orderByValue.Order.EqualsNull() || (orderByValue.Order.Name.HasValue() == false)) return new KeyValuePair<string, bool>();

            //如果是系统字段，也不能直接返回。 ---- fixed by udi. 2012年9月18日
            //if (orderByValue.Order.IsConst() && orderByValue.Order.DbType.DbTypeIsNumber())
            //{
            //    return new KeyValuePair<string, bool>(orderByValue.Order.Name, orderByValue.IsAsc);
            //}

            var colInTable = false;

            if (orderByValue.Order.IsSimple() && !orderByValue.Order.IsConst())
            {
                var orderColInMasterTable = Context.Dna.FirstOrDefault(o => o.IsColumn() && (o as ColumnClip).GetAlias() == orderByValue.Order.GetAlias()) as ColumnClip;
                if (orderColInMasterTable.EqualsNull() == false)
                {
                    colInTable = true;
                    //如果在表中存在
                    orderByValue.Order.Name = orderColInMasterTable.Name;

                    return new KeyValuePair<string, bool>(GetMyName(orderByValue.Order.Name), orderByValue.IsAsc);
                }
            }

            if (colInTable == false)
            {
                //如果 OrderBy 中的列, 在查询列中不存在, 则自动添加. ------------------------------- auto fixed order by column

                //在各个表中找.
                var colInTab = Context.CurrentRule.GetColumn(orderByValue.Order.GetAlias());
                if (colInTab.EqualsNull() == false)
                {
                    //在某个Join表中找到。 起个别名，返回结果忽略该列

                    var aliasOrderName = "__IgNoRe__" + Context.GetIdValue().ToString();
                    orderByValue.Order.Name = aliasOrderName;

                    Context.Dna.Add((orderByValue.Order.Clone() as ColumnClip).As(aliasOrderName));

                    return new KeyValuePair<string, bool>(GetMyName(orderByValue.Order.Name), orderByValue.IsAsc);
                }
                else
                {
                    //遍历各个 JoinTable.
                    var joins = Context.Dna.Where(o => o.Key.IsIn(SqlKeyword.Join, SqlKeyword.LeftJoin, SqlKeyword.RightJoin)).ToArray();
                    for (var i = 0; i < joins.Length; i++)
                    {
                        var item = joins[i];
                        JoinTableClip joinTable = item as JoinTableClip;
                        if (joinTable.Table != null)
                        {
                            var colInJoin = joinTable.Table.GetColumn(orderByValue.Order.GetAlias());
                            if (colInJoin.EqualsNull() == false)
                            {
                                //在Join表中找到，同样，起个别名，返回结果忽略该列
                                var aliasOrderName = "__IgNoRe__" + Context.GetIdValue().ToString();
                                orderByValue.Order.Name = aliasOrderName;

                                Context.Dna.Add((orderByValue.Order.Clone() as ColumnClip).As(aliasOrderName));

                                return new KeyValuePair<string, bool>(GetMyName(orderByValue.Order.Name), orderByValue.IsAsc);
                            }
                        }
                        else
                        {
                            //join 的 subselect 
                            var subDict = AutoAddOrderSql(joinTable.SubSelect, orderByValue, false);
                            if (subDict.HasValue())
                            {
                                return subDict;
                            }
                        }
                    }
                }

                //如果在表中找不到，则自动添加。
                if (IsTop)
                {
                    var aliasOrderName = "__IgNoRe__" + Context.GetIdValue().ToString();
                    orderByValue.Order.Name = aliasOrderName;
                    Context.Dna.Add((orderByValue.Order.Clone() as ColumnClip).As(aliasOrderName));

                    return new KeyValuePair<string, bool>(GetMyName(orderByValue.Order.Name), orderByValue.IsAsc);
                }
            }

            throw new GodError("找不到排序列：" + orderByValue.Order.Name, null, Context.CurrentRule.GetFullName().ToString());
        }

        //private Dictionary<string, bool> GetOrderSql()
        //{
        //    var orderByValue = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.OrderBy) as OrderByClip;
        //    Dictionary<string, bool> dict = new Dictionary<string, bool>();
        //    while (orderByValue != null)
        //    {
        //        var kv = ProcSkipSql(Context, orderByValue, true);
        //        if (kv.HasValue())
        //        {
        //            dict.Add(kv.Key, kv.Value);
        //        }
        //        orderByValue = orderByValue.Next;
        //    }
        //    return dict;
        //}

        /// <summary>
        /// 判断分页模式，有两种情况要用 rownumber 分，即 1：有skip值 ， 2：有 top,且 order by 非单一主键列 的时候。
        /// </summary>
        /// <param name="myCmd"></param>
        /// <returns></returns>
        private bool isUseRowNumber(MyCommand myCmd)
        {
            var orderByValue = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.OrderBy) as OrderByClip;
            if (orderByValue != null && !orderByValue.Order.EqualsNull())
            {
                var take = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Take) as TakeClip;
                if (take != null && take.TakeNumber > 0)
                {
                    //如果排序列不包含 自增列，主键列，唯一列。 否则 返回 true 。

                    var entity = Context.CurrentRule;
                    var incKey = entity.GetAutoIncreKey();
                    var uniKey = entity.GetUniqueKey();
                    var pks = entity.GetPrimaryKeys();

                    var hasInc = false;
                    var hasUni = false;
                    var priAll = false;
                    var priCol = new Dictionary<string, bool>();
                    pks.All(o =>
                    {
                        priCol[o.DbName] = false;
                        return true;
                    });


                    while (orderByValue != null && !orderByValue.Order.EqualsNull())
                    {
                        var orderColumn = orderByValue.Order;
                        if (orderColumn.NameEquals(incKey))
                        {
                            hasInc = true;
                            break;
                        }

                        if (orderColumn.NameEquals(uniKey))
                        {
                            hasUni = true;
                            break;
                        }

                        if (pks.Any(o =>
                            {
                                if (o.NameEquals(orderColumn))
                                {
                                    priCol[o.DbName] = true;
                                    return true;
                                }
                                else return false;
                            }))
                        {
                            if (priCol.Any(o => o.Value == false) == false)
                            {
                                priAll = true;
                                break;
                            }
                        }

                        orderByValue = orderByValue.Next;
                    }

                    if (!hasInc && !hasUni && !priAll) return true;
                }
            }


            var skip = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Skip) as SkipClip;
            if (skip == null) return false;
            if (skip.SkipNumber <= 0) return false;

            return true;
        }

        //        /// <summary>
        //        /// 如果有 Skip , 则启用 分页,同时去除 Take 子句. 否则返回空.
        //        /// </summary>
        //        /// <returns>返回空,需要手动添加OrderBy 子句,否则不必添加 Order子句.</returns>
        //        private Action<CommandValue> FormatSkipSql(MyCommand myCmd)
        //        {
        //            if (UseRowNumber == false) return null;

        //            var skipNumber = 0;
        //            var skip = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Skip) as SkipClip;

        //            if (skip != null && skip.SkipNumber >= 0)
        //            {
        //                skipNumber = skip.SkipNumber;
        //            }


        //            //有 Skip 一定要有 Take
        //            //如果是 insert select  子句 或 join ( select ) 子句， 其中 select 子句有分页，需要使用  row_number

        //            string orderPara = string.Empty;

        //            if (Context.Dna.Any(o => o.Key == SqlKeyword.OrderBy))
        //            {
        //                var order = GetOrderText();

        //                if (order != null || order.Sql.HasValue())
        //                {
        //                    orderPara = string.Join(",", GetOrderSql().Select(o => o.Key + (o.Value ? " asc" : " desc")).ToArray());

        //                    if (orderPara.HasValue())
        //                    {
        //                        orderPara = "Order by " + orderPara;
        //                    }
        //                }
        //            }

        //            string strSql = string.Empty;

        //            if (orderPara.HasValue())
        //            {
        //                strSql = @"
        //Select * From 
        //( Select Row_Number() Over ({3}) As [__IgNoRe__AutoId], * From 
        //( 
        //{0}
        //) As [__SubQuery__] ) As [___SubQuery___]
        //where [__IgNoRe__AutoId] between {1} and {2}
        //{3}";
        //            }
        //            else
        //            {
        //                strSql = @"
        //Select * From 
        //( Select Row_Number() Over (Order By  [__IgNoRe__SubId] asc ) As [__IgNoRe__AutoId], * From 
        //( Select 1 as [__IgNoRe__SubId] , * From 
        //( 
        //{0} 
        //) As [_SubQuery_] 
        //) As [__SubQuery__] 
        //) As [___SubQuery___]
        //where [__IgNoRe__AutoId] between {1} and {2}
        //";
        //            }


        //            var take = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Take) as TakeClip;
        //            var takeCount = 0;
        //            if (take != null)
        //            {
        //                takeCount = take.TakeNumber;
        //                //Context.Dna.RemoveAll(o => o.Key == SqlKeyword.Take);
        //            }
        //            else
        //            {
        //                takeCount = int.MaxValue - skipNumber;
        //            }

        //            return cmd =>
        //                {
        //                    cmd.Sql = string.Format(
        //                        strSql,
        //                        cmd.Sql,
        //                        skipNumber + 1,
        //                        skipNumber + takeCount,
        //                        orderPara);
        //                };
        //        }


        protected override void ToBulkInsertCommand(MyCommand myCmd)
        {
            var set = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.MyOqlSet) as MyOqlSet;
            var dbConn = GetConnection(Context.CurrentRule.GetConfig().db);

            if (dbo.Event.OnExecute(Context) == false) return;

            dbo.Open(dbConn, () =>
            {
                SqlBulkCopy bulk = new SqlBulkCopy(dbConn as SqlConnection, SqlBulkCopyOptions.Default, (SqlTransaction)Context.Transaction);

                bulk.DestinationTableName = Context.CurrentRule.GetFullName().ToString();
                bulk.BatchSize = 900;

                DataTable dt = new DataTable();
                set.Columns.All(o =>
                {
                    dt.Columns.Add(new DataColumn(o.Key == SqlKeyword.Simple ? (o as SimpleColumn).DbName : o.Name, o.DbType.GetCsType()));
                    return true;
                });
                set.Rows.All(o =>
                {
                    var dr = dt.NewRow();

                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        var col = dt.Columns[c];
                        dr[col] = o[col.ColumnName];
                    }
                    dt.Rows.Add(dr);
                    return true;
                });
                bulk.WriteToServer(dt);

                myCmd.Command.CommandText = " Select " + set.Rows.Count.AsString();
                return true;
            });
        }


        //public override string GetAliasText(string Alias)
        //{
        //    if (Alias.HasValue()) return " As " + GetMyName(Alias);
        //    else return string.Empty;
        //}


        public override string GenSql(RuleBase rule)
        {
            var colDefine = new List<string>();
            var strSql = string.Empty;

            rule.GetColumns().All(o =>
            {
                var nulFlag = true;
                if (rule.GetPrimaryKeys().Contains(p => p.Name == o.Name)) nulFlag = false;
                else if (rule.GetAutoIncreKey().EqualsNull() == false)
                {
                    if (rule.GetAutoIncreKey().Name == o.Name) nulFlag = false;
                }

                colDefine.Add(GetMyName(o.DbName) + " " + this.ToSqlType(o.DbType) + getDbTypeLen(rule, o) +
                    (nulFlag == false ? " not null " : " null")
                    );

                return true;
            });

            strSql = "Create Table " + rule.GetFullName() + @"(
" + "\t" + colDefine.Join("," + Environment.NewLine + "\t") + @"
);
go
";

            strSql += "ALTER TABLE " + rule.GetFullName() + " ADD CONSTRAINT  PK_" + rule.GetDbName() + " PRIMARY KEY CLUSTERED (" + rule.GetPrimaryKeys().Select(o => GetMyName(o.DbName)).Join(",") + @" )ON [PRIMARY] ;
go
";

            if (rule.GetUniqueKey().EqualsNull() == false)
            {
                strSql += "CREATE UNIQUE NONCLUSTERED INDEX I_" + rule.GetDbName() + " On " + rule.GetFullName() + " (" + GetMyName(rule.GetUniqueKey().DbName) + @" asc  )ON [PRIMARY] ;
go
";
            }


            return strSql;
        }

        private string getDbTypeLen(RuleBase rule, ColumnClip col)
        {

            if (col.DbType.IsIn(DbType.AnsiString, DbType.AnsiStringFixedLength))
            {
                if (col.Length <= 0) return "(max)";
                else return "(" + col.Length.ToString() + ")";
            }

            if (col.DbType.IsIn(DbType.String, DbType.StringFixedLength))
            {
                if (col.Length <= 0) return "(max)";
                else return "(" + col.Length.ToString() + ")";
            }

            if (rule.GetAutoIncreKey().EqualsNull() == false && rule.GetAutoIncreKey().Name == col.Name)
                return " IDENTITY(1,1)";

            return string.Empty;
        }

        public override string GetOperator(IOperator Col)
        {
            SqlOperator Operator = Col.Operator;
            var retVal = base.GetOperator(Col);
            if (string.IsNullOrEmpty(retVal) || retVal == "{0}")
            {
                if (dbo.MyFunction.ContainsKey(Operator))
                {
                    return dbo.MyFunction[Operator](DatabaseType.SqlServer).AsString(retVal);
                }
            }
            return retVal;
        }

        //[Obsolete]
        //public override IEnumerable<TypeMap> GetTypeMap()
        //{
        //    List<TypeMap> _DbTypeDict = new List<TypeMap>();

        //    _DbTypeDict.Add(new TypedDict<string>(DbType.AnsiString, SqlDbType.VarChar));
        //    _DbTypeDict.Add(new TypedDict<string>(DbType.String, SqlDbType.NVarChar));
        //    _DbTypeDict.Add(new TypedDict<string>(DbType.AnsiStringFixedLength, SqlDbType.Char));
        //    _DbTypeDict.Add(new TypedDict<string>(DbType.StringFixedLength, SqlDbType.NChar));
        //    _DbTypeDict.Add(new TypedDict<string>(DbType.AnsiString, SqlDbType.Text));
        //    _DbTypeDict.Add(new TypedDict<string>(DbType.String, SqlDbType.NText));
        //    _DbTypeDict.Add(new TypedDict<string>(DbType.Xml, SqlDbType.Xml));


        //    _DbTypeDict.Add(new TypedDict<byte[]>(DbType.Binary, SqlDbType.Binary));
        //    _DbTypeDict.Add(new TypedDict<byte[]>(DbType.Binary, SqlDbType.Image));
        //    _DbTypeDict.Add(new TypedDict<byte[]>(DbType.Binary, SqlDbType.VarBinary));
        //    _DbTypeDict.Add(new TypedDict<byte[]>(DbType.Binary, SqlDbType.Timestamp));

        //    _DbTypeDict.Add(new TypedDict<DateTime>(DbType.DateTime, SqlDbType.DateTime));
        //    _DbTypeDict.Add(new TypedDict<DateTime>(DbType.DateTime, SqlDbType.SmallDateTime));
        //    _DbTypeDict.Add(new TypedDict<DateTime>(DbType.DateTime, SqlDbType.Date));
        //    _DbTypeDict.Add(new TypedDict<DateTime>(DbType.Time, SqlDbType.Time));
        //    _DbTypeDict.Add(new TypedDict<DateTime>(DbType.DateTime2, SqlDbType.DateTime2));
        //    _DbTypeDict.Add(new TypedDict<DateTime>(DbType.DateTimeOffset, SqlDbType.DateTimeOffset));


        //    _DbTypeDict.Add(new TypedDict<char>(DbType.AnsiStringFixedLength, SqlDbType.Char));
        //    _DbTypeDict.Add(new TypedDict<bool>(DbType.Boolean, SqlDbType.Bit));
        //    _DbTypeDict.Add(new TypedDict<byte>(DbType.Byte, SqlDbType.TinyInt));
        //    _DbTypeDict.Add(new TypedDict<int>(DbType.Int32, SqlDbType.Int));
        //    _DbTypeDict.Add(new TypedDict<Guid>(DbType.Guid, SqlDbType.UniqueIdentifier));
        //    _DbTypeDict.Add(new TypedDict<Int64>(DbType.Int64, SqlDbType.BigInt));


        //    _DbTypeDict.Add(new TypedDict<Int16>(DbType.Int16, SqlDbType.SmallInt));


        //    _DbTypeDict.Add(new TypedDict<decimal>(DbType.Decimal, SqlDbType.Decimal));
        //    _DbTypeDict.Add(new TypedDict<decimal>(DbType.Currency, SqlDbType.Money));
        //    _DbTypeDict.Add(new TypedDict<decimal>(DbType.Decimal, SqlDbType.SmallMoney));

        //    _DbTypeDict.Add(new TypedDict<float>(DbType.Single, SqlDbType.Float));
        //    _DbTypeDict.Add(new TypedDict<float>(DbType.Single, SqlDbType.Real));

        //    _DbTypeDict.Add(new TypedDict<double>(DbType.Double, SqlDbType.Float));
        //    _DbTypeDict.Add(new TypedDict<object>(DbType.Object, SqlDbType.Variant));
        //    _DbTypeDict.Add(new TypedDict<object>(DbType.Object, SqlDbType.Udt));
        //    _DbTypeDict.Add(new TypedDict<object>(DbType.Object, SqlDbType.Structured));


        //    //解决Bug,见:http://www.cnblogs.com/newsea/archive/2011/12/26/2302198.html
        //    // SByte=14,
        //    // UInt16=18,
        //    // UInt32=19,
        //    // UInt64=20,
        //    // VarNumeric
        //    _DbTypeDict.Add(new TypedDict<decimal>(DbType.VarNumeric, SqlDbType.Decimal));
        //    _DbTypeDict.Add(new TypedDict<Int16>(DbType.SByte, SqlDbType.SmallInt));
        //    _DbTypeDict.Add(new TypedDict<UInt16>(DbType.UInt16, SqlDbType.SmallInt));
        //    _DbTypeDict.Add(new TypedDict<UInt32>(DbType.UInt32, SqlDbType.Int));
        //    _DbTypeDict.Add(new TypedDict<UInt64>(DbType.UInt64, SqlDbType.BigInt));


        //    _DbTypeDict.Add(new TypedDict<MyDate>(DbType.DateTime, SqlDbType.DateTime));
        //    _DbTypeDict.Add(new TypedDict<MyDate>(DbType.DateTime, SqlDbType.SmallDateTime));
        //    _DbTypeDict.Add(new TypedDict<MyDate>(DbType.DateTime, SqlDbType.Date));
        //    _DbTypeDict.Add(new TypedDict<MyDate>(DbType.Time, SqlDbType.Time));
        //    _DbTypeDict.Add(new TypedDict<MyDate>(DbType.DateTime2, SqlDbType.DateTime2));
        //    _DbTypeDict.Add(new TypedDict<MyDate>(DbType.DateTimeOffset, SqlDbType.DateTimeOffset));


        //    return _DbTypeDict;
        //}

        //public override void GenFullSql(MyCommand myCmd)
        //{
        //    string fullSql = "exec sp_executesql N'" + myCmd.Command.CommandText.Replace("'", "''") + "'";
        //    var map = new SqlServer().GetTypeMap();
        //    var parasDefine = new List<string>();
        //    var parasVal = new List<string>();

        //    foreach (DbParameter item in myCmd.Command.Parameters)
        //    {
        //        var typeMap = map.First(o => o.DbType == item.DbType);
        //        if (typeMap.DbType.DbTypeIsString())
        //        {
        //            parasDefine.Add(item.ParameterName + " " + typeMap.SqlType.ToString() + "(" + item.Value.AsString().Length + ")");
        //        }
        //        else
        //        {
        //            parasDefine.Add(item.ParameterName + " " + typeMap.SqlType.ToString());

        //        }

        //        if (item.Value.IsDBNull())
        //        {
        //            parasVal.Add(item.ParameterName + "= null");
        //        }
        //        else if (typeMap.DbType.DbTypeIsNumber())
        //        {
        //            parasVal.Add(item.ParameterName + "=" + item.Value.AsString());
        //        }
        //        else
        //        {
        //            parasVal.Add(item.ParameterName + "='" + item.Value.AsString().Replace("'", "''") + "'");
        //        }
        //    }

        //    if (parasDefine.Count > 0)
        //    {
        //        fullSql += ",N'" + string.Join(",", parasDefine.ToArray()) + "'";

        //        parasVal.ForEach(o =>
        //        {
        //            fullSql += "," + o;
        //        });
        //    }

        //    myCmd.FullSql = fullSql;
        //}

        public override string GetWithLockSql(RuleBase rule)
        {
            //如果不是表，则跳过。
            if ((rule as ITableRule) == null) return string.Empty;

            //如果指定了事务，则跳过。
            if (Transaction.Current != null) return string.Empty;
            if (Context.Transaction != null) return string.Empty;

            if (Context.Key != SqlKeyword.Select) return string.Empty;

            var ruleConfig = rule.GetRecofing();
            if (ruleConfig.Contains(ReConfigEnum.NoLock))
            {
                return "with(nolock)";
            }
            else if (ruleConfig.Contains(ReConfigEnum.ReadPast))
            {
                return "with(readpast)";
            }
            else if (MyOqlConfigScope.Config != null && MyOqlConfigScope.Config.IsValueCreated)
            {
                if (MyOqlConfigScope.Config.Value.Contains(ReConfigEnum.ReadPast))
                {
                    return "with(readpast)";
                }
                else if (MyOqlConfigScope.Config.Value.Contains(ReConfigEnum.NoLock))
                {
                    return "with(nolock)";
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 对 Sqlserver 来说， varchar(max) = text , nvarchar(max) = ntext
        /// 对数据库来说，应该尽量使用 varchar(max) ，所以 MyOql 中的 text 在传递给数据库时，使用  varchar(max)  而没有了 text
        /// </remarks>
        /// <param name="type"></param>
        /// <returns></returns>
        //public override string ToSqlType(DbType type)
        //{
        //    switch (type)
        //    {
        //        case DbType.AnsiString:
        //            return "varchar";
        //        case DbType.AnsiStringFixedLength:
        //            return "char";
        //        case DbType.Binary:
        //            break;
        //        case DbType.Boolean:
        //            return "bit";
        //        case DbType.Byte:
        //            return "tinyint";
        //        case DbType.Currency:
        //            return "Decimal";
        //        case DbType.Date:
        //            return "Date";
        //        case DbType.DateTime:
        //            return "DateTime";
        //        case DbType.DateTime2:
        //            return "DateTime2";
        //        case DbType.DateTimeOffset:
        //            return "datetimeoffset";
        //        case DbType.Decimal:
        //            return "Decimal";
        //        case DbType.Double:
        //            return "bigint";
        //        case DbType.Guid:
        //            return "uniqueidentifier";
        //        case DbType.Int16:
        //            return "smallint";
        //        case DbType.Int32:
        //            return "int";
        //        case DbType.Int64:
        //            return "bigint";
        //        case DbType.Object:
        //            break;
        //        case DbType.SByte:
        //            return "tinyint";
        //        case DbType.Single:
        //            return "float";
        //        case DbType.String:
        //            return "Nvarchar";
        //        case DbType.StringFixedLength:
        //            return "Nchar";
        //        case DbType.Time:
        //            return "time";
        //        case DbType.UInt16:
        //            return "smallint";
        //        case DbType.UInt32:
        //            return "int";
        //        case DbType.UInt64:
        //            return "bigint";
        //        case DbType.VarNumeric:
        //            return "Decimal";
        //        case DbType.Xml:
        //            return "Xml";
        //        default:
        //            break;
        //    }

        //    return type.ToString();
        //}
    }
}
