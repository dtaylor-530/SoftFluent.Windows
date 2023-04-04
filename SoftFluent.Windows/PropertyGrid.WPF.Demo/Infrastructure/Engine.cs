using Abstractions;
using PropertyGrid.Abstractions;
using SoftFluent.Windows;
using SoftFluent.Windows.Samples;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace PropertyGrid.WPF.Demo.Infrastructure
{
    public class Engine : IPropertyGridEngine
    {
        public Engine()
        {
        }

        public IEnumerable Convert(object data)
        {
            return new PropertyNode(Guid.NewGuid()) { Data = data };
        }

        public static Engine Instance { get; } = new Engine();
    }
}
