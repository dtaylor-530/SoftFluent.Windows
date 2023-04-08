using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Abstractions;
using PropertyGrid.WPF.Demo;
using PropertyGrid.WPF.Demo.Infrastructure;
using Key = PropertyGrid.Infrastructure.Key;

namespace PropertyGrid.Infrastructure
{
    public record PropertyChange(IKey Key, object Value) : IPropertyChange;
    public class Order
    {
        public Key Key { get; set; }
        public OrderType OrderType { get; set; }
        public object Value { get; set; }
    }


    public class PropertyStore : IPropertyStore, IObserver<ControlType>, IObserver<object>
    {
        readonly Dictionary<IKey, IObserver> dictionary = new(new KeyComparer());

        private readonly DirectoryInfo directory;
        readonly Repository repo;
        readonly History history = new();
        readonly Controllable controllable = new();
        readonly System.Timers.Timer timer = new(TimeSpan.FromSeconds(0.1));

        private PropertyStore()
        {
            directory = Directory.CreateDirectory("../../../Data");
            repo = new(directory.FullName);
            controllable.Subscribe(this);
            history.Subscribe(this);

            timer.Elapsed += Timer_Elapsed;

        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (history.Future.GetEnumerator().MoveNext())
                history.Forward();
        }

        public IHistory History => history;
        public IControllable Controllable => controllable;


        public void GetValue(IKey key)
        {
            if (key is not Key { } _key)
            {
                throw new Exception("reg 43cs ");
            }

            Observable
                .Return(new Order { Key = _key, OrderType = OrderType.Get })
                .Subscribe(history.Add);
        }

        public void SetValue(IKey key, object value)
        {
            if (key is not Key { } _key)
            {
                throw new Exception("reg 43cs ");
            }
            Observable
                .Return(new Order { Key = _key, OrderType = OrderType.Set, Value = value })
                .Subscribe(history.Add);
        }

        public IDisposable Subscribe(IObserver observer)
        {
            dictionary[observer] = observer;
            return Disposable.Empty;
        }

        public string Validate(string memberName)
        {
            return string.Empty;
        }

        public async Task<Guid> GetGuidByParent(IKey key)
        {
            if (key is not Key { Guid: var guid, Name: var name, Type: var type } _key)
            {
                throw new Exception("reg 43cs ");
            }
            return await repo.FindOrCreateKeyByParent(guid, name, type);
        }

        //public async Task<Guid> GetGuid(Guid guid, string? name, System.Type type)
        //{
        //    return await repo.FindOrCreateKey(guid, name, type);
        //}

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(ControlType value)
        {
            switch (value)
            {
                case ControlType.Pause:
                    timer.Stop();
                    break;
                case ControlType.Play:
                    timer.Start();
                    break;
                case ControlType.Forward:
                    history.Forward();
                    break;
                case ControlType.Back:
                    history.Back();
                    break;
            }
        }

        public async void OnNext(object value)
        {
            if (value is not Order order)
            {
                throw new Exception("g 3434 3");
            }

            switch (order.OrderType)
            {
                case OrderType.Get:
                    {
                        try
                        {
                            var guid = await GetGuidByParent(order.Key);
                            var find = await repo.Find(guid);
                            if (find.Any())
                            {
                                Update(find.Last(), order);
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        break;
                    }
                case OrderType.Set:
                    {
                        try
                        {
                            var guid = await GetGuidByParent(order.Key);
                            await repo.Update(guid, order.Value);
                            Update(order.Value, order);
                        }
                        catch (Exception ex)
                        {

                        }

                        break;
                    }

            }
        }

        private void Update(object a, Order order)
        {
            dictionary[order.Key].OnNext(new PropertyChange(order.Key, a));
        }

        public static PropertyStore Instance { get; } = new();

        class KeyComparer : IEqualityComparer<IKey>
        {
            public bool Equals(IKey? x, IKey? y)
            {
                return x.Equals(y);
            }

            public int GetHashCode([DisallowNull] IKey obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
