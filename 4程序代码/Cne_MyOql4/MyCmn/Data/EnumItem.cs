using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Runtime.Serialization;
using System.Linq;
using System.Collections;
using Newtonsoft.Json;

namespace MyCmn
{
    public interface IEnm
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
    /// <typeparam name="ClassT"></typeparam>
    /// <typeparam name="CodeT"></typeparam>
    [Serializable]
    public abstract class EnmItem<ClassT, CodeT> : IEnm
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

        public override int GetHashCode()
        {
            return ((0M + typeof(ClassT).GetHashCode() + Code.GetHashCode() + Name.GetHashCode()) % int.MaxValue).AsInt();
        }
    }

    public class MyEnmItemJsonNetConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var ienm = objectType.GetInterface("MyCmn.IEnm");
            return (ienm != null);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var ie = reader.Value as IEnm;
            if (ie != null)
            {
                return ValueProc.AsType(objectType.BaseType.GetGenericArguments()[1], ie.GetCode());
            }
            else return reader.Value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var ie = value as IEnm;
            if (ie != null)
            {
                writer.WriteValue(ValueProc.AsType(value.GetType().BaseType.GetGenericArguments()[1], ie.GetCode()));
            }
            else
            {
                writer.WriteValue(value);
            }
        }
    }


}
