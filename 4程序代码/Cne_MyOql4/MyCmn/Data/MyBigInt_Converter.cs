using System;
using System.ComponentModel;

namespace MyCmn
{
    public class MyBigIntConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType.FullName == "System.String") return true;
            if (sourceType.FullName == "System.DBNull") return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType.FullName == "System.String") return true;
            if (destinationType.FullName == "System.DBNull") return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value.IsDBNull()) return new MyBigInt();

            var str = value as string;

            if (str != null) return new MyBigInt(str);

            return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value.IsDBNull()) return new MyBigInt();
            var obj = value as MyBigInt;

            if (destinationType.FullName == "System.String") return obj.ToString();

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
