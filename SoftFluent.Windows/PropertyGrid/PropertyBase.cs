using System.Collections;
using System.ComponentModel;
using System.Globalization;
using Abstractions;
using Extensions = Utilities.Extensions;

namespace SoftFluent.Windows
{
    public abstract class PropertyBase : PropertyNode, IProperty
    {
        public PropertyBase(Guid guid) : base(guid)
        {
        }
        public virtual string Name { get; }

        public bool IsCollection => PropertyType != null ? PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(PropertyType) : false;
        public bool IsFlagsEnum => Extensions.IsFlagsEnum(PropertyType);
        public bool IsValueType => PropertyType.IsValueType;

        public virtual int CollectionCount => Value is IEnumerable enumerable ? enumerable.Cast<object>().Count() : 0;

        public virtual Type CollectionItemPropertyType => !IsCollection ? null : Extensions.GetElementType(PropertyType);

        public virtual bool IsCollectionItemValueType => CollectionItemPropertyType != null && CollectionItemPropertyType.IsValueType;

        public virtual bool IsError { get => GetProperty<bool>(); set => SetProperty(value); }

        public virtual Type PropertyType => Data.GetType();

        public abstract bool IsReadOnly { get; }

        public override object Content =>  Name;

        protected override async Task<bool> RefreshAsync()
        {
            if ((PropertyType.IsValueType || PropertyType ==typeof(string)) != true)
                return await base.RefreshAsync();

            return await Task.FromResult(true);
        }

        public abstract object Value { get; set; }

        public bool IsString => PropertyType == typeof(string);

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


        //public string TemplateKey { get => GetProperty<string>(); set => SetProperty(value); }


        //public string EditorTemplateKey { get => GetProperty<string>(); set => SetProperty(value); }


        //public string PanelKey { get => GetProperty<string>(); set => SetProperty(value); }





        //int IComparable.CompareTo(object? obj)
        //{
        //    return CompareTo(obj as Property);
        //}

        //public virtual int CompareTo(Property? other)
        //{
        //    if (other == null)
        //    {
        //        throw new ArgumentNullException("other");
        //    }

        //    if (SortOrder != 0)
        //    {
        //        return SortOrder.CompareTo(other.SortOrder);
        //    }

        //    if (other.SortOrder != 0)
        //    {
        //        return -other.SortOrder.CompareTo(0);
        //    }

        //    if (DisplayName == null)
        //    {
        //        return 1;
        //    }

        //    return string.Compare(DisplayName, other.DisplayName, StringComparison.OrdinalIgnoreCase);
        //}


    }
}