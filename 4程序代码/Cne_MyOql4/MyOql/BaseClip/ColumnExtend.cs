using System;
using System.Collections.Generic;
using System.Linq;

namespace MyOql
{
    /// <summary>
    /// 在列上定义的扩展属性.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    [Serializable]
    public sealed class ColumnExtend : List<KeyValuePair<SqlOperator, object>>
    {
        public object this[SqlOperator key]
        {
            get
            {
                var cou = this.Count(o => o.Key == key);
                if (cou == 0)
                {
                    return null;
                }
                else return this.First(o => o.Key == key).Value;
            }
            set
            {
                if (key != 0)
                {
                    this.Add(new KeyValuePair<SqlOperator, object>(key, value));
                }
            }
        }


        /// <summary>
        /// 能接受 常见的数据类型,Column,WhereClip
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(SqlOperator key, object value)
        {
            this.Add(new KeyValuePair<SqlOperator, object>(key, value));
        }

        public bool ContainsKey(SqlOperator key)
        {
            return this.Count(o => o.Key == key) > 0;
        }

        public IEnumerable<SqlOperator> Keys
        {
            get
            {
                return this.Select(o => o.Key);
            }
        }


        public bool Remove(SqlOperator key)
        {
            bool found = false;
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Key == key)
                {
                    this.RemoveAt(i);
                    i--;
                    found = true;
                }
            }
            return found;
        }

    }
}
