using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyOql;
using MyCmn;
using System.Data.Common;
using MyBiz;
using System.Configuration;
using DbEnt;
using System.Text;
using System.Data;


namespace MyBiz.Sys
{
    public partial class MyOqlEvent
    {


        bool dbo_Creating(ContextClipBase Context)
        {
            //var Entity = Context.CurrentRule;

            //如果不是 insert Select 
            if (Context.Dna.Any(o => o.Key == SqlKeyword.Select) == false)
            {
                var model = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Model) as ModelClip;
                dbo.Check(model == null, "添加的实体不能为空", Context.CurrentRule);

                if ("更新时数据截断" == "更新时数据截断")
                {
                    for (var i = 0; i < model.Model.Count; i++)
                    {
                        var o = model.Model.ElementAt(i);
                        if (o.Key.Length > 0)
                        {
                            if (o.Key.DbType.IsIn(DbType.AnsiString, DbType.AnsiStringFixedLength))
                            {
                                var val = o.Value as string;
                                if (val != null)
                                {
                                    var charLen = UnicodeEncoding.Default.GetCharCount(UnicodeEncoding.Default.GetBytes(val));

                                    if (charLen > o.Key.Length)
                                    {
                                        var newVal = val.Substring(0, (o.Key.Length / 1.5).AsInt());
                                        charLen = UnicodeEncoding.Default.GetCharCount(UnicodeEncoding.Default.GetBytes(newVal));

                                        if (charLen > o.Key.Length)
                                        {
                                            model.Model[o.Key] = val.Substring(0, o.Key.Length / 2);
                                        }
                                        else
                                        {
                                            model.Model[o.Key] = newVal;
                                        }
                                    }
                                }
                            }
                            else if (o.Key.DbType.IsIn(DbType.String, DbType.StringFixedLength))
                            {
                                var val = o.Value as string;
                                if (val != null)
                                {
                                    if (val.Length > o.Key.Length)
                                    {
                                        model.Model[o.Key] = val.Substring(0, o.Key.Length);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (Context.ContainsConfig(ReConfigEnum.SkipPower)) return true;



            //如果主表和关联表都没有启用权限，直接返回 true
            if (Context.Rules.All(o =>
            {
                var con = new MyContextClip(Context, o).GetUsePower();
                if (con.Contains(MyOqlActionEnum.Read) || con.Contains(MyOqlActionEnum.Row))
                {
                    return false;
                }
                else return true;
            }))
            {
                return true;
            }


            var myPower = MySession.GetMyPower();
            GodError.Check(myPower == null, () => "请登录！");

            var select = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Select) as SelectClip;

            if (myPower.Create.Max && (select == null))
            {
                return true;
            }

            //行集权限只有 Insert Select 一种情况
            if (select != null)
            {
                if (myPower.Row.View == null || myPower.Row.View.Count == 0) return false;

                Power_Select(select, myPower.Row.View);
            }

            return true;


            //日后完善数据权限。
            //    if (string.Equals(ConfigurationManager.AppSettings["MyDataPower"], "true", StringComparison.CurrentCultureIgnoreCase))
            //    {
            //        var tab = dbr.PowerTable.NoPower()
            //            .FindFirst(o => o.Table == Entity.GetFullName().AsString() | o.Table == Entity.GetName() | o.Table == Entity.GetDbName(), o => o._);
            //        if (tab == null) return retVal;

            //        var cols = dbr.PowerColumn.NoPower()
            //            .SelectWhere(o => o.TableID == tab.Id).ToEntityList(o => o._);
            //        if (cols == null || cols.Count == 0) return retVal;


            //        List<ColumnClip> koColumns = new List<ColumnClip>();
            //        cols.All(o =>
            //        {
            //            if ((MySession.GetMyPower().Create & MyBigInt.CreateByBitPositon((uint)o.Id)) == MyBigInt.Zero)
            //            {
            //                var col = Columns.FirstOrDefault(c => c.Name == o.Column);
            //                if (col.EqualsNull() == false)
            //                {
            //                    koColumns.Add(col);
            //                }
            //            }
            //            return true;
            //        });
            //        retVal.KoColumns = koColumns.ToArray();

            //        if (koColumns.Count >= Columns.Count())
            //        {
            //            retVal.IsKoed = true;
            //        }
            //    }

            // CrudMaxFunc(MyOqlActionEnum.Create, Context , null, model.Model.Select(o => o.Key).ToArray(), o =>
            //{
            //    if (o.IsKoed)
            //    {
            //        LogTo(MyOqlActionEnum.Create,Context.CurrentRule, InfoEnum.Error,
            //            () => string.Format(@"无权限对表 {0} 进行创建,已忽略全部列({1})",
            //            Context.CurrentRule.GetFullName(), string.Join(",", o.KoColumns.Select(c => c.Name).ToArray()))
            //            );
            //    }
            //    else if (o.KoColumns.Count() > 0)
            //    {
            //        LogTo(MyOqlActionEnum.Create, Context.CurrentRule, InfoEnum.Error,
            //            () => string.Format(@"对表 {0} 进行创建时过滤了列: {1}",
            //            Context.CurrentRule.GetFullName(), string.Join(",", o.KoColumns.Select(c => c.Name).ToArray()))
            //            );
            //    }
            //});


            ////测试能否移除. udi 
            //if (result.KoColumns != null)
            //{
            //    for (int i = 0; i < result.KoColumns.Count(); i++)
            //    {
            //        //model.Model.FirstOrDefault(o => o.Key.GetFullName() == result.KoColumns[i].GetFullName());
            //        model.Model.Remove(result.KoColumns.ElementAt(i));
            //    }
            //}

            //if (0 == model.Model.Count)
            //{
            //    return false; // Context.Dna.Add(new ConstRule().ConstColumn);
            //}


            return true;
        }

        bool dbo_Deleting(ContextClipBase Context)
        {
            //暂时关闭 删除权限 Udi 2012年8月19日10:29:40
            return true;
            if (Context.ContainsConfig(ReConfigEnum.SkipPower)) return true;

            if (Context.GetUsePower().Contains(MyOqlActionEnum.Delete) == false)
            {
                if (Context.Rules.All(o =>
                {
                    var con = new MyContextClip(Context, o).GetUsePower();
                    if (con.Contains(MyOqlActionEnum.Read) || con.Contains(MyOqlActionEnum.Row))
                    {
                        return false;
                    }
                    else return true;
                }))
                {
                    return true;
                }
            }
            //暂时关闭 删除权限 Udi 2012年8月19日10:29:40
            return true;

            //删除 本身和 Where In（Select）也涉及行集权限。
            var myPower = MySession.GetMyPower();
            GodError.Check(myPower == null, () => "请登录！");

            //当前 Where条件（In 表， In子查询），Join表，Join子查询。
            var Entity = Context.CurrentRule;
            var where = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Where) as WhereClip;

            if (where == null)
            {
                where = new WhereClip();
                Context.Dna.Add(where);
            }

            //没有启用行集，且查看权限最大。
            if (Context.GetUsePower().Contains(MyOqlActionEnum.Row))
            {
                string Id = null;
                if (where.IsNull() == false)
                {
                    Id = where.GetIdValue(Entity);
                }

                FilterReadRow(myPower.Row.Delete, Entity, where, Id);
            }



            if (where != null)
            {
                where.RecusionQuery(select =>
                {
                    Power_Select(select, myPower.Row.View);
                    return true;
                });
            }


            //if (Context.GetUsePower().Contains(MyOqlActionEnum.Delete) == false)
            //{
            //    //如果没有启用 Action 权限.则直接返回默认结果
            //    return true;
            //}


            //Func<string> idFunc = () =>
            //{
            //    var where = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Where) as WhereClip;
            //    if (WhereClip.IsNull(where) == false)
            //    {
            //        return where.GetIdValue(Context.CurrentRule);
            //    }
            //    return null;
            //};

            //if (string.Equals(ConfigurationManager.AppSettings["MyDataPower"], "true", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    var myPower = MySession.GetMyPower();
            //    //如果没有配置权限过滤. 则返回true 
            //    if ((Entity.GetUsePower() & MyOqlActionEnum.Delete) == 0) return true;

            //    if (myPower == null) return false;
            //    if (myPower.Delete.Max == true) return true;

            //    var tab = dbr.PowerTable.NoPower().FindFirst(o => o.Table == Entity.GetFullName().DbName, o => o._);
            //    if (tab == null) return true;

            //    if ((MySession.GetMyPower().Delete & MyBigInt.CreateByBitPositon((uint)tab.Id)) == MyBigInt.Zero)
            //    {
            //        LogTo(MyOqlActionEnum.Delete, Entity, InfoEnum.Error, () => string.Format(@"无权限对表 {0} 进行删除,试除删除的ID: {1}", Entity.GetFullName(), Id));

            //        return false;
            //    }
            //}
            return true;
        }

        bool dbo_BulkUpdating(ContextClipBase Context)
        {
            //var model = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.MyOqlSet) as MyOqlSet;
            //dbo.Check(model == null, "批量添加的实体不能为空", Context.CurrentRule);


            //  CrudMaxFunc(MyOqlActionEnum.Update, Entity, null, Data.Columns, o =>
            //{
            //    if (o.IsKoed)
            //    {
            //        LogTo(MyOqlActionEnum.Read, Entity, InfoEnum.Error,
            //            () => string.Format(@"无权限对表 {0} 进行批量创建,已忽略全部列({1})",
            //            Entity.GetFullName(), string.Join(",", o.KoColumns.Select(c => c.Name).ToArray()))
            //            );
            //    }
            //    else if (o.KoColumns.Count() > 0)
            //    {
            //        LogTo(MyOqlActionEnum.Read, Entity, InfoEnum.Error,
            //            () => string.Format(@"对表 {0} 进行批量创建时过滤了列: {1}",
            //            Entity.GetFullName(), string.Join(",", o.KoColumns.Select(c => c.Name).ToArray()))
            //            );
            //    }
            //});



            //  if (result.IsKoed) return false;

            //  //测试能否移除. udi 
            //  if (result.KoColumns != null)
            //  {
            //      model.Columns = model.Columns.Minus(result.KoColumns).ToArray();
            //  }

            //  if (0 == model.Columns.Length)
            //  {
            //      return false; // Context.Dna.Add(new ConstRule().ConstColumn);
            //  }

            return true;
        }
        bool dbo_Procing(ContextClipBase Context)
        {
            //暂时关闭， udi 2012年11月10日
            return true;
            if (Context.ContainsConfig(ReConfigEnum.SkipPower)) return true;


            //if (string.Equals(ConfigurationManager.AppSettings["MyDataPower"], "true", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    if (Entity.GetUsePower().Contains(MyOqlActionEnum.Proc) == false) return true;

            //    if (MySession.GetMyPower().Delete.Max == true) return true;

            //    var tab = dbr.PowerTable.NoPower().FindFirst(o => o.Table == Entity.GetFullName().DbName, o => o._);
            //    if (tab == null) return true;

            //    if ((MySession.GetMyPower().Delete & MyBigInt.CreateByBitPositon((uint)tab.Id)) == MyBigInt.Zero)
            //    {
            //        LogTo(MyOqlActionEnum.Proc, Entity, InfoEnum.Error, () => string.Format(@"无权限执行存储过程 {0} , 尝试参数: {1}", Entity.GetFullName(), Data.ToJson()));

            //        return false;
            //    }
            //}

            return true;
        }



        bool dbo_Reading(ContextClipBase Context)
        {
            if (Context.ContainsConfig(ReConfigEnum.SkipPower)) return true;

            //如果所有实体都没有配置权限，则跳过。
            if (Context.Rules.All(o =>
            {
                var con = new MyContextClip(Context, o).GetUsePower();
                if (con.Contains(MyOqlActionEnum.Read) || con.Contains(MyOqlActionEnum.Row)) return false;
                return true;
            })) return true;


            var myPower = MySession.GetMyPower();
            GodError.Check(myPower == null, () => "请登录！");


            Power_Select(Context, myPower.Row.View);

            return true;
        }

        private static void Power_Select(ContextClipBase Context, Dictionary<string, MyBigInt> myPower)
        {
            //对于每个查询，都要看是否有权限，并注入。
            (Context as SelectClip).Recusion(s =>
            {
                var Entity = s.CurrentRule;

                //没有启用行集，且查看权限最大。
                if (s.GetUsePower().Contains(MyOqlActionEnum.Row))
                {
                    string Id = null;
                    var where = s.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Where) as WhereClip;

                    if (where == null)
                    {
                        where = new WhereClip();
                        s.Dna.Add(where);
                    }
                    else if (where.IsNull() == false)
                    {
                        Id = where.GetIdValue(Entity);
                    }


                    FilterReadRow(myPower, Entity, where, Id);
                }


                //判断该Join表是否应用行集权限。
                s.Dna.Where(o => o.Key.IsIn(SqlKeyword.Join, SqlKeyword.LeftJoin, SqlKeyword.RightJoin))
                    .Select(o => o as JoinTableClip)
                    .ToList()
                    .All(o =>
                    {
                        if (o.Table != null)
                        {
                            //仅仅Join了一个表。
                            if (MyContextClip.CreateBySelect(o.Table).GetUsePower().Contains(MyOqlActionEnum.Row))
                            {
                                if (o.OnWhere == null) { o.OnWhere = new WhereClip(); }

                                FilterReadRow(myPower, o.Table, o.OnWhere, null);
                            }
                        }
                        return true;
                    });
                return true;
            });
        }

        private static void FilterReadRow(Dictionary<string, MyBigInt> myPower, RuleBase Entity, WhereClip where, string Id)
        {
            if (Entity.GetIdKey().NameEquals(Entity.GetUniqueNumberKey()))
            {
                // 如果有行集权限， 权限字里没有该表，则不能查看。 
                if (myPower.ContainsKey(Entity.GetDbName()) == false)
                {
                    where.Query = new ConstColumn(1);
                    where.Operator = SqlOperator.Equal;
                    where.Value = 0;

                }
                else if (myPower[Entity.GetDbName()].Max == false)
                {
                    //单条记录检查.仅简单过滤: 数字唯一列 与 系统ID 列是一致的情况.

                    if (Id.HasValue())
                    {
                        if (myPower[Entity.GetDbName()].ToPositions().Contains(Id.AsInt()) == false)
                        {
                            where.Query = new ConstColumn(1);
                            where.Operator = SqlOperator.Equal;
                            where.Value = 0;
                        }
                    }
                    else
                    {
                        //如果是多条更新， where  name like 'abc%' ,则追加条件 。 
                        var myRows = myPower[Entity.GetDbName()].ToPositions().ToArray();

                        if (myRows.Length == 0)
                        {
                            //行集权限不能出现 0 , 所以  id in ( 0 )  也意味着  1=0 
                            myRows = new int[] { 0 };
                        }

                        //对每个Select子句进行行集过滤。
                        var numberColumn = Entity.GetUniqueNumberKey();

                        if (where.IsNull())
                        {
                            where.Query = numberColumn.Clone() as ColumnClip;
                            where.Operator = SqlOperator.In;
                            where.Value = myRows;
                        }
                        else
                        {
                            var whereClone = where.Clone() as WhereClip;

                            where.Query = numberColumn.Clone() as ColumnClip;
                            where.Operator = SqlOperator.In;
                            where.Value = myRows;

                            where.Linker = SqlOperator.And;
                            where.Next = whereClone;
                        }
                    }
                }
            }
            return;
        }


        bool dbo_Updating(ContextClipBase Context)
        {
            //如果不是 update Select 
            if (Context.Dna.Any(o => new SqlKeyword[] { SqlKeyword.LeftJoin, SqlKeyword.RightJoin, SqlKeyword.Join }.Contains(o.Key)) == false)
            {
                var model = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Model) as ModelClip;
                dbo.Check(model == null, "更新的实体不能为空", Context.CurrentRule);

                for (var i = 0; i < model.Model.Count; i++)
                {
                    var o = model.Model.ElementAt(i);
                    if (o.Key.Length > 0)
                    {
                        if (o.Key.DbType.IsIn(DbType.AnsiString, DbType.AnsiStringFixedLength))
                        {
                            var val = o.Value.AsString();
                            var charLen = UnicodeEncoding.Default.GetCharCount(UnicodeEncoding.Default.GetBytes(val));

                            if (charLen > o.Key.Length)
                            {
                                var newVal = val.Substring(0, (o.Key.Length / 1.5).AsInt());
                                charLen = UnicodeEncoding.Default.GetCharCount(UnicodeEncoding.Default.GetBytes(newVal));

                                if (charLen > o.Key.Length)
                                {
                                    model.Model[o.Key] = val.Substring(0, o.Key.Length / 2);
                                }
                                else
                                {
                                    model.Model[o.Key] = newVal;
                                }
                            }
                        }
                        else if (o.Key.DbType.IsIn(DbType.String, DbType.StringFixedLength))
                        {
                            var val = o.Value.AsString();
                            if (val.Length > o.Key.Length)
                            {
                                model.Model[o.Key] = val.Substring(0, o.Key.Length);
                            }
                        }
                    }
                }
            }

            if (Context.ContainsConfig(ReConfigEnum.SkipPower)) return true;

            //如果所有实体都没有启用权限。则跳过。
            if (Context.Rules.All(o =>
            {
                var con = new MyContextClip(Context, o).GetUsePower();
                if (con.Contains(MyOqlActionEnum.Update) || con.Contains(MyOqlActionEnum.Row) || con.Contains(MyOqlActionEnum.Read)) return false;
                return true;
            })) return true;

            //var model = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Model) as ModelClip;

            var myPower = MySession.GetMyPower();
            GodError.Check(myPower == null, () => "请登录！");


            //当前 Where条件（In 表， In子查询），Join表，Join子查询。
            var Entity = Context.CurrentRule;
            var where = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Where) as WhereClip;

            if (where == null)
            {
                where = new WhereClip();
                Context.Dna.Add(where);
            }

            //没有启用行集，且查看权限最大。
            if (Context.GetUsePower().Contains(MyOqlActionEnum.Row))
            {
                string Id = null;
                if (where.IsNull() == false)
                {
                    Id = where.GetIdValue(Entity);
                }


                FilterReadRow(myPower.Row.Edit, Entity, where, Id);
            }

            where.RecusionQuery(o =>
            {
                //在 Where in （ select id from RowTable ) ，RowTable 仅需要View的行集权限。
                Power_Select(o, myPower.Row.View);
                return true;
            });


            //判断该Join表是否应用行集权限。
            Context.Dna.Where(o => o.Key.IsIn(SqlKeyword.Join, SqlKeyword.LeftJoin, SqlKeyword.RightJoin))
                .Select(o => o as JoinTableClip)
                .ToList()
                .All(o =>
                {
                    if (o.Table != null)
                    {
                        //仅仅Join了一个表。
                        if (MyContextClip.CreateBySelect(o.Table).GetUsePower().Contains(MyOqlActionEnum.Row))
                        {
                            if (o.OnWhere == null) { o.OnWhere = new WhereClip(); }

                            //在Join Table里，也只需要该Table的 View 的行集权限。
                            FilterReadRow(myPower.Row.View, o.Table, o.OnWhere, null);

                            o.OnWhere.RecusionQuery(q =>
                            {
                                //同样，在 Join 的OnWhere In （ select Id from RowTable ) 中， RowTable 也仅需要View的行集权限。
                                Power_Select(q, myPower.Row.View);
                                return true;
                            });
                        }
                    }
                    return true;
                });


            //if (string.Equals(ConfigurationManager.AppSettings["MyDataPower"], "true", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    var tab = dbr.PowerTable.NoPower()
            //        .FindFirst(o => o.Table == Entity.GetFullName().AsString() | o.Table == Entity.GetName() | o.Table == Entity.GetDbName(), o => o._);
            //    if (tab == null) return retVal;

            //    var cols = dbr.PowerColumn.NoPower()
            //        .SelectWhere(o => o.TableID == tab.Id).ToEntityList(o => o._);
            //    if (cols == null || cols.Count == 0) return retVal;


            //    List<ColumnClip> koColumns = new List<ColumnClip>();
            //    cols.All(o =>
            //    {
            //        if ((MySession.GetMyPower().Create & MyBigInt.CreateByBitPositon((uint)o.Id)) == MyBigInt.Zero)
            //        {
            //            var col = Columns.FirstOrDefault(c => c.Name == o.Column);
            //            if (col.EqualsNull() == false)
            //            {
            //                koColumns.Add(col);
            //            }
            //        }
            //        return true;
            //    });
            //    retVal.KoColumns = koColumns.ToArray();

            //    if (koColumns.Count >= Columns.Count())
            //    {
            //        retVal.IsKoed = true;
            //    }
            //}

            // CrudMaxFunc(MyOqlActionEnum.Update, Entity, Id, Columns, o =>
            //{
            //    if (o.IsKoed)
            //    {
            //        LogTo(MyOqlActionEnum.Update, Entity, InfoEnum.Error,
            //           () => string.Format(@"无权限对表 {0} 进行更新,已忽略全部列({1})",
            //            Entity.GetFullName(), string.Join(",", o.KoColumns.Select(c => c.Name).ToArray()))
            //            );
            //    }
            //    else if (o.KoColumns.Count() > 0)
            //    {
            //        LogTo(MyOqlActionEnum.Update, Entity, InfoEnum.Error,
            //           () => string.Format(@"对表 {0} 进行更新时过滤了列: {1}",
            //            Entity.GetFullName(), string.Join(",", o.KoColumns.Select(c => c.Name).ToArray()))
            //            );
            //    }
            //});


            ////测试能否移除. udi 
            //if (result.KoColumns != null)
            //{
            //    for (int i = 0; i < result.KoColumns.Count(); i++)
            //    {
            //        //model.Model.FirstOrDefault(o => o.Key.GetFullName() == result.KoColumns[i].GetFullName());
            //        model.Model.Remove(result.KoColumns.ElementAt(i));
            //    }
            //}


            //if (0 == model.Model.Count)
            //{
            //    return false; // Context.Dna.Add(new ConstRule().ConstColumn);
            //}

            return true;
        }

        bool dbo_BulkInserting(ContextClipBase Context)
        {
            //var model = Context.Dna.FirstOrDefault(o => o.Key == SqlKeyword.MyOqlSet) as MyOqlSet;
            //dbo.Check(model == null, "批量添加的实体不能为空", Context.CurrentRule);



            // CrudMaxFunc(MyOqlActionEnum.Create, Entity, null, Data.Columns, o =>
            //{
            //    if (o.IsKoed)
            //    {
            //        LogTo(MyOqlActionEnum.Read, Entity, InfoEnum.Error,
            //            () => string.Format(@"无权限对表 {0} 进行批量创建,已忽略全部列({1})",
            //            Entity.GetFullName(), string.Join(",", o.KoColumns.Select(c => c.Name).ToArray()))
            //            );
            //    }
            //    else if (o.KoColumns.Count() > 0)
            //    {
            //        LogTo(MyOqlActionEnum.Read, Entity, InfoEnum.Error,
            //            () => string.Format(@"对表 {0} 进行批量创建时过滤了列: {1}",
            //            Entity.GetFullName(), string.Join(",", o.KoColumns.Select(c => c.Name).ToArray()))
            //            );
            //    }
            //});

            // if (result.IsKoed) return false;

            // //测试能否移除. udi 
            // if (result.KoColumns != null)
            // {
            //     model.Columns = model.Columns.Minus(result.KoColumns).ToArray();
            // }

            // if (0 == model.Columns.Length)
            // {
            //     return false; // Context.Dna.Add(new ConstRule().ConstColumn);
            // }


            return true;
        }

    }
}
