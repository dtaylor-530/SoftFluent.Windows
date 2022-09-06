using System;

namespace SoftFluent.Windows
{
    public  class ActivatorHelper
    {
       private readonly IActivator _activator;

       public ActivatorHelper(IActivator activator)
       {
          _activator = activator;
       }

        public  object CreateInstance( Type type, params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            object obj = _activator.CreateInstance(type, args);
            return obj;
        }

        public  T CreateInstance<T>(params object[] args)
        {
            return (T)CreateInstance(typeof(T), args);
        }

        public  object CreateInstance(Type type)
        {
            return CreateInstance(type, null);
        }

        public  T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T), null);
        }
    }
}
