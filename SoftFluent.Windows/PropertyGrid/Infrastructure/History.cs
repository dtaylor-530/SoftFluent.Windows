using System.Collections;
using System.Reactive.Linq;
using PropertyGrid.Infrastructure;
using Utility.Collections;

namespace PropertyGrid.WPF.Demo.Infrastructure
{
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
            if(isDirty)
            {
                future.Clear();
            }
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
        }

        bool isDirty;

        public void Back()
        {
            isDirty = true;
            var d = past[^0];
            //if (past.Any())
            future.Insert(0, present[0]);
            if (present.Count > 0)
                present.RemoveAt(0);
            present.Add(d);
            past.Remove(d);
            Broadcast(present);
        }


        private void Broadcast(object obj) { foreach (var observer in observers) observer.OnNext(obj); }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return new Disposer<object>(observers, observer);
        }
    }
}
