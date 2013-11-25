
using System.Collections.Generic;
namespace MyOql
{
    /// <summary>
    /// 常量表,用处不大.
    /// </summary>
    public class ConstTable : RuleBase
    {
        public ConstTable() : base(string.Empty) { }

        public override string GetDbName()
        {
            return string.Empty;
        }

        public override SimpleColumn[] GetColumns()
        {
            return new SimpleColumn[] { };
        }

        public override SimpleColumn[] GetPrimaryKeys()
        {
            return new SimpleColumn[] { };
        }

        public override SimpleColumn[] GetComputeKeys()
        {
            return new SimpleColumn[] { };
        }

        public override SimpleColumn GetAutoIncreKey()
        {
            return null;
        }

        public override SimpleColumn GetUniqueKey()
        {
            return null;
        }

        public override object Clone()
        {
            var tab = new ConstTable();
            tab._Config_ = this._Config_;
            tab.Name = this.Name;
            //.All(o =>
            //    {
            //        tab._Extend[o.Key] = o.Value;
            //        return true;
            //    });

            tab.Name = this.Name;
            return tab;
        }
    }
}
