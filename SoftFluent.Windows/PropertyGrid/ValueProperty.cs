using System.ComponentModel;
using System.Globalization;
using Extensions = Utilities.Extensions;

namespace SoftFluent.Windows
{
    public class ValueProperty : PropertyBase
    {
        public ValueProperty(Guid guid) : base(guid)
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
                return this.GetProperty(PropertyType) ?? Descriptor.GetValue(Data); 
            }
            set
            {
                if (!TryChangeType(value, PropertyType, CultureInfo.CurrentCulture, out object changedValue))
                {
                    throw new ArgumentException("Cannot convert value {" + value + "} to type '" + PropertyType.FullName + "'.");
                }

                if (Descriptor != null)
                {
                    try
                    {
                        Descriptor.SetValue(Data, changedValue);
                        //var finalValue = Descriptor.GetValue(Data);
                        this.SetProperty(changedValue, PropertyType);
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("Cannot set value {" + value + "} to object.", e);
                    }
                }
            }
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