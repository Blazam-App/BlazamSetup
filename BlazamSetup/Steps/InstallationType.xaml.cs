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

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for InstallationType.xaml
    /// </summary>
    public partial class InstallationType : UserControl, IInstallationStep
    {
        public InstallationType()
        {
            InitializeComponent();
            if (InstallationConfiguraion.InstallationType == null)
            {

                InstallationConfiguraion.InstallationType = InstallType.IIS;

            }
            switch (InstallationConfiguraion.InstallationType)
            {
                case InstallType.IIS:
                    iisInstall.IsChecked = true; break;
                case InstallType.Service:
                    serviceInstall.IsChecked = true; break;
            }

        }


        IInstallationStep IInstallationStep.NextStep()
        {
            switch (InstallationConfiguraion.InstallationType)
            {
                case InstallType.IIS:
                    return new ConfigureIIS();
                case InstallType.Service:
                    return new ConfigureService();
            }
            return null;
        }

        private void iisInstall_Checked(object sender, RoutedEventArgs e)
        {

            InstallationConfiguraion.InstallationType = InstallType.IIS;

        }

        private void serviceInstall_Checked(object sender, RoutedEventArgs e)
        {

            InstallationConfiguraion.InstallationType = InstallType.Service;

        }
    }
}
