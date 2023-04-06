using System.ComponentModel;
using Abstractions;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Collections;
using System.Reflection;

namespace SoftFluent.Windows
{
    public interface IGuidConverter
    {
        Guid Convert(object data);
    }

    public class GuidConverter : IGuidConverter
    {

        public Guid Convert(object data)
        {
            if (data is IGuid iguid)
            {
                return iguid.Guid;
            }
            throw new Exception("esfdd 33");
        }
    }

    public abstract class DescriptorFilters : IEnumerable<Predicate<PropertyDescriptor>>
    {
        public abstract IEnumerator<Predicate<PropertyDescriptor>> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class PropertyFilter
    {

        BaseActivator activator = new();
        //IGuidConverter guidConverter = new GuidConverter();

        public IObservable<IProperty> FilterProperties(object data, Guid guid, DescriptorFilters? filters = null)
        {
            Subject<IProperty> subject = new();

            Task.Run(() =>
            {
                foreach (var prop in EnumerateProperties(data, guid, filters))
                {
                    if (prop != null)
                        subject.OnNext(prop);
                }
            });

            return subject;
        }

        public IEnumerable<IProperty?> EnumerateProperties(object data, Guid guid, DescriptorFilters? filters = null)
        {
            if (data is IEnumerable enumerable)
            {
                int i = 0;
                foreach (var item in enumerable)
                {
                    yield return FromIndex(i, item);
                    i++;
                }
            }
            else
            {
                var descriptors = PropertyDescriptors(data).ToArray();
                foreach (var descriptor in descriptors)
                {
                    yield return FromPropertyDescriptor(descriptor);
                }
            }


            IProperty? FromIndex(int i, object? item)
            {
                try
                {
                    return activator.CreateCollectionItemProperty(guid, i, item).Result;
                }
                catch (Exception ex)
                {

                }

                return null;
            }

            IEnumerable<PropertyDescriptor> PropertyDescriptors(object data)
            {
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(data)
                    .Cast<PropertyDescriptor>()
                    .Where(a => filters?.All(f => f.Invoke(a)) != false)
                    .OrderBy(d => d.Name))
                {
                    yield return descriptor;
                }
            }


            IProperty? FromPropertyDescriptor(PropertyDescriptor descriptor)
            {
                if (descriptor.PropertyType == typeof(MethodBase))
                    return null;
                if (descriptor.PropertyType == typeof(Type))
                    return null;
                try
                {
                    return CreateProperty(data, guid, descriptor);
                }
                catch (Exception ex)
                {
                }

                return null;

                IProperty CreateProperty(object data, Guid guid, PropertyDescriptor descriptor)
                {
                    IProperty property;
                    if (IsValueOrStringProperty(descriptor))
                    {
                        property = activator.CreateValueProperty(guid, descriptor, data).Result;
                    }
                    else/* if(IsCollectionProperty(descriptor))*/
                    {
                        property = activator.CreateReferenceProperty(guid, descriptor, data).Result;
                    }   
                    //else 
                    //{
                    //    var item = descriptor.GetValue(data);
                    //    property = activator.CreateProperty(guid, descriptor, item).Result;
                    //}

                    return property;

                    static bool IsValueOrStringProperty(PropertyDescriptor? descriptor)
                    {
                        return descriptor.PropertyType.IsValueType || descriptor.PropertyType == typeof(string);
                    }

                    static bool IsCollectionProperty(PropertyDescriptor? descriptor)
                    {
                        return descriptor.PropertyType != null ? descriptor.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(descriptor.PropertyType) : false;
                    }
                }            
            }
        }


        public static PropertyFilter Instance { get; } = new PropertyFilter();
    }
}

