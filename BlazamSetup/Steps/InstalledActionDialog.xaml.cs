using BlazamSetup.Steps.Repair;
using BlazamSetup.Steps.Uninstall;
using BlazamSetup.Steps.Update;
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
    /// Interaction logic for InstalledActionDialog.xaml
    /// </summary>
    public partial class InstalledActionDialog : UserControl,IInstallationStep
    {

        public InstalledActionDialog()
        {
            InitializeComponent();
            if (InstallationConfiguraion.ExecutableExists)
                exeNotFoundWarning.Visibility = Visibility.Collapsed;
            if (!InstallationConfiguraion.InstalledVersion.IsNullOrEmpty())
            {
                installedVrsionLabel.Visibility = Visibility.Visible;
                installedVrsionLabel.Content = "Version: "+InstallationConfiguraion.InstalledVersion;
            }
            

        }

        private void Update_Checked(object sender, RoutedEventArgs e)
        {
            InstallationConfiguraion.InstalledAction = InstalledAction.Update;
        }

        private void Repair_Checked(object sender, RoutedEventArgs e)
        {
            InstallationConfiguraion.InstalledAction = InstalledAction.Repair;

        }

        private void Remove_Checked(object sender, RoutedEventArgs e)
        {

            InstallationConfiguraion.InstalledAction = InstalledAction.Remove;
        }

        IInstallationStep IInstallationStep.NextStep()
        {
           switch(InstallationConfiguraion.InstalledAction)
            {
                case InstalledAction.Repair:
                    return new WelcomeRepair();
                case InstalledAction.Update:
                    return new WelcomeUpdate();
                case InstalledAction.Remove:
                default:
                    return new WelcomeUninstall();
            }
        }
    }
}
