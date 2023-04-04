using SoftFluent.Windows.Utilities;
using System.ComponentModel;
using Abstractions;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Collections;
using System.Collections.Specialized;
using Models;
using System;
using Trees;
using Utility.Observables;

namespace SoftFluent.Windows
{

    public class PropertySource : AutoObject, INode, IEnumerable, INotifyCollectionChanged
    {
        //private object data;
        protected Collection _children = new();
        protected Collection _branches = new();
        protected Collection _leaves = new();
        bool flag = false;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public PropertySource(Guid guid) : base(guid)
        {
            _children.CollectionChanged += (s, e) => CollectionChanged?.Invoke(this, e);
        }
        public INode Parent { get; set; }

        public virtual IEnumerable Ancestors
        {
            get
            {
                return GetAncestors();
            }

        }

        public virtual IObservable Children
        {
            get
            {
                _ = RefreshAsync();
                return _children;
            }
        }
        public virtual IObservable Leaves
        {
            get
            {
                _ = RefreshAsync();
                return _leaves;
            }
        }

        public virtual IObservable Branches
        {
            get
            {
                _ = RefreshAsync();
                return _branches;
            }
        }


        private IEnumerable GetAncestors()
        {
            INode parent = this;
            while (parent != null)
            {
                yield return parent;
                parent = parent.Parent;
            }
        }

        public override string ToString()
        {
            return Data.GetType().Name;
        }

        public int Count => _children.Count;


        public virtual object Content => Data.GetType().Name;

        public object Data { get; set; }

        public IEnumerator GetEnumerator()
        {
            _ = RefreshAsync();
            return _children.GetEnumerator();
        }

        protected virtual async Task<bool> RefreshAsync()
        {
            if (flag == true)
                return await Task.FromResult(true);

            flag = true;

            PropertyHelper.Instance
                .GenerateProperties(Data)
                   .Subscribe(prop =>
                   {
                       if (prop.IsValueType)
                       {
                           Context.Post(a => { _leaves.Add(a); }, prop);
                       }
                       else
                           Context.Post(a => { _branches.Add(a); }, prop);

                       Context.Post(a => { _children.Add(a); }, prop);
                   });

            return await Task.FromResult(true);
        }

        public Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }


        public Task<object?> GetChildren() => throw new NotImplementedException();

        public Task<object?> GetProperties()
        {
            throw new NotImplementedException();
        }
    }





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

                            var property = activator.CreateProperty2(guid, i.ToString(), item).Result;
                            //RefreshProperty(property);
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

