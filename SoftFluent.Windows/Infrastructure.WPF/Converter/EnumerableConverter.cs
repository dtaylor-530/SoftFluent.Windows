using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace SoftFluent.Windows
{
    public class EnumerableConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(string), typeof(EnumerableConverter), new PropertyMetadata("{0}"));

        public static readonly DependencyProperty MaxItemsProperty =
                    DependencyProperty.Register("MaxItems", typeof(int), typeof(EnumerableConverter), new PropertyMetadata(10));

        public static readonly DependencyProperty SeparatorProperty =
            DependencyProperty.Register("Separator", typeof(string), typeof(EnumerableConverter), new PropertyMetadata(", "));

        public string Format
        {
            get => (string)GetValue(FormatProperty);
            set => SetValue(FormatProperty, value);
        }

        public int MaxItems
        {
            get => (int)GetValue(MaxItemsProperty);
            set => SetValue(MaxItemsProperty, value);
        }

        public string Separator
        {
            get => (string)GetValue(SeparatorProperty);
            set => SetValue(SeparatorProperty, value);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(string) && !(value is string) && value is IEnumerable)
            {
                StringBuilder sb = new StringBuilder();
                IEnumerable enumerable = value as IEnumerable;
                if (enumerable != null)
                {
                    foreach (object obj in enumerable)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(Separator);
                        }
                        sb.AppendFormat(Format, obj);
                    }
                }
                return sb.ToString();
            }
            return ConversionHelper.ChangeType(value, targetType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}