using Abstractions;

namespace SoftFluent.Windows;

public interface IPropertyGridItem
{
   bool IsZero { get; set; }
   string Name { get; set; }
   object Value { get; set; }
   bool? IsChecked { get; set; }
   IPropertyGridProperty Property { get; set; }
   bool IsUnset { get; set; }
}