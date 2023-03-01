using Abstractions;

namespace SoftFluent.Windows;

public interface IPropertyEngine
{
    IProperty GetProperty(string name);
}