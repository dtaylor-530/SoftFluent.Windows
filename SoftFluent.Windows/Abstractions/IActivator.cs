using System;

namespace SoftFluent.Windows
{
    public interface IActivator
    {
        Task<object?> CreateInstance(Guid parent, string name, Type type);
    }
}
