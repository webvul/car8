using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using MyCmn;
using System.Web.Compilation;
using System.Collections;
using System.Linq.Expressions;
using System.Linq;

namespace MyOql
{
    /// <summary>
    /// Where 条件，同时也是一个表达示树，可用于Inser,Update 的Model表示。
    /// Where 是一个链式+树状的数据结构.
    /// </summary>
    [Serializable]
    public class WhereClip : SqlClipBase, IXmlSerializable, IReadEntity, ICloneable,IOperator
    {
        //[DataMember]
        /// <summary>
        /// 操作符
        /// </summary>
        public SqlOperator Operator { get; set; }
        //[DataMember]
        /// <summary>
        /// 列
        /// </summary>
        public ColumnClip Query { get; set; }

        public WhereValueEnum ValueType { get; set; }

        private object _Value { get; set; }

        /// <summary>
        /// Where表达式的条件值，可能有五种类型： 值类型（如字符串，数字），值类型数组(In,NotIn,Between)，列,复合列，子查询（In，NotIn）。
        /// </summary>
        public object Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;

                if (object.Equals(_Value, null))
                {
                    ValueType = WhereValueEnum.Value;
                    return;
                }

                var col = _Value as SqlClipBase;

                if (col != null)
                {
                    if (col.IsColumn())
                    {
                        ValueType = WhereValueEnum.Column;
                    }
                    else if (col.Key == SqlKeyword.Select)
                    {
                        ValueType = WhereValueEnum.SubQuery;
                    }

                    return;
                }
                else
                {
                    var ienm = _Value as IEnumerable;

                    if (ienm != null)
                    {
                        //如果是 char[] , List<char>  就是 值类型。
                        Type vt = ienm.GetType();

                        if (vt.FullName == "System.String")
                        {
                            ValueType = WhereValueEnum.Value;
                        }
                        else if (vt.HasElementType && vt.GetElementType().FullName == "System.Char")
                        {
                            ValueType = WhereValueEnum.Value;
                        }
                        else if (vt.IsGenericType && vt.GetGenericArguments()[0].FullName == "System.Char")
                        {
                            ValueType = WhereValueEnum.Value;
                        }
                        else
                        {
                            ValueType = WhereValueEnum.ValueArray;
                        }

                    }
                    else ValueType = WhereValueEnum.Value;

                    return;
                }
            }
        }

        //[DataMember]
        /// <summary>
        /// 连接符,用于连接 Next .
        /// </summary>
        public SqlOperator Linker { get; set; }
        //[DataMember]
        /// <summary>
        /// 下一个节点
        /// </summary>
        public WhereClip Next { get; set; }

        /// <summary>
        /// Child 与 (Query,Value,Linker)互斥，说明此Where是一个集合体。
        /// </summary>
        //[DataMember]
        public WhereClip Child { get; set; }

        public WhereClip()
        {
            this.Key = SqlKeyword.Where;
        }

        /// <summary>
        /// 重制构造
        /// </summary>
        /// <param name="where"></param>
        public WhereClip(WhereClip where)
        {
            this.Key = where.Key;
            this.Linker = where.Linker;
            this.Next = where.Next;
            this.Operator = where.Operator;
            this.Query = where.Query;
            this.Value = where.Value;
            this.Child = where.Child;
        }

        /// <summary>
        /// 用逻辑与 连接两个Where
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static WhereClip operator &(WhereClip left, WhereClip right)
        {
            if (right.IsNull()) return left;
            if (left.IsNull()) return right;

            var clone = left.Clone() as WhereClip;
            WhereClip whe = clone.GetLast();
            whe.Linker = SqlOperator.And;
            whe.Next = right.Clone() as WhereClip;

            return clone;
        }



        /// <summary>
        /// 用逻辑或 连接两个 Where
        /// </summary>
        /// <remarks> 把两边都放到 子组里, 很严谨, 但可能会多出括号. </remarks>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static WhereClip operator |(WhereClip left, WhereClip right)
        {
            if (right.IsNull()) return left;
            if (left.IsNull()) return right;

            WhereClip leftWrap = new WhereClip();
            leftWrap.Child = left;

            WhereClip rightWrap = new WhereClip();
            rightWrap.Child = right;

            leftWrap.Linker = SqlOperator.Or;
            leftWrap.Next = rightWrap;

            WhereClip where = new WhereClip();
            where.Child = leftWrap;

            return where;
        }

        /// <summary>
        /// MyOql内部使用.得到最后一个 Where
        /// </summary>
        /// <returns></returns>
        public WhereClip GetLast()
        {
            WhereClip whe = this;
            while (whe.Next != null)
            {
                whe = whe.Next;
            }
            return whe;
        }

        /// <summary>
        /// 从Where条件中得到 行的唯一表示.主要用于日志记录。
        /// </summary>
        /// <remarks>
        /// 原则上讲，所有的表都要有唯一列（或自增，或单主键，或唯一约束列）。
        /// 该方法还兼容了非唯一主键,没有自增列的行.
        ///     例如三个主键 A,B,C 列,值分别为 "hello","oql","!" 则返回  #5,3.hellooql!
        /// 如果查询条件里没有主键值,则返回null
        /// 
        /// 注意：缓存操作时，需要唯一列，按 唯一列原则进行。
        /// </remarks>
        /// <param name="Rule"></param>
        /// <returns>返回Id值表示法。当有单唯一列时，Id值表示单唯一列的值，否则表示为：#值1长度,值2长度.值1，值2，值3</returns>
        public string GetIdValue(RuleBase Rule)
        {
            var id = Rule.GetIdKey();
            if (dbo.EqualsNull(id) == false)
            {
                return QueryColumnValue(id.Name);
            }
            else
            {
                //var dict = new StringDict();

                var pks = Rule.GetPrimaryKeys();
                StringLinker sl = new StringLinker();
                List<string> values = new List<string>();
                for (var i = 0; i < pks.Length; i++)
                {
                    var col = pks[i];
                    var val = QueryColumnValue(col.Name);
                    //如果其中一个主键值为null , 则返回 null 
                    if (val.HasValue() == false) return null;

                    if (i < pks.Length - 1)
                    {
                        sl += val.Length.AsString() + ",";
                    }
                    else
                    {
                        sl += "." + val;
                    }

                    values.Add(val);
                }


                foreach (var item in values)
                {
                    sl += item;
                }

                if (sl.Length == 0) return string.Empty;
                else return "#" + sl.AsString();
            }
        }


        /// <summary>
        /// 在Where里查某一列的值.
        /// </summary>
        /// <param name="Column"></param>
        /// <returns></returns>
        public string QueryColumnValue(string Column)
        {
            WhereClip whe = this;
            if (dbo.EqualsNull(whe.Query) == false)
            {
                if (whe.Query.Name == Column && whe.Operator == SqlOperator.Equal)
                    return whe.Value.AsString();
            }
            else if (whe.Child != null)
            {
                var retVal = whe.Child.QueryColumnValue(Column);
                if (retVal != null) return retVal;
            }

            if (whe.Next != null)
            {
                var retVal = whe.Next.QueryColumnValue(Column);
                if (retVal != null) return retVal;
            }
            return null;

        }


        /// <summary>
        /// 遍历 非空数据的 WhereClip
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        public bool Recusion(Func<WhereClip, bool> Func)
        {
            var retVal = true;
            //改变 whe 不会改变 this 变量。  udi test。
            var whe = this;
            while (whe != null)
            {
                if (whe.Query.EqualsNull())
                {
                    var subWhere = whe.Child;

                    if (subWhere != null)
                    {
                        retVal &= subWhere.Recusion(Func);
                    }
                }
                else
                {
                    retVal &= Func(whe);
                }

                if (retVal == false) return false;
                whe = whe.Next;
            }

            return retVal;
        }

        //public Func<bool> WhereLambda()
        //{
        //    return null;
        //}


        ///// <summary>
        ///// 构造一个Where计算表达式。
        ///// 形如：
        ///// <code>
        ///// System.Linq.Expressions.Expression&lt;Func&lt;bool&gt;&gt; exp ;
        ///// bool ret = exp(col =&gt; new object )
        ///// where.Recusion(w=> ()=> { return true ;} ) ; 将生成：
        ///// </code>
        ///// </summary>
        ///// <returns></returns>
        //public Expression<Func<Func<WhereClip, bool>, WhereClip[], bool>> RecusionExpressionFunc(Action<WhereClip> GatherWhere)
        //{
        //    var eachWhereFunc = Expression.Parameter(typeof(Func<WhereClip, bool>), "eachWhereFunc");
        //    var wheres = Expression.Parameter(typeof(WhereClip[]), "wheres");

        //    var curIndex = 0;

        //    //生成表达式
        //    Func<WhereClip, Expression> _func = null;
        //    Func<WhereClip, Expression> func = whe =>
        //        {
        //            Expression exp = null;
        //            if (whe != null)
        //            {
        //                if (whe.Query.EqualsNull())
        //                {
        //                    var subWhere = whe.Child;

        //                    if (subWhere != null)
        //                    {
        //                        exp = _func(subWhere);
        //                    }
        //                }
        //                else
        //                {
        //                    GatherWhere(whe);
        //                    exp = Expression.Invoke(eachWhereFunc, Expression.ArrayIndex(wheres, Expression.Constant(curIndex++)));

        //                }

        //                if (whe.Linker.HasValue())
        //                {
        //                    if (whe.Linker == SqlOperator.And)
        //                    {
        //                        return Expression.AndAlso(exp, _func(whe.Next));
        //                    }
        //                    else if (whe.Linker == SqlOperator.Or)
        //                    {
        //                        return Expression.OrElse(exp, _func(whe.Next));
        //                    }
        //                }
        //                else return exp;
        //            }
        //            return exp;
        //        };
        //    _func = func;

        //    var expBody = func(this);

        //    Expression<Func<Func<WhereClip, bool>, WhereClip[], bool>> retVal = Expression.Lambda<Func<Func<WhereClip, bool>, WhereClip[], bool>>(
        //        expBody, eachWhereFunc, wheres);

        //    return retVal;
        //}

        public void SetKey(SqlKeyword Key)
        {
            this.Key = Key;
        }
        //public Dictionary<string, object> ToDictionary()
        //{
        //    Dictionary<string, object> dict = new Dictionary<string, object>();
        //    WhereClip set = this;
        //    while (set != null)
        //    {
        //        if (object.Equals(set.Query, null) == false)
        //        {
        //            dict[set.Query.Name] = set.Value;
        //            set = set.Next;
        //        }
        //        else if (set.Child != null)
        //        {
        //            var subD = set.Child.ToDictionary();
        //            foreach (var item in subD)
        //            {
        //                dict[item.Key] = item.Value;
        //            }
        //        }
        //        else break;
        //    }
        //    return dict;
        //}


        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }


        /// <summary>
        /// 反序列化Xml
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement || !reader.Read())
            {
                return;
            }

            reader.ReadStartElement("Query");
            this.Query = SerializerHelper.XmlToObj<ColumnClip>(typeof(ColumnClip), reader.ReadString());// (this.Query, o => o.DocumentElement.OuterXml));//.Base64_Serial());
            if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement();

            reader.ReadStartElement("Operator");
            this.Operator = reader.ReadString().ToEnum<SqlOperator>();
            if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement();

            reader.MoveToAttribute("Type");
            Type type = BuildManager.GetType(reader.ReadContentAsString(), false);
            reader.ReadStartElement("Value");
            this.Value = reader.ReadString().Base64_UnSerial(type);
            if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement();

            reader.ReadStartElement("Linker");
            this.Linker = reader.ReadString().ToEnum<SqlOperator>();
            if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement();

            reader.ReadStartElement("Next");
            if (reader.IsEmptyElement == false)
                this.Next = reader.ReadString().Base64_UnSerial<WhereClip>();//SerializerHelper.ObjToXml(this.Next, o => o.DocumentElement.OuterXml));
            if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement();

            reader.ReadStartElement("Child");
            if (reader.IsEmptyElement == false)
                this.Child = reader.ReadString().Base64_UnSerial<WhereClip>();//SerializerHelper.ObjToXml(this.Child, o => o.DocumentElement.OuterXml));
            if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement();

        }

        /// <summary>
        /// 序列化Xml
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Query");
            writer.WriteRaw(SerializerHelper.ObjToXml(this.Query, o => o.DocumentElement.OuterXml));//.Base64_Serial());
            writer.WriteEndElement();

            writer.WriteStartElement("Operator");
            writer.WriteString(this.Operator.AsString());
            writer.WriteEndElement();

            writer.WriteStartElement("Value");
            writer.WriteAttributeString("Type", this.Value.GetType().AssemblyQualifiedName);
            writer.WriteString(this.Value.Base64_Serial());
            writer.WriteEndElement();

            writer.WriteStartElement("Linker");
            writer.WriteString(this.Linker.AsString());
            writer.WriteEndElement();

            writer.WriteStartElement("Next");
            writer.WriteString(this.Next.Base64_Serial());
            writer.WriteEndElement();


            writer.WriteStartElement("Child");
            writer.WriteString(this.Child.Base64_Serial());
            writer.WriteEndElement();
        }

        public override object Clone()
        {
            var where = new WhereClip();
            if (this.Child != null)
            {
                where.Child = this.Child.Clone() as WhereClip;
            }

            where.Key = this.Key;
            where.Linker = this.Linker;
            where.Next = this.Next;
            where.Operator = this.Operator;

            if (this.Query.EqualsNull() == false)
            {
                where.Query = this.Query.Clone() as ColumnClip;
            }

            where.ValueType = this.ValueType;

            if (object.Equals(this.Value, null) == false)
            {
                switch (where.ValueType)
                {
                    case WhereValueEnum.Value:
                        where.Value = this.Value;
                        break;
                    case WhereValueEnum.ValueArray:
                        where.Value = this.Value.DeepClone();
                        break;
                    case WhereValueEnum.Column:
                        var col = this.Value as ColumnClip;
                        if (col.EqualsNull() == false)
                        {
                            where.Value = col.Clone();
                        }
                        break;
                    case WhereValueEnum.SubQuery:
                        var subQuery = this.Value as SelectClip;
                        if (subQuery != null)
                        {
                            where.Value = subQuery.Clone() as SelectClip;
                        }
                        break;
                    default:
                        break;
                }
            }

            return where;
        }

        /// <summary>
        /// 判断Where条件中是否存在 In NotIn（Select 子句）子句
        /// </summary>
        /// <returns></returns>
        public bool HasInQuery()
        {
            return !this.Recusion(w =>
                    {
                        if (w.Operator.IsIn(SqlOperator.In, SqlOperator.NotIn))
                        {
                            var inSelct = w.Value as SelectClip;
                            if (inSelct != null)
                            {
                                return false;
                            }
                        }
                        return true;
                    });

        }

        public bool RecusionQuery(Func<SelectClip, bool> Func)
        {
            var retVal = true;
            this.Recusion(w =>
                    {
                        if (w.Operator.IsIn(SqlOperator.In, SqlOperator.NotIn))
                        {
                            var inSelct = w.Value as SelectClip;
                            if (inSelct != null)
                            {
                                if (inSelct.HasSubQuery() == false)
                                {
                                    retVal &= Func(inSelct);
                                }

                                if (retVal == false) return false;

                                retVal &= inSelct.Recusion(Func);

                                if (retVal == false) return false;
                            }
                        }
                        return true;
                    });

            return retVal;
        }

        public string[] GetProperties()
        {
            List<string> list = new List<string>();
            this.Recusion(o =>
            {
                list.Add(o.Query.GetAlias());
                return true;
            });

            return list.ToArray();
        }

        public object GetPropertyValue(string PropertyName)
        {
            object val = null;
            this.Recusion(o =>
                {
                    if (o.Query.NameEquals(PropertyName, true))
                    {
                        val = o.Value;
                        return false;
                    }
                    return true;
                });

            return val;
        }
    }
}
