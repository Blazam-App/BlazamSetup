using BlazamSetup.Services;
using BlazamSetup.Steps;
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
            InitializeComponent();
            CurrentDispatcher = Dispatcher;

            InstallerFrame = Frame;
            NextStepButton = NextButton;
            MainWindow.InstallerFrame.Navigate(NavigationManager.CurrentPage);
            MainWindow.InstallerFrame.ContentRendered += InstallerFrame_ContentRendered;
        }

        private void InstallerFrame_ContentRendered(object sender, EventArgs e)
        {
            NavigationManager.CurrentPage = InstallerFrame.Content as IInstallationStep;
           if(NavigationManager.CurrentPage.GetType() == typeof(Welcome))
            {
                BackButton.IsEnabled = false;
            }
            else
            {
                BackButton.IsEnabled = true;

            }
        }

        public static Frame InstallerFrame { get; private set; }
        public static Button NextStepButton { get; private set; }
        public static Dispatcher CurrentDispatcher { get; private set; }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Next();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Back();
        }

        internal static void EnableNext()
        {
            CurrentDispatcher.Invoke(() => {
                NextStepButton.IsEnabled = true;

            });

        }

        internal static void DisableNext()
        {
            CurrentDispatcher.Invoke(() => {
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
    }
}
