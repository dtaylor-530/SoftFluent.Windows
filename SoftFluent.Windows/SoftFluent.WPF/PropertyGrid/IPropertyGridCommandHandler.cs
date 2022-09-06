using System.Windows.Input;
using Abstractions;

namespace SoftFluent.Windows {
   public interface IPropertyGridCommandHandler {
      void CanExecute(IPropertyGridProperty property, object sender, CanExecuteRoutedEventArgs e);
      void Executed(IPropertyGridProperty property, object sender, ExecutedRoutedEventArgs e);
   }
}