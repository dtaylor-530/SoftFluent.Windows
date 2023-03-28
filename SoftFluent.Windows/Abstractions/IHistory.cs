using System;
using System.Collections;

namespace PropertyGrid.WPF.Demo
{
    public interface IHistory : IObservable<object>
    {
        IEnumerable Past { get; }
        IEnumerable Present { get; }
        IEnumerable Future { get; }

        void Add(object order);
        void Back();
        void Forward();
    }
}
