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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            DownloadButton.IsEnabled = false;
            RecheckButton.IsEnabled = false;
            DownloadButton.Content = "Downloading/Installing...";
            try
            {
                await DependencyManager.DownloadAndInstallAspNetCoreRuntime();
                CheckForAspCoreRuntime();
            }
            catch
            {
                MessageBox.Show("Failed to download or install the ASP.NET Core Runtime. Please try again or install it manually.", "Error");
            }
            finally
            {
                DownloadButton.Content = "Download Prerequisites";
                DownloadButton.IsEnabled = true;
                RecheckButton.IsEnabled = true;
            }
        }

        private void Recheck_Click(object sender, RoutedEventArgs e)
        {
            CheckForAspCoreRuntime();
        }
    }
}
