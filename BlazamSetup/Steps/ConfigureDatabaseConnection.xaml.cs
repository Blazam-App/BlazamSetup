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
    /// Interaction logic for ConfigureDatabaseConnection.xaml
    /// </summary>
    public partial class ConfigureDatabaseConnection : UserControl,IInstallationStep
    {
        public ConfigureDatabaseConnection()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int port = 0;
            int.TryParse(PortTextBox.Text, out port);
            var settings = new DatabaseConfiguration()
            {
                Database = DatabaseTextBox.Text,
                Server = ServerTextBox.Text,
                Port = port,
                Username = UsernameTextBox.Text,
                Password = PasswordTextBox.Text,
            };

            if (settings.IsValid)
            {

            }
            else
            {
                MessageBox.Show(settings.ValidationMessage);
            }
        }

        IInstallationStep IInstallationStep.NextStep()
        {
            throw new NotImplementedException();
        }
    }
}
