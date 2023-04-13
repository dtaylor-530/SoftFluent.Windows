using System;
using System.Collections;
using System.Windows;
//using System.Windows.Forms;
using System.Windows.Input;
using Abstractions;
using PropertyGrid.Demo.Model;
using PropertyGrid.Infrastructure;
using PropertyGrid.WPF.Demo;
using PropertyGrid.WPF.Demo.Infrastructure;
using SoftFluent.Windows.Diagnostics;
using Utility.Collections;
using Application = System.Windows.Application;

namespace SoftFluent.Windows.Samples
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {

            SQLitePCL.Batteries.Init();
            var propertyStore = new WebStore();
            AutoObject.PropertyStore = BaseActivator.PropertyStore = propertyStore;
            Collection.Context = DispatcherTimer.Context = System.Threading.SynchronizationContext.Current;
            BaseActivator.Interfaces = new() { { typeof(IViewModel), typeof(ViewModel) } };
            var window = new Window { Content = new PropertyView { DataContext = new Model() } };
            window.Show();
            var controlWindow = new ControlWindow(propertyStore.Controllable, propertyStore.History);
            SetOnSecondScreen(controlWindow);
            controlWindow.Show();

            base.OnStartup(e);
#if DEBUG
            Tracing.Enable();
#endif
        }


        void SetOnSecondScreen(Window window)
        {
            Screen s = Screen.AllScreens[1];
            System.Drawing.Rectangle r = s.WorkingArea;
            window.Top = r.Top;
            window.Left = r.Left;
        }


        public class WebStore : PropertyStore
        {
            private HttpRepository store = new();

            protected override IRepository Repository => store;
        }
    }
}
