using Abstractions;
using Evan.Wpf;
using PropertyGrid.Abstractions;
using PropertyGrid.WPF;
using PropertyGrid.WPF.Infrastructure;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace SoftFluent.Windows
    public partial class PropertyGrid : UserControl
    {

        public static readonly RoutedEvent BrowseEvent = EventManager.RegisterRoutedEvent("Browse", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PropertyGrid));
        public static readonly RoutedEvent NavigateEvent = EventManager.RegisterRoutedEvent("Navigate", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PropertyGrid));
        public static readonly RoutedEvent RefreshEvent = EventManager.RegisterRoutedEvent("Refresh", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PropertyGrid));

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
            BrowseCommand = new(), NavigateCommand = new(), RefreshCommand = new();


        private int _inheritanceLevel;
        private IPropertyGridEngine engine;
        private IEnumerable source;
        public SynchronizationContext context;

        public PropertyGrid()
        {
            InitializeComponent();
            AddCommandBindings();
            LevelComboBox.SelectionChanged += LevelComboBox_SelectionChanged;
            context = SynchronizationContext.Current ?? throw new Exception("4g4e&&&&&");
            void AddCommandBindings()
            {
                CommandBindings.Add(new CommandBinding(BrowseCommand, OnBrowseCommandExecuted));
                CommandBindings.Add(new CommandBinding(NavigateCommand, OnNavigateCommandExecuted));
                CommandBindings.Add(new CommandBinding(RefreshCommand, OnRefreshCommandExecuted));
            }
        }


        public virtual double ChildEditorWindowOffset { get; set; } = 20;
        public virtual bool DecamelizePropertiesDisplayNames { get; set; } = true;

        public bool IsGrouping
        {
            get => PropertiesSource.GroupDescriptions.Count > 0;
            set => Helper.SetGroupByCategory(this, value);
        }

        public virtual string DefaultCategoryName { get; set; } = CategoryAttribute.Default.Category;



        public CollectionViewSource PropertiesSource => (CollectionViewSource)FindResource("PropertiesSource");

        public virtual void OnToggleButtonIsCheckedChanged(object sender, RoutedEventArgs e)
        {
            Helper2.Update(e);
        }


        public virtual async void RefreshSelectedObject()
        {
            engine = Engine;
            if (engine == null || SelectedObject == null)
            {
                return;
            }

            //var options = Options;
            ValueColumn.CellTemplateSelector = TemplateSelector;
            source =/* await Task.Run(() => */engine.Convert(SelectedObject);//);
            PropertiesGrid.ItemsSource = source;
            ItemsControl.ItemsSource = source;
            //PropertiesSource.Source = new ListSource(source, context);
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

        protected virtual void OnRefreshCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RoutedEventArgs browse = new RoutedEventArgs(NavigateEvent, e.OriginalSource);
            this.RaiseEvent(browse);
            if (browse.Handled)
            {
                return;
            }

            ItemsControl.ItemTemplateSelector = null;
            ItemsControl.ItemTemplateSelector = MyDataTemplateSelector.Instance;
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

        public event RoutedEventHandler Refresh
        {
            add { AddHandler(RefreshEvent, value); }
            remove { RemoveHandler(RefreshEvent, value); }
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

}