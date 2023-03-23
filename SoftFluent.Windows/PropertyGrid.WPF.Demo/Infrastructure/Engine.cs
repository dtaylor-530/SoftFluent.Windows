using Abstractions;
using PropertyGrid.Abstractions;
using SoftFluent.Windows;
using SoftFluent.Windows.Samples;
using System;
using System.Threading.Tasks;

namespace PropertyGrid.WPF.Demo.Infrastructure
{
    public class Engine : IPropertyGridEngine
    {
        private readonly IActivator activator;


        public Engine(IActivator activator)
        {
            this.activator = activator;

            Initialise();
        }

        async void Initialise()
        {
         
        }

        public  IPropertySource Convert(IPropertyGridOptions options)
        {

        
            return new PropertySource(activator, options) { };
                //await PropertyStore2.Instance.GetGuid(guid.Guid, options.GetType().Name, options.Data.GetType());
            
      
        }

        public static Engine Instance { get; } = new Engine(new BaseActivator());
    }
}
