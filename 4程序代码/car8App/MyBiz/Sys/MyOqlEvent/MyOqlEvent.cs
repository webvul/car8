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
using System.Diagnostics;
using System.Threading;


namespace MyBiz.Sys
{
    public partial class MyOqlEvent : IDboEvent
    {
        private static MyOqlEvent _MyOqlEvent = new MyOqlEvent();

        private static Stopwatch sw = Stopwatch.StartNew();
        private const long CACHETIME = 3000;
        private static bool loadingCacheData = false;

        public static MyOqlEvent GetInstance()
        {
            return _MyOqlEvent;
        }

        public override IDbr Idbr { get; set; }

        //protected Func<RuleBase, string, XmlDictionary<string, MyBigInt>, DoingResult> RowPowerFunc { get; set; }
        //protected MyCmn.Func<MyOqlActionEnum, ContextClipBase, string, IEnumerable<ColumnClip>, Action<DoingResult>, DoingResult> CrudMaxFunc { get; set; }

        private MyOqlEvent()
        {
            Idbr = new dbr();
            ////返回值仅涉及 列 和  IsKoed .
            //RowPowerFunc = (Entity, Id, RowPower) =>
            //{
            //    DoingResult retVal = new DoingResult();

            //    // 如果有行集权限， 则用行集权限过滤一下。
            //    if (Entity.GetUsePower().Contains(MyOqlActionEnum.Row) == false) return retVal;
            //    if (RowPower.ContainsKey(Entity.GetFullName().DbName) == false) return new DoingResult() { IsKoed = true };

            //    //单条记录检查.仅简单过滤: 数字唯一列 与 系统ID 列是一致的情况.
            //    if (Id.HasValue() && Entity.GetIdKey().NameEquals(Entity.GetUniqueNumberKey()))
            //    {
            //        if (RowPower[Entity.GetFullName().DbName].ToPositions().Contains(Id.AsInt()))
            //        {
            //            return retVal;
            //        }
            //        else return new DoingResult() { IsKoed = true };
            //    }
            //    else
            //    {
            //        retVal.RowFilter = RowPower.GetOrDefault(Entity.GetFullName().DbName).ToPositions().ToArray();

            //        if (retVal.RowFilter == null || (retVal.RowFilter.Count() == 0))
            //        {
            //            retVal.IsKoed = true;
            //        }

            //        return retVal;
            //    }
            //};
            //CrudMaxFunc = (Action, Context, Id, Columns, actedFunc) =>
            //{
            //    var Entity = Context.CurrentRule;
            //    DoingResult retVal = new DoingResult();
            //    var myPower = MySession.GetMyPower();
            //    if (myPower == null) return new DoingResult() { IsKoed = true };

            //    if (Action == MyOqlActionEnum.Read && Entity.GetUsePower().Contains(MyOqlActionEnum.Row))
            //    {
            //    }
            //    else if ((Entity.GetUsePower() & Action) == 0)
            //    {
            //        //如果没有启用 Action 权限.则直接返回默认结果
            //        return retVal;
            //    }

            //    switch (Action)
            //    {
            //        case MyOqlActionEnum.All:
            //            break;
            //        case MyOqlActionEnum.Create:
            //            if (myPower.Create.Max) return retVal;
            //            break;
            //        case MyOqlActionEnum.Delete:
            //            if (myPower.Delete.Max) return retVal;
            //            break;
            //        case MyOqlActionEnum.Read:
            //            if (myPower.Read.Max) return retVal;
            //            break;
            //        case MyOqlActionEnum.Row:
            //            break;
            //        case MyOqlActionEnum.Update:
            //            if (myPower.Update.Max) return retVal;

            //            break;
            //        default:
            //            break;
            //    }

            //    if (Action == MyOqlActionEnum.Read)
            //    {
            //        // 如果有行集权限， 则用行集权限过滤一下。
            //        var rowFilter = RowPowerFunc(Entity, Id, myPower.Row.View);
            //        if (rowFilter.IsKoed) return rowFilter;
            //        else
            //        {
            //            retVal.RowFilter = rowFilter.RowFilter;
            //        }
            //    }
            //    else if (Action == MyOqlActionEnum.Update)
            //    {
            //        // 如果有行集权限， 则用行集权限过滤一下。
            //        var rowFilter = RowPowerFunc(Entity, Id, myPower.Row.Edit);
            //        if (rowFilter.IsKoed) return rowFilter;
            //        else
            //        {
            //            retVal.RowFilter = rowFilter.RowFilter;
            //        }
            //    }
            //    //if (string.Equals(ConfigurationManager.AppSettings["MyDataPower"], "true", StringComparison.CurrentCultureIgnoreCase))
            //    //{
            //    //    var tab = dbr.PowerTable.NoPower()
            //    //        .FindFirst(o => o.Table == Entity.GetFullName().AsString() | o.Table == Entity.GetName() | o.Table == Entity.GetDbName(), o => o._);
            //    //    if (tab == null) return retVal;

            //    //    var cols = dbr.PowerColumn.NoPower()
            //    //        .SelectWhere(o => o.TableID == tab.Id).ToEntityList(o => o._);
            //    //    if (cols == null || cols.Count == 0) return retVal;


            //    //    List<ColumnClip> koColumns = new List<ColumnClip>();
            //    //    cols.All(o =>
            //    //    {
            //    //        if ((MySession.GetMyPower().Create & MyBigInt.CreateByBitPositon((uint)o.Id)) == MyBigInt.Zero)
            //    //        {
            //    //            var col = Columns.FirstOrDefault(c => c.Name == o.Column);
            //    //            if (col.EqualsNull() == false)
            //    //            {
            //    //                koColumns.Add(col);
            //    //            }
            //    //        }
            //    //        return true;
            //    //    });
            //    //    retVal.KoColumns = koColumns.ToArray();

            //    //    if (koColumns.Count >= Columns.Count())
            //    //    {
            //    //        retVal.IsKoed = true;
            //    //    }
            //    //}
            //    if (actedFunc != null) actedFunc(retVal);
            //    return retVal;
            //};


            this.RegisteMyOqlEvent();

        }

        public void RegisteMyOqlEvent()
        {
            this.DecrypteEvent += (arg) =>
            {
                return arg;
                //return MyCmn.Security.DecrypteString(arg);
            };

            this.Creating += new Func<ContextClipBase, bool>(dbo_Creating);
            this.BulkInserting += new Func<ContextClipBase, bool>(dbo_BulkInserting);

            this.BulkUpdating += new Func<ContextClipBase, bool>(dbo_BulkUpdating);

            this.Reading += new Func<ContextClipBase, bool>(dbo_Reading);

            this.Updating += new Func<ContextClipBase, bool>(dbo_Updating);

            this.Deleting += new Func<ContextClipBase, bool>(dbo_Deleting);

            //this.GenerateSqled += new Action<DatabaseType, MyCommand>(dbo_GenerateSqled);
            this.Procing += new Func<ContextClipBase, bool>(dbo_Procing);

            this.Executing += new Func<ContextClipBase, bool>(MyOqlEvent_Executing);
        }


        public override void LogTo(MyOqlActionEnum Action, ContextClipBase Context, InfoEnum LogType, Func<string> LogFunc)
        {
            //仅仅为了调试.
            base.LogTo(Action, Context, LogType, LogFunc);
        }


        /// <summary>
        /// 如果上了接口, 请添加获取 CacheTable的代码.
        /// </summary>
        /// <returns></returns>
        public override string[] GetChangedTable()
        {
            if (loadingCacheData) return null;
            loadingCacheData = true;

            if (sw.ElapsedMilliseconds > CACHETIME)
            {
                sw.Restart();
                using (var myScop = new MyOqlConfigScope(ReConfigEnum.SkipCache | ReConfigEnum.SkipLog | ReConfigEnum.SkipPower))
                {
                    return dbr.CacheTable.Select(o => o.Table).ToEntityList("").ToArray();
                }
            }

            loadingCacheData = false;

            return null;
            //如果上了接口, 请添加获取 CacheTable的代码.
            //return dbr.CacheTable.NoCache().NoLog().NoPower().Select(o => o.Table).ToEntityList("").ToArray();
        }

        /// <summary>
        /// 如果上了接口,则清除该缓存表.
        /// </summary>
        /// <param name="Table"></param>
        public override void ClearedCacheTable(string Table)
        {
            using (var myScop = new MyOqlConfigScope(ReConfigEnum.SkipCache | ReConfigEnum.SkipLog | ReConfigEnum.SkipPower))
            {
                dbr.CacheTable.Delete(o => o.Table == Table).Execute();
            }

            //如果上了接口,则清除该缓存表.
            //dbr.CacheTable.NoPower().NoLog().Delete(o => o.Table == Table).Execute();
        }

        public override bool OnReading(ContextClipBase Context)
        {
            var retVal = base.OnReading(Context);

            //#if DEBUG
            //            if (retVal)
            //            {
            //                Context.UserData = Stopwatch.StartNew();
            //            }
            //#endif
            return retVal;
        }

        public override void OnReaded(ContextClipBase Context)
        {
            //#if DEBUG
            //            var sw = Context.UserData as Stopwatch;
            //            if (sw != null)
            //            {
            //                var time = sw.ElapsedMilliseconds;

            //                StringLinker msg = Context.RunCommand.Command.CommandText + Environment.NewLine;

            //                foreach (DbParameter item in Context.RunCommand.Command.Parameters)
            //                {
            //                    msg += "   " + item.ParameterName + ":" + item.Value.AsString();
            //                }


            //                try
            //                {
            //                    //大于 500 毫秒的SQL
            //                    if (time > 500)
            //                    {
            //                        new Action<HttpContext>((con) =>
            //                            {
            //                                HttpContext.Current = con;
            //                                dbr.Log
            //                                   .Insert(o => o.AddTime == DateTime.Now & o.Msg == msg & o.Type == InfoEnum.Sql & o.UserName == MySession.OnlyGet(MySessionKey.UserName)
            //                                       & o.Url == HttpContext.Current.Request.Url.ToString() & o.Value == time
            //                                       & o.Ip == HttpContext.Current.Request.UserHostAddress)
            //                                    .SkipLog()
            //                                    .SkipPower()
            //                                   .Execute();
            //                            }).BeginInvoke(HttpContext.Current, null, null);

            //                    }
            //                }
            //                catch (Exception e)
            //                {

            //                }

            //                Context.UserData = null;
            //            }
            //#endif
            base.OnReaded(Context);
        }
    }
}
