using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;

namespace MyOql
{

    /// <summary>
    /// 模型, 表示插入或更新的模型.
    /// </summary>
    [Serializable]
    public class ModelClip : SqlClipBase
    {
        protected RuleBase CurrentRule { get; set; }

        private ModelClip()
        {
            this.Key = SqlKeyword.Model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Rule">如果为空,不报错,忽略,但部分函数报错. 如:GetIdValue</param>
        /// <param name="OriginalModel"></param>
        public ModelClip(RuleBase Rule, IModel OriginalModel)
            : this()
        {
            this.OriginalModel = OriginalModel;
            this.CurrentRule = Rule;
            this.Model = dbo.ModelToDictionary(Rule, OriginalModel);//.ToXmlDictionary(o => o.Key, o => o.Value);
        }
        /// <summary>
        /// 原始数据模型.
        /// </summary>
        public IModel OriginalModel { get; set; }

        /// <summary>
        /// 对原始数据模型的整理.
        /// </summary>
        /// <remarks>ColumnClip 只能是两种：  SimpleColumn 和  ConstColumn </remarks>
        public XmlDictionary<ColumnClip, object> Model { get; private set; }

        /// <summary>
        /// 逻辑同WhereClip.GetIdValue ，返回该Model 的Id值。
        /// 依赖CurrentRule
        /// </summary>
        /// <returns></returns>
        public string GetIdValue()
        {
            var id = this.CurrentRule.GetIdKey();
            if (dbo.EqualsNull(id) == false)
            {
                var findValue = Model.FirstOrDefault(o => o.Key.NameEquals(id));
                if (findValue.HasValue())
                {
                    return findValue.Value.AsString();
                }
            }

            //var dict = new StringDict();

            var pks = this.CurrentRule.GetPrimaryKeys();
            StringLinker sl = new StringLinker();
            List<string> values = new List<string>();
            for (var i = 0; i < pks.Length; i++)
            {
                var col = pks[i];
                var val = Model.FirstOrDefault(o => o.Key.NameEquals(id)).Value.AsString();

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

        public override object Clone()
        {
            return this.MustDeepClone();
        }
    }
}
