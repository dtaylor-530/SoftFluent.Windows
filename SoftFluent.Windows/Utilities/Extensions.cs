using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using SoftFluent.Windows.Utilities;

namespace Utilities
{
    public static class Extensions2
    {
        private const string _hexaChars = "0123456789ABCDEF";

        public static string ConcatenateCollection(IEnumerable collection, string expression, string separator)
        {
            return ConcatenateCollection(collection, expression, separator, null);
        }

        public static string ConcatenateCollection(IEnumerable collection, string expression, string separator, IFormatProvider formatProvider)
        {
            if (collection == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (object o in collection)
            {
                if (i > 0)
                {
                    sb.Append(separator);
                }
                else
                {
                    i++;
                }

                if (o != null)
                {
                    //object e = ConvertUtilities.Evaluate(o, expression, typeof(string), null, formatProvider);
                    object e = DataBindingEvaluator.Eval(o, expression, formatProvider, null, false);
                    if (e != null)
                    {
                        sb.Append(e);
                    }
                }
            }
            return sb.ToString();
        }


        public static bool EqualsIgnoreCase(this string thisString, string text)
        {
            return EqualsIgnoreCase(thisString, text, false);
        }

        public static bool EqualsIgnoreCase(this string thisString, string text, bool trim)
        {
            if (trim)
            {
                thisString = Nullify(thisString, true);
                text = Nullify(text, true);
            }

            if (thisString == null)
            {
                return text == null;
            }

            if (text == null)
            {
                return false;
            }

            if (thisString.Length != text.Length)
            {
                return false;
            }

            return string.Compare(thisString, text, StringComparison.OrdinalIgnoreCase) == 0;
        }

     


        public static string GetAllMessages(this Exception exception)
        {
            return GetAllMessages(exception, Environment.NewLine);
        }

        public static string GetAllMessages(this Exception exception, string separator)
        {
            if (exception == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            AppendMessages(sb, exception, separator);
            return sb.ToString().Replace("..", ".");
        }


        public static T GetAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            if (provider == null)
            {
                return null;
            }

            object[] o = provider.GetCustomAttributes(typeof(T), true);
            if (o == null || o.Length == 0)
            {
                return null;
            }

            return (T)o[0];
        }

        public static T GetAttribute<T>(this MemberDescriptor descriptor) where T : Attribute
        {
            if (descriptor == null)
            {
                return null;
            }

            return GetAttribute<T>(descriptor.Attributes);
        }

        public static T GetAttribute<T>(this AttributeCollection attributes) where T : Attribute
        {
            if (attributes == null)
            {
                return null;
            }

            foreach (object att in attributes)
            {
                if (typeof(T).IsAssignableFrom(att.GetType()))
                {
                    return (T)att;
                }
            }
            return null;
        }

        public static IEnumerable<T> GetAttributes<T>(this MemberInfo element) where T : Attribute
        {
            return (IEnumerable<T>)Attribute.GetCustomAttributes(element, typeof(T));
        }

      

        public static Type GetElementType(Type collectionType)
        {
            if (collectionType == null)
            {
                throw new ArgumentNullException("collectionType");
            }

            foreach (Type iface in collectionType.GetInterfaces())
            {
                if (!iface.IsGenericType)
                {
                    continue;
                }

                if (iface.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    return iface.GetGenericArguments()[1];
                }

                if (iface.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    return iface.GetGenericArguments()[0];
                }

                if (iface.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    return iface.GetGenericArguments()[0];
                }

                if (iface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return iface.GetGenericArguments()[0];
                }
            }
            return typeof(object);
        }

        public static int GetEnumMaxPower(Type enumType)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }

            if (!enumType.IsEnum)
            {
                throw new ArgumentException(null, "enumType");
            }

            Type utype = Enum.GetUnderlyingType(enumType);
            return GetEnumUnderlyingTypeMaxPower(utype);
        }

        public static int GetEnumUnderlyingTypeMaxPower(Type underlyingType)
        {
            if (underlyingType == null)
            {
                throw new ArgumentNullException("underlyingType");
            }

            if (underlyingType == typeof(long) || underlyingType == typeof(ulong))
            {
                return 64;
            }

            if (underlyingType == typeof(int) || underlyingType == typeof(uint))
            {
                return 32;
            }

            if (underlyingType == typeof(short) || underlyingType == typeof(ushort))
            {
                return 16;
            }

            if (underlyingType == typeof(byte) || underlyingType == typeof(sbyte))
            {
                return 8;
            }

            throw new ArgumentException(null, "underlyingType");
        }


        public static bool IsFlagsEnum(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (!type.IsEnum)
            {
                return false;
            }

            return type.IsDefined(typeof(FlagsAttribute), true);
        }

        public static bool IsNullable(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static string Nullify(this string thisString)
        {
            return Nullify(thisString, true);
        }

        public static string Nullify(this string thisString, bool trim)
        {
            if (string.IsNullOrWhiteSpace(thisString))
            {
                return null;
            }

            return trim ? thisString.Trim() : thisString;
        }


        public static string ToHexa(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }

            return ToHexa(bytes, 0, bytes.Length);
        }

        public static string ToHexa(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
            {
                return string.Empty;
            }

            if (offset < 0)
            {
                throw new ArgumentException(null, "offset");
            }

            if (count < 0)
            {
                throw new ArgumentException(null, "count");
            }

            if (offset >= bytes.Length)
            {
                return string.Empty;
            }

            count = Math.Min(count, bytes.Length - offset);

            StringBuilder sb = new StringBuilder(count * 2);
            for (int i = offset; i < (offset + count); i++)
            {
                sb.Append(_hexaChars[bytes[i] / 16]);
                sb.Append(_hexaChars[bytes[i] % 16]);
            }
            return sb.ToString();
        }

        private static void AppendMessages(StringBuilder sb, Exception e, string separator)
        {
            if (e == null)
            {
                return;
            }

            // this one is not interesting...
            if (!(e is TargetInvocationException))
            {
                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }
                sb.Append(e.Message);
            }
            AppendMessages(sb, e.InnerException, separator);
        }
    }
}