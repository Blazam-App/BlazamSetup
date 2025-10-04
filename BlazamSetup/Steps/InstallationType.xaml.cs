using System.Windows;
using System.Windows.Controls;

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for InstallationType.xaml
    /// </summary>
    public partial class InstallationType : UserControl, IInstallationStep
    {
        public int Order => 3;

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
