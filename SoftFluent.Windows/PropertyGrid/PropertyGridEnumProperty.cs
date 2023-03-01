using SoftFluent.Windows.Utilities;
using System.Linq;
using System.Reflection;
using PropertyGrid.Infrastructure;
using Utilities;

namespace SoftFluent.Windows
{
    public class PropertyGridEnumProperty : PropertyGridProperty
    {
        public PropertyGridEnumProperty(PropertyGridListSource provider, IActivator activator)
            : base(provider, activator)
        {
            EnumAttributes = provider.CreateDynamicObject();
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