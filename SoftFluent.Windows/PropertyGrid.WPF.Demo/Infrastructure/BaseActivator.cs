using SoftFluent.Windows.Utilities;
using System;
using PropertyGrid.Infrastructure;

namespace SoftFluent.Windows
{
    public class BaseActivator : IActivator
    {
        public virtual object? CreateInstance(Type type, params object?[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }


            return Activator.CreateInstance(type, args);
        }
    }
}
