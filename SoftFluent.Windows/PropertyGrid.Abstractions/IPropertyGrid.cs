using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Data;
//using System.Windows.Threading;

namespace Abstractions {
   public interface IPropertyGrid {
      bool IsReadOnly { get; set; }

      string DefaultCategoryName { get; set; }

      bool DecamelizePropertiesDisplayNames { get; set; }
      bool GroupByCategory { get; set; }
      bool IsGrouping { get; set; }
      object? SelectedObject { get; set; }


      Task InvokeAsync(Action action);
      void RefreshComboBox();
      void RefreshSelectedObject();
   }
}
