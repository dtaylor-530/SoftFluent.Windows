using PropertyGrid.Abstractions;
using SoftFluent.Windows;
using SoftFluent.Windows.Samples;
using System;

namespace PropertyGrid.WPF.Demo.Infrastructure
{
    public class Engine : IPropertyGridEngine
    {
        private readonly IActivator activator;
        Guid guid = Guid.Parse("901f3c6d-1424-4771-8672-0b77d7c44342");

        public Engine(IActivator activator)
        {
            this.activator = activator;

            Initialise();
        }

        async void Initialise()
        {
            await PropertyStore2.Instance.GetGuid(guid, nameof(PropertySource), typeof(PropertySource));
        }

        public IPropertySource Convert(IPropertyGridOptions options)
        {
            return new PropertySource(guid, activator, options) { };
        }

        public static Engine Instance { get; } = new Engine(new BaseActivator());
    }
}
