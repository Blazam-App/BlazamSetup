using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using BlazamSetup.Services;

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for IISPrerequisiteCheck.xaml
    /// </summary>
    public partial class IISPrerequisiteCheck : UserControl, IInstallationStep
    {
        private const string DotNetDownloadUrl = "https://aka.ms/dotnet-download";

        public bool FrameworkInstalled { get; set; }
        public Dispatcher CurrentDispatcher { get; }


        public int Order => 4;

        public IISPrerequisiteCheck()
        {
            InitializeComponent();
            MainWindow.DisableNext();
            CurrentDispatcher = Dispatcher;
            DataContext = this;
            CheckForAspCoreHosting();

        }

        void CheckForAspCoreHosting()
        {

            if (PrerequisiteChecker.CheckForAspCoreHosting())
            {
                CurrentDispatcher.Invoke(() =>
                {
                    frameworkCheckbox.IsChecked = true;
                });
            }
            else
            {
                CurrentDispatcher.Invoke(() =>
                {
                    frameworkCheckbox.IsChecked = false;
                });
            }
            UpdateNextButton();
        }

        private void Recheck_Click(object sender, RoutedEventArgs e)
        {
            CheckForAspCoreHosting();
        }

        private void UpdateNextButton()
        {
            CurrentDispatcher.Invoke(() =>
            {
                if (frameworkCheckbox.IsChecked == true)
                {
                    MainWindow.EnableNext();
                }
            });

        }



        IInstallationStep IInstallationStep.NextStep()
        {
            return new ConfigureIIS();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            DownloadButton.IsEnabled = false;
            RecheckButton.IsEnabled = false;
            DownloadButton.Content = "Downloading/Installing...";
            progressBar.Visibility = Visibility.Visible;

            try
            {
                await DependencyManager.DownloadAndInstallHostingBundle();
                CheckForAspCoreHosting();
            }
            catch
            {
                MessageBox.Show("Failed to download or install the ASP.NET Core Hosting Bundle. Please try again or install it manually.", "Error");
            }
            finally
            {
                DownloadButton.Content = "Download Prerequisites";
                DownloadButton.IsEnabled = true;
                RecheckButton.IsEnabled = true;
                progressBar.Visibility = Visibility.Hidden;
            }
        }

    }
}
