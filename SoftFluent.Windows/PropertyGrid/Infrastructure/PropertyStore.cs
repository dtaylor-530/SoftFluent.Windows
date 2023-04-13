using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Abstractions;
using PropertyGrid.Abstractions;
using PropertyGrid.WPF.Demo;
using PropertyGrid.WPF.Demo.Infrastructure;
using Utility.Trees;
using Utility.Trees.Abstractions;

namespace PropertyGrid.Infrastructure
{
    public enum HistoryState
    {
        None,
        Past,
        Present,
        Future
    }

    public record PropertyChange(IKey Key, object Value) : IPropertyChange;

    public class Order : ViewModel
    {
        private Exception exception;
        private int progress;

        public Key Key { get; set; }
        public HistoryState State { get; set; }
        public OrderType OrderType { get; set; }
        public object Value { get; set; }
        public int Progress { get => progress; set { progress = value; OnPropertyChanged(); } }
        public Exception Exception
        {
            get => exception; set { exception = value; OnPropertyChanged(); }
        }
    }

    public class DispatcherTimer : IObservable<DateTime>
    {
        Subject<DateTime> subject = new();
        public static SynchronizationContext Context { get; set; }

        public System.Timers.Timer Timer { get; set; } = new(TimeSpan.FromSeconds(0.1));

        public DispatcherTimer()
        {

            Timer.Elapsed += Timer_Elapsed;
        }

        public void Start()
        {
            Timer.Start();
        }
        public void Stop()
        {
            Timer.Stop();
        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Context.Post(a => subject.OnNext(e.SignalTime), null);
        }

        public IDisposable Subscribe(IObserver<DateTime> observer)
        {
            return subject.Subscribe(observer);
        }
    }

    public class PropertyStore : IPropertyStore, IObserver<ControlType>, IObserver<object>
    {
        readonly Dictionary<IKey, IObserver> dictionary = new(new KeyComparer());
        //readonly Repository repo;
        readonly History history = new();
        readonly Controllable controllable = new();
        DispatcherTimer timer = new();
        Lazy<IRepository> repository = new(() =>
        {
            var directory = Directory.CreateDirectory("../../../Data");
            return new SqliteRepository(directory.FullName);
        });
        public PropertyStore()
        {

            controllable.Subscribe(this);
            history.Subscribe(this);
            timer.Subscribe(a =>
            {
                if (history.Future.GetEnumerator().MoveNext())
                    history.Forward();
            });
        }

        protected virtual IRepository Repository
        {
            get => repository.Value;
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

        // Move this into history
        public async Task<Guid> GetGuidByParent(IKey key)
        {
            var childKey = await Repository.FindKeyByParent(key);
            return (childKey as Key)?.Guid ?? throw new Exception("dfb 43 4df");
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
                    timer.Stop();
                    history.Forward();
                    break;
                case ControlType.Back:
                    timer.Stop();
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

            order.Progress = 0;
            switch (order.OrderType)
            {
                case OrderType.Get:
                    {
                        try
                        {
                            var guid = await Repository.FindKeyByParent(order.Key);
                            order.Progress = 50;
                            var find = await Repository.FindValue(guid);
                            order.Progress = 100;

                            if (find != null)
                            {
                                Update(find, order);
                            }
                        }
                        catch (Exception ex)
                        {
                            order.Exception = ex;
                        }

                        break;
                    }
                case OrderType.Set:
                    {
                        try
                        {
                            var guid = await Repository.FindKeyByParent(order.Key);
                            order.Progress = 50;
                            await Repository.UpdateValue(guid, order.Value);
                            order.Progress = 100;
                            Update(order.Value, order);
                        }
                        catch (Exception ex)
                        {
                            order.Exception = ex;
                        }

                        break;
                    }

            }
        }

        private void Update(object a, Order order)
        {
            dictionary[order.Key].OnNext(new PropertyChange(order.Key, a));
        }

        //public static PropertyStore Instance { get; } = new();

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
