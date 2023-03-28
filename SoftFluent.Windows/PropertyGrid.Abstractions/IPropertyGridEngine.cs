using SoftFluent.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyGrid.Abstractions
{
    public interface IPropertyGridEngine
    {
        IPropertySource Convert(object options);

    }
}
