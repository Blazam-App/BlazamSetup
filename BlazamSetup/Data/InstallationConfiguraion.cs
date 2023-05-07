using BlazamSetup.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazamSetup
{
    public enum InstallType { IIS, Service }
    public enum DBType { Sqlite, SQL, MySQL }
    internal static class InstallationConfiguraion
    {
        /// <summary>
        /// This value identifies the application, it should never change
        /// </summary>
        internal static string ProductGuid => "44f8501a-c549-4b27-8216-48480c65bc31";
        /// <summary>
        /// This value identifies the version of the installer that was used, it should change every update.
        /// </summary>
        internal static string UpdateGuid => "a146351b-8ff5-457e-9aac-c6604a21bd1b";

        internal static ProductInformation ProductInformation { get; set; } = new ProductInformation();




        internal static WebHostConfiguration WebHostConfiguration = new WebHostConfiguration();

        internal static DBType? DatabaseType { get; set; } = null;
        internal static InstallType? InstallationType { get; set; } = null;
        /// <summary>
        /// The path to install to, or already installed at.
        /// </summary>
        internal static string InstallDirPath { get;  set; }
        internal static DatabaseConfiguration DatabaseConfiguration { get;  set; } = new DatabaseConfiguration();
    }
}
