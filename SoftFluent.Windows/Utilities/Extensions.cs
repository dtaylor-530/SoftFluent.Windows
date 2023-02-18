using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using SoftFluent.Windows;
using SoftFluent.Windows.Utilities;

namespace Utilities
{
    public static class Extensions
    {
        private const string _hexaChars = "0123456789ABCDEF";

        //public static string ConcatenateCollection(IEnumerable collection, string expression, string separator)
        //{
        //    return ConcatenateCollection(collection, expression, separator, null);
        //}

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


      public static object EnumToObject(Type enumType, object value) {
         if (enumType == null) {
            throw new ArgumentNullException("enumType");
         }

         if (!enumType.IsEnum) {
            throw new ArgumentException(null, "enumType");
         }

         if (value == null) {
            throw new ArgumentNullException("value");
         }

         Type underlyingType = Enum.GetUnderlyingType(enumType);
         if (underlyingType == typeof(long)) {
            return Enum.ToObject(enumType, ConversionHelper.ChangeType<long>(value));
         }

         if (underlyingType == typeof(ulong)) {
            return Enum.ToObject(enumType, ConversionHelper.ChangeType<ulong>(value));
         }

         if (underlyingType == typeof(int)) {
            return Enum.ToObject(enumType, ConversionHelper.ChangeType<int>(value));
         }

         if ((underlyingType == typeof(uint))) {
            return Enum.ToObject(enumType, ConversionHelper.ChangeType<uint>(value));
         }

         if (underlyingType == typeof(short)) {
            return Enum.ToObject(enumType, ConversionHelper.ChangeType<short>(value));
         }

         if (underlyingType == typeof(ushort)) {
            return Enum.ToObject(enumType, ConversionHelper.ChangeType<ushort>(value));
         }

         if (underlyingType == typeof(byte)) {
            return Enum.ToObject(enumType, ConversionHelper.ChangeType<byte>(value));
         }

         if (underlyingType == typeof(sbyte)) {
            return Enum.ToObject(enumType, ConversionHelper.ChangeType<sbyte>(value));
         }

         throw new ArgumentException(null, "enumType");
      }

      public static ulong EnumToUInt64(object value) {
         if (value == null) {
            throw new ArgumentNullException("value");
         }

         TypeCode typeCode = Convert.GetTypeCode(value);
         switch (typeCode) {
            case TypeCode.SByte:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
               return (ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture);

            case TypeCode.Byte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
               return Convert.ToUInt64(value, CultureInfo.InvariantCulture);

            //case TypeCode.String:
            default:
               return ConversionHelper.ChangeType<ulong>(value);
         }
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
        public static string ConvertToUnsecureString(this SecureString securePassword) {
           if (securePassword == null)
              throw new ArgumentNullException("securePassword");

           IntPtr unmanagedString = IntPtr.Zero;
           try {
              unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
              return Marshal.PtrToStringUni(unmanagedString);
           }
           finally {
              Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
           }
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



        public static string NormalizeGuidParameter(object parameter) {
           const string guidParameters = "DNBPX";
           string p = $"{parameter}".ToUpperInvariant();
           if (p.Length == 0) {
              return guidParameters[0].ToString(CultureInfo.InvariantCulture);
           }

           char ch = guidParameters.FirstOrDefault(c => c == p[0]);
           return ch == 0 ? guidParameters[0].ToString(CultureInfo.InvariantCulture) : ch.ToString(CultureInfo.InvariantCulture);
        }

      public static bool IsEnumOrNullableEnum(Type type, out Type enumType, out bool nullable) {
           if (type == null) {
              throw new ArgumentNullException("type");
           }

           nullable = false;
           if (type.IsEnum) {
              enumType = type;
              return true;
           }

           if (type.Name == typeof(Nullable<>).Name) {
              Type[] args = type.GetGenericArguments();
              if (args.Length == 1 && args[0].IsEnum) {
                 enumType = args[0];
                 nullable = true;
                 return true;
              }
           }

           enumType = null;
           return false;
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