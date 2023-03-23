using Abstractions;
using Evan.Wpf;
using PropertyGrid.Abstractions;
using PropertyGrid.WPF;
using SoftFluent.Windows.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Utilities;

namespace SoftFluent.Windows
{
    public class PropertyGridOptions : IPropertyGridOptions
    {
        public int InheritanceLevel { get; set; }
        public bool IsReadOnly { get; set; }
        public object Data { get; set; }

        public string DefaultCategoryName { get; set; }

        public bool DecamelizePropertiesDisplayNames { get; set; }
    }

    public partial class PropertyGrid : UserControl
    {

        public static readonly RoutedEvent BrowseEvent = EventManager.RegisterRoutedEvent("Browse", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PropertyGrid));
        public static readonly RoutedEvent NavigateEvent = EventManager.RegisterRoutedEvent("Navigate", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PropertyGrid));

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
               DependencyProperty.Register("GroupByCategory", typeof(bool), typeof(PropertyGrid), new PropertyMetadata(Helper.GroupByCategoryChanged)),

            EngineProperty =
            DependencyProperty.Register("Engine", typeof(IPropertyGridEngine), typeof(PropertyGrid),
               new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, EnginePropertyChanged));

        public static readonly DependencyProperty TemplateSelectorProperty = DependencyHelper.Register<DataTemplateSelector>();




        private static void EnginePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertyGrid propertyGrid && e.NewValue is IPropertyGridEngine _)
            {
                propertyGrid.RefreshSelectedObject();
            }
        }

        public static RoutedCommand
            BrowseCommand = new RoutedCommand(),
            NavigateCommand = new RoutedCommand();


        private int _inheritanceLevel;
        private IPropertyGridEngine engine;
        private IPropertySource source;
        public SynchronizationContext context;

        public PropertyGrid()
        {
            InitializeComponent();
            AddCommandBindings();
            LevelComboBox.SelectionChanged += LevelComboBox_SelectionChanged;
            context = SynchronizationContext.Current ?? throw new Exception("4g4e&&&&&");
            void AddCommandBindings()
            {
                //CommandBindings.Add(new CommandBinding(NewGuidCommand, OnGuidCommandExecuted, OnGuidCommandCanExecute));
                //CommandBindings.Add(new CommandBinding(EmptyGuidCommand, OnGuidCommandExecuted, OnGuidCommandCanExecute));
                //CommandBindings.Add(new CommandBinding(IncrementGuidCommand, OnGuidCommandExecuted, OnGuidCommandCanExecute));
                CommandBindings.Add(new CommandBinding(BrowseCommand, OnBrowseCommandExecuted));
                CommandBindings.Add(new CommandBinding(NavigateCommand, OnNavigateCommandExecuted));
            }
        }


        public IPropertyGridOptions Options => new PropertyGridOptions
        {
            IsReadOnly = this.IsReadOnly,
            InheritanceLevel = _inheritanceLevel,
            Data = SelectedObject,
            DecamelizePropertiesDisplayNames = this.DecamelizePropertiesDisplayNames,
            DefaultCategoryName = this.DefaultCategoryName
        };


        public virtual double ChildEditorWindowOffset { get; set; } = 20;
        public virtual bool DecamelizePropertiesDisplayNames { get; set; } = true;

        public bool IsGrouping
        {
            get => PropertiesSource.GroupDescriptions.Count > 0;
            set => Helper.SetGroupByCategory(this, value);
        }


        //public async Task InvokeAsync(Action action)
        //{
        //    await this.Dispatcher.InvokeAsync(action);
        //}

        public virtual string DefaultCategoryName { get; set; } = CategoryAttribute.Default.Category;



        public CollectionViewSource PropertiesSource => (CollectionViewSource)FindResource("PropertiesSource");
        //private static HashSet<Type> CollectionEditorHasOnlyOneColumnList => Helper.GetTypes();

        //public virtual bool CollectionEditorHasOnlyOneColumn(IProperty property)
        //{
        //    if (property == null)
        //    {
        //        throw new ArgumentNullException("property");
        //    }

        //    if (SoftFluent.Abstractions.Helper.FromProperty(property) is IPropertyGridOptionsAttribute att)
        //    {
        //        return att.CollectionEditorHasOnlyOneColumn;
        //    }

        //    if (CollectionEditorHasOnlyOneColumnList.Contains(property.CollectionItemPropertyType))
        //    {
        //        return true;
        //    }

        //    return !Helper.HasProperties(property.CollectionItemPropertyType);
        //}

        //public virtual IListSource GetListSource()
        //{
        //    return PropertiesSource.Source as IListSource;
        //}

        public virtual IProperty GetProperty(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            return source.GetProperty(name);

        }

        public virtual FrameworkElement GetValueCellContent(object dataItem)
        {
            if (dataItem == null)
            {
                throw new ArgumentNullException("dataItem");
            }

            return GetValueColumn()?.GetCellContent(dataItem);
        }

        public virtual DataGridColumn GetValueColumn()
        {
            return Helper.DataGridColumn(PropertiesGrid);
        }

        public virtual void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e?.PropertyName == null)
            {
                return;
            }

            if (GetProperty(e.PropertyName) is var property)
            {
                source.RefreshProperty(property);
                //OnPropertyChanged(this, CreateEventArgs(property));
            }

            //static bool ForceRaise(IProperty property)
            //{
            //    return SoftFluent.Abstractions.Helper.FromProperty(property) is var options &&
            //            options.ForcePropertyChanged;
            //}
        }


        public virtual void OnToggleButtonIsCheckedChanged(object sender, RoutedEventArgs e)
        {
            Helper2.Update(e);
        }

        public void RefreshComboBox()
        {
            Type[] types = SelectedObject.GetType().Inheritance().ToArray();
            LevelComboBox.ItemsSource = types;
        }

        public virtual async void RefreshSelectedObject()
        {
            engine = Engine;
            if (engine == null || SelectedObject == null)
            {
                return;
            }

            var options = Options;
            ValueColumn.CellTemplateSelector = TemplateSelector;
            source =/* await Task.Run(() => */engine.Convert(options);//);
            PropertiesSource.Source = new ListSource(source, context);
        }

        public virtual void UpdateCellBindings(IProperty dataItem, string childName, Func<Binding, bool> where, Action<BindingExpression> action)
        {
            Helper.UpdateBindings(this, dataItem, childName, where, action);
        }


        protected virtual void OnBrowseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RoutedEventArgs browse = new RoutedEventArgs(BrowseEvent, e.OriginalSource);
            this.RaiseEvent(browse);
            if (browse.Handled)
            {
                return;
            }

            if (PropertyGrid.FromEvent(e) is IProperty property)
            {
                //property.Executed(sender, e);
                if (!e.Handled)
                {
                    ShowEditor(property);
                }
            }
        }

        protected virtual void OnNavigateCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RoutedEventArgs browse = new RoutedEventArgs(NavigateEvent, e.OriginalSource);
            this.RaiseEvent(browse);
            if (browse.Handled)
            {
                return;
            }

            if (PropertyGrid.FromEvent(e) is IProperty property)
            {
                //property.Executed(sender, e);
                if (!e.Handled)
                {
                    SelectedObject = property.Value;
                }
            }
        }

        public static IProperty FromEvent(RoutedEventArgs e)
        {
            if (e == null)
            {
                return null;
            }

            FrameworkElement fe = e.OriginalSource as FrameworkElement;
            if (fe == null)
            {
                return null;
            }

            return fe.DataContext as IProperty;
        }


        //protected virtual void OnGuidCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    if (PropertyGrid.FromEvent(e) is IProperty property &&
        //        (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?)))
        //    {
        //        e.CanExecute = true;
        //    }
        //}

        //protected virtual void OnGuidCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        //{
        //    Helper2.ChangeText(e);
        //}

        protected virtual void OnUIElementPreviewKeyUp(object sender, KeyEventArgs e)
        {
            Helper2.OnUIElementPreviewKeyUp(e);
        }

        #region DependencyProperties

        public IPropertyGridEngine Engine
        {
            get { return (IPropertyGridEngine)GetValue(EngineProperty); }
            set { SetValue(EngineProperty, value); }
        }

        public bool GroupByCategory
        {
            get => (bool)GetValue(GroupByCategoryProperty);
            set => SetValue(GroupByCategoryProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public Brush ReadOnlyBackground
        {
            get => (Brush)GetValue(ReadOnlyBackgroundProperty);
            set => SetValue(ReadOnlyBackgroundProperty, value);
        }

        public object SelectedObject
        {
            get => GetValue(SelectedObjectProperty);
            set => SetValue(SelectedObjectProperty, value);
        }

        public DataTemplateSelector ValueEditorTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(ValueEditorTemplateSelectorProperty);
            set => SetValue(ValueEditorTemplateSelectorProperty, value);
        }

        public event RoutedEventHandler Browse
        {
            add { AddHandler(BrowseEvent, value); }
            remove { RemoveHandler(BrowseEvent, value); }
        }

        public event RoutedEventHandler Navigate
        {
            add { AddHandler(NavigateEvent, value); }
            remove { RemoveHandler(NavigateEvent, value); }
        }


        public DataTemplateSelector TemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(TemplateSelectorProperty); }
            set { SetValue(TemplateSelectorProperty, value); }
        }


        #endregion DependencyProperties


        #region editor

        public virtual bool? ShowEditor(IProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            if (EditorSelector.GetEditor(this, property) is Window editor)
            {
                return EditorSelector.ShowEditor(property, editor);
            }
            return null;
        }




        protected virtual void OnEditorSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Helper.OnEditorSelectorSelectionChanged(this, "CollectionEditorPropertiesGrid", sender, e);
        }

        protected virtual void OnEditorWindowCloseCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is Window window)
            {
                if (window.DataContext is IProperty prop)
                {
                    //prop.CanExecute(sender, e);
                    if (e.Handled)
                    {
                        return;
                    }
                }
            }

            e.CanExecute = true;
        }

        protected virtual void OnEditorWindowCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is Window window)
            {
                if (window.DataContext is IProperty prop)
                {
                    //prop.Executed(sender, e);
                    if (e.Handled)
                    {
                        return;
                    }
                }
                window.Close();
            }
        }

        protected virtual void OnEditorWindowSaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is Window window && window.DataContext is IProperty prop)
            {
                // prop.CanExecute(sender, e);
                if (e.Handled)
                {
                    return;
                }
            }
            e.CanExecute = true;
        }

        protected virtual void OnEditorWindowSaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is Window window && window.DataContext is IProperty prop)
            {
                // prop.Executed(sender, e);
            }
        }


        #endregion


        private void LevelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _inheritanceLevel = Array.IndexOf((Array)LevelComboBox.ItemsSource, e.AddedItems[0]);
                RefreshSelectedObject();
            }
        }
    }

    public class ListSource : IListSource
    {
        SynchronizationContext context;
        private readonly IPropertySource propertySource;
        IDisposable disposable;
        ObservableCollection<IProperty> collection = new();

        public ListSource(IPropertySource propertySource, SynchronizationContext context)
        {
            this.propertySource = propertySource;
            this.context = context;
        }

        public bool ContainsListCollection => false;

        public IList GetList()
        {

            if (disposable == null)
                disposable = propertySource
                    .Properties()
                    .ObserveOn(context)
                    .Subscribe(a => collection.Add(a));

            return collection;
        }
    }
}