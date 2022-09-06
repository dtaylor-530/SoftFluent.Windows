using System;
using System.ComponentModel;
using System.Globalization;

namespace SoftFluent.Windows.Samples
{
    public class AddressConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string s = value as string;
            if (s != null)
            {
                return Address.Parse(s);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}