using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Linq;

namespace MyCmn
{
    /// <summary>
    /// 以 OriData 为基础， 把 NewData 的数据批量更新到 OriData 的方法的数据模型。
    /// </summary>
    /// <remarks>
    /// 已经存在的减去交集 = 要删除的
    /// 要插入的减去交集 = 要真正插入的
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public sealed class DbPushModel<T>
    {
        /// <summary>
        /// 公共数据，数据的交集
        /// </summary>
        public IEnumerable<T> Intersect { get; private set; }

        /// <summary>
        /// 需要向 OriData 插入的数据
        /// </summary>
        public IEnumerable<T> ToInsert { get; private set; }

        /// <summary>
        /// 需要向 OriData 删除的数据
        /// </summary>
        public IEnumerable<T> ToDelete { get; private set; }



        public DbPushModel(IEnumerable<T> OriData, IEnumerable<T> NewData)
        {
            this.Intersect = OriData.Intersect(NewData);
            this.ToDelete = OriData.Minus(Intersect);
            this.ToInsert = NewData.Minus(this.Intersect);
        }
    }
}
