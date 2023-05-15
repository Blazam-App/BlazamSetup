﻿using BlazamSetup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for Install.xaml
    /// </summary>
    public partial class Install : UserControl, IInstallationStep
    {
        private string currentStep;

        public Install()
        {
            InitializeComponent();
            CurrentDispatcher = Dispatcher;
            MainWindow.DisableNext();
            InstallationService.OnProgress += (value) => StepProgress = value;
            InstallationService.OnStepTitleChanged += (value) => CurrentStep = value;
            InstallationService.OnInstallationFinished += () => MainWindow.EnableNext();
            RunInstallation();
        }
        public string CurrentStep
        {
            get => currentStep; set
            {
                currentStep = value;
                CurrentDispatcher.Invoke(() => { CurrentStepLabel.Content = value; });
            }
        }
        private double stepProgress;

        public double StepProgress
        {
            get => stepProgress; set
            {
                stepProgress = value;
                CurrentDispatcher.Invoke(() => { InstallProgressBar.Value = value; });

            }
        }
        public Dispatcher CurrentDispatcher { get; }

        IInstallationStep IInstallationStep.NextStep()
        {
            App.Quit();
            return new Welcome();
        }

        private async void RunInstallation()
        {
            await InstallationService.StartInstallationAsync();
        }

      
    }
}
