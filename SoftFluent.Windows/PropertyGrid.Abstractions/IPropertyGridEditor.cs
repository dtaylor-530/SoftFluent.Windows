using Abstractions;

namespace SoftFluent.Windows
{
    public interface IPropertyGridEditor
    {
        bool SetContext(IPropertyGridProperty property, object parameter);
    }
}