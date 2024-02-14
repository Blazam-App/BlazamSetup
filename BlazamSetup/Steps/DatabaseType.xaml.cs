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
    /// Interaction logic for DatabaseType.xaml
    /// </summary>
    public partial class DatabaseType : UserControl,IInstallationStep
    {
        public DatabaseType()
        {
            InitializeComponent();

            switch (InstallationConfiguraion.DatabaseType)
            {
                case DBType.Sqlite:
                    SqliteRadioButton.IsChecked = true; break;
                case DBType.SQL:
                    SqlRadioButton.IsChecked = true; break;
                case DBType.MySQL:
                    MysqlRadioButton.IsChecked = true; break;

                case null:
                    SqliteRadioButton.IsChecked = true; break;

            }
        }

        private void Sqlite_Checked(object sender, RoutedEventArgs e)
        {
            InstallationConfiguraion.DatabaseType =DBType.Sqlite;
        }

        private void SQL_Checked(object sender, RoutedEventArgs e)
        {
            InstallationConfiguraion.DatabaseType = DBType.SQL;

        }

        private void Mysql_Checked(object sender, RoutedEventArgs e)
        {
            InstallationConfiguraion.DatabaseType = DBType.MySQL;

        }

        IInstallationStep IInstallationStep.NextStep()
        {
            switch (InstallationConfiguraion.DatabaseType)
            {
                case DBType.SQL:
                case DBType.MySQL:
                    return new ConfigureDatabaseConnection();
                case DBType.Sqlite:
                    return new SQLiteDirectory();
            }
            return this;
        }
    }
}
