using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Abstractions;
using Models;
using SoftFluent.Windows;
using Key = SoftFluent.Windows.Key;

namespace PropertyGrid.WPF.Demo.Infrastructure
{
    public record PropertyChange(IKey Key, object Value) : IPropertyChange;
    public class Order
    {
        public Key Key { get; set; }
        public OrderType OrderType { get; set; }
        public object Value { get; set; }
        public System.Type Type { get; set; }

    }


    public class Controllable : IControllable
    {
        public List<IObserver<ControlType>> observers = new();

        public void Back()
        {
            Broadcast(ControlType.Back);

        }

        public void Forward()
        {
            Broadcast(ControlType.Forward);

        }

        public void Pause()
        {
            Broadcast(ControlType.Pause);

        }

        public void Play()
        {
            Broadcast(ControlType.Play);
        }

        private void Broadcast(ControlType type) { foreach (var observer in observers) observer.OnNext(type); }

        public IDisposable Subscribe(IObserver<ControlType> observer)
        {
            return new Disposer<ControlType>(observers, observer);
        }
    }

    public class History : IHistory
    {
        public List<IObserver<object>> observers = new();

        ThreadSafeObservableCollection<Order> past = new();
        ThreadSafeObservableCollection<Order> present = new();
        ThreadSafeObservableCollection<Order> future = new();

        public History()
        {
            ThreadSafeObservableCollection<Order>.Context = SynchronizationContext.Current;
        }

        public IEnumerable Past => past;


        public IEnumerable Present => present;


        public IEnumerable Future => future;

        public void Add(object order)
        {
            if (order is not Order o)
            {
                throw new Exception("rfe w3");
            }

            if (future.Any(a => a.Key == o.Key && a.OrderType == o.OrderType && a.Value == o.Value))
            {
                return;
            }

            future.Add(o);
        }

        public void Forward()
        {
            var d = future[0];
            if (present.Count > 0)
            {
                past.Add(present[0]);
            }
            if (present.Count > 0)
                present.RemoveAt(0);
            present.Add(d);
            future.RemoveAt(0);
            Broadcast(present[0]);

            //this.OnPropertyChanged(nameof(Present));
        }

        public void Back()
        {
            var d = past[^0];
            //if (past.Any())
            future.Insert(0, present[0]);
            if (present.Count > 0)
                present.RemoveAt(0);
            present.Add(d);
            past.Remove(d);
            Broadcast(present);

            //this.OnPropertyChanged(nameof(Present));
        }


        private void Broadcast(object obj) { foreach (var observer in observers) observer.OnNext(obj); }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return new Disposer<object>(observers, observer);
        }
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
            if (key is not Key { Guid: var guid, Name: var name, Type: var type } _key)
            {
                throw new Exception("reg 43cs ");
            }

            Observable
                .Return(new Order { Key = _key, OrderType = OrderType.Get, Type = type })

                .Subscribe(history.Add);
        }

        public void SetValue(IKey key, object value)
        {
            if (key is not Key { Guid: var guid, Name: var name, Type: var type } _key)
            {
                throw new Exception("reg 43cs ");
            }
            Observable
                .Return(new Order { Key = _key, OrderType = OrderType.Set, Value = value, Type = type })
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
        public async Task<Guid> GetGuid(Guid guid, string? name, System.Type type)
        {
            return await repo.FindOrCreateKey(guid, name, type);
        }

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
                            var a = await repo.FindOrCreateKeyByParent(order.Key.Guid, order.Key.Name, order.Type);
                            var find = await repo.Find(a);
                            if (find.Any())
                            {
                                Update(find.Last(), order);
                            }
                           ;
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
                            var a = await repo.FindOrCreateKeyByParent(order.Key.Guid, order.Key.Name, order.Type);
                            await repo.Update(a, order.Value);
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
