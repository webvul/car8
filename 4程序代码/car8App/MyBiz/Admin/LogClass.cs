using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyOql;
using MyCmn;
using DbEnt;

namespace MyBiz.Admin
{
    public class LogClass : IDisposable
    {
        /// <summary>
        /// 用户存储错误信息
        /// </summary>
        ///  <remarks>创建人员(日期):★ben★(101009 16:49)</remarks>
        public string ErrorMsg { set; get; }
        /// <summary>
        /// 获取Log表名称
        /// </summary>
        /// <returns></returns>
        ///  <remarks>创建人员(日期):★ben★(101009 17:06)</remarks>
        public string GetTableName()
        {
            try
            {
                return dbr.Log.GetName();
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取日志信息，模糊查询
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="beginTime">查询开始时间</param>
        /// <param name="endTime">查询结束时间</param>
        /// <param name="type">查询类型</param>
        /// <param name="skip">跳过的行数</param>
        /// <param name="take">获取的行数</param>
        /// <param name="totalCont">记录集中所有行数</param>
        ///  <remarks>创建人员(日期):★ben★(101009 16:55)</remarks>
        public List<RowData> GetLogs(
            string userName, string beginTime, string endTime,
            InfoEnum type, OrderByClip order, int skip, int take, out int totalCont)
        {
            totalCont = 0;
            try
            {
                WhereClip where = new WhereClip();

                if (userName.HasValue())
                {
                    where &= dbr.Log.UserName.Like("%" + userName + "%");
                }
                if (beginTime.HasValue() || endTime.HasValue())
                {
                    if (beginTime.HasValue() && endTime.HasValue())
                    {
                        where &= dbr.Log.AddTime.Between(string.Format("{0} 00:00:00", beginTime), string.Format("{0} 23:59:59", endTime));
                    }
                    if (beginTime.HasValue() && !endTime.HasValue())
                    {
                        where &= dbr.Log.AddTime.Between(string.Format("{0} 00:00:00", beginTime), string.Format("9999/12/23 23:59:59", endTime));
                    }
                    if (!beginTime.HasValue() && endTime.HasValue())
                    {
                        where &= dbr.Log.AddTime.Between(string.Format("1000/01/01 00:00:00"), string.Format("{0} 23:59:59", endTime));
                    }
                }
                if (type != 0)
                {
                    where &= dbr.Log.Type.Like("%" + ((int)type).ToString() + "%");
                }

                totalCont = dbr.Log.FindScalar(o => o.Count(), o => { return where; }).AsInt();
               
                var d = dbr.Log
                    .Select()
                    .Skip(skip)
                    .Take(take)
                    .Where(where)
                    .OrderBy(order)
                    .SkipLog()
                    .ToDictList();

                return d;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取一个日志信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ///  <remarks>创建人员(日期):★ben★(101009 16:47)</remarks>
        public bool Insert(InfoEnum LogType, string Msg, string User)
        {
            try
            {
                //一定要用 NoLog ,以防止在记录日志时出错而循环记录日志,Udi. 2010-10-14
                if (dbr.Log.Insert(o => o.AddTime == DateTime.Now & o.Msg == Msg & o.Type == LogType.AsString() & o.UserName == User).SkipLog().Execute() == 1)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                return false;
            }
            return false;
        }

        public void Dispose()
        {

        }
    }
}
