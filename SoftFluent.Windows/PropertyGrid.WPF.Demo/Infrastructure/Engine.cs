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

            Initialise();
        }

        async void Initialise()
        {

        }

        public IEnumerable Convert(object data)
        {


            return new PropertySource(Guid.NewGuid()) { Data = data };
            //await PropertyStore2.Instance.GetGuid(guid.Guid, options.GetType().Name, options.Data.GetType());


        }

        public static Engine Instance { get; } = new Engine();
    }
}
