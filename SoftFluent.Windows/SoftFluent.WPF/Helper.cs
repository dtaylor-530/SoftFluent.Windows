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
using PropertyGrid.Infrastructure;
using Utilities;

namespace SoftFluent.Windows {

   public static class Helper {


      public static void AddDynamicProperties(this DynamicObject dynamicObject, ICollection<PropertyGridAttribute> attributes) {
         if (attributes == null || dynamicObject == null) {
            return;
         }

         foreach (PropertyGridAttribute pga in attributes) {
            if (string.IsNullOrWhiteSpace(pga.Name)) {
               continue;
            }

            DynamicObjectProperty prop = dynamicObject.AddProperty(pga.Name, pga.Type, null);
            prop.SetValue(dynamicObject, pga.Value);
         }
      }

      public static PropertyGridProperty CreateProperty(this PropertyGridListSource Grid, PropertyDescriptor descriptor) {
         if (descriptor == null) {
            throw new ArgumentNullException("descriptor");
         }

         bool forceReadWrite = false;
         PropertyGridProperty property = null;
         PropertyGridOptionsAttribute options = descriptor.GetAttribute<PropertyGridOptionsAttribute>();
         if (options != null) {
            forceReadWrite = options.ForceReadWrite;
            if (options.PropertyType != null) {
               property = (PropertyGridProperty)Activator.CreateInstance(options.PropertyType, Grid);
            }
         }

         if (property == null) {
            options = descriptor.PropertyType.GetAttribute<PropertyGridOptionsAttribute>();
            if (options != null) {
               if (!forceReadWrite) {
                  forceReadWrite = options.ForceReadWrite;
               }

               if (options.PropertyType != null) {
                  property = (PropertyGridProperty)Activator.CreateInstance(options.PropertyType, Grid);
               }
            }
         }

         if (property == null) {
            property = Grid.CreateProperty();
         }

         Grid.Describe(property, descriptor);
         if (forceReadWrite) {
            property.IsReadOnly = false;
         }

         property.OnDescribed();
         property.RefreshValueFromDescriptor(true, false, true);
         return property;
      }

      public static void ChangeText(ExecutedRoutedEventArgs e) {
         if (e.OriginalSource is TextBox tb) {
            if (PropertyGrid.NewGuidCommand.Equals(e.Command)) {
               tb.Text = Guid.NewGuid().ToString(Helper.NormalizeGuidParameter(e.Parameter));
               return;
            }

            if (PropertyGrid.EmptyGuidCommand.Equals(e.Command)) {
               tb.Text = Guid.Empty.ToString(Helper.NormalizeGuidParameter(e.Parameter));
               return;
            }

            if (PropertyGrid.IncrementGuidCommand.Equals(e.Command)) {
               Guid g = ConversionHelper.ChangeType(tb.Text.Trim(), Guid.Empty);
               byte[] bytes = g.ToByteArray();
               bytes[15]++;
               tb.Text = new Guid(bytes).ToString(Helper.NormalizeGuidParameter(e.Parameter));
               return;
            }
         }
      }



      public static DataGridColumn DataGridColumn(DataGrid grid) {
         return grid
             .Columns
             .OfType<DataGridTemplateColumn>()
             .FirstOrDefault(c => c.CellTemplateSelector is PropertyGridDataTemplateSelector);
      }



      public static void FocusChildUsingBinding(FrameworkElement element) {
         if (element == null) {
            throw new ArgumentNullException("element");
         }

         // for some reason, this binding does not work, but we still use it and do our own automatically
         BindingExpression expr = element.GetBindingExpression(FocusManager.FocusedElementProperty);
         if (expr?.ParentBinding?.ElementName is var elem) {
            if (element.FindFocusableVisualChild<FrameworkElement>(elem) is FrameworkElement child) {
               child.Focus();
            }
         }
      }

      public static Window GetEditor(this PropertyGrid propertyGrid, PropertyGridProperty property, object parameter) {
         if (property == null) {
            throw new ArgumentNullException("property");
         }

         string resourceKey = $"{parameter}";
         if (string.IsNullOrWhiteSpace(resourceKey)) {
            if (PropertyGridOptionsAttribute.FromProperty(property) is PropertyGridOptionsAttribute att) {
               resourceKey = att.EditorResourceKey;
            }

            if (string.IsNullOrWhiteSpace(resourceKey)) {
               resourceKey = property.DefaultEditorResourceKey;
               if (string.IsNullOrWhiteSpace(resourceKey)) {
                  resourceKey = "ObjectEditorWindow";
               }
            }
         }

         Window editor = propertyGrid.TryFindResource(resourceKey) as Window;
         if (editor != null) {
            editor.Owner = propertyGrid.GetVisualSelfOrParent<Window>();
            if (editor.Owner != null) {
               PropertyGridWindowOptions wo = PropertyGridWindowManager.GetOptions(editor);
               if ((wo & PropertyGridWindowOptions.UseDefinedSize) == PropertyGridWindowOptions.UseDefinedSize) {
                  if (double.IsNaN(editor.Left)) {
                     editor.Left = editor.Owner.Left + propertyGrid.ChildEditorWindowOffset;
                  }

                  if (double.IsNaN(editor.Top)) {
                     editor.Top = editor.Owner.Top + propertyGrid.ChildEditorWindowOffset;
                  }

                  if (double.IsNaN(editor.Width)) {
                     editor.Width = editor.Owner.Width;
                  }

                  if (double.IsNaN(editor.Height)) {
                     editor.Height = editor.Owner.Height;
                  }
               }
               else {
                  editor.Left = editor.Owner.Left + propertyGrid.ChildEditorWindowOffset;
                  editor.Top = editor.Owner.Top + propertyGrid.ChildEditorWindowOffset;
                  editor.Width = editor.Owner.Width;
                  editor.Height = editor.Owner.Height;
               }
            }
            editor.DataContext = property;
            Selector selector = LogicalTreeHelper.FindLogicalNode(editor, "EditorSelector") as Selector;
            if (selector != null) {
               selector.SelectedIndex = 0;
            }

            Grid grid = LogicalTreeHelper.FindLogicalNode(editor, "CollectionEditorListGrid") as Grid;
            if (grid != null && grid.ColumnDefinitions.Count > 2) {
               if (property.IsCollection && propertyGrid.CollectionEditorHasOnlyOneColumn(property)) {
                  grid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Pixel);
                  grid.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Pixel);
               }
               else {
                  grid.ColumnDefinitions[1].Width = new GridLength(5, GridUnitType.Pixel);
                  grid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
               }
            }

            if (editor is IPropertyGridEditor pge) {
               if (!pge.SetContext(property, parameter)) {
                  return null;
               }
            }
         }
         return editor;
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

      public static string NormalizeGuidParameter(object parameter) {
         const string guidParameters = "DNBPX";
         string p = $"{parameter}".ToUpperInvariant();
         if (p.Length == 0) {
            return guidParameters[0].ToString(CultureInfo.InvariantCulture);
         }

         char ch = guidParameters.FirstOrDefault(c => c == p[0]);
         return ch == 0 ? guidParameters[0].ToString(CultureInfo.InvariantCulture) : ch.ToString(CultureInfo.InvariantCulture);
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

      public static void OnUIElementPreviewKeyUp(KeyEventArgs e) {
         if (e.Key == Key.Space) {
            if (e.OriginalSource is ListBoxItem item) {
               if (item.DataContext is PropertyGridItem gridItem) {
                  if (gridItem.IsChecked.HasValue) {
                     gridItem.IsChecked = !gridItem.IsChecked.Value;
                  }
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

      public static bool? ShowEditor(PropertyGridProperty property, Window editor) {
         bool? ret;
         IPropertyGridObject go = property.ListSource.Data as IPropertyGridObject;
         if (go != null) {
            if (go.TryShowEditor(property, editor, out ret)) {
               return ret;
            }

            Helper.RefreshSelectedObject(editor);
         }

         ret = editor.ShowDialog();
         go?.EditorClosed(property, editor);
         return ret;
      }

      public static void Update(RoutedEventArgs e) {
         if (e.OriginalSource is ToggleButton button) {
            if (button.DataContext is PropertyGridItem item && item.Property != null && item.Property.IsEnum &&
                item.Property.IsFlagsEnum) {
               if (button.IsChecked.HasValue) {
                  ulong itemValue = PropertyGridComboBoxExtension.EnumToUInt64(item.Property, item.Value);
                  ulong propertyValue = PropertyGridComboBoxExtension.EnumToUInt64(item.Property, item.Property.Value);
                  ulong newValue;
                  if (button.IsChecked.Value) {
                     if (itemValue == 0) {
                        newValue = 0;
                     }
                     else {
                        newValue = propertyValue | itemValue;
                     }
                  }
                  else {
                     newValue = propertyValue & ~itemValue;
                  }

                  object propValue = PropertyGridComboBoxExtension.EnumToObject(item.Property, newValue);
                  item.Property.Value = propValue;

                  if (button.GetVisualSelfOrParent<ListBoxItem>() is ListBoxItem li) {
                     if (ItemsControl.ItemsControlFromItemContainer(li) is ItemsControl parent) {
                        if (button.IsChecked == true && itemValue == 0) {
                           foreach (PropertyGridItem gridItem in parent.Items.OfType<PropertyGridItem>()) {
                              gridItem.IsChecked =
                                  PropertyGridComboBoxExtension.EnumToUInt64(item.Property, gridItem.Value) == 0;
                           }
                        }
                        else {
                           foreach (PropertyGridItem gridItem in parent.Items.OfType<PropertyGridItem>()) {
                              ulong gridItemValue =
                                  PropertyGridComboBoxExtension.EnumToUInt64(item.Property, gridItem.Value);
                              if (gridItemValue == 0) {
                                 gridItem.IsChecked = newValue == 0;
                                 continue;
                              }

                              gridItem.IsChecked = (newValue & gridItemValue) == gridItemValue;
                           }
                        }
                     }
                  }
               }
            }
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