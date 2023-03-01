using SoftFluent.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions {
   public interface IProperty: IExecute {
      string Name { get; set; }
      IPropertyGridOptionsAttribute Options { get; set; }
      PropertyDescriptor Descriptor { get; set; }
      Type CollectionItemPropertyType { get;  }
      Type PropertyType { get; set; }
      bool IsCollection { get;  }
      bool IsCollectionItemValueType { get;  }
      bool IsValueType { get;  }
      bool IsReadOnly { get;  }
      bool IsError { get;  }
      bool IsValid { get;  }
      bool IsFlagsEnum { get;  }
      string Category { get; set; }
      bool IsEnum { get;  }
      object Value { get; set; }
      string DefaultEditorResourceKey { get; }
      void RefreshValueFromDescriptor(bool b, bool forceRaise, bool b1);
      //void OnEvent(object propertyGridComboBoxExtension, IPropertyGridEventArgs createInstance);
      object BuildItems();

      IPropertyGridItem CreateItem();
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
      bool CollectionEditorHasOnlyOneColumn { get; set; }
      string EditorResourceKey { get;  }
   }
}
