using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using SoftFluent.Windows;
using SoftFluent.Windows.Utilities;
using Abstractions;
using System.Windows.Input;
using Utilities;

namespace PropertyGrid.WPF {
   public static class Helper2 {


      public static void Update(RoutedEventArgs e) {
         if (e.OriginalSource is ToggleButton button) {
            if (button.DataContext is IPropertyGridItem item && item.Property != null && item.Property.IsEnum &&
                item.Property.IsFlagsEnum) {
               if (button.IsChecked.HasValue) {
                  ulong itemValue = Helper5.EnumToUInt64(item.Property, item.Value);
                  ulong propertyValue = Helper5.EnumToUInt64(item.Property, item.Property.Value);
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

                  object propValue = Helper5.EnumToObject(item.Property, newValue);
                  item.Property.Value = propValue;

                  if (button.GetVisualSelfOrParent<ListBoxItem>() is ListBoxItem li) {
                     if (ItemsControl.ItemsControlFromItemContainer(li) is ItemsControl parent) {
                        if (button.IsChecked == true && itemValue == 0) {
                           foreach (IPropertyGridItem gridItem in parent.Items.OfType<IPropertyGridItem>()) {
                              gridItem.IsChecked =
                                 Helper5.EnumToUInt64(item.Property, gridItem.Value) == 0;
                           }
                        }
                        else {
                           foreach (IPropertyGridItem gridItem in parent.Items.OfType<IPropertyGridItem>()) {
                              ulong gridItemValue =
                                 Helper5.EnumToUInt64(item.Property, gridItem.Value);
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
      public static bool? ShowEditor(IPropertyGridProperty property, Window editor) {
         bool? ret;
         IPropertyGridObject go = property.ListSource.Data as IPropertyGridObject;
         if (go != null) {
            if (go.TryShowEditor(property, editor, out ret)) {
               return ret;
            }

            SoftFluent.Windows.Helper.RefreshSelectedObject(editor);
         }

         ret = editor.ShowDialog();
         go?.EditorClosed(property, editor);
         return ret;
      }
      public static void OnUIElementPreviewKeyUp(KeyEventArgs e) {
         if (e.Key == Key.Space) {
            if (e.OriginalSource is ListBoxItem item) {
               if (item.DataContext is IPropertyGridItem gridItem) {
                  if (gridItem.IsChecked.HasValue) {
                     gridItem.IsChecked = !gridItem.IsChecked.Value;
                  }
               }
            }
         }
      }

      public static void ChangeText(ExecutedRoutedEventArgs e) {
         if (e.OriginalSource is TextBox tb) {
            if (SoftFluent.Windows.PropertyGrid.NewGuidCommand.Equals(e.Command)) {
               tb.Text = Guid.NewGuid().ToString(Extensions2.NormalizeGuidParameter(e.Parameter));
               return;
            }

            if (SoftFluent.Windows.PropertyGrid.EmptyGuidCommand.Equals(e.Command)) {
               tb.Text = Guid.Empty.ToString(Extensions2.NormalizeGuidParameter(e.Parameter));
               return;
            }

            if (SoftFluent.Windows.PropertyGrid.IncrementGuidCommand.Equals(e.Command)) {
               Guid g = ConversionHelper.ChangeType(tb.Text.Trim(), Guid.Empty);
               byte[] bytes = g.ToByteArray();
               bytes[15]++;
               tb.Text = new Guid(bytes).ToString(Extensions2.NormalizeGuidParameter(e.Parameter));
               return;
            }
         }
      }


      public static Window GetEditor(this SoftFluent.Windows.PropertyGrid propertyGrid, IPropertyGridProperty property, object parameter) {
         if (property == null) {
            throw new ArgumentNullException("property");
         }

         string resourceKey = $"{parameter}";
         if (string.IsNullOrWhiteSpace(resourceKey)) {
            if (SoftFluent.Abstractions.Helper.FromProperty(property) is IPropertyGridOptionsAttribute att) {
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
   }
}
