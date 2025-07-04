﻿using BlazamSetup.Services;
using Org.BouncyCastle.Asn1.X509;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace BlazamSetup.Steps.Uninstall
{
    /// <summary>
    /// Interaction logic for Uninstall.xaml
    /// </summary>
    public partial class Uninstall : UserControl, IInstallationStep
    {
        public Uninstall()
        {
            InitializeComponent();
           
            MainWindow.DisableNext();

            CurrentDispatcher = Dispatcher;
            InstallationService.OnInstallationFinished += () => MainWindow.EnableNext();
            RunUninstall();
        }

        public Dispatcher CurrentDispatcher { get; }

        public int Order => 2;

        private async void RunUninstall()
        {
            await InstallationService.StartUninstallAsync();
            await CurrentDispatcher.InvokeAsync((() =>
            {
                NavigationManager.Next();

            }));
        }

      

       

        IInstallationStep IInstallationStep.NextStep()
        {
            return new UninstallResults();
        }
    }
}
