using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Abstractions;
using Extensions = Utilities.Extensions;

namespace SoftFluent.Windows
{
    public class Property2 : PropertySource, IProperty
    {

        public Property2(Guid guid) : base(guid)
        {
         
        }

        public string Name { get; set; }
        public string DisplayName => Name;
        public bool IsReadOnly => false;
        public bool IsCollection => PropertyType != null ? PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(PropertyType) : false;


        public virtual string? Category => "Collection-Item";            

        public virtual int CollectionCount => Value is IEnumerable enumerable ? enumerable.Cast<object>().Count() : 0;

        public virtual Type CollectionItemPropertyType => !IsCollection ? null : Extensions.GetElementType(PropertyType);

        public virtual string Description { get => GetProperty<string>(); set => SetProperty(value); }

        public virtual bool IsCollectionItemValueType => CollectionItemPropertyType != null && CollectionItemPropertyType.IsValueType;


        public virtual bool IsError { get => GetProperty<bool>(); set => SetProperty(value); }


        public virtual Type PropertyType => Data.GetType();


        public virtual object? Value
        {
            get => Data;
            set
            {

            }
        }

        public string TemplateKey { get => GetProperty<string>(); set => SetProperty(value); }


        public string EditorTemplateKey { get => GetProperty<string>(); set => SetProperty(value); }

        public bool IsValueType => PropertyType.IsValueType;

        public bool IsFlagsEnum => throw new NotImplementedException();



        public override string ToString()
        {
            return Name;
        }

        protected virtual bool TryChangeType(object value, Type type, IFormatProvider provider, out object changedValue)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return ConversionHelper.TryChangeType(value, type, provider, out changedValue);
        }
    }
}