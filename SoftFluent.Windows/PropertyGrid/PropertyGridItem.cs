using Abstractions;

namespace SoftFluent.Windows
{
    public class PropertyGridItem : AutoObject, IPropertyGridItem
    {
        public PropertyGridItem()
        {
            IsChecked = false;
        }

        public virtual bool IsZero { get { return GetProperty<bool>(); } set { SetProperty(value); } }
        public virtual string Name { get { return GetProperty<string>(); } set { SetProperty(value); } }
        public virtual object Value { get { return GetProperty<object>(); } set { SetProperty(value); } }
        public virtual bool? IsChecked { get { return GetProperty<bool?>(); } set { SetProperty(value); } }
        public virtual IPropertyGridProperty Property { get { return GetProperty<PropertyGridProperty>(); } set { SetProperty(value); } }
        public bool IsUnset { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}