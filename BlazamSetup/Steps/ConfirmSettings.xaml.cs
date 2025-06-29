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
    /// Interaction logic for ConfirmSettings.xaml
    /// </summary>
    public partial class ConfirmSettings : UserControl, IInstallationStep
    {
        public ConfirmSettings()
        {
            InitializeComponent();
            CurrentDispatcher = Dispatcher;
            InstallationTypeLabel.Content = InstallationConfiguraion.InstallationType.ToString();
            InstallationPathLabel.Content = InstallationConfiguraion.InstallDirPath;
            DatabaseTypeLabel.Content =  InstallationConfiguraion.DatabaseType.ToString();
            DatabaseServerLabel.Content = InstallationConfiguraion.DatabaseType==DBType.Sqlite?InstallationConfiguraion.DatabaseConfiguration.SqliteDirectory : InstallationConfiguraion.DatabaseConfiguration.Server;
            DatabaseNameLabel.Content = InstallationConfiguraion.DatabaseConfiguration.Database;
            MainWindow.NextStepButton.IsEnabled = true;

        }

        public Dispatcher CurrentDispatcher { get; }

        public int Order => 9;

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            if (oldParent == null)
                CurrentDispatcher.Invoke(() =>
                {
                    if (InstallationConfiguraion.DatabaseType == DBType.Sqlite)
                    {
                        DatabasePathLabel.Content = "Database Path";
                        DatabaseDBLabel.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        DatabasePathLabel.Content = "Database Server";
                        DatabaseDBLabel.Visibility = Visibility.Visible;

                    }
                    MainWindow.SetNextText("Install");
                    MainWindow.EnableNext();
                });

        }
        IInstallationStep IInstallationStep.NextStep()
        {
            return new Install();
        }
    }
  
}
