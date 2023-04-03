using SoftFluent.Windows.Utilities;
using System.ComponentModel;
using Abstractions;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Collections;
using System.Collections.Specialized;
using Models;

namespace SoftFluent.Windows
{

    public class PropertySource : Node, IPropertySource, INotifyCollectionChanged
    {
        //private readonly IActivator _activator;
        //private readonly IPropertyGridOptions options;
        private object data;
        private SynchronizationContext context;
        BaseActivator activator = new BaseActivator();
        bool flag = false;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public PropertySource(object data)
        {
            this.data = data;
            context = SynchronizationContext.Current ?? throw new Exception("er 434434");

            _children.CollectionChanged += (s, e) => CollectionChanged?.Invoke(this, e);
            _properties.CollectionChanged += (s, e) => CollectionChanged?.Invoke(this, e);

        }

        //public IPropertyGridOptions Options => options;

        //public IProperty GetProperty(string name)
        //{
        //    return _children.OfType<IProperty>().FirstOrDefault(p => p.Name.EqualsIgnoreCase(name));
        //}

        //protected virtual Property CreateProperty(PropertyDescriptor descriptor)
        //{
        //    return MyHelper.CreateProperty(this.Guid, descriptor, _activator, options.Data);
        //}

        protected virtual IObservable<IProperty> GenerateProperties()
        {
            Guid guid = Guid.NewGuid();
            if (data is IGuid iguid)
            {
                guid = iguid.Guid;
            }

            //if (_properties.Any())
            //{
            //    return _properties.ToObservable();
            //}

            Type highestType = data.GetType();

            return Generate(highestType);

            IObservable<IProperty> Generate(Type highestType)
            {
                Subject<IProperty> subject = new();

                var descriptors = PropertyDescriptors(highestType).ToArray();

                var t = Task(guid, subject, descriptors);

                return subject;
            }
        }

        private Task<List<IProperty>> Task(Guid guid, Subject<IProperty> subject, PropertyDescriptor[] descriptors)
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

                            var property = activator.CreateProperty2(guid, i.ToString(), item).Result;
                            RefreshProperty(property);
                            subject.OnNext(property);
                            list.Add(property);
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

        public override string ToString()
        {
            return data.GetType().Name;
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

        public int Count => _children.Count;

        public override object Content => data.GetType().Name;


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
            RefreshAsync();
            return _children.GetEnumerator();
        }

        protected override async Task<bool> RefreshAsync()
        {
            if (flag == true)
                return await System.Threading.Tasks.Task.FromResult(true);

            flag = true;
            GenerateProperties()
                   .Subscribe(prop =>
                   {
                       if (prop.IsValueType)
                       {
                           context.Post(a => { _properties.Add(prop); }, prop);
                       }
                       else
                           context.Post(a => { _children.Add(prop); }, new PropertySource(prop.Value) { Parent = this });

                   });

            return await System.Threading.Tasks.Task.FromResult(true);
        }



        public override Task<bool> HasMoreChildren()
        {
            return System.Threading.Tasks.Task.FromResult(flag == false);
        }

        public override Node ToNode(object value)
        {
            return new PropertySource(value);
        }

        public override Task<object?> GetChildren() => throw new NotImplementedException();

        public override Task<object?> GetProperties()
        {
            throw new NotImplementedException();
        }
    }
}

