
namespace MyOql
{
    /// <summary>
    /// Having 子句
    /// </summary>
    public class HavingClip:SqlClipBase
    {
        public HavingClip()
        {
            Key = SqlKeyword.Having;
            Where = new WhereClip();
        }

        /// <summary>
        /// Having 的 Where 条件
        /// </summary>
        public WhereClip Where { get; set; }

        public override object Clone()
        {
            var having = new HavingClip();
            having.Where = Where.Clone() as WhereClip;
            return having;
        }
    }
}
