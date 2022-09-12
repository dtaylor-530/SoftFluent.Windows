using SoftFluent.Windows.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using Abstractions;
using PropertyGrid.WPF;
using Utilities;

namespace SoftFluent.Windows {
   public class PropertyGridComboBoxExtension : MarkupExtension {
      //static PropertyGridComboBoxExtension() {
      //   _activatorHelper = new ActivatorHelper(new BaseActivator());
      //}

      protected class Converter : IValueConverter {
         public Converter(PropertyGridComboBoxExtension extension) {
            Extension = extension;

         }

         public PropertyGridComboBoxExtension Extension { get;  }

         public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            IPropertyGridProperty property = value as IPropertyGridProperty;
            if (property != null) {
               return property.BuildItems(targetType, parameter, culture);
            }

            return value;
         }

         public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value;
         }
      }

      private readonly Binding _binding;

      public PropertyGridComboBoxExtension(Binding binding) {
         _binding = binding; // may be null
      }

      public override object ProvideValue(IServiceProvider serviceProvider) {
         if (_binding == null) {
            throw new InvalidOperationException();
         }

         _binding.Converter = new Converter(this);
         return _binding.ProvideValue(serviceProvider);
      }
   }
}