using MyCmn;

namespace MyOql
{
    public class EntityName
    {
        public string Owner { get; set; }

        /// <summary>
        /// 表示未转义的在数据库中保存的名字。
        /// </summary>
        public string DbName { get; set; }

        private string _Name { get; set; }

        /// <summary>
        /// 表示程序转义的名字.
        /// </summary>
        public string Name
        {
            get { if (_Name.HasValue()) return _Name; else return DbName; }
            set { _Name = value; }
        }


        /// <summary>
        /// 返回 Owner + "." + DbName 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (DbName.HasValue() == false) return string.Empty;

            if (Owner.HasValue())
            {
                return Owner.AsString() + "." + DbName.AsString();
            }
            else
            {
                return DbName.AsString();
            }
        }

        public MyOqlConfigSect.EntityCollection.GroupCollection.EntityElement GetConfig()
        {
            return dbo.MyOqlSect.Entitys.GetConfig(this);
        }


        /// <summary>
        /// 取实体全名.实体严格区分大小写.
        /// </summary>
        /// <remarks>
        /// 外部系统应该调用 MyOql EntityName 类.来破坏缓存. 
        /// 或者自行构造 EntityName 类. 其中 Owner应该是动态维护的.
        /// </remarks>
        /// <param name="IdValue"></param>
        /// <returns>返回内容如下:"MyOql|" + this.AsString() + "|" + IdValue.AsString()</returns>
        public string GetCacheKey(string IdValue)
        {
            return "MyOql|" + this.DbName + "|" + IdValue.AsString().ToUpperInvariant();
        }


        ///// <summary>
        ///// 取Box缓存项.
        ///// </summary>
        ///// <returns></returns>
        //public string GetCacheBoxKey()
        //{
        //    return "MyOql|Box|" + this.AsString();
        //}
    }
}
