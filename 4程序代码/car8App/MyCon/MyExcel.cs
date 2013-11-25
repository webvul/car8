using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using MyCmn;
using MyOql;
using System.Xml;
using System.Collections;
using System.Web.Mvc;
using MyBiz;

namespace System.Web.Mvc
{

    public static partial class MyHtmlHelper
    {
        #region 母板字符串
        private const string EXCEL_TEMPLATE = @"<?xml version=""1.0""?>
<?mso-application progid=""Excel.Sheet""?>
<Workbook xmlns=""urn:schemas-microsoft-com:office:spreadsheet""
 xmlns:o=""urn:schemas-microsoft-com:office:office""
 xmlns:x=""urn:schemas-microsoft-com:office:excel""
 xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet""
 xmlns:html=""http://www.w3.org/TR/REC-html40"">
 <DocumentProperties xmlns=""urn:schemas-microsoft-com:office:office"">
  <Created>2006-09-16T00:00:00Z</Created>
  <LastSaved>2006-09-16T00:00:00Z</LastSaved>
  <Version>14.00</Version>
 </DocumentProperties>
 <OfficeDocumentSettings xmlns=""urn:schemas-microsoft-com:office:office"">
  <AllowPNG/>
  <RemovePersonalInformation/>
 </OfficeDocumentSettings>
 <ExcelWorkbook xmlns=""urn:schemas-microsoft-com:office:excel"">
  <WindowHeight>8010</WindowHeight>
  <WindowWidth>14805</WindowWidth>
  <WindowTopX>240</WindowTopX>
  <WindowTopY>105</WindowTopY>
  <ProtectStructure>False</ProtectStructure>
  <ProtectWindows>False</ProtectWindows>
 </ExcelWorkbook>
 <Styles>
  <Style ss:ID=""Default"" ss:Name=""Normal"">
   <Alignment ss:Vertical=""Bottom""/>
   <Borders/>
   <Font ss:FontName=""宋体"" x:CharSet=""134"" ss:Size=""11"" ss:Color=""#000000""/>
   <Interior/>
   <NumberFormat/>
   <Protection/>
  </Style>
 </Styles>
</Workbook>
";
        #endregion

        public static DownloadResult ExportToExcel(this MyOqlSet dataSource, string fileName, StringDict dictMap, Action<int, int, XmlNode> StyleFunc = null)
        {
            var ram1 = new Random();
            var ramStr = ram1.Next();
            var newExName = fileName.Replace(".", ramStr.AsString() + ".");
            return new DownloadResult(newExName, GetExcelCotent(dataSource, dictMap, StyleFunc));
        }


        /// <summary>
        /// 用于住户的分组，合并单元格 lcc 2012-10-16
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="fileName"></param>
        /// <param name="dictMap"></param>
        /// <param name="GroupName"></param>
        /// <param name="StyleFunc"></param>
        /// <returns></returns>
        public static DownloadResult ExportToExcel(this MyOqlSet dataSource, string fileName, StringDict dictMap, string[] GroupName, Action<int, int, XmlNode> StyleFunc = null, int[] hideColumnIdex = null)
        {

            var content = GetExcelCotent(dataSource, dictMap, StyleFunc, hideColumnIdex);

            return new DownloadResult(fileName, XmlExcel.SetGroupStyle(content, "Sheet1", 0, GroupName));
        }


        /// <summary>
        /// 生成excel文件
        /// 批量入账使用
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="dictMap"></param>
        /// <param name="StyleFunc"></param>
        /// <returns></returns>
        public static string GetExcelCotent(this MyOqlSet dataSource, StringDict dictMap, Action<int, int, XmlNode> StyleFunc = null, int[] hideColumnIndex = null)
        {
            var ds = dataSource.Clone() as MyOqlSet;


            var dict = new RowData();
            dataSource.Columns.All(o =>
            {
                dict[o.Name] = string.Empty;
                return true;
            });

            dictMap.All(o =>
            {
                if (dict.ContainsKey(o.Key)) { dict[o.Key] = o.Value; }
                return true;
            });


            ds.Rows.Insert(0, dict);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(EXCEL_TEMPLATE);


            var content = XmlExcel.ToExcel(ds, xmlDoc, "Sheet1", dictMap.Select(o => o.Key).ToArray(),
                (rowIndex, columnIndex, cell) =>
                {
                    if (rowIndex == 0)
                    {
                        var atr = cell.Attributes.GetNamedItem("ss:StyleID") as XmlAttribute;
                        if (atr != null)
                        {
                            atr.Value = "Center";
                        }
                    }

                    if (StyleFunc != null)
                    {
                        StyleFunc(rowIndex, columnIndex, cell);
                    }

                }, (rowIndex, column, val) => rowIndex == 0 ? DbType.AnsiString : (DbType)(-1),
                hideColumnIndex);

            return content;
        }

    }
}