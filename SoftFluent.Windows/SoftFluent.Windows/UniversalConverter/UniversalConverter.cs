﻿using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace SoftFluent.Windows
{
    /// <summary>
    /// Provides a converter to convert any object into another object, using a switch/case paradigm.
    /// </summary>
    public class UniversalConverter : IValueConverter
    {
        private readonly ObservableCollection<UniversalConverterCase> _cases = new ObservableCollection<UniversalConverterCase>();

        /// <summary>
        /// Gets or sets the default value to use if no case matches.
        /// </summary>
        /// <value>
        /// The default value to use if no case matches.
        /// </value>
        public virtual object DefaultValue { get; set; }

        /// <summary>
        /// Gets the list of cases.
        /// </summary>
        /// <value>
        /// The list of cases.
        /// </value>
        public virtual ObservableCollection<UniversalConverterCase> Switch => _cases;

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (_cases.Count == 0)
            {
                return ConversionHelper.ChangeType(value, targetType, culture);
            }

            foreach (UniversalConverterCase c in _cases)
            {
                if (c.Matches(value, parameter, culture))
                {
                    if ((c.Options & UniversalConverterOptions.ConvertedValueIsConverterParameter) == UniversalConverterOptions.ConvertedValueIsConverterParameter)
                    {
                        return ConversionHelper.ChangeType(parameter, targetType, culture);
                    }

                    return ConversionHelper.ChangeType(c.ConvertedValue, targetType, null, culture);
                }
            }

            return ConversionHelper.ChangeType(DefaultValue, targetType, null, culture);
        }

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public virtual object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert(value, targetType, parameter, CultureInfoFromName(language));
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConversionHelper.ChangeType(parameter, targetType, null, culture);
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public virtual object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ConvertBack(value, targetType, parameter, CultureInfoFromName(language));
        }

        internal static CultureInfo CultureInfoFromName(string language)
        {
            if (language == null)
            {
                return null;
            }

            try
            {
                return new CultureInfo(language);
            }
            catch
            {
                return null;
            }
        }
    }
}