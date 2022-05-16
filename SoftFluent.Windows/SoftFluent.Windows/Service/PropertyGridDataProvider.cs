using SoftFluent.Windows.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace SoftFluent.Windows
{
    public class PropertyGridDataProvider : IListSource
    {
        private readonly int _inheritanceLevel;

        public PropertyGridDataProvider(PropertyGrid grid, object data, int inheritanceLevel = 0)
        {
            if (grid == null)
            {
                throw new ArgumentNullException("grid");
            }

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            _inheritanceLevel = inheritanceLevel;

            Grid = grid;
            Data = data;
            Properties = new ObservableCollection<PropertyGridProperty>();
            ScanProperties();
        }

        bool IListSource.ContainsListCollection => false;
        public object Data { get; }
        public PropertyGrid Grid { get; }
        public virtual ObservableCollection<PropertyGridProperty> Properties { get; }

        public static void AddDynamicProperties(IEnumerable<PropertyGridAttribute> attributes,
            DynamicObject dynamicObject)
        {
            if (attributes == null || dynamicObject == null)
            {
                return;
            }

            foreach (PropertyGridAttribute pga in attributes)
            {
                if (string.IsNullOrWhiteSpace(pga.Name))
                {
                    continue;
                }

                DynamicObjectProperty prop = dynamicObject.AddProperty(pga.Name, pga.Type, null);
                prop.SetValue(dynamicObject, pga.Value);
            }
        }

        public static bool HasProperties(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(type))
            {
                if (!descriptor.IsBrowsable)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        public virtual PropertyGridProperty AddProperty(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            PropertyGridProperty prop = Properties.FirstOrDefault(p => p.Name == propertyName);
            if (prop != null)
            {
                return prop;
            }

            PropertyDescriptor desc = TypeDescriptor.GetProperties(Data).OfType<PropertyDescriptor>()
                .FirstOrDefault(p => p.Name == propertyName);
            if (desc != null)
            {
                prop = CreateProperty(desc);
                if (prop != null)
                {
                    Properties.Add(prop);
                }
            }

            return prop;
        }

        public virtual DynamicObject CreateDynamicObject()
        {
            return ActivatorHelper.CreateInstance<DynamicObject>();
        }

        public virtual PropertyGridProperty CreateProperty()
        {
            return ActivatorHelper.CreateInstance<PropertyGridProperty>(this);
        }

        public virtual PropertyGridProperty CreateProperty(PropertyDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException("descriptor");
            }

            bool forceReadWrite = false;
            PropertyGridProperty property = null;
            PropertyGridOptionsAttribute options = descriptor.GetAttribute<PropertyGridOptionsAttribute>();
            if (options != null)
            {
                forceReadWrite = options.ForceReadWrite;
                if (options.PropertyType != null)
                {
                    property = (PropertyGridProperty)Activator.CreateInstance(options.PropertyType, this);
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
                        property = (PropertyGridProperty)Activator.CreateInstance(options.PropertyType, this);
                    }
                }
            }

            if (property == null)
            {
                property = CreateProperty();
            }

            Describe(property, descriptor);
            if (forceReadWrite)
            {
                property.IsReadOnly = false;
            }

            property.OnDescribed();
            property.RefreshValueFromDescriptor(true, false, true);
            return property;
        }

        IList IListSource.GetList()
        {
            return Properties;
        }

        protected virtual void Describe(PropertyGridProperty property, PropertyDescriptor descriptor)
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
                descriptor.Category.EqualsIgnoreCase(CategoryAttribute.Default.Category)
                    ? Grid.DefaultCategoryName
                    : descriptor.Category;
            property.IsReadOnly = descriptor.IsReadOnly;
            property.Description = descriptor.Description;
            property.DisplayName = descriptor.DisplayName;
            if (Grid.DecamelizePropertiesDisplayNames && property.DisplayName == descriptor.Name)
            {
                property.DisplayName = DecamelizationHelper.Decamelize(property.DisplayName);
            }

            property.IsEnum = descriptor.PropertyType.IsEnum;
            property.IsFlagsEnum = descriptor.PropertyType.IsEnum && Extensions.IsFlagsEnum(descriptor.PropertyType);

            PropertyGridOptionsAttribute options = descriptor.GetAttribute<PropertyGridOptionsAttribute>();
            if (options != null)
            {
                if (options.SortOrder != 0)
                {
                    property.SortOrder = options.SortOrder;
                }

                property.IsEnum = options.IsEnum;
                property.IsFlagsEnum = options.IsFlagsEnum;
            }

            DefaultValueAttribute att = descriptor.GetAttribute<DefaultValueAttribute>();
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
                    if (PropertyGridComboBoxExtension.TryGetDefaultValue(options, out string defaultValue))
                    {
                        property.DefaultValue = defaultValue;
                        property.HasDefaultValue = true;
                    }
                }
            }

            AddDynamicProperties(descriptor.Attributes.OfType<PropertyGridAttribute>(), property.Attributes);
            AddDynamicProperties(descriptor.PropertyType.GetAttributes<PropertyGridAttribute>(),
                property.TypeAttributes);
        }

        protected virtual void ScanProperties()
        {
            Properties.Clear();
            List<PropertyGridProperty> props = new List<PropertyGridProperty>();
            Type highestType = Data.GetType();

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(Data))
            {
                int level = InheritanceHelper.InheritanceLevel(descriptor.ComponentType, highestType);

                if (level <= _inheritanceLevel &&
                    descriptor.IsBrowsable &&
                    CreateProperty(descriptor) is PropertyGridProperty property)
                {
                    props.Add(property);
                }
            }

            IPropertyGridObject pga = Data as IPropertyGridObject;
            if (pga != null)
            {
                pga.FinalizeProperties(this, props);
            }

            props.Sort();
            foreach (PropertyGridProperty property in props)
            {
                Properties.Add(property);
            }
        }
    }
}