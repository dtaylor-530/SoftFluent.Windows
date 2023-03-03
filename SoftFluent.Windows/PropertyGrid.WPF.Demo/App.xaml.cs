using System.Windows;
using System.Windows.Input;
using SoftFluent.Windows.Diagnostics;

namespace SoftFluent.Windows.Samples
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var window = new Window { Content = new Customer2() };
            window.Show();

            base.OnStartup(e);
#if DEBUG
            Tracing.Enable();
#endif
        }

    }
}
