using Abstractions;
using SoftFluent.Windows;
using SoftFluent.Windows.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

internal static class EditorSelector
{
    public static bool? ShowEditor(IProperty property, Window editor)
    {
        bool? ret;
        //IPropertyGridObject go = property.ListSource.Data as IPropertyGridObject;
        //if (go != null) {
        //   if (go.TryShowEditor(property, editor, out ret)) {
        //      return ret;
        //   }

        //   SoftFluent.Windows.Helper.RefreshSelectedObject(editor);
        //}

        ret = editor.ShowDialog();
        //go?.EditorClosed(property, editor);
        return ret;
    }



    public static Window GetEditor(this SoftFluent.Windows.PropertyGrid propertyGrid, IProperty property)
    {
        return null;
        //if (property == null)
        //{
        //    throw new ArgumentNullException("property");
        //}



        //string resourceKey = $"{parameter}";
        //if (string.IsNullOrWhiteSpace(resourceKey))
        //{
        //    if (SoftFluent.Abstractions.Helper.FromProperty(property) is IPropertyGridOptionsAttribute att)
        //    {
        //        resourceKey = att.EditorResourceKey;
        //    }

        //    if (string.IsNullOrWhiteSpace(resourceKey))
        //    {
        //        resourceKey = property.DefaultEditorResourceKey;
        //        if (string.IsNullOrWhiteSpace(resourceKey))
        //        {
        //            resourceKey = "ObjectEditorWindow";
        //        }
        //    }
        //}

        //Window editor = propertyGrid.TryFindResource(resourceKey) as Window;
        //if (editor != null)
        //{
        //    editor.Owner = propertyGrid.GetVisualSelfOrParent<Window>();
        //    if (editor.Owner != null)
        //    {
        //        PropertyGridWindowOptions wo = PropertyGridWindowManager.GetOptions(editor);
        //        if ((wo & PropertyGridWindowOptions.UseDefinedSize) == PropertyGridWindowOptions.UseDefinedSize)
        //        {
        //            if (double.IsNaN(editor.Left))
        //            {
        //                editor.Left = editor.Owner.Left + propertyGrid.ChildEditorWindowOffset;
        //            }

        //            if (double.IsNaN(editor.Top))
        //            {
        //                editor.Top = editor.Owner.Top + propertyGrid.ChildEditorWindowOffset;
        //            }

        //            if (double.IsNaN(editor.Width))
        //            {
        //                editor.Width = editor.Owner.Width;
        //            }

        //            if (double.IsNaN(editor.Height))
        //            {
        //                editor.Height = editor.Owner.Height;
        //            }
        //        }
        //        else
        //        {
        //            editor.Left = editor.Owner.Left + propertyGrid.ChildEditorWindowOffset;
        //            editor.Top = editor.Owner.Top + propertyGrid.ChildEditorWindowOffset;
        //            editor.Width = editor.Owner.Width;
        //            editor.Height = editor.Owner.Height;
        //        }
        //    }
        //    editor.DataContext = property;
        //    Selector selector = LogicalTreeHelper.FindLogicalNode(editor, "EditorSelector") as Selector;
        //    if (selector != null)
        //    {
        //        selector.SelectedIndex = 0;
        //    }

        //    Grid grid = LogicalTreeHelper.FindLogicalNode(editor, "CollectionEditorListGrid") as Grid;
        //    if (grid != null && grid.ColumnDefinitions.Count > 2)
        //    {
        //        if (property.IsCollection && propertyGrid.CollectionEditorHasOnlyOneColumn(property))
        //        {
        //            grid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Pixel);
        //            grid.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Pixel);
        //        }
        //        else
        //        {
        //            grid.ColumnDefinitions[1].Width = new GridLength(5, GridUnitType.Pixel);
        //            grid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
        //        }
        //    }
        //}
        //return editor;
    }
}