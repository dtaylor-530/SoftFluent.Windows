using SoftFluent.Windows.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utilities;

namespace SoftFluent.Windows {

   public static class Helper {






      public static DataGridColumn DataGridColumn(DataGrid grid) {
         return grid
             .Columns
             .OfType<DataGridTemplateColumn>()
             .FirstOrDefault(c => c.CellTemplateSelector is PropertyGridDataTemplateSelector);
      }





     

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

      public static void IsReadOnlyPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e) {
         PropertyGrid grid = (PropertyGrid)source;
         grid.PropertiesSource.Source = grid.PropertiesSource.Source;
      }

      public static void GroupByCategoryChanged(DependencyObject source, DependencyPropertyChangedEventArgs e) {
         PropertyGrid grid = (PropertyGrid)source;
         grid.IsGrouping = (bool)e.NewValue;
      }



      public static void OnEditorSelectorSelectionChanged(string childPropertyGridName, object sender, SelectionChangedEventArgs e) {
         OnEditorSelectorSelectionChanged(null, childPropertyGridName, sender, e);
      }

      public static void OnEditorSelectorSelectionChanged(PropertyGrid parentGrid, string childPropertyGridName, object sender, SelectionChangedEventArgs e) {
         if (childPropertyGridName == null) {
            throw new ArgumentNullException("childPropertyGridName");
         }

         if (sender is FrameworkElement element && e.AddedItems.Count > 0) {
            if (element.GetSelfOrParent<Window>() is Window window) {
               if (LogicalTreeHelper.FindLogicalNode(window, childPropertyGridName) is PropertyGrid pg) {
                  if (parentGrid != null) {
                     pg.DefaultCategoryName = parentGrid.DefaultCategoryName;
                  }
                  pg.SelectedObject = e.AddedItems[0];
               }
            }
         }
      }

      public static void RefreshSelectedObject(DependencyObject editor) {
         foreach (PropertyGrid grid in editor.GetChildren<PropertyGrid>()) {
            grid.RefreshSelectedObject();
         }
      }

      public static void SelectedObjectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
         if (d is PropertyGrid propertyGrid) {
            Helper.SelectedObjectPropertyChanged(propertyGrid, e);
         }
      }

      public static void SelectedObjectPropertyChanged(PropertyGrid grid, DependencyPropertyChangedEventArgs e) {
         if (e.OldValue is INotifyPropertyChanged pc) {
            pc.PropertyChanged -= OnDispatcherSourcePropertyChanged;
         }

         if (e.NewValue == null) {
            grid.PropertiesSource.Source = null;
            return;
         }

         grid.RefreshComboBox();

         if (Extensions2.GetAttribute<ReadOnlyAttribute>(e.NewValue.GetType()) is ReadOnlyAttribute roa &&
             roa.IsReadOnly) {
            grid.IsReadOnly = true;
         }
         else {
            grid.IsReadOnly = false;
         }

         if (e.NewValue is INotifyPropertyChanged npc) {
            npc.PropertyChanged += OnDispatcherSourcePropertyChanged;
         }

         grid.PropertiesSource.Source = grid.CreatePropertyListSource(e.NewValue);

         void OnDispatcherSourcePropertyChanged(object sender, PropertyChangedEventArgs eventArgs) {
            if (sender is PropertyGrid dispatcherObject) {
               if (!dispatcherObject.Dispatcher.CheckAccess()) {
                  dispatcherObject.Dispatcher.Invoke(() => dispatcherObject.OnSourcePropertyChanged(sender, eventArgs));
               }
               else {
                  dispatcherObject.OnSourcePropertyChanged(sender, eventArgs);
               }
            }
         }
      }

      public static void SetGroupByCategory(PropertyGrid grid, bool value) {
         switch (value) {
            case true when !grid.GroupByCategory:
               grid.PropertiesSource.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
               break;

            case false when grid.GroupByCategory:
               grid.PropertiesSource.GroupDescriptions.Clear();
               break;
         }
      }

  

      public static void UpdateBindings(PropertyGrid grid, object dataItem, string childName, Func<Binding, bool> where, Action<BindingExpression> action) {
         if (dataItem == null) {
            throw new ArgumentNullException("dataItem");
         }

         if (action == null) {
            throw new ArgumentNullException("action");
         }

         if (!(grid.GetValueCellContent(dataItem) is var fe)) {
            return;
         }

         if (childName == null) {
            foreach (UIElement child in fe.EnumerateVisualChildren(true).OfType<UIElement>()) {
               Action(child);
            }
         }
         else {
            FrameworkElement child = fe.FindVisualChild<FrameworkElement>(childName);
            if (child != null) {
               Action(child);
            }
         }

         void Action(UIElement element) {
            if (element == null) {
               throw new ArgumentNullException("element");
            }

            if (action == null) {
               throw new ArgumentNullException("action");
            }

            if (@where == null) {
               @where = b => true;
            }

            foreach (DependencyProperty prop in Extensions.EnumerateMarkupDependencyProperties(element)) {
               BindingExpression expr = BindingOperations.GetBindingExpression(element, prop);
               if (expr != null && expr.ParentBinding != null && @where(expr.ParentBinding)) {
                  action(expr);
               }
            }
         }
      }
   }
}