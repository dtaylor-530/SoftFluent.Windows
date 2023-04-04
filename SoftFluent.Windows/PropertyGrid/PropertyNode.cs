using System.Reactive.Linq;
using System.Collections;
using System.Collections.Specialized;
using Models;
using System;
using Trees;
using Utility.Observables;

namespace SoftFluent.Windows
{

    public class PropertyNode : AutoObject, INode, IEnumerable, INotifyCollectionChanged
    {
        protected Collection _children = new();
        protected Collection _branches = new();
        protected Collection _leaves = new();
        bool flag = false;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public PropertyNode(Guid guid) : base(guid)
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
                       if (prop.IsValueType || prop.IsString)
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
}

