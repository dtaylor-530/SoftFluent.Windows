using Abstractions;
using Evan.Wpf;
using PropertyGrid.Abstractions;
using SoftFluent.Windows;
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

namespace PropertyGrid.WPF
{
    //public class PropertyGridOptions : IPropertyGridOptions
    //{
    //    public int InheritanceLevel { get; set; }
    //    public bool IsReadOnly { get; set; }
    //    public object Data { get; set; }

    //    public string DefaultCategoryName { get; set; }

    //    public bool DecamelizePropertiesDisplayNames { get; set; }
    //}

    public partial class PropertyTree : UserControl
    {
        public static readonly DependencyProperty


            SelectedObjectProperty =
            DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertyTree),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, Helper.SelectedObjectPropertyChanged)),


            EngineProperty =
            DependencyProperty.Register("Engine", typeof(IPropertyGridEngine), typeof(PropertyTree),
               new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, EnginePropertyChanged));

        public static readonly DependencyProperty TemplateSelectorProperty = DependencyHelper.Register<DataTemplateSelector>();




        private static void EnginePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertyTree propertyGrid && e.NewValue is IPropertyGridEngine _)
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

        public PropertyTree()
        {
            InitializeComponent();


            context = SynchronizationContext.Current ?? throw new Exception("4g4e&&&&&");

        }


        //public IPropertyGridOptions Options => new PropertyGridOptions
        //{
        //    IsReadOnly = this.IsReadOnly,
        //    InheritanceLevel = _inheritanceLevel,
        //    Data = SelectedObject,
        //    DecamelizePropertiesDisplayNames = this.DecamelizePropertiesDisplayNames,
        //    DefaultCategoryName = this.DefaultCategoryName
        //};




        //public async Task InvokeAsync(Action action)
        //{
        //    await this.Dispatcher.InvokeAsync(action);
        //}

        public virtual string DefaultCategoryName { get; set; } = CategoryAttribute.Default.Category;






        public virtual async void RefreshSelectedObject()
        {
            engine = Engine;
            if (engine == null || SelectedObject == null)
            {
                return;
            }

            //var options = Options;

            source =/* await Task.Run(() => */engine.Convert(SelectedObject);//);
            Tree.ItemsSource = new[] { source };

            //PropertiesSource.Source = new ListSource(source, context);
        }

        //public virtual void UpdateCellBindings(IProperty dataItem, string childName, Func<Binding, bool> where, Action<BindingExpression> action)
        //{
        //    Helper.UpdateBindings(this, dataItem, childName, where, action);
        //}



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


        #endregion DependencyProperties




    }
    
}