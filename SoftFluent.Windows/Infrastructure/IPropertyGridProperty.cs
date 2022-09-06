using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions {
   public interface IPropertyGridProperty {
      string Name { get; set; }
      IPropertyGridOptionsAttribute Options { get; set; }
      PropertyDescriptor Descriptor { get; set; }
      Type CollectionItemPropertyType { get;  }
      void RefreshValueFromDescriptor(bool b, bool forceRaise, bool b1);
   }

   public interface IPropertyGridOptionsAttribute
   {
      bool ForcePropertyChanged { get; set; }
      string[] EnumNames { get; set; }
      object[] EnumValues { get; set; }
      string EnumSeparator { get; set; }
      int EnumMaxPower { get; set; }
      bool IsEnum { get; set; }
      bool IsFlagsEnum { get; set; }
      object EditorDataTemplateResourceKey { get; set; }
      Type EditorType { get; set; }
      string EditorDataTemplateSelectorPropertyPath { get; set; }
      string EditorDataTemplatePropertyPath { get; set; }
   }
}
