using System;
using System.Collections;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Abstractions;
using Models;
using PropertyGrid.Demo.Model;
using PropertyGrid.WPF.Demo;
using PropertyGrid.WPF.Demo.Infrastructure;
using SoftFluent.Windows.Diagnostics;
using Application = System.Windows.Application;

namespace SoftFluent.Windows.Samples
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {

            SQLitePCL.Batteries.Init();

            AutoObject.PropertyStore = PropertyStore.Instance;
            Collection.Context = System.Threading.SynchronizationContext.Current;
            BaseActivator.Interfaces = new() { { typeof(IViewModel), typeof(ViewModel) } };
            var window = new Window { Content = new Customer2() };
            window.Show();
            var controlWindow = new ControlWindow(PropertyStore.Instance.Controllable, PropertyStore.Instance.History);
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
    }
}
