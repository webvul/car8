using System.Collections.Generic;
using MyCmn;
using System.Xml;

namespace MyOql
{
    public partial class SelectClip 
    {
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement || !reader.Read())
            {
                return;
            }
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("Key");
                this.Key = reader.ReadContentAsString().ToEnum<SqlKeyword>();// (this.Key.AsString());
                if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement();

                reader.ReadStartElement("CurrentRule");
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(reader.ReadOuterXml());
                this.CurrentRule = SerializerHelper.XmlToObj<RuleBase>(System.Web.Compilation.BuildManager.GetType(xmlDoc.InnerText, true), xmlDoc.OuterXml);
                if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement();

                reader.ReadStartElement("Dna");
                this.Dna = reader.ReadContentAsString().Base64_UnSerial<List<SqlClipBase>>();// SerializerHelper.XmlToObj<List<SqlClipBase>>(typeof(List<SqlClipBase>), );
                if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Key");
            writer.WriteString(this.Key.AsString());
            writer.WriteEndElement();

            writer.WriteStartElement("CurrentRule");
            writer.WriteRaw(SerializerHelper.ObjToXml(this.CurrentRule, o => o.DocumentElement.OuterXml));
            writer.WriteEndElement();

            writer.WriteStartElement("Dna");
            writer.WriteString(this.Dna.Base64_Serial());
            writer.WriteEndElement();
        }
 
    }
}

