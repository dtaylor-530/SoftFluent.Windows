using System.ComponentModel;
using Extensions = Utilities.Extensions;

namespace SoftFluent.Windows
{
    public class ReferenceProperty : PropertyBase
    {
        public ReferenceProperty(Guid guid) : base(guid)
        {
        }
        public override string Name => Descriptor.Name;
        public string DisplayName => Descriptor.DisplayName;
        public override bool IsReadOnly => Descriptor.IsReadOnly;
        public bool IsFlagsEnum => Extensions.IsFlagsEnum(PropertyType);

        public virtual string? Category => string.IsNullOrWhiteSpace(Descriptor.Category) ||
                Extensions.EqualsIgnoreCase(Descriptor.Category, CategoryAttribute.Default.Category)
                    ? null
                    : Descriptor.Category;
        public virtual TypeConverter Converter => Descriptor.Converter;

        public virtual PropertyDescriptor Descriptor { get; set; }

        public override Type PropertyType => Descriptor.PropertyType;

        public override object Content =>  Name;

        protected override async Task<bool> RefreshAsync()
        {
            if ((PropertyType.IsValueType || PropertyType ==typeof(string)) != true)
                return await base.RefreshAsync();

            return await Task.FromResult(true);
        }

        public override object? Value
        {
            get
            {
                var property =Data;
                return property;
            }
            set => throw new Exception("aa 4 43321``");
        }


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