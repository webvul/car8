using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;

namespace MyOql
{
    public partial class SelectClip<T>
    {
        /// <summary>
        /// 连接select 后的选择列。
        /// </summary>
        /// <param name="FuncSelect"></param>
        /// <returns></returns>
        public SelectClip<T> _(Func<T, ColumnClip> FuncSelect)
        {
            this.Dna.Add(FuncSelect((T)(object)this.CurrentRule));
            return this;
        }



        /// <summary>
        /// 生成 order by 子句.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public SelectClip<T> OrderBy(Func<T, OrderByClip> func)
        {
            this.OrderBy(func.Invoke((T)(object)this.CurrentRule));
            return this;
        }

        /// <summary>
        /// 去重查询.
        /// </summary>
        /// <returns></returns>
        public SelectClip<T> Distinct ( )  
        {
            this.Dna.Add(new MySqlClipBase(SqlKeyword.Distinct));
            return this;
        }
    }
}
