using System.Collections.Generic;

namespace MyOql
{
    public class DoingResult
    {
        public DoingResult()
        {
            IsKoed = false;
        }

        public IEnumerable<ColumnClip> KoColumns { get; set; }
        //public string KoIds { get; set; }

        /// <summary>
        /// 是否被全部打死.包括列权限,单条行集权限
        /// </summary>
        public bool IsKoed { get; set; }

        /// <summary>
        /// 行集过滤. 允许的RowID,如果为空，表示没有行集权限。
        /// </summary>
        /// <remarks>对于单条查询,在 Reading 时,直接返回 false 过滤.</remarks>
        public IEnumerable<int> RowFilter { get; set; }
    }
}
