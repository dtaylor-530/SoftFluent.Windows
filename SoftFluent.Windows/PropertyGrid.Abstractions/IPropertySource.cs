using Abstractions;

namespace SoftFluent.Windows;

public interface IPropertySource
{
    IProperty GetProperty(string name);

    IObservable<IProperty> Properties();

    void RefreshProperty(IProperty property);
    public int Count { get; }
}