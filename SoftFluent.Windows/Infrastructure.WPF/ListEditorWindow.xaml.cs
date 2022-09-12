using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Abstractions;

namespace SoftFluent.Windows.Samples
{
    /// <summary>
    /// Interaction logic for AddressListEditorWindow.xaml
    /// </summary>
    public partial class ListEditorWindow : Window
    {
       public static readonly DependencyProperty ItemsSourceProperty = ItemsControl.ItemsSourceProperty.AddOwner(typeof(ListEditorWindow));
       private Grid? MainGrid;
       private ListBox EditorSelector;


      public ListEditorWindow()
        {
            InitializeComponent();

        }

      public IEnumerable ItemsSource {
         get { return (IEnumerable)GetValue(ItemsSourceProperty); }
         set { SetValue(ItemsSourceProperty, value); }
      }

      public override void OnApplyTemplate()
      {
         EditorSelector = this.GetTemplateChild("EditorSelector") as ListBox;
         base.OnApplyTemplate();
      }


      private void NewCommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var cvs = CollectionViewSource.GetDefaultView(EditorSelector.ItemsSource);
            if (cvs == null)
                return;

            if (cvs.SourceCollection is IList collection)
            {
               var itemType = collection.GetType().GetGenericArguments().Single();
               var newItem = Activator.CreateInstance(itemType);
               collection.Add(newItem);
               cvs.MoveCurrentToLast();
               EditorSelector.SelectedIndex =collection.Count-1;
            }
            else
            {
               throw new Exception("FDS DDss");
            }
        }

        private void DeleteCommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
           if (EditorSelector == null)
              return;
            var cvs = CollectionViewSource.GetDefaultView(EditorSelector.ItemsSource);
            if (cvs == null)
                return;

            e.CanExecute = cvs.CurrentItem != null;
        }

        private void DeleteCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var cvs = CollectionViewSource.GetDefaultView(EditorSelector.ItemsSource);
            if (cvs == null) 
                return;

            var currentItem = cvs.CurrentItem as object;
            if (currentItem == null) 
                return;

            var collection = cvs.SourceCollection as IList;
            collection?.Remove(currentItem);
        }

        protected virtual void OnEditorWindowCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
           if (sender is Window window)
           {
              if (window is { DataContext: IExecute prop })
              {
                 prop.Executed(sender, e);
                 if (e.Handled)
                    return;
              }
              window.Close();
           }
        }

        protected virtual void OnEditorWindowCloseCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
           if (sender is Window { DataContext: IExecute prop} )
            {
                prop.CanExecute(sender, e);
                if (e.Handled)
                    return;
            }
            e.CanExecute = true;
        }
    }
}
