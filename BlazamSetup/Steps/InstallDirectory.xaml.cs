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
using System.Windows.Threading;

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for InstallDirectory.xaml
    /// </summary>
    public partial class InstallDirectory : System.Windows.Controls.UserControl, IInstallationStep
    {
        public InstallDirectory()
        {
            InitializeComponent();
            CurrentDispatcher = Dispatcher;
            if (InstallationConfiguraion.InstallDirPath.IsNullOrEmpty())
            {
                if (InstallationConfiguraion.InstallationType != InstallType.Service)
                {
                    if (Directory.Exists("C:\\inetpub\\"))
                        InstallationConfiguraion.InstallDirPath = "C:\\inetpub\\";



                }
                else
                {
                    if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)))
                        InstallationConfiguraion.InstallDirPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
                }

            }
            directoryTextBox.Text = InstallationConfiguraion.InstallDirPath;
        }

        public Dispatcher CurrentDispatcher { get; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderPicker() { InputPath = InstallationConfiguraion.InstallDirPath };

            if (dialog.ShowDialog() == true)
            {
                InstallationConfiguraion.InstallDirPath = dialog.ResultPath;
                CurrentDispatcher.Invoke(() =>
                {
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
