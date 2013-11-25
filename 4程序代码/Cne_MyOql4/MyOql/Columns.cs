using System;
using System.Collections.Generic;

namespace MyOql
{

    //[DataContract]
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 该类不记录CRUD事件. 在子类中记录事件
    /// </remarks>
    [Serializable]
    public partial class Columns : List<ColumnClip>
    {
        public Columns(params ColumnClip[] cols)
        {
            if (cols != null)
            {
                this.AddRange(cols);
            }
        }

        public Columns(IEnumerable<ColumnClip> cols)
        {
            if (cols != null)
            {
                this.AddRange(cols);
            }
        }
    }
}
