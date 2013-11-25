using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using System.Web;
using MyOql;
using MyCmn;
using ICSharpCode.SharpZipLib.Zip;
using MyBiz;

namespace MyBiz.Sys
{
    public class OpenXml
    {
        public Dictionary<string, XmlDocument> Xmls { get; set; }
        public Dictionary<string, byte[]> Extend { get; set; }

        ///// <summary>
        ///// 一定要是空的文件夹.
        ///// </summary>
        //public string TempFolder { get; set; }

        public void Load(DataTable dt, string SheetName)
        {
            if (this.GetSheets().ContainsValue(SheetName) == false)
            {
                this.AddNewSheet(SheetName);
            }

            Func<DataRow, string[]> toData = row =>
            {
                var list = new List<string>();
                foreach (DataColumn col in row.Table.Columns)
                {
                    list.Add(row[col].AsString());
                }
                return list.ToArray();
            };


            //默认该Sheet 内容为空.
            foreach (DataRow row in dt.Rows)
            {
                this.AddRow(SheetName, toData(row));
            }
        }

        /// <summary>
        /// 加载数据.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="SheetName"></param>
        /// <param name="objCol"></param>
        public void LoadWithColumn(DataTable dt, string SheetName, XmlDictionary<string, string> objCol)
        {
            if (this.GetSheets().ContainsValue(SheetName) == false)
            {
                this.AddNewSheet(SheetName);
            }

            Func<DataRow, string[]> toData = row =>
            {
                var list = new List<string>();
                foreach (DataColumn col in row.Table.Columns)
                {
                    list.Add(row[col].AsString());
                }
                return list.ToArray();
            };

            var cols = new List<string>();
            foreach (DataColumn co in dt.Columns)
            {
                if (objCol.Keys.Contains(co.ColumnName))
                {

                    cols.Add(objCol[co.ColumnName]);
                }
                else
                    cols.Add(co.ColumnName);
            }


            this.AddRow(SheetName, cols.ToArray());
            //默认该Sheet 内容为空.
            foreach (DataRow row in dt.Rows)
            {
                this.AddRow(SheetName, toData(row));
            }
        }

        /// <summary>
        /// 加载Excel 到 中间对象.
        /// </summary>
        /// <param name="ExcelStream"></param>
        /// <param name="TempFolder">传入~/Upload 对应的物理文件夹.</param>
        public OpenXml(Stream ExcelStream, string TempFolder)
            : this()
        {
            ToFolder(TempFolder);

            GodError.Check(Directory.GetFiles(this.Folder, "*", SearchOption.AllDirectories).Length > 0,   "解压缩的临时文件夹不能包含文件");

            Init(ExcelStream);
        }

        private void Init(Stream ExcelStream)
        {
            new FastZip().ExtractZip(ExcelStream, this.Folder, FastZip.Overwrite.Always, null, "", "", false, true);

            // MyCompress.DeCompress(ExcelStream, TempFolder);

            foreach (var fileName in Directory.GetFiles(this.Folder, "*.*", SearchOption.AllDirectories))
            {
                var fi = new FileInfo(fileName);
                if (fi.Extension == ".xml")
                {
                    var xml = new XmlDocument();
                    xml.Load(fileName);

                    Xmls[fi.FullName.Substring(this.Folder.Length + 1)] = xml;
                }
                else
                {
                    Extend[fi.FullName.Substring(this.Folder.Length + 1)] = File.ReadAllBytes(fileName);
                }
            }

            //
            Tidy();
        }

        private void ToFolder(string TempFolder)
        {
            var folder = TempFolder.TrimEnd(@"\") + @"\" + Guid.NewGuid().ToString();
            DirectoryInfo di = new DirectoryInfo(folder);
            if (di.Exists == false)
            {
                di.Create();
            }

            this.Folder = folder;
        }



        private void Tidy()
        {
            foreach (var key in Xmls.Keys)
            {
                if (key.StartsWith(@"xl\worksheets\"))
                {

                    Xmls[key].DocumentElement.RemoveChild(
                        Xmls[key].DocumentElement.SelectSingleNode("ns:dimension", Xmls[key].GetNameSpaceManager("ns")));
                    //Xmls[key].DocumentElement.FirstChild
                    //);
                }
            }

            GodError.Check(Xmls.ContainsKey(@"xl\sharedStrings.xml") == false,   @"找不到必要的 xl\sharedStrings.xml ");

        }

        public OpenXml()
        {
            this.Xmls = new Dictionary<string, XmlDocument>();
            this.Extend = new Dictionary<string, byte[]>();

            //GodError.Check(Directory.GetFiles(this.TempFolder).Length > 0, "临时文件夹,不能包含文件.");
            //GodError.Check(Directory.GetDirectories(this.TempFolder).Length > 0, "临时文件夹,不能包含文件夹.");
        }

        public Dictionary<int, string> GetSheets()
        {
            var list = new Dictionary<int, string>();
            foreach (XmlNode node in Xmls[@"xl\workbook.xml"].DocumentElement.SelectNodes(@"ns:sheets/ns:sheet", Xmls[@"xl\workbook.xml"].GetNameSpaceManager("ns")))
            {
                list[node.Attributes["sheetId"].Value.AsInt()] = node.Attributes["name"].Value;
            }
            return list;
        }

        public void AddNewSheet(string SheetName)
        {
        }

        public void ChangeSheetName(string SheetName, string NewName)
        {
        }

        public int GetColumnLength()
        {
            return 0;
        }

        public int GetRowLength()
        {
            return 0;
        }


        public string[] GetRowData(int RowIndex)
        {
            return null;
        }

        public XmlDocument GetSheetXml(string SheetName)
        {
            var sheets = this.GetSheets();
            if (sheets.ContainsValue(SheetName) == false)
                return null;

            foreach (var key in Xmls.Keys)
            {
                if (key.StartsWith(@"xl\worksheets\sheet" + sheets.First(o => o.Value == SheetName).Key + ".xml"))
                {
                    return Xmls[key];
                }
            }
            return null;
        }

        public void AddRow(string SheetName, string[] Data)
        {
            var excl = GetSheetXml(SheetName);
            if (excl == null)
            {
                this.AddNewSheet(SheetName);

                excl = GetSheetXml(SheetName);
            }
            var exclDef = excl.SelectSingleNode(@"ns:worksheet/ns:sheetData", excl.GetNameSpaceManager("ns"));

            var maxRowId = 0;

            foreach (XmlNode n in exclDef.ChildNodes)
            {
                maxRowId = Math.Max(n.Attributes["r"].Value.AsInt(), maxRowId);
            }

            //row Id 从1 开始.
            maxRowId++;

            var exclNewRow = excl.CreateElement("row", exclDef.NamespaceURI);
            {
                var r = excl.CreateAttribute("r");
                r.Value = maxRowId.AsString();
                exclNewRow.Attributes.Append(r);
            }

            Action<XmlNode, string> AddValue = (e, v) =>
            {
                var si = e.OwnerDocument.CreateElement("si", e.NamespaceURI);
                var t = e.OwnerDocument.CreateElement("t", si.NamespaceURI);

                t.InnerText = v;

                si.AppendChild(t);
                e.AppendChild(si);
            };

            var data = Xmls[@"xl\sharedStrings.xml"].DocumentElement;


            for (int index = 0; index < Data.Length; index++) //each (var item in Data)
            {
                var item = Data[index];
                if (item.HasValue() == false) continue;

                var c = excl.CreateElement("c", exclNewRow.NamespaceURI);
                {
                    var r = excl.CreateAttribute("r");
                    r.Value = GetExcelColumn(index) + maxRowId;
                    c.Attributes.Append(r);
                }
                {
                    var t = excl.CreateAttribute("t");
                    t.Value = "s";
                    c.Attributes.Append(t);
                }

                var maxValueIndex = data.ChildNodes.Count;

                AddValue(data, item);

                var v = excl.CreateElement("v", c.NamespaceURI);

                // Cell Index 从0 开始. 
                v.InnerText = maxValueIndex.AsString();


                c.AppendChild(v);
                exclNewRow.AppendChild(c);
            }

            exclDef.AppendChild(exclNewRow);
        }


        /// <summary>
        /// 转换成 Excel
        /// </summary>
        /// <returns></returns>
        public byte[] ToExcel()
        {
            return ToExcel(true);
        }

        private byte[] ToExcel(bool DeleteTempFolder)
        {
            foreach (var key in Xmls.Keys)
            {
                Xmls[key].Save(this.Folder + Path.DirectorySeparatorChar + key);
            }

            foreach (var key in Extend.Keys)
            {
                File.WriteAllBytes(this.Folder + Path.DirectorySeparatorChar + key, Extend[key]);
            }


            MemoryStream mem = new MemoryStream();

            new FastZip().CreateZip(mem, this.Folder, true, "", "");

            if (DeleteTempFolder)
            {
                Directory.Delete(this.Folder, true);
            }

            return mem.ToArray();
        }

        /// <summary>
        /// 将列号 转换成列名 eg. 702=AAA
        /// </summary>
        /// <param name="Index"></param>
        private string GetExcelColumn(Int32 Index)
        {
            //Index--;
            //GodError.Check(Index <= 0, "列索引要求大于0");

            //C# 索引算法.
            string CharSequece = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            List<char> retVal = new List<char>();

            while (true)
            {
                var mod = Index % CharSequece.Length;
                retVal.Add(CharSequece[mod]);

                if (Index < CharSequece.Length) break;
                Index = Index / CharSequece.Length - 1;
            }
            retVal.Reverse();
            return new string(retVal.ToArray());

        }



        public string Folder { get; set; }
    }
}
