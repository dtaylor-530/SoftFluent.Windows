using SoftFluent.Windows.Utilities;
using System.ComponentModel;
using Abstractions;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Collections;

namespace SoftFluent.Windows
{
    class PropertyHelper
    {

        BaseActivator activator = new BaseActivator();

        public IObservable<IProperty> GenerateProperties(object data)
        {

            Guid guid = Guid.NewGuid();
            if (data is IGuid iguid)
            {
                guid = iguid.Guid;
            }

            Type highestType = data.GetType();

            return Generate(highestType);

            IObservable<IProperty> Generate(Type highestType)
            {
                Subject<IProperty> subject = new();

                var t = Task(data, guid, subject, highestType);

                return subject;
            }
        }



        public Task<List<IProperty>> Task(object data, Guid guid, Subject<IProperty> subject, Type highestType)
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                List<IProperty> list = new();
                if (data is IEnumerable enumerable)
                {
                    int i = 0;
                    foreach (var item in enumerable)
                    {
                        try
                        {

                            var property = activator.CreateCollectionProperty(guid, i, item).Result;
                            //RefreshProperty(property);
                            subject.OnNext(property);
                            list.Add(property);
                        }
                        catch (Exception ex)
                        {

                        }
                        i++;
                    }
                }
                else
                {
                    var descriptors = PropertyDescriptors(data, highestType).ToArray();
                    foreach (var descriptor in descriptors)
                    {
                        try
                        {
                            IProperty property;

                            if (descriptor.PropertyType.IsValueType || descriptor.PropertyType == typeof(string))
                            {

                                 property = activator.CreateProperty(guid, descriptor, data).Result;
                            }
                            else
                            {
                                var item = descriptor.GetValue(data);
                                if(item ==null)
                                {
                                    throw new Exception("g 34 r");
                                }
                                 property = activator.CreateProperty(guid, descriptor, item).Result;
                            }
                            //RefreshProperty(property);
                            subject.OnNext(property);
                            list.Add(property);

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                return list;
            });
        }

        public IEnumerable<PropertyDescriptor> PropertyDescriptors(object data, Type highestType)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(data).Cast<PropertyDescriptor>().OrderBy(d => d.Name))
            {

                int level = descriptor.ComponentType.InheritanceLevel(highestType);

                if (level == 0 /*<= options.InheritanceLevel*/ &&
                    descriptor.IsBrowsable)
                    yield return descriptor;

            }
        }

        public static PropertyHelper Instance { get; } = new PropertyHelper();
    }
}

