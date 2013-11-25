using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCmn;
using System.ComponentModel;

namespace DbEnt
{
    public interface IEnm<T>
    {
        string Name { get; }
        string GetCode();
    }
    /// <summary>
    /// 定义的枚举类。
    /// </summary>
    /// <example>
    /// <code>
    /// public class ddd :IEnm
    /// {
    ///     public static EnmItem&lt;ddd&gt; Park { get { return new EnmItem&lt;ddd&gt;("Bs002", "车位"); } }
    /// }
    /// </code>
    /// </example>
    /// <typeparam name="T"></typeparam>
    //[TypeConverter(typeof(EnmItemConverter))]
    [Serializable]
    public abstract class EnmItem<ClassT, CodeT> : IEnm<ClassT>
    {

        public CodeT Code { get; private set; }

        [NonSerialized]
        private string _Name;

        public string Name { get { return _Name; } }

        public EnmItem(CodeT code, string name)
        {
            this.Code = code;
            this._Name = name;
        }

        public string GetCode() { return Code.AsString(); }

        public static implicit operator string(EnmItem<ClassT, CodeT> enm)
        {
            return enm.AsString();//yy edit null时报错 
        }

        public override string ToString()
        {
            return this.Code.AsString();
        }

        public override bool Equals(object obj)
        {
            return this.AsString() == obj.AsString();
        }
    }

    //public interface IEnum
    //{
    //}
}
