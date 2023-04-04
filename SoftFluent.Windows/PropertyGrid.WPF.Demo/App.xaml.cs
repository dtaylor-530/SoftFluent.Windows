using System;
using System.Windows;
using System.Windows.Input;
using PropertyGrid.WPF.Demo;
using PropertyGrid.WPF.Demo.Infrastructure;
using SoftFluent.Windows.Diagnostics;

namespace SoftFluent.Windows.Samples
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {

            SQLitePCL.Batteries.Init();

            AutoObject.PropertyStore = PropertyStore2.Instance;
            AutoObject.Context = System.Threading.SynchronizationContext.Current;
            var window = new Window { Content = new Customer2() };
            window.Show();
            new ControlWindow(PropertyStore2.Instance.Controllable, PropertyStore2.Instance.History).Show();

            base.OnStartup(e);
#if DEBUG
            Tracing.Enable();
#endif
        }
    }
}
