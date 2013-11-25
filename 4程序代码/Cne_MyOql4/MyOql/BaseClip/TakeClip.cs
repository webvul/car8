using System;

namespace MyOql
{
    /// <summary>
    /// 分页中 获取行数.
    /// </summary>
    [Serializable]
    public class TakeClip : SqlClipBase
    {
        public TakeClip()
        {
            this.Key = SqlKeyword.Take;
        }
        //[DataMember]
        public int TakeNumber { get; set; }

        public override object Clone()
        {
            var take = new TakeClip();
            take.TakeNumber = this.TakeNumber;
            return take;
        }
    }
}
