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
    /// Interaction logic for ConfigureService.xaml
    /// </summary>
    public partial class ConfigureService : UserControl, IInstallationStep
    {
        public ConfigureService()
        {
            InitializeComponent();
            if (InstallationConfiguraion.WebHostConfiguration.ListeningAddress.IsNullOrEmpty()) 
                FQDNTextBox.Text = "*";
            else
                FQDNTextBox.Text = InstallationConfiguraion.WebHostConfiguration.ListeningAddress;

            HTTPPortTextBox.Text = InstallationConfiguraion.WebHostConfiguration.HttpPort.ToString();
            HTTPSPortTextBox.Text = InstallationConfiguraion.WebHostConfiguration.HttpsPort.ToString();
        }

      
        IInstallationStep IInstallationStep.NextStep()
        {
            return new InstallDirectory();
        }

        private void FQDNTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InstallationConfiguraion.WebHostConfiguration.ListeningAddress= FQDNTextBox.Text;
        }

        private void HTTPPortTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InstallationConfiguraion.WebHostConfiguration.HttpPort = int.Parse(HTTPPortTextBox.Text);

        }

        private void HTTPSPortTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InstallationConfiguraion.WebHostConfiguration.HttpsPort = int.Parse(HTTPSPortTextBox.Text);

        }
    }
}
