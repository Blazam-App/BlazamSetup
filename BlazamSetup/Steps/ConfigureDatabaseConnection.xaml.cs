using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MySql.Data.MySqlClient;

namespace BlazamSetup.Steps
{
    /// <summary>
    /// Interaction logic for ConfigureDatabaseConnection.xaml
    /// </summary>
    public partial class ConfigureDatabaseConnection : UserControl, IInstallationStep
    {
        public Dispatcher CurrentDispatcher { get; }
        public bool TestPassed { get; private set; }

        public int Order => 8;

        public ConfigureDatabaseConnection()
        {
            InitializeComponent();
            MainWindow.NextStepButton.IsEnabled = false;
            CurrentDispatcher = Dispatcher;
            ServerTextBox.Text = InstallationConfiguraion.DatabaseConfiguration.Server;
            PortTextBox.Text = InstallationConfiguraion.DatabaseConfiguration.Port.ToString();
            DatabaseTextBox.Text = InstallationConfiguraion.DatabaseConfiguration.Database;
            UsernameTextBox.Text = InstallationConfiguraion.DatabaseConfiguration.Username;
            PasswordTextBox.Password = InstallationConfiguraion.DatabaseConfiguration.Password;
        }


        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            if (oldParent == null)
                CurrentDispatcher.Invoke(() =>
                {
                    MainWindow.NextStepButton.IsEnabled = TestPassed;
                });

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (InstallationConfiguraion.DatabaseConfiguration.IsValid)
            {
                PerformTest();
            }
            else
            {
                MessageBox.Show(InstallationConfiguraion.DatabaseConfiguration.ValidationMessage);
            }
        }
        private void UpdateStatus(string status)
        {
            CurrentDispatcher.Invoke(() =>
            {
                StatusLabel.Text = status;
            });
        }
        private void PerformTest()
        {
            string connString = "";
            switch (InstallationConfiguraion.DatabaseType)
            {
                case DBType.SQL:
                    connString = $"Server={InstallationConfiguraion.DatabaseConfiguration.Server},{InstallationConfiguraion.DatabaseConfiguration.Port};Database={InstallationConfiguraion.DatabaseConfiguration.Database};User ID={InstallationConfiguraion.DatabaseConfiguration.Username};Password={InstallationConfiguraion.DatabaseConfiguration.Password};Connection Timeout=10;";

                    break;
                case DBType.MySQL:
                    connString = $"Server={InstallationConfiguraion.DatabaseConfiguration.Server},{InstallationConfiguraion.DatabaseConfiguration.Port};Database={InstallationConfiguraion.DatabaseConfiguration.Database};User={InstallationConfiguraion.DatabaseConfiguration.Username};Password={InstallationConfiguraion.DatabaseConfiguration.Password};Connection Timeout=10;";

                    break;

            }
            UpdateStatus("Testing connection please wait...");
            try
            {
                if (InstallationConfiguraion.DatabaseType == DBType.MySQL)
                {

                    MySqlConnection testConn = new MySqlConnection(connString);
                    testConn.Open();
                    if (testConn.State == System.Data.ConnectionState.Open)
                    {
                        UpdateStatus("Test Successful \r\n"
                            + "MySQL Version: " + testConn.ServerVersion);
                    }
                    testConn.Close();

                }
                else
                {
                    SqlConnection testConn = new SqlConnection(connString);
                    testConn.Open();
                    if (testConn.State == System.Data.ConnectionState.Open)
                    {
                        UpdateStatus("Test Successful \r\n"
                            + "SQL Version: " + testConn.ServerVersion);
                    }
                    testConn.Close();
                }
                TestSucceeded();
            }
            catch (Exception ex)
            {
                UpdateStatus(ex.Message);

                TestFailed();
            }
        }

        private void TestSucceeded()
        {
            CurrentDispatcher.Invoke(() =>
            {
                TestPassed = true;
                MainWindow.NextStepButton.IsEnabled = true;

                MainWindow.EnableNext();
            });
        }

        private void TestFailed()
        {
            CurrentDispatcher.Invoke(() =>
            {
                TestPassed = false;

                MainWindow.NextStepButton.IsEnabled = TestPassed;
            });
        }

        IInstallationStep IInstallationStep.NextStep()
        {
            return new ConfirmSettings();
        }

        private void ServerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InstallationConfiguraion.DatabaseConfiguration.Server = ServerTextBox.Text;
            TestFailed();

        }

        private void PortTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                InstallationConfiguraion.DatabaseConfiguration.Port = int.Parse(PortTextBox.Text);
            }
            catch
            {
                //ignore
            }
            TestFailed();

        }

        private void DatabaseTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InstallationConfiguraion.DatabaseConfiguration.Database = DatabaseTextBox.Text;
            TestFailed();

        }

        private void PasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!PasswordTextBox.Password.IsNullOrEmpty())
                InstallationConfiguraion.DatabaseConfiguration.Password = PasswordTextBox.Password;

            TestFailed();
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InstallationConfiguraion.DatabaseConfiguration.Username = UsernameTextBox.Text;
            TestFailed();

        }
    }
}
