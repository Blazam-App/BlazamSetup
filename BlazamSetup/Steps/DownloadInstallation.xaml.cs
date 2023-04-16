﻿using BlazamSetup.Services;
using System;
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
    /// Interaction logic for DownloadInstallation.xaml
    /// </summary>
    public partial class DownloadInstallation : UserControl,IInstallationStep
    {
        public Dispatcher CurrentDispatcher { get; }

        public DownloadInstallation()
        {
            InitializeComponent();
            CurrentDispatcher = Dispatcher;

            MainWindow.DisableNext();

            DownloadService.DownloadPercentageChanged += DownloadProgressMade;
            DownloadLatestVersion();
        }

        IInstallationStep IInstallationStep.NextStep()
        {
            return new InstallationType();

        }
        private void DownloadProgressMade(int value)
        {
            CurrentDispatcher.Invoke(() => {
                DownloadProgressBar.Value = value;
            });
        }

        private async Task DownloadLatestVersion()
        {



            if (await DownloadService.Download())
            {

                CurrentDispatcher.Invoke(() => {
                    DownloadingNote.Visibility = Visibility.Hidden;
                    MainWindow.EnableNext();
                });


            }
        }
    }
}
