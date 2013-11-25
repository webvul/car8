using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCmn;
using System.Configuration;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using MyOql;
using System.Windows.Forms;
using System.Xml;

namespace MyTool
{
    public static class xml_Helper
    {
        public static string Attr(this XmlNode xml, string key, string value = null)
        {
            var sect = key.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var prefix = string.Empty;
            if (sect.Length == 2)
            {
                prefix = sect[0];
                key = sect[1];
            }

            if (xml.Attributes == null) return null;

            var attr = xml.Attributes.GetNamedItem(key) as XmlAttribute;

            if (value != null)
            {
                if (attr == null)
                {
                    attr = xml.OwnerDocument.CreateAttribute(prefix, key, xml.NamespaceURI);
                    attr.Value = value;
                    xml.Attributes.Append(attr);
                }
                else
                {
                    attr.Value = value;
                }

                return value;
            }


            if (attr == null) return null;
            return attr.Value;
        }

        public static XmlNamespaceManager GetNameSpaceManager(this XmlNode node, string NameSpace)
        {
            XmlDocument xml = null;
            if (node.NodeType == XmlNodeType.Document) xml = node as XmlDocument;
            else xml = node.OwnerDocument;

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xml.NameTable);

            foreach (XmlAttribute item in xml.DocumentElement.Attributes)
            {
                if (item.Prefix.HasValue())
                {
                    nsmgr.AddNamespace(item.LocalName, item.Value);
                }
            }


            if (NameSpace.HasValue())
            {
                nsmgr.AddNamespace(NameSpace, xml.DocumentElement.NamespaceURI);
            }
            return nsmgr;
        }

    }

    public class ExtendXmlHandler : ICommandHandler
    {
        public string[] ListFiles { get; set; }
        public string ExtendXml { get; set; }
        public int CodePage { get; set; }

        public ExtendXmlHandler(CmdArgs arg)
        {
            arg.ToModel(this);

            if (this.CodePage == 0)
            {
                this.CodePage = 65001;
            }
        }

        public string Do()
        {
            Encoding encode = Encoding.GetEncoding(CodePage);
            foreach (var item in ListFiles)
            {
                GodError.Check(File.Exists(item) == false, () =>
                {
                    Console.WriteLine("     找不到文件：" + item + "!!!");
                    return "找不到文件：" + item;
                }
               );
            }

            var str = "";

            for (var i = ListFiles.Count() - 1; i >= 0; i--)
            {
                str = Extend(File.ReadAllText(ListFiles[i]), str);
            }

            var xml = new XmlDocument();
            xml.LoadXml(@"<?xml version=""1.0""?>
<!-- （自动生成）合并Xml，By: " + SystemInformation.ComputerName + "." + WindowsGroupUser.GetSingleUserInfo(SystemInformation.UserName, null).FullName + "(" + Environment.UserName + ")" + "   At:" + DateTime.Now.ToString() + @" -->
" + str);
            xml.Save(ExtendXml);


            return "         ● 扩展Xml 《" + ExtendXml + "》 完成   !";
        }

        private string Extend(string strDefault, string strDefine)
        {
            if (strDefine.HasValue() == false) return strDefault;
            if (strDefault.HasValue() == false) return strDefine;

            var xmlDefault = new XmlDocument();
            xmlDefault.LoadXml(strDefault);

            var xmlDefine = new XmlDocument();
            xmlDefine.LoadXml(strDefine);

            //先属性。
            _Extend(xmlDefault, xmlDefine);

            return xmlDefault.DocumentElement.OuterXml;
        }


        /// <summary>
        /// 覆盖　xmlDefault 
        /// </summary>
        /// <param name="xmlDefault"></param>
        /// <param name="xmlDefine"></param>
        private void _Extend(XmlNode xmlDefault, XmlNode xmlDefine)
        {
            if (xmlDefine == null) return;
            if (xmlDefine.NodeType == XmlNodeType.XmlDeclaration) { return; }
            else if (xmlDefine.NodeType == XmlNodeType.Comment) { return; }

            if (xmlDefine.NodeType == XmlNodeType.Text)
            {
                if (xmlDefine.InnerXml.HasValue())
                {
                    xmlDefault.InnerXml = xmlDefine.InnerXml;
                }
            }
            else
            {
                if (xmlDefine.Attributes != null)
                {
                    for (int i = 0; i < xmlDefine.Attributes.Count; i++)
                    {
                        var atr = xmlDefine.Attributes[i];
                        if (atr.Value.HasValue())
                        {
                            xmlDefault.Attr(atr.Name, atr.Value);
                        }
                    }
                }

                //child
                for (var i = 0; i < xmlDefine.ChildNodes.Count; i++)
                {
                    var cl = xmlDefine.ChildNodes[i];
                    if (cl.NodeType == XmlNodeType.XmlDeclaration) { continue; }
                    else if (cl.NodeType == XmlNodeType.Comment) { continue; }


                    var key = cl.Attr("Name").HasValue() ? "Name" :
                            cl.Attr("Key").HasValue() ? "Key" :
                            cl.Attr("name").HasValue() ? "name" :
                            cl.Attr("key").HasValue() ? "key" : "";

                    if (key.HasValue())
                    {
                        var nodes = xmlDefault.SelectNodes(string.Format(@"ns:{0}[@{1}=""{2}""]", cl.Name, key, cl.Attr(key)), xmlDefault.OwnerDocument.GetNameSpaceManager("ns"));

                        if (nodes.Count > 1) throw new GodError("找到多个节点。请检查：" + cl.OuterXml);
                        if (nodes.Count == 1)
                        {
                            _Extend(nodes[0], cl);
                        }
                        else
                        {
                            if (cl.OuterXml.HasValue())
                            {
                                xmlDefault.InnerXml += cl.OuterXml;
                            }
                        }
                    }
                    else if (cl.NodeType == XmlNodeType.Text)
                    {
                        if (cl.OuterXml.HasValue())
                        {
                            xmlDefault.InnerXml += cl.OuterXml;
                        }
                    }
                    else
                    {
                        //如果没有定义 Name , Key ，则该子节点 nodename 的 Key 为空的只允许有一个。
                        var nodes = xmlDefault.SelectNodes(string.Format(@"ns:{0}", cl.Name), xmlDefault.GetNameSpaceManager("ns"));

                        XmlNode emptyNode = null;
                        for (int k = 0; k < nodes.Count; k++)
                        {
                            var n = nodes[k];

                            if (n.Attr("Name").HasValue() || n.Attr("name").HasValue() || n.Attr("Key").HasValue() || n.Attr("key").HasValue())
                            {
                                continue;
                            }
                            else
                            {
                                if (emptyNode != null)
                                {
                                    throw new GodError("找到多个节点。请检查：" + cl.OuterXml);
                                }

                                emptyNode = n;
                            }
                        }

                        if (emptyNode != null)
                        {
                            _Extend(emptyNode, cl);
                        }
                        else
                        {
                            if (cl.OuterXml.HasValue())
                            {
                                xmlDefault.InnerXml += cl.OuterXml;
                            }
                        }

                    }
                }
            }
        }
    }
}
