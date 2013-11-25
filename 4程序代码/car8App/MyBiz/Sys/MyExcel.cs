using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using MyCmn;
using MyOql;

namespace MyBiz
{
    public class MyExcel
    {
        internal class ExcelDefine
        {
            public bool IsUnique { get; set; }
            public string ExcelColumn { get; set; }
            public string DbSqlColumn { get; set; }
            public Type ColumnType { get; set; }

            public ExcelDefine(string ExcelColumn, string DbSqlColumn, Type type)
                : this(ExcelColumn, DbSqlColumn, type, false) { }

            public ExcelDefine(string ExcelColumn, string DbSqlColumn, Type type, bool IsUnique)
            {
                this.ExcelColumn = ExcelColumn;
                this.DbSqlColumn = DbSqlColumn;
                this.IsUnique = IsUnique;
                ColumnType = type;
            }

            public ExcelDefine(string DbSqlColumn, Type type, bool IsUnique) : this(null, DbSqlColumn, type, IsUnique) { }

            public ExcelDefine(string DbSqlColumn, Type type) : this(DbSqlColumn, type, false) { }
        }

        public static string GetConnectionString(string FileName)
        {
            return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FileName + ";Extended Properties='Excel 8.0;HDR=No;IMEX=1;';Persist Security Info=False";
        }

        public static string GetSheetName(string FileName, int Index)
        {
            System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection();
            conn.ConnectionString = MyExcel.GetConnectionString(FileName);
            conn.Open();
            string sheet = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null).Rows[Index]["TABLE_NAME"].AsString();
            conn.Close();
            return sheet;
        }

        public static List<string> ToDbSql(string FileName, string SheetName, string TableName, int ExcelRowStartIndex, Dictionary<string, string> DictExcelAndSqlColumnsMap)
        {
            return ToDbSql(FileName, SheetName, TableName, ExcelRowStartIndex, DictExcelAndSqlColumnsMap, null);
        }
        /// <summary>
        /// 从Excel生成 插入到数据库的SQL
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="SheetName"></param>
        /// <param name="TableName"></param>
        /// <param name="ExcelRowStartIndex">Excel 里开始导入的索引号，一个表头的话，开始号为1,它的前一项作为表头。</param>
        /// <param name="DictColumnMap">Key 是 Excel 的列名。 Value 是数据库列名。</param>
        /// <returns></returns>
        public static List<string> ToDbSql(string FileName, string SheetName, string TableName, int ExcelRowStartIndex, Dictionary<string, string> DictExcelAndSqlColumnsMap, Func<Dictionary<string, string>, bool> func)
        {
            List<ExcelDefine> DictColumnMap = TidyToExcelDefine(TableName, DictExcelAndSqlColumnsMap);

            DataTable dt = TidyToDataTable(FileName, SheetName);
            List<string> listColumns = TidyGetExcelColumns(ExcelRowStartIndex, dt);
            List<Dictionary<string, string>> dictSqlValue = __ToSql(TableName, ExcelRowStartIndex, DictColumnMap, dt, listColumns);


            List<string> listSql = new List<string>();
            foreach (var item in dictSqlValue)
            {
                if (func != null && func.Invoke(item) == false) continue;
                listSql.Add(TidyInsertSql(TableName, item));
            }

            return listSql;
        }

        private static List<Dictionary<string, string>> __ToSql(string TableName, int ExcelRowStartIndex, List<ExcelDefine> DictColumnMap, DataTable dt, List<string> listColumns)
        {

            Dictionary<string, List<string>> listUnion = new Dictionary<string, List<string>>();
            //先做本Excel 的唯一校验
            foreach (var item in DictColumnMap.Where(o => o.IsUnique))
            {
                listUnion.Add(item.DbSqlColumn, new List<string>());
            }


            Dictionary<KeyValuePair<string, string>, KeyValuePair<int, List<string>>> listForeign = new Dictionary<KeyValuePair<string, string>, KeyValuePair<int, List<string>>>();


            DataTable dtForeign = dbo.ToDataTable("数据库配置", string.Format(@"
select object_name(f.parent_object_id) as HostTable , object_name(f.referenced_object_id) as ReferenceTable 
,hc.name as HostColumn , rc.name as ReferenceColumn
 from sys.foreign_keys as f 
 join sys.foreign_key_columns as k on ( f.object_id = k.constraint_object_id)
 join sys.columns as hc on ( k.parent_object_id = hc.object_id and k.parent_column_id = hc.column_id )
join sys.columns as rc on ( k.referenced_object_id = rc.object_id and k.referenced_column_id = rc.column_id )
where f.parent_object_id = object_id('{0}')"
                , TableName)
                );
            if (dtForeign.DataTableHasData())
            {
                foreach (DataRow row in dtForeign.Rows)
                {
                    listForeign.Add(new KeyValuePair<string, string>(row["ReferenceTable"].AsString(), row["ReferenceColumn"].AsString())
                        , new KeyValuePair<int, List<string>>(listColumns.IndexOf(DictColumnMap.Where(o => o.DbSqlColumn == row["ReferenceColumn"].AsString()).FirstOrDefault().ExcelColumn),
                            new List<string>()));
                }
            }


            List<Dictionary<string, string>> listSql = new List<Dictionary<string, string>>();

            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (i < ExcelRowStartIndex)
                {
                    i++;
                    continue;
                }

                bool IsEnd = true;
                foreach (DataColumn col in dt.Columns)
                {
                    if ((row[col] is DBNull) == false)
                    {
                        IsEnd = false; break;
                    }
                }

                if (IsEnd) break;

                Dictionary<string, string> dict = DictColumnMap.ToDictionary(o => o.DbSqlColumn, o => row[listColumns.IndexOf(o.ExcelColumn)].AsString().Replace("'", "''"));



                //校验类型。

                foreach (ExcelDefine item in DictColumnMap)
                {
                    if (item.ColumnType == typeof(string)) continue;
                    else if (item.ColumnType == typeof(int))
                    {
                        try
                        {
                            Convert.ChangeType(row[listColumns.IndexOf(item.ExcelColumn)], item.ColumnType);
                        }
                        catch
                        {
                            throw new Exception(string.Format(@"第 {0} 行 {1} 列数据类型转换错误,请检查!", i, listColumns.IndexOf(item.ExcelColumn) + 1));
                        }
                    }
                    else if (item.ColumnType == typeof(DateTime))
                    {
                        try
                        {
                            Convert.ChangeType(row[listColumns.IndexOf(item.ExcelColumn)], item.ColumnType);
                        }
                        catch
                        {
                            throw new Exception(string.Format(@"第 {0} 行 {1} 列数据类型转换错误,请检查!", i + 1, listColumns.IndexOf(item.ExcelColumn) + 1));
                        }
                    }
                }


                listSql.Add(dict);

                //添加唯一校验数据
                foreach (var c in listUnion.Keys)
                {
                    listUnion[c].Add(row
                        [
                            listColumns.IndexOf(DictColumnMap.Where(o => o.DbSqlColumn == c).Select(o => o.ExcelColumn).First())
                        ].AsString().Replace("'", "''"));
                }

                //添加外键数据。
                foreach (var fg in listForeign)
                {
                    fg.Value.Value.Add(row[fg.Value.Key].AsString());
                }
            }

            TidyCheck(TableName, listUnion, listForeign, DictColumnMap);
            return listSql;
        }

        private static string TidyInsertSql(string TableName, Dictionary<string, string> dict)
        {
            string strSql = string.Format("insert into {0} ({1}) values ('{2}');",
                TableName,
                string.Join(",", dict.Select(o => o.Key).ToArray()),
                string.Join("','", dict.Select(o => o.Value).ToArray())
                );
            return strSql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="listUnion">检查唯一性，Key：列名，Value：列应该具有的值</param>
        /// <param name="listForeignKeyData">检查外键 Key: KeyValue 引用表的表名和列名。 Value 的 Key 是Excel列索引，Value 是表名和列名所应该具有的值。</param>
        /// <param name="DictColumnMap"></param>
        private static void TidyCheck(string TableName, Dictionary<string, List<string>> listUnion, Dictionary<KeyValuePair<string, string>, KeyValuePair<int, List<string>>> listForeignKeyData, List<ExcelDefine> DictColumnMap)
        {
            //唯一校验
            foreach (var c in listUnion.Keys)
            {
                var cou = listUnion[c].GroupBy(o => o).Where(o => o.Count() > 1).Select(o => o.Key);
                if (cou.Count() > 0)
                {
                    throw new Exception(string.Format(@"第{0}列在Excel存在重复值{1}!",
                        (DictColumnMap.IndexOf(DictColumnMap.Where(o => o.DbSqlColumn == c).First()) + 1).AsString(),
                        cou.ElementAt(0)));
                }
            }

            //在数据库校验 
            foreach (var c in listUnion.Keys)
            {
                for (int k = 0; k < listUnion[c].Count; k++)
                {
                    int fetch = Math.Min(200, listUnion[c].Count - k);
                    string query = string.Format(@"select {1} from {0} where {1} in ('{2}');", TableName, c,
                        string.Join("','", listUnion[c].Skip(k).Take(fetch).ToArray()
                        ));

                    string exits = dbo.ToScalar("数据库配置", query).AsString();
                    if (!string.IsNullOrEmpty(exits))
                    {
                        throw new Exception(string.Format(@"第{0}列在数据库里存在重复值{1}!",
                            (DictColumnMap.IndexOf(DictColumnMap.Where(o => o.DbSqlColumn == c).First()) + 1).AsString(),
                            exits));
                    }

                    k = fetch - 1;
                }
            }

            //外键约束

            foreach (var item in listForeignKeyData)
            {
                string refTable = item.Key.Key;
                string refColumn = item.Key.Value;
                int excelIndex = item.Value.Key;
                var values = item.Value.Value;

                for (int k = 0; k < values.Count; k++)
                {
                    int fetch = Math.Min(200, values.Count - k);
                    string query = string.Format(@"
declare @tab table(id int)
insert into @tab {0}
select * from @tab where id not in
(select {1} from {2}
where {1}  in (select id from @tab))
"
                       , string.Join(" union ", values.Skip(k).Take(fetch).Select(o => "select '" + o + "'").ToArray())
                       , refColumn, refTable
                    );

                    string exits = dbo.ToScalar("数据库配置", query).AsString();
                    if (exits.HasValue())
                    {
                        throw new Exception(string.Format(@"在{0}表中，不存在主键为：{1}的数据！",
                            refTable, exits));
                    }

                    k = fetch - 1;
                }
            }

        }

        private static DataTable TidyToDataTable(string FileName, string SheetName)
        {
            OleDbDataAdapter adp = new OleDbDataAdapter("select * from [" + SheetName + "]",
                GetConnectionString(FileName));
            DataTable dt = new DataTable();
            adp.Fill(dt);
            return dt;
        }

        private static List<string> TidyGetExcelColumns(int ExcelRowStartIndex, DataTable dt)
        {
            List<string> listColumns = new List<string>();
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (i < ExcelRowStartIndex)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string theOne = row[j].AsString().Trim();
                        if (listColumns.Count <= j)
                        {
                            listColumns.Add(theOne);
                        }
                        else if (theOne.HasValue())
                        {
                            listColumns[j] = theOne;
                        }
                    }
                    i++;
                    continue;
                }
                else break;
            }
            return listColumns;
        }

        private static List<ExcelDefine> TidyToExcelDefine(string TableName, Dictionary<string, string> DictExcelAndSqlColumnsMap)
        {
            List<ExcelDefine> DictColumnMap = new List<ExcelDefine>();

            DataTable dtScalar = dbo.ToDataTable("数据库配置", string.Format(@"
select c.name as ColName, t.name as TypeName, c.max_length as MaxLen, id.is_unique as IsUnique
from sys.columns as c 
join sys.types as t on (c.user_type_id =  t.user_type_id)
left join
(
	select i.object_id , i.is_unique , si.colid  
	from sys.indexes as i join sys.sysindexkeys as si 
	on ( i.object_id = si.id )
) as id on ( c.object_id = id.object_id and c.column_id = id.colid )
where c.object_id = object_id('{0}')
and c.is_computed = 0 
"
                , TableName));

            foreach (DataRow row in dtScalar.Rows)
            {
                if (DictExcelAndSqlColumnsMap.ContainsValue(row["ColName"].AsString()) == false) continue;
                int idn = DictExcelAndSqlColumnsMap.Values.ToList().IndexOf(row["ColName"].AsString());
                DictColumnMap.Add(
                    new ExcelDefine(DictExcelAndSqlColumnsMap.ElementAt(idn).Key,
                        row["ColName"].AsString(),
                        new MyOql.Provider.SqlServer().ToDbType(row["TypeName"].AsString()).GetCsType(),
                    //ValueProc.GetDbTypeDict().Where(o => o.SqlType == row["TypeName"].AsString()).First().CsType,
                    row["IsUnique"].AsBool()));
            }
            return DictColumnMap;
        }

        public static List<string> ToDbSql(string FileName, string SheetName, string TableName, int ExcelRowStartIndex, int DbSqlColumnStartIndex, int[] IgnoreExcelColumnIndexs)
        {
            return ToDbSql(FileName, SheetName, TableName, ExcelRowStartIndex, DbSqlColumnStartIndex, IgnoreExcelColumnIndexs, null);
        }


        /// <summary>
        /// 根据Excel生成插入到数据库的SQL
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="SheetName"></param>
        /// <param name="TableName"></param>
        /// <param name="ExcelRowStartIndex"></param>
        /// <param name="DbSqlColumnStartIndex"></param>
        /// <param name="IgnoreExcelColumnIndexs"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static List<string> ToDbSql(string FileName, string SheetName, string TableName, int ExcelRowStartIndex, int DbSqlColumnStartIndex, int[] IgnoreExcelColumnIndexs, Func<Dictionary<string, string>, bool> func)
        {
            {
                OleDbDataAdapter adp = new OleDbDataAdapter("select * from [" + SheetName + "]", MyExcel.GetConnectionString(FileName));
                DataTable dt = new DataTable();
                adp.Fill(dt);

                List<string> listColumns = new List<string>();

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (IgnoreExcelColumnIndexs.Contains(i)) continue;
                    listColumns.Add(dt.Columns[i].ColumnName);
                }

                Dictionary<string, string> DictExcelAndSqlColumnsMap = new Dictionary<string, string>();

                DataTable dtScalar = dbo.ToDataTable("数据库配置", string.Format(@"
select c.name as ColName, t.name as TypeName, c.max_length as MaxLen, id.is_unique as IsUnique
from sys.columns as c 
join sys.types as t on (c.user_type_id =  t.user_type_id)
left join
(
	select i.object_id , i.is_unique , si.colid  
	from sys.indexes as i join sys.sysindexkeys as si 
	on ( i.object_id = si.id )
) as id on ( c.object_id = id.object_id and c.column_id = id.colid )
where c.object_id = object_id('{0}')
and c.is_computed = 0 and column_id >= {1}
"
                , TableName
                , DbSqlColumnStartIndex
                ));

                for (int i = 0; i < listColumns.Count; i++)
                {
                    DictExcelAndSqlColumnsMap.Add(listColumns[i], dtScalar.Rows[i + DbSqlColumnStartIndex]["ColName"].AsString());
                }

                List<ExcelDefine> DictColumnMap = TidyToExcelDefine(TableName, DictExcelAndSqlColumnsMap);


                List<Dictionary<string, string>> dictSqlValue = __ToSql(TableName, ExcelRowStartIndex + 1, DictColumnMap, dt, listColumns);

                List<string> listSql = new List<string>();
                foreach (var item in dictSqlValue)
                {
                    if (func != null && func.Invoke(item) == false) continue;
                    listSql.Add(TidyInsertSql(TableName, item));
                }

                return listSql;
            }
        }

        /// <summary>
        /// 生成插入到  Excel 的 SQL 语句。 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="SheetName"></param>
        /// <param name="Columns">指 obj  里的 Key 值。</param>
        /// <returns></returns>
        public static OleDbCommand GenExcelCommand(XmlDictionary<string, object> obj, string SheetName, XmlDictionary<string, string> Columns)
        {
            StringLinker strSql = "insert into [";
            strSql += SheetName;
            strSql += "] (";

            strSql += string.Join(",", Columns.Select(o => "[" + o.Value + "]").ToArray());

            strSql += ") values(";
            strSql += string.Join(",", Columns.Select(o => "?").ToArray());
            strSql += ")";

            OleDbCommand cmd = new OleDbCommand(strSql);

            Columns.All(o =>
            {
                var p = new OleDbParameter("?", OleDbType.VarChar);
                p.Value = obj[o.Key];
                cmd.Parameters.Add(p);
                return true;
            });

            return cmd;
        }

        /// <summary>
        /// 生成插入到  Excel 的 SQL 语句。 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="SheetName"></param>
        /// <param name="Columns">指 obj  里的 Key 值。</param>
        /// <returns></returns>
        public static OleDbCommand GenExcelCommand(XmlDictionary<string, object> obj, string SheetName, params string[] Columns)
        {
            StringLinker strSql = "insert into [";
            strSql += SheetName;
            strSql += "] (";
            strSql += string.Join(",", Columns.Select(o => "[" + o + "]").ToArray());

            strSql += ") values(";
            strSql += string.Join(",", Columns.Select(o => "?").ToArray());
            strSql += ")";

            OleDbCommand cmd = new OleDbCommand(strSql);

            Columns.All(o =>
            {
                var p = new OleDbParameter("?", OleDbType.VarChar);
                p.Value = obj[o];
                cmd.Parameters.Add(p);
                return true;
            });

            return cmd;
        }
    }
}
