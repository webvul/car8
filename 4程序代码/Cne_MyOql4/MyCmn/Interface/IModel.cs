using System;

namespace MyCmn
{
    /// <summary>
    /// 仅仅做为一个标识,表示,对象是一个模型.
    /// </summary>
    /// <remarks>
    /// 目前, XmlDictionary ,IEntity ,MyOql.WhereClip ,MyOqlSet 都是一个模型.
    /// </remarks>
    public interface IModel : ICloneable
    {
        /// <summary>
        /// 获取属性列表.
        /// </summary>
        /// <returns></returns>
        string[] GetProperties();

    }
}