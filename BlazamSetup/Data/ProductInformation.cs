using Octokit;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace BlazamSetup
{
    /// <summary>
    /// Information to populate registry for control
    /// panel uninstall functionality
    /// </summary>
    internal class ProductInformation
    {
        public string DisplayName { get; set; } = "Blazam";
        public string Publisher { get; set; } = "blazam.org";
        public string DisplayVersion { get; set; } = "1.0";
        public string Comments { get; set; } = " A Wweb base Active Directry management portal";
        public int NoRepair { get; set; } = 1;
        public int NoModify { get; set; } = 1;
        public string Contact { get; set; } = "Blazam Support";
        public string HelpLink { get; set; } = "https://blazam.org";
        public int Language { get; set; } = 1033;
        public string Size { get; set; }
        public string UninstallString => '"'+Path.GetFullPath(InstallLocation + "\\setup.exe /u"+'"');
        public string InstallLocation => InstallationConfiguraion.InstallDirPath;
        public long InstallDate { get; set; } = DateTime.UtcNow.ToFileTimeUtc();
        public long Version { get; set; }
        public int VersionMajor = 1;
        public int VersionMinor= 0;

        public ProductInformation()
        {
            Version = InstallationConfiguraion.UpdateGuid.ToString().GetAppHashCode();
        }
    }
}