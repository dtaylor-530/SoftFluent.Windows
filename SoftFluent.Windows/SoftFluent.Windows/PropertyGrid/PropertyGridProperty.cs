using SoftFluent.Windows.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace SoftFluent.Windows {
   public class PropertyGridProperty : AutoObject, IComparable, IComparable<PropertyGridProperty> {
      private object _clonedValue;

      private string _defaultEditorResourceKey;

      private bool _valueCloned;
      private bool _isVisible = true;

      public PropertyGridProperty(PropertyGridListSource listSource) {
         if (listSource == null) {
            throw new ArgumentNullException("listSource");
         }

         ListSource = listSource;
         PropertyType = typeof(object);
         Attributes = listSource.CreateDynamicObject();
         TypeAttributes = listSource.CreateDynamicObject();
      }

      public bool IsVisible {
         get => _isVisible;
         set {
            _isVisible = value;
            OnPropertyChanged();
         }
      }

      public virtual DynamicObject Attributes { get; }

      public bool? BooleanValue {
         get {
            if (Value == null) {
               return null;
            }

            bool def = HasDefaultValue && ConversionHelper.ChangeType(DefaultValue, false);
            return ConversionHelper.ChangeType(Value, def);
         }
         set {
            if (value == null) {
               Value = null;
               return;
            }

            Value = value.Value;
         }
      }

      public virtual string Category { get => GetProperty<string>(); set => SetProperty(value); }

      public virtual object ClonedValue {
         get {
            CloneValue(false);
            return _clonedValue;
         }
      }

      public virtual int CollectionCount {
         get {
            if (Value is IEnumerable enumerable) {
               return enumerable.Cast<object>().Count();
            }

            return 0;
         }
      }

      public virtual Type CollectionItemPropertyType {
         get {
            if (!IsCollection) {
               return null;
            }

            return Extensions.GetElementType(PropertyType);
         }
      }

      public virtual TypeConverter Converter { get => GetProperty<TypeConverter>(); set => SetProperty(value); }

      public PropertyGridListSource ListSource { get; private set; }

      public virtual string DefaultEditorResourceKey {
         get {
            if (_defaultEditorResourceKey != null) {
               return _defaultEditorResourceKey;
            }

            if (IsCollection) {
               return "CollectionEditorWindow";
            }

            return "ObjectEditorWindow";
         }
         set => _defaultEditorResourceKey = value;
      }

      public virtual object DefaultValue {
         get => GetProperty<object>();
         set {
            if (SetProperty(value)) {
               DefaultValues["Value"] = value;
               OnPropertyChanged("IsDefaultValue");
            }
         }
      }

      public virtual string Description { get => GetProperty<string>(); set => SetProperty(value); }

      public virtual PropertyDescriptor Descriptor { get => GetProperty<PropertyDescriptor>(); set => SetProperty(value); }

      public virtual string DisplayName { get => GetProperty<string>(); set => SetProperty(value); }

      public virtual bool HasDefaultValue { get => GetProperty<bool>(); set => SetProperty(value); }

      public virtual bool IsCollection {
         get {
            if (PropertyType == null) {
               return false;
            }

            if (PropertyType == typeof(string)) {
               return false;
            }

            return typeof(IEnumerable).IsAssignableFrom(PropertyType);
         }
      }

      public virtual bool IsCollectionItemValueType => CollectionItemPropertyType != null && CollectionItemPropertyType.IsValueType;

      public virtual bool IsDefaultValue {
         get {
            if (!HasDefaultValue) {
               return false;
            }

            if (DefaultValue == null) {
               return Value == null;
            }

            return DefaultValue.Equals(Value);
         }
      }

      public virtual bool IsEnum { get => GetProperty<bool>(); set => SetProperty(value); }

      public virtual bool IsError { get => GetProperty<bool>(); set => SetProperty(value); }

      public virtual bool IsFlagsEnum { get => GetProperty<bool>(); set => SetProperty(value); }

      public virtual bool IsReadOnly {
         get {
            bool def = ListSource?.Grid != null && ListSource.Grid.IsReadOnly;

            return GetProperty(def);
         }
         set {
            if (SetProperty(value)) {
               OnPropertyChanged("IsReadWrite");
            }
         }
      }

      public bool IsReadWrite {
         get => !IsReadOnly;
         set => IsReadOnly = !value;
      }

      public virtual bool IsValueType => PropertyType != null && PropertyType.IsValueType;

      public virtual string Name { get => GetProperty<string>(); set => SetProperty(value); }

      public virtual PropertyGridOptionsAttribute Options { get; set; }

      public virtual Type PropertyType { get => GetProperty<Type>(); set => SetProperty(value); }

      public virtual int SortOrder { get; set; }

      public virtual object Tag { get; set; }

      public virtual string TextValue {
         get {
            if (Converter != null && Converter.CanConvertTo(typeof(string))) {
               return (string)Converter.ConvertTo(Value, typeof(string));
            }

            return ConversionHelper.ChangeType<string>(Value);
         }
         set {
            if (Converter != null) {
               if (Converter.CanConvertFrom(typeof(string))) {
                  Value = Converter.ConvertFrom(value);
                  return;
               }

               if (Descriptor != null && Converter.CanConvertTo(Descriptor.PropertyType)) {
                  Value = Converter.ConvertTo(value, Descriptor.PropertyType);
                  return;
               }
            }

            if (Descriptor != null) {
               if (ConversionHelper.TryChangeType(value, Descriptor.PropertyType, out object v)) {
                  Value = v;
                  return;
               }
            }
            Value = value;
         }
      }

      public virtual DynamicObject TypeAttributes { get; private set; }

      public virtual object Value {
         get => GetProperty<object>();
         set {
            if (!TryChangeType(value, PropertyType, CultureInfo.CurrentCulture, out object changedValue)) {
               throw new ArgumentException("Cannot convert value {" + value + "} to type '" + PropertyType.FullName + "'.");
            }

            if (Descriptor != null) {
               try {
                  Descriptor.SetValue(ListSource.Data, changedValue);
                  object finalValue = Descriptor.GetValue(ListSource.Data);
                  SetValue(finalValue, true, false, true);
               }
               catch (Exception e) {
                  throw new ArgumentException("Cannot set value {" + value + "} to object.", e);
               }
            }
         }
      }

      public event EventHandler<PropertyGridEventArgs> Event;

      public static PropertyGridProperty FromEvent(RoutedEventArgs e) {
         if (e == null) {
            return null;
         }

         FrameworkElement fe = e.OriginalSource as FrameworkElement;
         if (fe == null) {
            return null;
         }

         return fe.DataContext as PropertyGridProperty;
      }

      public static bool IsEnumOrNullableEnum(Type type, out Type enumType, out bool nullable) {
         if (type == null) {
            throw new ArgumentNullException("type");
         }

         nullable = false;
         if (type.IsEnum) {
            enumType = type;
            return true;
         }

         if (type.Name == typeof(Nullable<>).Name) {
            Type[] args = type.GetGenericArguments();
            if (args.Length == 1 && args[0].IsEnum) {
               enumType = args[0];
               nullable = true;
               return true;
            }
         }

         enumType = null;
         return false;
      }

      public virtual void CanExecute(object sender, CanExecuteRoutedEventArgs e) {
         IPropertyGridCommandHandler handler = Value as IPropertyGridCommandHandler;
         if (handler != null) {
            handler.CanExecute(this, sender, e);
         }
      }

      public virtual void CloneValue(bool refresh) {
         if (_valueCloned && !refresh) {
            return;
         }

         ICloneable c = Value as ICloneable;
         _clonedValue = c != null ? c.Clone() : Value;
         _valueCloned = true;
      }

      int IComparable.CompareTo(object obj) {
         return CompareTo(obj as PropertyGridProperty);
      }

      public virtual int CompareTo(PropertyGridProperty other) {
         if (other == null) {
            throw new ArgumentNullException("other");
         }

         if (SortOrder != 0) {
            return SortOrder.CompareTo(other.SortOrder);
         }

         if (other.SortOrder != 0) {
            return -other.SortOrder.CompareTo(0);
         }

         if (DisplayName == null) {
            return 1;
         }

         return string.Compare(DisplayName, other.DisplayName, StringComparison.OrdinalIgnoreCase);
      }

      public virtual void Executed(object sender, ExecutedRoutedEventArgs e) {
         IPropertyGridCommandHandler handler = Value as IPropertyGridCommandHandler;
         if (handler != null) {
            handler.Executed(this, sender, e);
         }
      }

      public virtual void OnDescribed() {
      }

      public virtual void OnEvent(object sender, PropertyGridEventArgs e) {
         EventHandler<PropertyGridEventArgs> handler = Event;
         if (handler != null) {
            handler(sender, e);
         }
      }

      public virtual void OnValueChanged() {
         OnPropertyChanged("TextValue");
         OnPropertyChanged("BooleanValue");
         OnPropertyChanged("IsCollection");
         OnPropertyChanged("CollectionCount");
         OnPropertyChanged("IsDefaultValue");
      }

      public virtual bool RaiseOnPropertyChanged(string name) {
         return OnPropertyChanged(name);
      }

      public virtual void RefreshValueFromDescriptor(bool setChanged, bool forceRaise, bool trackChanged) {
         if (Descriptor == null) {
            return;
         }

         try {
            object value = Descriptor.GetValue(ListSource.Data);
            SetValue(value, setChanged, forceRaise, trackChanged);
         }
         catch (Exception e) {
            if (PropertyType == typeof(string)) {
               Value = e.GetAllMessages();
            }
            IsError = true;
         }
      }

      public void ResetClonedValue() {
         _valueCloned = false;
      }

      public virtual void SetValue(object value, bool setChanged, bool forceRaise, bool trackChanged) {
         bool set = SetProperty("Value", value, setChanged, forceRaise, trackChanged);
         if (set || forceRaise) {
            OnValueChanged();
         }
      }

      public override string ToString() {
         return Name;
      }

      public void UpdateCellBindings(Action<BindingExpression> action) {
         UpdateCellBindings(null, null, action);
      }

      public void UpdateCellBindings(string childName, Action<BindingExpression> action) {
         UpdateCellBindings(childName, null, action);
      }

      public virtual void UpdateCellBindings(string childName, Func<Binding, bool> where, Action<BindingExpression> action) {
         ListSource.Grid.UpdateCellBindings(this, childName, where, action);
      }

      protected virtual bool TryChangeType(object value, Type type, IFormatProvider provider, out object changedValue) {
         if (type == null) {
            throw new ArgumentNullException("type");
         }

         return ConversionHelper.TryChangeType(value, type, provider, out changedValue);
      }
   }
}