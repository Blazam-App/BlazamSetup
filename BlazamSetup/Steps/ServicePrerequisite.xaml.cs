using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using BlazamSetup.Services;

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for ServicePrerequisite.xaml
    /// </summary>
    public partial class ServicePrerequisite : UserControl, IInstallationStep
    {
        public ServicePrerequisite()
        {
            InitializeComponent();
            CurrentDispatcher = Dispatcher;
            CheckForAspCoreRuntime();
        }

        public Dispatcher CurrentDispatcher { get; }

        public int Order => 4;

        void CheckForAspCoreRuntime()
        {

            if (PrerequisiteChecker.CheckForAspCore())
            {
                CurrentDispatcher.Invoke(() =>
                {
                    frameworkCheckbox.IsChecked = true;
                    MainWindow.EnableNext();
                });
            }
            else
            {
                CurrentDispatcher.Invoke(() =>
                {
                    frameworkCheckbox.IsChecked = false;
                    MainWindow.DisableNext();

                });
            }

        }


        IInstallationStep IInstallationStep.NextStep()
        {
            return new ConfigureService();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


            Process.Start("https://aka.ms/dotnet-download");

        }

        private void Recheck_Click(object sender, RoutedEventArgs e)
        {
            CheckForAspCoreRuntime();
        }
    }
}
