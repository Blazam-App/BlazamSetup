using BlazamSetup.Services;
using BlazamSetup.Steps;
using BlazamSetup.Steps.Uninstall;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace BlazamSetup
{
    public delegate void InstallEvent();
    public delegate void InstallEvent<T>(T value);
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                CurrentDispatcher = Dispatcher;

                InstallerFrame = Frame;
                LastStepButton = BackButton;
                NextStepButton = NextButton;
                ActionTextLabel = ActionLabel;
                MainWindow.InstallerFrame.ContentRendered += InstallerFrame_ContentRendered;
                if (RegistryService.InstallationExists)
                {
                    InstallationConfiguraion.ProductInformation = RegistryService.GetProductInformation();
                    InstallationConfiguraion.InstallDirPath = RegistryService.InstallLocation;
                    InstallationConfiguraion.InstalledVersion = FileSystemService.GetFileVersion(InstallationConfiguraion.ProductInformation.InstallLocation+"\\Blazam.exe");
                }
                if (App.StartupArgs.Args.Any(arg => arg.StartsWith("/u")))
                {
                    Log.Information("Uninstaller Started");
                    MainWindow.InstallerFrame.Navigate(new WelcomeUninstall());

                }
                else
                {
                 
                    Log.Information("Installer Started");
                    if (RegistryService.InstallationExists)
                    {
                        MainWindow.InstallerFrame.Navigate(new InstalledActionDialog());

                    }
                    else
                    {
                        MainWindow.InstallerFrame.Navigate(NavigationManager.CurrentPage);

                    }

                }
            }catch (Exception ex)
            {
                Log.Error("Uncaught Exception: {@Error}", ex);
            }

        }

        private void InstallerFrame_ContentRendered(object sender, EventArgs e)
        {
            NavigationManager.CurrentPage = InstallerFrame.Content as IInstallationStep;
            if (NavigationManager.CurrentPage.GetType() == typeof(Welcome))
            {
                BackButton.IsEnabled = false;
            }
            else
            {
                BackButton.IsEnabled = true;

            }
        }

        public static Frame InstallerFrame { get; private set; }
        public static Label ActionTextLabel  { get; private set; }
        public static Button LastStepButton { get; private set; }
        public static Button NextStepButton { get; private set; }
        public static Dispatcher CurrentDispatcher { get; private set; }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            SetActionLabel("");
            NavigationManager.Next();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SetActionLabel("");

            NavigationManager.Back();
        }

        internal static void CollapseBack()
        {
            CurrentDispatcher.Invoke(() =>
            {
                LastStepButton.Visibility = Visibility.Collapsed;

            });

        }
         internal static void SetActionLabel(string text)
        {
            CurrentDispatcher.Invoke(() =>
            {
                ActionTextLabel.Content= text;

            });

        }
        internal static void EnableNext()
        {
            CurrentDispatcher.Invoke(() =>
            {
                NextStepButton.IsEnabled = true;

            });

        }

        internal static void DisableNext()
        {
            CurrentDispatcher.Invoke(() =>
            {
                NextStepButton.IsEnabled = false;

            });

        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            App.Current.MainWindow.DragMove();
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key== Key.Enter)
            {
                if (NextButton.IsEnabled)
                {
                    NextButton_Click(null,null);
                }
            }
        }

        internal static void DisableBack()
        {
            CurrentDispatcher.Invoke(() =>
            {
                LastStepButton.IsEnabled = false;

            });
        }

        internal static void SetNextText(string text)
        {
            CurrentDispatcher.Invoke(() =>
            {
                NextStepButton.Content = text;

            });
        }
    }
}
