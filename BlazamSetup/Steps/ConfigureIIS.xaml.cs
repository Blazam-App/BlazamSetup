using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
    /// Interaction logic for ConfigureIIS.xaml
    /// </summary>
    public partial class ConfigureIIS : UserControl,IInstallationStep
    {
        public ConfigureIIS()
        {
            InitializeComponent();
        }


        IInstallationStep IInstallationStep.NextStep()
        {
            return new InstallDirectory();
        }

        private void ChooseCertificateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var store = new X509Store("MY", System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                var collection = store.Certificates;
                var certs = X509Certificate2UI.SelectFromCollection(collection, "Select", "Select a certificate to sign", X509SelectionFlag.SingleSelection);
                if (certs.Count > 0)
                {
                    InstallationConfiguraion.WebHostConfiguration.SSLCert = certs[0];
                    HTTPSHelpLabel.Visibility=Visibility.Hidden;
                    HTTPSPortTextBox.IsEnabled=true;
                    sslCertLabel.Content = InstallationConfiguraion.WebHostConfiguration.SSLCert.FriendlyName;
                    
                }
            }
            catch (Exception ex)
            {
                sslCertLabel.Content = ex.Message;
            }
        }
    }
}
