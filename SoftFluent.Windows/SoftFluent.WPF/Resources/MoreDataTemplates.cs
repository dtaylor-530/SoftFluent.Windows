using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace PropertyGrid.WPF.Resources {
   public partial class MoreDataTemplatesResourceDictionary {


      private void OnEditorWindowCloseCanExecute(object sender, CanExecuteRoutedEventArgs e) {
         e.CanExecute = true;
      }

      private void OnEditorWindowCloseExecuted(object sender, ExecutedRoutedEventArgs e) {
         Window window = (Window)sender;
         window.DialogResult = false;
         window.Close();
      }
   }
}
