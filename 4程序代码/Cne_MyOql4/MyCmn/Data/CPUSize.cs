using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using System.Configuration;


namespace MyCmn
{
    public enum CPUSizeEnum
    {
        Bytes = 1,
        KB,
        MB,
        GB,
        TB
    }


    public struct CPUSize
    {
        public double Value { get; set; }
        public CPUSizeEnum Unit { get; set; }

        public override string ToString()
        {
            return string.Format(@"{0} {1}", Value.ToString("0.##"), Unit.ToString());
        }

        public string ToFixedString()
        {
            if (Unit == CPUSizeEnum.TB || Value <= 1024)
            {
                return ToString();
            }

            var cur = this;

            if (cur.Value > 1024)
            {
                cur.Value = cur.Value / 1024;
                cur.Unit = (cur.Unit.AsInt() + 1).ToEnum<CPUSizeEnum>();
                return cur.ToFixedString();
            }
            return ToString();
        }
    }

}
