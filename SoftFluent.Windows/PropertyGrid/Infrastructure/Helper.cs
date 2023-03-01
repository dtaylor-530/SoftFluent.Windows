﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Abstractions;
using PropertyGrid.Infrastructure;
using SoftFluent.Windows;
using SoftFluent.Windows.Utilities;

namespace Utilities {
   public static class Helper {


      public static PropertyGridProperty CreateProperty(this PropertyGridListSource Grid, PropertyDescriptor descriptor, IActivator activator) {
         if (descriptor == null) {
            throw new ArgumentNullException("descriptor");
         }

         bool forceReadWrite = false;
         PropertyGridProperty property = null;
         PropertyGridOptionsAttribute options = descriptor.GetAttribute<PropertyGridOptionsAttribute>();
         if (options != null) {
            forceReadWrite = options.ForceReadWrite;
            if (options.PropertyType != null) {
               property = (PropertyGridProperty)Activator.CreateInstance(options.PropertyType, Grid, activator);
            }
         }

         if (property == null) {
            options = descriptor.PropertyType.GetAttribute<PropertyGridOptionsAttribute>();
            if (options != null) {
               if (!forceReadWrite) {
                  forceReadWrite = options.ForceReadWrite;
               }

               if (options.PropertyType != null) {
                  property = (PropertyGridProperty)Activator.CreateInstance(options.PropertyType, Grid);
               }
            }
         }

         if (property == null) {
            property = Grid.CreateProperty();
         }

         Grid.Describe(property, descriptor);
         if (forceReadWrite) {
            property.IsReadOnly = false;
         }

         property.OnDescribed();
         property.RefreshValueFromDescriptor(true, false, true);
         return property;
      }




      //public static object EnumToObject(Type enumType, object value) {
      //   if (enumType == null) {
      //      throw new ArgumentNullException("enumType");
      //   }

      //   if (!enumType.IsEnum) {
      //      throw new ArgumentException(null, "enumType");
      //   }

      //   if (value == null) {
      //      throw new ArgumentNullException("value");
      //   }

      //   Type underlyingType = Enum.GetUnderlyingType(enumType);
      //   if (underlyingType == typeof(long)) {
      //      return Enum.ToObject(enumType, ConversionHelper.ChangeType<long>(value));
      //   }

      //   if (underlyingType == typeof(ulong)) {
      //      return Enum.ToObject(enumType, ConversionHelper.ChangeType<ulong>(value));
      //   }

      //   if (underlyingType == typeof(int)) {
      //      return Enum.ToObject(enumType, ConversionHelper.ChangeType<int>(value));
      //   }

      //   if ((underlyingType == typeof(uint))) {
      //      return Enum.ToObject(enumType, ConversionHelper.ChangeType<uint>(value));
      //   }

      //   if (underlyingType == typeof(short)) {
      //      return Enum.ToObject(enumType, ConversionHelper.ChangeType<short>(value));
      //   }

      //   if (underlyingType == typeof(ushort)) {
      //      return Enum.ToObject(enumType, ConversionHelper.ChangeType<ushort>(value));
      //   }

      //   if (underlyingType == typeof(byte)) {
      //      return Enum.ToObject(enumType, ConversionHelper.ChangeType<byte>(value));
      //   }

      //   if (underlyingType == typeof(sbyte)) {
      //      return Enum.ToObject(enumType, ConversionHelper.ChangeType<sbyte>(value));
      //   }

      //   throw new ArgumentException(null, "enumType");
      //}

      //public static ulong EnumToUInt64(object value) {
      //   if (value == null) {
      //      throw new ArgumentNullException("value");
      //   }

      //   TypeCode typeCode = Convert.GetTypeCode(value);
      //   switch (typeCode) {
      //      case TypeCode.SByte:
      //      case TypeCode.Int16:
      //      case TypeCode.Int32:
      //      case TypeCode.Int64:
      //         return (ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture);

      //      case TypeCode.Byte:
      //      case TypeCode.UInt16:
      //      case TypeCode.UInt32:
      //      case TypeCode.UInt64:
      //         return Convert.ToUInt64(value, CultureInfo.InvariantCulture);

      //      //case TypeCode.String:
      //      default:
      //         return ConversionHelper.ChangeType<ulong>(value);
      //   }
      //}


      public static string Format(object obj, string format, IFormatProvider formatProvider) {
         if (obj == null) {
            return string.Empty;
         }

         if (string.IsNullOrEmpty(format)) {
            return obj.ToString();
         }

         if ((format.StartsWith("*")) ||
             (format.StartsWith("#"))) {
            char sep1 = ' ';
            char sep2 = ':';
            if (format.Length > 1) {
               sep1 = format[1];
            }
            if (format.Length > 2) {
               sep2 = format[2];
            }

            StringBuilder sb = new StringBuilder();
            foreach (PropertyInfo pi in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
               if (!pi.CanRead) {
                  continue;
               }

               if (pi.GetIndexParameters().Length > 0) {
                  continue;
               }

               object value;
               try {
                  value = pi.GetValue(obj, null);
               }
               catch {
                  continue;
               }
               if (sb.Length > 0) {
                  if (sep1 != ' ') {
                     sb.Append(sep1);
                  }
                  sb.Append(' ');
               }

               if (format[0] == '#') {
                  sb.Append(BaseDecamelizer.Decamelize(pi.Name));
               }
               else {
                  sb.Append(pi.Name);
               }
               sb.Append(sep2);
               sb.Append(ConversionHelper.ChangeType(value, string.Format("{0}", value), formatProvider));
            }
            return sb.ToString();
         }

         if (format.StartsWith("Item[", StringComparison.CurrentCultureIgnoreCase)) {
            string enumExpression;
            int exprPos = format.IndexOf(']', 5);
            if (exprPos < 0) {
               enumExpression = string.Empty;
            }
            else {
               enumExpression = format.Substring(5, exprPos - 5).Trim();
               // enumExpression is a lambda like expression with index as the variable
               // ex: {0: Item[index < 10]} will enum all objects with index < 10
               // errrhh... so far, since lambda cannot be parsed at runtime, we do nothing...
            }

            IEnumerable enumerable = obj as IEnumerable;
            if (enumerable != null) {
               format = format.Substring(6 + enumExpression.Length);
               string expression;
               string separator;
               if (format.Length == 0) {
                  expression = null;
                  separator = ",";
               }
               else {
                  int pos = format.IndexOf(',');
                  if (pos <= 0) {
                     separator = ",";
                     // skip '.'
                     expression = format.Substring(1);
                  }
                  else {
                     separator = format.Substring(pos + 1);
                     expression = format.Substring(1, pos - 1);
                  }
               }
               return Utilities.Extensions.ConcatenateCollection(enumerable, expression, separator, formatProvider);
            }
         }
         else if (format.IndexOf(',') >= 0) {
            StringBuilder sb = new StringBuilder();
            foreach (string propName in format.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
               PropertyInfo pi = obj.GetType().GetProperty(propName, BindingFlags.Instance | BindingFlags.Public);
               if ((pi == null) || (!pi.CanRead)) {
                  continue;
               }

               if (pi.GetIndexParameters().Length > 0) {
                  continue;
               }

               object value;
               try {
                  value = pi.GetValue(obj, null);
               }
               catch {
                  continue;
               }
               if (sb.Length > 0) {
                  sb.Append(' ');
               }
               sb.Append(pi.Name);
               sb.Append(':');
               sb.AppendFormat(formatProvider, "{0}", value);
            }
            return sb.ToString();
         }

         int pos2 = format.IndexOf(':');
         if (pos2 > 0) {
            object inner = DataBindingEvaluator.Eval(obj, format.Substring(0, pos2), false);
            if (inner == null) {
               return string.Empty;
            }

            return string.Format(formatProvider, "{0:" + format.Substring(pos2 + 1) + "}", inner);
         }
         return DataBindingEvaluator.Eval(obj, format, formatProvider, null, false);
      }

      public static void Describe(this IPropertyGrid propertyGrid, PropertyGridProperty property, PropertyDescriptor descriptor) {
         if (property == null) {
            throw new ArgumentNullException("property");
         }

         if (descriptor == null) {
            throw new ArgumentNullException("descriptor");
         }

         property.Descriptor = descriptor;
         property.Name = descriptor.Name;
         property.PropertyType = descriptor.PropertyType;

         // unset by default. conversion service does the default job
         //property.Converter = descriptor.Converter;

         property.Category =
             string.IsNullOrWhiteSpace(descriptor.Category) ||
             Extensions.EqualsIgnoreCase(descriptor.Category, CategoryAttribute.Default.Category)
                 ? propertyGrid.DefaultCategoryName
                 : descriptor.Category;
         property.IsReadOnly = descriptor.IsReadOnly;
         property.Description = descriptor.Description;
         property.DisplayName = descriptor.DisplayName;
         if (propertyGrid.DecamelizePropertiesDisplayNames && property.DisplayName == descriptor.Name) {
            property.DisplayName = BaseDecamelizer.Decamelize(property.DisplayName);
         }

         property.IsEnum = descriptor.PropertyType.IsEnum;
         property.IsFlagsEnum = descriptor.PropertyType.IsEnum && Extensions.IsFlagsEnum(descriptor.PropertyType);

         PropertyGridOptionsAttribute options = Extensions.GetAttribute<PropertyGridOptionsAttribute>((MemberDescriptor)descriptor);
         if (options != null) {
            if (options.SortOrder != 0) {
               property.SortOrder = options.SortOrder;
            }

            property.IsEnum = options.IsEnum;
            property.IsFlagsEnum = options.IsFlagsEnum;
         }

         DefaultValueAttribute att = Extensions.GetAttribute<DefaultValueAttribute>((MemberDescriptor)descriptor);
         if (att != null) {
            property.HasDefaultValue = true;
            property.DefaultValue = att.Value;
         }
         else if (options != null) {
            if (options.HasDefaultValue) {
               property.HasDefaultValue = true;
               property.DefaultValue = options.DefaultValue;
            }
            else {
               if (TryGetDefaultValue(options, out string defaultValue)) {
                  property.DefaultValue = defaultValue;
                  property.HasDefaultValue = true;
               }
            }
         }

         property.Attributes.AddDynamicProperties(descriptor.Attributes.OfType<PropertyGridAttribute>().ToArray());
         property.TypeAttributes.AddDynamicProperties(Extensions.GetAttributes<PropertyGridAttribute>(descriptor.PropertyType)
             .ToArray());

         static bool TryGetDefaultValue(PropertyGridOptionsAttribute options, out string value) {
            value = null;
            if (options == null || !options.IsEnum && !options.IsFlagsEnum) {
               return false;
            }

            if (options.EnumNames != null && options.EnumNames.Length > 0) {
               value = options.EnumNames.First();
               return true;
            }
            return false;
         }
      }


      public static void AddDynamicProperties(this DynamicObject dynamicObject, ICollection<PropertyGridAttribute> attributes) {
         if (attributes == null || dynamicObject == null) {
            return;
         }

         foreach (PropertyGridAttribute pga in attributes) {
            if (string.IsNullOrWhiteSpace(pga.Name)) {
               continue;
            }

            DynamicObjectProperty prop = dynamicObject.AddProperty(pga.Name, pga.Type, null);
            prop.SetValue(dynamicObject, pga.Value);
         }
      }

   }
}