using PropertyGrid.WPF.Demo.Infrastructure;
using System;
using System.Collections;
using System.Windows;

namespace PropertyGrid.WPF.Demo
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlWindow : Window
    {
        private readonly IControllable controllable;
        private readonly IHistory history;

        public ControlWindow(IControllable controllable, IHistory history)
        {
            InitializeComponent();
            this.controllable = controllable;
            this.history = history;

            HistoryPanel.DataContext = history; 
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            controllable.Back();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            controllable.Play();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            controllable.Pause();
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            controllable.Forward();
        }
    }

    public interface IControllable : IObservable<ControlType>
    {
        void Back();
        void Play();
        void Pause();
        void Forward();
    }

    public interface IHistory : IObservable<object>
    {
        IEnumerable Past { get; }
        object Present { get; }
        IEnumerable Future { get; }

        void Add(object order);
        void Back();
        void Forward();
    }
}
