using System;
using System.Windows;
using System.Windows.Controls;
using Abstractions;
using SoftFluent.Windows.Utilities;
using Utilities;

namespace SoftFluent.Windows
{
    public class PropertyGridOptionsAttributeHelper
    {

        public static DataTemplate SelectTemplate(IProperty property, object item, DependencyObject container)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            //IPropertyGridOptionsAttribute att = SoftFluent.Abstractions.Helper.FromProperty(property);
            //if (att == null)
            //    return null;

            if (property.TemplateKey != null)
            {
                if (Application.Current != null)
                {
                    DataTemplate dt = (DataTemplate)Application.Current.TryFindResource(property.TemplateKey);
                    if (dt != null)
                        return dt;
                }

                var fe = container as FrameworkElement;
                if (fe != null)
                {
                    var dt = (DataTemplate)fe.TryFindResource(property.TemplateKey);
                    if (dt != null)
                        return dt;
                }

                return null;
            }

            //if (att.EditorType != null)
            //{
            //    object editor = Activator.CreateInstance(att.EditorType);
            //    if (att.EditorDataTemplateSelectorPropertyPath != null)
            //    {
            //        var dts = (DataTemplateSelector)DataBindingEvaluator.GetPropertyValue(editor, att.EditorDataTemplateSelectorPropertyPath);
            //        return dts != null ? dts.SelectTemplate(item, container) : null;
            //    }

            //    if (att.EditorDataTemplatePropertyPath != null)
            //        return (DataTemplate)DataBindingEvaluator.GetPropertyValue(editor, att.EditorDataTemplatePropertyPath);

            //    var cc = editor as ContentControl;
            //    if (cc != null)
            //    {
            //        if (cc.ContentTemplateSelector != null)
            //        {
            //            DataTemplate template = cc.ContentTemplateSelector.SelectTemplate(item, container);
            //            if (template != null)
            //                return template;
            //        }

            //        return cc.ContentTemplate;
            //    }

            //    var cp = editor as ContentPresenter;
            //    if (cp != null)
            //    {
            //        if (cp.ContentTemplateSelector != null)
            //        {
            //            DataTemplate template = cp.ContentTemplateSelector.SelectTemplate(item, container);
            //            if (template != null)
            //                return template;
            //        }

            //        return cp.ContentTemplate;
            //    }
            //}
            return null;
        }


    }
}