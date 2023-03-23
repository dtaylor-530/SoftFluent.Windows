using System;
using SoftFluent.Windows.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Abstractions;
using Utilities;
using PropertyGrid.Abstractions;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using SoftFluent.Windows;

namespace SoftFluent.Windows
{

    public class PropertySource : IPropertySource
    {
        private readonly IActivator _activator;
        private readonly IPropertyGridOptions options;
        private readonly ObservableCollection<IProperty> _properties = new();

        public PropertySource(IActivator activator, IPropertyGridOptions options)
        {

            _activator = activator;
            this.options = options;

            options.Data = options.Data ?? throw new ArgumentNullException("data");

            UpdateProperties();

            void UpdateProperties()
            {
                Properties().Subscribe(prop =>
                {
                    _properties.Add(prop);
                });
            }
        }

        public IPropertyGridOptions Options => options;

        public IProperty GetProperty(string name)
        {
            return _properties.FirstOrDefault(p => p.Name.EqualsIgnoreCase(name));
        }

        //protected virtual Property CreateProperty(PropertyDescriptor descriptor)
        //{
        //    return MyHelper.CreateProperty(this.Guid, descriptor, _activator, options.Data);
        //}

        public virtual IObservable<IProperty> Properties()
        {
            Guid guid = Guid.NewGuid();
            if (options.Data is IGuid iguid)
            {
                guid = iguid.Guid;
            }

            if (_properties.Any())
            {
                return _properties.ToObservable();
            }

            Type highestType = options.Data.GetType();

            return Generate(highestType);

            Subject<IProperty> Generate(Type highestType)
            {
                Subject<IProperty> subject = new();
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(options.Data).Cast<PropertyDescriptor>().OrderBy(d => d.Name))
                {
                    int level = descriptor.ComponentType.InheritanceLevel(highestType);

                    if (level <= options.InheritanceLevel &&
                        descriptor.IsBrowsable
                     )
                    {
                        MyHelper.CreateProperty(guid, descriptor, _activator, options.Data).ToObservable()
                            .Subscribe(property =>
                        {
                            RefreshProperty(property);
                            subject.OnNext(property);
                        });
                    }
                }
                return subject;
            }
        }


        //protected override T GetProperty<T>([CallerMemberName] string? name = null)
        //{
        //    var baseValue = base.GetProperty<T>();
        //    if (baseValue != null)
        //        return baseValue;

        //    var property = GetProperty(name);

        //    try
        //    {
        //        object value = property.Descriptor.GetValue(Options.Data);
        //        return (T)value;
        //    }
        //    catch (Exception e)
        //    {
        //        if (property.PropertyType == typeof(string))
        //        {
        //            property.Value = e.GetAllMessages();
        //        }
        //        property.IsError = true;
        //        return default;
        //    }

        //}


        public void RefreshProperty(IProperty property)
        {
            if (property.Descriptor == null)
            {
                return;
            }
            try
            {
                //object value = property.Descriptor.GetValue(Options.Data);
                //bool set = SetProperty(value, property.Name);
                //OnPropertyChanged(property.Name);
            }
            catch (Exception e)
            {
                if (property.PropertyType == typeof(string))
                {
                    property.Value = e.GetAllMessages();
                }
                property.IsError = true;
            }
        }
    }
}

static class MyHelper
{
    public static async Task<Property> CreateProperty(Guid parent, PropertyDescriptor descriptor, IActivator activator, object data)
    {
        if (descriptor == null)
        {
            throw new ArgumentNullException("descriptor");
        }

        //PropertyGridOptionsAttribute options = descriptor.GetAttribute<PropertyGridOptionsAttribute>();


        var property = (Property)await activator.CreateInstance(parent, descriptor.Name, typeof(Property));
        property.Descriptor = descriptor;

        //property.Options = options;
        property.Data = data;
        //Describe(property, descriptor, gridOptions.DefaultCategoryName, gridOptions.DecamelizePropertiesDisplayNames);
        return property;
    }


    //public static void Describe(Property property, PropertyDescriptor descriptor, string defaultCategoryName, bool decamelizePropertiesDisplayNames)
    //{
    //    if (property == null)
    //    {
    //        throw new ArgumentNullException("property");
    //    }

    //    if (descriptor == null)
    //    {
    //        throw new ArgumentNullException("descriptor");
    //    }

    //    property.Descriptor = descriptor;

    //    if (decamelizePropertiesDisplayNames && property.DisplayName == descriptor.Name)
    //    {
    //        property.DisplayName = BaseDecamelizer.Decamelize(property.DisplayName);
    //    }


    //    PropertyGridOptionsAttribute options = Extensions.GetAttribute<PropertyGridOptionsAttribute>((MemberDescriptor)descriptor);
    //    if (options != null)
    //    {
    //        if (options.SortOrder != 0)
    //        {
    //            property.SortOrder = options.SortOrder;
    //        }
    //    }




    //    property.Attributes.AddDynamicProperties(descriptor.Attributes.OfType<PropertyGridAttribute>().ToArray());
    //    property.TypeAttributes.AddDynamicProperties(Extensions.GetAttributes<PropertyGridAttribute>(descriptor.PropertyType)
    //        .ToArray());


    //}
}
