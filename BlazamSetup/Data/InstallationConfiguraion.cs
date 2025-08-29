using System;
using System.IO;

namespace BlazamSetup
{
    public enum InstallType { IIS, Service }
    public enum InstalledAction { Update, Repair, Remove }
    public enum DBType { Sqlite, SQL, MySQL }
    public static class InstallationConfiguraion
    {
        /// <summary>
        /// This value identifies the application, it should never change
        /// </summary>
        internal static string ProductGuid => "44f8501a-c549-4b27-8216-48480c65bc31".ToUpper();
        /// <summary>
        /// This value identifies the version of the installer that was used, it should change every update.
        /// </summary>
        internal static string UpdateGuid => "410dd7f9-001d-4916-a991-5ca86987b709".ToUpper();

        internal static ProductInformation ProductInformation { get; set; } = new ProductInformation();




        public static WebHostConfiguration WebHostConfiguration { get; } = new WebHostConfiguration();
        private static string installDirPath;

        internal static DBType? DatabaseType { get; set; } = null;
        internal static InstallType? InstallationType { get; set; } = null;
        /// <summary>
        /// The path to install to, or already installed at.
        /// </summary>
        public static string InstallDirPath
        {
            get => installDirPath; set
            {
                installDirPath = Path.GetFullPath(value + Path.DirectorySeparatorChar);
                ProductInformation.InstallLocation = Path.GetFullPath(value + Path.DirectorySeparatorChar);
                ProductInformation.UninstallString = '"' + Path.GetFullPath(value + Path.DirectorySeparatorChar + "setup.exe") + "\" /u";
            }
        }
        internal static DatabaseConfiguration DatabaseConfiguration { get; set; } = new DatabaseConfiguration();

        public static string ProgramDataDir => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + Path.DirectorySeparatorChar + "Blazam";

        /// <summary>
        /// The path %temp%\BlazamSetup\
        /// </summary>
        public static string SetupTempDirectory => Path.GetTempPath() + "BlazamSetup" + Path.DirectorySeparatorChar;

        public static InstalledAction InstalledAction { get; internal set; }
        public static string InstalledVersion { get; internal set; }
        public static bool ExecutableExists => File.Exists(ProductInformation.InstallLocation + Path.DirectorySeparatorChar + "Blazam.exe");
    }
}
