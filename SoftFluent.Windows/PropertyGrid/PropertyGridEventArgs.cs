using System.ComponentModel;
using Abstractions;

namespace SoftFluent.Windows
{
   public class PropertyGridEventArgs : CancelEventArgs, IPropertyGridEventArgs
   {
        public PropertyGridEventArgs(PropertyGridProperty property)
            : this(property, null)
        {
        }

        public PropertyGridEventArgs(PropertyGridProperty property, object context)
        {
            Property = property;
            Context = context;
        }

        public IPropertyGridProperty Property { get; }
        public object Context { get; set; }
    }
}