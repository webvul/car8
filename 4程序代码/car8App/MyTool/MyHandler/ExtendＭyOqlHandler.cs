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
using System.Xml.Linq;

namespace MyTool
{
    public class ExtendMyOqlHandler : ICommandHandler
    {
        public string AutoFile { get; set; }
        public string MyFile { get; set; }
        public string ExtendMyOql { get; set; }
        public int CodePage { get; set; }
        /// <summary>
        /// 以哪个文件的名字为准
        /// </summary>
        public string StandardName { get; set; }

        public ExtendMyOqlHandler(CmdArgs arg)
        {
            arg.ToModel(this);

            if (this.CodePage == 0)
            {
                this.CodePage = 65001;
            }
        }

        public string Do()
        {
            if (MyFile.HasValue() == false) return MyFile;
            if (AutoFile.HasValue() == false) return AutoFile;

            var xmlAuto = XDocument.Load(AutoFile);
            var xmlMy = XDocument.Load(MyFile);

            _Extend(xmlAuto, xmlMy);

            xmlMy.Save(ExtendMyOql);

            //            var xmlDefault = new XmlDocument();
            //            xmlDefault.Load(AutoFile);

            //            var xmlDefine = new XmlDocument();
            //            xmlDefine.Load(MyFile);

            //            //先属性。
            //            _Extend(xmlDefault, xmlDefine);
            var reader = xmlMy.CreateReader();
            reader.MoveToContent();
            var str = reader.ReadOuterXml();


            var xml = new XmlDocument();
            xml.LoadXml(@"<?xml version=""1.0""?>
<!-- （自动生成）合成 MyOql.config，By: " + SystemInformation.ComputerName + "." + WindowsGroupUser.GetSingleUserInfo(SystemInformation.UserName, null).FullName + "(" + Environment.UserName + ")" + "   At:" + DateTime.Now.ToString() + @" -->
" + str);
            xml.Save(ExtendMyOql);

            return "         ● 扩展Xml 《" + ExtendMyOql + "》 完成   !";
        }

        private void _Extend(XDocument xmlAuto, XDocument xmlMy)
        {
            //从　xmlMy　遍历　Entity　节点，在　xmlDefault中查找。如果找到，则扩展该节点。
            var xmlAutos = xmlAuto.Descendants("Entity");
            xmlMy.Descendants("Entity").All(defineXmlEntity =>
            {
                //自定义文件可能存在 大小写和空格 的不标准格式。
                var autoXmlEntity = xmlAutos.FirstOrDefault(o => string.Equals(o.Attribute("Name").Value, defineXmlEntity.Attribute("Name").Value.Trim(), StringComparison.CurrentCultureIgnoreCase));
                if (autoXmlEntity != null)
                {
                    defineXmlEntity.Attribute("Name").Value = autoXmlEntity.Attribute("Name").Value;
                    _UseDefault(autoXmlEntity, defineXmlEntity);
                }
                return true;
            });
        }

        private void _UseDefault(XElement xmlDefault, XElement xmlMyDefine)
        {
            if (xmlDefault == null) return;

            if (xmlMyDefine.NodeType == XmlNodeType.XmlDeclaration) { return; }
            else if (xmlMyDefine.NodeType == XmlNodeType.Comment) { return; }

            if (xmlMyDefine.NodeType == XmlNodeType.Text)
            {
                return;
            }
            else
            {
                if (xmlMyDefine.HasAttributes == false)
                {
                    throw new GodError("xmlMyDefine.Attributes　为空。");
                }

                if (xmlDefault.HasAttributes)
                {
                    for (int i = 0; i < xmlDefault.Attributes().Count(); i++)
                    {
                        var atr = xmlDefault.Attributes().ElementAt(i);
                        if (atr.Value.HasValue())
                        {
                            var defineAttr = xmlMyDefine.Attribute(atr.Name);
                            if (defineAttr == null)
                            {
                                var xA = new XAttribute(atr.Name, atr.Value);
                                xmlMyDefine.Add(xA);
                            }
                            else
                            {
                                xmlMyDefine.Attribute(atr.Name).Value = atr.Value;
                            }
                        }
                    }
                }
            }
        }


        private void _Extend(XmlDocument xmlDefault, XmlDocument xmlMyDefine)
        {
            //从　strDefine　遍历　Entity　节点，在　xmlDefault中查找。如果找到，则扩展该节点。

            foreach (XmlNode node in xmlMyDefine.DocumentElement.ChildNodes)
            {
                if (node.Name == "Table")
                {
                    foreach (XmlNode group in node.ChildNodes)
                    {
                        foreach (XmlNode ent in group.ChildNodes)
                        {
                            var path = string.Format(@"/ns:MyOqlCodeGen/ns:Table/ns:Group/ns:Entity[@Name=""{0}""]", ent.Attr("Name"));
                            _UseDefault(xmlDefault.SelectSingleNode(path, xmlMyDefine.GetNameSpaceManager("ns")), ent);
                        }
                    }
                }

                if (node.Name == "View")
                {
                    foreach (XmlNode group in node.ChildNodes)
                    {
                        foreach (XmlNode ent in group.ChildNodes)
                        {
                            var path = string.Format(@"/ns:MyOqlCodeGen/ns:View/ns:Group/ns:Entity[@Name=""{0}""]", ent.Attr("Name"));
                            _UseDefault(xmlDefault.SelectSingleNode(path, xmlMyDefine.GetNameSpaceManager("ns")), ent);
                        }
                    }
                }

                if (node.Name == "Proc")
                {
                    foreach (XmlNode group in node.ChildNodes)
                    {
                        foreach (XmlNode ent in group.ChildNodes)
                        {
                            var path = string.Format(@"/ns:MyOqlCodeGen/ns:Proc/ns:Group/ns:Entity[@Name=""{0}""]", ent.Attr("Name"));
                            _UseDefault(xmlDefault.SelectSingleNode(path, xmlMyDefine.GetNameSpaceManager("ns")), ent);
                        }
                    }
                }

                if (node.Name == "Function")
                {
                    foreach (XmlNode group in node.ChildNodes)
                    {
                        foreach (XmlNode ent in group.ChildNodes)
                        {
                            var path = string.Format(@"/ns:MyOqlCodeGen/ns:Function/ns:Group/ns:Entity[@Name=""{0}""]", ent.Attr("Name"));
                            _UseDefault(xmlDefault.SelectSingleNode(path, xmlMyDefine.GetNameSpaceManager("ns")), ent);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 覆盖到　xmlDefine 上。
        /// </summary>
        /// <param name="xmlDefault"></param>
        /// <param name="xmlMyDefine"></param>
        private void _UseDefault(XmlNode xmlDefault, XmlNode xmlMyDefine)
        {
            if (xmlDefault == null) return;

            if (xmlMyDefine.NodeType == XmlNodeType.XmlDeclaration) { return; }
            else if (xmlMyDefine.NodeType == XmlNodeType.Comment) { return; }

            if (xmlMyDefine.NodeType == XmlNodeType.Text)
            {
                if (xmlMyDefine.InnerXml.HasValue() == false)
                {
                    xmlMyDefine.InnerXml = xmlDefault.InnerXml;
                }
            }
            else
            {
                if (xmlMyDefine.Attributes == null)
                {
                    throw new GodError("xmlMyDefine.Attributes　为空。");
                }

                if (xmlDefault.Attributes != null)
                {
                    for (int i = 0; i < xmlDefault.Attributes.Count; i++)
                    {
                        var atr = xmlDefault.Attributes[i];
                        if (atr.Value.HasValue() && !xmlMyDefine.Attr(atr.Name).HasValue())
                        {
                            xmlMyDefine.Attr(atr.Name, atr.Value);
                        }
                    }
                }
            }
        }
    }
}
