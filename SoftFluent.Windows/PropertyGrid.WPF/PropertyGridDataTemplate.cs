using SoftFluent.Windows.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace SoftFluent.Windows
{
   [ContentProperty("DataTemplate")]
    public class PropertyGridDataTemplate 
    {
        private class NullableEnum { }
        public static readonly Type NullableEnumType = typeof(NullableEnum);

        private List<Type>? _resolvedPropertyTypes;
        private List<Type>? _resolvedCollectionItemPropertyTypes;

        public string? PropertyType { get; set; }
        public string? CollectionItemPropertyType { get; set; }
        public DataTemplate? DataTemplate { get; set; }  // note: may be null
        public bool? IsCollection { get; set; }
        public bool? IsReadOnly { get; set; }
        public bool? IsError { get; set; }
        public bool? IsValid { get; set; }
        public bool? IsFlagsEnum { get; set; }
        public bool? IsCollectionItemValueType { get; set; }
        public bool? IsValueType { get; set; }
        public string? Category { get; set; }
        public string? Name { get; set; }

        public virtual IList<Type> ResolvedPropertyTypes
        {
            get
            {
               const string nullableEnum = "System.Nullable`1[System.Enum]";
                if (_resolvedPropertyTypes == null)
                {
                    _resolvedPropertyTypes = new List<Type>();
                    List<string> names = PropertyType.SplitToList<string>('|');
                    foreach (var name in names.Where(name => !string.IsNullOrWhiteSpace(name)))
                    {    
                       // a hack to handle nullable enum in a general way
                       if ((name == nullableEnum ? 
                              NullableEnumType :
                              TypeResolutionHelper.ResolveType(name)) is { } type)
                       {
                          _resolvedPropertyTypes.Add(type);
                       }
                    }
                }
                return _resolvedPropertyTypes;
            }
        }

        public virtual IList<Type> ResolvedCollectionItemPropertyTypes
        {
            get
            {
                if (_resolvedCollectionItemPropertyTypes == null)
                {
                    _resolvedCollectionItemPropertyTypes = new List<Type>();
                    List<string>? names = CollectionItemPropertyType?.SplitToList<string>('|');
                    foreach (string name in names?? throw new Exception("FSD we"))
                    {
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            continue;
                        }

                        if (TypeResolutionHelper.ResolveType(name) is { } type)
                        {
                            _resolvedCollectionItemPropertyTypes.Add(type);
                        }
                    }
                }
                return _resolvedCollectionItemPropertyTypes;
            }
        }
    }
}