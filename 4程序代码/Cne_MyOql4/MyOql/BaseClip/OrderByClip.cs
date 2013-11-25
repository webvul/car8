using System;
using System.Xml.Serialization;
using System.Xml;
using MyCmn;
using System.Collections;
using System.Collections.Generic;

namespace MyOql
{
    /// <summary>
    /// 排序子句
    /// </summary>
    [Serializable]
    public class OrderByClip : SqlClipBase, IXmlSerializable, ICloneable
    {
        /// <summary>
        /// 默认值是升序（true)，Sql 的默认值（不写）也是升序
        /// </summary>
        public bool IsAsc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>在真正解析成SQL的时候，要从DNA中查找到合适的列。</remarks>
        public string PostOrderName { get; set; }

        /// <summary>
        /// 从页面上看，要按 Name 判断相等 
        /// </summary>
        public ColumnClip Order { get; set; }

        public OrderByClip Next { get; set; }

        public OrderByClip(ColumnClip OrderColumn)
            : this(OrderColumn, true)
        {
        }

        public OrderByClip(ColumnClip OrderColumn, bool isAsc)
        {
            this.Key = SqlKeyword.OrderBy;
            IsAsc = isAsc;
            this.Order = OrderColumn.Clone() as ColumnClip;
        }

        /// <summary>
        /// 如果以 # 开头，则Name会去掉第一个#
        /// </summary>
        /// <param name="OrderExpression">页面上定义的列别名。</param>
        public OrderByClip(string OrderExpression)
        {
            this.Key = SqlKeyword.OrderBy;
            IsAsc = true;
            this.PostOrderName = OrderExpression;
            this.Order = new ConstColumn(null);
            //if (OrderExpression.HasValue())
            //{
            //    this.Order = new ConstColumn(OrderExpression.StartsWith("#") ? OrderExpression : ("#" + OrderExpression));
            //}
            //else
            //{
            //    this.Order = new ConstColumn(null);
            //}
        }



        public static OrderByClip operator +(OrderByClip left, OrderByClip right)
        {
            return left & right;
        }

        public static OrderByClip operator &(OrderByClip left, OrderByClip right)
        {
            if (right.HasData() == false) return left;
            if (left.HasData() == false) return right;

            OrderByClip whe = GetLast(left);
            whe.Next = right;

            return left;
        }

        public List<OrderByClip> ToList()
        {
            //本函数中不能使用HasData ，因为HasData可能会使用此函数，造成死循环。
            List<OrderByClip> list = new List<OrderByClip>();
            OrderByClip whe = this;
            while (whe != null)
            {
                if (whe.Order.EqualsNull() == false && whe.Order.Name.HasValue())
                {
                    list.Add(whe);
                }
                whe = whe.Next;
            }

            return list;
        }


        public static OrderByClip GetLast(OrderByClip where)
        {
            OrderByClip whe = where;
            while (whe.Next.HasData())
            {
                whe = whe.Next;
            }
            return whe;
        }

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

            reader.ReadStartElement("Order");
            this.Order = SerializerHelper.XmlToObj<ColumnClip>(typeof(ColumnClip), reader.ReadOuterXml());

            if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement();

            reader.ReadStartElement("IsAsc");
            this.IsAsc = reader.ReadContentAsString().AsBool();
            if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement();


            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Next")
            {
                reader.ReadStartElement("Next");
                if (reader.IsEmptyElement == false)
                {
                    this.Next = SerializerHelper.XmlToObj<OrderByClip>(typeof(OrderByClip), reader.ReadOuterXml());
                }
                if (reader.NodeType == XmlNodeType.EndElement) reader.ReadEndElement();
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Order");
            writer.WriteRaw(SerializerHelper.ObjToXml(this.Order, o => o.DocumentElement.OuterXml));
            writer.WriteEndElement();

            writer.WriteStartElement("IsAsc");
            writer.WriteString(this.IsAsc.AsString());
            writer.WriteEndElement();

            if (this.Next != null)
            {
                writer.WriteStartElement("Next");
                writer.WriteRaw(SerializerHelper.ObjToXml(this.Next, o => o.DocumentElement.OuterXml));
                writer.WriteEndElement();
            }
            //OrderByClip order = this;
            //bool IsFirst = true;
            //while (order != null)
            //{
            //    if (IsFirst == false)
            //    {
            //        writer.WriteStartElement("Next");
            //    }



            //    if (IsFirst == false) writer.WriteEndElement();
            //    if (IsFirst) IsFirst = false;
            //    order = order.Next;
            //}
        }

        public override object Clone()
        {
            if (this.HasData() == false) return null;

            var orderBy = new OrderByClip(this.Order);
            orderBy.IsAsc = this.IsAsc;


            if (this.Next.HasData())
            {
                orderBy.Next = this.Next.Clone() as OrderByClip;
            }

            return orderBy;
        }
    }
}
