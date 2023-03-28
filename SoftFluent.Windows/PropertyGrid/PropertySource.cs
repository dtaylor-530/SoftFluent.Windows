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
using System.Collections;
using System.Collections.Specialized;

namespace SoftFluent.Windows
{

    public class PropertySource : IPropertySource, INotifyCollectionChanged
    {
        //private readonly IActivator _activator;
        //private readonly IPropertyGridOptions options;
        private readonly ObservableCollection<IProperty> _properties = new();
        private object data;
        private SynchronizationContext? context;
        BaseActivator activator = new BaseActivator();

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public PropertySource(object data)
        {

            //_activator = activator;
            this.data = data;
            context = SynchronizationContext.Current?? throw new Exception("er 434434");

            //options.Data = options.Data ?? throw new ArgumentNullException("data");


            _properties.CollectionChanged += _properties_CollectionChanged;
            UpdateProperties();

            void UpdateProperties()
            {
                Properties()
                   
                    .Subscribe( prop =>
                    {
                        context.Post(a => { _properties.Add(prop); }, prop);
                       
                    });
            }
        }

        private void _properties_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        //public IPropertyGridOptions Options => options;

        public IProperty GetProperty(string name)
        {
            return _properties.FirstOrDefault(p => p.Name.EqualsIgnoreCase(name));
        }

        //protected virtual Property CreateProperty(PropertyDescriptor descriptor)
        //{
        //    return MyHelper.CreateProperty(this.Guid, descriptor, _activator, options.Data);
        //}

        protected virtual IObservable<IProperty> Properties()
        {
            Guid guid = Guid.NewGuid();
            if (data is IGuid iguid)
            {
                guid = iguid.Guid;
            }

            if (_properties.Any())
            {
                return _properties.ToObservable();
            }

            Type highestType = data.GetType();

            return Generate(highestType);

            IObservable<IProperty> Generate(Type highestType)
            {
                Subject<IProperty> subject = new();

                var descriptors = PropertyDescriptors(highestType).ToArray();

                _ = Task.Run(() =>
                {
                    if (data is IEnumerable enumerable)
                    {
                        int i = 0;
                        foreach (var item in enumerable)
                        {
                            try
                            {

                                var property = activator.CreateProperty2(guid, i.ToString(), item).Result;
                                RefreshProperty(property);
                                subject.OnNext(property);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    else
                    {
       
                        foreach (var descriptor in descriptors)
                        {
                            try
                            {
                                var property = activator.CreateProperty(guid, descriptor, data).Result;

                                RefreshProperty(property);
                                subject.OnNext(property);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                });

                
                return subject;
            }
        }

        IEnumerable<PropertyDescriptor> PropertyDescriptors(Type highestType)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(data).Cast<PropertyDescriptor>().OrderBy(d => d.Name))
            {

                int level = descriptor.ComponentType.InheritanceLevel(highestType);

                if (level == 0 /*<= options.InheritanceLevel*/ &&
                    descriptor.IsBrowsable)
                    yield return descriptor;

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

        public int Count => _properties.Count;


        public void RefreshProperty(IProperty property)
        {
            //if (property.Descriptor == null)
            //{
            //    return;
            //}
            //try
            //{
            //    //object value = property.Descriptor.GetValue(Options.Data);
            //    //bool set = SetProperty(value, property.Name);
            //    //OnPropertyChanged(property.Name);
            //}
            //catch (Exception e)
            //{
            //    if (property.PropertyType == typeof(string))
            //    {
            //        property.Value = e.GetAllMessages();
            //    }
            //    property.IsError = true;
            //}
        }

        public IEnumerator GetEnumerator()
        {
            return _properties.GetEnumerator();
        }
    }
}

