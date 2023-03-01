using System;
using System.Windows.Input;
using Abstractions;

namespace SoftFluent.Windows {
   public interface IPropertyGridCommandHandler {
      void CanExecute(IProperty property, object sender, EventArgs e);
      void Executed(IProperty property, object sender, EventArgs e);
   }
}