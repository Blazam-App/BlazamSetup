using BlazamSetup.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BlazamSetup
{
    public enum InstallType { IIS, Service }
    public enum DBType { Sqlite,SQL,MySQL}
    internal static class InstallationConfiguraion
    {
        public static DBType? DatabaseType { get; set; } = null;
        public static InstallType? InstallationType { get; set; } = null;
        public static X509Certificate2 SSLCert { get; internal set; }
        public static string InstallDirPath { get; internal set; }
    }
}
