using SoftFluent.Windows.Utilities;
using System.Linq;
using System.Reflection;
using PropertyGrid.Infrastructure;
using Utilities;
using SoftFluent.Windows;
using PropertyGrid.Abstractions;

namespace PropertyGrid.Extra
{
    public class PropertyGridEnumProperty : PropertyGridProperty
    {
        public PropertyGridEnumProperty(IPropertyGridOptions provider)
            : base(provider)
        {
            EnumAttributes = new DynamicObject();
        }

        public override void OnValueChanged()
        {
            base.OnValueChanged();
            EnumAttributes.Properties.Clear();
            foreach (FieldInfo fi in PropertyType.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (fi.Name.Equals($"{base.Value}"))
                {
                    EnumAttributes.AddDynamicProperties(fi.GetAttributes<PropertyGridAttribute>().ToArray());
                }
            }
        }

        public virtual DynamicObject EnumAttributes { get; private set; }
    }
}