using MyCmn;
using MyOql;

namespace DbEnt
{
    public static class EntityExtend
    {
        public static ColumnClip ClrFContains(this ColumnClip Col, int Index)
        {
            return ConstColumn.CreateSystemColumn("dbo.CLRFContains(" + Col.GetFullName() + "," + Index + ")");
        }
    }
}
