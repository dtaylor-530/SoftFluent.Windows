using SoftFluent.Windows.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Abstractions;
using Utilities;

namespace SoftFluent.Windows {

   public static class Helper {






 


      //public static void FocusChildUsingBinding(FrameworkElement element) {
      //   if (element == null) {
      //      throw new ArgumentNullException("element");
      //   }

      //   // for some reason, this binding does not work, but we still use it and do our own automatically
      //   BindingExpression expr = element.GetBindingExpression(FocusManager.FocusedElementProperty);
      //   if (expr?.ParentBinding?.ElementName is var elem) {
      //      if (element.FindFocusableVisualChild<FrameworkElement>(elem) is FrameworkElement child) {
      //         child.Focus();
      //      }
      //   }
      //}

     

      public static HashSet<Type> GetTypes() {
         return new HashSet<Type>(new[]
         {
                typeof(string), typeof(decimal), typeof(byte), typeof(sbyte), typeof(float), typeof(double),
                typeof(int), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong),
                typeof(bool), typeof(Guid), typeof(char),
                typeof(Uri), typeof(Version)
                // NOTE: timespan, datetime?
            });
      }

      public static bool HasProperties(Type type) {
         if (type == null) {
            throw new ArgumentNullException("type");
         }

         foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(type)) {
            if (!descriptor.IsBrowsable) {
               continue;
            }

            return true;
         }

         return false;
      }


      public static void OnEditorSelectorSelectionChanged(string childPropertyGridName, object sender, SelectionChangedEventArgs e) {
         OnEditorSelectorSelectionChanged(null, childPropertyGridName, sender, e);
      }
      public static void GroupByCategoryChanged(DependencyObject source, DependencyPropertyChangedEventArgs e) {
         IPropertyGrid grid = (IPropertyGrid)source;
         grid.IsGrouping = (bool)e.NewValue;
      }



 



      public static void OnEditorSelectorSelectionChanged(IPropertyGrid parentGrid, string childPropertyGridName, object sender, SelectionChangedEventArgs e) {
         if (childPropertyGridName == null) {
            throw new ArgumentNullException("childPropertyGridName");
         }

         if (sender is FrameworkElement element && e.AddedItems.Count > 0) {
            if (element.GetSelfOrParent<Window>() is Window window) {
               if (LogicalTreeHelper.FindLogicalNode(window, childPropertyGridName) is IPropertyGrid pg) {
                  if (parentGrid != null) {
                     pg.DefaultCategoryName = parentGrid.DefaultCategoryName;
                  }
                  pg.SelectedObject = e.AddedItems[0];
               }
            }
         }
      }

      public static void RefreshSelectedObject(DependencyObject editor) {
         foreach (IPropertyGrid grid in editor.GetChildren<IPropertyGrid>()) {
            grid.RefreshSelectedObject();
         }
      }

    


   }
}