//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace MyOql
//{
//    /// <summary>
//    /// 描述列的复合表达式，比如： ( Id + 1 ) * 2
//    /// 它不能单独存在， 它依赖于 ColumnClip ， 存在于 ColumnClip 的 Extend 中。
//    /// </summary>
//    /// <remarks>
//    /// 和 WhereClip 不是一种表达式树。它更简单。更规范。 更靠近 ExpressionTree。
//    /// 当功能稳定后，把WhereClip 重构为ExpressoinTree结构。
//    /// </remarks>
//    public class ComplexExpresstion : SqlClipBase
//    {
//        public ComplexExpresstion()
//        {
//            this.Key = SqlKeyword.Complex;
//        }

//        /// <summary>
//        /// 是否是叶子。
//        /// </summary>
//        /// <returns></returns>
//        public bool IsLeave()
//        {
//            return this.Operator == 0;
//        }

//        //[DataMember]
//        /// <summary>
//        /// 前一项。
//        /// </summary>
//        public ComplexExpresstion Query { get; set; }
//         //[DataMember]
//        /// <summary>
//        /// 操作符，用来连接 Query 和 Value
//        /// </summary>
//        public SqlOperator Operator { get; set; }
//       //[DataMember]
//        /// <summary>
//        /// 值
//        /// </summary>
//        public ComplexExpresstion Value { get; set; }

//        /// <summary>
//        /// 表示叶子值。 和 Query,Operator,Value 为互斥。
//        /// </summary>
//        public ColumnClip Modal { get; set; }
//    }
//}
