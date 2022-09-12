using SoftFluent.Windows.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Abstractions;
using PropertyGrid.WPF;

namespace SoftFluent.Windows {

   public partial class PropertyGrid : UserControl, IPropertyGrid {
      public static readonly RoutedEvent BrowseEvent = EventManager.RegisterRoutedEvent("Browse", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PropertyGrid));

      public static readonly DependencyProperty
          IsReadOnlyProperty =
          DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(PropertyGrid),
          new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, Helper.IsReadOnlyPropertyChanged)),

          ReadOnlyBackgroundProperty =
          DependencyProperty.Register("ReadOnlyBackground", typeof(Brush), typeof(PropertyGrid),
          new FrameworkPropertyMetadata(Brushes.LightSteelBlue, FrameworkPropertyMetadataOptions.AffectsRender)),

          SelectedObjectProperty =
          DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertyGrid),
          new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, Helper.SelectedObjectPropertyChanged)),

          ValueEditorTemplateSelectorProperty =
          DependencyProperty.Register("ValueEditorTemplateSelector", typeof(DataTemplateSelector), typeof(PropertyGrid),
              new FrameworkPropertyMetadata(null)),
          GroupByCategoryProperty =
             DependencyProperty.Register("GroupByCategory", typeof(bool), typeof(PropertyGrid), new PropertyMetadata(Helper.GroupByCategoryChanged));

      public static RoutedCommand
          BrowseCommand = new RoutedCommand(),
          EmptyGuidCommand = new RoutedCommand(),
          IncrementGuidCommand = new RoutedCommand(),
          NewGuidCommand = new RoutedCommand();

      private int _inheritanceLevel;
      private IActivator activatorHelper = new BaseActivator();
      public PropertyGrid() {
         InitializeComponent();
         AddCommandBindings();
         LevelComboBox.SelectionChanged += LevelComboBox_SelectionChanged;

         void AddCommandBindings() {
            CommandBindings.Add(new CommandBinding(NewGuidCommand, OnGuidCommandExecuted, OnGuidCommandCanExecute));
            CommandBindings.Add(new CommandBinding(EmptyGuidCommand, OnGuidCommandExecuted, OnGuidCommandCanExecute));
            CommandBindings.Add(new CommandBinding(IncrementGuidCommand, OnGuidCommandExecuted, OnGuidCommandCanExecute));
            CommandBindings.Add(new CommandBinding(BrowseCommand, OnBrowseCommandExecuted));
         }
      }

      public virtual double ChildEditorWindowOffset { get; set; } = 20;
      public virtual bool DecamelizePropertiesDisplayNames { get; set; } = true;

      public async Task InvokeAsync(Action action)
      {
          await this.Dispatcher.InvokeAsync(action);
      }

      public virtual string DefaultCategoryName { get; set; } = CategoryAttribute.Default.Category;

      public bool IsGrouping {
         get => PropertiesSource.GroupDescriptions.Count > 0;
         set => Helper.SetGroupByCategory(this, value);
      }

      public CollectionViewSource PropertiesSource => (CollectionViewSource)FindResource("PropertiesSource");
      private static HashSet<Type> CollectionEditorHasOnlyOneColumnList => Helper.GetTypes();

      public event EventHandler<PropertyGridEventArgs> PropertyChanged;

      public virtual bool CollectionEditorHasOnlyOneColumn(IPropertyGridProperty property) {
         if (property == null) {
            throw new ArgumentNullException("property");
         }

         if (SoftFluent.Abstractions.Helper.FromProperty(property) is IPropertyGridOptionsAttribute att) {
            return att.CollectionEditorHasOnlyOneColumn;
         }

         if (CollectionEditorHasOnlyOneColumnList.Contains(property.CollectionItemPropertyType)) {
            return true;
         }

         return !Helper.HasProperties(property.CollectionItemPropertyType);
      }

      public virtual PropertyGridEventArgs CreateEventArgs(IPropertyGridProperty property) {
         return activatorHelper.CreateInstance<PropertyGridEventArgs>(property);
      }

      public virtual IPropertyGridListSource CreatePropertyListSource(object value) {
         return activatorHelper.CreateInstance<PropertyGridListSource>(activatorHelper, this, value, _inheritanceLevel);
      }

      public virtual IPropertyGridListSource GetListSource() {
         return PropertiesSource.Source as IPropertyGridListSource;
      }

      public virtual IPropertyGridProperty GetProperty(string name) {
         if (name == null) {
            throw new ArgumentNullException("name");
         }

         if (GetListSource() is var context) {
            return context.GetByName(name);
         }

         return null;
      }

      public virtual FrameworkElement GetValueCellContent(object dataItem) {
         if (dataItem == null) {
            throw new ArgumentNullException("dataItem");
         }

         return GetValueColumn()?.GetCellContent(dataItem);
      }

      public virtual DataGridColumn GetValueColumn() {
         return Helper.DataGridColumn(PropertiesGrid);
      }

      public virtual void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e) {
         if (e?.PropertyName == null) {
            return;
         }

         if (GetProperty(e.PropertyName) is var property) {
            bool forceRaise = SoftFluent.Abstractions.Helper.FromProperty(property) is var options &&
                              options.ForcePropertyChanged;

            property.RefreshValueFromDescriptor(true, forceRaise, true);
            OnPropertyChanged(this, CreateEventArgs(property));
         }
      }

      public virtual void OnToggleButtonIsCheckedChanged(object sender, RoutedEventArgs e) {
         Helper2.Update(e);
      }

      public void RefreshComboBox() {
         Type[] types = SelectedObject.GetType().Inheritance().ToArray();
         LevelComboBox.ItemsSource = types;
      }

      public virtual async void RefreshSelectedObject() {
         if (SelectedObject == null) {
            return;
         }

         object selected = SelectedObject;
         IPropertyGridListSource source = await Task.Run(() => CreatePropertyListSource(selected));
         PropertiesSource.Source = source;
      }

      public virtual bool? ShowEditor(IPropertyGridProperty property, object parameter) {
         if (property == null) {
            throw new ArgumentNullException("property");
         }

         if (GetEditor(property, parameter) is Window editor) {
            return Helper2.ShowEditor(property, editor);
         }
         return null;
      }

      public virtual void UpdateCellBindings(IPropertyGridProperty dataItem, string childName, Func<Binding, bool> where, Action<BindingExpression> action) {
         Helper.UpdateBindings(this, dataItem, childName, where, action);
      }


      protected virtual Window GetEditor(IPropertyGridProperty property, object parameter) {
         return Helper2.GetEditor(this, property, parameter);
      }

      protected virtual void OnBrowseCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
         RoutedEventArgs browse = new RoutedEventArgs(BrowseEvent, e.OriginalSource);
         RaiseEvent(browse);
         if (browse.Handled) {
            return;
         }

         if (PropertyGrid.FromEvent(e) is IPropertyGridProperty property) {
            property.Executed(sender, e);
            if (!e.Handled) {
               ShowEditor(property, e.Parameter);
            }
         }
      }
      public static IPropertyGridProperty FromEvent(RoutedEventArgs e) {
         if (e == null) {
            return null;
         }

         FrameworkElement fe = e.OriginalSource as FrameworkElement;
         if (fe == null) {
            return null;
         }

         return fe.DataContext as IPropertyGridProperty;
      }

      protected virtual void OnEditorSelectorSelectionChanged(object sender, SelectionChangedEventArgs e) {
         Helper.OnEditorSelectorSelectionChanged(this, "CollectionEditorPropertiesGrid", sender, e);
      }

      protected virtual void OnEditorWindowCloseCanExecute(object sender, CanExecuteRoutedEventArgs e) {
         if (sender is Window window) {
            if (window.DataContext is IPropertyGridProperty prop) {
               prop.CanExecute(sender, e);
               if (e.Handled) {
                  return;
               }
            }
         }

         e.CanExecute = true;
      }

      protected virtual void OnEditorWindowCloseExecuted(object sender, ExecutedRoutedEventArgs e) {
         if (sender is Window window) {
            if (window.DataContext is IPropertyGridProperty prop) {
               prop.Executed(sender, e);
               if (e.Handled) {
                  return;
               }
            }
            window.Close();
         }
      }

      protected virtual void OnEditorWindowSaveCanExecute(object sender, CanExecuteRoutedEventArgs e) {
         if (sender is Window window && window.DataContext is IPropertyGridProperty prop) {
            prop.CanExecute(sender, e);
            if (e.Handled) {
               return;
            }
         }
         e.CanExecute = true;
      }

      protected virtual void OnEditorWindowSaveExecuted(object sender, ExecutedRoutedEventArgs e) {
         if (sender is Window window && window.DataContext is IPropertyGridProperty prop) {
            prop.Executed(sender, e);
         }
      }

      protected virtual void OnGuidCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
         if (PropertyGrid.FromEvent(e) is IPropertyGridProperty property &&
             (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?))) {
            e.CanExecute = true;
         }
      }

      protected virtual void OnGuidCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
         Helper2.ChangeText(e);
      }

      protected virtual void OnPropertyChanged(object sender, PropertyGridEventArgs e) {
         if (PropertyChanged is EventHandler<PropertyGridEventArgs> handler) {
            handler(sender, e);
         }
      }

      protected virtual void OnUIElementPreviewKeyUp(object sender, KeyEventArgs e) {
         Helper2.OnUIElementPreviewKeyUp(e);
      }

      #region DependencyProperties

      public bool GroupByCategory {
         get => (bool)GetValue(GroupByCategoryProperty);
         set => SetValue(GroupByCategoryProperty, value);
      }

      public bool IsReadOnly {
         get => (bool)GetValue(IsReadOnlyProperty);
         set => SetValue(IsReadOnlyProperty, value);
      }


      public Brush ReadOnlyBackground {
         get => (Brush)GetValue(ReadOnlyBackgroundProperty);
         set => SetValue(ReadOnlyBackgroundProperty, value);
      }

      public object SelectedObject {
         get => GetValue(SelectedObjectProperty);
         set => SetValue(SelectedObjectProperty, value);
      }

      public DataTemplateSelector ValueEditorTemplateSelector {
         get => (DataTemplateSelector)GetValue(ValueEditorTemplateSelectorProperty);
         set => SetValue(ValueEditorTemplateSelectorProperty, value);
      }

      public event RoutedEventHandler Browse {
         add { AddHandler(BrowseEvent, value); }
         remove { RemoveHandler(BrowseEvent, value); }
      }

      #endregion DependencyProperties

      private void LevelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
         _inheritanceLevel = Array.IndexOf((Array)LevelComboBox.ItemsSource, e.AddedItems[0]);
         RefreshSelectedObject();
      }
   }
}