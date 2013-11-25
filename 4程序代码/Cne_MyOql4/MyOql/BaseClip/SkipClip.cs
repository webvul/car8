using System;

namespace MyOql
{
    /// <summary>
    /// 分页的 跳过行数.
    /// </summary>
    [Serializable]
    public class SkipClip :SqlClipBase 
    {
        public SkipClip()
        {
            this.Key = SqlKeyword.Skip;
        }
        //[DataMember]
        public int SkipNumber { get; set; }

        public override object Clone()
        {
            var skip = new SkipClip();
            skip.SkipNumber = this.SkipNumber;
            return skip;
        }
    }
}
