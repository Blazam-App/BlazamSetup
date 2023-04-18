using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for InstallDirectory.xaml
    /// </summary>
    public partial class InstallDirectory : System.Windows.Controls.UserControl,IInstallationStep
    {
        public InstallDirectory()
        {
            InitializeComponent();
            CurrentDispatcher = Dispatcher;
            if(InstallationConfiguraion.InstallationType == InstallType.Service)
            {
                InstallationConfiguraion.InstallDirPath = "C:\\Program Files\\";

            }
            else
            {
                if (Directory.Exists("C:\\inetpub\\"))
                {
                    InstallationConfiguraion.InstallDirPath = "C:\\inetpub\\";

                }
                else
                {
                    InstallationConfiguraion.InstallDirPath = "C:\\";

                }
            }
            directoryTextBox.Text= InstallationConfiguraion.InstallDirPath;
        }

        public Dispatcher CurrentDispatcher { get; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderPicker() { InputPath = InstallationConfiguraion.InstallDirPath } ;

            if (dialog.ShowDialog() == true)
            {
                InstallationConfiguraion.InstallDirPath = dialog.ResultPath;
                CurrentDispatcher.Invoke(() => {
                    directoryTextBox.Text = InstallationConfiguraion.InstallDirPath;
                });
            }
            
        }

        IInstallationStep IInstallationStep.NextStep()
        {
            return new DatabaseType();
        }
    }
}
