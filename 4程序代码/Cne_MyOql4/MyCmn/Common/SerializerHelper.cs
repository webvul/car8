using System;
using System.Text;
using System.Web.UI.WebControls;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Security.Permissions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.IO.Compression;

namespace MyCmn
{
    /// <summary>
    /// 序列化类。
    /// </summary>
    public static partial class SerializerHelper
    {

        /// <summary>
        /// 获取二进制序列化是否被使用。
        /// </summary>
        public static bool CanBinarySerialize { get; private set; }

        /// <summary>
        /// 静态构造函数仅在设置CanBinarySerialize值中使用一次。
        /// </summary>
        static SerializerHelper()
        {
            SecurityPermission sp = new SecurityPermission(SecurityPermissionFlag.SerializationFormatter);
            try
            {
                sp.Demand();
                CanBinarySerialize = true;
            }
            catch (SecurityException)
            {
                CanBinarySerialize = false;
            }
        }

        /// <summary>
        /// 返回二进制编码后的Base64编码 ， 和 Base64_UnSerial 对应使用。 [★]
        /// </summary>
        /// <param name="ObjWith_Base64_UnSerial"></param>
        /// <returns></returns>
        public static string Base64_Serial<T>(this T ObjWith_Base64_UnSerial)
        {
            if (ObjWith_Base64_UnSerial == null) return string.Empty;

            //兼容性处理 String 类型的。
            if (ObjWith_Base64_UnSerial.GetType().IsSimpleType()) return ObjWith_Base64_UnSerial.AsString();

            Enum enu = ObjWith_Base64_UnSerial as Enum;
            if (enu != null) return ObjWith_Base64_UnSerial.AsInt().AsString();

            // if (ObjWith_Base64_UnSerial is string && ObjWith_Base64_UnSerial.AsString().HasValue() == false) return string.Empty;

            return Convert.ToBase64String(ConvertToBytes(ObjWith_Base64_UnSerial));
        }

        //public static string Base64_Serial(this object ObjWith_Base64_UnSerial,Type TypeToSerial)
        //{
        //    if (ObjWith_Base64_UnSerial == null) return string.Empty;

        //    //兼容性处理 String 类型的。
        //    if (ObjWith_Base64_UnSerial.GetType().IsSimpleType()) return ObjWith_Base64_UnSerial.AsString();

        //    Enum enu = ObjWith_Base64_UnSerial as Enum;
        //    if (enu != null) return ObjWith_Base64_UnSerial.GetInt().AsString();

        //    // if (ObjWith_Base64_UnSerial is string && ObjWith_Base64_UnSerial.AsString().HasValue() == false) return string.Empty;

        //    return Convert.ToBase64String(ConvertToBytes(ObjWith_Base64_UnSerial));
        //}

        /// <summary>
        /// 对 Base64_Serial 函数编码的反序列化.
        /// </summary>
        /// <param name="StringDealdWith_Base64_Serial">经过 Base64_Serial 编码的序列化文本</param>
        /// <returns></returns>
        public static T Base64_UnSerial<T>(this string StringDealdWith_Base64_Serial)
        {
            if (StringDealdWith_Base64_Serial.HasValue() == false) return default(T);

            return (T)Base64_UnSerial(StringDealdWith_Base64_Serial, typeof(T));
        }

        public static object Base64_UnSerial(this string StringDealdWith_Base64_Serial, Type TypeToReturn)
        {
            //兼容性处理 String 类型的。
            if (TypeToReturn.IsSimpleType()) return ValueProc.AsType(TypeToReturn, StringDealdWith_Base64_Serial);

            if (TypeToReturn.IsSubclassOf(typeof(Enum))) return StringDealdWith_Base64_Serial.AsInt();

            //if (string.IsNullOrEmpty(StringDealdWith_Base64_Serial)) return null;

            return ConvertToObject(Convert.FromBase64String(StringDealdWith_Base64_Serial));
        }

        /// <summary>
        /// 将对象转化成二进制的数组。和 ConvertToObject 对应使用。 [★] 
        /// </summary>
        /// <param name="objectToConvert">用于转化的对象。</param>
        /// <returns>返回转化后的数组，如果CanBinarySerialize为false则返回null。</returns>
        public static byte[] ConvertToBytes(object objectToConvert)
        {
            if (CanBinarySerialize)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                using (MemoryStream ms = new MemoryStream())
                {
                    binaryFormatter.Serialize(ms, objectToConvert);
                    ms.Seek(0, SeekOrigin.Begin);
                    return Compress(ms);
                    //using (GZipStream gz = new GZipStream(ms, CompressionMode.Compress))
                    //{
                    //    binaryFormatter.Serialize(gz, objectToConvert);
                    //    return ms.ToArray();
                    //}
                }
            }
            return null;
        }

        /// <summary>
        /// 将一个二进制的数组转化为对象，必须通过类型转化自己想得到的相应对象。如果数组为空则返回空。 
        /// 和 ConvertToBytes 对应使用。  [★] .
        /// </summary>
        /// <param name="byteArray">用于转化的二进制数组。</param>
        /// <returns>返回转化后的对象实例，如果数组为空，则返回空对象。</returns>
        public static object ConvertToObject(byte[] byteArray)
        {
            object convertedObject = null;
            if (CanBinarySerialize && byteArray != null && byteArray.Length > 0)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                using (MemoryStream ms = new MemoryStream(byteArray))
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    var deBytes = DeCompress(ms);
                    using (MemoryStream msOut = new MemoryStream(deBytes))
                    {
                        msOut.Seek(0, SeekOrigin.Begin);
                        convertedObject = binaryFormatter.Deserialize(msOut);
                        return convertedObject;
                    }

                }
                //using (MemoryStream ms = new MemoryStream(Decompress(byteArray)))
                //{
                //ms.Position = 0;
                //ms.Write(, 0, byteArray.Length);

                //using (MemoryStream msOut = new MemoryStream())
                //{
                //    using (GZipStream gz = new GZipStream(ms, CompressionMode.Decompress))
                //    {
                //        Pump(gz, msOut);
                //        gz.Close();
                //        //BinaryReader reader = new BinaryReader(gz);
                //        //byte[] buffer = null;
                //        //int offset = 0;
                //        //while (true)
                //        //{
                //        //    buffer = new byte[1024];
                //        //    int bytesRead = reader.Read(buffer, offset, 1024);

                //        //    if (bytesRead == 0)
                //        //        break;


                //        //    msOut.Write(buffer, offset, bytesRead);
                //        //    offset += bytesRead;
                //        //}
                //    }

                //convertedObject = binaryFormatter.Deserialize(ms);


                //binaryFormatter.Serialize(gz, objectToConvert);
                //}


                //if (byteArray.Length > 4)
                //    convertedObject = binaryFormatter.Deserialize(ms);


                //}
            }
            return convertedObject;
        }


        private static byte[] Compress(Stream stream)
        {
            MemoryStream msOut = new MemoryStream();
            try
            {
                GZipStream gzip = new GZipStream(msOut, CompressionMode.Compress);
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, 4096)) > 0)
                {
                    gzip.Write(buffer, 0, bytesRead);
                }
                // 这个地方一定要Close了,gzip才会执行最后的压缩过程,不是很知道原因
                // 是不是一定要close,Flush不知道可不可以
                gzip.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Compress failed!", ex);
            }
            // 因为上面的close会把相关流都close掉,这里要返回一个新流
            return msOut.ToArray();
        }

        private static byte[] DeCompress(Stream stream)
        {
            MemoryStream msOut = new MemoryStream();
            try
            {
                GZipStream gzip = new GZipStream(stream, CompressionMode.Decompress);
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = gzip.Read(buffer, 0, 4096)) > 0)
                {
                    msOut.Write(buffer, 0, bytesRead);
                }
                // 这里为了安全起见,也把zip流关闭,再返回新流
                gzip.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("DeCompress failed!", ex);
            }

            // 如果这里返回原流的话,一定要把流seek到开始点
            return msOut.ToArray();
        }
         


        public static string ObjToXml<T>(T obj, Func<XmlDocument, string> Func)
        {
            if ((object)obj == null) return "";
            XmlSerializer serialize = new XmlSerializer(obj.GetType());
            StringBuilder sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);

            serialize.Serialize(tw, obj);

            string strRet = sb.ToString();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(strRet);

            if (Func != null) strRet = Func(xmlDoc);
            return strRet;
        }

        public static T XmlToObj<T>(Type type, string Xml)
        {
            if (Xml.HasValue() == false) return default(T);
            XmlSerializer serialize = new XmlSerializer(type);
            TextReader tr = new StringReader(Xml);

            return (T)serialize.Deserialize(tr);
        } 
          

        
        /// <summary>
        /// 通过序列化和反序列化进行深度克隆。
        /// 如果对象继承了 ICloneable , 则使用 ICloneable.Clone.
        /// 值类型,string 类型不需要深度克隆.直接返回.
        /// </summary>
        /// <remarks>
        /// 如果对象继承了 ICloneable , 则使用 ICloneable.Clone. 所以  ICloneable.Clone 实现中,不能使用 DeepClone 方法.
        /// 如果在 ICloneable.Clone 实现中使用 DeepClone , 会造成死循环.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T Source)
        {
            var type = typeof(T);
            if (type.FullName == "System.Object")
            {
                type = Source.GetType();
            }

            if (type.IsValueType) return Source;
            if (type.FullName == "Systemm.String") return Source;


            var clone = Source as ICloneable;
            if (object.Equals(clone, null) != false) return (T)clone.Clone();

            return MustDeepClone(Source);
        }

        /// <summary>
        /// 继承自 IClone 接口的对象要实现 深克隆,不能使用 DeepClone ,而需要改用 MustDeepClone .否则,会出现死循环.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static T MustDeepClone<T>(this T Source)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Context = new System.Runtime.Serialization.StreamingContext(System.Runtime.Serialization.StreamingContextStates.Clone);
                formatter.Serialize(stream, Source);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }



        /// <summary>
        /// IEntity 自定义克隆
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static T CloneIEntity<T>(this T Entity)
            where T : IEntity, new()
        {
            var retVal = new T();
            foreach (var prop in retVal.GetProperties())
            {
                retVal.SetPropertyValue(prop, Entity.GetPropertyValue(prop));
            }
            return retVal;
        }
    }
}