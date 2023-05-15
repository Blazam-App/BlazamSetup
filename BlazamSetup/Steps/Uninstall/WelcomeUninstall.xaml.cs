using BlazamSetup.Services;
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

namespace BlazamSetup.Steps.Uninstall
{
    /// <summary>
    /// Interaction logic for WelcomeUninstall.xaml
    /// </summary>
    public partial class WelcomeUninstall : UserControl,IInstallationStep
    {
        public WelcomeUninstall()
        {
            InitializeComponent();
            if (!RegistryService.InstallationExists)
            {
                Label notInstalledLabel = new Label() { Content="Blazam is not currently installed."};
                MainWindow.DisableNext();
                textStackPanel.Children.Clear();
                textStackPanel.Children.Add(notInstalledLabel);
            }

        }

        IInstallationStep IInstallationStep.NextStep()
        {
    return new Uninstall();
        }
    }
}
