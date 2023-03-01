using PropertyGrid.Abstractions;
using SoftFluent.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyGrid.WPF.Demo.Infrastructure
{
    public class Engine : IPropertyGridEngine
    {
        private readonly IActivator activator;

        public Engine(IActivator activator)
        {
            this.activator = activator;
        }

        public IPropertyEngine Convert(IPropertyGridOptions options)
        {
            return new PropertyGridListSource(activator, options) { };
        }

        public static Engine Instance { get; } = new Engine(new BaseActivator());
    }
}
