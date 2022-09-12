using Abstractions;

namespace SoftFluent.Windows;

public interface IPropertyGridListSource
{
   object? Data { get; }
   IPropertyGrid Grid { get; }
   IPropertyGridProperty GetByName(string name);
}