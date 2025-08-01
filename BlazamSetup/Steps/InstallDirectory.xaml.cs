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
                        InstallationConfiguraion.InstallDirPath = Path.GetFullPath("C:\\inetpub\\Blazam");



                }
                else
                {
                    if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)))
                        InstallationConfiguraion.InstallDirPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)+"\\Blazam");
                }

            }
            directoryTextBox.Text = InstallationConfiguraion.InstallDirPath;
        }

        public Dispatcher CurrentDispatcher { get; }

        public int Order => 6;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var path="";
            if (Directory.Exists(directoryTextBox.Text)){
                path = directoryTextBox.Text;
            }
            var dialog = new FolderPicker() { InputPath = path };

            if (dialog.ShowDialog() == true)
            {
                InstallationConfiguraion.InstallDirPath = Path.GetFullPath(dialog.ResultPath);
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
