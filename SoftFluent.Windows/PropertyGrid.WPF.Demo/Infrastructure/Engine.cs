using PropertyGrid.Abstractions;
using SoftFluent.Windows;
using System;

namespace PropertyGrid.WPF.Demo.Infrastructure
{
    public class Engine : IPropertyGridEngine
    {
        public Guid Guid { get; } = Guid.Parse("7e0c787a-30d0-4038-9376-2808cc66a389");
        
        public Engine()
        {
        }

        public IPropertyNode Convert(object data)
        {
            return new PropertyNode(Guid) { Data = data };
        }

        public static Engine Instance { get; } = new Engine();
    }
}
