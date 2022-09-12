using Abstractions;

namespace SoftFluent.Windows;

public interface IPropertyGridEventArgs
{
   IPropertyGridProperty Property { get; }
   object Context { get; set; }
   bool Cancel { get; set; }
}