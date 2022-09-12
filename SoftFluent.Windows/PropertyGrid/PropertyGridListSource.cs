using SoftFluent.Windows.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Abstractions;
using PropertyGrid.Infrastructure;
using Utilities;

namespace SoftFluent.Windows {

   public class PropertyGridListSource : IPropertyGridListSource, IListSource {
      private readonly IActivator _activator;
      private readonly int _inheritanceLevel;
      private readonly ObservableCollection<IPropertyGridProperty> _properties = new ObservableCollection<IPropertyGridProperty>();

      public PropertyGridListSource(IActivator  activator, IPropertyGrid grid, object data, int inheritanceLevel = 0) {
         _activator = activator;
         _inheritanceLevel = inheritanceLevel;

         Grid = grid ?? throw new ArgumentNullException("grid");
         Data = data ?? throw new ArgumentNullException("data");

         UpdateProperties();

         async void UpdateProperties() {
            foreach (PropertyGridProperty prop in Properties()) {
               await Task.Delay(10);
               await Grid.InvokeAsync(() => _properties.Add(prop));
            }

            if (data is IPropertyGridObject pga) {
               pga.FinalizeProperties(this, _properties);
            }
         }
      }

      bool IListSource.ContainsListCollection => false;

      public object Data { get; }

      public IPropertyGrid Grid { get; }

      public virtual IPropertyGridProperty AddProperty(string propertyName) {
         if (propertyName == null) {
            throw new ArgumentNullException("propertyName");
         }

         if (_properties.FirstOrDefault(p => p.Name == propertyName) is var prop) {
            if (PropertyDescription() is PropertyDescriptor desc) {
               if (CreateProperty(desc) is var createdProp) {
                  _properties.Add(createdProp);
               }
            }

            return prop;
         }

         return null;

         PropertyDescriptor PropertyDescription() {
            return TypeDescriptor
                .GetProperties(Data)
                .OfType<PropertyDescriptor>()
                .FirstOrDefault(p => p.Name == propertyName);
         }
      }

      public virtual DynamicObject CreateDynamicObject() {
         return _activator.CreateInstance<DynamicObject>();
      }

      public virtual PropertyGridProperty CreateProperty() {
         return _activator.CreateInstance<PropertyGridProperty>(this);
      }

      public virtual PropertyGridProperty CreateProperty(PropertyDescriptor descriptor) {
         return Helper.CreateProperty(this, descriptor, _activator);
      }

      public virtual void Describe(PropertyGridProperty property, PropertyDescriptor descriptor) {
         Grid.Describe(property, descriptor);
      }

      public IPropertyGridProperty GetByName(string name) {
         return _properties.FirstOrDefault(p => p.Name.EqualsIgnoreCase(name));
      }

      IList IListSource.GetList() {
         return _properties;
      }

      protected virtual IEnumerable<PropertyGridProperty> Properties() {
         Type highestType = Data.GetType();

         foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(Data).Cast<PropertyDescriptor>().OrderBy(d => d.Name)) {
            int level = descriptor.ComponentType.InheritanceLevel(highestType);

            if (level <= _inheritanceLevel &&
                descriptor.IsBrowsable &&
                CreateProperty(descriptor) is PropertyGridProperty property) {
               yield return property;
            }
         }
      }
   }
}