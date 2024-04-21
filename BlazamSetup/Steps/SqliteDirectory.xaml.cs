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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for SqliteDirectory.xaml
    /// </summary>
    public partial class SQLiteDirectory : UserControl,IInstallationStep
    {
        public SQLiteDirectory()
        {
            InitializeComponent();
            directoryTextBox.Text = InstallationConfiguraion.DatabaseConfiguration.SqliteDirectory;
            CurrentDispatcher = Dispatcher;
        }

        public Dispatcher CurrentDispatcher { get; }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderPicker() { InputPath = InstallationConfiguraion.DatabaseConfiguration.SqliteDirectory };

            if (dialog.ShowDialog() == true)
            {
                InstallationConfiguraion.DatabaseConfiguration.SqliteDirectory = Path.GetFullPath(dialog.ResultPath);
                CurrentDispatcher.Invoke(() => {
                    directoryTextBox.Text = InstallationConfiguraion.DatabaseConfiguration.SqliteDirectory;
                });
            }
        }

        IInstallationStep IInstallationStep.NextStep()
        {
            InstallationConfiguraion.DatabaseConfiguration.SqliteDirectory = Path.GetFullPath(directoryTextBox.Text);
            return new ConfirmSettings();

        }

       
    }
}
