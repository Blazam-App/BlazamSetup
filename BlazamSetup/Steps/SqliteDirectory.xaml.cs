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
using System.Windows.Threading;

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for SqliteDirectory.xaml
    /// </summary>
    public partial class SqliteDirectory : UserControl,IInstallationStep
    {
        public SqliteDirectory()
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
                InstallationConfiguraion.InstallDirPath = dialog.ResultPath;
                CurrentDispatcher.Invoke(() => {
                    directoryTextBox.Text = InstallationConfiguraion.InstallDirPath;
                });
            }
        }

        IInstallationStep IInstallationStep.NextStep()
        {
            return new ConfirmSettings();

        }
    }
}
