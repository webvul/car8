using System.Data.Common;

namespace MyOql
{

    /// <summary>
    /// 绝对无用.自定义实例化  ContextClipBase。
    /// </summary>
    public sealed class MySqlClipBase : SqlClipBase
    {
        public MySqlClipBase() { }
        public MySqlClipBase(SqlKeyword Key)
        {
            this.Key = Key;
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
