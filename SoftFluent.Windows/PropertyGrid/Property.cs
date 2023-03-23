using System.Collections;
using System.ComponentModel;
using System.Globalization;
using Abstractions;
using Extensions = Utilities.Extensions;

namespace SoftFluent.Windows
{
    public class Property : AutoObject, IProperty //, IComparable, IComparable<Property>
    {
        //private bool _isVisible = true;

        public Property(Guid guid) : base(guid)
        {
  
        }

        public object Data { get; set; }

        public string Name => Descriptor.Name;
        public string DisplayName => Descriptor.DisplayName;
        public bool IsReadOnly => Descriptor.IsReadOnly;
        public bool IsCollection => PropertyType != null ? PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(PropertyType) : false;

        //public bool IsVisible
        //{
        //    get => _isVisible;
        //    set
        //    {
        //        _isVisible = value;
        //        OnPropertyChanged();
        //    }
        //}

        //public virtual DynamicObject Attributes { get; }

        //public bool? BooleanValue => Value == null ?
        //            null :
        //            ConversionHelper.ChangeType(Value, HasDefaultValue && ConversionHelper.ChangeType(DefaultValue, false));

        public virtual string? Category => string.IsNullOrWhiteSpace(Descriptor.Category) ||
                Extensions.EqualsIgnoreCase(Descriptor.Category, CategoryAttribute.Default.Category)
                    ? null
                    : Descriptor.Category;

        public virtual int CollectionCount => Value is IEnumerable enumerable ? enumerable.Cast<object>().Count() : 0;

        public virtual Type CollectionItemPropertyType => !IsCollection ? null : Extensions.GetElementType(PropertyType);

        public virtual TypeConverter Converter => Descriptor.Converter;

        public virtual string Description { get => GetProperty<string>(); set => SetProperty(value); }

        public virtual PropertyDescriptor Descriptor { get; set; }

        //public virtual bool HasDefaultValue => DefaultValue != default;

        public virtual bool IsCollectionItemValueType => CollectionItemPropertyType != null && CollectionItemPropertyType.IsValueType;


        public virtual bool IsError { get => GetProperty<bool>(); set => SetProperty(value); }


        //public virtual string Name { get; }


        public virtual Type PropertyType => Descriptor.PropertyType;


        //public virtual IPropertyGridOptionsAttribute Options { get; set; }



        //public virtual int SortOrder => Options.SortOrder != 0 ? Options.SortOrder : default;

        //public virtual object Tag { get; set; }

        //public virtual string? TextValue
        //{
        //    get
        //    {
        //        return Converter?.CanConvertTo(typeof(string)) ?? false
        //            ? (string?)Converter.ConvertTo(Value, typeof(string))
        //            : ConversionHelper.ChangeType<string>(Value);
        //    }
        //    set
        //    {
        //        if (Converter != null)
        //        {
        //            if (Converter.CanConvertFrom(typeof(string)))
        //            {
        //                Value = Converter.ConvertFrom(value);
        //                return;
        //            }

        //            if (Descriptor != null && Converter.CanConvertTo(Descriptor.PropertyType))
        //            {
        //                Value = Converter.ConvertTo(value, Descriptor.PropertyType);
        //                return;
        //            }
        //        }

        //        if (Descriptor != null)
        //        {
        //            if (ConversionHelper.TryChangeType(value, Descriptor.PropertyType, out object v))
        //            {
        //                Value = v;
        //                return;
        //            }
        //        }
        //        Value = value;
        //    }
        //}

        public virtual object? Value
        {
            get => GetProperty<object>() ?? Descriptor.GetValue(Data);
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
                        var finalValue = Descriptor.GetValue(Data);
                        this.SetProperty(finalValue);
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("Cannot set value {" + value + "} to object.", e);
                    }
                }
            }
        }

        public string TemplateKey { get => GetProperty<string>(); set => SetProperty(value); }


        public string EditorTemplateKey { get => GetProperty<string>(); set => SetProperty(value); }


        
        //public virtual void CanExecute(object sender, EventArgs e)
        //{
        //    if (Value is IExecute handler)
        //    {
        //        handler.CanExecute(this, e);
        //    }
        //}

        //public virtual void CloneValue(bool refresh)
        //{
        //    if (_valueCloned && !refresh)
        //    {
        //        return;
        //    }

        //    _clonedValue = Value is ICloneable c ? c.Clone() : Value;
        //    _valueCloned = true;
        //}

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

        //public virtual void Executed(object sender, EventArgs e)
        //{
        //    IExecute handler = Value as IExecute;
        //    if (handler != null)
        //    {
        //        handler.Executed(this, e);
        //    }
        //}


        //public virtual string DisplayName { get => GetProperty<string>(); set => SetProperty(value); }


        //public virtual object ClonedValue
        //{
        //    get
        //    {
        //        CloneValue(false);
        //        return _clonedValue;
        //    }
        //}


        //public IPropertyGridListSource ListSource { get; private set; }
        //public object Data { get; }
        //public virtual string DefaultEditorResourceKey
        //{
        //    get
        //    {
        //        if (_defaultEditorResourceKey != null)
        //        {
        //            return _defaultEditorResourceKey;
        //        }

        //        if (IsCollection)
        //        {
        //            return "CollectionEditorWindow";
        //        }

        //        return "ObjectEditorWindow";
        //    }
        //    set => _defaultEditorResourceKey = value;
        //}

        //public virtual bool IsCollection
        //{
        //    get
        //    {
        //        if (PropertyType == null)
        //        {
        //            return false;
        //        }

        //        if (PropertyType == typeof(string))
        //        {
        //            return false;
        //        }

        //        return typeof(IEnumerable).IsAssignableFrom(PropertyType);
        //    }
        //}

        //public virtual bool IsDefaultValue
        //{
        //    get
        //    {
        //        if (!HasDefaultValue)
        //        {
        //            return false;
        //        }

        //        if (DefaultValue == null)
        //        {
        //            return Value == null;
        //        }

        //        return DefaultValue.Equals(Value);
        //    }
        //}


        //public virtual void OnEvent(object sender, IPropertyGridEventArgs e) {
        //   EventHandler<IPropertyGridEventArgs> handler = Event;
        //   if (handler != null) {
        //      handler(sender, e);
        //   }
        //}

        //public object BuildItems()
        //{
        //    var items = EnumToObjectConverter.BuildItems(/*_activator, */this);
        //    //Dictionary<string, object> ctx = new Dictionary<string, object> {
        //    //   ["items"] = items
        //    //};
        //    //this.OnEvent(this, _activator.CreateInstance<PropertyGridEventArgs>(this, ctx));
        //    return items;
        //}

        //public virtual void OnValueChanged()
        //{
        //    OnPropertyChanged("TextValue");
        //    OnPropertyChanged("BooleanValue");
        //    OnPropertyChanged("IsCollection");
        //    OnPropertyChanged("CollectionCount");
        //    OnPropertyChanged("IsDefaultValue");
        //}

        //public virtual bool RaiseOnPropertyChanged(string name)
        //{
        //    return OnPropertyChanged(name);
        //}

        //public void ResetClonedValue()
        //{
        //    _valueCloned = false;
        //}

        //public virtual void SetValue(object value, bool setChanged, bool forceRaise, bool trackChanged)
        //{
        //    bool set = SetProperty(nameof(Value), value, setChanged, forceRaise, trackChanged);
        //    if (set || forceRaise)
        //    {
        //        OnValueChanged();
        //    }
        //}

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