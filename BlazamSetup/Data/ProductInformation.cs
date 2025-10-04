using System;
using System.IO;

namespace BlazamSetup
{
    /// <summary>
    /// Information to populate registry for control
    /// panel uninstall functionality
    /// </summary>
    internal class ProductInformation
    {
        private static string SetupPath => Path.GetFullPath(Path.GetFullPath(InstallationConfiguraion.InstallDirPath + Path.DirectorySeparatorChar + "Blazam") + Path.DirectorySeparatorChar + "setup" + Path.DirectorySeparatorChar + "setup.exe");

        public string DisplayName { get; set; } = "Blazam";
        public string DisplayIcon { get; set; } = SetupPath + ",0";
        public string Publisher { get; set; } = "Jacobsen Productions LLC";
        public string DisplayVersion { get; set; } = "1.0";
        public string Comments { get; set; } = " A web based Active Directory management portal";
        public int NoRepair { get; set; } = 0;
        public int NoModify { get; set; } = 1;
        public string Contact { get; set; } = "Blazam Support";
        public string HelpLink { get; set; } = "https://blazam.org";
        public int Language { get; set; } = 1033;
        public string Size { get; set; }
        public string UninstallString { get; set; } = '"' + SetupPath + "\" /u";
        public string InstallLocation { get; set; } = Path.GetFullPath(InstallationConfiguraion.InstallDirPath + Path.DirectorySeparatorChar + "Blazam");
        public string InstallDate { get; set; } = DateTime.UtcNow.Year + DateTime.UtcNow.Month.ToString("00") + DateTime.UtcNow.Day.ToString("00");
        public long Version { get; set; }
        public int EstimatedSize { get; set; }
        public int VersionMajor { get; set; } = 0;
        public int VersionMinor { get; set; } = 8;

        public ProductInformation()
        {
            Version = InstallationConfiguraion.UpdateGuid.ToString().GetAppHashCode();
        }
    }
}