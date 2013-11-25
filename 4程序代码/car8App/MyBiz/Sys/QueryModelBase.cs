using System;
using MyCmn;
using System.Web;
using MyOql;

namespace System
{
    /// <summary>
    /// 查询Model的基类 ,基类属性全部小写 （避免与自定义Model重名）。
    /// </summary>
    public class QueryModelBase
    {
        public QueryModelBase()
        {
            if (HttpContext.Current == null)
            {
                sort = new OrderByClip(string.Empty);
            }
            else
            {
                sort = new OrderByClip(Mvc.Model["FlexiGrid_SortName"].HasValue(o => "#" + o, o => string.Empty));
                sort.IsAsc = string.Equals(Mvc.Model["FlexiGrid_SortOrder"], "asc", StringComparison.CurrentCultureIgnoreCase);

                this.skip = Mvc.Model["FlexiGrid_Skip"].AsInt();
                this.take = Mvc.Model["FlexiGrid_Take"].AsInt(int.MaxValue);

                this.id = Mvc.Model["FlexiGrid_Id"].AsString();
                this.refid = Mvc.Model["FlexiGrid_RefId"].AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                this.cols = Mvc.Model["FlexiGrid_Cols"].AsString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// 排序
        /// </summary>
        public OrderByClip sort;

        /// <summary>
        /// 要取的条数，规范：当 &lt;= 0 时，取全部数据。
        /// </summary>
        public int take;


        /// <summary>
        /// 从1开始的当前页数。
        /// </summary>
        public int pageIndex
        {
            get { return (this.take <= 0 || this.take == int.MaxValue) ? 1 : (skip / take + 1); }
        }

        public SelectOption option
        {
            get
            {
                return (this.take <= 0 || this.take == int.MaxValue) ? SelectOption.NoCount : SelectOption.WithCount;
            }
        }

        /// <summary>
        /// 跳过条数
        /// </summary>
        public int skip;

        /// <summary>
        /// 选中的Id
        /// </summary>
        public string[] refid;

        /// <summary>
        /// Id列。
        /// </summary>
        public string id;

        /// <summary>
        /// 列定义
        /// </summary>
        public string[] cols;
    }
}