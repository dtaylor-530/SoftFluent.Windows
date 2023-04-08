using Abstractions;

namespace PropertyGrid.WPF.Demo.Infrastructure
{
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
}
