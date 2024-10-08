﻿using Octokit;
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
        public string Comments { get; set; } = " A web based Active Directry management portal";
        public int NoRepair { get; set; } = 0;
        public int NoModify { get; set; } = 1;
        public string Contact { get; set; } = "Blazam Support";
        public string HelpLink { get; set; } = "https://blazam.org";
        public int Language { get; set; } = 1033;
        public string Size { get; set; }
        public string UninstallString { get; set; }= '"' + Path.GetFullPath(Path.GetFullPath(InstallationConfiguraion.InstallDirPath + Path.DirectorySeparatorChar + "Blazam") + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar + "setup.exe")+"\" /u"  ;
        public string InstallLocation { get; set; }= Path.GetFullPath(InstallationConfiguraion.InstallDirPath+Path.DirectorySeparatorChar + "Blazam");
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