using Abstractions;
using PropertyGrid.Abstractions;
using PropertyGrid.Demo.Model;
using SoftFluent.Windows;
using SoftFluent.Windows.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Helper;

namespace PropertyGrid.WPF.Demo
{
    /// <summary>
    /// Interaction logic for PropertyView.xaml
    /// </summary>
    public partial class PropertyView : UserControl
    {
        public PropertyView()
        {
            InitializeComponent();
            ViewModelTree.Engine = new Engine();
            this.Loaded += PropertyView_Loaded;
        }

        private void PropertyView_Loaded(object sender, RoutedEventArgs e)
        {
            this.PropertyTree.SelectedObject = this.DataContext;

        }

        private void PropertyTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is IProperty property)
            {
                ViewModelTree.SelectedObject = property;
            }
        }


        public class Engine : IPropertyGridEngine
        {

            public Engine()
            {
            }

            public IPropertyNode Convert(object data)
            {
                if (data is IGuid guid)
                {
                    return new PropertyNode(guid.Guid) { Data = data, Predicates = new ViewModelPredicate() };
                }
                throw new Exception(" 4 wewfwe");

            }

            public static Engine Instance { get; } = new Engine();
        }

        public class ViewModelPredicate : DescriptorFilters
        {

            List<Predicate<PropertyDescriptor>> predicates;
            public ViewModelPredicate()
            {

                predicates = new(){
                new Predicate<PropertyDescriptor>(descriptor=>
            {
                   return descriptor.PropertyType==typeof(IViewModel);
            }) };
            }

            public override IEnumerator<Predicate<PropertyDescriptor>> GetEnumerator()
            {
                return predicates.GetEnumerator();
            }
        }

        private void refresh_click(object sender, RoutedEventArgs e)
        {
            var treeView = new TreeView { };
            var property = PropertyTree.Source as PropertyNode;
            Create(treeView.Items, property);
            ContentGrid.Children.Add(treeView);
        }


        static void Create(ItemCollection items, PropertyNode property)
        {
            foreach (var item in property.Children)
            {
                if (item is PropertyBase node)
                {
                    ItemsPanelTemplate? panelTemplate = default;
                    DataTemplate? headerTemplate = default;
                    if (node.ViewModel is ViewModel viewModel)
                    {
                        panelTemplate = (ItemsPanelTemplate)Application.Current.TryFindResource(viewModel.Panel.Type);
                        if (viewModel.Template.DataTemplateKey != null)
                            headerTemplate = (DataTemplate)Application.Current.TryFindResource(viewModel.Template.DataTemplateKey);
                    }
                    var treeViewItem = new TreeViewItem() { Header = node.Name, HeaderTemplate = headerTemplate, ItemsPanel = panelTemplate };
                    //treeViewItem.ItemsPanel =
                    items.Add(treeViewItem);
                    Create(treeViewItem.Items, node);
                }
            }
        }
    }
}
