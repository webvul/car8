using System.Collections.Generic;
using System.Linq;

namespace MyOql
{
    /// <summary>
    /// Group by 子句
    /// </summary>
    public class GroupByClip:SqlClipBase
    {
        public GroupByClip()
        {
            this.Key = SqlKeyword.GroupBy;
            this.Groups = new List<ColumnClip>();
        }

        /// <summary>
        /// Group by 列集合
        /// </summary>
        public List<ColumnClip> Groups { get; set; }

        public override object Clone()
        {
            var group = new GroupByClip();
            group.Groups = this.Groups.Select(o => o.Clone() as ColumnClip).ToList();
            return group;
        }
    }
}
