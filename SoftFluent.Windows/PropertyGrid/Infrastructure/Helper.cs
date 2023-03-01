using System.Collections;
using System.Reflection;
using System.Text;
using SoftFluent.Windows;
using SoftFluent.Windows.Utilities;
using Utilities;

namespace PropertyGrid.Infrastructure
{
    public static class Helper
    {

        public static string Format(object obj, string format, IFormatProvider formatProvider)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(format))
            {
                return obj.ToString();
            }

            if (format.StartsWith("*") ||
                format.StartsWith("#"))
            {
                char sep1 = ' ';
                char sep2 = ':';
                if (format.Length > 1)
                {
                    sep1 = format[1];
                }
                if (format.Length > 2)
                {
                    sep2 = format[2];
                }

                StringBuilder sb = new StringBuilder();
                foreach (PropertyInfo pi in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (!pi.CanRead)
                    {
                        continue;
                    }

                    if (pi.GetIndexParameters().Length > 0)
                    {
                        continue;
                    }

                    object value;
                    try
                    {
                        value = pi.GetValue(obj, null);
                    }
                    catch
                    {
                        continue;
                    }
                    if (sb.Length > 0)
                    {
                        if (sep1 != ' ')
                        {
                            sb.Append(sep1);
                        }
                        sb.Append(' ');
                    }

                    if (format[0] == '#')
                    {
                        sb.Append(BaseDecamelizer.Decamelize(pi.Name));
                    }
                    else
                    {
                        sb.Append(pi.Name);
                    }
                    sb.Append(sep2);
                    sb.Append(ConversionHelper.ChangeType(value, string.Format("{0}", value), formatProvider));
                }
                return sb.ToString();
            }

            if (format.StartsWith("Item[", StringComparison.CurrentCultureIgnoreCase))
            {
                string enumExpression;
                int exprPos = format.IndexOf(']', 5);
                if (exprPos < 0)
                {
                    enumExpression = string.Empty;
                }
                else
                {
                    enumExpression = format.Substring(5, exprPos - 5).Trim();
                    // enumExpression is a lambda like expression with index as the variable
                    // ex: {0: Item[index < 10]} will enum all objects with index < 10
                    // errrhh... so far, since lambda cannot be parsed at runtime, we do nothing...
                }

                IEnumerable enumerable = obj as IEnumerable;
                if (enumerable != null)
                {
                    format = format.Substring(6 + enumExpression.Length);
                    string expression;
                    string separator;
                    if (format.Length == 0)
                    {
                        expression = null;
                        separator = ",";
                    }
                    else
                    {
                        int pos = format.IndexOf(',');
                        if (pos <= 0)
                        {
                            separator = ",";
                            // skip '.'
                            expression = format.Substring(1);
                        }
                        else
                        {
                            separator = format.Substring(pos + 1);
                            expression = format.Substring(1, pos - 1);
                        }
                    }
                    return Extensions.ConcatenateCollection(enumerable, expression, separator, formatProvider);
                }
            }
            else if (format.IndexOf(',') >= 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string propName in format.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    PropertyInfo pi = obj.GetType().GetProperty(propName, BindingFlags.Instance | BindingFlags.Public);
                    if (pi == null || !pi.CanRead)
                    {
                        continue;
                    }

                    if (pi.GetIndexParameters().Length > 0)
                    {
                        continue;
                    }

                    object value;
                    try
                    {
                        value = pi.GetValue(obj, null);
                    }
                    catch
                    {
                        continue;
                    }
                    if (sb.Length > 0)
                    {
                        sb.Append(' ');
                    }
                    sb.Append(pi.Name);
                    sb.Append(':');
                    sb.AppendFormat(formatProvider, "{0}", value);
                }
                return sb.ToString();
            }

            int pos2 = format.IndexOf(':');
            if (pos2 > 0)
            {
                object inner = DataBindingEvaluator.Eval(obj, format.Substring(0, pos2), false);
                if (inner == null)
                {
                    return string.Empty;
                }

                return string.Format(formatProvider, "{0:" + format.Substring(pos2 + 1) + "}", inner);
            }
            return DataBindingEvaluator.Eval(obj, format, formatProvider, null, false);
        }


        public static void AddDynamicProperties(this DynamicObject dynamicObject, ICollection<PropertyGridAttribute> attributes)
        {
            if (attributes == null || dynamicObject == null)
            {
                return;
            }

            foreach (PropertyGridAttribute pga in attributes)
            {
                if (string.IsNullOrWhiteSpace(pga.Name))
                {
                    continue;
                }

                DynamicObjectProperty prop = dynamicObject.AddProperty(pga.Name, pga.Type, null);
                prop.SetValue(dynamicObject, pga.Value);
            }
        }

    }
}
