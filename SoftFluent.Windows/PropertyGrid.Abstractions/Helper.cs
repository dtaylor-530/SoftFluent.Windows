using Abstractions;
using SoftFluent.Windows;
using System.ComponentModel;

namespace SoftFluent.Abstractions {
   public static class Helper {
      public static IPropertyGridOptionsAttribute FromProperty(IProperty property) {
         if (property == null)
            throw new ArgumentNullException("property");

         IPropertyGridOptionsAttribute att = property.Options;
         if (att != null)
            return att;

         if (property.Descriptor != null) {
            att = property.Descriptor.GetAttribute<PropertyGridOptionsAttribute>();
         }
         return att;
      }

      public static T GetAttribute<T>(this MemberDescriptor descriptor) where T : Attribute {
         if (descriptor == null) {
            return null;
         }

         return GetAttribute<T>(descriptor.Attributes);
      }

      public static T GetAttribute<T>(this AttributeCollection attributes) where T : Attribute {
         if (attributes == null) {
            return null;
         }

         foreach (object att in attributes) {
            if (att is T attribute) {
               return attribute;
            }
         }
         return null;
      }
   }
}