using SoftFluent.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyGrid
{
    public class RootProperty : PropertyBase
    {
        public RootProperty(Guid guid) : base(guid)
        {
        }

        public override string Name => PropertyType.Name;

        public override bool IsReadOnly => false;

        public override object Value { get; set; }
    }
}
