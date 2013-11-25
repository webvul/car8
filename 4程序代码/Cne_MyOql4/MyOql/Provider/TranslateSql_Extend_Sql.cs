using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using MyCmn;
using System;

namespace MyOql.Provider
{
    public abstract partial class TranslateSql
    {
        /// <summary>
        /// 生成 From 子句Sql语句。
        /// </summary>
        /// <returns></returns>
        protected virtual CommandValue GetFromText()
        {
            CommandValue command = new CommandValue();

            if (Context.Key == SqlKeyword.Update)
            {
                var fullName = GetTableFullName(Context.CurrentRule);
                if (fullName.HasValue() == false) return command;

                command.Sql = " From " + GetTableFullName(Context.CurrentRule);

                command.Sql += string.Format(GetOperator(SqlOperator.As), string.Empty, GetMyName(Context.CurrentRule.GetName()));

                return command;

            }
            else if (Context.Key == SqlKeyword.Select)
            {
                var subSelect = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Select) as ContextClipBase;
                if (subSelect == null)
                {
                    var fullName = GetTableFullName(Context.CurrentRule);
                    if (fullName.HasValue() == false) return command;

                    command.Sql = " From " + GetTableFullName(Context.CurrentRule);

                    command.Sql += string.Format(GetOperator(SqlOperator.As), string.Empty, GetMyName(Context.CurrentRule.GetName()));

                    GetWithLockSql(Context.CurrentRule).HasValue(o => command.Sql += " " + o);

                    return command;
                }
                //如果是子查询.
                else
                {
                    subSelect.ParameterColumn = Context.ParameterColumn;
                    //subSelect.Rules = Context.Rules;
                    var subSelectCommand = subSelect.ToCommand(this.Context);
                    command.Sql += " From (" + subSelectCommand.Command.CommandText + ")";
                    var subSel = subSelect as SelectClip;

                    if (subSel.Alias.HasValue())
                    {
                        command.Sql += string.Format(GetOperator(SqlOperator.As), string.Empty, GetMyName(subSel.Alias));
                    }

                    foreach (DbParameter para in subSelectCommand.Command.Parameters)
                    {
                        command.Parameters.Add(para);
                    }

                    subSelectCommand.Command.Parameters.Clear();
                    return command;
                }
            }
            return command;
        }

        public CommandValue GetWhereText(WhereClip where)
        {
            return GetWhereText(where, true);
        }

        public virtual CommandValue GetWhereText(WhereClip where, bool withTableAlias)
        {
            CommandValue whereSect = new CommandValue();

            if (where == null) return whereSect;

            StringBuilder sb = new StringBuilder();

            //sb.Append(where.Query + " " + GetOperator(where.Operator) + " " + where.Value.AsString());

            // 不必克隆， Dna 引用不会被破坏。
            WhereClip curWhere = where;

            while (curWhere != null)
            {
                CommandValue cmd1 = null;
                if (curWhere.Query.EqualsNull())
                {
                    cmd1 = GetWhereText(curWhere.Child, withTableAlias);
                    if (cmd1.Sql.HasValue())
                    {
                        sb.Append("(" + cmd1.Sql + ")");
                    }
                }

                else
                {
                    //处理 非子查询 的情况。
                    bool ValueProcNull = false;
                    if (curWhere.Operator == SqlOperator.Equal || curWhere.Operator == SqlOperator.NotEqual)
                    {
                        if (curWhere.Value.IsDBNull())
                        {
                            ValueProcNull = true;
                        }
                        else if ((curWhere.Value as ColumnClip).EqualsNull() && curWhere.Query.DbType.DbTypeIsDateTime() && (curWhere.Value.AsString() != string.Empty && curWhere.Value.AsDateTime() == DateTime.MinValue))
                        {
                            ValueProcNull = true;
                        }
                    }

                    if (ValueProcNull)
                    {
                        sb.Append(string.Format(" {0} {1} null ",
                            GetColumnExpression(curWhere.Query, withTableAlias),
                            (curWhere.Operator == SqlOperator.Equal ? "is" : "is not")));
                    }
                    else
                    {
                        cmd1 = GetOnlyWhereValue(curWhere);
                        sb.Append(string.Format(GetOperator(curWhere), ToParaValues(GetColumnExpression(curWhere.Query, withTableAlias), cmd1.Sql)));
                    }
                }

                if (cmd1 != null && cmd1.Parameters != null)
                {
                    whereSect.Parameters.AddRange(cmd1.Parameters);
                }

                if (curWhere.Linker == 0) break;
                sb.Append(" " + GetOperator(curWhere.Linker) + " ");

                curWhere = curWhere.Next;
            }

            whereSect.Sql = sb.ToString();

            return whereSect;
        }

        /// <summary>
        /// 生成 Group by 子句Sql语句。
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        protected virtual CommandValue GetGroupText()
        {
            CommandValue whereSect = new CommandValue();

            object objGroup = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.GroupBy);
            if (objGroup == null) return whereSect;

            GroupByClip group = objGroup as GroupByClip;

            whereSect.Sql = " group by " + string.Join(",", group.Groups.Select(o => GetColumnExpression(o)).ToArray());

            return whereSect;
        }

        /// <summary>
        /// 生成 Order by  子句Sql语句。
        /// </summary>
        /// <returns></returns>
        protected CommandValue GetOrderText()
        {
            CommandValue whereSect = new CommandValue();

            var orderSql = GetOrderExp().Select(o => GetColumnExpression(o.Order) + " " + (o.IsAsc ? "asc" : "desc")).Join(",");
            if (orderSql.HasValue())
            {
                whereSect.Sql = " Order by " + orderSql;
            }

            return whereSect;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>列，是否升序的字典</returns>
        protected virtual List<OrderByClip> GetOrderExp()
        {
            var ret = new List<OrderByClip>();

            var objOrder = Context.Dna.Where(o => o.Key == SqlKeyword.OrderBy).ToArray();
            if (objOrder.Length == 0) return ret;

            foreach (OrderByClip item in objOrder)
            {
                var order = item;
                while (order != null)
                {
                    if (order.HasData() == false) break;
                    ret.Add(order);
                    order = order.Next;
                }
            }

            return ret;
        }

        /// <summary>
        /// 生成 Having 子句Sql语句。
        /// </summary>
        /// <param name="myCmd"></param>
        /// <returns></returns>
        protected virtual CommandValue GetHavingText(MyCommand myCmd)
        {
            CommandValue whereSect = new CommandValue();

            object objOrder = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Having);
            if (objOrder == null) return whereSect;

            HavingClip group = objOrder as HavingClip;

            whereSect = GetWhereText(group.Where);
            whereSect.Sql = " having " + whereSect.Sql;

            return whereSect;
        }


        //protected virtual CommandValue GetWhereWithHavingText(MyCommand myCmd)
        //{
        //    CommandValue whereSect = null;
        //    var where = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Where) as WhereClip;
        //    var hasWhere = true;
        //    whereSect = GetWhereText(where);

        //    if (whereSect == null || whereSect.Sql.HasValue() == false)
        //    {
        //        whereSect = new CommandValue();
        //        hasWhere = false;
        //    }

        //    object objOrder = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Having);
        //    if (objOrder == null) return whereSect;

        //    HavingClip group = objOrder as HavingClip;

        //    var groupText = GetWhereText(group.Where);
        //    if (groupText != null && groupText.Sql.HasValue())
        //    {
        //        if (hasWhere)
        //        {
        //            whereSect.Sql += " and ";
        //        }
        //        whereSect &= groupText;
        //    }


        //    return whereSect;
        //}

        /// <summary>
        /// 生成 Join 子句Sql语句。
        /// </summary>
        /// <returns></returns>
        protected virtual CommandValue GetJoinText()
        {
            CommandValue whereSect = new CommandValue();
            var sbSql = new StringLinker();

            foreach (var item in Context.Dna.Where(o => o.Key.IsIn(SqlKeyword.Join, SqlKeyword.LeftJoin, SqlKeyword.RightJoin)))
            {
                JoinTableClip joinTable = item as JoinTableClip;
                string strJoin = "";
                switch (joinTable.Key)
                {
                    case SqlKeyword.LeftJoin: strJoin = "left join";
                        break;
                    case SqlKeyword.RightJoin: strJoin = "right join";
                        break;
                    case SqlKeyword.Join: strJoin = "inner join";
                        break;
                    default:
                        break;
                }

                if (joinTable.Table != null)
                {
                    sbSql += " " + strJoin + " " + GetTableFullName(joinTable.Table);


                    //if (joinTable.Table.IsAliased())
                    {
                        sbSql += string.Format(GetOperator(SqlOperator.As), string.Empty, GetMyName(joinTable.Table.GetName()));
                    }


                    GetWithLockSql(joinTable.Table).HasValue(o => sbSql += " " + o);
                }
                else
                {
                    joinTable.SubSelect.Connection = this.Context.Connection;

                    var subSelect = joinTable.SubSelect.ToCommand(this.Context);
                    sbSql += " " + strJoin + "(" + subSelect.Command.CommandText + ")";
                    var sel = joinTable.SubSelect as SelectClip;

                    if (sel.Alias.HasValue())
                    {
                        sbSql += string.Format(GetOperator(SqlOperator.As), string.Empty, sel.Alias);
                    }

                    //var subAs = joinTable.SubSelect.Dna.FirstOrDefault(o => o.Key == SqlKeyword.As);
                    //if (subAs != null)
                    //{
                    //    var ac = subAs as AsClip;
                    //    if (ac != null)
                    //    {
                    //        if (ac.Alias.HasValue())
                    //        {
                    //            sbSql += string.Format(GetOperator(SqlOperator.As), string.Empty, ac.Alias);
                    //        }
                    //    }
                    //}


                    foreach (DbParameter para in subSelect.Command.Parameters)
                    {
                        whereSect.Parameters.Add(para);
                    }

                    subSelect.Command.Parameters.Clear();
                }
                var wh = GetWhereText(joinTable.OnWhere);
                sbSql += " on (" + wh.Sql + ")";


                whereSect.Parameters.AddRange(wh.Parameters.ToArray());
            }

            whereSect.Sql = sbSql.ToString();
            return whereSect;
        }


    }
}
