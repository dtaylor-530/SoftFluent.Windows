using PropertyGrid.Abstractions;
using SoftFluent.Windows;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utility.WPF.Controls.Trees;
using static Evan.Wpf.DependencyHelper;

namespace PropertyGrid.WPF
{
    public partial class PropertyTree : UserControl
    {
        public static readonly DependencyProperty


            SelectedObjectProperty =
            DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertyTree),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, Helper.SelectedObjectPropertyChanged)),


            EngineProperty =
            DependencyProperty.Register("Engine", typeof(IPropertyGridEngine), typeof(PropertyTree),
               new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, EnginePropertyChanged)),
            TemplateSelectorProperty = Register(),
            SelectionChangedProperty = Register(),
            SourceProperty = Register();

        private static void EnginePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertyTree propertyGrid && e.NewValue is IPropertyGridEngine _)
            {
                propertyGrid.RefreshSelectedObject();
            }
        }

        public static RoutedCommand
            BrowseCommand = new(), NavigateCommand = new(), RefreshCommand = new();


        private IPropertyGridEngine engine;
        public SynchronizationContext context;

        public PropertyTree()
        {
            InitializeComponent();
            Tree.SelectedItemChanged += Tree_SelectedItemChanged;

            context = SynchronizationContext.Current ?? throw new Exception("4g4e&&&&&");

        }


        private void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        public virtual string DefaultCategoryName { get; set; } = CategoryAttribute.Default.Category;

        private void Tree_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            TreeListView _ListView = sender as TreeListView;
            var _ActualWidth = _ListView.ActualWidth - SystemParameters.VerticalScrollBarWidth - _ListView.Columns[0].Width;
            var separateWidth = (_ActualWidth * 1d) / (_ListView.Columns.Count-1);
            for (int i = 1; i < _ListView.Columns.Count; i++)
            {
                _ListView.Columns[i].Width = separateWidth;
            }
        }

        public virtual async void RefreshSelectedObject()
        {
            engine = Engine;
            if (engine == null || SelectedObject == null)
            {
                return;
            }

            Source = engine.Convert(SelectedObject);
            Tree.ItemsSource = new[] { Source };

        }


        #region DependencyProperties

        public IPropertyGridEngine Engine
        {
            get { return (IPropertyGridEngine)GetValue(EngineProperty); }
            set { SetValue(EngineProperty, value); }
        }

        public object SelectedObject
        {
            get => GetValue(SelectedObjectProperty);
            set => SetValue(SelectedObjectProperty, value);
        }

        public DataTemplateSelector TemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(TemplateSelectorProperty); }
            set { SetValue(TemplateSelectorProperty, value); }
        }

        public ICommand SelectionChanged
        {
            get { return (ICommand)GetValue(SelectionChangedProperty); }
            set { SetValue(SelectionChangedProperty, value); }
        }

        public IPropertyNode Source
        {
            get { return (IPropertyNode)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }



        #endregion DependencyProperties

    }

}