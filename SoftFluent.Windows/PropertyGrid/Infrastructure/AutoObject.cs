using Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace SoftFluent.Windows
{

    public record Key(Guid Guid, string Name) : IKey
    {
        public bool Equals(IKey? other)
        {
            return this.Equals(other as Key);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
    }

    /// <summary>
    /// Defines a utility class to implement objects with typed properties without private fields.
    /// This class supports automatically property change notifications and error validations.
    /// </summary>
    public abstract class AutoObject : IDataErrorInfo, INotifyPropertyChanged, IKey, IObserver
    {
        //private readonly Dictionary<string, object> _defaultValues = new Dictionary<string, object>();
        private readonly Guid guid;
        public static IPropertyStore PropertyStore { get; set; }
        private IDisposable disposable;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoObject"/> class.
        /// </summary>
        protected AutoObject(Guid guid)
        {
            this.guid = guid;

            Initialise();
        }

        async void Initialise()
        {
            //lock (PropertyStore)
            //    PropertyStore?.Subscribe(this);
        }


        public Guid Guid => guid;


        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An error message indicating what is wrong with this object. The default is an empty string ("").
        /// </returns>
        string IDataErrorInfo.Error => Validate(null);

        /// <summary>
        /// Gets a value indicating whether at least a property has changed since the last time this property has been set to false.
        /// </summary>
        /// <value>
        /// <c>true</c> if at least a property has changed; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        [Browsable(false)]
        public virtual bool HasChanged { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        [Browsable(false)]
        public virtual bool IsValid => Validate(null) == null;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the <see cref="string"/> with the specified column name.
        /// </summary>
        /// <value></value>
        string IDataErrorInfo.this[string columnName] => Validate(columnName);


        /// <summary>
        /// Gets the default value for a given property.
        /// </summary>
        /// <param name="propertyName">The property name. May not be null.</param>
        /// <returns>The default value. May be null.</returns>
        protected virtual object GetDefaultValue(PropertyDescriptor descriptor)
        {
            if (descriptor.Name == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            PropertyInfo pi = descriptor.ComponentType.GetProperty(descriptor.Name);
            if (pi == null)
            {
                //if (ThrowOnInvalidProperty)
                //{
                //throw new InvalidOperationException(SR.GetString("invalidPropertyName", GetType().FullName, propertyName));
                throw new InvalidOperationException("invalidPropertyName");
                // }

                //  return null;
            }

            object defaultValue = pi.PropertyType.IsValueType ? Activator.CreateInstance(pi.PropertyType) : null;
            DefaultValueAttribute att = Attribute.GetCustomAttribute(pi, typeof(DefaultValueAttribute), true) as DefaultValueAttribute;
            if (att != null)
            {
                return ConversionHelper.ChangeType(att.Value, defaultValue);
            }

            return defaultValue;
        }





        /// <summary>
        /// Called when a property changed.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="setChanged">if set to <c>true</c> set the HasChanged property to true.</param>
        /// <param name="forceRaise">if set to <c>true</c> force the raise, even if RaisePropertyChanged is set to false.</param>
        /// <returns>
        /// true if the event has been raised; otherwise false.
        /// </returns>
        protected virtual bool OnPropertyChanged(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            return true;
        }

        /// <summary>
        /// Gets a property value.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <returns>The value automatically converted into the requested type.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual T GetProperty<T>([CallerMemberName] string? name = null)
        {
            disposable ??= PropertyStore.Subscribe(this);
            var result = PropertyStore.GetValue<T>(new Key(this.guid, name));

            return result;
        }

        /// <summary>
        /// Sets a property value.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>true if the value has changed; otherwise false.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool SetProperty(object value, [CallerMemberName] string name = null)
        {
            disposable ??= PropertyStore.Subscribe(this);

            //return SetProperty(name, value);
            PropertyStore.SetValue(new Key(Guid, name), value);
            return true;
        }



        /// <summary>
        /// Gets a property value.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <returns>The value automatically converted into the requested type.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual object GetProperty(Type type, [CallerMemberName] string? name = null)
        {
            var result = PropertyStore.GetValue(new Key(this.guid, name), type);

            return result;
        }

        /// <summary>
        /// Sets a property value.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>true if the value has changed; otherwise false.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool SetProperty(object value, Type type, [CallerMemberName] string name = null)
        {
            //return SetProperty(name, value);
            PropertyStore.SetValue(new Key(Guid, name), value, type);
            return true;
        }



        /// <summary>
        /// Validates the specified member name.
        /// </summary>
        /// <param name="memberName">The member to validate or null to validate all members.</param>
        /// <returns>A string if an error occured; null otherwise.</returns>
        protected virtual string Validate(string? memberName)
        {

            return PropertyStore.Validate(memberName);
        }

        public bool Equals(IKey? other)
        {
            return this.Guid == ((other as AutoObject)?.Guid ?? (other as Key)?.Guid);
        }


        public override int GetHashCode()
        {
            return this.Guid.GetHashCode();
        }


        public void OnNext(IPropertyChange propertyResult)
        {
            OnPropertyChanged(propertyResult.Name);
        }
    }
}