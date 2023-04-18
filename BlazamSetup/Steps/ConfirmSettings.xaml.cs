﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for ConfirmSettings.xaml
    /// </summary>
    public partial class ConfirmSettings : UserControl,IInstallationStep
    {
        public ConfirmSettings()
        {
            InitializeComponent();
            CurrentDispatcher = Dispatcher;
        }

        public Dispatcher CurrentDispatcher { get; }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            if (oldParent == null)
                CurrentDispatcher.Invoke(() =>
                {
                    MainWindow.NextStepButton.Content = "Install";
                });

        }

        IInstallationStep IInstallationStep.NextStep()
        {
            return new Install();
        }
    }
  
}
