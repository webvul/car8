using MyCmn;
using System.Xml;
using System;

namespace MyOql
{
    public abstract partial class RuleBase
    {

        /// <summary>
        /// 配置.
        /// </summary>
        protected RuleRuntimeConfig _Config_ { get; set; }

        /// <summary>
        /// 优化了 dbr.Entity 性能.
        /// </summary>
        public RuleBase(string Name)
        {
            this.Name = Name;
        }

        /// <summary>
        /// 只读,不能设置.
        /// </summary>
        /// <returns></returns>
        public RuleRuntimeConfig GetConfig()
        {
            if (_Config_ == null)
            {
                _Config_ = new RuleRuntimeConfig(dbo.MyOqlSect.Entitys.GetConfig(new EntityName { DbName = this.GetDbName(), Name = this.GetName() }));
            }
            return _Config_;
        }

        ///// <summary>
        ///// 是否启用权限控制.
        ///// 默认在配置文件中配置.见 MyOqlConfigSect
        ///// 真正使用的时候，尽量使用 ContextClipBase 的 <see cref="ContextClipBase.GetUsePower">GetUsePower</see> ，它兼容处理了是否跳过权限处理的环节。
        ///// </summary>
        ///// <returns></returns>
        //[Obsolete("请使用 Context.GetUsePower ,除非你知道两个的差别！")]
        //public MyOqlActionEnum GetUsePower()
        //{
        //    return GetConfig().UsePower.ToEnum<MyOqlActionEnum>();
        //}

        ///// <summary>
        ///// 是否启用日志记录.
        ///// 默认在配置文件中配置.见 MyOqlConfigSect
        ///// 真正使用的时候，尽量使用 ContextClipBase 的 <see cref="ContextClipBase.GetUseLog">GetUseLog</see> ，它兼容处理了是否跳过日志处理的环节。
        ///// </summary>
        ///// <returns></returns>
        //[Obsolete("请使用 Context.GetUseLog ,除非你知道两个的差别！")]
        //public MyOqlActionEnum GetUseLog()
        //{
        //    return GetConfig().Log.ToEnum<MyOqlActionEnum>();
        //}

        public int GetCacheTime() { return GetConfig().CacheTime; }

        public int GetCacheSqlTime() { return GetConfig().CacheSqlTime; }


        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// 反序列化 Xml
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            reader.MoveToAttribute("As");
            this.Name = reader.ReadContentAsString();
        }

        /// <summary>
        /// 序列化Xml
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("As", this.GetName());
            writer.WriteString(this.GetType().AssemblyQualifiedName);
        }

    }
}
