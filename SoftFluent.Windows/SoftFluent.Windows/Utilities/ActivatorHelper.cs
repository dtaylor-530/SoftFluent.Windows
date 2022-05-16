using System;

namespace SoftFluent.Windows
{
    public static class ActivatorHelper
    {
        public static object CreateInstance(this Type type, params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            object obj = ServiceProvider.Current.GetService<IActivator>().CreateInstance(type, args);
            return obj;
        }

        public static T CreateInstance<T>(params object[] args)
        {
            return (T)CreateInstance(typeof(T), args);
        }

        public static object CreateInstance(Type type)
        {
            return CreateInstance(type, null);
        }

        public static T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T), null);
        }
    }
}
