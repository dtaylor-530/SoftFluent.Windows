using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;

namespace Abstractions {
   public interface IPropertyGrid {
      bool IsReadOnly { get; }
      Dispatcher Dispatcher { get; }
      string DefaultCategoryName { get; set; }
      bool DecamelizePropertiesDisplayNames { get; set; }

      //void Describe(IPropertyGridProperty property, PropertyDescriptor descriptor);
      void UpdateCellBindings(IPropertyGridProperty propertyGridProperty, string childName, Func<Binding,bool> where, Action<BindingExpression> action);
   }
}
