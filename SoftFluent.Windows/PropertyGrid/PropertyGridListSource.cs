using SoftFluent.Windows.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Abstractions;
using PropertyGrid.Infrastructure;
using Utilities;
using PropertyGrid.Abstractions;

namespace SoftFluent.Windows
{

    public class PropertyGridListSource : IPropertyEngine, IListSource
    {
        private readonly IActivator _activator;
        private readonly IPropertyGridOptions options;
        private readonly ObservableCollection<IProperty> _properties = new ();

        public PropertyGridListSource(IActivator activator, IPropertyGridOptions options)
        {
            _activator = activator;
            this.options = options;

            options.Data = options.Data ?? throw new ArgumentNullException("data");

            UpdateProperties();

            void UpdateProperties()
            {
                foreach (PropertyGridProperty prop in Properties())
                {
                    _properties.Add(prop);
                }
            }
        }

        #region IListSource

        bool IListSource.ContainsListCollection => false;

        IList IListSource.GetList()
        {
            return _properties;
        }
        #endregion

        public IProperty GetProperty(string name)
        {
            return _properties.FirstOrDefault(p => p.Name.EqualsIgnoreCase(name));
        }

        protected virtual PropertyGridProperty CreateProperty(PropertyDescriptor descriptor)
        {
            return MyHelper.CreateProperty(options, descriptor, _activator);
        }

        protected virtual IEnumerable<PropertyGridProperty> Properties()
        {
            Type highestType = options.Data.GetType();

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(options.Data).Cast<PropertyDescriptor>().OrderBy(d => d.Name))
            {
                int level = descriptor.ComponentType.InheritanceLevel(highestType);

                if (level <= options.InheritanceLevel &&
                    descriptor.IsBrowsable &&
                    CreateProperty(descriptor) is PropertyGridProperty property)
                {
                    yield return property;
                }
            }
        }
    }

    static class MyHelper
    {
        public static PropertyGridProperty CreateProperty(IPropertyGridOptions gridOptions, PropertyDescriptor descriptor, IActivator activator)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException("descriptor");
            }

            bool forceReadWrite = false;
            PropertyGridProperty? property = null;
            PropertyGridOptionsAttribute options = descriptor.GetAttribute<PropertyGridOptionsAttribute>();
            if (options != null)
            {
                forceReadWrite = options.ForceReadWrite;
                if (options.PropertyType != null)
                {
                    property = (PropertyGridProperty)activator.CreateInstance(options.PropertyType, gridOptions);
                }
            }

            if (property == null)
            {
                options = descriptor.PropertyType.GetAttribute<PropertyGridOptionsAttribute>();
                if (options != null)
                {
                    if (!forceReadWrite)
                    {
                        forceReadWrite = options.ForceReadWrite;
                    }

                    if (options.PropertyType != null)
                    {
                        property = (PropertyGridProperty)activator.CreateInstance(options.PropertyType, gridOptions);
                    }
                }
            }

            if (property == null)
            {
                property = new PropertyGridProperty(gridOptions);
            }

            Describe(property, descriptor, gridOptions.DefaultCategoryName, gridOptions.DecamelizePropertiesDisplayNames);

            if (forceReadWrite)
            {
                property.IsReadOnly = false;
            }

            property.OnDescribed();
            property.RefreshValueFromDescriptor(true, false, true);
            return property;
        }


        public static void Describe(PropertyGridProperty property, PropertyDescriptor descriptor, string defaultCategoryName, bool decamelizePropertiesDisplayNames)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            if (descriptor == null)
            {
                throw new ArgumentNullException("descriptor");
            }

            property.Descriptor = descriptor;
            property.Name = descriptor.Name;
            property.PropertyType = descriptor.PropertyType;

            // unset by default. conversion service does the default job
            //property.Converter = descriptor.Converter;

            property.Category =
                string.IsNullOrWhiteSpace(descriptor.Category) ||
                Extensions.EqualsIgnoreCase(descriptor.Category, CategoryAttribute.Default.Category)
                    ? defaultCategoryName
                    : descriptor.Category;
            property.IsReadOnly = descriptor.IsReadOnly;
            property.Description = descriptor.Description;
            property.DisplayName = descriptor.DisplayName;

            if (decamelizePropertiesDisplayNames && property.DisplayName == descriptor.Name)
            {
                property.DisplayName = BaseDecamelizer.Decamelize(property.DisplayName);
            }

            property.IsEnum = descriptor.PropertyType.IsEnum;
            property.IsFlagsEnum = descriptor.PropertyType.IsEnum && Extensions.IsFlagsEnum(descriptor.PropertyType);

            PropertyGridOptionsAttribute options = Extensions.GetAttribute<PropertyGridOptionsAttribute>((MemberDescriptor)descriptor);
            if (options != null)
            {
                if (options.SortOrder != 0)
                {
                    property.SortOrder = options.SortOrder;
                }

                property.IsEnum = options.IsEnum;
                property.IsFlagsEnum = options.IsFlagsEnum;
            }

            DefaultValueAttribute att = Extensions.GetAttribute<DefaultValueAttribute>((MemberDescriptor)descriptor);
            if (att != null)
            {
                property.HasDefaultValue = true;
                property.DefaultValue = att.Value;
            }
            else if (options != null)
            {
                if (options.HasDefaultValue)
                {
                    property.HasDefaultValue = true;
                    property.DefaultValue = options.DefaultValue;
                }
                else
                {
                    if (TryGetDefaultValue(options, out string defaultValue))
                    {
                        property.DefaultValue = defaultValue;
                        property.HasDefaultValue = true;
                    }
                }
            }

            property.Attributes.AddDynamicProperties(descriptor.Attributes.OfType<PropertyGridAttribute>().ToArray());
            property.TypeAttributes.AddDynamicProperties(Extensions.GetAttributes<PropertyGridAttribute>(descriptor.PropertyType)
                .ToArray());

            static bool TryGetDefaultValue(PropertyGridOptionsAttribute options, out string value)
            {
                value = null;
                if (options == null || !options.IsEnum && !options.IsFlagsEnum)
                {
                    return false;
                }

                if (options.EnumNames != null && options.EnumNames.Length > 0)
                {
                    value = options.EnumNames.First();
                    return true;
                }
                return false;
            }
        }
    }
}