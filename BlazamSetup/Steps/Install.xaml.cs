using BlazamSetup.Services;
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

        public Install()
        {
            InitializeComponent();
            MainWindow.DisableNext();
            CurrentDispatcher = Dispatcher;
            InstallationService.OnInstallationFinished += () =>
            {
                CurrentDispatcher.Invoke((() =>
                {
                    NavigationManager.Next();

                }));
            };

            RunInstallation();
        }

        public Dispatcher CurrentDispatcher { get; }

        public int Order => 10;

        IInstallationStep IInstallationStep.NextStep()
        {
            return new PostInstallation();
        }

        private async void RunInstallation()
        {
            await InstallationService.StartInstallationAsync();
        }


    }
}
