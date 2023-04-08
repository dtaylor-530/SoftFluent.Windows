﻿using System.Windows;
using Utility.Commands;
using Utility.Enums;

namespace PropertyGrid.WPF.Demo
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlWindow : Window
    {
        public ControlWindow(IControllable controllable, IHistory history)
        {
            InitializeComponent();

            HistoryPanel.DataContext = history;

            StepButtons.Enabled = Step.Walk| Step.Backward| Step.Forward ;

            StepButtons.Command = new Command<Step>(step =>
            {
                switch (step)
                {
                    case Step.None:
                        break;
                    case Step.Backward:
                        controllable.Back();
                        break;
                    case Step.Forward:
                        controllable.Forward();             
                        break;
                    case Step.Walk:
                        controllable.Play();
                        StepButtons.Enabled = Step.Wait;
                        break;
                    case Step.Wait:
                        controllable.Pause();
                        StepButtons.Enabled = Step.Walk | Step.Backward | Step.Forward;
                        break;
                    case Step.All:
                        break;
                }
            });
        }
    }
}
