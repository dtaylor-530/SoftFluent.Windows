using System;

namespace SoftFluent.Windows
{
    public class BaseTypeResolver 
    {
        public static Type ResolveType(string fullName, bool throwOnError)
        {
            if (fullName == null)
                throw new ArgumentNullException("fullName");

            var type = Type.GetType(fullName, throwOnError);
            return type;
        }
    }
}
