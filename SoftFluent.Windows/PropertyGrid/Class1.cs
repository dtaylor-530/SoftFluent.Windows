using Abstractions;
using SoftFluent.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System;
using System.Collections;
using Utilities;

namespace PropertyGrid {
   public static class Class1 {

      public static object EnumToObject(IPropertyGridProperty property, object value) {
         if (property == null) {
            throw new ArgumentNullException("property");
         }

         if (value != null && property.PropertyType.IsEnum) {
            return Extensions.EnumToObject(property.PropertyType, value);
         }

         if (value != null && value.GetType().IsEnum) {
            return Extensions.EnumToObject(value.GetType(), value);
         }

         if (property.PropertyType != typeof(string)) {
            return ConversionHelper.ChangeType(value, property.PropertyType);
         }

         IPropertyGridOptionsAttribute options = SoftFluent.Abstractions.Helper.FromProperty(property);
         if (options == null) {
            return ConversionHelper.ChangeType(value, property.PropertyType);
         }

         return EnumToObject(options, property.PropertyType, value);
      }

      public static object EnumToObject(IPropertyGridOptionsAttribute options, Type propertyType, object value) {
         if (options == null) {
            throw new ArgumentNullException("options");
         }

         if (propertyType == null) {
            throw new ArgumentNullException("propertyType");
         }

         if (value != null && propertyType.IsEnum) {
            return Extensions.EnumToObject(propertyType, value);
         }

         if (value != null && value.GetType().IsEnum) {
            return Extensions.EnumToObject(value.GetType(), value);
         }

         if (propertyType != typeof(string)) {
            return ConversionHelper.ChangeType(value, propertyType);
         }

         if (options == null || options.EnumNames == null || options.EnumValues == null || options.EnumValues.Length != options.EnumNames.Length) {
            return ConversionHelper.ChangeType(value, propertyType);
         }

         if (BaseConverter.IsNullOrEmptyString(value)) {
            return string.Empty;
         }

         StringBuilder sb = new StringBuilder();
         string svalue = string.Format("{0}", value);
         if (!ulong.TryParse(svalue, out ulong ul)) {
            List<string> enums = ParseEnum(svalue);
            if (enums.Count == 0) {
               return string.Empty;
            }

            string[] enumValues = options.EnumValues.Select(v => string.Format("{0}", v)).ToArray();
            foreach (string enumValue in enums) {
               int index = IndexOf(enumValues, enumValue);
               if (index < 0) {
                  index = IndexOf(options.EnumNames, enumValue);
               }

               if (index >= 0) {
                  if (sb.Length > 0 && options.EnumSeparator != null) {
                     sb.Append(options.EnumSeparator);
                  }
                  sb.Append(options.EnumNames[index]);
               }
            }
         }
         else // a string
         {
            ulong bitsCount = (ulong)GetEnumMaxPower(options) - 1; // skip first
            ulong b = 1;
            for (ulong bit = 1; bit < bitsCount; bit++) // signed, skip highest bit
            {
               if ((ul & b) != 0) {
                  int index = IndexOf(options.EnumValues, b);
                  if (index >= 0) {
                     if (sb.Length > 0 && options.EnumSeparator != null) {
                        sb.Append(options.EnumSeparator);
                     }
                     sb.Append(options.EnumNames[index]);
                  }
               }
               b *= 2;
            }
         }

         string s = sb.ToString();
         if (s.Length == 0) {
            int index = IndexOf(options.EnumValues, 0);
            if (index >= 0) {
               s = options.EnumNames[index];
            }
         }

         return s;
      }

      public static ulong EnumToUInt64(IPropertyGridProperty property, object value) {
         if (property == null) {
            throw new ArgumentNullException("property");
         }

         if (value == null) {
            return 0;
         }

         Type type = value.GetType();
         if (type.IsEnum) {
            return Extensions.EnumToUInt64(value);
         }

         TypeCode typeCode = Convert.GetTypeCode(value);
         switch (typeCode) {
            case TypeCode.SByte:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
               return (ulong)Convert.ToInt64(value);

            case TypeCode.Byte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
               return Convert.ToUInt64(value);
         }

         IPropertyGridOptionsAttribute att = SoftFluent.Abstractions.Helper.FromProperty(property);
         if (att == null || att.EnumNames == null) {
            return 0;
         }

         string svalue = string.Format("{0}", value);
         if (ulong.TryParse(svalue, out ulong ul)) {
            return ul;
         }

         List<string> enums = ParseEnum(svalue);
         if (enums.Count == 0) {
            return 0;
         }

         foreach (string name in enums) {
            int index = IndexOf(att.EnumNames, name);
            if (index < 0) {
               continue;
            }

            ulong ulvalue = Extensions.EnumToUInt64(att.EnumValues[index]);
            ul |= ulvalue;
         }
         return ul;
      }

      public static int GetEnumMaxPower(IPropertyGridOptionsAttribute options) {
         if (options == null) {
            throw new ArgumentNullException("options");
         }

         return options.EnumMaxPower <= 0 ? 32 : options.EnumMaxPower;
      }

 



      public static bool ShowEnumField(IPropertyGridProperty property, Type type, string name, out string displayName) {
         if (property == null) {
            throw new ArgumentNullException("property");
         }

         if (type == null) {
            throw new ArgumentNullException("type");
         }

         if (name == null) {
            throw new ArgumentNullException("name");
         }

         FieldInfo fi = type.GetField(name, BindingFlags.Static | BindingFlags.Public);
         displayName = fi.Name;
         BrowsableAttribute ba = fi.GetAttribute<BrowsableAttribute>();
         if (ba != null && !ba.Browsable) {
            return false;
         }

         DescriptionAttribute da = fi.GetAttribute<DescriptionAttribute>();
         if (da != null && !string.IsNullOrWhiteSpace(da.Description)) {
            displayName = da.Description;
         }
         return true;
      }

      private static int IndexOf(string[] names, string name) {
         for (int i = 0; i < names.Length; i++) {
            if (names[i] == null) {
               continue;
            }

            if (string.Compare(names[i], name, StringComparison.OrdinalIgnoreCase) == 0) {
               return i;
            }
         }
         return -1;
      }

      private static int IndexOf(object[] names, ulong value) {
         for (int i = 0; i < names.Length; i++) {
            if (names[i] == null) {
               continue;
            }

            if (!ulong.TryParse(string.Format("{0}", names[i]), out ulong ul)) {
               continue;
            }

            if (ul == value) {
               return i;
            }
         }
         return -1;
      }

      private static List<string> ParseEnum(string text) {
         List<string> enums = new List<string>();
         string[] split = text.Split(',', ';', '|', ' ');
         if (split.Length >= 0) {
            foreach (string sp in split) {
               if (string.IsNullOrWhiteSpace(sp)) {
                  continue;
               }

               enums.Add(sp.Trim());
            }
         }
         return enums;
      }
   }
}