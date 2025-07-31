using BlazamSetup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using static System.Net.Mime.MediaTypeNames;

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for IISPrerequisiteCheck.xaml
    /// </summary>
    public partial class IISPrerequisiteCheck : UserControl, IInstallationStep
    {
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
            CheckForWebSockets();
            CheckForAppInit();
        }
       
        void CheckForAspCoreHosting()
        {
            
                if (PrerequisiteChecker.CheckForAspCoreHosting())
                {
                    CurrentDispatcher.Invoke(() => {
                        frameworkCheckbox.IsChecked = true;
                    });
                }
                else
                {
                    CurrentDispatcher.Invoke(() => {
                        frameworkCheckbox.IsChecked = false;
                    });
                }
            UpdateNextButton();
        }

        private void UpdateNextButton()
        {
            CurrentDispatcher.Invoke(() => {
                if (websocketsCheckbox.IsChecked==true && frameworkCheckbox.IsChecked == true)
                {
                    MainWindow.EnableNext();
                }
            });
          
        }

        void CheckForWebSockets()
        {
            
                if (PrerequisiteChecker.CheckForWebSockets())
                {
                    CurrentDispatcher.Invoke(() => {
                        websocketsCheckbox.IsChecked = true;
                        enableWebSocketsButton.IsEnabled = false;
                    });
                }
                else
                {
                    CurrentDispatcher.Invoke(() => {
                        websocketsCheckbox.IsChecked = false;
                        enableWebSocketsButton.IsEnabled = true;
                    });
                }
            UpdateNextButton();

        }

        void CheckForAppInit()
        {
            if (PrerequisiteChecker.CheckForApplicationInitializationModule())
            {
                CurrentDispatcher.Invoke(() => {
                    appInitCheckbox.IsChecked = true;
                    enableAppInitButton.IsEnabled = false;
                });
            }
            else
            {
                CurrentDispatcher.Invoke(() => {
                    appInitCheckbox.IsChecked = false;
                    enableAppInitButton.IsEnabled = true;
                });
            }
            UpdateNextButton();
        }

        IInstallationStep IInstallationStep.NextStep()
        {
            return new ConfigureIIS();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
                Process.Start("https://aka.ms/dotnet-download");
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CurrentDispatcher.Invoke(() => {
                enableWebSocketsButton.IsEnabled = false;
                progressBar.Visibility = Visibility.Visible;
            });
            try
            {
                await Task.Run(() => {
                    var dism = Process.Start("Dism", "/online /Enable-Feature /FeatureName:IIS-WebSockets /All");
                    dism.WaitForExit();
                });
                
            }catch (Exception ex)
            {
                
            }
            CurrentDispatcher.Invoke(() => {
                progressBar.Visibility = Visibility.Hidden;
            });
            CheckForWebSockets();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            CurrentDispatcher.Invoke(() => {
                enableAppInitButton.IsEnabled = false;
                progressBar.Visibility = Visibility.Visible;
            });
            try
            {
                await Task.Run(() => {
                    var dism = Process.Start("Dism", "/online /Enable-Feature /FeatureName:IIS-ApplicationInit /All");
                    dism.WaitForExit();
                });

            }
            catch (Exception ex)
            {

            }
            CurrentDispatcher.Invoke(() => {
                progressBar.Visibility = Visibility.Hidden;
            });
            CheckForAppInit();
        }
    }
}
