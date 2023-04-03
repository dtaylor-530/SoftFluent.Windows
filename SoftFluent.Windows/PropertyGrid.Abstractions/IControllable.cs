using Abstractions;
using System;

namespace PropertyGrid.WPF.Demo
{
    public interface IControllable : IObservable<ControlType>
    {
        void Back();
        void Play();
        void Pause();
        void Forward();
    }
}
