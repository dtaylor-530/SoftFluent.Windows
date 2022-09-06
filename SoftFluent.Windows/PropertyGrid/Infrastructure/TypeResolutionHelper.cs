using System;

namespace SoftFluent.Windows
{
    public static class TypeResolutionHelper
    {
        public static Type ResolveType(string fullName)
        {
            return ResolveType(fullName, false);
        }

        public static Type ResolveType(string fullName, bool throwOnError)
        {
            return BaseTypeResolver.ResolveType(fullName, throwOnError);
        }
    }
}
