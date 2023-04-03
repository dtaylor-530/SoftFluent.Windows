using System;
using System.Windows.Controls;
using System.Windows;

namespace BOT.WPF
{
    /// <summary>
    /// Class that helps the creation of control and data templates by using delegates.
    /// </summary>
    /// <remarks>
    /// Paulo Zemek
    /// <a href="https://stackoverflow.com/questions/5471405/create-datatemplate-in-code-behind"></a>
    /// </remarks>
    public static class TemplateGenerator
    {
        private sealed class TemplateGeneratorControl : ContentControl
        {
            internal static readonly DependencyProperty FactoryProperty = DependencyProperty.Register("Factory", typeof(Func<object>), typeof(TemplateGeneratorControl), new PropertyMetadata(null, FactoryChanged));

            private static void FactoryChanged(DependencyObject instance, DependencyPropertyChangedEventArgs args)
            {
                var control = (TemplateGeneratorControl)instance;
                var factory = (Func<object>)args.NewValue;
                control.Content = factory();
            }
        }

        /// <summary>
        /// Creates a data-template that uses the given delegate to create new instances.
        /// </summary>
        public static DataTemplate CreateDataTemplate(Func<object> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            var frameworkElementFactory = new FrameworkElementFactory(typeof(TemplateGeneratorControl));
            frameworkElementFactory.SetValue(TemplateGeneratorControl.FactoryProperty, factory);

            DataTemplate dataTemplate = new(typeof(DependencyObject))
            {
                VisualTree = frameworkElementFactory
            };
            return dataTemplate;
        }

        /// <summary>
        /// Creates a items-panel-template
        /// </summary>
        public static ItemsPanelTemplate CreateItemsPanelTemplate<TFrameworkElement>(Action<FrameworkElementFactory> action)
            where TFrameworkElement : FrameworkElement
        {
            return CreateItemsPanelTemplate(action, typeof(TFrameworkElement));
        }

        /// <summary>
        /// Creates a items-panel-template
        /// </summary>
        public static ItemsPanelTemplate CreateItemsPanelTemplate(Action<FrameworkElementFactory> action, Type type)
        {
            ItemsPanelTemplate itemsPanelTemplate = new()
            {
                VisualTree = new FrameworkElementFactory(type)
            };
            action(itemsPanelTemplate.VisualTree);
            return itemsPanelTemplate;
        }


        /// <summary>
        /// Creates a control-template
        /// </summary>
        public static ControlTemplate CreateControlTemplate<TFrameworkElement>(Func<object> factory)
            where TFrameworkElement : FrameworkElement
        {
            return CreateControlTemplate(factory, typeof(TFrameworkElement));
        }

        /// <summary>
        /// Creates a control-template
        /// </summary>
        public static ControlTemplate CreateControlTemplate(Func<object> factory, Type controlType)
        {
            if (controlType == null)
                throw new ArgumentNullException(nameof(controlType));

            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            var frameworkElementFactory = new FrameworkElementFactory(typeof(TemplateGeneratorControl));
            frameworkElementFactory.SetValue(TemplateGeneratorControl.FactoryProperty, factory);

            ControlTemplate controlTemplate = new(controlType)
            {
                VisualTree = frameworkElementFactory
            };
            return controlTemplate;
        }
    }
}
