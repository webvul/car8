using System.Xml;
using System;
using MyOql;
using MyCmn;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using DbEnt;
namespace MyBiz
{
    /// <summary>
    /// Excel 2003 Xml 电子表格操作类
    /// </summary>
    /// <remarks>
    /// 设置样式时可以在 StyleFunc 中给 node 设置 。如下：
    /// (rowIndex,colIndex,cell)=> {
    ///     if ( rowIndex > 0 ){
    ///         if ( rowIndex == 5 && colIndex == 3) {
    ///             XmlExcel.Attr(cell,"ss:MergeAcross","1") ;  //水平合并单元格。
    ///             XmlExcel.Attr(cell,"ss:MergeDown","1") ;    //垂直合并单元格。还要在相当的单元格上增加 ss:Index  属性，其值是该列的vb索引
    ///         }
    ///     }
    /// } ;
    /// </remarks>
    public static class XmlExcel
    {
        #region 全局变量
        private static XmlDictionary<string, int> ColMaxLengths = new XmlDictionary<string, int>();
        private static int COLMAXWIDTH = 200;
        private static System.Text.Encoding ExcelCoding = System.Text.Encoding.GetEncoding("GB2312");

        #endregion

        private static XmlAttribute Attr(this XmlNode xml, string key, string value = null)
        {
            var sect = key.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var prefix = string.Empty;
            if (sect.Length == 2)
            {
                prefix = sect[0];
                key = sect[1];
            }

            var attr = xml.Attributes.GetNamedItem(key) as XmlAttribute;
            if (attr == null)
            {
                attr = xml.OwnerDocument.CreateAttribute(prefix, key, xml.NamespaceURI);
            }

            if (value != null)
            {
                attr.Value = value;
                xml.Attributes.Append(attr);
            }

            return attr;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="xmlDoc">表示excel2003电子表格样式的Xml</param>
        /// <param name="sheetName">数据导出目标Sheet名</param>
        /// <param name="cols">要导出的列别名</param>
        /// <param name="StyleFunc">样式回调，返回null表示不使用自定义样式，参数为：行索引，列索引，单元格</param>
        /// <param name="TypeFunc">类型回调，返回(DbType)0表示不使用自定义类型，参数为：行索引，列，值。</param>
        /// <returns></returns>
        public static string ToExcel(this MyOqlSet dataSource, XmlDocument xmlDoc, string sheetName, string[] cols, Action<int, int, XmlNode> StyleFunc, Func<int, ColumnClip, object, DbType> TypeFunc, params int[] hideColumnIndex)
        {
            CheckDefaultStyle(xmlDoc, sheetName);

            var tableRoot = xmlDoc.SelectSingleNode("/ns:Workbook/ns:Worksheet[@ss:Name='" + sheetName + "']/ns:Table", xmlDoc.GetNameSpaceManager("ns"));

            TranslateXml(tableRoot, dataSource, cols, StyleFunc, TypeFunc);


            //设置列数和行数
            tableRoot.Attr("ExpandedColumnCount", Math.Max(tableRoot.Attr("ExpandedColumnCount").Value.AsInt(), dataSource.Columns.Select(o => o.Name).Intersect(cols).Count()).ToString());
            tableRoot.Attr("ExpandedRowCount", (tableRoot.Attr("ExpandedRowCount").Value.AsInt() + dataSource.Rows.Count + 1).ToString());



            Func<int, int> CalcColWidth = o =>
            {
                var colw = 10 + o * 6;
                if (colw > COLMAXWIDTH)
                    colw = COLMAXWIDTH;
                return colw;
            };

            ColMaxLengths.Reverse().All(o =>
            {
                var tempColumn = xmlDoc.CreateElement("Column", tableRoot.NamespaceURI);
                var tmpcolindex = cols.IndexOf(o.Key);
                if (hideColumnIndex != null && hideColumnIndex.Length > 0)
                {
                    if (hideColumnIndex.Contains(tmpcolindex))
                    {
                        tempColumn.Attr("ss:Index", (tmpcolindex + 1).AsString());
                        tempColumn.Attr("ss:Hidden", "1");

                    }
                }

                tempColumn.Attr("ss:Width", CalcColWidth(o.Value).AsString());
                tableRoot.PrependChild(tempColumn);
                return true;
            });

            return xmlDoc.InnerXml.Replace("&lt;br&gt;", "&#10;").Replace("&lt;br /&gt;", "&#10;");
        }

        /// <summary>
        /// 添加默认格式。
        /// </summary>
        /// <param name="xmlDoc"></param>
        private static void CheckDefaultStyle(XmlDocument xmlDoc, string sheetName)
        {
            var styles = xmlDoc.SelectSingleNode(@"/ns:Workbook/ns:Styles", xmlDoc.GetNameSpaceManager("ns"));

            var validDateStyle = false;
            var validBorderStyle = false;
            var validCenterStyle = false;

            //实例时间 格式
            foreach (XmlNode item in styles.ChildNodes)
            {
                var styleName = item.Attributes["ss:ID"].Value;
                if (styleName == "DateTime")
                {
                    validDateStyle = true;
                }
                else if (styleName == "Border")
                {
                    validBorderStyle = true;
                }
            }

            Func<XmlElement, XmlElement> GenBorder = pBorderNode =>
            {
                xmlDoc.CreateElement("Alignment", pBorderNode.NamespaceURI).ReturnSelf(e =>
                {
                    e.Attr("ss:Vertical", "Center");
                    e.Attr("ss:WrapText", "1");
                    pBorderNode.AppendChild(e);
                });

                var borders = xmlDoc.CreateElement("Borders", pBorderNode.NamespaceURI).ReturnSelf(e =>
                {
                    pBorderNode.AppendChild(e);
                });

                xmlDoc.CreateElement("Border", borders.NamespaceURI)
                .ReturnSelf(e =>
                {
                    e.Attr("ss:Position", "Bottom");
                    e.Attr("ss:LineStyle", "Continuous");
                    e.Attr("ss:Weight", "1");
                    borders.AppendChild(e);
                });

                xmlDoc.CreateElement("Border", borders.NamespaceURI)
                .ReturnSelf(e =>
                {
                    e.Attr("ss:Position", "Left");
                    e.Attr("ss:LineStyle", "Continuous");
                    e.Attr("ss:Weight", "1");
                    borders.AppendChild(e);
                });

                xmlDoc.CreateElement("Border", borders.NamespaceURI)
                .ReturnSelf(e =>
                {
                    e.Attr("ss:Position", "Right");
                    e.Attr("ss:LineStyle", "Continuous");
                    e.Attr("ss:Weight", "1");
                    borders.AppendChild(e);
                });


                xmlDoc.CreateElement("Border", borders.NamespaceURI)
                .ReturnSelf(e =>
                {
                    e.Attr("ss:Position", "Top");
                    e.Attr("ss:LineStyle", "Continuous");
                    e.Attr("ss:Weight", "1");
                    borders.AppendChild(e);
                });
                return borders;
            };

            if (validDateStyle == false)
            {
                /*
      <Style ss:ID="DateTime">
        <NumberFormat ss:Format="General Date" /> 
      </Style>
    */
                var style = xmlDoc.CreateElement("Style", styles.NamespaceURI).ReturnSelf(o =>
                {
                    o.Attr("ss:ID", "DateTime");
                    styles.AppendChild(o);
                });

                GenBorder(style);

                xmlDoc.CreateElement("NumberFormat", style.NamespaceURI)
                    .ReturnSelf(e =>
                    {
                        e.Attr("ss:Format", "General Date");
                        style.AppendChild(e);
                    });
            }

            if (validCenterStyle == false)
            {

                var style = xmlDoc.CreateElement("Style", styles.NamespaceURI).ReturnSelf(o =>
                {
                    o.Attr("ss:ID", "Center");
                    styles.AppendChild(o);
                });

                GenBorder(style);

                var atr = style.FirstChild;

                atr.Attr("ss:Horizontal", "Center");

                //xmlDoc.CreateElement("Alignment", style.NamespaceURI)
                //    .ReturnSelf(e =>
                //    {
                //        e.Attr("ss:Horizontal", "Center");
                //        style.AppendChild(e);
                //    });
            }

            //补全边框样式。
            if (validBorderStyle == false)
            {
                var style = xmlDoc.CreateElement("Style", styles.NamespaceURI).ReturnSelf(o =>
                {
                    o.Attr("ss:ID", "Border");
                    styles.AppendChild(o);
                });


                /*
       <Style ss:ID="Border">
       <Borders>
        <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>
        <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>
        <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>
        <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>
       </Borders>
      </Style>*/

                GenBorder(style);
            }


            //补全 Worksheet  , Worksheet/Table
            var tableRoot = xmlDoc.SelectSingleNode("/ns:Workbook/ns:Worksheet[@ss:Name='" + sheetName + "']", xmlDoc.GetNameSpaceManager("ns"));
            var pNode = xmlDoc.DocumentElement;
            if (tableRoot == null)
            {
                var sheet = xmlDoc.CreateElement("Worksheet", xmlDoc.DocumentElement.NamespaceURI);
                sheet.Attr("ss:Name", sheetName);
                pNode.AppendChild(sheet);
                pNode = sheet;
            }

            tableRoot = pNode.SelectSingleNode("ns:Table", xmlDoc.GetNameSpaceManager("ns"));
            if (tableRoot == null)
            {
                var table = xmlDoc.CreateElement("Table", pNode.NamespaceURI);
                table.Attr("ss:ExpandedColumnCount", "0");
                table.Attr("ss:ExpandedRowCount", "0");

                pNode.AppendChild(table);
            }
        }

        private static XmlNode[] TranslateXml(XmlNode tableRoot, MyOqlSet dataSource, string[] cols, Action<int, int, XmlNode> StyleFunc, Func<int, ColumnClip, object, DbType> TypeFunc)
        {
            //加载数据前，清空静态变量
            ColMaxLengths = new XmlDictionary<string, int>();
            List<XmlNode> list = new List<XmlNode>();
            for (var i = 0; i < dataSource.Rows.Count; i++)
            {
                list.Add(TranslateXml(tableRoot, dataSource, i, cols, StyleFunc, TypeFunc));
            }
            return list.ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableRoot"></param>
        /// <param name="dataSource"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cols">要导出的列别名</param>
        /// <param name="StyleFunc">样式回调，返回null表示不使用自定义样式，参数为：行索引，列索引，单元格。</param>
        /// <param name="TypeFunc">类型回调，返回(DbType)0表示不使用自定义类型，参数为：行索引，列，值。</param>
        /// <returns></returns>
        private static XmlNode TranslateXml(XmlNode tableRoot, MyOqlSet dataSource, int rowIndex, string[] cols, Action<int, int, XmlNode> StyleFunc, Func<int, ColumnClip, object, DbType> TypeFunc)
        {
            var xmlDoc = tableRoot.OwnerDocument;
            XmlNode node = xmlDoc.CreateElement("Row", tableRoot.NamespaceURI);
            tableRoot.AppendChild(node);
            for (var i = 0; i < cols.Length; i++)
            {
                var colName = cols[i];
                var colIndex = dataSource.Columns.IndexOf(o => o.Name == cols[i]);
                if (colIndex < 0) continue;

                XmlNode cell = xmlDoc.CreateElement("Cell", node.NamespaceURI);
                node.AppendChild(cell);

                SetStyleId(dataSource, rowIndex, StyleFunc, colIndex, cell);

                XmlNode data = xmlDoc.CreateElement("Data", cell.NamespaceURI);
                cell.AppendChild(data);


                DbType type = (DbType)0;

                if (TypeFunc != null)
                {
                    type = TypeFunc(rowIndex, dataSource.Columns[colIndex], dataSource.Rows[rowIndex][colName]);
                }

                if (type.HasValue() == false)
                {
                    type = dataSource.Columns[colIndex].DbType;
                }

                data.Attr("ss:Type", TranslateExcelType(type));


                data.InnerText = TranslateExcelValue(dataSource.Columns[colIndex].DbType, dataSource.Rows[rowIndex][colName]);
                //计算列宽
                var curCellLength = 0;
                if (data.InnerText.HasValue() && data.InnerText.AsDateTime().HasValue() && data.InnerText.Length > 11)
                {
                    curCellLength = 16;
                }
                else
                {
                    curCellLength = ExcelCoding.GetByteCount(data.InnerText.Trim());
                }

                if (ColMaxLengths.Keys.Contains(cols[i]))
                {
                    var lastCellLength = ColMaxLengths[cols[i]];
                    if (curCellLength > lastCellLength)
                    {
                        ColMaxLengths[cols[i]] = curCellLength;
                    }
                }
                else
                {
                    var colLength = ExcelCoding.GetByteCount(data.InnerText);
                    if (colLength > curCellLength)
                    {
                        ColMaxLengths.Add(cols[i], colLength);
                    }
                    else
                        ColMaxLengths.Add(cols[i], curCellLength);
                }
                //end 

                if (data.InnerText.HasValue() == false)
                {
                    data.Attr("ss:Type", "String");
                }
            }

            return node;
        }

        private static string TranslateExcelValue(DbType dbType, object value)
        {
            if (dbType.DbTypeIsDateTime())
            {
                var date = value.AsDateTime();
                if (date.HasValue() && date.Year > 1)
                {
                    return date.ToString("yyyy-MM-ddTHH:mm:ss.000");
                }

                else return value.AsString();
            }
            else return value.AsString();
        }


        private static void SetStyleId(MyOqlSet dataSource, int rowIndex, Action<int, int, XmlNode> StyleFunc, int cellIndex, XmlNode cell)
        {
            if (dataSource.Columns[cellIndex].DbType.DbTypeIsDateTime()) cell.Attr("ss:StyleID", "DateTime");
            else cell.Attr("ss:StyleID", "Border");

            if (StyleFunc != null)
            {
                StyleFunc(rowIndex, cellIndex, cell);
            }
        }

        private static string TranslateExcelType(DbType dbType)
        {
            if (dbType.DbTypeIsNumber()) return "Number";
            else if (dbType.DbTypeIsDateTime()) return "DateTime";
            else return "String";
        }

        public static string SetGroupStyle(string xmlExcelContent, string sheetName, int headerRowIndex, params string[] groups)
        {
            GodError.Check(groups == null, () => "分组列不能为空");
            GodError.Check(groups.Length == 0, () => "不能没有分组列");

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlExcelContent);

            var tableRoot = xmlDoc.SelectSingleNode("/ns:Workbook/ns:Worksheet[@ss:Name='" + sheetName + "']/ns:Table", xmlDoc.GetNameSpaceManager("ns"));
            var groupCellIndexs = new List<int>();

            var header = tableRoot.SelectNodes("ns:Row[" + (headerRowIndex + 1) + @"]/ns:Cell/ns:Data", xmlDoc.GetNameSpaceManager("ns"));
            groups.All(o =>
            {
                for (int i = 0; i < header.Count; i++)
                {
                    if (header[i].InnerText == o)
                    {
                        //XPath的定义中没有第0元素这种东西，索引是从1开始的。
                        groupCellIndexs.Add(i + 1);
                        break;
                    }
                }
                return true;
            });

            var cellDict = new Dictionary<int, XmlNodeList>();

            groupCellIndexs.ForEach(groupCellIndex =>
            {
                cellDict[groupCellIndex] = tableRoot.SelectNodes("ns:Row/ns:Cell[" + groupCellIndex + "]", xmlDoc.GetNameSpaceManager("ns"));
            });


            var prevGroupCellIndex = -1;
            var newGroupRowIndex = -1;
            var prevGroupCellText = string.Empty;

            groupCellIndexs.ForEach(groupCellIndex =>
            {

                var cells = cellDict[groupCellIndex];
                for (int i = headerRowIndex; i < cells.Count; i++)
                {
                    var pwbs = string.Empty;
                    if (prevGroupCellIndex >= 0)
                    {
                        pwbs = cellDict[prevGroupCellIndex][i].Attr("wbs").Value + ".";
                    }

                    var currentText = cells[i].InnerText;
                    if (currentText != prevGroupCellText)
                    {
                        newGroupRowIndex = i;
                    }

                    cells[i].Attr("wbs", pwbs + newGroupRowIndex.ToString());

                    //设置下该行的下一个元素的 Index 属性
                    var next = cells[i].NextSibling;
                    if (next != null)
                    {
                        next.Attr("ss:Index", (groupCellIndex + 1).ToString());
                    }
                    prevGroupCellText = currentText;
                }

                prevGroupCellIndex = groupCellIndex;
            });

            var newWbs = string.Empty;
            int newWbsCellIndex = headerRowIndex;
            //按groupCellIndexs倒序，根据 wbs 增加合并行属性，并去除多余的行。
            groupCellIndexs.Reverse();
            groupCellIndexs.ForEach(groupCellIndex =>
            {
                var cells = cellDict[groupCellIndex];

                for (int i = headerRowIndex + 1; i < cells.Count; i++)
                {
                    var wbs = cells[i].Attr("wbs").Value;

                    if (wbs == newWbs)
                    {
                        cells[i].ParentNode.RemoveChild(cells[i]);
                    }
                    else
                    {
                        if (i - newWbsCellIndex - 1 > 0)
                        {
                            XmlExcel.Attr(cells[newWbsCellIndex], "ss:MergeDown", (i - newWbsCellIndex - 1).ToString());
                        }

                        newWbsCellIndex = i;
                        newWbs = wbs;
                    }

                }
            });



            groupCellIndexs.ForEach(groupCellIndex =>
            {
                var cells = tableRoot.SelectNodes("ns:Row/ns:Cell[" + groupCellIndex + "]", xmlDoc.GetNameSpaceManager("ns"));
                for (int i = headerRowIndex; i < cells.Count; i++)
                {
                    cells[i].Attributes.Remove(cells[i].Attr("wbs"));
                }
            });


            return xmlDoc.InnerXml;
        }


        public static string LoadExcel(int AnnexId, string SheetName)
        {
            var client = new OleDbServiceClient();

            var annexEnt = dbr.Annex.FindById(AnnexId);
            var excelPath = HttpContext.Current.Server.MapPath(annexEnt.FullName);

            try
            {
                return client.ReadAsMyOqlSet(excelPath, SheetName);
            }
            catch { throw new GodError("工作表不存在！请重新输入。"); }
        }

        public static string[] GetExcelColumns(int AnnexId, string SheetName)
        {
            var client = new OleDbServiceClient();

            var annexEnt = dbr.Annex.FindById(AnnexId);
            var excelPath = HttpContext.Current.Server.MapPath(annexEnt.FullName);

            try
            {
                return client.GetExcelColumns(excelPath, SheetName);
            }
            catch { throw new GodError("工作表不存在！请重新输入。"); }
        }
    }

    public enum XmlExcelStyle
    {
        DateTime = 1,
        Border,
    }
}