using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MyCmn;
using System.Data.Common;
using System.Data;
using System.Reflection;
using System.Collections;
using System.Web.UI;
using System.ComponentModel;
using Newtonsoft.Json;

namespace MyOql
{
    [Serializable]
    public class MyOqlSet : SqlClipBase, IModel
    {
        public MyOqlSet()
        {
            this.Key = SqlKeyword.MyOqlSet;
            this.Rows = new List<RowData>();
            this.Columns = new ColumnClip[] { };

        }

        public MyOqlSet(RuleBase obj)
            : this()
        {
            this.Entity = obj;
        }

        //public MyOqlSet LoadDictList<T>(IEnumerable<T> ObjectData)
        //    where T : IDictionary
        //{
        //    if (ObjectData == null || (ObjectData.Any() == false))
        //    {
        //        return this;
        //    }
        //    if (this.Count == 0)
        //    {
        //        this.Count = ObjectData.Count();
        //    }

        //    //先初始化 Columns
        //    ObjectData.Any(dict =>
        //    {
        //        var keys = dict.Keys.GetEnumerator();

        //        while (keys.MoveNext())
        //        {
        //            var key = keys.Current.AsString();
        //            if (this.Entity == null)
        //            {
        //                this.InsertColumn(this.Columns.Length, new ConstColumn(key), (rowIndex, v) => string.Empty);
        //                return true;
        //            }
        //            ColumnClip col = this.Entity.GetColumn(key);
        //            if (col.EqualsNull())
        //            {
        //                col = new ConstColumn(key);
        //            }
        //            this.InsertColumn(this.Columns.Length, col, (rowIndex, v) => string.Empty);
        //            return true;
        //        };
        //        return true;
        //    });


        //    //加载数据
        //    ObjectData.All(o =>
        //    {
        //        this.Rows.Add(TranslateValueFromDict(o));
        //        return true;
        //    });

        //    return this;
        //}

        public RuleBase Entity { get; set; }

        /// <summary>
        /// 数据查询，返回 0 ,表示没有取条数，或结果集本身没有数据。
        /// </summary>
        public long Count { get; set; }

        /// <summary>
        /// 定义的数据列。
        /// </summary>
        public ColumnClip[] Columns { get; set; }

        /// <summary>
        /// 行数据。
        /// </summary>
        public List<RowData> Rows { get; set; }

        /// <summary>
        /// 在数据查询时的排序列。
        /// </summary>
        public OrderByClip OrderBy { get; set; }

        /// <summary>
        /// 根据Object设置 Columns 和 Rows 
        /// 如果没有数据，则不会生成 Columns
        /// </summary>
        /// <param name="ObjectData"></param>
        public MyOqlSet Load<T>(IEnumerable<T> ObjectData)
            where T : IReadEntity, new()
        {
            if (ObjectData == null || (ObjectData.Any() == false))
            {
                return this;
            }


            if (this.Count == 0)
            {
                this.Count = ObjectData.Count();
            }
            //如果追回数据，则需要自行处理 Count



            //先初始化 Columns

            ObjectData.Any(e =>
                {
                    e.GetProperties().All(o =>
                        {
                            if (this.Entity == null)
                            {
                                this.InsertColumn(new ConstColumn(o), (rowIndex, v) => string.Empty);
                                return true;
                            }
                            ColumnClip col = this.Entity.GetColumn(o);
                            if (col.EqualsNull())
                            {
                                col = new ConstColumn(o);
                            }
                            this.InsertColumn(col, (rowIndex, v) => string.Empty);
                            return true;
                        });
                    return true;
                });

            //加载数据
            ObjectData.All(o =>
            {
                this.Rows.Add(Translate(o));
                return true;
            });

            return this;

        }

        //private object[] TranslateValueFromDict(IDictionary Entity)
        //{
        //    List<object> listValues = new List<object>();


        //    var cols = Entity.Keys.ToMyList(o => o.AsString());
        //    this.Columns.All(o =>
        //    {
        //        if (cols.Contains(o.Name))
        //        {
        //            listValues.Add(Entity[o.Name]);
        //        }
        //        else if (cols.Contains(o.DbName))
        //        {
        //            listValues.Add(Entity[o.DbName]);
        //        }
        //        else if (cols.Contains(o.GetAlias()))
        //        {
        //            listValues.Add(Entity[o.GetAlias()]);
        //        }
        //        else listValues.Add(string.Empty);

        //        return true;
        //    });
        //    return listValues.ToArray();
        //}

        private RowData Translate<T>(T Entity) where T : IReadEntity
        {
            var listValues = new RowData();

            var cols = Entity.GetProperties();
            this.Columns.All(o =>
            {
                if (cols.Contains(o.Name))
                {
                    listValues[o.Name] = Entity.GetPropertyValue(o.Name);
                    return true;
                }

                if (o.IsSimple())
                {
                    var simple = o as SimpleColumn;

                    if (cols.Contains(simple.DbName))
                    {
                        listValues[simple.Name] = Entity.GetPropertyValue(simple.DbName);
                        return true;
                    }
                }

                listValues[o.Name] = string.Empty;

                return true;
            });
            return listValues;
        }


        //private MyOqlSet LoadDict<T>(List<XmlDictionary<string, object>> ObjectData) where T : class, new()
        //{
        //    var type = typeof(T);

        //    if (ObjectData == null || (ObjectData.Count == 0))
        //    {
        //        return this;
        //    }
        //    if (this.Count == 0)
        //    {
        //        this.Count = ObjectData.Count;
        //    }

        //    ObjectData.First().All(o =>
        //      {
        //          if (this.Entity == null)
        //          {
        //              this.InsertColumn(this.Columns.Length, new ConstColumn(o), (rowIndex, v) => "");
        //              return true;
        //          }
        //          ColumnClip col = this.Entity.GetColumn(o.Key);
        //          if (col.EqualsNull())
        //          {
        //              col = new ConstColumn(o.Key);
        //          }
        //          this.InsertColumn(this.Columns.Length, col, (rowIndex, v) => "");
        //          return true;
        //      });

        //    Func<XmlDictionary<string, object>, object[]> Translate = (dict) =>
        //    {
        //        List<object> listValues = new List<object>();

        //        this.Columns.All(c =>
        //        {
        //            listValues.Add(dict.ContainsKey(c.Name) ? dict[c.Name] : null);
        //            return true;
        //        });
        //        return listValues.ToArray();
        //    };
        //    ObjectData.All(o =>
        //    {
        //        this.Rows.Add(Translate(o));
        //        return true;
        //    });

        //    return this;
        //}

        /// <summary>
        /// 如果Entity 为空. 则生成ConstColumn
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public MyOqlSet Load(DataTable dataTable)
        {
            //IEnumerable<TypeMap> provider = GetEntityTypeMap();

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                ColumnClip col = null;
                if (this.Entity == null)
                {
                    col = new ConstColumn(dataTable.Columns[i].ColumnName);
                }
                else
                {
                    col = this.Entity.GetColumn(dataTable.Columns[i].ColumnName);
                }
                if (col.EqualsNull())
                {
                    col = new ConstColumn(dbo.TranslateDbName(dataTable.Columns[i].ColumnName));
                    //if (provider != null)
                    {
                        //使用公共类型转换 udi@2013年10月20日2时2分
                        //var tyM = provider.FirstDbTypeByCsType(dataTable.Columns[i].DataType);

                        col.DbType = dataTable.Columns[i].DataType.GetDbType(); //tyM;
                    }
                }

                this.InsertColumn(col, (rowIndex, v) => string.Empty);
            }

            Count = Math.Max(dataTable.Rows.Count, Count);

            //List<int> ColIndexs = new List<int>();
            //Columns.All(o =>
            //{
            //    ColIndexs.Add(this.Columns.IndexOf(p =>
            //    {
            //        return p.NameEquals(o);
            //    }));
            //    return true;
            //});


            Func<DataRow, RowData> Translate = (row) =>
            {
                var list = new RowData();

                for (int i = 0; i < Columns.Length; i++)
                {
                    list[Columns[i].Name] = row[Columns[i].Name];
                }

                return list;
            };
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                Rows.Add(Translate(dataTable.Rows[i]));
            }

            if (this.Count == 0)
            {
                this.Count = dataTable.Rows.Count;
            }
            return this;
        }

        //private IEnumerable<TypeMap> GetEntityTypeMap()
        //{
        //    IEnumerable<TypeMap> provider = null;
        //    if (this.Entity != null)
        //    {
        //        var p = this.Entity.GetDbProvider();
        //        if (p != null)
        //        {
        //            provider = p.GetTypeMap();
        //        }
        //    }
        //    return provider;
        //}

        ///// <summary>
        ///// 从字典列表中加载数据
        ///// </summary>
        ///// <param name="dictList"></param>
        ///// <param name="Count">强制使用该条数.</param>
        ///// <returns></returns>
        //public MyOqlSet Load(List<XmlDictionary<string, object>> dictList)
        //{
        //    this.Count = dictList.Count;

        //    if (dictList.Count == 0) return this;


        //    Func<XmlDictionary<string, object>, object[]> Translate = (row) =>
        //    {
        //        List<object> list = new List<object>();

        //        foreach (var item in row)
        //        {
        //            list.Add(item.Value);
        //        }

        //        return list.ToArray();
        //    };


        //    var firstDict = dictList.First().All(o =>
        //   {
        //       ColumnClip col = this.Entity.GetColumn(o.Key);
        //       if (col.EqualsNull())
        //       {
        //           col = new ConstColumn(o.Key);
        //       }

        //       this.InsertColumn(this.Columns.Count(), col, v => "");
        //       return true;
        //   });



        //    for (int i = 0; i < dictList.Count; i++)
        //    {
        //        var dict = dictList[i];


        //        Rows.Add(Translate(dict));
        //    }


        //    return this;
        //}

        //public RowData ToDictModel(int rowIndex)
        //{
        //    if (rowIndex < 0) return null;
        //    if (rowIndex >= this.Data.Count) return null;

        //    var dict = new XmlDictionary<string, object>();
        //    for (int i = 0; i < this.Columns.Length; i++)
        //    {
        //        dict[this.Columns[i].GetAlias()] = this.Data[rowIndex][i];
        //    }

        //    return dict;
        //}



        //public IEnumerable<XmlDictionary<string, object>> ToDictArray()
        //{
        //    for (var r = 0; r < this.Data.Count; r++)
        //    {
        //        var dict = new XmlDictionary<string, object>();

        //        for (int i = 0; i < this.Columns.Length; i++)
        //        {
        //            dict[this.Columns[i].GetAlias()] = this.Rows[r][i];
        //        }
        //        yield return dict;
        //    }
        //}

        //public IEnumerable<XmlDictionary<ColumnClip, object>> ToDictList()
        //{
        //    for (var r = 0; r < this.Rows.Count; r++)
        //    {
        //        var dict = new XmlDictionary<ColumnClip, object>();

        //        for (int i = 0; i < this.Columns.Length; i++)
        //        {
        //            dict[this.Columns[i]] = this.Rows[r][i];
        //        }
        //        yield return dict;
        //    }
        //}

        //public DataTable ToDateTable()
        //{
        //    return ToDateTable(this.Entity.IsDBNull() ? DatabaseType.SqlServer : this.Entity.GetDataBase(), null);
        //}

        /// <summary>
        /// 把MyOqlSet 列别名,做为 DataTable 的列名. 
        /// </summary>
        /// <param name="db">数据库类型</param>
        /// <param name="TranslateValueFunc">行数据自定义转换函数 ,可以为空.</param>
        /// <returns></returns>
        //public DataTable ToDateTable(DatabaseType db, Func<object[], object[]> TranslateValueFunc)
        //{
        //    DataTable dt = new DataTable();
        //    IEnumerable<TypeMap> map = dbo.GetDbProvider(db).GetTypeMap();
        //    this.Columns.All(o =>
        //        {
        //            dt.Columns.Add(o.GetAlias(), map.First(m => m.DbType == o.DbType).CsType);
        //            return true;
        //        });

        //    this.Data.All(o =>
        //        {
        //            if (TranslateValueFunc == null)
        //            {
        //                dt.Rows.Add(o);
        //            }
        //            else
        //            {
        //                dt.Rows.Add(TranslateValueFunc(o));
        //            }
        //            return true;
        //        });
        //    return dt;
        //}


        //public List<T> ToEntityList<T>(Func<T> NewModelFunc)
        //{
        //    return ToDictArray().Select(o => dbo.DictionaryToModel(o, NewModelFunc)).ToList();
        //}


        /// <summary>
        /// 给 MyOqlSet 添加一列。 
        /// </summary>
        /// <param name="Index">添加列索引。如果为负数，则在最后添加。</param>
        /// <param name="Column">添加 Column</param>
        /// <param name="EachValueFunc">每一列的值。</param>
        public void InsertColumn(ColumnClip Column, Func<int, RowData, object> EachValueFunc)
        {
            if (this.Columns.Count(o => o.GetAlias() == Column.GetAlias()) > 0) return;

            this.Columns = this.Columns.AddOne(Column).ToArray();

            for (int i = 0; i < this.Rows.Count; i++)
            {
                object val = null;
                if (EachValueFunc != null)
                {
                    val = EachValueFunc(i, this.Rows[i]);
                }

                this.Rows[i][Column.Name] = val;
            }
        }


        public void InsertRow<T>(int index, T Entity) where T : IReadEntity
        {
            if (index < 0)
            {
                this.Rows.Add(Translate(Entity));
            }
            else this.Rows.Insert(index, Translate(Entity));
        }


        /// <summary>
        /// 批量插入.
        /// </summary>
        /// <example>
        /// <code>
        /// var insertRows = new MyOqlBulkSet&lt;ProjectChangeSetRule&gt;(dbr.ProjectChangeSet);
        /// insertRows.SetColumns(o => new ColumnClip[] { o.ChangeType, o.Trait, o.Key, o.Value, o.TaskWBS, o.VerID });
        /// insertRows.Rows = Translate(this).ToArray();
        /// insertRows.SaveToServer(null);
        /// </code>
        /// </example>
        /// <param name="Tran"></param>
        /// <returns></returns>
        public int InsertToServer(DbTransaction Tran)
        {
            if (this.Columns.Length == 0) return 0;
            if (this.Rows.Count == 0) return 0;


            MyContextClip insert = MyContextClip.CreateByKey(SqlKeyword.BulkInsert);

            insert.CurrentRule = this.Entity;

            //批量插入时使用.
            insert.Dna.Add(this);

            if (dbo.Event.OnBulkInserting(insert) == false) return 0;

            var myCmd = insert.UseTransaction(Tran).ToCommand();


            var retVal = 0;

            if (dbo.Event.OnExecute(insert) == false) return 0;

            dbo.Open<int>(myCmd, insert, () =>
            {
                try
                {
                    return myCmd.Command.ExecuteScalar().AsInt();
                }
                catch (Exception ee)
                {
                    ee.ThrowError(myCmd);
                    throw;
                }
            });

            dbo.Event.OnCreated(insert);
            return retVal;
        }



        /// <summary>
        /// 批量更新
        /// </summary>
        /// <remarks>
        /// 如果传入事务, 则使用事务里的连接 , 否则创建新的连接.
        /// </remarks>
        /// <param name="Tran"></param>
        /// <returns></returns>
        public int UpdateToServer(DbTransaction Tran)
        {
            if (this.Columns.Length == 0) return 0;
            if (this.Rows.Count == 0) return 0;

            Func<XmlDictionary<ColumnClip, object>> GetFirstModel = () =>
                {
                    var dict = new XmlDictionary<ColumnClip, object>();
                    var row = this.Rows[0];
                    for (int i = 0; i < this.Columns.Length; i++)
                    {
                        var col = this.Columns[i];

                        dict[col] = row[col.Name];
                    }

                    return dict;

                };

            MyContextClip update = MyContextClip.CreateByKey(SqlKeyword.Update);

            update.CurrentRule = this.Entity;
            update.Dna.Add(new ModelClip(this.Entity, GetFirstModel()));


            //权限用.
            update.Dna.Add(this);

            if (dbo.Event.OnBulkUpdating(update) == false) return 0;

            var myCmd = update.UseTransaction(Tran).ToCommand();

            if (dbo.Event.OnExecute(update) == false) return 0;

            var retVal = 0;
            dbo.Open<int>(myCmd, update, () =>
            {
                try
                {
                    //每个参数在 Rows 里的顺序,比如:第一个参数 @P11 对应的是 Pid , 但是它在 Columns 里的顺序是 2
                    //var paraDict = new List<int>();
                    //for (int j = 0; j < myCmd.Command.Parameters.Count; j++)
                    //{
                    //    paraDict.Add(this.Columns.IndexOf(o => o.NameEquals(update.ParameterColumn[j].Column.Name)));
                    //}


                    for (int i = 0; i < this.Rows.Count; i++)
                    {
                        var row = this.Rows[i];

                        for (int j = 0; j < myCmd.Command.Parameters.Count; j++)
                        {
                            myCmd.Command.Parameters[j].Value = row[update.ParameterColumn[j].Column.Name];
                        }

                        retVal += myCmd.Command.ExecuteScalar().AsInt();
                    }

                    return retVal;
                }
                catch (Exception ee)
                {
                    ee.ThrowError(myCmd);
                    throw;
                }
            });

            dbo.Event.OnUpdated(update);
            return retVal;

        }

        public override object Clone()
        {
            MyOqlSet set = new MyOqlSet(this.Entity.Clone() as RuleBase);
            set.Columns = Enumerable.ToArray(Enumerable.Select(this.Columns, o => o.Clone() as ColumnClip));
            set.Rows.AddRange(this.Rows.Select(o => o.DeepClone()).ToArray());

            if (this.OrderBy != null)
            {
                set.OrderBy = this.OrderBy.Clone() as OrderByClip;
            }
            //set.Rows.AddRange(this.Rows.Select(o =>
            //{
            //    var ary = new object[o.Length];
            //    Array.Copy(o, ary, o.Length);
            //    return ary;
            //}).ToArray());
            set.Count = this.Count;

            return set;
        }

        ///// <summary>
        ///// 根据行索引和列名进行查找.
        ///// </summary>
        ///// <param name="RowIndex"></param>
        ///// <param name="ColumnAlias"></param>
        ///// <returns></returns>
        //public object GetValue(int RowIndex, string ColumnAlias)
        //{
        //    return this.Data[RowIndex][GetColumnIndex(ColumnAlias)];
        //}


        /// <summary>
        /// 优先根据列别名查找列索引，如果列别名找不到，则使用列名进行查找。 (包含表名.)
        /// </summary>
        /// <param name="ColumnAlias"></param>
        /// <returns></returns>
        public int GetColumnIndex(string ColumnAlias)
        {
            var ret= this.Columns.IndexOf(o => o.GetAlias() == ColumnAlias);
            if (ret < 0)
            {
                ret = this.Columns.IndexOf(o => o.GetFullDbName().ToString() == ColumnAlias);
            }
            if (ret < 0)
            {
                ret = this.Columns.IndexOf(o => o.GetFullDbName().Column == ColumnAlias);
            }
            if (ret < 0)
            {
                ret = this.Columns.IndexOf(o => o.GetFullName().ToString() == ColumnAlias);
            }
           
            return ret;
        }

        /// <summary>
        /// 通过Id进行查找.
        /// </summary>
        /// <param name="Id">Id必须有值,且不能包含#</param>
        /// <returns></returns>
        //public XmlDictionary<ColumnClip, object> FindById(string Id)
        //{
        //    if (Id.HasValue() == false) return null;
        //    if (Id.StartsWith("#")) return null;

        //    var data = Data.FirstOrDefault(o => o[Columns.IndexOf(c => c.NameEquals(this.Entity.GetIdKey()))].AsString() == Id);

        //    if (data == null) return null;

        //    XmlDictionary<ColumnClip, object> dict = new XmlDictionary<ColumnClip, object>();


        //    for (int i = 0; i < Columns.Length; i++)
        //    {
        //        dict[Columns[i]] = data[i];
        //    }
        //    return dict;
        //}

        public string[] GetProperties()
        {
            return this.Columns.Select(o => o.GetAlias()).ToArray();
        }
    }


    [Serializable]
    public class MyOqlSet<T> : MyOqlSet
        where T : RuleBase
    {
        public MyOqlSet(RuleBase obj)
            : base(obj)
        {
        }
    }

    public class MyOqlSetJsonNetConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType.FullName == "MyOql.MyOqlSet") return true;
            if (objectType.IsSubclassOf(typeof(MyOqlSet))) return true;

            return false;
        }

        private T[] readArray<T>(JsonReader reader, Func<JsonReader, T> func = null)
        {
            List<T> list = new List<T>();
            if (reader.TokenType != JsonToken.StartArray) return list.ToArray();
            while (true)
            {
                if (reader.Read() == false) break;
                if (reader.TokenType == JsonToken.EndArray) break;
                if (func != null)
                {
                    list.Add(func(reader));
                }
                else
                {
                    list.Add(ValueProc.As<T>(reader.Value));
                }
            }

            return list.ToArray();
        }

        private RowData readDict(JsonReader reader)
        {
            var list = new RowData();
            if (reader.TokenType != JsonToken.StartObject) return list;
            var lastKey = string.Empty;
            while (true)
            {
                if (reader.Read() == false) break;
                if (reader.TokenType == JsonToken.EndObject) break;
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    lastKey = reader.Value.AsString();
                }
                else
                {
                    list[lastKey] = reader.Value;
                }
            }

            return list;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            MyOqlSet set = new MyOqlSet();
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    if (reader.Value.AsString() == "Entity")
                    {
                        set.Entity = dbo.Event.Idbr.GetMyOqlEntity(reader.ReadAsString());
                    }
                    else if (reader.Value.AsString() == "Count")
                    {
                        reader.Read();
                        set.Count = reader.Value.AsLong();
                    }
                    else if (reader.Value.AsString() == "Columns")
                    {
                        reader.Read();  //到 StartArray

                        set.Columns = Enumerable.Select(readArray<string>(reader), o => new ConstColumn(o)).ToArray();
                    }
                    else if (reader.Value.AsString() == "Rows")
                    {
                        reader.Read();

                        set.Rows = readArray<RowData>(reader, r => readDict(r)).ToList();
                    }
                }
            }


            return set;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var set = value as MyOqlSet;

            writer.WriteStartObject();

            if (set.Entity.IsDBNull() == false)
            {
                writer.WritePropertyName("Entity");
                writer.WriteValue(set.Entity.GetDbName());
            }

            writer.WritePropertyName("Count");
            writer.WriteValue(set.Count);

            writer.WritePropertyName("Columns");
            writer.WriteStartArray();
            foreach (var item in set.Columns)
            {
                writer.WriteValue(item.GetAlias());
            }
            writer.WriteEndArray();

            writer.WritePropertyName("Rows");
            writer.WriteRawValue(set.Rows.ToJson());

            writer.WriteEndObject();
        }
    }

    public class RowData : XmlDictionary<string, object>
    {
        public override object Clone()
        {
            var ret = new RowData();
            this.Keys.All(o =>
            {
                var itemValue = this[o];
                if (itemValue.IsDBNull())
                {
                    ret[o] = itemValue;
                    return true;
                }

                var val = itemValue as ValueType;
                if (val != null)
                {
                    ret[o] = itemValue;
                    return true;
                }

                var str = itemValue as string;
                if (str != null)
                {
                    ret[o] = itemValue;
                    return true;
                }


                ret[o] = itemValue.MustDeepClone();
                return true;
            });
            return ret;
        }
    }

}
