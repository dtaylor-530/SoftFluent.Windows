using System.Reactive.Linq;
using System.Collections;
using System.Collections.Specialized;
using System;
using Utility.Observables;
using PropertyGrid.Abstractions;
using System.ComponentModel;
using SoftFluent.Windows.Utilities;
using PropertyGrid.Infrastructure;
using Utility.Collections;
using Utility.Nodes.Abstractions;

namespace SoftFluent.Windows
{
    public class PropertyNode : AutoObject, INode, IPropertyNode, INotifyCollectionChanged
    {
        protected Collection _children = new();
        protected Collection _branches = new();
        protected Collection _leaves = new();
        bool flag = false;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        Lazy<DescriptorFilters> lazyPredicates;
        DescriptorFilters predicates;

        public PropertyNode(Guid guid) : base(guid)
        {
            lazyPredicates = new(() => new DefaultFilter(Data));
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

        IEnumerable IPropertyNode.Children => this.Children;

        protected virtual async Task<bool> RefreshAsync()
        {
            if (flag == true)
                return await Task.FromResult(true);

            flag = true;

            PropertyFilter
                .Instance
                .FilterProperties(Data, Guid, Predicates)
                   .Subscribe(prop =>
                   {
                       if (prop.IsValueType || prop.IsString)
                       {
                          _leaves.Add(prop);
                       }
                       else
                            _branches.Add( prop);

                       _children.Add(prop);
                   });

            return await Task.FromResult(true);
        }

        public Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }

        public DescriptorFilters Predicates { get => predicates ?? lazyPredicates.Value; set => predicates = value; }
    }


    public class DefaultFilter : DescriptorFilters
    {
        List<Predicate<PropertyDescriptor>> predicates;
        public DefaultFilter(object data)
        {
            var type = data.GetType();
            predicates = new(){
                new Predicate<PropertyDescriptor>(descriptor=>
            {
                      int level = descriptor.ComponentType.InheritanceLevel(type);

                   return level == 0 /*<= options.InheritanceLevel*/ && descriptor.IsBrowsable;
            }) };
        }

        public override IEnumerator<Predicate<PropertyDescriptor>> GetEnumerator()
        {
            return predicates.GetEnumerator();
        }
    }
}

